<%@ Page Title="Produção Diária Prevista / Realizada" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ProducaoDiaria.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.ProducaoDiaria" %>

<%@ Register src="../../Controls/ctrlData.ascx" tagname="ctrlData" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <script type="text/javascript">
        var postData;
        
        function imprimir()
        {
            var data = FindControl("ctrlData_txtData", "input").value;
            var imagem = FindControl("chtPrevisaoProducao", "img");

            var canvas = document.createElement("canvas");
            canvas.width = imagem.width;
            canvas.height = imagem.height;
            
            var ctx = canvas.getContext("2d");
            ctx.drawImage(imagem, 0, 0);
            
            imagem = canvas.toDataURL("image/jpeg");
            openWindow(600, 800, "RelBase.aspx?postData=getPostData()");

            postData = new Object();
            postData["rel"] = "ProducaoDiaria";
            postData["data"] = data;
            postData["imagem"] = imagem;
        }

        function getPostData()
        {
            return postData;
        }
    </script>
    <div class="filtro">
        <div>
            <span>
                <asp:Label ID="Label1" runat="server" Text="Data Fábrica" AssociatedControlID="ctrlData"></asp:Label>
                <uc1:ctrlData ID="ctrlData" runat="server" />
                <asp:ImageButton ID="imgPesq" runat="server" 
                    ImageUrl="~/Images/Pesquisar.gif" CssClass="botaoPesquisar" 
                    onclick="imgPesq_Click" />
            </span>
        </div>
    </div>
    <table>
        <tr>
            <td>
                <asp:GridView ID="grdProducaoDiaria" runat="server" AutoGenerateColumns="false" 
                    CssClass="gridStyle" AlternatingRowStyle-CssClass="alt" GridLines="None">
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
            <td>
                <asp:Chart ID="chtPrevisaoProducao" runat="server" Width="900px" />
            </td>
        </tr>
    </table>
    <br />
    <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="imprimir(); return false">
        <img src="../../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
</asp:Content>

