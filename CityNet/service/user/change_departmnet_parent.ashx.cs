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
    /// change_departmnet_parent 的摘要说明
    /// </summary>
    /// 

   // departmentid: node.id,
    //                            parentid: newParent.id
    public class change_departmnet_parent : Security
    {


        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string departmentid = context.Request["departmentid"];
            string parentid = context.Request["parentid"];

            int idepartmentid = -1;
            int iparentid = 0;

            bool s1 = int.TryParse(departmentid, out idepartmentid);
            if (!int.TryParse(parentid, out iparentid))
            {
                iparentid = 0;
            }
            if (s1)
            {
                string sql = "update [Department] set [ParentID] = @pid where [ID] = @id";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@pid", iparentid));
                list.Add(new DictionaryEntry("@id", idepartmentid));
                DBAccess.NoQuery(sql, list);
                returnInfo(context,"修改成功");
            }
            else
            {
                returnErrorInfo(context,"部门ID解析错误！");
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