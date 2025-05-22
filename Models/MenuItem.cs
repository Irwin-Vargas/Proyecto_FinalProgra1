using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FinalProgra1.Models
{
    [Table("MenuItem")]
    public class MenuItem
    {
        public int Id { get; set; }

        [Required, MaxLength(40)]
        public string? ItemName { get; set; }

        [Required, MaxLength(40)]
        public string? Description { get; set; }

        [Required]
        public double Price { get; set; }

        public string? Image { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public List<OrderDetail> OrderDetail { get; set; } = new();
        public List<CartDetail> CartDetail { get; set; } = new();
        public Stock? Stock { get; set; }

        [NotMapped]
        public string? CategoryName { get; set; }

        [NotMapped]
        public int Quantity { get; set; }
    }
}