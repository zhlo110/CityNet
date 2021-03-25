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
    /// search_session 的摘要说明
    /// </summary>
    public class search_session : Security
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
            string searchtext = context.Request["searchtxt"];
            if (searchtext != null && searchtext != "")
            {
                searchtext = searchtext.Trim();
                if (searchtext.Length > 0) //修改
                {
                    SessionNode pNode = new SessionNode();
                    IList searchRoot = pNode.search(searchtext, true);
                    int nCount = searchRoot.Count;
                    string children = "";
                    int i;
                    IList param = new ArrayList();
                    param.Add(false);
                    param.Add(true);
                    for (i = 0; i < nCount; i++)
                    {
                        TreeNode root = searchRoot[i] as TreeNode;
                        children += root.toJson(param) + ",";
                    }
                    if (children.Length > 0)
                    {
                        nCount = children.Length;
                        children = children.Substring(0, nCount - 1);
                    }
                    string ret = "[" + children + "]";
                    context.Response.Write(ret);
                }
            }
        }
    }
}