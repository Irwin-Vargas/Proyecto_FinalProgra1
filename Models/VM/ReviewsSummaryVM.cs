using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_FinalProgra1.Models.VM
{
    public class ReviewsSummaryVM
    {
        public string Producto { get; set; }
        public int Total { get; set; }
        public int Positivas { get; set; }
        public int Negativas { get; set; }
        public int PorcentajePositivas { get; set; }
        public int PorcentajeNegativas { get; set; }
    }
}