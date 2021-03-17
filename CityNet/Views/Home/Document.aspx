<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="/js/Document/document.js" type="text/javascript"></script>
    <script>
        Ext.onReady(function () {
            var usermsg = '<%=ViewBag.Message%>';
            var managerParams = '<%=ViewBag.Params%>';
            var documentID = '<%=ViewBag.DocumentID%>';
            documentview(usermsg, managerParams, documentID);
            //  setupui(usermsg, managerParams);
        });
  </script>
</asp:Content>
