using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.user
{
    /// <summary>
    /// change_department_user_relationship 的摘要说明
    /// </summary>
    public class change_department_user_relationship : Security
    {


        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string departmentid = context.Request["departmentid"];
            string userid = context.Request["userid"];
            string check = context.Request["checked"];

            int idepartmentid=-1;
            int iuserid=-1;
            bool su1 = int.TryParse(departmentid, out idepartmentid);
            bool su2 = int.TryParse(userid, out iuserid);
            if(su1&&su2)
            {
                string sql= "";
                if (check.Trim().Equals("true"))
                {
                    sql = "insert User_Department([UserID],[DepartmentID]) values(@uid,@did)";
                }
                else {
                    sql = "delete from User_Department where [UserID]=@uid and [DepartmentID] = @did";
                }
                //一个用户仅仅属于一个部门
                 
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@uid", iuserid));
                list.Add(new DictionaryEntry("@did", idepartmentid));
                DBAccess.NoQuery(sql, list);
                returnInfo(context, "修改用户部门成功！");
            }
            else{
                returnErrorInfo(context,"解析ID错误");
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