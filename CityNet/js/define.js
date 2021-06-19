Ext.define('functionInfo', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'functionid', type: 'int' },
        { name: 'functioncheck', xtype: 'checkcolumn', dataIndex: 'functioncheck' },
        { name: 'functionname', type: 'string' },
        { name: 'priority', type: 'int' },
        { name: 'functionurl', type: 'string' },
        { name: 'functiondate', type: 'string' },
        { name: 'functiondcreator', type: 'string' },
        { name: 'description', type: 'string' }
    ]
});

Ext.define('roleInfo', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'roleid', type: 'int' },
        { name: 'rolecheck', xtype: 'checkcolumn' },
        { name: 'rolename', type: 'string' },
        { name: 'realname', type: 'string' },
        { name: 'description', type: 'string' },
        { name: 'rolecreator', type: 'string' }
    ]
});

Ext.define('userInfo', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'userid', type: 'int' },
        { name: 'usercheck', xtype: 'checkcolumn' },
        { name: 'username', type: 'string' },
        { name: 'realname', type: 'string' },
        { name: 'department', type: 'string' },
        { name: 'position', type: 'string' },
        { name: 'rolename', type: 'string' }
    ]
});


Ext.define('taskState', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'stateid', type: 'int' },
        { name: 'statename', type: 'string' },
        { name: 'statepriority', type: 'int' },
        { name: 'editable', type: 'bool' },
        { name: 'creator', type: 'string' },
        { name: 'createdate', type: 'string' },
        { name: 'description', type: 'string' }
    ]
});

Ext.define('Task', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'taskid', type: 'int' },
        { name: 'taskname', type: 'string' },
        { name: 'taskcreatedate', type: 'string' },
        { name: 'creator', type: 'string' },
        { name: 'creatordep', type: 'string' },
        { name: 'auditor', type: 'string' },
        { name: 'auditordep', type: 'string' },
        { name: 'state', type: 'string' }

    ]
});


Ext.define('ProjectSite', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'taskchecked', xtype: 'checkcolumn' },
        { name: 'taskid', type: 'int' },
        { name: 'taskname', type: 'string' },
        { name: 'sitetype', type: 'string' },
        { name: 'sitestate', type: 'string' },
        { name: 'range', type: 'string' },
        { name: 'direction', type: 'string' },
        { name: 'description', type: 'string' }

    ]
});

Ext.define('TaskSimpe', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'taskid', type: 'int' },
        { name: 'taskname', type: 'string' }
    ]
});

Ext.define('Document', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'docid', type: 'int' },
        { name: 'docname', type: 'string' },
        { name: 'creator', type: 'string' },
        { name: 'url', type: 'string' },
        { name: 'department', type: 'string' },
        { name: 'pages', type: 'int' },
        { name: 'result', type: 'int' }
    ]
});

Ext.define('Point', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'id', type: 'int' },
        { name: 'taskid', type: 'int' },
        { name: 'pointname', type: 'string' },
        { name: 'firsttime', type: 'string' },
        { name: 'longitude', type: 'string' },
        { name: 'latitude', type: 'string' }
    ]
});

Ext.define('BasePoint', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'id', type: 'int' },
        { name: 'pointname', type: 'string' },
        { name: 'sitename', type: 'string' },
        { name: 'selectcheck', xtype: 'checkcolumn' },
        { name: 'taskid', type: 'int' },
        { name: 'x', type: 'string' },
        { name: 'y', type: 'string' },
        { name: 'z', type: 'string' },
        { name: 'lon', type: 'string' },
        { name: 'lat', type: 'string' },
        { name: 'creator', type: 'string' },
        { name: 'type', type: 'string' }
    ]
});

Ext.define('PointSign', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'pointid', type: 'int' },
        { name: 'pointname', type: 'string' },
        { name: 'longitude', type: 'string' },
        { name: 'latitude', type: 'string' },
        { name: 'sign', type: 'string' }
    ]
});

