using Core.Extensions;
using MES.Middlewares;

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
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TLG - MES - API");
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

            TokenExtensions.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
        }
    }
}
