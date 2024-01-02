using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController:ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager
            )
        {
            _context = context;
            _mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var existeLibro = await _context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }
            var comentarios = await _context.Comentarios.Where(comentarioDB => comentarioDB.LibroId == libroId).ToListAsync();

            return _mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id:int}", Name = "ObtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetById(int id)
        {
            var comentario = await _context.Comentarios.FirstOrDefaultAsync(comentarioDB=>comentarioDB.Id==id);

            if (comentario ==null)
            {
                return NotFound();
            }

            return _mapper.Map<ComentarioDTO>(comentario);
        }
        
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            //Aquí obtener el email del usuario a traves del JSON WebToken con HttpContext
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;
            var existeLibro = await _context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = _mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;
            comentario.UsuarioId = usuarioId;
            _context.Add(comentario);
            await _context.SaveChangesAsync();

            var comentarioDTO = _mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("ObtenerComentario", new { id = comentario.Id, libroId = libroId }, comentarioDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int libroId, int id, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibro = await _context.Libros.AnyAsync(libro => libro.Id == libroId);

            if (!existeLibro)
            {
                return NotFound($"El libro con el id {libroId} no existe");
            }

            var existeComentario = await _context.Comentarios.AnyAsync(comentario => comentario.Id == id);

            if (!existeComentario)
            {
                return NotFound($"El comentario con el id {id} no existe");
            }
            var comentario = _mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;
            _context.Update(comentario);

            await _context.SaveChangesAsync();

            return NoContent(); 
        }
    
    }
}
