using System.Collections.Generic;

namespace TrabalhoFinalDWEB2026.Models
{
    public class Doutor : Utente
    {
        /// <summary>
        /// Receitas médicas emitidas pelo médico para os utentes/pacientes
        /// </summary>
        public ICollection<Receita> ReceitasGiven { get; set; } = new List<Receita>();
    }
}