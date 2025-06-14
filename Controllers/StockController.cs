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
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var stocks = _context.Stock
                .Include(s => s.MenuItem)
                .OrderBy(s => s.MenuItem.ItemName)
                .ToList();
            return View(stocks);
        }

        public IActionResult Create()
    {
        var usedIds = _context.Stock.Select(s => s.MenuItemId).ToList();
        var availableItems = _context.MenuItem
            .Where(m => !usedIds.Contains(m.Id))
            .ToList();

        ViewBag.MenuItems = new SelectList(availableItems, "Id", "ItemName");
        return View();
    }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Stock stock)
        {
            if (!ModelState.IsValid)
            {
                var usedIds = _context.Stock.Select(s => s.MenuItemId).ToList();
                var availableItems = _context.MenuItem
                    .Where(m => !usedIds.Contains(m.Id))
                    .ToList();

                ViewBag.MenuItems = new SelectList(availableItems, "Id", "ItemName", stock.MenuItemId);

                return View(stock);
            }

            var existing = _context.Stock.FirstOrDefault(s => s.MenuItemId == stock.MenuItemId);
            if (existing != null)
{
    existing.Quantity += stock.Quantity;
    _context.Stock.Update(existing);
    TempData["success"] = "Stock actualizado correctamente.";
}
else
{
    _context.Stock.Add(stock);
    TempData["success"] = "Stock agregado correctamente.";
}

_context.SaveChanges();
return RedirectToAction("Index");

        }

        public IActionResult Edit(int id)
        {
            var stock = _context.Stock.Include(s => s.MenuItem).FirstOrDefault(s => s.Id == id);
            if (stock == null) return NotFound();

            return View(stock);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Stock stock)
{
    if (!ModelState.IsValid)
    {
        TempData["error"] = "Datos inválidos.";
        return View(stock);
    }

    var existingStock = _context.Stock.AsNoTracking().FirstOrDefault(s => s.Id == stock.Id);
    if (existingStock == null)
        return NotFound();

    stock.MenuItemId = existingStock.MenuItemId; // 🔐 asegurarse de no cambiar

    _context.Stock.Update(stock);
    _context.SaveChanges();

    TempData["success"] = "Stock actualizado correctamente.";
    return RedirectToAction("Index");
}

        public IActionResult Delete(int id)
        {
            var stock = _context.Stock.Include(s => s.MenuItem).FirstOrDefault(s => s.Id == id);
            if (stock == null) return NotFound();

            return View(stock);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var stock = _context.Stock.Find(id);
            if (stock == null) return NotFound();

            _context.Stock.Remove(stock);
            _context.SaveChanges();
            TempData["success"] = "Stock eliminado.";
            return RedirectToAction("Index");
        }
    }
}