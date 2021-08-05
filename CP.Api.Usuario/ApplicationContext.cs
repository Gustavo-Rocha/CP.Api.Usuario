using CP.Api.Usuario.Models;
using Microsoft.EntityFrameworkCore;

namespace CP.Api.Usuario
{
    public class ApplicationContext : DbContext
    {

        public DbSet<Api.Usuario.Models.Usuario> Usuarios { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

        public ApplicationContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=UsuariosDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.Usuario>().HasKey(c => c.Cpf);

        }

    }
}
