using Authentication.API.Extensions;
using Common.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

Log.Information("Starting Auth API up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);

    builder.Services.AddControllers();
    builder.Services.AddConfigurationSettings(builder.Configuration);

    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddJwtAuthentication();   
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Station Manager API",
            Version = "v1"
        });


    }); ;
    builder.Services.AddApiVersioning();

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    {
        app.UseDeveloperExceptionPage();
        //app.UseSwaggerAuthorized();
        app.UseSwagger(options =>
        {
            options.SerializeAsV2 = true;
        });
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Station Manager - AUTH - API");
        });
    }

    app.UseRouting();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseCors("_myAllowSpecificOrigins");
    //app.MapControllers();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapDefaultControllerRoute();
    });

    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;

    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down Auth API Complete");
    Log.CloseAndFlush();
}

