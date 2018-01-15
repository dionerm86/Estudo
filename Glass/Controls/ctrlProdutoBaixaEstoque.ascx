<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlProdutoBaixaEstoque.ascx.cs"
    Inherits="Glass.UI.Web.Controls.ctrlProdutoBaixaEstoque" %>
<%@ Register Src="ctrlSelProduto.ascx" TagName="ctrlSelProduto" TagPrefix="uc1" %>

<table runat="server" id="tabela" class="pos" cellpadding="1" cellspacing="1">
    <tr>
        <td>
            <asp:HiddenField ID="hdfIdProdBaixaEst" runat="server" ClientIDMode="Static"/>
            <uc1:ctrlSelProduto ID="ctrlSelProduto" runat="server" Callback="selProdutoCallback" class="pos" />
        </td>
        <td>
             Qtde.:
        </td>
        <td>
            <asp:TextBox ID="txtQtde" runat="server" onkeypress="return soNumeros(event, false, true)"
                Width="50px" onchange="selProdutoCallback(this.id, 0)"></asp:TextBox>
            <asp:CustomValidator ID="ctvValidaQtde" runat="server" ErrorMessage="*" ClientValidationFunction="validaQtdeProdutoBaixa"
                Display="Dynamic" ControlToValidate="txtQtde" ValidateEmptyText="True"></asp:CustomValidator>
        </td>
        <td class='<%# "tdProcApl_"+ this.ID %>'>Proc.:</td>
        <td class='<%# "tdProcApl_"+ this.ID %>'>
            <asp:TextBox ID="txtProc" runat="server" onkeypress="return !(isEnter(event));" Width="30px" ClientIDMode="Static"
                Enabled="false"></asp:TextBox>
            <asp:HiddenField runat="server" ID="hdfIdProc" ClientIDMode="Static"/>
        </td>
        <td class='<%# "tdProcApl_"+ this.ID %>'>
            <input type="image" id="" onclick="openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx?idControle=-' + this.id); return false;" src="../Images/Pesquisar.gif" />
        </td>
        <td class='<%# "tdProcApl_"+ this.ID %>'>Apl.:</td>
        <td class='<%# "tdProcApl_"+ this.ID %>'>
            <asp:TextBox ID="txtApl" runat="server" onkeypress="return !(isEnter(event));" Width="30px" ClientIDMode="Static"
                Enabled="false"></asp:TextBox>
            <asp:HiddenField runat="server" ID="hdfIdApl" ClientIDMode="Static"/>
        </td>
        <td class='<%# "tdProcApl_"+ this.ID %>'>
            <input type="image" id="" onclick="openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx?idControle=-' + this.id); return false;" src="../Images/Pesquisar.gif" />
        </td>
                <td class='<%# "tdAltLargProdBaixa_"+ this.ID %>'>Altura: </td>
        <td class='<%# "tdAltLargProdBaixa_"+ this.ID %>'>
            <asp:TextBox ID="txtAlturaProdBaixa" runat="server" onkeypress="return soNumeros(event, false, true)"
                Width="50px" onchange="selProdutoCallback(this.id, 0)"></asp:TextBox>
        </td>
        <td class='<%# "tdAltLargProdBaixa_"+ this.ID %>'>Largura: </td>
        <td class='<%# "tdAltLargProdBaixa_"+ this.ID %>'>
            <asp:TextBox ID="txtLarguraProdBaixa" runat="server" onkeypress="return soNumeros(event, false, true)"
                Width="50px" onchange="selProdutoCallback(this.id, 0)"></asp:TextBox>
        </td>
        <td class='<%# "tdAltLargProdBaixa_"+ this.ID %>'>Forma: </td>
        <td class='<%# "tdAltLargProdBaixa_"+ this.ID %>'>
            <asp:TextBox ID="txtFormaProdBaixa" runat="server" Width="50px" onchange="selProdutoCallback(this.id, 0)"></asp:TextBox>
        </td>
        <td>
            <asp:ImageButton ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" style="display: none" />
        </td>
        <td class='<%# "tdimgLimpar_"+ this.ID %>'>
            <input type="image" id="" onclick="procAmbiente = false; aplAmbiente = false; limparProcessoAplicacao(this); return false"; title="Limpar processo e aplicação"  src="../Images/eraser.png" />
        </td>
    </tr>
</table>

<asp:HiddenField ID="hdfIdsProdBaixaEst" runat="server" />
<asp:HiddenField ID="hdfIdProd" runat="server" />
<asp:HiddenField ID="hdfQtde" runat="server" />
<asp:HiddenField ID="hdfProc" runat="server" />
<asp:HiddenField ID="hdfApl" runat="server" />
<asp:HiddenField ID="hdfAlturaProdBaixa" runat="server" />
<asp:HiddenField ID="hdfLarguraProdBaixa" runat="server" />
<asp:HiddenField ID="hdfFormaProdBaixa" runat="server" />

<script>

    $(".tdProcApl_ctrlProdutoBaixaEstoque2").css("display", "none");
    $(".tdProcApl_ctrlProdutoBaixaEstoque1").css("display", "none");
    $(".tdimgLimpar_ctrlProdutoBaixaEstoque2").css("display", "none");
    $(".tdimgLimpar_ctrlProdutoBaixaEstoque1").css("display", "none");
    $(".tdAltLargProdBaixa_ctrlProdutoBaixaEstoque2").css("display", "none");
    $(".tdAltLargProdBaixa_ctrlProdutoBaixaEstoque1").css("display", "none");
</script>