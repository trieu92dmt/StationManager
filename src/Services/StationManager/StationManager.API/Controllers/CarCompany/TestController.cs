using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StationManager.Application.DTOs.CarCompany;
using System.Net;
using System.Net.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using ZaloPay.Helper;
using ZaloPay.Helper.Crypto;

namespace StationManager.API.Controllers.CarCompany
{
    [Route("api/v{version:apiVersion}/CarCompany/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TestController : ControllerBase
    {

        private string key2 = "Iyz2habzyr7AG8SgvoBCbKwKi3UzlLi3";
        private readonly IConfiguration _config;

        public TestController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Lưu ảnh lên cloudinary
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("save-img-cloudinary")]
        public IActionResult SaveImageCloudinary([FromForm] ImageUpload image)
        {
            var cloudinary = new Cloudinary(new Account("minhtrieu-cloudinary", "568465589926894", "abhM0GqLGiZf2OuZM4qaP4-nqPw"));

            //Upload
            using (var stream = image.Image.OpenReadStream())
            {

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(image.Image.FileName, stream),
                    PublicId = "my-test-image"
                };

                var uploadResult = cloudinary.Upload(uploadParams);

                //Transformation
                cloudinary.Api.UrlImgUp.Transform(new Transformation().Width(100).Height(150).Crop("fill")).BuildUrl("my-test-image");

                return Ok(uploadResult.Uri.AbsoluteUri);
            }
            
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var app_id = "2553";
            var key1 = "PcY4iZIKFCIdgZvA6ueMcMHHUbRLYjPL";
            var create_order_url = "https://sb-openapi.zalopay.vn/v2/create";

            Random rnd = new Random();
            var embed_data = new { };
            var items = new[] { new { } };
            var param = new Dictionary<string, string>();
            var app_trans_id = rnd.Next(1000000); // Generate a random order's ID.

            param.Add("app_id", app_id);
            param.Add("app_user", "user123");
            param.Add("app_time", Utils.GetTimeStamp().ToString());
            param.Add("amount", "50000");
            param.Add("app_trans_id", DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
            param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
            param.Add("item", JsonConvert.SerializeObject(items));
            param.Add("description", "Lazada - Thanh toán đơn hàng #" + app_trans_id);
            param.Add("bank_code", "zalopayapp");

            var data = app_id + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync(create_order_url, param);

            return Ok(result);
        }

        [HttpGet("get-list-bank")]
        public async Task<IActionResult> GetBank()
        {
            var appid = "2553";
            var key1 = "PcY4iZIKFCIdgZvA6ueMcMHHUbRLYjPL";
            var getBankListUrl = "https://sbgateway.zalopay.vn/api/getlistmerchantbanks";

            var reqtime = Utils.GetTimeStamp().ToString();

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appid", appid);
            param.Add("reqtime", reqtime);
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, appid + "|" + reqtime));

            var result = await HttpHelper.PostFormAsync<BankListResponse>(getBankListUrl, param);

            return Ok(result);
        }

        [HttpGet("sendSMS")]
        public async Task<IActionResult> SendSMS()
        {

            TwilioClient.Init("AC22d029c239301baa811ba8d03d412489", "0be3ef329f1dc40c92643c4dcc4c2dbe");

            var message = MessageResource.Create(
                body: "Test SMS",
                from: new Twilio.Types.PhoneNumber("+18583302669"),
                to: new Twilio.Types.PhoneNumber("+84948513923")
            );

            return Ok(message.Sid);
        }

        /// <summary>
        /// Test Send Mail
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("send-mail-test")]
        public IActionResult SendMailTest()
        {
            var email = _config.GetSection("MailService")["Email"];
            var password = _config.GetSection("MailService")["Password"];

            MailMessage msg = new MailMessage();

            msg.From = new MailAddress(email);
            msg.To.Add("trieu251101@gmail.com");
            msg.Subject = "test";
            msg.Body = "Test Content";
            //msg.Priority = MailPriority.High;


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

            //var client = new SmtpClient("smtp.gmail.com", 587)
            //{
            //    EnableSsl = true,
            //    Credentials = new NetworkCredential(email, password),

            //};

            //client.UseDefaultCredentials = true;

            //client.Send(email, "trieu251101@gmail.com", "test", "testbody");

            return Ok("Hi");
        }
    }

    public class ImageUpload
    {
        public IFormFile Image { get; set; }
    }
}
