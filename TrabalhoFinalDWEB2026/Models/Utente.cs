using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TrabalhoFinalDWEB2026.Models
{
    public class Utente : IdentityUser<string>
    {
   
        [System.ComponentModel.DataAnnotations.StringLength(9, MinimumLength = 9)]
        public string NumeroUtente { get; set; } = string.Empty;

        public string Nome { get; set; } = string.Empty;

        public System.DateTime DataNascimento { get; set; }

        public ICollection<Receita> Receitas { get; set; } = new List<Receita>();
    }
}