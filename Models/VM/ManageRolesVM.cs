using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_FinalProgra1.Models.VM
{
    public class ManageRolesVM
    {
        public string UserId { get; set; }
        public string Email { get; set; }

        // IMPORTANTE: inicializamos para evitar null
        public List<RoleSelectionVM> Roles { get; set; } = new List<RoleSelectionVM>();
    }

    public class RoleSelectionVM
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}