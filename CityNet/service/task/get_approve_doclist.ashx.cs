using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace CityNet.service.task
{
    /// <summary>
    /// get_approve_doclist 的摘要说明
    /// </summary>
    public class get_approve_doclist : Security
    {

        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //分页显示权限组
            string ret = getErrorMessage();
            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);

            string approveid = context.Request["approveid"];
            int iapproveid = -1;
            if (!int.TryParse(approveid, out iapproveid))
            {
                iapproveid = -1;
            }
            string taskid = context.Request["taskid"];
            int itaskid = -1;
            if (!int.TryParse(taskid,out itaskid))
            {
                itaskid = -1;
            }

            string viewmode = context.Request["viewmode"];
            int iviewmode = 0;
            if (!int.TryParse(viewmode, out iviewmode))
            {
                iviewmode = 0;
            }//1 只是看看， 不能修改（在ApproveDocument表中） 0，可以删除（TempApproveDocument）



            string username = (string)parameters[0];
            string password = (string)parameters[1];
            int userid = LogUtility.GetUserID(username, password);


            string sql = "select count(distinct ID) from [ApproveDocument] where [ApproveID] =@aid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@aid", iapproveid));

            int totalCount = DBAccess.QueryStatistic(sql, list);
            string children = "";
            int pagesize = limit;
            string searchtable = "";
            list.Clear();

            if (iviewmode == 0)// 可以删除（TempApproveDocument）
            {
                searchtable = "select distinct DocumentID from TempApproveDocument where TaskID=@tid and UserID=@uid";
                list.Add(new DictionaryEntry("@tid", itaskid));
                list.Add(new DictionaryEntry("@uid", userid));
            }
            else
            {
                searchtable = "select distinct DocumentID from [ApproveDocument] where [ApproveID] =@aid";
                list.Add(new DictionaryEntry("@aid", iapproveid));
            }
            list.Add(new DictionaryEntry("@start", start + 1));
            list.Add(new DictionaryEntry("@end", start + limit));

            sql =
                "select * from(" +
                "select ROW_NUMBER() OVER(Order by d.ID desc) as RowNum, " +
                "d.*,u.ID as uerID ,u.RealName,dep.DepartmentName from [Document] d ,[User] u,Department dep " +
                "where d.UpdataUserID = u.ID and d.DepartmentID=dep.ID and d.ID in" +
                "(" + searchtable + ")) as unions " +
                "where unions.RowNum between @start and @end";

           

            DataSet ds = DBAccess.Query(sql, "Document", list);


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


                        int ID = DatabaseUtility.getIntValue(row, "ID", -1);
                        string docName = DatabaseUtility.getStringValue(row, "DocumentName");
                        string createName = DatabaseUtility.getStringValue(row, "RealName");
                        string url = DatabaseUtility.getStringValue(row, "DocumentUrl");
                        string DepartmentName = DatabaseUtility.getStringValue(row, "DepartmentName");
                        int pages = DatabaseUtility.getIntValue(row, "PageCount", 0);
                        int result = DatabaseUtility.getIntValue(row, "IsResult", 0);

                        url = url.Trim();
                        if (url.Length > 0 && url.LastIndexOf('/') == url.Length - 1)
                        {
                            url = url.Substring(0, url.Length - 1);
                            url += Path.GetExtension(docName);
                        }


                        children += "{\"docid\":" + ID.ToString() + ",\"url\":'" + url + "',\"docname\":\"" + docName
                            + "\"," + "\"creator\":\"" + createName + "\",\"pages\":"
                            + pages.ToString() + ",\"result\":" + result.ToString() + ",department:'" + DepartmentName + "'},";
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