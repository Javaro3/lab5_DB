using lab5.Models;

namespace lab5.Data {
    public static class DbInitializer {
        public static void Initialize(InsuranceCompanyContext db) {
            db.Database.EnsureCreated();

            if (!db.AgentTypes.Any()) {
                InitializeAgentTypes(db);
            }

            if (!db.Contracts.Any()) {
                InitializeContracts(db);
            }

            if (!db.InsuranceAgents.Any()) {
                InitializeInsuranceAgents(db);
            }
        }

        private static void InitializeAgentTypes(InsuranceCompanyContext db) {
            db.AgentTypes.AddRange(
                new AgentType() { Type = "Штатный работник" },
                new AgentType() { Type = "Совместитель" });
            db.SaveChanges();
        }

        private static void InitializeContracts(InsuranceCompanyContext db) {
            Random rand = new();
            var responsibilities = new List<string>() {
                "Страховой агент",
                "Актюарий",
                "Агент по обслуживанию клиентов",
                "Менеджер по продажам",
                "Администратор страховых полисов",
                "Эксперт по риску",
                "Управляющий отделом страхования",
                "Аналитик по страхованию",
                "Администратор базы данных страхования"
            };

            for (int i = 0; i < 100; i++) {
                db.Contracts.Add(
                    new Contract() {
                        Responsibilities = responsibilities[rand.Next(0, responsibilities.Count)],
                        StartDeadline = rand.NextDate(),
                        EndDeadline = rand.NextDate(),
                    });
            }

            db.SaveChanges();
        }

        private static void InitializeInsuranceAgents(InsuranceCompanyContext db) {
            Random rand = new();
            var names = new List<string>() {
                "Иван",
                "Анна",
                "Петр",
                "Екатерина",
                "Александр",
                "София",
                "Михаил",
                "Елена",
                "Дмитрий",
                "Мария"
            };

            var surnames = new List<string>() {
                "Иванов",
                "Петров",
                "Сидоров",
                "Смирнов",
                "Козлов",
                "Михайлов",
                "Александров",
                "Егоров",
                "Васильев",
                "Кузнецов"
            };

            var middleNames = new List<string>() {
                "Иванович",
                "Петрович",
                "Сидорович",
                "Михайлович",
                "Александрович",
                "Дмитриевич",
                "Сергеевич",
                "Андреевич",
                "Николаевич",
                "Геннадьевич"
            };

            for (int i = 0; i < 100; i++) {
                db.InsuranceAgents.Add(
                    new InsuranceAgent() {
                        Name = names[rand.Next(0, names.Count)],
                        Surname = surnames[rand.Next(0, surnames.Count)],
                        MiddleName = middleNames[rand.Next(0, middleNames.Count)],
                        AgentTypeId = rand.Next(1, 3),
                        ContractId = rand.Next(1, 101),
                        Salary = (decimal)(100 * rand.NextDouble()),
                        TransactionPercent = rand.NextDouble()
                    });
            }

            db.SaveChanges();
        }

        private static DateTime NextDate(this Random rand) {
            var day = rand.Next(1, 29);
            var month = rand.Next(1, 13);
            var year = rand.Next(2010, 2022);
            return new DateTime(year, month, day);
        }
    }

}
