<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="T2.Cms.Extend.SSO.Demo.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            SessionKey: 
            <asp:TextBox ID="tb_sessionKey" runat="server" /><br />
            SessionSecret: 
            <asp:TextBox ID="tb_sessionSecret" runat="server" /><br />
            <asp:Button runat="server" OnClick="btn_Click" Text="验证" /><br />
            <br />

            <asp:LinkButton ID="btn_logout" runat="server" target="_blank" Text=">>用户退出" />

            <br />
            <div style="padding: 10px; border: solid 1px #ff6600; background:orange; color: black">
                <asp:Label ID="lb_result" runat="server" Text="输入并点击验证按钮" />
            </div>
        </div>
    </form>
</body>
</html>
