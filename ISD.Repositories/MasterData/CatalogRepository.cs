using ISD.Constant;
using ISD.EntityModels;
using ISD.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ISD.Repositories
{
    public class CatalogRepository
    {
        EntityDataContext _context;
        public CatalogRepository(EntityDataContext db)
        {
            _context = db;
        }

        /// <summary>
        /// Lấy danh sách Catalog theo CatalogType
        /// </summary>
        /// <param name="CataLogType">string: CatalogType</param>
        /// <returns>Danh sách catalog</returns>
        public List<CatalogViewModel> GetBy(string CataLogType)
        {
            var ret = (from p in _context.CatalogModel
                       where CataLogType == p.CatalogTypeCode && p.Actived == true
                       orderby p.OrderIndex, p.CatalogCode
                       select new CatalogViewModel()
                       {
                           CatalogCode = p.CatalogCode,
                           CatalogText_vi =p.CatalogCode + " | " + p.CatalogText_vi,
                           CatalogText_en = p.CatalogText_en,
                           OrderIndex = p.OrderIndex,
                       }).ToList();
            
            
            return ret;
        }

        /// <summary>
        /// Lấy danh sách Catalog theo CatalogType và theo cấu hình
        /// </summary>
        /// <param name="CataLogType">string: CatalogType</param>
        /// <param name="ConfigCode">string: ConfigCode</param>
        /// <returns>Danh sách catalog</returns>
        public List<CatalogViewModel> GetByConfig(string CataLogType, string ConfigCode)
        {
            var ret = (from p in _context.CatalogModel
                       where p.CatalogTypeCode == CataLogType
                       && p.Actived == true
                       && p.CatalogText_en.Contains(ConfigCode)
                       orderby p.OrderIndex, p.CatalogCode
                       select new CatalogViewModel()
                       {
                           CatalogCode = p.CatalogCode,
                           CatalogText_vi = p.CatalogText_vi,
                           CatalogText_en = p.CatalogText_en,
                           OrderIndex = p.OrderIndex,
                       }).ToList();


            return ret;
        }

        /// <summary>
        /// Lấy danh sách phân nhóm khách hàng
        /// </summary>
        /// <param name=""></param>
        /// <returns>Danh sách phân nhóm khách hàng</returns>
        public List<CatalogViewModel> GetCustomerAccountGroup()
        {
            var ret = (from p in _context.CatalogModel
                       where p.CatalogTypeCode == ConstCatalogType.CustomerAccountGroup && p.Actived == true
                       && p.CatalogCode.StartsWith("Z")
                       orderby p.OrderIndex, p.CatalogCode
                       select new CatalogViewModel()
                       {
                           CatalogCode = p.CatalogCode,
                           CatalogText_vi = p.CatalogText_vi,
                           CatalogText_en = p.CatalogText_en,
                           OrderIndex = p.OrderIndex,
                       }).ToList();


            return ret;
        }

        /// <summary>
        /// Lấy Khu vực theo Đối tượng
        /// </summary>
        /// <param name="isForeignCustomer"></param>
        /// <returns></returns>
        public List<CatalogViewModel> GetSaleOffice(bool? isForeignCustomer)
        {
            var saleOfficeList = GetBy(ConstCatalogType.SaleOffice);

            //Trong nước
            if (isForeignCustomer == false)
            {
                saleOfficeList = saleOfficeList.Where(p => p.CatalogCode == ConstSaleOffice.MienBac ||
                                                           p.CatalogCode == ConstSaleOffice.MienTrung ||
                                                           p.CatalogCode == ConstSaleOffice.MienNam ||
                                                           p.CatalogCode == ConstSaleOffice.Khac).ToList();
            }
            //Nước ngoài
            else if (isForeignCustomer == true)
            {
                saleOfficeList = saleOfficeList.Where(p => p.CatalogCode != ConstSaleOffice.MienBac &&
                                                           p.CatalogCode != ConstSaleOffice.MienTrung &&
                                                           p.CatalogCode != ConstSaleOffice.MienNam).ToList();
            }
            //Chưa chọn thì không có gì
            else
            {
                saleOfficeList = new List<CatalogViewModel>();
            }
            return saleOfficeList;
        }

        public List<CatalogViewModel> GetCustomerCategory(string CompanyCode)
        {
            var customerGroupList = GetBy(ConstCatalogType.CustomerGroup);
            //Load nhóm KH theo công ty (đang đăng nhập)
            List<CatalogViewModel> customerGroupBySaleOrgLst = new List<CatalogViewModel>();
            if (customerGroupList != null && customerGroupList.Count > 0)
            {
                foreach (var item in customerGroupList)
                {
                    if (!string.IsNullOrEmpty(item.CatalogText_en) && item.CatalogText_en.Contains(CompanyCode))
                    {
                        customerGroupBySaleOrgLst.Add(item);
                    }
                }
            }
            return customerGroupBySaleOrgLst;
        }

        public List<CatalogViewModel> GetCustomerCareer(string CompanyCode)
        {
            var customerCareerList = GetBy(ConstCatalogType.CustomerCareer);
            //Load nhóm KH theo công ty (đang đăng nhập)
            List<CatalogViewModel> customerCareerBySaleOrgLst = new List<CatalogViewModel>();
            if (customerCareerList != null && customerCareerList.Count > 0)
            {
                foreach (var item in customerCareerList)
                {
                    if (!string.IsNullOrEmpty(item.CatalogText_en) && item.CatalogText_en.Contains(CompanyCode))
                    {
                        customerCareerBySaleOrgLst.Add(item);
                    }
                }
            }
            return customerCareerBySaleOrgLst;
        }

        /// <summary>
        /// Lấy danh sách danh mục lỗi
        /// </summary>
        /// <param name=""></param>
        /// <returns>Danh sách danh mục lỗi</returns>
        public List<ISDSelectGuidItem> GetErrorList()
        {
            var ret = (from p in _context.ErrorListModel
                       //join d in _context.AllDepartmentModel on p.DepartmentId equals d.AllDepartmentId into dTemp
                       // from dpt in dTemp.DefaultIfEmpty()
                       // join e in _context.PhysicsWorkShopModel on p.DepartmentId equals e.PhysicsWorkShopId into eTemp
                       // from pws in eTemp.DefaultIfEmpty()
                       where p.Actived == true
                       orderby p.OrderIndex
                       select new ISDSelectGuidItem()
                       {
                           id = p.ErrorListId,
                           //name = string.IsNullOrEmpty(dpt.DepartmentName) ? pws.PhysicsWorkShopName : dpt.DepartmentName + " | " + p.ErrorListName + " | " + p.Personnel,
                           name = p.ErrorListName,
                       }).ToList();


            return ret;
        }

        /// <summary>
        /// Lấy danh sách phòng ban lỗi
        /// </summary>
        /// <param name=""></param>
        /// <returns>Danh sách phòng ban lỗi</returns>
        public List<ISDSelectGuidItem> GetErrorDepartmentList()
        {
            var ret = (from p in _context.AllDepartmentModel
                       where p.Actived == true
                       orderby p.OrderIndex
                       select new ISDSelectGuidItem()
                       {
                           id = p.AllDepartmentId,
                           name = p.DepartmentName,
                       }).ToList();


            return ret;
        }
    }
}