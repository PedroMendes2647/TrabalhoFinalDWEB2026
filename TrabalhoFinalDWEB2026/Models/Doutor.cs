using System.Collections.Generic;

namespace TrabalhoFinalDWEB2026.Models
{
    public class Doutor : MyUser
    {
        public ICollection<Receita> ReceitasGiven { get; set; } = new List<Receita>();
    }
}