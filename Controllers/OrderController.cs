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
using Microsoft.AspNetCore.Identity;

namespace Proyecto_FinalProgra1.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            string userId = user.Id;

            var cart = await _context.ShoppingCart
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

            if (cart == null)
            {
                TempData["error"] = "No se encontró carrito.";
                return RedirectToAction("Index", "ShoppingCart");
            }

            var cartDetails = await _context.CartDetail
                .Include(cd => cd.MenuItem)
                .ThenInclude(m => m.Stock)
                .Where(cd => cd.ShoppingCartId == cart.Id)
                .ToListAsync();

            if (!cartDetails.Any())
            {
                TempData["error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index", "ShoppingCart");
            }

            // 🔍 Validación de stock
            foreach (var item in cartDetails)
            {
                var stock = await _context.Stock.FirstOrDefaultAsync(s => s.MenuItemId == item.MenuItemId);
                if (stock == null || stock.Quantity < item.Quantity)
                {
                    TempData["error"] = $"No hay suficiente stock para el producto: {item.MenuItem?.ItemName}";
                    return RedirectToAction("Index", "ShoppingCart");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(order);
            }

            // ✅ Guardar el pedido
            order.UserId = userId;
            order.OrderStatusId = 1;
            order.CreateDate = DateTime.UtcNow;

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cartDetails)
            {
                _context.OrderDetail.Add(new OrderDetail
                {
                    OrderId = order.Id,
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });

                // 📉 Descontar stock
                var stock = await _context.Stock.FirstOrDefaultAsync(s => s.MenuItemId == item.MenuItemId);
                if (stock != null)
                {
                    stock.Quantity -= item.Quantity;
                    _context.Stock.Update(stock);
                }
            }

            cart.IsDeleted = true;
            _context.ShoppingCart.Update(cart);
            await _context.SaveChangesAsync();

            TempData["success"] = "Pedido realizado con éxito.";
            return RedirectToAction("MyOrders");
        }

        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            string userId = user.Id;

            var orders = await _context.Order
                .Include(o => o.OrderStatus)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreateDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            string userId = user.Id;

            var order = await _context.Order
                .Include(o => o.OrderDetail)
                .ThenInclude(d => d.MenuItem)
                .Include(o => o.OrderStatus)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound();

            return View(order);
        }
    }
}