Ext.define('Ext.overrides.tree.Column', {
    override: 'Ext.tree.Column',

    treeRenderer: function (value, metaData, record, rowIdx, colIdx, store, view) {
        var me = this,
            cls = record.get('cls'),
            rendererData;
        // The initial render will inject the cls into the TD's attributes.
        // If cls is ever *changed*, then the full rendering path is followed.
        if (metaData && cls) {
            metaData.tdCls += ' ' + cls;
        }
        rendererData = me.initTemplateRendererData(value, metaData, record, rowIdx, colIdx, store, view);

        //导航树复选框的半选状态实现
        var indeterminate = record.get('indeterminate');
        if ('boolean' === typeof (indeterminate) && indeterminate) {
            rendererData.checkboxCls += ' x-tree-checkbox-indeterminate';
        }

        return me.getTpl('cellTpl').apply(rendererData);
    }
});

Ext.define('Ext.ux.CheckboxTree', {
    extend: 'Ext.tree.Panel',
    alias: ['widget.checkboxtree'],
    scrollable: 'y',
    rootVisible: false,
    submitcount:0,
    viewConfig: {
        preserveScrollOnRefresh: true,//
        preserveScrollOnReload: true,//
        scrollable: 'y'
    },
    plugins: [
    {
        ptype: 'bufferedrenderer',
        trailingBufferZone: 20,  // Keep 20 rows rendered in the table behind scroll
        leadingBufferZone: 50   // Keep 50 rows rendered in the table ahead of scroll
    }],

    listeners: {
        "checkchange": function (node, checked, eOpts) {
            //console.log(node.data.text);
            //     node.set('indeterminate', false);
            var myMask =  Ext.getCmp('check_task_lock');
            if(myMask==null)
            {
                myMask = new Ext.LoadMask({
                    id:'check_task_lock',
                    target: this,
                    msg: '正在更新树列表，请稍后！'});
            }
            myMask.show();
            var me = this;
            me.submitcount = 0;

            this.travelChildrenChecked(node, checked, eOpts);
            node.set('indeterminate', false);
            node.set('checked', !checked);
            node.set('checked', checked);
            this.travelParentChecked(node, checked, eOpts);
            this.savefunction(node,checked,false);
        }
    },
    /** 递归遍历父节点 **/
    travelParentChecked: function (node, opts) {
        //父节点
        var me = this;
        var upNode = node.parentNode;
        if (upNode != null) {
            var opts = {};
            opts["isPassive"] = true;
            //父节点当前选中状态
            var upChecked = upNode.data.checked;

            //选中状态，遍历父节点，判断有父节点下的子节点是否都全选
	        
            var checkednum = 0; //子节点选中的数目
            var uncheckednum = 0;
            //此时父节点不可能是选中状态
            //如果有一个节点未选中，可以判断，当前父节点肯定是未选中状态，所以此时不必向上遍历
            upNode.eachChild(function (child) {
                if (child.get('indeterminate')) {
                    uncheckednum = 1;
                    checkednum = 1;
                    return;
                }

                if (!child.data.checked) {
                    uncheckednum = uncheckednum + 1;
                }
                else {
                    checkednum = checkednum + 1;
                }
            });

            if (checkednum > 0 && uncheckednum >0) //既有选中的，也也有没选中的
            {
                upNode.set('indeterminate', true);
                upNode.set('checked', !upNode.get('checked'));
                me.savefunction(upNode,true,true);//半选
            }
            else if (checkednum > 0) //只有选中的
            {
                upNode.set('indeterminate', false);
                upNode.set('checked', false);
                upNode.set('checked', true);
                me.savefunction(upNode,true,false);//全选
            }
            else { //只有没选中的
                upNode.set('indeterminate', false);
                upNode.set('checked', true);
                upNode.set('checked', false);
                me.savefunction(upNode,false,false);//不选
            }
            this.travelParentChecked(upNode, opts);
        }
    },
    /** 递归遍历子节点，复选框 **/
    travelChildrenChecked:  function(node, checkStatus, eOpts){
        var isLeaf = node.data.leaf;
        var me = this;
        if(!isLeaf){
            me.submitcount = me.submitcount + 1;
            node.expand(false, function()
            {
                if(eOpts["isPassive"] == null){//主动点击
                    node.eachChild(function (child) {
                        child.set('indeterminate', false);
                        child.set('checked', !checkStatus);
                        child.set('checked', checkStatus);
                        me.savefunction(child,checkStatus,false);
                        me.travelChildrenChecked(child, checkStatus, eOpts);
                    });
                }
                me.submitcount = me.submitcount - 1;
                if(me.submitcount <= 0)
				{
                    me.settaskfunction();
                }
               
            });
        

        }

    },
});
