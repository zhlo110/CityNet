var scrolldir = -1;
var lock = false;
function documentview(usermsg, managerParams, documentID) {

    Ext.define('Document', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'src', type: 'string' }
        ]
    });

    var datastore = Ext.create('Ext.data.Store', {
        model: 'Document',
        pageSize: 1,
        proxy: {
            type: 'ajax',
            url: '/service/document/get_documentview_byid.ashx?params=' + managerParams,
            extraParams:
            {
                documentid: documentID
            },
            reader: {
                type: 'json',
                rootProperty: 'roots',
                totalProperty: 'totalCount'
            },
            autoLoad: true
        }
    });
    var imageTpl = new Ext.XTemplate(
    '<tpl for=".">',
        '<div style="text-align:center;background-color:gray;" class="thumb-wrap">',
          '<img src="{src}"  height="1123" width="794"/>',
        '</div>',
    '</tpl>');

    datastore.loadPage(1);
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
               xtype:"panel",
            //   layout: 'center',
               region: 'center',
               id:'data_viewpanel_id',
               bodyStyle: 'background:#a4a4a4;',
               scrollable: 'y',


               onScrollMove: function (x, y) {
                   scrolldir = -1;
               },
    
               items:[{
                   xtype: 'gridpanel',
                   id: 'gridview_id',
                   forceFit: true,
                   store: datastore,
                   bodyStyle: 'background:#a4a4a4;',
                   height: 1139,
                   scrollable: null,
                   rowLines: false,
                   columnLines: false,
                   hideHeaders: true,
                   border:false,
                   plugins: [
                        {
                            ptype: 'bufferedrenderer',
                            trailingBufferZone: 20,  // Keep 20 rows rendered in the table behind scroll
                            leadingBufferZone: 50   // Keep 50 rows rendered in the table ahead of scroll
                        }],
                   viewConfig:
                       {
                           preserveScrollOnRefresh: true,
                           preserveScrollOnReload: true,
                           stripeRows:false
                         //  width: 794,
                        //   height: 1123,
                        //   tpl: imageTpl,
                       //    itemSelector: 'div.thumb-wrap',
                       },
                   columns: [{
                       minWidth: 1,
                       dataIndex: 'src',
                       bodyStyle: 'background:#a4a4a4;',
                       renderer: function (v, m) {
                           m.tdCls = 'documentcell';
                           var src = '<center><img src="' + v + '"  height="1123" width="794"/></center>';
                           return src;
                       }
                   }]
               }],
               bbar: Ext.create('Ext.PagingToolbar', {
                   id:'tool_page_bar_id',
                   store: datastore,
                   displayInfo: true,
                   displayMsg: '显示的条目 {0} - {1} of {2}',
                   emptyMsg: "没有下载项目"
               })
           }]
   });

    Ext.get('data_viewpanel_id').on('mousewheel', function (e) {
        if (lock) return;
        lock = true;
        var delta = e.getWheelDelta();
        //已经充满
        var workspace = Ext.getCmp('data_viewpanel_id');
        var height = workspace.getHeight();
        var toolbar = Ext.getCmp('tool_page_bar_id');
        if (height > 1123) { //已经充满
            //  console.log('full');
            if (delta == -1) {
                nextPage(datastore);
            }
            else if (delta == 1) {
                prevPage(datastore);
            }
            else {
                lock = false;
            }
        }
        else {
            var scrolly = workspace.getScrollY();
            if (scrolldir == 1) {
                if (delta == -1 && scrolly > workspace.getScrollable().getMaxPosition().y-2) {
                    nextPage(datastore);
                }
                else if (delta == 1 && scrolly < 2) {
                    prevPage(datastore);
                }
                else {
                    lock = false;
                }
            }
            else {
                lock = false;
            }
        }
        scrolldir = 1;
    });
}

function nextPage(datastore) {
   
    var workspace = Ext.getCmp('data_viewpanel_id');

    if (datastore.currentPage != datastore.totalCount) {
        datastore.nextPage({
            scope: this,
            callback: function (records, operation, success) {
                workspace.setScrollY(0, {
                    listeners: {
                        afteranimate: function () {
                            lock = false;
                        }
                    }
                });
                
            }
        });

    }
    else {
        lock = false;
    }
}
function prevPage(datastore) {
    var workspace = Ext.getCmp('data_viewpanel_id');
    if (datastore.currentPage != 1) {
        datastore.previousPage(
            {
                scope: this,
                callback: function (records, operation, success) {

                    workspace.setScrollY(workspace.getScrollable().getMaxPosition().y, {
                        listeners: {
                            afteranimate: function () {
                                lock = false;
                            }
                        }
                    });
                }
            });
    }
    else {
        lock = false;
    }
}