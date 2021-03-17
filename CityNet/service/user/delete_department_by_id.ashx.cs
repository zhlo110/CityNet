using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.user
{
    /// <summary>
    /// delete_department_by_id 的摘要说明
    /// </summary>
    public class delete_department_by_id : Security
    {

        

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string departmentid = context.Request["departmentid"];
            int idepartmentid = -1;
            if (int.TryParse(departmentid, out idepartmentid))
            {
                //删除节点和叶子节点 Department

                TreeNode departmentNode = new DepartmentNode();
                departmentNode = departmentNode.createNode(idepartmentid);
                if (departmentNode != null)
                {
                    departmentNode.deleteNode(999);//删除所有departmentNode，包括自己
                    returnInfo(context,"删除部门成功");
                }
                else
                {
                    returnErrorInfo(context, "没有查询到该部门！");
                }

                //删除部门和人员的对应关系User_Department

            }
            else
            {
                returnErrorInfo(context, "部门ID解析出错!");
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