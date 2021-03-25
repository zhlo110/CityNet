using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.projectsession
{
    /// <summary>
    /// build_session_tree_by_taskID 的摘要说明
    /// </summary>
    public class build_session_tree_by_taskID : Security
    {

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
                paramaters.Add(false); //是否有叶子节点
                paramaters.Add(false); //是否展开
                paramaters.Add(true); //是否有checkbox
                paramaters.Add(itaskid); //传入taskid参数
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