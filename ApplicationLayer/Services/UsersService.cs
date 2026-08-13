using ApplicationLayer.DTOs;
using ApplicationLayer.Helpers;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ApplicationLayer.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            return _userManager.GetUserId(principal);
        }

        public async Task<AppUser> GetUser(string ID)
        {
            return await _userManager.FindByIdAsync(ID);
        }

        public IEnumerable<UserDTO> GetUsers()
        {
            return _userManager.Users
                .Select(u => new UserDTO()
                {
                    ID = u.Id,
                    Name = u.UserName,
                    Email = u.Email,
                    Roles = _userManager.GetRolesAsync(u).Result
                });
        }

        public UsersDTO AllUsers(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var allUsers = _userManager.Users;

            var totalPages = (int)Math.Ceiling(allUsers.Count() / (double)pageSize);

            var users = allUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .Select(u => new UserDTO()
                {
                    ID = u.Id,
                    Name = u.UserName,
                    Email = u.Email,
                    Roles = _userManager.GetRolesAsync(u).Result
                });

            return new UsersDTO
            {
                Users = users,
                CurrentPage = pageNumber,
                TotalPages = totalPages
            };
        }

        public async Task<UserRolesDTO> GetAllRolesWithUserSelectedRoles(string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);
            var roles = _roleManager.Roles.ToList();

            var data = new UserRolesDTO()
            {
                ID = user.Id,
                Name = user.UserName,
                Roles = roles.Select(r => new RoleDTO
                {
                    Name = r.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, r.Name).Result
                }).ToList()
            };

            return data;
        }

        public async Task<Result> ManageRoles(UserRolesDTO data)
        {
            var user = await _userManager.FindByIdAsync(data.ID);

            if (user is null)
                return new Result()
                {
                    Success = false,
                    Error = "User is null."
                };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in data.Roles)
            {
                if (userRoles.Any(r => r == role.Name) && !role.IsSelected)
                    await _userManager.RemoveFromRoleAsync(user, role.Name);

                if (!userRoles.Any(r => r == role.Name) && role.IsSelected)
                    await _userManager.AddToRoleAsync(user, role.Name);
            }

            return new Result() { Success = true };
        }

        public async Task<Result> Update(SettingsDTO data)
        {
            var user = await _userManager.FindByIdAsync(data.UserID);

            if (user is not null)
            {
                user.UserName = data.UserName;
                user.Email = data.Email;
                user.PhoneNumber = data.PhoneNumber;

                await _unitOfWork.Commit();

                return new Result() { Success = true };
            }
            else
                return new Result() { Success = true, Error = "User didn't updated" };
        }

        public async Task<Result> Delete(AppUser data)
        {
            var result = await _userManager.DeleteAsync(data);

            return result.Succeeded ? new Result() { Success = true } :
                new Result() { Success = false, Error = string.Join('-', result.Errors.Select(e => e.Description).ToList()) };
        }
    }
}
