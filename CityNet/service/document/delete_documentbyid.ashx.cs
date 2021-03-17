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
    /// delete_documentbyid 的摘要说明
    /// </summary>
    public class delete_documentbyid : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string docid = context.Request["docid"];
            int idocid = -1;
            if (!int.TryParse(docid, out idocid))
            {
                idocid = -1;
            }
            //删除 DocumentDataRow
            string sql = "delete from DocumentDataRow where ID in"+
                "(select ddr.ID from DocumentDataRow ddr left join "+
                "DocumentTable dt on dt.ID = ddr.TableID where dt.DocumentID = @docid)";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@docid", idocid));
            DBAccess.NoQuery(sql, list);

            sql = "delete from DocumentTable where DocumentID = @docid";
            DBAccess.NoQuery(sql, list);

            sql = "delete from ApproveDocument where DocumentID = @docid";
            DBAccess.NoQuery(sql, list);

            sql = "delete from TempApproveDocument where DocumentID = @docid";
            DBAccess.NoQuery(sql, list);


            sql = "select DocumentUrl,DocumentName from Document where ID = @docid";
             //DBAccess.Query(sql, list);
            DataSet ds = DBAccess.Query(sql, "Document", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    if (nCount>0)
                    {
                        DataRow row = dt.Rows[0];
                        string url = DatabaseUtility.getStringValue(row, "DocumentUrl");
                        string name = DatabaseUtility.getStringValue(row, "DocumentName");

                        string filePath = context.Server.MapPath("~/documents/");
                        string ext = Path.GetExtension(name);
                        string[] vec = url.Split(new char[]{'/'},StringSplitOptions.RemoveEmptyEntries);
                        string filewithoutext = "";
                        if (vec.Length > 0)
                        {
                            filewithoutext = vec[vec.Length - 1];
                        }
                        string filename = filewithoutext + ext;
                        if (File.Exists(filePath + "\\" + filename))
                        {
                            File.Delete(filePath + "\\" + filename);
                        }
                        if (Directory.Exists(filePath + "\\" + filewithoutext))
                        {
                            Directory.Delete(filePath + "\\" + filewithoutext,true);
                        }
                    }
                }
            }

            sql = "delete from Document where ID = @docid";
            DBAccess.NoQuery(sql, list);
            returnInfo(context, "删除成功");
            
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