namespace lab5.Models {
    public class Contract {
        public int Id { get; set; }
        public string Responsibilities { get; set; } = null!;
        public DateTime StartDeadline { get; set; }
        public DateTime EndDeadline { get; set; }
        public virtual ICollection<InsuranceAgent> InsuranceAgents { get; set; } = new List<InsuranceAgent>();

    }
}
