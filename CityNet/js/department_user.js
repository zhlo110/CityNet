//用户权限管理模块
function department_manager(params) {
    var funpanel = Ext.getCmp('function_area_id');
    funpanel.removeAll();
    var searchlock = false;
    var newtreenode = null;
    var firstexpand = false;
    var scrollYPos = 0;

    Ext.Ajax.request({
        url: '/service/user/get_userinfomation.ashx?params=' + params,
        success: function (form, action) {
            var userjson = Ext.decode(form.responseText);//当前用户信息

            treestore = new Ext.data.TreeStore(
            {
                //    autoSync: false,
                proxy: {
                    type: 'ajax',
                    url: '/service/user/build_department_tree.ashx?showPerson=false',
                    reader: 'json'
                },
                root: {
                    name: '根节点',
                    id: 'project_id'
                },
                listeners: {
                    nodebeforemove: function( node, oldParent, newParent, index, eOpts )
                    {
                        if (node.data.leaf) {
                            return false;
                        }
                        else {
                            return true;
                        }
                    },
                    nodemove: function (node, oldParent, newParent, index, eOpts) {
                    if (oldParent.id == newParent.id) {
                        return;
                    }
                    else {
                        //更改节点父节点
                        Ext.Ajax.request({
                            url: '/service/user/change_departmnet_parent.ashx?params=' + params,
                            params: {
                                departmentid: node.id,
                                parentid: newParent.id
                            },
                            success: function (form, action) {
                            },
                            failure: function (form, action) {
                                //删除
                                var errorjson = Ext.decode(form.responseText);
                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                            }

                        });
                    }
                }
            }

            });

            var datastore = Ext.create('Ext.data.Store', {
                model: 'userInfo',
                pageSize: 15,
                proxy: {
                    type: 'ajax',
                    url: '/service/user/get_wild_user.ashx?params=' + params,
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

                var childpanel = Ext.getCmp('user_dep_workedchild_id');
                childpanel.setHeight(height - 35);

                var panel1 = Ext.getCmp('department_tree_id');
                panel1.setWidth(width / 2);

            });


            var panel = Ext.create({
                xtype: 'panel',
                layout: 'border',
                minHeight: 1,
                minWidth: 1,
                id:'user_dep_workedchild_id',
                height: funpanel.getHeight() - 35,
                width: funpanel.getWidth(),
                border: false,
                resizable: true,
                items: [
                    {
                        xtype: 'treepanel',
                        id: 'department_tree_id',
                        padding: '10 0 10 10',
                        region: 'west',
                        width: funpanel.getWidth() / 2,
                        minWidth: 1,
                        split: true,
                        resizable: true,
                        minHeight: 300,
                        border: true,
                        scrollable: 'y',
                        title: '用户组织树',
                        store: treestore,
                        viewConfig:{
                            preserveScrollOnRefresh:true,//
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
                                text: '名称'
                            },
                            {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'createtime',
                                text: '创建日期'
                            },
                            {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'description',
                                editor: { xtype: 'textfield' },
                                text: '备注'
                            }
                        ],
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
                                filterfuncton: function (record)
                                {
                                    if (record.data.leaf || record.data.id == 'root')
                                        return false;
                                    else
                                        return true;
                                },
                                listeners: {
                                    edit: function (editor, context, eOpts) {
                                        //提交
                                        Ext.Ajax.request({
                                            url: '/service/user/insert_or_update_department.ashx?params=' + params,
                                            params: {
                                                departmentid: context.record.data.id,
                                                departmentname: context.record.data.text,
                                                departmentdescription: context.record.data.description,
                                                parentid: context.record.parentNode.id
                                            },
                                            success: function (form, action) {
                                                var resjson = Ext.decode(form.responseText);
                                                Ext.MessageBox.alert("提示信息", resjson.msg);
                                                if(context.record.parentNode.id == 'root')
                                                {
                                                    treestore.load();
                                                }
                                                else
                                                {
                                                    treestore.load({
                                                        node:context.record.parentNode
                                                    });
                                                }
                                               // treestore.load();
                                                //if (newtreenode != null) {
                                                  //  context.record.data.id = resjson.newid;
                                                    //context.record.id = resjson.newid;
                                                //}
                                                newtreenode = null;
                                            },
                                            failure: function (form, action) {
                                                //删除
                                                var errorjson = Ext.decode(form.responseText);
                                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                var rolepanel = Ext.getCmp('department_tree_id');
                                                var selnode = newtreenode;
                                                if (newtreenode != null) {
                                                    if (selnode.id == 'new_item_id') {
                                                        selnode.parentNode.removeChild(selnode);
                                                    }
                                                }
                                                newtreenode = null;
                                            }

                                        });

                                        //  alert(+","+);
                                    },
                                    canceledit: function (editor, context, eOpts) {
                                        var roletreepanel = Ext.getCmp('department_tree_id');
                                        var selnode = newtreenode;
                                        if (newtreenode != null) {
                                            if (selnode.id == 'new_item_id') {
                                                selnode.parentNode.removeChild(selnode);
                                            }
                                        }
                                        newtreenode = null;

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
                                     text: '新建',
                                     handler: function () {
                                         var rolepanel = Ext.getCmp('department_tree_id');
                                         rolepanel.getView().editingPlugin.cancelEdit();
                                         var parentNode = null;
                                         parentNode = rolepanel.selection;
                                         if (parentNode == null) {
                                             parentNode = rolepanel.getRootNode();
                                             parentNode = parentNode.findChild('id', 'root');
                                         }
                                         if (parentNode.isLeaf()) return;

                                         if (!parentNode.isExpanded()) {
                                             rolepanel.expandNode(parentNode, false, function () {
                                                 var newnode = parentNode.findChild('id', 'new_item_id');
                                                 if (newnode == null) {
                                                     var curDate = new Date();
                                                     newnode = parentNode.insertChild(0, { id: 'new_item_id', text: '', expanded: true, createtime: Ext.Date.format(curDate, 'Y年m月d日'), description: '', leaf: false });
                                                     rolepanel.getSelectionModel().select(newnode);
                                                     rolepanel.getView().editingPlugin.startEdit(newnode);
                                                     newtreenode = newnode;
                                                 }
                                             });
                                         }
                                         else {
                                             var newnode = parentNode.findChild('id', 'new_item_id');
                                             if (newnode == null) {
                                                 var curDate = new Date();
                                                 newnode = parentNode.insertChild(0, { id: 'new_item_id', text: '', expanded: true, createtime: Ext.Date.format(curDate, 'Y年m月d日'), description: '', leaf: false });
                                                 rolepanel.getSelectionModel().select(newnode);
                                                 rolepanel.getView().editingPlugin.startEdit(newnode);
                                                 newtreenode = newnode;
                                             }
                                         }
                                     }
                                 },
                                 {
                                     xtype: 'button',
                                     iconCls: 'button delete',
                                     text: '删除',
                                     handler: function () {
                                         var treepanel = Ext.getCmp('department_tree_id');
                                         var selitem = treepanel.selection;
                                         var n = parseInt(selitem.data.id);
                                         if (!isNaN(n)) {
                                             Ext.Ajax.request({
                                                 url: '/service/user/department_has_child.ashx?params=' + params,
                                                 params: {
                                                     departmentid: selitem.data.id
                                                 },
                                                 success: function (form, action) {

                                                     var resjson = Ext.decode(form.responseText);
                                                     var message = '';
                                                     if (resjson.childrenNum == 0) {
                                                         message = '要删除部门“' + selitem.data.text + '”吗？';
                                                     }
                                                     else {
                                                         message = '部门“' + selitem.data.text + '”存在下属部门或人员，需要删除吗？';
                                                     }
                                                     Ext.MessageBox.confirm("提示", message, function (btnId) {
                                                         if (btnId == "yes") {

                                                             Ext.Ajax.request({
                                                                 url: '/service/user/delete_department_by_id.ashx?params=' + params,
                                                                 params: {
                                                                     departmentid: selitem.data.id
                                                                 },
                                                                 success: function (form, action) {

                                                                     var resjson = Ext.decode(form.responseText);
                                                                     Ext.MessageBox.alert("提示信息", resjson.msg);
                                                                     if (selitem.parentNode.id == 'root') {
                                                                         treestore.load();
                                                                     }
                                                                     else {
                                                                         treestore.load({
                                                                             node: selitem.parentNode
                                                                         });
                                                                     }

                                                                 },
                                                                 failure: function (form, action) {
                                                                     var errorjson = Ext.decode(form.responseText);
                                                                     Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                                 }
                                                             });
                                                         }
                                                     });
                                                 },
                                                 failure: function (form, action) {
                                                     var errorjson = Ext.decode(form.responseText);
                                                     Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                 }

                                             });
                                         }
                                         else {
                                             Ext.MessageBox.alert("提示信息", '不是部门，无法删除');
                                         }

                                        // alert(selitem);
                                     }
                                 },
                                 {
                                     xtype: 'button',
                                     iconCls: 'button refresh',
                                     text: '刷新',
                                     handler: function () {
                                         treestore.load();
                                     }
                                 },
                                { xtype: 'tbspacer', width: 10 },
                                {
                                    xtype: 'textfield',
                                    id: 'search_user_id_text',
                                    text: '搜索',
                                    emptyText: '请输入姓名或组织结构。'
                                },
                                {
                                    xtype: 'button',
                                    iconCls: 'button search',
                                    text: '搜索',
                                    handler: function () {
                                        var searchfield = Ext.getCmp('search_user_id_text');
                                        //alert(searchfield.value);
                                        var treepanel = Ext.getCmp('department_tree_id');
                                        var store = treepanel.getStore();
                                        store.proxy.url = '/service/user/build_department_tree.ashx?showPerson=false&searchtxt=' + searchfield.value;
                                        store.load();
                                    }
                                },
                                {
                                     xtype: 'button',
                                     iconCls: 'button download',
                                     text: '同步部门数据',
                                     handler: function () {
                                         var progressbar = Ext.getCmp('download_department_progressbar_id');
                                         progressbar.createbar('正在下载部门信息', 1000, '', function (taskid) {
                                             if (!Ext.isEmpty(taskid)) {
                                                 progressbar.start();

                                                 Ext.Ajax.request({
                                                     url: '/service/tkyserver/download_department.ashx?params=' + params,
                                                     timeout: 3600000,
                                                     params: {
                                                         taskid: taskid
                                                     },
                                                     success: function (form, action) {
                                                         var errorjson = Ext.decode(form.responseText);
                                                         Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                         treestore.load();
                                                         progressbar.hide();
                                                     },
                                                     failure: function (form, action) {
                                                         var errorjson = Ext.decode(form.responseText);
                                                         Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                         progressbar.hide();
                                                     }
                                                 });
                                             }
                                         });
                                         //  treestore.load();
                                     }
                                 },

                            ]
                        }],
                        listeners: {
                            itemclick: function (panel, record, item, index, e, eOpts) {
                                    var id = record.data.id;
                                    datastore.proxy.url = '/service/user/get_wild_user.ashx?params=' + params + '&departmentid=' + id;
                                    var page = datastore.currentPage;
                                    datastore.loadPage(page);  
                                // alert('aa');
                            }
                        },
                        bbar:{
                            xtype: 'backprogress',
                            hidden:true,
                            id:'download_department_progressbar_id',
                            getcurrenturl: '/service/progress/get_progressbar_info.ashx?params=' + params,
                            createurl: '/service/progress/create_progressbar.ashx?params=' + params,
                            getinfourl: '/service/progress/get_progressbar_info.ashx?params=' + params,
                            deleteurl: '/service/progress/delete_progressbar.ashx?params=' + params,
                            //text: '搜索'
                        },
                        rootVisible: false
                    },
                    {
                        xtype: 'gridpanel',
                        id: "user_gridpanel_id",
                        padding: '10 30 10 0',
                        region: 'center',
                        resizable: true,
                        forceFit: true,
                        title: '用户列表',
                        minHeight: 300,
                        minWidth: 1,
                        store: datastore,
                        columns: [
                            { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'userid' },
                            {
                                header: '选择', xtype: 'checkcolumn', dataIndex: 'usercheck', minWidth: 1,
                                listeners:
                                {
                                    beforecheckchange: function ( item, rowIndex, checked, record, eOpts ) 
                                    {
                                        var treepanel = Ext.getCmp('department_tree_id');
                                        if (treepanel.selection == null) {
                                            Ext.MessageBox.alert("提示信息", '请先选择左侧组织树中的节点');
                                            return false;
                                        }
                                        else if (userjson.id == record.data.userid)
                                        {
                                            Ext.MessageBox.alert("提示信息", '无法更改当前用户的部门');
                                            return false;
                                        }
                                        else {
                                            if (treepanel.selection.data.id!='root') {
                                                var departmentid = treepanel.selection.data.id;
                                                var userid = record.data.userid;

                                                Ext.Ajax.request({
                                                    url: '/service/user/change_department_user_relationship.ashx?params=' + params,
                                                    params: {
                                                        departmentid: departmentid,
                                                        userid: userid,
                                                        checked: checked
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
                                                return true;
                                            }
                                            else {
                                                Ext.MessageBox.alert("提示信息", '请先选择左侧组织树中的节点');
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
                                header: '用户名', minWidth: 1,
                                width: 160, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'username'
                            },
                            {
                                header: '姓名', minWidth: 1,
                                width: 160, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'realname'
                            },
                            { header: '职位', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'position' },
                            {
                                header: '角色', minWidth: 1,
                                sortable: false, menuDisabled: true, draggable: false, dataIndex: 'rolename'
                            }
                        ],
                        dockedItems: [{
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                                { xtype: 'tbspacer', width: 10 },
                                {
                                    xtype: 'label',
                                    text: '搜索'
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'searchbox_id',
                                    text: '搜索',
                                    emptyText: '请输入要搜索的名称',
                                    listeners: {
                                        change: function (textfield, newValue, oldValue, eOpts) {
                                            //alert(newValue);

                                            var gridpanel = Ext.getCmp('user_gridpanel_id');
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
                                },
                                 {
                                     xtype: 'button',
                                     iconCls: 'button user',
                                     text: '同步用户数据',
                                     handler: function () {
                                         var progressbar = Ext.getCmp('download_user_progressbar_id');
                                         progressbar.createbar('正在下载用户信息', 1000, '', function (taskid) {
                                             if (!Ext.isEmpty(taskid)) {
                                                 progressbar.start();

                                                 Ext.Ajax.request({
                                                     url: '/service/tkyserver/download_users.ashx?params=' + params,
                                                     timeout:3600000,
                                                     params: {
                                                         taskid: taskid
                                                     },
                                                     success: function (form, action) {
                                                         var errorjson = Ext.decode(form.responseText);
                                                         Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                         datastore.loadPage(1);
                                                         progressbar.hide();
                                                     },
                                                     failure: function (form, action) {
                                                         var errorjson = Ext.decode(form.responseText);
                                                         Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                         progressbar.hide();
                                                     }
                                                 });
                                             }
                                         });
                                         //  treestore.load();
                                     }
                                 },
                                 {
                                     xtype: 'backprogress',
                                     hidden: true,
                                     width:200,
                                     id: 'download_user_progressbar_id',
                                     getcurrenturl: '/service/progress/get_progressbar_info.ashx?params=' + params,
                                     createurl: '/service/progress/create_progressbar.ashx?params=' + params,
                                     getinfourl: '/service/progress/get_progressbar_info.ashx?params=' + params,
                                     deleteurl: '/service/progress/delete_progressbar.ashx?params=' + params,
                                     //text: '搜索'
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