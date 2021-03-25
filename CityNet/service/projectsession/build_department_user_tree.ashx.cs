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
            string sessionid = context.Request["sessionid"];
            int isessionid = -1;
            if (!int.TryParse(sessionid, out isessionid))
            {
                isessionid = -1;
            }

            //配置json的输出样式
            IList jsonparamater = new ArrayList();
            jsonparamater.Add(true);//是否查User表
            jsonparamater.Add(true);//已经用过
            jsonparamater.Add(true);//是否要从数据库中查询State
            jsonparamater.Add(isessionid);

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

            IList children = node.createChildrenLeaves();

            foreach (int key in roots.Keys)
            {
                TreeNode pChild = roots[key] as TreeNode;
                json += pChild.toJson(jsonparamater) + ",";
            }
            int i;
            int nCount = children.Count;
            for (i = 0; i < nCount; i++ )
            {
                TreeNode pChild = children[i] as TreeNode;
                json += pChild.leafjson(jsonparamater)+",";
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