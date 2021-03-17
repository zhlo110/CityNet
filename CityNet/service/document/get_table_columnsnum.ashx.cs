using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.document
{
    /// <summary>
    /// get_table_columnsnum 的摘要说明
    /// </summary>
    public class get_table_columnsnum : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string tableid = context.Request["tableid"];
            int itableid = 0;
            if (!int.TryParse(tableid, out itableid))
            {
                itableid = 0;
            }
            string sql = "select MaxColumns from DocumentTable where [ID]=@tid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tid", itableid));
            DataSet ds = DBAccess.Query(sql, "DocumentTable", list);
            string ret = "{success:0,msg:'获取数据失败',colnums:0}";
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        int columnsNum = DatabaseUtility.getIntValue(row, "MaxColumns", 0);
                        ret = "{success:1,msg:'获取数据成功',colnums:" + columnsNum.ToString() + "}";
                    }
                }
            }
            context.Response.Write(ret);
        }

        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户验证失败！',html:'',tableid:0,children:[]}";
        }
        protected override int getErrorCode()
        {
            return 200;
        }
    }
}