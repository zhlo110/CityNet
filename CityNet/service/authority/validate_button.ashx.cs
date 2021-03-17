using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.authority
{
    /// <summary>
    /// validate_button 的摘要说明
    /// </summary>
    public class validate_button : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userID = LogUtility.GetUserID(username, password);
            string sql = "select GroupID from User_Group_View where [ID] = @uid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@uid", userID));
            DataSet dataset = DBAccess.Query(sql, "User",list);
            string buttonid = context.Request["buttonid"];
            
            if(dataset != null)
            {
                int nCount = dataset.Tables.Count;
                if (nCount > 0)
                {
                    DataTable dt = dataset.Tables[0];
                    nCount = dt.Rows.Count;
                    if(nCount > 0)
                    {
                        DataRow row =  dt.Rows[0];
                        Object groupid = row["GroupID"];
                        if (groupid != null) //有权限
                        {
                            string sgroupid = groupid.ToString();
                            int igroupid = -1;
                            bool suc1 = int.TryParse(sgroupid, out igroupid);
                            if (suc1)
                            {
                                sql = "select count([ID]) from Action_Group_View where [GroupID] = @gid and [ActionUrl] = @buttonid";
                                list.Clear();
                                list.Add(new DictionaryEntry("@gid", igroupid));
                                list.Add(new DictionaryEntry("@buttonid", buttonid));

                                int count = DBAccess.QueryStatistic(sql, list);
                                if (count > 0)
                                {
                                    context.Response.Write("{success:1,msg:'用户有"+buttonid+"的权限'}");
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            context.Response.Write("{success:0,msg:'无权限'}");

        }

        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误！'}";
        }

        protected override int getErrorCode()
        {
            return 500;
        }
    }
}