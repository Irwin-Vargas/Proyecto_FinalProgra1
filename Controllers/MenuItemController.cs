using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_FinalProgra1.Data;
using Proyecto_FinalProgra1.Models;
using Proyecto_FinalProgra1.MLModels;

namespace Proyecto_FinalProgra1.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public MenuItemController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var items = _context.MenuItem
                .Include(c => c.Category)
                .Include(m => m.Reviews)
                .ToList();

            var recomendados = new List<MenuItem>();

            foreach (var item in items)
            {
                var avgRating = item.Reviews.Any() ? (float)item.Reviews.Average(r => r.Rating) : 0f;
                var reviewCount = item.Reviews.Count;
                var totalOrders = _context.OrderDetail
                    .Where(od => od.MenuItemId == item.Id)
                    .Sum(od => od.Quantity);

                var prediction = ProductPopularityModelBuilder.Predict(new ProductData
                {
                    ProductId = item.Id,
                    AverageRating = avgRating,
                    ReviewCount = reviewCount,
                    TotalOrders = totalOrders
                });

                if (prediction.IsPopular)
                    recomendados.Add(item);
            }

bool modoPrueba = true;

if (!recomendados.Any() && modoPrueba && items.Any())
{
    recomendados.Add(items.First());
}


            ViewBag.Recomendados = recomendados;

            return View(items);
        }

        public IActionResult Details(int id)
        {
            var menuItem = _context.MenuItem
                .Include(m => m.Category)
                .Include(m => m.Reviews)
                .FirstOrDefault(m => m.Id == id);

            if (menuItem == null) return NotFound();

            var userIds = menuItem.Reviews.Select(r => r.UserId).Distinct().ToList();
            var users = _userManager.Users.Where(u => userIds.Contains(u.Id)).ToList();
            var userNameDict = users.ToDictionary(u => u.Id, u => u.UserName);

            ViewBag.UserNameDict = userNameDict;

            return View(menuItem);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.CategoryList = new SelectList(_context.Category.ToList(), "Id", "CategoryName");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MenuItem menuItem, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    imageFile.CopyTo(fileStream);

                    menuItem.Image = "/images/" + uniqueFileName;
                }

                _context.MenuItem.Add(menuItem);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryList = new SelectList(_context.Category.ToList(), "Id", "CategoryName", menuItem.CategoryId);
            return View(menuItem);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = _context.MenuItem.Find(id);
            if (item == null) return NotFound();

            ViewBag.CategoryList = new SelectList(_context.Category.ToList(), "Id", "CategoryName", item.CategoryId);
            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MenuItem menuItem, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                var itemFromDb = _context.MenuItem.AsNoTracking().FirstOrDefault(m => m.Id == menuItem.Id);
                if (itemFromDb == null) return NotFound();

                if (imageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    imageFile.CopyTo(fileStream);

                    menuItem.Image = "/images/" + uniqueFileName;
                }
                else
                {
                    menuItem.Image = itemFromDb.Image;
                }

                _context.MenuItem.Update(menuItem);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryList = new SelectList(_context.Category.ToList(), "Id", "CategoryName", menuItem.CategoryId);
            return View(menuItem);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = _context.MenuItem.Include(c => c.Category).FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var item = _context.MenuItem.Find(id);
            if (item == null) return NotFound();

            if (!string.IsNullOrEmpty(item.Image))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, item.Image.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.MenuItem.Remove(item);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        [Authorize(Roles = "Admin")]
public IActionResult EntrenarModelo()
{
    var trainer = new ProductPopularityTrainer(_context);
    trainer.TrainAndSaveModel();
    TempData["success"] = "✅ Modelo entrenado exitosamente con datos reales.";
    return RedirectToAction("Index");
}

    }
}
