using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FinalProgra1.Models
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int MenuItemId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public double UnitPrice { get; set; }

        public Order? Order { get; set; }

        public MenuItem? MenuItem { get; set; }
    }
}