
function need_reform_list(params) 
{
    rectify_reform_list_imp(params, 1, 'get_approve_tasks', '立即处理');
}
function rectify_reform_list(params) {
    rectify_reform_list_imp(params, 0, 'get_all_tasks','查看详请');
}
//整改列表
function rectify_reform_list_imp(params,mode,url,buttontext)
{
    var workplace = Ext.getCmp('index_workplace_id');
    workplace.removeAll();
    Ext.Ajax.request({
        url: '/service/user/get_userinfomation.ashx?params=' + params,
        success: function (form, action) {
            var userjson = Ext.decode(form.responseText);//当前用户信息

            var datastore = Ext.create('Ext.data.Store', {
                model: 'Task',
                pageSize: 15,
                proxy: {
                    type: 'ajax',
                    url: '/service/task/'+url+'.ashx?params=' + params,
                    extraParams: {
                        tasktype: 2,
                        priority: 5
                    },
                    reader: {
                        type: 'json',
                        rootProperty: 'roots',
                        totalProperty: 'totalCount'
                    },
                    autoLoad: true
                }
            });
            datastore.loadPage(1);
            var panel = Ext.create('Ext.grid.Panel', {
                id: 'task_gridview_id',
                store: datastore,
                forceFit: true,

                columns: [
                    { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'taskid' },
                    {
                        header: '整改主题', minWidth: 1,align:'center',renderer:rendercell,
                        sortable: false, menuDisabled: true, draggable: false, dataIndex: 'taskname'
                    },
                    {
                        header: '创建时间', minWidth: 1, align: 'center', renderer: rendercell,
                        sortable: false, menuDisabled: true, draggable: false, dataIndex: 'taskcreatedate'
                    },
                    { header: '提交人', align: 'center', renderer: rendercell, minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'creator' },
                    { header: '提交人单位', align: 'center', renderer: rendercell, minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'creatordep' },
                    {
                        header: '整改负责人', minWidth: 1, align: 'center', renderer: rendercell,
                        sortable: false, menuDisabled: true, draggable: false, dataIndex: 'auditor'
                    },
                    {
                        header: '整改单位', minWidth: 1, align: 'center', renderer: rendercell,
                        sortable: false, menuDisabled: true, draggable: false, dataIndex: 'auditordep'
                    },
                    {
                        header: '当前状态', minWidth: 1, align: 'center', renderer: rendercell,
                        sortable: false, menuDisabled: true, draggable: false, dataIndex: 'state'
                    },
                    {
                        header: '操作栏', minWidth: 1, align: 'center', renderer: rendercell,
                        id:'show_task_detail_columnid',
                        xtype: 'authoritywidgetcolumn',
                        params: Ext.application.params,
                        height:25,
                        //dataIndex:'buttonName',
                        widget: {
                            width: 100,
                            iconCls: 'button detail',
                            xtype: 'button',
                            text: buttontext,
                            handler: function (a, b) {
                                var panel = this.up('widgetgrid');
                                var taskid = a.$widgetRecord.data.taskid;
                                workplace.removeAll();
                                view_rectify_detail(workplace, params, userjson, taskid, mode);

                            }
                        }
                    }
                ],

                bbar: Ext.create('Ext.PagingToolbar', {
                    store: datastore,
                    displayInfo: true,
                    displayMsg: '显示的条目 {0} - {1} of {2}',
                    emptyMsg: "没有下载项目"
                }),
            });
            workplace.add(panel);
            datastore.loadPage(1);
        }
    }
    );
}

