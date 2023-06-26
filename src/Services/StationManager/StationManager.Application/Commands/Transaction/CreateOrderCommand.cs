using CloudinaryDotNet;
using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Newtonsoft.Json;
using ZaloPay.Helper;
using ZaloPay.Helper.Crypto;

namespace StationManager.Application.Commands.Transaction
{
    public class CreateOrderCommand : IRequest<object>
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string TransactionType { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, object>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TransactionModel> _transRepo;

        public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IRepository<TransactionModel> transRepo)
        {
            _unitOfWork = unitOfWork;
            _transRepo = transRepo;
        }

        public async Task<object> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var app_id = "2553";
            var key1 = "PcY4iZIKFCIdgZvA6ueMcMHHUbRLYjPL";
            var create_order_url = "https://sb-openapi.zalopay.vn/v2/create";

            Random rnd = new Random();
            var embed_data = new {
                promotioninfo= "",
                merchantinfo= "embeddata123",
                redirecturl= "http://localhost:3000/home"
            };
            var items = new[] { new {
                itemid= "knb",
                itemname= "kim nguyen bao",
                itemprice= 198400,
                itemquantity = 1
            } };
            var param = new Dictionary<string, string>();
            var app_trans_id = rnd.Next(1000000); // Generate a random order's ID.

            param.Add("app_id", app_id);
            param.Add("app_user", "user123");
            param.Add("app_time", Utils.GetTimeStamp().ToString());
            param.Add("amount", request.Amount.ToString());
            param.Add("app_trans_id", DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
            param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
            param.Add("item", JsonConvert.SerializeObject(items));
            param.Add("description", $"StationMn - Thanh toán đơn hàng ({request.Description}) #{app_trans_id}");
            //param.Add("callback_url", "http://localhost:5003/api/v1/Transaction/Order/callback-api");
            //param.Add("bank_code", "CC");

            var data = app_id + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync(create_order_url, param);

            var newTransaction = new TransactionModel
            {
                TransactionId = Guid.NewGuid(),
                TransactionType = request.TransactionType,
                Price = request.Amount,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedTime = DateTime.Now,
            };

            _transRepo.Add(newTransaction);

            await _unitOfWork.SaveChangesAsync();

            return result;
        }
    }
}
