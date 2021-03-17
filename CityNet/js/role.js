
function log_panel_information(params)
{
    var funpanel = Ext.getCmp('function_area_id');
    funpanel.removeAll();
	Ext.Ajax.request({
		url: '/service/user/get_userinfomation.ashx?params=' + params,
		success: function (form, action) {
		    var json = Ext.decode(form.responseText);
			
		    var form = Ext.create(
			{
			    xtype: 'panel',
			    layout: 'border',
                items:[{
                xtype: 'form',
                region: 'north',
			    id:'updatapasswordformid',
			    height: 300,
			    border: false,
			    title: '',
			    layout: {
			        type: 'vbox',
			        align: 'middle',
			        pack: 'center'
			    },
			    items: [
				{
				    xtype: 'panel',
				    layout: 'anchor',
				    bodyPadding: '20px',
				    border: false,
				    title: '',
				    defaults:
					{
					    padding: 10
					},
				    items: [
					{
					    xtype: 'textfield',
					    fieldLabel: '用户名',
					    name: 'username',
					    width: 600,
					    border: false,
					    msgTarget: 'under',
					    value: json.username,
						
					    editable: false
					},
					{
					    xtype: 'textfield',
					    fieldLabel: '已登录次数',
					    name: 'times',
					    width: 600,
					    border: false,
					    value: json.logtimes,
					    editable: false
					},

					{
					    xtype: 'textfield',
					    id: 'oldpasswordid',
					    name: 'oldpassword',
					    regex: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[\s\S]{6,16}$/,
					    fieldLabel: '原始密码',
					    msgTarget: 'under',
					    inputType: 'password',
					    width: 600,
					    allowBlank: false,
					    blankText: '密码不能为空',
					    regexText: '密码有6-16位，包括至少1个大写字母，1个小写字母，1个数字',
					    emptyText: '密码不能为空'
					},

					{
					    xtype: 'textfield',
					    id: 'password1',
					    name: 'newpassword1',
					    regex: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[\s\S]{6,16}$/,
					    fieldLabel: '新的密码',
					    msgTarget: 'under',
					    inputType: 'password',
					    width: 600,
					    allowBlank: false,
					    blankText: '密码不能为空',
					    regexText: '密码有6-16位，包括至少1个大写字母，1个小写字母，1个数字',
					    emptyText: '密码不能为空'
					},
					{
					    xtype: 'textfield',
					    id: 'password2',
					    name: 'newpassword2',
					    fieldLabel: '再次输入密码',
					    msgTarget: 'under',
					    inputType: 'password',
					    vtype: "password",
					    width: 600,
					    allowBlank: false,
					    blankText: '密码不能为空，且两次密码输入必须一致',
					    emptyText: '密码不能为空，且两次密码输入必须一致',
					    vtypeText: "两次密码不一致！",
					    confirmTo: "password1",//要比较的另外一个的组件的id  
					}
				    ]
				},
				{
				    xtype: 'button',
				    width: 70,
				    text: '修改密码',
				    handler: function () {
				        var form = Ext.getCmp('updatapasswordformid');
				        var thisForm = form.getForm();
				        if (thisForm.isValid()) {
				            thisForm.submit({
				                url: '/service/user/updata_password.ashx?params=' + params,
				                success: function (form, action) {
				                    Ext.MessageBox.alert("提示信息", action.result.msg, function (id, msg)
				                    {
				                        if (id == 'ok') {
				                            self.location = "../Login/Login";
				                        }
				                    }
									);
				                },
				                failure: function (form, action) {

				                    var mes = Ext.decode(action.response.responseText);
				                    Ext.MessageBox.alert("提示信息", mes.msg);

				                    Ext.getCmp('oldpasswordid').setValue('');
				                    Ext.getCmp('password1').setValue('');
				                    Ext.getCmp('password2').setValue('');

				                    //alert(form);
				                }
				            });
				        }
				    }
				}]
			}]});
			
			funpanel.add(form);
		}
	});
		
}
function basic_panel_information(params)
{
    var funpanel = Ext.getCmp('function_area_id');
    funpanel.removeAll();
	Ext.Ajax.request({
		url: '/service/user/get_userinfomation.ashx?params=' + params,
		success: function (form, action) {
			var json = Ext.decode(form.responseText);

			var form = Ext.create(
			{
			    xtype: 'panel',
			    layout: 'border',
			    items: [{
			        xtype: 'form',
			        region: 'north',
			        id: 'updatapersoninfo',
			        height: 500,
			        border: false,
			        title: '',
			        layout: {
			            type: 'vbox',
			            align: 'middle',
			            pack: 'center'
			        },
			        items: [
                    {
                        xtype: 'panel',
                        layout: 'anchor',
                        bodyPadding: '20px',
                        border: false,
                        title: '',
                        defaults:
                        {
                            padding: 10
                        },
                        items: [
                            {
                                xtype: 'textfield',
                                anchor: '100%',
                                name: "realname",
                                fieldLabel: '真实姓名',
                                msgTarget: 'under',
                                value: json.realname,
                                allowBlank: false,
                                emptyText: '真实姓名不能为空',
                                width: 600,
                                blankText: '真实姓名不能为空',
                                regex: /^([\u4e00-\u9fa5]{1,20}|[a-zA-Z\.\s]{1,20})$/,
                                regexText: '必须为真实姓名'

                            },
                            {
                                xtype: 'comboboxtree',
                                anchor: '100%',
                                fieldLabel: '部门',
                                name: "departmentform",
                                id: "departmentformid",
                                value: json.department,
                                submitValue: json.departmentID,
                                width: 600,
                                msgTarget: 'under',
                                allowBlank: false,
                                selectNodeModel: 'exceptRoot',
                                editable: false,
                                blankText: '部门不能为空',
                                url: '/service/user/build_department_tree.ashx'
                            },
                            {
                                xtype: 'textfield',
                                width: 600,
                                name: "occupation",
                                value: json.occupation,
                                anchor: '100%',
                                fieldLabel: '职位'
                            },
                            {
                                xtype: 'radiogroup',
                                name: 'gender',
                                id: 'genderid',
                                height: '',
                                width: 600,
                                fieldLabel: '性别',
                                items: [
                                    {
                                        name: 'sex',
                                        id: 'sex_male_id',
                                        boxLabel: '男',
                                        inputValue: 1
                                    },
                                    {
                                        name: 'sex',
                                        id: 'sex_famale_id',
                                        boxLabel: '女',
                                        inputValue: 2
                                    }
                                ]
                            },
                            {
                                xtype: 'textfield',
                                anchor: '100%',
                                fieldLabel: '办公室电话',
                                value: json.officephone,
                                name: "officephone",
                                width: 600,
                                regex: /^\d{3,4}-\d{7,8}$/,
                                regexText: '请输入正确的办公室电话 XXX(X)-XXXXXXX(X)',
                                msgTarget: 'under'
                            },
                            {
                                xtype: 'textfield',
                                anchor: '100%',
                                fieldLabel: '手机号码',
                                name: "phone",
                                value: json.phone,
                                width: 600,
                                msgTarget: 'under',
                                allowBlank: false,
                                regex: /^1(?:3\d|4[4-9]|5[0-35-9]|6[67]|7[013-8]|8\d|9\d)\d{8}$/,
                                emptyText: '手机号码不能为空',
                                blankText: '手机号码不能为空',
                                regexText: '请输入正确的手机号'
                            },
                            {
                                xtype: 'textfield',
                                anchor: '100%',
                                fieldLabel: '电子邮箱',
                                width: 600,
                                name: "email",
                                value: json.email,
                                regexText: '请输入正确的电子邮箱',
                                regex: /^([A-Za-z0-9])(\w)+@(\w)+(\.)(com|com\.cn|net|cn|net\.cn|org|biz|info|gov|gov\.cn|edu|edu\.cn)/,
                                msgTarget: 'under'
                            },
                            {
                                xtype: 'textfield',
                                width: 600,
                                anchor: '100%',
                                regex: /^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$|^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}([0-9]|X)$/,
                                regexText: '请输入正确的身份证号',
                                msgTarget: 'under',
                                name: "idcard",
                                value: json.idcard,
                                fieldLabel: '身份证号码'
                            },
                            {
                                xtype: 'datefield',
                                anchor: '100%',
                                name: "birthday",
                                format: 'Y年m月d日',
                                width: 600,
                                value: json.birthday,
                                fieldLabel: '出生年月',
                                msgTarget: 'under'
                            },
                            {
                                xtype: 'textfield',
                                width: 600,
                                anchor: '100%',
                                name: "other",
                                value: json.other,
                                fieldLabel: '其他'
                            }
                        ]
                    },
                    {
                        xtype: 'button',
                        width: 100,
                        text: '修改基本信息',
                        handler: function () {

                            var form = Ext.getCmp('updatapersoninfo');
                            var thisForm = form.getForm();
                            if (thisForm.isValid()) {

                                var comboxtree = Ext.getCmp('departmentformid');

                                thisForm.submit({
                                    url: '/service/user/update_person_info.ashx?params=' + params + "&department=" + comboxtree.submitValue,
                                    success: function (form, action) {
                                        Ext.MessageBox.alert("提示信息", action.result.msg, function (id, msg) {
                                            if (id == 'ok') {
                                                self.location = "../Login/Login";
                                            }
                                        }
                                        );
                                    },
                                    failure: function (form, action) {
                                        var mes = Ext.decode(action.response.responseText);
                                        Ext.MessageBox.alert("提示信息", mes.msg);
                                    }
                                });
                            }
                        }
                    }]
			    }]
			});
			Ext.getCmp('sex_male_id').setValue((json.sex == 1));
			Ext.getCmp('sex_famale_id').setValue((json.sex != 1));
			Ext.getCmp('departmentformid').submitValue = json.departmentID;
			
			funpanel.add(form);
		}
	});
}