Ext.define('TableSchemeName', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'schid', type: 'int' },
        { name: 'valid', type: 'int' },
        { name: 'schname', type: 'string' },
        { name: 'priority', xtype: 'checkcolumn' },
        { name: 'createdata', type: 'string' },
        { name: 'description', type: 'string' },
        { name: 'creator', type: 'string' },
        { name: 'color', type: 'string' },
        { name: 'hasprojection', xtype: 'checkcolumn' }
    ]
});

Ext.define('TableSchemeRow', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'rowid', type: 'int' },
        { name: 'alarmcheck', xtype: 'checkcolumn' },
        { name: 'rowname', type: 'string' },
        { name: 'colrel', type: 'string' },
        { name: 'no', type: 'int' },
        { name: 'valid', type: 'int' },
        { name: 'type', type: 'string' },
        { name: 'description', type: 'string' }
    ]
});

Ext.define('Approve', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'approveid', type: 'int' },
        { name: 'date', type: 'string' },
        { name: 'creator', type: 'string' },
        { name: 'auditor', type: 'string' }
    ]
});

Ext.define('Alarm', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'alarmid', type: 'int' },
        { name: 'alarmname', type: 'string' },
        { name: 'minlevel', type: 'string' },
        { name: 'maxlevel', type: 'string' },
        { name: 'color', type: 'string' },
        { name: 'unit', type: 'string' },
        { name: 'alarmlevel', type: 'int' },
        { name: 'description', type: 'string' }
    ]
});

Ext.define('AlarmColumn', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'alarmid', type: 'int' },
        { name: 'alarmname', type: 'string' },
        { name: 'alarmcol', type: 'string' }
    ]
});

function gridloadcurrentPage(datastore) {
    if (datastore.totalCount % datastore.pageSize == 1) {
        if (datastore.currentPage > 1) {
            datastore.loadPage(datastore.currentPage - 1);
        }
        else {
            datastore.loadPage(datastore.currentPage);
        }
    }
    else {
        datastore.loadPage(datastore.currentPage);
    }
}

function rendercell(value, eOpts) {
    eOpts.tdStyle = ' vertical-align:middle;height:40px';
    return value;
}

Ext.define('ColorField', {
    extend: 'Ext.form.field.Picker',
    alias: 'widget.colorfield',
    triggerTip: '请选择一个颜色',
    labelWidth: 60,
    value: '#FFFFFF',
    SelctColorFun: null,
    onTriggerClick: function () {
        var me = this;
        var config = {
            pickerField: this,
            ownerCt: this,
            renderTo: Ext.getBody(),
            floating: true,
            focusOnShow: true,
            // style: {
            //     backgroundColor: "#fff"
            // },
            value: me.value,
            listeners: {
                scope: this,
                select: function (field, value, opts) {
                    me.setValue('#' + value);
                    me.inputEl.applyStyles({
                        backgroundColor: '#' + value
                    });
                    me.picker.hide();
                    if (typeof me.SelctColorFun === "function") {
                        me.SelctColorFun('#' + value);
                    }
                }
            }
        };
        if (Ext.isArray(me.colors)) {
            config.colors = me.colors;
        }
        if (!me.picker) {
            me.picker = Ext.create('Ext.picker.Color', config);
            // me.picker.alignTo(me.inputEl, 'tl-bl?');
        }
        me.picker.show();
        var attached = me.attached || false;
        me.lastShow = new Date();
        if (!attached) {
            Ext.getDoc().on('mousedown', me.onMouseDown, me, {
                buffer: Ext.isIE9m ? 10 : undefined
            });
            me.attached = true;
        }
    },
    onMouseDown: function (e) {
        var lastShow = this.lastShow,
            doHide = true;
        if (Ext.Date.getElapsed(lastShow) > 50 && !e.getTarget('#' + this.picker.id)) {
            if (Ext.isIE9m && !Ext.getDoc().contains(e.target)) {
                doHide = false;
            }
            if (doHide) {
                this.picker.hide();
                Ext.getDoc().un('mousedown', this.onMouseDown, this);
                this.attached = false;
            }
        }
    }
});
