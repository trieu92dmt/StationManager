using FluentValidation.AspNetCore;
using ISD.API.Core;
using ISD.API.Core.Attributes;
using ISD.API.EntityModels.Data;
using ISD.API.Extensions;
using ISD.API.Extensions.Jwt;
using ISD.API.ViewModels;
using ISD.Middlewares;
using ITP_MES_API.Configures;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace MP_CRM_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var origins = Configuration.GetValue<string>("AllowedOrigins").Split(";");
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader();
                                  });
            });

            services.AddControllers();
            services.AddJWTTokenServices(Configuration);

            //MediatR
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);

            // Swagger Config
            SwaggerConfig.Configure(services, Configuration);

            //Common Config
            CommonConfig.Configure(services, Configuration);

            services.Configure<JwtSettings>(Configuration.GetSection("JsonWebTokenKeys"));
            services.AddControllers(config =>
            {
                config.Filters.Add(new ValidateModelAttribute());
            });/*.AddNewtonsoftJson()*/

            services.AddHttpContextAccessor();

            //Global filter
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddFluentValidation(s =>
            {
                s.RegisterValidatorsFromAssemblyContaining<Startup>();
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = CustomFluentResponse.FluentValidationResponse;
            });   

            //services.AddSingleton(new SAPService);

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<EntityDataContext>(options => options.UseSqlServer(connectionString));
            services.AddApiVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor accessor)
        {
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwaggerAuthorized();
                app.UseSwagger(options =>
                {
                    options.SerializeAsV2 = true;
                });
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ISD - ITF - iMes");
                });
            }

            //app.UseExceptionMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseMiddleware<JwtMiddleware>();

            ControllerBaseAPI.SetHttpContextAccessor(accessor);

            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Upload", "Images")),
            //    RequestPath = new PathString("/Images")
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            TokenExtensions.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

        }
    }
}
