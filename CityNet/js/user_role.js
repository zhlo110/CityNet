//用户权限管理模块
function user_role_manager(params) {
    var funpanel = Ext.getCmp('function_area_id');
    funpanel.removeAll();
    var searchlock = false;
    Ext.Ajax.request({
        url: '/service/user/get_userinfomation.ashx?params=' + params,
        success: function (form, action) {
            var userjson = Ext.decode(form.responseText);//当前用户信息

            treestore = new Ext.data.TreeStore(
            {
                autoSync: false,
                proxy: {
                    type: 'ajax',
                    url: '/service/user/build_department_tree.ashx?showPerson=true',
                    reader: 'json'
                },
                root: {
                    name: '根节点',
                    id: 'project_id'
                }
            });

            var datastore = Ext.create('Ext.data.Store', {
                model: 'roleInfo',
                pageSize: 15,
                proxy: {
                    type: 'ajax',
                    url: '/service/function/get_allrole.ashx?params=' + params,
                    reader: {
                        type: 'json',
                        rootProperty: 'roots',
                        totalProperty: 'totalCount'
                    },
                    autoLoad: true
                }
            });
            datastore.loadPage(1);

            funpanel.addListener('resize', function (panel, width, height, oldWidth, oldHeight, eOpts) {

                var childpanel = Ext.getCmp('user_roles_workedchild_id');
                childpanel.setHeight(height - 35);

                var panel1 = Ext.getCmp('user_role_tree_id');
                panel1.setWidth(width / 2);

            });


            var panel = Ext.create({
                xtype: 'panel',
                layout: 'border',
                minHeight: 1,
                minWidth: 1,
                height: funpanel.getHeight() - 35,
                width: funpanel.getWidth(),
                resizable: true,
                id:'user_roles_workedchild_id',
                border: false,
                items: [
                    {
                        xtype: 'treepanel',
                        id: 'user_role_tree_id',
                        padding: '15 0 15 0',
                        padding: '10 0 10 10',
                        region: 'west',
                        width: funpanel.getWidth() / 2,
                        minWidth: 1,
                        split: true,
                        resizable: true,
                        minHeight: 300,
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
                        
                        title: '用户组织树',
                        store: treestore,
                        listeners:
                        {
                            //  itemclick: function (panel, record, item, index, e, eOpts) {
                            //      datastore.proxy.url = '/service/function/getfunctionbyclass.ashx?params=' + params + '&groupid=' + record.data.qtip;
                            //      datastore.loadPage(1);
                            //  }
                        },
                        dockedItems: [{
                            xtype: 'toolbar',
                            dock: 'bottom',
                            items: [
                                { xtype: 'tbspacer', width: 10 },
                                {
                                    xtype: 'textfield',
                                    id:'search_user_id_text',
                                    text: '搜索',
                                    emptyText: '请输入姓名或组织结构。'
                                },
                                {
                                    xtype: 'button',
                                    text: '搜索',
                                    handler: function () {
                                        var searchfield = Ext.getCmp('search_user_id_text');
                                        //alert(searchfield.value);
                                        var treepanel = Ext.getCmp('user_role_tree_id');
                                        var store = treepanel.getStore();
                                        store.proxy.url = '/service/user/build_department_tree.ashx?showPerson=true&searchtxt=' + searchfield.value;
                                        store.load();
                                    }
                                }
                            ]
                        }],
                        listeners: {
                            itemclick: function (panel, record, item, index, e, eOpts) {
                                if(record.data.leaf)
                                {
                                    var id = record.data.qtip;
                                    datastore.proxy.url = '/service/function/get_allrole.ashx?params=' + params + '&userid=' + id;
                                    var page = datastore.currentPage;
                                    datastore.loadPage(page);
                                }
                                // alert('aa');
                            }
                        },
                        rootVisible: false
                    },
                    {
                        xtype: 'gridpanel',
                        id: "role_gridpanel_id",
                        padding: '10 30 10 0',
                        region: 'center',
                        resizable: true,
                        title: '角色列表',
                        minHeight: 300,
                        forceFit: true,
                        store: datastore,
                        columns: [
                            { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'roleid' },
                            {
                                header: '选择', xtype: 'checkcolumn', dataIndex: 'rolecheck', minWidth: 1,
                                listeners:
                                {
                                  //  checkchange: function (pan, rowIndex, checked, record, eOpts) {
                                      beforecheckchange: function ( item, rowIndex, checked, record, eOpts ) {
                                        //
                                        var treepanel = Ext.getCmp('user_role_tree_id');
                                        if (treepanel.selection == null) {
                                            Ext.MessageBox.alert("提示信息", '请先选择左侧用户组织树中的用户');
                                            return false;
                                        }
                                        else if (treepanel.selection.data.qtip == userjson.id)
                                        {
                                            Ext.MessageBox.alert("提示信息", '不能更改当前用户的角色');
                                            return false;
                                        }
                                        else {
                                            if (treepanel.selection.data.leaf) {
                                                var userid = treepanel.selection.data.qtip;
                                                var roleid = record.data.roleid;
                                                
                                                Ext.Ajax.request({
                                                    url: '/service/function/delete_or_add_tousergroup.ashx?params=' + params,
                                                    params: {
                                                        roleid: roleid,
                                                        userid: userid,
                                                        ischeck: checked
                                                    },
                                                    success: function (form, action) {
                                                        datastore.loadPage(datastore.currentPage);
                                                    },
                                                    failure: function (form, action) {
                                                        var errorjson = Ext.decode(form.responseText);
                                                        Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                        datastore.loadPage(1);
                                                    }
                                                });


                                            }
                                            else {
                                                Ext.MessageBox.alert("提示信息", '请先选择左侧用户组织树中的用户');
                                                return false;
                                            }
                                        }
                                        // alert(treepanel.selection);
                                        //alert('aa');
                                    }
                                },
                                width: 50, sortable: false, menuDisabled: true, draggable: false
                            },
                            {
                                header: '角色名称（英文）', minWidth: 1,
                                width: 160, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'rolename'
                            },
                            {
                                header: '角色名称（中文）', minWidth: 1,
                                width: 160, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'realname'
                            },
                            { header: '创建人', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'rolecreator' },
                            {
                                header: '描述', minWidth: 1,
                                sortable: false, menuDisabled: true, draggable: false, dataIndex: 'description'
                            }
                        ],
                        dockedItems: [{
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                                { xtype: 'tbspacer', width: 10 },
                                {
                                    xtype: 'label',
                                    text: '搜索',
                                   
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'searchbox_id',
                                    text: '搜索',
                                    emptyText: '请输入要搜索的名称',
                                    listeners: {
                                        change: function (textfield, newValue, oldValue, eOpts) {
                                            //alert(newValue);

                                            var gridpanel = Ext.getCmp('role_gridpanel_id');
                                            var gridstore = gridpanel.getStore();
                                            var filters = gridstore.getFilters();
                                            gridstore.clearFilter();

                                            gridstore.filterBy(function (record, id) {
                                                //newValue
                                                var v = newValue.trim();
                                                if (record.data.realname.indexOf(v) >= 0 || v.length == 0) {
                                                    return true;
                                                }
                                                else {
                                                    return false;
                                                }
                                            });

                                        }
                                    }
                                }
                            ]
                        }],
                        bbar: Ext.create('Ext.PagingToolbar', {
                            store: datastore,
                            displayInfo: true,
                            displayMsg: '显示的条目 {0} - {1} of {2}',
                            emptyMsg: "没有下载项目"
                        })
                    }
                ]
            });
            funpanel.add(panel);
            datastore.loadPage(1);
        }
    });
}