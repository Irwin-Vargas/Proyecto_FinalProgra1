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

        // ENTRENAMIENTO
        public IActionResult EntrenarModelo()
        {
            var datos = _context.OrderDetail
                .Include(o => o.MenuItem)
                .GroupBy(o => o.MenuItem.ItemName)
                .Select(g => new { Nombre = g.Key, Ventas = g.Sum(x => x.Quantity) })
                .ToList()
                .Select(d => (d.Nombre, d.Ventas));

            ProductPopularityModelBuilder.TrainModel(datos, umbral: 5); // umbral bajo para testing

            return Content("Modelo entrenado con éxito");
        }

        // PREDICCIÓN POR NOMBRE
        public IActionResult PredecirPopularidad(string nombre)
        {
            var ventas = _context.OrderDetail
                .Include(o => o.MenuItem)
                .Where(o => o.MenuItem.ItemName == nombre)
                .Sum(o => o.Quantity);

            var predictor = ProductPopularityModelBuilder.LoadPredictionEngine();

            var input = new ProductData { Ventas = ventas };
            var prediction = predictor.Predict(input);

            return Json(new
            {
                Producto = nombre,
                Ventas = ventas,
                EsPopular = prediction.EsPopular,
                Probabilidad = $"{prediction.Probability:P2}"
            });
        }
    }
}