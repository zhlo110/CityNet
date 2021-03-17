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
    /// insert_or_update_department 的摘要说明
    /// </summary>
    public class insert_or_update_department : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string departmentid = context.Request["departmentid"];
            int idepartmentid = -1;
            string sql = "";
            string departmentname = context.Request["departmentname"];
            string departmentdescription = context.Request["departmentdescription"];
            string parentid = context.Request["parentid"];
            int iparentid = 0;
            if (!int.TryParse(parentid, out iparentid))
            {
                iparentid = 0;
            }
            IList list = new ArrayList();
            string description = "";
            int newid = -1;
            if (int.TryParse(departmentid, out idepartmentid)) //更新数据
            {
                sql = "update Department set [DepartmentName] = @dname,[Description]=@des where [ID] = @did";
                list.Add(new DictionaryEntry("@dname", departmentname));
                list.Add(new DictionaryEntry("@des", departmentdescription));
                list.Add(new DictionaryEntry("@did", idepartmentid));
                description = "更新成功";
                newid = idepartmentid;
            }
            else if (departmentid.Trim().Equals("new_item_id")) //插入数据
            {
                sql = "insert Department([ID],[DepartmentName],[Description],[ParentID],[CreateTime]) values((select MAX([ID])+1 from Department),@dname,@des,@pid,@now)";
                list.Add(new DictionaryEntry("@dname", departmentname));
                list.Add(new DictionaryEntry("@des", departmentdescription));
                list.Add(new DictionaryEntry("@pid", iparentid));
                list.Add(new DictionaryEntry("@now",DateTime.Now));
                description = "插入成功";
            }
            else
            {
                context.Response.StatusCode = getErrorCode();
                context.Response.Write("{success:0,msg:'ID参数解析错误！'}");
                return;
            }
            DBAccess.NoQuery(sql, list);
            if (description.Equals("插入成功"))
            {
                sql = "select Max([ID]) from Department";
                newid = DBAccess.QueryStatistic(sql,null);
            }
            context.Response.Write("{success:1,newid:" + newid.ToString() + ",msg:'" + description + "'}");
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