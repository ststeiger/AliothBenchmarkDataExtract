/*
SELECT 
	 bm_uid
	,bm_rubrique
	,bm_language
	,bm_secs
	,bm_kb
	,bm_gz
	,bm_cpu
	,bm_percent
	,bm_failed
	,bm_bad_output
	,bm_no_program
	,bm_no_programs
FROM t_benchmark 
WHERE bm_rubrique = 'fannkuch-redux'
-- WHERE bm_failed = 'true' 
-- WHERE bm_bad_output = 'true' 
-- WHERE bm_no_program = 'true' 
-- WHERE bm_no_programs = 'true' 
;
*/

--ALTER TABLE t_benchmark ADD COLUMN bm_fine boolean;


SELECT 
	 bm_uid
	,bm_rubrique
	,bm_language
	,bm_secs
	,bm_kb
	,bm_gz
	,bm_cpu
	--,bm_percent
	--,bm_failed
	--,bm_bad_output
	--,bm_no_program
	--,bm_no_programs
FROM t_benchmark
WHERE (1=1) 
AND bm_fine = 'true'  










update t_benchmark
set bm_fine = 'true'
WHERE (1=1) 
AND NOT  
(
	   COALESCE(bm_failed, 'false') = 'true' 
	OR COALESCE(bm_bad_output, 'false') = 'true' 
	OR COALESCE(bm_no_program, 'false') = 'true' 
	OR COALESCE(bm_no_program, 'false') = 'true' 
) 
