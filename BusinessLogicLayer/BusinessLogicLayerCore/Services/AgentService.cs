using System.Text;
using System.Text.Json;
using BusinessLogicLayerCore.DTOs;
using BusinessLogicLayerCore.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;




namespace BusinessLogicLayerCore.Services
{
    public class AgentService : IAgentService
    {
        private readonly string? _apiKey;
        private readonly string _model;
        private readonly HttpClient _httpClient;

        public AgentService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _apiKey = configuration["AiAgent:ApiKey"];
            _model = configuration["AiAgent:Model"] ?? "google/gemini-2.0-flash-exp:free";
            
            _httpClient = httpClientFactory.CreateClient();
            
            if (!string.IsNullOrEmpty(_apiKey)){
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            }
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:5017");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "Task Manager");
        }

        public async Task<TaskDTO> GenerateTask(string userPromnt)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new Exception("OpenRouter API key is null");

            if (string.IsNullOrWhiteSpace(userPromnt))
                throw new Exception("Prompt is empty");

            var systemMessage = HelperLayer.AIAgent.SystemPrompts.TaskManagement;
                
            var body = new {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemMessage },
                    new { role = "user", content = userPromnt }
                },
                temperature = 0.7,
                max_tokens = 1000,
                response_format = new { type = "json_object" }
            };

            try {
                var requestContent = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(
                    "https://openrouter.ai/api/v1/chat/completions",
                    requestContent
                );

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"[*] Error: OpenRouter API Error: {response.StatusCode}. {error}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);

                var textResponse = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                if (string.IsNullOrWhiteSpace(textResponse))
                    throw new Exception("[*] Error: AI returned empty response");

                textResponse = CleanJson(textResponse);

                var options = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true,
                    Converters = { new HelperLayer.AIAgent.DateTimeConverter() }
                };
                
                var dto = JsonSerializer.Deserialize<TaskDTO>(textResponse, options);
                
                return dto ?? throw new Exception("[*] Error: Failed to parse AI response");
            }

            catch (Exception){
                return new TaskDTO{
                    Title = userPromnt.Length > 50 ? userPromnt[..50] + "..." : userPromnt,
                    Description = "Generated task",
                    Status = DataAccessLayerCore.Enum.StatusTask.InProgress,
                    Priority = DataAccessLayerCore.Enum.PriorityTask.Medium,
                    Deadline = DateTime.Now.AddDays(7)
                };
            }
        }

        public async Task<string> GenerateAnswer(string userPromnt){
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new Exception("[*] Error: OpenRouter API key is null");

            if (string.IsNullOrWhiteSpace(userPromnt))
                throw new Exception("[*] Error: Prompt is empty");

            var systemMessage = HelperLayer.AIAgent.SystemPrompts.AnswerManagment;

            var body = new {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemMessage },
                    new { role = "user", content = userPromnt }
                },
                temperature = 0.7,
                max_tokens = 1000,
                response_format = new { type = "json_object" }
            };

            var response = await _httpClient.PostAsync(
                "https://openrouter.ai/api/v1/chat/completions",
                new StringContent(JsonSerializer.Serialize(body), 
                    Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"AI API error: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            
            var aiResponse = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return aiResponse?.Trim() ?? "[*] Error: Look's like promnts are done ;(";
        }


        private string CleanJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return json;
            
            json = json.Replace("```json", "")
                       .Replace("```", "")
                       .Trim();
            
            int start = json.IndexOf('{');
            int end = json.LastIndexOf('}');
            
            if (start >= 0 && end > start)
            {
                return json.Substring(start, end - start + 1);
            }
            
            return json;
        }
    }
}