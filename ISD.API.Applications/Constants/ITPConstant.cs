namespace ISD.API.Applications.Constants
{
    public class ITPConstant
    {
        /// <summary>
        /// Loại chuyển công đoạn
        /// </summary>
        public struct TypeStageTranfer 
        {
            public const string StageTranfer = "StageTranferType";
        }

        public struct QCTypeCode
        {
            //QC nguyên vật liệu đầu vào
            public const string QCNVLInput = "1";
            //QC NVL xuất vào sx
            public const string QCExport = "2";
            //QC công đoạn sx
            public const string QCProduction = "3";
            //QC TP xuất bán
            public const string QCSell = "4";
        }

        public struct QCTypeName
        {
            //QC nguyên vật liệu đầu vào
            public const string QCNVLInput = "QCNVLInput";
            //QC NVL xuất vào sx
            public const string QCExport = "QCExport";
            //QC công đoạn sx
            public const string QCProduction = "QCProduction";
            //QC TP xuất bán
            public const string QCSell = "QCSell";
        }
    }
}
