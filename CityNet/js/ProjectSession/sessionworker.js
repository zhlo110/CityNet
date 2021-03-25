function session_worker_manager(params)
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
                fields: ['text', 'sessiontype', 'creator', 'createtime', 'description'],
                proxy: {
                    type: 'ajax',
                    url: '/service/projectsession/build_session_tree.ashx?params=' + params,
                    reader: 'json'
                },
                root: {
                    name: '根节点',
                    id: 'project_id'
                },
                sorters: [
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


            var userstore = new Ext.data.TreeStore(
            {
                autoSync: false,
                proxy: {
                    type: 'ajax',
                    url: '/service/projectsession/build_department_user_tree.ashx?params=' + params,
                    extraParams: {
                        sessionid: -1
                    },
                    reader: 'json'
                },
                root: {
                    name: '根节点',
                    id: 'project_id'
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
                id: 'projection_session_workedchild_id',
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
                        region: 'west',
                        width: funpanel.getWidth() / 2,
                        minWidth: 1,
                        split: true,
                        resizable: true,
                        padding: '10 0 10 10',
                        minHeight: 300,
                        border: true,
                        scrollable: true,
                        viewConfig: {
                            preserveScrollOnRefresh: true,
                            preserveScrollOnReload: true,
                            scrollable: 'y'
                        },
                        listeners: {
                            rowclick: function (panel, record, element, rowIndex, e, eOpts) {
                                var recordid = record.data.id;
                                userstore.proxy.extraParams.sessionid = recordid;
                                userstore.load();

                            }
                        },
                        title: '工区目录',
                        store: treestore,
                        dockedItems: [{
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
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
                                        var searchfield = Ext.getCmp('search_session_text');
                                        var value = searchfield.value.trim();
                                        if (value != '') {
                                            treestore.proxy.url = '/service/ProjectSession/search_session.ashx?params=' + params + '&searchtxt=' + value;
                                            treestore.load();
                                        }
                                        else {
                                            treestore.proxy.url = '/service/ProjectSession/build_session_tree.ashx?params=' + params,
                                            treestore.load();
                                        }
                                    }
                                }
                            ]
                        }],
                        rootVisible: false
                   },
                   {
                       xtype: 'checkboxtree',
                       id: 'sel_department_treepanel_id',
                       region: 'center',
                       title: '用户组织树',
                       width: funpanel.getWidth() / 2,
                       minWidth: 1,
                       split: true,
                       resizable: true,
                       padding: '10 25 10 0',
                       minHeight: 300,
                       border: true,
                       scrollable: true,
                       viewConfig: {
                           preserveScrollOnRefresh: true,//
                           preserveScrollOnReload: true,//
                           scrollable: 'y'
                       },
                       store: userstore,
                       rootVisible: false,
                       settaskfunction: function () {
                           var me = this;
                           var taskmsg = me.mask;
                           if (!Ext.isEmpty(taskmsg)) {
                               taskmsg.hide();
                           }
                       },
                       savefunction: function (node, checkstate, uncheck)
                       {
                           var worktree = Ext.getCmp('sel_project_session_treepanel_id_0');
                           var sessionnode = worktree.getSelection()[0];
                           var me = this;
                           if (sessionnode == null) return;
                           var checked = 0;
                           if (uncheck) {
                               checked = 1;//半选
                           }
                           else {
                               if (checkstate) {
                                   checked = 2;
                               }
                               else {
                                   checked = 0;
                               }
                           }
                           var text = node.data.text;
                           var nodeid = node.data.id;
                           me.submitcount = me.submitcount + 1;
                           Ext.Ajax.request({
                               url: '/service/projectsession/save_session_department.ashx?params=' + params,
                               params: {
                                   checked: checked,
                                   departmentid: nodeid,
                                   sessionid: sessionnode.data.id,
                                   userid:node.data.qtip,
                                   isleaf:node.data.leaf
                               },
                               success: function (form, action) {
                                   me.submitcount = me.submitcount - 1;
                                   if (me.submitcount <= 0) {
                                       me.settaskfunction();
                                   }
                               },
                               failure: function (form, action) {
                                   me.submitcount = me.submitcount - 1;
                                   if (me.submitcount <= 0) {
                                       me.settaskfunction();
                                   }
                               }
                           });
                       }
                   }

                ]
            });
            funpanel.add(panel);
        }
    });
}