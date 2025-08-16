using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PresentationLayer.ViewModels.Identity;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var allUsers = _userManager.Users;

            var totalPages = (int)Math.Ceiling(allUsers.Count() / (double)pageSize);

            var users = await allUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .Select(u => new UserViewModel()
                {
                    ID = u.Id,
                    Name = u.UserName,
                    Email = u.Email,
                    Roles = _userManager.GetRolesAsync(u).Result
                }).ToListAsync();

            var usersVM = new UsersViewModel()
            {
                Users = users,
                CurrentPage = pageNumber,
                TotalPages = totalPages
            };

            return View(usersVM);
        }

        public async Task<IActionResult> ManageRoles(string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);
            var roles = _roleManager.Roles.ToList();

            if (user is not null)
            {
                var userVM = new UserRolesViewModel()
                {
                    ID = user.Id,
                    Name = user.UserName,
                    Roles = roles.Select(r => new RoleViewModel()
                    {
                        Name = r.Name,
                        IsSelected = _userManager.IsInRoleAsync(user, r.Name).Result
                    }).ToList()
                };

                return View(userVM);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ManageRoles(UserRolesViewModel data)
        {
            var user = await _userManager.FindByIdAsync(data.ID);
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in data.Roles)
            {
                if (userRoles.Any(r => r == role.Name) && !role.IsSelected)
                    await _userManager.RemoveFromRoleAsync(user, role.Name);

                if (!userRoles.Any(r => r == role.Name) && role.IsSelected)
                    await _userManager.AddToRoleAsync(user, role.Name);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);

            return user is not null ? View(user) : NotFound("User not found.");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(AppUser user)
        {
            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded ? RedirectToAction("Index") : BadRequest(string.Join('-', result.Errors.Select(e => e.Description).ToList()));
        }

        public async Task<IActionResult> Settings(string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);

            if (user is not null)
            {
                var settingsVM = new SettingsViewModel()
                {
                    UserID = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };

                return View(settingsVM);
            }

            return NotFound("User not found.");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Settings(SettingsViewModel data)
        {
            var user = await _userManager.FindByIdAsync(data.UserID);

            if (user is not null)
            {
                user.UserName = data.UserName;
                user.Email = data.Email;
                user.PhoneNumber = data.PhoneNumber;

                await _unitOfWork.Commit();

                return RedirectToAction("Index");
            }
            else
                return BadRequest("User didn't updated");
        }
    }
}
