function TaskFun(instanceConfig) {
    var me = this,
    cfg = me.getConfigurator();
    me.initConfig = Ext.emptyFn; // ignore subsequent calls to initConfig 
    me.initialConfig = instanceConfig || {};
    cfg.configure(me, instanceConfig);
    Ext.Ajax.request({
        url: '/service/task/can_eidtable.ashx?params=' + this.params,
        async: true,
        params: {
            taskid: me.taskid
        },
        success: function (response, options) {
            var json = Ext.decode(response.responseText);
            me.enablefunction(json.editable);
        },
        failure: function (response, options) {
            me.enablefunction(0);
        }
    });
    return me;
}

Ext.define("Task.Button", {
    extend: "Ext.Button",
    params: '',
    enablefunction: function (success) {
        var me = this;
        if (me.viewmode == 1) { //查看模式
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
    },
    alias: ['widget.taskbutton'],
    initConfig: TaskFun
});

Ext.define("Task.column.Action", {
    extend: "Ext.grid.column.Action",
    params: '',
    enablefunction: function (success) {
        var me = this;
        if (me.viewmode == 1) { //查看模式
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
    },
    alias: ['widget.taskaction'],
    initConfig: TaskFun
});
