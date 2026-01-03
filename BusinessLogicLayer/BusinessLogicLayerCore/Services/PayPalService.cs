
using BusinessLogicLayerCore.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System.Globalization;


namespace BusinessLogicLayerCore.Services;

public class PayPalService : IPayPalSerivce{
    private readonly PayPalHttpClient _client;
    private readonly string? _clientID;
    private readonly string? _secret;
    private readonly string? _url;
    private readonly string? _mode;

    public PayPalService(IConfiguration config){
        _clientID = config["PayPal:Client"];
        _secret = config["PayPal:Secret"];
        _url = config["PayPal:Url"];
        _mode = config["PayPal:Mode"];

        PayPalEnvironment environment = _mode == "Live"
            ? new LiveEnvironment(_clientID, _secret)
            : new SandboxEnvironment(_clientID, _secret);

        _client = new PayPalHttpClient(environment);
    }

    public async Task<Order> CreateOrder(decimal amount, string currency = "USD"){
        var orderRequest = new OrderRequest{
            CheckoutPaymentIntent = "CAPTURE",
            PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new()
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = currency,
                        Value = amount.ToString("F2", CultureInfo.InvariantCulture)
                    }
                }
            }
        };
        Console.WriteLine(orderRequest);

        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(orderRequest);

        Console.WriteLine(request);

        var response = await _client.Execute(request);
        return response.Result<Order>();
    }

    private bool CaptureOrder(){

        return true;
    }

    private void GetOrder(){
        
    }
}