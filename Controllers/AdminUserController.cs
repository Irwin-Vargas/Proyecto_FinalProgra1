using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Proyecto_FinalProgra1.Models;
using Proyecto_FinalProgra1.Models.VM;

namespace Proyecto_FinalProgra1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminUserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> ManageRoles(string userId)
{
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null) return NotFound();

    var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
    var userRoles = await _userManager.GetRolesAsync(user);

    var viewModel = new ManageRolesVM
    {
        UserId = user.Id,
        Email = user.Email,
        Roles = allRoles.Select(role => new RoleSelectionVM
        {
            RoleName = role,
            IsSelected = userRoles.Contains(role)
        }).ToList()
    };

    return View(viewModel);
}


[HttpPost]
public async Task<IActionResult> UpdateRoles(ManageRolesVM model)
{
    Console.WriteLine($"==> Entrando a UpdateRoles para el usuario: {model.Email}");

    if (model.Roles == null)
    {
        Console.WriteLine("⚠️ No se recibieron roles.");
        return RedirectToAction("Index");
    }

    var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();
    Console.WriteLine($"==> Roles seleccionados: {string.Join(", ", selectedRoles)}");

    var user = await _userManager.FindByIdAsync(model.UserId);
    if (user == null)
    {
        Console.WriteLine("❌ Usuario no encontrado.");
        return NotFound();
    }

    var currentRoles = await _userManager.GetRolesAsync(user);
    await _userManager.RemoveFromRolesAsync(user, currentRoles);
    Console.WriteLine("✅ Roles actuales eliminados.");

    if (selectedRoles.Any())
    {
        await _userManager.AddToRolesAsync(user, selectedRoles);
        Console.WriteLine("✅ Nuevos roles asignados.");
    }

    return RedirectToAction("Index");
}


        [HttpPost]
public async Task<IActionResult> LockUser(string userId)
{
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null) return NotFound();

    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
    return RedirectToAction(nameof(Index));
}

[HttpPost]
public async Task<IActionResult> UnlockUser(string userId)
{
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null) return NotFound();

    await _userManager.SetLockoutEndDateAsync(user, null);
    return RedirectToAction(nameof(Index));
}

[HttpPost]
public async Task<IActionResult> DeleteUser(string userId)
{
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null) return NotFound();

    await _userManager.DeleteAsync(user);
    return RedirectToAction(nameof(Index));
}

    }
}
