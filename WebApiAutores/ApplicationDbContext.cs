using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    public class ApplicationDbContext : DbContext
    {
        //Generamos un constructor para pasar el connection string
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
