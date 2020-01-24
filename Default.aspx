<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        div {
            margin: 5%;
        }
        input{
            margin: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="RideTB" placeholder="Ride ID" runat="server"></asp:TextBox>
            <br />
            <asp:TextBox ID="UserTB" placeholder="User ID" runat="server"></asp:TextBox>
            <br />

            <asp:Button ID="cancelBTN" runat="server" Text="Notify cancel" OnClick="cancelBTN_Click" />
        </div>

        <div>
            <asp:TextBox ID="TextBox3" placeholder="Ride ID" runat="server"></asp:TextBox>
            <br />
            

            <asp:Button ID="Button1" runat="server" Text="Notify BackupToPrimary" OnClick="backupToPrimary_Click" />
        </div>

        <div>
            <asp:TextBox ID="TextBox1" placeholder="Title" runat="server"></asp:TextBox>
            <br />
            <asp:TextBox ID="TextBox2" placeholder="Messege" runat="server"></asp:TextBox>
            <br />

            <asp:Button ID="globalBTN" runat="server" Text="Global messege" OnClick="globalBTN_Click" />
        </div>
    </form>
</body>
</html>
