<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImpostoCte.ascx.cs" Inherits="Glass.UI.Web.Controls.CTe.ImpostoCte" %>

<div id="<%= this.ClientID %>" class="dtv-row-imposto">
    <div class="dtvHeader">
        CST <%= ObtemTextoCampoObrigatorio(cvdrpCst) %>
    </div>
    <div class="dtvAlternatingRow">
        <asp:DropDownList ID="drpCst" runat="server" Height="20px" Width="300px" AppendDataBoundItems="True"
            OnLoad="drpCst_Load" Enabled="true" Visible="true">
            <asp:ListItem Value="ND" Text="Selecione" />
        </asp:DropDownList>
        <asp:CompareValidator ID="cvdrpCst" ControlToValidate="drpCst" runat="server" ErrorMessage=""
            ValueToCompare="ND" Operator="NotEqual" Display="Dynamic" ValidationGroup="c" Enabled='<%# AtivarValidadores %>'>*</asp:CompareValidator>
    </div>
    <div id="valor-bc">
        <div class="dtvHeader">
            <asp:Label ID="Label3" runat="server" Text="Valor BC *" Font-Bold="True"></asp:Label>
        </div>
        <div class="dtvAlternatingRow">
            <div class="dtvAlternatingRow">
                <asp:TextBox ID="txtBC" runat="server" Width="140" MaxLength="20" onclick="mascaraValor(this, 2); return false;"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rftxtBC" ControlToValidate="txtBC" runat="server" ErrorMessage=""
                    Display="Dynamic" ValidationGroup="c">*</asp:RequiredFieldValidator>
            </div>
        </div>
    </div>
    <div id="perc-red-bc">
        <div class="dtvHeader">
            <asp:Label ID="Label1" runat="server" Text="Percentual Red. BC *" Font-Bold="True"></asp:Label>
        </div>
        <div class="dtvAlternatingRow">
            <div class="dtvAlternatingRow">
                <asp:TextBox ID="txtPercRedBC" runat="server" MaxLength="6" onclick="mascaraValor(this, 2); return false;"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rftxtPercRedBC" ControlToValidate="txtPercRedBC" runat="server" ErrorMessage=""
                    Display="Dynamic" ValidationGroup="c">*</asp:RequiredFieldValidator>
            </div>
        </div>
    </div>
    <div id="aliquota">
        <div class="dtvHeader">
            <asp:Label ID="Label4" runat="server" Text="Alíquota *" Font-Bold="True"></asp:Label>
        </div>
        <div class="dtvAlternatingRow">
            <div class="dtvAlternatingRow">
                <asp:TextBox ID="txtAliquota" runat="server" MaxLength="6" onclick="mascaraValor(this, 2); return false;"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rftxtAliquota" ControlToValidate="txtAliquota" runat="server" ErrorMessage=""
                    Display="Dynamic" ValidationGroup="c">*</asp:RequiredFieldValidator>
            </div>
        </div>
    </div>
    <div id="valor-imposto">
        <div class="dtvHeader">
            <asp:Label ID="Label5" runat="server" Text="Valor *" Font-Bold="True"></asp:Label>
        </div>
        <div class="dtvAlternatingRow">
            <div class="dtvAlternatingRow">
                <asp:TextBox ID="txtValorImposto" runat="server" Width="140" MaxLength="20" onclick="mascaraValor(this, 2); return false;"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rftxtValorImposto" ControlToValidate="txtValorImposto" runat="server" ErrorMessage=""
                    Display="Dynamic" ValidationGroup="c">*</asp:RequiredFieldValidator>
            </div>
        </div>
    </div>
    <div id="bc-st-retido">
        <div class="dtvHeader">
            <asp:Label ID="Label6" runat="server" Text="BC ST Retido *" Font-Bold="True"></asp:Label>
        </div>
        <div class="dtvAlternatingRow">
            <div class="dtvAlternatingRow">
                <asp:TextBox ID="txtBCSTRetido" runat="server" Width="140" MaxLength="20" onclick="mascaraValor(this, 2); return false;"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rftxtBCSTRetido" ControlToValidate="txtBCSTRetido" runat="server" ErrorMessage=""
                    Display="Dynamic" ValidationGroup="c">*</asp:RequiredFieldValidator>
            </div>
        </div>
    </div>
    <div id="aliquota-st-retido">
        <div class="dtvHeader">
            <asp:Label ID="Label7" runat="server" Text="Alíquota ST Retido *" Font-Bold="True"></asp:Label>
        </div>
        <div class="dtvAlternatingRow">
            <div class="dtvAlternatingRow">
                <asp:TextBox ID="txtAliquotaStRetido" runat="server" MaxLength="6" onclick="mascaraValor(this, 2); return false;"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rftxtAliquotaStRetido" ControlToValidate="txtAliquotaStRetido" runat="server" ErrorMessage=""
                    Display="Dynamic" ValidationGroup="c">*</asp:RequiredFieldValidator>
            </div>
        </div>
    </div>
    <div id="valor-st-retido">
        <div class="dtvHeader">
            <asp:Label ID="Label8" runat="server" Text="Valor ST Retido *" Font-Bold="True"></asp:Label>
        </div>
        <div class="dtvAlternatingRow">
            <div class="dtvAlternatingRow">
                <asp:TextBox ID="txtValorSTRetido" runat="server" MaxLength="20" onclick="mascaraValor(this, 2); return false;"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rftxtValorSTRetido" ControlToValidate="txtValorSTRetido" runat="server" ErrorMessage=""
                    Display="Dynamic" ValidationGroup="c">*</asp:RequiredFieldValidator>
            </div>
        </div>
    </div>
    <div id="valor-credito">
        <div class="dtvHeader">
            <asp:Label ID="Label9" runat="server" Text="Valor Crédito" Font-Bold="True"></asp:Label>
        </div>
        <div class="dtvAlternatingRow">
            <div class="dtvAlternatingRow">
                <asp:TextBox ID="txtValorCredito" runat="server" MaxLength="20" onclick="mascaraValor(this, 2); return false;"></asp:TextBox>
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function() {
        <%= ExibirControlesImposto(true) %>;
    });
</script>