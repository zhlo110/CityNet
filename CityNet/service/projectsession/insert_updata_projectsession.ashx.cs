using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.projectsession
{
    /// <summary>
    /// insert_updata_projectsession 的摘要说明
    /// </summary>
    public class insert_updata_projectsession : Security
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
            String session_name = context.Request["sessionname"];
            String session_type = context.Request["sessiontype"];
            String session_id = context.Request["sessionid"];
            string description = context.Request["description"];
            DateTime now = DateTime.Now;
            int userID = LogUtility.GetUserID(username, password);
            string sql = "";
            IList list = new ArrayList();
            string msg = "";

            if (session_id == "new_item_id")
            {
                sql = "insert into Project_Session ([Name],[ParentID],[Type],[Description],[CreatorID],[CreateTime])" +
                    " values (@name,0,@type,@des,@cid,@ctime)";
                list.Add(new DictionaryEntry("@name", session_name));
                list.Add(new DictionaryEntry("@type", session_type));
                list.Add(new DictionaryEntry("@des", description));
                list.Add(new DictionaryEntry("@cid", userID));
                list.Add(new DictionaryEntry("@ctime", now));
                msg = "数据插入成功。";
            }
            else
            {
                int isession_id = 0;
                if (!int.TryParse(session_id, out isession_id))
                {
                    isession_id = 0;
                }

                sql = "update Project_Session set [Name] = @name,Description=@des,[Type]=@type where [ID]= @id";
                list.Add(new DictionaryEntry("@name", session_name));
                list.Add(new DictionaryEntry("@type", session_type));
                list.Add(new DictionaryEntry("@des", description));

                list.Add(new DictionaryEntry("@id", isession_id));
                msg = "数据保存成功。";
            }
            DBAccess.NoQuery(sql, list);
            context.Response.Write("{success:1,msg:'" + msg + "'}");

        }
    }
}