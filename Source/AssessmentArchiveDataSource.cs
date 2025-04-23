using System.Diagnostics;
using DocumentArchiver.Source.Assessments;
using DocumentArchiver.Source.Assessments.Sql;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NPoco;

namespace DocumentArchiver.Source
{
    internal class AssessmentArchiveDataSource
    {
        private readonly AssessmentMapper assessmentMapper = new AssessmentMapper();
        private readonly string EclipseConnectionString;

        public AssessmentArchiveDataSource(IConfigurationRoot? configRoot)
        {
            if (configRoot == null)
            {
                throw new ArgumentNullException(nameof(configRoot), "Configuration cannot be null.");
            }
            EclipseConnectionString = configRoot.GetConnectionString("Eclipse") ?? "";
        }

        public object GetArchiveData(string recordId)
        {
            try
            {
                using (var context = new Database(EclipseConnectionString, DatabaseType.SqlServer2012, SqlClientFactory.Instance))
                {
                    var header = context.SingleOrDefault<DbAssessmentHeader>(Sql.Builder
                        .Append(AssessmentHeader.Sql, recordId));

                    if (header != null)
                    {
                        var answers = context.Query<DbAssessmentAnswer>(Sql.Builder
                            .Append(AssessmentAnswers.Sql, recordId))
                            .ToList();

                        var asm = assessmentMapper.MapFrom(header, answers);
                        return asm;
                    }
                    else
                    {
                        throw new Exception($"Assessment {recordId} not found.");
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("The following error has occurred for \"{0}\"", recordId);
                Trace.TraceError("Exception: {0}", e.Message);
                if (e.InnerException != null)
                    Trace.TraceError("Inner Exception: {0}", e.InnerException.Message);
                Trace.TraceError("Stack Trace: {0}", e.StackTrace);
                throw;
            }
        }
    }

/// <summary>
/// This represents the assessment header data: client details, assessment type, dates and status etc.
/// </summary>
internal class DbAssessmentHeader
    {
        public string? assessment_id { get; set; }

        public string? assessment_priority { get; set; }

        public string? form_type { get; set; }

        public string? client_id { get; set; }

        public string? client_name { get; set; }

        public DateTime? client_dob { get; set; }

        public string? client_age { get; set; }

        public string? client_gender { get; set; }

        //public string? client_address { get; set; }

        //public string? client_telephone { get; set; }

        public DateTime form_start_date { get; set; }

        public DateTime activity_date { get; set; }

        public DateTime? form_end_date { get; set; }

        //public string? form_outcome { get; set; }

        //public string? form_outcome_reason { get; set; }

        public string? worker_id { get; set; }

        public string? worker_name { get; set; }

        public string? worker_team { get; set; }

        public string? share_data { get; set; }

        public string? share_data_notes { get; set; }

        public DateTime? completed_date { get; set; }

       // public string? completed_by_id { get; set; }

        public string? completed_by_name { get; set; }

        public string? completed_by_team { get; set; }

        public DateTime? submitted_date { get; set; }

        public string? submitted_by_id { get; set; }

        public string? submitted_by_name { get; set; }

        // public string? authorisation_required { get; set; }

        // public DateTime? authorised_date { get; set; }

        //public string? authorised_by_id { get; set; }


        //public string? authorised_by_name { get; set; }

        //public string? authorisation_notes { get; set; }
    }

    /// <summary>
    /// This represents an individual assessment answer row
    /// </summary>
    internal class DbAssessmentAnswer
    {
        public string? assessment_id { get; set; }

        public string? section_id { get; set; }

        public string? section_title { get; set; }

        public int section_sequence { get; set; }

        public string? question_id { get; set; }

        public int question_sequence { get; set; }

        public int question_section { get; set; }

        public string? is_free_text { get; set; }

        public string? question_text { get; set; }

        public string? question_hint_text { get; set; }

        public string? question_other_text { get; set; }

        public int row_number { get; set; }

        public string? row_header { get; set; }

        public int column_number { get; set; }

        public string? column_header { get; set; }

        public string? answer_value { get; set; }

        public string? answer_other_value { get; set; }
    }
}