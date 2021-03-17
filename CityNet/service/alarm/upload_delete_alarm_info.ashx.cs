using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.alarm
{
    /// <summary>
    /// upload_delete_alarm_info 的摘要说明
    /// </summary>
    public class upload_delete_alarm_info : Security
    {

        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //更新所有的警报信息
            string rel = context.Request["rel"].Trim();
            string measureid = context.Request["measureid"].Trim();
            string htmlvalue = context.Request["htmlvalue"].Trim();
            htmlvalue = System.Web.HttpUtility.HtmlDecode(htmlvalue);
            int imeasureid = -1;
            if (!int.TryParse(measureid, out imeasureid))
            {
                imeasureid = -1;
            }
            if (rel.Length > 0)
            {
                string sql = "update PointAlarm set "+rel+"_des=@html where MeasurePointID = @mid";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@html", htmlvalue));
                list.Add(new DictionaryEntry("@mid", imeasureid));
                DBAccess.NoQuery(sql, list);
                returnInfo(context,"消警成功");
            }
            else
            {
                returnErrorInfo(context,"传入参数错误");
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