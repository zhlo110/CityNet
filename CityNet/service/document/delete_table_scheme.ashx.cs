using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.document
{
    /// <summary>
    /// delete_table_scheme 的摘要说明
    /// </summary>
    public class delete_table_scheme : Security
    {
        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误，请重新登录.'}";
        }
        protected override int getErrorCode()
        {
            return 500;
        }
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //删除功能
            string schid = context.Request["schid"];
            int ischid = this.stringtoint(schid, -1);
            string taskid = context.Request["taskid"];
            int itaskid = this.stringtoint(taskid, -1);


            string sql = "";
            IList list = new ArrayList();
            sql = "delete from Task_TableScheme where TableSchemeID=@tsid and TaskID=@tid";
            list.Add(new DictionaryEntry("@tsid", ischid));
            list.Add(new DictionaryEntry("@tid", itaskid));
            DBAccess.NoQuery(sql, list);

            sql = "select count(ID) from Task_TableScheme where TableSchemeID=@tsid";
            list.Clear();
            list.Add(new DictionaryEntry("@tsid", ischid));
            int count = DBAccess.QueryStatistic(sql, list);

            if (count == 0 && ischid > 0)
            {
                sql = "DECLARE @success char(50) " +
                             "EXEC dbo.delete_TableScheme @schemeid = @id, @success = @success OUTPUT " +
                             "SELECT	@success as 'returnvalue'";
                list.Clear();
                list.Add(new DictionaryEntry("@id", ischid));
                string result = DBAccess.QueryString(sql, list);
                context.Response.Write("{success:1,msg:'" + result + "'}");
            }
            else
            {
                this.returnInfo(context,"该监测项还存在其他工点中");
            }
        }
    }
}