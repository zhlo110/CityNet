using CityNet.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.task
{
    /// <summary>
    /// delete_tasks 的摘要说明
    /// </summary>
    public class delete_tasks : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
           // throw new NotImplementedException();
            string ids = context.Request["taskids"];
            if (ids.Length > 0)
            {
                ids = ids.Substring(0, ids.Length - 1);
            }


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