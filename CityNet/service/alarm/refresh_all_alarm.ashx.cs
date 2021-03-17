using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.alarm
{
    /// <summary>
    /// refresh_all_alarm 的摘要说明
    /// </summary>
    public class refresh_all_alarm : Security
    {
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //更新所有的警报信息
            UpdateAlarm.updateALlAlarm();   
            returnInfo(context, "更新成功");
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