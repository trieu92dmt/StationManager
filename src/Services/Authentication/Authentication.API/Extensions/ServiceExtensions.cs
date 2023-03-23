using Core.Commons;
using Core.Identity;
using Infrastructure.Commons;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Jwt;
using System.Text;


namespace Authentication.API.Extensions
{
    public static class ServiceExtensions
    {
        private static string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection(nameof(JwtSettings))
                .Get<JwtSettings>();

            services.AddSingleton(jwtSettings);
            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
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

            services.AddDbContext<EntityDataContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    builder => builder.MigrationsAssembly(typeof(EntityDataContext).Assembly.FullName));
            });

            services.AddScoped<EntityDataContext>();
            //services.AddTransient<JwtSettings>();
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<ITokenService, TokenService>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            var setting = services.GetOptions<JwtSettings>(nameof(JwtSettings));
            if (setting == null || string.IsNullOrEmpty(setting.IssuerSigningKey))
                throw new ArgumentNullException($"{nameof(JwtSettings)} is not configured propely.");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.IssuerSigningKey));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = false
            };
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = tokenValidationParameters;

            });

            return services;
        }
    }
}
