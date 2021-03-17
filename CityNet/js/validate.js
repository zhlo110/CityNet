var userisok = true;
var phoneisok = true;
var cardidisok = true;
var emailisok = true;
Ext.apply(Ext.form.VTypes, {
    password: function (val, field) {               //val指这里的文本框值，field指这个文本框组件，大家要明白这个意思  
        if (field.confirmTo) {                    //confirmTo是我们自定义的配置参数，一般用来保存另外的组件的id值  
            var pwd = Ext.getCmp(field.confirmTo);   //取得confirmTo的那个id的值  
            return (val == pwd.getValue());
        }
        return true;
    },
    greater:function (val, field) {
        if (field.confirmTo) {                   
            var pwd = Ext.getCmp(field.confirmTo);
            return parseFloat(val) > parseFloat(pwd.getValue());
        }
        return false;
    },
    smaller:function (val, field) {
        if (field.confirmTo) {                   
            var pwd = Ext.getCmp(field.confirmTo);   
            return parseFloat(val) < parseFloat(pwd.getValue());
        }
        return false;
    },

    emptyfield:function(val,field)
    {
        var temp = val.trim();
        return !Ext.isEmpty(temp);
    },

    degree: function (val, field)
    {
        var value = val.trim();
        var num = Number(value);
        if (isNaN(num)) {
            // 30°13′26.85″
            var vec = value.split('°');
            if (vec.length == 2) {
                var degree = vec[0];
                value = vec[1];
                vec = value.split('′');
                if (vec.length == 2) {
                    var minute = vec[0];
                    value = vec[1];
                    vec = value.split('″');
                    if (vec.length == 2) {
                        var second = vec[0];
                        if (!isNaN(Number(degree)) && !isNaN(Number(minute)) &&
                            !isNaN(Number(second)) && vec[1].length == 0 && degree.length > 0 && minute.length > 0 && second.length>0) {
                            value = Number(degree) + Number(minute) / 60 + Number(second)/ 360;
                            return value <= field.max && value >= field.min;
                        }
                        else {
                            return false;
                        }
                        //alert(second);
                    }
                    else {
                        return false;
                    }
                }
                else {
                    return false;
                }

            }
            else {
                return false;
            }
        }
        else {
            return num <= field.max && num >= field.min;
        }
    },

    docordocx: function (val, field)
    {
        var filename = val.trim();
        var index1 = filename.lastIndexOf(".");
        var index2 = filename.length;
        var type = filename.substring(index1, index2);
        return type == ".doc" || type == ".docx";
    },

    docdocxorpdf: function (val, field) {
        var filename = val.trim();
        var index1 = filename.lastIndexOf(".");
        var index2 = filename.length;
        var type = filename.substring(index1, index2);
        return type == ".doc" || type == ".docx" || type == ".pdf";
    },

    colname: function (val, field) {
        var gridpanel = Ext.getCmp(field.tablepanelid);
        var seletionnode = gridpanel.getSelection()[0];
        if (seletionnode != null) {
            Ext.Ajax.request(
                {
                    url: '/service/document/check_columnname.ashx?params=' + field.params,
                    reader: 'json',
                    params: {
                        colname: val,
                        schid: seletionnode.data.schid,
                        rowid: field.up().getRecord().data.rowid
                    },
                    success: function (response, options) {
                        if (response.responseText == "1") {
                            userisok = false;
                        }
                        else {
                            userisok = true;
                        }
                    },
                    failure: function (response, options) {
                        userisok = false;
                    }

                });
        }
        else {
            userisok = true;
        }
        return userisok;
    },

    username: function (val, field) {
        Ext.Ajax.request(
            {
                url: '/service/user/check_user.ashx',
                reader: 'json',
                params: { username: val },
                success: function (response, options) {
                    if (response.responseText == "1") {
                        userisok = false;
                    }
                    else {
                        userisok = true;
                    }
                },
                failure: function (response, options) {
                    userisok = false;
                }

            });
        return userisok;
    },
    phone: function (val, field) {
        Ext.Ajax.request(
            {
                url: '/service/user/check_phonenumber.ashx',
                reader: 'json',
                params: { phone: val },
                success: function (response, options) {
                    if (response.responseText == "1") {
                        phoneisok = false;
                    }
                    else {
                        phoneisok = true;
                    }
                },
                failure: function (response, options) {
                    phoneisok = false;
                }

            });
        return phoneisok;
    },
    cardid: function (val, field) {
        Ext.Ajax.request(
            {
                url: '/service/user/check_cardid.ashx',
                reader: 'json',
                params: { cardid: val },
                success: function (response, options) {
                    if (response.responseText == "1") {
                        cardidisok = false;
                    }
                    else {
                        cardidisok = true;
                    }
                },
                failure: function (response, options) {
                    cardidisok = false;
                }

            });
        return cardidisok;
    },


    email: function (val, field) {
    Ext.Ajax.request(
        {
            url: '/service/user/check_email.ashx',
            reader: 'json',
            params: { email: val },
            success: function (response, options) {
                if (response.responseText == "1") {
                    emailisok = false;
                }
                else {
                    emailisok = true;
                }
            },
            failure: function (response, options) {
                emailisok = false;
            }

        });
    return emailisok;
}

});