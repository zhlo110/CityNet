function edit_resource_state(params) {
    var funpanel = Ext.getCmp('function_area_id');
    funpanel.removeAll();

    Ext.Ajax.request({
        url: '/service/user/get_userinfomation.ashx?params=' + params,
        success: function (form, action) {
            var userjson = Ext.decode(form.responseText);//当前用户信息
            var datastore = Ext.create('Ext.data.Store', {
                model: 'taskState',
                pageSize: 15,
                proxy: {
                    type: 'ajax',
                    url: '/service/task/get_all_taskstate.ashx?params=' + params,
                    reader: {
                        type: 'json',
                        rootProperty: 'roots',
                        totalProperty: 'totalCount'
                    },
                    autoLoad: true
                }
            });
            datastore.loadPage(1);

            var panel = Ext.create({
                xtype: 'panel',
                layout: 'column',
                width: '98%',
                border: false,
                items: [ 
                    {
                        xtype: 'gridpanel',
                        id: "state_gridpanel_id",
                        padding: '15 0 15 0',
                        columnWidth: 0.99,
                        title: '任务状态',
                        minHeight: 300,
                        forceFit: true,
                        store: datastore,
                        columns: [
                            { header: 'ID',minWidth:1, hidden:true,sortable: false, menuDisabled: true, draggable: false, dataIndex: 'stateid' },
                            {
                                header: '任务可编辑', minWidth: 1,
                                xtype: 'checkcolumn',
                                listeners:
                                {
                                    beforecheckchange: function (item, rowIndex, checked, record, eOpts) {
                                        Ext.Ajax.request({
                                            url: '/service/task/change_state_eidtable.ashx?params=' + params,
                                            params: {
                                                stateid: record.data.stateid,
                                                editable: checked
                                            },
                                            success: function (form, action) {
                                                datastore.loadPage(datastore.currentPage);
                                            },
                                            failure: function (form, action) {
                                                var errorjson = Ext.decode(form.responseText);
                                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                datastore.loadPage(datastore.currentPage);
                                            }
                                        });
                                    }
                                },
                              //  checkchange ( this, rowIndex, checked, record, eOpts ) 
                                sortable: false, menuDisabled: true, draggable: false, dataIndex: 'editable'
                            },
                            {
                                header: '状态名', minWidth: 1,
                                editor: { xtype: 'textfield', allowBlank: false, emptyText: '状态名不能为空', blankText: '状态名不能为空' },
                                sortable: false, menuDisabled: true, draggable: false, dataIndex: 'statename'
                            },
                            {
                                header: '排序优先级', minWidth: 1,
                                editor: {
                                    xtype: 'textfield', allowBlank: false, emptyText: '优先级不能为空', blankText: '优先级不能为空',
                                    regex: /^$|^[1-9]\d*$/,
                                    regexText: '输入必须为数字'
                                },
                                sortable: false, menuDisabled: true, draggable: false, dataIndex: 'statepriority'
                            },

                            

                            { header: '创建人', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'creator' },
                            { header: '创建时间', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'createdate' },
                            {
                                header: '描述', minWidth: 1,
                                editor: { xtype: 'textfield'},
                                sortable: false, menuDisabled: true, draggable: false, dataIndex: 'description'
                            }
                        ],
                        plugins: {
                            ptype: 'treeediting',
                            clicksToEdit: 2,
                            pluginId: 'state_editingplugin_id',
                            saveBtnText: '保存',
                            cancelBtnText: "取消",
                            listeners: {
                                edit: function (editor, context, eOpts) {

                                    Ext.Ajax.request({
                                        url: '/service/task/insert_update_task_state.ashx?params=' + params,
                                        params: {
                                            stateid: context.record.data.stateid,
                                            statename: context.record.data.statename,
                                            statepriority: context.record.data.statepriority,
                                            description: context.record.data.description
                                        },
                                        success: function (form, action) {
                                            var json = Ext.decode(form.responseText);
                                            Ext.MessageBox.alert("提示信息", json.msg);
                                            datastore.loadPage(1);
                                        },
                                        failure: function (form, action) {
                                            var errorjson = Ext.decode(form.responseText);
                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                            datastore.loadPage(1);
                                        }
                                    });
                                    //      context.record.data.functionname,context.record.data.description,
                                },
                                canceledit: function (editor, context, eOpts) {

                                    var gridpanel = Ext.getCmp('state_gridpanel_id');
                                    var gridstore = gridpanel.getStore();
                                    var index = gridstore.find('stateid', -1);
                                    if (index >=0 ) {
                                        gridstore.removeAt(index);
                                        return;
                                    }
                                }
                            }
                        },
                        bbar: Ext.create('Ext.PagingToolbar', {
                            store: datastore,
                            displayInfo: true,
                            displayMsg: '显示的条目 {0} - {1} of {2}',
                            emptyMsg: "没有下载项目"
                        }),
                        dockedItems: [{
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                               {
                                   xtype: 'button',
                                   iconCls: 'button add',
                                   text: '添加',
                                   handler: function () {
                                       var statepanel = Ext.getCmp('state_gridpanel_id');
                                       statepanel.getView().editingPlugin.cancelEdit();
                                       var gridstore = statepanel.getStore();

                                       var index = gridstore.find('stateid', -1);
                                       if (index < 0) {
                                           var curDate = new Date();
                                           var newnode = gridstore.insert(0, {
                                               stateid: -1,
                                               statename: '', statepriority: 1, creator: userjson.realname, createdate: Ext.Date.format(curDate, 'Y年m月d日'), description: ''
                                           });
                                           statepanel.getSelectionModel().select(newnode);
                                           statepanel.getPlugin('state_editingplugin_id').startEdit(statepanel.getSelection()[0]);
                                       }

                                   }
                               },
                                {
                                    xtype: 'button',
                                    iconCls: 'button delete',
                                    text: '删除',
                                    handler: function () {
                                        var statepanel = Ext.getCmp('state_gridpanel_id');
                                        statepanel.getView().editingPlugin.cancelEdit();
                                        var selectNode = statepanel.selection;
                                        if (selectNode == null) {
                                            Ext.MessageBox.alert("提示信息", '当前选择的节点为空，请选择要删除的节点');
                                        }
                                        else {
                                            //  alert(selectNode);

                                            Ext.Ajax.request({
                                                url: '/service/task/check_taskstate_is_used.ashx?params=' + params,
                                                params: {
                                                    stateid: selectNode.data.stateid,
                                                },
                                                success: function (form, action) {
                                                    var rightjson = Ext.decode(form.responseText);
                                                    if (rightjson.userednum > 0) {
                                                        Ext.MessageBox.confirm("提示", "该状态已经被使用，是否要删除？", function (btnId) {
                                                            if (btnId == "yes") {
                                                                Ext.Ajax.request({
                                                                    url: '/service/task/delete_task_state.ashx?params=' + params,
                                                                    params: {
                                                                        stateid: selectNode.data.stateid,
                                                                    },
                                                                    success: function (form, action) {
                                                                        var errorjson = Ext.decode(form.responseText);
                                                                        Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                                        gridloadcurrentPage(datastore);
                                                                    },
                                                                    failure: function (form, action) {
                                                                        var errorjson = Ext.decode(form.responseText);
                                                                        Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                                    }
                                                                });
                                                            }
                                                        });

                                                    }
                                                    else {
                                                        Ext.Ajax.request({
                                                            url: '/service/task/delete_task_state.ashx?params=' + params,
                                                            params: {
                                                                stateid: selectNode.data.stateid,
                                                            },
                                                            success: function (form, action) {
                                                                var errorjson = Ext.decode(form.responseText);
                                                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                                gridloadcurrentPage(datastore);
                                                            },
                                                            failure: function (form, action) {
                                                                var errorjson = Ext.decode(form.responseText);
                                                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                            }
                                                        });
                                                    }
                                                },
                                                failure: function (form, action) {
                                                    var errorjson = Ext.decode(form.responseText);
                                                    Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                }
                                            });
                                        }
                                    }
                                }
                            ]
                        }]
                    }
                ]
            });

            funpanel.add(panel);
            datastore.loadPage(1);
        }
    });

}