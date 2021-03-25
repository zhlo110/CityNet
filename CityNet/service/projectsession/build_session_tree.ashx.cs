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
    /// build_session_tree 的摘要说明
    /// </summary>
    public class build_session_tree : Security
    {

        protected override string getErrorMessage()
        {
            return "[]";
        }
        protected override int getErrorCode()
        {
            return 200;
        }

        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //创建功能树，不包含叶子节点
            string node = context.Request["node"];

            TreeNode pNode = new SessionNode();
            int nodeid = 0;
            if (int.TryParse(node, out nodeid))
            {
                pNode = pNode.createNode(nodeid);
            }
            else
            {
                pNode = pNode.createNode(0);
            }
            Hashtable table = new Hashtable();
            pNode.TraceChildren(table, 1);

            IList paramaters = new ArrayList();
            paramaters.Add(false); //是否展开
            paramaters.Add(false); //是否展开
            String writeret = "";
            foreach (string keys in table.Keys)
            {
                TreeNode root = (TreeNode)table[keys];
                string nodejson = root.toJson(paramaters);
                if (nodejson.Length > 0)
                {
                    writeret += nodejson + ",";
                }
            }
            if (writeret.Length > 0)
            {
                writeret = writeret.Substring(0, writeret.Length - 1);
            }
            writeret = "[" + writeret + "]";
            context.Response.Write(writeret);
        }

    }
}