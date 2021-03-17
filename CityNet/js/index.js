var map = null;

var imageTpl = new Ext.XTemplate(
    '<tpl for=".">',
        '<div style="margin-bottom: 10px;display: {visible};" class="thumb-wrap">',
          '<br\><center><img src="{src}" /></center>',
          '<center>{caption}</center>',
        '</div>',
    '</tpl>');
function setupui(usermsg, managerUrl, params) {
    Ext.override(Ext.grid.plugin.RowExpander, { // Override to fire collapsebody & expandbody
        init: function (grid) {
            this.callParent(arguments);
            //  grid.getView().addEvents('collapsebody', 'expandbody');//ext论坛找到的解决办法，这样也无法添加事件
            //存储grid对象
            this.grid = grid
            this.grid.addStateEvents('collapsebody', 'expandbody');//给grid对象添加事件
        },
        toggleRow: function (rowIdx, record) {
            
            var me = this,
             view = me.view,
             rowNode = view.getNode(rowIdx),
             row = Ext.fly(rowNode, '_rowExpander');
            if (Ext.isEmpty(row)) {
                return;
            }
             
            var nextBd = row.down(me.rowBodyTrSelector, true),
             isCollapsed = row.hasCls(me.rowCollapsedCls),
             addOrRemoveCls = isCollapsed ? 'removeCls' : 'addCls',
             ownerLock, rowHeight, fireView;

            
            // Suspend layouts because of possible TWO views having their height change
            Ext.suspendLayouts();
            row[addOrRemoveCls](me.rowCollapsedCls);
            Ext.fly(nextBd)[addOrRemoveCls](me.rowBodyHiddenCls);
            me.recordsExpanded[record.internalId] = isCollapsed;
            view.refreshSize();


            // Sync the height and class of the row on the locked side
            if (me.grid.ownerLockable) {
                ownerLock = me.grid.ownerLockable;
                //   fireView = ownerLock.getView();
                view = ownerLock.lockedGrid.view;
                fireView = ownerLock.lockedGrid.view;
                rowHeight = row.getHeight();
                // EXTJSIV-9848: in Firefox the offsetHeight of a row may not match
                // it is actual rendered height due to sub-pixel rounding errors. To ensure
                // the rows heights on both sides of the grid are the same, we have to set
                // them both.
                row.setHeight(isCollapsed ? rowHeight : '');
                row = Ext.fly(view.getNode(rowIdx), '_rowExpander');
                row.setHeight(isCollapsed ? rowHeight : '');
                row[addOrRemoveCls](me.rowCollapsedCls);
                view.refreshSize();
            } else {
                fireView = view;
            }
            //通过grid触发事件，而不是view
            this.grid.fireEvent(isCollapsed ? 'expandbody' : 'collapsebody', row.dom, record, nextBd);
            //下面为ext论坛的解决办法，无效
            //fireView.fireEvent(isCollapsed ? 'expandbody' : 'collapsebody', row.dom, record, nextBd);
            // Coalesce laying out due to view size changes
            Ext.resumeLayouts(true);
        },
    });


    Ext.QuickTips.init();
    Ext.define('Image', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'src', type: 'string' },
            { name: 'caption', type: 'string' },
            { name: 'visible', type: 'string' },
            { name: 'url', type: 'string' }
        ]
    });
    Ext.application.params = params;
    
    var mappanel = createmappanel(params);

    var viewport = Ext.create('Ext.container.Viewport',
    {
        layout: 'border',
        items: [
            {
                xtype: 'panel',
                id:'workspace_viewpoint_id',
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
                                        {
                                            xtype: 'authoritybutton', text: '管理账号',
                                            id: 'manager_button',
                                            params: params,
                                            listeners:
                                            {
                                                click: function () {
                                                    self.location = managerUrl;
                                                },
                                                beforeshow: function (button, eOpts) {
                                                    return false;
                                                }
                                            }
                                        }
                             ]
                         }
                        ]
                    }]
            },
            {
                xtype: 'panel',
                region: 'west',
                id:'index_navagation_bar_id',
                defaults: {
                    bodyPadding: 10,
                    header: {
                        height: 35,
                        border: 2
                    },
                },
                layout: 'accordion',
                width: 240,
                items:
                [
                ]
            },
            {
                xtype: 'panel',
                id:'index_workplace_id',
                layout: 'fit',
                region: 'center',
                items: [mappanel]
            }]
    });
    loadNavigation(params);
    createmap(params);
    addpointto_mainmap(params);
}


