using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using SellerApp.Interfaces;

namespace SellerApp.Controllers;

[Route("[controller]")]
[ApiController]

public class PriceSchedulesController(IOrderCloudClient orderCloudClient, ILogger<PriceSchedulesController> logger, IMapper mapper, IPriceScheduleService priceScheduleService) : ControllerBase
{

    private readonly IOrderCloudClient _orderCloudClient = orderCloudClient;
    private readonly ILogger<PriceSchedulesController> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly IPriceScheduleService _priceScheduleService = priceScheduleService;


    [HttpPost]
    public async Task<ActionResult<PriceSchedule>> CreatePriceSchedule(PriceSchedule priceSchedule)
    {
        //check if priceSchedule is valid
        //check if priceSchedule is existing

        var newPriceSchedule = await _orderCloudClient.PriceSchedules.CreateAsync(priceSchedule);

        return Ok(newPriceSchedule);
    }


    // file contains Price schedule Create new update existing,Assignemnt to the product and buyer
    // 
    [HttpPost("Upload")]
    public async Task<IActionResult> SavePriceScheduleAndAssigmentIt(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file selected for upload.");
        }

        try
        {
            await _priceScheduleService.CreateOrUpdateFromCSV(_orderCloudClient, mapper, file);

            return Ok("Product items uploaded successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while uploading the ProductPriceSchedule: {ex.Message}");
        }
    }


}
