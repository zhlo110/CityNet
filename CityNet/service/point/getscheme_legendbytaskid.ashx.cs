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
    /// getscheme_legendbytaskid 的摘要说明
    /// </summary>
    public class getscheme_legendbytaskid : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string ret = getErrorMessage();
            string children = "";
            string taskid = context.Request["taskid"];
            int itaskid = -1;
            if (!int.TryParse(taskid, out itaskid))
            {
                itaskid = -1;
            }
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);
   
            string sql =  "select ID,Name,color from TableScheme where ID in"+
                "(select distinct SchemeID from Point_User_View where TaskID=@tid and UserID=@uid)";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tid", itaskid));
            list.Add(new DictionaryEntry("@uid", userid));

            DataSet ds = DBAccess.Query(sql, "TableScheme", list);
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