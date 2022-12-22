using ISD.API.Core.Exceptions;
using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using ISD.API.Resources;
using MediatR;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace ISD.API.Applications.Commands.MasterData
{
    public class ImportEquipmentExcelCommand : IRequest<bool>
    {
        public IFormFile FileImport { get; set; }
    }

    public class ImportEquipmentExcelCommandHandler : IRequestHandler<ImportEquipmentExcelCommand, bool>
    {
        private readonly IRepository<EquipmentModel> _equipRepo;
        private readonly IISDUnitOfWork _unitOfWork;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<WorkShopModel> _workShopRepo;

        public ImportEquipmentExcelCommandHandler(IRepository<EquipmentModel> equipRepo, IISDUnitOfWork unitOfWork, IRepository<CatalogModel> cataRepo,
                                                  IRepository<WorkShopModel> workShopRepo)
        {
            _equipRepo = equipRepo;
            _unitOfWork = unitOfWork;
            _cataRepo = cataRepo;
            _workShopRepo = workShopRepo;
        }

        public async Task<bool> Handle(ImportEquipmentExcelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //Check length file
                if (request.FileImport == null || request.FileImport.Length <= 0)
                    throw new ISDException(CommonResource.Msg_FileEmpty);

                //Check định dạng file
                if (!Path.GetExtension(request.FileImport.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase) && !Path.GetExtension(request.FileImport.FileName).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                    throw new ISDException(CommonResource.Msg_CheckExtensionFileExcel);

                //var productRoutings = new List<Product_Routing_Mapping>();

                using (var stream = new MemoryStream())
                {
                    //Copy file
                    await request.FileImport.CopyToAsync(stream, cancellationToken);

                    // Creating an instance of ExcelPackage
                    var excel = new ExcelPackage(stream);

                    //Get sheet excel
                    ExcelWorksheet worksheet = excel.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    //Row index      
                    for (int rowIndex = 6; rowIndex <= rowCount; rowIndex++)
                    {
                        //Check tồn tại
                        var checkExist = worksheet.Cells[$"B{rowIndex}"].Value == null ? null : worksheet.Cells[$"B{rowIndex}"].Value.ToString().Trim();
                        if (!string.IsNullOrEmpty(checkExist))
                        {
                            //Phân xưởng
                            var workShopName = worksheet.Cells[$"F{rowIndex}"].Value == null ? null : worksheet.Cells[$"F{rowIndex}"].Value.ToString().Trim();
                            var workShop = await _workShopRepo.FindOneAsync(x => x.WorkShopName == workShopName);
                            //Mã nhóm
                            var EquipGrName = worksheet.Cells[$"D{rowIndex}"].Value == null ? null : worksheet.Cells[$"D{rowIndex}"].Value.ToString().Trim();
                            var EquipmentGr = await _cataRepo.FindOneAsync(x => (x.CatalogTypeCode == "MachineChainType") && (x.CatalogText_vi.Trim() == EquipGrName));
                            //Phân loại
                            var EquipTypeName = worksheet.Cells[$"E{rowIndex}"].Value == null ? null : worksheet.Cells[$"E{rowIndex}"].Value.ToString().Trim();
                            var EquipmentType = await _cataRepo.FindOneAsync(x => (x.CatalogTypeCode == "MachineChain") && (x.CatalogText_vi.Trim() == EquipTypeName));
                            //Đơn vị tính công suất
                            var unit = worksheet.Cells[$"I{rowIndex}"].Value == null ? null : worksheet.Cells[$"I{rowIndex}"].Value.ToString().Trim();
                            var unitCata = await _cataRepo.FindOneAsync(x => (x.CatalogTypeCode == "EquipmentPowerUnit") && (x.CatalogText_vi.Trim() == unit));
                            //Trạng thái máy móc
                            var status = worksheet.Cells[$"J{rowIndex}"].Value == null ? null : worksheet.Cells[$"J{rowIndex}"].Value.ToString().Trim();
                            var statusCata = await _cataRepo.FindOneAsync(x => (x.CatalogTypeCode == "MachineChainStatus") && (x.CatalogText_vi.Trim() == status));
                            //Check equipment
                            var equip = await _equipRepo.FindOneAsync(x => x.EquipmentCode.Trim() == worksheet.Cells[$"B{rowIndex}"].Value.ToString().Trim());
                            //Chưa có thì tạo mới, có thì cập nhật
                            if (equip != null)
                            {
                                //Tên máy móc chuyền
                                equip.EquipmentName = worksheet.Cells[$"C{rowIndex}"].Value == null ? null : worksheet.Cells[$"C{rowIndex}"].Value.ToString().Trim();
                                //Mã nhóm
                                equip.EquipmentGroupCode = EquipmentGr == null ? null : EquipmentGr.CatalogCode;
                                //Phân loại
                                equip.EquipmentTypeCode = EquipmentType == null ? null : EquipmentType.CatalogCode;

                                equip.WorkShopId = workShop.WorkShopId;
                                //Mô tả
                                equip.Description = worksheet.Cells[$"G{rowIndex}"].Value == null ? null : worksheet.Cells[$"G{rowIndex}"].Value.ToString().Trim();
                                //Công suất máy/chuyền
                                equip.EquipmentProduction = worksheet.Cells[$"H{rowIndex}"].Value == null ? null : decimal.Parse(worksheet.Cells[$"H{rowIndex}"].Value.ToString().Trim());
                                //Đơn vị tính công suất
                                equip.Unit = unitCata == null ? null : unitCata.CatalogCode;
                                //Trạng thái máy móc
                                equip.EquipmentStatus = statusCata == null ? null : statusCata.CatalogCode;
                            }
                            else
                            {
                                _equipRepo.Add(new EquipmentModel
                                {
                                    //Id
                                    EquipmentId = Guid.NewGuid(),
                                    //Mã máy móc/chuyền
                                    EquipmentCode = worksheet.Cells[$"B{rowIndex}"].Value == null ? null : worksheet.Cells[$"B{rowIndex}"].Value.ToString().Trim(),
                                    //Tên máy móc chuyền
                                    EquipmentName = worksheet.Cells[$"C{rowIndex}"].Value == null ? null : worksheet.Cells[$"C{rowIndex}"].Value.ToString().Trim(),
                                    //Mã nhóm
                                    EquipmentGroupCode = EquipmentGr == null ? null : EquipmentGr.CatalogCode,
                                    //Phân loại
                                    EquipmentTypeCode = EquipmentType == null ? null : EquipmentType.CatalogCode,
                                    //Phân xưởng
                                    WorkShopId = workShop?.WorkShopId,
                                    //Mô tả
                                    Description = worksheet.Cells[$"G{rowIndex}"].Value == null ? null : worksheet.Cells[$"G{rowIndex}"].Value.ToString().Trim(),
                                    //Công suất máy/chuyền
                                    EquipmentProduction = worksheet.Cells[$"H{rowIndex}"].Value == null ? null : decimal.Parse(worksheet.Cells[$"H{rowIndex}"].Value.ToString().Trim()),
                                    //Đơn vị tính công suất
                                    Unit = unitCata == null ? null : unitCata.CatalogCode,
                                    //Trạng thái máy móc
                                    EquipmentStatus = statusCata == null ? null : statusCata.CatalogCode,
                                    Actived = true
                                });
                            }
                        }
                    }
                    await _unitOfWork.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception)
            {
                throw new ISDException("Sai dữ liệu công đoạn sản xuất");
            }
        }
    }
}
