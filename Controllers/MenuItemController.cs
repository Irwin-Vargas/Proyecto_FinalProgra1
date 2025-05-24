using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_FinalProgra1.Data;
using Proyecto_FinalProgra1.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto_FinalProgra1.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MenuItemController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var items = _context.MenuItem.Include(c => c.Category).ToList();
            return View(items);
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
            foreach (var entry in ModelState)
            {
                Console.WriteLine($"[ERROR] {entry.Key}:");
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"  - {error.ErrorMessage}");
                }
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
    }
}