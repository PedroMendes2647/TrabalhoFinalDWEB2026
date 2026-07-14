using System;

namespace TrabalhoFinalDWEB2026.Models
{
    public class Receita
    {
        public int Id { get; set; }

        public int MyUserId { get; set; }
        public MyUser? MyUser { get; set; }

        public int DoctorId { get; set; }
        public Doutor? Doctor { get; set; }

        public ICollection<ReceitaMedicamento> ReceitaMedicamentos { get; set; } = new List<ReceitaMedicamento>();

        public DateTime Date { get; set; }
    }
}