function createmappanel(params) {

    
    var datastore = Ext.create('Ext.data.Store', {
        model: 'PointSign',
        pageSize: 15,
        proxy: {
            type: 'ajax',
            url: '/service/point/get_pointlist_by_user.ashx?params=' + params,
            reader: {
                type: 'json',
                rootProperty: 'roots',
                totalProperty: 'totalCount'
            },
            autoLoad: true
        }
    });
    datastore.loadPage(1);

    var panel = Ext.create('Ext.panel.Panel', {
        title: '地图首页',
        bodyPadding: '5px',
        layout:'border',
        html: '<div style="height:100%;width:100%;"></div>',
        
        listeners:
        {
            resize: function (panel, width, height, oldWidth, oldHeight, eOpts) {
                if (map != null) {
                    map.invalidateSize(true);
                }
                //alert(width);
            }
        },

        items: [
            {
                xtype: 'gridpanel',
                title: '控制点列表',
                id: 'index_group_treepanel_id',
                region: 'west',
                width:240,
                collapsible: true,
                split: true,
                store: datastore,
                forceFit: true,
                scrollable: 'y',
                viewConfig: {
                    getRowClass: function (record, rowIndex, rowParams, store) {
                        if (Ext.isEmpty(record.get("longitude")) || Ext.isEmpty(record.get("latitude")) || Ext.isEmpty(record.get("sign")))
                            return 'x-grid-red';
                        else
                            return 'x-grid-green';
                    }
                },
                columns: [
                   { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'pointid' },
                   {
                       header: '点名', minWidth: 1, align: 'center',
                       sortable: false, menuDisabled: true, draggable: false, dataIndex: 'pointname',renderer: rendercell
                   }],
                border: true,

                listeners: {
                    rowclick: function (grid, record, element, rowIndex, e, eOpts) {
                        if (map!=null&&!Ext.isEmpty(record.data.latitude) && !Ext.isEmpty(record.data.longitude)) {
                            map.setView([record.data.latitude, record.data.longitude], 17);

                        }
                        //  alert(record.data.id);
                    }
                },

                tbar: [
                {
                    xtype: 'textfield',
                    id: 'search_point_sign_text_id',
                    emptyText: '请输入要搜索的点名'
                },
                {
                    xtype: 'button',
                    text: '搜索',
                    iconCls: 'button search',
                    handler: function () {
                        var textfield = Ext.getCmp('search_point_sign_text_id');
                        var gridpanel = this.up('gridpanel');
                        var store = gridpanel.getStore();
                        store.proxy.url = '/service/point/get_pointlist_by_user.ashx?params=' + params + '&searchtext=' + textfield.value,
                        store.loadPage(1);
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
                xtype: 'panel',
                region: 'center',
                id:'index_leaflet_map_panel_id',
                html: '<div id="mapid" style="height:100%;width:100%;"></div>'
            }
            ]
    });
    return panel;
}

function showmappanel(params) {
    var workplace = Ext.getCmp('index_workplace_id');
    workplace.removeAll();
    var panel = createmappanel(params);
    workplace.add(panel);
    createmap(params);
    addpointto_mainmap(params);
}
function showdocumentpanel(params) {
    showdocumentpanelimp(params, 0, 'get_all_tasks');
}
function showdocumentpanelimp(params,mode,url) {
    var workplace = Ext.getCmp('index_workplace_id');
    workplace.removeAll();
    Ext.Ajax.request({
        url: '/service/user/get_userinfomation.ashx?params=' + params,
        success: function (form, action) {
            var userjson = Ext.decode(form.responseText);//当前用户信息

            var datastore = Ext.create('Ext.data.Store', {
                model: 'Task',
                pageSize: 15,
                proxy: {
                    type: 'ajax',
                    url: '/service/task/' + url + '.ashx?params=' + params,
                    extraParams: {
                        mode:mode
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
            var panel = Ext.create('resource.WidgetGrid', {
                id: 'task_gridview_id',
                store: datastore,
                params: params,
                userjson: userjson,
                mode:mode,
                bbar: Ext.create('Ext.PagingToolbar', {
                    store: datastore,
                    displayInfo: true,
                    displayMsg: '显示的条目 {0} - {1} of {2}',
                    emptyMsg: "没有下载项目"
                }),

                dockedItems: [{
                    xtype: 'toolbar',
                    dock: 'top',
                    items: [
                        {
                            xtype: 'authoritybutton',
                            params: Ext.application.params,
                            id: 'add_task_button_id',
                            iconCls: 'button add',
                            mode:mode,
                            text: '添加任务',
                            handler: function () {
                                workplace.removeAll();
                                createuploadwizard(workplace, Ext.application.params, userjson, -1);
                            }
                        },
                        {
                            xtype: 'authoritybutton',
                            id: 'delete_task_button_id',
                            params: Ext.application.params,
                            iconCls: 'button delete',
                            text: '删除任务',
                            handler: function () {
                            }
                        },

                        {
                            xtype: 'authoritybutton',
                            id: 'update_task_button_id',
                            params: Ext.application.params,
                            iconCls: 'button modify',
                            mode: mode,
                            text: '修改任务',
                            handler: function () {
                                var gridpanel = Ext.getCmp('task_gridview_id');
                                var selectNode = gridpanel.selection;
                                if (selectNode == null || selectNode.length <= 0) {
                                    Ext.MessageBox.alert("提示信息", '请选择要编辑的任务');
                                }
                                else {
                                    //是否可编辑
                                    Ext.Ajax.request({
                                        url: '/service/task/can_eidtable.ashx?params=' + params,
                                        params: {
                                            taskid: selectNode.data.taskid
                                        },
                                        success: function (form, action) {

                                            var json = Ext.decode(form.responseText);
                                            if (json.editable == 1) {
                                                workplace.removeAll();
                                                createuploadwizard(workplace, Ext.application.params, userjson, selectNode.data.taskid);
                                                //workplace.add(wizardpanel);
                                            }
                                            else {
                                                Ext.MessageBox.alert("提示信息", "该状态下无法编辑");
                                            }
                                           // editable
                                        },
                                        failure: function (form, action) {
                                            var errorjson = Ext.decode(form.responseText);
                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                        }
                                    });
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

function logout(params) {
    Ext.MessageBox.confirm("提示", "是否要退出系统？", function (btnId) {
        if (btnId == "yes") {
            self.location = '../Login/Login';
        }
    });
}

function managerPanel(params) {
    self.location = '../Home/Manager?params=' + params;
}
function managerpanel(params) {
}

function loadNavigation(params) {
    var navPanel = Ext.getCmp('index_navagation_bar_id');
    Ext.Ajax.request({
        url: '/service/user/build_function_tree.ashx?funtype=4&params=' + params,
        success: function (form, action) {
            var json = Ext.decode(form.responseText);
            json.sort(function (a, b) {
                if (a.priority > b.priority) return 1;
                else if (a.priority < b.priority) return -1;
                else return 0;
            });
            var len = json.length;
            var i;
            for (i = 0; i < len; i++)
            {
                var sub = json[i];
                var panel = Ext.create('Ext.panel.Panel', {
                    title: '<div style="font-size:14px;">' + sub.text + '</div>',
                    xtype: 'panel',
                    scrollable: 'y',
                    layout: {
                        type: 'vbox',
                        pack: 'start',
                        align: 'stretch'
                    },
                    defaults: {
                        frame: true,
                        bodyPadding: 10
                    }
                });

                var children = sub.children;
                children.sort(function (a, b) {
                    if (a.priority > b.priority) return 1;
                    else if (a.priority < b.priority) return -1;
                    else return 0;
                });
                var j;
                var jCount = children.length;

                var store = Ext.create('Ext.data.Store', {
                    model: 'Image'
                });

                for (j = 0; j < jCount; j++)
                {
                    var subfun = children[j];
                    store.add({ src: subfun.description, caption: subfun.text, visible: 'block', url: subfun.url });
                }
                panel.add(
                    {
                        xtype: 'dataview',
                        store: store,
                        tpl: imageTpl,
                        itemSelector: 'div.thumb-wrap',
                        emptyText: 'No images available',
                        listeners: {
                            itemclick: function (view, record, item, index, e, eOpts) {
                                eval(record.data.url)(params);
                            }
                        }
                    });
                navPanel.add(panel);
            }
        },
        failure: function (form, action) {
            var mes = Ext.decode(action.response.responseText);
            Ext.MessageBox.alert("提示信息", mes.msg);
        }
    });
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

function addpointto_mainmap(params) {

    Ext.Ajax.request({
        url: '/service/point/get_geojson_by_user.ashx?params=' + params,
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