using blogpessoal.Configuration;
using blogpessoal.Model;
using Microsoft.EntityFrameworkCore;

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
            var insertedEntries = this.ChangeTracker.Entries()
                                   .Where(x => x.State == EntityState.Added)
                                   .Select(x => x.Entity);

            foreach (var insertedEntry in insertedEntries)
            {
                //Se uma propriedade da Classe Auditable estiver sendo criada. 
                if (insertedEntry is Auditable auditableEntity)
                {
                    auditableEntity.Data = DateTimeOffset.Now;
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
                    auditableEntity.Data = DateTimeOffset.Now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        // Ajusta a Data para o formato UTC - Compatível com o Postgres
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<DateTimeOffset>()
                .HaveConversion<DateTimeOffsetConverter>();
        }

    }
}
