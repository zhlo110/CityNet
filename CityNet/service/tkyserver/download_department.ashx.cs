using Newtonsoft.Json;
using CityNet.Controllers;
using CityNet.Models;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.tkyserver
{
    /// <summary>
    /// download_department 的摘要说明
    /// </summary>
    public class download_department : Security
    {

        public bool getServerData(HttpContext context, string url)
        {
            bool ret = false;
            string json = HttpUitls.Get(url);
            string taskid = context.Request["taskid"];
            if (json != null&&!json.Trim().Equals("[]"))
            {
                try
                {
                    IList<DepartmentClass> depList = new List<DepartmentClass>();
                    JsonSerializerSettings jsetting = new JsonSerializerSettings();
                    jsetting.NullValueHandling = NullValueHandling.Ignore;
                    depList = JsonConvert.DeserializeObject<IList<DepartmentClass>>(json,jsetting);
                    int nCount = depList.Count;
                    
                    int i;
                    string sql = "";
                    sql = "select count([ID]) from [Department] where [ID]=@id";
                    IList list = new ArrayList();
                    
                   

                    string insertsql = "insert [Department]([ID],[DepartmentName],[ParentID],"+
                        "[Description],[CreateTime],[tkyShortName],"+
                        "[tkyGrade],[tkydepLevel],[tkyCategoryID],[tkyProjectInfoID],[tkyProjectSectionID],[tkyProjectAreaID]) values"+
                        "(@id,@name,@pid,@des,@ct,@shortname,@tg,@tl,@tc,@tpid,@tpsid,@tpaid)";

                    string updatesql = "update [Department] set " +
                        "[DepartmentName] = @name," +
                        "[ParentID] = @pid," +
                        "[Description] = @des," +
                        "[CreateTime] = @ct," +
                        "[tkyShortName] = @shortname," +
                        "[tkyGrade] = @tg," +
                        "[tkydepLevel] = @tl," +
                        "[tkyCategoryID] = @tc," +
                        "[tkyProjectInfoID] = @tpid," +
                        "[tkyProjectSectionID] = @tpsid," +
                        "[tkyProjectAreaID] = @tpaid where [ID] = @id";


                    initProgressBar(taskid, nCount);
                    for (i = 0; i < nCount; i++)
                    {

                        updateProgress(taskid,i);
                        DepartmentClass dc = depList[i];
                        dc.trim();
                        if(dc!= null)
                        {
                            list.Clear();
                            list.Add(new DictionaryEntry("@id", dc.ID));
                            int querycountb = DBAccess.QueryStatistic(sql, list);
                            list.Clear();
                            DateTime ctime = DateTime.Now;
                          //  if (dc.CreateDate > 0)
                          //  {
                             //   ctime = new DateTime(dc.CreateDate);
                          //  }
                            
                            list.Add(new DictionaryEntry("@id", dc.ID));
                            list.Add(new DictionaryEntry("@name", dc.Name));
                            list.Add(new DictionaryEntry("@pid", dc.ParentID));
                            list.Add(new DictionaryEntry("@des", ""));
                            list.Add(new DictionaryEntry("@ct", ctime));
                            list.Add(new DictionaryEntry("@shortname", dc.ShortName == null ? "" : dc.ShortName));
                            list.Add(new DictionaryEntry("@tg", dc.Grade));
                            list.Add(new DictionaryEntry("@tl", dc.DepLevel));
                            list.Add(new DictionaryEntry("@tc", dc.CategoryId));
                            list.Add(new DictionaryEntry("@tpid", dc.ProjectInfoId));
                            list.Add(new DictionaryEntry("@tpsid", dc.ProjectSectionId));
                            list.Add(new DictionaryEntry("@tpaid", dc.ProjectAreaId));

                            if (querycountb > 0) //更新数据
                            {
                                DBAccess.NoQuery(updatesql, list);
                            }
                            else //插入数据
                            {
                                DBAccess.NoQuery(insertsql, list);
                            }

                        }
                    }
                    ret = true;
                }
                catch (Exception ex)
                {
                }
            }
            return ret;
        }

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            int k = 1;
            while (getServerData(context, ServerConfig.mDownloadDepartmentUrl+k.ToString()))
            {
                k++;
            }

            string taskid = context.Request["taskid"];
            deleteProgress(taskid);
            returnInfo(context,"更新成功！");
           // throw new NotImplementedException();
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