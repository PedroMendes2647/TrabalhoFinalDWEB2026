using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Data {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }

        public DbSet<MyUser> MyUsers { get; set; }
        public DbSet<Medicamentos> Medicamentos { get; set; }
        public DbSet<Receita> Receitas { get; set; }
        public DbSet<ReceitaMedicamentos> ReceitaMedicamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // ***************************************************************************
            //  CONFIGURAÇÃO DA HERANÇA TPH (Table per Hierarchy)
            // ***************************************************************************
            modelBuilder.Entity<MyUser>()
                .HasDiscriminator<string>("UserType")
                .HasValue<MyUser>("MyUser")       // Utente/Paciente
                .HasValue<Doutor>("Doutor")       // Médico
                .HasValue<Farmaceuta>("Farmaceuta"); // Farmacêuta

            // ***************************************************************************
            // CONFIGURAÇÃO DAS RELAÇÕES DA RECEITA (N:1)
            // ***************************************************************************

            // Relacionamento: Quem recebe a receita (Paciente/Utente)
            modelBuilder.Entity<Receita>()
                .HasOne(r => r.MyUser)
                .WithMany(u => u.Receitas)
                .HasForeignKey(r => r.MyUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento: Quem passa a receita (Doutor)
            modelBuilder.Entity<Receita>()
                .HasOne(r => r.Doctor)
                .WithMany(d => d.ReceitasGiven)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento: Quem avia a receita (Farmaceuta) 
            modelBuilder.Entity<Receita>()
                .HasOne(r => r.Farmaceuta)
                .WithMany(f => f.ReceitasAviadas)
                .HasForeignKey(r => r.FarmaceutaId)
                .OnDelete(DeleteBehavior.Restrict);

            // ***************************************************************************
            // CONFIGURAÇÃO DA TABELA ReceitaMedicamentos do relacionamento M:N
            // ***************************************************************************

            // Chave Primária Composta (ReceitaId + MedicamentoId)
            modelBuilder.Entity<ReceitaMedicamentos>()
                .HasKey(rm => new { rm.ReceitaId, rm.MedicamentoId });

            // Relação M:N - Lado da Receita
            modelBuilder.Entity<ReceitaMedicamentos>()
                .HasOne(rm => rm.Receita)
                .WithMany(r => r.ListaDeMedicamentos)
                .HasForeignKey(rm => rm.ReceitaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relação M:N - Lado do Medicamento
            modelBuilder.Entity<ReceitaMedicamentos>()
                .HasOne(rm => rm.Medicamento)
                .WithMany(m => m.ListaDeReceitas)
                .HasForeignKey(rm => rm.MedicamentoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}