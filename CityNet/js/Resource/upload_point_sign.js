var pointsignmap = null;
var signlayer = [];
function uploadpointsign(parentpanel, params, userjson,viewmode) {
    parentpanel.removeAll();
    var taskid = parentpanel.up().taskid;
    var docstore = gettaskducumentstore(params, taskid);
    var pointstore = gettaskpointstore(params, taskid);
    if (!Ext.isEmpty(Ext.getCmp('move-prev'))) {
        Ext.getCmp('move-prev').setDisabled(true);
    }
    if (!Ext.isEmpty(Ext.getCmp('move-next'))) {
        Ext.getCmp('move-next').setDisabled(true);//等待地图加载完才显示
    }

    parentpanel.add({
        xtype: 'panel',
        layout: 'border',
        width: 500,
        height: 300,
        items: [
        {
            region: 'west',
            collapsible: true,
            split: true,
            xtype: 'gridpanel',
            id:'step2_sign_point_list',
            title: '本次上传点列表',
            forceFit: true,
            store: pointstore,
            viewConfig: {
                getRowClass: function (record, rowIndex, rowParams, store) {
                    if (Ext.isEmpty(record.get("longitude")) || Ext.isEmpty(record.get("latitude")) || Ext.isEmpty(record.get("sign")))
                        return  'x-grid-red';
                    else
                        return 'x-grid-green';
                }
            },
            columns: [
            { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'pointid' },
            {
                header: '点名', minWidth: 1, align: 'center',
                sortable: false, menuDisabled: true, draggable: false, dataIndex: 'pointname'
            },
            {
                header: '添加/更改点之记', minWidth: 1, align: 'center',
                xtype: 'taskaction',
                taskid: taskid,
                params: params,
                viewmode:viewmode,
                items: [
                {
                    iconCls: 'button add',
                    handler: function (grid, rowIndex, colIndex) {
                        var rec = grid.getStore().getAt(rowIndex);
                        uploadsinglesign('step2_sign_point_list',rec, taskid, params);
                    }
                }]
            }],
            listeners: {
                rowclick: function (grid, record, element, rowIndex, e, eOpts) {
                    clickgridrow(record,params);
                    //  alert(record.data.id);
                }
            },
            tbar: [{
                text: '批量添加点之记',
                xtype: 'taskbutton',
                taskid: taskid,
                params: params,
                viewmode: viewmode,
                iconCls: 'button add',
                handler: function () {
                    uploadbatchsign('step2_sign_point_list', taskid, params)
                }
            }, '',
            {
                xtype: 'textfield',
                id:'search_point_sign_text_id',
                emptyText:'请输入要搜索的点名'
            },
            {
                xtype: 'button',
                text: '搜索',
                iconCls: 'button search',
                handler: function () {
                    var textfield = Ext.getCmp('search_point_sign_text_id');
                    var cardpanel = Ext.getCmp('card_navigation_id');
                    var gridpanel = this.up('gridpanel');
                    var store = gridpanel.getStore();
                    store.proxy.url = '/service/point/get_point_by_taskid.ashx?params=' + params+'&searchtext='+textfield.value,
                    store.loadPage(1);
                }
            }],
            bbar: Ext.create('Ext.PagingToolbar', {
                displayInfo: true,
                store: pointstore,
                displayMsg: '显示的条目 {0} - {1} of {2}',
                emptyMsg: "没有下载项目"
            }),
            width: 340
            // could use a TreePanel or AccordionLayout for navigational items
        }, {
            region: 'south',
            title: '文档列表',
            id: 'pointsign_girdlist_id',
            xtype: 'gridpanel',
            collapsible: true,
            split: true,
            store:docstore,
            height: 200,
            forceFit: true,
            viewConfig: {
                getRowClass: function (record, rowIndex, rowParams, store) {
                    return record.get("result") ? "x-grid-red" : "x-grid-green";
                }
            },
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
            ],

            tbar: [
                   {
                       
                       text: '附件上传',
                       xtype: 'taskbutton',
                       taskid: taskid,
                       params: params,
                       viewmode: viewmode,
                       iconCls: 'button add',
                       handler: function () {
                           var panel = Ext.getCmp('card_navigation_id');
                           //    alert(panel.taskid);
                           uploadwindows('pointsign_girdlist_id', params, panel.taskid, 0,0);
                       }
                   },
                   {
                       text: '删除文档',
                       xtype: 'taskbutton',
                       taskid: taskid,
                       params: params,
                       viewmode: viewmode,
                       iconCls: 'button delete',
                       handler: function () {
                           var gridpanel = this.up('gridpanel');
                           var seletionnode = gridpanel.getSelection()[0];
                           if (seletionnode.data.result == 1) {
                               Ext.MessageBox.alert("提示信息", '文档为成果数据，该步骤无法删除');
                           }
                           else if (seletionnode != null) {
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
                   }
            ],

            bbar: Ext.create('Ext.PagingToolbar', {
                displayInfo: true,
                store:docstore,
                displayMsg: '显示的条目 {0} - {1} of {2}',
                emptyMsg: "没有下载项目"
            }),
            minHeight: 20
        },
        {
            region: 'east',
            id:'point_sign_html_id',
            title: '点之记面板',
            xtype: 'panel',
            collapsible: true,
            split: true,
            scrollable:true,
            width: 240,
            minWidth:1
        },
        {
            region: 'center',
            title: '地图展示',
            xtype: 'panel',
            x: 0,
            y: 0,
            listeners: {
                afterrender: function (panel, eOpts) {     
                    createmap2(params,taskid);
                    addpointtomap(taskid, params);
                },
                resize: function (panel, width, height, oldWidth, oldHeight, eOpts) {

                //    var smappanel = Ext.getCmp('task_show_map_panel_id');
                 //   smappanel.setHeight((height - 47) > 1 ? (height - 47) : 1);

                    if (pointsignmap != null) {
                        pointsignmap.invalidateSize(true);
                    }
                }
            },
            id: 'task_show_map_panel_id',
            html: '<div id="taskshowmapid" style="height:100%;width:100%;"></div>'
        }]
    });
    
    pointsignmap = null;
}

function gettaskducumentstore(params, taskid) {
    var datastore = Ext.create('Ext.data.Store', {
        model: 'Document',
        pageSize: 4,
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

//批量上传
function uploadbatchsign(gridid, taskid, params) {
  
    var panel = Ext.getCmp('upload_batch_pointsign_windows_id');
    if (!Ext.isEmpty(panel)) return;
    Ext.create('Ext.window.Window',
    {
        id: 'upload_batch_pointsign_windows_id',
        title: '点之记批量上传面板',
        height: 160,
        width: 400,
        layout: 'fit',
        modal:true,
        closable:false,
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
                        fieldLabel: '点之记文件',
                        buttonText: '选择文件',
                        anchor: '100%',
                        msgTarget: 'under',
                        padding: 5,
                        allowBlank: false,
                        vtype: 'docordocx',
                        vtypeText: '必须为doc或docx文件',
                        blankText: '点之记文件不能为空',
                        emptyText: '点之记文件不能为空'
                    },
                    {
                        xtype: 'backprogress',
                        anchor: '100%',
                        height:'24px',
                        hidden:true,
                        id:'upload_sign_document_progressbar_id',
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
                        var progressbar = Ext.getCmp('upload_sign_document_progressbar_id');
                        progressbar.createbar('正在上传文档信息', 1000, '', function (progressid){
                            progressbar.start();
                            form.submit({
                                url: '/service/point/upload_batch_sign.ashx?params=' + params,
                                params: {
                                    progressid:progressid,
                                    taskid: taskid
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
                                    //重新加载地图数据
                                    removepoiintfrommap();
                                    addpointtomap(taskid, params);
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


function uploadsinglesign(gridid, rec, taskid, params) {
    var panel = Ext.getCmp('upload_single_pointsign_windows_id');
    if (!Ext.isEmpty(panel)) return;
    Ext.create('Ext.window.Window',
    {
        id: 'upload_single_pointsign_windows_id',
        title: '点之记上传面板',
        height: 260,
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
                        xtype: 'hiddenfield',
                        name: 'pointid',
                        value: rec.data.pointid
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '点名',
                        anchor: '100%',
                        msgTarget: 'under',
                        name: 'pointname',
                        editable:false,
                        allowBlank: false,
                        value: rec.data.pointname,
                        padding: 5,
                        blankText: '请输入表名在表格上方第几行，必须为数字',
                        emptyText: '请输入表名在表格上方第几行，必须为数字'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '经度',
                        name: 'longitude',
                        anchor: '100%',
                        padding: 5,
                        max: 180.0,
                        min:-180.0,
                        msgTarget: 'under',
                        allowBlank: false,
                        value: rec.data.longitude,
                        vtype: 'degree',
                        vtypeText: '经度为度分秒格式（如30°13′26.85″）或小数（如30.133），范围在[-180.0,180.0]',
                        blankText: '请输入经度',
                        emptyText: '请输入经度'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '纬度',
                        name: 'latitude',
                        anchor: '100%',
                        max: 90.0,
                        min: -90.0,
                        padding: 5,
                        msgTarget: 'under',
                        vtype: 'degree',
                        vtypeText: '纬度为度分秒格式（如30°13′26.85″）或小数（如30.133），范围在[-90.0,90.0]',
                        allowBlank: false,
                        value: rec.data.latitude,
                        blankText: '请输入纬度',
                        emptyText: '请输入纬度'
                    },
                    {
                        xtype: 'filefield',
                        name: 'file',
                        fieldLabel: '点之记文件',
                        buttonText: '选择文件',
                        anchor: '100%',
                        msgTarget: 'under',
                        allowBlank: false,
                        padding: 5,
                        vtype: 'docordocx',
                        vtypeText: '必须为doc或docx文件',
                        blankText: '点之记文件不能为空',
                        emptyText: '点之记文件不能为空'
                    },
                    {
                        xtype: 'backprogress',
                        anchor: '100%',
                        height:'24px',
                        hidden:true,
                        id:'upload_single_sign_progressbar_id',
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
                        var progressbar = Ext.getCmp('upload_single_sign_progressbar_id');
                        progressbar.createbar('正在上传文档信息', 1000, '', function (progressid){
                            progressbar.start();
                            form.submit({
                                url: '/service/point/upload_single_sign.ashx?params=' + params,
                                params: {
                                    progressid:progressid,
                                    taskid: taskid
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
                                    //重新加载地图数据
                                    removepoiintfrommap();
                                    addpointtomap(taskid, params);
                                    clickgridrow(rec,params);
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


function gettaskpointstore(params, taskid) {
    var datastore = Ext.create('Ext.data.Store', {
        model: 'PointSign',
        pageSize: 20,
        proxy: {
            type: 'ajax',
            url: '/service/point/get_point_by_taskid.ashx?params=' + params,
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
function clickgridrow(record,params)
{
    if (pointsignmap!=null&&!Ext.isEmpty(record.data.latitude) && !Ext.isEmpty(record.data.longitude)) {
        pointsignmap.setView([record.data.latitude, record.data.longitude], 17);

        Ext.Ajax.request({
            url: '/service/point/get_html_bypointid.ashx?params=' + params,
            params: {
                pointid: record.data.pointid
            },
            success: function (response) {
                var htmlpanel = Ext.getCmp('point_sign_html_id');
                if (!Ext.isEmpty(htmlpanel)) {
                    if (!Ext.isEmpty(htmlpanel.body)) {
                        htmlpanel.body.update('<center>'+response.responseText+'</center>');
                    }
                }
            }
        });


    }
}

function createmap2(params, taskid) {
   
    Ext.Ajax.request({
        url: '/service/point/getscheme_legendbytaskid.ashx?params=' + params,
        params: {
            taskid: taskid
        },
        success: function (response) {
            var json = Ext.decode(response.responseText);
            var Google = L.tileLayer.chinaProvider('Google.Normal.Map', {
                maxZoom: 18,
                minZoom: 5
            });
            var GoogleAnn = L.tileLayer.chinaProvider('Google.Normal.Annotion', {
                maxZoom: 18,
                minZoom: 5
            });
            var Googleimgem = L.tileLayer.chinaProvider('Google.Satellite.Map', {
                maxZoom: 18,
                minZoom: 5
            });
            var Googleimga = L.tileLayer.chinaProvider('Google.Satellite.Annotion', {
                maxZoom: 18,
                minZoom: 5
            });
            var Googleimage = L.layerGroup([Googleimgem, Googleimga]);
            var Googlest = L.layerGroup([Google, GoogleAnn]);

            var baseLayers = {
                "标准地图": Googlest,
                "影像地图": Googleimage
            }
            var maplayers = Googlest;
            var lat = 30.62351686255914;
            var lng = 103.98707171672822;
            var zoom = 17;
            if (pointsignmap != null) {
                lat = pointsignmap.getCenter().lat;
                lng = pointsignmap.getCenter().lng;
                zoom = pointsignmap.getZoom();
                var layercount = 0;
                pointsignmap.eachLayer(function (layer) {
                    if (layercount < layer.options.maxZoom) {
                        layercount = layer.options.maxZoom;
                    }
                    // layercount = layercount + 1;
                });
                if (layercount > 18) {
                    maplayers = Googleimage;
                }
            }

            pointsignmap = L.map("taskshowmapid", {
                center: [lat, lng],
                zoom: zoom,
                layers: [maplayers],
                zoomControl: false,
                trackResize: true,
                attributionControl: false
            });
            L.control.layers(baseLayers, null).addTo(pointsignmap);
            L.control.zoom({
                zoomInTitle: '放大',
                zoomOutTitle: '缩小'
            }).addTo(pointsignmap);
            pointsignmap.invalidateSize(true);


            var legend = L.control.legend({ position: 'bottomright',json:json });
            //添加图例
            legend.addTo(pointsignmap);
        }
    });


   

}

function pointtosign(e, params) {
    Ext.Ajax.request({
        url: '/service/point/get_html_bypointid.ashx?params=' + params,
        params: {
            pointid: e.target.options.pointid
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
function removepoiintfrommap() {
    if (pointsignmap != null) {
        var i = 0;
        for(i = 0; i <signlayer.length;i++)
        {
            var layer = signlayer[i];
            layer.removeFrom(pointsignmap);
        }
        layer = [];
    }
}

function addpointtomap(taskid, params) {
   /*
    var myIcon = L.divIcon({
        html: 'aaaa',
        className: 'leaflet-div-icon',
        iconSize: 30
    });
    L.marker([30.62351686255914, 103.98707171672822], { icon: myIcon }).addTo(pointsignmap);*/
   
    Ext.Ajax.request({
        url: '/service/point/get_geojson_bytaskid.ashx?params=' + params,
        params: {
            taskid: taskid
        },
        success: function (form, action) {
            var geo = Ext.decode(form.responseText);
            if (pointsignmap != null) {
                L.geoJSON(geo, {
                    pointToLayer: function (feature, latlng) {
                        var greenIcon = L.icon({
                            iconUrl: feature.properties.colors,
                            iconSize: [20, 20], // size of the icon
                        });
                        var sign = L.marker(latlng, { opacity: 0.8, icon: greenIcon, pointid: feature.properties.pointid });
                        sign.on({
                            click: function (e) {
                                pointtosign(e, params);
                            }
                        });
                        signlayer.push(sign);
                        return sign;
                    },
                    onEachFeature: function (feature, layer) {
                        var myIcon = L.divIcon({
                            html: '<br\>' + feature.properties.name,
                            className: 'maplabel'
                        });
                        var maker = L.marker([feature.geometry.coordinates[1], feature.geometry.coordinates[0]], { icon: myIcon, pointid: feature.properties.pointid }).addTo(pointsignmap);
                        maker.on({
                            click: function (e) {
                                pointtosign(e, params);
                            }
                        });
                        signlayer.push(maker);
                    }
                }).addTo(pointsignmap);
            }

            if (!Ext.isEmpty(Ext.getCmp('move-prev'))) {
                Ext.getCmp('move-prev').setDisabled(false);
            }
            if (!Ext.isEmpty(Ext.getCmp('move-next'))) {
                Ext.getCmp('move-next').setDisabled(false);//等待地图加载完才显示
            }

        }
    });
}

L.Control.Legend = L.Control.extend({
    options: {
        position: 'topright' //初始位置
    },
    initialize: function (options) {
        L.Util.extend(this.options, options);

    },
    onAdd: function (map) {
        //创建一个class为info legend的div
        this._container = L.DomUtil.create('div', 'info legend');
        var le;
        var labels = [];
        //创建一个图片要素
        for (var i = 0; i < this.options.json.length; i++) {
            le = this.options.json[i];
            labels.push(
                '<div class="color" style="font-weight:bold;background:' + le.color + ';"> </div><i></i> ' +
                le.name);
        }
        this._container.innerHTML = labels.join('<br>');
        return this._container;
    },

    onRemove: function (map) {
        // Nothing to do here
    }
});
L.control.legend = function (opts) {
    return new L.Control.Legend(opts);
}
