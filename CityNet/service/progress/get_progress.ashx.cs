using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.progress
{
    /// <summary>
    /// get_progress 的摘要说明
    /// </summary>
    public class get_progress : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            //throw new NotImplementedException();

            string uniqueID = context.Request["uniqueid"];
            int progress = ProgressBarUtil.getProgress(uniqueID);
            string isover = "true";
            if (progress >= 0)
            {
                isover = "false";
            }
            context.Response.Write("{success:1,current:" + progress.ToString() + ",over:"+isover+"}");
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