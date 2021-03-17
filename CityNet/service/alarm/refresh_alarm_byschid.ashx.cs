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
    /// refresh_alarm_byschid 的摘要说明
    /// </summary>
    public class refresh_alarm_byschid : Security
    {

        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            string schid = context.Request["schid"];
      
            int ischid = -1;
            if (!int.TryParse(schid, out ischid))
            {
                ischid = -1;
            }
            if (ischid > 0)
            {
               // UpdateAlarm.updateALlAlarm();
                 UpdateAlarm.updateAlarm(ischid);//更新报警列
            }
            returnInfo(context,"更新成功");
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