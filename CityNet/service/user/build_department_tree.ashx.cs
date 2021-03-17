using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CityNet.Controllers;
using CityNet.Utility;

namespace CityNet.service.user
{
    /// <summary>
    /// build_department_tree 的摘要说明
    /// </summary>
    public class build_department_tree : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Request["node"] == "project_id")//整个项目文档
            {
                string searchtext = context.Request["searchtxt"];
                if (searchtext != null)
                {
                    searchtext = searchtext.Trim();
                    if (searchtext.Length > 0) //修改
                    {
                        searchfromtree(context, searchtext);
                        return;
                    }
                }
                string sql = "select * from Department where [ParentID] = 0 order by [DepartmentName] ";
                DataSet dataset = DBAccess.Query(sql, "Department");
                int nCount = dataset.Tables.Count;
                int kCount = 1;
                string children = "";
                if (nCount > 0)
                {
                    DataTable dt = dataset.Tables[0];
                    nCount = dt.Rows.Count;
                    int i;
                    string str = "";
                    for (i = 0; i < nCount; i++)
                    {
                        DataRow row = dt.Rows[i];
                        string name = row["DepartmentName"].ToString();
                        int id = int.Parse(row["ID"].ToString());
                        DateTime dcreatetime = (DateTime)row["CreateTime"];
                        string createtime = dcreatetime.ToString("yyyy年MM月dd日");
                        string description = row["Description"].ToString();
                      //  int leaf = int.Parse(row["Leaf"].ToString());
                        string isleaf = "false";
                        str += "{\"text\":'" + name + "',\"createtime\":\"" + createtime
                            + "\",\"description\":\"" + description + "\",\"projectName\":'" + name + "'," + "\"id\":" + id.ToString() + ",\"qtip\":'"+name+"',\"leaf\": " 
                            + isleaf +  ",\"provider\":''},";
                        kCount++;
                    }
                    if (str != "")
                    {
                        nCount = str.Length;
                        str = str.Substring(0, nCount - 1);
                    }
                    else
                    {
                        str = "";
                    }
                    children = str;
                }
                string ret = "[{\"text\":'组织机构',\"root\":true,\"id\":'root',\"qtip\":'组织机构',\"leaf\": false,\"expanded\":'true',\"cls\":'folder',\"children\":[" + children + "]}]";
                context.Response.Write(ret);
            }
            else //其他节点
            {
                string sid = context.Request["node"];
                string showPerson = context.Request["showPerson"];
                string checkedperson = context.Request["checkedperson"];
                bool ischecked = false;
                if (checkedperson != null && checkedperson.Length > 0)
                {
                    checkedperson = checkedperson.Trim();
                    if (checkedperson.Equals("true"))
                    {
                        ischecked = true;
                    }
                }
                string schecked = "";
                if (ischecked)
                {
                    schecked = "\"checked\":false,";
                }

                int pid = 0;
                bool success = int.TryParse(sid, out pid);
                if (success)
                {
                    string sql  = "select * from Department where [ParentID] =  @pid order by [DepartmentName] ";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@pid", pid));
                    DataSet dataset = DBAccess.Query(sql, "Department", list);
                    int nCount = dataset.Tables.Count;
                    int kCount = 1;
                    string children = "";
                    if (nCount > 0)
                    {
                        DataTable dt = dataset.Tables[0];
                        nCount = dt.Rows.Count;
                        int i;
                        string str = "";
                        for (i = 0; i < nCount; i++)
                        {
                            DataRow row = dt.Rows[i];
                            string name = row["DepartmentName"].ToString();
                            int id = int.Parse(row["ID"].ToString());
                            DateTime dcreatetime = (DateTime)row["CreateTime"];
                            string createtime = dcreatetime.ToString("yyyy年MM月dd日");
                            string description = row["Description"].ToString();

                   //         int leaf = int.Parse(row["Leaf"].ToString());
                            string isleaf = "false";
                            str += "{\"text\":'" + name + "',\"createtime\":\"" + createtime
                            + "\",\"description\":\"" + description + "\",\"projectName\":'" + name + "'," + "\"id\":" + id.ToString() + ",\"qtip\":'" + name + "',\"leaf\": "
                                + isleaf + ",\"provider\":''},";
                            kCount++;
                        }
                        if (str != "")
                        {
                            nCount = str.Length;
                            str = str.Substring(0, nCount - 1);
                        }
                        else
                        {
                            str = "";
                        }
                        children = str;
                    }

                    if (showPerson != null)
                    {
                        if (children.Length > 0)
                        {
                            children += ",";
                        }
                        if (showPerson.Trim().Equals("true"))
                        {
                            //根据DepartmentID找人
                            sql = "select u.ID,u.RealName,u.CreateTime,u.Description,ud.DepartmentID from [User] u,"
                                +" User_Department ud where u.ID = ud.UserID and ud.DepartmentID=@did";
                            list.Clear();
                            list.Add(new DictionaryEntry("@did", pid));
                            dataset = DBAccess.Query(sql, "User", list);
                            nCount = dataset.Tables.Count;
                            if (nCount > 0)
                            {
                                DataTable dt = dataset.Tables[0];
                                nCount = dt.Rows.Count;
                                int i;
                                for (i = 0; i < nCount; i++)
                                {
                                    DataRow row = dt.Rows[i];
                                    int id = int.Parse(row["ID"].ToString());
                                    string realName = row["RealName"].ToString();

                                    DateTime dcreatetime = (DateTime)row["CreateTime"];
                                    string createtime = dcreatetime.ToString("yyyy年MM月dd日");
                                    string description = row["Description"].ToString();

                                    string isleaf = "true";
                                    children += "{\"text\":'" + realName + "',\"createtime\":\"" + createtime
                                        + "\"," + schecked + "\"description\":\"" + description + "\",\"projectName\":'" 
                                        + realName + "'," + "\"id\":\"User_ID_" + id.ToString() + "DepartID" +
                                        pid.ToString() + "\",\"qtip\":" +
                                        id.ToString()+",\"leaf\": "+ isleaf + ",\"provider\":''},";
                                }
                                if (children != "")
                                {
                                    nCount = children.Length;
                                    children = children.Substring(0, nCount - 1);
                                }
                            }
                        }
                    }

                    context.Response.Write("[" + children + "]");
                }

            }
            //context.Response.Write("1");
        }
        //从树节点中查询数据，包括人员和部门
        private void searchfromtree(HttpContext context, string searchtext)
        {

            string showPerson = context.Request["showPerson"];
            string checkedperson = context.Request["checkedperson"];
            bool ischecked = false;
            if (checkedperson != null && checkedperson.Length > 0)
            {
                checkedperson = checkedperson.Trim();
                if (checkedperson.Equals("true"))
                {
                    ischecked = true;
                }
            }
           



            DepartmentNode dNode = new DepartmentNode();
            IList searchRoot= dNode.search(searchtext, true);
            int i;
            int nCount = searchRoot.Count;
            string children = "";
            IList param = new ArrayList();
            if (showPerson == null)
            {
                param.Add(false);
            }
            else
            {
                if (showPerson.Trim().Equals("true"))
                {
                    param.Add(true);
                }
                else
                {
                    param.Add(false);
                }
            }

            param.Add(ischecked);
            
            for(i = 0; i < nCount; i++)
            {
                TreeNode root = searchRoot[i] as TreeNode;
                children += root.toJson(param) + ",";
            }
            if (children.Length > 0)
            {
                nCount = children.Length;
                children = children.Substring(0, nCount - 1);
            }
            string ret = "[{\"text\":'组织机构',\"root\":true,\"id\":'root',\"qtip\":'组织机构',\"leaf\": false,\"expanded\":'true',\"cls\":'folder',\"children\":[" + children + "]}]";
            context.Response.Write(ret);

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}