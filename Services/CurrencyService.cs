using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Proyecto_FinalProgra1.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _http;

        public CurrencyService(HttpClient http)
        {
            _http = http;
        }

        public async Task<decimal> GetUsdToPenRateAsync()
        {
            try
            {
                var response = await _http.GetAsync("https://api.exchangerate-api.com/v4/latest/USD");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("rates", out JsonElement rates) &&
                    rates.TryGetProperty("PEN", out JsonElement penRate))
                {
                    return penRate.GetDecimal();
                }

                throw new Exception("No se encontró la tasa PEN en la respuesta.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("💥 Error Currency API: " + ex.Message);
                return 0;
            }
        }
    }
}