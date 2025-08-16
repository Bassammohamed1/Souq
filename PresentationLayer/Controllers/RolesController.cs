using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PresentationLayer.ViewModels.Identity;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            return View(roles ?? Enumerable.Empty<IdentityRole>());
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleFormViewModel role)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            var isRoleExists = await _roleManager.RoleExistsAsync(role.Name);

            if (isRoleExists)
            {
                ModelState.AddModelError("Name", $"Role {role.Name} is already exist.");
                return View("Index", _roleManager.Roles);
            }
            else
                await _roleManager.CreateAsync(new IdentityRole(role.Name));

            return RedirectToAction("Index");
        }
    }
}
