using AutoMapper;
using OrderCloud.SDK;
using SellerApp.Interfaces;

namespace SellerApp.Services;

public class ProductService : IProductService
{
    public async Task CreateOrUpdateAsync(IOrderCloudClient orderCloudClient, Product product, IMapper mapper)
    {
        //check product if valid 
        //Find the best logic (check Owner, PriceScheduleID...)
        product.QuantityMultiplier = product.QuantityMultiplier < 1 ? 1 : product.QuantityMultiplier;
        
        var existingProduct = await GetExistingProductAsync(orderCloudClient, product);

        if (existingProduct != null)
        {
            await UpdateExistingProduct(orderCloudClient, mapper, existingProduct);
        }

        await CreateNewProduct(orderCloudClient, product);
    }

    private static async Task UpdateExistingProduct(IOrderCloudClient orderCloudClient, IMapper mapper, Product existingProduct)
    {
        var partialProduct = mapper.Map<PartialProduct>(existingProduct);
        await orderCloudClient.Products.PatchAsync(existingProduct.ID, partialProduct);
    }

    private static async Task CreateNewProduct(IOrderCloudClient orderCloudClient, Product Product)
    {
        try
        {
            await orderCloudClient.Products.CreateAsync(Product);

        }
        catch (Exception)
        {
            throw;
        }
    }

    private static async Task<Product?> GetExistingProductAsync(IOrderCloudClient orderCloudClient, Product Product)
    {
        try
        {
            var existingProduct = await orderCloudClient.Products.GetAsync(Product.ID);
            return existingProduct;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public List<Product> GetProductsFromCSV(string csvFilePath)
    {
        List<Product> products = [];
        using var reader = new StreamReader(csvFilePath);

        // Skip the header row
        string? headerLine = reader.ReadLine();

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] values = line.Split(',');


            // Create a record and parse it instead  of get values [i];

            Product product = new()
            {
                OwnerID = values[0],
                DefaultPriceScheduleID = values[1],
                AutoForward = bool.Parse(values[2]),
                ID = values[3],
                ParentID = values[4],
                IsParent = bool.Parse(values[5]),
                IsBundle = bool.Parse(values[6]),
                Name = values[7],
                Description = values[8],
                QuantityMultiplier = int.Parse(values[9]),
                ShipWeight = int.Parse(values[10]),
                ShipHeight = int.Parse(values[11]),
                ShipWidth = int.Parse(values[12]),
                ShipLength = int.Parse(values[13]),
                Active = bool.Parse(values[14]),
                SpecCount = int.Parse(values[15]),
                VariantCount = int.Parse(values[16]),
                ShipFromAddressID = values[17],
                Inventory = new Inventory
                {
                    Enabled = bool.Parse(values[18]),
                    NotificationPoint = int.Parse(values[19]),
                    VariantLevelTracking = bool.Parse(values[20]),
                    OrderCanExceed = bool.Parse(values[21]),
                    QuantityAvailable = int.Parse(values[22]),
                    LastUpdated = DateTime.Parse(values[23])
                },
                DefaultSupplierID = values[24],
                AllSuppliersCanSell = bool.Parse(values[25]),
                Returnable = bool.Parse(values[26]),
                xp = values[27]
            };

            
            products.Add(product);
        }

        return products;
    }
}
