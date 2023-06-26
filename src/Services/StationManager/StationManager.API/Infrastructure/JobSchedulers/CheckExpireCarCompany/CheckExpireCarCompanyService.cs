using Core.Extensions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shared.Models;
using System.Net;
using System.Net.Mail;

namespace StationManager.API.Infrastructure.JobSchedulers.CheckExpireCarCompany
{
    public class CheckExpireCarCompanyService
    {
        private readonly IServiceProvider _serverProvider;

        public CheckExpireCarCompanyService(IServiceProvider serverProvider)
        {
            _serverProvider = serverProvider;
        }

        public async Task Run()
        {
            try
            {
                //Create scope db context
                using var scope = _serverProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<EntityDataContext>();

                var email = "trieu.genshin.clone@gmail.com";
                var password = "csjrgwzxhdrgulir";

                
                //Xóa data log
                //Lấy ra các dòng đang được lấy số cân
                var listCarCompany = context.CarCompanyModel.Where(x => x.Actived == true).ToList();

                //Duyệt để update lại trọng lượng cân
                foreach (var item in listCarCompany)
                {
                    MailMessage msg = new MailMessage();

                    msg.From = new MailAddress(email);
                    msg.To.Add(item.Email);
                    msg.Subject = "Thông báo!";
                    

                    var mapping = await context.CarCompany_Package_MappingModel.FirstOrDefaultAsync(x => x.CarCompanyId == item.CarCompanyId);

                    var timeRemain = (mapping.ExpireTime - DateTime.Now).Value.TotalDays;

                    if (timeRemain < 7 && timeRemain >0)
                    {
                        msg.Body = "Tài khoản của bạn sắp hết hạn! Vui lòng thực hiện gia hạn để tiếp tục sử dụng dịch vụ từ hệ thống!";
                        using (SmtpClient client = new SmtpClient())
                        {
                            client.EnableSsl = true;
                            client.UseDefaultCredentials = false;
                            client.Credentials = new NetworkCredential(email, password);
                            client.Host = "smtp.gmail.com";
                            client.Port = 587;
                            client.DeliveryMethod = SmtpDeliveryMethod.Network;

                            client.Send(msg);
                        }
                    }
                    else if (timeRemain < 0)
                    {
                        msg.Body = "Tài khoản của bạn đã bị khóa do hết hạn!";
                        using (SmtpClient client = new SmtpClient())
                        {
                            client.EnableSsl = true;
                            client.UseDefaultCredentials = false;
                            client.Credentials = new NetworkCredential(email, password);
                            client.Host = "smtp.gmail.com";
                            client.Port = 587;
                            client.DeliveryMethod = SmtpDeliveryMethod.Network;

                            client.Send(msg);
                        }
                        item.Actived = false;

                        var account = await context.AccountModel.FirstOrDefaultAsync(x => x.AccountId == item.AccountId);
                        account.Actived = false;

                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
