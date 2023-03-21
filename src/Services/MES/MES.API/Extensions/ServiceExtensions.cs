using Core.Attributes;
using Core.Extensions;
using Core.Jwt;
using DTOs.Configurations;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using MediatR;
using MES.API.Configures;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MES.API.Extensions
{
    public static class ServiceExtensions
    {
        private static string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
     
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //CORS
            var origins = configuration.GetValue<string>("AllowedOrigins").Split(";");
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader();
                                  });
            });

            services.AddControllers();
            services.AddJWTTokenServices(configuration);
            services.AddHttpClient();
            //MediatR
            services.AddMediatR(Assembly.GetExecutingAssembly());

            // Swagger Config
            SwaggerConfig.Configure(services, configuration);

            //Common config
            CommonConfig.Configure(services, configuration);

            services.Configure<JwtSettings>(configuration.GetSection("JsonWebTokenKeys"));
            services.AddControllers(config =>
            {
                config.Filters.Add(new ValidateModelAttribute());
            });

            services.AddHttpContextAccessor();

            //Global filter
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddFluentValidation(s =>
            {
                s.RegisterValidatorsFromAssemblyContaining<Program>();
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = CustomFluentResponse.FluentValidationResponse;
            });

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<EntityDataContext>(options => options.UseSqlServer(connectionString));
            services.AddApiVersioning();

            return services;
        }      
    }
}
