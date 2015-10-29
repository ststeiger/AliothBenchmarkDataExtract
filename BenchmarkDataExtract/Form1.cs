
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



        private void button1_Click(object sender, System.EventArgs e)
        {
            System.Data.DataTable dt = GetAllBenchmarks();
            this.dataGridView1.DataSource = dt;
        } // End Sub button1_Click 


        private void button2_Click(object sender, System.EventArgs e)
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
            System.Data.DataTable dtBenchs = GetAllBenchmarks();

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

        } // End Sub 


        private System.Data.DataTable GetData(string path)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            string baseURL = "http://benchmarksgame.alioth.debian.org/";
            string URL = baseURL + path;

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

                    dr["Rubrique"] = rubrique;

                    int count = 0;
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

