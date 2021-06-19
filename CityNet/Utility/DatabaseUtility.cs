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


        public static double getDoubleValue(DataRow row, string key, double defaultvalue)
        {
            double ret = defaultvalue;
            Object obj = row[key];
            if (obj != null && obj.ToString().Length != 0)
            {
                if (!double.TryParse(obj.ToString(), out ret))
                {
                    ret = defaultvalue;
                }
            }
            return ret;
        }

        //得到里程标准格式 dk100+500
        //
        public static string getMileageText(string prefix, double value)
        {
            //得到千位
            double kilometer = value / 1000.0;
            int inkilo = (int)kilometer;
            double sub = value - inkilo * 1000; //百里范围内
            return prefix + inkilo.ToString() + "+" +  sub.ToString("f2");
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