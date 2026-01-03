using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Core;


namespace BusinessLogicLayerCore.Services.Interfaces

{
    public interface IPayPalSerivce{
        Task<Order> CreateOrder(decimal amount, string currency = "USD");
    }
}
