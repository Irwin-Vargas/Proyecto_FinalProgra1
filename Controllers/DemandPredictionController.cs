using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proyecto_FinalProgra1.MLModels;
using Proyecto_FinalProgra1.Models; 

namespace Proyecto_FinalProgra1.Controllers
{
    [Route("[controller]")]
    public class DemandPredictionController : Controller
    {
        private readonly ILogger<DemandPredictionController> _logger;
        private readonly DemandPredictionModel _predictionModel;

        public DemandPredictionController(ILogger<DemandPredictionController> logger)
        {
            _logger = logger;
            _predictionModel = new DemandPredictionModel(); // Instancia del modelo
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("forecast")]
        public IActionResult Forecast([FromQuery] string productId, [FromQuery] float day)
        {
            try
            {
                var data = new SalesData { ProductId = productId, Day = day };
                var prediction = _predictionModel.Predict(data);
                return Json(new
                {
                    ProductId = productId,
                    Day = day,
                    PredictedQuantity = prediction
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al predecir la demanda.");
                return StatusCode(500, "Ocurrió un error al realizar la predicción.");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}