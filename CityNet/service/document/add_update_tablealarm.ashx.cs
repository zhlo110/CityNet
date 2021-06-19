using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.document
{
    /// <summary>
    /// add_update_tablealarm 的摘要说明
    /// </summary>
    public class add_update_tablealarm : Security
    {
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            string schid = context.Request["schid"];
            string alarmid = context.Request["alarmid"];
            string alarmname = context.Request["alarmname"];
            string rule =  System.Web.HttpUtility.HtmlDecode(context.Request["rule"]);
            string ruletype = System.Web.HttpUtility.HtmlDecode(context.Request["ruletype"]);
            string color = context.Request["color"];
            string unit = context.Request["unit"];
            string description = context.Request["description"];
            string alarmlevel = context.Request["alarmlevel"];
            int ialarmlevel = 1;
            if(!int.TryParse(alarmlevel,out ialarmlevel))
            {
                ialarmlevel = 1;
            }
            int ischid = -1;
            if (!int.TryParse(schid, out ischid))
            {
                ischid = -1;
            }
            

            if (alarmid != null && alarmname != null)
            {
                int ialarmid = -1;
                if (!int.TryParse(alarmid, out ialarmid))
                {
                    ialarmid = -1;
                }
                if (ialarmid == -1)//添加
                {
                    string sql = "insert into [AlarmScheme] (Name,SchemeID,Rules,color,unit,Description,AlarmLevel,[Type],ErrorMsg)" +
                        " values (@na,@sid,@rul,@color,@unit,@des,@alevel,@type,'')";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@na", alarmname));
                    list.Add(new DictionaryEntry("@sid", ischid));
                    list.Add(new DictionaryEntry("@rul", rule));
                    list.Add(new DictionaryEntry("@color", color));
                    list.Add(new DictionaryEntry("@unit", unit));
                    list.Add(new DictionaryEntry("@des", description));
                    list.Add(new DictionaryEntry("@alevel", ialarmlevel));
                    list.Add(new DictionaryEntry("@type", ruletype));
                    DBAccess.NoQuery(sql, list);
                    
               //     UpdateAlarm.updateAlarm(ischid);//更新报警列
                    context.Response.Write("{success:1,msg:'插入成功'}");

                }
                else
                {
                    // (Name,Description,creatorID,Priority,createTime)" +
                    //    " values (@sn,@des,@createid,@pri,@ct)
                    string sql = "update [AlarmScheme] set Name=@na,Rules=@rul,"
                        + "color=@color,unit=@unit,Description = @des,AlarmLevel=@alevel,[Type]=@type,ErrorMsg=@err where ID=@id";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@na", alarmname));
                    list.Add(new DictionaryEntry("@rul", rule));
                    list.Add(new DictionaryEntry("@color", color));
                    list.Add(new DictionaryEntry("@unit", unit));
                    list.Add(new DictionaryEntry("@des", description));
                    list.Add(new DictionaryEntry("@alevel", ialarmlevel));
                    list.Add(new DictionaryEntry("@type", ruletype));
                    list.Add(new DictionaryEntry("@err", ""));
                    list.Add(new DictionaryEntry("@id", ialarmid));
                    DBAccess.NoQuery(sql, list);
                //    UpdateAlarm.updateAlarm(ischid);//更新报警列
                    context.Response.Write("{success:1,msg:'更新成功'}");
                }
            }
            else
            {
                context.Response.Write("{success:0,msg:'表单格式错误'}");
                context.Response.StatusCode = getErrorCode();
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