using Hangfire;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace StationManager.API.Infrastructure.JobSchedulers.CheckExpireCarCompany
{
    public class CheckExpireCarCompanyScheduler
    {
        public static async Task Invoke(IServiceProvider serviceProvider)
        {
            //Create scope context
            using var scope = serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<EntityDataContext>();

            //Check trạng thái run job
            var settingJob = await dbContext.SettingJob.SingleOrDefaultAsync(x => x.JobName == "CheckExpire");
            if (settingJob?.IsRun == false) return;

            var service = new CheckExpireCarCompanyService(serviceProvider);

            try
            {
                //Xóa job nếu tồn tại trước khi thêm mới
                RecurringJob.RemoveIfExists("CheckExpire");
                //Thêm mới job và run
                RecurringJob.AddOrUpdate(
                    "CheckExpire",
                    () => service.Run(),
                    //1 ngày chạy 1 lần
                    "0 0 * * *");
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