function functiontree_manage(params) {
    var funpanel = Ext.getCmp('function_area_id');
    funpanel.removeAll();
    var searchlock = false;
    Ext.Ajax.request({
        url: '/service/user/get_userinfomation.ashx?params=' + params,
        success: function (form, action) {
            var json = Ext.decode(form.responseText);
            treestore = new Ext.data.TreeStore(
            {
                autoSync: false,
                id: 'function_tree_store',
                fields: ['text', 'actiontype','priority','description'],
                proxy: {
                    type: 'ajax',
                    url: '/service/function/build_function_folder.ashx?params=' + params,
                    reader: 'json'
                },
                root: {
                    name: '根节点',
                    id: 'project_id'
                },
                sorters:[
                    {
                        property: 'qtip',
                        direction: 'desc'
                    }
                ],
                listeners: {
                    nodemove: function (node, oldParent, newParent, index, eOpts) {
                        if (oldParent.id == newParent.id) {
                            return;
                        }
                        else {
                            //更改节点父节点
                            Ext.Ajax.request({
                                url: '/service/function/update_function_parent.ashx?params=' + params,
                                params: {
                                    funid: node.id,
                                    parentid: newParent.id
                                },
                                success: function (form, action) {
                                    //  var errorjson = Ext.decode(form.responseText);
                                    //  Ext.MessageBox.alert("提示信息", errorjson.msg);
                                    treestore.load();

                                },
                                failure: function (form, action) {
                                    //删除
                                    var errorjson = Ext.decode(form.responseText);
                                    Ext.MessageBox.alert("提示信息", errorjson.msg);
                                    treestore.load();
                                }

                            });
                        }
                        //alert(store);
                    }
                }
            });

            


            var datastore = Ext.create('Ext.data.Store', {
                model: 'functionInfo',
                pageSize: 20,
                proxy: {
                    type: 'ajax',
                    url: '/service/function/getfunctionbyclass.ashx?params=' + params,
                    reader: {
                        type: 'json',
                        rootProperty: 'roots',
                        totalProperty: 'totalCount'
                    },
                    autoLoad: true
                }
            });
            datastore.loadPage(1);

            funpanel.addListener('resize', function (panel, width, height, oldWidth, oldHeight, eOpts) {

                var childpanel = Ext.getCmp('func_workedchild_id');
                childpanel.setHeight(height - 35);

                var panel1 = Ext.getCmp('sel_function_treepanel_id_0');
                panel1.setWidth(width / 2);

            });


            var panel = Ext.create({
                xtype: 'panel',
                layout: 'border',
                id:'func_workedchild_id',
                minHeight: 300,
                minWidth: 1,
                height: funpanel.getHeight() - 35,
                width: funpanel.getWidth(),
                border: false,
                resizable: true,
                items: [
                    {
                        xtype: 'treepanel',
                        id: 'sel_function_treepanel_id_0',
                        region: 'west',
                        width: funpanel.getWidth() / 2,
                        minWidth: 1,
                        split: true,
                        resizable: true,
                        padding: '10 0 10 10',
                        minHeight: 300,
                        border: true,
                        scrollable: true,
                        viewConfig: {
                            preserveScrollOnRefresh: true,//
                            preserveScrollOnReload: true,//
                            scrollable: 'y',
                            plugins: {
                                ptype: "treeviewdragdrop",
                                containerScroll: true,
                                appendOnly: true
                            }
                        },
                        columns: [
                            {
                                xtype: 'treecolumn',
                                flex: 1,
                                dataIndex: 'text',
                                editor: { xtype: 'textfield', allowBlank: false, emptyText: '名称不能为空', blankText: '名称不能为空' },
                                text: '名称'
                            },
                            {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'actiontype',
                                renderer : function (value, metaData, record, rowIndex, colIndex, store, view) {
                                    return functiontypelist.data.items[value - 1].data.name;
                                },
                                editor: {
                                    xtype: 'combobox',
                                    store: functiontypelist,
                                    id: 'functiontype_combox_id',
                                    queryMode: 'local',
                                    displayField: 'name',
                                    valueField: 'id',
                                    hiddenName: 'name',
                                    selectOnFocus: true,
                                    triggerAction: 'all',
                                    allowBlank: false,

                                    emptyText: '类型不能为空',
                                    listeners:
                                    {
                                        change: function( combo, newValue, oldValue, eOpts )
                                        {
                                            combo.setValue(newValue);
                                           // alert(oldValue);
                                        }
                                    },
                                    blankText: '类型不能为空'
                                },
                                text: '功能类型'
                            },
                            {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'priority',
                                editor: {
                                    xtype: 'textfield', allowBlank: false,
                                    regex: /^[0-9]*[1-9][0-9]*$/,
                                    regexText: '排序号必须为大于0的正整数',
                                    blankText: '排序号不能为空', emptyText: '排序号不能为空'
                                },
                                text: '排序'
                            },
                            {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'createtime',
                                text: '创建日期'
                            },
                            {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'creater',
                                text: '创建人'
                            },
                            {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'description',
                                editor: { xtype: 'textfield'},
                                text: '功能描述'
                            }
                        ],
                        title: '功能树目录',
                        store: treestore,
                        listeners:
                        {
                            itemclick: function (panel, record, item, index, e, eOpts){
                                datastore.proxy.url = '/service/function/getfunctionbyclass.ashx?params=' + params+'&groupid='+record.data.qtip;
                                datastore.loadPage(1);
                            }
                        },
                        plugins: [
                            {
                                ptype: 'bufferedrenderer',
                                trailingBufferZone: 20,  // Keep 20 rows rendered in the table behind scroll
                                leadingBufferZone: 50   // Keep 50 rows rendered in the table ahead of scroll
                            },
                            {
                                id: 'rowediting_plugin_id',
                                ptype: 'treeediting',
                                saveBtnText: '保存',
                                cancelBtnText: "取消",
                                clicksToMoveEditor: 1,
                                listeners: {
                                    edit: function (editor, context, eOpts) {
                                     //   var combox = Ext.getCmp('functiontype_combox_id');
                                       // alert();
                                        // alert(context.record.data.actiontype);
                                        //提交
                                        Ext.Ajax.request({
                                            url: '/service/function/insert_function_group.ashx?params=' + params,
                                            params: {
                                                funid: context.record.data.id,
                                                funname: context.record.data.text,
                                                priority:context.record.data.priority,
                                                funtype: context.record.data.actiontype,
                                                fundescription: context.record.data.description
                                            },
                                            success: function (form, action) {
                                                var errorjson = Ext.decode(form.responseText);
                                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                treestore.load();

                                            },
                                            failure: function (form, action) {
                                                //删除
                                                var errorjson = Ext.decode(form.responseText);
                                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                var functionpanel = Ext.getCmp('sel_function_treepanel_id_0');
                                                var RootNode = functionpanel.getRootNode();
                                                var newnode = RootNode.findChild('id', 'new_item_id');
                                                if (newnode != null) {
                                                    RootNode.removeChild(newnode);
                                                }
                                            }

                                        });

                                        //  alert(+","+);
                                    },
                                    canceledit: function (editor, context, eOpts) {
                                        var functionpanel = Ext.getCmp('sel_function_treepanel_id_0');
                                        var RootNode = functionpanel.getRootNode();
                                        var newnode = RootNode.findChild('id', 'new_item_id');
                                        if (newnode != null) {
                                            RootNode.removeChild(newnode);
                                            return;
                                        }

                                    }
                                }
                            }],
                        dockedItems: [{
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                                {
                                    xtype: 'button',
                                    iconCls: 'button add',
                                    text: '添加',
                                    handler: function () {
                                        var functionpanel = Ext.getCmp('sel_function_treepanel_id_0');
                                        functionpanel.getView().editingPlugin.cancelEdit();
                                        var RootNode = functionpanel.getRootNode();
                                        var newnode = RootNode.findChild('id', 'new_item_id');
                                        if (newnode == null) {
                                            var curDate = new Date();
                                            newnode = RootNode.insertChild(0, { id: 'new_item_id', text: '', priority:1,actiontype:1,expanded: true, createtime: Ext.Date.format(curDate, 'Y年m月d日'), creater: json.realname, description: '', leaf: false });
                                            functionpanel.getSelectionModel().select(newnode);
                                            functionpanel.getView().editingPlugin.startEdit(newnode);
                                        }
                                    }
                                },
                                {
                                    xtype: 'button',
                                    iconCls: 'button delete',
                                    text: '删除',
                                    handler: function () {
                                        var functionpanel = Ext.getCmp('sel_function_treepanel_id_0');
                                        functionpanel.getView().editingPlugin.cancelEdit();
                                        var selectNode = functionpanel.selection;
                                        if (selectNode == null) {
                                            Ext.MessageBox.alert("提示信息", '当前选择的节点为空，请选择要删除的节点');
                                        }
                                        else {
                                            //  alert(selectNode);

                                            Ext.Ajax.request({
                                                url: '/service/function/check_functionhaschild.ashx?params=' + params,
                                                params: {
                                                    funid: selectNode.id,
                                                },
                                                success: function (form, action) {
                                                    var rightjson = Ext.decode(form.responseText);
                                                    if (rightjson.haschildren) {
                                                        Ext.MessageBox.confirm("提示", "该功能目录树中含有功能，是否要删除？", function (btnId) {
                                                            if (btnId == "yes") {
                                                                Ext.Ajax.request({
                                                                    url: '/service/function/delete_function_group.ashx?params=' + params,
                                                                    params: {
                                                                        funid: selectNode.id,
                                                                    },
                                                                    success: function (form, action) {
                                                                        var errorjson = Ext.decode(form.responseText);
                                                                        Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                                        treestore.load();
                                                                    },
                                                                    failure: function (form, action) {
                                                                        var errorjson = Ext.decode(form.responseText);
                                                                        Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                                    }
                                                                });
                                                            }
                                                        });

                                                    }
                                                    else {
                                                        Ext.Ajax.request({
                                                            url: '/service/function/delete_function_group.ashx?params=' + params,
                                                            params: {
                                                                funid: selectNode.id,
                                                            },
                                                            success: function (form, action) {
                                                                var errorjson = Ext.decode(form.responseText);
                                                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                                treestore.load();
                                                            },
                                                            failure: function (form, action) {
                                                                var errorjson = Ext.decode(form.responseText);
                                                                Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                            }
                                                        });
                                                    }
                                                },
                                                failure: function (form, action) {
                                                    var errorjson = Ext.decode(form.responseText);
                                                    Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                }
                                            });
                                        }
                                    }
                                },
                                {
                                    xtype: 'button',
                                    iconCls: 'button moveup',
                                    text: '移动到顶层',
                                    handler: function () {
                                        var functionpanel = Ext.getCmp('sel_function_treepanel_id_0');
                                        functionpanel.getView().editingPlugin.cancelEdit();
                                        var node = functionpanel.selection;
                                        if (node != null) {
                                            //更改节点父节点
                                            Ext.Ajax.request({
                                                url: '/service/function/update_function_parent.ashx?params=' + params,
                                                params: {
                                                    funid: node.id,
                                                    parentid: 'parent_id_0'
                                                },
                                                success: function (form, action) {
                                                    treestore.load();

                                                },
                                                failure: function (form, action) {
                                                    //删除
                                                    var errorjson = Ext.decode(form.responseText);
                                                    Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                    treestore.load();
                                                }

                                            });
                                        }
                                        else {
                                            Ext.MessageBox.alert("提示信息", '当前选择的节点为空，请选择要移动的节点');
                                        }
                                    }
                                },
                                {
                                    xtype: 'label',
                                    text: '注意：可以通过拖拽移动节点'
                                }
                            ]
                        }],
                        rootVisible: false
                    },
                    {
                        xtype: 'gridpanel',
                        id:"function_gridpanel_id",
                        padding: '10 30 10 0',
                        region: 'center',
                        resizable: true,
                        
                        title: '功能列表',
                        forceFit: true,
                        minHeight: 300,
                        store: datastore,
                        columns: [
                            { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'functionid' },
                            {
                                header: '选择', minWidth: 1, width: 40, sortable: false, menuDisabled: true, draggable: false, xtype: 'checkcolumn', dataIndex: 'functioncheck',
                                listeners:
                                {
                                    checkchange: function (pan, rowIndex, checked, record, eOpts) {
                                        var functionpanel = Ext.getCmp('sel_function_treepanel_id_0');
                                        var node = functionpanel.selection;
                                        if (node != null) {

                                            Ext.Ajax.request({
                                                url: '/service/function/change_function_class.ashx?params=' + params,
                                                params: {
                                                    classid:node.data.qtip,
                                                    ischeck:checked,
                                                    functionid:record.data.functionid
                                                },
                                                success: function (form, action) {},
                                                failure: function (form, action) {
                                                    var errorjson = Ext.decode(form.responseText);
                                                    Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                }
                                            });
                                            //checked
                                            //  record.data.functionid
                                            // alert(node.data.qtip);
                                        }
                                        
                                    }
                                }
                            },
                            { header: '功能名称', minWidth: 1, editor: { xtype: 'textfield', allowBlank: false, emptyText: '名称不能为空', blankText: '名称不能为空' }, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'functionname' },
                            { header: '功能函数', minWidth: 1, editor: { xtype: 'textfield', allowBlank: false, emptyText: '功能函数为空', blankText: '功能函数不能为空' }, width: 150, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'functionurl' },
                            {
                                header: '排序', minWidth: 1, editor:
                                  {
                                      xtype: 'textfield', allowBlank: false,
                                      regex: /^[0-9]*[1-9][0-9]*$/,
                                      regexText: '排序号必须为大于0的正整数',
                                      emptyText: '排序号不能为空', blankText: '排序号不能为空'
                                  },
                                   width: 50, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'priority'
                            },
                            { header: '创建日期', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'functiondate' },
                            { header: '创建人', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'functiondcreator' },
                            { header: '描述', minWidth: 1, editor: { xtype: 'textfield' }, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'description' }
                        ],
                        dockedItems: [{
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                                {
                                    xtype: 'button',
                                    iconCls: 'button add',
                                    text: '添加',
                                    handler: function () {
                                        var gridpanel = Ext.getCmp('function_gridpanel_id');
                                        gridpanel.getPlugin('editingplugin_id').cancelEdit();
                                        var gridstore = gridpanel.getStore();
                                        var index = gridstore.find('functionid', -1);
                                        if (index < 0) {
                                            var curDate = new Date();
                                            var newnode = gridstore.insert(0, {
                                                functionid: -1, functioncheck: false,priority:1,
                                                functionname: '', functionurl: '', functiondate: Ext.Date.format(curDate, 'Y年m月d日'), functiondcreator: json.realname, description: ''
                                            });
                                            gridpanel.getSelectionModel().select(newnode);
                                            gridpanel.getPlugin('editingplugin_id').startEdit(gridpanel.getSelection()[0]);
                                        }
                                      //  gridpanel.getView().editingPlugin.startEdit(newnode);

                                    }
                                },
                                {
                                    xtype: 'button',
                                    iconCls: 'button delete',
                                    text: '删除',
                                    handler: function () {
                                        var gridpanel = Ext.getCmp('function_gridpanel_id');
                                        gridpanel.getPlugin('editingplugin_id').cancelEdit();
                                        var gridstore = gridpanel.getStore();
                                        var seletionnode = gridpanel.getSelection()[0];
                                        if (seletionnode != null) {
                                            //seletionnode.data.functionid;


                                            Ext.MessageBox.confirm("提示", "是否要删除功能'" + seletionnode.data.functionname + "'？", function (btnId) {
                                                if (btnId == "yes") {
                                                    Ext.Ajax.request({
                                                        url: '/service/function/delete_function.ashx?params=' + params,
                                                        params: {
                                                            functionid: seletionnode.data.functionid
                                                        },
                                                        success: function (form, action) {
                                                            var errorjson = Ext.decode(form.responseText);
                                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                            gridloadcurrentPage(datastore);
                                                            // datastore.loadPage(1);
                                                        },
                                                        failure: function (form, action) {
                                                            var errorjson = Ext.decode(form.responseText);
                                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                        }
                                                    });
                                                }
                                            });

                                           
                                           // seletionnode.data.functionid

                                        }
                                    }
                                },
                                {
                                    xtype: 'checkbox',
                                    id:'checkboxallid',
                                    text: '显示全部',
                                    listeners: {
                                        change: function (box, newValue, oldValue, eOpts) {
                                            if (searchlock) return;
                                            searchlock = true;
                                            var gridpanel = Ext.getCmp('function_gridpanel_id');
                                            gridpanel.getPlugin('editingplugin_id').cancelEdit();
                                            var gridstore = gridpanel.getStore();
                                            var textfield = Ext.getCmp('searchbox_id');
                                            textfield.setValue('');
                                            gridstore.clearFilter();
                                            if (newValue) {
                                                gridstore.filter('functioncheck', true);
                                            }
                                            searchlock = false;
                                        }
                                    }

                                },
                                {
                                    xtype: 'label',
                                    text: '仅显示选中功能'
                                },
                                { xtype: 'tbspacer', width: 50 },
                                '-',
                                {
                                    xtype: 'label',
                                    text: '搜索'
                                },
                                
                                {
                                    xtype: 'textfield',
                                    id:'searchbox_id',
                                    text: '搜索',
                                    listeners: {
                                        change: function (textfield, newValue, oldValue, eOpts) {
                                            //alert(newValue);
                                            if (searchlock) return;
                                            searchlock = true;
                                            var gridpanel = Ext.getCmp('function_gridpanel_id');
                                            var checkbox = Ext.getCmp('checkboxallid');
                                            checkbox.setValue(false);
                                            gridpanel.getPlugin('editingplugin_id').cancelEdit();
                                            var gridstore = gridpanel.getStore();
                                            var filters = gridstore.getFilters();
                                            gridstore.clearFilter();

                                            gridstore.filterBy(function (record, id) {
                                                //newValue
                                                var v = newValue.trim();
                                                if (record.data.functionname.indexOf(v) >= 0 || v.length == 0) {
                                                    return true;
                                                }
                                                else {
                                                    return false;
                                                }
                                            });
                                            searchlock = false;
                                        }
                                    }
                                }
                            ]
                        }],
                        plugins: {
                            ptype: 'treeediting',
                            clicksToEdit: 2,
                            pluginId: 'editingplugin_id',
                            saveBtnText: '保存',
                            cancelBtnText: "取消",
                            listeners: {
                                edit: function (editor, context, eOpts) {

                                    Ext.Ajax.request({
                                        url: '/service/function/update_function_info.ashx?params=' + params,
                                        params: {
                                            functionid: context.record.data.functionid,
                                            description: context.record.data.description,
                                            functionurl: context.record.data.functionurl,
                                            priority: context.record.data.priority,
                                            functionname: context.record.data.functionname
                                        },
                                        success: function (form, action) {
                                            var json = Ext.decode(form.responseText);
                                            Ext.MessageBox.alert("提示信息", json.msg);
                                            datastore.loadPage(datastore.currentPage);
                                        },
                                        failure: function (form, action) {
                                            var errorjson = Ext.decode(form.responseText);
                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                            datastore.loadPage(datastore.currentPage);
                                        }
                                    });
                                    //      context.record.data.functionname,context.record.data.description,
                                },
                                canceledit: function (editor, context, eOpts) {

                                    var gridpanel = Ext.getCmp('function_gridpanel_id');
                                    var gridstore = gridpanel.getStore();
                                    var index = gridstore.find('functionid', -1);
                                    if (index >=0 ) {
                                        gridstore.removeAt(index);
                                        return;
                                    }
                                }
                            },
                        },
                        bbar: Ext.create('Ext.PagingToolbar', {
                            store: datastore,
                            displayInfo: true,
                            displayMsg: '显示的条目 {0} - {1} of {2}',
                            emptyMsg: "没有下载项目"
                        })
                    }
                ]
            });
            funpanel.add(panel);
            datastore.loadPage(1);
        }
    });
}

