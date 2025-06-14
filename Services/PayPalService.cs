using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Proyecto_FinalProgra1.Services
{
    public static class PayPalService
    {
        private static string clientId;
        private static string secret;
        private static readonly string baseUrl = "https://api-m.sandbox.paypal.com"; // sandbox

        public static void Configure(IConfiguration config)
        {
            clientId = config["PayPal:ClientId"];
            secret = config["PayPal:Secret"];
        }

        public static async Task<string> GetAccessToken()
        {
            using var client = new HttpClient();
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{secret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

            var body = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await client.PostAsync($"{baseUrl}/v1/oauth2/token", body);

            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo obtener el token de acceso PayPal");

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            return result?.access_token ?? throw new Exception("Token inválido o no retornado por PayPal");
        }

        public static async Task<string> CreateOrderAsync(decimal total)
        {
            var token = await GetAccessToken();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var order = new
            {
                intent = "CAPTURE",
                purchase_units = new[] {
                    new {
                        amount = new {
                            currency_code = "USD",
                            value = total.ToString("F2")
                        }
                    }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{baseUrl}/v2/checkout/orders", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error creando la orden: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            return result?.id ?? throw new Exception("PayPal no retornó un ID de orden");
        }

        public static async Task<bool> CaptureOrderAsync(string orderId)
        {
            var token = await GetAccessToken();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{baseUrl}/v2/checkout/orders/{orderId}/capture", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("❌ Error capturando orden PayPal: " + error);
                return false;
            }

            return true;
        }
    }
}
