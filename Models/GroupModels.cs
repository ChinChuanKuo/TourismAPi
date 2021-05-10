using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using tourismAPi.App_Code;

namespace tourismAPi.Models
{
    public class GroupClass
    {
        public sitemsModels GetSearchModels(string licenses, string cuurip)
        {
            DataTable mainRows = new DataTable();
            List<dbparam> dbparamlist = new List<dbparam>();
            dbparamlist.Add(new dbparam("@groupid", licenses));
            mainRows = new database().checkSelectSql("mssql", "sysstring", "select userid,username,idcard,birthday,gender,place,category,traffic,location from web.tourism where groupid = @groupid order by id asc;", dbparamlist);
            switch (mainRows.Rows.Count)
            {
                case 0:
                    return new sitemsModels() { showWarn = true };
            }
            List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
            foreach (DataRow dr in mainRows.Rows)
            {
                items.Add(new Dictionary<string, object>() { { "userid", dr["userid"].ToString().TrimEnd() }, { "username", dr["username"].ToString().TrimEnd() }, { "idcard", dr["idcard"].ToString().TrimEnd() }, { "birthday", dr["birthday"].ToString().TrimEnd() }, { "showGender", dr["gender"].ToString().TrimEnd() == "1" }, { "showHolder", new datetime().differentday(DateTime.Parse("2021/06/18"), DateTime.Parse(DateTime.ParseExact(dr["birthday"].ToString().TrimEnd(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd"))) < 3 }, { "showPlace", dr["place"].ToString().TrimEnd() == "1" } });
            }
            return new sitemsModels() { category = mainRows.Rows[0]["category"].ToString().TrimEnd(), traffic = mainRows.Rows[0]["traffic"].ToString().TrimEnd() == "搭車", location = mainRows.Rows[0]["location"].ToString().TrimEnd() == "內湖", items = items, showWarn = false };
        }
    }
}