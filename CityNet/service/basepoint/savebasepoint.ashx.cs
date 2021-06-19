using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.basepoint
{
    /// <summary>
    /// savebasepoint 的摘要说明
    /// </summary>
    public class savebasepoint : Security
    {
        private bool validatetext(string text,out int value,int defaultvalue)
        {
            value = defaultvalue;
            bool success = true;
            if (!int.TryParse(text, out value))
            {
                success = false;
                value = defaultvalue;
            }
            return success;

        }

        private bool validatetext(string text, out double value, double defaultvalue)
        {

            value = defaultvalue;
            bool success = true;
            if (!double.TryParse(text, out value))
            {
                success = false;
                value = defaultvalue;
            }
            return success;

        }

        private double palseDegree(string value)
        {
            double ret = -1000.0;
            if (!double.TryParse(value, out ret))
            {
                ret = -1000.0;
                string[] vec = value.Split(new char[] { '°', '′', '″' }, StringSplitOptions.RemoveEmptyEntries);
                if (vec.Length == 3)
                {
                    double degree = 0.0;
                    double minute = 0.0;
                    double second = 0.0;
                    double.TryParse(vec[0], out degree);
                    double.TryParse(vec[1], out minute);
                    double.TryParse(vec[2], out second);
                    ret = degree + minute / 60.0 + second / 3600.0;
                }
            }
            return ret;
        }

        private void convertdbstring(bool success,double value,out string svalue)
        {
            if (success)
            {
                svalue = value.ToString();
            }
            else
            {
                svalue = null;
            }
        }

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            
            string type = context.Request["type"].Trim();
            string x = context.Request["x"];
            string y = context.Request["y"];
            string z = context.Request["z"];
            string lon = context.Request["lon"];
            string lat = context.Request["lat"];
            string mode = context.Request["mode"].Trim();
            string pointname = "";
            string taskid = "";
            if (mode == "true")
            {
                pointname = context.Request["pointname"].Trim();
                taskid = context.Request["taskid"];
            }
            else
            {
                pointname = context.Request["extrapointname"].Trim();
                taskid = context.Request["extrataskid"];
            }


            int itaskid = -1;
            double dx = 0.0;
            double dy = 0.0;
            double dz = 0.0;
            bool xsuccess, ysuccess, zsuccess, tasksuccess;
            //验证数据的有效性
            tasksuccess = validatetext(taskid, out itaskid, -1);
            xsuccess =  validatetext(x, out dx, -1);
            ysuccess = validatetext(y, out dy, -1);
            zsuccess = validatetext(z, out dz, -1);
            double dlon = palseDegree(lon);
            double dlat = palseDegree(lat);
            string xvalue, yvalue, zvalue, lonvalue, latvalue;
            convertdbstring(xsuccess, dx, out xvalue);
            convertdbstring(ysuccess, dy, out yvalue);
            convertdbstring(zsuccess, dz, out zvalue);
            convertdbstring(dlon != -1000.0, dlon, out lonvalue);
            convertdbstring(dlat != -1000.0, dlat, out latvalue);



            string ret = getErrorMessage();
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            IList list = new ArrayList();
            int userid = LogUtility.GetUserID(username, password);

            //逻辑 1点名不能重复 此task下的
            string sql = "select count(ID) from BasePoint where TaskID=@tid and Name=@tname";
            list.Add(new DictionaryEntry("@tid", taskid));
            list.Add(new DictionaryEntry("@tname", pointname));
            int nCount = DBAccess.QueryStatistic(sql, list);
            if (nCount == 0)
            {

                if (!xsuccess && !ysuccess && !zsuccess)
                {
                    returnInfo(context, "插入基点失败，x,y,z坐标都为空");
                }
                else //可以插入
                {

                    sql = "insert into BasePoint(Name,X,Y,Z,Type,Lat,Lon,TaskID,UserID) values(@name,@x,@y,@z,@type,@lat,@lon,@taskid,@uid)";
                    list.Clear();
                    list.Add(new DictionaryEntry("@name", pointname));
                    list.Add(new DictionaryEntry("@x", xvalue));
                    list.Add(new DictionaryEntry("@y", yvalue));
                    list.Add(new DictionaryEntry("@z", zvalue));
                    list.Add(new DictionaryEntry("@type", type));
                    list.Add(new DictionaryEntry("@lon", lonvalue));
                    list.Add(new DictionaryEntry("@lat", latvalue));
                    list.Add(new DictionaryEntry("@taskid", taskid));
                    list.Add(new DictionaryEntry("@uid", userid));
                    DBAccess.NoQuery(sql, list);
                    returnInfo(context, "插入基点成功");
                }
            }
            else //有重复的点
            {
                if (mode == "false")
                {
                    sql = "update BasePoint set X=@x,Y=@y,Z=@z,Type=@type,Lat=@lat,Lon=@lon where TaskID=@tid and Name=@name";
                    list.Clear();
                    
                    list.Add(new DictionaryEntry("@x", xvalue));
                    list.Add(new DictionaryEntry("@y", yvalue));
                    list.Add(new DictionaryEntry("@z", zvalue));
                    list.Add(new DictionaryEntry("@type", type));
                    list.Add(new DictionaryEntry("@lon", lonvalue));
                    list.Add(new DictionaryEntry("@lat", latvalue));
                    list.Add(new DictionaryEntry("@tid", taskid));
                    list.Add(new DictionaryEntry("@name", pointname));
                    DBAccess.NoQuery(sql, list);
                    returnInfo(context, "更新基点成功");
                }
                else
                {
                    returnInfo(context, "插入基点失败，点名重复");
                }
            }
        }

        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误，请重新登录.'}";
        }
        protected override int getErrorCode()
        {
            return 500;
        }

    }
}