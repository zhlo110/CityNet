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

namespace CityNet.service.teyserver
{
    /// <summary>
    /// build_project_section_tree_from_tkyserver 的摘要说明
    /// </summary>
    public class build_project_section_tree_from_tkyserver : Security
    {
        public delegate IList<AreaClass> filterResult(string json, string pattern, int parentid);

        public IList<AreaClass> nofilterResult(string json, string pattern, int parentid)
        {
            return JsonConvert.DeserializeObject<IList<AreaClass>>(json);
        }

        public IList<AreaClass> filterResultbyID(string json, string pattern, int parentid)
        {
            IList<AreaClass> areaList = new List<AreaClass>();
            if (parentid == 0)
            {
                areaList = JsonConvert.DeserializeObject<IList<AreaClass>>(json);
            }
            else
            {
                int i, nCount;
                if (pattern.Equals(ServerConfig.ProjectType.SECTION))
                {
                    IList<ProjectSection> sections = JsonConvert.DeserializeObject<IList<ProjectSection>>(json);
                    nCount = sections.Count;
                    for (i = 0; i < nCount; i++)
                    {
                        ProjectSection ps = sections[i];
                        if (ps.ProjectinfoId == parentid)
                        {
                            AreaClass ac = new AreaClass();
                            ac.Name = ps.Name;
                            ac.ID = ps.ID;
                            areaList.Add(ac);
                        }
                    }
                }
                else if (pattern.Equals(ServerConfig.ProjectType.AREA))
                {
                    IList<ProjectArea> sections = JsonConvert.DeserializeObject<IList<ProjectArea>>(json);
                    nCount = sections.Count;
                    for (i = 0; i < nCount; i++)
                    {
                        ProjectArea ps = sections[i];
                        if (ps.ProjectSectionId == parentid)
                        {
                            AreaClass ac = new AreaClass();
                            ac.Name = ps.Name;
                            ac.ID = ps.ID;
                            areaList.Add(ac);
                        }
                    }
                }
            }
            return areaList;
        }

        private int getState(AreaClass ac, string pattern, int taskid)
        {
            int state = -1;
            string insql = "select ID from TKY_Project where TKY_ID = @tid and Type = @ty";
            string sql = "select State from TKY_Task where ProjectID = (" + insql + ") and TaskID=@taskid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tid", ac.ID));
            list.Add(new DictionaryEntry("@ty", pattern));
            list.Add(new DictionaryEntry("@taskid", taskid));

