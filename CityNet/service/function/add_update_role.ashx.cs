using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.function
{
    /// <summary>
    /// add_update_role 的摘要说明
    /// </summary>
    public class add_update_role : Security
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
            string roleid = context.Request["roleid"];
            string rolename = context.Request["rolename"];
            string realname = context.Request["realname"];
            string description = context.Request["description"];
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();

            if (roleid != null && rolename != null && realname != null)
            {
                int iroleid = int.Parse(roleid);
                if (iroleid == -1)//添加
                {
                    int userID = LogUtility.GetUserID(username, password);
                    DateTime now = DateTime.Now;
                    string sql = "insert into [Group] (GroupName,RealGroupName,CreateID,Description)" +
                        " values (@gn,@rgn,@createid,@des)";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@gn", rolename));
                    list.Add(new DictionaryEntry("@rgn", realname));
                    list.Add(new DictionaryEntry("@createid", userID));
                    list.Add(new DictionaryEntry("@des", description));
                    DBAccess.NoQuery(sql, list);
                    context.Response.Write("{success:1,msg:'插入角色成功'}");

                }
                else
                {
                    string sql = "update [Group] set GroupName=@gn,RealGroupName=@rgn,Description=@des where ID=@id";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@gn", rolename));
                    list.Add(new DictionaryEntry("@rgn", realname));
                    list.Add(new DictionaryEntry("@des", description));
                    list.Add(new DictionaryEntry("@id", iroleid));
                    DBAccess.NoQuery(sql, list);
                    context.Response.Write("{success:1,msg:'更新角色成功'}");
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