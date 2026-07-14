using System.Collections.Generic;

namespace TrabalhoFinalDWEB2026.Models
{
    public class Medicamento
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Weight { get; set; }

        public ICollection<ReceitaMedicamento> ReceitaMedicamentos { get; set; } = new List<ReceitaMedicamento>();
    }
}