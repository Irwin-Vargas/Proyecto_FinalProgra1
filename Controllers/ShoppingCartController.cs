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

namespace Proyecto_FinalProgra1.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            Console.WriteLine("User.Identity.IsAuthenticated: " + User.Identity.IsAuthenticated);
Console.WriteLine("UserId: " + User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
            var cart = _context.ShoppingCart.FirstOrDefault(c => c.UserId == userId && !c.IsDeleted);

            if (cart == null)
                return View(new List<CartDetail>());

            var cartDetails = _context.CartDetail
                .Include(cd => cd.MenuItem)
                .Where(cd => cd.ShoppingCartId == cart.Id)
                .ToList();

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
    }
}