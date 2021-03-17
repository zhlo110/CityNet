using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using CityNet.Controllers;
using CityNet.Utility;

namespace CityNet.service.user
{
    /// <summary>
    /// register 的摘要说明
    /// </summary>
    public class register : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string username = context.Request["username"];
            string password = context.Request["passwordname1"];
            string realname = context.Request["realname"];
            string departmentid = context.Request["departmentform"];
            string occupation = context.Request["occupation"];
            string sex = context.Request["sex"];
            string officephone = context.Request["officephone"];
            string phone = context.Request["phone"];
            string email = context.Request["email"];
            string idcard = context.Request["idcard"];
            string birthday = context.Request["birthday"];
            string other = context.Request["other"];
            DateTime now = DateTime.Now;
            int auditid = 0;
            string sql = "select count([ID]) from [User]";
            int nCount = DBAccess.QueryStatistic(sql,null);
            if (nCount > 0)
            {
                sql = "insert into [User] ([ID],UserName,PassWord,RealName,State"
                        + ",PhoneNumber,Description,LogTime,CreateTime,Sex,AuditID,OfficeNumber,Email,IDCard"
                        + ",Brithday,Position) values ((select MAX([ID])+1 from [User]),@username,@password,@realname,0,"
                        + "@phone,@other,0,@now,@sex,@auditid,@officephone,@email,@idcard,@birthday,@occupation)";
            }
            else
            {
                sql = "insert into [User] ([ID],UserName,PassWord,RealName,State"
                        + ",PhoneNumber,Description,LogTime,CreateTime,Sex,AuditID,OfficeNumber,Email,IDCard"
                        + ",Brithday,Position) values (1,@username,@password,@realname,0,"
                        + "@phone,@other,0,@now,@sex,@auditid,@officephone,@email,@idcard,@birthday,@occupation)";
            }


            DateTime birthdaydt = DateTime.Now;
            try
            {
                birthdaydt = DateTime.ParseExact(birthday, "yyyy年MM月dd日", CultureInfo.InvariantCulture);
            }
            catch (Exception) { }

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@username", username));
            list.Add(new DictionaryEntry("@password", password));
            list.Add(new DictionaryEntry("@realname", realname));
            list.Add(new DictionaryEntry("@phone", phone));
            list.Add(new DictionaryEntry("@other", other));
            list.Add(new DictionaryEntry("@now", now));
            list.Add(new DictionaryEntry("@sex", sex));
            list.Add(new DictionaryEntry("@auditid", auditid));
            list.Add(new DictionaryEntry("@officephone", officephone));
            list.Add(new DictionaryEntry("@email", email));
            list.Add(new DictionaryEntry("@idcard", idcard));
            if (birthday != "")
            {
                list.Add(new DictionaryEntry("@birthday", birthdaydt));
            }
            else
            {
                list.Add(new DictionaryEntry("@birthday", DBNull.Value));
            }
            list.Add(new DictionaryEntry("@occupation", occupation));

            DBAccess.NoQuery(sql, list);
            sql = "select Max([ID]) from [User]";
            int UserID = DBAccess.QueryStatistic(sql, null);

            //添加到部门表
            sql = "delete from User_Department where UserID =@uid and DepartmentID = @did";
            list.Clear();
            int idid = -1;
            if(!int.TryParse(departmentid,out idid))
            {
                idid = -1;
            }
            list.Add(new DictionaryEntry("@uid", UserID));
            list.Add(new DictionaryEntry("@did", idid));
            DBAccess.NoQuery(sql,list);
            if (idid != -1)
            {
                sql = "insert into User_Department(UserID,DepartmentID) values(@uid,@did)";
                DBAccess.NoQuery(sql, list);
            }

            context.Response.Write("{success:1,msg:'注册成功，请返回首页登录。'}");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}