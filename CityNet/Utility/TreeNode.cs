using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.Utility
{
    //通用叶子节点
    public abstract class TreeNode
    {
        //孩子节点
        IList mChildren = new ArrayList();

        //叶子节点的属性
        Hashtable mProperity = new Hashtable();

        public IList Children
        {
            get { return mChildren; }
            set { mChildren = value; }
        }

        //添加属性
        public void setValue(Object key, Object Value)
        {
            mProperity[key] = Value;
        }

        public Object getValue(Object key)
        {
            if (mProperity.ContainsKey(key))
            {
                return mProperity[key];
            }
            else
            {
                return null;
            }
        }

        //父节点
        TreeNode mParent = null;

        public TreeNode Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        //查询
        private Object mKey = null;

        public Object Key
        {
            get { return mKey; }
            set { mKey = value; }
        }

        //创建树,根据权限roleid创建树
        //roleid 权限（管理员）
        //type 功能类型
        //ignorewilder = true 没有归类的功能不查询，false 全部都查询
        //ignorewildclass 是否忽略查询没有功能的功能类
        public IList createTree(int roleid, int type, bool ignorewilder,bool ignorewildclass)
        {
            Hashtable table = searchleaves(roleid, type, ignorewilder, ignorewildclass);
            return searchbynode(table);
        }

        public IList searchbynode(Hashtable nodes)
        {
            IList Root = new ArrayList();
            //遍历整个关键节点
            IList tempKeys = new ArrayList();
            foreach (Object key in nodes.Keys)
            {
                tempKeys.Add(key);

            }
            foreach (Object key in tempKeys)
            {
                TreeNode node = nodes[key] as TreeNode;
                node.TraceParent(nodes, 999);
                //     node.TraceChildren(result, 1);
            }

            foreach (Object key in nodes.Keys)
            {
                TreeNode node = nodes[key] as TreeNode;
                if (node.Parent == null)
                {
                    Root.Add(node);
                }
            }
            return Root;
        }

        //搜索建树
        //返回树的根节点,pattern搜索关键词
        //includeLeaves 是否搜索叶子节点
        public IList search(string pattern,bool includeLeaves)
        {
           
            //得到所有的查询节点
            Hashtable result = searchImp(pattern,includeLeaves);
            return searchbynode(result);

        }
        //判断有没有该孩子节点
        public bool hasChildren(TreeNode pNode)
        {
            int i,nCount;
            nCount = mChildren.Count;
            for (i = 0; i < nCount; i++ )
            {
                TreeNode child = mChildren[i] as TreeNode;
                if (pNode.Key.ToString().Trim().Equals(child.Key.ToString().Trim()))
                {
                    return true;
                }
            }
            return false;
        }

        //从node中追踪孩子节点
        //allNode 目前的节点
        //deep追踪的层数，
        public void TraceChildren(Hashtable allNode,int deep)
        {
            if (deep > 0)
            {
                //创建孩子节点
                IList list = createChildren();
                if(list.Count > 0)
                {
                    int i;
                    int nCount = list.Count;
                    for (i = 0; i < nCount; i++)
                    {
                        TreeNode pNode = list[i] as TreeNode;
                        if (!allNode.ContainsKey(pNode.Key))//如果节点不存在
                        {
                            allNode.Add(pNode.Key, pNode);
                        }
                        if (!hasChildren(pNode))
                        {
                            pNode.Parent = this;
                            Children.Add(pNode);
                        }
                        pNode.TraceChildren(allNode, deep - 1);
                    }
                }
            }
        }
        //从node中追踪父节点
        //allNode 目前的节点
        //deep追踪的层数，
        public void TraceParent(Hashtable allNode,int deep)
        {
            if (deep > 0)
            {
                TreeNode parentNode = null;
                if (this.Parent == null)
                {
                    //创建树节点
                    parentNode = createParent();
                    if (parentNode != null)
                    {
                        if (!allNode.ContainsKey(parentNode.Key))//如果节点不存在
                        {
                            allNode.Add(parentNode.Key, parentNode);
                            if (!parentNode.hasChildren(this))
                            {
                                parentNode.Children.Add(this);
                            }
                        }
                        else
                        {
                            parentNode = allNode[parentNode.Key] as TreeNode;
                            if (!parentNode.hasChildren(this))
                            {
                                parentNode.Children.Add(this);
                            }
                        }
                        this.Parent = parentNode;
                    }
                }
                else
                {
                    parentNode = this.Parent;
                }
                if (parentNode != null)
                {
                    parentNode.TraceParent(allNode, deep-1);
                }
            }
        }
        //将类转换为Json字符串
        public string toJson(IList paramaters)
        {
            bool showleaf = (bool)paramaters[0]; //是否显示叶子节点
            string children = "";
            bool isLeaf = this.isLeaf(); //是否为叶子节点
            if (isLeaf)
            {
                if (showleaf)
                {
                    return leafjson(paramaters);
                }
                else
                {
                    return "";
                }
            }
            else
            {
                int i;
                int nCount = this.Children.Count;
                for (i = 0; i < nCount; i++)
                {
                    TreeNode child = this.Children[i] as TreeNode;
                    string childrenjson = child.toJson(paramaters);
                    if (childrenjson.Trim().Length > 0)
                    {
                        children += childrenjson + ",";
                    }
                }
                if (children.Length > 0)
                {
                    nCount = children.Length;
                    children = children.Substring(0, nCount - 1);
                }
                return nodejson(children, paramaters);
            }
        }
        //要删除的节点，级联删除
        public void deleteNode(int deep)
        {
            Hashtable table = new Hashtable();
            table.Add(this.Key, this);
            this.TraceChildren(table, 999); //查找所有的子节点
            int did = -1;
            int batchdeleteNum = 50; //一次性删除多少条记录
            int k = 0;
            string ids = "";
            foreach (Object key in table.Keys)
            {
                did = (int)key;
                ids += did.ToString() + ",";
                if (k >= batchdeleteNum)//进行一次删除
                {
                    batchdelete(ids);
                    k = 0;
                    ids = "";
                }
                k++;
            }
            batchdelete(ids);
        }

        public void batchdelete(string ids)
        {
            if (ids.Length > 0)
            {
                ids = ids.Substring(0, ids.Length - 1);
                deletebyids(ids);
            }
        }

        //批量删除点
        public abstract void deletebyids(string ids);
        //查找并创建父节点，如果父节点为空表示为根节点
        public abstract TreeNode createParent();
        //查找并创建子节点，如果父节点为空表示为根节点
        public abstract IList createChildren();
        //查找并创建子叶子节点
        public abstract IList createChildrenLeaves();
        //是不是叶子节点
        public abstract bool isLeaf();
        //获取单个节点的JSON,叶子节点
        public abstract string leafjson(IList parameter);
        //获取节点的JSON,非叶子节点，childrenjson为叶子节点
        public abstract string nodejson(string childrenjson, IList parameter);

        public abstract TreeNode createNode(int ID);
        //搜索叶子节点
        //roleid 权限（管理员）
        //type 功能类型
        //ignorewilder = true 没有归类的功能不查询，false 全部都查询
        public abstract Hashtable searchleaves(int roleid, int type, bool ignorewilder, bool ignorewildclass);

        //查询数据
        protected abstract Hashtable searchImp(string pattern,bool includeLeaves);
    }
}