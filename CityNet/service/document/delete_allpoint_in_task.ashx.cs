using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.document
{
    /// <summary>
    /// delete_allpoint_in_task 的摘要说明
    /// </summary>
    public class delete_allpoint_in_task : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string taskid = context.Request["taskid"];
            int itaskid = -1;
            if (!int.TryParse(taskid, out itaskid))
            {
                itaskid = -1;
            }
    
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);



            
            //删除PointAlarm
            string sql = "delete from PointAlarm where MeasurePointID in(select ID from Point_User_View where TaskID=@tid and UserID=@uid)";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tid", itaskid));
            list.Add(new DictionaryEntry("@uid", userid));
            DBAccess.NoQuery(sql, list);

            //删除 DocumentDataRow
            sql = "delete from MeasurePoint where ID in(select ID from Point_User_View where TaskID=@tid and UserID=@uid)";
            DBAccess.NoQuery(sql, list);

            sql = "delete from Point where ID in(select ID from Point_Number_View where Num=0)";
            DBAccess.NoQuery(sql, null);
            returnInfo(context, "删除成功");
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