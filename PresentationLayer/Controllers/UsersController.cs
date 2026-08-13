using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels.Identity;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUsersService _users;

        public UsersController(IUsersService users)
        {
            _users = users;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var users = _users.AllUsers(page);

            var usersVM = new UsersViewModel()
            {
                Users = users.Users.Select(u => new UserViewModel
                {
                    ID = u.ID,
                    Email = u.Email,
                    Name = u.Name,
                    Roles = u.Roles
                }),
                CurrentPage = users.CurrentPage,
                TotalPages = users.TotalPages
            };

            return View(usersVM);
        }

        public async Task<IActionResult> ManageRoles(string userId)
        {
            if (userId == null)
                return NotFound("Invalid userID");

            var result = await _users.GetAllRolesWithUserSelectedRoles(userId);

            var viewModel = new UserRolesViewModel()
            {
                ID = result.ID,
                Name = result.Name,
                Roles = result.Roles.Select(r => new RoleViewModel()
                {
                    Name = r.Name,
                    IsSelected = r.IsSelected
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ManageRoles(UserRolesViewModel data)
        {
            var dto = new UserRolesDTO()
            {
                ID = data.ID,
                Name = data.Name,
                Roles = data.Roles.Select(r => new RoleDTO() { Name = r.Name, IsSelected = r.IsSelected }).ToList()
            };

            var result = await _users.ManageRoles(dto);

            return result.Success ? RedirectToAction(nameof(Index)) : View(data);

        }

        public async Task<IActionResult> Delete(string userID)
        {
            var user = await _users.GetUser(userID);

            return user is not null ? View(user) : NotFound("User not found.");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(AppUser user)
        {
            var result = await _users.Delete(user);

            return result.Success ? RedirectToAction("Index") : BadRequest(result.Error);
        }

        public async Task<IActionResult> Settings(string userID)
        {
            var user = await _users.GetUser(userID);

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
            var settingsDTO = new SettingsDTO()
            {
                UserID = data.UserID,
                Email = data.Email,
                PhoneNumber = data.PhoneNumber,
                UserName = data.UserName
            };

            var result = await _users.Update(settingsDTO);

            return result.Success ? RedirectToAction("Index") : BadRequest(result.Error);
        }
    }
}
