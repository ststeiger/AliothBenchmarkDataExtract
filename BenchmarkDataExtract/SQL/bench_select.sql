
SELECT 
	 bm_uid
	,bm_rubrique
	,bm_language
	,bm_secs -- execution time 
	,bm_kb  -- memory size
	,bm_gz -- sourcecode gz size
	,bm_cpu -- 
        ,bm_percent
	--,bm_failed
	--,bm_bad_output
	--,bm_no_program
	--,bm_no_programs
FROM t_benchmark
WHERE (1=1) 
AND bm_fine = 'true'  

--and bm_secs IS NULL 
--and bm_kb IS NULL
--and bm_gz IS NULL
 --and bm_cpu is NULL 
