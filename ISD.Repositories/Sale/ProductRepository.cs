using ISD.Constant;
using ISD.EntityModels;
using ISD.ViewModels;
using ISD.ViewModels.Sale;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;

namespace ISD.Repositories
{
    public class ProductRepository
    {
        private EntityDataContext _context;
        private static string appPath = HttpContext.Current.Server.MapPath("~/Logging");
        private static string logFilePath = Path.Combine(appPath, DateTime.Now.ToString("yyyyMMdd") + "-SyncDataFromSAP" + ".log");
        /// <summary>
        /// Khởi tạo ProductRepository truyển vào DataContext
        /// </summary>
        /// <param name="db">EntityDataContext</param>
        public ProductRepository(EntityDataContext db)
        {
            _context = db;
        }

        /// <summary>
        /// Tìm kiếm sản phẩm
        /// </summary>
        /// <param name="productSearchModel">Model Search</param>
        /// <returns>Danh sách sản phẩm</returns>
        public List<ProductViewModel> Search(ProductSearchViewModel productSearchModel)
        {
            var productList = (from p in _context.ProductModel
                               join c in _context.CategoryModel on p.CategoryId equals c.CategoryId
                               where (productSearchModel.ProductCode == null || p.ProductCode == productSearchModel.SearchProductCode)
                               && (productSearchModel.ProductName == null || p.ProductName == productSearchModel.SearchProductName)
                               && (productSearchModel.CategoryId == null || p.CategoryId == productSearchModel.SearchCategoryId)
                               && (productSearchModel.Actived == null || p.Actived == productSearchModel.Actived)
                               select new ProductViewModel
                               {
                                   ProductId = p.ProductId,
                                   ProductCode = p.ProductCode,
                                   ERPProductCode = p.ERPProductCode,
                                   ProductName = p.ProductName,
                                   CategoryId = p.CategoryId,
                                   CategoryName = c.CategoryName,
                                   ImageUrl = p.ImageUrl,
                                   Actived = p.Actived
                               }).ToList();
            return productList;
        }

        public IQueryable<ProductViewModel> SearchQuery(ProductSearchViewModel productSearchModel)
        {
            //var productList = (from p in _context.ProductModel
            //                   join c in _context.CategoryModel on p.CategoryId equals c.CategoryId
            //                   join attr in _context.ProductAttributeModel on p.ProductId equals attr.ProductId
            //                   where (productSearchModel.ProductCode == null || p.ProductCode == productSearchModel.SearchProductCode)
            //                   && (productSearchModel.ProductName == null || p.ProductName == productSearchModel.SearchProductName)
            //                   && (productSearchModel.CategoryId == null || p.CategoryId == productSearchModel.SearchCategoryId)
            //                   && (productSearchModel.Actived == null || p.Actived == productSearchModel.Actived)
            //                   select new ProductViewModel
            //                   {
            //                       ProductId = p.ProductId,
            //                       ProductCode = p.ProductCode,
            //                       ERPProductCode = p.ERPProductCode,
            //                       ProductName = p.ProductName,
            //                       CategoryId = p.CategoryId,
            //                       CategoryName = c.CategoryName,
            //                       ImageUrl = p.ImageUrl,
            //                       Unit = attr.Unit,
            //                       Actived = p.Actived
            //                   });

            var query = (from p in _context.ProductModel
                         join c in _context.CategoryModel on p.CategoryId equals c.CategoryId into tmpCa
                         from ca in tmpCa.DefaultIfEmpty()
                         join br in _context.CategoryModel on p.ParentCategoryId equals br.CategoryId into tmpPca
                         from pca in tmpPca.DefaultIfEmpty()
                         join com in _context.CompanyModel on p.CompanyId equals com.CompanyId into tmpCom
                         from co in tmpCom.DefaultIfEmpty()
                         join attr in _context.ProductAttributeModel on p.ProductId equals attr.ProductId
                         where
                         //search by ERPProductCode
                         (productSearchModel.SearchERPProductCode == null || p.ERPProductCode.Contains(productSearchModel.SearchERPProductCode))
                         //search by ProductCode
                         && (productSearchModel.SearchProductCode == null || p.ProductCode.Contains(productSearchModel.SearchProductCode))
                         //search by ProductName
                         && (productSearchModel.SearchProductName == null || p.ProductName.Contains(productSearchModel.SearchProductName))
                         //search by ParentCategoryId
                         && (productSearchModel.SearchParentCategoryId == null || p.ParentCategoryId == productSearchModel.SearchParentCategoryId)
                         //search by CategoryId
                         && (productSearchModel.SearchCategoryId == null || p.CategoryId == productSearchModel.SearchCategoryId)
                         //search by Actived
                         && (productSearchModel.Actived == null || p.Actived == productSearchModel.Actived)
                         orderby co.CompanyCode, p.ERPProductCode
                         select new ProductViewModel()
                         {
                             ProductId = p.ProductId,
                             ERPProductCode = p.ERPProductCode,
                             ProductCode = p.ProductCode,
                             ProductName = p.ProductName,
                             ParentCategoryId = p.ParentCategoryId,
                             ParentCategoryName = pca.CategoryName,
                             CategoryId = p.CategoryId,
                             CategoryName = ca.CategoryName,
                             ImageUrl = p.ImageUrl,
                             OrderIndex = p.OrderIndex,
                             Unit = attr.Unit,
                             Actived = p.Actived
                         });
            return query;
        }

