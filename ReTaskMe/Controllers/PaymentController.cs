using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BusinessLogicLayerCore.Services.Interfaces;
using PayPalCheckoutSdk.Orders;

namespace ReTaskMe.Controllers;


[ApiController]
[Route("api/v1/[controller]")]
[Authorize]    
public class PaymentController : BaseController{

    private readonly IPayPalSerivce? _paypal;
    
    public PaymentController(IPayPalSerivce? _paypal){
        this._paypal = _paypal;
    }

    [Authorize]
    [HttpPost("registerProfile")] // TODO: Clean arhitecture
    public async Task<IActionResult> MakeTransfer(){ // TODO: Middleware

        var order = await _paypal.CreateOrder(10.00m);

        var approveLink = order.Links.FirstOrDefault(x => x.Rel == "approve")?.Href;

        return Ok(new { // TODO: OrderResponse
            orderId = order.Id,
            approveLink
        });
    }

}