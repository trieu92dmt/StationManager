using System.Collections.Generic;

namespace ISD.API.Core.Sortings.Reports
{
    public class EquipmentReportSorting
    {
        public static Dictionary<string, string> Mapping = new Dictionary<string, string>
        {
                { "workshopname"                        , "WorkShopName" },
                { "equipmentname"                       , "EquipmentName" },
                { "numberworker"                        , "NumberWorker" },
                { "type"                                , "Type" },
                { "workordercode"                       , "WorkOrderCode" },
                { "quantity"                            , "Quantity" },
                { "productcode"                         , "ProductCode" },
                { "kssxnumber"                          , "KSSXNumber" },
                { "hourstart"                           , "HourStart" },
                { "hourend"                             , "HourEnd" },
                { "status"                              , "Status" },
        };
    }
}
