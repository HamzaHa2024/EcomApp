using AutoMapper;
using OrderCloud.SDK;

namespace SellerApp.Interfaces
{
    public interface IPriceScheduleService
    {
        Task CreateOrUpdateFromCSV(IOrderCloudClient orderCloudClient, IMapper mapper, IFormFile file);
        Task CreateOrUpdateAsync(IOrderCloudClient orderCloudClient, PriceSchedule catalog, IMapper mapper);
    }
}

