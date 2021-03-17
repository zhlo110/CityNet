//资料审批步
function point_approve(parentpanel, params, userjson)
{
    parentpanel.removeAll();
    var taskid = parentpanel.up().taskid;

    var audittree = new Ext.data.TreeStore(
    {
        autoSync: false,
        proxy: {
            type: 'ajax',
            url: '/service/user/build_department_tree.ashx?showPerson=true&checkedperson=true',
            reader: 'json'
        },
        root: {
            name: '根节点',
            id: 'project_id'
        }
    });

   var copytotree = new Ext.data.TreeStore(
   {
       autoSync: false,
       proxy: {
           type: 'ajax',
           url: '/service/user/build_department_tree.ashx?showPerson=true&checkedperson=true',
           reader: 'json'
       },
       root: {
           name: '根节点',
           id: 'project_id'
       }
   });

    parentpanel.add({
        xtype: 'panel',
        layout: 'border',
        width: 500,
        height: 300,
        items: [
        {
            xtype: 'treepanel',
            id: 'user_auditid_tree_id',
            minHeight: 1,
            title: '审核人',
            region: 'north',     // position for region
            height: 200,
            split: true,         // enable resizing
            margin: '0 5 5 5',
            scrollable: true,
            border: true,
            scrollable: 'y',
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
            store: audittree,
            dockedItems: [{
                xtype: 'toolbar',
                dock: 'top',
                items: [
                    { xtype: 'tbspacer', width: 10 },
                    {
                        xtype: 'textfield',
                        id:'search_audit_user_id_text',
                        text: '搜索',
                        emptyText: '请输入姓名或组织结构。'
                    },
                    {
                        xtype: 'button',
                        text: '搜索',
                        handler: function () {
                            var searchfield = Ext.getCmp('search_audit_user_id_text');
                            //alert(searchfield.value);
                            var treepanel = Ext.getCmp('user_auditid_tree_id');
                            var store = treepanel.getStore();
                            store.proxy.url = '/service/user/build_department_tree.ashx?showPerson=true&checkedperson=true&searchtxt=' + searchfield.value;
                            store.load();
                        }
                    }
                ]
            }],
            listeners: {
                checkchange: function (node, checked, eOpts) {
                    treeCheckfalse(this); //只能单选

                    if (node.data.qtip != userjson.id) {
                        node.set('checked', checked);
                    }
                }
            },
            rootVisible: false
        },
        {
            xtype: 'treepanel',
            id: 'user_copyto_tree_id',
            minHeight: 1,
            title: '抄送人',
            region: 'center',     // position for region
            height: 200,
            margin: '0 5 5 5',
            scrollable: true,
            border: true,
            scrollable: 'y',
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
            store: copytotree,
            dockedItems: [{
                xtype: 'toolbar',
                dock: 'top',
                items: [
                    { xtype: 'tbspacer', width: 10 },
                    {
                        xtype: 'textfield',
                        id:'search_copyto_user_id_text',
                        text: '搜索',
                        emptyText: '请输入姓名或组织结构。'
                    },
                    {
                        xtype: 'button',
                        text: '搜索',
                        handler: function () {
                            var searchfield = Ext.getCmp('search_copyto_user_id_text');
                            //alert(searchfield.value);
                            var treepanel = Ext.getCmp('user_copyto_tree_id');
                            var store = treepanel.getStore();
                            store.proxy.url = '/service/user/build_department_tree.ashx?showPerson=true&checkedperson=true&searchtxt=' + searchfield.value;
                            store.load();
                        }
                    }
                ]
            }],
            listeners: {
               checkchange: function (node, checked, eOpts) {

                   if (node.data.qtip == userjson.id) {
                       node.set('checked', false);
                   }
               }
            },
            rootVisible: false
        },
        {
            title: '说明/任务提交',
            region: 'south',     // position for region
            xtype: 'panel',
            layout: 'fit',
            height: 200,
            split: true,         // enable resizing
            margin: '0 5 5 5',
            items: [
                {
                    xtype: 'form',
                    layout: 'fit',
                    id:'submit_task_form_id',
                    bodyPadding: 5,
                    buttonAlign:'center',
                    items: [{
                        //                        name: 'description',
                        id:'html_value_id',
                        xtype: 'htmleditor'
                    }],
                    buttons: [
                        {
                            text: '提交任务',
                            handler: function () {
                                //
                                //
                                var auditpanel = Ext.getCmp('user_auditid_tree_id');
                                var copytopanel = Ext.getCmp('user_copyto_tree_id');
                                var htmleditor = Ext.getCmp('html_value_id');
                                var htmlvalue = Ext.String.htmlEncode(htmleditor.value);

                                var nodes = auditpanel.getChecked();
                                if (nodes.length <= 0) {
                                    Ext.MessageBox.alert("提示信息", "请选择审核人");
                                }
                                else {
                                    var i = 0;
                                    var node = nodes[0];
                                    var auditid = node.data.qtip;//审核人
                                    var copynodes = copytopanel.getChecked();
                                    var copynodesid = [];
                                    for (i = 0; i < copynodes.length; i++) {
                                        node = copynodes[i];
                                        copynodesid.push(node.data.qtip);//抄送人
                                    }
                                    Ext.MessageBox.confirm("提示", "是否要提交任务？", function (btnId) {
                                        if (btnId == "yes") {
                                            var form = Ext.getCmp('submit_task_form_id');

                                            var thisForm = form.getForm();
                                            if (thisForm.isValid()) {
                                                thisForm.submit({
                                                    url: '/service/task/submit_task.ashx?params=' + params,
                                                    params: {
                                                        taskid: taskid,
                                                        auditid: auditid,
                                                        description: htmlvalue,
                                                        copynodesid: copynodesid
                                                    },
                                                    success: function (form, action) {
                                                        var json = Ext.decode(action.response.responseText)
                                                        Ext.MessageBox.alert("提示信息", json.msg);
                                                        showdocumentpanel(params);
                                                    },
                                                    failure: function (form, action) {
                                                        var json = Ext.decode( action.response.responseText)
                                                        Ext.MessageBox.alert("提示信息", json.msg);
                                                        showdocumentpanel(params);
                                                    }

                                                });
                                            }
                                           
                                        }
                                    });


                                }


                            }
                        },
                        {
                            text: '终止任务',
                            handler: function () {

                                Ext.MessageBox.confirm("提示", "是否要终止任务？", function (btnId) {
                                    if (btnId == "yes") {

                                        Ext.Ajax.request({
                                            url: '/service/task/endtask.ashx?params=' + params,
                                            params: {
                                                taskid: taskid
                                            },
                                            success: function (form, action) {
                                                var json = Ext.decode(form.responseText);
                                                Ext.MessageBox.alert("提示信息", json.msg);
                                                showdocumentpanel(params);
                                            },
                                            failure: function (form, action) {
                                                var errorjson = Ext.decode(form.responseText);
                                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                showdocumentpanel(params);
                                            }
                                        });

                                    }
                                });

                                


                            }
                        }
                    ]
                }
            ]
        }]
    });
}

function treeCheckfalse(tree) {
    var nodes = tree.getChecked();
    if (nodes && nodes.length) {
        for (var i = 0; i < nodes.length; i++) {
            nodes[i].set('checked', false);
        }
    }
}
