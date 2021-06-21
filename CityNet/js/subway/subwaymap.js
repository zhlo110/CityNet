
var map = null;

//监测点地图展示
function monitor_map(params) {
    showmappanel(params)
}

function showmappanel(params) {
    
    Ext.Ajax.request(
    {
        url: '/service/projectsite/getprojectsite_byuserid.ashx?params=' + params,
        reader: 'json',
        success: function (response, options) {
            var json = Ext.decode(response.responseText);
            var workplace = Ext.getCmp('index_workplace_id');
            workplace.removeAll();
            workplace.clearListeners();
            var panel = createmappanel(params,json);
            workplace.add(panel);
            createmap(params);

        },
        failure: function (response, options) {
        }
    });
}

function schemestoreload(datastore,params) {
    datastore.loadPage(1, {
        callback: function (records, operation, success) {
            var taskid = Ext.getCmp('select_projectsite_id').getValue();//任务ID
            var schemeid = -1;//监测项ID
            if (records.length > 0) {
                schemeid = records[0].data.schid;
            }
            addpointto_mainmap(params, taskid, schemeid);
        }
    });
}

function createmappanel(params,json) {
    

    var datastore = Ext.create('Ext.data.Store', {
        model: 'TableSchemeName',
        pageSize: 15,
        proxy: {
            type: 'ajax',
            url: '/service/document/get_alltablescheme.ashx?params=' + params,
            extraParams: {
                taskid: json.taskid
            },
            reader: {
                type: 'json',
                rootProperty: 'roots',
                totalProperty: 'totalCount'
            },
            autoLoad: true
        }
    });
    schemestoreload(datastore, params);


    var combostore = Ext.create('Ext.data.Store', {
        fields: ['taskid', 'taskname'],
        model: 'TaskSimpe',
        proxy: {
            type: 'ajax',
            async: false,
            url: '/service/basepoint/gettaskbyuseranddepartment.ashx?params=' + params,
            extraParams: {
                departmentid: json.departmentid
            },
            reader: {
                type: 'json',
                rootProperty: 'data',
                totalProperty: 'totalCount'
            },
            autoLoad: true,
        }
    });
    combostore.load();


    var panel = Ext.create('Ext.panel.Panel', {
        title: '首页',
        bodyPadding: '5px',
        layout: 'border',
        html: '<div style="height:100%;width:100%;"></div>',
        dockedItems: [{
            xtype: 'toolbar',
            dock: 'top',
            height:35,
            items: [
                {
                    xtype: 'comboboxtree',
                    fieldLabel: '选择部门',
                    id: 'select_department_id',
                    msgTarget: 'under',
                    selectNodeModel: 'exceptRoot',
                    editable: false,
                    width: 400,
                    value:json.departmentname,
                    labelAlign: 'right',
                    url: '/service/user/build_department_tree.ashx',
                    listeners: {
                        change: function (combo, newValue, oldValue, eOpts) {
                            combostore.proxy.extraParams.departmentid = newValue;
                            combostore.load({
                                callback: function (records, operation, success) {
                                    if (!Ext.isEmpty(records[0].data.taskid)) {
                                        Ext.getCmp('select_projectsite_id').setValue(records[0].data.taskid);
                                    }
                                    else {
                                        Ext.getCmp('select_projectsite_id').setValue(-1);
                                    }
                                }
                            });
                        }
                    }
                },
                {
                    xtype: 'combo',
                    id:'select_projectsite_id',
                    fieldLabel: '选择工点', //此时的项目就是任务
                    labelAlign: 'right',
                    editable: false,
                    store: combostore,
                    displayField: 'taskname',
                    valueField: 'taskid',
                    triggerAction: 'all',
                    value: json.taskid,
                    forceSelection: true,
                    listeners: {
                        change: function (combo, newValue, oldValue, eOpts) {
                            var departmentid = Ext.getCmp('select_department_id').getValue();
                            var taskid = newValue;
                            if (Ext.isEmpty(taskid))
                            {
                                taskid = -1;
                            }
                            if (!Ext.isEmpty(departmentid)) {
                                //alert(departmentid + ',' + taskid);
                                datastore.proxy.extraParams.taskid = taskid;
                                schemestoreload(datastore, params);
                            }   
                        }
                    }
                }]
        }],
        listeners:
        {
            resize: function (panel, width, height, oldWidth, oldHeight, eOpts) {
                if (map != null) {
                    map.invalidateSize(true);
                }
            }
        },

        items: [
            {
                xtype: 'gridpanel',
                title: '监测项次',
                id: 'index_group_treepanel_id',
                region: 'west',
                width: 240,
                collapsible: true,
                split: true,
                store: datastore,
                forceFit: true,
                scrollable: 'y',
                columns: [
                   { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'schid' },
                   {
                       header: '监测项次名称', minWidth: 1, align: 'center',
                       sortable: false, menuDisabled: true, draggable: false, dataIndex: 'schname', renderer: rendercell
                   }],
                border: true,

                listeners: {
                    rowclick: function (grid, record, element, rowIndex, e, eOpts) {
                        var schemeid = record.data.schid;
                        var taskid = Ext.getCmp('select_projectsite_id').getValue();//任务ID
                        addpointto_mainmap(params, taskid, schemeid);
                        //alert(record);
                    }
                },

                tbar: [
                {
                    xtype: 'textfield',
                    id: 'search_point_sign_text_id',
                    emptyText: '请输入要搜索的监测项'
                },
                {
                    xtype: 'button',
                    text: '搜索',
                    iconCls: 'button search',
                    handler: function () {
                        var textfield = Ext.getCmp('search_point_sign_text_id');
                        var gridpanel = this.up('gridpanel');
                        var store = gridpanel.getStore();
                        store.proxy.url = '/service/document/get_alltablescheme.ashx?params=' + params + '&searchtext=' + textfield.value;
                        schemestoreload(store, params);
                    }
                }],
                bbar: Ext.create('Ext.PagingToolbar', {
                    displayInfo: true,
                    store: datastore,
                    displayMsg: '显示的条目 {0} - {1} of {2}',
                    emptyMsg: "没有下载项目"
                })

            },
            {
                xtype: 'tabpanel',
                region: 'center',
                items: [{
                    xtype: 'panel',
                    title: '地图展示',
                    id: 'index_leaflet_map_panel_id',
                    html: '<div id="mapid" style="height:100%;width:100%;"></div>'
            },{
                xtype: 'panel',
                title: '表格展示'
            },
            {
                xtype: 'authoritytappanel',
                id: 'subway_upload_point_panel_id',
                params:params,
                title: '数据上传'
            }]
            }
        ]
    });
    return panel;
}



