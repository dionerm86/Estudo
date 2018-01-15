<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CobrancaCte.ascx.cs" Inherits="Glass.UI.Web.Controls.CTe.CobrancaCte" %>
<%@ Register Src="~/Controls/CTe/CobrancaDuplCte.ascx" TagName="cobrDupl" TagPrefix="uc1" %>
<uc1:cobrDupl ID="ctrlCobrancaDuplCte" runat="server" />

<script type="text/javascript">
    function setPlanoConta(idConta, descricao) {
        var planoConta = FindControl("drpPlanoContas", "select");

        if (planoConta == null)
            return false;

        planoConta.value = idConta;
    }
</script>

<div class="dtvRow">
    <div class="dtvHeader">
        <asp:Label ID="lblNumFatura" runat="server" Text="Número Fatura" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox ID="txtNumFatura" runat="server" MaxLength="60" Width="150px"></asp:TextBox>
    </div>
    <div class="dtvHeader">
        <asp:Label ID="lblValorOrigFatura" runat="server" Text="Valor Fatura" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox ID="txtValorOrigFatura" runat="server" MaxLength="20" onclick="mascaraValor(this, 2); return false;"
            Width="140px"></asp:TextBox>
    </div>
</div>
<div class="dtvRow">
    <div class="dtvHeader">
        <asp:Label ID="lblDescontoFatura" runat="server" Text="Desconto Fatura" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox ID="txtDescontoFatura" runat="server" MaxLength="20" onclick="mascaraValor(this, 2); return false;"
            Width="140px"></asp:TextBox>
    </div>
    <div class="dtvHeader">
        <asp:Label ID="lblValorLiquidoFatura" runat="server" Text="Valor Líquido Fatura"
            Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox ID="txtValorLiquidoFatura" runat="server" MaxLength="20" onclick="mascaraValor(this, 2); return false;"
            Width="140px"></asp:TextBox>
    </div>
</div>
<div class="dtvRow" runat="server" id="contasPagar">
    <div class="dtvHeader">
        <asp:Label ID="Label1" runat="server" Text="Contas a Pagar" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:CheckBox ID="chkGerarContasPagar" runat="server" Text="Gerar Contas a Pagar?" />
    </div>
    <div class="dtvHeader">
        <asp:Label ID="Label2" runat="server" Text="Plano de Contas *" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:DropDownList ID="drpPlanoContas" runat="server"
            AppendDataBoundItems="True" DataSourceID="odsPlanoContas"
            DataTextField="DescrPlanoGrupo" DataValueField="IdConta">
            <asp:ListItem Value="Selecione">Selecione um Plano de Contas</asp:ListItem>
        </asp:DropDownList>
        <asp:CompareValidator ID="cvdrpPlanoContas" runat="server" ControlToValidate="drpPlanoContas"
            ErrorMessage="Selecione um plano de contas" ValueToCompare="Selecione" Operator="NotEqual"
            ValidationGroup="c">*</asp:CompareValidator>
        <asp:LinkButton ID="lnkSelPlanoConta" runat="server" OnClientClick="openWindow(600, 700, '../Utils/SelPlanoConta.aspx'); return false;">
            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
    </div>
</div>
<sync:ObjectDataSource runat="server" ID="odsPlanoContas"
    SelectMethod="GetPlanoContasCte" TypeName="Glass.Data.DAL.PlanoContasDAO"
    UseDAOInstance="True">
    <SelectParameters>
        <asp:Parameter Name="idCte" Type="UInt32" />
    </SelectParameters>
</sync:ObjectDataSource>
