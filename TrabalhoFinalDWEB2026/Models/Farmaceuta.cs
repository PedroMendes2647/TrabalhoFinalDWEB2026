namespace TrabalhoFinalDWEB2026.Models
{
    public class Farmaceuta : MyUser
    {
        // Add pharmacist specific properties here if necessary later on.
        public ICollection<Receita> ReceitasAviadas { get; set; } = new List<Receita>();
    }
}