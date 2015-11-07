
using System.Windows.Forms;


namespace BenchmarkDataExtract
{

    // Google Sheets:
    // IMPORTHTML("http://stackoverflow.com/questions/20000791/language-benchmark-page-from-debian-alioth","table",1)


    // http://benchmarksgame.alioth.debian.org/
    // http://stackoverflow.com/questions/20000791/language-benchmark-page-from-debian-alioth
    // http://stackoverflow.com/questions/6157153/how-can-i-get-the-source-codes-for-the-computer-language-benchmarks-game-form
    
    public partial class Form1 : System.Windows.Forms.Form
    {


        public Form1()
        {
            InitializeComponent();
        }



        private void btnFetch_Click(object sender, System.EventArgs e)
        {
            System.Data.DataTable dt = GetAllBenchmarks();
            this.dataGridView1.DataSource = dt;
        } // End Sub btnFetch_Click 


        private void btnInsert_Click(object sender, System.EventArgs e)
        {
            Insert_Benchmarks();
        } // End Sub button2_Click 


        private System.Data.DataTable GetAllBenchmarks()
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            //HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument(); doc.Load(@"");
            HtmlAgilityPack.HtmlDocument doc = web.Load(@"http://benchmarksgame.alioth.debian.org/");


            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Url", typeof(string));

            System.Data.DataRow dr = null;

            foreach (HtmlAgilityPack.HtmlNode link in doc.DocumentNode.SelectNodes("//section[1]//li/a[@href]"))
            {
                dr = dt.NewRow();
                // System.Console.WriteLine(link);
                dr["Name"] = System.Web.HttpUtility.HtmlDecode(link.InnerText);
                dr["Url"] = link.Attributes["href"].Value;

                dt.Rows.Add(dr);
            } // Next link

            
            System.Data.DataView dv = dt.DefaultView;
            dv.Sort = "Name ASC";
            System.Data.DataTable sortedDT = dv.ToTable();

            return sortedDT;
        } // End Function GetAllBenchmarks


        private void Insert_Benchmarks()
        {
            ExecuteNonQuery("DELETE FROM __Bench");

            using (System.Data.DataTable dtBenchs = GetAllBenchmarks())
            {

                int count = -1;
                foreach (System.Data.DataRow dr in dtBenchs.Rows)
                {
                    count++;
                    string path = System.Convert.ToString(dr["Url"]);
                    System.Data.DataTable dt = GetData(path);

                    System.Data.DataView dv = dt.DefaultView;
                    //dv.Sort = "Name ASC";
                    System.Data.DataTable sortedDT = dv.ToTable();
                    this.dataGridView1.DataSource = sortedDT;


                    string SQL = @"
INSERT INTO __Bench
(
	 bm_uid
	,url
    ,rubrique
	,column_1
	,secs
	,KB
	,gz
	,cpu
	,column_6
)
VALUES
( 
	 NEWID() --bm_uid -- uniqueidentifier
	,@__url -- nvarchar(1000)
    ,@__Rubrique -- nvarchar(200)
	,@__COLUMN_1 -- nvarchar(200)
	,@__secs -- nvarchar(200)
	,@__KB -- nvarchar(200)
	,@__gz -- nvarchar(200)
	,@__cpu -- nvarchar(200)
	,@__COLUMN_6 -- nvarchar(200))
)
;
";
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    foreach (System.Data.DataRow drData in sortedDT.Rows)
                    {
                        string str = SQL;

                        foreach (System.Data.DataColumn dc in sortedDT.Columns)
                        {
                            string strData = System.Convert.ToString(drData[dc.ColumnName]);
                            strData = strData.Replace("'", "''");
                            str = str.Replace("@__" + dc.ColumnName, "'" + strData + "'");
                        } // Next dc 

                        sb.AppendLine(str);
                        //System.Data.SqlClient.SqlCommand
                    } // Next drData 

                    string strExecute = sb.ToString();
                    sb.Length = 0;
                    sb = null;

                    // System.Console.WriteLine(strExecute);
                    ExecuteNonQuery(strExecute);

                    // if(count == 1) break;
                } // Next dr 

            }  // End Using dtBenchs


            string sqlUpdate = @"
UPDATE __Bench
   SET failed = CASE WHEN KB = 'failed' THEN 'true' ELSE NULL END 
      ,bad_output = CASE WHEN KB = 'Bad Output' THEN 'true' ELSE NULL END 
      ,no_program = CASE WHEN secs = 'No programs' THEN 'true' ELSE NULL END 
      ,no_programs = CASE WHEN secs = 'No program' THEN 'true' ELSE NULL END 
;

	
UPDATE __Bench SET fail_army = COALESCE(failed, bad_output, no_program, no_programs, 'false'); 


UPDATE __Bench SET secs = null WHERE secs IN ('', '?');
UPDATE __Bench SET kb = null WHERE kb IN ('', '?');
UPDATE __Bench SET gz = null WHERE gz IN ('', '?');
UPDATE __Bench SET cpu = null WHERE cpu IN ('', '?');


UPDATE __Bench SET fine = 'false';

UPDATE __Bench 
	SET fine = 'true' 
WHERE (1=1) 
AND NOT 
(
	   COALESCE(failed, 'false') = 'true' 
	OR COALESCE(bad_output, 'false') = 'true' 
	OR COALESCE(no_program, 'false') = 'true' 
	OR COALESCE(no_programs, 'false') = 'true' 
) 
AND NOT 
(
	   secs	IS NULL 
	OR kb IS NULL 
	OR gz IS NULL 
	OR cpu IS NULL 
)
;

";

