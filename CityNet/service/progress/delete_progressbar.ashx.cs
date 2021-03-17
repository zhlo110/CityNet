using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.progress
{
    /// <summary>
    /// delete_progressbar 的摘要说明
    /// </summary>
    public class delete_progressbar : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string uniqueID = context.Request["uniqueid"];
            ProgressBarUtil.delete(uniqueID);
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