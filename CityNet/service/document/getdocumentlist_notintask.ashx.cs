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

namespace CityNet.service.document
{
    /// <summary>
    /// getdocumentlist_notintask 的摘要说明
    /// </summary>
    public class getdocumentlist_notintask : Security
    {
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //分页显示权限组
            string ret = getErrorMessage();
            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);

            string snotintask = context.Request["notintask"];
            int notintask = 1;
            if (!int.TryParse(snotintask, out notintask))
            {
                notintask = 1;
            }
            string searchtext = context.Request["searchtext"];
            bool search = false;
            string condition = "";
            if (searchtext != null)
            {
                searchtext = searchtext.Trim();
                if (searchtext.Length > 0)
                {
                    condition = " and DocumentName like '%'+@seachtext+'%'";
                    search = true;
                }
            }


            string sql = "select count(ID) from [Document] where (notinTask =@notask or  TaskID < 0 )" + condition;
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@notask", notintask));
            if (search)
            {
                list.Add(new DictionaryEntry("@seachtext", searchtext));
            }

            int totalCount = DBAccess.QueryStatistic(sql, list);
            string children = "";

            sql =
                "select * from(" +
                "select ROW_NUMBER() OVER(Order by d.ID desc) as RowNum, " +
                "d.*,u.ID as uerID ,u.RealName,dep.DepartmentName from [Document] d ,[User] u,Department dep " +
                "where d.UpdataUserID = u.ID and d.DepartmentID=dep.ID and (d.notinTask=@notask or TaskID < 0)" + 
                condition + ") as unions " +
                "where unions.RowNum between @start and @end";

            list.Clear();
            if (search)
            {
                list.Add(new DictionaryEntry("@seachtext", searchtext));
            }
            list.Add(new DictionaryEntry("@notask", notintask));
            list.Add(new DictionaryEntry("@start", start + 1));
            list.Add(new DictionaryEntry("@end", start + limit));
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