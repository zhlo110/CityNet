<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="/js/define.js" type="text/javascript"></script>
    <script src="/js/authority.js" type="text/javascript"></script>
    <script src="/js/Document/pointdetail.js" type="text/javascript"></script>
    <script>

        Ext.onReady(function () {
            var usermsg = '<%=ViewBag.Message%>';
            var managerParams = '<%=ViewBag.Params%>';
            var pointid = '<%=ViewBag.PointID%>';
            pointview(usermsg, managerParams, pointid);
        });
  </script>
</asp:Content>