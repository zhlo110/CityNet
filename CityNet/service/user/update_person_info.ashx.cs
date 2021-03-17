using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using CityNet.Controllers;
using CityNet.Utility;
using CityNet.security;

namespace CityNet.service.user
{
    /// <summary>
    /// update_person_info 的摘要说明
    /// </summary>
    public class update_person_info : Security
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
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            string department = parameters[2].ToString();

            String realname = context.Request["realname"];
       //     String department = context.Request["department"];
            String occupation = context.Request["occupation"];

            String sex = context.Request["sex"];
            String officephone = context.Request["officephone"];
            String email = context.Request["email"];
            String phone = context.Request["phone"];
            String idcard = context.Request["idcard"];
            String birthday = context.Request["birthday"];
            String other = context.Request["other"];

            string sql = "update [User] set "
                + "RealName=@realname,"
                + "Position=@occupation,"
                + "Sex=@sex,"
                + "OfficeNumber=@officephone,"
                + "Email=@email,"
                + "PhoneNumber=@phone,"
                + "IDCard=@idcard,"
                + "Brithday=@birthday,"
                + "Description=@other "
                + "where UserName= @un and PassWord = @pw";
            DateTime birthdaydt = DateTime.Now;
            try
            {
                birthdaydt = DateTime.ParseExact(birthday, "yyyy年MM月dd日", CultureInfo.InvariantCulture);
            }
            catch (Exception) { }

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@realname", realname));
            list.Add(new DictionaryEntry("@occupation", occupation));
            list.Add(new DictionaryEntry("@sex", sex));
            list.Add(new DictionaryEntry("@officephone", officephone));
            list.Add(new DictionaryEntry("@email", email));
            list.Add(new DictionaryEntry("@phone", phone));
            list.Add(new DictionaryEntry("@idcard", idcard));
            if (birthday != "")
            {
                list.Add(new DictionaryEntry("@birthday", birthdaydt));
            }
            else
            {
                list.Add(new DictionaryEntry("@birthday", DBNull.Value));
            }

            list.Add(new DictionaryEntry("@other", other));
            list.Add(new DictionaryEntry("@un", username));
            list.Add(new DictionaryEntry("@pw", password));

            DBAccess.NoQuery(sql, list);

            int userID = LogUtility.GetUserID(username, password);
           
            list.Clear();
            int departmentId = -1;
            if (!int.TryParse(department, out departmentId))
            {
                departmentId = -1;
            }
            int newdepartmentid = -1;
            String newdepartment = context.Request["department"];
            if (!int.TryParse(newdepartment, out newdepartmentid))
            {
                newdepartmentid = -1;
            }

            if (departmentId != -1 && newdepartmentid != -1)
            {
                if (departmentId != newdepartmentid) //用户要修改部门ID
                {
                    sql = "select count(*) from User_Department where UserID = @uid and DepartmentID = @did";
                    list.Add(new DictionaryEntry("@did", newdepartment));
                    list.Add(new DictionaryEntry("@uid", userID));
                    int nCount = DBAccess.QueryStatistic(sql, list);
                    if (nCount > 0)
                    {
                        context.Response.Write("{success:1,msg:'个人信息修改成功,部门信息修改失败，用户"+username+"已经在修改的部门中'}");
                    }
                    else
                    {
                        sql = "update User_Department set DepartmentID = @did where UserID = @uid and DepartmentID = @oldid";
                        list.Add(new DictionaryEntry("@oldid", departmentId));
                        DBAccess.NoQuery(sql, list);
                        context.Response.Write("{success:1,msg:'个人信息修改成功!'}");
                    }
                }
                else
                {
                    context.Response.Write("{success:1,msg:'个人信息修改成功!'}");
                }
                
            }
            else
            {
                context.Response.Write("{success:1,msg:'个人信息修改成功,部门信息修改失败，原因是部门ID不对'}");
            }
            

        }
        
    }
}