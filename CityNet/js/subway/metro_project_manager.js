
function metro_project_manager(params) {
    var funpanel = Ext.getCmp('index_workplace_id');
    funpanel.removeAll();
    funpanel.clearListeners();
    Ext.Ajax.request({
        url: '/service/user/get_userinfomation.ashx?params=' + params,
        success: function (form, action) {
            var userjson = Ext.decode(form.responseText);//当前用户信息
           treestore = new Ext.data.TreeStore(
           {
               proxy: {
                   type: 'ajax',
                   url: '/service/user/build_department_tree.ashx?showPerson=false',
                   reader: 'json'
               },
               root: {
                   name: '根节点',
                   id: 'project_id'
               }
           });
           
           var datastore = Ext.create('Ext.data.Store', {
               model: 'ProjectSite',
               pageSize: 15,
               proxy: {
                   type: 'ajax',
                   url: '/service/projectsite/get_available_projectsite.ashx?params=' + params,
                   extraParams: {
                       departmentid: -1
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
          

           funpanel.addListener('resize', function (panel, width, height, oldWidth, oldHeight, eOpts) {
               var childpanel = Ext.getCmp('workedchild_id');
               childpanel.setHeight(height - 35);
               var panel1 = Ext.getCmp('task_project_list_id');
               panel1.setWidth(width / 3);
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
				    xtype: 'treepanel',
				    id: "task_department_list_id",
				    padding: '10 0 10 10',
				    region: 'west',
				    width: funpanel.getWidth() / 3,
				    minWidth: 1,
				    split: true,
				    resizable: true,
				    title: '部门组织树',
				    rootVisible: false,
				    minHeight: 300,
				    forceFit: true,
				    store: treestore,
				    listeners: {
				        itemclick: function ( tree, record, item, index, e, eOpts ) {
				            datastore.proxy.extraParams.departmentid = record.data.id;
				            datastore.loadPage(1);
				            //datastore
				        }
				    }
				},
				{
				    title: '工点列表',
				    xtype: 'gridpanel',
				    id: 'projectsite_gird_id',
				    padding: '10 10 10 2',
				    region: 'center',
				    minWidth: 1,
				    split: true,
				    resizable: true,
				    forceFit: true,
				    store: datastore,
				    viewConfig: {
				        getRowClass: function (record, rowIndex, rowParams, store) {
				            if (record.get("dtstate")==1)
				                return 'x-grid-red';
				            else
				                return 'x-grid-green';
				        }
				    },
				    columns: [
                             { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'taskid' },
                             {
                                 header: '工点名称', minWidth: 1, align: 'center',
                                 sortable: false, menuDisabled: true, draggable: false, dataIndex: 'taskname'
                             },
                             {
                                 header: '工点类型', minWidth: 1, align: 'center',
                                 sortable: false, menuDisabled: true, draggable: false, dataIndex: 'sitetype'
                             },
                             {
                                 header: '工点状态', minWidth: 1, align: 'center',
                                 sortable: false, menuDisabled: true, draggable: false, dataIndex: 'sitestate'
                             },
                             {
                                 header: '里程范围', minWidth: 1, align: 'center',
                                 sortable: false, menuDisabled: true, draggable: false, dataIndex: 'range'
                             },
                             {
                                 header: '施工方向', minWidth: 1, align: 'center',
                                 sortable: false, menuDisabled: true, draggable: false, dataIndex: 'direction'
                             },
                             {
                                 header: '描述', minWidth: 1, align: 'center',
                                 sortable: false, menuDisabled: true, draggable: false, dataIndex: 'description'
                             }
				    ],
				    bbar: Ext.create('Ext.PagingToolbar', {
				        store: datastore,
				        displayInfo: true,
				        displayMsg: '显示的条目 {0} - {1} of {2}',
				        emptyMsg: "没有下载项目"
				    }),
				    dockedItems: [{
				        xtype: 'toolbar',
				        dock: 'top',
				        items: [{
				            xtype: 'authoritybutton',
				            params: Ext.application.params,
				            id: 'add_project_site_button_id',
				            iconCls: 'button add',
				            text: '添加工点',
				            handler: function () {
				                var treepannel = Ext.getCmp('task_department_list_id');
				                if (treepannel.getSelection().length == 0) {
				                    Ext.MessageBox.alert("提示信息","请先选择左侧组织机构");
				                }
				                else {
                                    //组织树id
				                    var departmentid = treepannel.getSelection()[0].data.id;
				                    showsitewindow(departmentid, datastore,"new",null);
				                }

				            }
				        },
				        {
				            xtype: 'authoritybutton',
				            params: Ext.application.params,
				            id: 'delete_project_site_button_id',
				            iconCls: 'button delete',
				            text: '删除工点',
				            handler: function () {
				                var projectsitegrid = Ext.getCmp('projectsite_gird_id');
				                if (projectsitegrid.getSelection().length <= 0) {
				                    Ext.MessageBox.alert("提示信息", '请选择要删除的工点');
				                }
				                else {
				                    
				                    var selectNode = projectsitegrid.getSelection()[0];
                                    Ext.MessageBox.confirm("提示", "确定要删除工点‘" + selectNode.data.taskname + "’吗？", function (btnId) {
                                        if (btnId == "yes") {
                                            Ext.Ajax.request({
                                                url: '/service/task/delete_tasks.ashx?params=' + params,
                                                params: {
                                                    taskid: selectNode.data.taskid,
                                                },
                                                success: function (form, action) {
                                                    var errorjson = Ext.decode(form.responseText);
                                                    Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                    gridloadcurrentPage(datastore);
    
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
                             xtype: 'authoritybutton',
                             params: Ext.application.params,
                             id: 'update_project_site_button_id',
                             iconCls: 'button delete',
                             text: '修改工点',
                             handler: function () {

                                 var treepannel = Ext.getCmp('task_department_list_id');
                                 if (treepannel.getSelection().length == 0) {
                                     Ext.MessageBox.alert("提示信息", "请先选择左侧组织机构");
                                 }
                                 else {
                                     var projectsitegrid = Ext.getCmp('projectsite_gird_id');
                                     if (projectsitegrid.getSelection().length == 0) {
                                         Ext.MessageBox.alert("提示信息", "请先选择要修改的工点");
                                     }
                                     else {
                                         var seldata = projectsitegrid.getSelection()[0].data;
                                         //组织树id
                                          var departmentid = treepannel.getSelection()[0].data.id;
                                         showsitewindow(departmentid, datastore, "update", seldata);
                                     }
                                 }

                             }
                        }]
				    }]
				}
               ]
           });
           funpanel.add(panel);
        }
    });
}

function enablectlfield(show, id) {
    var field = Ext.getCmp(id);
    if (show) {
        field.show();
        field.setValue('');
    }
    else {
        field.hide();
        field.setValue('-99999.0');
    }
}

function showsitewindow(departmentid, datastore,oper,seldata) {
    var panel = Ext.getCmp('add_modify_projectsite_windows_id');
    Ext.QuickTips.init();
    if (!Ext.isEmpty(panel)) return;

    var typestore = Ext.create('Ext.data.Store', {
        fields: [ 'name'],
        data: [
            { "name": "地铁盾构施工" },
            { "name": "地铁矿山法施工" },
            { "name": "地铁车站施工" },
            { "name": "明挖区间施工" }
        ]
    });

    var statestore = Ext.create('Ext.data.Store', {
        fields: ['name'],
        data: [
            { "name": "未开始" },
            { "name": "施工期" },
            { "name": "运营期" }
        ]
    });
    var winheight = 400;
    if (seldata != null && seldata.sitetype == '地铁车站施工') {
        winheight = 305;
    }


    Ext.create('Ext.window.Window',
    {
        id: 'add_modify_projectsite_windows_id',
        title: '添加/修改工点',
        height: winheight,
        width: 400,
        layout: 'fit',
        modal: true,
        closable: false,
        items: [{
            xtype: 'form',
            width: '100%',
            fieldDefaults: {
                labelAlign: 'right',
                labelWidth: 60
            },
            bodyPadding: '20px',
            items:
            [
               {
                   xtype: 'textfield',
                   name: 'sitename',
                   fieldLabel: '工点名称',
                   blankText: '请输入工点名称',
                   anchor: '100%',
                   msgTarget: 'qtip',
                   value:seldata==null?'':seldata.taskname,
                   padding: 5,
                   allowBlank: false
               },
               {
                   xtype: 'combo',
                   name: 'sitetype',
                   fieldLabel: '工点类型',
                   blankText: '请输入工点类型',
                   anchor: '100%',
                   store: typestore,
                   queryMode: 'local',
                   displayField: 'name',
                   valueField: 'name',
                   msgTarget: 'qtip',
                   value: seldata == null ? '地铁盾构施工' : seldata.sitetype,
                   padding: 5,
                   allowBlank: false,
                   listeners: {
                       change: function (combo, newValue, oldValue, eOpts) {
                           if (newValue == '地铁车站施工') {
                               enablectlfield(false, 'site_profix_field_id');
                               enablectlfield(false, 'site_beginmileage_field_id');
                               enablectlfield(false, 'site_endmileage_field_id');
                               var windows = Ext.getCmp('add_modify_projectsite_windows_id');
                               windows.setHeight(305);
                           }
                           else {
                               enablectlfield(true, 'site_profix_field_id');
                               enablectlfield(true, 'site_beginmileage_field_id');
                               enablectlfield(true, 'site_endmileage_field_id');
                               var windows = Ext.getCmp('add_modify_projectsite_windows_id');
                               windows.setHeight(400);
                           }
                       }
                   }
               },
               {
                   xtype: 'textfield',
                   id:'site_profix_field_id',
                   name: 'prefix',
                   fieldLabel: '里程前缀',
                   blankText: '请输入里程前缀',
                   anchor: '100%',
                   msgTarget: 'qtip',
                   value: seldata == null ? '' : seldata.prefix,
                   padding: 5,
                   hidden: seldata != null && seldata.sitetype=='地铁车站施工',
                   allowBlank: false
               },
               {
                   xtype: 'textfield',
                   id: 'site_beginmileage_field_id',
                   name: 'beginmileage',
                   regexText: '里程必须为数字',
                   regex: /^(-?\d+)(\.\d+)?$/,
                   fieldLabel: '开始里程',
                   blankText: '请输入开始里程',
                   anchor: '100%',
                   msgTarget: 'qtip',
                   hidden: seldata != null && seldata.sitetype == '地铁车站施工',
                   value: seldata == null ? '' : seldata.mileagebegin,
                   padding: 5,
                   allowBlank: false
               },
               {
                   xtype: 'textfield',
                   id: 'site_endmileage_field_id',
                   name: 'endmileage',
                   regexText: '里程必须为数字',
                   regex: /^(-?\d+)(\.\d+)?$/,
                   fieldLabel: '结束里程',
                   blankText: '请输入结束里程',
                   anchor: '100%',
                   msgTarget: 'qtip',
                   hidden: seldata != null && seldata.sitetype == '地铁车站施工',
                   padding: 5,
                   value: seldata == null ? '' : seldata.mileageend,
                   allowBlank: false
               },
               {
                   xtype: 'radiogroup',
                   fieldLabel: '施工方向',
                   anchor: '100%',
                   columns: 2,
                   items: [
                       { boxLabel: '正向施工', name: 'direction', inputValue: '1', checked: seldata == null || seldata.direction =='正向施工' },
                       { boxLabel: '逆向施工', name: 'direction', inputValue: '0', checked: seldata != null && seldata.direction != '正向施工' }
                   ],
                   padding: 5,
                   allowBlank: false
               },
               {
                   xtype: 'combo',
                   name: 'state',
                   fieldLabel: '施工状态',
                   anchor: '100%',
                   msgTarget: 'qtip',
                   padding: 5,
                   store: statestore,
                   queryMode: 'local',
                   displayField: 'name',
                   valueField: 'name',
                   value: seldata == null ? '未开始' : seldata.sitestate,
                   allowBlank: false
               },
               {
                   xtype: 'textarea',
                   name: 'description',
                   fieldLabel: '描述',
                   anchor: '100%',
                   value: seldata == null ? '' : seldata.description,
                   height:70,
                   padding: 5
               }
            ],
            buttonAlign: 'center',
            buttons: [{
                text: '数据提交',
                handler: function () {
                    var form = this.up('form').getForm();
                    var windows = this.up('window');
                    if (form.isValid()) {
                        form.submit({
                            url: '/service/projectsite/update_new_projectsite.ashx?params=' + Ext.application.params,
                            params: {
                                departmentid: departmentid,
                                taskid: seldata == null ? -1 : seldata.taskid,
                                operation: oper
                            },
                            success: function (fp, o) {
                                Ext.Msg.alert('提示', o.result.msg);
                                windows.close();
                                datastore.loadPage(datastore.currentPage);
                            },
                            failure: function (form, action) {
                                Ext.Msg.alert('提示', Ext.decode(action.response.responseText).msg);
                                windows.close();
                                datastore.loadPage(datastore.currentPage);
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
            }]
        }]
    }).show();
}
