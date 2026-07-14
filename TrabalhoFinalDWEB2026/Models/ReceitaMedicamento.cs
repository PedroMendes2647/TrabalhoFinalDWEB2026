using System;

namespace TrabalhoFinalDWEB2026.Models
{
    public class ReceitaMedicamento
    {
        public int ReceitaId { get; set; }
        public Receita? Receita { get; set; }

        public int MedicamentoId { get; set; }
        public Medicamento? Medicamento { get; set; }

        public int Quantity { get; set; }
    }
}