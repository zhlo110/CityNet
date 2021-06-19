function task_to_department(params) {
	var funpanel = Ext.getCmp('function_area_id');
	funpanel.removeAll();
	var searchlock = false;
	Ext.Ajax.request({
		url: '/service/user/get_userinfomation.ashx?params=' + params,
		success: function (form, action) {
			var userjson = Ext.decode(form.responseText);//当前用户信息
			

			var treestore = new Ext.data.TreeStore(
            {
            	autoSync: false,
            	proxy: {
            		type: 'ajax',
            		url: '/service/projectsession/build_department_user_tree.ashx?params=' + params,
            		extraParams: {
            			sessionid: -1,
            			taskid:-1,
						showleaves:"false"
            		},
            		reader: 'json'
            	},
            	root: {
            		name: '根节点',
            		id: 'project_id'
            	}
            });



			var datastore = Ext.create('Ext.data.Store', {
				model: 'Task',
				pageSize: 15,
				proxy: {
					type: 'ajax',
					url: '/service/task/get_available_task.ashx?params=' + params,
					extraParams: {
						mode: 0
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
			funpanel.addListener('resize', function (panel, width, height, oldWidth, oldHeight, eOpts) {
				var childpanel = Ext.getCmp('workedchild_id');
				childpanel.setHeight(height - 35);
				var panel1 = Ext.getCmp('workflow_gird_id');
				panel1.setWidth(width / 2);
			});
			var destorypanel = Ext.getCmp('workflow_gird_id');
			if (!Ext.isEmpty(destorypanel)) {
				destorypanel = null;
			}
			var panel = Ext.create({
				xtype: 'panel',
				layout: 'border',
				id: 'workedchild_id',
				minHeight: 1,
				minWidth: 1,
				height: funpanel.getHeight() - 35,
				width: funpanel.getWidth(),
				border: false,
				resizable: true,
				items: [
                    {
                    	xtype: 'gridpanel',
                    	title: '任务表（已审核）',
                    	id: 'workflow_gird_id',
                    	padding: '10 0 10 10',
                    	region: 'west',
                    	width: funpanel.getWidth() / 2,
                    	minWidth: 1,
                    	split: true,
                    	resizable: true,
                    	forceFit: true,
                    	store: datastore,
                    	columns: [
                             { header: 'ID', minWidth: 1, hidden: true, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'taskid' },
                             {
                             	header: '任务名', minWidth: 1, align: 'center', renderer: rendercell,
                             	sortable: false, menuDisabled: true, draggable: false, dataIndex: 'taskname'
                             },
                             {
                             	header: '任务创建时间', minWidth: 1, align: 'center', renderer: rendercell,
                             	sortable: false, menuDisabled: true, draggable: false, dataIndex: 'taskcreatedate'
                             },
                             { header: '提交人', align: 'center', renderer: rendercell, minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'creator' },
                             { header: '提交人单位', align: 'center', renderer: rendercell, minWidth: 1, sortable: false, menuDisabled: true, draggable: false, dataIndex: 'creatordep' },
                             {
                             	header: '审核人', minWidth: 1, align: 'center', renderer: rendercell,
                             	sortable: false, menuDisabled: true, draggable: false, dataIndex: 'auditor'
                             }
                    	],
                    	listeners: {
                    		rowclick: function (grid, record, element, rowIndex, e, eOpts) {
                    			treestore.proxy.extraParams.sessionid = -1;
                    			treestore.proxy.extraParams.taskid = record.data.taskid;
                    			treestore.proxy.extraParams.showleaves = "false"
                    			treestore.load();
                    		}
                    	},
                    	bbar: Ext.create('Ext.PagingToolbar', {
                    		store: datastore,
                    		displayInfo: true,
                    		displayMsg: '显示的条目 {0} - {1} of {2}',
                    		emptyMsg: "没有下载项目"
                    	})
                    },
                    {
                    	xtype: 'checkboxtree',
                    	id: 'user_role_tree_id',
                    	padding: '10 30 10 0',
                    	region: 'center',
                    	resizable: true,
                    	minHeight: 463,
                    	border: true,
                    	title: '部门组织树',
                    	store: treestore,
                    	settaskfunction: function () {

							//重建Task_Visible表，将该Taskid下的数据删除，在根据表重建立
                    		var gridpanel = Ext.getCmp('workflow_gird_id');
                    		var seletionnode = gridpanel.getSelection()[0];
                    		var me = this;
                    		Ext.Ajax.request({
                    			url: '/service/projectsession/set_task_by_department.ashx?params=' + params,
                    			params: {
                    				taskid: seletionnode.data.taskid
                    			},
                    			success: function (form, action) {
                    				var taskmsg = me.mask;
                    				if (!Ext.isEmpty(taskmsg)) {
                    					taskmsg.hide();
                    				}
                    			},
                    			failure: function (form, action) {

                    				var taskmsg = me.mask;
                    				if (!Ext.isEmpty(taskmsg)) {
                    					taskmsg.hide();
                    				}
                    			}
                    		});
                    	},
                    	savefunction: function (node, checkstate, uncheck) {
                    		var gridpanel = Ext.getCmp('workflow_gird_id');
                    		var seletionnode = gridpanel.getSelection()[0];
                    		var me = this;
                    		if (seletionnode == null) return;
                    		var checked = 0;
                    		if (uncheck) {
                    			checked = 1;//半选
                    		}
                    		else {
                    			if (checkstate) {
                    				checked = 2;
                    			}
                    			else {
                    				checked = 0;
                    			}
                    		}
                    		var text = node.data.text;
                    		var nodeid = node.data.id;

                    		me.submitcount = me.submitcount + 1;
                    		Ext.Ajax.request({
                    			url: '/service/projectsession/save_department_task.ashx?params=' + params,
                    			params: {
                    				checked: checked,
                    				departmentid: node.data.id,
                    				departmentname: node.data.text,
                    				taskid: seletionnode.data.taskid
                    			},
                    			success: function (form, action) {
                    				me.submitcount = me.submitcount - 1;
                    				if (me.submitcount <= 0) {
                    					me.settaskfunction();
                    				}
                    			},
                    			failure: function (form, action) {
                    				me.submitcount = me.submitcount - 1;
                    				if (me.submitcount <= 0) {
                    					me.settaskfunction();
                    				}
                    			}
                    		});
                    	}
                    }
				]
			});
			funpanel.add(panel);
		}
	});


}