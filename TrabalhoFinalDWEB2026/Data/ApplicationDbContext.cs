using Microsoft.EntityFrameworkCore;
using TrabalhoFinalDWEB2026.Models;

namespace TrabalhoFinalDWEB2026.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MyUser> MyUsers { get; set; }
        public DbSet<Doutor> Doutores { get; set; }
        public DbSet<Farmaceuta> Farmaceutas { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }
        public DbSet<Receita> Receitas { get; set; }
        public DbSet<ReceitaMedicamento> ReceitaMedicamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table per Hierarchy (TPH) is default, but ensuring uniqueness if required for the future
            modelBuilder.Entity<MyUser>()
                .HasDiscriminator<string>("UserType")
                .HasValue<MyUser>("MyUser")
                .HasValue<Doutor>("Doutor")
                .HasValue<Farmaceuta>("Farmaceuta");

            modelBuilder.Entity<Receita>()
                .HasOne(r => r.MyUser)
                .WithMany(u => u.Receitas)
                .HasForeignKey(r => r.MyUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Receita>()
                .HasOne(r => r.Doctor)
                .WithMany(u => u.ReceitasGiven)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReceitaMedicamento>()
                .HasKey(rm => new { rm.ReceitaId, rm.MedicamentoId });
        }
    }
}