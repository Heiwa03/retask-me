// System packages
using Microsoft.Extensions.Configuration;

// BL
using System.Text;
using System.Text.Json;
using BusinessLogicLayerCore.DTOs;
using BusinessLogicLayerCore.Services.Interfaces;
using System.Net.Http.Headers;



namespace BusinessLogicLayerCore.Services
{
    public class AgentService : IAgentService{
        private readonly string? _apiKey;
        private readonly string _modelId = "gemini‑2.5‑flash";
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ITaskService _taskService;

        public AgentService(IConfiguration _configuration, HttpClient _httpClient, ITaskService _taskService){
            this._apiKey = _configuration["AiAgent:ApiKey"];
            this._httpClient = _httpClient;
            this._taskService = _taskService;
        }

        public async Task<TaskDTO> GenerateTask(string userPromnt){
            if(string.IsNullOrWhiteSpace(_apiKey)){
                throw new Exception("[Error]: API key is null ;(");
            }

            if(string.IsNullOrWhiteSpace(userPromnt)){
                throw new Exception("[Error]: Promnt is empty ;(");
            }

            // Promt for Gemini to server to tell him that he is the best task AI manager ever (lmao)
            var promnt = HelperLayer.AIAgent.SystemPrompts.TaskManagement;

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "system",          
                        parts = new[]
                        {
                            new { text = HelperLayer.AIAgent.SystemPrompts.TaskManagement }
                        }
                    },
                    new
                    {
                        role = "user",            
                        parts = new[]
                        {
                            new { text = userPromnt }
                        }
                    }
                }
            };




            using var request = new HttpRequestMessage(HttpMethod.Post,
                $"https://generativelanguage.googleapis.com/v1beta2/models/{_modelId}:generateText");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception("AI Error. Status: " + response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var textResponse =
                doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(textResponse))
                throw new Exception("AI returns nothing (literary).");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var dto = JsonSerializer.Deserialize<TaskDTO>(textResponse);

            if (dto == null)
                throw new Exception("AI returns broken asf json.");

            return dto;                             
        }

        // this shit was moved, it will be removed soon...
        //
        // public async Task AddGeneratedTask(Guid uuid, string prompt){
        //     try {
        //         TaskDTO taskDto = await GenerateTask(prompt);
        //         await _taskService.CreateAndSaveTask(taskDto, uuid);
        //     }
        //     catch (Exception ex){
        //         throw new Exception($"Failed to add generated task: {ex.Message}", ex);
        //     }
        // }
    }
}
