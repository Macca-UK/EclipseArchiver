using System.Text;

namespace DocumentArchiver.Source.Assessments.Model
{
    public class Question
    {
        public Question()
        {
            Answer = new Answer();
        }

        public string? Number { get; set; }

        public string? Title { get; set; }

        public string? Hint { get; set; }

        public string? OtherText { get; set; }

        public bool IsFreeText { get; set; }

        public Answer Answer { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("--Question--");
            sb.AppendFormat("Title: {0}\n", Title);
            sb.AppendFormat("Hint: {0}\n", Hint);
            sb.AppendFormat("OtherText: {0}\n", OtherText);

            sb.Append(Answer.ToString());

            return sb.ToString();
        }
    }
}