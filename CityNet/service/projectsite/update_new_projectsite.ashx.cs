using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.projectsite
{
    /// <summary>
    /// update_new_projectsite 的摘要说明
    /// </summary>
    /// 新增或修改工点
    public class update_new_projectsite : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);

            //部门的ID
            string sdepartmentid = context.Request["departmentid"];
            if (sdepartmentid == null) sdepartmentid = "";
            int departmentid = this.stringtoint(sdepartmentid, -1);
            string operation = context.Request["operation"].Trim();

            //taskid更新的时候使用
            string staskid = context.Request["taskid"];
            if (staskid == null) staskid = "";
            staskid = staskid.Trim();
            int itaskid = this.stringtoint(staskid, -1);

            string taskname = context.Request["sitename"].Trim();
            string sitetype = context.Request["sitetype"].Trim();
            string prefix = context.Request["prefix"].Trim();
            string begin = context.Request["beginmileage"].Trim();//
            string end = context.Request["endmileage"].Trim();//
            string sdirection = context.Request["direction"].Trim();//
            string state = context.Request["state"].Trim();
            string description = context.Request["description"].Trim();
            int direction = this.stringtoint(sdirection,1);
            double dbegin = this.stringtodouble(begin, 0.0);
            double dend = this.stringtodouble(end, 0.0);

            string sql = "";
            IList list = new ArrayList();
            


            if (operation.Equals("new")) //新增
            {
                list.Add(new DictionaryEntry("@uid", userid));
                list.Add(new DictionaryEntry("@did", departmentid));
                list.Add(new DictionaryEntry("@tname", taskname));
                list.Add(new DictionaryEntry("@stype", sitetype));
                list.Add(new DictionaryEntry("@begin", dbegin));
                list.Add(new DictionaryEntry("@end", dend));
                list.Add(new DictionaryEntry("@pre", prefix));
                list.Add(new DictionaryEntry("@dir", direction));
                list.Add(new DictionaryEntry("@st", state));
                list.Add(new DictionaryEntry("@des", description));


                sql = "EXEC dbo.create_project_site @userid = @uid,@departmentid=@did,@taskname=@tname,"+
                    "@sitetype=@stype,@mileagebegin=@begin,@mileageend=@end,@prefix=@pre,@direction=@dir,@state=@st,@description=@des";
                DBAccess.NoQuery(sql, list);
                this.returnInfo(context,"添加工点成功！");
            }
            else if (operation.Equals("update"))
            {
                list.Add(new DictionaryEntry("@tid", itaskid));
                list.Add(new DictionaryEntry("@tname", taskname));
                list.Add(new DictionaryEntry("@stype", sitetype));
                list.Add(new DictionaryEntry("@begin", dbegin));
                list.Add(new DictionaryEntry("@end", dend));
                list.Add(new DictionaryEntry("@pre", prefix));
                list.Add(new DictionaryEntry("@dir", direction));
                list.Add(new DictionaryEntry("@st", state));
                list.Add(new DictionaryEntry("@des", description));

                sql = "EXEC dbo.update_project_site @taskid = @tid,@taskname=@tname," +
                    "@sitetype=@stype,@mileagebegin=@begin,@mileageend=@end,@prefix=@pre,@direction=@dir,@state=@st,@description=@des";
                DBAccess.NoQuery(sql, list);
                this.returnInfo(context, "修改工点成功！");
            }
            else
            {
                this.returnErrorInfo(context,"操作错误");
            }
        }
        protected override int getErrorCode()
        {
            return 500;
        }
        protected override string getErrorMessage()
        {
            return "用户名密码错误，请重新登陆";
        }
    }
}