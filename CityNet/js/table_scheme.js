function tablescheme_manager(params) {
    var funpanel = Ext.getCmp('function_area_id');
    funpanel.removeAll();
   
    Ext.Ajax.request({
        url: '/service/user/get_userinfomation.ashx?params=' + params,
        success: function (form, action) {
            var userjson = Ext.decode(form.responseText);//当前用户信息
           
            var schemenamestore = Ext.create('Ext.data.Store', {
                model: 'TableSchemeName',
                pageSize: 15,
                proxy: {
                    type: 'ajax',
                    url: '/service/document/get_alltablescheme.ashx?params=' + params,
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
                        schid:-1
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

            funpanel.addListener('resize',function(panel, width, height, oldWidth, oldHeight, eOpts){
            
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
                resizable:true,
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
                        title: '方案名称列表',
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
                                header: '推荐方案', xtype: 'checkcolumn', dataIndex: 'priority', minWidth: 1,
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
                                header: '方案名', minWidth: 1,
                                editor: { xtype: 'textfield', allowBlank: false, emptyText: '方案名称不能为空', blankText: '方案名称不能为空' },
                                width: 160, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'schname'
                            },
                            {
                                header: '标识颜色',
                                renderer: function (value, m) {
                                    m.style = 'background:' + value + ';';
                                    return value;
                                },
                                editor: { xtype: 'colorfield',editable:false },
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
                                    Ext.Ajax.request({
                                        url: '/service/document/add_update_tablescheme.ashx?params=' + params,
                                        params: {
                                            schid: context.record.data.schid,
                                            schname: context.record.data.schname,
                                            color: context.record.data.color,
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
                                        //  gridpanel.getView().editingPlugin.startEdit(newnode);

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
                                        if (seletionnode != null) {
                                            //seletionnode.data.functionid;
                                            Ext.MessageBox.confirm("提示", "是否要删除方案'" + seletionnode.data.schname + "'？", function (btnId) {
                                                if (btnId == "yes") {
                                                    Ext.Ajax.request({
                                                        url: '/service/document/delete_table_scheme.ashx?params=' + params,
                                                        params: {
                                                            schid: seletionnode.data.schid,
                                                            taskid:-1
                                                        },
                                                        success: function (form, action) {
                                                            var errorjson = Ext.decode(form.responseText);
                                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                            gridloadcurrentPage(schemenamestore);
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
                            ]
                        }],
                        listeners: {
                            rowclick: function( panel, record, element, rowIndex, e, eOpts )
                            {
                                rowdatastore.loadPage(1, {
                                    params: {
                                        schid: record.data.schid
                                    }
                                });

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
                        padding: '10 30 10 0',
                        region: 'center',
                        resizable: true,
                        title: '方案列表',
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
                                //    tpl: '<tpl for="."><div class="x-combo-list-item" ext:qtip="{description}">{description}</div></tpl>',
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
                                                    rowdatastore.loadPage(rowdatastore.currentPage, {
                                                        params: {
                                                            schid: schemenode.data.schid
                                                        }
                                                    });
                                                }
                                            },
                                            failure: function (form, action) {
                                                var errorjson = Ext.decode(form.responseText);
                                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                var schemepanel = Ext.getCmp('table_scheme_id');
                                                var schemenode = schemepanel.getSelection()[0];
                                                if (!Ext.isEmpty(schemenode)) {
                                                    rowdatastore.loadPage(rowdatastore.currentPage, {
                                                        params: {
                                                            schid: schemenode.data.schid
                                                        }
                                                    });
                                                }
                                            }
                                        });
                                    }
                                },
                                width: 100, sortable: false, menuDisabled: true, draggable: false
                            },
                            {
                                header: '类型',
                                editor: { xtype: 'textfield', editable: false},
                                minWidth: 1, width: 100, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'type'
                            },
                            {
                                header: '描述', minWidth: 1,width: 100,
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
                                                rowdatastore.loadPage(1, {
                                                    params: {
                                                        schid: seletionnode.data.schid
                                                    }});
                                            },
                                            failure: function (form, action) {
                                                var errorjson = Ext.decode(form.responseText);
                                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                rowdatastore.loadPage(1, {
                                                    params: {
                                                        schid: seletionnode.data.schid
                                                    }
                                                });
                                            }
                                        });
                                    }
                                    else {
                                        Ext.MessageBox.alert("提示信息", '无法保存，请先选中左侧方案');
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
                                                rowid: -1, no: gridstore.data.items.length + 1, rowname: '',type:'', colrel: '', description: '',alarmcheck:0
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
                                                                rowdatastore.loadPage(1, {
                                                                    params: {
                                                                        schid: schemenode.data.schid
                                                                    }
                                                                });
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
                                                Ext.MessageBox.alert("提示信息", '请在左侧列表中选择方案');
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