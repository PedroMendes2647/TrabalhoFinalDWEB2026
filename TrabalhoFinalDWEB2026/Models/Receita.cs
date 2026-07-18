using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrabalhoFinalDWEB2026.Models {
    /// <summary>
    /// Representa uma receita médica emitida para um utente, associada a um médico e opcionalmente aviada por um farmacêutico
    /// </summary>
    public class Receita {
        /// <summary>
        /// PK
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Data e hora em que a receita foi criada pelo médico
        /// </summary>
        [Required]
        [DisplayName("Data de Emissão")]
        [DataType(DataType.DateTime)]
        public DateTime DataEmissao { get; set; } = DateTime.Now;

        /// <summary>
        /// Estado atual da receita (ex: "Emitida", "Aviada", "Expirada")
        /// </summary>
        [Required]
        [StringLength(20)]
        [DisplayName("Estado da Receita")]
        public string Estado { get; set; } = "Emitida";

        /// <summary>
        /// Data e hora em que a receita foi aviada pelo farmacêutico
        /// </summary>
        [DisplayName("Data de Dispensação")]
        [DataType(DataType.DateTime)]
        public DateTime? DataDispensacao { get; set; }

        /* *****************************************************
         ************* relações entre entidades N-1 ************
         ***************************************************** */

        /// <summary>
        /// FK para o Utente/Paciente que vai receber a medicação (Classe Base)
        /// </summary>
        [Required]
        [ForeignKey("Utente")]
        [DisplayName("Paciente/Utente")]
        public string UtenteId { get; set; } = string.Empty;
        public virtual Utente? Utente { get; set; }

        /// <summary>
        /// FK para o Médico que prescreveu a receita
        /// </summary>
        [Required]
        [ForeignKey("DoutorUtente")]
        [DisplayName("Doutor Prescritor")]
        public string DoutorId { get; set; } = string.Empty;
        public virtual Utente? DoutorUtente { get; set; }

        /// <summary>
        /// FK para o Farmacêutico que efetuou o aviamento
        /// </summary>
        [ForeignKey("FarmaceutaUtente")]
        [DisplayName("Farmacêuta Responsável")]
        public string? FarmaceutaId { get; set; }
        public virtual Utente? FarmaceutaUtente { get; set; }

        /* *****************************************************
         ************* relações entre entidades M-N ************
         ***************************************************** */

        /// <summary>
        /// Lista de medicamentos incluídos nesta receita médica
        /// </summary>
        [DisplayName("Medicamentos Receitados")]
        public ICollection<ReceitaMedicamentos> ListaDeMedicamentos { get; set; } = [];

        /* **************************************************** */
    }
}