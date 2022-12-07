//using ISD.API.Core.Exceptions;
//using ISD.API.EntityModels.Models;
//using ISD.API.Repositories.Infrastructure.Database;
//using ISD.API.Repositories.Infrastructure.Repositories;
//using MediatR;
//using Microsoft.AspNetCore.Http;
//using OfficeOpenXml;

//namespace ISD.API.Applications.Commands.MasterData
//{
//    public class ImportExcelSampMethodCommand : IRequest<bool>
//    {
//        public IFormFile File { get; set; }
//    }
//    public class ImportExcelSampMethodCommandHandler : IRequestHandler<ImportExcelSampMethodCommand, bool>
//    {
//        private readonly IGeneRepo<SampMethodModel> _samMethodRep;
//        private readonly IGeneRepo<AccountModel> _userRep;
//        private readonly IISDUnitOfWork _unitOfWork;

//        public ImportExcelSampMethodCommandHandler(IGeneRepo<SampMethodModel> samMethodRep, IGeneRepo<AccountModel> userRep, IISDUnitOfWork unitOfWork)
//        {
//            _samMethodRep = samMethodRep;
//            _userRep = userRep;
//            _unitOfWork = unitOfWork;
//        }
//        public async Task<bool> Handle(ImportExcelSampMethodCommand request, CancellationToken cancellationToken)
//        {
//            //Check length file
//            if (request.File == null || request.File.Length <= 0)
//                throw new ISDException("File trống.");

//            //Check định dạng file
//            if (!Path.GetExtension(request.File.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase) && !Path.GetExtension(request.File.FileName).Equals(".xls", StringComparison.OrdinalIgnoreCase))
//                throw new ISDException("File không đúng định dạng !");

//            using (var stream = new MemoryStream())
//            {
//                //Copy file
//                await request.File.CopyToAsync(stream);

//                // Creating an instance of ExcelPackage
//                var excel = new ExcelPackage(stream);

//                //Get sheet excel
//                ExcelWorksheet worksheet = excel.Workbook.Worksheets[0];
//                var rowCount = worksheet.Dimension.Rows;

//                var sampMethods = new List<SampMethodModel>();

//                var admin = await _userRep.FindOneAsync(x => x.UserName == "admin");

//                //Row index      
//                for (int rowIndex = 2; rowIndex <= rowCount; rowIndex++)
//                {
//                    //Check tồn tại
//                    var checkExist = worksheet.Cells[$"A{rowIndex}"].Value == null ? null : worksheet.Cells[$"A{rowIndex}"].Value.ToString().Trim();

//                    if (!string.IsNullOrEmpty(checkExist))
//                    {
//                        var sampMethod = new SampMethodModel();

//                        //Id
//                        sampMethod.SampMethodId = Guid.NewGuid();
//                        //Cỡ lô
//                        var sampleSize = worksheet.Cells[$"A{rowIndex}"].Value == null ? null : worksheet.Cells[$"A{rowIndex}"].Value.ToString().Trim();

//                        //Cỡ lô chi tiết
//                        sampMethod.SampleSize = sampleSize;

//                        //Cỡ lô từ
//                        var sampleSizeSpl = sampleSize.Split(" Đến ");

//                        //Cỡ lô từ
//                        sampMethod.SampleSizeFrom = int.Parse(sampleSizeSpl[0].Replace(",", ""));

//                        //Cỡ lỗ đến
//                        if (sampleSizeSpl[1] == "Trở lên")
//                            sampMethod.SampleSizeTo = Int32.MaxValue;
//                        else
//                            sampMethod.SampleSizeTo = int.Parse(sampleSizeSpl[1].Replace(",", ""));

//                        //Tên phương pháp lấy mẫu
//                        sampMethod.SampleName = worksheet.Cells[$"B{rowIndex}"].Value == null ? null : worksheet.Cells[$"B{rowIndex}"].Value.ToString().Trim();
//                        //Số lượng lấy mẫu
//                        sampMethod.SampleQuantity = worksheet.Cells[$"C{rowIndex}"].Value == null ? null : int.Parse(worksheet.Cells[$"C{rowIndex}"].Value.ToString().Trim());

//                        //Common
//                        sampMethod.CreateTime = DateTime.Now;
//                        sampMethod.CreateBy = admin?.AccountId;
//                        sampMethod.Actived = true;

//                        sampMethods.Add(sampMethod);
//                    }
//                }

//                _samMethodRep.AddRange(sampMethods);
//            }

//            await _unitOfWork.SaveChangesAsync();

//            return true;
//        }
//    }
//}
