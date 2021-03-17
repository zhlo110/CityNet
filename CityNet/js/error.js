function setupui() {

    var viewport = Ext.create('Ext.container.Viewport',
    {
        layout: 'border',
        items: [
            {
                xtype: 'panel',
                html: '<table width="100%" height = "100%"><tr><td height="100%"><img src="../Images/topbar.png" /></td></tr></table>',
                height: 160,
                region: 'north',
                dockedItems: [
                    {
                        xtype: 'toolbar',
                        id: 'maintoolbarid',
                        dock: 'bottom',
                        align: 'middle',
                        height: 10
                    }]
            },
            {
                xtype: 'panel',
                width: '100%',
                height: '100%',
                bodyStyle: {
                    background: 'url(../Images/error.jpg) no-repeat #FFFFFF center'
                },
                region: 'center',
                layout: 'center'
            }]
    });

}