namespace lab5.Models {
    public class InsuranceAgent {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string MiddleName { get; set; } = null!;
        public int AgentTypeId { get; set; }
        public decimal Salary { get; set; }
        public int ContractId { get; set; }
        public double TransactionPercent { get; set; }
        public virtual AgentType AgentType { get; set; } = null!;
        public virtual Contract Contract { get; set; } = null!;
    }
}
