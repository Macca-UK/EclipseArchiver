using System;

namespace DocumentArchiver.Helpers
{
    public static class DateTimeHelpers
    {
        /// <summary>
        /// Return formatted date
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToFormattedDate(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Return formatted date
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToFormattedDate(this DateTime? dateTime)
        {
            if (dateTime == null || !dateTime.HasValue)
                return string.Empty;

            return dateTime.Value.ToFormattedDate();
        }

        /// <summary>
        /// Return formatted date/time
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToFormattedDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }

        /// <summary>
        /// Return formatted date/time
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToFormattedDateTime(this DateTime? dateTime)
        {
            if (dateTime == null || !dateTime.HasValue)
                return string.Empty;

            return dateTime.Value.ToFormattedDateTime();
        }
    }
}