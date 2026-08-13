using ApplicationLayer.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels.Identity;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly IRolesService _roles;

        public RolesController(IRolesService roles)
        {
            _roles = roles;
        }

        public IActionResult Index()
        {
            var roles = _roles.AllRoles();

            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleFormViewModel role)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            var result = await _roles.CreateRole(role.Name);

            if (!result.Success)
            {
                ModelState.AddModelError("Name", result.Error);

                return View("Index", _roles.AllRoles());
            }
            else
                return RedirectToAction("Index");
        }
    }
}