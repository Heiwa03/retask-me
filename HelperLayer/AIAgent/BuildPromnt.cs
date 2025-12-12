// using System.Text;
// using 

// namespace HelperLayer.AIAgent;

// public class BuildPromnt{
//     private string BuildPrompt(TaskCreationRequest request)
//     {
//         var promptBuilder = new StringBuilder();
        
//         promptBuilder.AppendLine("Ты - помощник по управлению задачами. Создай структурированную задачу на основе описания пользователя.");
//         promptBuilder.AppendLine();
        
//         if (!string.IsNullOrEmpty(request.ProjectContext))
//         {
//             promptBuilder.AppendLine($"Контекст проекта: {request.ProjectContext}");
//             promptBuilder.AppendLine();
//         }
        
//         promptBuilder.AppendLine($"Описание задачи от пользователя: \"{request.UserDescription}\"");
//         promptBuilder.AppendLine();
        
//         promptBuilder.AppendLine("Верни ответ ТОЛЬКО в формате JSON без каких-либо дополнительных текстов.");
//         promptBuilder.AppendLine("Формат JSON:");
//         promptBuilder.AppendLine(@"
//                 {
//                     ""title"": ""Краткий и понятный заголовок задачи (максимум 10 слов)"",
//                     ""description"": ""Детальное описание задачи с конкретными требованиями"",
//                     ""priority"": ""Low"" или ""Medium"" или ""High"",
//                     ""deadline"": ""YYYY-MM-DD"" или null если не указано,
//                     ""tags"": [""список"", ""релевантных"", ""тегов""],
//                     ""assignee"": ""Имя ответственного или команда"",
//                     ""estimatedHours"": число от 1 до 40
//                 }");
        
//         promptBuilder.AppendLine();
//         promptBuilder.AppendLine("Правила:");
//         promptBuilder.AppendLine("- Если deadline не указан, используй null");
//         promptBuilder.AppendLine("- Если assignee неясен, используй 'Не назначено'");
//         promptBuilder.AppendLine("- Приоритет определяй по срочности и важности");
        
//         return promptBuilder.ToString();
//     }

// }