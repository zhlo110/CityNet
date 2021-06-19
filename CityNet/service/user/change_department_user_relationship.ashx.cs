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
                int istate = 0;
                if (check.Trim().Equals("true"))
                {
                    sql = "insert User_Department([UserID],[DepartmentID]) values(@uid,@did)";
                    istate = 1;
                }
                else {
                    sql = "delete from User_Department where [UserID]=@uid and [DepartmentID] = @did";
                    istate = 0;
                }
                //一个用户仅仅属于一个部门
                 
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@uid", iuserid));
                list.Add(new DictionaryEntry("@did", idepartmentid));
                DBAccess.NoQuery(sql, list);

                //由于对用户的部门进行了更改，该工区下的任务对该用户的可视关系也应做相应的修改
                //调用存储过程修改

                sql = " EXEC dbo.change_department_user_task @userid = @uid, @departmentid = @did, @removes = @rmv";
                list.Clear();
                list.Add(new DictionaryEntry("@did", idepartmentid));
                list.Add(new DictionaryEntry("@uid", iuserid));
                list.Add(new DictionaryEntry("@rmv", istate)); // 0表示删除，1表示添加
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