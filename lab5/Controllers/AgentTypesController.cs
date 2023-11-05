using lab5.Data;
using lab5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab5.Controllers {
    [Authorize]
    public class AgentTypesController : Controller {
        private readonly InsuranceCompanyContext _context;

        public AgentTypesController(InsuranceCompanyContext context) {
            _context = context;
        }

        // GET: AgentTypes
        public async Task<IActionResult> Index() {
            var agentTypes = GetAgentTypesCookies();
            return View(agentTypes);
        }

        [HttpPost]
        public async Task<IActionResult> Index(AgentType agentType) {
            var agentTypes = SetAgentTypesCookies(agentType);
            return View(agentTypes);
        }

        private IEnumerable<AgentType> GetAgentTypesCookies() {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();
            if (HttpContext.Request.Cookies.ContainsKey("AgentType")) {
                var type = HttpContext.Request.Cookies["AgentType"];
                ViewData["Type"] = type;
                if (type != "")
                    return cache.GetAgentTypes().Where(e => e.Type == type);
            }
            return cache.GetAgentTypes();
        }

        private IEnumerable<AgentType> SetAgentTypesCookies(AgentType agentType) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();
            ViewData["Type"] = agentType.Type;
            HttpContext.Response.Cookies.Append("AgentType", agentType.Type == null ? "" : agentType.Type);
            if (agentType.Type != null)
                return cache.GetAgentTypes().Where(e => e.Type == agentType.Type);
            return cache.GetAgentTypes();
        }

        // GET: AgentTypes/Details/5
        public async Task<IActionResult> Details(int? id) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            if (id == null) {
                return NotFound();
            }

            var agentType = cache.GetAgentTypes()
                .FirstOrDefault(m => m.Id == id);
            if (agentType == null) {
                return NotFound();
            }

            return View(agentType);
        }


        // GET: AgentTypes/Create
        public IActionResult Create() {
            return View();
        }

        // POST: AgentTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type")] AgentType agentType) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            if (ModelState.IsValid) {
                _context.Add(agentType);
                await _context.SaveChangesAsync();
                cache.SetAgentTypes();
                return RedirectToAction(nameof(Index));
            }
            return View(agentType);
        }

        // GET: AgentTypes/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            if (id == null) {
                return NotFound();
            }

            var agentType = cache.GetAgentTypes().FirstOrDefault(e => e.Id == id);
            if (agentType == null) {
                return NotFound();
            }
            return View(agentType);
        }

        // POST: AgentTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type")] AgentType agentType) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            if (id != agentType.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(agentType);
                    await _context.SaveChangesAsync();
                    cache.SetAgentTypes();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!AgentTypeExists(agentType.Id)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(agentType);
        }

        // GET: AgentTypes/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            if (id == null) {
                return NotFound();
            }

            var agentType = cache.GetAgentTypes()
                .FirstOrDefault(m => m.Id == id);
            if (agentType == null) {
                return NotFound();
            }

            return View(agentType);
        }

        // POST: AgentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            var agentType = cache.GetAgentTypes().FirstOrDefault(e => e.Id == id);
            if (agentType != null) {
                _context.AgentTypes.Remove(agentType);
            }

            await _context.SaveChangesAsync();
            cache.SetAgentTypes();
            return RedirectToAction(nameof(Index));
        }

        private bool AgentTypeExists(int id) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            return (cache.GetAgentTypes()?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
