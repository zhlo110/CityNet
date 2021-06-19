function showrulerpanel(params) {
    var workplace = Ext.getCmp('index_workplace_id');
    workplace.removeAll();
    workplace.clearListeners();
    var datastore = Ext.create('Ext.data.Store', {
        model: 'Document',
        pageSize: 15,
        proxy: {
            type: 'ajax',
            url: '/service/document/getdocumentlist_notintask.ashx?params=' + params,
            extraParams: {
                notintask: 1
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
    workplace.add(
        {
            xtype: 'gridpanel',
            title: '技术规范&作业细则',
            region: 'north',
            height: 500,
            collapsible: true,
            split: true,
            minWidth: 1,
            minHeight: 1,
            forceFit: true,
            store: datastore,
            id: 'document_guide_document_list_id',
            columns: [
                 { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'docid' },
                 {
                     header: '文档名称', minWidth: 1, align: 'center', renderer: rendercell,
                     sortable: false, menuDisabled: true, draggable: false, dataIndex: 'docname'
                 },
                 {
                     header: '提交人', align: 'center', minWidth: 1, sortable: false, menuDisabled: true,
                     renderer: rendercell,
                     draggable: false,
                     dataIndex: 'creator'
                 },
                 {
                     header: '提交人单位', align: 'center', minWidth: 1,
                     renderer: rendercell,
                     sortable: false, menuDisabled: true, draggable: false, dataIndex: 'department'
                 },
                 {
                     header: '页数', align: 'center', minWidth: 1,
                     renderer: rendercell,
                     sortable: false, menuDisabled: true, draggable: false, dataIndex: 'pages'
                 },
                 {
                     header: '查看文档', minWidth: 1, align: 'center', renderer: rendercell,
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
                     xtype: 'actioncolumn', renderer: rendercell,
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
            tbar: ['',
            {
                xtype: 'textfield',
                id: 'search_document_text_id',
                emptyText: '请输入要搜索的文档'
            },
            {
                xtype: 'button',
                text: '搜索',
                iconCls: 'button search',
                handler: function () {
                    var textfield = Ext.getCmp('search_document_text_id');
                    var gridpanel = this.up('gridpanel');
                    var store = gridpanel.getStore();
                    store.proxy.url = '/service/document/getdocumentlist_notintask.ashx?params=' + params + '&searchtext=' + textfield.value,
                    store.loadPage(1);
                }
            }],
            bbar: [{
                xtype: 'pagingtoolbar',
                displayInfo: true,
                store: datastore,
                displayMsg: '显示的条目 {0} - {1} of {2}',
                emptyMsg: "没有下载项目"
            }]

        }
    );
}

function approve_guide_upload(params) {
    var workplace = Ext.getCmp('index_workplace_id');
    workplace.removeAll();
    workplace.clearListeners();
    var datastore = Ext.create('Ext.data.Store', {
        model: 'Document',
        pageSize: 15,
        proxy: {
            type: 'ajax',
            url: '/service/document/getdocumentlist_notintask.ashx?params=' + params,
            extraParams: {
                notintask: 1
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

    workplace.add(
        {
            xtype: 'gridpanel',
            title: '技术规范&作业细则',
            region: 'north',
            height:500,
            collapsible: true,
            split: true,
            minWidth: 1,
            minHeight: 1,
            forceFit: true,
            store: datastore,
            id: 'document_guide_document_list_id',
            columns: [
                 { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'docid' },
                 {
                     header: '文档名称', minWidth: 1, align: 'center', renderer: rendercell,
                     sortable: false, menuDisabled: true, draggable: false, dataIndex: 'docname'
                 },
                 {
                     header: '提交人', align: 'center', minWidth: 1, sortable: false, menuDisabled: true,
                     renderer: rendercell,
                     draggable: false,
                     dataIndex: 'creator'
                 },
                 {
                     header: '提交人单位', align: 'center', minWidth: 1,
                     renderer: rendercell,
                     sortable: false, menuDisabled: true, draggable: false, dataIndex: 'department'
                 },
                 {
                     header: '页数', align: 'center', minWidth: 1,
                     renderer: rendercell,
                     sortable: false, menuDisabled: true, draggable: false, dataIndex: 'pages'
                 },
                 {
                     header: '查看文档', minWidth: 1, align: 'center', renderer: rendercell,
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
                     xtype: 'actioncolumn', renderer: rendercell,
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
                store: datastore,
                displayMsg: '显示的条目 {0} - {1} of {2}',
                emptyMsg: "没有下载项目"
            }],
            tbar: [
                {
                    xtype: 'button',
                    text: '文档上传',
                    iconCls: 'button add',
                    handler: function () {
                        upload_guidefile_windows('document_guide_document_list_id',params,1);
                    }
                },
                {
                    xtype: 'button',
                    text: '删除文档',
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
                '',
                {
                    xtype: 'textfield',
                    id: 'search_document_text_id',
                    emptyText: '请输入要搜索的文档'
                },
                {
                    xtype: 'button',
                    text: '搜索',
                    iconCls: 'button search',
                    handler: function () {
                        var textfield = Ext.getCmp('search_document_text_id');
                        var gridpanel = this.up('gridpanel');
                        var store = gridpanel.getStore();
                        store.proxy.url = '/service/document/getdocumentlist_notintask.ashx?params=' + params + '&searchtext=' + textfield.value,
                        store.loadPage(1);
                    }
                }


            ]


        }
    );

}



function upload_guidefile_windows(gridid, params,isnottask) {
    var panel = Ext.getCmp('upload_guide_file_windows_id');
    if (!Ext.isEmpty(panel)) return;
    Ext.create('Ext.window.Window',
    {
        id: 'upload_guide_file_windows_id',
        title: '数据上传面板',
        height: 170,
        width: 400,
        modal:true,
        closable:false,
        layout: 'fit',
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
                        xtype: 'filefield',
                        name: 'file',
                        fieldLabel: '规范&细则',
                        buttonText: '选择文件',
                        anchor: '100%',
                        msgTarget: 'under',
                        padding: 5,
                        allowBlank: false,
                        vtype: 'docordocx',
                        vtypeText: '必须为doc,docx或pdf文件',
                        blankText: '上传文档不能为空',
                        emptyText: '上传文档不能为空'
                    },
                    {
                        xtype: 'backprogress',
                        anchor: '100%',
                        height:'24px',
                        hidden:true,
                        id:'upload_guide_document_progressbar_id',
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
                        var progressbar = Ext.getCmp('upload_guide_document_progressbar_id');
                        progressbar.createbar('正在上传文档信息', 1000, '', function (progressid){
                            progressbar.start();
                            form.submit({
                                url: '/service/document/upload_document.ashx?params=' + params,
                                params: {
                                    progressid:progressid,
                                    notintask: isnottask
                                },
                                success: function (fp, o) {
                                    //显示浏览数据按钮
                                    //从文档中读取表格并存储
                                    var gridpanel = Ext.getCmp(gridid);
                                    var store = gridpanel.getStore();
                                    store.loadPage(store.currentPage);
                                    Ext.Msg.alert('提示', o.result.msg);
                                    progressbar.hide();
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
            }]
        }]
    }).show();
}