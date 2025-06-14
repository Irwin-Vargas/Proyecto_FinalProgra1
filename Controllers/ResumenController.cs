using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_FinalProgra1.Data;
using Proyecto_FinalProgra1.Models.VM;
using Proyecto_FinalProgra1.MLModels;

namespace Proyecto_FinalProgra1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ResumenController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResumenController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            CargarResumen();
            return View();
        }

        // AJAX para predicción dinámica
        [HttpPost]
        public JsonResult PredecirDemanda(int day, string productId)
        {
            try
            {
                var model = new DemandPredictionModel();
                var prediccion = model.Predict(new SalesData
                {
                    Day = day,
                    ProductId = productId
                });

                var message = $"<strong>Predicción para el producto <b>{productId}</b> en el día <b>{day}</b>:</strong><br />" +
                              $"Cantidad esperada: <b>{Math.Round(prediccion, 2)}</b>";
                return Json(new { success = true, message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error en la predicción: " + ex.Message });
            }
        }

        // ----------- Métodos privados de carga de datos -----------

        private void CargarResumen()
        {
            ViewBag.TotalPedidos = _context.Order.Count();
            ViewBag.Pendientes = _context.Order.Count(o => o.OrderStatusId == 1);
            ViewBag.Productos = _context.MenuItem.Count();
            ViewBag.Categorias = _context.Category.Count();

            // Resumen de reseñas por producto (IA)
            var resumen = _context.MenuItem
                .Include(mi => mi.Reviews)
                .Select(mi => new ReviewsSummaryVM
                {
                    Producto = mi.ItemName,
                    Total = mi.Reviews.Count(),
                    Positivas = mi.Reviews.Count(r => r.SentimentPositive == true),
                    Negativas = mi.Reviews.Count(r => r.SentimentPositive == false),
                    PorcentajePositivas = mi.Reviews.Count() == 0 ? 0 :
                        (int)(100.0 * mi.Reviews.Count(r => r.SentimentPositive == true) / mi.Reviews.Count()),
                    PorcentajeNegativas = mi.Reviews.Count() == 0 ? 0 :
                        (int)(100.0 * mi.Reviews.Count(r => r.SentimentPositive == false) / mi.Reviews.Count())
                })
                .OrderByDescending(x => x.PorcentajePositivas)
                .ToList();

            ViewBag.ResumenResenias = resumen;
        }
    }
}
