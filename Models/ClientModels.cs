using System.Collections.Generic;
using System.Data;
using tourismAPi.App_Code;

namespace tourismAPi.Models
{
    public class ClientClass
    {
        public string GetSearchModels(string userid, string username, string cuurip)
        {
            DataTable mainRows = new DataTable();
            List<dbparam> dbparamlist = new List<dbparam>();
            dbparamlist.Add(new dbparam("@userid", userid.Trim()));
            mainRows = new database().checkSelectSql("mssql", "sysstring", "select 姓名 from dbo.webperdata where 員工編號 = @userid;", dbparamlist);
            return mainRows.Rows.Count == 0 ? username : mainRows.Rows[0]["姓名"].ToString().TrimEnd();
        }
    }
}