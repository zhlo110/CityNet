using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CityNet.Controllers;

namespace CityNet.service.user
{
    /// <summary>
    /// check_cardid 的摘要说明
    /// </summary>
    public class check_cardid : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            String cardid = context.Request["cardid"];


            string sql = "select count(*) from [User] where [IDCard] = @cardid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@cardid", cardid));
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