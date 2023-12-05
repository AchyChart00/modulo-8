namespace WebApiAutores.Middlewares;



public static class LoguearRespuestaMiddlewareExtensions
{
    public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LoguearRespuestaMiddleware>();
    }
}
public class LoguearRespuestaMiddleware
{
    private readonly RequestDelegate _siguiente;
    private readonly ILogger<LoguearRespuestaMiddleware> _logger;

    public LoguearRespuestaMiddleware(RequestDelegate siguiente, 
        ILogger<LoguearRespuestaMiddleware> logger)
    {
        _siguiente = siguiente;
        _logger = logger;
    }
    
    //Invoke o InvokeAsync
    //Para utilizar
    public async Task InvokeAsync(HttpContext contexto)
    {
        using (var ms = new MemoryStream())
        {
            var cuerpoOriginalRespuesta = contexto.Response.Body;
            contexto.Response.Body = ms;

            await _siguiente(contexto);

            ms.Seek(0, SeekOrigin.Begin);
            string respuesta = new StreamReader(ms).ReadToEnd();
            ms.Seek(0, SeekOrigin.Begin);

            await ms.CopyToAsync(cuerpoOriginalRespuesta);

            contexto.Response.Body = cuerpoOriginalRespuesta;
                    
            _logger.LogInformation(respuesta);
                    
        }
    }
    
}