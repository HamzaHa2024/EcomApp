using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using OrderCloud.SDK;
using SellerApp.Interfaces;
using SellerApp.Options;
using System.Globalization;

namespace SellerApp.Services;

public class PriceScheduleService : IPriceScheduleService
{
    public async Task CreateOrUpdateFromCSV(IOrderCloudClient orderCloudClient, IMapper mapper, IFormFile file)
    {
        var stream = file.OpenReadStream();
        var reader = new StreamReader(stream);
        var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
        var PriceScheduleItems = csvReader.GetRecords<PriceScheduleItemCsvModel>().ToList();

        foreach (var item in PriceScheduleItems)
        {
            var partialPriceSchedule = mapper.Map<PriceSchedule>(item);
            await CreateOrUpdateAsync(orderCloudClient, partialPriceSchedule, mapper);
        }

        //Link Price Schedule to Product
        foreach (var priceSchedule in PriceScheduleItems)
        {
            //Add Price Schedule to Product
            try
            {
                var existingAssignments = await orderCloudClient.Products.ListAssignmentsAsync(priceSchedule.ProductID, priceSchedule.ID);

                // Check if the product is already assigned.
                var isProductAssigned = existingAssignments.Items.Any(a => a.ProductID == priceSchedule.ProductID
                                                                      && a.BuyerID == priceSchedule.BuyerID
                                                                      && a.PriceScheduleID == priceSchedule.ID);

                if (!isProductAssigned)
                {
                    // Create a new product assignment.
                    var newAssignment = new ProductAssignment
                    {
                        ProductID = priceSchedule.ProductID,
                        BuyerID = priceSchedule.BuyerID,
                        PriceScheduleID = priceSchedule.ID
                    };

                    // Add the assignment to the buyer.
                    await orderCloudClient.Products.SaveAssignmentAsync(newAssignment);
                }
            }
            catch (OrderCloudException ex)
            {
                // Handle OrderCloud exceptions.
                Console.WriteLine($"An error occurred while assigning the product: {ex.Message}");
                // Log the exception or handle it appropriately.
            }
            catch (Exception ex)
            {
                // Handle other exceptions.
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                // Log the exception or handle it appropriately.
            }
        }
    }


    public async Task CreateOrUpdateAsync(IOrderCloudClient orderCloudClient, PriceSchedule PriceSchedule, IMapper mapper)
    {
        var existingPriceSchedule = await GetExistingPriceScheduleAsync(orderCloudClient, PriceSchedule);

        if (existingPriceSchedule != null)
        {
            var partialPriceSchedule = mapper.Map<PartialPriceSchedule>(existingPriceSchedule);
            await orderCloudClient.PriceSchedules.PatchAsync(existingPriceSchedule.ID, partialPriceSchedule);
        }

        await CreatePriceScheduleAsync(orderCloudClient, PriceSchedule);
    }

    private static async Task CreatePriceScheduleAsync(IOrderCloudClient orderCloudClient, PriceSchedule PriceSchedule)
    {
        try
        {
            await orderCloudClient.PriceSchedules.CreateAsync(PriceSchedule);

        }
        catch (Exception)
        {
            throw;
        }
    }

    private static async Task<PriceSchedule?> GetExistingPriceScheduleAsync(IOrderCloudClient orderCloudClient, PriceSchedule PriceSchedule)
    {
        try
        {
            return await orderCloudClient.PriceSchedules.GetAsync(PriceSchedule.ID);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
