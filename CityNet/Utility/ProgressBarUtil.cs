using CityNet.Controllers;
using CityNet.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.Utility
{
    public class ProgressBarUtil
    {
        public static void delete(string uniqueID)
        {
            string sql = "delete from ProgressBar where UniqueID =@uid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@uid", uniqueID));
            DBAccess.NoQuery(sql, list);
        }

        public static void insert(string uniqueID, string text, int totalNum, int current, string description)
        {
            IList list = new ArrayList();
            string sql = "insert ProgressBar([UniqueID],[Text],[TotakProcess],[CurrentPos],[Descriptoin]) " +
                "values(@uid,@txt,@tp,@cp,@des)";

            list.Add(new DictionaryEntry("@uid", uniqueID));
            list.Add(new DictionaryEntry("@txt", text));
            list.Add(new DictionaryEntry("@tp", totalNum));
            list.Add(new DictionaryEntry("@cp", 0));
            list.Add(new DictionaryEntry("@des", description));
            DBAccess.NoQuery(sql, list);
        }

        public static void updateProgressText(string uniqueID, string value)
        {
            IList list = new ArrayList();
            string sql = "update ProgressBar set [Text] = @val where [UniqueID] = @uid";
            list.Add(new DictionaryEntry("@val", value));
            list.Add(new DictionaryEntry("@uid", uniqueID));
            DBAccess.NoQuery(sql, list);
        }

        public static void updateProgress(string uniqueID,int value)
        {
            IList list = new ArrayList();
            string sql = "update ProgressBar set [CurrentPos] = @val where [UniqueID] = @uid";

            list.Add(new DictionaryEntry("@val", value));
            list.Add(new DictionaryEntry("@uid", uniqueID));
            DBAccess.NoQuery(sql, list);
        }

        public static int getProgress(string uniqueID)
        {
            string sql = "select [CurrentPos] from ProgressBar where [UniqueID] =@uid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@uid", uniqueID));
            int progress = -1;


            DataSet ds = DBAccess.Query(sql, "Progressbar", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        progress = int.Parse(row["CurrentPos"].ToString());
                    }
                }
            }
            return progress;
        }

        public static ProgressInfo getProgressInfo(string uniqueID)
        {
            ProgressInfo info = new ProgressInfo();

            string sql = "select * from ProgressBar where [UniqueID] =@uid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@uid", uniqueID));

            //  string unique = "";

            DataSet ds = DBAccess.Query(sql, "Progressbar", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        info.Current = int.Parse(row["CurrentPos"].ToString());
                        info.Description = row["Descriptoin"].ToString();
                        info.Text = row["Text"].ToString();
                        info.Total = int.Parse(row["TotakProcess"].ToString());
                        info.UniqueID = uniqueID;
                    }
                }
            }
            return info;
        }
    }
}