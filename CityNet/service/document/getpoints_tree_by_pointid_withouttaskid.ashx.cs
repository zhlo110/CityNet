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
    /// getpoints_tree_by_pointid_withouttaskid 的摘要说明
    /// </summary>
    public class getpoints_tree_by_pointid_withouttaskid : Security
    {
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            string pointid = context.Request["pointid"];//多个
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userID = LogUtility.GetUserID(username, password);

            string[] vec = pointid.Split(new char[]{','});
            int i, nCount;
            nCount = vec.Length;
            IList pointlist = new ArrayList();
            for (i = 0; i < nCount; i++ )
            {
                int ipointid = -1;
                if (int.TryParse(vec[i], out ipointid))
                {
                    pointlist.Add(ipointid);
                }
            }
            IList list = new ArrayList();
            string sql = "";
            if (pointlist.Count > 0)
            {
                string children = "";

                for (i = 0; i < pointlist.Count; i++)
                {
                    //查找每个点
                    sql = "select ID,FirstMeasureTime,Description,PointName,longitude,latitude from Point where ID=@pid";
                    int ipointid = (int)pointlist[i];
                    list.Clear();
                    list.Add(new DictionaryEntry("@pid", ipointid));
                    DataSet ds = DBAccess.Query(sql, "Point", list);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            nCount = dt.Rows.Count;
                            int j;
                            for (j = 0; j < nCount; j++)
                            {
                                DataRow row = dt.Rows[j];
                                IList childrennode = new ArrayList();
                                int ID = DatabaseUtility.getIntValue(row, "ID", -1);
                                string PointName = DatabaseUtility.getStringValue(row, "PointName");
                                string Datetime = DatabaseUtility.getDatetimeValue(row, "FirstMeasureTime");
                                string Description = DatabaseUtility.getStringValue(row, "Description");
                                string longitude = DatabaseUtility.getStringValue(row, "longitude");
                                string latitude = DatabaseUtility.getStringValue(row, "latitude");

                                calnodeleaf(-1, "longitude", childrennode, "经度", longitude);
                                calnodeleaf(-1, "latitude", childrennode, "纬度", latitude);
                                calnodeleaf(-1, "FirstMeasureTime", childrennode, "初次上传时间", Datetime);
                                calnodeleaf(-1, "Description",childrennode, "描述", Description);

                                pointsheme(childrennode, userID, ID);
                                string schildren = this.converttonode(childrennode);

                                children += "{text:'" + PointName + "',qtip:'" + PointName
                                    + "',leaf:false,expanded:true,cls:'folder',children:[" + schildren + "]},";

                            }
                        }
                    }
                }
                if (children.Length > 0)
                {
                    children = children.Substring(0, children.Length - 1);
                    children = "[" + children + "]";
                }
               
                context.Response.Write(children);
            }
            else
            {
                string ret = getErrorMessage();
                context.Response.Write(ret);
            }
        }
        //获取消警信息
        private string getcolumndes(int measureid, string columns)
        {
            string res = "";
            string sql = "select " + columns + "_des from PointAlarm where MeasurePointID = @mid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@mid", measureid));
            DataSet ds = DBAccess.Query(sql, "PointAlarm", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        res = DatabaseUtility.getStringValue(row, columns+"_des").Trim();
                    }
                }
            }
            return res;
        }
        private void calnodeleaf(int measureid,string columns, IList children,string text, string qtip)
        {
            qtip =qtip.Trim();
            if (qtip.Length > 0)
            {
                if (measureid <= 0)
                {
                    children.Add("{text:'" + text + ":" + qtip + "',qtip:'" + qtip + "',alarmed:0,alarmcolor:'',leaf:true}");
                }
                else
                {
                    string des = getcolumndes(measureid, columns);

                    string insql = "select " + columns + " from PointAlarm where MeasurePointID = @mid and (" + columns + "_des is null or " + columns + "_des='')";
                    string sql = "select * from AlarmScheme where ID in(" + insql + ")";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@mid", measureid));
                    DataSet ds = DBAccess.Query(sql, "AlarmScheme", list);
                    int alarmed = 0;
                    if (des.Length > 0)
                    {
                        alarmed = 2;
                        text += "(已消警)";
                    }

                    string addstr = "{text:'" + text + ":" + qtip + "',qtip:'" + qtip + "',alarmed:"+alarmed.ToString()+",measureid:"+
                        measureid + ",rel:'" + columns + "',alarmcolor:'',alarminfo:'',leaf:true}";

                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            int nCount = dt.Rows.Count;
                            if (nCount > 0)
                            {
                                DataRow row = dt.Rows[0];
                                alarmed = 1; //没有消警
                                
                                string color = DatabaseUtility.getStringValue(row, "color");
                                addstr = "{text:'<font color="+color+">" + text + ": " + qtip + "</font>',qtip:'" +
                                    qtip + "',alarmed:" + alarmed.ToString() + ",alarminfo:'',measureid:" + measureid
                                    + ",rel:'" + columns + "',alarmcolor:'" + color + "',leaf:true}";
                            }
                        }
                    }
                    //查找报警信息
                    children.Add(addstr);
                }
            }
        }

        private string getAlarm(int i, DataTable table, int schemeid)
        {
            string ret = "";

            //获取TableRowScheme中用于比较的行


            return ret;
        }

        private string converttonode(IList children)
        {
            int i;
            int nCount = children.Count;
            string ret = "";

            for (i = 0; i < nCount; i++)
            {
                ret += children[i].ToString() + ",";
            }
            if(ret.Length > 0)
            {
                ret = ret.Substring(0,ret.Length-1);
            }
            return ret;
        }


        private void pointsheme(IList childrennode, int userID,int ipointid)
        {
            //先把方案查出来
            string sql = "select [ID],[Name],[hasprojection] " +
                "from TableScheme where [ID] in " +
                "(select p.SchemeID from Point_User_View p where p.PointID=@pid and UserID=@uid)";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@pid", ipointid));
            list.Add(new DictionaryEntry("@uid", userID));

            DataSet ds = DBAccess.Query(sql, "TableScheme", list);
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
                        IList children = new ArrayList();
                        getshemechild(children, ipointid, ID, userID, hasproject);
                        if (children.Count > 0)
                        {
                            string schildren = this.converttonode(children);
                            string shemeroot = "{text:'" + TableName + "',id:'table_scheme_item_id_" + ID.ToString() + "',qtip:'" + TableName
                            + "',leaf:false ,expanded:true,schemeid:'" + ID.ToString() + "',cls:'folder',children:[" + schildren + "]}";
                            childrennode.Add(shemeroot);
                        }
                    }
                }
            }


        }

        private void getdetail(IList children, int pointid, int schemeid, int userid, string l0, string h)
        {
            string sql = "";
            IList list = new ArrayList();


            if (l0.Length == 0 || h.Length == 0)
            {
                sql = "select ID,TaskID,x,y,z,MeasureTime,description,endpoint,parallelbais,gravitybais,pointdescription,sharedes from Point_User_View " +
                    "where PointID = @pid and UserID = @uid and SchemeID=@schid "+
                    "and TaskID in(select ID from Task_View where priority=4) order by ID";
                list.Add(new DictionaryEntry("@pid", pointid));
                list.Add(new DictionaryEntry("@uid", userid));
                list.Add(new DictionaryEntry("@schid", schemeid));
            }
            else
            {
                sql = "select ID,TaskID,x,y,z,MeasureTime,description,endpoint,parallelbais,gravitybais,pointdescription,sharedes from Point_User_View where PointID = @pid and UserID = @uid and SchemeID=@schid" +
                    " and L0=@l0 and h=@h "+
                    "and TaskID in(select ID from Task_View where priority=4) order by ID";
                list.Add(new DictionaryEntry("@pid", pointid));
                list.Add(new DictionaryEntry("@uid", userid));
                list.Add(new DictionaryEntry("@schid", schemeid));
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

                        IList childrennode = new ArrayList();
                        calnodeleaf(ID, "x",childrennode, "x坐标", x);
                        calnodeleaf(ID, "y",childrennode, "y坐标", y);
                        calnodeleaf(ID, "z",childrennode, "z坐标", z);
                        calnodeleaf(ID, "MeasureTime",childrennode, "测量时间", measuretime);
                        calnodeleaf(ID, "description",childrennode, "测量备注", description);
                        calnodeleaf(ID, "sharedes",childrennode, "共桩情况", sharedes);
                        calnodeleaf(-1, "pointdescription",childrennode, "注意事项", pntdescription);
                        calnodeleaf(ID, "endpoint",childrennode, "终点", endpoint);
                        calnodeleaf(ID, "parallelbais",childrennode, "水准面不平行改正数", parallel);
                        calnodeleaf(ID, "gravitybais",childrennode, "重力异常改正数", gravity);

                        string schildren = this.converttonode(childrennode);
                        string text = "第"+(i+1).ToString()+"次";
                        //判断报警

                        string times = "{text:'" + text + "',qtip:'"
                                + text + "',leaf:false ,expanded:true,cls:'folder',children:[" + schildren + "]}";
                        children.Add(times);

                    }
                }
            }
        }
        
        private void getshemechild(IList children,int pointid, int schemeid, int userid, int hasproject)
        {
            string sql = "";
            if (hasproject > 0)
            {
                sql = "select L0,h from Point_User_View where PointID = @pid and UserID = @uid and SchemeID=@schid group by L0,h";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@pid", pointid));
                list.Add(new DictionaryEntry("@uid", userid));
                list.Add(new DictionaryEntry("@schid", schemeid));
                HashSet<string> set = new HashSet<string>();

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
                            string l0 = DatabaseUtility.getStringValue(row, "L0");
                            string h = DatabaseUtility.getStringValue(row, "h");
                            IList schemeList = new ArrayList();


                          //  calnodeleaf(schemeList, "中央经线", l0);
                         //   calnodeleaf(schemeList, "大地高", h);
                            string name = "中央经线：" + l0 + ",大地高：" + h;
                            getdetail(schemeList, pointid, schemeid, userid, l0, h);

                            string schildren = this.converttonode(schemeList);

                            string lh = "{text:'" + name + "',qtip:'"
                                + name + "',leaf:false ,expanded:true,cls:'folder',children:[" + schildren + "]}";
                            children.Add(lh);
                        }
                    }
                }
            }
            else
            {

                getdetail(children, pointid, schemeid, userid, "", "");
            }
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