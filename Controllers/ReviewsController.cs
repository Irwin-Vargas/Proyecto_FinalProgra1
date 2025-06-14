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
using Proyecto_FinalProgra1.MLModels;

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
            // Analizar sentimiento usando ML.NET
            var sentimentResult = SentimentPredictor.Predict(comment);

            var review = new Review
            {
                MenuItemId = menuItemId,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Comment = comment,
                CreatedAt = DateTime.UtcNow,
                SentimentPositive = sentimentResult.Prediction,
                SentimentProbability = sentimentResult.Probability
            };

            _context.Reviews.Add(review);
            _context.SaveChanges();

            return RedirectToAction("Details", "MenuItem", new { id = menuItemId });
        }
    }
}
