using Core.Extensions;
using MES.Middlewares;
using StationManager.API.Configures;
using StationManager.API.Infrastructure.JobSchedulers;

namespace MES.API.Extensions
{
    public static class ApplicationExtensions
    {
        private static string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public static void UseInfrastructure(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
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
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "StationManager - API");
                });
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            HangfireConfig.HangfireMiddleware(app);

            TokenExtensions.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            HangfireJob.Run(app.ApplicationServices.GetRequiredService<IServiceProvider>());
        }
    }
}
