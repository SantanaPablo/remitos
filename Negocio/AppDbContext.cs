using Microsoft.EntityFrameworkCore;
using Dominio;

namespace Negocio
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Remito> Remitos { get; set; }
        public DbSet<ItemRemito> ItemsRemito { get; set; }

        public DbSet<NotaSalida> NotasSalida { get; set; }
        public DbSet<ItemSalida> ItemsSalida { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<ItemRemito>()
                .HasOne(i => i.Remito)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.id_remito)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

           
            modelBuilder.Entity<ItemRemito>()
                .HasIndex(i => new { i.serial, i.id_remito })
                .IsUnique();


            modelBuilder.Entity<NotaSalida>()
                .HasOne(n => n.Autorizante)
                .WithMany()
                .HasForeignKey(n => n.AutorizanteId);

            modelBuilder.Entity<ItemSalida>()
                .HasOne(i => i.NotaSalida)
                .WithMany(n => n.Items)
                .HasForeignKey(i => i.NotaSalidaId);

            base.OnModelCreating(modelBuilder);
        }
    }
}