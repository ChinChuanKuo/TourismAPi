using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using tourismAPi.App_Code;

namespace tourismAPi.Models
{
    public class InsertClass
    {
        public statusModels GetSendModels(string categoryId, bool traffic, bool location, string items, string cuurip)
        {
            string[] tr = { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            var dataitems = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(items);
            if (string.IsNullOrWhiteSpace(dataitems[0]["userid"].ToString().Trim()))
            {
                return new statusModels() { showWarn = true, status = $"Please enter the office id on the {tr[0]} lines" };
            }
            List<Dictionary<string, object>> officeitems = new List<Dictionary<string, object>>(), familyitems = new List<Dictionary<string, object>>();
            foreach (var data in dataitems.Select((item, i) => new { i, item }))
            {
                if (data.item["username"].ToString().Trim().Length == 0) return new statusModels() { showWarn = true, status = $"Please enter the correct name on the {tr[data.i]} lines" };
                if (!Regex.IsMatch(data.item["idcard"].ToString().Trim(), @"^([a-zA-Z]+\d+|\d+[a-zA-Z]+)[a-zA-Z0-9]*$") || data.item["idcard"].ToString().Trim().Length == 0) return new statusModels() { showWarn = true, status = $"Please enter the correct id card on the {tr[data.i]} lines" };
                if (!Regex.IsMatch(data.item["birthday"].ToString().Trim(), @"^[0-9]+$") || data.item["birthday"].ToString().Trim().Length != 8) return new statusModels() { showWarn = true, status = $"Please enter the correct birthday on the {tr[data.i]} lines" };
                int year = new datetime().differentday(DateTime.Parse("2021/06/18"), DateTime.Parse(DateTime.ParseExact(data.item["birthday"].ToString().Trim(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd")));
                if (year > 150) return new statusModels() { showWarn = true, status = $"This Person is tool oldest on the {tr[data.i]} lines" };
                if (year < 0) return new statusModels() { showWarn = true, status = $"This Person is from the future？" };
                switch (new MoneyClass().checkOffice(data.item["userid"].ToString().Trim()))
                {
                    case true:
                        officeitems.Add(new Dictionary<string, object>() { { "userid", data.item["userid"].ToString().Trim() }, { "username", data.item["username"].ToString().Trim() }, { "idcard", data.item["idcard"].ToString().Trim() }, { "birthday", data.item["birthday"].ToString().Trim() }, { "showGender", data.item["showGender"].ToString().TrimEnd() }, { "showPlace", data.item["showPlace"].ToString().Trim() } });
                        break;
                    default:
                        familyitems.Add(new Dictionary<string, object>() { { "username", data.item["username"].ToString().Trim() }, { "idcard", data.item["idcard"].ToString().Trim() }, { "birthday", data.item["birthday"].ToString().Trim() }, { "showGender", data.item["showGender"].ToString().TrimEnd() }, { "showPlace", data.item["showPlace"].ToString().Trim() } });
                        break;
                }
            }
            database database = new database();
            DataTable mainRows = new DataTable();
            List<dbparam> dbparamlist = new List<dbparam>();
            dbparamlist.Add(new dbparam("@userid", officeitems[0]["userid"].ToString().Trim()));
            dbparamlist.Add(new dbparam("@count", JsonSerializer.Deserialize<List<Dictionary<string, object>>>(items).Count));
            mainRows = database.checkSelectSql("mssql", "sysstring", "exec web.checkalltourism @userid,@count;", dbparamlist);
            dbparamlist.Clear();
            dbparamlist.Add(new dbparam("@userid", officeitems[0]["userid"].ToString().Trim()));
            database.checkActiveSql("mssql", "sysstring", "exec web.deletealltourism @userid;", dbparamlist);
            List<string> offices = new List<string>();
            switch (mainRows.Rows[0]["issafe"].ToString().Trim())
            {
                case "1":
                    foreach (var item in officeitems)
                    {
                        bool showPlace = bool.Parse(item["showPlace"].ToString().Trim());
                        dbparamlist.Clear();
                        dbparamlist.Add(new dbparam("@groupid", mainRows.Rows[0]["groupid"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@userid", item["userid"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@username", item["username"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@idcard", item["idcard"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@birthday", item["birthday"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@gender", bool.Parse(item["showGender"].ToString().Trim()) ? "1" : "0"));
                        dbparamlist.Add(new dbparam("@place", "1"));
                        dbparamlist.Add(new dbparam("@category", categoryId));
                        dbparamlist.Add(new dbparam("@traffic", traffic ? "搭車" : "自行前往"));
                        dbparamlist.Add(new dbparam("@location", traffic ? location ? "內湖" : "林口" : ""));
                        dbparamlist.Add(new dbparam("@ago", new datetime().differentday(DateTime.Parse("2021/06/18"), DateTime.Parse(DateTime.ParseExact(item["birthday"].ToString().Trim(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd")))));
                        dbparamlist.Add(new dbparam("@money", categoryId == "0" ? traffic ? 0 : -200 : traffic ? -400 : -500));
                        dbparamlist.Add(new dbparam("@indate", mainRows.Rows[0]["indate"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@intime", mainRows.Rows[0]["intime"].ToString().Trim()));
                        if (new database().checkActiveSql("mssql", "sysstring", "exec web.inserttourismitem @groupid,@userid,@username,@idcard,@birthday,@gender,@place,@category,@traffic,@location,@ago,@money;", dbparamlist) != "istrue")
                        {
                            return new statusModels() { showWarn = true, status = "Please contact the engineer" };
                        }
                        offices.Add(item["username"].ToString().Trim());
                    }
                    int i = officeitems.Count;
                    var orditems = familyitems.OrderBy(e => e["birthday"].ToString().Trim());
                    foreach (var item in orditems)
                    {
                        bool showPlace = bool.Parse(item["showPlace"].ToString().Trim());
                        int ago = new datetime().differentday(DateTime.Parse("2021/06/18"), DateTime.Parse(DateTime.ParseExact(item["birthday"].ToString().Trim(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd")));
                        dbparamlist.Clear();
                        dbparamlist.Add(new dbparam("@groupid", mainRows.Rows[0]["groupid"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@userid", ""));
                        dbparamlist.Add(new dbparam("@username", item["username"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@idcard", item["idcard"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@birthday", item["birthday"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@gender", bool.Parse(item["showGender"].ToString().Trim()) ? "1" : "0"));
                        dbparamlist.Add(new dbparam("@place", ago < 3 ? traffic ? showPlace ? "1" : "0" : "1" : "1"));
                        dbparamlist.Add(new dbparam("@category", categoryId));
                        dbparamlist.Add(new dbparam("@traffic", traffic ? "搭車" : "自行前往"));
                        dbparamlist.Add(new dbparam("@location", traffic ? location ? "內湖" : "林口" : ""));
                        dbparamlist.Add(new dbparam("@ago", ago));
                        dbparamlist.Add(new dbparam("@money", new MoneyClass().checkMoney(categoryId, i, traffic, showPlace, item["birthday"].ToString().Trim())));
                        dbparamlist.Add(new dbparam("@indate", mainRows.Rows[0]["indate"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@intime", mainRows.Rows[0]["intime"].ToString().Trim()));
                        if (new database().checkActiveSql("mssql", "sysstring", "exec web.inserttourismitem @groupid,@userid,@username,@idcard,@birthday,@gender,@place,@category,@traffic,@location,@ago,@money;", dbparamlist) != "istrue")
                        {
                            return new statusModels() { showWarn = true, status = "Please contact the engineer" };
                        }
                        i++;
                    }
                    break;
                default:
                    foreach (var item in officeitems)
                    {
                        dbparamlist.Clear();
                        dbparamlist.Add(new dbparam("@groupid", mainRows.Rows[0]["groupid"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@userid", item["userid"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@username", item["username"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@idcard", item["idcard"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@birthday", item["birthday"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@gender", bool.Parse(item["showGender"].ToString().Trim()) ? "1" : "0"));
                        dbparamlist.Add(new dbparam("@place", "1"));
                        dbparamlist.Add(new dbparam("@category", categoryId));
                        dbparamlist.Add(new dbparam("@traffic", traffic ? "搭車" : "自行前往"));
                        dbparamlist.Add(new dbparam("@location", traffic ? location ? "內湖" : "林口" : ""));
                        dbparamlist.Add(new dbparam("@ago", new datetime().differentday(DateTime.Parse("2021/06/18"), DateTime.Parse(DateTime.ParseExact(item["birthday"].ToString().Trim(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd")))));
                        dbparamlist.Add(new dbparam("@money", categoryId == "0" ? traffic ? 0 : -200 : traffic ? -400 : -500));
                        if (new database().checkActiveSql("mssql", "sysstring", "exec web.inserttourismitem @groupid,@userid,@username,@idcard,@birthday,@gender,@place,@category,@traffic,@location,@ago,@money;", dbparamlist) != "istrue")
                        {
                            return new statusModels() { showWarn = true, status = "Please contact the engineer" };
                        }
                        offices.Add(item["username"].ToString().Trim());
                    }
                    i = officeitems.Count;
                    orditems = familyitems.OrderBy(e => e["birthday"].ToString().Trim());
                    foreach (var item in orditems)
                    {
                        bool showPlace = bool.Parse(item["showPlace"].ToString().Trim());
                        int ago = new datetime().differentday(DateTime.Parse("2021/06/18"), DateTime.Parse(DateTime.ParseExact(item["birthday"].ToString().Trim(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd")));
                        dbparamlist.Clear();
                        dbparamlist.Add(new dbparam("@groupid", mainRows.Rows[0]["groupid"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@userid", ""));
                        dbparamlist.Add(new dbparam("@username", item["username"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@idcard", item["idcard"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@birthday", item["birthday"].ToString().Trim()));
                        dbparamlist.Add(new dbparam("@gender", bool.Parse(item["showGender"].ToString().Trim()) ? "1" : "0"));
                        dbparamlist.Add(new dbparam("@place", ago < 3 ? traffic ? showPlace ? "1" : "0" : "1" : "1"));
                        dbparamlist.Add(new dbparam("@category", categoryId));
                        dbparamlist.Add(new dbparam("@traffic", traffic ? "搭車" : "自行前往"));
                        dbparamlist.Add(new dbparam("@location", traffic ? location ? "內湖" : "林口" : ""));
                        dbparamlist.Add(new dbparam("@ago", new datetime().differentday(DateTime.Parse("2021/06/18"), DateTime.Parse(DateTime.ParseExact(item["birthday"].ToString().Trim(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd")))));
                        dbparamlist.Add(new dbparam("@money", new MoneyClass().checkMoney(categoryId, i, traffic, showPlace, item["birthday"].ToString().Trim())));
                        if (new database().checkActiveSql("mssql", "sysstring", "exec web.inserttourismitem @groupid,@userid,@username,@idcard,@birthday,@gender,@place,@category,@traffic,@location,@ago,@money;", dbparamlist) != "istrue")
                        {
                            return new statusModels() { showWarn = true, status = "Please contact the engineer" };
                        }
                        i++;
                    }
                    break;
            }
            switch (mainRows.Rows[0]["issafe"].ToString().Trim())
            {
                case "0":
                    dbparamlist.Clear();
                    dbparamlist.Add(new dbparam("@mAddrName", string.Join(',', offices)));
                    dbparamlist.Add(new dbparam("@mAddrBCCName", "郭晉全"));
                    dbparamlist.Add(new dbparam("@mSubject", "旅遊報名更新序號"));
                    dbparamlist.Add(new dbparam("@mBody", $"<div style='width: 300px;text-align:center;'><div style='padding: 12px; border:2px solid white;'><div><h3 style='color: red;'>FN SYSTEM NEWS</h3></div><div> <hr /></div><div><a href='http://221.222.222.16:4500/tourism/2021money.pdf' style='color: red;'>旅遊金額對應表</a></div><div><h3 style='color: red;'>旅遊報名更新序號</h3></div><div style='font-size: 16px;'>{new datetime().sqldate("mssql", "sysstring")} {new datetime().sqltime("mssql", "sysstring")}</div><div><h4>請勿遺失此更新序號</h4></div><div>更新序號：{mainRows.Rows[0]["groupid"].ToString().Trim()}</div></div></div>"));
                    database.checkActiveSql("mssql", "mailstring", "insert into dbo.MailBox (mAddrName,mAddrBCCName,mSubject,mBody) values (@mAddrName,@mAddrBCCName,@mSubject,@mBody);", dbparamlist);
                    break;
            }
            return new statusModels() { license = mainRows.Rows[0]["groupid"].ToString().Trim(), showWarn = false, status = "Congratulations, Information Has Been Updated" };
        }
    }
}