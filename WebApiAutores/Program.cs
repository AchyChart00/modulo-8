using WebApiAutores;

namespace WebApiAutores
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var startup = new Startup(builder.Configuration);

            startup.ConfigureServices(builder.Services);

            var app = builder.Build();

            var servicioLogger = (ILogger<Startup>) app.Services.GetService(typeof(ILogger<Startup>));
            
            startup.Configure(app, app.Environment, servicioLogger);

            app.Run();
        }
    }
}