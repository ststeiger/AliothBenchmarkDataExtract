
CREATE TABLE dbo.__Bench
(
	bm_uid uniqueidentifier NOT NULL,
	rubrique nvarchar(200) NULL,
	column_1 nvarchar(200) NULL,
	secs nvarchar(200) NULL,
	KB nvarchar(200) NULL,
	gz nvarchar(200) NULL,
	cpu nvarchar(200) NULL,
	column_6 nvarchar(200) NULL,
	failed bit NULL,
	bad_output bit NULL,
	no_program bit NULL,
	no_programs bit NULL
);


CREATE TABLE dbo.__Bench2
(
	bm_uid uniqueidentifier NOT NULL,
	rubrique nvarchar(200) NULL,
	column_1 nvarchar(200) NULL,
	secs float NULL,
	KB float NULL,
	gz float NULL,
	cpu float NULL,
	column_6 nvarchar(200) NULL,
	failed bit NULL,
	bad_output bit NULL,
	no_program bit NULL,
	no_programs bit NULL
);

UPDATE [__Bench]
   SET [failed] = CASE WHEN KB = 'failed' THEN 'true' ELSE NULL END 
      ,[bad_output] = CASE WHEN KB = 'Bad Output' THEN 'true' ELSE NULL END 
      ,[no_program] = CASE WHEN secs = 'No programs' THEN 'true' ELSE NULL END 
      ,[no_programs] = CASE WHEN secs = 'No program' THEN 'true' ELSE NULL END 





-- SELECT DISTINCT rubrique FROM __Bench ORDER BY 1 
SELECT DISTINCT column_1 FROM __Bench WHERE column_1 <> ' ' ORDER BY 1



SELECT 
	 rubrique
	,column_1
	,secs
	,KB
	,gz
	,cpu
	,column_6
FROM TestDb.dbo.__Bench
GROUP BY 
	 rubrique
	,column_1
	,secs
	,KB
	,gz
	,cpu
	,column_6
	