using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Proyecto_FinalProgra1.Data;
using Proyecto_FinalProgra1.MLModels;

namespace Proyecto_FinalProgra1.Controllers
{
    public class MLController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MLController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Acción para entrenar el modelo con datos reales
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EntrenarModelo()
        {
            var trainer = new ProductPopularityTrainer(_context);
            trainer.TrainAndSaveModel();

            TempData["success"] = "✅ Modelo entrenado correctamente con datos reales.";
            return RedirectToAction("Index", "Home"); // Redirecciona a donde tú prefieras
        }

        // Acción de prueba para predecir (ya la tenías)
        public IActionResult PredecirPopularidad()
        {
            var testProduct = new ProductData
            {
                ProductId = 1,
                AverageRating = 4.5f,
                ReviewCount = 25,
                TotalOrders = 60
            };

            var resultado = ProductPopularityModelBuilder.Predict(testProduct);

            ViewBag.Resultado = resultado;
            return View();
        }
    }
}
