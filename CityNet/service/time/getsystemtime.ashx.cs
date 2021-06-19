using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.time
{
    /// <summary>
    /// getsystemtime 的摘要说明
    /// </summary>
    public class getsystemtime : Security
    {

        //获取系统时间
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            DateTime now = DateTime.Now;
            returnInfo(context, now.ToString("yyyy-MM-dd hh:mm:ss"));
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