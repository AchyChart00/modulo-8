using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper _mapper;

        public IConfiguration Configuration { get; }

        public AutoresController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            _mapper = mapper;
            Configuration = configuration;
        }
        
        [HttpGet]// api/autores
        [AllowAnonymous]//AllowAnonymous permite consumir el API sin Token
        public async Task<List<AutorDTO>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            return _mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("{id:int}", Name = "ObtenerAutor")]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor =  await context.Autores
                .Include(autorDB=>autorDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Libro)
                .FirstOrDefaultAsync(autorDB => autorDB.Id == id);
            if (autor == null)
            {
                return NotFound();
            }

            return _mapper.Map<AutorDTOConLibros>(autor);
        }
        
        [HttpGet("{nombre}")] // api/autores/{id}   api/autores/1
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute]string nombre)
        {
            var autores =  await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

            return _mapper.Map<List<AutorDTO>>(autores);
        }
        
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]AutorCreacionDTO autorCreacionDto)
        {
            //validación personalizada en el método de nuestro controlador
            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x=> x.Nombre == autorCreacionDto.Nombre);

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDto.Nombre}");
            }

            var autor = _mapper.Map<Autor>(autorCreacionDto);
            context.Add(autor);
            //Para guardar los cambios de manera asincrona
            await context.SaveChangesAsync();

            var autorDTO = _mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("ObtenerAutor", new {id=autor.Id}, autorDTO);
        }
        //se agrega un parametro de ruta por medio de llaves y ponemos que sea un int
        [HttpPut("{id:int}")]// api/autores/1
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            var autor = _mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;
            
            context.Update(autor);
            await context.SaveChangesAsync();
            //return Ok();
            return NoContent();
        }

        [HttpDelete("{id:int}")]//api/autores/2
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor(){Id = id});
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
