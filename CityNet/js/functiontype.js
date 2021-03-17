//定义功能树，功能组的类型
var functiontypelist = Ext.create('Ext.data.Store', {
    fields: ['id', 'name', 'description'],
    data: [
        { 'id': 1, 'name': '管理面板导航树', 'description': '显示在管理界面的导航树中' },
        { 'id': 2, 'name': '界面按钮', 'description': '控制按钮是否显示' },
        { 'id': 3, 'name': '数据查看功能', 'description': '控制用户查看数据的范围' },
        { 'id': 4, 'name': '主页导航栏', 'description': '控制主页导航栏的显示与隐藏' },
        { 'id': 5, 'name': '功能控制', 'description': '用于控制平台函数的某些功能' }
    ]
});