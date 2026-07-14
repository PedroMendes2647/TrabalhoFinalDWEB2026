using System.Collections.Generic;

namespace TrabalhoFinalDWEB2026.Models
{
    public class Doutor : Utente
    {
        public ICollection<Receita> ReceitasGiven { get; set; } = new List<Receita>();
    }
}