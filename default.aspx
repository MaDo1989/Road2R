<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="_default" %>
<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Road2R._default" %>--%>
<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="SignalR_WebForms._default" %>--%>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SignalR Using webForms</title>

    <script src="Scripts/jquery-3.6.0.js"></script>

    <script src="Scripts/jquery.signalR-2.4.2.js"></script>

    <script src="signalr/hubs"></script>


    <script type="text/javascript">

        $(function() {

            var logger = $.connection.logHub;

            logger.client.logMessage = function(msg) {

                $("#logUl").append("<li>" + msg + "</li>");

            };

            $.connection.hub.start();

        });

    </script>

</head>
<body>
    <form id="form1" runat="server">
    <div>

    <h3>Log Items</h3>
    <asp:listview id="logListView" runat="server" itemplaceholderid="itemPlaceHolder" clientidmode="Static" enableviewstate="false">
        <layouttemplate>
            <ul id="logUl">
                <li runat="server" id="itemPlaceHolder"></li>
            </ul>
        </layouttemplate>
        <itemtemplate>
                <li><span class="logItem"><%#Container.DataItem.ToString() %></span></li>
        </itemtemplate>
    </asp:listview>



    </div>



    </form>
</body>
</html>