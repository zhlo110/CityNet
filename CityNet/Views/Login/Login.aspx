<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="/js/login.js" type="text/javascript"></script>
    <script>
        Ext.USE_NATIVE_JSON = false;
        Ext.onReady(function () {
            setupui();
        });
    </script>
</asp:Content>

