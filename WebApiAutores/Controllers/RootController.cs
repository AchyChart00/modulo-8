using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "ObtenerRoot")]
        public ActionResult<IEnumerable<DatosHATEOAS>> Get()
        {
            var datosHateoas = new List<DatosHATEOAS>();

            datosHateoas.Add(new DatosHATEOAS(
                enlace : Url.Link("ObtenerRoot", new { }), 
                descripcion:"self", 
                metodo: "GET"));

            return datosHateoas;
        }
    }
}
