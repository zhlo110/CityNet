using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CityNet.Controllers;
using CityNet.Utility;
using CityNet.security;

namespace CityNet.service.function
{
    /// <summary>
    /// insert_function_group 的摘要说明
    /// </summary>
    public class insert_function_group : Security
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
            //添加和更新功能组
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            String function_name = context.Request["funname"];
            String function_description = context.Request["fundescription"];
            String function_id = context.Request["funid"];
            string function_type = context.Request["funtype"];
            DateTime now = DateTime.Now;
            int userID = LogUtility.GetUserID(username, password);
            string sql = "";
            IList list = new ArrayList();
            string msg = "";
            int ifunctiontype = 1;//默认为1
            if (!int.TryParse(function_type, out ifunctiontype))
            {
                ifunctiontype = 1;
            }
            string spriority = context.Request["priority"];
            int priority = 1;
            if (!int.TryParse(spriority, out priority))
            {
                priority = 1;
            }


            if (function_id == "new_item_id")
            {
                sql = "insert into ActionClass (ActionClassName,ParentID,Description,CreateID,ActionCreateTime,ActionType,[Priority])" +
                    " values (@funname,0,@des,@createid,@now,@aid,@pri)";
                list.Add(new DictionaryEntry("@funname", function_name));
                list.Add(new DictionaryEntry("@des", function_description));
                list.Add(new DictionaryEntry("@createid", userID));
                list.Add(new DictionaryEntry("@now", now));
                list.Add(new DictionaryEntry("@aid", ifunctiontype));
                list.Add(new DictionaryEntry("@pri", priority));
                msg = "数据插入成功。";
            }
            else
            {
                char[] splitc = new char[] { '_' };
                string[] vec2 = function_id.Split(splitc);
                sql = "update ActionClass set ActionClassName = @funname,Description=@des,ActionType=@aid,[Priority]=@pri where [ID]= @id";
                list.Add(new DictionaryEntry("@funname", function_name));
                list.Add(new DictionaryEntry("@des", function_description));
                list.Add(new DictionaryEntry("@aid", ifunctiontype));
                list.Add(new DictionaryEntry("@pri", priority));
                list.Add(new DictionaryEntry("@id", vec2[2]));
                msg = "数据保存成功。";
            }

            DBAccess.NoQuery(sql, list);
            context.Response.Write("{success:1,msg:'" + msg + "'}");

        }
      
    }
}