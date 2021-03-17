using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.Utility
{
    public class DatabaseUtility
    {
        public static int getIntValue(DataRow row, string key, int defaultvalue)
        {
            int ret = defaultvalue;
            Object obj = row[key];
            if (obj != null && obj.ToString().Length != 0)
            {
                if (!int.TryParse(obj.ToString(), out ret))
                {
                    ret = defaultvalue;
                }
            }
            return ret;
        }

        public static string getDatetimeValue(DataRow row, string key)
        {
            string ret = "";
            Object obj = row[key];
            if (obj != null && obj.ToString().Length != 0)
            {
                DateTime dt = (DateTime)obj;
                ret = dt.ToString("yyyy年MM月dd日");
            }
            return ret;
        }

        public static string getDatetimeValue(DataRow row, string key, string format)
        {
            string ret = "";
            Object obj = row[key];
            if (obj != null && obj.ToString().Length != 0)
            {
                DateTime dt = (DateTime)obj;
                ret = dt.ToString(format);
            }
            return ret;
        }

        public static string getStringValue(DataRow row, string key)
        {
            string ret = "";
            Object obj = row[key];
            if (obj != null && obj.ToString().Length != 0)
            {
                ret = obj.ToString();
            }
            return ret.Trim();
        }
    }
}