function createmap(params) {
    Ext.Ajax.request({
        url: '/service/point/getscheme_legendbyuser.ashx?params=' + params,
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
            if (map != null) {
                lat = map.getCenter().lat;
                lng = map.getCenter().lng;
                zoom = map.getZoom();
                var layercount = 0;
                map.eachLayer(function (layer) {
                    if (layercount < layer.options.maxZoom) {
                        layercount = layer.options.maxZoom;
                    }
                    // layercount = layercount + 1;
                });
                if (layercount > 18) {
                    maplayers = Googleimage;
                }
            }

            map = L.map("mapid", {
                center: [lat, lng],
                zoom: zoom,
                layers: [maplayers],
                zoomControl: false,
                trackResize: true,
                attributionControl: false
            });
            L.control.layers(baseLayers, null).addTo(map);
            L.control.zoom({
                zoomInTitle: '放大',
                zoomOutTitle: '缩小'
            }).addTo(map);


            var legend = L.control.legend({ position: 'bottomright', json: json });
            //添加图例
            legend.addTo(map);

        }
    });
}

var makercount = 0;
blinkMarker = function (point, property) {
    // 使用js标签,便于操作,这个temDivEle的作用是将divEle通过innerHTML的方式获取为字符串
    var tempDivEle = document.createElement("div");
    var divEle = document.createElement("div");
    var spanEl = document.createElement("span");
    var aEl = document.createElement("a");
    tempDivEle.append(divEle);
    divEle.append(spanEl);
    spanEl.append(aEl);


    // 设置上基础的样式

    Ext.util.CSS.createStyleSheet(".makercss_" + makercount + "{display: inline-block;width: 21px;height: 21px;border-radius: 100%;background-position:center;background-image: url(" +
        property.icon.options.iconUrl + ");background-size:100% 100%;position: relative;box-shadow: 1px 1px 5px 0 rgba(0, 0, 0, 0.1);}", "red");

    spanEl.classList.add("makercss_" + makercount);
    makercount = makercount + 1;
    aEl.classList.add("dive-icon");
    // 操作样式

    //sheet = style.sheet;
    // 主体颜色
    if (property) {
        if (property.color) {
            //        spanEl.style.backgroundImage = 'url('+property.icon+')';
            if (!property.diveColor) {
                aEl.style.boxShadow = "0 0 6px 2px " + property.color;
            }
        }
        // 标记大小
        if (property.iconSize) {
            spanEl.style.width = property.iconSize[0] + "px";
            spanEl.style.height = property.iconSize[1] + "px";
        }
        // 发散的color
        if (property.diveColor) {
            // 发散的重度
            if (property.level) {
                aEl.style.boxShadow = "0 0 " + (property.level * 3) + "px " + property.level + "px " + property.diveColor;
            } else {
                aEl.style.boxShadow = "0 0 6px 2px " + property.diveColor;
            }
        }
        // 发散的重度
        if (property.level) {
            if (property.diveColor) {
                aEl.style.boxShadow = "0 0 " + (property.level * 3) + "px " + property.level + "px " + property.diveColor;
            } else if (property.color) {
                aEl.style.boxShadow = "0 0 " + (property.level * 3) + "px " + property.level + "px " + property.color;
            } else {
                aEl.style.boxShadow = "0 0 " + (property.level * 3) + "px " + property.level + "px red";
            }
        }

        // 闪烁的速度
        if (property.speedTime) {
            aEl.style.setProperty("animation", "pulsate " + property.speedTime + "s infinite")
        }
    }
    var myIcon = L.divIcon({ className: 'my-div-icon', html: tempDivEle.innerHTML });
    var marker = L.marker(point, { icon: myIcon, opacity: property.opacity, pointid: property.pointid, title: property.title });
    return marker;
}

