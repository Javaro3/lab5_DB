using lab5.Migrations;
using lab5.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace lab5.Data {
    public class InsuranceCompanyContext : IdentityDbContext {
        public InsuranceCompanyContext() {}

        public InsuranceCompanyContext(DbContextOptions<InsuranceCompanyContext> options) : base(options) { }

        public DbSet<AgentType> AgentTypes { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<InsuranceAgent> InsuranceAgents { get; set; }
    }
}
