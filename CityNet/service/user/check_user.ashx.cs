using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CityNet.Controllers;

namespace CityNet.service.user
{
    /// <summary>
    /// compare_user 的摘要说明
    /// </summary>
    public class check_user : IHttpHandler
    {
        //判断user是否存在
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            String username = context.Request["username"];


            string sql = "select count(*) from [User] where [UserName] = @un";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@un", username));
            int iExist = DBAccess.QueryStatistic(sql, list);
            if (iExist > 0)
            {
                context.Response.Write("1");
            }
            else
            {
                context.Response.Write("0");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}