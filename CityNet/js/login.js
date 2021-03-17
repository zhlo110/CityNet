function setupui() {
    var firstexpand = true;

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
                    background: ' no-repeat #FFFFFF center'
                },
                region: 'center',
                layout: 'center',
                items: [
                {
                    xtype: 'form',
                    id: 'logform',
                    width: '300px',
                    bodyStyle: 'padding:35px',
                    fieldDefaults:
                    {
                        width :270
                    },
                    layout: {
                        type: 'vbox',
                        align: 'center'
                    },
                    items:
                    [
                        { xtype: 'textfield',id: 'username_field_id', allowBlank: false, value: Ext.util.Cookies.get("username"), labelWidth: 50, fieldLabel: '用户名', labelAlign: 'right', name: 'username', emptyText: '请输入用户名', blankText: '请输入用户名' },
                        { xtype: 'textfield', id: 'password_field_id', allowBlank: false, value: Ext.util.Cookies.get("password"), labelWidth: 50, inputType: 'password', fieldLabel: '密       码', labelAlign: 'right', name: 'password', emptyText: '请输入密码', blankText: '请输入密码' },
                        {
                            xtype: 'combo',
                            displayField: 'departmentName',
                            valueField: 'departmentID',
                            id: 'department_items_id',
                            listeners: {
                                expand: function (field, eOpts) {
                                    var store = Ext.getCmp('department_items_id').getStore();
                                    store.load();
                                }
                            },
                            store: Ext.create('Ext.data.Store', {
                                autoLoad:false,
                                fields: ['departmentID', 'departmentName'],
                                proxy: {
                                    type: 'ajax',
                                    url: '/service/user/get_user_department.ashx',
                                    reader: 'json',
                                },
                                listeners:
                                {
                                    load: function (store, records, successful, operation, eOpts) {
                                        if (!successful) {
                                            var errorjson = Ext.decode(operation.error.response.responseText);
                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                            Ext.getCmp('department_items_id').setValue('');
                                        }
                                    },
                                    beforeload: function( store, operation, eOpts )
                                    {
                                        var susername = Ext.getCmp('username_field_id').value;
                                        var spassword = Ext.getCmp('password_field_id').value;
                                        if (Ext.isEmpty(susername) || Ext.isEmpty(spassword)) {

                                            Ext.MessageBox.alert("提示信息", '请输入账号和密码！');
                                            Ext.getCmp('department_items_id').setValue('');
                                            return false;
                                        }
                                        else {
                                            store.proxy.url = '/service/user/get_user_department.ashx?username='+susername+
                                                "&password="+spassword;
                                            return true;
                                        }
                                    }
                                }
                            }),
                            allowBlank: false, labelWidth: 50, fieldLabel: '部门', labelAlign: 'right', name: 'department', emptyText: '请选择部门', blankText: '请选择部门'
                        }
                    ],
                    buttons: [
                        {
                            text: '登      录',
                            handler: function () {
                                var form = Ext.getCmp('logform');
                                var thisForm = form.getForm();
                                if (thisForm.isValid()) {
                                    thisForm.submit({
                                        url: '/service/user/logx.ashx',
                                        success: function (form, action) {
                                            //移除并替换
                                            Ext.util.Cookies.set("username", action.result.username);
                                            Ext.util.Cookies.set("password", action.result.password);
                                            self.location = action.result.managerurl;
                                        },
                                        failure: function (form, action) {
                                            //移除并替换
                                            Ext.MessageBox.alert("提示信息", action.result.msg);
                                        }
                                    });
                                }
                            }
                        },
                        {
                            text: '注      册',
                            handler: function () {
                                self.location = "../Login/Register";
                            }
                        }
                    ]
                }]
            }]
    });

}