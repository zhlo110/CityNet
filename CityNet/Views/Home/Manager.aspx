<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="/js/define.js" type="text/javascript"></script>
    <script src="/js/functiontype.js" type="text/javascript"></script>
    <script src="/js/utility.js" type="text/javascript"></script>
    <script src="/js/backgroundprogress.js" type="text/javascript"></script>
    <script src="/js/comboboxtree.js" type="text/javascript"></script>
    <script src="/js/checkboxtree.js" type="text/javascript"></script>
    <script src="/js/treeeditor.js" type="text/javascript"></script>
    <script src="/js/validate.js" type="text/javascript"></script>
    <script src="/js/role.js" type="text/javascript"></script>
    <script src="/js/user_role.js" type="text/javascript"></script>
    <script src="/js/manager.js" type="text/javascript"></script>
    <script src="/js/department_user.js" type="text/javascript"></script>
    <script src="/js/resource_eidt.js" type="text/javascript"></script>
    <script src="/js/project_ui.js" type="text/javascript"></script>
    <script src="/js/table_scheme.js" type="text/javascript"></script>
    <script src="/js/alarm_scheme.js" type="text/javascript"></script>
    <script src="/js/ProjectSession/projectsession.js" type="text/javascript"></script>
    <script src="/js/ProjectSession/sessionworker.js" type="text/javascript"></script>
    <script src="/js/task_to_department.js" type="text/javascript"></script>
    <script>
        Ext.onReady(function () {
            var usermsg = '<%=ViewBag.Message%>'
            var managerParams = '<%=ViewBag.Params%>'
            setupui(usermsg, managerParams);
        });
  </script>

</asp:Content>
