using StationManager.API.Infrastructure.JobSchedulers.CheckExpireCarCompany;

namespace StationManager.API.Infrastructure.JobSchedulers
{
    public static class HangfireJob
    {
        public static async void Run(IServiceProvider serviceProvider)
        {
            await CheckExpireCarCompanyScheduler.Invoke(serviceProvider);
        }
    }
}