function group_manager(params) {
    var funpanel = Ext.getCmp('function_area_id');
    var treeexpandlock = false;
	funpanel.removeAll();
	Ext.Ajax.request({
	    url: '/service/user/get_userinfomation.ashx?params=' + params,
	    success: function (form, action) {
	        var userjson = Ext.decode(form.responseText);
	        treestore = new Ext.data.TreeStore(
            {
                autoSync: false,
                id: 'role_structure_store',
                fields: ['text', 'description'],
                proxy: {
                    type: 'ajax',
                    url: '/service/function/build_all_function_tree.ashx?roleid=0&params=' + params,
                    reader: 'json'
                },
                root: {
                    name: '根节点',
                    id: 'project_id'
                }
            });

	        var datastore = Ext.create('Ext.data.Store', {
	            model: 'roleInfo',
	            pageSize: 15,
	            proxy: {
	                type: 'ajax',
	                url: '/service/function/get_allrole.ashx?params=' + params,
	                reader: {
	                    type: 'json',
	                    rootProperty: 'roots',
	                    totalProperty: 'totalCount'
	                },
	                autoLoad: true
	            }
	        });
            datastore.loadPage(1);
            funpanel.addListener('resize', function (panel, width, height, oldWidth, oldHeight, eOpts) {

                var childpanel = Ext.getCmp('role_orkedchild_id');
                childpanel.setHeight(height - 35);

                var panel1 = Ext.getCmp('role_gridpanel_id');
                panel1.setWidth(width / 2);

            });
            

	        var panel = Ext.create({
	            xtype: 'panel',
                layout: 'border',
                minHeight: 1,
                minWidth: 1,
                height: funpanel.getHeight() - 35,
                width: funpanel.getWidth(),
                border: false,
                id:'role_orkedchild_id',
	            items: [
                    {
                        xtype: 'gridpanel',
                        id: "role_gridpanel_id",
                        padding: '10 0 10 10',
                        region: 'west',
                        width: funpanel.getWidth() / 2,
                        split: true,
                        resizable: true,
                        title: '角色列表',
                        minHeight: 300,
                        forceFit: true,
                        store: datastore,
                        columns: [
                            { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'roleid' },
                            {
                                header: '角色名称（英文）', minWidth: 1,
                                editor: { xtype: 'textfield', allowBlank: false, emptyText: '名称（英文）不能为空', blankText: '名称（英文）不能为空' },
                                width: 160, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'rolename'
                            },
                            {
                                header: '角色名称（中文）', minWidth: 1,
                                editor: { xtype: 'textfield', allowBlank: false, emptyText: '名称（中文）不能为空', blankText: '名称（中文）不能为空' },
                                width: 160, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'realname'
                            },
                            { header: '创建人', minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'rolecreator' },
                            {
                                header: '描述', minWidth: 1,
                                editor: { xtype: 'textfield' },
                                sortable: false, menuDisabled: true, draggable: false, dataIndex: 'description'
                            }
                        ],
                        dockedItems: [{
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                                {
                                    xtype: 'button',
                                    iconCls: 'button add',
                                    text: '添加',
                                    handler: function () {
                                        var gridpanel = Ext.getCmp('role_gridpanel_id');
                                        gridpanel.getPlugin('role_editor_plugin_id').cancelEdit();
                                        var gridstore = gridpanel.getStore();
                                        var index = gridstore.find('roleid', -1);
                                        if (index < 0) {
                                            var curDate = new Date();
                                            var newnode = gridstore.insert(0, {
                                                roleid: -1, rolename: '', realname: '', rolecreator: userjson.realname, description: ''
                                            });
                                            gridpanel.getSelectionModel().select(newnode);
                                            gridpanel.getPlugin('role_editor_plugin_id').startEdit(gridpanel.getSelection()[0]);
                                        }
                                        //  gridpanel.getView().editingPlugin.startEdit(newnode);

                                    }
                                },
                                {
                                    xtype: 'button',
                                    iconCls: 'button delete',
                                    text: '删除',
                                    handler: function () {
                                        var gridpanel = Ext.getCmp('role_gridpanel_id');
                                        gridpanel.getPlugin('role_editor_plugin_id').cancelEdit();
                                        var gridstore = gridpanel.getStore();
                                        var seletionnode = gridpanel.getSelection()[0];
                                        if (seletionnode != null) {
                                            //seletionnode.data.functionid;


                                            Ext.MessageBox.confirm("提示", "是否要删除角色'" + seletionnode.data.realname + "'？", function (btnId) {
                                                if (btnId == "yes") {
                                                    Ext.Ajax.request({
                                                        url: '/service/function/delete_role.ashx?params=' + params,
                                                        params: {
                                                            roleid: seletionnode.data.roleid
                                                        },
                                                        success: function (form, action) {
                                                            var errorjson = Ext.decode(form.responseText);
                                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                            gridloadcurrentPage(datastore);
                                                            //datastore.loadPage(1);
                                                        },
                                                        failure: function (form, action) {
                                                            var errorjson = Ext.decode(form.responseText);
                                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                                        }
                                                    });
                                                }
                                            });


                                            // seletionnode.data.functionid

                                        }
                                    }
                                },
                                { xtype: 'tbspacer', width: 50 },
                                '->',
                                {
                                    xtype: 'label',
                                    text: '搜索'
                                },

                                {
                                    xtype: 'textfield',
                                    id: 'searchbox_id',
                                    text: '搜索',
                                    listeners: {
                                        change: function (textfield, newValue, oldValue, eOpts) {
                                            //alert(newValue);
                                     
                                            var gridpanel = Ext.getCmp('role_gridpanel_id');
                                            gridpanel.getPlugin('role_editor_plugin_id').cancelEdit();
                                            var gridstore = gridpanel.getStore();
                                            var filters = gridstore.getFilters();
                                            gridstore.clearFilter();

                                            gridstore.filterBy(function (record, id) {
                                                //newValue
                                                var v = newValue.trim();
                                                if (record.data.realname.indexOf(v) >= 0 || v.length == 0) {
                                                    return true;
                                                }
                                                else {
                                                    return false;
                                                }
                                            });
                                        
                                        }
                                    }
                                }
                            ]
                        }],
                        plugins: {
                            ptype: 'treeediting',
                            clicksToEdit: 2,
                            pluginId: 'role_editor_plugin_id',
                            saveBtnText: '保存',
                            cancelBtnText: "取消",
                            listeners: {
                                edit: function (editor, context, eOpts) {

                                    Ext.Ajax.request({
                                        url: '/service/function/add_update_role.ashx?params=' + params,
                                        params: {
                                            roleid: context.record.data.roleid,
                                            rolename: context.record.data.rolename,
                                            realname: context.record.data.realname,
                                            description: context.record.data.description
                                        },
                                        success: function (form, action) {
                                            var json = Ext.decode(form.responseText);
                                            Ext.MessageBox.alert("提示信息", json.msg);
                                            datastore.loadPage(1);
                                        },
                                        failure: function (form, action) {
                                            var errorjson = Ext.decode(form.responseText);
                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                            datastore.loadPage(1);
                                        }
                                    });
                                    //      context.record.data.functionname,context.record.data.description,
                                },
                                canceledit: function (editor, context, eOpts) {

                                    var gridpanel = Ext.getCmp('role_gridpanel_id');
                                    var gridstore = gridpanel.getStore();
                                    var index = gridstore.find('roleid', -1);
                                    if (index >= 0) {
                                        gridstore.removeAt(index);
                                        return;
                                    }
                                }
                            },
                        },
                        bbar: Ext.create('Ext.PagingToolbar', {
                            store: datastore,
                            displayInfo: true,
                            displayMsg: '显示的条目 {0} - {1} of {2}',
                            emptyMsg: "没有下载项目"
                        }),
                        listeners:
                        {
                            itemclick: function (panel, record, item, index, e, eOpts) {
                                treestore.proxy.url = '/service/function/build_all_function_tree.ashx?roleid=' + record.data.roleid+'&params=' + params;
                                treestore.load();
                            }
                        }
                    },
                    {
                        xtype: 'treepanel',
                        id: 'all_function_tree_id',
                        padding: '10 30 10 0',
                        region: 'center',
                        resizable: true,
                        minHeight: 300,
                        scrollable: true,
                        scrollable: 'y',
                        viewConfig: {
                            preserveScrollOnRefresh: true,//
                            preserveScrollOnReload: true,//
                            scrollable: 'y'
                        },
                        plugins: [
                        {
                            ptype: 'bufferedrenderer',
                            trailingBufferZone: 20,  // Keep 20 rows rendered in the table behind scroll
                            leadingBufferZone: 50   // Keep 50 rows rendered in the table ahead of scroll
                        }],
                        border: true,
                        columns: [
                            {
                                xtype: 'treecolumn',
                                flex: 1,
                                dataIndex: 'text',
                                editor: { xtype: 'textfield', allowBlank: false, emptyText: '名称不能为空', blankText: '名称不能为空' },
                                text: '名称'
                            },
                            {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'createtime',
                                text: '创建日期'
                            },
                            {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'creater',
                                text: '创建人'
                            },
                            {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'description',
                                editor: { xtype: 'textfield' },
                                text: '描述'
                            }
                        ],
                        title: '功能目录',
                        store: treestore,

                        dockedItems: [{
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                                '->',
                                {
                                    xtype: 'checkbox',
                                    text: '显示全部',
                                    listeners: {
                                        change: function (box, newValue, oldValue, eOpts) {
                                            var treepanel = Ext.getCmp('all_function_tree_id');
                                            var treestore = treepanel.getStore();
                                            treestore.clearFilter();
                                            if (newValue) {
                                                treestore.filterBy(function (record, id) {
                                                    //newValue
                                                    if (record.data.leaf) {
                                                        return record.data.checked;
                                                    }
                                                    else {
                                                        return true;
                                                    }
                                                });
                                            }
                                        }
                                    }

                                },
                                {
                                    xtype: 'label',
                                    text: '仅显示选中功能'
                                }
                            ]
                        }],
                        listeners:
                        {
                            checkchange: function (node, checked, eOpts) {
                                if (Ext.getCmp('role_gridpanel_id').selection != null) {
                                    var functionid = node.data.qtip;
                                    var roleid = Ext.getCmp('role_gridpanel_id').selection.data.roleid;
                                    var actionurl = ""
                                    if (checked) {
                                        actionurl = '/service/function/insert_action_group.ashx?params=' + params;
                                    }
                                    else {
                                        actionurl = '/service/function/delete_action_group.ashx?params=' + params;
                                    }

                                    Ext.Ajax.request({
                                        url: actionurl,
                                        params: {
                                            groupid: roleid,
                                            actoinid: functionid
                                        },
                                        success: function (form, action) {
                                            //treestore.load();
                                        },
                                        failure: function (form, action) {
                                            var errorjson = Ext.decode(form.responseText);
                                            Ext.MessageBox.alert("提示信息", errorjson.msg);
                                            treestore.load();
                                        }
                                    });
                                }

                            }
                        },
                        rootVisible: false
                    }

	            ]
	        });
	        funpanel.add(panel);
	        datastore.loadPage(1);

	    },
	    failure: function (form, action) {
	    }
	});

}