using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.projectsite
{
    /// <summary>
    /// get_available_projectsite 的摘要说明
    /// </summary>
    public class get_available_projectsite : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            //分页显示权限组
            string ret = getErrorMessage();
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);

            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);

            string sdepartmentid = context.Request["departmentid"];
            if (sdepartmentid == null) sdepartmentid = "";
            int departmentid = this.stringtoint(sdepartmentid, -1);

            int pagesize = limit;

            string sql = "EXEC dbo.get_available_project_site_count @userid = @uid,@departmentid=@did";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@uid", userid));
            list.Add(new DictionaryEntry("@did", departmentid));
            int totalCount = DBAccess.QueryStatistic(sql, list);


            sql = "EXEC dbo.get_available_project_site @userid = @uid,@departmentid=@did,@start=@st,@end=@en";
            list.Add(new DictionaryEntry("@st", start + 1));
            list.Add(new DictionaryEntry("@en", start + limit));

            DataSet ds = DBAccess.Query(sql, "get_available_project_site", list);
            string children = "";
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    for (i = 0; i < nCount; i++)
                    {
                        DataRow row = dt.Rows[i];

                        int ID =  DatabaseUtility.getIntValue(row, "ID", -1);
                        string TaskName = DatabaseUtility.getStringValue(row, "TaskName");

                        double mileagebegin = DatabaseUtility.getDoubleValue(row, "MileageBegin",-1.0);
                        double mileageend = DatabaseUtility.getDoubleValue(row, "MileageEnd", -1.0);
                        int direction = DatabaseUtility.getIntValue(row, "Direction",-1);
                        string Prefix = DatabaseUtility.getStringValue(row, "Prefix");
                        string State = DatabaseUtility.getStringValue(row, "State");
                        string SiteType = DatabaseUtility.getStringValue(row, "SiteType");
                        string Description = DatabaseUtility.getStringValue(row, "Description");
                        int dtstate = DatabaseUtility.getIntValue(row, "DTState", -1);

                        string mileagerange = "";
                        if (mileagebegin > -99998 && mileageend > -99998)
                        {
                            mileagerange = DatabaseUtility.getMileageText(Prefix, mileagebegin)
                               + "<-->" + DatabaseUtility.getMileageText(Prefix, mileageend);
                        }
                        string sdirection = "正向施工";
                        if (direction == 0)
                        {
                            sdirection = "逆向施工";
                        }
                        children += "{taskid:" + ID.ToString() + ",taskname:'" + TaskName + "',sitetype:'" + SiteType
                            + "',sitestate:'" + State + "',range:'" + mileagerange + "',direction:'" + sdirection
                            + "',description:'" + Description + "',dtstate:" + dtstate.ToString()
                            + ",prefix:'" + Prefix + "',mileagebegin:" + mileagebegin.ToString() + ",mileageend:" + mileageend.ToString() + "},";
                    }
                    if (children.Length > 0)
                    {
                        children = children.Substring(0, children.Length - 1);
                    }
                    ret = "{\"totalCount\":" + totalCount.ToString() + ",\"roots\":[" + children + "]}";
                }
            }
            context.Response.Write(ret);

        }

        protected override string getErrorMessage()
        {
            return "{\"totalCount\":0,\"roots\":[]}";
        }

        protected override int getErrorCode()
        {
            return 200;
        }

    }
}