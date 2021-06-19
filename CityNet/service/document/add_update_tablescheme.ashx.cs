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
    /// add_update_tablescheme 的摘要说明
    /// </summary>
    public class add_update_tablescheme : Security
    {
        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误！'}";
        }
        protected override int getErrorCode()
        {
            return 500;
        }
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            string schid = context.Request["schid"];
            string schname = context.Request["schname"];
            string description = context.Request["description"];
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            string color = context.Request["color"];
            string taskid = context.Request["taskid"];


            if (schid != null && schname != null)
            {
                int ischid = -1;
                if (!int.TryParse(schid, out ischid))
                {
                    ischid = -1;
                }
                if (ischid == -1)//添加
                {
                    int userID = LogUtility.GetUserID(username, password);
                    DateTime now = DateTime.Now;

                    string sql = "insert into [TableScheme] (Name,Description,creatorID,Priority,createTime,valid,color,hasprojection,TaskID)" +
                        " values (@sn,@des,@createid,@pri,@ct,@v,@color,@hasprj,@tid)";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@sn", schname));
                    list.Add(new DictionaryEntry("@des", description));
                    list.Add(new DictionaryEntry("@createid", userID));
                    list.Add(new DictionaryEntry("@pri", 0));
                    list.Add(new DictionaryEntry("@ct", now));
                    list.Add(new DictionaryEntry("@v", 0));//设定为无效
                    list.Add(new DictionaryEntry("@color", color));//设定颜色
                    list.Add(new DictionaryEntry("@hasprj", 0));//无投影信息
                    list.Add(new DictionaryEntry("@tid", taskid));//无投影信息
                    DBAccess.NoQuery(sql, list);
                    context.Response.Write("{success:1,msg:'插入方案成功'}");

                }
                else
                {
                  // (Name,Description,creatorID,Priority,createTime)" +
                    //    " values (@sn,@des,@createid,@pri,@ct)
                    string sql = "update [TableScheme] set Name=@sn,Description=@des,color=@color where ID=@id";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@sn", schname));
                    list.Add(new DictionaryEntry("@des", description));
                    list.Add(new DictionaryEntry("@color", color));//设定为无效
                    list.Add(new DictionaryEntry("@id", ischid));
                    DBAccess.NoQuery(sql, list);
                    context.Response.Write("{success:1,msg:'更新方案成功'}");
                }
            }
            else
            {
                context.Response.Write("{success:0,msg:'表单格式错误'}");
                context.Response.StatusCode = getErrorCode();
            }
        }
    }
}