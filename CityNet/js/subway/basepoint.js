//工作基点
function basepoint_manager(params) {

    var mode = 0; //全选
    var workplace = Ext.getCmp('index_workplace_id');
    workplace.removeAll();
    workplace.clearListeners();
    Ext.Ajax.request({
        url: '/service/user/get_userinfomation.ashx?params=' + params,
        success: function (form, action) 
        {
            var userjson = Ext.decode(form.responseText);//当前用户信息
            //分页显示
            var datastore = Ext.create('Ext.data.Store', {
                model: 'BasePoint',
                pageSize: 15,
                proxy: {
                    type: 'ajax',
                    url: '/service/basepoint/getbasepoint.ashx?params=' + params,
                    extraParams: {
                        mode: mode
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
                id: 'basepoint_gridview_id',
                forceFit: true,
                store: datastore,
                columns: [
                    { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, renderer: rendercell, draggable: false, dataIndex: 'id' },
                    { header: 'TaskID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, renderer: rendercell, draggable: false, dataIndex: 'taskid' },
                    { header: '选择列', xtype: 'checkcolumn', dataIndex: 'selectcheck', minWidth: 1, sortable: false, menuDisabled: true, draggable: false ,defaultAlign:'l-l'},
                    { header: '所属工点', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, renderer: rendercell, dataIndex: 'sitename' },
                    { header: '点名', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, renderer: rendercell, dataIndex: 'pointname' },
                    { header: '类型', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, renderer: rendercell, dataIndex: 'type' },
                    { header: 'X坐标(m)', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, renderer: rendercell, dataIndex: 'x' },
                    { header: 'Y坐标(m)', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, renderer: rendercell, dataIndex: 'y' },
                    { header: '高程(Z坐标)(m)', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, renderer: rendercell, dataIndex: 'z' },
                    { header: '经度(°)', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, renderer: rendercell, dataIndex: 'lon' },
                    { header: '纬度(°)', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, renderer: rendercell, dataIndex: 'lat' },
                    { header: '上传人', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, renderer: rendercell, dataIndex: 'creator' }],
                bbar: Ext.create('Ext.PagingToolbar', {
                    store: datastore,
                    displayInfo: true,
                    displayMsg: '显示的条目 {0} - {1} of {2}',
                    emptyMsg: "没有下载项目"
                }),

                dockedItems: [{
                    xtype: 'toolbar',
                    height:40,
                    dock: 'top',
                    items: [
                        '->',
                        {
                            xtype: 'authoritybutton',
                            params: Ext.application.params,
                            id: 'add_basepoint_button_id',
                            iconCls: 'button add',
                            text: '添加基点',
                            handler: function () {
                                uploadbasepoint("basepoint_gridview_id", params,null);
                            }
                        },
                        {
                            xtype: 'authoritybutton',
                            params: Ext.application.params,
                            id: 'delete_basepoint_button_id',
                            iconCls: 'button delete',
                            text: '删除基点',
                            handler: function () {
                                Ext.MessageBox.confirm("提示", "是否删除选择的工作基点？", function (btnId) {
                                    if (btnId == "yes") {
                                        var items = Ext.getCmp('basepoint_gridview_id').getStore().data.items;
                                        var length = items.length;
                                        var i;
                                        var ids = "";
                                        for (i = 0; i < length; i++) {
                                            var item = items[i];
                                            if (item.data.selectcheck) {
                                                ids += item.data.id + ",";
                                            }
                                        }
                                        Ext.Ajax.request({
                                            url: '/service/basepoint/deletebasepoint.ashx?params=' + params,
                                            params: {
                                                ids:ids
                                            },
                                            success: function (form, action) {
                                                var json = Ext.decode(form.responseText);
                                                Ext.MessageBox.alert("提示信息", json.msg);
                                                Ext.getCmp('basepoint_gridview_id').getStore().loadPage(1);
                                            },
                                            failure: function (form, action) {
                                                var errorjson = Ext.decode(form.responseText);
                                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                Ext.getCmp('basepoint_gridview_id').getStore().loadPage(1);
                                            }
                                        });
                                    }
                                });
                            }
                        },
                        {
                            xtype: 'authoritybutton',
                            params: Ext.application.params,
                            id: 'modify_basepoint_button_id',
                            iconCls: 'button modify',
                            text: '修改基点',
                            handler: function () {
                                var items = Ext.getCmp('basepoint_gridview_id').getStore().data.items;
                                var length = items.length;
                                var i;
                                var ids = "";
                                var sel_item = null;
                                for (i = 0; i < length; i++) {
                                    var item = items[i];
                                    if (item.data.selectcheck) {
                                        sel_item = item;
                                        break;
                                    }
                                }
                                if (sel_item != null) {
                                    uploadbasepoint("basepoint_gridview_id", params, sel_item);
                                }
                                else {
                                    Ext.MessageBox.alert("提示信息", '请选择要修改的列');
                                }

                            }
                        }
                    ]
                }]
            });
            workplace.add(panel);
            datastore.loadPage(1);
        }
    }
    );
}

function uploadbasepoint(gridid, params,item) {
    var panel = Ext.getCmp('addbasepoint_windows_id');
    if (!Ext.isEmpty(panel)) return;


    var unitstore = Ext.create('Ext.data.Store', {
        fields: ['name'],
        data: [
            { "name": "水准仪基点" },
            { "name": "全站仪基点" },
            { "name": "水准仪全站仪共用基点" }
        ]
    });
    /*
    var combostore = Ext.create('Ext.data.JsonStore', {
        fields: ['taskid', 'taskname'],
        proxy: {
            type: 'jsonp',
            url: '/service/basepoint/getusertask.ashx?params=' + params,
            reader: {
                type: 'json',
                rootProperty: 'roots',
                totalProperty: 'totalCount'
            }
        },
       
    });*/

    var combostore = Ext.create('Ext.data.Store', {
        fields: ['taskid', 'taskname'],
        model: 'TaskSimpe',
        proxy: {
            type: 'ajax',
            url: '/service/basepoint/getusertask.ashx?params=' + params,
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
        title: '添加工作基点',
        height: 460,
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
                        xtype: 'textfield',
                        name: 'pointname',
                        fieldLabel: '点名 *',
                        anchor: '100%',
                        msgTarget: 'under',
                        padding: 5,
                        editable: item == null,
                        disabled: item != null,
                        allowBlank: false,
                        value: item == null ? '' : item.data.pointname
                    },

                    {
                        xtype: 'combobox',
                        name: 'type',
                        fieldLabel: '类型 *',
                        anchor: '100%',
                        msgTarget: 'under',
                        padding: 5,
                        store: unitstore,
                        queryMode: 'local',
                        displayField: 'name',
                        valueField: 'name',
                        editable: false,
                        value: item == null ? '水准仪基点' : item.data.type,
                        allowBlank: false
                    },
                    {
                        xtype: 'combobox',
                        name: 'taskid',
                        fieldLabel: '工点名称 *', //此时的项目就是任务
                        anchor: '100%',
                        msgTarget: 'under',
                        editable: false,
                        disabled:item != null,
                        padding: 5,
                        queryMode: 'local',
                        store: combostore,
                        displayField: 'taskname',
                        valueField: 'taskid',
                        triggerAction: 'all',
                        forceSelection: true,
                        value: item == null ? '' : item.data.taskid,
                        allowBlank: false
                    },

                    {
                        xtype: 'textfield',
                        name: 'x',
                        fieldLabel: 'x坐标(米)',
                        anchor: '100%',
                        msgTarget: 'under',
                        regexText: '必须为数字',
                        regex: /^(-?\d+)(\.\d+)?$/,
                        value: item == null ? '' : item.data.x,
                        padding: 5
                    },

                    {
                        xtype: 'textfield',
                        name: 'y',
                        fieldLabel: 'y坐标(米)',
                        anchor: '100%',
                        msgTarget: 'under',
                        regexText: '必须为数字',
                        regex: /^(-?\d+)(\.\d+)?$/,
                        value: item == null ? '' : item.data.y,
                        padding: 5
                    },

                    {
                        xtype: 'textfield',
                        name: 'z',
                        fieldLabel: '高程-z坐标(米)',
                        anchor: '100%',
                        msgTarget: 'under',
                        regexText: '必须为数字',
                        regex: /^(-?\d+)(\.\d+)?$/,
                        value: item == null ? '' : item.data.z,
                        padding: 5
                    },

                    {
                        xtype: 'textfield',
                        name: 'lon',
                        fieldLabel: '经度(°)',
                        anchor: '100%',
                        max: 180.0,
                        min: -180.0,
                        msgTarget: 'under',
                        vtype: 'degree',
                        value: item == null ? '' : item.data.lon,
                        vtypeText: '经度为度分秒格式（如30°13′26.85″）或小数（如30.133），范围在[-180.0,180.0]',
                        padding: 5
                    },

                    {
                        xtype: 'textfield',
                        name: 'lat',
                        fieldLabel: '纬度(°)',
                        anchor: '100%',
                        msgTarget: 'under',
                        padding: 5,
                        max: 90.0,
                        min: -90.0,
                        value: item == null ? '' : item.data.lat,
                        vtype: 'degree',
                        vtypeText: '纬度为度分秒格式（如30°13′26.85″）或小数（如30.133），范围在[-90.0,90.0]'
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
                        form.submit({
                            url: '/service/basepoint/savebasepoint.ashx?params=' + params,
                            params: {
                                mode: item == null,
                                extrapointname:item == null ? "" : item.data.pointname,
                                extrataskid: item == null ? "" : item.data.taskid
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
