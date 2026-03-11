using PayPalCheckoutSdk.Orders;


namespace BusinessLogicLayerCore.Services.Interfaces;

public interface IPayPalSerivce{
    Task<Order> CreateOrder(decimal amount, string currency);
    Task<Order> CaptureOrder(string orderId);
    Task<Order> GetOrder(string orderId);
}

