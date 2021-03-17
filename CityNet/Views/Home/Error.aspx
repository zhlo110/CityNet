<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="/js/error.js" type="text/javascript"></script>
    <script>
        Ext.onReady(function () {
            setupui();
        });
  </script>
</asp:Content>
