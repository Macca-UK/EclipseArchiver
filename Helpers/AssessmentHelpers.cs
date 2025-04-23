using DocumentArchiver.Source.Assessments.Model;

namespace DocumentArchiver.Helpers
{
    public static class AssessmentHelpers
    {
        /// <summary>
        /// Return unencoded string - replacing newlines with html line breaks if necessary
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetFileName(this Assessment asm)
        {
            DateTime asmDate = asm.EndDate.HasValue ? asm.EndDate.Value : asm.StartDate;

            return string.Format("{0}_{1}_{2}_{3}", asm.PartyId, asmDate.ToString("yyyyMMdd"), asm.AssessmentId, FormatTitle(asm.Title));
        }

        /// <summary>
        /// Remove all invalid filename characters from string
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private static string FormatTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return string.Empty;
            }

            title = title.Trim();

            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()) + ".,;()";

            foreach (char c in invalid)
            {
                title = title.Replace(c.ToString(), "");
            }

            return title.Replace(' ', '_');
        }
    }
}