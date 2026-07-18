using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TrabalhoFinalDWEB2026.Models
{
    public class Utente : IdentityUser<string>
    {

        /// <summary>
        /// número de utente do SNS (Serviço Nacional de Saúde) do paciente/utente
        /// </summary>
        [System.ComponentModel.DataAnnotations.StringLength(9, MinimumLength = 9)]
        public string NumeroUtente { get; set; } = string.Empty;

        /// <summary>
        /// Nome completo do paciente/utente
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Data de nascimento do paciente/utente
        /// </summary>
        public System.DateTime DataNascimento { get; set; }

        /// <summary>
        /// Relação entre utentes e receitas médicas, onde um utente pode ter várias receitas médicas associadas a ele
        /// </summary>
        public ICollection<Receita> Receitas { get; set; } = new List<Receita>();
    }
}