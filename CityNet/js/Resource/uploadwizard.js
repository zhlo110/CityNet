var navigate = function (panel, direction) {
    var layout = panel.getLayout();
    layout[direction]();

    if (!Ext.getCmp('move-prev').disabled || !Ext.getCmp('move-next').disabled) {
        Ext.getCmp('move-prev').setDisabled(!layout.getPrev());
        Ext.getCmp('move-next').setDisabled(!layout.getNext());
    }
};
function createuploadwizard(workplace, params, userjson, taskid) {
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
            createuploadwizardImp(workplace, params, userjson, taskjson);
            // editable
        },
        failure: function (form, action) {
            var errorjson = Ext.decode(form.responseText);
            Ext.MessageBox.alert("提示信息", errorjson.msg);
        }
    });
}

function createuploadwizardImp(workplace, params, userjson, taskjson) {

    var panel = Ext.create('Ext.panel.Panel', {
        title: '创建任务导航',
        layout: 'card',
        id:'card_navigation_id',
        taskid: -1,
        defaults: {
            border: false
        },
        bbar: [
            {
                id: 'move-prev',
                text: '上一步',
                iconCls: 'button pref',
                handler: function (btn) {
                    var parent_panel = btn.up("panel");
                    var layout = parent_panel.getLayout();
                    var form = layout.getActiveItem();
                    form.removeAll();
                    navigate(btn.up("panel"), "prev");
                },
                disabled: true
            },
            {
                id: 'move-next',
                text: '下一步',
                iconCls: 'button next',
                handler: function (btn) {
                    var parent_panel = btn.up("panel");
                    var layout = parent_panel.getLayout();
                    var form = layout.getActiveItem();
                    if (form.skip) {
                        
                        if (!Ext.isEmpty(form)) {
                            form.removeAll();
                        }
                        navigate(panel, "next");
                    }
                    else {
                        if (form.isValid()) {
                            form.submit({
                                url: form.submiturl,
                                success: function (resform, action) {
                                    var panel = btn.up("panel");
                                    panel.taskid = action.result.taskid;
                                    if (!Ext.isEmpty(form)) {
                                        form.removeAll();
                                    }
                                    navigate(panel, "next");
                                    
                                },
                                failure: function (form, action) {
                                    Ext.MessageBox.alert("提示信息", action.result.msg);
                                }
                            });
                        }
                        else {
                            Ext.MessageBox.alert("提示信息", '表单验证错误，请检查输入项');
                        }
                    }
                    
                }
            },
            '->', // greedy spacer so that the buttons are aligned to each side
            {
                text: '返回',
                iconCls: 'button return',
                handler: function (btn) {
                    showdocumentpanel(params);
                }
            }
        ],
        // the panels (or "cards") within the layout
        items: [{
            id: 'step-0',
            skip: false,
        //    showfunction: function () { },
            xtype: 'form',
            layout: {
                align: 'middle',
                pack: 'center'
            },
            submiturl: '/service/task/create_update_task.ashx?params=' + params,
            bodyPadding: '40px',
            defaults:
            {
                padding: 5
            },
            listeners:{
                activate : function (panel, eOpts) {
                 //   alert('aa');
                    this.add(
                        {
                            xtype: 'hiddenfield',
                            name: 'taskid',
                            value: taskjson.taskid,
                        },
                        {
                            xtype: 'textfield',
                            anchor: '100%',
                            name: 'task_name',
                            fieldLabel: '任务名 *',
                            msgTarget: 'under',
                            value: taskjson.taskname,
                            allowBlank: false,
                            vtype: 'emptyfield',
                            vtypeText: '任务名不能为空',
                            blankText: '任务名不能为空',
                            emptyText: '任务名不能为空'
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
                        }
                    );
                }
            }
        },
        {
            xtype: 'panel',
            skip: true,
            layout: 'fit',
            scrollable: 'y',
            id: 'step-1',
            listeners:{
                activate: function (panel, eOpts) {
                    var panel = Ext.getCmp('card_navigation_id');
                    var docliststore = createdocumentstore(panel.taskid, params);
                    upload_point(this, params, userjson, docliststore);
                }
            }
        }, 
        {
            id: 'step-2',
            skip: true,
            layout: 'fit',
            listeners: {
                activate: function (panel, eOpts) {
                    uploadpointsign(this, params, userjson,0);
                }
            }

        },
        {
            id: 'step-3',
            layout: 'fit',
            skip: true,
            listeners: {
                activate: function (panel, eOpts) {
                    var panel = Ext.getCmp('card_navigation_id');
                    var me = this;
                    Ext.Ajax.request({
                        url: '/service/task/get_taskstate_bytaskid.ashx?params=' + params,
                        params: {
                            taskid: panel.taskid
                        },
                        success: function (form, action) {
                         
                           
                            var json = Ext.decode(form.responseText);
                            if (json.stateid > 0) {
                                if (json.priority == 1) { //提交状态
                                    point_approve(me, params, userjson);
                                }
                                else if (json.priority == 3)  //返回修改状态
                                {
                                    view_approve(me, params, userjson, 1, false, createsumbitform);
                                }
                            }
                        }
                    });

                    //
                   

                }
            }
        }]
    });
    
    workplace.add(panel);
}
//待上传的文件列表
function createdocumentstore(taskid,params) {
    //
    var datastore = Ext.create('Ext.data.Store', {
        model: 'Document',
        pageSize: 2,
        proxy: {
            type: 'ajax',
            url: '/service/document/getdocumentlistbytid.ashx?params=' + params,
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
    datastore.loadPage(1);
    return datastore;
}
function uploadpointwindows(parentpanel, params, tableid, documentid, schemeid, hasprojection) {

    Ext.Ajax.request({
        url: '/service/document/gettable_l0_h_desbyID.ashx?params=' + params,
        params: {
            tableid: tableid
        },
        success: function (form, action) {
            var successjs = Ext.decode(form.responseText);
            uploadpointwindowsimp(successjs, parentpanel, params, tableid, documentid, schemeid, hasprojection);
           // alert(form.responseText);
        },
        failure: function (form, action) {
            var errorjs = Ext.decode(form.responseText);
            Ext.MessageBox.alert("提示信息", errorjs.msg);
        }
    });

}
function uploadpointwindowsimp(successjs, parentpanel, params, tableid, documentid, schemeid, hasprojection) {

    var panel = Ext.getCmp('uploadpointwindows_id');
    if (!Ext.isEmpty(panel)) return;
    var height = 200;
    if (hasprojection == 1) {
        height = 350;
    }
    else {
        height = 240;
    }
    var windows = Ext.create('Ext.window.Window',
    {
        id: 'uploadpointwindows_id',
        title: '点上传面板',
        height: height,
        width: 400,
        layout: 'fit',
        items: [{
            xtype: 'form',
            id:'upload_form_id',
            width: '100%',
            layout: {
                align: 'middle',
                pack: 'center'
            },
            bodyPadding: '20px',
            defaults:
            {
                padding: 5
            },
            buttonAlign: 'center',
            buttons: [{
                text: '数据上传',
                handler: function () {
                    var form = this.up('form').getForm();
                    var windows = this.up('window');
                    if (form.isValid()) {

                        form.submit({
                            url: '/service/document/upload_point.ashx?params=' + params,
                            waitMsg: '正在上传文件',
                            params: {
                                tableid: tableid,
                                documentid:documentid,
                                schemeid:schemeid
                            },
                            success: function (fp, o) {
                                //显示浏览数据按钮

                                //两个store需要重新load

                                var store = parentpanel.getStore();
                                store.load();
                                var pan = Ext.getCmp('has_in_database_table');
                                pan.getStore().loadPage(pan.getStore().currentPage);
                                var lefttreepane = Ext.getCmp('read_to_upload_tree');
                                lefttreepane.getStore().load();
                               // store.loadPage(store.currentPage);
                                Ext.Msg.alert('提示', o.result.msg);
                                windows.close();
                            }
                        });
                    }
                }
            }]
        }]
    });
    var formpanel = Ext.getCmp('upload_form_id');
    if (hasprojection == 1) {
        formpanel.add(
            [
                {
                    xtype: 'textfield',
                    name: 'L0',
                    fieldLabel: 'L0',
                    msgTarget: 'under',
                    value:successjs.l0,
                    allowBlank: false,
                    blankText: 'L0不能为空',
                    anchor: '100%'
                },
                {
                    xtype: 'textfield',
                    name: 'h',
                    msgTarget: 'under',
                    allowBlank: false,
                    value: successjs.h,
                    blankText: 'h不能为空',
                    fieldLabel: 'h',
                    anchor: '100%'
                },
                {
                    xtype: 'textareafield',
                    name: 'description',
                    value: successjs.description,
                    height: 100,
                    fieldLabel: '描述',
                    anchor: '100%'
                },
                {
                    xtype: 'fieldcontainer',
                    fieldLabel: '重复点处理',
                    name: 'option',
                    defaultType: 'radiofield',
                    defaults: {
                        flex: 1
                    },
                    layout: 'hbox',
                    items: [
                        {
                            boxLabel: '标识为复测',
                            name: 'option',
                            inputValue: 0,
                            checked: true,
                            id: 'radio1'
                        }, {
                            boxLabel: '舍弃',
                            name: 'option',
                            inputValue: 1,
                            id: 'radio2'
                        }
                    ]
                }

            ]);
    }
    else {

        formpanel.add(
           [
               {
                   xtype: 'hiddenfield',
                   name: 'L0',
                   value:''
               },
               {
                   xtype: 'hiddenfield',
                   name: 'h',
                   value: ''
               },
               {
                   xtype: 'textareafield',
                   name: 'description',
                   height: 100,
                   value: successjs.description,
                   fieldLabel: '描述',
                   anchor: '100%'
               },
               {
                   xtype: 'fieldcontainer',
                   fieldLabel: '重复点处理',
                   name: 'option',
                   defaultType: 'radiofield',
                   defaults: {
                       flex: 1
                   },
                   layout: 'hbox',
                   items: [
                       {
                           boxLabel: '标识为复测',
                           name: 'option',
                           inputValue: 0,
                           checked: true,
                           id: 'radio1'
                       }, {
                           boxLabel: '舍弃',
                           name: 'option',
                           inputValue: 1,
                           id: 'radio2'
                       }
                   ]
               }

           ]);

    }

    windows.show();
}
//isresult 是否为成果数据，如果是成果数据需要解析DOC文件
//inserttemptable 是否将上传的文档加入到TempApproveDocument表中 表示插入，0表示不插入

function uploadwindows(gridid, params, taskid, isresult,inserttemptable) {
    var panel = Ext.getCmp('uploadwindows_id');
    if (!Ext.isEmpty(panel)) return;
    Ext.create('Ext.window.Window', 
    {
        id:'uploadwindows_id',
        title: '数据上传面板',
        height: 190,
        width: 400,
        layout: 'fit',
        modal:true,
        closable:false,
        items:[{
            xtype: 'form',
            width:'100%',
            layout: {
                align: 'middle',
                pack: 'center'
            },
            bodyPadding: '20px',
            items:
                [
                    {
                        xtype: 'filefield',
                        name: 'file',
                        fieldLabel: '文档文件',
                        buttonText: '选择文件',
                        anchor: '100%',
                        msgTarget: 'under',
                        allowBlank: false,
                        padding: 5,
                        vtype: 'docordocx',
                        vtypeText: '必须为doc或docx文件',
                        blankText: '任务名不能为空',
                        emptyText: '任务名不能为空'
                    },
                    {
                        xtype: 'textfield',
                        name: 'fileline',
                        hidden: isresult != 1,
                        fieldLabel: '表名的位置',
                        anchor: '100%',
                        msgTarget: 'under',
                        padding: 5,
                        allowBlank: false,
                        value:2,
                        blankText: '请输入表名在表格上方第几行，必须为数字',
                        emptyText: '请输入表名在表格上方第几行，必须为数字'
                    },
                    {
                        xtype: 'backprogress',
                        anchor: '100%',
                        height:'24px',
                        hidden:true,
                        id:'upload_document_progressbar_id',
                        getcurrenturl: '/service/progress/get_progressbar_info.ashx?params=' + params,
                        createurl: '/service/progress/create_progressbar.ashx?params=' + params,
                        getinfourl: '/service/progress/get_progressbar_info.ashx?params=' + params,
                        deleteurl: '/service/progress/delete_progressbar.ashx?params=' + params,
                    }
                ],
  
            buttonAlign: 'center',
            buttons: [{
                text: '数据上传',
                handler: function () {
                    var form = this.up('form').getForm();
                    var windows = this.up('window');
                    var closebutton = Ext.getCmp('close_window_button_id');
                    if (form.isValid()) {
                        this.disabled = true;
                        closebutton.disabled = true;
                        var progressbar = Ext.getCmp('upload_document_progressbar_id');
                        progressbar.createbar('正在上传文档信息', 1000, '', function (progressid){
                            progressbar.start();
                            form.submit({
                                url: '/service/document/upload_document.ashx?params=' + params,
                             //   waitMsg: '正在上传文件',
                                params: {
                                    progressid:progressid,
                                    taskid: taskid,
                                    isresult: isresult,
                                    inserttemptable: inserttemptable
                                },
                                success: function (fp, o) {
                                    //显示浏览数据按钮
                                    //从文档中读取表格并存储
                                    var gridpanel = Ext.getCmp(gridid);
                                    var store = gridpanel.getStore();
                                    store.loadPage(store.currentPage);
                                    progressbar.hide();
                                    Ext.Msg.alert('提示', o.result.msg);
                                    windows.close();
                                },
                                failure: function (form, action) {
                                    progressbar.hide();
                                    Ext.Msg.alert('提示', action.result.msg);
                                    windows.close();
                                }
                            });


                        });

                        
                    }
                }
            },
            {
                text: '关闭窗口',
                id:'close_window_button_id',
                handler: function () {
                    var windows = this.up('window');
                    windows.close();
                }
            }
        
        ]}]
    }).show();
}

function parseDocData(tableid, documentid,params)
{
    //获取解析数据，并显示到面板中
    var showpanel = Ext.getCmp('show_parsetable_id');
    showpanel.removeAll();
    //创建gird
    Ext.Ajax.request({
        url: '/service/document/get_table_columnsnum.ashx?params=' + params,
        params: {
            tableid: tableid
        },
        success: function (form, action) {
            var succesjs = Ext.decode(form.responseText);
            if (succesjs.success == 1) {
                // alert(succesjs.colnums);
                var num = succesjs.colnums;
                Ext.Ajax.request({
                    url: '/service/document/get_table_schemeby_rownum.ashx?params=' + params,
                    params: {
                        columns: num
                    },
                    success: function (form, action) {
                        var i;
                        var columnsjs = Ext.decode(form.responseText);
                        var columns = [];
                        var storemodel = [];
                        if (columnsjs.length == 0) return;
                        var schemid = -1;
                        var hasprojection = 0;
                        for (i = 0; i < num; i++) {
                            var coljs = columnsjs[i];
                            schemid = coljs.schid;
                            hasprojection = coljs.hasprojection;
                            columns.push({
                                text: coljs.name,
                                tooltip: coljs.name,
                                dataIndex: "field_" + i,
                                //cls: 'nocolumnread',
                                align: 'center', minWidth: 1, sortable: false, menuDisabled: true, draggable: false,
                                listeners: {
                                    headerclick: function (ct, column, e, t, eOpts) {
                                    }
                                }
                            });
                            storemodel.push({ name: 'field_' + i, type: 'string' });
                        }

                        var model = Ext.create('Ext.data.Model', {
                            fields: storemodel
                        });

                        var datastore = Ext.create('Ext.data.Store', {
                            model: model,
                            proxy: {
                                type: 'ajax',
                                url: '/service/document/get_table_rows.ashx?params=' + params,
                                extraParams: {
                                    tableid: tableid
                                },
                                reader: {
                                    type: 'json'
                                },
                                autoLoad: true
                            }
                        });
                        datastore.load();
                        var gridpanel = Ext.create('Ext.grid.Panel', {
                            columns: columns,
                            minwidth: 1,
                            store: datastore,
                            tableid: tableid,//删除表格说用
                            documentid: documentid,//入库所用,由DocumentID找taskid
                            schemeid: schemid,//入库方案ID
                            hasprojection:hasprojection,//是否必须要投影信息
                            listeners: {
                                itemmouseenter: function (view, record, item, index, e, eOpts) {
                                    if (view.tip == null) 
                                    {  //这块判断很重要，不能每次都创建个tooltip，要不显示会有问题。
                                        view.tip = Ext.create('Ext.tip.ToolTip', {
                                            // The overall target element.
                                            target: view.el,
                                            // Each grid row causes its own separate show and hide.
                                            delegate: view.itemSelector,
                                            // Moving within the row should not hide the tip.
                                            //  trackMouse: false,
                                            // Render immediately so that tip.body can be referenced prior to the first show.
                                            renderTo: Ext.getBody()

                                        });
                                    };
                                    view.tip.update(record.data.qtip);
                                }
                            },
                            tbar: [
                                {
                                    xtype: 'combo',
                                    emptyText: '选择表格入库方案',
                                    displayField: 'name',
                                    valueField: 'schid',
                                    listeners: {
                                        change: function (combo, newValue, oldValue, eOpts)
                                        {
                                            
                                            var schemeid = newValue;
                                            var me = this;
                                            if (schemeid != null && this.up('gridpanel').schemeid != schemeid) {
                                                Ext.Ajax.request({
                                                    url: '/service/document/get_table_schemeby_id.ashx?params=' + params,
                                                    params: {
                                                        schid: schemeid
                                                    },
                                                    success: function (form, action) {
                                                        var retjson = Ext.decode(form.responseText);
                                                        var gridpanel = me.up('gridpanel');
                                                        var i;
                                                        var columns = gridpanel.getColumns();
                                                        var ncount = retjson.length;
                                                        var hasprojection = 0;
                                                        for (i = 0; i < ncount; i++) {
                                                            var single = retjson[i];
                                                            hasprojection = single.hasprojection;
                                                            columns[i].setText(single.name);
                                                        }
                                                        gridpanel.schemeid = schemeid;
                                                        gridpanel.hasprojection = hasprojection;
                                                    }
                                                });
                                            }
                                        }
                                    },
                                    store: {
                                        fields: ['name', 'schid'],
                                        proxy: {
                                            type: 'ajax',
                                            url: '/service/document/get_table_all_schemeby_rownum.ashx?params=' + params,
                                            extraParams: {
                                                columns: num
                                            },
                                            reader: {
                                                type: 'json'
                                            },
                                            autoLoad: true
                                        }
                                    }
                                },
                                '',
                                {
                                    xtype: 'button',
                                    text: '数据入库',
                                    iconCls: 'button database',
                                    handler: function () {
                                        var parentpanel = this.up('gridpanel');
                                        var tableid = parentpanel.tableid;
                                        var documentid = parentpanel.documentid;
                                        var schemeid = parentpanel.schemeid;
                                        var hasprojection = parentpanel.hasprojection;
                                        if (Ext.isEmpty(tableid) || tableid<=0)
                                        {
                                            Ext.MessageBox.alert("提示信息", '表格中未发现数据');
                                            return;
                                        }
                                        if (Ext.isEmpty(documentid) || documentid <= 0) {
                                            Ext.MessageBox.alert("提示信息", '表格中未关联文档');
                                            return;
                                        }
                                        if (Ext.isEmpty(schemeid) || schemeid <= 0) {
                                            Ext.MessageBox.alert("提示信息", '未发现入库方案，请联系管理员创建入库方案');
                                            return;
                                        }

                                        uploadpointwindows(parentpanel, params, tableid, documentid, schemeid,hasprojection);
                                        //  tableid: tableid,//删除表格说用
                                      //  documentid: documentid,//入库所用,由DocumentID找taskid
                                      //  schemeid: schemid,//入库方案ID
                                       // if()
                                    }
                                }
                            ],
                            forceFit: true
                        });

                        showpanel.add(gridpanel);

                    }
                });
            }
            else {
                Ext.MessageBox.alert("提示信息", succesjs.msg);
            }
            //panel.getStore().load();
        },
        failure: function (form, action) {
            var errorjs = Ext.decode(form.responseText);
            Ext.MessageBox.alert("提示信息", errorjs.msg);
        }
    });

  //  showpanel.add(panel);
}