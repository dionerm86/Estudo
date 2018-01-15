<%@ Page Title="Avisos do sistema" Language="C#" MasterPageFile="~/Layout.master"
    AutoEventWireup="true" CodeFile="AlertaAdministrador.aspx.cs" Inherits="Utils_AlertaAdministrador" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <asp:Label ID="lblPedido" runat="server" Text="Existe(m) pedido(s) pronto(s) e não entregue(s) a mais de 30 dias." Visible="false"></asp:Label>
    <br />
    <a id="linkPedidos" runat="server" href="../Relatorios/ListaPedidos.aspx?pedidosProntosNaoEntregues=true" visible="false">Visualizar pedidos</a>
    <br />
    <br />
    <asp:Label ID="lblBoleto" runat="server" Text="Existe(m) boleto(s) com data de vencimento para hoje." Visible="false"></asp:Label>
    <br />
    <a id="linkBoleto" runat="server" href="~/Cadastros/CadContaReceber.aspx?listarBoletosVencimentoHoje=true" visible="false">Visualizar boletos</a>
</asp:Content>
