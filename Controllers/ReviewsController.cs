using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Proyecto_FinalProgra1.Data;
using Proyecto_FinalProgra1.Models;
using System.Security.Claims;

namespace Proyecto_FinalProgra1.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddReview(int menuItemId, string comment)
        {
            var review = new Review
            {
                MenuItemId = menuItemId,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            _context.SaveChanges();

            return RedirectToAction("Details", "MenuItem", new { id = menuItemId });
        }
    }
}