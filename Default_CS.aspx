<%@ Page Language="C#" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>2FA proof of concept</title>
</head>
<body>
    <form id="aspnetForm" runat="server">
        <asp:PlaceHolder ID="plcLoginArea" runat="server">User name:
            <asp:TextBox ID="txtUserName" runat="server" />
            <br />
            Password:
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
            <br />
            <asp:Button ID="btnSubmitLogin" runat="server" Text="Log in" OnClick="btnSubmitLogin_Click" />
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plcTokenEntry" runat="server" Visible="false">Enter Token:
            <asp:TextBox ID="txtToken" runat="server" />
            <br />
            <asp:Button ID="btnTokenEntry" runat="server" Text="Submit" OnClick="btnTokenEntry_Click" />
        </asp:PlaceHolder>
    </form>
</body>
</html>
