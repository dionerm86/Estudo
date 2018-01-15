<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowFoto.aspx.cs" Inherits="Glass.UI.Web.Utils.ShowFoto"
    Title="Imagem" MasterPageFile="~/Layout.master" %>
    
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>

<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <script type="text/javascript">
        function alteraVisibilidade(botao, exibir)
        {
            var itens = document.getElementById("tabelaPrincipal").rows;
            
            for (var i = 0; i < itens.length; i++)
                if (itens[i].id != "pagina" && itens[i].id != "rodape")
                    itens[i].style.display = exibir ? "" : "none";

            botao.style.display = exibir ? "" : "none";
        }
        
        function imprimir(botao)
        {
            alteraVisibilidade(botao, false);
            window.print();
            alteraVisibilidade(botao, true);
        }
    </script>
    <table align="center" width="100%">
        <tr>
            <td align="center">
                <asp:Image ID="imgFull" runat="server"  Width="100%"/>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" onclientclick="imprimir(this); return false;">
                    <img border="0" src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>