using DocumentArchiver.Source.Assessments.Model;

namespace DocumentArchiver.Source.Assessments
{
    internal class AssessmentMapper
    {
        /// <summary>
        /// Map Swift assessment data to an Assessment view model
        /// </summary>
        /// <param name="header"></param>
        /// <param name="answers"></param>
        /// <returns></returns>
        public Assessment MapFrom(DbAssessmentHeader header, IList<DbAssessmentAnswer> answers)
        {
            var asm = new Assessment()
            {
                AssessmentId = header.assessment_id,
                AssessmentPriority = header.assessment_priority,
                Title = header.form_type,
                PartyId = header.client_id,
                ClientName = header.client_name,
                ClientBirthDate = header.client_dob,
                ClientAge = header.client_age,
                ClientGender = header.client_gender,
                //ClientAddress = header.client_address,
                //ClientTelephone = header.client_telephone,
                StartDate = header.form_start_date,
                ActivityDate = header.activity_date,
                EndDate = header.form_end_date,
                WorkerId = header.worker_id,
                WorkerName = header.worker_name,
                WorkerTeam = header.worker_team,
              //Outcome = header.form_outcome,
              //OutcomeReason = header.form_outcome_reason,
              //CompletedById = header.completed_by_id,
                CompletedByName = header.completed_by_name,
                CompletedDate = header.completed_date,
                CompletedByTeam = header.completed_by_team,
                SubmittedById = header.submitted_by_id,
                SubmittedByName = header.submitted_by_name,
                SubmittedDate = header.submitted_date
                //IsAuthorisationRequired = header.authorisation_required != null && header.authorisation_required == "Y",
                //AuthorisedById = header.authorised_by_id,
                //AuthorisedByName = header.authorised_by_name,
                //AuthorisedDate = header.authorised_date,
                //AuthorisationNotes = header.authorisation_notes,
                //DataSharing = header.share_data,
                //DataSharingNotes = header.share_data_notes
            };

            // No data
            if (answers == null || answers.Count == 0)
                return asm;

            // Otherwise map answers
            asm.AssessmentId = answers.First().assessment_id;

            var grouped = answers.GroupBy(x => new { x.section_id, x.section_title, x.question_id, x.question_text })
                                 .GroupBy(x => new { x.Key.section_id, x.Key.section_title });

            foreach (var sectionGroup in grouped)
            {
                var section = new Section() { Title = sectionGroup.Key.section_title };
                foreach (var questionGroup in sectionGroup)
                {
                    var first = questionGroup.First();

                    string questionNumber = string.Empty;

                    //// Don't number free-text questions
                    if (first.is_free_text == "N")
                    {
                        questionNumber = string.Format("{0}.{1}", first.section_sequence, first.question_sequence);
                    }

                    var question = new Question()
                    {
                        Number = questionNumber,
                        Title = questionGroup.Key.question_text,
                        Hint = first.question_hint_text,
                        OtherText = first.question_other_text,
                        IsFreeText = first.is_free_text == "Y"
                    };

                    // Get the column headers from the first row items
                    var columnHeaders = questionGroup.Where(x => x.row_number == 1)
                                                     .Select(x => x.column_header)
                                                     .ToArray();

                    question.Answer.SetHeader(new Row(string.Empty, columnHeaders));

                    question.Answer.SetOtherAnswer(first.answer_other_value);

                    section.Questions.Add(question);

                    // Load the rows
                    var answerRows = questionGroup.GroupBy(x => new { x.row_number, x.row_header })
                                                  .Select(
                                                    g => new Row(g.Key.row_header, g.Select(i => i.answer_value).ToArray())
                                                  ).ToArray();

                    question.Answer.AddRows(answerRows);
                }

                asm.Sections.Add(section);
            }

            return asm;
        }
    }
}