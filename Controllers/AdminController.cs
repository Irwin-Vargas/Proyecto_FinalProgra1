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
using Proyecto_FinalProgra1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Proyecto_FinalProgra1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _context.Order
                .Include(o => o.OrderStatus)
                .OrderByDescending(o => o.CreateDate)
                .ToList();

            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = _context.Order
                .Include(o => o.OrderDetail)
                .ThenInclude(od => od.MenuItem)
                .Include(o => o.OrderStatus)
                .FirstOrDefault(o => o.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        public IActionResult ChangeStatus(int id)
        {
            var order = _context.Order
                .Include(o => o.OrderStatus)
                .FirstOrDefault(o => o.Id == id);

            if (order == null) return NotFound();

            ViewBag.StatusList = new SelectList(_context.OrderStatus.ToList(), "StatusId", "StatusName", order.OrderStatusId);

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatus(int id, int statusId)
        {
            var order = _context.Order.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            order.OrderStatusId = statusId;
            _context.SaveChanges();

            TempData["success"] = "Estado actualizado correctamente.";
            return RedirectToAction("Index");
        }
    }
}