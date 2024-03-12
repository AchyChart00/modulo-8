using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using System.Collections.Generic;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper _mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            _mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libros = await context.Libros.Include(libroBD => libroBD.Comentarios)
                .Include(libroDB => libroDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libros == null)
            {
                return NotFound();
            }


            libros.AutoresLibros = libros.AutoresLibros.OrderBy(x => x.Orden).ToList();
            return _mapper.Map<LibroDTOConAutores>(libros);
        }

        [HttpPost(Name = "CrearLibro")]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds == null)
            {
                return BadRequest("No se puede craer un libro sin autores");
            }
            var autoresIds = await context.Autores
                .Where(autorBD =>
                    libroCreacionDTO.AutoresIds.Contains(autorBD.Id)
                )
                .Select(x => x.Id).ToListAsync();

            if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = _mapper.Map<Libro>(libroCreacionDTO);

            AsignarOrdenAutores(libro);


            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = _mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroDTO);

        }

        [HttpPut("{id:int}", Name = "ActualizarLibro")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDB = await context.Libros
                .Include(x => x.AutoresLibros)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null)
            {
                return NotFound();
            }

            libroDB = _mapper.Map(libroCreacionDTO, libroDB);
            AsignarOrdenAutores(libroDB);

            await context.SaveChangesAsync();

            return NoContent();
        }

        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }

        [HttpPatch("{id:int}", Name = "PatchLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var libroDB = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null)
            {
                return NotFound();
            }

            var libroDTO = _mapper.Map<LibroPatchDTO>(libroDB);

            patchDocument.ApplyTo(libroDTO, ModelState);

            var esValido = TryValidateModel(libroDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(libroDTO, libroDB);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "BorrarLibro")]//api/autores/2
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Libros.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Libro() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}

