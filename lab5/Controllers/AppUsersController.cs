using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace lab5.Controllers {

    [Authorize(Roles = "Admin")]
    public class AppUsersController : Controller {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AppUsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index() {
            List<(IdentityUser User, string Role)> usersRole = new();
            var users = _userManager.Users.ToList();
            foreach (var user in users) {
                var role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();
                usersRole.Add(new(user, role[0]));
            }
            return View(usersRole);
        }

        public async Task<IActionResult> Delete(string? id) {
            if (id == null || _userManager.Users == null) {
                return NotFound();
            }

            var user = await _userManager.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create() {
            ViewData["RoleList"] = _roleManager.Roles.Select(e => new SelectListItem() {
                Text = e.Name,
                Value = e.Name
            });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string email, string password, string role) {
            var user = new IdentityUser() { UserName = email, Email = email };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded) {
                var currentUser = await _userManager.FindByNameAsync(user.UserName);

                var roleresult = await _userManager.AddToRoleAsync(currentUser, role);
            }


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string? id) {
            ViewData["RoleList"] = _roleManager.Roles.Select(e => new SelectListItem() {
                Text = e.Name,
                Value = e.Name
            });
            var user = _userManager.Users.FirstOrDefault(e => e.Id == id);
            var role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult()[0];
            return View((User: user, Role: role));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email, string role) {
            var user = _userManager.Users.FirstOrDefault(e => e.Id == id);
            user.UserName = email;
            user.Email = email;
            _userManager.UpdateAsync(user).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(user, role).GetAwaiter().GetResult();
            var roles = _roleManager.Roles.ToList();
            foreach (var i in roles) {
                if (i.Name != role)
                    _userManager.RemoveFromRoleAsync(user, i.Name).GetAwaiter().GetResult();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
