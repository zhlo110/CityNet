<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="/leaflet/leaflet.css"/>
    <script src="/js/authority.js" type="text/javascript"></script>
    <script src="/js/validate.js" type="text/javascript"></script>
    <script src="/js/define.js" type="text/javascript"></script>
    <script src="/js/Resource/uploadwizard.js" type="text/javascript"></script>
    <script src="/js/index.js" type="text/javascript"></script>
    <script src="/js/checkboxtree.js" type="text/javascript"></script>
    <script src="/js/comboboxtree.js" type="text/javascript"></script>
    <script src="/js/backgroundprogress.js" type="text/javascript"></script>
    <script src="/leaflet/leaflet.js" type="text/javascript"></script>
    <script src="/leaflet/leaflet.ChineseTmsProviders.js" type="text/javascript"></script>
    <script src="/js/Resource/upload_point_sign.js" type="text/javascript"></script>
    <script src="/js/Resource/upload_point.js" type="text/javascript"></script>
    <script src="/js/Resource/point_approve.js" type="text/javascript"></script>
    <script src="/js/Resource/view_detail.js" type="text/javascript"></script>
    <script src="/js/Resource/task_ui.js" type="text/javascript"></script>
    <script src="/js/Resource/approvepanel.js" type="text/javascript"></script>
    <script src="/js/Resource/approve_guide_upload.js" type="text/javascript"></script>
    <script src="/js/Resource/supervisor.js" type="text/javascript"></script>
    <script src="/js/subway/subwaymap.js" type="text/javascript"></script>
    <script src="/js/subway/basepoint.js" type="text/javascript"></script>
    <script src="/js/treeeditor.js" type="text/javascript"></script>
    <script src="/js/subway/alarm_manager.js" type="text/javascript"></script>
    <script src="/js/subway/monitor_alarm.js" type="text/javascript"></script>
    <script src="/js/subway/monitor_manager.js" type="text/javascript"></script>
    <script src="/js/subway/metro_project_manager.js" type="text/javascript"></script>
    <script>


        Ext.Loader.setConfig({
            enabled: true,
            paths: {
                'resource': '../js/Resource'
            }
        });
        Ext.require('resource.*');

        Ext.onReady(function () {
            Ext.require('resource.WidgetGrid');
            var usermsg = '<%=ViewBag.Message%>'
            var managerUrl = '<%=ViewBag.Url%>'
            var params = '<%=ViewBag.Params%>'
            setupui(usermsg, managerUrl, params);
        });
  </script>

</asp:Content>

