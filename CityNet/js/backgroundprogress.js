Ext.define('Ext.ux.BackProgressBar', {
    extend: 'Ext.ProgressBar',
    alias: ['widget.backprogress'],
    getcurrenturl: '',
    createurl: '',
    getinfourl: '',
    deleteurl: '',
    uniqueid: '',
    isover:function(func)
    {
        uniqueid = this.uniqueid;
        Ext.Ajax.request(
        {
            url: this.getinfourl,
            reader: 'json',
            params: {
                uniqueid: this.uniqueid
            },
            success: function (response, options) {
                var mes = Ext.decode(response.responseText);
                if (uniqueid != '') {
                    if(mes.over)
                    {
                        func();
                    }
                    else
                    {
                        Ext.MessageBox.alert("提示信息",'数据正在下载，请勿重复点击');
                    }
                }
                else {
                    func();
                }
            },
            failure: function (response, options) {
                var mes = Ext.decode(response.responseText);
                Ext.MessageBox.alert("提示信息", mes.msg);
             //   me.deletebar(uniqueid);
            }
        });
    },
    deletebar:function(uniqueid)
    {
        Ext.Ajax.request(
       {
           url: this.deleteurl,
           reader: 'json',
           params: {
               uniqueid: this.uniqueid
           },
           success: function (response, options) {
           },
           failure: function (response, options) {
           }
       });
    },

    start:function()
    {
        this.show();
        var me = this;
        task = {
            run: function () {
                var metask = this;
              Ext.Ajax.request(
              {
                  url: me.getcurrenturl,
                  reader: 'json',
                  params: {
                      uniqueid: me.uniqueid
                  },
                  success: function (response, options) {
                      var mes = Ext.decode(response.responseText);
                      if (mes.over) {
                          Ext.TaskManager.stop(metask);
                      }
                      else {
                          var current = mes.current;
                          var total = mes.total;
                          if (total > 0) {
                              var text = mes.showtext;
                              var percentage = (current / total);
                              processText = text;
                              me.updateProgress(percentage, processText);
                          }
                          //me.setValue(mes.current)
                      }
                  },
                  failure: function (response, options) {
                  }
              });
            },
            interval: 1000
        };
        Ext.TaskManager.start(task);
    },
    createbar: function (text,total,des,func) {
        //创建进度条
        var me = this;
        this.isover(function () {
            Ext.Ajax.request(
            {
                url: me.createurl,
                reader: 'json',
                params: {
                    showtext: text,
                    total: total,
                    description: des
                },
                success: function (response, options) {
                    var mes = Ext.decode(response.responseText);
                    me.uniqueid = mes.uniqueid;
                    
                    func(mes.uniqueid);
                    //this.uniqueid
                },
                failure: function (response, options) {
                    var mes = Ext.decode(response.responseText);
                    Ext.MessageBox.alert("提示信息", mes.msg);
                    //me.deletebar();
                }
            });
        });
    }
});