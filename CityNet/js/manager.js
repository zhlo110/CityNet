

function setupui(usermsg, managerParams)
{
    var treestore = new Ext.data.TreeStore(
    {
        autoSync: true,
        proxy: {
            type: 'ajax',
            url: '/service/user/build_function_tree.ashx?funtype=1&params=' + managerParams,
            reader: 'json'
        },
        root: {
            name: '根节点',
            id: 'project_id'
        },
        autoLoad: true,
        listeners: {
            'load': function (trees, records, successful, operation, node, eOpts) {
                if (successful) {
                    var rootNode = trees.getRootNode();
                    var leaf = findfirstleaf(rootNode);
                    if (leaf != null && leaf.isLeaf()) {
                        var treepanel = Ext.getCmp('function_treepanel_id');
                        treepanel.getSelectionModel().select(leaf);
                        eval(leaf.data.url)(managerParams);
                    }
                }
                //alert(successful);
            }
        }
    });

    Ext.define('AccordionMenu.view.MainViewViewModel', {
        extend: 'Ext.app.ViewModel',
        alias: 'viewmodel.mainview'

    });
    Ext.define('AccordionMenu.view.MainView', {
        extend: 'Ext.container.Viewport',
        alias: 'widget.mainview',
        requires: [
            'AccordionMenu.view.MainViewViewModel',
            'Ext.menu.Menu',
            'Ext.menu.Item'
        ],

        viewModel: {
            type: 'mainview'
        },
        itemId: 'mainView',
        layout: 'border',

        items: [
            {
                xtype: 'panel',
                region: 'north',
                html: '<table width="100%" height = "100%"><tr><td height="100%"><img src="../Images/topbar.png" /></td></tr></table>',
                height: 185,
                xtype: 'panel',
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
                                       { xtype: 'label', style: "position:relative;top:3px;", padding: 10, text: usermsg },
                                       {
                                           xtype: 'button', text: '返回首页', handler: function () {
                                               self.location = '../Home/Index?params=' + managerParams;
                                           }
                                       },
                            ]
                        }
                       ]
                   }]
                
            },
            {
                xtype: 'treepanel',
                region: 'west',
                width: 300,
                id: 'function_treepanel_id',
                title: '功能树',
                rootVisible: false,
                store: treestore,
                scrollable: true,
                viewConfig: {
                    width: 300
                },
                listeners: {
                    'itemclick': function (node, record, item) {
                        if (record.data.leaf) {
                            var centerpanel = Ext.getCmp('function_area_id');
                            centerpanel.clearListeners();
                            eval(record.data.url)(managerParams);
                            // alert(record.data.url);
                        }
                    }
                }
            },
            {
                xtype: 'panel',
                flex: 1,
                region: 'center',
                itemId: 'contentPanel',
                id: 'function_area_id',
                title: '功能区',
                width: '100%',
                border: false,
                bodyStyle: 'overflow-x:hidden;overflow-y:scroll',
                layout:'fit'
            }
        ]
    });

    var viewport = Ext.create('AccordionMenu.view.MainView');
    //加载功能菜单
}