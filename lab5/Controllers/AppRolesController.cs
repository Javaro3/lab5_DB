using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab5.Controllers {
    [Authorize(Roles = "Admin")]
    public class AppRolesController : Controller {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AppRolesController(RoleManager<IdentityRole> roleManager) {
            _roleManager = roleManager;
        }

        public IActionResult Index() {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole model) {
            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult()) {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string? id) {
            if (id == null || _roleManager.Roles == null) {
                return NotFound();
            }

            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (role == null) {
                return NotFound();
            }

            await _roleManager.DeleteAsync(role);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string? id) {
            if (id == null || _roleManager.Roles == null) {
                return NotFound();
            }

            var role = await _roleManager.Roles.FirstOrDefaultAsync(e => e.Id == id);
            if (role == null) {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IdentityRole role) {
            var updateRole = _roleManager.Roles.FirstOrDefault(e => e.Id == role.Id);
            updateRole.Name = role.Name;
            _roleManager.UpdateAsync(updateRole).GetAwaiter().GetResult();
            return RedirectToAction(nameof(Index));
        }
    }
}
