using AutoMapper;
using OrderCloud.SDK;

namespace SellerApp.Interfaces
{
    public interface ICatalogService
    {
        List<Dictionary<string, string>> ConvertCsvToJson(string csvFilePath);
        Task CreateOrUpdate(IOrderCloudClient orderCloudClient, IMapper mapper, IFormFile file);
        Task CreateOrUpdateAsync(IOrderCloudClient orderCloudClient, Catalog catalog, IMapper mapper);
    }
}

