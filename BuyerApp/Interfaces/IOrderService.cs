using AutoMapper;
using OrderCloud.SDK;

namespace BuyerApp.Interfaces
{
    public interface IOrderService
    {
        public Task<Order> CreateOrUpdateAsync(IOrderCloudClient orderCloudClient, Order order, IMapper mapper);
        bool Validate(LineItem lineItem);
    }
}
