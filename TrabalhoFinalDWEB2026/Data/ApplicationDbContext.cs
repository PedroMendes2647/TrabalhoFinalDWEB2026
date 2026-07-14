using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Data
{
    public class ApplicationDbContext : IdentityDbContext<Utente, IdentityRole<string>, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Utente> Utentes { get; set; }
        public DbSet<Doutor> Doutores { get; set; }
        public DbSet<Farmaceuta> Farmaceutas { get; set; }
        public DbSet<Medicamentos> Medicamentos { get; set; }
        public DbSet<Receita> Receitas { get; set; }
        public DbSet<ReceitaMedicamentos> ReceitaMedicamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Utente>()
                .HasIndex(u => u.NumeroUtente)
                .IsUnique();

            // Table per Hierarchy (TPH) is default, but ensuring uniqueness if required for the future
            modelBuilder.Entity<Utente>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Utente>("Utente")
                .HasValue<Doutor>("Doutor")
                .HasValue<Farmaceuta>("Farmaceuta");

            modelBuilder.Entity<Receita>()
                .HasOne(r => r.Utente)
                .WithMany(u => u.Receitas)
                .HasForeignKey(r => r.UtenteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Receita>()
                .HasOne(r => r.Doutor)
                .WithMany(u => u.ReceitasGiven)
                .HasForeignKey(r => r.DoutorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReceitaMedicamentos>()
                .HasKey(rm => new { rm.ReceitaId, rm.MedicamentoId });
        }
    }
}