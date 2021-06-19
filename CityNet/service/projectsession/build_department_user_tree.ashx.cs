using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.projectsession
{
    /// <summary>
    /// build_department_user_tree 的摘要说明
    /// </summary>
    public class build_department_user_tree : Security
    {

        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //得到sessionID
            string showleaves = context.Request["showleaves"];
            string sessionid = context.Request["sessionid"]; //人员管理中的参数
            string taskid = context.Request["taskid"]; //授权的参数

            int itaskid = -1;
            if (taskid != null && taskid != "")
            {
                taskid = taskid.Trim();
                if (!int.TryParse(taskid, out itaskid))
                {
                    itaskid = -1;
                }
                if (itaskid == -1)
                {
                    context.Response.Write("[]");
                    return;
                }
            }


            int isessionid = -1;
            if (!int.TryParse(sessionid, out isessionid))
            {
                isessionid = -1;
            }
            bool bshowleaves = true; //默认为true
            if (showleaves != null && showleaves != "")
            {
                showleaves = showleaves.Trim();
                if (showleaves.Equals("false"))
                {
                    bshowleaves = false;
                }
            }


            //配置json的输出样式
            IList jsonparamater = new ArrayList();
            jsonparamater.Add(true);//是否查User表
            jsonparamater.Add(true);//已经用过
            jsonparamater.Add(true);//是否要从数据库中查询State
            jsonparamater.Add(isessionid);
            if (itaskid > 0)//taskid起作用，isessionid将不起作用
            {
                jsonparamater.Add(itaskid);
            }

            TreeNode node = new DepartmentNode();
            //判断节点是否为根节点
            if (context.Request["node"] == "project_id")//整个项目文档
            {
                //搜索根节点数据
                node.setValue("ID", 0);
            }
            else //其他节点（department节点）
            {
                string departmentid = context.Request["node"];
                int idepartmentid = -1;
                if (!int.TryParse(departmentid, out idepartmentid))
                {
                    idepartmentid = -1;
                }
                node.setValue("ID", idepartmentid);
            }

            Hashtable roots = new Hashtable();
            node.TraceChildren(roots, 1);
            string json = "";
            IList children = null;
            if (bshowleaves) //显示叶子节点
            {
                children = node.createChildrenLeaves();
            }

            foreach (int key in roots.Keys)
            {
                TreeNode pChild = roots[key] as TreeNode;
                json += pChild.toJson(jsonparamater) + ",";
            }
            if (children != null)
            {
                int i;
                int nCount = children.Count;
                for (i = 0; i < nCount; i++)
                {
                    TreeNode pChild = children[i] as TreeNode;
                    json += pChild.leafjson(jsonparamater) + ",";
                }
            }

            if (json.Length > 0)
            {
                json = json.Substring(0, json.Length - 1);
            }
            json = "[" + json + "]";
            context.Response.Write(json);
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