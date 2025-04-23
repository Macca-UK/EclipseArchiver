using System.Text;

namespace DocumentArchiver.Source.Assessments.Model
{
    public class Answer
    {
        private readonly List<Row> _rows = new List<Row>();

        public Answer()
        {
            Header = new Row();
        }

        /// <summary>
        /// Set the header row
        /// </summary>
        /// <param name="header"></param>
        public void SetHeader(Row header)
        {
            if (header != null)
                Header = header;
        }

        public void SetOtherAnswer(string otherAnswer)
        {
            OtherAnswer = otherAnswer;
        }

        /// <summary>
        /// Add a data row
        /// </summary>
        /// <param name="row"></param>
        public void AddRow(Row row)
        {
            if (row != null)
                _rows.Add(row);
        }

        /// <summary>
        /// Add a collection of data rows
        /// </summary>
        /// <param name="rows"></param>
        public void AddRows(params Row[] rows)
        {
            if (rows != null)
                _rows.AddRange(rows);
        }

        /// <summary>
        /// Header row
        /// </summary>
        public Row Header { get; private set; }

        /// <summary>
        /// Data rows
        /// </summary>
        public IEnumerable<Row> Rows
        { get { return _rows.AsEnumerable(); } }

        /// <summary>
        /// The number of cells in this row
        /// </summary>
        public int CellCount
        { get { return HasRowHeaderValues ? Header.CellCount : Header.CellCount - 1; } }

        /// <summary>
        /// Does this row contain any non-empty column header values?
        /// </summary>
        public bool HasColumnHeaderValues
        { get { return Header.Cells.Any(x => !string.IsNullOrWhiteSpace(x)); } }

        /// <summary>
        /// Does this row contain any non-empty row header values?
        /// </summary>
        public bool HasRowHeaderValues
        { get { return Rows.Any(x => !string.IsNullOrWhiteSpace(x.Cells.First())); } }

        /// <summary>
        /// Does this answer have any answer values?
        /// </summary>
        public bool HasAnswerValues
        { get { return Rows.Any(x => x.HasAnswerValue); } }

        /// <summary>
        /// Is this a simple (one-element) answer?
        /// </summary>
        public bool IsSimpleAnswer
        { get { return CellCount == 1 && Rows.Count() == 1; } }

        public string? FirstAnswer
        { get { return _rows[0].FirstAnswer; } }

        public string? OtherAnswer { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Header);

            foreach (var row in Rows)
                sb.Append(row.ToString());

            return sb.ToString();
        }
    }
}