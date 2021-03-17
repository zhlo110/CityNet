
function alarm_scheme_rule(params) {
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
                model: 'Alarm',
                pageSize: 15,
                proxy: {
                    type: 'ajax',
                    url: '/service/document/get_alarmby_schid.ashx?params=' + params,
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

            var unitstore = Ext.create('Ext.data.Store', {
                fields: ['name'],
                data: [
                    { "name": "毫米" },
                    { "name": "厘米" },
                    { "name": "分米" },
                    { "name": "米" }
                ]
            });


            funpanel.addListener('resize', function (panel, width, height, oldWidth, oldHeight, eOpts) {

                var childpanel = Ext.getCmp('workedchild_id');
                childpanel.setHeight(height - 35);

                var panel1 = Ext.getCmp('table_alarm_scheme_id');
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
                        id: "table_alarm_scheme_id",
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
                                editor: { xtype: 'colorfield', editable: false },
                                minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'color'
                            },
                            { header: '创建人', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'creator' },
                            { header: '创建日期', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'createdata' },
                            {
                                header: '描述', minWidth: 1,
                                editor: { xtype: 'textfield' },
                                sortable: false, menuDisabled: true, draggable: false, dataIndex: 'description'
                            }
                        ],
                        
                        listeners: {
                            rowclick: function (panel, record, element, rowIndex, e, eOpts) {
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
                        id: "table_scheme_alarm_id",
                        padding: '10 30 10 0',
                        region: 'center',
                        resizable: true,
                        title: '报警列表',
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
                            { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'alarmid' },
                            {
                                header: '报警名称', minWidth: 1,
                                editor: {
                                    xtype: 'textfield',
                                    allowBlank: false, emptyText: '名称不能为空', blankText: '名称不能为空'
                                },
                                width: 100, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'alarmname'
                            },

                            {
                                header: '报警等级', minWidth: 1,
                                editor: {
                                    xtype: 'textfield',
                                    regex: /^[0-9]*[1-9][0-9]*$/,
                                    regexText: '报警等级必须为大于0的正整数',
                                    allowBlank: false, emptyText: '报警等级不能为空', blankText: '报警等级不能为空'
                                },
                                width: 100, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'alarmlevel'
                            },

                            {
                                header: '单位',
                                editor: {
                                    xtype: 'combobox',
                                    store: unitstore,
                                    queryMode: 'local',
                                    displayField: 'name',
                                    valueField: 'name',
                                    editable: false
                                },
                                minWidth: 1, width: 100, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'unit'
                            },
                            {
                                header: '报警上限', minWidth: 1,
                                editor: {
                                    xtype: 'textfield',
                                    id: 'upper_value_id',
                                    vtype: "greater",
                                    confirmTo: "lower_value_id",
                                    regex: /^(-?[0-9]+)(\.[0-9]+)?$/,
                                    regexText: '输入必须为数字',
                                    vtypeText: "上限的数字要比下限的数字大",
                                    allowBlank: false, emptyText: '报警上限不能为空', blankText: '报警上限不能为空'
                                },
                                width: 100, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'maxlevel'
                            },
                            {
                                header: '报警下限', minWidth: 1,
                                editor: {
                                    xtype: 'textfield',
                                    id: 'lower_value_id',
                                    vtype: "smaller",
                                    confirmTo: "upper_value_id",
                                    regex: /^(-?[0-9]+)(\.[0-9]+)?$/,
                                    regexText: '输入必须为数字',
                                    vtypeText: "下限的数字要比上限的数字小",
                                    allowBlank: false, emptyText: '报警上限不能为空', blankText: '报警上限不能为空'
                                },
                                width: 100, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'minlevel'
                            },
                            {
                                header: '颜色',
                                renderer: function (value, m) {
                                    m.style = 'background:' + value + ';';
                                    return value;
                                },
                                editor: { xtype: 'colorfield', editable: false },
                                minWidth: 1, width: 100, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'color'
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
                            pluginId: 'table_scheme_alarm_plugin_id',
                            saveBtnText: '保存',
                            cancelBtnText: "取消",
                            listeners: {
                                edit: function (editor, context, eOpts) {
                                    var gridpanel = Ext.getCmp('table_alarm_scheme_id');
                                    var gridstore = gridpanel.getStore();
                                    var seletionnode = gridpanel.getSelection()[0];
                                    if (seletionnode != null) {
                                        Ext.Ajax.request({
                                            url: '/service/document/add_update_tablealarm.ashx?params=' + params,
                                            params: {
                                                schid: seletionnode.data.schid,
                                                alarmid:context.record.data.alarmid,
                                                alarmname: context.record.data.alarmname,
                                                minlevel: context.record.data.minlevel,
                                                maxlevel: context.record.data.maxlevel,
                                                color: context.record.data.color,
                                                unit: context.record.data.unit,
                                                alarmlevel:context.record.data.alarmlevel,
                                                description: context.record.data.description
                                            },
                                            success: function (form, action) {
                                                var json = Ext.decode(form.responseText);
                                                Ext.MessageBox.alert("提示信息", json.msg);
                                                rowdatastore.loadPage(1, {
                                                    params: {
                                                        schid: seletionnode.data.schid
                                                    }
                                                });
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
                                        var gridpanel = Ext.getCmp('table_scheme_alarm_id');
                                        var gridstore = gridpanel.getStore();
                                        var index = gridstore.find('alarmid', -1);
                                        if (index >= 0) {
                                            gridstore.removeAt(index);
                                            return;
                                        }

                                    }
                                    //      context.record.data.functionname,context.record.data.description,
                                },
                                canceledit: function (editor, context, eOpts) {

                                    var gridpanel = Ext.getCmp('table_scheme_alarm_id');
                                    var gridstore = gridpanel.getStore();
                                    var index = gridstore.find('alarmid', -1);
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

                                        var gridpanel = Ext.getCmp('table_scheme_alarm_id');
                                        gridpanel.getPlugin('table_scheme_alarm_plugin_id').cancelEdit();
                                        var gridstore = gridpanel.getStore();
                                        var index = gridstore.find('alarmid', -1);
                                        var minlevel = 0.0;
                                        var unit = '米';
                                        var alarmlevel = 1;

                                        if (!Ext.isEmpty(gridstore.getAt(0))) {
                                            minlevel = gridstore.getAt(0).data.maxlevel;
                                            unit = gridstore.getAt(0).data.unit;
                                            alarmlevel = gridstore.getAt(0).data.alarmlevel+1;
                                        }
                                        
                                        if (index < 0) {

                                            var newnode = gridstore.insert(0, {
                                                alarmid: -1, alarmlevel: alarmlevel, alarmname: '', minlevel: minlevel, maxlevel: '', description: '', color: '#FFFFFF', unit: unit
                                            });
                                            gridpanel.getSelectionModel().select(newnode);
                                            gridpanel.getPlugin('table_scheme_alarm_plugin_id').startEdit(gridpanel.getSelection()[0]);
                                        }
                                        //  gridpanel.getView().editingPlugin.startEdit(newnode);

                                    }
                                },
                                {
                                    xtype: 'button',
                                    iconCls: 'button delete',
                                    text: '删除',
                                    handler: function () {
                                        var gridpanel = Ext.getCmp('table_scheme_alarm_id');
                                        gridpanel.getPlugin('table_scheme_alarm_plugin_id').cancelEdit();
                                        var gridstore = gridpanel.getStore();
                                        var seletionnode = gridpanel.getSelection()[0];
                                        if (seletionnode != null) {
                                            var schemepanel = Ext.getCmp('table_alarm_scheme_id');
                                            var schemenode = schemepanel.getSelection()[0];
                                            if (schemenode != null) {
                                                //seletionnode.data.functionid;
                                                Ext.MessageBox.confirm("提示", "是否要删除列'" + seletionnode.data.alarmname + "'？", function (btnId) {
                                                    if (btnId == "yes") {
                                                        Ext.Ajax.request({
                                                            url: '/service/document/delete_table_scheme_alarm.ashx?params=' + params,
                                                            params: {
                                                                schid: schemenode.data.schid,
                                                                alarmid: seletionnode.data.alarmid
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
                                },
                                {
                                    xtype: 'button',
                                    iconCls: 'button refresh',
                                    text: '更新报警点',
                                    handler: function () {
                                        var gridpanel = Ext.getCmp('table_scheme_alarm_id');
                                        gridpanel.getPlugin('table_scheme_alarm_plugin_id').cancelEdit();
                                        var schemepanel = Ext.getCmp('table_alarm_scheme_id');
                                        var schemenode = schemepanel.getSelection()[0];
                                        if (schemenode != null) {
                                            Ext.MessageBox.confirm("提示", "是否要更新报警方案'" + schemenode.data.schname + "'？", function (btnId) {
                                                if (btnId == "yes") {
                                                    var myMask = new Ext.LoadMask({
                                                        target: gridpanel,
                                                        msg: '正在更新报警方案，请稍后！'
                                                    });
                                                    myMask.show();
                                                    Ext.Ajax.request({
                                                        url: '/service/alarm/refresh_alarm_byschid.ashx?params=' + params,
                                                        params: {
                                                            schid: schemenode.data.schid
                                                        },
                                                        success: function (form, action) {
                                                            var errorjson = Ext.decode(form.responseText);
                                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                            myMask.hide();
                                                        },
                                                        failure: function (form, action) {
                                                            var errorjson = Ext.decode(form.responseText);
                                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                            myMask.hide();
                                                        }
                                                    });
                                                    }
                                                });
                                            }
                                            else {
                                                Ext.MessageBox.alert("提示信息", '请在左侧列表中选择方案');
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