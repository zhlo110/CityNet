using Newtonsoft.Json;
using CityNet.Controllers;
using CityNet.Models;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.tkyserver
{
    /// <summary>
    /// set_task_visible 的摘要说明
    /// </summary>
    public class set_task_visible : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string taskid = context.Request["taskid"];
            int itaskid = -1;
            if (!int.TryParse(taskid, out itaskid))
            {
                itaskid = -1;
            }
            //查找taskid是否存在
            string sql = "select count(ID) from Task where ID = @id";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@id", itaskid));
            int nCount = DBAccess.QueryStatistic(sql,list);
            if (nCount > 0) //存在
            {
                //删除所有与taskid有关的记录
                sql = "delete from Task_Visible where TaskID=@taskid and UserType=0";
                list.Clear();
                list.Add(new DictionaryEntry("@taskid", itaskid));
                DBAccess.NoQuery(sql, list);

                //重建
                sql = "select ProjectID from TKY_Task where TaskID=@taskid and State = 2";
                sql = "select TKY_ID,Type from TKY_Project where ID in ("+sql+")";
                list.Clear();
                list.Add(new DictionaryEntry("@taskid", itaskid));
                DataSet ds = DBAccess.Query(sql, "TKY_Project", list);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        nCount = dt.Rows.Count;
                        int i;
                        for (i = 0; i < nCount; i++)
                        {
                            DataRow row = dt.Rows[i];
                            int tkyid = DatabaseUtility.getIntValue(row, "TKY_ID", -1);
                            string type = DatabaseUtility.getStringValue(row,"Type").Trim();
                            if (tkyid > 0)
                            {
                                string url = ServerConfig.getuser_url(type);
                                if (url != "")
                                {
                                    //从网站中下载use数据
                                    url += tkyid.ToString();
                                    downloaduser(url, itaskid);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                returnErrorInfo(context,"任务不存在");
            }

        }

        private void downloaduser(string url,int itaskid)
        {
             string json = HttpUitls.Get(url);
             if (json != null && !json.Trim().Equals("[]"))
             {
                 IList<UserClass> userList = new List<UserClass>();
                 JsonSerializerSettings jsetting = new JsonSerializerSettings();
                 jsetting.NullValueHandling = NullValueHandling.Ignore;

                 userList = JsonConvert.DeserializeObject<IList<UserClass>>(json, jsetting);
                 int nCount = userList.Count;

                 int i;
                 for (i = 0; i < nCount; i++ )
                 {
                     UserClass uc = userList[i];
                     uc.trim();
                     if (uc.ID > 0) //数据有效
                     {
                         //查找userid是否存在
                         string sql = "select count(ID) from [User] where ID=@uid";
                         IList list = new ArrayList();
                         list.Add(new DictionaryEntry("@uid", uc.ID));
                         int userexist = DBAccess.QueryStatistic(sql, list);


                         //查找表里是否有数据
                         sql = "select count(ID) from Task_Visible where TaskID=@tid and UserID=@uid";
                         list.Clear();
                         list.Add(new DictionaryEntry("@tid", itaskid));
                         list.Add(new DictionaryEntry("@uid", uc.ID));
                         int length = DBAccess.QueryStatistic(sql, list);


                         if (length == 0 && userexist>0)//没有，插入
                         {
                             sql = "insert into Task_Visible(TaskID,UserID,UserType) values(@tid,@uid,0)";
                             DBAccess.NoQuery(sql, list);
                         }
                     }
                 }
             }
        }

        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误！'}";
        }

        protected override int getErrorCode()
        {
            return 200;
        }
    }
}