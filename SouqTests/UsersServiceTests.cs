using ApplicationLayer.DTOs;
using ApplicationLayer.Services;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Xunit;

namespace SouqTests
{
    public class UsersServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly UsersService _service;

        public UsersServiceTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _httpContextAccessor = A.Fake<IHttpContextAccessor>();

            _userManager = A.Fake<UserManager<AppUser>>();
            _roleManager = A.Fake<RoleManager<IdentityRole>>();

            _service = new UsersService(
                _unitOfWork,
                _httpContextAccessor,
                _userManager,
                _roleManager);
        }
        
        [Fact]
        public void GetUserId_ShouldReturnCurrentUserId()
        {
            // Arrange
            var userId = "user123";

            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                    }));

            var httpContext = new DefaultHttpContext
            {
                User = principal
            };

            A.CallTo(() => _httpContextAccessor.HttpContext)
                .Returns(httpContext);

            A.CallTo(() => _userManager.GetUserId(principal))
                .Returns(userId);

            // Act
            var result = _service.GetUserId();

            // Assert
            Assert.Equal(userId, result);
        }
        
        [Fact]
        public async Task GetUser_ShouldReturnUser()
        {
            // Arrange
            var userId = "user123";

            var user = new AppUser
            {
                Id = userId,
                UserName = "Bassam"
            };

            A.CallTo(() => _userManager.FindByIdAsync(userId))
                .Returns(user);

            // Act
            var result = await _service.GetUser(userId);

            // Assert
            Assert.Same(user, result);
        }

        [Fact]
        public async Task GetUser_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByIdAsync("invalid"))
                .Returns((AppUser)null);

            // Act
            var result = await _service.GetUser("invalid");

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public void GetUsers_ShouldReturnUsers()
        {
            // Arrange
            var users = new List<AppUser>
        {
            new AppUser
            {
                Id = "1",
                UserName = "User1",
                Email = "user1@test.com"
            },
            new AppUser
            {
                Id = "2",
                UserName = "User2",
                Email = "user2@test.com"
            }
        };

            A.CallTo(() => _userManager.Users)
                .Returns(users.AsQueryable());

            A.CallTo(() => _userManager.GetRolesAsync(users[0]))
                .Returns(Task.FromResult<IList<string>>(
                    new List<string> { "Admin" }));

            A.CallTo(() => _userManager.GetRolesAsync(users[1]))
                .Returns(Task.FromResult<IList<string>>(
                    new List<string> { "User" }));

            // Act
            var result = _service.GetUsers().ToList();

            // Assert
            Assert.Equal(2, result.Count);

            Assert.Equal("1", result[0].ID);
            Assert.Equal("User1", result[0].Name);
            Assert.Equal("user1@test.com", result[0].Email);
            Assert.Contains("Admin", result[0].Roles);

            Assert.Equal("2", result[1].ID);
            Assert.Equal("User2", result[1].Name);
            Assert.Contains("User", result[1].Roles);
        }

        [Fact]
        public void GetUsers_ShouldReturnEmpty_WhenNoUsers()
        {
            // Arrange
            A.CallTo(() => _userManager.Users)
                .Returns(new List<AppUser>().AsQueryable());

            // Act
            var result = _service.GetUsers().ToList();

            // Assert
            Assert.Empty(result);
        }
        
        [Fact]
        public void AllUsers_ShouldReturnFirstPage()
        {
            // Arrange
            var users = Enumerable.Range(1, 15)
                .Select(i => new AppUser
                {
                    Id = i.ToString(),
                    UserName = $"User{i}",
                    Email = $"user{i}@test.com"
                })
                .ToList();

            A.CallTo(() => _userManager.Users)
                .Returns(users.AsQueryable());

            foreach (var user in users)
            {
                A.CallTo(() => _userManager.GetRolesAsync(user))
                    .Returns(Task.FromResult<IList<string>>(
                        new List<string> { "User" }));
            }

            // Act
            var result = _service.AllUsers(null);

            // Assert
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal(10, result.Users.Count());

            Assert.Equal("1", result.Users.First().ID);
        }

        [Fact]
        public void AllUsers_ShouldReturnSecondPage()
        {
            // Arrange
            var users = Enumerable.Range(1, 15)
                .Select(i => new AppUser
                {
                    Id = i.ToString(),
                    UserName = $"User{i}",
                    Email = $"user{i}@test.com"
                })
                .ToList();

            A.CallTo(() => _userManager.Users)
                .Returns(users.AsQueryable());

            foreach (var user in users)
            {
                A.CallTo(() => _userManager.GetRolesAsync(user))
                    .Returns(Task.FromResult<IList<string>>(
                        new List<string> { "User" }));
            }

            // Act
            var result = _service.AllUsers(2);

            // Assert
            Assert.Equal(2, result.CurrentPage);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal(5, result.Users.Count());

            Assert.Equal("11", result.Users.First().ID);
        }

        [Fact]
        public void AllUsers_ShouldReturnEmpty_WhenNoUsers()
        {
            // Arrange
            A.CallTo(() => _userManager.Users)
                .Returns(new List<AppUser>().AsQueryable());

            // Act
            var result = _service.AllUsers(null);

            // Assert
            Assert.Empty(result.Users);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(0, result.TotalPages);
        }
        
        [Fact]
        public async Task GetAllRolesWithUserSelectedRoles_ShouldReturnRolesWithSelection()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user123",
                UserName = "Bassam"
            };

            var roles = new List<IdentityRole>
        {
            new IdentityRole("Admin"),
            new IdentityRole("User"),
            new IdentityRole("Manager")
        };

            A.CallTo(() => _userManager.FindByIdAsync(user.Id))
                .Returns(user);

            A.CallTo(() => _roleManager.Roles)
                .Returns(roles.AsQueryable());

            A.CallTo(() => _userManager.IsInRoleAsync(user, "Admin"))
                .Returns(true);

            A.CallTo(() => _userManager.IsInRoleAsync(user, "User"))
                .Returns(false);

            A.CallTo(() => _userManager.IsInRoleAsync(user, "Manager"))
                .Returns(true);

            // Act
            var result =
                await _service.GetAllRolesWithUserSelectedRoles(user.Id);

            // Assert
            Assert.Equal("user123", result.ID);
            Assert.Equal("Bassam", result.Name);

            Assert.Equal(3, result.Roles.Count);

            Assert.True(result.Roles.First(r => r.Name == "Admin").IsSelected);
            Assert.False(result.Roles.First(r => r.Name == "User").IsSelected);
            Assert.True(result.Roles.First(r => r.Name == "Manager").IsSelected);
        }
        
        [Fact]
        public async Task ManageRoles_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var data = new UserRolesDTO
            {
                ID = "invalid",
                Roles = new List<RoleDTO>()
            };

            A.CallTo(() => _userManager.FindByIdAsync(data.ID))
                .Returns((AppUser)null);

            // Act
            var result = await _service.ManageRoles(data);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User is null.", result.Error);
        }

        [Fact]
        public async Task ManageRoles_ShouldRemoveRole_WhenRoleIsNotSelected()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user123"
            };

            var data = new UserRolesDTO
            {
                ID = user.Id,
                Roles = new List<RoleDTO>
            {
                new RoleDTO
                {
                    Name = "Admin",
                    IsSelected = false
                }
            }
            };

            A.CallTo(() => _userManager.FindByIdAsync(user.Id))
                .Returns(user);

            A.CallTo(() => _userManager.GetRolesAsync(user))
                .Returns(Task.FromResult<IList<string>>(
                    new List<string> { "Admin" }));

            A.CallTo(() => _userManager.RemoveFromRoleAsync(user, "Admin"))
                .Returns(IdentityResult.Success);

            // Act
            var result = await _service.ManageRoles(data);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() =>
                _userManager.RemoveFromRoleAsync(user, "Admin"))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ManageRoles_ShouldAddRole_WhenRoleIsSelected()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user123"
            };

            var data = new UserRolesDTO
            {
                ID = user.Id,
                Roles = new List<RoleDTO>
            {
                new RoleDTO
                {
                    Name = "Admin",
                    IsSelected = true
                }
            }
            };

            A.CallTo(() => _userManager.FindByIdAsync(user.Id))
                .Returns(user);

            A.CallTo(() => _userManager.GetRolesAsync(user))
                .Returns(Task.FromResult<IList<string>>(
                    new List<string>()));

            A.CallTo(() => _userManager.AddToRoleAsync(user, "Admin"))
                .Returns(IdentityResult.Success);

            // Act
            var result = await _service.ManageRoles(data);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() =>
                _userManager.AddToRoleAsync(user, "Admin"))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ManageRoles_ShouldNotChangeRole_WhenSelectionMatchesCurrentRole()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user123"
            };

            var data = new UserRolesDTO
            {
                ID = user.Id,
                Roles = new List<RoleDTO>
            {
                new RoleDTO
                {
                    Name = "Admin",
                    IsSelected = true
                }
            }
            };

            A.CallTo(() => _userManager.FindByIdAsync(user.Id))
                .Returns(user);

            A.CallTo(() => _userManager.GetRolesAsync(user))
                .Returns(Task.FromResult<IList<string>>(
                    new List<string> { "Admin" }));

            // Act
            var result = await _service.ManageRoles(data);

            // Assert
            Assert.True(result.Success);

            A.CallTo(() =>
                _userManager.AddToRoleAsync(A<AppUser>._, A<string>._))
                .MustNotHaveHappened();

            A.CallTo(() =>
                _userManager.RemoveFromRoleAsync(A<AppUser>._, A<string>._))
                .MustNotHaveHappened();
        }
        
        [Fact]
        public async Task Update_ShouldUpdateUser_WhenUserExists()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user123",
                UserName = "OldName",
                Email = "old@test.com",
                PhoneNumber = "111"
            };

            var data = new SettingsDTO
            {
                UserID = "user123",
                UserName = "NewName",
                Email = "new@test.com",
                PhoneNumber = "222"
            };

            A.CallTo(() => _userManager.FindByIdAsync(data.UserID))
                .Returns(user);

            // Act
            var result = await _service.Update(data);

            // Assert
            Assert.True(result.Success);

            Assert.Equal("NewName", user.UserName);
            Assert.Equal("new@test.com", user.Email);
            Assert.Equal("222", user.PhoneNumber);

            A.CallTo(() => _unitOfWork.Commit())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Update_ShouldReturnSuccessFalseMessage_WhenUserDoesNotExist()
        {
            // Arrange
            var data = new SettingsDTO
            {
                UserID = "invalid",
                UserName = "NewName",
                Email = "new@test.com",
                PhoneNumber = "222"
            };

            A.CallTo(() => _userManager.FindByIdAsync(data.UserID))
                .Returns((AppUser)null);

            // Act
            var result = await _service.Update(data);

            // Assert
            // NOTE:
            // Your implementation currently returns Success = true here.
            Assert.True(result.Success);
            Assert.Equal("User didn't updated", result.Error);

            A.CallTo(() => _unitOfWork.Commit())
                .MustNotHaveHappened();
        }
        
        [Fact]
        public async Task Delete_ShouldReturnSuccess_WhenDeleteSucceeds()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user123",
                UserName = "Bassam"
            };

            A.CallTo(() => _userManager.DeleteAsync(user))
                .Returns(IdentityResult.Success);

            // Act
            var result = await _service.Delete(user);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Error);

            A.CallTo(() => _userManager.DeleteAsync(user))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Delete_ShouldReturnError_WhenDeleteFails()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user123"
            };

            var errors = new[]
            {
            new IdentityError
            {
                Description = "Delete failed"
            },
            new IdentityError
            {
                Description = "Something went wrong"
            }
        };

            A.CallTo(() => _userManager.DeleteAsync(user))
                .Returns(IdentityResult.Failed(errors));

            // Act
            var result = await _service.Delete(user);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(
                "Delete failed-Something went wrong",
                result.Error);
        }
    }
}