using Common.Logging;
using MES.API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Information("Starting Station Manager API up");
try
{
    builder.Host.UseSerilog(Serilogger.Configure);

    builder.Services.AddInfrastructure(builder.Configuration);

    // ---------------------------------------------------------------

    var app = builder.Build();

    app.UseInfrastructure(app.Environment);

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
    Log.Information("Shut down Station Manager API Complete");
    Log.CloseAndFlush();
}


