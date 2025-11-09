
using System.ClientModel;
using Azure.AI.OpenAI;


using BusinessLogicLayerCore.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;



namespace BusinessLogicLayerCore.Services
{
    public class AgentService{
        private readonly String? _deploymentName;
        private readonly String? _apiKey;
        private readonly String? _endpoint;
        private readonly AzureOpenAIClient _client;
        private readonly ILogger<AgentService> _logger;

        public AgentService(IConfiguration config, ILogger<AgentService> _logger){
            this._endpoint = config["AzureOpenAI:Endpoint"];
            this._apiKey = config["AzureOpenAI:ApiKey"];
            this._deploymentName = config["AzureOpenAI:ApiKey"];
            this._client = new AzureOpenAIClient(new Uri(_endpoint), new ApiKeyCredential(_apiKey));
            this._logger = _logger;
        }

        
        public async Task<string> GenerateTaskAsync(string userPromnt){
            // Validate config 
            ValidateConfig();

            //AIAgent agentService = _client.GetChatClient(_deploymentName);

            // Promnt to AI
            string message = HelperLayer.AIAgent.SystemPromnt.GenerateTaskPromnt();

            // Generate stuff
            // var answer = await _client.RunAsync

            return "abobus";
        }

        


        private void ValidateConfig(){
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(_endpoint))
                errors.Add("Error -> AzureOpenAI:Endpoint is required in configuration");

            if (string.IsNullOrWhiteSpace(_apiKey))
                errors.Add("Error -> AzureOpenAI:ApiKey is required in configuration");

            if (string.IsNullOrWhiteSpace(_deploymentName))
                errors.Add("Error -> AzureOpenAI:DeploymentName is required in configuration");

            if (!string.IsNullOrWhiteSpace(_endpoint) && !Uri.IsWellFormedUriString(_endpoint, UriKind.Absolute))
                errors.Add("Error -> AzureOpenAI:Endpoint must be a valid URL");

            if (!string.IsNullOrWhiteSpace(_apiKey) && 
                (_apiKey.Contains("your-") || _apiKey.Length < 10))
                errors.Add("Error -> AzureOpenAI:ApiKey appears to be invalid or a placeholder");

            if (errors.Any()){
                var errorMessage = string.Join("; ", errors);
                _logger.LogError("Erorr -> Configuration validation failed: {Errors}", errorMessage);
                throw new InvalidOperationException($"Configuration validation failed: {errorMessage}");
            }

            _logger.LogInformation("Azure OpenAI configuration validated successfully (this shit is working? :0 )");
        }
        
    }
}
