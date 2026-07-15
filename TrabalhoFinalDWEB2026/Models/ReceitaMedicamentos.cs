using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrabalhoFinalDWEB2026.Models {
    /// <summary>
    /// Tabela de associação (M:N) que liga um medicamento específico a uma receita,
    /// permitindo definir detalhes de ingestão individuais.
    /// </summary>
    public class ReceitaMedicamentos {
        /// <summary>
        /// FK para a Receita (Parte da PK composta)
        /// </summary>
        [Required]
        [ForeignKey("Receita")]
        [DisplayName("Receita")]
        public int ReceitaId { get; set; }
        public virtual Receita? Receita { get; set; }

        /// <summary>
        /// FK para o Medicamento (Parte da PK composta)
        /// </summary>
        [Required]
        [ForeignKey("Medicamento")]
        [DisplayName("Medicamento")]
        public int MedicamentoId { get; set; }
        public virtual Medicamentos? Medicamento { get; set; }

        /* *****************************************************
         ***** Atributos extra específicos desta associação ****
         ***************************************************** */

        /// <summary>
        /// Quantidade de caixas ou unidades deste medicamento a aviar na receita
        /// </summary>
        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Range(1, 10, ErrorMessage = "A quantidade deve ser entre 1 e 10.")]
        [DisplayName("Quantidade")]
        public int Quantidade { get; set; } = 1;

        /// <summary>
        /// Instruções específicas para este medicamento nesta receita (ex: "1 comp. após o pequeno-almoço")
        /// </summary>
        [Required(ErrorMessage = "As instruções de posologia são obrigatórias para o medicamento.")]
        [StringLength(255, ErrorMessage = "A posologia não pode exceder os 255 caracteres.")]
        [DisplayName("Posologia / Modo de Toma")]
        public string Posologia { get; set; } = "";

        /* **************************************************** */
    }
}