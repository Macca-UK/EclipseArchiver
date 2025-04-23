namespace DocumentArchiver.Source.Assessments.Sql
{
	public static class AssessmentAnswers
	{
		public const string Sql = @";WITH FormAnswers
AS
(
	SELECT 
		fa.instanceid AS assessment_id,
		efd.GlobalSequence AS GlobalSequence,
		efd.UpdatedGlobalSequence AS UpdatedGlobalSequence,
		fcv.sectionnumber AS section_id,
		ISNULL(fcv.pagetitle +' - ' +fcv.sectiontitle, CONCAT(fcv.pagetitle, ' - Section ', fcv.sectionnumber)) AS section_title,
		IIF(efd.GroupHasHeaderRow = 1 , ISNULL(efd.QuestionSet, efd.LocalID), efd.LocalID) AS question_id,
		NULL AS question_number,
		fcv.sectionnumber AS question_set_sequence, 
		fcv.subsectionnumber AS question_section,
		IIF(fcv.datatype = 'TEXT' AND efd.UpdatedRowNumber IS NULL, 'N', 'N') AS is_free_text,
		IIF(efd.GroupHasHeaderRow = 1, ISNULL(ev2.UpdatedQuestionText, efd.GroupName), COALESCE(ev1.UpdatedQuestionText, IIF(efd.GroupName IS NOT NULL AND efd.Label = 'Checkboxes', efd.GroupName, efd.Label))) AS question_text,
		NULL AS question_hint_text,
		NULL AS question_other_text,
		IIF(efd.TableNumber IS NOT NULL, RANK() OVER(PARTITION BY efd.DesignID,  efd.PageNumber, efd.UpdatedTableNumber ORDER BY efd.UpdatedRowNumber), 1) As row_number,
		NULL AS row_header,
		efd.ColumnNumber AS column_number,
		efd.ColumnHeader column_header,
		IIF(fcv.datasubtype = 'STATIC_TEXT' AND efd.GroupName IS NOT NULL, fcv.controllabel, fa.answervalue) AS answer_value
	FROM
		[WNC_LAS].[dbo].[EclipseFormDesign] efd
	JOIN
		[EclipseLive].[dbo].[formcontrolview] fcv ON efd.DesignID = fcv.designid
			AND
				efd.LocalID = fcv.controllocalid
	JOIN
		[EclipseLive].[dbo].[formanswerpersonview_indexed] fa ON efd.DesignID = fa.designid
			AND
				fcv.controllocalid = fa.controllocalid
	JOIN
		[EclipseLive].[dbo].[form_control] fc ON fcv.controlid = fc.id
	/* Rename based on LocalID */
	LEFT JOIN
		[WNC_LAS].[dbo].[EclipseFormAnswerElementView] ev1 ON efd.DesignGUID = ev1.DesignGIUD
			AND
				efd.LocalID = ev1.ControlID
			AND
				ev1.IncludeElement = 'Y'
	/* Rename based on QuestionSet */
	LEFT JOIN
		[WNC_LAS].[dbo].[EclipseFormAnswerElementView] ev2 ON efd.DesignGUID = ev2.DesignGIUD
			AND
				efd.QuestionSet = ev2.QuestionSet
			AND
				ev2.IncludeElement = 'Y'
	WHERE
		efd.IsHidden = 0
	AND
		efd.IsHeaderRow = 0
	AND
		efd.IsGroupQuestion = 0
	AND
		efd.IsTableQuestion = 0
	AND
		fa.instanceid = @0
	/* Group Answers (not using EclipseGroupedQuestionDesign) */
	UNION
	SELECT 
		fa.instanceid AS assessment_id,
		efd.GlobalSequence AS GlobalSequence,
		efd.UpdatedGlobalSequence AS UpdatedGlobalSequence,
		fcv.sectionnumber AS section_id,
		ISNULL(fcv.pagetitle +' - ' +fcv.sectiontitle, CONCAT(fcv.pagetitle, ' - Section ', fcv.sectionnumber)),
		CONCAT(efd.CollectionLocalID, '-', fa.recordindex) AS question_id,
		/*efd.CollectionLocalID AS question_id,*/
		NULL AS question_number,
		efd.sectionnumber AS question_set_sequence, 
		fcv.subsectionnumber question_section,
		'N' AS is_free_text,
		/*COALESCE(ev.UpdatedQuestionText, efd.GroupName, fcv.controllabel) AS question_text,*/
		COALESCE(ev.UpdatedQuestionText, efd.GroupName, fcv.sectiontitle, CONCAT('Section ', fcv.sectionnumber)) AS question_text,
		NULL AS question_hint_text,
		NULL AS question_other_text,
		efd.UpdatedRowNumber As row_number,
		efd.Label AS row_header,
		fa.recordindex AS column_number,
		efd.RowHeader column_header,
		IIF(fcv.datasubtype = 'STATIC_TEXT', fcv.controllabel, fa.answervalue) AS answer_value
	FROM
		[WNC_LAS].[dbo].[EclipseFormDesign] efd
	JOIN
		[EclipseLive].[dbo].[formcontrolview] fcv ON efd.DesignID = fcv.designid
			AND	
				efd.LocalID = fcv.controllocalid
	LEFT JOIN
		[EclipseLive].[dbo].[formanswerpersonview_indexed] fa ON fcv.DesignID = fa.designid
			AND
				fcv.controllocalid = fa.controllocalid
	/* Rename based on CollectionID */
	LEFT JOIN
		[WNC_LAS].[dbo].[EclipseFormAnswerElementView] ev ON efd.DesignGUID = ev.DesignGIUD
			AND
				efd.CollectionLocalID = ev.CollectionID
			AND
				ev.IncludeElement = 'Y'
	WHERE
		efd.IsHeaderRow = 0
	AND
		efd.IsGroupQuestion = 1
	AND
		fa.instanceid = @0
	/* Table Answers (not using EclipseGroupedQuestionDesign) */
	UNION
	SELECT 
		fa.instanceid AS assessment_id,
		efd.GlobalSequence AS GlobalSequence,
		efd.UpdatedGlobalSequence AS UpdatedGlobalSequence,
		fcv.sectionnumber AS section_id,
		ISNULL(fcv.pagetitle +' - ' +fcv.sectiontitle, CONCAT(fcv.pagetitle, ' - Section ', fcv.sectionnumber)) AS Section_Title,
		/*CONCAT(efd.CollectionLocalID, '-', fa.recordindex) AS question_id,*/
		efd.CollectionLocalID AS question_id,
		NULL AS question_number,
		efd.sectionnumber AS question_set_sequence, 
		fcv.subsectionnumber question_section,
		'N' AS is_free_text,
		COALESCE(ev.UpdatedQuestionText, IIF(efd.RepeatingGroupName LIKE '%Collection%', 'Details', efd.RepeatingGroupName), efd.GroupName, fcv.sectiontitle, CONCAT('Section ', fcv.sectionnumber)) AS question_text,
		NULL AS question_hint_text,
		NULL AS question_other_text,
		fa.recordindex As row_number,
		efd.RowHeader AS row_header,
		efd.ColumnNumber AS column_number,
		efd.Label AS column_header,
		IIF(fcv.datasubtype = 'STATIC_TEXT', fcv.controllabel, fa.answervalue) AS answer_value
	FROM
		[WNC_LAS].[dbo].[EclipseFormDesign] efd
	JOIN
		[EclipseLive].[dbo].[formcontrolview] fcv ON efd.DesignID = fcv.designid
			AND	
				efd.LocalID = fcv.controllocalid
	JOIN
		[EclipseLive].[dbo].[formanswerpersonview_indexed] fa ON fcv.DesignID = fa.designid
			AND
				fcv.controllocalid = fa.controllocalid
	/* Rename based on CollectionID */
	LEFT JOIN
		[WNC_LAS].[dbo].[EclipseFormAnswerElementView] ev ON efd.DesignGUID = ev.DesignGIUD
			AND
				efd.CollectionLocalID = ev.CollectionID
			AND
				ev.IncludeElement = 'Y'
	WHERE
		efd.IsHeaderRow = 0
	AND
		efd.IsTableQuestion = 1
	AND
		fa.instanceid = @0
)
SELECT
	assessment_id,
	GlobalSequence,
	section_id,
	section_title,
	question_id,
	question_number,
	question_set_sequence,
	question_section,
	is_free_text,
	question_text,
	question_hint_text,
	question_other_text,
	[row_number],
	row_header,
	column_number,
	column_header,
	answer_value
FROM
	FormAnswers
	
ORDER BY
	UpdatedGlobalSequence,
	question_id,
	[row_number],
	column_number";
	}
}