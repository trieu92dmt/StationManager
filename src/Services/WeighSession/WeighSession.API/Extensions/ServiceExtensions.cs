using Core.SeedWork.Repositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using WeighSession.API.Repositories;
using WeighSession.API.Repositories.Interfaces;
using WeighSession.Infrastructure.Models;

namespace WeighSession.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<DataCollectionContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    builder => builder.MigrationsAssembly(typeof(DataCollectionContext).Assembly.FullName));
            });

            services.AddTransient<DataCollectionContext>();
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped<IWeighSessionRepository, WeighSessionRepository>();

            return services;
        }
    }
}
