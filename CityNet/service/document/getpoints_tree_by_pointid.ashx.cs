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
    /// getpoints_tree_by_pointid 的摘要说明
    /// </summary>
    public class getpoints_tree_by_pointid : Security
    {
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            string pointid = context.Request["pointid"];
            int ipointid = -1;
            if (!int.TryParse(pointid, out ipointid))
            {
                ipointid = -1;
            }

            string taskid = context.Request["taskid"];
            int itaskid = -1;
            if (!int.TryParse(taskid, out itaskid))
            {
                itaskid = -1;
            }


            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userID = LogUtility.GetUserID(username, password);

            //先把方案查出来
            string sql = "select [ID],[Name],[hasprojection] from TableScheme "+
                "where [ID] in (select p.SchemeID from Point_User_View p where p.PointID = @pid and TaskID=@tid and UserID=@uid)";

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@pid", ipointid));
            list.Add(new DictionaryEntry("@tid", itaskid));
            list.Add(new DictionaryEntry("@uid", userID));
            DataSet ds = DBAccess.Query(sql, "TableScheme", list);
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
                        int ID = DatabaseUtility.getIntValue(row, "ID", -1);
                        string TableName = DatabaseUtility.getStringValue(row, "Name");
                        int hasproject = DatabaseUtility.getIntValue(row, "hasprojection", 0);
                        string schildren = getshemechild(ipointid, ID, userID, hasproject,itaskid);
                        children +=
                        "{text:'" + TableName + "',id:'table_scheme_item_id_" + ID.ToString() + "',qtip:'" + TableName
                            + "',leaf:false ,expanded:true,schemeid:'" + ID.ToString() + "',cls:'folder',children:[" + schildren + "]},";
                    }
                }
            }
            if (children.Length > 0)
            {
                children = children.Substring(0, children.Length - 1);
            }
            children = "[" + children + "]";
            context.Response.Write(children);
        }

        private string getdetail(string text, int pointid, int schemeid, int userid, string l0, string h, int taskid)
        {
            string ret = "";
            string sql = "";
            IList list = new ArrayList();
            if (l0.Length == 0 || h.Length == 0)
            {
                sql = "select ID,x,y,z,MeasureTime,description,endpoint,parallelbais,gravitybais,pointdescription,sharedes from Point_User_View where PointID = @pid and UserID = @uid and SchemeID=@schid and TaskID=@tid ";
                list.Add(new DictionaryEntry("@pid", pointid));
                list.Add(new DictionaryEntry("@uid", userid));
                list.Add(new DictionaryEntry("@tid", taskid));
                list.Add(new DictionaryEntry("@schid", schemeid));
            }
            else
            {
                sql = "select ID,x,y,z,MeasureTime,description,endpoint,parallelbais,gravitybais,pointdescription,sharedes from Point_User_View where PointID = @pid and UserID = @uid and SchemeID=@schid" +
                    " and L0=@l0 and h=@h and TaskID=@tid";
                list.Add(new DictionaryEntry("@pid", pointid));
                list.Add(new DictionaryEntry("@uid", userid));
                list.Add(new DictionaryEntry("@schid", schemeid));
                list.Add(new DictionaryEntry("@tid", taskid));
                list.Add(new DictionaryEntry("@l0", l0));
                list.Add(new DictionaryEntry("@h", h));
            }
            DataSet ds = DBAccess.Query(sql, "Point_User_View", list);
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
                        string x = DatabaseUtility.getStringValue(row, "x");
                        string y = DatabaseUtility.getStringValue(row, "y");
                        string z = DatabaseUtility.getStringValue(row, "z");
                        string measuretime = DatabaseUtility.getDatetimeValue(row, "MeasureTime");
                        string description = DatabaseUtility.getStringValue(row, "description");
                        string pntdescription = DatabaseUtility.getStringValue(row, "pointdescription");
                        string sharedes = DatabaseUtility.getStringValue(row, "sharedes");

                        string endpoint = DatabaseUtility.getStringValue(row, "endpoint");
                        string parallel = DatabaseUtility.getStringValue(row, "parallelbais");
                        string gravity = DatabaseUtility.getStringValue(row, "gravitybais");

                        
                        string rtext = text;
                        if (rtext.Length <= 0)
                        {
                            rtext = (i + 1).ToString();
                        }
                        string tip = rtext + ",x坐标：" + x + ",y坐标：" + y + ",z坐标：" + z
                            + ",测量时间：" + measuretime + ",备注:"
                            + description + ",共桩情况：" + sharedes + ",注意事项:" + pntdescription
                            + ",终点：" + endpoint + ",水准面不平行改正数：" + parallel + ",重力异常改正数：" + gravity;

                        ret += "{text:'" + rtext + "',x:'" + x + "',y:'" + y + "',z:'" + z + "',measuretime:'" + measuretime
                            + "',description:'" + description + "',pntdescription:'" + pntdescription
                            + "',sharedes:'" + sharedes + "',id:'table_measure_item_id_" + ID.ToString() + "',qtip:'" + tip
                        + "',leaf:true,endpoint:'" 
                        + endpoint + "',parallel:'" + parallel + "',gravity:'" + gravity + "',measureid:'" + ID.ToString() + "'},";
                    }
                    if (ret.Length > 0)
                    {
                        ret = ret.Substring(0, ret.Length - 1);
                    }
                }
            }
            return ret;
        }

        private string getshemechild(int pointid,int schemeid,int userid,int hasproject,int taskid)
        {
            string sql = "";
            string ret = "";
            if (hasproject > 0)
            {
                sql = "select ID,L0,h from Point_User_View where PointID = @pid and UserID = @uid and SchemeID=@schid and TaskID=@tid";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@pid", pointid));
                list.Add(new DictionaryEntry("@uid", userid));
                list.Add(new DictionaryEntry("@schid", schemeid));
                list.Add(new DictionaryEntry("@tid", taskid));

                DataSet ds = DBAccess.Query(sql, "Point_User_View", list);
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
                            string l0 = DatabaseUtility.getStringValue(row, "L0");
                            string h = DatabaseUtility.getStringValue(row, "h");
                            string name = "中央经线：" + l0 + ",大地高："+h;
                            string schildren = getdetail(name,pointid, schemeid, userid, l0, h,taskid);
                            ret += schildren + ",";
                        }
                    }
                }
                if (ret.Length > 0)
                {
                    ret = ret.Substring(0, ret.Length - 1);
                }
            }
            else
            {
                ret = getdetail("",pointid, schemeid, userid,"","",taskid);

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