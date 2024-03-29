﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatosHATEOAS>>> Get()
        {
            var datosHateoas = new List<DatosHATEOAS>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            datosHateoas.Add(new DatosHATEOAS(
                enlace: Url.Link("ObtenerRoot", new { }),
                descripcion: "self",
                metodo: "GET"));

            datosHateoas.Add(new DatosHATEOAS(
                enlace: Url.Link("ObtenerAutores", new { }),
                descripcion: "Autores",
                metodo: "GET"));

            if (esAdmin.Succeeded)
            {
                datosHateoas.Add(new DatosHATEOAS(
                enlace: Url.Link("CrearAutor", new { }),
                descripcion: "Autor-crear",
                metodo: "POST"));

                datosHateoas.Add(new DatosHATEOAS(
                    enlace: Url.Link("CrearLibro", new { }),
                    descripcion: "libro-crear",
                    metodo: "POST"));
            }

            return datosHateoas;
        }
    }
}
