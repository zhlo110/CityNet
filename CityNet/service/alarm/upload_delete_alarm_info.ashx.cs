using CityNet.Controllers;
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
    /// upload_delete_alarm_info 的摘要说明
    /// </summary>
    public class upload_delete_alarm_info : Security
    {

        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //更新所有的警报信息
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);
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
                string sql = "update AlarmPoint set Desciption=@html,Eluminated=1,"+
                    "EluminatedUser=@eu,EluminatedDate=@ed " +
                    "where MeasurePointID = @mid";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@html", htmlvalue));
                list.Add(new DictionaryEntry("@eu", userid));
                list.Add(new DictionaryEntry("@ed", DateTime.Now));
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