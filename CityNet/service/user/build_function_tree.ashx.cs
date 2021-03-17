using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CityNet.Controllers;
using CityNet.Utility;
using CityNet.security;

namespace CityNet.service.user
{
    /// <summary>
    /// build_function_tree 的摘要说明
    /// </summary>
    public class build_function_tree : Security
    {
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //根据用户ID决定管理树左侧内容
            //Type 要为1
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);
            IList paramaters = new ArrayList();
            paramaters.Add(true); //是否显示叶子节点
            paramaters.Add(false);//是否显示checkbox
            paramaters.Add(-1);
            string type = context.Request["funtype"];
            int itype = 1;
            if (!int.TryParse(type, out itype))
            {
                itype = 1;
            }
            
            if (userid > 0) //数据验证成功
            {
                //找groupID
                int groupID = LogUtility.GetGroupIDByUserID(userid);
                if (groupID > 0) //找GroupID
                {
                    TreeNode pNode = new FunctionClassNode();
                    IList tree = pNode.createTree(groupID, itype, true, true);

                    String writeret = "";
                    int i;
                    int nCount = tree.Count;
                    for (i = 0; i < nCount; i++)
                    {
                        TreeNode root = (TreeNode)tree[i];
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
                    context.Response.Write(getErrorMessage());
                }
            }
            else
            {
                context.Response.Write(getErrorMessage());
            }
        }

        /*
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            string emptytree = getErrorMessage();
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            string department = parameters[2].ToString();


            String sql = "select g.ID,d.DepartmentName,g.GroupName,g.RealGroupName " +
                          "from [User] u,[Department] d, [User_Group] ug,[Group] g,[User_Department] ud " +
                          "where ud.UserID=u.ID and ud.DepartmentID = d.ID " +
                          "and ug.UserID = u.ID and ug.GroupID = g.ID " +
                          "and u.UserName = @un and u.PassWord = @pw " +
                          "and d.ID=@did";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@un", username));
            list.Add(new DictionaryEntry("@pw", password));
            list.Add(new DictionaryEntry("@did", department));
            DataSet dataset = DBAccess.Query(sql, "Group", list);
            if (dataset != null)
            {
                int nCount = dataset.Tables.Count;
                if (nCount > 0)
                {
                    DataTable dt = dataset.Tables[0];
                    nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        int i;
                        string ids = "";
                        for (i = 0; i < nCount; i++)
                        {
                            DataRow row = dt.Rows[i];
                            string id = row["ID"].ToString();
                            ids += id + ",";
                        }
                        ids = "(" + ids.Substring(0, ids.Length - 1) + ")";

                        sql = "select distinct a.* from [Action] a,[Action_Group] ag where a.[ID] = ag.ActionID and ag.[GroupID] in " + ids;
                        DataSet actiondataset = DBAccess.Query(sql, "Action");
                        if (actiondataset != null)
                        {
                            nCount = actiondataset.Tables.Count;
                            if (nCount > 0)
                            {
                                dt = actiondataset.Tables[0];
                                nCount = dt.Rows.Count;
                                if (nCount > 0)
                                {
                                    IList userids = new ArrayList();
                                    for (i = 0; i < nCount; i++)
                                    {
                                        userids.Add(int.Parse(dt.Rows[i]["ID"].ToString()));
                                    }

                                    //
                                    IList leaflist = new ArrayList();
                                    IList tree = FunctionTreeUtility.createFullFucTree(leaflist, false, 0, false);
                                    FunctionTreeUtility.resetTreeState(leaflist, userids);

                                    nCount = tree.Count;
                                    String writeret = "";
                                    for (i = 0; i < nCount; i++)
                                    {
                                        FunctionTreeNode root = (FunctionTreeNode)tree[i];
                                        string nodejson = root.toJson(true, false, false, false);
                                        if (nodejson.Length > 0)
                                        {
                                            writeret += nodejson + ",";
                                        }
                                    }
                                    if (writeret.Length > 0)
                                    {
                                        writeret = writeret.Substring(0, writeret.Length - 1);
                                    }
                                    // writeret = "[{\"text\":'项目根目录',\"id\":'root',\"qtip\":'root',\"leaf\": false,\"expanded\":'true',\"cls\":'folder',\"children\":[" + writeret + "]}]";
                                    writeret = "[" + writeret + "]";
                                    context.Response.Write(writeret);
                                }
                                else
                                {
                                    context.Response.Write(emptytree);
                                }
                            }
                            else
                            {
                                context.Response.Write(emptytree);
                            }
                        }
                        else
                        {
                            context.Response.Write(emptytree);
                        }

                    }
                    else
                        context.Response.Write(emptytree);
                }
                else
                    context.Response.Write(emptytree);
            }
            else
                context.Response.Write(emptytree);

        }*/
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