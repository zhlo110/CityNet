 function authorityFun(instanceConfig) {
        var me = this,
        cfg = me.getConfigurator();
        me.initConfig = Ext.emptyFn; // ignore subsequent calls to initConfig 
        me.initialConfig = instanceConfig || {};
        cfg.configure(me, instanceConfig);
        Ext.Ajax.request({
            url: '/service/authority/validate_button.ashx?params=' + this.params,
            async: true,
            params: {
                buttonid: me.id
            },
            success: function (response, options) {
                var json = Ext.decode(response.responseText);
                me.enablefunction(json.success);
            },
            failure: function (response, options) {
                me.enablefunction(0);
            }
        });
        return me;
 }

Ext.define("Autority.Button", {
    extend: "Ext.Button",
    params: '',
    enablefunction: function (success) {
        var me = this;
        var mode = me.mode;
        if (Ext.isEmpty(mode)) {
            if (success == 1) {
                me.setVisible(true);
            }
            else {
                me.setVisible(false);
            }
        }
        else {
            if (mode == 1) {
                me.setVisible(false);
            }
            else {
                if (success == 1) {
                    me.setVisible(true);
                }
                else {
                    me.setVisible(false);
                }
            }
        }
    },
    alias: ['widget.authoritybutton'],
    initConfig: authorityFun
});

Ext.define("Autority.TapPanel", {
    extend: "Ext.panel.Panel",
    params: '',
    alias: ['widget.authoritytappanel'],
    enablefunction: function (success) {
        var me = this;
        if (success != 1) {
            me.close();
        }
    },
    initConfig: authorityFun
});

Ext.define("Autority.WidgetColumn", {
    extend: "Ext.grid.column.Widget",
    params: '',
    alias: ['widget.authoritywidgetcolumn'],
    enablefunction: function (success) {
        var me = this;
        if (success != 1) {
            me.setVisible(false);
        }
    },
    initConfig: authorityFun
});

