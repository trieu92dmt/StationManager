namespace Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static int ConvertDateTimeToInt(DateTime? dateTime)
        {
            int dateKey;
            try
            {
                dateKey = Convert.ToInt32(string.Format("{0:yyyyMMdd}", dateTime));
                return dateKey;
            }
            catch
            {
                return 0;
            }
        }
    }
}
