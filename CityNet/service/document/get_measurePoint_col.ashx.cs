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
    /// get_measurePoint_col 的摘要说明
    /// </summary>
    public class get_measurePoint_col : Security
    {
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            string sql = "SELECT b.name as name" +
                        ",Type_name(b.xusertype) as type" +
                        ",Isnull(c.VALUE,'') as description " +
                        "FROM sysobjects a join syscolumns b on a.id = b.id " +
                        "JOIN sys.extended_properties c " +
                        "ON b.id = c.major_id " +
                        "AND b.colid = c.minor_id " +
                        "WHERE a.id = Object_id('MeasurePoint') " +
                        "ORDER BY b.colid";
            DataSet ds = DBAccess.Query(sql, "measurePoint", null);
            string ret = "[]";
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    string str = "";
                    for (i = 0; i < nCount; i++)
                    {
                        DataRow row = dt.Rows[i];
                        string name = DatabaseUtility.getStringValue(row, "name");
                        string type = DatabaseUtility.getStringValue(row, "type");
                        string description = DatabaseUtility.getStringValue(row, "description");
                        str += "{name:'" + name + "',type:'" + type + "',description:'" + description + "'},";
                    }
                    if (str.Length > 0)
                    {
                        str = "{name:'pointname',type:'vchar',description:'控制点名'}," + str;
                    }
                    if (str.Length > 0)
                    {
                        
                        str = str.Substring(0, str.Length - 1);
                    }
                    ret = "[" + str + "]";
                }
            }
            context.Response.Write(ret);
        }
        protected override string getErrorMessage()
        {
            return "[]";
        }
        protected override int getErrorCode()
        {
            return 200;
        }
    }
}