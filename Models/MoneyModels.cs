using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using tourismAPi.App_Code;

namespace tourismAPi.Models
{
    public class MoneyClass
    {
        public string GetSearchModels(string categoryId, string traffic, string items, string cuurip)
        {
            int total = 0;
            bool showTraffic = bool.Parse(traffic);
            foreach (var item in JsonSerializer.Deserialize<List<Dictionary<string, object>>>(items).Select((value, i) => new { i, value }))
            {
                bool showPlace = bool.Parse(item.value["showPlace"].ToString().Trim());
                int money = checkMoney(categoryId, item.i, showTraffic, showPlace, item.value["birthday"].ToString().Trim());
                switch (item.value["userid"].ToString().Trim())
                {
                    case "":
                        total += money < 0 ? 0 : money;
                        break;
                    default:
                        switch (checkOffice(item.value["userid"].ToString().Trim()))
                        {
                            case false:
                                total += money < 0 ? 0 : money;
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

        public int checkMoney(string categoryId, int count, bool traffic, bool place, string day)
        {
            if (!Regex.IsMatch(day, @"^[0-9]+$") || day.Length != 8) {
                return 0;
            }
            DateTime nowDate = DateTime.Now, birthday = DateTime.Parse(DateTime.ParseExact(day.ToString().Trim(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd"));
            int year = nowDate.Year - birthday.Year;
            if (year >= 12)
            {
                switch (categoryId)
                {
                    case "0":
                        switch (traffic)
                        {
                            case true:
                                return 1500;
                        }
                        return 1240;
                }
                switch (count)
                {
                    case 1:
                        switch (traffic)
                        {
                            case true:
                                return 200;
                            default:
                                return -300;
                        }
                }
                switch (traffic)
                {
                    case true:
                        return 860;
                    default:
                        return 600;
                }
            }
            else if (year >= 6)
            {
                switch (categoryId)
                {
                    case "0":
                        switch (traffic)
                        {
                            case true:
                                return 1100;
                        }
                        return 840;
                }
                switch (count)
                {
                    case 1:
                        switch (traffic)
                        {
                            case true:
                                return 200;
                            default:
                                return -300;
                        }
                }
                switch (traffic)
                {
                    case true:
                        return 860;
                    default:
                        return 600;
                }
            }
            else if (year >= 3)
            {
                switch (categoryId)
                {
                    case "0":
                        switch (traffic)
                        {
                            case true:
                                return 650;
                        }
                        return 390;
                }
                switch (count)
                {
                    case 1:
                        switch (traffic)
                        {
                            case true:
                                return 100;
                            default:
                                return -400;
                        }
                }
                switch (traffic)
                {
                    case true:
                        return 730;
                    default:
                        return 470;
                }
            }
            switch (categoryId)
            {
                case "0":
                    switch (traffic)
                    {
                        case true:
                            switch (place)
                            {
                                case true:
                                    return 300;
                            }
                            return 40;
                    }
                    return 40;
            }
            switch (count)
            {
                case 1:
                    switch (traffic)
                    {
                        case true:
                            switch (place)
                            {
                                case true:
                                    return -300;
                            }
                            return -400;
                        default:
                            return -500;
                    }
            }
            switch (traffic)
            {
                case true:
                    switch (place)
                    {
                        case true:
                            return 300;
                    }
                    return 40;
            }
            return 40;
        }
    }
}