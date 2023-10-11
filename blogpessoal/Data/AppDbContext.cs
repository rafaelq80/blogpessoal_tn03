using blogpessoal.Configuration;
using blogpessoal.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace blogpessoal.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Postagem>().ToTable("tb_postagens");
            modelBuilder.Entity<Tema>().ToTable("tb_temas");
            modelBuilder.Entity<User>().ToTable("tb_usuarios");

            modelBuilder.Entity<Postagem>()
                .HasOne(p => p.Tema)
                .WithMany(t => t.Postagem)
                .HasForeignKey("TemaId")
                .OnDelete(DeleteBehavior.Cascade);

             modelBuilder.Entity<Postagem>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Postagem)
                .HasForeignKey("UsuarioId")
                .OnDelete(DeleteBehavior.Cascade);

        }

        // Registrar DbSet - Objeto responsável por manipular a Tabela

        public DbSet<Postagem> Postagens { get; set; } = null!;
        public DbSet<Tema> Temas { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var brasilia = TimeZoneInfo.FindSystemTimeZoneById("Brazil/East");

            var insertedEntries = this.ChangeTracker.Entries()
                                   .Where(x => x.State == EntityState.Added)
                                   .Select(x => x.Entity);

            foreach (var insertedEntry in insertedEntries)
            {
                //Se uma propriedade da Classe Auditable estiver sendo criada. 
                if (insertedEntry is Auditable auditableEntity)
                {
                    auditableEntity.Data = new DateTimeOffset(DateTime.Now, new TimeSpan(-3,0,0)).ToLocalTime();
                }
            }

            var modifiedEntries = ChangeTracker.Entries()
                       .Where(x => x.State == EntityState.Modified)
                       .Select(x => x.Entity);

            foreach (var modifiedEntry in modifiedEntries)
            {
                //Se uma propriedade da Classe Auditable estiver sendo atualizada.  
                if (modifiedEntry is Auditable auditableEntity)
                {
                    auditableEntity.Data = new DateTimeOffset(DateTime.Now, new TimeSpan(-3, 0, 0)).ToLocalTime();
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        // Ajusta a Data para o formato UTC - Compatível com qualquer Banco de dados Relacional
        //protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        //{
        //    configurationBuilder
        //        .Properties<DateTimeOffset>()
        //        .HaveConversion<DateTimeOffsetConverter>();
        //}

    }
}
