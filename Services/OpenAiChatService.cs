using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Proyecto_FinalProgra1.Services
{
    public class OpenAiChatService
    {
        private readonly string _apiKey;

        public OpenAiChatService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<string> GetChatResponse(string message)
        {
            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var requestData = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] {
                    new { role = "user", content = message }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode)
                return $"Error al comunicarse con OpenAI ({response.StatusCode})";

            var responseContent = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseContent);

            var answer = jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return answer?.Trim() ?? "Respuesta vacía.";
        }
    }
}
