namespace DocumentArchiver.Source.Assessments.Sql
{
	public static class AssessmentHeader
	{
		public const string Sql = @"

SELECT 
	fi.instanceid AS assessment_id,
	fd.designname AS form_type,
	pv.personid AS client_id,
	CONCAT(pv.title, ' ', pv.forename, ' ', pv.surname) client_name,
	pv.dateofbirth client_dob,
	[WNC_BASE].[dbo].[GetAgeForDate](pv.dateofbirth, fi.activitydate) AS client_age,
	ISNULL(pv.gender, 'Not Recorded') client_gender,
	fi.datestarted AS form_start_date,
	fi.datecompleted AS form_end_date,
	fi.activitydate AS activity_date,
	w.personid AS worker_id,
	CONCAT(w.title +' ', w.forename +' ', w.surname) AS worker_name,
	fi.creatorteamname AS worker_team,
	fi.datecompleted AS completed_date,
	CONCAT(compw.title +' ', compw.forename +' ', compw.surname) AS completed_by_name,
	fi.completerteamname AS completed_by_team,
	fi.datesubmitted AS submitted_date,
	sw.personid AS submitted_id,
	NULLIF(CONCAT(sw.title +' ', sw.forename +' ', sw.surname), '') AS submitted_by_name
FROM
	[EclipseLive].[dbo].[forminstanceview] fi
JOIN
	[EclipseLive].[dbo].[formdesignview] fd ON fi.designid = fd.designid
JOIN
	[EclipseLive].[dbo].[personview] pv ON fi.personid = pv.personid
LEFT JOIN
	[EclipseLive].[dbo].[personview] w ON fi.creatorpersonid = w.personid
LEFT JOIN
	[EclipseLive].[dbo].[personview] sw ON fi.submitterpersonid = sw.personid
LEFT JOIN
	[EclipseLive].[dbo].[personview] compw ON fi.completerpersonid = compw.personid
WHERE 
	fi.instanceid = @0";
	}
}