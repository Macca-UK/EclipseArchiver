using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentArchiver.Source.Assessments.Model
{
    public class Row
    {
        private readonly List<string> _cells = new List<string>();

        public Row(string rowheader, params string[] cells)
        {
            _cells.Add(rowheader ?? string.Empty);

            if (cells != null)
                _cells.AddRange(cells);
        }

        public Row()
            : this(string.Empty)
        { }

        public IEnumerable<string> Cells
        { get { return _cells.AsEnumerable(); } }

        public int CellCount
        { get { return _cells.Count(); } }

        /// <summary>
        /// Does this row contain any non-empty answer values?
        /// </summary>
        public bool HasAnswerValue
        { get { return _cells.Any(x => !string.IsNullOrWhiteSpace(x)); } }

        /// <summary>
        /// Get first answer value in row
        /// (ignoring_cells[0], which is the row header)
        /// </summary>
        public string FirstAnswer
        { get { return _cells[1]; } }

        public override string ToString()
        {
            var sb = new StringBuilder();

            bool bFirst = true;
            foreach (var cell in Cells)
            {
                if (!bFirst)
                    sb.Append("\t");
                else
                    bFirst = false;

                sb.Append(cell);
            }

            sb.Append("\n");

            return sb.ToString();
        }
    }
}