function pointview(usermsg, managerParams, pointid)
{
    var params = managerParams;
    Ext.QuickTips.init();
    var treestore = new Ext.data.TreeStore(
    {
        autoSync: false,
        proxy: {
            type: 'ajax',
            url: '/service/document/getpoints_tree_by_pointid_withouttaskid.ashx?params=' + params,
            extraParams: {
                pointid: pointid
            },
            reader: 'json'
        },
        root: {
            name: '根节点',
            id: 'project_id'
        }
    });

   var taskstore = new Ext.data.TreeStore(
   {
       autoSync: false,
       fields: ['text', 'taskid', 'creator', 'datetime', 'department'],
       proxy: {
           type: 'ajax',
           url: '/service/point/gettask_bypointid.ashx?params=' + params,
           extraParams: {
               pointid: pointid
           },
           reader: 'json'
       },
       root: {
           name: '根节点',
           id: 'project_id'
       }
   });
  var viewport = Ext.create('Ext.container.Viewport',
  {
      layout: 'border',
      items: [
      {
          xtype: 'panel',
          html: '<table width="100%" height = "100%"><tr><td height="100%"><img src="../Images/topbar.png" /></td></tr></table>',
          height: 185,
          region: 'north',
          dockedItems: [
              {
                  xtype: 'toolbar',
                  id: 'maintoolbarid',
                  dock: 'bottom',
                  align: 'middle',
                  height: 35,
                  items: [
                      '->',
                   {
                       xtype: 'panel',
                       ui: 'default-toolbar',
                       items: [
                           { xtype: 'label', style: "position:relative;top:1px;", padding: 0, text: usermsg },
                       ]
                   }
                  ]
              }]
      },
      {
          xtype: 'panel',
          region: 'center',
          layout: 'fit',
          items: [{
          xtype: 'panel',
          layout: 'border',
          listeners:{
              resize: function (panel, width, height, oldWidth, oldHeight, eOpts) {
                  var dataarea = Ext.getCmp('data_area_id');
                  dataarea.setWidth(width / 2);
                  var taskarea = Ext.getCmp('point_task_list_area');
                  taskarea.setWidth(width / 2);
              }
          },
          items: [ {
              // xtype: 'panel' implied by default
              title: '数据区',
              region: 'west',
              id:'data_area_id',
              xtype: 'treepanel',
              margin: '5 0 0 5',
              scrollable:true,
              width: 200,
              minWidth: 1,
              minHeight:1,
              split: true,
              store: treestore,
              forceFit: true,
              rootVisible: false,
              dockedItems: [{
                  xtype: 'toolbar',
                  dock: 'top',
                  items: [
                      {
                          iconCls: 'button database',
                          xtype: 'authoritybutton',
                          params: params,
                          id: 'delete_alarm_by_hand_id',
                          text: '消除警报',
                          handler: function () {
                              var treepanel = this.up('treepanel');
                              var treenode = treepanel.getSelection()[0];
                              if (!Ext.isEmpty(treenode))//
                              {
                                  if (treenode.data.alarmed == 1) {
                                      var measureid = treenode.data.measureid;
                                      var rel = treenode.data.rel;
                                      delete_alarm_windows(measureid, rel, params);
                                  }
                                  else {
                                      Ext.MessageBox.alert("提示信息", '节点没有报警，无法消警');
                                  }
                              }
                              else {
                                  Ext.MessageBox.alert("提示信息", '请先选择要消警的节点');
                              }
                          }
                      }
                  ]
              }],
              plugins: [{
                  ptype: 'bufferedrenderer',
                  trailingBufferZone: 20,  // Keep 20 rows rendered in the table behind scroll
                  leadingBufferZone: 50   // Keep 50 rows rendered in the table ahead of scroll
              }]
          }, {
              title: '点之记',
              region: 'center',     // center region is required, no width/height specified
              xtype: 'panel',
              layout: 'fit',
              id:'point_sign_html_id',
              scrollable: true,
              minWidth: 1,
              minHeight: 1,
              margin: '5 5 0 0'
          },
          {
              title: '关联任务',
              region: 'south',     // position for region
              xtype: 'panel',
              height: 300,
              minWidth: 1,
              minHeight: 1,
              layout: 'border',
              split: true,         // enable resizing
              collapsible: true,
              margin: '0 5 5 5',
              items: [{
                  xtype: 'treepanel',
                  region: 'west',
                  title: '任务列表',
                  scrollable: true,
                  split: true,
                  store: taskstore,
                  width: 200,
                  forceFit: true,
                  rootVisible: false,
                  id:'point_task_list_area',
                  minWidth: 1,
                  minHeight: 1,
                  listeners: {
                      rowclick: function (panel, record, element, rowIndex, e, eOpts) {
                          if (Ext.isEmpty(record.data.taskid)) return;

                          var gridpanel = Ext.getCmp('point_task_document_list_id');
                          if (Ext.isEmpty(gridpanel.store.proxy.url)) {
                              var datastore = Ext.create('Ext.data.Store', {
                                  model: 'Document',
                                  pageSize: 8,
                                  proxy: {
                                      type: 'ajax',
                                      url: '/service/document/getdocumentlistbytid.ashx?params=' + params,
                                      extraParams: {
                                          taskid: record.data.taskid
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
                              var pagebar = Ext.getCmp('point_task_document_list_page_bar');
                              pagebar.setStore(datastore);
                              gridpanel.setStore(datastore);
                          }
                          else {
                              var store = gridpanel.getStore();
                              gridpanel.store.proxy.extraParams.taskid = record.data.taskid;
                              // store.proxy.url = '/service/document/getdocumentlistbytid.ashx?params=' + params + '&taskid=' + record.data.taskid;
                              store.loadPage(1);
                          }

                          //record.data.id;
                      }

                  },
                  columns: [{
                  xtype: 'treecolumn',
                  dataIndex: 'text',
                  text: '名称'
                  },
                  {
                      xtype: 'gridcolumn',
                      dataIndex: 'taskid',
                      hidden:true,
                      text: '任务ID'
                  }, {
                      xtype: 'gridcolumn',
                      dataIndex: 'creator',
                      text: '创建人'
                  }, {
                      xtype: 'gridcolumn',
                      dataIndex: 'department',
                      text: '创建人部门'
                  }, {
                      xtype: 'gridcolumn',
                      dataIndex: 'datetime',
                      text: '创建时间'
                  }
                  ]

              }, {
                  xtype: 'gridpanel',
                  region: 'center',
                  id:'point_task_document_list_id',
                  title: '文档列表',
                  forceFit: true,
              //    store: docstore,
                  minWidth: 1,
                  minHeight: 1,
                  bbar: Ext.create('Ext.PagingToolbar', {
                      displayInfo: true,
                      id: 'point_task_document_list_page_bar',
                      displayMsg: '显示的条目 {0} - {1} of {2}',
                      emptyMsg: "没有下载项目"
                  }),
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
                  ]
              }]
          }
          ]
       }]}
      ]
  });

    //fields: ['text', 'taskid', 'creator', 'datetime', 'department'],
  //点之记
  Ext.Ajax.request({
      url: '/service/point/get_html_bypointid.ashx?params=' + params,
      params: {
          pointid: pointid
      },
      success: function (response) {
          var htmlpanel = Ext.getCmp('point_sign_html_id');
          if (!Ext.isEmpty(htmlpanel)) {
              if (!Ext.isEmpty(htmlpanel.body)) {
                  htmlpanel.body.update('<center>' + response.responseText + '</center>');
              }
          }
      }
  });

}


function delete_alarm_windows(measureid,rel,params) {
    var panel = Ext.getCmp('delete_alarm_windows_id');
    if (!Ext.isEmpty(panel)) return;
    Ext.create('Ext.window.Window',
    {
        id: 'delete_alarm_windows_id',
        title: '消除警报',
        height: 300,
        width: 400,
        layout: 'fit',
        items: [{
            xtype: 'form',
            width: '100%',
            height: '100%',
            layout: {
                align: 'middle',
                pack: 'center'
            },
            defaults:
            {
                padding: 5
            },
            items:
                [{
                    xtype: 'label',
                    text:'消警说明：'
                },
                {
                    xtype: 'htmleditor',
                    id: 'comment_html_editor_id'
                }],
            buttonAlign: 'center',
            buttons: [{
                text: '提交',
                handler: function () {
                    var form = this.up('form').getForm();
                    var windows = this.up('window');
                    var htmleditor = Ext.getCmp('comment_html_editor_id');
                    var htmlvalue = Ext.String.htmlEncode(htmleditor.value);
                    if (htmlvalue.length <= 0) {
                        Ext.Msg.alert('提示', "消警说明不能为空");
                        return;
                    }
                    if (form.isValid()) {
                        form.submit({
                            url: '/service/alarm/upload_delete_alarm_info.ashx?params=' + params,
                            waitMsg: '正在提交数据',
                            params: {
                                htmlvalue: htmlvalue,
                                measureid: measureid,
                                rel: rel
                            },
                            success: function (fp, o) {
                                //显示浏览数据按钮
                                //从文档中读取表格并存储
                                Ext.Msg.alert('提示', o.result.msg);
                                var treepanel = Ext.getCmp('data_area_id');
                                treepanel.getStore().load();
                                windows.close();
                            }
                        });
                    }
                }
            }]
        }]
    }).show();
}