//任务审批按钮
function approvepanel(params) {
    showdocumentpanelimp(params, 1, 'get_approve_tasks')
}
function approvefrom2(taskid, htmlvalue, params,userjson) {
    var panel = Ext.getCmp('approve_2_windows_id');
    if (!Ext.isEmpty(panel)) return;

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


    Ext.create('Ext.window.Window',
    {
        id: 'approve_2_windows_id',
        title: '选择审核人',
        height: 360,
        width: 400,
        layout: 'fit',
        items: [{
            xtype: 'form',
            layout:'fit',
            items: [
             {
                    xtype: 'treepanel',
                    id: 'user_auditid_tree_id',
                    minHeight: 1,
                    title: '审核人',
                    height: 200,
                    margin: '1 1 1 1',
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
                                id: 'search_audit_user_id_text',
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
                }
            ],
            buttonAlign: 'center',
            buttons: [{
                text: '提交',
                handler: function () {
                    var form = this.up('form').getForm();
                    var treepanel = Ext.getCmp('user_auditid_tree_id');
                    var checkedlist = treepanel.getChecked();
                    if (Ext.isEmpty(checkedlist) || checkedlist.length == 0)
                    {
                        Ext.MessageBox.alert("提示信息", "请选择审核人");
                        return;
                    }
                    var node = checkedlist[0];
                    var auditid = node.data.qtip;//审核人
                    var windows = this.up('window');
                    if (form.isValid()) {
                        form.submit({
                            url: '/service/task/return_modify_task.ashx?params=' + params,
                            params: {
                                taskid: taskid,
                                auditid: auditid,
                                description: htmlvalue
                            },
                            success: function (fp, o) {
                                //显示浏览数据按钮
                                //从文档中读取表格并存储
                                Ext.Msg.alert('提示', o.result.msg);
                                windows.close();
                                approvepanel(params);
                            }
                        });
                    }
                }


            }]
        }]
    }).show();
}