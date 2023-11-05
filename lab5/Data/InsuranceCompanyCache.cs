using lab5.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace lab5.Data {
    public class InsuranceCompanyCache {
        private IMemoryCache _cache;
        private InsuranceCompanyContext _db;


        public InsuranceCompanyCache(InsuranceCompanyContext db, IMemoryCache memoryCache) {
            _db = db;
            _cache = memoryCache;
        }

        public IEnumerable<AgentType> GetAgentTypes() {
            _cache.TryGetValue("AgentTypes", out IEnumerable<AgentType>? agentTypes);

            if (agentTypes is null) {
                agentTypes = SetAgentTypes();
            }
            return agentTypes;
        }

        public IEnumerable<AgentType> SetAgentTypes() {
            var agentTypes = _db.AgentTypes.ToList();
            _cache.Set("AgentTypes", agentTypes, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(100000)));
            return agentTypes;
        }

        public IEnumerable<Contract> GetContracts() {
            _cache.TryGetValue("Contracts", out IEnumerable<Contract>? contracts);

            if (contracts is null) {
                contracts = SetContracts();
            }
            return contracts;
        }

        public IEnumerable<Contract> SetContracts() {
            var contracts = _db.Contracts.ToList();
            _cache.Set("Contracts", contracts, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(100000)));
            return contracts;
        }

        public IEnumerable<InsuranceAgent> GetInsuranceAgents() {
            _cache.TryGetValue("InsuranceAgents", out IEnumerable<InsuranceAgent>? insuranceAgents);

            if (insuranceAgents is null) {
                insuranceAgents = SetInsuranceAgents();
            }
            return insuranceAgents;
        }

        public IEnumerable<InsuranceAgent> SetInsuranceAgents() {
            var insuranceAgents = _db.InsuranceAgents.Include(e => e.AgentType).Include(e => e.Contract).ToList();
            _cache.Set("InsuranceAgents", insuranceAgents, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(100000)));
            return insuranceAgents;
        }
    }
}
