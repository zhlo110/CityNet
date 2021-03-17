using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CityNet.Controllers;
using CityNet.Utility;
using CityNet.security;

namespace CityNet.service.function
{
    /// <summary>
    /// update_function_info 的摘要说明
    /// </summary>
    public class update_function_info : Security
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

            //添加和更新功能，双击表格弹出
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();

            string functionid = context.Request["functionid"];
            string description = context.Request["description"];
            string functionname = context.Request["functionname"];
            string functionurl = context.Request["functionurl"];
            string spriority = context.Request["priority"];
            int priority = 1;
            if (!int.TryParse(spriority, out priority))
            {
                priority = 1;
            }

            if (functionid != null && description != null && functionname != null && functionurl != null)
            {
                int ifunctionid = int.Parse(functionid);
                if (ifunctionid == -1)//添加
                {
                    int userID = LogUtility.GetUserID(username, password);
                    DateTime now = DateTime.Now;
                    string sql = "insert into Action (ActionName,ActionUrl,ActionGroupID,ActionCreateTime,Description,CreateID,[Priority])" +
                        " values (@an,@au,-1,@time,@des,@createid,@pri)";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@an", functionname));
                    list.Add(new DictionaryEntry("@au", functionurl));
                    list.Add(new DictionaryEntry("@time", now));
                    list.Add(new DictionaryEntry("@des", description));
                    list.Add(new DictionaryEntry("@createid", userID));
                    list.Add(new DictionaryEntry("@pri", priority));
                    DBAccess.NoQuery(sql, list);

                    //自动把当前的功能加入到用户所在的组中
                    int groupid = LogUtility.GetGroupIDByUserID(userID);
                    //得到最新的ID
                    sql = "select Max([ID]) from Action";
                    int actionID = DBAccess.QueryStatistic(sql, null);

                    if (groupid != -1 && actionID != -1)
                    {
                        sql = "insert into Action_Group ([ActionID],[GroupID]) values (@aid,@gid)";
                        list.Clear();
                        list.Add(new DictionaryEntry("@aid", actionID));
                        list.Add(new DictionaryEntry("@gid", groupid));
                        DBAccess.NoQuery(sql, list);
                        context.Response.Write("{success:1,msg:'添加成功'}");
                    }
                    else
                    {
                        context.Response.Write("{success:0,msg:'插入失败或当前所在用户工作组不存在'}");
                        context.Response.StatusCode = getErrorCode();
                    }

                }
                else
                {
                    string sql = "update Action set ActionName=@an,Description=@des,ActionUrl=@au,[Priority]=@pri where ID=@id";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@an", functionname));
                    list.Add(new DictionaryEntry("@des", description));
                    list.Add(new DictionaryEntry("@au", functionurl));
                    list.Add(new DictionaryEntry("@id", ifunctionid));
                    list.Add(new DictionaryEntry("@pri", priority));
                    DBAccess.NoQuery(sql, list);
                    context.Response.Write("{success:1,msg:'更新成功'}");
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