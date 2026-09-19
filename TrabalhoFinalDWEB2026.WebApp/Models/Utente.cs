using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TrabalhoFinalDWEB2026.WebApp.Models {
    public class Utente : IdentityUser<string> {
        /// <summary>
        /// Número de utente do SNS (Serviço Nacional de Saúde) do paciente/utente
        /// </summary>
        [Required(ErrorMessage = "O número de utente é obrigatório.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "O número de utente deve ter exatamente 9 dígitos.")]
        public string NumeroUtente { get; set; } = string.Empty;

        /// <summary>
        /// Nome completo do paciente/utente
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Data de nascimento do paciente/utente
        /// </summary>
        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public System.DateTime DataNascimento { get; set; }

        /// <summary>
        /// Relação entre utentes e receitas médicas
        /// </summary>
        public ICollection<Receita> Receitas { get; set; } = new List<Receita>();
    }
}
