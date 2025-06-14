using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Proyecto_FinalProgra1.Services
{
    public class GeolocationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessKey = "229e02b866fbf222cd0dbc640acdee18"; // ⚠️ REEMPLAZAR

        public GeolocationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetUserLocationAsync()
        {
            try
            {
                string ip = await _httpClient.GetStringAsync("https://api.ipify.org");
                string url = $"http://api.positionstack.com/v1/reverse?access_key={_accessKey}&query={ip}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return "Ubicación no disponible";

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                var location = json["data"]?.First;
                if (location == null) return "Ubicación no disponible";

                var city = location["locality"]?.ToString();
                var country = location["country"]?.ToString();

                return !string.IsNullOrEmpty(city) ? $"{city}, {country}" : country ?? "Ubicación no disponible";
            }
            catch
            {
                return "Ubicación no disponible";
            }
        }
    }
}
