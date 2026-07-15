using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Data
{
    public class ApplicationDbContext : IdentityDbContext<Utente, IdentityRole<string>, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }

        public DbSet<Utente> Utentes { get; set; }
        public DbSet<Doutor> Doutores { get; set; }
        public DbSet<Farmaceuta> Farmaceutas { get; set; }
        public DbSet<Medicamentos> Medicamentos { get; set; }
        public DbSet<Receita> Receitas { get; set; }
        public DbSet<ReceitaMedicamentos> ReceitaMedicamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Utente>()
                .HasIndex(u => u.NumeroUtente)
                .IsUnique();

            // ***************************************************************************
            //  CONFIGURAÇÃO DA HERANÇA TPH (Table per Hierarchy)
            // ***************************************************************************
            modelBuilder.Entity<Utente>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Utente>("Utente")
                .HasValue<Doutor>("Doutor")
                .HasValue<Farmaceuta>("Farmaceuta");

            // ***************************************************************************
            // CONFIGURAÇÃO DAS RELAÇÕES DA RECEITA (N:1)
            // ***************************************************************************

            // Relacionamento: Quem recebe a receita (Paciente/Utente)
            modelBuilder.Entity<Receita>()
                .HasOne(r => r.Utente)
                .WithMany(u => u.Receitas)
                .HasForeignKey(r => r.UtenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento: Quem passa a receita (Doutor)
            modelBuilder.Entity<Receita>()
                .HasOne(r => r.Doutor)
                .WithMany(u => u.ReceitasGiven)
                .HasForeignKey(r => r.DoutorId)
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