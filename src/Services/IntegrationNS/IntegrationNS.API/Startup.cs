﻿using FluentValidation.AspNetCore;
using IntegrationNS.API.Configures;
using ISD.API.Core.Attributes;
using ISD.Core.Extensions;
using ISD.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace IntegrationNS.API
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

            //Swagger Config
            SwaggerConfig.Configure(services, Configuration);

            //Common Config
            CommonConfig.Configure(services, Configuration);

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
                s.RegisterValidatorsFromAssemblyContaining<Startup>();
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = CustomFluentResponse.FluentValidationResponse;
            });

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
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TLG - Integration Netsuite API");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();

            app.UseCors(MyAllowSpecificOrigins);


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}