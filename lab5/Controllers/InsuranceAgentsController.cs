using lab5.Data;
using lab5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace lab5.Controllers {
    [Authorize]
    public class InsuranceAgentsController : Controller {
        private readonly InsuranceCompanyContext _context;

        public InsuranceAgentsController(InsuranceCompanyContext context) {
            _context = context;
        }

        // GET: InsuranceAgents
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10) {
            var insuranceAgents = GetInsuranceAgentsCookies();
            ViewData["ItemsCount"] = insuranceAgents.Count();
            insuranceAgents = insuranceAgents.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            return View(insuranceAgents);
        }

        [HttpPost]
        public async Task<IActionResult> Index(InsuranceAgent insuranceAgent, string type, string responsibilities, int page = 1, int pageSize = 10) {
            page = 1;
            var insuranceAgents = SetInsuranceAgentsCookies(insuranceAgent, type, responsibilities);
            ViewData["ItemsCount"] = insuranceAgents.Count();
            insuranceAgents = insuranceAgents.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            return View(insuranceAgents);
        }

        private IEnumerable<InsuranceAgent> GetInsuranceAgentsCookies() {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();
            IEnumerable<InsuranceAgent> insuranceAgents = cache.GetInsuranceAgents();

            if (HttpContext.Request.Cookies.ContainsKey("InsuranceAgentName")) {
                var name = HttpContext.Request.Cookies["InsuranceAgentName"];
                ViewData["InsuranceAgentName"] = name;
                insuranceAgents = insuranceAgents.Where(e => name == "" || e.Name == name);
            }

            if (HttpContext.Request.Cookies.ContainsKey("InsuranceAgentSurname")) {
                var surname = HttpContext.Request.Cookies["InsuranceAgentSurname"];
                ViewData["InsuranceAgentSurname"] = surname;
                insuranceAgents = insuranceAgents.Where(e => surname == "" || e.Surname == surname);
            }

            if (HttpContext.Request.Cookies.ContainsKey("InsuranceAgentMiddleName")) {
                var middleName = HttpContext.Request.Cookies["InsuranceAgentMiddleName"];
                ViewData["InsuranceAgentMiddleName"] = middleName;
                insuranceAgents = insuranceAgents.Where(e => middleName == "" || e.MiddleName == middleName);
            }

            if (HttpContext.Request.Cookies.ContainsKey("InsuranceAgentSalary")) {
                var salary = HttpContext.Request.Cookies["InsuranceAgentSalary"];
                ViewData["InsuranceAgentSalary"] = salary;
                if (salary != "") {
                    var salaryDecimal = Decimal.Parse(salary);
                    insuranceAgents = insuranceAgents.Where(e => e.Salary >= salaryDecimal);
                }
            }

            if (HttpContext.Request.Cookies.ContainsKey("InsuranceAgentTransactionPercent")) {
                var transactionPercent = HttpContext.Request.Cookies["InsuranceAgentTransactionPercent"];
                ViewData["InsuranceAgentTransactionPercent"] = transactionPercent;
                if (transactionPercent != "") {
                    var transactionPercentDouble = Double.Parse(transactionPercent);
                    insuranceAgents = insuranceAgents.Where(e => e.TransactionPercent >= transactionPercentDouble);
                }
            }

            if (HttpContext.Request.Cookies.ContainsKey("InsuranceAgentType")) {
                var type = HttpContext.Request.Cookies["InsuranceAgentType"];
                ViewData["InsuranceAgentType"] = type;
                insuranceAgents = insuranceAgents.Where(e => type == "" || e.AgentType.Type == type);
            }

            if (HttpContext.Request.Cookies.ContainsKey("InsuranceAgentResponsibilities")) {
                var responsibilities = HttpContext.Request.Cookies["InsuranceAgentResponsibilities"];
                ViewData["InsuranceAgentResponsibilities"] = responsibilities;
                insuranceAgents = insuranceAgents.Where(e => responsibilities == "" || e.Contract.Responsibilities == responsibilities);
            }

            return insuranceAgents;
        }

        private IEnumerable<InsuranceAgent> SetInsuranceAgentsCookies(InsuranceAgent insuranceAgent, string type, string responsibilities) {
            ViewData["InsuranceAgentName"] = insuranceAgent.Name;
            ViewData["InsuranceAgentSurname"] = insuranceAgent.Surname;
            ViewData["InsuranceAgentMiddleName"] = insuranceAgent.MiddleName;
            ViewData["InsuranceAgentSalary"] = insuranceAgent.Salary;
            ViewData["InsuranceAgentTransactionPercent"] = insuranceAgent.TransactionPercent;
            ViewData["InsuranceAgentType"] = type;
            ViewData["InsuranceAgentResponsibilities"] = responsibilities;


            HttpContext.Response.Cookies.Append("InsuranceAgentName", insuranceAgent.Name == null ? "" : insuranceAgent.Name);
            HttpContext.Response.Cookies.Append("InsuranceAgentSurname", insuranceAgent.Surname == null ? "" : insuranceAgent.Surname);
            HttpContext.Response.Cookies.Append("InsuranceAgentMiddleName", insuranceAgent.MiddleName == null ? "" : insuranceAgent.MiddleName);
            HttpContext.Response.Cookies.Append("InsuranceAgentType", type == null ? "" : type);
            HttpContext.Response.Cookies.Append("InsuranceAgentResponsibilities", responsibilities == null ? "" : responsibilities);
            HttpContext.Response.Cookies.Append("InsuranceAgentSalary", insuranceAgent.Salary == default ? "" : insuranceAgent.Salary.ToString());
            HttpContext.Response.Cookies.Append("InsuranceAgentTransactionPercent", insuranceAgent.TransactionPercent == default ? "" : insuranceAgent.TransactionPercent.ToString());

            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();
            IEnumerable<InsuranceAgent> insuranceAgents = cache.GetInsuranceAgents();

            if (insuranceAgent.Name != null)
                insuranceAgents = insuranceAgents.Where(e => e.Name == insuranceAgent.Name);

            if (insuranceAgent.Surname != null)
                insuranceAgents = insuranceAgents.Where(e => e.Surname == insuranceAgent.Surname);

            if (insuranceAgent.MiddleName != null)
                insuranceAgents = insuranceAgents.Where(e => e.MiddleName == insuranceAgent.MiddleName);

            if (type != null)
                insuranceAgents = insuranceAgents.Where(e => e.AgentType.Type == type);

            if (responsibilities != null)
                insuranceAgents = insuranceAgents.Where(e => e.Contract.Responsibilities == responsibilities);

            if (insuranceAgent.Salary != default)
                insuranceAgents = insuranceAgents.Where(e => e.Salary >= insuranceAgent.Salary);

            if (insuranceAgent.TransactionPercent != default)
                insuranceAgents = insuranceAgents.Where(e => e.TransactionPercent >= insuranceAgent.TransactionPercent);

            return insuranceAgents;
        }

        // GET: InsuranceAgents/Details/5
        public async Task<IActionResult> Details(int? id) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();
            if (id == null) {
                return NotFound();
            }

            var insuranceAgent = cache.GetInsuranceAgents().FirstOrDefault(m => m.Id == id);
            if (insuranceAgent == null) {
                return NotFound();
            }

            return View(insuranceAgent);
        }

        // GET: InsuranceAgents/Create
        public IActionResult Create() {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();
            ViewData["AgentTypeId"] = new SelectList(cache.GetAgentTypes(), "Id", "Type");
            ViewData["ContractId"] = new SelectList(cache.GetContracts(), "Id", "Responsibilities");
            return View();
        }

        // POST: InsuranceAgents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,MiddleName,AgentTypeId,Salary,ContractId,TransactionPercent")] InsuranceAgent insuranceAgent) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            if (ModelState.ErrorCount <= 2) {
                _context.Add(insuranceAgent);
                await _context.SaveChangesAsync();
                cache.SetInsuranceAgents();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AgentTypeId"] = new SelectList(cache.GetAgentTypes(), "Id", "Type", insuranceAgent.AgentTypeId);
            ViewData["ContractId"] = new SelectList(cache.GetContracts(), "Id", "Responsibilities", insuranceAgent.ContractId);
            return View(insuranceAgent);
        }

        // GET: InsuranceAgents/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            if (id == null) {
                return NotFound();
            }

            var insuranceAgent = cache.GetInsuranceAgents().FirstOrDefault(e => e.Id == id);
            if (insuranceAgent == null) {
                return NotFound();
            }
            ViewData["AgentTypeId"] = new SelectList(cache.GetAgentTypes(), "Id", "Type", insuranceAgent.AgentTypeId);
            ViewData["ContractId"] = new SelectList(cache.GetContracts(), "Id", "Responsibilities", insuranceAgent.ContractId);
            return View(insuranceAgent);
        }

        // POST: InsuranceAgents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,MiddleName,AgentTypeId,Salary,ContractId,TransactionPercent")] InsuranceAgent insuranceAgent) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            if (id != insuranceAgent.Id) {
                return NotFound();
            }

            if (ModelState.ErrorCount <= 2) {
                try {
                    _context.Update(insuranceAgent);
                    await _context.SaveChangesAsync();
                    cache.SetInsuranceAgents();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!InsuranceAgentExists(insuranceAgent.Id)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AgentTypeId"] = new SelectList(cache.GetAgentTypes(), "Id", "Type", insuranceAgent.AgentTypeId);
            ViewData["ContractId"] = new SelectList(cache.GetContracts(), "Id", "Responsibilities", insuranceAgent.ContractId);
            return View(insuranceAgent);
        }

        // GET: InsuranceAgents/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            if (id == null) {
                return NotFound();
            }

            var insuranceAgent = cache.GetInsuranceAgents().FirstOrDefault(m => m.Id == id);
            if (insuranceAgent == null) {
                return NotFound();
            }

            return View(insuranceAgent);
        }

        // POST: InsuranceAgents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();

            var insuranceAgent = cache.GetInsuranceAgents().FirstOrDefault(e => e.Id == id);
            if (insuranceAgent != null) {
                _context.InsuranceAgents.Remove(insuranceAgent);
            }

            await _context.SaveChangesAsync();
            cache.SetInsuranceAgents();
            return RedirectToAction(nameof(Index));
        }

        private bool InsuranceAgentExists(int id) {
            var cache = HttpContext.RequestServices.GetService<InsuranceCompanyCache>();
            return (cache.GetInsuranceAgents()?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
