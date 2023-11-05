namespace lab5.Models {
    public class AgentType {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public virtual ICollection<InsuranceAgent> InsuranceAgents { get; set; } = new List<InsuranceAgent>();
    }
}
