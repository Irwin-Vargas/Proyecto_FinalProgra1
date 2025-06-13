using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Proyecto_FinalProgra1.Data;
using Proyecto_FinalProgra1.Models;
using Microsoft.AspNetCore.Identity;
using Proyecto_FinalProgra1.Services; // ← NUEVO: para usar el servicio de geolocalización

namespace Proyecto_FinalProgra1.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly GeolocationService _geoService; // ← NUEVO: Servicio inyectado

        public ShoppingCartController(ApplicationDbContext context, UserManager<IdentityUser> userManager, GeolocationService geoService)
        {
            _context = context;
            _userManager = userManager;
            _geoService = geoService;
        }

        public async Task<IActionResult> Index() // ← Ahora async
        {
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
            var cart = _context.ShoppingCart.FirstOrDefault(c => c.UserId == userId && !c.IsDeleted);

            if (cart == null)
            {
                ViewBag.Ubicacion = await _geoService.GetUserLocationAsync(); // ← Obtener ubicación aunque no haya carrito
                return View(new List<CartDetail>());
            }

            var cartDetails = _context.CartDetail
                .Include(cd => cd.MenuItem)
                .Where(cd => cd.ShoppingCartId == cart.Id)
                .ToList();

            ViewBag.Ubicacion = await _geoService.GetUserLocationAsync(); // ← Agregar ubicación
            return View(cartDetails);
        }

        public IActionResult AddToCart(int id)
        {
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

            var cart = _context.ShoppingCart.FirstOrDefault(c => c.UserId == userId && !c.IsDeleted);
            if (cart == null)
            {
                cart = new ShoppingCart { UserId = userId };
                _context.ShoppingCart.Add(cart);
                _context.SaveChanges();
            }

            var menuItem = _context.MenuItem.Include(m => m.Stock).FirstOrDefault(m => m.Id == id);
            if (menuItem == null)
                return NotFound();

            var stock = _context.Stock.FirstOrDefault(s => s.MenuItemId == menuItem.Id);
            if (stock == null || stock.Quantity <= 0)
            {
                TempData["error"] = "Este producto está agotado.";
                return RedirectToAction("Index", "MenuItem");
            }

            var cartDetail = _context.CartDetail.FirstOrDefault(cd => cd.MenuItemId == id && cd.ShoppingCartId == cart.Id);

            if (cartDetail != null)
            {
                if (cartDetail.Quantity + 1 > stock.Quantity)
                {
                    TempData["error"] = "No hay suficiente stock para este producto.";
                    return RedirectToAction("Index", "MenuItem");
                }

                cartDetail.Quantity++;
                cartDetail.UnitPrice = menuItem.Price;
                _context.CartDetail.Update(cartDetail);
            }
            else
            {
                cartDetail = new CartDetail
                {
                    MenuItemId = menuItem.Id,
                    ShoppingCartId = cart.Id,
                    Quantity = 1,
                    UnitPrice = menuItem.Price
                };
                _context.CartDetail.Add(cartDetail);
            }

            _context.SaveChanges();
            TempData["success"] = "Producto agregado al carrito.";
            return RedirectToAction("Index", "MenuItem");
        }

        public IActionResult Remove(int id)
        {
            var cartDetail = _context.CartDetail.FirstOrDefault(cd => cd.Id == id);
            if (cartDetail == null) return NotFound();

            _context.CartDetail.Remove(cartDetail);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetCartTotal()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var total = await _context.CartDetail
                .Where(cd => cd.ShoppingCart.UserId == user.Id && !cd.ShoppingCart.IsDeleted)
                .SumAsync(cd => cd.Quantity * cd.UnitPrice);

            return Json(new { total });
        }
    }
}