        /// <summary>
        /// Lấy danh sách sản phầm có quản lý tồn kho
        /// </summary>
        /// <returns>Danh sách sản phẩm có quản lý tồn kho</returns>
        public List<ProductViewModel> GetProInventory()
        {
            var productList = (from p in _context.ProductModel
                               where p.Actived == true && p.isInventory == true
                               select new ProductViewModel
                               {
                                   ProductId = p.ProductId,
                                   ProductName = p.ProductCode + " | " + p.ProductName
                               }).ToList();
            return productList;
        }

        public ProductModel GetProductInventoryByCode(string ProductCode)
        {
            var product = (from p in _context.ProductModel
                           where p.Actived == true && p.isInventory == true && p.ProductCode.Contains(ProductCode)
                           select p
                           ).FirstOrDefault();
            return product;
        }
           public List<ProductViewModel> GetAllProductForDropdown()
        {
            var result = _context.ProductModel.Where(p => p.Actived == true).Select(p => new ProductViewModel {
                ProductId = p.ProductId,
                ProductCode = p.ProductCode,
                ProductName = p.ProductCode + " | " + p.ProductName
            }).OrderBy(p => p.ProductCode).ToList();
            return result;
        }
        public ProductViewModel GetById(Guid id)
        {
            var product = _context.ProductModel.Select(x =>
                   new ProductViewModel
                   {
                       ProductId = x.ProductId,
                       
                   }).FirstOrDefault(x => x.ProductId == id);

            var lstAttachment = GetListFileAttachment(id);

            product.ListFileAttachment = lstAttachment;

            return product;
        }
        public List<FileAttachmentViewModel> GetListFileAttachment(Guid productId)
        {
            var lstFiles = (from p in _context.FileAttachmentModel
                            join m in _context.Product_FileAttachment_Mapping on p.FileAttachmentId equals m.FileAttachmentId
                            where m.ProductId == productId && p.FileType == "BANVE"
                            select new FileAttachmentViewModel
                            {
                                FileAttachmentId = p.FileAttachmentId,
                                ObjectId = p.ObjectId,
                                FileAttachmentCode = p.FileAttachmentCode,
                                FileAttachmentName = p.FileAttachmentName,
                                FileExtention = p.FileExtention,
                                FileUrl = p.FileUrl,
                                CreateTime = p.CreateTime,
                                DrawingCode = p.Description1,
                                DrawingMoldCode = p.Description2,
                                DrawingNote = p.Description3,
                            }).ToList();
            return lstFiles;
        }
        public List<ProductAutocompleViewModel> GetForAutocomple(string SearchText)
        {
            var result = (from p in _context.ProductModel
                          where p.isInventory == true
                          && p.Actived == true
                          && (p.ProductCode.Contains(SearchText) || p.ProductName.Contains(SearchText) || p.ERPProductCode.Contains(SearchText)|| p.Serial.Contains(SearchText))
                          select new ProductAutocompleViewModel
                          {
                              ProductCode = p.ProductCode,
                              ProductName = p.ProductName,
                              ProductId = p.ProductId,
                              //ProductSearchName = p.ProductCode + " | " + p.ProductName,
                              ProductSearchName = p.ProductName,
                              Serial=p.Serial
                          }).Take(10).ToList();
            return result;
        }
        public List<ProductAutocompleViewModel> GetForAutocomple2(string SearchText)
        {
            //Phân cấp sản phẩm: CategoryTypeCode==Category1
            var classTypeId = _context.CategoryTypeModel.FirstOrDefault(x => x.CategoryTypeCode == "Category1")?.CategoryTypeId;
            //Phân loại sản phẩm là khuôn
            var moldType = _context.CategoryModel.FirstOrDefault(x => x.CategoryCode == "5" && x.CategoryTypeId == classTypeId)?.CategoryId;
            //Danh sách sản phẩm là khuôn
            var productMolds = _context.ProductModel.Where(p => p.ParentCategoryId == moldType&&(!string.IsNullOrEmpty(SearchText.Trim()) ?
                                                                                                          p.ProductCode.Contains(SearchText) ||
                                                                                                          p.ProductName.Contains(SearchText) ||
                                                                                                          p.ERPProductCode.Contains(SearchText) ||
                                                                                                          p.Serial.Contains(SearchText) :true) )
                                                    .Select(p => new ProductAutocompleViewModel
                                                    {
                                                        ProductCode = p.ProductCode,
                                                        ProductName = p.ProductName,
                                                        ProductId = p.ProductId,
                                                        //ProductSearchName = p.ProductCode + " | " + p.ProductName,
                                                        ProductSearchName = p.ProductName,
                                                        Price = p.Price,
                                                        Serial = p.Serial,
                                                    }).Take(10).ToList();
            //var result = (from p in _context.ProductModel
            //              where p.isInventory == true
            //              && p.Actived == true
            //              //&&(Guid) p.CategoryId== "62E4C7F7-F14C-41A4-83F7-AB5E008329C8"
            //              //&&
            //              && (p.ProductCode.Contains(SearchText) || p.ProductName.Contains(SearchText) || p.ERPProductCode.Contains(SearchText) || p.Serial.Contains(SearchText))
            //              select new ProductAutocompleViewModel
            //              {
            //                  ProductCode = p.ProductCode,
            //                  ProductName = p.ProductName,
            //                  ProductId = p.ProductId,
            //                  //ProductSearchName = p.ProductCode + " | " + p.ProductName,
            //                  ProductSearchName = p.ProductName,
            //                  Price = p.Price,
            //                  Serial = p.Serial,
            //              }).Take(10).ToList();
            return productMolds;
        }
        public List<BomDetailViewModel> GetBOMDetail(string MATNR, string WERKS)
        {
            var result = (from bh in _context.BOMHeaderModel
                          join bd in _context.BOMDetailModel on bh.STLNR equals bd.STLNR
                          where bh.MATNR == MATNR && bh.WERKS == WERKS
                          orderby bd.IDNRK, bd.POSNR
                          select new BomDetailViewModel
                          {
                              STLTY = bd.STLTY,
                              STLNR = bd.STLNR,
                              STLKN = bd.STLKN,
                              STPOZ = bd.STPOZ,
                              DATUV = bd.DATUV,
                              LKENZ = bd.LKENZ,
                              ANDAT = bd.ANDAT,
                              ANNAM = bd.ANNAM,
                              IDNRK = bd.IDNRK_MES,
                              MAKTX = bd.MAKTX,
                              MTART = bd.MTART,
                              POSTP = bd.POSTP,
                              POSNR = bd.POSNR,
                              MENGE = bd.MENGE,
                              MEINS = bd.MEINS,
                              AUSCH = bd.AUSCH,
                              AVOAU = bd.AVOAU,
                              POTX1 = bd.POTX1,
                              POTX2 = bd.POTX2,
                              WERKS = bd.WERKS,
                              MATNR = bd.MATNR,
                              AENNR = bd.AENNR,
                          }).ToList();
            return result;
        }
        public List<BomDetailViewModel> GetBOMSearchDetail(string MATNR, string WERKS,SearchBOMViewModel viewmodel)
        {
            var result = (from bh in _context.BOMHeaderModel
                          join bd in _context.BOMDetailModel on bh.STLNR equals bd.STLNR
                          where bh.MATNR == MATNR && bh.WERKS == WERKS
                          && (viewmodel.Version == null || bh.STLAL == viewmodel.Version)
                          && (viewmodel.FromDate == null || bh.CreateTime >= viewmodel.FromDate)
                          && (viewmodel.ToDate == null || bh.CreateTime <= viewmodel.ToDate)
                          orderby bd.IDNRK, bd.POSNR
                          select new BomDetailViewModel
                          {
                              STLTY = bd.STLTY,
                              STLNR = bd.STLNR,
                              STLKN = bd.STLKN,
                              STPOZ = bd.STPOZ,
                              DATUV = bd.DATUV,
                              LKENZ = bd.LKENZ,
                              ANDAT = bd.ANDAT,
                              ANNAM = bd.ANNAM,
                              IDNRK = bd.IDNRK_MES,
                              MAKTX = bd.MAKTX,
                              MTART = bd.MTART,
                              POSTP = bd.POSTP,
                              POSNR = bd.POSNR,
                              MENGE = bd.MENGE,
                              MEINS = bd.MEINS,
                              AUSCH = bd.AUSCH,
                              AVOAU = bd.AVOAU,
                              POTX1 = bd.POTX1,
                              POTX2 = bd.POTX2,
                              WERKS = bd.WERKS,
                              MATNR = bd.MATNR,
                              AENNR = bd.AENNR,
                          }).ToList();
            return result;
        }

