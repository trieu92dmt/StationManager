namespace ISD.API.Constant.MESP2
{
    public static class MESP2Constant
    {
        public struct CheckType
        {
            /// <summary>
            /// Đạt
            /// </summary>
            public const string CheckIn = "CHECKIN";
            /// <summary>
            /// Không đạt
            /// </summary>
            public const string CheckOut = "CHECKOUT";
        }

        public struct EquipmentStatusDisplay
        {
            public const string Blank = "Trống";
            public const string Stand = "Canh máy";
            public const string Active = "Hoạt động";
            public const string Clean = "Vệ sinh";
            public const string Stop = "Ngưng máy";
        }

        public struct EquipmentStatusCode
        {
            public const string Blank = "T";
            public const string Stand = "CM";
            public const string Active = "HĐ";
            public const string Clean = "VS";
            public const string Stop = "NM";
        }

        public struct OutputRecordType
        {
            /// <summary>
            /// Đạt
            /// </summary>
            public const string D = "D";
            /// <summary>
            /// Không đạt
            /// </summary>
            public const string KD = "KD";
        }

        /// <summary>
        /// MovementType ghi nhận sản lượng
        /// </summary>
        public struct MovementTypeOR
        {
            /// <summary>
            /// Ghi nhận sản lượng
            /// </summary>
            public const string ADD = "ADD";
            /// <summary>
            /// Chuyển công đoạn
            /// </summary>
            public const string TRANSFER = "TRANSFER";
        }

        /// <summary>
        /// Danh sách danh mục
        /// </summary>
        public struct CataTypeCode
        {
            /// <summary>
            /// Trạng thái máy móc
            /// </summary>
            public const string EquipmentWorkStatus = "EquipmentWorkStatus";

            /// <summary>
            /// Lý do ngưng máy
            /// </summary>
            public const string EquipmentStopReason = "EquipmentStopReason";

        }
    }
}
