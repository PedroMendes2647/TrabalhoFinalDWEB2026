using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Data {
    public class ApplicationDbContext : IdentityDbContext<Utente, IdentityRole<string>, string> {
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

            // Configuração de Herança TPH (Table per Hierarchy)
            modelBuilder.Entity<Utente>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Utente>("Utente")
                .HasValue<Doutor>("Doutor")
                .HasValue<Farmaceuta>("Farmaceuta");

            // Relações N:1 da Receita
            modelBuilder.Entity<Receita>()
                .HasOne(r => r.Utente)
                .WithMany(u => u.Receitas)
                .HasForeignKey(r => r.UtenteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Receita>()
                .HasOne(r => r.DoutorUtente)
                .WithMany()
                .HasForeignKey(r => r.DoutorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Receita>()
                .HasOne(r => r.FarmaceutaUtente)
                .WithMany()
                .HasForeignKey(r => r.FarmaceutaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relação M:N (ReceitaMedicamentos)
            modelBuilder.Entity<ReceitaMedicamentos>()
                .HasKey(rm => new { rm.ReceitaId, rm.MedicamentoId });

            modelBuilder.Entity<ReceitaMedicamentos>()
                .HasOne(rm => rm.Receita)
                .WithMany(r => r.ListaDeMedicamentos)
                .HasForeignKey(rm => rm.ReceitaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReceitaMedicamentos>()
                .HasOne(rm => rm.Medicamento)
                .WithMany(m => m.ListaDeReceitas)
                .HasForeignKey(rm => rm.MedicamentoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}