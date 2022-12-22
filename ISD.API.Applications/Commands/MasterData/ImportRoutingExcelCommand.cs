using ISD.API.Core.Exceptions;
using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using ISD.API.Resources;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ISD.API.Applications.Commands.MasterData
{
    public class ImportRoutingExcelCommand : IRequest<bool>
    {
        public IFormFile FileImport { get; set; }
    }
    public class ImportRoutingExcelCommandHandler : IRequestHandler<ImportRoutingExcelCommand, bool>
    {
        private readonly IRepository<Product_Routing_Mapping> _prdRoutingRep;
        private readonly IISDUnitOfWork _unitOfWork;
        private readonly IRepository<Product_Routing_Mold_Mapping> _prdRoutingMoldRep;
        private readonly IRepository<WorkOrderModel> _woRep;
        private readonly IRepository<ProductModel> _prodRepo;

        public ImportRoutingExcelCommandHandler(IRepository<Product_Routing_Mapping> prdRoutingRep, IISDUnitOfWork unitOfWork, IRepository<Product_Routing_Mold_Mapping> prdRoutingMoldRep
                                                , IRepository<WorkOrderModel> woRep, IRepository<ProductModel> prodRepo)
        {
            _prdRoutingRep = prdRoutingRep;
            _unitOfWork = unitOfWork;
            _prdRoutingMoldRep = prdRoutingMoldRep;
            _woRep = woRep;
            _prodRepo = prodRepo;
        }
        public async Task<bool> Handle(ImportRoutingExcelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //Check length file
                if (request.FileImport == null || request.FileImport.Length <= 0)
                    throw new ISDException(CommonResource.Msg_FileEmpty);

                //Check định dạng file
                if (!Path.GetExtension(request.FileImport.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase) && !Path.GetExtension(request.FileImport.FileName).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                    throw new ISDException(CommonResource.Msg_CheckExtensionFileExcel);

                var productRoutings = new List<Product_Routing_Mapping>();

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
                    for (int rowIndex = 10; rowIndex <= rowCount; rowIndex++)
                    {
                        //Check tồn tại
                        var checkExist = worksheet.Cells[$"C{rowIndex}"].Value == null ? null : worksheet.Cells[$"C{rowIndex}"].Value.ToString().Trim();
                        if (!string.IsNullOrEmpty(checkExist))
                        {
                            //ID product_routing
                            var idProductRouting = Guid.NewGuid();

                            //Danh sách khuôn
                            var molds = worksheet.Cells[$"L{rowIndex}"].Value == null ? null : worksheet.Cells[$"L{rowIndex}"].Value.ToString().Trim();

                            var moldRoutings = new List<Product_Routing_Mold_Mapping>();

                            if (!string.IsNullOrEmpty(molds))
                            {
                                //Cắt dấu phẩy
                                var moldSplits = molds.Trim().Split(",");

                                foreach (var mold in moldSplits)
                                {
                                    var mRouting = new Product_Routing_Mold_Mapping
                                    {
                                        Product_Routing_MappingId = idProductRouting,
                                        Product_Routing_Mold_MappingId = Guid.NewGuid(),
                                        //Khuôn
                                        ProductCode = mold.Trim()
                                    };
                                    moldRoutings.Add(mRouting);
                                }
                            }

                            var prodRt = new Product_Routing_Mapping();
                            prodRt.Product_Routing_MappingId = idProductRouting;
                            //Routing version
                            prodRt.RoutingVersion = worksheet.Cells[$"B{rowIndex}"].Value == null ? null : worksheet.Cells[$"B{rowIndex}"].Value.ToString().Trim();
                            //Mã TP/BTP
                            prodRt.ProductCode = worksheet.Cells[$"C{rowIndex}"].Value == null ? null : worksheet.Cells[$"C{rowIndex}"].Value.ToString().Trim();
                            //Thứ tự công đoạn
                            prodRt.OrderIndex = worksheet.Cells[$"E{rowIndex}"].Value == null ? null : int.Parse(worksheet.Cells[$"E{rowIndex}"].Value.ToString().Trim());
                            //Mã công đoạn
                            prodRt.StepCode = worksheet.Cells[$"F{rowIndex}"].Value == null ? null : worksheet.Cells[$"F{rowIndex}"].Value.ToString().Trim();
                            //Hướng dẫn sản xuất
                            prodRt.ProductionGuide = worksheet.Cells[$"H{rowIndex}"].Value == null ? null : worksheet.Cells[$"H{rowIndex}"].Value.ToString().Trim();
                            //NVL sử dụng
                            prodRt.ComponentUsed = worksheet.Cells[$"I{rowIndex}"].Value == null ? null : worksheet.Cells[$"H{rowIndex}"].Value.ToString().Trim();
                            //Thời gian chuẩn bị
                            prodRt.SetupTime = worksheet.Cells[$"J{rowIndex}"].Value == null ? null : decimal.Parse(worksheet.Cells[$"J{rowIndex}"].Value.ToString().Trim());
                            //Thời gian làm ra sản phẩm
                            prodRt.RatedTime = worksheet.Cells[$"K{rowIndex}"].Value == null ? null : decimal.Parse(worksheet.Cells[$"K{rowIndex}"].Value.ToString().Trim());
                            //Danh sách khuôn
                            prodRt.Product_Routing_Mold_Mapping = moldRoutings;
                            //Số SP/Tờ
                            prodRt.ProductPerPage = worksheet.Cells[$"M{rowIndex}"].Value == null ? null : int.Parse(worksheet.Cells[$"M{rowIndex}"].Value.ToString().Trim());

                            productRoutings.Add(prodRt);
                        }
                    }
                }

                if (!productRoutings.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu công đoạn sản xuất");


                var listPrdRouting = _prdRoutingRep.GetQuery().Include(x => x.Product_Routing_Mold_Mapping);
                var sum = productRoutings.Sum(x => x.RatedTime);

                //Check product routing có cùng product code và routing version
                var prdRoutings = productRoutings.Select(x => new { x.ProductCode, x.RoutingVersion }).Distinct();
                var prdRoutingExist = new List<Product_Routing_Mapping>();
                //Xóa
                foreach (var item in prdRoutings)
                {
                    prdRoutingExist.AddRange(listPrdRouting.Where(x => (x.ProductCode == item.ProductCode) && (x.RoutingVersion == item.RoutingVersion)).ToList());
                }
                _prdRoutingRep.RemoveRange(prdRoutingExist);

                //Check tồn tại thì cập nhật, chưa có thì tạo mới
                foreach (var item in productRoutings)
                {
                    //Check tồn tại mã sản phẩm
                    var product = await _prodRepo.FindOneAsync(p => p.ProductCode == item.ProductCode);
                    if (product == null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Mã sản phẩm");

                    //Công đoạn sx của sản phẩm
                    var productRoutingMold = await listPrdRouting.FirstOrDefaultAsync(x => (x.ProductCode == item.ProductCode) && 
                                                                                           (x.StepCode == item.StepCode) &&
                                                                                           (x.OrderIndex == item.OrderIndex) &&
                                                                                           (x.RoutingVersion == item.RoutingVersion));

                    //Danh sách lệnh sản xuất theo ProductCode
                    var wos = await _woRep.GetQuery(x => x.ProductCode == item.ProductCode)
                                          .Include(x => x.WorkOrder_Mold_Mapping)
                                          .ToListAsync();

                    //Ước tính hoàn thành
                    
                    item.EstimateComplete = sum == 0 ? null : (item.RatedTime / sum) * 100;

                    //Chưa tồn tại => tạo mới
                    if (productRoutingMold == null)
                    {
                        var prdRoutingId = Guid.NewGuid();

                        var prdRouting = new Product_Routing_Mapping
                        {
                            Product_Routing_MappingId = prdRoutingId,
                            //Routing version
                            RoutingVersion = item.RoutingVersion,
                            //Mã TP/BTP
                            ProductCode = item.ProductCode,
                            //Thứ tự công đoạn
                            OrderIndex = item.OrderIndex,
                            //Mã công đoạn
                            StepCode = item.StepCode,
                            //Hướng dẫn sản xuất
                            ProductionGuide = item.ProductionGuide,
                            //NVL sử dụng
                            ComponentUsed = item.ComponentUsed,
                            //Thời gian định mức
                            RatedTime = item.RatedTime,
                            //
                            SetupTime = item.SetupTime,
                            //
                            EstimateComplete = item.EstimateComplete,
                            //Danh sách khuôn
                            Product_Routing_Mold_Mapping = item.Product_Routing_Mold_Mapping,
                            //Số SP/Tờ
                            ProductPerPage = item.ProductPerPage,
                        };

                        //Danh sách khuôn
                        if (item.Product_Routing_Mold_Mapping.Any())
                        {
                            var listPrdRoutingMold = new List<Product_Routing_Mold_Mapping>();
                            listPrdRoutingMold = item.Product_Routing_Mold_Mapping.Select(e => new Product_Routing_Mold_Mapping
                            {
                                Product_Routing_Mold_MappingId = Guid.NewGuid(),
                                Product_Routing_MappingId = prdRoutingId,
                                ProductCode = e.ProductCode,
                            }).ToList();

                            prdRouting.Product_Routing_Mold_Mapping = listPrdRoutingMold;

                        }
                        _prdRoutingRep.Add(prdRouting); 
                    }
                    //Tồn tại cập nhật
                    //else
                    //{

                    //    //Thứ tự công đoạn
                    //    productRoutingMold.OrderIndex = item.OrderIndex;
                    //    //Hướng dẫn sản xuất
                    //    productRoutingMold.ProductionGuide = item.ProductionGuide;
                    //    //NVL sử dụng
                    //    productRoutingMold.ComponentUsed = item.ComponentUsed;
                    //    productRoutingMold.SetupTime = item.SetupTime;
                    //    //Thời gian định mức
                    //    productRoutingMold.RatedTime = item.RatedTime;
                    //    productRoutingMold.EstimateComplete = item.EstimateComplete;
                    //    //Số SP/Tờ
                    //    productRoutingMold.ProductPerPage = item.ProductPerPage;

                    //    //Danh sách khuôn
                    //    if (item.Product_Routing_Mold_Mapping.Any())
                    //    {
                    //        foreach (var mold in item.Product_Routing_Mold_Mapping)
                    //        {
                    //            var moldRouting = productRoutingMold.Product_Routing_Mold_Mapping.FirstOrDefault(x => x.ProductCode == mold.ProductCode);
                    //            //var moldRouting = await _prdRoutingMoldRep.FindOneAsync(x => x.ProductCode == mold.ProductCode);
                    //            //Chưa có thì tạo mới
                    //            if (moldRouting is null)
                    //            {
                    //                _prdRoutingMoldRep.Add(new Product_Routing_Mold_Mapping
                    //                {
                    //                    Product_Routing_Mold_MappingId = Guid.NewGuid(),
                    //                    Product_Routing_MappingId = productRoutingMold.Product_Routing_MappingId,
                    //                    ProductCode = mold.ProductCode,
                    //                });
                    //            }
                    //        }
                    //    }
                    //}

                    //Thêm chi tiết khuôn của lsx
                    if (item.Product_Routing_Mold_Mapping.Any())
                    {
                        foreach (var mold in item.Product_Routing_Mold_Mapping)
                        {
                            foreach (var wo in wos)
                            {
                                var woMold = wo.WorkOrder_Mold_Mapping.FirstOrDefault(x => (x.StepCode == item.StepCode) && (x.MoldCode == mold.ProductCode));
                                if (woMold == null)
                                    wo.WorkOrder_Mold_Mapping.Add(new WorkOrder_Mold_Mapping
                                    {
                                        Id = Guid.NewGuid(),
                                        WorkOrderId = wo.WorkOrderId,
                                        MoldCode = mold.ProductCode,
                                        StepCode = item.StepCode
                                    });
                            }
                        }
                    }
                    
                }

                //Danh sách LSX không có Routing thực tế
                var workOrders = _woRep.GetQuery().Include(x => x.WorkOrder_Routing_Mapping)
                                                  .Where(x => !x.WorkOrder_Routing_Mapping.Any());

                foreach (var item in workOrders)
                {
                    //Danh sách routing kế hoạch theo LSX
                    var prdRouting = productRoutings.Where(x => x.ProductCode == item.ProductCode);

                    //Khi upload Routing vào Product_Routing_Mapping, tìm tất cả các WorkOrder chưa có WorkOrder_Routing_Mapping => thì insert vào luôn
                    item.WorkOrder_Routing_Mapping = prdRouting.Select(e => new WorkOrder_Routing_Mapping
                    {
                        //Id
                        WorkOrder_Routing_Mapping_Id = Guid.NewGuid(),
                        //Mã lsx
                        WorkOrderId = item.WorkOrderId,
                        //Độ ưu tiên
                        OrderIndex = e.OrderIndex,
                        //Mã công đoạn
                        StepCode = e.StepCode,
                        //Hướng dẫn sản xuất
                        ProductionGuide = e.ProductionGuide,
                        //Số SP/Tờ/Công đoạn
                        ProductPerPage = e.ProductPerPage,
                        //NVL sử dụng
                        ComponentUsed = e.ComponentUsed,
                        //% Ước tính hoàn thành
                        EstimateComplete = e.EstimateComplete,
                        //Thời gian chuẩn bị máy
                        SetupTime = e.SetupTime,
                        //Thời gian định mức (min)
                        RatedTime = e.RatedTime,
                    }).ToList();
                }

                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw new ISDException("Sai dữ liệu công đoạn sản xuất");
            }          
        }
    }
}
