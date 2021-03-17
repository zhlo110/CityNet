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
    /// department_has_child 的摘要说明
    /// </summary>
    public class department_has_child : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string departmentid = context.Request["departmentid"];
            int idepartmentid = -1;
            if (int.TryParse(departmentid, out idepartmentid))
            {
                // 
                string sql = "select count([ID]) from Department where ParentID = @pid";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@pid", idepartmentid));
                int nCount2 = DBAccess.QueryStatistic(sql, list);
                sql = "select count([ID]) from User_Department where DepartmentID = @pid";
                int nCount1 = DBAccess.QueryStatistic(sql, list);
                context.Response.Write("{success:1,msg:'查询成功',childrenNum:'" + (nCount1 + nCount2).ToString() + "'}");
            }
            else
            {
                returnErrorInfo(context, "部门ID解析出错!");
            }
            //throw new NotImplementedException();
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