            DataSet ds = DBAccess.Query(sql, "TKY_Task", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    if(nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        state = DatabaseUtility.getIntValue(row, "State",-1);
                    }
                }
            }
            return state;
        }

        public void getServerData(HttpContext context,string url, string pattern,
            string leaf,int parentID,filterResult func,int taskid)
        {
            string json = HttpUitls.Get(url);
            if (json != null&&json!="")
            {
                try
                {
                    IList<AreaClass> areaList = null;
                    areaList = func(json, pattern, parentID);
                  //  areaList = JsonConvert.DeserializeObject<IList<AreaClass>>(json);
                    int nCount = areaList.Count;
                    int i;
                    string wstr = "";
                    for (i = 0; i < nCount; i++)
                    {
                        AreaClass ac = areaList[i];
                        int state = getState(ac,pattern,taskid);
                        string check = "false";
                        string indeterminate = "false";
                        if (state == 1)
                        {
                            check = "true";
                            indeterminate = "true";
                        }
                        else if (state == 2)
                        {
                            check = "true";
                            indeterminate = "false";
                        }

                        wstr += "{\"text\":'" + ac.Name + "',\"checked\":" + check 
                            + ",\"indeterminate\":" + indeterminate + ",\"id\":'" 
                            + pattern + "_" + ac.ID.ToString() + "',\"qtip\":'" + ac.Name + "',\"leaf\": " + leaf 
                            + ",\"expanded\":false,\"cls\":'folder'},";
                    }
                    if (wstr.Length > 0)
                    {
                        wstr = wstr.Substring(0, wstr.Length - 1);
                    }
                    context.Response.Write("[" + wstr + "]");
                }
                catch (Exception ex)
                {
                    string ret = "[{\"text\":'数据解析出错',\"root\":true,\"id\":'root',\"qtip\":'root',\"leaf\": false,\"expanded\":'false',\"cls\":'folder'}]";
                    context.Response.Write(ret);
                }
            }
            else
            {
                string ret = "[{\"text\":'该用户没有权限查询标段，请联系管理员设置',\"root\":true,\"id\":'root',\"qtip\":'root',\"leaf\": false,\"expanded\":'true',\"cls\":'folder'}]";
                context.Response.Write(ret);
            }
        }



        private void getAllProject(HttpContext context, Hashtable table,bool urladdid, filterResult fun,int taskid)
        {
            context.Response.ContentType = "text/plain";
            if (table.ContainsKey(context.Request["node"]))
          //  if (context.Request["node"] == "project_id")//整个项目文档
            {
                //获取7大区
                IList list = (IList)table[context.Request["node"]];
                getServerData(context, list[0].ToString(), list[1].ToString(), list[2].ToString(), 0,fun,taskid);
             //   pair.Key,
             //   getServerData(context, ServerConfig.mSevenArea, "area", "false");
            }
            else //其他节点
            {
                string parentid = context.Request["node"];
                char[] split = new char[1];
                split[0] = '_';
                string[] vec = parentid.Split(split);
                if (vec.Length >= 2)
                {
                    string type = vec[0].Trim();
                    string sid = vec[1].Trim();
                    if (table.ContainsKey(type))
                    {
                        int iid = -1;
                        if (!int.TryParse(sid, out iid))
                        {
                            iid = -1;
                        }
                        IList list = (IList)table[type];
                        if (urladdid)
                            getServerData(context, list[0].ToString() + iid.ToString(), list[1].ToString(), list[2].ToString(), iid,fun,taskid);
                        else
                            getServerData(context, list[0].ToString(), list[1].ToString(), list[2].ToString(), iid, fun,taskid);
                    }
                }
            }
        }

        //从铁科院网址中下载数据
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            //
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);
            string taskid = "";
            if (context.Request["taskid"] != null)
            {
                taskid = context.Request["taskid"].Trim();
            }
            int itaskid = -1;
            if (!int.TryParse(taskid, out itaskid))
            {
                itaskid = -1;
            }
            if (userid > 0 && itaskid > 0)
            {
                int groupid = LogUtility.GetGroupIDByUserID(userid);
                if (groupid > 0)
                {
                    string sql = "select count(agv.ID) from Action_Group_View agv,ActionClass ac " +
                        "where agv.ActionGroupID = ac.ID and agv.ActionUrl = @au and agv.GroupID= @gid and ac.ActionType=@at";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@au", "all_projectinfo"));
                    list.Add(new DictionaryEntry("@gid", groupid));
                    list.Add(new DictionaryEntry("@at", 5));
                    int iExist = DBAccess.QueryStatistic(sql, list);
                    Hashtable table = new Hashtable();
                    
                    if (iExist > 0)
                    {
                        //查看所有的数据
                        //第一层，七大区
                        IList list1 = new ArrayList();
                        list1.Add(ServerConfig.mSevenArea);
                        list1.Add(ServerConfig.ProjectType.ALLAREA);
                        list1.Add("false");
                        table["project_id"] = list1;
                        //第二层，项目
                        IList list2 = new ArrayList();
                        list2.Add(ServerConfig.mProjectInfoUrl);
                        list2.Add(ServerConfig.ProjectType.PROJECT);
                        list2.Add("false");
                        table[ServerConfig.ProjectType.ALLAREA] = list2;

                        //第三层，标段
                        IList list3 = new ArrayList();
                        list3.Add(ServerConfig.mProjectSectionUrl);
                        list3.Add(ServerConfig.ProjectType.SECTION);
                        list3.Add("false");
                        table[ServerConfig.ProjectType.PROJECT] = list3;

                        //第四层，工区
                        IList list4 = new ArrayList();
                        list4.Add(ServerConfig.mProjectAreaUrl);
                        list4.Add(ServerConfig.ProjectType.AREA);
                        list4.Add("true");
                        table[ServerConfig.ProjectType.SECTION] = list4;
                        //projectsection
                        getAllProject(context, table,true,new filterResult(this.nofilterResult),itaskid);
                    }
                    else
                    {
                        //仅查看用户可以看到的数据
                        //第一层，项目
                        IList list1 = new ArrayList();
                        list1.Add(ServerConfig.mUserVisibleProjectUrl + userid.ToString());
                        list1.Add(ServerConfig.ProjectType.PROJECT);
                        list1.Add("false");
                        table["project_id"] = list1;

                        //第二层，标段
                        IList list2 = new ArrayList();
                        list2.Add(ServerConfig.mUserVisibleSectionUrl + userid.ToString());
                        //list2.Add("projectsection");
                        list2.Add(ServerConfig.ProjectType.SECTION);
                        list2.Add("false");
                        table[ServerConfig.ProjectType.PROJECT] = list2;

                        //第三层，工区
                        IList list3 = new ArrayList();
                        list3.Add(ServerConfig.mUserVisibleProjectAreaUrl + userid.ToString());
                        //list3.Add("projectarea");
                        list3.Add(ServerConfig.ProjectType.AREA);
                        list3.Add("true");
                        table[ServerConfig.ProjectType.SECTION] = list3;

                        getAllProject(context, table, false ,new filterResult(this.filterResultbyID),itaskid);
                    }
                }
                else
                {
                    context.Response.Write("[]");
                }
            }
            else
            {
                context.Response.Write("[]");
            }
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