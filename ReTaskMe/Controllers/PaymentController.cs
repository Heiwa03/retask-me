using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BusinessLogicLayerCore.Services.Interfaces;
using ReTaskMe.Models.Responses;

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
    [HttpPost("MakeTransfer")] // TODO: Clean arhitecture
    public async Task<IActionResult> MakeTransfer(){ // TODO: Middleware
        var order = await _paypal.CreateOrder(10.00m, "USD"); 
        Console.WriteLine($"Order: {order.Links}");

        var approveLink = order.Links.FirstOrDefault(x => x.Rel == "approve")?.Href;
        Console.WriteLine($"link: {approveLink}");

        return Ok(new OrderResponse { 
            orderId = order.Id,
            approveLink = approveLink
        });
    }

    [Authorize]
    [HttpPost("CaptureTransfer")] // TODO: Clean arhitecture
    public async Task<IActionResult> CaptureTransfer([FromBody] string orderid){ // TODO: Middleware
        var capture = await _paypal.CaptureOrder(orderid);
        Console.WriteLine($"Capture: {capture}");

        if (capture.Status == "COMPLETED")
        {
            return Ok(capture);
        }

        return BadRequest(capture);
    }

}