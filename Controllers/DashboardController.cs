using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Proyecto_FinalProgra1.Data;
using Proyecto_FinalProgra1.Models.VM;

namespace Proyecto_FinalProgra1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _context.Order.Include(o => o.OrderStatus).ToList();
            var products = _context.MenuItem.Include(c => c.Category).ToList();
            var categories = _context.Category.ToList();
            var stocks = _context.Stock.Include(s => s.MenuItem).ToList();

            var ventas = _context.OrderDetail
                .Include(od => od.MenuItem)
                .GroupBy(od => od.MenuItem.ItemName)
                .Select(group => new
                {
                    Nombre = group.Key,
                    Total = group.Sum(x => x.Quantity)
                }).ToList();
            ViewBag.VentasPorProducto = ventas;
            ViewBag.TotalOrders = orders.Count;
            ViewBag.TotalProducts = products.Count;
            ViewBag.TotalCategories = categories.Count;
            ViewBag.TotalStockItems = stocks.Count;

            // --- Resumen de reseñas por producto (IA) ---
            var resumen = _context.MenuItem
                .Include(mi => mi.Reviews)
                .Select(mi => new ReviewsSummaryVM
                {
                    Producto = mi.ItemName,
                    Total = mi.Reviews.Count(),
                    Positivas = mi.Reviews.Count(r => r.SentimentPositive == true),
                    Negativas = mi.Reviews.Count(r => r.SentimentPositive == false),
                    PorcentajePositivas = mi.Reviews.Count() == 0 ? 0 : (int)(100.0 * mi.Reviews.Count(r => r.SentimentPositive == true) / mi.Reviews.Count()),
                    PorcentajeNegativas = mi.Reviews.Count() == 0 ? 0 : (int)(100.0 * mi.Reviews.Count(r => r.SentimentPositive == false) / mi.Reviews.Count())
                })
                .OrderByDescending(x => x.PorcentajePositivas)
                .ToList();

            ViewBag.ResumenResenias = resumen;

            return View();
        }

        public IActionResult Resumen()
        {
            ViewBag.TotalOrders = _context.Order.Count();
            ViewBag.PedidosPendientes = _context.Order.Count(o => o.OrderStatusId == 1); // Suponiendo que 1 = Pendiente
            ViewBag.TotalProducts = _context.MenuItem.Count();
            ViewBag.TotalCategories = _context.Category.Count();
            ViewBag.TotalStockItems = _context.Stock.Count();

            return View();
        }
    }
}
