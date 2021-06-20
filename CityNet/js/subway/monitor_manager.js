function monitor_manager(params) {
    var funpanel = Ext.getCmp('index_workplace_id');
    funpanel.removeAll();
    funpanel.clearListeners();
    Ext.Ajax.request({
        url: '/service/user/get_userinfomation.ashx?params=' + params,
        success: function (form, action) {
            var userjson = Ext.decode(form.responseText);//当前用户信息
            var combostore = Ext.create('Ext.data.Store', {
            fields: ['taskid', 'taskname'],
            model: 'TaskSimpe',
            proxy: {
                type: 'ajax',
                async: false,
                url: '/service/basepoint/getusertask.ashx?params=' + params,
                reader: {
                    type: 'json',
                    rootProperty: 'data',
                    totalProperty: 'totalCount'
                },
                autoLoad: true,
            }
            });
            combostore.load({
                callback: function (records, operation, success) {
                    if (Ext.isEmpty(records[0].data.taskid)) {
                        Ext.MessageBox.alert("提示信息", "没有项目，无法创建监测项");
                        return;
                    }
                    var schemenamestore = Ext.create('Ext.data.Store', {
                        model: 'TableSchemeName',
                        pageSize: 15,
                        proxy: {
                            type: 'ajax',
                            url: '/service/document/get_alltablescheme.ashx?params=' + params,
                            extraParams: {
                                taskid: records[0].data.taskid
                            },
                            reader: {
                                type: 'json',
                                rootProperty: 'roots',
                                totalProperty: 'totalCount'
                            },
                            autoLoad: true
                        }
                    });
                    schemenamestore.loadPage(1);
                    var rowdatastore = Ext.create('Ext.data.Store', {
                        model: 'TableSchemeRow',
                        pageSize: 15,
                        proxy: {
                            type: 'ajax',
                            url: '/service/document/get_allrowby_schid.ashx?params=' + params,
                            extraParams: {
                                schid: -1
                            },
                            reader: {
                                type: 'json',
                                rootProperty: 'roots',
                                totalProperty: 'totalCount'
                            },
                            autoLoad: true
                        }
                    });
                    rowdatastore.loadPage(1);
                    var databasecol = Ext.create('Ext.data.Store', {
                        fields: ['name', 'type', 'description'],
                        proxy: {
                            type: 'ajax',
                            url: '/service/document/get_measurePoint_col.ashx?params=' + params,
                            reader: {
                                type: 'json'
                            },
                            autoLoad: true
                        }
                    });
                    databasecol.load();
                    funpanel.addListener('resize', function (panel, width, height, oldWidth, oldHeight, eOpts) {
                        var childpanel = Ext.getCmp('workedchild_id');
                        childpanel.setHeight(height - 35);
                        var panel1 = Ext.getCmp('table_scheme_id');
                        panel1.setWidth(width / 2);

                    });
                    var panel = Ext.create({
                        xtype: 'panel',
                        layout: 'border',
                        id: 'workedchild_id',
                        minHeight: 1,
                        minWidth: 1,
                        height: funpanel.getHeight() - 35,
                        width: funpanel.getWidth(),
                        border: false,
                        resizable: true,
                        items: [
                            {
                                xtype: 'gridpanel',
                                id: "table_scheme_id",
                                padding: '10 0 10 10',
                                region: 'west',
                                width: funpanel.getWidth() / 2,
                                minWidth: 1,
                                split: true,
                                resizable: true,
                                title: '监测项名称列表',
                                minHeight: 300,
                                forceFit: true,
                                viewConfig: {
                                    getRowClass: function (record, rowIndex, rowParams, store) {
                                        if (record.data.valid == 0)
                                            return "x-grid-red";
                                    }
                                },
                                store: schemenamestore,
                                columns: [
                                    { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'schid' },
                                    { header: 'valid', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'valid' },
                                    {
                                        header: '推荐监测项', xtype: 'checkcolumn', dataIndex: 'priority', minWidth: 1,
                                        listeners:
                                        {
                                            beforecheckchange: function (item, rowIndex, checked, record, eOpts) {

                                                Ext.Ajax.request({
                                                    url: '/service/document/change_scheme_priority.ashx?params=' + params,
                                                    params: {
                                                        schid: record.data.schid,
                                                        editable: checked
                                                    },
                                                    success: function (form, action) {
                                                        schemenamestore.loadPage(schemenamestore.currentPage);
                                                    },
                                                    failure: function (form, action) {
                                                        var errorjson = Ext.decode(form.responseText);
                                                        Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                        schemenamestore.loadPage(schemenamestore.currentPage);
                                                    }
                                                });
                                            }
                                        },
                                        width: 100, sortable: false, menuDisabled: true, draggable: false
                                    },
                                    {
                                        header: '监测项名称', minWidth: 1,
                                        editor: { xtype: 'textfield', allowBlank: false, emptyText: '监测项名称不能为空', blankText: '监测项名称不能为空' },
                                        width: 160, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'schname'
                                    },
                                    {
                                        header: '标识颜色',
                                        renderer: function (value, m) {
                                            m.style = 'background:' + value + ';';
                                            return value;
                                        },
                                        editor: { xtype: 'colorfield', editable: false },
                                        minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'color'
                                    },
                                    { header: '创建人', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'creator' },
                                    { header: '创建日期', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'createdata' },
                                    {
                                        header: '添加投影', xtype: 'checkcolumn', dataIndex: 'hasprojection', minWidth: 1,
                                        listeners:
                                        {
                                            beforecheckchange: function (item, rowIndex, checked, record, eOpts) {

                                                Ext.Ajax.request({
                                                    url: '/service/document/change_scheme_project.ashx?params=' + params,
                                                    params: {
                                                        schid: record.data.schid,
                                                        editable: checked
                                                    },
                                                    success: function (form, action) {
                                                        schemenamestore.loadPage(schemenamestore.currentPage);
                                                    },
                                                    failure: function (form, action) {
                                                        var errorjson = Ext.decode(form.responseText);
                                                        Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                        schemenamestore.loadPage(schemenamestore.currentPage);
                                                    }
                                                });
                                            }
                                        },
                                        width: 100, sortable: false, menuDisabled: true, draggable: false
                                    },
                                    {
                                        header: '描述', minWidth: 1,
                                        editor: { xtype: 'textfield' },
                                        sortable: false, menuDisabled: true, draggable: false, dataIndex: 'description'
                                    }
                                ],
                                plugins: {
                                    ptype: 'treeediting',
                                    clicksToEdit: 2,
                                    pluginId: 'table_scheme_editor_plugin_id',
                                    saveBtnText: '保存',
                                    cancelBtnText: "取消",
                                    listeners: {
                                        edit: function (editor, context, eOpts) {
                                            var projectcombox = Ext.getCmp('montor_project_combo_id');
                                            Ext.Ajax.request({
                                                url: '/service/document/add_update_tablescheme.ashx?params=' + params,
                                                params: {
                                                    schid: context.record.data.schid,
                                                    schname: context.record.data.schname,
                                                    color: context.record.data.color,
                                                    taskid:projectcombox.getValue(),
                                                    description: context.record.data.description
                                                },
                                                success: function (form, action) {
                                                    var json = Ext.decode(form.responseText);
                                                    Ext.MessageBox.alert("提示信息", json.msg);
                                                    schemenamestore.loadPage(1);
                                                },
                                                failure: function (form, action) {
                                                    var errorjson = Ext.decode(form.responseText);
                                                    Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                    schemenamestore.loadPage(1);
                                                }
                                            });
                                            //      context.record.data.functionname,context.record.data.description,
                                        },
                                        canceledit: function (editor, context, eOpts) {

                                            var gridpanel = Ext.getCmp('table_scheme_id');
                                            var gridstore = gridpanel.getStore();
                                            var index = gridstore.find('schid', -1);
                                            if (index >= 0) {
                                                gridstore.removeAt(index);
                                                return;
                                            }
                                        }
                                    },
                                },
                                dockedItems: [{
                                    xtype: 'toolbar',
                                    dock: 'top',
                                    items: [
                                        {
                                            xtype: 'button',
                                            iconCls: 'button add',
                                            text: '添加',
                                            handler: function () {
                                                var gridpanel = Ext.getCmp('table_scheme_id');
                                                gridpanel.getPlugin('table_scheme_editor_plugin_id').cancelEdit();
                                                var gridstore = gridpanel.getStore();
                                                var index = gridstore.find('schid', -1);
                                                if (index < 0) {
                                                    var curDate = new Date();
                                                    var newnode = gridstore.insert(0, {
                                                        schid: -1, priority: false, createdata: Ext.Date.format(curDate, 'Y年m月d日'), schname: '', color: '#FFFFFF', creator: userjson.realname, description: ''
                                                    });
                                                    gridpanel.getSelectionModel().select(newnode);
                                                    gridpanel.getPlugin('table_scheme_editor_plugin_id').startEdit(gridpanel.getSelection()[0]);
                                                }
                                            }
                                        },
                                        {
                                            xtype: 'button',
                                            iconCls: 'button delete',
                                            text: '删除',
                                            handler: function () {
                                                var gridpanel = Ext.getCmp('table_scheme_id');
                                                gridpanel.getPlugin('table_scheme_editor_plugin_id').cancelEdit();
                                                var gridstore = gridpanel.getStore();
                                                var seletionnode = gridpanel.getSelection()[0];
                                                var projectcombox = Ext.getCmp('montor_project_combo_id');
                                                if (seletionnode != null) {
                                                    //seletionnode.data.functionid;
                                                    Ext.MessageBox.confirm("提示", "是否要删除监测项'" + seletionnode.data.schname + "'？", function (btnId) {
                                                        if (btnId == "yes") {
                                                            Ext.Ajax.request({
                                                                url: '/service/document/delete_table_scheme.ashx?params=' + params,
                                                                params: {
                                                                    schid: seletionnode.data.schid,
                                                                    taskid:projectcombox.getValue()
                                                                },
                                                                success: function (form, action) {
                                                                    var errorjson = Ext.decode(form.responseText);
                                                                    Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                                    
                                                                    schemenamestore.loadPage(1);
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
                                            xtype: 'button',
                                            iconCls: 'button modify',
                                            text: '从其他工点拷贝',
                                            handler: function () {
                                                var projectcombox = Ext.getCmp('montor_project_combo_id');
                                                copyfromothersite(params, projectcombox.getValue(), 'table_scheme_id');
                                            }
                                        },
                                        '->',
                                        {
                                            xtype: 'combo',
                                            id:'montor_project_combo_id',
                                            fieldLabel: '所属工点', //此时的项目就是任务
                                            labelAlign: 'right',
                                            msgTarget: 'under',
                                            editable: false,
                                            queryMode: 'local',
                                            store: combostore,
                                            displayField: 'taskname',
                                            valueField: 'taskid',
                                            triggerAction: 'all',
                                            value:records[0].data.taskid,
                                            forceSelection: true,
                                            listeners: {
                                                change: function (combo, newValue, oldValue, eOpts) {
                                                    schemenamestore.proxy.extraParams.taskid = newValue;
                                                    schemenamestore.loadPage(1);
                                                    rowdatastore.proxy.extraParams.schid = -1;
                                                    rowdatastore.loadPage(1);
                                                } 
                                            }
                                        }
                                    ]
                                }],
                                listeners: {
                                    rowclick: function (panel, record, element, rowIndex, e, eOpts) {

                                        rowdatastore.proxy.extraParams.schid = record.data.schid;
                                        rowdatastore.loadPage(1);

                                        //alert(record);
                                    }
                                },
                                bbar: Ext.create('Ext.PagingToolbar', {
                                    store: schemenamestore,
                                    displayInfo: true,
                                    displayMsg: '显示的条目 {0} - {1} of {2}',
                                    emptyMsg: "没有下载项目"
                                })
                            },
                            {
                                xtype: 'gridpanel',
                                id: "table_scheme_row_id",
                                padding: '10 5 10 0',
                                region: 'center',
                                resizable: true,
                                title: '监测项详细信息',
                                minHeight: 300,
                                minWidth: 1,
                                forceFit: true,
                                store: rowdatastore,
                                viewConfig: {
                                    getRowClass: function (record, rowIndex, rowParams, store) {
                                        if (record.data.valid == 0)
                                            return "x-grid-red";
                                    }
                                },
                                columns: [
                                    { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'rowid' },
                                    { header: 'validid', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'valid' },
                                    {
                                        header: '序号', minWidth: 1,
                                        editor: {
                                            xtype: 'textfield',
                                            regex: /^[0-9]*[1-9][0-9]*$/,
                                            regexText: '序号必须为大于0的正整数',
                                            allowBlank: false, emptyText: '序号不能为空', blankText: '序号不能为空'
                                        },
                                        width: 100, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'no'
                                    },
                                    {
                                        header: '文档列名', minWidth: 1,
                                        editor: {
                                            xtype: 'textfield',
                                            name: 'colname',
                                            tablepanelid: 'table_scheme_id',
                                            params: params,
                                            vtype: "colname",
                                            vtypeText: "列名不能重复",
                                            allowBlank: false, emptyText: '文档列名不能为空', blankText: '文档列名不能为空'
                                        },
                                        width: 100, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'rowname'
                                    },
                                    {
                                        header: '关联列名', minWidth: 1,
                                        editor: {
                                            xtype: 'combo',
                                            store: databasecol,
                                            displayField: 'name',
                                            valueField: 'name',
                                            editable: false,
                                            triggerAction: 'all',
                                            tpl: Ext.create('Ext.XTemplate',
                                                '<ul class="x-list-plain"><tpl for=".">',
                                                    '<li role="option" class="x-boundlist-item">{description}</li>',
                                                '</tpl></ul>'
                                            ),
                                            allowBlank: false, emptyText: '关联列名不能为空', blankText: '关联列名不能为空'
                                        },
                                        width: 100, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'colrel'
                                    },

                                    {
                                        header: '报警列', xtype: 'checkcolumn', dataIndex: 'alarmcheck', minWidth: 1,
                                        listeners:
                                        {
                                            beforecheckchange: function (item, rowIndex, checked, record, eOpts) {

                                                Ext.Ajax.request({
                                                    url: '/service/document/change_scheme_row_alarm.ashx?params=' + params,
                                                    params: {
                                                        rowid: record.data.rowid,
                                                        editable: checked
                                                    },
                                                    success: function (form, action) {

                                                        var schemepanel = Ext.getCmp('table_scheme_id');
                                                        var schemenode = schemepanel.getSelection()[0];
                                                        if (!Ext.isEmpty(schemenode)) {
                                                            rowdatastore.loadPage(rowdatastore.currentPage);
                                                        }
                                                    },
                                                    failure: function (form, action) {
                                                        var errorjson = Ext.decode(form.responseText);
                                                        Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                        var schemepanel = Ext.getCmp('table_scheme_id');
                                                        var schemenode = schemepanel.getSelection()[0];
                                                        if (!Ext.isEmpty(schemenode)) {
                                                            rowdatastore.loadPage(rowdatastore.currentPage);
                                                        }
                                                    }
                                                });
                                            }
                                        },
                                        width: 100, sortable: false, menuDisabled: true, draggable: false
                                    },
                                    {
                                        header: '类型',
                                        editor: { xtype: 'textfield', editable: false },
                                        minWidth: 1, width: 100, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'type'
                                    },
                                    {
                                        header: '描述', minWidth: 1, width: 100,
                                        editor: { xtype: 'textfield' },
                                        sortable: false, menuDisabled: true, draggable: false, dataIndex: 'description'
                                    }
                                ],
                                plugins: {
                                    ptype: 'treeediting',
                                    clicksToEdit: 2,
                                    pluginId: 'table_scheme_row_plugin_id',
                                    saveBtnText: '保存',
                                    cancelBtnText: "取消",
                                    listeners: {
                                        edit: function (editor, context, eOpts) {
                                            var gridpanel = Ext.getCmp('table_scheme_id');
                                            var gridstore = gridpanel.getStore();
                                            var seletionnode = gridpanel.getSelection()[0];
                                            if (seletionnode != null) {

                                                var index = databasecol.find('name', context.record.data.colrel);
                                                var type = databasecol.getAt(index).data.type;
                                                Ext.Ajax.request({
                                                    url: '/service/document/add_update_tablescheme_row.ashx?params=' + params,
                                                    params: {
                                                        rowid: context.record.data.rowid,
                                                        rowname: context.record.data.rowname,
                                                        colrel: context.record.data.colrel,
                                                        schid: seletionnode.data.schid,
                                                        no: context.record.data.no,
                                                        alarmcheck: context.record.data.alarmcheck,
                                                        type: type,
                                                        description: context.record.data.description
                                                    },
                                                    success: function (form, action) {
                                                        var json = Ext.decode(form.responseText);
                                                        Ext.MessageBox.alert("提示信息", json.msg);
                                                        rowdatastore.loadPage(1);
                                                    },
                                                    failure: function (form, action) {
                                                        var errorjson = Ext.decode(form.responseText);
                                                        Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                        rowdatastore.loadPage(1);
                                                    }
                                                });
                                            }
                                            else {
                                                Ext.MessageBox.alert("提示信息", '无法保存，请先选中左侧监测项');
                                                var gridpanel = Ext.getCmp('table_scheme_row_id');
                                                var gridstore = gridpanel.getStore();
                                                var index = gridstore.find('rowid', -1);
                                                if (index >= 0) {
                                                    gridstore.removeAt(index);
                                                    return;
                                                }
                                            }
                                            //      context.record.data.functionname,context.record.data.description,
                                        },
                                        canceledit: function (editor, context, eOpts) {

                                            var gridpanel = Ext.getCmp('table_scheme_row_id');
                                            var gridstore = gridpanel.getStore();
                                            var index = gridstore.find('rowid', -1);
                                            if (index >= 0) {
                                                gridstore.removeAt(index);
                                                return;
                                            }
                                        }
                                    },
                                },
                                dockedItems: [{
                                    xtype: 'toolbar',
                                    dock: 'top',
                                    items: [
                                        {
                                            xtype: 'button',
                                            iconCls: 'button add',
                                            text: '添加',
                                            handler: function () {
                                                var gridpanel = Ext.getCmp('table_scheme_row_id');
                                                gridpanel.getPlugin('table_scheme_row_plugin_id').cancelEdit();
                                                var gridstore = gridpanel.getStore();
                                                var index = gridstore.find('rowid', -1);
                                                if (index < 0) {
                                                    var newnode = gridstore.insert(0, {
                                                        rowid: -1, no: gridstore.data.items.length + 1, rowname: '', type: '', colrel: '', description: '', alarmcheck: 0
                                                    });
                                                    gridpanel.getSelectionModel().select(newnode);
                                                    gridpanel.getPlugin('table_scheme_row_plugin_id').startEdit(gridpanel.getSelection()[0]);
                                                }
                                                //  gridpanel.getView().editingPlugin.startEdit(newnode);

                                            }
                                        },
                                        {
                                            xtype: 'button',
                                            iconCls: 'button delete',
                                            text: '删除',
                                            handler: function () {
                                                var gridpanel = Ext.getCmp('table_scheme_row_id');
                                                gridpanel.getPlugin('table_scheme_row_plugin_id').cancelEdit();
                                                var gridstore = gridpanel.getStore();
                                                var seletionnode = gridpanel.getSelection()[0];
                                                if (seletionnode != null) {
                                                    var schemepanel = Ext.getCmp('table_scheme_id');
                                                    var schemenode = schemepanel.getSelection()[0];
                                                    if (schemenode != null) {
                                                        //seletionnode.data.functionid;
                                                        Ext.MessageBox.confirm("提示", "是否要删除列'" + seletionnode.data.rowname + "'？", function (btnId) {
                                                            if (btnId == "yes") {
                                                                Ext.Ajax.request({
                                                                    url: '/service/document/delete_table_scheme_row.ashx?params=' + params,
                                                                    params: {
                                                                        schid: schemenode.data.schid,
                                                                        rowid: seletionnode.data.rowid
                                                                    },
                                                                    success: function (form, action) {
                                                                        var errorjson = Ext.decode(form.responseText);
                                                                        Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                                        rowdatastore.loadPage(1);
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
                                                        Ext.MessageBox.alert("提示信息", '请在左侧列表中选择监测项');
                                                    }
                                                }
                                                else {
                                                    Ext.MessageBox.alert("提示信息", '请在列表中选择要删除的列');
                                                }
                                            }
                                        }
                                    ]
                                }],
                                bbar: Ext.create('Ext.PagingToolbar', {
                                    store: rowdatastore,
                                    displayInfo: true,
                                    displayMsg: '显示的条目 {0} - {1} of {2}',
                                    emptyMsg: "没有下载项目"
                                })
                            }
                        ]
                    });
                    funpanel.add(panel);
                }
            });
        }
    });
}

function copyfromothersite(params, currentid, gridid) {
    var panel = Ext.getCmp('copy_fromothersite_windows_id');
    if (!Ext.isEmpty(panel)) return;

    var combostore = Ext.create('Ext.data.Store', {
        fields: ['taskid', 'taskname'],
        model: 'TaskSimpe',
        proxy: {
            type: 'ajax',
            url: '/service/basepoint/getusertask.ashx?params=' + params,
            extraParams:
            {
                excepttaskid: currentid
            },
            reader: {
                type: 'json',
                rootProperty: 'data',
                totalProperty: 'totalCount'
            },
            autoLoad: true
        }
    });
    combostore.load();

    Ext.create('Ext.window.Window',
   {
       id: 'addbasepoint_windows_id',
       title: '拷贝监测项',
       height: 145,
       width: 400,
       layout: 'fit',
       modal: true,
       closable: false,
       items: [{
           xtype: 'form',
           width: '100%',
           layout: {
               align: 'middle',
               pack: 'center'
           },
           bodyPadding: '20px',
           items:
           [  
               {
                   xtype: 'combobox',
                   name: 'taskid',
                   fieldLabel: '拷贝的工点*', //此时的项目就是任务
                   anchor: '100%',
                   msgTarget: 'under',
                   editable: false,
                   padding: 5,
                   queryMode: 'local',
                   store: combostore,
                   displayField: 'taskname',
                   valueField: 'taskid',
                   triggerAction: 'all',
                   forceSelection: true,
                   allowBlank: false
               }
           ],
           buttonAlign: 'center',
           buttons: [{
               text: '数据拷贝',
               handler: function () {
                   var form = this.up('form').getForm();
                   var windows = this.up('window');
                   var closebutton = Ext.getCmp('close_window_button_id');

                   if (form.isValid()) {
                       form.submit({
                           url: '/service/projectsite/copy_tablescheme.ashx?params=' + params,
                           params: {
                               destid: currentid
                           },
                           success: function (fp, o) {
                               //显示浏览数据按钮
                               //从文档中读取表格并存储
                               var gridpanel = Ext.getCmp(gridid);
                               var store = gridpanel.getStore();
                               store.loadPage(store.currentPage);
                               Ext.Msg.alert('提示', o.result.msg);
                               windows.close();
                           },
                           failure: function (form, action) {
                               Ext.Msg.alert('提示', action.result.msg);
                               windows.close();
                           }
                       });


                   }
               }
           },
           {
               text: '关闭窗口',
               id: 'close_window_button_id',
               handler: function () {
                   var windows = this.up('window');
                   windows.close();
               }
           }

           ]
       }]
   }).show();
}