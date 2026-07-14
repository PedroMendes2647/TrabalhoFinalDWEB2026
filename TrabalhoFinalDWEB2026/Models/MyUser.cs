using System.Collections.Generic;

namespace TrabalhoFinalDWEB2026.Models
{
    public class MyUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public ICollection<Receita> Receitas { get; set; } = new List<Receita>();
    }
}