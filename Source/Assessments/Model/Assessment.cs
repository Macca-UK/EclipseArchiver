using System.Text;

namespace DocumentArchiver.Source.Assessments.Model
{
    public class Assessment
    {
        public Assessment()
        {
            Sections = new List<Section>();
        }

        // Assessment header details

        public int Id { get; set; }

        public string? AssessmentId { get; set; }

        public string? AssessmentPriority { get; set; }

        public string? Title { get; set; }

        public string? PartyId { get; set; }

        public string? ClientName { get; set; }

        public DateTime? ClientBirthDate { get; set; }

        public string? ClientAge { get; set; }

        public string? ClientGender { get; set; }

       //public string? ClientAddress { get; set; }

       //public string? ClientTelephone { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime ActivityDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? WorkerId { get; set; }

        public string? WorkerName { get; set; }

        public string? WorkerTeam { get; set; }

        //public string? Outcome { get; set; }

       // public string? OutcomeReason { get; set; }

        //public string? CompletedById { get; set; }

        public string? CompletedByName { get; set; }

        public DateTime? CompletedDate { get; set; }

        public string? CompletedByTeam { get; set; }

        public string? SubmittedById { get; set; }
        
        public string? SubmittedByName { get; set; }

        public DateTime? SubmittedDate { get; set; }

        //public bool IsAuthorisationRequired { get; set; }

        //public string? AuthorisedById { get; set; }

        //public string? AuthorisedByName { get; set; }

        //public DateTime? AuthorisedDate { get; set; }

        //public string? AuthorisationNotes { get; set; }

        //public string? AuthorisationStatus
        //{
        //    get
        //    {
        //        if (!IsAuthorisationRequired)
        //            return "Not required";

        //        return AuthorisedDate != null && AuthorisedDate.HasValue ? "Authorised" : "Not authorised";
        //    }
        //}

        //public string? DataSharing { get; set; }

        //public string? DataSharingNotes { get; set; }

        // Assessment Sections
        public IList<Section> Sections { get; private set; }

        // For testing
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("Id: {0}\n", Id);
            sb.AppendFormat("Assessment Id: {0}\n", AssessmentId);
            sb.AppendFormat("Title: {0}\n", Title);
            sb.AppendFormat("ClientId: {0}\n", PartyId);
            sb.AppendFormat("ClientName: {0}\n", ClientName);

            foreach (var section in Sections)
                sb.Append(section);

            return sb.ToString();
        }

        public DateTime? CreatedDate
        { get { return EndDate != null && EndDate.HasValue ? EndDate : StartDate; } }

        public string? FileType { get; set; }

        public string? TargetFileType { get; set; }
    }
}