//查看详细情况和审阅界面
function viewtask_detail(workplace, params, userjson, taskid,mode) {
    workplace.removeAll();
    var signfirst = true;
    var basicfirst = true;
    var pointfirst = true;
    var approvefirst = true;
    workplace.add({
        xtype: 'tabpanel',
        taskid:taskid,
        items: [
        {
            title: '基本信息',
            listeners: {
                activate: function (panel, eOpts) {
                    if (basicfirst) {
                        view_basic_info(this, params, userjson);
                        basicfirst = false;
                    }
                }
            }
        },
        {
            title: '成果数据',
            layout: 'fit',
            listeners: {
                activate: function (panel, eOpts) {
                    if (pointfirst) {
                        view_task_point(this, params, userjson);
                        pointfirst = false;
                    }
                }
            }
        },
        {
            title: '点之记数据',
            layout:'fit',
            listeners: {
                activate: function (panel, eOpts) {
                    if (signfirst) {
                        uploadpointsign(this, params, userjson,1);
                        signfirst = false;
                    }
                }
            }
        },
        {
            title: '审批流程',
            layout: 'fit',
            listeners: {
                activate: function (panel, eOpts) {
                    if (approvefirst) {
                        view_approve(this, params, userjson, mode, true, createsumbitform);
                        approvefirst = false;
                    }
                }
            }
        }]
    });
}


function view_rectify_detail(workplace, params, userjson, taskid, mode) {
    workplace.removeAll();
    var signfirst = true;
    var basicfirst = true;
    var pointfirst = true;
    var approvefirst = true;
    workplace.add({
        xtype: 'tabpanel',
        taskid: taskid,
        items: [
        {
            title: '基本信息',
            listeners: {
                activate: function (panel, eOpts) {
                    if (basicfirst) {
                        view_basic_info2(this, params, userjson);
                        basicfirst = false;
                    }
                }
            }
        },
        {
            title: '整改流程',
            layout: 'fit',
            listeners: {
                activate: function (panel, eOpts) {
                    if (approvefirst) {
                        view_approve(this, params, userjson, mode, true, rectifysumbitform);
                        approvefirst = false;
                    }
                }
            }
        }]
    });
}

