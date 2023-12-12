using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    //Para trabajar con Identity debemos de heredar de IdentityDBContext
    public class ApplicationDbContext : IdentityDbContext
    //public class ApplicationDbContext : DbContext
    {
        //Generamos un constructor para pasar el connection string
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Si se sobreescribe el OnModelCreating es importante que se mantenga esta linea de código
            base.OnModelCreating(modelBuilder);

            //Estoy asignando una llave primaria compuesta en AutorLibro,
            //donde su llave primaria compuesta es AutorId Y LibroId
            modelBuilder.Entity<AutorLibro>().
                HasKey(al=> new
                {
                    al.AutorId,
                    al.LibroId
                });
        }

        //de esta forma generamos las tablas, apartir del esquema de la clase
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<AutorLibro> AutoresLibros { get; set; }
    }
}