//添加点到地图中
function addpointto_mainmap(params,taskid,schemeid) {

    Ext.Ajax.request({
        url: '/service/point/get_geojson_by_task_schemeid.ashx?params=' + params,
        params: {
            taskid: taskid,
            schemeid:schemeid
        },
        success: function (form, action) {
            var geo = Ext.decode(form.responseText);
            if (map != null) {
                L.geoJSON(geo, {
                    pointToLayer: function (feature, latlng) {
                        var greenIcon = L.icon({
                            iconUrl: feature.properties.colors,
                            iconSize: [20, 20], // size of the icon
                        });
                        var sign = null;
                        if (feature.properties.isalarm == 1) {
                            sign = blinkMarker(latlng, {
                                iconSize: [20, 20], opacity: 1,
                                icon: greenIcon, pointid: feature.properties.pointid, color: feature.properties.alarmcolor, speedTime: 1.0
                            }).addTo(map);
                        }
                        else {
                            sign = L.marker(latlng, { opacity: 0.8, icon: greenIcon, pointid: feature.properties.pointid });
                        }


                        //     var 
                        sign.on({
                            click: function (e) {
                                window.open('../Home/PointDetail?params=' + params + '&pointid=' + e.target.options.pointid);
                                //     pointtosign(e, params);
                            }
                        });
                        signlayer.push(sign);
                        return sign;
                    },
                    onEachFeature: function (feature, layer) {//文字
                        var myIcon = L.divIcon({
                            html: '<br\>' + feature.properties.name,
                            className: 'maplabel'
                        });
                        var maker = L.marker([feature.geometry.coordinates[1], feature.geometry.coordinates[0]], { icon: myIcon, pointid: feature.properties.pointid }).addTo(map);
                        maker.on({
                            click: function (e) {
                                window.open('../Home/PointDetail?params=' + params + '&pointid=' + e.target.options.pointid);
                            }
                        });
                        signlayer.push(maker);
                    }
                }).addTo(map);
            }
        }
    });
}