using System.Web.Mvc;
using ISD.Core;
using System.Linq;
using System;
using ISD.ViewModels;
using ISD.Repositories;
using ISD.Constant;
using ISD.ViewModels.MasterData;
using System.Collections.Generic;

namespace ISD.Admin.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult ChangeLanguage(string lang, string returnUrl)
        {
            //new MultiLanguage().SetLanguage(lang);
            new MultiLanguage().SetLanguage("en");
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/";
            }
            return Redirect(returnUrl);
        }

        public ActionResult Index()
        {
            var _taskRepository = new TaskRepository(_context);
            //Reset selected_module
            if (Request.Cookies["selected_module"] != null)
            {
                Response.Cookies["selected_module"].Expires = DateTime.Now.AddDays(-1);
            }
            ViewBag.SelectedModuleId = new Guid();
            ViewBag.SelectedModuleName = Resources.LanguageResource.Btn_Choose;


            if (CurrentUser.isShowChoseModule)
            {
                //Hiển thị lựa chọn module
                ViewBag.isShowChoseModule = CurrentUser.isShowChoseModule;
                ViewBag.ModuleList = _context.ModuleModel
                                        .Where(p => p.isSystemModule == false)
                                        .OrderBy(p => p.OrderIndex)
                                        .Select(p => new ModuleViewModel()
                                        {
                                            ModuleId = p.ModuleId,
                                            ModuleName = p.ModuleName,
                                            ImageUrl = p.ImageUrl,
                                            Description = p.Description
                                        }).ToList();
            }
            if (CurrentUser.isShowDashboard)
            {
                //Hiển thị dashboard thống kê
                ViewBag.isShowDashBoard = CurrentUser.isShowDashboard;
                //1. Tổng số lượng khách hàng
                var customerCount = _context.ProfileModel.Where(p => p.CustomerTypeCode == ConstProfileType.Account && p.Actived == true).Count();
                ViewBag.Customer = string.Format("{0:n0}", customerCount);
                //2. Tổng số lượng liên hệ
                var contactCount = _context.ProfileModel.Where(p => p.CustomerTypeCode == ConstProfileType.Contact && p.Actived == true).Count();
                ViewBag.Contact = string.Format("{0:n0}", contactCount);
                //3. Tổng công việc
                var taskCount = _taskRepository.GetCountOfTask(ConstTaskStatus.All, null);
                ViewBag.Task = string.Format("{0:n0}", taskCount);

                #region Task: Công việc
                //1. Việc cần làm
                //Là việc có status với ProcessCode (Giai đoạn) = Todo
                ViewBag.Todo = _taskRepository.GetCountOfTask(ConstTaskStatus.Todo, CurrentUser.EmployeeCode);
                //2. Đang thực hiện
                //Là việc có TaskModel.Reporter = CurrentUserId
                //ViewBag.Follow = _taskRepository.GetCountOfTask(ConstTaskStatus.Follow, CurrentUser.EmployeeCode);
                ViewBag.Incomplete = _taskRepository.GetCountOfTask(ConstTaskStatus.Processing, CurrentUser.EmployeeCode);
                //3. Việc đã hoàn thành
                //Là việc có TaskModel.Reporter = CurrentUserId
                //Là việc có TaskAssignModel.SalesEmployeeCode = CurrentUserCode
                ViewBag.Completed = _taskRepository.GetCountOfTask(ConstTaskStatus.Completed, CurrentUser.EmployeeCode);
                //4. Quá hạn
                //Là việc có status với ProcessCode (Giai đoạn) <> Completed và chưa được phân cho ai
                //ViewBag.Unassign = _taskRepository.GetCountOfTask(ConstTaskStatus.Unassign, CurrentUser.EmployeeCode);
                ViewBag.Expired = _taskRepository.GetCountOfTask(ConstTaskStatus.Expired, CurrentUser.EmployeeCode);
                #endregion
            }

            //Thông tin cập nhật
            var TotalTask = 0;
            var taskList = _taskRepository.Get4TaskRecently(out TotalTask);
            ViewBag.TotalTask = TotalTask;

            //Bảng tin
            //var newsCategory = _context.NewsCategoryModel.FirstOrDefault(p => p.OrderIndex == 1);
            //ViewBag.News = _context.NewsModel.Where(p => p.NewsCategoryId == newsCategory.NewsCategoryId && p.CreateBy == CurrentUser.AccountId).ToList();
            ViewBag.News = (from n in _context.NewsModel
                            join m in _context.News_Company_Mapping on n.NewsId equals m.NewsId
                            join nc in _context.NewsCategoryModel on n.NewsCategoryId equals nc.NewsCategoryId
                            where m.CompanyId == CurrentUser.CompanyId
                            select new NewsHomeViewModel()
                            {
                                NewsId = n.NewsId,
                                Title = n.Title,
                                CreateTime = n.CreateTime,
                                ImageUrl = n.ImageUrl,
                                ImageCategoryUrl = nc.ImageUrl
                            }).ToList();
            //if (newsCategory != null)
            //{
            //    ViewBag.ImageUrlNewsCategory = newsCategory.ImageUrl;
            //}
            //else
            //{
            //    NewsCategoryModel NewsCategory = new NewsCategoryModel();
            //    ViewBag.ImageUrlNewsCategory = NewsCategory.ImageUrl;
            //}

            var permision = (from p in _context.PageModel
                             join d in _context.PagePermissionModel on p.PageId equals d.PageId
                             join r in _context.RolesModel on d.RolesId equals r.RolesId
                             where d.FunctionId == "ICONDASHBOARD" && r.RolesName == CurrentUser.RolesName
                             orderby p.OrderIndex
                             select new PageViewModel
                             {
                                 PageName = p.PageName,
                                 PageUrl = p.PageUrl,
                                 Parameter = p.Parameter,
                                 Icon = p.Icon
                             });
            ViewBag.permision = permision;
            return View(taskList);
        }

        public ActionResult App()
        {
            return Redirect("itms-services://?action=download-manifest&amp;url=https://giahoamobile.citek.vn/app.plist");
        }

        [AllowAnonymous]
        public ActionResult GetPPOrderOpen()
        {
            var ppOrderOpenLst = (from a in _context.ProductionOrderModel
                                  join b in _context.TaskModel on a.ProductionOrderId equals b.TaskId
                                  where (b.isDeleted == null || b.isDeleted == false)
                                  group a by new { a.AUFNR, a.DWERK } into groupTemp
                                  select new
                                  {
                                      PPOrderNumber = groupTemp.Key.AUFNR,
                                      Plant = groupTemp.Key.DWERK
                                  }
                               ).ToList();

            var jsonResult = Json(ppOrderOpenLst, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public class LSXSAPViewModel
        {
            public string PPOrderNumber { get; set; }
            public string Plant { get; set; }
        }
        [AllowAnonymous]
        public ActionResult UpdatePPOrderClose(List<LSXSAPViewModel> pporder)
        {
            if (pporder != null && pporder.Count > 0)
            {
                try
                {
                    foreach (var item in pporder)
                    {
                        var ppOrderOpen = (from a in _context.ProductionOrderModel
                                           where a.AUFNR == item.PPOrderNumber || a.AUFNR_MES == item.PPOrderNumber
                                           && a.DWERK == item.Plant
                                           select a).FirstOrDefault();
                        if (ppOrderOpen != null)
                        {
                            //Update task
                            var lsxSAP = (from a in _context.TaskModel
                                          where a.TaskId == ppOrderOpen.ProductionOrderId
                                          select a).FirstOrDefault();
                            if (lsxSAP != null)
                            {
                                lsxSAP.isDeleted = true;
                                _context.Entry(lsxSAP).State = System.Data.Entity.EntityState.Modified;
                            }

                            //Update pporder
                            ppOrderOpen.ZCLOSE = "X";
                            ppOrderOpen.LOEKZ = "X";
                            _context.Entry(lsxSAP).State = System.Data.Entity.EntityState.Modified;

                            _context.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Json("Đã xảy ra lỗi: " + ex.Message, JsonRequestBehavior.AllowGet);
                }

            }

            var jsonResult = Json(new { IsSucess = true, Message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        //Lấy danh sách DSX theo thứ tự ưu tiên
        /*
            1. Plant
            2. LSX DT
            3. DSX
            4. TTUT
            note: lấy những PP Order đang open, lấy TTUT nhập tay, ngày kết thúc điều chỉnh 
         */
        public class DSXViewModel
        {
            public string Plant { get; set; }
            public string LSXDT { get; set; }
            public string DSX { get; set; }
            public string LSXSAP { get; set; }
            public int? ThuTuUuTien { get; set; }
            //public int? STT { get; set; }
            //public DateTime? KTDC { get; set; }
        }
        [AllowAnonymous]
        public ActionResult GetDSXByPriority()
        {
            var result = (from b in _context.TaskModel
                              //DSX
                          join c in _context.TaskModel on b.ParentTaskId equals c.TaskId
                          //Workflow
                          join d in _context.WorkFlowModel on b.WorkFlowId equals d.WorkFlowId
                          //Plant
                          join e in _context.CompanyModel on b.CompanyId equals e.CompanyId
                          //BC01
                          join f in _context.BC01Model on b.TaskId equals f.LSXSAPId
                          where (b.isDeleted == null || b.isDeleted == false)
                          && (d.WorkFlowCode == "LSXC")
                          orderby f.STT.HasValue descending, f.STT, b.ReceiveDate.HasValue descending, b.ReceiveDate
                          group new { b, c, e, f } by new { e.CompanyCode, b.Property3, c.Summary, f.STT, LSXSAP = b.Summary } into groupTemp
                          select new DSXViewModel
                          {
                              Plant = groupTemp.Key.CompanyCode,
                              LSXDT = groupTemp.Key.Property3,
                              DSX = groupTemp.Key.Summary,
                              LSXSAP = groupTemp.Key.LSXSAP,
                          }).ToList();

            if (result != null && result.Count > 0)
            {
                int index = 0;
                foreach (var item in result)
                {
                    index++;
                    item.ThuTuUuTien = index;
                }
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

    }
}