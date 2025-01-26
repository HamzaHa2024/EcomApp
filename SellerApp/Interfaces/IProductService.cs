using AutoMapper;
using OrderCloud.SDK;

namespace SellerApp.Interfaces;

public interface IProductService
{
    Task CreateOrUpdateAsync(IOrderCloudClient orderCloudClient, Product product, IMapper mapper);
    public List<Product> GetProductsFromCSV(string csvFilePath);
}

