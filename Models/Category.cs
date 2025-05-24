using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FinalProgra1.Models
{
    [Table("Category")]
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(40)]
        public string CategoryName { get; set; }

        public List<MenuItem> MenuItems { get; set; } = new();
    }
}