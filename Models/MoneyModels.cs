using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using tourismAPi.App_Code;

namespace tourismAPi.Models
{
    public class MoneyClass
    {
        public string GetSearchModels(string categoryId, string traffic, string items, string cuurip)
        {
            int total = 0;
            bool showTraffic = bool.Parse(traffic);
            foreach (var item in JsonSerializer.Deserialize<List<Dictionary<string, object>>>(items))
            {
                bool showPlace = bool.Parse(item["showPlace"].ToString().Trim());
                switch (item["userid"].ToString().Trim())
                {
                    case "":
                        total += checkMoney(categoryId, showTraffic, showPlace, item["birthday"].ToString().Trim());
                        break;
                    default:
                        switch (checkOffice(item["userid"].ToString().Trim()))
                        {
                            case true:
                                switch (showTraffic)
                                {
                                    case false:
                                        total -= 200;
                                        break;
                                }
                                break;
                            default:
                                total += checkMoney(categoryId, showTraffic, showPlace, item["birthday"].ToString().Trim());
                                break;
                        }
                        break;
                }
            }
            return total.ToString().Trim();
        }

        public bool checkOffice(string userid)
        {
            database database = new database();
            List<dbparam> dbparamlist = new List<dbparam>();
            dbparamlist.Add(new dbparam("@userid", userid));
            return new database().checkSelectSql("mssql", "sysstring", "select username from web.siteber where userid = @userid;", dbparamlist).Rows.Count > 0;
        }

        public int checkMoney(string categoryId, bool traffic, bool place, string day)
        {
            switch (string.IsNullOrWhiteSpace(day))
            {
                case true:
                    return 0;
            }
            DateTime nowDate = DateTime.Now, birthday = DateTime.Parse(DateTime.ParseExact(day.ToString().Trim(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd"));
            int year = nowDate.Year - birthday.Year;
            if (year >= 12)
            {
                switch (categoryId)
                {
                    case "0":
                        return 1500;
                }
                switch (traffic)
                {
                    case true:
                        return 900;
                    default:
                        return 600;
                }
            }
            else if (year >= 6)
            {
                switch (categoryId)
                {
                    case "0":
                        return 1100;
                }
                switch (traffic)
                {
                    case true:
                        return 900;
                    default:
                        return 600;
                }
            }
            else if (year >= 3)
            {
                switch (categoryId)
                {
                    case "0":
                        return 650;
                    default:
                        return 730;
                }
            }
            switch (place)
            {
                case true:
                    return 300;
                default:
                    return 40;
            }
        }
    }
}