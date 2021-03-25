function project_secssion_manager(params)
{
        var funpanel = Ext.getCmp('function_area_id');
        funpanel.removeAll();
        var searchlock = false;
        Ext.Ajax.request({
            url: '/service/user/get_userinfomation.ashx?params=' + params,
            success: function (form, action) {
                var json = Ext.decode(form.responseText);
                var treestore = new Ext.data.TreeStore(
                {
                    autoSync: false,
                    id: 'project_session_tree_store',
                    fields: ['text', 'sessiontype','creator','createtime','description'],
                    proxy: {
                        type: 'ajax',
                        url: '/service/projectsession/build_session_tree.ashx?params=' + params,
                        reader: 'json'
                    },
                    root: {
                        name: '根节点',
                        id: 'project_id'
                    },
                    sorters:[
                        {
                            property: 'qtip',
                            direction: 'desc'
                        }
                    ],
                    listeners: {
                        //拖拽功能
                        nodemove: function (node, oldParent, newParent, index, eOpts) {
                            if (oldParent.id == newParent.id) {
                                return;
                            }
                            else {
                                //更改节点父节点
                                Ext.Ajax.request({
                                    url: '/service/projectsession/update_session_parent.ashx?params=' + params,
                                    params: {
                                        sessionid: node.id,
                                        parentid: newParent.id
                                    },
                                    success: function (form, action) {
                       

                                    },
                                    failure: function (form, action) {
                                        var errorjson = Ext.decode(form.responseText);
                                        Ext.MessageBox.alert("提示信息", errorjson.msg);
               
                                    }

                                });
                            }
                            //alert(store);
                        }
                    }
                });


                funpanel.addListener('resize', function (panel, width, height, oldWidth, oldHeight, eOpts) {

                    var childpanel = Ext.getCmp('projection_session_workedchild_id');
                    childpanel.setHeight(height - 35);

                    var panel1 = Ext.getCmp('sel_project_session_treepanel_id_0');
                    panel1.setWidth(width / 2);

                });


                var panel = Ext.create({
                    xtype: 'panel',
                    layout: 'border',
                    id:'projection_session_workedchild_id',
                    minHeight: 300,
                    minWidth: 1,
                    height: funpanel.getHeight() - 35,
                    width: funpanel.getWidth(),
                    border: false,
                    resizable: true,
                    items: [
                        {
                            xtype: 'treepanel',
                            id: 'sel_project_session_treepanel_id_0',
                            region: 'center',
                            width: funpanel.getWidth() / 2,
                            minWidth: 1,
                            split: true,
                            resizable: true,
                            padding: '10 30 10 10',
                            minHeight: 300,
                            border: true,
                            scrollable: true,
                            viewConfig: {
                                preserveScrollOnRefresh: true,//
                                preserveScrollOnReload: true,//
                                scrollable: 'y',
                                plugins: {
                                    ptype: "treeviewdragdrop",
                                    containerScroll: true,
                                    appendOnly: true
                                }
                            },
                            columns: [
                                {
                                    xtype: 'treecolumn',
                                    flex: 1,
                                    dataIndex: 'text',
                                    editor: { xtype: 'textfield', allowBlank: false, emptyText: '名称不能为空', blankText: '名称不能为空' },
                                    text: '工区名称'
                                },
                                {
                                    xtype: 'gridcolumn',
                                    flex: 1,
                                    dataIndex: 'sessiontype',
                                    editor: { xtype: 'textfield' },
                                    text: '工区类型'
                                },
                              //  fields: ['text', 'sessiontype','creator','createtime','description'],
                                {
                                    xtype: 'gridcolumn',
                                    flex: 1,
                                    dataIndex: 'createtime',
                                    text: '创建日期'
                                },
                                {
                                    xtype: 'gridcolumn',
                                    flex: 1,
                                    dataIndex: 'creator',
                                    text: '创建人'
                                },
                                {
                                    xtype: 'gridcolumn',
                                    flex: 1,
                                    dataIndex: 'description',
                                    editor: { xtype: 'textfield'},
                                    text: '描述'
                                }
                            ],
                            title: '工区目录',
                            store: treestore,
                            plugins: [
                                {
                                    ptype: 'bufferedrenderer',
                                    trailingBufferZone: 20,  // Keep 20 rows rendered in the table behind scroll
                                    leadingBufferZone: 50   // Keep 50 rows rendered in the table ahead of scroll
                                },
                                {
                                    id: 'rowediting_plugin_id',
                                    ptype: 'treeediting',
                                    saveBtnText: '保存',
                                    cancelBtnText: "取消",
                                    clicksToMoveEditor: 1,
                                    listeners: {
                                        edit: function (editor, context, eOpts) {
                                            //提交
                                            Ext.Ajax.request({
                                                url: '/service/projectsession/insert_updata_projectsession.ashx?params=' + params,
                                                params: {
                                                    sessionid: context.record.data.id,
                                                    sessionname: context.record.data.text,
                                                    sessiontype: context.record.data.sessiontype,
                                                    description: context.record.data.description
                                                },
                                                success: function (form, action) {
                                                    var errorjson = Ext.decode(form.responseText);
                                                    Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                    treestore.load();

                                                },
                                                failure: function (form, action) {
                                                    //删除
                                                    var errorjson = Ext.decode(form.responseText);
                                                    Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                    var functionpanel = Ext.getCmp('sel_project_session_treepanel_id_0');
                                                    var RootNode = functionpanel.getRootNode();
                                                    var newnode = RootNode.findChild('id', 'new_item_id');
                                                    if (newnode != null) {
                                                        RootNode.removeChild(newnode);
                                                    }
                                                }

                                            });

                                            //  alert(+","+);
                                        },
                                        canceledit: function (editor, context, eOpts) {
                                            var functionpanel = Ext.getCmp('sel_project_session_treepanel_id_0');
                                            var RootNode = functionpanel.getRootNode();
                                            var newnode = RootNode.findChild('id', 'new_item_id');
                                            if (newnode != null) {
                                                RootNode.removeChild(newnode);
                                                return;
                                            }

                                        }
                                    }
                                }],
                            dockedItems: [{
                                xtype: 'toolbar',
                                dock: 'top',
                                items: [
                                    {
                                        xtype: 'button',
                                        iconCls: 'button add',
                                        text: '添加',
                                        handler: function () {
                                            var functionpanel = Ext.getCmp('sel_project_session_treepanel_id_0');
                                            functionpanel.getView().editingPlugin.cancelEdit();
                                            var RootNode = functionpanel.getRootNode();
                                            var newnode = RootNode.findChild('id', 'new_item_id');
                                            if (newnode == null) {
                                                var curDate = new Date();
                                                newnode = RootNode.insertChild(0, {
                                                    id: 'new_item_id', text: '',
                                                    sessiontype:'', expanded: true, createtime: Ext.Date.format(curDate, 'Y年m月d日'), creater: json.realname, description: '', leaf: false
                                                });
                                                functionpanel.getSelectionModel().select(newnode);
                                                functionpanel.getView().editingPlugin.startEdit(newnode);
                                            }
                                        }
                                    },
                                    {
                                        xtype: 'button',
                                        iconCls: 'button delete',
                                        text: '删除',
                                        handler: function () {
                                            var functionpanel = Ext.getCmp('sel_project_session_treepanel_id_0');
                                            functionpanel.getView().editingPlugin.cancelEdit();
                                            var selectNode = functionpanel.selection;
                                            if (selectNode == null) {
                                                Ext.MessageBox.alert("提示信息", '当前选择的工区为空，请选择要删除的工区');
                                            }
                                            else {
                                                //  alert(selectNode);

                                                Ext.Ajax.request({
                                                    url: '/service/projectsession/check_sessionhaschild.ashx?params=' + params,
                                                    params: {
                                                        sessionid: selectNode.id,
                                                    },
                                                    success: function (form, action) {
                                                        var rightjson = Ext.decode(form.responseText);
                                                        if (rightjson.haschildren) {
                                                            Ext.MessageBox.confirm("提示", "该工区下有子工区，是否要删除？", function (btnId) {
                                                                if (btnId == "yes") {
                                                                    Ext.Ajax.request({
                                                                        url: '/service/projectsession/delete_sesssion_byid.ashx?params=' + params,
                                                                        params: {
                                                                            sessionid: selectNode.id,
                                                                        },
                                                                        success: function (form, action) {
                                                                            var errorjson = Ext.decode(form.responseText);
                                                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                                            treestore.load();
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
                                                                url: '/service/projectsession/delete_sesssion_byid.ashx?params=' + params,
                                                                params: {
                                                                    sessionid: selectNode.id,
                                                                },
                                                                success: function (form, action) {
                                                                    var errorjson = Ext.decode(form.responseText);
                                                                    Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                                    treestore.load();
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
                                    },
                                    {
                                        xtype: 'button',
                                        iconCls: 'button moveup',
                                        text: '移动到顶层',
                                        handler: function () {
                                            var functionpanel = Ext.getCmp('sel_project_session_treepanel_id_0');
                                            functionpanel.getView().editingPlugin.cancelEdit();
                                            var node = functionpanel.selection;
                                            if (node != null) {
                                                //更改节点父节点
                                                Ext.Ajax.request({
                                                    url: '/service/projectsession/update_session_parent.ashx?params=' + params,
                                                    params: {
                                                        sessionid: node.id,
                                                        parentid:0
                                                    },
                                                    success: function (form, action) {
                                                        treestore.load();

                                                    },
                                                    failure: function (form, action) {
                                                        //删除
                                                        var errorjson = Ext.decode(form.responseText);
                                                        Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                        treestore.load();
                                                    }

                                                });
                                            }
                                            else {
                                                Ext.MessageBox.alert("提示信息", '当前选择的节点为空，请选择要移动的节点');
                                            }
                                        }
                                    },
                                    {
                                        xtype: 'label',
                                        text: '注意：可以通过拖拽移动工区'
                                    },
                                    '->',
                                    {
                                        xtype: 'textfield',
                                        id: 'search_session_text',
                                        text: '搜索',
                                        emptyText: '请输入标段名称。'
                                    },
                                    {
                                        xtype: 'button',
                                        iconCls: 'button search',
                                        text: '搜索',
                                        handler: function () {
                                            var functionpanel = Ext.getCmp('sel_project_session_treepanel_id_0');
                                            functionpanel.getView().editingPlugin.cancelEdit();
                                            var searchfield = Ext.getCmp('search_session_text');
                                            var value = searchfield.value.trim();
                                            if (value != '') {
                                                treestore.proxy.url = '/service/ProjectSession/search_session.ashx?params=' + params + '&searchtxt=' + value;
                                                treestore.load();
                                            }
                                            else {
                                                treestore.proxy.url = '/service/projectsession/build_session_tree.ashx?params=' + params,
                                                treestore.load();
                                            }
                                        }
                                    }
                                ]
                            }],
                            rootVisible: false
                        }
                    ]
                });
                funpanel.add(panel);
            }
        });
}