using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.basepoint
{
    /// <summary>
    /// getbasepoint 的摘要说明
    /// </summary>
    public class getbasepoint : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string ret = getErrorMessage();
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);
            string mode = context.Request["mode"];

            int imode = 0;//0,全选
            if (!int.TryParse(mode, out imode))
            {
                imode = 0;
            }
            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);
            int pagesize = limit;
            string sql = "select count(ID) from BasePoint where TaskID in(select TaskID from Task_Visible where UserID = @uid)";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@uid", userid));
            int totalCount = DBAccess.QueryStatistic(sql, list);
            sql = "EXEC dbo.get_basepoint_by_user @userid = @uid,@start = @st,@end = @en";
            list.Add(new DictionaryEntry("@st", start + 1));
            list.Add(new DictionaryEntry("@en", start + limit));

            DataSet ds = DBAccess.Query(sql, "BasePoint", list);
            string children = "";
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
                        int ID = DatabaseUtility.getIntValue(row,"ID",-1);
                        string Name = DatabaseUtility.getStringValue(row, "Name");
                        string sitename = DatabaseUtility.getStringValue(row, "TaskName");
                        string x = DatabaseUtility.getStringValue(row, "X");
                        string y = DatabaseUtility.getStringValue(row, "Y");
                        string z = DatabaseUtility.getStringValue(row, "Z");
                        string type = DatabaseUtility.getStringValue(row, "Type");
                        string lat = DatabaseUtility.getStringValue(row, "Lat");
                        string lon = DatabaseUtility.getStringValue(row, "Lon");
                        int taskid = DatabaseUtility.getIntValue(row, "TaskID", -1);
                        string realname = DatabaseUtility.getStringValue(row, "RealName");
                        children += "{id:" + ID.ToString() + ",pointname:'" + Name + "',selectcheck:0,taskid:" + taskid.ToString() 
                            + ",x:'"+x+"',y:'"+y+"',z:'"+z
                            + "',lon:'" + lon + "',lat:'" + lat + "',type:'" + type + "',creator:'" + realname + "',sitename:'" + sitename + "'},";
                    }
                }
            }

            if (children.Length > 0)
            {
                children = children.Substring(0, children.Length - 1);
            }
            ret = "{\"totalCount\":" + totalCount.ToString() + ",\"roots\":[" + children + "]}";

            context.Response.Write(ret);

        }

        protected override string getErrorMessage()
        {
            return "{\"totalCount\":0,\"roots\":[]}";
        }

        protected override int getErrorCode()
        {
            return 200;
        }
    }
}