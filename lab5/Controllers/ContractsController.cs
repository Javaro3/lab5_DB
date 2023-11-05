using lab5.Data;
using lab5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab5.Controllers {
    [Authorize]
    public class ContractsController : Controller {
        private readonly InsuranceCompanyContext _context;

        public ContractsController(InsuranceCompanyContext context) {
            _context = context;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10) {
            var agentTypes = GetContractCookies();
            ViewData["ItemsCount"] = agentTypes.Count();
            agentTypes = agentTypes.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            return View(agentTypes);
        }

        [HttpPost]
        public async Task<IActionResult> Index(Contract contract, int page = 1, int pageSize = 10) {
            var agentTypes = SetContractsCookies(contract);
            ViewData["ItemsCount"] = agentTypes.Count();
            agentTypes = agentTypes.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            return View(agentTypes);
        }

        private IEnumerable<Contract> GetContractCookies() {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();
            IEnumerable<Contract> contracts = cache.GetContracts();

            if (HttpContext.Request.Cookies.ContainsKey("ContractResponsibilities")) {
                var responsibilities = HttpContext.Request.Cookies["ContractResponsibilities"];
                ViewData["ContractResponsibilities"] = responsibilities;
                contracts = contracts.Where(e => responsibilities == "" || e.Responsibilities == responsibilities);
            }

            if (HttpContext.Request.Cookies.ContainsKey("ContractStartDeadline")) {
                var startDeadline = HttpContext.Request.Cookies["ContractStartDeadline"];
                ViewData["ContractStartDeadline"] = startDeadline;
                if (startDeadline != "") {
                    var startDeadlineDate = DateTime.Parse(startDeadline);
                    contracts = contracts.Where(e => e.StartDeadline >= startDeadlineDate);
                }
            }

            if (HttpContext.Request.Cookies.ContainsKey("ContractEndDeadline")) {
                var endDeadline = HttpContext.Request.Cookies["ContractEndDeadline"];
                ViewData["ContractEndDeadline"] = endDeadline;
                if (endDeadline != "") {
                    var endDeadlineDate = DateTime.Parse(endDeadline);
                    contracts = contracts.Where(e => e.EndDeadline <= endDeadlineDate);
                }
            }

            return contracts;
        }

        private IEnumerable<Contract> SetContractsCookies(Contract contract) {
            ViewData["ContractResponsibilities"] = contract.Responsibilities;
            ViewData["ContractStartDeadline"] = contract.StartDeadline == default ? null : contract.StartDeadline;
            ViewData["ContractEndDeadline"] = contract.EndDeadline == default ? null : contract.EndDeadline;
            HttpContext.Response.Cookies.Append("ContractResponsibilities", contract.Responsibilities == null ? "" : contract.Responsibilities);
            HttpContext.Response.Cookies.Append("ContractStartDeadline", contract.StartDeadline == default ? "" : contract.StartDeadline.ToString());
            HttpContext.Response.Cookies.Append("ContractEndDeadline", contract.EndDeadline == default ? "" : contract.EndDeadline.ToString());

            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();
            IEnumerable<Contract> contracts = cache.GetContracts();
            if (contract.Responsibilities != null)
                contracts = contracts.Where(e => e.Responsibilities == contract.Responsibilities);

            if (contract.StartDeadline != default)
                contracts = contracts.Where(e => e.StartDeadline >= contract.StartDeadline);

            if (contract.EndDeadline != default)
                contracts = contracts.Where(e => e.EndDeadline <= contract.EndDeadline);

            return contracts;
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();
            var contract = cache.GetContracts()
                .FirstOrDefault(m => m.Id == id);
            if (contract == null) {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Responsibilities,StartDeadline,EndDeadline")] Contract contract) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            if (ModelState.IsValid) {
                _context.Add(contract);
                await _context.SaveChangesAsync();
                cache.SetContracts();
                return RedirectToAction(nameof(Index));
            }
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            if (id == null) {
                return NotFound();
            }

            var contract = cache.GetContracts().FirstOrDefault(e => e.Id == id);
            if (contract == null) {
                return NotFound();
            }
            return View(contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Responsibilities,StartDeadline,EndDeadline")] Contract contract) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();
            if (id != contract.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(contract);
                    cache.SetContracts();
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!ContractExists(contract.Id)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            if (id == null) {
                return NotFound();
            }

            var contract = cache.GetContracts().FirstOrDefault(m => m.Id == id);
            if (contract == null) {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            var contract = cache.GetContracts().FirstOrDefault(e => e.Id == id);
            if (contract != null) {
                _context.Contracts.Remove(contract);
            }

            await _context.SaveChangesAsync();
            cache.SetContracts();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();
            return (cache.GetContracts()?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
