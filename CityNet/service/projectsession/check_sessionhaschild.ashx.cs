using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.projectsession
{
    /// <summary>
    /// check_sessionhaschild 的摘要说明
    /// </summary>
    public class check_sessionhaschild : Security
    {
        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误，请重新登录.'}";
        }
        protected override int getErrorCode()
        {
            return 500;
        }

        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //判断功能组中有没有功能
            String sessionid = context.Request["sessionid"];
            int isessionid = 0;
            if(!int.TryParse(sessionid,out isessionid))
            {
                isessionid = 0;
            }

            string sql = "select count(ID) from [Project_Session] where ParentID=@id";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@id",isessionid));
            int size = DBAccess.QueryStatistic(sql, list);
            if (size > 0)
            {
                context.Response.Write("{success:1,haschildren:true,msg:'节点有孩子.'}");
            }
            else
            {
                context.Response.Write("{success:1,haschildren:false,msg:'节点没有孩子.'}");
            }
        }
    }
}