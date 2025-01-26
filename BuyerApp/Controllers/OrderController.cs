using AutoMapper;
using BuyerApp.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;

namespace BuyerApp.Controllers;

[Route("[controller]")]
[ApiController]

public class OrderController(IOrderCloudClient orderCloudClient, ILogger<OrderController> logger, IMapper mapper, IOrderService orderService) : ControllerBase
{

    private readonly IOrderCloudClient _orderCloudClient = orderCloudClient;
    private readonly ILogger<OrderController> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly IOrderService _orderService = orderService;


    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        try
        {
            return Ok(await _orderService.CreateOrUpdateAsync(_orderCloudClient, order, _mapper));
        }
        catch (Exception)
        {
            // Log the error
            return StatusCode(500, "An error occurred while creating the order.");
        }
    }

    [HttpPost("{orderId}/lines")]
    public async Task<IActionResult> CreateOrderLine(string orderId, [FromBody] LineItem lineItem)
    {
        try
        {
            // Validate the incoming line item data
            if (!_orderService.Validate(lineItem))
            {
                return BadRequest("Invalid line item data.");
            }

            // Create the line item in OrderCloud
            var createdLineItem = await _orderCloudClient.LineItems.SaveAsync(OrderDirection.Outgoing, orderId, lineItem.ID, lineItem);
            return Ok(createdLineItem);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while creating the order line.");
        }
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportOrders()
    {
        try
        {

            // Create a CSV file stream
            var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
            {
                var orders = await _orderCloudClient.Orders.ListAsync(OrderDirection.Outgoing);

                if (orders == null || !orders.Items.Any())
                {
                    return NotFound("No orders found.");
                }

                // Write order header
                csvWriter.WriteHeader<Order>();
                csvWriter.NextRecord();

                // Write order data
                foreach (var order in orders.Items)
                {
                    csvWriter.WriteRecord(order);
                    csvWriter.NextRecord();

                    // Write line items for each order
                    var lineItems = await _orderCloudClient.LineItems.ListAsync(OrderDirection.Outgoing, order.ID);

                    if (lineItems == null || !lineItems.Items.Any())
                    {
                        continue;
                    }
                    foreach (var lineItem in lineItems.Items)
                    {
                        csvWriter.WriteRecord(lineItem);
                        csvWriter.NextRecord();
                    }
                }
            }

            // Set response headers for CSV download
            memoryStream.Position = 0;
            return File(memoryStream, "text/csv", "orders.csv");
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while exporting orders.");
        }
    }
}
