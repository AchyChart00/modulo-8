using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using WebApiAutores.DTOs;

namespace WebApiAutores.Servicios
{
    public class GeneradorEnlaces
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IActionContextAccessor actionContextAccessor;

        public IHttpContextAccessor HttpContextAccessor { get; }

        public GeneradorEnlaces(
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            HttpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper ConstruirURLHelper()
        {
            var factoria = HttpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factoria.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        private async Task<bool> EsAdmin()
        {
            var httpContext = HttpContextAccessor.HttpContext;  
            var resultado =  await authorizationService.AuthorizeAsync(httpContext.User, "esAdmin");
            return resultado.Succeeded;
        }
        public async Task GenerarEnlaces(AutorDTO autorDTO)
        {
            var esAdmin = await EsAdmin();
            var Url = ConstruirURLHelper();
            
            autorDTO.Enlaces.Add(new DatosHATEOAS(
                enlace: Url.Link("ObtenerAutor", new { id = autorDTO.Id }),
                descripcion: "self",
                metodo: "GET"));

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
    }
}
