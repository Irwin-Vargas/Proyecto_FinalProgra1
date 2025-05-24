using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_FinalProgra1.Models
{
    [Table("OrderStatus")]
    public class OrderStatus
    {
        public int Id { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required, MaxLength(20)]
        public string? StatusName { get; set; }
    }
}