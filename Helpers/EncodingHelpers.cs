namespace DocumentArchiver.Helpers
{
    public static class EncodingHelpers
    {
        /// <summary>
        /// Return unencoded string - replacing newlines with html line breaks if necessary
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FormatBreaks(this string input)
        {
            // If null, or already html-formatted, return as-is
            if (string.IsNullOrWhiteSpace(input) || input.Contains("<br"))
                return input;

            // Otherwise replace newlines with breaks
            return input.Trim().Replace(Environment.NewLine, "<br/>");
        }
    }
}