using AutoMapper;
using BuyerApp.Interfaces;
using OrderCloud.SDK;

namespace BuyerApp.Services
{
    public class OrderService : IOrderService
    {
        public async Task<Order> CreateOrUpdateAsync(IOrderCloudClient orderCloudClient, Order order, IMapper mapper)
        {
            try
            {
                // Validate the order
               

                // Create the order
              return  await orderCloudClient.Orders.CreateAsync(OrderDirection.Outgoing, order);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool Validate(LineItem lineItem)
        {
            throw new NotImplementedException();
        }
    }
}
