using ISD.EntityModels;
using ISD.ViewModels;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.MES
{
    public class ConsumableMaterialsDeliveryRepository
    {
        EntityDataContext _context;
        public ConsumableMaterialsDeliveryRepository(EntityDataContext context)
        {
            _context = context;
        }

        public List<AllotmentResultViewModel> AllotBOM(AllotmentViewModel viewModel)
        {
            var result = new List<AllotmentResultViewModel>();
            #region Component
            //Build your record
            var tableComponentSchema = new List<SqlMetaData>(1)
                {
                    new SqlMetaData("Code", SqlDbType.NVarChar, 100),
                    new SqlMetaData("Quantity", SqlDbType.Decimal)
                }.ToArray();

            //And a table as a list of those records
            var tableComponent = new List<SqlDataRecord>();
            if (viewModel.dataAllotmentList != null && viewModel.dataAllotmentList.Count > 0)
            {
                foreach (var r in viewModel.dataAllotmentList)
                {
                    var tableRow = new SqlDataRecord(tableComponentSchema);
                    if (!string.IsNullOrEmpty(r.DataAllotment))
                    {
                        //tableRow.SetString(0, r.DataAllotment.TrimStart(new Char[] { '0' }));
                        tableRow.SetString(0, r.DataAllotment);
                        tableRow.SetDecimal(1, r.DataAllotmentQuantity.HasValue? r.DataAllotmentQuantity.Value : 0);
                    }
                    tableComponent.Add(tableRow);
                }
            }
            else
            {
                var tableRow = new SqlDataRecord(tableComponentSchema);
                tableComponent.Add(tableRow);
            }
            #endregion

            string sqlQuery = "EXEC [MES].[usp_AllotBOM] @DocumentDate, @StepCode, @ComponentList";

            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "DocumentDate",
                    Value = viewModel.DocumentDate ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "StepCode",
                    Value = viewModel.SelectedStepCode ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ComponentList",
                    TypeName = "[dbo].[StringQuantityList]", //Don't forget this one!
                    Value = tableComponent
                },
            };
            #endregion

            result = _context.Database.SqlQuery<AllotmentResultViewModel>(sqlQuery, parameters.ToArray()).ToList();
            return result;
        }

        public string Confirm(ConsumableMaterialsDeliveryFormViewModel viewModel, Guid? CurrentAccountId)
        {
            string error = string.Empty;
            if (viewModel.DocumentDate.HasValue && viewModel.detail.Count > 0)
            {
                foreach (var item in viewModel.detail)
                {
                    ConsumableMaterialsDeliveryModel modelAdd = new ConsumableMaterialsDeliveryModel();
                    modelAdd.ConsumableMaterialsDeliveryId = Guid.NewGuid();
                    modelAdd.DocumentDate = viewModel.DocumentDate;
                    modelAdd.ProductionOrderCode = item.LSXSAP;
                    modelAdd.ProductCode = item.ProductCode;
                    modelAdd.ProductDetailCode = item.ProductDetailCode;
                    modelAdd.ProductDetailName = item.ProductDetailName;
                    modelAdd.ProductDetailActualQty = item.ProductDetailActualQty;
                    modelAdd.BOM = item.BOM;
                    modelAdd.BOMUnit = item.BOMUnit;
                    modelAdd.BOMQty = item.BOMQty;
                    modelAdd.StepName = item.StepName;
                    modelAdd.ActualQty = item.ActualQty;
                    modelAdd.CreateBy = CurrentAccountId;
                    modelAdd.CreateTime = DateTime.Now;

                    _context.Entry(modelAdd).State = System.Data.Entity.EntityState.Added;
                }
                _context.SaveChanges();
            }
            else
            {
                error = "Vui lòng nhập đầy đủ thông tin phân bổ";
            }

            return error;
        }
    }
}
