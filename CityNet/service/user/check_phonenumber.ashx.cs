using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CityNet.Controllers;

namespace CityNet.service.user
{
    /// <summary>
    /// check_phonenumber 的摘要说明
    /// </summary>
    public class check_phonenumber : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            String phonenumber = context.Request["phone"];


            string sql = "select count(*) from [User] where [PhoneNumber] = @phone";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@phone", phonenumber));
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