using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_FinalProgra1.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MenuItemId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool? SentimentPositive { get; set; }
        public float? SentimentProbability { get; set; }  


        // Relaciones
        public MenuItem MenuItem { get; set; }
    }
}