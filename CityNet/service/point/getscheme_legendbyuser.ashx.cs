using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.point
{
    /// <summary>
    /// getscheme_legendbyuser 的摘要说明
    /// </summary>
    public class getscheme_legendbyuser : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string ret = getErrorMessage();
            string children = "";
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);

            // 从Task_Visible取得TaskID
            // task必须是完成的审批的项目
            string sql = "select tv.TaskID,taskview.priority from Task_Visible tv left join Task_View taskview on tv.TaskID=taskview.ID " +
                         "where tv.UserID=@uid and taskview.priority=4";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@uid", userid));
            HashSet<int> taskset = new HashSet<int>();//所有的TaskID
            DataSet ds = DBAccess.Query(sql, "Task_Visible", list);
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    for (i = 0; i < nCount; i++)
                    {
                        DataRow row = dt.Rows[i];
                        int taskid = DatabaseUtility.getIntValue(row, "TaskID", -1);
                        taskset.Add(taskid);
                    }
                }
            }
            IList taskids = new ArrayList();
            foreach (int value in taskset)
            {
                taskids.Add(value);
            }
            string inconditon = CityNet.Utility.Point.getincondition(taskids);

            sql = "select ID,Name,color from TableScheme where ID in" +
                "(select distinct SchemeID from Point_User_View where TaskID in(" + inconditon + ") and UserID=@uid)";
        //    IList list = new ArrayList();
            list.Clear();
            list.Add(new DictionaryEntry("@uid", userid));
            ds = DBAccess.Query(sql, "TableScheme", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    for (i = 0; i < nCount; i++)
                    {
                        DataRow row = dt.Rows[i];
                        int id = DatabaseUtility.getIntValue(row, "ID", -1);
                        string name = DatabaseUtility.getStringValue(row, "Name").Trim();
                        string color = DatabaseUtility.getStringValue(row, "color").Trim();
                        children += "{schid:" + id.ToString() + ",name:'" + name
                            + "',color:'" + color + "'},";
                    }
                }
            }

            if (children.Length > 0)
            {
                children = children.Substring(0, children.Length - 1);
                ret = "[" + children + "]";
            }
            context.Response.Write(ret);

        }

        protected override string getErrorMessage()
        {
            return "[]";
        }

        protected override int getErrorCode()
        {
            return 200;
        }
    }
}