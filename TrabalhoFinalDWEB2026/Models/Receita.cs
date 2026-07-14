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

        /* *****************************************************
         ************* relações entre entidades N-1 ************
         ***************************************************** */

        /// <summary>
        /// FK para o Utente/Paciente que vai receber a medicação (Classe Base)
        /// </summary>
        [Required]
        [ForeignKey("MyUser")]
        [DisplayName("Paciente/Utente")]
        public int MyUserId { get; set; }
        public virtual MyUser? MyUser { get; set; }

        /// <summary>
        /// FK para o Médico que prescreveu a receita
        /// </summary>
        [Required]
        [ForeignKey("Doutor")]
        [DisplayName("Médico Prescritor")]
        public int DoctorId { get; set; }
        public virtual Doutor? Doctor { get; set; } 

        /// <summary>
        /// FK para o Farmacêutico que efetuou o aviamento
        /// </summary>
        [ForeignKey("Farmaceuta")]
        [DisplayName("Farmacêutico Responsável")]
        public int? FarmaceutaId { get; set; }
        public virtual Farmaceuta? Farmaceuta { get; set; } 

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