function view_basic_info2(parentpanel, params, userjson) {
    parentpanel.removeAll();
    var taskid = parentpanel.up().taskid;
    Ext.Ajax.request({
        url: '/service/task/get_taskbyid.ashx?params=' + params,
        params: {
            taskid: taskid,
            userid: userjson.id,
            departmentid: userjson.departmentID,
            realname: userjson.realname,
            department: userjson.department

        },
        success: function (form, action) {
            var taskjson = Ext.decode(form.responseText);

            parentpanel.add(
            {
                xtype: 'form',
                layout: 'form',
                layout: {
                    align: 'middle',
                    pack: 'center'
                },
                bodyPadding: '40px',
                defaults:
               {
                   padding: 5
               },
                items: [
                {
                    xtype: 'hiddenfield',
                    name: 'taskid',
                    value: taskjson.taskid,
                },
                {
                    xtype: 'textfield',
                    anchor: '100%',
                    name: 'task_name',
                    fieldLabel: '整改主题',
                    value: taskjson.taskname,
                    editable: false
                },
                {
                    xtype: 'textfield',
                    anchor: '100%',
                    editable: false,
                    value: taskjson.submittime,
                    fieldLabel: '创建日期'
                },
                {
                    xtype: 'textfield',
                    anchor: '100%',
                    editable: false,
                    value: taskjson.createname,
                    fieldLabel: '提交人'
                },
                {
                    xtype: 'textfield',
                    anchor: '100%',
                    editable: false,
                    value: taskjson.createdepname,
                    fieldLabel: '提交人单位'
                },
                {
                    xtype: 'textfield',
                    anchor: '100%',
                    editable: false,
                    value: taskjson.statename,
                    fieldLabel: '任务状态'
                }]
            });


        },
        failure: function (form, action) {
            var errorjson = Ext.decode(form.responseText);
            Ext.MessageBox.alert("提示信息", errorjson.msg);
        }
    });
}
function view_task_point(parentpanel, params, userjson) {
    parentpanel.removeAll();
    var taskid = parentpanel.up().taskid;
    var datainstore = Ext.create('Ext.data.Store', {
        model: 'Point',
        pageSize: 15,
        proxy: {
            type: 'ajax',
            url: '/service/document/get_pointlist_byuserid.ashx?params=' + params,
            extraParams: {
                taskid: taskid
            },
            reader: {
                type: 'json',
                rootProperty: 'roots',
                totalProperty: 'totalCount'
            },
            autoLoad: true
        }
    });
    datainstore.loadPage(1);

    parentpanel.add(
     {
            title: '本次上传的数据',
            margin: '5 0 0 0',
            minWidth: 1,
            minHeight: 1,
            height:300,
            xtype: 'gridpanel',
            store: datainstore,
            forceFit: true,
            tbar: [
            '',
            {
                xtype: 'textfield',
                id: 'search_point_text_id',
                emptyText: '请输入要搜索的点名'
            },
            {
                xtype: 'button',
                text: '搜索',
                iconCls: 'button search',
                handler: function () {
                    var textfield = Ext.getCmp('search_point_text_id');
                    var cardpanel = Ext.getCmp('card_navigation_id');
                    var gridpanel = this.up('gridpanel');
                    var store = gridpanel.getStore();
                    store.proxy.url = '/service/document/get_pointlist_byuserid.ashx?params='
                        + params + '&searchtext=' + textfield.value,
                    store.loadPage(1);
                }
            }],
            columns: [
                { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'id' },
                { header: 'TaskID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'taskid' },
                {
                    header: '点名', minWidth: 1, align: 'left',
                    sortable: false, menuDisabled: true, draggable: false, dataIndex: 'pointname'
                },
                {
                    header: '初次上传时间', minWidth: 1, align: 'left',
                    sortable: false, menuDisabled: true, draggable: false, dataIndex: 'firsttime'
                },
                { header: '经度', align: 'left', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'longitude' },
                { header: '纬度', align: 'left', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'latitude' }
            ],
           

            listeners: {
                collapsebody: function (rowNode, record, expandRow, eOpts) {
                    var panel = Ext.getCmp('point_detail_tree_inner_' + record.id);
                    if (!Ext.isEmpty(panel)) {
                        panel.destroy();
                    }
                },
                expandbody: function (rowNode, record, expandRow, eOpts) {
                    var panel = Ext.getCmp('point_detail_tree_inner_' + record.id);
                    if (!Ext.isEmpty(panel)) {
                        panel.destroy();
                    }
                    var treestore = new Ext.data.TreeStore(
                    {
                        autoSync: false,
                        proxy: {
                            type: 'ajax',
                            url: '/service/document/getpoints_tree_by_pointid.ashx?params=' + params,
                            extraParams: {
                                taskid:taskid,
                                pointid: record.id
                            },
                            reader: 'json'
                        },
                        root: {
                            name: '根节点',
                            id: 'project_id'
                        }
                    });

                    var treepanel = Ext.create({
                        xtype: 'treepanel',
                        store: treestore,
                        height: 100,
                        id: 'point_detail_tree_inner_' + record.id,
                        scrollable: true,
                        forceFit: true,
                        plugins: [{
                            ptype: 'bufferedrenderer',
                            trailingBufferZone: 20,  // Keep 20 rows rendered in the table behind scroll
                            leadingBufferZone: 50   // Keep 50 rows rendered in the table ahead of scroll
                        }],
                        columns: [
                            { xtype: 'treecolumn',  flex: 1, dataIndex: 'text', text: '方案名称' },
                            { xtype: 'gridcolumn', hidden: true, dataIndex: 'measureid' },
                            { xtype: 'gridcolumn', dataIndex: 'x', text: 'x坐标' },
                            { xtype: 'gridcolumn', dataIndex: 'y', text: 'y坐标' },
                            { xtype: 'gridcolumn', dataIndex: 'z', text: 'z坐标' },

                            { xtype: 'gridcolumn', dataIndex: 'endpoint', text: '终点' },
                            { xtype: 'gridcolumn', dataIndex: 'parallel', text: '水平面改正数' },
                            { xtype: 'gridcolumn', dataIndex: 'gravity', text: '重力改正数' },

                            { xtype: 'gridcolumn', dataIndex: 'measuretime', text: '测量时间' },
                            { xtype: 'gridcolumn', dataIndex: 'description', text: '备注' },
                            { xtype: 'gridcolumn', dataIndex: 'sharedes', text: '共桩情况' },
                            { xtype: 'gridcolumn', dataIndex: 'pntdescription', text: '注意事项' }
                        ],
                        rootVisible: false,
                        renderTo: 'point_detail_div_' + record.id
                    });


                    //record.id  Point点ID 

                    //alert(rowNode);
                }
            },

            plugins: [
                {
                    ptype: 'bufferedrenderer',
                    trailingBufferZone: 20,  // Keep 20 rows rendered in the table behind scroll
                    leadingBufferZone: 50   // Keep 50 rows rendered in the table ahead of scroll
                },
                {
                    ptype: 'rowexpander',
                    haveRowExpander: function (record) {
                        return true;
                    },
                    rowBodyTpl: ['<div id="point_detail_div_{id}"></div>']

                }],
            bbar: Ext.create('Ext.PagingToolbar', {
                store: datainstore,
                displayInfo: true,
                displayMsg: '显示的条目 {0} - {1} of {2}',
                emptyMsg: "没有下载项目"
            }),
            colspan: 2
        });
}
function view_basic_info(parentpanel, params, userjson) {
    parentpanel.removeAll();
    var taskid = parentpanel.up().taskid;
    Ext.Ajax.request({
        url: '/service/task/get_taskbyid.ashx?params=' + params,
        params: {
            taskid: taskid,
            userid: userjson.id,
            departmentid: userjson.departmentID,
            realname: userjson.realname,
            department: userjson.department

        },
        success: function (form, action) {
            var taskjson = Ext.decode(form.responseText);

            parentpanel.add(
            {
                xtype:'form',
                layout: 'form',
                layout: {
                    align: 'middle',
                    pack: 'center'
                },
                bodyPadding: '40px',
                defaults:
               {
                   padding: 5
               },
                items:[
                {
                    xtype: 'hiddenfield',
                    name: 'taskid',
                    value: taskjson.taskid,
                },
                {
                    xtype: 'textfield',
                    anchor: '100%',
                    name: 'task_name',
                    fieldLabel: '任务名',
                    value: taskjson.taskname,
                    editable: false
                },
                {
                    xtype: 'textfield',
                    anchor: '100%',
                    editable: false,
                    value: taskjson.submittime,
                    fieldLabel: '创建日期'
                },
                {
                    xtype: 'textfield',
                    anchor: '100%',
                    editable: false,
                    value: taskjson.createname,
                    fieldLabel: '创建人'
                },
                {
                    xtype: 'textfield',
                    anchor: '100%',
                    editable: false,
                    value: taskjson.createdepname,
                    fieldLabel: '创建人单位'
                },
                {
                    xtype: 'textfield',
                    anchor: '100%',
                    editable: false,
                    value: taskjson.statename,
                    fieldLabel: '任务状态'
                }]
                });


        },
        failure: function (form, action) {
            var errorjson = Ext.decode(form.responseText);
            Ext.MessageBox.alert("提示信息", errorjson.msg);
        }
    });
}
function view_approve(parentpanel, params, userjson,mode,enablebutton,submitform) {
    parentpanel.removeAll();
    var taskid = parentpanel.up().taskid;
    if (mode == 0) mode = 1;
    else mode = 0;


    var approvestore = Ext.create('Ext.data.Store', {
        model: 'Approve',
        proxy: {
            type: 'ajax',
            url: '/service/task/get_approvelist_bytaskid.ashx?params=' + params,
            extraParams: {
                taskid: taskid
            },
            reader: {
                type: 'json'
            },
            autoLoad: true
        }
    });
    approvestore.load();

    var docstore = Ext.create('Ext.data.Store', {
        model: 'Document',
        pageSize: 4,
        proxy: {
            type: 'ajax',
            url: '/service/task/get_approve_doclist.ashx?params='
                + params + '&approveid=-1&viewmode=' + mode + '&taskid=' + taskid,
            reader: {
                type: 'json',
                rootProperty: 'roots',
                totalProperty: 'totalCount'
            },
            autoLoad: true
        }
    });
    docstore.loadPage(1);

   


    parentpanel.add({
        xtype: 'panel',
        layout: 'border',
        width: 500,
        height: 300,
        items: [{
            // xtype: 'panel' implied by default
            title: '提交记录',
            region: 'west',
            xtype: 'gridpanel',
            forceFit: true,
            margin: '5 0 0 5',
            width: 260,
            store: approvestore,
            minHeight: 1,
            minWidth: 1,
            split: true,
            collapsible: true,   // make collapsible
            layout: 'fit',
            columns: [
            { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'approveid' },
            { header: '提交时间', align: 'left', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'date' },
            { header: '提交人', align: 'left', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'creator' },
            { header: '处理人', align: 'left', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'auditor' }],
            listeners: {
                rowclick: function (grid, record, element, rowIndex, e, eOpts) {
                  
                    Ext.getCmp('approve_upload_doc_button').setDisabled(true);
                    Ext.getCmp('approve_delete_doc_button').setDisabled(true);
                    Ext.Ajax.request({
                        url: '/service/task/get_approve_comment.ashx?params=' + params,
                        params: {
                            approveid: record.data.approveid
                        },
                        success: function (response) {
                            var htmlpanel = Ext.getCmp('comment_detail_id');
                            htmlpanel.removeAll();
                            if (!Ext.isEmpty(htmlpanel)) {
                                if (!Ext.isEmpty(htmlpanel.body)) {
                                    htmlpanel.body.update(Ext.String.htmlDecode(response.responseText));
                                 //   htmlpanel.body.update('<font color="#ff0000" style="" size="4">​<b style="">审核意见：</b></font></br>' + response.responseText);
                                }
                            }
                        }
                    });
                    docstore.proxy.url = '/service/task/get_approve_doclist.ashx?params=' + params
                        + '&approveid=' + record.data.approveid + '&taskid=' + taskid + '&viewmode=1';
                    docstore.loadPage(1);
                }
            }
        },
        {
            title: '详请',
            region: 'center',     // center region is required, no width/height specified
            xtype: 'panel',
            layout: 'fit',
            id:'comment_detail_id',
            minHeight: 1,
            scrollable:'y',
            minWidth: 1,
            tbar: [{
                xtype: 'authoritybutton',
                mode: mode,
                params:params,
                id: 'authority_page_button',
                text: '转到提交页面',
                handler: function () {
                    docstore.proxy.url = '/service/task/get_approve_doclist.ashx?params='
                                           + params + '&approveid=-1&viewmode=' + mode + '&taskid=' + taskid;
                    docstore.loadPage(1);
                    submitform('comment_detail_id', taskid, params, enablebutton, userjson,submitform);
                }
            }],
            margin: '5 5 0 0'
        },
        {
            region: 'south',
            title: '附件列表',
            id: 'approvedoc_list_id',
            xtype: 'gridpanel',
            collapsible: true,
            split: true,
            store: docstore,
            height: 200,
            forceFit: true,
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
                       mode:mode,
                       id:'approve_upload_doc_button',
                       iconCls: 'button add',
                       handler: function () {
                           var panel = Ext.getCmp('card_navigation_id');
                           //    alert(panel.taskid);
                           //不是成果数据，但要插入临时表
                           docstore.proxy.url = '/service/task/get_approve_doclist.ashx?params='
                                          + params + '&approveid=-1&viewmode=' + mode + '&taskid=' + taskid;
                           uploadwindows('approvedoc_list_id', params, taskid, 0, 1);
                       }
                   },
                   {
                       text: '删除附件',
                       xtype: 'authoritybutton',
                       params: params,
                       disabled: false,
                       mode: mode,
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
        ]
    });
    if (mode != 1) {
        submitform('comment_detail_id', taskid, params, enablebutton, userjson);
    }
}

function createsumbitform(panelid, taskid, params, enablebutton, userjson) {
    var htmlpanel = Ext.getCmp(panelid);
    htmlpanel.removeAll();
    if (!Ext.isEmpty(htmlpanel)) {
        if (!Ext.isEmpty(htmlpanel.body)) {
            htmlpanel.body.update('');
        }
    }
    
    Ext.getCmp('approve_upload_doc_button').setDisabled(!enablebutton);
    Ext.getCmp('approve_delete_doc_button').setDisabled(!enablebutton);
    
    htmlpanel.add({
        xtype: 'form',
        layout: 'fit',
        id: 'submit_approve_form_id',
        bodyPadding: 5,
        buttonAlign: 'center',
        items: [{
            //                        name: 'description',
            id: 'html_approve_id',
            xtype: 'htmleditor'
        }],
        buttons: [{
            text: '提交下一个审核人',//从状态3提交还是从状态2提交
            xtype: 'authoritybutton',
            params: params,
            id: 'next_appove_button',
            handler: function () {
                var me = this;
                var htmleditor = Ext.getCmp('html_approve_id');
                var htmlvalue = Ext.String.htmlEncode(htmleditor.value);
                Ext.Ajax.request({
                    url: '/service/task/get_taskstate_bytaskid.ashx?params=' + params,
                    params: {
                        taskid: taskid
                    },
                    success: function (form, action) {
                        var json = Ext.decode(form.responseText);
                        if (json.stateid > 0) {
                            if (json.priority == 2) { //提交状态,审核状态下提交，提交给下一个审核人
                                approvefrom2(taskid, htmlvalue, params, userjson)
                            }
                            else if (json.priority == 3)  //返回修改状态
                            {

                                Ext.MessageBox.confirm("提示", "是否要提交修改？", function (btnId) {
                                    if (btnId == "yes") {
                                        var formpanel = me.up('form');
                                        var thisForm = formpanel.getForm();
                                        if (thisForm.isValid()) {
                                            thisForm.submit({
                                                url: '/service/task/submit_approve_3.ashx?params=' + params,
                                                params: {
                                                    taskid: taskid,
                                                    description: htmlvalue
                                                },
                                                success: function (form, action) {
                                                    var json = Ext.decode(action.response.responseText)
                                                    Ext.MessageBox.alert("提示信息", json.msg);
                                                    showdocumentpanel(params);


                                                  //  approvepanel(params);
                                                },
                                                failure: function (form, action) {
                                                    var json = Ext.decode(action.response.responseText)
                                                    Ext.MessageBox.alert("提示信息", json.msg);
                                                    showdocumentpanel(params);
                                                 //   approvepanel(params);
                                                }

                                            });

                                        }
                                    }
                                });

                               // 
                               // alert(3);
                            }
                        }
                    }
                });

            }
        },
        {
            text: '退回修改',
            xtype: 'authoritybutton',
            params: params,
            id: 'return_appove_button',
            disabled:!enablebutton,
            handler: function () {
                var me = this;
                var htmleditor = Ext.getCmp('html_approve_id');
                var htmlvalue = Ext.String.htmlEncode(htmleditor.value);

                Ext.MessageBox.confirm("提示", "是否要提退回修改？", function (btnId) {
                    if (btnId == "yes") {
                        var formpanel = me.up('form');
                        var thisForm = formpanel.getForm();
                        if (thisForm.isValid()) {
                            thisForm.submit({
                                url: '/service/task/return_modify_task.ashx?params=' + params,
                                params: {
                                    taskid: taskid,
                                    auditid: -1,
                                    description: htmlvalue
                                },
                                success: function (form, action) {
                                    var json = Ext.decode(action.response.responseText)
                                    Ext.MessageBox.alert("提示信息", json.msg);
                                    approvepanel(params);
                                },
                                failure: function (form, action) {
                                    var json = Ext.decode(action.response.responseText)
                                    Ext.MessageBox.alert("提示信息", json.msg);
                                    approvepanel(params);
                                }

                            });

                        }
                    }
                });
            }
        },
        {
            text: '审核完成',
            disabled: !enablebutton,
            xtype: 'authoritybutton',
            params: params,
            id: 'finshed_appove_button',
            handler: function () {
                var me = this;
                var htmleditor = Ext.getCmp('html_approve_id');
                var htmlvalue = Ext.String.htmlEncode(htmleditor.value);

                Ext.MessageBox.confirm("提示", "是否要完成审核？", function (btnId) {
                    if (btnId == "yes") {
                        var formpanel = me.up('form');
                        var thisForm = formpanel.getForm();
                        if (thisForm.isValid()) {
                            thisForm.submit({
                                url: '/service/task/return_modify_task.ashx?params=' + params,
                                params: {
                                    taskid: taskid,
                                    auditid: userjson.id,
                                    description: htmlvalue
                                },
                                success: function (form, action) {
                                 //   var json = Ext.decode(action.response.responseText)
                                 //   Ext.MessageBox.alert("提示信息", json.msg);
                                 //   approvepanel(params);

                                    var myMask = new Ext.LoadMask({
                                        target: htmlpanel,
                                        msg: '正在完成审核，请稍后！'
                                    });
                                    myMask.show();
                                    Ext.Ajax.request({
                                        url: '/service/alarm/refresh_all_alarm.ashx?params=' + params,
                                        success: function (form, action) {
                                            myMask.hide();
                                            var errorjson = Ext.decode(form.responseText);
                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                            approvepanel(params);

                                        },
                                        failure: function (form, action) {
                                            myMask.hide();
                                            var errorjson = Ext.decode(form.responseText);
                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                            approvepanel(params);
                                           
                                        }
                                    });

                                },
                                failure: function (form, action) {
                                    var json = Ext.decode(action.response.responseText)
                                    Ext.MessageBox.alert("提示信息", json.msg);
                                    approvepanel(params);
                                }

                            });

                        }
                    }
                });
            }




        }]
    });
}

function rectifysumbitform(panelid, taskid, params, enablebutton, userjson) {
    var htmlpanel = Ext.getCmp(panelid);
    htmlpanel.removeAll();
    if (!Ext.isEmpty(htmlpanel)) {
        if (!Ext.isEmpty(htmlpanel.body)) {
            htmlpanel.body.update('');
        }
    }
    Ext.getCmp('approve_upload_doc_button').setDisabled(!enablebutton);
    Ext.getCmp('approve_delete_doc_button').setDisabled(!enablebutton);
    //得到Task的createid

    Ext.Ajax.request({
        url: '/service/task/get_taskbyid.ashx?params=' + params,
        params: {
            taskid: taskid
        },
        success: function (form, action) {
            var taskjson = Ext.decode(form.responseText);
            var mode = 0;
            //当前用户就是该Task的创建人
            var buttontext = '提交整改';
            if (taskjson.createid == userjson.id) {
                mode = 0;
                buttontext = '继续整改';
            }
            else {
                mode = 1;
                buttontext = '提交整改';
            }


            htmlpanel.add({
                xtype: 'form',
                layout: 'fit',
                id: 'submit_approve_form_id',
                bodyPadding: 5,
                buttonAlign: 'center',
                items: [{
                    //                        name: 'description',
                    id: 'html_approve_id',
                    xtype: 'htmleditor'
                }],
                buttons: [
                {
                    text: buttontext,
                    xtype: 'authoritybutton',
                    params: params,
                    id: 'return_appove_button',
                    handler: function () {
                        var me = this;
                        var htmleditor = Ext.getCmp('html_approve_id');
                        var htmlvalue = Ext.String.htmlEncode(htmleditor.value);
                        Ext.MessageBox.confirm("提示", "是否要提交整改？", function (btnId) {
                            if (btnId == "yes") {
                                var formpanel = me.up('form');
                                var thisForm = formpanel.getForm();
                                if (thisForm.isValid()) {
                                    thisForm.submit({
                                        url: '/service/task/return_modify_task.ashx?params=' + params,
                                        params: {
                                            taskid: taskid,
                                            auditid: -1,
                                            description: htmlvalue
                                        },
                                        success: function (form, action) {
                                            var json = Ext.decode(action.response.responseText)
                                            Ext.MessageBox.alert("提示信息", json.msg);
                                            need_reform_list(params);
                                        },
                                        failure: function (form, action) {
                                            var json = Ext.decode(action.response.responseText)
                                            Ext.MessageBox.alert("提示信息", json.msg);
                                            need_reform_list(params);
                                        }

                                    });

                                }
                            }
                        });
                    }
                },
                {
                    text: '整改完成',
                    mode:mode,
                    xtype: 'authoritybutton',
                    params: params,
                    id: 'finshed_appove_button',
                    handler: function () {
                        var me = this;
                        var htmleditor = Ext.getCmp('html_approve_id');
                        var htmlvalue = Ext.String.htmlEncode(htmleditor.value);

                        Ext.MessageBox.confirm("提示", "是否要完成审核？", function (btnId) {
                            if (btnId == "yes") {
                                var formpanel = me.up('form');
                                var thisForm = formpanel.getForm();
                                if (thisForm.isValid()) {
                                    thisForm.submit({
                                        url: '/service/task/return_modify_task.ashx?params=' + params,
                                        params: {
                                            taskid: taskid,
                                            auditid: userjson.id,
                                            description: htmlvalue
                                        },
                                        success: function (form, action) {
                                            var json = Ext.decode(action.response.responseText)
                                            Ext.MessageBox.alert("提示信息", json.msg);
                                            need_reform_list(params);
                                        },
                                        failure: function (form, action) {
                                            var json = Ext.decode(action.response.responseText)
                                            Ext.MessageBox.alert("提示信息", json.msg);
                                            need_reform_list(params);
                                        }

                                    });

                                }
                            }
                        });
                    }




                }]
            });



        }
    })
    
}