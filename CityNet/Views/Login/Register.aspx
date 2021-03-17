<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="/js/validate.js" type="text/javascript"></script>
    <script src="/js/comboboxtree.js" type="text/javascript"></script>
    <script src="/js/register.js" type="text/javascript"></script>
    <script>
        Ext.Loader.setConfig({ enabled: true });
        Ext.setGlyphFontFamily('FontAwesome');
        Ext.require([
            'Ext.tree.*',
            'Ext.tab.*',
            'Ext.data.*',
            'Ext.view.View'
        ]);
        Ext.onReady(function () {
            setupui();
        });
  </script>

</asp:Content>
