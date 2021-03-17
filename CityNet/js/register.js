function setupui() {

    var viewport = Ext.create('Ext.container.Viewport',
    {
        layout: 'border',
        requires: [
            'Ext.form.Panel',
            'Ext.form.field.Text',
            'Ext.form.RadioGroup',
            'Ext.form.field.Radio',
            'Ext.button.Button'
        ],
        items: [
            {
                xtype: 'panel',
                region: 'north',
                html: '<table height = "150px"><tr><td height="100%"><img src="../Images/topbar.png" /></td></tr></table>',
                height: 150,
                xtype: 'panel'
            },
            {
                xtype: 'panel',
                region: 'center',
                title: '  ',
                layout: {
                    type: 'hbox',
                    pack: 'center'
                },
                bodyStyle: 'overflow-x:hidden;overflow-y:scroll',
                items: [
                {
                    xtype: 'form',
                    id: 'registerform',
                    maxWidth: 800,
                    width: 800,
                    title: '新用户注册',
                    layout: {
                        align: 'middle',
                        pack: 'center'
                    },
                    items: [
                    {
                        xtype: 'panel',
                        dock: 'top',
                        layout: 'anchor',
                        bodyBorder: true,
                        bodyPadding: '40px',
                        title: '登录信息',
                        defaults:
					    {
					        padding: 5
					    },
                        items:
                            [{
                                xtype: 'textfield',
                                anchor: '100%',
                                fieldLabel: '用户名 *',
                                name: 'username',
                                msgTarget: 'under',
                                allowBlank: false,
                                vtype: "username",
                                regex: /^[a-zA-Z0-9_-]{4,16}$/,
                                blankText: '用户名不能为空',
                                regexText: '用户名为4到16位（字母，数字，下划线，减号）',
                                emptyText: '用户名不能为空',
                                vtypeText: "用户名已经存在"
                            },
                              {
                                  xtype: 'textfield',
                                  id: 'password1',
                                  name: 'passwordname1',
                                  anchor: '100%',
                                  regex: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[\s\S]{6,16}$/,
                                  width: 150,
                                  fieldLabel: '密码 *',
                                  msgTarget: 'under',
                                  inputType: 'password',
                                  allowBlank: false,
                                  blankText: '密码不能为空',
                                  regexText: '密码有6-16位，包括至少1个大写字母，1个小写字母，1个数字',
                                  emptyText: '密码不能为空'
                              },
                               {
                                   xtype: 'textfield',
                                   name: 'passwordname2',
                                   id: 'password2',
                                   anchor: '100%',
                                   width: 150,
                                   fieldLabel: '再次输入密码 *',
                                   msgTarget: 'under',
                                   inputType: 'password',
                                   vtype: "password",
                                   allowBlank: false,
                                   blankText: '密码不能为空，且两次密码输入必须一致',
                                   emptyText: '密码不能为空，且两次密码输入必须一致',
                                   vtypeText: "两次密码不一致！",
                                   confirmTo: "password1"//要比较的另外一个的组件的id  
                               }
                            ]
                    },
                        {
                            xtype: 'panel',
                            dock: 'top',
                            layout: 'anchor',
                            bodyPadding: '40px',
                            title: '基本信息',
                            defaults:
                           {
                               padding: 5,
                               autoHeight: true,
                               autoWidth: true
                           },
                            items: [
                                {
                                    xtype: 'textfield',
                                    anchor: '100%',
                                    fieldLabel: '真实姓名 *',
                                    name: "realname",
                                    msgTarget: 'under',
                                    allowBlank: false,
                                    emptyText: '真实姓名不能为空',
                                    blankText: '真实姓名不能为空',
                                    regex: /^([\u4e00-\u9fa5]{1,20}|[a-zA-Z\.\s]{1,20})$/,
                                    regexText: '必须为真实姓名'

                                },
                                {
                                    xtype: 'comboboxtree',
                                    anchor: '100%',
                                    name: "departmentform",
                                    id: "departmentformid",
                                    fieldLabel: '部门 *',
                                    msgTarget: 'under',
                                    allowBlank: false,
                                    selectNodeModel: 'exceptRoot',
                                    editable: false,
                                    blankText: '部门不能为空',
                                    url: '/service/user/build_department_tree.ashx'
                                    
                                },
                                {
                                    xtype: 'textfield',
                                    anchor: '100%',
                                    name: "occupation",
                                    fieldLabel: '职位'
                                },
                                {
                                    xtype: 'radiogroup',
                                    fieldLabel: '性别',
                                   
                                    items: [
                                        {
                                            xtype: 'radiofield',
                                            name: 'sex',
                                            boxLabel: '男',
                                            inputValue: 1,
                                            checked: true
                                        },
                                        {
                                            xtype: 'radiofield',
                                            name: 'sex',
                                            boxLabel: '女',
                                            inputValue: 2
                                        }
                                    ]
                                },
                                {
                                    xtype: 'textfield',
                                    anchor: '100%',
                                    name: "officephone",
                                    fieldLabel: '办公室电话',
                                    regex: /^\d{3,4}-\d{7,8}$/,
                                    regexText: '请输入正确的办公室电话 XXX(X)-XXXXXXX(X)',
                                    msgTarget: 'under'
                                },
                                {
                                    xtype: 'textfield',
                                    anchor: '100%',
                                    name: "phone",
                                    fieldLabel: '手机号码 *',
                                    msgTarget: 'under',
                                    allowBlank: false,
                                    vtype: "phone",
                                    regex: /^1(?:3\d|4[4-9]|5[0-35-9]|6[67]|7[013-8]|8\d|9\d)\d{8}$/,
                                    emptyText: '手机号码不能为空',
                                    blankText: '手机号码不能为空',
                                    regexText: '请输入正确的手机号',
                                    vtypeText: "该手机号已经注册"
                                },
                                {
                                    xtype: 'textfield',
                                    anchor: '100%',
                                    name: "email",
                                    fieldLabel: '电子邮箱',
                                    vtype: 'email',
                                    vtypeText: '该电子邮箱已经注册',
                                    regexText: '请输入正确的电子邮箱',
                                    regex: /^([A-Za-z0-9])(\w)+@(\w)+(\.)(com|com\.cn|net|cn|net\.cn|org|biz|info|gov|gov\.cn|edu|edu\.cn)/,
                                    msgTarget: 'under'
                                },
                                {
                                    xtype: 'textfield',
                                    anchor: '100%',
                                    name: "idcard",
                                    regex: /^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$|^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}([0-9]|X)$/,
                                    regexText: '请输入正确的身份证号',
                                    msgTarget: 'under',
                                    vtype: 'cardid',
                                    vtypeText: '该身份证号已经注册',
                                    fieldLabel: '身份证号码'
                                },
                                {
                                    xtype: 'datefield',
                                    anchor: '100%',
                                    name: "birthday",
                                    format: 'Y年m月d日',
                                    fieldLabel: '出生年月',
                                    msgTarget: 'under'
                                },
                                {
                                    xtype: 'textfield',
                                    anchor: '100%',
                                    name: "other",
                                    fieldLabel: '其他'
                                }
                            ]
                        },
                        {
                            xtype: 'container',
                            dock: 'bottom',
                            padding: '10px',
                            layout: {
                                type: 'hbox',
                                pack: 'center'
                            },
                            items: [
                                {
                                    xtype: 'button',
                                    text: '注    册',
                                    listeners: {
                                        'click': function () {
                                            var form = Ext.getCmp('registerform');
                                            var thisForm = form.getForm();
                                            if (thisForm.isValid()) {
                                                thisForm.submit({
                                                    url: '/service/user/register.ashx',
                                                    success: function (form, action) {
                                                        Ext.MessageBox.alert("提示信息", action.result.msg, function (id, msg) {

                                                        }
                                                        );
                                                    },
                                                    failure: function (form, action) {
                                                        Ext.MessageBox.alert("提示信息", action.result.msg);
                                                        var pwd1 = Ext.getCmp('password1');
                                                        pwd1.setValue('');
                                                        var pwd2 = Ext.getCmp('password2');
                                                        pwd2.setValue('');
                                                    }

                                                });
                                            }
                                            else {
                                                var pwd1 = Ext.getCmp('password1');
                                                pwd1.setValue('');
                                                var pwd2 = Ext.getCmp('password2');
                                                pwd2.setValue('');
                                            }
                                        }
                                    }

                                },
                                {
                                    xtype: 'label',
                                    padding: '10px',
                                    text: '                      '
                                },
                                {
                                    xtype: 'button',
                                    text: '返    回',
                                    listeners: {
                                        'click': function () {
                                            self.location = "../Login/Login";
                                        }
                                    }
                                }
                            ]
                        }
                    ]
                }]
            }
        ]

    });
}