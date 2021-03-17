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
    /// get_table_rows 的摘要说明
    /// </summary>
    public class get_table_rows : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string tableid = context.Request["tableid"];
            int itableid = 0;
            string ret = "[]";
            if (!int.TryParse(tableid, out itableid))
            {
                itableid = 0;
            }
            string sql = "select MaxColumns from DocumentTable where [ID]=@tid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tid", itableid));
            DataSet ds = DBAccess.Query(sql, "DocumentTable", list);
          
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
                        if (columnsNum > 0)
                        {
                            sql = "select [Row],qtip from DocumentDataRow where TableID =@tid order by [ID]";
                            ds = DBAccess.Query(sql, "DocumentDataRow", list);
                            string str = "";
                            if (ds != null)
                            {
                                if (ds.Tables.Count > 0)
                                {
                                    DataTable rowtable = ds.Tables[0];
                                    nCount = rowtable.Rows.Count;
                                    int i;
                                    for (i = 0; i < nCount; i++)
                                    {
                                         row = rowtable.Rows[i];
                                         string srow = DatabaseUtility.getStringValue(row, "Row");
                                         string qtip = DatabaseUtility.getStringValue(row, "qtip");
                                         str += parserow(srow,qtip,columnsNum);
                                    }
                                }
                            }
                            if (str.Length > 0)
                            {
                                str = str.Substring(0,str.Length-1);
                            }
                            ret = "["+str+"]";
                        }
                    }
                }
            }
            context.Response.Write(ret);

        }

        private string trim(string row)
        {
            if (row.IndexOf('[') == 0)
            {
                row = row.Substring(1, row.Length - 1);
            }
            if (row.Length > 0 && row.LastIndexOf(']') == row.Length - 1)
            {
                row = row.Substring(0, row.Length - 1);
            }
            return row;
        }

        private string parserow(string row,string qtip,int columnsNum)
        {
            string ret = "";
            //[CP011]#[-855412.5666]#[5422350.4181]#[3242212.8429]#[与CPⅠQ54共桩]
            string[] split = new string[1];
            split[0] = "]#";

            string[] vec = row.Split(split, columnsNum, StringSplitOptions.None);
            int columns = columnsNum;
            int k = 0;
            while (columns > 0)
            {
                string value = "";
                if (k < vec.Length)
                {
                    value = vec[k];
                }
                else
                {
                    value = " ";
                }
                value = trim(value);
                value = HttpUitls.String2Json(value);
                string dateindex = "field_" + k.ToString();
                ret += dateindex+":'" + value + "',";
                k++;
                columns--;
            }
            if (ret.Length > 0)
            {
                ret += "qtip:'" + qtip + "',";
                ret = ret.Substring(0, ret.Length - 1);
                ret = "{" + ret + "},";
            }
            return ret;
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