﻿using AutoMapper;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService authorizationService;

        public IConfiguration Configuration { get; }

        public AutoresController(
            ApplicationDbContext context, 
            IMapper mapper, 
            IConfiguration configuration,
            IAuthorizationService authorizationService)
        {
            this.context = context;
            _mapper = mapper;
            Configuration = configuration;
            this.authorizationService = authorizationService;
        }
        
        [HttpGet(Name = "ObtenerAutores")]// api/autores
        [AllowAnonymous]//AllowAnonymous permite consumir el API sin Token
        public async Task<IActionResult> Get([FromQuery] bool incluirHATEOAS = true)
        {
            var autores = await context.Autores.ToListAsync();
            var dtos =  _mapper.Map<List<AutorDTO>>(autores);
            
            if (incluirHATEOAS)
            {
                var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

                dtos.ForEach(dto => GenerarEnlaces(dto, esAdmin.Succeeded));
                
                var resultado = new ColeccionDeRecursos<AutorDTO> { Valores = dtos };
                resultado.Enlaces.Add(new DatosHATEOAS(
                    enlace: Url.Link("obtenerAutores", new { }),
                    descripcion: "self",
                    metodo: "GET"
                    ));

                if (esAdmin.Succeeded)
                {
                    resultado.Enlaces.Add(new DatosHATEOAS(
                    enlace: Url.Link("crearAutor", new { }),
                    descripcion: "crear-autor",
                    metodo: "POST"
                    ));
                }

                return Ok(resultado);   
            }

            return Ok(dtos);
        }

        [HttpGet("{id:int}", Name = "ObtenerAutor")]
        [AllowAnonymous]
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

            var dto = _mapper.Map<AutorDTOConLibros>(autor);
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            GenerarEnlaces(dto, esAdmin.Succeeded);    
            return dto;
        }
        
        private void GenerarEnlaces(AutorDTO autorDTO, bool esAdmin)
        {
            
            autorDTO.Enlaces.Add(new DatosHATEOAS(
                enlace: Url.Link("ObtenerAutor",  new {id = autorDTO.Id }), 
                descripcion:"self", 
                metodo:"GET"));

            if (esAdmin)
            {
                autorDTO.Enlaces.Add(new DatosHATEOAS(
                enlace: Url.Link("ActualizarAutor", new { id = autorDTO.Id }),
                descripcion: "autor-actualizar",
                metodo: "PUT"));

                autorDTO.Enlaces.Add(new DatosHATEOAS(
                    enlace: Url.Link("BorrarAutor", new { id = autorDTO.Id }),
                    descripcion: "self",
                    metodo: "DELETE"));
            }

        }

        [HttpGet("{nombre}", Name ="ObtenerAutorPorNombre")] // api/autores/{id}   api/autores/1
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute]string nombre)
        {
            var autores =  await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

            return _mapper.Map<List<AutorDTO>>(autores);
        }
        
        [HttpPost(Name = "CrearAutor")]
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
        [HttpPut("{id:int}", Name = "ActualizarAutor")]// api/autores/1
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

        [HttpDelete("{id:int}", Name = "BorrarAutor")]//api/autores/2
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
