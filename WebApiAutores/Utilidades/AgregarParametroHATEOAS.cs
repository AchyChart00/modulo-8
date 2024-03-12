using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace WebApiAutores.Utilidades
{
    public class AgregarParametroHATEOAS : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //Agregamos este filtro para evitar que el parametro de HATEOAS aparezca en todos los endpoints en SWAGGER
            if (context.ApiDescription.HttpMethod != "GET")
            {
                return;
            }

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter{
                Name = "IncluirHATEOAS",
                In = ParameterLocation.Header,
                Required = false,    
            });
        }
    }
}
