
USE [TestDb];

-- DELETE FROM __Bench 
/*
INSERT INTO __Bench2
(
	 bm_uid
	,rubrique
	,column_1
	,secs
	,KB
	,gz
	,cpu
	,column_6
) 
SELECT 
	 MAX(CAST(BM_UID AS varchar(36))) AS BM_UID 
	,rubrique
	,column_1
	,secs
	,NULLIF(KB, '?') AS KB
	,gz
	,cpu
	,column_6
FROM __Bench 

WHERE (1=1) 
-- AND rubrique = 'spectral-norm' 

AND NOT 
(
	   secs = 'No program' 
	OR secs = 'No programs' 
	OR KB = 'failed'
	OR KB = 'Bad Output'
)

GROUP BY 
	 rubrique
	,column_1
	,secs
	,KB
	,gz
	,cpu
	,column_6
	
	
ORDER BY rubrique, column_1 

*/


INSERT INTO __Bench2
(
	 bm_uid
	,rubrique
	,column_1
	,failed
	,bad_output
	,no_program
	,no_programs
) 
SELECT 
	 MAX(CAST(BM_UID AS varchar(36))) AS BM_UID 
	,rubrique
	,column_1
	,failed
	,bad_output
	,no_program
	,no_programs
FROM __Bench 

WHERE (1=1) 
-- AND rubrique = 'spectral-norm' 

-- AND NOT 
AND   
(
	   secs = 'No program' 
	OR secs = 'No programs' 
	OR KB = 'failed'
	OR KB = 'Bad Output'
)

GROUP BY 
	 rubrique
	,column_1
	,secs
	,KB
	,gz
	,cpu
	,column_6
	,failed
	,bad_output
	,no_program
	,no_programs
	
ORDER BY rubrique, column_1 
