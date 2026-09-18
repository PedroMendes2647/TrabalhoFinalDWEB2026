using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrabalhoFinalDWEB2026.WebApp.Models {
    /// <summary>
    /// Representa uma receita médica emitida para um utente, associada a um médico e opcionalmente aviada por um farmacêutico
    /// </summary>
    public class Receita {
        public enum State {
            Emitida,
            Aviada,
            Expirada
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Data de Emissão")]
        [DataType(DataType.DateTime)]
        public DateTime DataEmissao { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("Estado da Receita")]
        public State Estado { get; set; } = State.Emitida;

        [DisplayName("Data de Dispensação")]
        [DataType(DataType.DateTime)]
        public DateTime? DataDispensacao { get; set; }

        /* *****************************************************
         ************* relações entre entidades N-1 ************
         ***************************************************** */

        [Required]
        [DisplayName("Paciente/Utente")]
        public string UtenteId { get; set; } = string.Empty;
        public virtual Utente? Utente { get; set; }

        [Required]
        [DisplayName("Doutor Prescritor")]
        public string DoutorId { get; set; } = string.Empty;
        public virtual Utente? DoutorUtente { get; set; }

        [DisplayName("Farmacêutico Responsável")]
        public string? FarmaceutaId { get; set; }
        public virtual Utente? FarmaceutaUtente { get; set; }

        /* *****************************************************
         ************* relações entre entidades M-N ************
         ***************************************************** */

        [DisplayName("Medicamentos Receitados")]
        public ICollection<ReceitaMedicamentos> ListaDeMedicamentos { get; set; } = new List<ReceitaMedicamentos>();
    }
}
