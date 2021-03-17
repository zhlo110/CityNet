using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.task
{
    /// <summary>
    /// insert_update_task_state 的摘要说明
    /// </summary>
    public class insert_update_task_state : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string stateid = context.Request["stateid"];
            int istateid = -1;
            string sql = "";
            string statename = context.Request["statename"];
            string description = context.Request["description"];
            string statepriority = context.Request["statepriority"];
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);

            int ipriority = 1;
            if (!int.TryParse(statepriority, out ipriority))
            {
                ipriority = 1;
            }
            IList list = new ArrayList();
            if(!int.TryParse(stateid, out istateid))
            {
                istateid = -1;
            }
            string retdescription = "";
            int newid = -1;
            if (istateid>0) //更新数据
            {
                sql = "update SubmitState set [Name] = @sname,[Description]=@des,[Priority]=@pr where [ID] = @sid";
                list.Add(new DictionaryEntry("@sname", statename));
                list.Add(new DictionaryEntry("@des", description));
                list.Add(new DictionaryEntry("@pr", statepriority));
              //  list.Add(new DictionaryEntry("@ed", 0));
                list.Add(new DictionaryEntry("@sid", istateid));
                retdescription = "更新成功";
                newid = istateid;
            }
            else if (istateid == -1) //插入数据
            {
                sql = "insert SubmitState([Name],[Description],[CreateTime],[CreateID],[Priority],[Editable]) values(@sname,@des,@now,@cid,@pr,@ed)";
                list.Add(new DictionaryEntry("@sname", statename));
                list.Add(new DictionaryEntry("@des", description));
                list.Add(new DictionaryEntry("@now", DateTime.Now));
                list.Add(new DictionaryEntry("@cid", userid));
                list.Add(new DictionaryEntry("@pr", statepriority));
                list.Add(new DictionaryEntry("@ed", 0));
                retdescription = "插入成功";
            }
            else
            {
                context.Response.StatusCode = getErrorCode();
                context.Response.Write("{success:0,msg:'ID参数解析错误！'}");
                return;
            }
            DBAccess.NoQuery(sql, list);
            if (retdescription.Equals("插入成功"))
            {
                sql = "select Max([ID]) from SubmitState";
                newid = DBAccess.QueryStatistic(sql, null);
            }
            context.Response.Write("{success:1,newid:" + newid.ToString() + ",msg:'" + retdescription + "'}");
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