using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.function
{
    /// <summary>
    /// delete_or_add_tousergroup 的摘要说明
    /// </summary>
    public class delete_or_add_tousergroup : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string roleid = context.Request["roleid"];
            string userid = context.Request["userid"];
            int iroleid = -1;
            int iuserid = -1;
            bool success1 = int.TryParse(roleid, out iroleid);
            bool success2 = int.TryParse(userid, out iuserid);
            if (success1 & success2)
            {
                //每个用户只能有一个角色，要先把其他的角色删除
                string isinsert = context.Request["ischeck"];
                isinsert = isinsert.Trim();
                string sql = "delete from User_Group where UserID = @uid";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@uid", iuserid));
                DBAccess.NoQuery(sql, list);

                if (isinsert.Equals("true")) //插入
                {
                    list.Clear();
                    sql = "insert into User_Group(UserID,GroupID) values(@uid,@roleid)";
                    list.Add(new DictionaryEntry("@uid", iuserid));
                    list.Add(new DictionaryEntry("@roleid", iroleid));
                    DBAccess.NoQuery(sql, list);
                }
            }
            else
            {
                context.Response.Write("传入参数错误！");
                context.Response.StatusCode = getErrorCode();
            }


            //throw new NotImplementedException();
        }

        protected override string getErrorMessage()
        {
            return "用户名或密码错误";
        }

        protected override int getErrorCode()
        {
            return 500;
        }
    }
}