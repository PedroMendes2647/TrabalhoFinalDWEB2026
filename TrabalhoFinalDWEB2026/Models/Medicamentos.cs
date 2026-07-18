using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrabalhoFinalDWEB2026.Models {
    /// <summary>
    /// Representa um medicamento disponível no sistema (ex: "Paracetamol", "Ibuprofeno", etc.)
    /// </summary>
    public class Medicamentos {

        /// <summary>
        /// PK
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome do medicamento
        /// </summary>
        [Required(ErrorMessage = "O nome do medicamento é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder os 100 caracteres.")]
        [DisplayName("Nome do Medicamento")]
        public string Nome { get; set; } = "";

        /// <summary>
        /// Dosagem do medicamento
        /// </summary>
        [StringLength(50, ErrorMessage = "A dosagem não pode exceder os 50 caracteres.")]
        [DisplayName("Dosagem")]
        public string Dosagem { get; set; } = "";

        /// <summary>
        /// Tipo de medicamento (ex: Comprimido, Injeção, Xarope)
        /// </summary>
        [StringLength(50, ErrorMessage = "O tipo não pode exceder os 50 caracteres.")]
        [DisplayName("Tipo")]
        public string Tipo { get; set; } = "";


        /* *****************************************************
         ************* relações entre entidades M-N ************
         ***************************************************** */

        /// <summary>
        /// Lista de receitas onde este medicamento foi incluído
        /// </summary>
        public ICollection<ReceitaMedicamentos> ListaDeReceitas { get; set; } = [];

        /* **************************************************** */
    }
}