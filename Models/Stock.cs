using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_FinalProgra1.Models
{
    [Table("Stock")]
    public class Stock
    {
        public int Id { get; set; }
        [Required]
        public int MenuItemId { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser un número positivo.")]
        public int Quantity { get; set; }

        public MenuItem? MenuItem { get; set; }
    }
}