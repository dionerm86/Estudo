<%@ Page Title="Duplicar orçamento" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadDuplicarOrcamento.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadDuplicarOrcamento" %>

<%@ Register src="../Controls/ctrlBenef.ascx" tagname="ctrlBenef" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/RecalcularOrcamento.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">
        var duplicando = false;
        
        function setOrcamento(idOrcamento)
        {
            FindControl("txtIdOrcamento", "input").value = idOrcamento;
        }
        
        function duplicar()
        {
            if (duplicando)
                return;
            
            var idOrcamento = FindControl("txtIdOrcamento", "input").value;
            if (idOrcamento == "")
            {
                alert("Digite o número do orçamento.");
                return;
            }
            
            if (!confirm('Duplicar orçamento?'))
                return;
            
            duplicando = true;
            document.getElementById("loading").style.display = "";
            var resposta = CadDuplicarOrcamento.GetDadosOrcamento(idOrcamento).value.split(";");
            
            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                return;
            }
            
            var campoIdCliente = document.getElementById("<%= hdfIdCliente.ClientID %>");
            var campoPercComissao = document.getElementById("<%= hdfPercComissao.ClientID %>");
            var campoRevenda = document.getElementById("<%= hdfRevenda.ClientID %>");
            var campoTipoEntrega = document.getElementById("<%= hdfTipoEntrega.ClientID %>");
            
            campoIdCliente.value = resposta[1];
            campoPercComissao.value = resposta[2];
            campoRevenda.value = resposta[3];
            campoTipoEntrega.value = resposta[4];
            
            resposta = CadDuplicarOrcamento.Duplicar(idOrcamento).value.split(";");
            
            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                return;
            }
            
            if (recalcular(resposta[1]))
            {
                alert("Orçamento duplicado com sucesso! Novo orçamento: " + resposta[1]);
                redirectUrl("CadOrcamento.aspx?idorca=" + resposta[1]);
            }
        }
        
        function recalcular(idOrcamento)
        {
            var nomeControleBenef = "<%= ctrlBenef1.ClientID %>";
            var campoAltura = "<%= hdfBenefAltura.ClientID %>";
            var campoEspessura = "<%= hdfBenefEspessura.ClientID %>";
            var campoLargura = "<%= hdfBenefLargura.ClientID %>";
            var campoIdProd = "<%= hdfBenefIdProd.ClientID %>";
            var campoQtde = "<%= hdfBenefQtde.ClientID %>";
            var campoTotM = "<%= hdfBenefTotM.ClientID %>";
            var campoValorUnit = "<%= hdfBenefValorUnit.ClientID %>";
            var campoTipoEntrega = document.getElementById("<%= hdfTipoEntrega.ClientID %>");
           
            return recalcularOrcamento(idOrcamento, false, null, nomeControleBenef, campoAltura, campoEspessura,
                campoLargura, campoIdProd, campoQtde, campoTotM, campoValorUnit, campoTipoEntrega != undefined && campoTipoEntrega != null ? campoTipoEntrega.value : "");
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Número do orçamento
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdOrcamento" runat="server" Width="70px"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbPesq" runat="server" 
                                ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(600, 800, '../Utils/SelOrcamento.aspx'); return false" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnDuplicar" runat="server" Text="Duplicar" OnClientClick="duplicar(); return false;" />
                <br />
                <br />
                <img id="loading" src="../Images/load.gif" border="0" style="display: none" />
                <asp:HiddenField ID="hdfIdCliente" runat="server" />
                <asp:HiddenField ID="hdfPercComissao" runat="server" />
                <asp:HiddenField ID="hdfRevenda" runat="server" />
                <asp:HiddenField ID="hdfTipoEntrega" runat="server" />
            </td>
        </tr>
    </table>
    <div style="display: none">
        <uc1:ctrlBenef ID="ctrlBenef1" runat="server" OnLoad="ctrlBenef1_Load" />
        <asp:HiddenField ID="hdfBenefAltura" runat="server" />
        <asp:HiddenField ID="hdfBenefEspessura" runat="server" />
        <asp:HiddenField ID="hdfBenefLargura" runat="server" />
        <asp:HiddenField ID="hdfBenefIdProd" runat="server" />
        <asp:HiddenField ID="hdfBenefQtde" runat="server" />
        <asp:HiddenField ID="hdfBenefTotM" runat="server" />
        <asp:HiddenField ID="hdfBenefValorUnit" runat="server" />
    </div>
</asp:Content>

