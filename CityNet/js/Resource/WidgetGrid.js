

//var sm = new Ext.grid.CheckboxSelectionModel({ handleMouseDown: Ext.emptyFn });
Ext.define('resource.WidgetGrid',
{
    extend: 'Ext.grid.Panel',
    alias: ['widget.widgetgrid'],
    forceFit: true,
    selModel: {
        checkOnly: true,
        mode: 'SINGLE',
        selType: 'checkboxmodel'
    },
    bodyPadding: 5,
 //   viewConfig: {
   //     getRowClass: function (record, index)
    //    {
        //    return "height";
     //   }
  //  },
    columns: [
        { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'taskid' },
        {
            header: '任务名', minWidth: 1,align:'center',renderer:rendercell,
            sortable: false, menuDisabled: true, draggable: false, dataIndex: 'taskname'
        },
        {
            header: '任务创建时间', minWidth: 1, align: 'center', renderer: rendercell,
            sortable: false, menuDisabled: true, draggable: false, dataIndex: 'taskcreatedate'
        },
        { header: '提交人', align: 'center', renderer: rendercell, minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'creator' },
        { header: '提交人单位', align: 'center', renderer: rendercell, minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'creatordep' },
        {
            header: '审核人', minWidth: 1, align: 'center', renderer: rendercell,
            sortable: false, menuDisabled: true, draggable: false, dataIndex: 'auditor'
        },
        {
            header: '审核人单位', minWidth: 1, align: 'center', renderer: rendercell,
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
            dataIndex:'buttonName',
            widget: {
                width: 100,
                iconCls: 'button detail',
                xtype: 'button',
                handler: function (a, b) {
                    var panel = this.up('widgetgrid');
                    var params = panel.params;
                    var mode = panel.mode;
                    var workplace = Ext.getCmp('index_workplace_id');
                    var userjson = panel.userjson;
                    var taskid = a.$widgetRecord.data.taskid;
                    workplace.removeAll();
                    viewtask_detail(workplace, params, userjson, taskid,mode);

                }
            }
        }
    ]
   
});