//提出整改
function propose_reform_list(params) {
    var workplace = Ext.getCmp('index_workplace_id');
    workplace.removeAll();
    Ext.Ajax.request({
        url: '/service/user/get_userinfomation.ashx?params=' + params,
        success: function (form, action) {
            var userjson = Ext.decode(form.responseText);//当前用户信息


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


            var docstore = Ext.create('Ext.data.Store', {
                model: 'Document',
                pageSize: 4,
                proxy: {
                    type: 'ajax',
                    url: '/service/task/get_approve_doclist.ashx?params='
                        + params + '&approveid=-1&viewmode=0&taskid=-1',
                    reader: {
                        type: 'json',
                        rootProperty: 'roots',
                        totalProperty: 'totalCount'
                    },
                    autoLoad: true
                }
            });
            docstore.loadPage(1);

            workplace.add({
                xtype: 'panel',
                layout: 'border',
                items: [
                    {
                        title: '整改意见',
                        id: 'opinion_panel_id',
                        region: 'north',     // position for region
                        xtype: 'form',
                        height: 100,
                        layout: 'border',
                        minWidth: 1,
                        minHeight: 1,
                        split: true,         // enable resizing
                        collapsible: true,
                        margin: '5 5 0 5',
                        items: [{
                            region: 'north',
                            height: 22,
                            name:'theme',
                            labelAlign: 'right',
                            labelWidth:50,
                            fieldLabel: '主     题',
                            emptyText: '请输入主题',
                            allowBlank: false,
                            blankText:'请输入主题',
                            xtype: 'textfield'

                        }, {
                            region: 'center',
                            name: 'content',
                            id:'content_html_text',
                            xtype: 'htmleditor'
                        }]
                    },
                    {
                        // xtype: 'panel' implied by default
                        title: '整改单位负责人',
                        id: 'responsibility_person_id',
                        region: 'west',
                        xtype: 'panel',
                        margin: '0 0 0 5',
                        width: 200,
                        minWidth: 1,
                        minHeight: 1,
                        split: true,
                        layout: 'fit',
                        items: [
                            {
                                xtype: 'treepanel',
                                id: 'user_auditid_tree_id',
                                minHeight: 1,
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

                        ]
                    }, {
                        title: '抄送人',
                        id: 'copyto_persons_id',
                        region: 'center',
                        minWidth: 1,
                        minHeight: 1,
                        xtype: 'panel',
                        layout: 'fit',
                        margin: '0 5 0 0',
                        items: [

                            {
                                xtype: 'treepanel',
                                id: 'user_copyto_tree_id',
                                minHeight: 1,
                                region: 'center',     // position for region
                                height: 200,
                                margin: '1 1 1 1',
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
                                            id: 'search_copyto_user_id_text',
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
                            }

                        ]
                    },
               
                {
                    region: 'south',
                    title: '附件列表',
                    id: 'approvedoc_list_id',
                    xtype: 'gridpanel',
                    collapsible: true,
                    split: true,
                    minWidth: 1,
                    minHeight: 1,
                    store: docstore,
                    height: 200,
                    forceFit: true,
                    margin: '0 5 5 5',
                    columns: [
                    { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'docid' },
                    {
                        header: '文档名称', minWidth: 1, align: 'left',
                        sortable: false, menuDisabled: true, draggable: false, dataIndex: 'docname'
                    },
                    { header: '提交人', align: 'left', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'creator' },
                    { header: '提交人单位', align: 'center', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'department' },
                    { header: '页数', align: 'left', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'pages' },
                    {
                        header: '查看文档', minWidth: 1, align: 'center',
                        xtype: 'actioncolumn',
                        items: [
                        {
                            iconCls: 'button detail',
                            handler: function (grid, rowIndex, colIndex) {
                                var rec = grid.getStore().getAt(rowIndex);
                                window.open('../Home/Document?params=' + params + '&docid=' + rec.get('docid'));
                                //  alert("Edit " + rec.get('firstname'));
                            }
                        }]
                    },
                    {
                        header: '文档下载', minWidth: 1, align: 'center', dataIndex: 'url',
                        xtype: 'actioncolumn',
                        items: [
                        {
                            iconCls: 'button download',
                            handler: function (grid, rowIndex, colIndex) {
                                var rec = grid.getStore().getAt(rowIndex);
                                window.open(rec.get('url'));
                            }
                        }]
                    }
                    ],

                    tbar: [
                           {
                               text: '附件上传',
                               xtype: 'authoritybutton',
                               params: params,
                               disabled: false,
                               mode: 0,
                               id: 'approve_upload_doc_button',
                               iconCls: 'button add',
                               handler: function () {
                                   var panel = Ext.getCmp('card_navigation_id');
                                   //    alert(panel.taskid);
                                   //不是成果数据，但要插入临时表
                                   docstore.proxy.url = '/service/task/get_approve_doclist.ashx?params='
                                                  + params + '&approveid=-1&viewmode=0&taskid=-1';
                                   uploadwindows('approvedoc_list_id', params, -1, 0, 1);
                               }
                           },
                           {
                               text: '删除附件',
                               xtype: 'authoritybutton',
                               params: params,
                               disabled: false,
                               mode: 0,
                               id: 'approve_delete_doc_button',
                               iconCls: 'button delete',
                               handler: function () {
                                   var gridpanel = this.up('gridpanel');
                                   var seletionnode = gridpanel.getSelection()[0];
                                   if (Ext.isEmpty(seletionnode)) {
                                       Ext.MessageBox.alert("提示信息", '情选择要删除的文档');
                                   }
                                   if (seletionnode.data.result == 1) {
                                       Ext.MessageBox.alert("提示信息", '文档为成果数据，该步骤无法删除');
                                   }
                                   else if (seletionnode != null) {
                                       Ext.MessageBox.confirm("提示", "是否要删除文档'" + seletionnode.data.docname + "'？", function (btnId) {
                                           if (btnId == "yes") {
                                               Ext.Ajax.request({
                                                   url: '/service/document/delete_documentbyid.ashx?params=' + params,
                                                   params: {
                                                       docid: seletionnode.data.docid
                                                   },
                                                   success: function (form, action) {
                                                       var errorjson = Ext.decode(form.responseText);
                                                       Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                       gridloadcurrentPage(gridpanel.getStore());
                                                   },
                                                   failure: function (form, action) {
                                                       var errorjson = Ext.decode(form.responseText);
                                                       Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                   }
                                               });
                                           }
                                       });
                                   }
                               }
                           },

                           {
                               text: '强制删除文件',
                               xtype: 'authoritybutton',
                               params: params,
                               id: 'approve_force_delete_appdoc_button',
                               iconCls: 'button delete',
                               handler: function () {
                                   var gridpanel = this.up('gridpanel');
                                   var seletionnode = gridpanel.getSelection()[0];
                                   if (Ext.isEmpty(seletionnode)) {
                                       Ext.MessageBox.alert("提示信息", '情选择要删除的文档');
                                   }
                                   if (seletionnode.data.result == 1) {
                                       Ext.MessageBox.alert("提示信息", '文档为成果数据，该步骤无法删除');
                                   }
                                   else if (seletionnode != null) {
                                       Ext.MessageBox.confirm("提示", "是否要删除文档'" + seletionnode.data.docname + "'？", function (btnId) {
                                           if (btnId == "yes") {
                                               Ext.Ajax.request({
                                                   url: '/service/document/delete_documentbyid.ashx?params=' + params,
                                                   params: {
                                                       docid: seletionnode.data.docid
                                                   },
                                                   success: function (form, action) {
                                                       var errorjson = Ext.decode(form.responseText);
                                                       Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                       gridloadcurrentPage(gridpanel.getStore());
                                                   },
                                                   failure: function (form, action) {
                                                       var errorjson = Ext.decode(form.responseText);
                                                       Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                   }
                                               });
                                           }
                                       });
                                   }
                               }
                           }

                    ],

                    bbar: Ext.create('Ext.PagingToolbar', {
                        displayInfo: true,
                        store: docstore,
                        displayMsg: '显示的条目 {0} - {1} of {2}',
                        emptyMsg: "没有下载项目"
                    }),
                    minHeight: 20
                }
                ],
                listeners: {
                    resize: function (panel, width, height, oldWidth, oldHeight, eOpts) {
                        var resposibilty = Ext.getCmp('responsibility_person_id');
                        //    var copies = Ext.getCmp('copyto_persons_id');
                        resposibilty.setWidth(width / 2);
                        var opinion = Ext.getCmp('opinion_panel_id');
                        opinion.setHeight(height / 2);

                    }
                },
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '提  交',
                        handler: function () {
                            var form = Ext.getCmp('opinion_panel_id');
                            var thisForm = form.getForm();
                            if (thisForm.isValid()) {
                                var contentvalue = Ext.getCmp('content_html_text').value;
                                var htmlvalue = Ext.String.htmlEncode(contentvalue);

                                var auditpanel = Ext.getCmp('user_auditid_tree_id');
                                var copytopanel = Ext.getCmp('user_copyto_tree_id');

                                var nodes = auditpanel.getChecked();
                                if (nodes.length <= 0) {
                                    Ext.MessageBox.alert("提示信息", "请选择整改单位负责人");
                                }
                                else {
                                    Ext.MessageBox.confirm("提示", "是否要提交任务？", function (btnId) {
                                        if (btnId == "yes") {
                                            var i = 0;
                                            var node = nodes[0];
                                            var auditid = node.data.qtip;//审核人
                                            var copynodes = copytopanel.getChecked();
                                            var copynodesid = [];
                                            for (i = 0; i < copynodes.length; i++) {
                                                node = copynodes[i];
                                                copynodesid.push(node.data.qtip);//抄送人
                                            }

                                            thisForm.submit({
                                                url: '/service/task/submit_supervisor_task.ashx?params=' + params,
                                                params: {
                                                    auditid: auditid,
                                                    description: htmlvalue,
                                                    copynodesid: copynodesid
                                                },
                                                success: function (form, action) {
                                                    var json = Ext.decode(action.response.responseText)
                                                    Ext.MessageBox.alert("提示信息", json.msg);
                                                    rectify_reform_list(params)//跳转到整改列表中
                                                },
                                                failure: function (form, action) {
                                                    var json = Ext.decode(action.response.responseText)
                                                    Ext.MessageBox.alert("提示信息", json.msg);
                                                    rectify_reform_list(params)//跳转到整改列表中
                                                }
                                            });


                                        }
                                    });


                                }





                            }


                        }

                    }

                ]
            });

        }
    });
    
    
}