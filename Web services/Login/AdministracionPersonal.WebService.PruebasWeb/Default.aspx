<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AdministracionPersonal.WebService.PruebasWeb._Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Prueba del servicio de autenticación (Core4)</title>
    <style type="text/css">
        body { font-family: Arial, Helvetica, sans-serif; margin: 20px; }
        .panel { border: 1px solid #ccc; padding: 12px; margin-bottom: 16px; }
        .row { margin-bottom: 8px; }
        label { display: inline-block; width: 160px; font-weight: bold; vertical-align: top; }
        input[type=text], input[type=password] { width: 320px; }
        .actions button, .actions input { margin-right: 8px; }
        .mensaje { margin: 12px 0; font-weight: bold; }
        .ok { color: #1a7f37; }
        .error { color: #b42318; }
        table { border-collapse: collapse; width: 100%; }
        th, td { border: 1px solid #ddd; padding: 6px 8px; text-align: left; }
        th { background: #f4f4f4; width: 200px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Core4 &mdash; Servicio de autenticación</h1>

        <div class="panel">
            <h2>Conexión</h2>
            <div class="row">
                <label for="txtServicioUrl">URL del servicio</label>
                <asp:TextBox ID="txtServicioUrl" runat="server" />
            </div>
        </div>

        <div class="panel">
            <h2>Credenciales</h2>
            <div class="row">
                <label for="txtUsuario">Usuario</label>
                <asp:TextBox ID="txtUsuario" runat="server" />
            </div>
            <div class="row">
                <label for="txtPassword">Contraseña</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
            </div>
            <div class="actions">
                <asp:Button ID="btnAutenticar" runat="server" Text="Autenticar" OnClick="btnAutenticar_Click" />
                <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" OnClick="btnLimpiar_Click" />
            </div>
            <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" />
        </div>

        <div class="panel">
            <h2>Resultado</h2>
            <table>
                <tr><th>Autenticado</th><td><asp:Literal ID="litAutenticado" runat="server" /></td></tr>
                <tr><th>Mensaje</th><td><asp:Literal ID="litMensajeServicio" runat="server" /></td></tr>
                <tr><th>Id usuario</th><td><asp:Literal ID="litIdUsuario" runat="server" /></td></tr>
                <tr><th>Nombre completo</th><td><asp:Literal ID="litNombreCompleto" runat="server" /></td></tr>
                <tr><th>Token</th><td><asp:Literal ID="litToken" runat="server" /></td></tr>
                <tr><th>Expira (UTC)</th><td><asp:Literal ID="litExpira" runat="server" /></td></tr>
            </table>
        </div>
    </form>
</body>
</html>
