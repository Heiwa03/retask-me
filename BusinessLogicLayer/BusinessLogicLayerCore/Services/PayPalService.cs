
using Microsoft.Extensions.Configuration;


namespace BusinessLogicLayerCore.Services;

public class PayPalService{
    private readonly string? _client;
    private readonly string? _secret;
    private readonly string? _url;

    public PayPalService(IConfiguration config, string _client, string _secret, string _url){
        this._client = config["PayPal:Client"];
        this._secret = config["PayPal:Secret"];
        this._url = config["PayPal:Url"];
    }

    // yes, it's empty for now;(



    
}