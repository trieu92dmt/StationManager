using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;

namespace StationManager.API.Configures
{
    public class HangfireConfig
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
                config.UseSimpleAssemblyNameTypeSerializer();
                config.UseRecommendedSerializerSettings();
                try
                {
                    config.UseStorage(
                        new SqlServerStorage(
                            configuration.GetConnectionString("DefaultConnection"),
                            new SqlServerStorageOptions
                            {
                                QueuePollInterval = TimeSpan.FromSeconds(15),
                                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                                PrepareSchemaIfNecessary = true,
                                DashboardJobListLimit = 50000,
                                TransactionTimeout = TimeSpan.FromMinutes(1),
                                SchemaName = "TLGJOB"
                            }
                        ));
                }
                catch
                {
                    config.UseStorage(
                        new SqlServerStorage(
                            configuration.GetConnectionString("DefaultConnection"),
                            new SqlServerStorageOptions
                            {
                                QueuePollInterval = TimeSpan.FromSeconds(15),
                                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                                PrepareSchemaIfNecessary = false,
                                DashboardJobListLimit = 50000,
                                TransactionTimeout = TimeSpan.FromMinutes(1),
                                SchemaName = "TLGJOB"
                            }
                        ));
                }
            });
            services.AddHangfireServer();
        }

        public static void HangfireMiddleware(IApplicationBuilder builder)
        {
            builder.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                IsReadOnlyFunc = (DashboardContext context) => true
            });

            builder.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });

        }
    }
}
