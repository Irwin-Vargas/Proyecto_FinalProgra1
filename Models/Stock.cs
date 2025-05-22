using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FinalProgra1.Models
{
    [Table("Stock")]
    public class Stock
    {
        public int Id { get; set; }

        public int MenuItemId { get; set; }

        public int Quantity { get; set; }

        public MenuItem? MenuItem { get; set; }
    }
}