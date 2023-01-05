using FluentValidation.AspNetCore;
using ISD.Core.Attributes;
using ISD.Core.Extensions;
using ISD.Core.Jwt;
using ISD.Infrastructure.Data;
using MediatR;
using MES.API.Configures;
using MES.Middlewares;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

//CORS
var origins = builder.Configuration.GetValue<string>("AllowedOrigins").Split(";");
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader();
                      });
});

builder.Services.AddControllers();
builder.Services.AddJWTTokenServices(builder.Configuration);

//MediatR
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

// Swagger Config
SwaggerConfig.Configure(builder.Services, builder.Configuration);

//Common config
CommonConfig.Configure(builder.Services, builder.Configuration);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JsonWebTokenKeys"));
builder.Services.AddControllers(config =>
{
    config.Filters.Add(new ValidateModelAttribute());
});

builder.Services.AddHttpContextAccessor();

//Global filter
builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
}).AddFluentValidation(s =>
{
    s.RegisterValidatorsFromAssemblyContaining<Program>();
}).ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = CustomFluentResponse.FluentValidationResponse;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EntityDataContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddApiVersioning();

// ---------------------------------------------------------------

var app = builder.Build();

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

app.MapControllers();

app.Run();