        /// <summary>
        /// Lấy thông tin sản phẩm từ SAP
        /// </summary>
        /// <param name="type"> 1 or 2,3,4</param>
        /// <param name="CompanyCode">1000,2000,3000,4000</param>
        /// <param name="MaterialType">Z83,Z86,...</param>
        /// <param name="ERPProductCode">ERPProductCode: 830000284</param>
        /// <returns></returns>
        public DataTable GetDataMaterial(int type, string CompanyCode, string MaterialType, string ERPProductCode, out string mess)
        {
            DataTable datatable = new DataTable();
            mess = string.Empty;
            try
            {
                ////Khởi tạo thư viện và kết nối
                //var _sap = new SAPRepository();
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.YAC_FM_CRM_GET_MATERIAL);

                ////Thông số truyền vào
                //function.SetValue("IM_TYPE", type);
                ////function.SetValue("IM_FRDATE", DateTime.Now.AddDays(-7).ToString("yyyyMMdd"));
                //function.SetValue("IM_FRDATE", DateTime.Now.ToString("yyyyMMdd"));
                //function.SetValue("IM_TODATE", DateTime.Now.ToString("yyyyMMdd"));
                //function.SetValue("IM_WERKS", CompanyCode);
                //function.SetValue("IM_MTART", MaterialType);
                //if (ERPProductCode.Length > 2)
                //{
                //    function.SetValue("IM_MATNR", ERPProductCode);
                //}
                //else
                //{
                //    function.SetValue("IM_MATNR", "%");
                //}
                ////Thực thi
                //function.Invoke(destination);

                //datatable = function.GetTable("MAT_T").ToDataTable("MAT_T");
                ////Nếu là sản phẩm mới add theo công ty
                //if (type == 1 && datatable != null && datatable.Rows.Count > 0)
                //{
                //    var CompanyId = _context.CompanyModel.Where(p => p.CompanyCode == CompanyCode).Select(p => p.CompanyId).FirstOrDefault();
                //    datatable.Columns.Add(new DataColumn() { ColumnName = "CompanyId", DefaultValue = CompanyId });
                //}
            }
            catch (Exception ex)
            {
                mess = ex.Message;
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        mess = ex.InnerException.InnerException.Message;
                    }
                    else
                    {
                        mess = ex.InnerException.Message;
                    }
                }
                //ghi log
                WriteLogFile(logFilePath, "Sync Data error: " + mess);
            }
            return datatable;
        }

        /// <summary>
        /// Đồng bộ sản phẩm theo ERPProductCode từ SAP
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="type"></param>
        /// <param name="matType"></param>
        /// <param name="eRPProductCode"></param>
        /// <returns>True || False, out error message</returns>
        public bool SyncMaterialByERPProductCode(string companyCode, int type, string matType, string eRPProductCode, out string mess)
        {
            var categoryList = _context.CategoryModel.Where(p => p.Actived == true).AsNoTracking().ToList();

            // Get Datatable
            var data = GetDataMaterial(type, companyCode, matType, eRPProductCode, out mess);

            #region Add data into db
            if (data != null && data.Rows.Count > 0)
            {
                try
                {
                    //var productNew = new ProductModel();
                    //var productAttrNew = new ProductAttributeModel();
                    var productList = new List<ProductModel>();
                    var productAttrList = new List<ProductAttributeModel>();

                    foreach (DataRow item in data.Rows)
                    {
                        var ProductId = Guid.NewGuid();
                        var ProductCode = item["MATNR"].ToString();
                        var CompanyId = Guid.Parse(item["CompanyId"].ToString());
                        var WarrantyCode = string.Format("BH{0}T", item["WARRANTY"].ToString());
                        var WarrantyId = _context.WarrantyModel.Where(p => p.WarrantyCode == WarrantyCode).Select(p => p.WarrantyId).FirstOrDefault();
                        //ParentCategory
                        var ParentCategoryCode = item["MTART"].ToString();
                        Guid? ParentCategoryId = categoryList.Where(p => p.CategoryCode == ParentCategoryCode).Select(p => p.CategoryId).FirstOrDefault();
                        if (ParentCategoryId == Guid.Empty)
                        {
                            ParentCategoryId = null;
                        }
                        //Category
                        var CategoryCode = item["MATKL"].ToString();
                        Guid? CategoryId = categoryList.Where(p => p.CategoryCode == CategoryCode).Select(p => p.CategoryId).FirstOrDefault();
                        if (CategoryId == Guid.Empty)
                        {
                            CategoryId = null;
                        }

                        #region Product
                        var productExist = _context.ProductModel.Where(p => p.ERPProductCode == ProductCode && p.CompanyId == CompanyId).FirstOrDefault();
                        if (productExist == null)
                        {
                            var productNew = new ProductModel();

                            productNew.ProductId = ProductId;
                            productNew.ProductCode = ProductCode;
                            productNew.ERPProductCode = ProductCode;
                            productNew.ProductName = item["MAKTX"].ToString();
                            productNew.ParentCategoryId = ParentCategoryId;
                            productNew.CategoryId = CategoryId;
                            productNew.Actived = true;
                            productNew.CompanyId = CompanyId;
                            productNew.WarrantyId = WarrantyId;

                            productList.Add(productNew);
                        }
                        else
                        {
                            ProductId = productExist.ProductId;
                            productExist.ERPProductCode = ProductCode;
                            productExist.ProductName = item["MAKTX"].ToString();
                            productExist.ParentCategoryId = ParentCategoryId;
                            productExist.CategoryId = CategoryId;
                            productExist.Actived = true;
                            productExist.CompanyId = CompanyId;
                            productExist.WarrantyId = WarrantyId;

                            _context.Entry(productExist).State = EntityState.Modified;
                        }
                        #endregion

                        #region Product Attribute
                        //Delete exist attribute
                        var existAttr = _context.ProductAttributeModel.Where(p => p.ProductId == ProductId).FirstOrDefault();
                        if (existAttr != null)
                        {
                            _context.Entry(existAttr).State = EntityState.Deleted;
                        }
                        var productAttrNew = new ProductAttributeModel();
                        //Add new again
                        productAttrNew.ProductId = ProductId;
                        productAttrNew.Description = item["MAKTXDESC"].ToString();
                        productAttrNew.Unit = item["MEINS"].ToString();
                        productAttrNew.Color = item["EXTWG"].ToString();
                        productAttrNew.Thickness = item["LABOR"].ToString();
                        productAttrNew.Allocation = item["KOSCH"].ToString();
                        productAttrNew.Grade = item["MEDIUM"].ToString();
                        productAttrNew.Surface = item["ZEIFO"].ToString();
                        productAttrNew.NumberOfSurface = item["BLANZ"].ToString();
                        productAttrNew.GrossWeight = Convert.ToDecimal(item["BRGEW"].ToString());
                        productAttrNew.NetWeight = Convert.ToDecimal(item["NTGEW"].ToString());
                        productAttrNew.WeightUnit = item["GEWEI"].ToString();
                        productAttrList.Add(productAttrNew);
                        #endregion
                    }

                    if (productList != null && productList.Count > 0)
                    {
                        _context.ProductModel.AddRange(productList);
                    }
                    _context.ProductAttributeModel.AddRange(productAttrList);
                    _context.SaveChanges();
                    return true;
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        WriteLogFile(logFilePath, string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            WriteLogFile(logFilePath, string.Format("Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                        }
                    }
                }
                catch (Exception ex)
                {
                    mess = ex.Message;
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.InnerException != null)
                        {
                            mess = ex.InnerException.InnerException.Message;
                        }
                        else
                        {
                            mess = ex.InnerException.Message;
                        }
                    }
                    //ghi log
                    //MessageBox.Show(mess);
                    WriteLogFile(logFilePath, "Sync Data error: " + mess);
                }
            }
            #endregion
            return false;
        }

        /// <summary>
        /// Đồng bộ sản phẩm theo ERPProductCode từ SAP
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="type"></param>
        /// <param name="matType"></param>
        /// <param name="eRPProductCode"></param>
        /// <returns>GUID: Id sản phẩm sau khi đồng bộ thành công</returns>
        public Guid SyncMaterial(string companyCode, int type, string matType, string eRPProductCode)
        {
            var categoryList = _context.CategoryModel.Where(p => p.Actived == true).AsNoTracking().ToList();
            var mess = string.Empty;
            // Get Datatable
            var data = GetDataMaterial(type, companyCode, matType, eRPProductCode, out mess);

            #region Add data into db
            if (data != null && data.Rows.Count > 0)
            {
                try
                {
                    ProductModel productNew = new ProductModel();
                    ProductAttributeModel productAttrNew = new ProductAttributeModel();

                    DataRow productSAP = data.Rows[0];

                    var ProductId = Guid.NewGuid();
                    var ProductCode = productSAP["MATNR"].ToString();
                    var CompanyId = Guid.Parse(productSAP["CompanyId"].ToString());
                    var WarrantyCode = string.Format("BH{0}T", productSAP["WARRANTY"].ToString());
                    var WarrantyId = _context.WarrantyModel.Where(p => p.WarrantyCode == WarrantyCode).Select(p => p.WarrantyId).FirstOrDefault();
                    //ParentCategory
                    var ParentCategoryCode = productSAP["MTART"].ToString();
                    Guid? ParentCategoryId = categoryList.Where(p => p.CategoryCode == ParentCategoryCode).Select(p => p.CategoryId).FirstOrDefault();
                    if (ParentCategoryId == Guid.Empty)
                    {
                        ParentCategoryId = null;
                    }
                    //Category
                    var CategoryCode = productSAP["MATKL"].ToString();
                    Guid? CategoryId = categoryList.Where(p => p.CategoryCode == CategoryCode).Select(p => p.CategoryId).FirstOrDefault();
                    if (CategoryId == Guid.Empty)
                    {
                        CategoryId = null;
                    }

                    #region Product
                    var exist = _context.ProductModel.Where(p => p.ERPProductCode == ProductCode && p.CompanyId == CompanyId).FirstOrDefault();
                    if (exist == null)
                    {
                        productNew.ProductId = ProductId;
                        productNew.ProductCode = ProductCode;
                        productNew.ERPProductCode = ProductCode;
                        productNew.ProductName = productSAP["MAKTX"].ToString();
                        productNew.ParentCategoryId = ParentCategoryId;
                        productNew.CategoryId = CategoryId;
                        productNew.ProductName = productSAP["MAKTX"].ToString();
                        productNew.Actived = true;
                        productNew.CompanyId = CompanyId;
                        productNew.WarrantyId = WarrantyId;
                    }
                    #endregion

                    #region Product Attribute
                    //Delete exist attribute
                    var existAttr = _context.ProductAttributeModel.Where(p => p.ProductId == ProductId).FirstOrDefault();
                    if (existAttr != null)
                    {
                        _context.Entry(existAttr).State = EntityState.Deleted;
                    }
                    //Add new again
                    productAttrNew.ProductId = ProductId;
                    productAttrNew.Description = productSAP["MAKTXDESC"].ToString();
                    productAttrNew.Unit = productSAP["MEINS"].ToString();
                    productAttrNew.Color = productSAP["EXTWG"].ToString();
                    productAttrNew.Thickness = productSAP["LABOR"].ToString();
                    productAttrNew.Allocation = productSAP["KOSCH"].ToString();
                    productAttrNew.Grade = productSAP["MEDIUM"].ToString();
                    productAttrNew.Surface = productSAP["ZEIFO"].ToString();
                    productAttrNew.NumberOfSurface = productSAP["BLANZ"].ToString();
                    productAttrNew.GrossWeight = Convert.ToDecimal(productSAP["BRGEW"].ToString());
                    productAttrNew.NetWeight = Convert.ToDecimal(productSAP["NTGEW"].ToString());
                    productAttrNew.WeightUnit = productSAP["GEWEI"].ToString();

                    #endregion

                    _context.Entry(productNew).State = EntityState.Added;
                    _context.Entry(productAttrNew).State = EntityState.Added;
                    _context.SaveChanges();
                    return ProductId;
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        WriteLogFile(logFilePath, string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            WriteLogFile(logFilePath, string.Format("Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                        }
                    }
                }
                catch (Exception ex)
                {
                    mess = ex.Message;
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.InnerException != null)
                        {
                            mess = ex.InnerException.InnerException.Message;
                        }
                        else
                        {
                            mess = ex.InnerException.Message;
                        }
                    }
                    //ghi log
                    //MessageBox.Show(mess);
                    WriteLogFile(logFilePath, "Sync Data error: " + mess);
                }
            }
            #endregion

            return Guid.Empty;
        }


        #region WriteLogFile
        public static void WriteLogFile(string filePath, string message)
        {
            if (File.Exists(filePath))
            {
                if (!File.Exists(filePath))
                    File.Create(filePath);
            }
            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                fileStream.Flush();
                fileStream.Close();
            }

            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                string lastRecordText = "# " + System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " # " + Environment.NewLine + "#" + message + " #" + Environment.NewLine;
                sw.WriteLine(lastRecordText);
                sw.Close();
            }
        }
        #endregion
    }
}