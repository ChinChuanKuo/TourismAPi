using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using tourismAPi.App_Code;

namespace tourismAPi.Models
{
    public class InsertClass
    {
        public statusModels GetSendModels(bool traffic, bool location, string items, string cuurip)
        {
            DateTime dateTime = DateTime.Now;
            string[] tr = { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            var dataitems = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(items);
            foreach (var data in dataitems.Select((item, i) => new { i, item }))
            {
                if (data.item["userid"].ToString().Trim().Length > 0)
                {
                    if (data.item["userid"].ToString().Trim().Length != 6)
                    {
                        return new statusModels() { showWarn = true, status = $"Please enter the correct office id on the {tr[data.i]} lines" };
                    }
                }
                if (data.item["username"].ToString().Trim().Length == 0)
                {
                    return new statusModels() { showWarn = true, status = $"Please enter the correct name on the {tr[data.i]} lines" };
                }
                if (!Regex.IsMatch(data.item["idcard"].ToString().Trim(), @"^([a-zA-Z]+\d+|\d+[a-zA-Z]+)[a-zA-Z0-9]*$") || data.item["idcard"].ToString().Trim().Length == 0)
                {
                    return new statusModels() { showWarn = true, status = $"Please enter the correct id card on the {tr[data.i]} lines" };
                }
                if (!Regex.IsMatch(data.item["birthday"].ToString().Trim(), @"^[0-9]+$") || data.item["birthday"].ToString().Trim().Length != 8)
                {
                    return new statusModels() { showWarn = true, status = $"Please enter the correct birthday on the {tr[data.i]} lines" };
                }
                DateTime days = DateTime.Parse(DateTime.ParseExact(data.item["birthday"].ToString().Trim(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd"));
                if (dateTime.Year - days.Year > 150)
                {
                    return new statusModels() { showWarn = true, status = $"This Person is tool oldest on the {tr[data.i]} lines" };
                }
            }
            database database = new database();
            foreach (var item in dataitems)
            {
                List<dbparam> dbparamlist = new List<dbparam>();
                dbparamlist.Add(new dbparam("@idcard", item["idcard"].ToString().Trim()));
                if (database.checkActiveSql("mssql", "sysstring", "delete from web.tourism where idcard = @idcard;", dbparamlist) != "istrue")
                {
                    return new statusModels() { showWarn = true, status = "Please contact the engineer" };
                }
                dbparamlist.Add(new dbparam("@userid", item["userid"].ToString().Trim()));
                dbparamlist.Add(new dbparam("@username", item["username"].ToString().Trim()));
                dbparamlist.Add(new dbparam("@birthday", item["birthday"].ToString().Trim()));
                dbparamlist.Add(new dbparam("@traffic", traffic ? "搭車" : "自行前往"));
                dbparamlist.Add(new dbparam("@location", traffic ? location ? "內湖" : "林口" : ""));
                dbparamlist.Add(new dbparam("@ago", "0"));
                dbparamlist.Add(new dbparam("@money", traffic ? "0" : "-200"));
                if (database.checkActiveSql("mssql", "sysstring", "exec web.inserttourismitem @userid,@username,@idcard,@birthday,@traffic,@location,@ago,@money;", dbparamlist) != "istrue")
                {
                    return new statusModels() { showWarn = true, status = "Please contact the engineer" };
                }
            }
            return new statusModels() { showWarn = false, status = "Congratulations, Information Has Been Updated" };
        }
    }
}