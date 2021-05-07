using System;
using System.Globalization;
using tourismAPi.App_Code;

namespace tourismAPi.Models
{
    public class BirthdayClass
    {
        public int GetSearchModels(string birthday, string cuurip)
        {
            return new datetime().differentday(DateTime.Parse("2021/06/18"), DateTime.Parse(DateTime.ParseExact(birthday, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd")));
        }
    }
}