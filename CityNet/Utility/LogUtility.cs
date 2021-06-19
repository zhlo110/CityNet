using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CityNet.Controllers;

namespace CityNet.Utility
{
    public class LogUtility
    {
        public static Boolean Login(string username,string password)
        {
              String sql = "select Count(*) from [User] where UserName = @un and PassWord = @pw";

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@un", username));
            list.Add(new DictionaryEntry("@pw", password));
            int size = DBAccess.QueryStatistic(sql, list);
            if (size > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetGroupIDByUserID(int userID)
        {
            String sql = "select [GroupID] from User_Group where [UserID] = @uid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@uid", userID));
            int ID = -1;
            DataSet dataset = DBAccess.Query(sql, "User_Group", list);
            if (dataset != null)
            {
                int nCount = dataset.Tables.Count;
                if (nCount > 0)
                {
                    DataTable dt = dataset.Tables[0];
                    nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        ID = int.Parse(row["GroupID"].ToString());
                    }
                }
            }
            return ID;
        }

        public static string getRealName(int id)
        {
            String sql = "select [RealName] from [User] where [ID] = @id";

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@id", id));
            string realname = "";

            DataSet dataset = DBAccess.Query(sql, "User", list);
            if (dataset != null)
            {
                int nCount = dataset.Tables.Count;
                if (nCount > 0)
                {
                    DataTable dt = dataset.Tables[0];
                    nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        realname = row["RealName"].ToString();
                    }
                }
            }
            return realname;
        }

        //获取部门名称
        public static string getDepartmentName(int departmentid)
        {
            string departmentname = "";
            string sql = "select DepartmentName from [Department] where ID = @id";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@id", departmentid));
            DataSet dataset = DBAccess.Query(sql, "Department", list);
            if (dataset != null)
            {
                int nCount = dataset.Tables.Count;
                if (nCount > 0)
                {
                    DataTable dt = dataset.Tables[0];
                    nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        departmentname = DatabaseUtility.getStringValue(row, "DepartmentName");
                    }
                }
            }
            return departmentname;
        }

        public static int GetUserID(string username, string password)
        {
            String sql = "select [ID] from [User] where UserName = @un and PassWord = @pw";

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@un", username));
            list.Add(new DictionaryEntry("@pw", password));
            int ID = -1;

            DataSet dataset = DBAccess.Query(sql, "User", list);
            if (dataset != null)
            {
                int nCount = dataset.Tables.Count;
                if (nCount > 0)
                {
                    DataTable dt = dataset.Tables[0];
                    nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        ID = int.Parse(row["ID"].ToString());
                    }
                }
            }
            return ID;
        }
    }
}