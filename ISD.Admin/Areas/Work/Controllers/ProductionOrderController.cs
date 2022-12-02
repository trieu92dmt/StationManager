using ISD.Constant;
using ISD.Core;
using ISD.Repositories;
using ISD.Resources;
using ISD.ViewModels;
using ISD.ViewModels.Work;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Work.Controllers
{
    public class ProductionOrderController : BaseController
    {
        private string GoogleMapAPIKey = WebConfigurationManager.AppSettings["GoogleMapAPIKey"].ToString();
        // GET: SlipCommand

        #region BEGIN Index
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult _Search(TaskSearchViewModel searchModel)
        {
            return ExecuteSearch(() =>
            {
                var searchResult = _unitOfWork.ProductionManagementRepository.Search(searchModel);
                return PartialView(searchResult);
            });
        }
        #endregion END index

        #region BEGIN Chia task
        public ActionResult _DivisionOfTask(Guid Id)
        {
            //Lấy thông tin Task
            var task = _unitOfWork.ProductionManagementRepository.DevisionOfTask(Id);
            ViewBag.subtask = _unitOfWork.ProductionManagementRepository.DevisionOfTaskDetail(Id);

            //Configlist
            CreateViewBag(task.WorkFlowId);
            return PartialView(task);
        }
        public ActionResult _DivisionOfTaskDetail(Guid Id)
        {
            //Lấy subtask đã có
            var subtask = _unitOfWork.ProductionManagementRepository.DevisionOfTaskDetail(Id);
            if (subtask != null)
            {
                ViewBag.subtask = subtask.OrderBy(p => p.Summary);
            }
            CreateViewBag();
            return PartialView(subtask);
        }
        public ActionResult _DivisionOfTaskDetailUpdate(string LSXDT)
        {
            //Lấy danh sách đợt và LSX SAP đã có
            var subtask = _unitOfWork.ProductionManagementRepository.GetDevisionOfTaskDetailByLSXDT(LSXDT);
            //Lấy trạng thái của LSX SAP
            var workFlowId = _context.WorkFlowModel.Where(p => p.WorkFlowCode == ConstWorkFlow.LSXC).Select(p => p.WorkFlowId).FirstOrDefault();
            CreateViewBag(workFlowId);
            return PartialView("_DivisionOfTaskDetail", subtask);
        }
        public ActionResult _DivisionOfTaskStepUpdate(string LSXDT)
        {
            //Lấy danh sách đợt đã có
            var subtask = _unitOfWork.ProductionManagementRepository.GetDevisionOfTaskStepByLSXDT(LSXDT);
            CreateViewBag();
            return PartialView("_DivisionOfTaskStep", subtask);
        }
        public ActionResult _DetailProductionOrder(Guid WorkFlowId, int qty)
        {
            ViewBag.Qty = qty;
            CreateViewBag(WorkFlowId);
            return PartialView();
        }
        //Add row
        public ActionResult _AddRow(List<CreateSubTaskViewModel> createSubTaskViewModels, Guid WorkFlowId)
        {
            CreateViewBag(WorkFlowId);
            return PartialView(createSubTaskViewModels.OrderBy(p => p.Summary).ToList());
        }

        //Update
        public ActionResult _DivisionOfTaskBy(string LSXDT)
        {
            //Lấy thông tin task từ LSX DT để tách đợt
            var task = _unitOfWork.ProductionManagementRepository.DevisionOfTaskBy(LSXDT);
            Guid? WorkFlowId = Guid.Empty;
            if (task != null)
            {
                WorkFlowId = task.WorkFlowId;
            }
            //Configlist
            CreateViewBag(WorkFlowId);
            return PartialView("_DivisionOfTask", task);
        }
        #endregion END Chia task

        #region BEGIN Create task or sub task
        [HttpPost]
        public JsonResult Create(List<CreateSubTaskViewModel> createSubTaskViewModels, Guid? ParentTaskId)
        {
            return ExecuteContainer(() =>
            {
                //Có dữ liệu truyền lên để phân bổ
                if (createSubTaskViewModels != null)
                {

                    //TODO: Lấy dữ liệu task Cha
                    var ParentTask = _unitOfWork.TaskRepository.GetSubtaskInfo(ParentTaskId);
                    foreach (var createSubTaskViewModel in createSubTaskViewModels)
                    {
                        //Mã đợt
                        ParentTask.Summary = createSubTaskViewModel.Summary;
                        ParentTask.WorkFlowId = createSubTaskViewModel.WorkFlowId;
                        //Ngày bắt đầu
                        ParentTask.StartDate = createSubTaskViewModel.StartDate;
                        //Ngày dự kiến kết thúc
                        ParentTask.EstimateEndDate = createSubTaskViewModel.EstimateEndDate;
                        //Số lượng Kế hoạch
                        ParentTask.Qty = createSubTaskViewModel.Qty;
                        //Trạng thái
                        ParentTask.TaskStatusId = createSubTaskViewModel.TaskStatusId;
                        //TODO: Tạo view model task con
                        CreateTaskViewModel result = null;
                        if (createSubTaskViewModel.TaskId != null && createSubTaskViewModel.TaskId != Guid.Empty)
                        {
                            ParentTask.TaskId = createSubTaskViewModel.TaskId;
                            _unitOfWork.TaskRepository.Update(ParentTask);
                            _context.SaveChanges();
                        }
                        else
                        {
                            result = _unitOfWork.TaskRepository.CreateTask(ParentTask, null, ParentTask.taskAssignList, ParentTask.taskAssignGroupList, ParentTask.taskAssignPersonGroupList, ParentTask.taskReporterList, ParentTask.Type, false, null, CurrentUser, GoogleMapAPIKey);
                            if (!result.Success)
                            {
                                _Error("Lỗi phân bổ, vui lòng thử lại!");
                            }
                        }
                    }
                }
                else
                {
                    return _Error("Vui lòng nhập dữ liệu phân bổ!");
                }

                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.ProductionOrder.ToLower())
                });

            });
        }

        [HttpPost]
        public JsonResult CheckLSXSAPValid(List<DivisionOfTaskByLSXDTSubtaskViewModel> LSXSAPList)
        {
            return ExecuteContainer(() =>
            {
                bool IsValid = false;
                string Message = string.Empty;
                //Check trong LSX SAP:
                //1. SUM(SL ĐC) == SL KH => return true
                //2. SUM(SL ĐC) != SL KH => return false
                //======================================
                //Lấy danh sách LSX SAP => cùng lệnh 
                var LSXSAPNameList = LSXSAPList.Select(p => p.LSXSAP).Distinct().ToList();
                if (LSXSAPNameList != null && LSXSAPNameList.Count > 0)
                {
                    foreach (var item in LSXSAPNameList)
                    {
                        //SL KH
                        var SLKH = LSXSAPList.Where(p => p.LSXSAP == item).Select(p => p.Qty).FirstOrDefault();
                        //SL ĐC
                        var SLDC = LSXSAPList.Where(p => p.LSXSAP == item).Sum(p => p.Number2);
                        if (SLKH != SLDC)
                        {
                            Message = Message + string.Format("<p>SL KH và SL ĐC của LSX SAP \"{0}\" không đồng bộ!</p>", item);
                        }
                    }
                }

                if (string.IsNullOrEmpty(Message))
                {
                    IsValid = true;
                }
                else
                {
                    Message = Message + "<p> Vui lòng kiểm tra lại thông tin! Nhấn nút \"Lưu\" nếu bạn vẫn muốn lưu thay đổi này!</p>";
                }

                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    IsValid = IsValid,
                    Data = Message,
                });;

            });
        }

        [HttpPost]
        public JsonResult CreateDivisionOfTask(List<DivisionOfTaskByLSXDTViewModel> DotSXList, List<DivisionOfTaskByLSXDTSubtaskViewModel> LSXSAPList, List<DivisionOfTaskByLSXDTSubtaskViewModel> CongDoanList)
        {
            return ExecuteContainer(() =>
            {
                _unitOfWork.TaskRepository.CreateDivisionOfTask(DotSXList, LSXSAPList, CongDoanList, CurrentUser.CompanyId, CurrentUser.AccountId);

                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = "Đã lưu thông tin đợt sản xuất thành công!",
                    RedirectUrl = "/Work/Timeline",
                });

            });
        }
        #endregion END Create task or sub task
        private void CreateViewBag(Guid? WorkFlowId = null, Guid? TaskStatusId = null)
        {
            //Configlist
            if (WorkFlowId != null && WorkFlowId != Guid.Empty)
            {
                var configList = _context.WorkFlowConfigModel.Where(p => p.WorkFlowId == WorkFlowId).ToList();
                ViewBag.WorkFlowConfig = configList;
            }

            //ListTaskStatus
            //Lấy WorkFlowId của LSX SAP
            var WorkFlowId_LSXC = _unitOfWork.WorkFlowRepository.FindWorkFlowIdByCode(ConstWorkFlow.LSXC);
            //Lấy danh sách status theo workflowId của LSX SAP
            var lst = _unitOfWork.TaskStatusRepository.GetTaskStatusByWorkFlowId(WorkFlowId_LSXC);
            //Tạo viewBag để truyền danh sách status vào view
            ViewBag.TaskStatusList = new SelectList(lst, "TaskStatusId", "TaskStatusName");

            //Công đoạn thực hiện
            var stepLst = (from a in _context.PlantRoutingConfigModel
                            where a.Actived == true
                            && a.Attribute10 == CurrentUser.CompanyCode
                            orderby a.OrderIndex
                            select new ISDSelectStringItem()
                            {
                                id = SqlFunctions.StringConvert((double)a.PlantRoutingCode).Trim(),
                                name = a.PlantRoutingName
                            }).ToList();

            ViewBag.StepList = new SelectList(stepLst, "id", "name");
        }
    }
}