            ExecuteNonQuery(sqlUpdate);


            // SELECT * FROM __Bench WHERE (1=1) AND rubrique = 'fasta-redux' AND column_1 = 'Java' 
            string sqlFixJunkData = @"
UPDATE __Bench SET fine = 'false' 
WHERE (1=1) 
 
AND 
(
	(
		rubrique = 'pidigits' AND column_1 = 'Java' AND url IN  ('./u64q/lua.html', './u64q/javascript.html')
	)
	OR 
	(
		rubrique = 'regex-dna' AND column_1 = 'Java' AND url IN  ('./u64q/pascal.html')
	)
	OR 
	(
		rubrique = 'reverse-complement' AND column_1 = 'Java' AND url IN  ('./u64q/lua.html')
	)
	OR
	(
		rubrique = 'mandelbrot' AND column_1 = 'Java' AND url IN  ('./u64q/javascript.html')
	)
	OR 
	(
		rubrique = 'fasta' AND column_1 = 'Java' AND url IN  ('./u64q/lua.html')
	)
)

";
            ExecuteNonQuery(sqlFixJunkData);

        } // End Sub Insert_Benchmarks


        public static void ExecuteNonQuery(string strSQL)
        {
            System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder();
            csb.DataSource = System.Environment.MachineName;
            csb.InitialCatalog = "TestDB";
            csb.IntegratedSecurity = true;

            string conString = csb.ConnectionString;

            using (System.Data.IDbConnection con = new System.Data.SqlClient.SqlConnection(conString))
            {
                using (System.Data.IDbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = strSQL;

                    if (cmd.Connection.State != System.Data.ConnectionState.Open)
                        cmd.Connection.Open();

                    cmd.ExecuteNonQuery();

                    if (cmd.Connection.State != System.Data.ConnectionState.Closed)
                        cmd.Connection.Close();
                } // End Using cmd 
                
            } // End Using con

        } // End Sub ExecuteNonQuery


        private System.Data.DataTable GetData(string path)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            string baseURL = "http://benchmarksgame.alioth.debian.org/";
            string URL = baseURL + path;

            dt.Columns.Add("url", typeof(string));
            dt.Columns.Add("Rubrique", typeof(string));
            System.Data.DataRow dr = null;


            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            //HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument(); doc.Load(@"");
            HtmlAgilityPack.HtmlDocument doc = web.Load(URL);


            string rubrique = null;
            bool haveHeaders = false;

            foreach (HtmlAgilityPack.HtmlNode link in doc.DocumentNode.SelectNodes("//table[1]//tr"))
            {
                dr = dt.NewRow();
                // System.Console.WriteLine(link);

                HtmlAgilityPack.HtmlNode a = link.SelectSingleNode("./th[@colspan=\"3\"]//a");

                if (a != null)
                {
                    rubrique = a.InnerText;
                    // System.Console.WriteLine(rubrique);
                    continue;
                } // End if (a != null) 


                var tableHeaders = link.SelectNodes("./th");

                if (tableHeaders != null)
                {
                    if (haveHeaders)
                        continue;

                    int count = 0;
                    foreach (HtmlAgilityPack.HtmlNode th in tableHeaders)
                    {
                        count++;
                        string colname= th.InnerText.Trim(new char[] { ' ', '\t', '\r', '\n' });
                        // System.Console.WriteLine(colname);

                        if (string.IsNullOrEmpty(colname))
                            colname = "COLUMN_" + count.ToString();

                        if (!dt.Columns.Contains(colname))
                        {
                            dt.Columns.Add(colname, typeof(string));
                        }
                        
                    } // Next th 

                    continue;
                } // End if (tableHeaders != null) 



                var tableData = link.SelectNodes("./td");

                if (tableData != null)
                {

                    dr = dt.NewRow();

                    dr["url"] = path;
                    dr["Rubrique"] = rubrique;

                    int count = 1;
                    foreach (HtmlAgilityPack.HtmlNode td in tableData)
                    {
                        count++;
                        string val = td.InnerText.Trim(new char[] { ' ', '\t', '\r', '\n' });
                        val = System.Web.HttpUtility.HtmlDecode(val);
                        val = val.Replace(",", "");

                        dr[count] = val;
                    } // Next td

                    dt.Rows.Add(dr);
                    continue;
                } // End if (tableData != null) 

                // System.Console.WriteLine(link);

                // dr["Name"] = System.Web.HttpUtility.HtmlDecode(link.InnerText);
                // dr["Url"] = link.Attributes["href"].Value;

            } // Next link
            
            return dt;
        } // End Function GetData


    } // End Class 


} // End Namespace 

