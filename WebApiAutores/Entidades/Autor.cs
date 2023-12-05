using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    //Esta entidad corresponde con una tabla de la base de datos
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength:120, ErrorMessage="Este campo {0} no debe de tener más de {1} caracteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        public List<AutorLibro> AutoresLibros { get; set; }
    }
}
