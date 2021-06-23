using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.point
{
    /// <summary>
    /// get_geojson_by_task_schemeid 的摘要说明
    /// </summary>
    public class get_geojson_by_task_schemeid : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string staskid = context.Request["taskid"];
            int taskid = this.stringtoint(staskid, -1);

            string sschemeid = context.Request["schemeid"];
            int schemeid = this.stringtoint(sschemeid, -1);

            IList taskids = new ArrayList();
            taskids.Add(taskid);

            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);

            string sql = "select ID,longitude,latitude,PointName from " +
                         "Point where ID in(select distinct PointID from Point_User_View where TaskID=@tid and SchemeID=@sid)";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tid", taskid));
            list.Add(new DictionaryEntry("@sid", schemeid));

            DataSet ds = DBAccess.Query(sql, "Point_User_View", list);

            string str = "";
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    HashSet<CityNet.Utility.Point> Set = new HashSet<CityNet.Utility.Point>();
                    for (i = 0; i < nCount; i++)
                    {
                        DataRow row = dt.Rows[i];
                        int ID = DatabaseUtility.getIntValue(row, "ID", -1);
                        string PointName = DatabaseUtility.getStringValue(row, "PointName").Trim();
                        string longitude = DatabaseUtility.getStringValue(row, "longitude").Trim();
                        string latitude = DatabaseUtility.getStringValue(row, "latitude").Trim();
                        double dlongitude = -1000;
                        double dlatitude = -1000;
                        bool s1 = double.TryParse(longitude, out dlongitude);
                        bool s2 = double.TryParse(latitude, out dlatitude);
                        if (s1 && s2) //解析成功
                        {
                            CityNet.Utility.Point p = new CityNet.Utility.Point();
                            p.IDS.Add(ID);
                            p.Names.Add(PointName);
                            p.latitude = dlatitude;
                            p.longitude = dlongitude;
                            CityNet.Utility.Point.getSchemeColor(p, taskids, schemeid);
                            Set.Add(p);
                        }
                    }
              //      int kount = 0;
                    foreach (CityNet.Utility.Point s in Set)
                    {
                        int alarmid = 0;
                        string alarmcolors = "";
                        s.calculateAlarm(out alarmid, out alarmcolors);
                        if (alarmid > 0)
                        {
                            alarmid = 1;
                        }
                        else
                        {
                            alarmcolors = "#FFFFFF";
                        }
                       
                        /*kount++;
                        if (kount % 2 == 0)
                        {
                            alarmid = 1;
                            alarmcolors = "#FF0000";
                        }*/
                        str += "{\"type\": \"Feature\"," +
                                "\"properties\": {" +
                                "\"name\": " + CityNet.Utility.Point.getArrayListStr(s.Names, ",") + "," +
                                "\"colors\": '" + CityNet.Utility.Point.GetPointColor(s.Colors) + "'," +
                                "\"isalarm\":" + alarmid.ToString() + "," +
                                "\"alarmcolor\":\"" + alarmcolors + "\"," +
                                "\"pointid\": " + CityNet.Utility.Point.getArrayListStr(s.IDS, ",") + "}," +
                                "\"geometry\": {" +
                                "\"type\": \"Point\"," +
                                "\"coordinates\": [" + s.longitude.ToString() + ", " + s.latitude.ToString() + "]}},";

                    }

                }
            }
            if (str.Length > 0)
            {
                str = str.Substring(0, str.Length - 1);
                str = "[" + str + "]";
            }
            else
            {
                str = "[]";
            }
            context.Response.Write(str);
        }


        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误，请重新登录.'}";
        }
        protected override int getErrorCode()
        {
            return 200;
        }
    }
}