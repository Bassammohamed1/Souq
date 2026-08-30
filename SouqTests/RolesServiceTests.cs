using ApplicationLayer.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace SouqTests
{
    public class RolesServiceTests
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RolesService _service;

        public RolesServiceTests()
        {
            _roleManager = A.Fake<RoleManager<IdentityRole>>();
            _service = new RolesService(_roleManager);
        }

        [Fact]
        public void AllRoles_ShouldReturnRoles()
        {
            var roles = new List<IdentityRole>
        {
            new IdentityRole("Admin"),
            new IdentityRole("User")
        };

            A.CallTo(() => _roleManager.Roles)
                .Returns(roles.AsQueryable());

            var result = _service.AllRoles();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.Name == "Admin");
            Assert.Contains(result, r => r.Name == "User");
        }

        [Fact]
        public void AllRoles_WhenNoRoles_ShouldReturnEmpty()
        {
            A.CallTo(() => _roleManager.Roles)
                .Returns(Enumerable.Empty<IdentityRole>().AsQueryable());

            var result = _service.AllRoles();

            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateRole_WhenRoleAlreadyExists_ShouldReturnFailure()
        {
            A.CallTo(() => _roleManager.RoleExistsAsync("Admin"))
                .Returns(true);

            var result = await _service.CreateRole("Admin");

            Assert.False(result.Success);
            Assert.Equal(
                "Role Admin is already exist.",
                result.Error);

            A.CallTo(() => _roleManager.CreateAsync(A<IdentityRole>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task CreateRole_WhenRoleDoesNotExist_ShouldCreateRole()
        {
            A.CallTo(() => _roleManager.RoleExistsAsync("Admin"))
                .Returns(false);

            A.CallTo(() => _roleManager.CreateAsync(
                    A<IdentityRole>.That.Matches(r => r.Name == "Admin")))
                .Returns(IdentityResult.Success);

            var result = await _service.CreateRole("Admin");

            Assert.True(result.Success);
            Assert.Null(result.Error);

            A.CallTo(() => _roleManager.CreateAsync(
                    A<IdentityRole>.That.Matches(r => r.Name == "Admin")))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateRole_ShouldTrimRoleName()
        {
            A.CallTo(() => _roleManager.RoleExistsAsync("  Admin  "))
                .Returns(false);

            A.CallTo(() => _roleManager.CreateAsync(
                    A<IdentityRole>.That.Matches(r => r.Name == "Admin")))
                .Returns(IdentityResult.Success);

            var result = await _service.CreateRole("  Admin  ");

            Assert.True(result.Success);

            A.CallTo(() => _roleManager.CreateAsync(
                    A<IdentityRole>.That.Matches(r => r.Name == "Admin")))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateRole_WhenCreateFails_ShouldReturnFailure()
        {
            A.CallTo(() => _roleManager.RoleExistsAsync("Admin"))
                .Returns(false);

            var errors = new[]
            {
            new IdentityError
            {
                Code = "DuplicateRole",
                Description = "Role already exists."
            }
        };

            A.CallTo(() => _roleManager.CreateAsync(
                    A<IdentityRole>._))
                .Returns(IdentityResult.Failed(errors));

            var result = await _service.CreateRole("Admin");

            Assert.False(result.Success);
            Assert.NotNull(result.Error);
        }
    }
}
