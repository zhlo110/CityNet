function upload_point(panel,params,docliststore,taskid,schemeid) {
    
    panel.add(
    {
        xtype: 'panel',
        layout: 'border',
        width: 500,
        height: 300,
        items: [
        {
            xtype: 'gridpanel',
            title: '已上传的成果文件',
            region: 'north',
            height:500,
            collapsible: true,
            split: true,
            minWidth: 1,
            minHeight: 1,
            forceFit: true,
            store: docliststore,
            id: 'document_girdlist_id',
            viewConfig: {
                getRowClass: function (record, rowIndex, rowParams, store) {
                    return record.get("result") ? "x-grid-green" : "x-grid-red";
                }
            },
            listeners:
            {
                rowclick: function (panel, record, element, rowIndex, e, eOpts) {
                    //;
                    var treepanel = Ext.getCmp('read_to_upload_tree');
                    var treestore = new Ext.data.TreeStore(
                    {
                        autoSync: false,
                        proxy: {
                            type: 'ajax',
                            url: '/service/document/getTables_By_docid.ashx?params=' + params,
                            extraParams: {
                                docid: record.data.docid
                            },
                            reader: 'json'
                        },
                        root: {
                            name: '根节点',
                            id: 'project_id'
                        }
                    });
                    treepanel.setStore(treestore);

                }
            },
            columns: [
                 { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'docid' },
                 {
                     header: '文档名称', minWidth: 1, align: 'center',
                     sortable: false, menuDisabled: true, draggable: false, dataIndex: 'docname'
                 },
                 { header: '提交人', align: 'center', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'creator' },
                 { header: '提交人单位', align: 'center', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'department' },
                 { header: '页数', align: 'center', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'pages' },
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
            bbar: [{
                xtype: 'pagingtoolbar',
                displayInfo: true,
                store: docliststore,
                displayMsg: '显示的条目 {0} - {1} of {2}',
                emptyMsg: "没有下载项目"
            }],
            tbar: [
                {
                    xtype: 'button',
                    text: '文档上传',
                    iconCls: 'button add',
                    handler: function () {
                        uploadwindows('document_girdlist_id', params, taskid, 1,0);
                    }
                },
                {
                    xtype: 'button',
                    text: '删除附件',
                    iconCls: 'button delete',
                    handler: function () {
                        var gridpanel = this.up('gridpanel');
                        var seletionnode = gridpanel.getSelection()[0];
                        if (seletionnode != null) {
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
                                            var treepanel = Ext.getCmp('read_to_upload_tree');
                                            if (!Ext.isEmpty(treepanel.getStore())) {
                                                treepanel.getStore().load();
                                            }
                                            var workspace = Ext.getCmp('pick_workspace_id');
                                            if (!Ext.isEmpty(workspace)) {
                                                workspace.removeAll();
                                            }
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
            height: 160
        },
        {
            title: '待入库的表格列表',
            scrollable: 'y',
            xtype: 'treepanel',
            margin: '0 5 0 0',

            region: 'west',
            collapsible: true,
            split: true,
            width: 260,
            id: 'read_to_upload_tree',
            rootVisible: false,
            tbar: [
                {
                    xtype: 'button',
                    iconCls: 'button delete',
                    text: '删除选中的表',
                    handler: function () {
                        var panel = this.up('treepanel');
                        var list = panel.getChecked();
                        var ids = '';
                        var documentid = 0;
                        list.forEach(function (element) {
                            ids = ids + element.data.tableid + ',';
                            documentid = element.data.documentid;
                        });
                        if (ids.length > 0) {
                            ids = ids.substring(0, ids.length - 1);
                            Ext.Ajax.request({
                                url: '/service/document/set_table_deletesign.ashx?params=' + params,
                                params: {
                                    docid: documentid,
                                    tableids: ids
                                },
                                success: function (form, action) {
                                    var successjs = Ext.decode(form.responseText);
                                    Ext.MessageBox.alert("提示信息", successjs.msg);
                                    panel.getStore().load();
                                },
                                failure: function (form, action) {
                                    var errorjs = Ext.decode(form.responseText);
                                    Ext.MessageBox.alert("提示信息", errorjs.msg);
                                }
                            });

                        }
                        else {
                            Ext.MessageBox.alert("提示信息", '请选择要删除的表');
                        }
                    }
                },
                {
                    xtype: 'button',
                    iconCls: 'button refresh',
                    text: '恢复删除的表',
                    handler: function () {
                        var documentlistpanel = Ext.getCmp('document_girdlist_id');
                        var sel = documentlistpanel.selection;
                        var panel = this.up('treepanel');
                        if (Ext.isEmpty(sel)) {
                            Ext.MessageBox.alert("提示信息", '请在上方选择要恢复的文档');
                        }
                        else {
                            Ext.Ajax.request({
                                url: '/service/document/restore_doc_table.ashx?params=' + params,
                                params: {
                                    docid: sel.data.docid
                                },
                                success: function (form, action) {
                                    var successjs = Ext.decode(form.responseText);
                                    Ext.MessageBox.alert("提示信息", successjs.msg);
                                    panel.getStore().load();
                                },
                                failure: function (form, action) {
                                    var errorjs = Ext.decode(form.responseText);
                                    Ext.MessageBox.alert("提示信息", errorjs.msg);
                                }
                            });
                            //
                        }
                    }
                }
            ],
            listeners:
            {
                selectionchange: function (panel, record, opts) {
                    //
                    //获取解析的数据
                    var workspace = Ext.getCmp('pick_workspace_id');
                    workspace.removeAll();
                    if (record.length <= 0) return;

                    var datainstore = Ext.create('Ext.data.Store', {
                        model: 'Point',
                        pageSize: 8,
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

                    workspace.add({
                        xtype: 'panel',
                        height: 600,
                        width:400,
                        layout: 'border',
                        //   margin: '5 5 0 0',
                        items: [
                            {
                                xtype: 'panel',
                                title: '解析后的数据',
                                margin: '0 2 0 0',
                                id: 'show_parsetable_id',
                                minWidth: 1,
                                minHeight: 1,
                                scrollable: true,
                                region: 'center',
                            },
                            {
                                xtype: 'panel',
                                title: '原始数据',
                                margin: '0 0 0 2',
                                scrollable: true,
                                minWidth: 1,
                                minHeight: 1,
                                region: 'east',
                                width: workspace.getWidth()/2,
                                collapsible: true,
                                split: true,
                                id: 'origin_data_viewpanel'
                            },
                            {
                                margin: '5 0 0 0',
                                region: 'south',
                                height: 500,
                                minWidth: 1,
                                minHeight: 1,
                                collapsible: true,
                                split: true,
                                xtype: 'gridpanel',
                                id: 'has_in_database_table',
                                forceFit: true,
                                tbar: [
                                '',
                                {
                                    xtype: 'button',
                                    text: '全部清除',
                                    iconCls: 'button delete',
                                    handler: function () {
                                        var gridpanel = this.up('gridpanel');
                                        Ext.MessageBox.confirm("提示", "是否要全部清除上传的数据？", function (btnId) {
                                            if (btnId == "yes") {
                                                Ext.Ajax.request({
                                                    url: '/service/document/delete_allpoint_in_task.ashx?params=' + params,
                                                    params: {
                                                        taskid: taskid
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
                                },
                                '',
                                {
                                    xtype: 'textfield',
                                    id:'search_point_text_id',
                                    emptyText:'请输入要搜索的点名'
                                },
                                {
                                    xtype: 'button',
                                    text: '搜索',
                                    iconCls: 'button search',
                                    handler: function () {
                                        var textfield = Ext.getCmp('search_point_text_id');
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
                                    { header: '纬度', align: 'left', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'latitude' },
                                    {
                                        header: '删除本次上传数据', minWidth: 1, align: 'center',
                                        xtype: 'actioncolumn',
                                        items: [
                                        {
                                            iconCls: 'button delete',
                                            handler: function (grid, rowIndex, colIndex) {
                                                var rec = grid.getStore().getAt(rowIndex);
                                                var taskid = rec.get('taskid');
                                                var pointid = rec.get('id');
                                                var pointname = rec.get('pointname');

                                                Ext.MessageBox.confirm("提示", "是否要删除点'" + pointname + "'？", function (btnId) {
                                                    if (btnId == "yes") {
                                                        Ext.Ajax.request({
                                                            url: '/service/document/delete_point_in_task.ashx?params=' + params,
                                                            params: {
                                                                taskid: taskid,
                                                                pointid: pointid
                                                            },
                                                            success: function (form, action) {
                                                                var errorjson = Ext.decode(form.responseText);
                                                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                                gridloadcurrentPage(grid.getStore());
                                                            },
                                                            failure: function (form, action) {
                                                                var errorjson = Ext.decode(form.responseText);
                                                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                            }
                                                        });
                                                    }
                                                });


                                                // window.open('../Home/Document?params=' + params + '&docid=' + rec.get('docid'));
                                                //  alert("Edit " + rec.get('firstname'));
                                            }
                                        }]
                                    }
                                    //,
                                    //{ header: 'x坐标', align: 'center', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'x' },
                                    //{ header: 'y坐标', align: 'center', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'y' },
                                   // { header: 'z坐标', align: 'center', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'z' },
                                   // { header: 'L0', align: 'center', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'l0' },
                                   // { header: 'h', align: 'center', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'h' },

                                   // { header: '共桩情况', align: 'center', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'sharedes' },
                                   // { header: '备注', align: 'center', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'description' }
                                ],
                                store: datainstore,
                                title: '本次上传的数据',
                                height: 260,

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
                                                    pointid: record.id,
                                                    taskid: taskid
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
                                            height: 140,
                                            id: 'point_detail_tree_inner_' + record.id,
                                            scrollable: true,
                                            forceFit:true,
                                            plugins: [{
                                                ptype: 'bufferedrenderer',
                                                trailingBufferZone: 20,  // Keep 20 rows rendered in the table behind scroll
                                                leadingBufferZone: 50   // Keep 50 rows rendered in the table ahead of scroll
                                            }],
                                            columns: [
                                                { xtype: 'treecolumn', flex: 1, dataIndex: 'text', text: '方案名称' },
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
                            }
                        ]
                    });

                    workspace.addListener('resize', function (panel, width, height, oldWidth, oldHeight, eOpts) {
                        var panel1 = Ext.getCmp('origin_data_viewpanel');
                        if (!Ext.isEmpty(panel1)) {
                            panel1.setWidth(width / 2);
                        }

                    });

                    parseDocData(record[0].data.tableid, record[0].data.documentid, params, taskid, schemeid);

                    //获取原始数据
                    Ext.Ajax.request({
                        url: '/service/document/get_document_detail.ashx?params=' + params,
                        params: {
                            docid: record[0].data.tableid
                        },
                        success: function (response) {
                            var oripanel = Ext.getCmp('origin_data_viewpanel');
                            if (!Ext.isEmpty(oripanel)) {
                                if (!Ext.isEmpty(oripanel.body)) {
                                    oripanel.body.update(response.responseText);
                                }
                            }
                        }
                    });
                }
            },
            plugins: [{
                ptype: 'bufferedrenderer', trailingBufferZone: 20, leadingBufferZone: 50
            }]
        },
        {
            xtype: 'panel',
            id: 'pick_workspace_id',
            region: 'center',
            height: 600,
            layout: 'fit',
            columnWidth: 0.75
        }]
    });
}