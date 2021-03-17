using CityNet.Controllers;
using CityNet.Models;
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
    /// get_progressbar_info 的摘要说明
    /// </summary>
    public class get_progressbar_info : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            //throw new NotImplementedException();

            string uniqueID = context.Request["uniqueid"];

            ProgressInfo info = ProgressBarUtil.getProgressInfo(uniqueID);

            string isover = "true";
            if (info.Current >= 0)
            {
                isover = "false";
            }
            context.Response.Write("{success:1,msg:'查询成功',uniqueid:'" + uniqueID
                + "',showtext:'" + info.Text + "',total:" + info.Total.ToString()
                + ",description:'" + info.Description + "',current:" 
                + info.Current.ToString() + ",over:"+isover+"}");
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