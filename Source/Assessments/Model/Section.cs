using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentArchiver.Source.Assessments.Model
{
    public class Section
    {
        public Section()
        {
            Questions = new List<Question>();
        }

        public string Title { get; set; } = "";

        public IList<Question> Questions { get; set; }

        public int MaxCellCount
        { get { return Questions.Max(x => x.Answer.CellCount); } }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("==SECTION==");
            sb.AppendFormat("Title: {0}\n", Title);

            foreach (var question in Questions)
                sb.Append(question);

            return sb.ToString();
        }
    }
}