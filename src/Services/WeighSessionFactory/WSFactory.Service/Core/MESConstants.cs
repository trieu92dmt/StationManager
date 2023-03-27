namespace WSFactory.Service.Core
{
    public static class MESConstants
    {
        //Cân sản xuất
        public struct ScaleProduction
        {
            /// <summary>
            /// Cân nguyên liệu đầu vào để sản xuất số 1 (150kg)
            /// </summary>
            public const string NVL_Input1 = "A100_40";
            /// <summary>
            /// Cân nguyên liệu đầu vào để sản xuất số 2 (150kg)
            /// </summary>
            public const string NVL_Input2 = "A100_41";

            /// <summary>
            /// 150Kg Cân thành phẩm đầu ra 1
            /// </summary>
            public const string NVL_Output1 = "A100_42";
            /// <summary>
            /// 150Kg Cân thành phẩm đầu ra 2
            /// </summary>
            public const string NVL_Output2 = "A100_43";

            /// <summary>
            /// 150Kg Cân tấm thành phẩm đầu ra 1
            /// </summary>
            public const string TTP_Output1 = "A100_44";
            /// <summary>
            /// 150Kg Cân tấm thành phẩm đầu ra 2
            /// </summary>
            public const string TTP_Output2 = "A100_45";

        }
    }
}
