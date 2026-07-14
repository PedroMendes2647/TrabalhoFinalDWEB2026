namespace TrabalhoFinalDWEB2026.Models
{
    public class Farmaceuta : Utente
    {
        /// <summary>
        /// Receitas médicas aviadas pelo farmacêutico para os utentes/pacientes
        /// </summary>
        public ICollection<Receita> ReceitasAviadas { get; set; } = new List<Receita>();
    }
}