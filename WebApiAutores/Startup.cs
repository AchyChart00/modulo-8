using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Controllers;
using WebApiAutores.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApiAutores.Filtros;
using Microsoft.AspNetCore.Identity;

namespace WebApiAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            
            // Add services to the container.
            services.AddControllers(opciones =>
                {
                    opciones.Filters.Add(typeof(FiltroDeExcepcion));
                })
                .AddJsonOptions(
                    x=> x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
                    ).AddNewtonsoftJson();

            //Agregamos la configuración de nuestro DBContext
            services.AddDbContext<ApplicationDbContext>(opt=>
            opt.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddAutoMapper(typeof(Startup));
            //Configuración del Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseLoguearRespuestaHTTP(); 
            
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); 
            });
        }
    }
}
