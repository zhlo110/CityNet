using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.user
{
    /// <summary>
    /// get_wild_user 的摘要说明
    /// </summary>
    public class get_wild_user : Security
    {
        /*
         //构建视图
        
         */

        /*
         
         
         Ext.define('userInfo', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'userid', type: 'int' },
        { name: 'usercheck', xtype: 'checkcolumn' },
        { name: 'username', type: 'string' },
        { name: 'realname', type: 'string' },
        { name: 'department', type: 'string' },
        { name: 'position', type: 'string' },
        { name: 'rolename', type: 'string' }
    ]
});
         */

        private string rowtostring(DataRow row, int idepartmentid)
        {
            string userid = row["ID"].ToString();



            string usercheck = "false";
            string username = row["UserName"].ToString();
            string realname = row["RealName"].ToString();
            string position = row["Position"].ToString();
            if(row["DepartmentID"] != null)
            {
                string departmentid = row["DepartmentID"].ToString();
                if (departmentid.Trim().Equals(idepartmentid.ToString().Trim()))
                {
                    usercheck = "true";
                }
            }
   //         string department = row["DepartmentName"].ToString();
            string rolename = "用户未授权";
            if (row["RealGroupName"] != null)
            {
                rolename = row["RealGroupName"].ToString();
            }
            return "{userid:" + userid + ",usercheck:" + usercheck
                + ",username:'" + username + "',realname:'" + realname
                + "',department:'',position:'" + position + "',rolename:'" + rolename + "'}";
        }

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            // 查询所有没有部门的用户
            string departmentid = context.Request["departmentid"];
            string sql = "";
            string ret = "";
            int idepartmentid = -1;
            if (departmentid != null)
            {
                if (!int.TryParse(departmentid, out idepartmentid))
                {
                    idepartmentid = -1;
                }
            }
            
            //没有部门的用户
            sql = "select COUNT([ID]) from User_Group_View where DepartmentID is NULL or DepartmentID=@did";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@did", idepartmentid));
            int totalCount = DBAccess.QueryStatistic(sql, list);

            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);

            sql =
                "select * from( " +
                "select ROW_NUMBER() OVER(Order by v.ID desc) as RowNum, " +
                "v.* from User_Group_View v " +
                "where v.DepartmentID is NULL or v.DepartmentID=@did) as unions " +
                "where unions.RowNum between @start and @end";
            list.Clear();
            list.Add(new DictionaryEntry("@did", idepartmentid));
            list.Add(new DictionaryEntry("@start", start + 1));
            list.Add(new DictionaryEntry("@end", start + limit));

            DataSet ds = DBAccess.Query(sql, "User", list);
            if (ds != null)
            {
                int nCount = ds.Tables.Count;
                if (nCount > 0)
                {
                    DataTable dt = ds.Tables[0];
                    nCount = dt.Rows.Count;
                    int i;
                    for (i = 0; i < nCount; i++)
                    {
                        DataRow row = dt.Rows[i];
                        ret += rowtostring(row, idepartmentid) + ",";
                        //  table = dt;
                    }
                }
            }
            if (ret.Length > 0)
            {
                ret = ret.Substring(0, ret.Length - 1);
            }
            ret = "{\"totalCount\":" + totalCount.ToString() + ",\"roots\":[" + ret + "]}";
            context.Response.Write(ret);
        }

        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误，请重新登录.'}";
        }

        protected override int getErrorCode()
        {
            return 500;
        }
    }
}