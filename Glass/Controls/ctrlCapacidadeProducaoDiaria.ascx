<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlCapacidadeProducaoDiaria.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlCapacidadeProducaoDiaria" %>

<%@ Register src="ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>

<div id="<%= this.ClientID %>" style="display: inline-block">
    <asp:Repeater ID="rptPrincipal" runat="server">
        <ItemTemplate>
            <table cellpadding="0" cellspacing="0" class="pos" width="100%">
                <tr>
                    <td width="100%">
                        <asp:Label ID="lblData" runat="server" Font-Size="1.3em"
                            ForeColor="Blue" Text='<%# Bind("Data", "{0:d}") %>'></asp:Label>
                    </td>
                    <td style="padding-left: 8px; white-space: nowrap; text-align: right">
                        <asp:ImageButton ID="imgEditar" runat="server" ToolTip="Editar" CausesValidation="false"
                            ImageUrl="~/Images/EditarGrid.gif" OnClientClick='<%# this.ClientID + ".Editar(); return false" %>' />
                        <asp:ImageButton ID="imgAtualizar" runat="server" ToolTip="Atualizar" CausesValidation="true" 
                            ValidationGroup='<%# this.ClientID %>' ImageUrl="~/Images/Ok.gif" style="display: none"
                            OnClientClick='<%# this.ClientID + ".Atualizar(); return false" %>' />
                        <asp:ImageButton ID="imgCancelar" runat="server" ToolTip="Cancelar" CausesValidation="false"
                            ImageUrl="~/Images/ExcluirGrid.gif" style="display: none"
                            OnClientClick='<%# this.ClientID + ".Cancelar(); return false" %>' />
                        <uc1:ctrlLogPopup ID="ctrlLogPopup" runat="server" Tabela="CapacidadeProducaoDiaria" 
                            IdRegistro='<%# Eval("CodigoParaLog") %>' />
                    </td>
                </tr>
                <tr runat="server" visible='<%# ExibirMaximoVendas() %>'>
                    <td width="100%" colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr runat="server" visible='<%# ExibirMaximoVendas() %>'>
                    <td width="100%">
                        <asp:Label ID="Label1" runat="server" Text="Máximo Vendas"></asp:Label>
                    </td>
                    <td style="padding-left: 8px; white-space: nowrap; text-align: right">
                        <asp:Label ID="lblMaximoVendas" runat="server" Text='<%# Eval("MaximoVendasM2") %>'></asp:Label>
                        <asp:TextBox ID="txtMaximoVendas" runat="server" Width="40px" style="display: none"
                            onkeypress="return soNumeros(event, true, true)" Text='<%# Bind("MaximoVendasM2") %>'
                            ValidationGroup='<%# this.ClientID %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvMaximoVendas" runat="server" ErrorMessage="*"
                            ControlToValidate="txtMaximoVendas" Display="Dynamic" ValidationGroup='<%# this.ClientID %>'></asp:RequiredFieldValidator>
                        m²
                    </td>
                </tr>
                <tr runat="server" visible='<%# ExibirCapacidadeSetor() %>'>
                    <td width="100%" colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr runat="server" visible='<%# ExibirCapacidadeSetor() %>'>
                    <td width="100%" colspan="2">
                        <asp:Label ID="Label2" runat="server" Text="Capacidade de Produção" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <asp:Repeater ID="rptCapacidadeProducao" runat="server" DataSource='<%# Eval("CapacidadeSetores") %>' Visible='<%# ExibirCapacidadeSetor() %>'>
                    <ItemTemplate>
                        <tr>
                            <td width="100%">
                                <asp:Label ID="Label3" runat="server" Text='<%# GetNomeSetor(Eval("Key")) %>'></asp:Label>
                            </td>
                            <td style="padding-left: 8px; white-space: nowrap; text-align: right" codigoSetor='<%# Eval("Key") %>'>
                                <asp:Label ID="lblSetor" runat="server" Text='<%# Eval("Value") %>'></asp:Label>
                                <asp:TextBox ID="txtSetor" runat="server" Width="40px" style="display: none" ValidationGroup='<%# this.ClientID %>'
                                    onkeypress="return soNumeros(event, true, true)" Text='<%# Bind("Value") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvSetor" runat="server" ErrorMessage="*"
                                     ControlToValidate="txtSetor" Display="Dynamic" ValidationGroup='<%# this.ClientID %>'></asp:RequiredFieldValidator>
                                m²
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

                <tr runat="server" visible='<%# ExibirCapacidadeSetor() %>'>
                    <td width="100%" colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr runat="server" visible='<%# ExibirCapacidadeSetor() %>'>
                    <td width="100%" colspan="2">
                        <asp:Label ID="Label4" runat="server" Text="Capacidade de Produção - Classificação" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                 <asp:Repeater ID="rptCapacidadeProducaoClassificacao" runat="server" DataSource='<%# Eval("CapacidadeClassificacao") %>' Visible='<%# ExibirCapacidadeSetor() %>'>
                    <ItemTemplate>
                        <tr>
                            <td width="100%">
                                <asp:Label ID="Label3" runat="server" Text='<%# GetNomeClassificacao(Eval("Key")) %>'></asp:Label>
                            </td>
                            <td style="padding-left: 8px; white-space: nowrap; text-align: right" codigoClassificacao='<%# Eval("Key") %>'>
                                <asp:Label ID="lblClassificacao" runat="server" Text='<%# Eval("Value") %>'></asp:Label>
                                <asp:TextBox ID="txtClassificacao" runat="server" Width="40px" style="display: none" ValidationGroup='<%# this.ClientID %>'
                                    onkeypress="return soNumeros(event, true, true)" Text='<%# Bind("Value") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvClassificacao" runat="server" ErrorMessage="*"
                                     ControlToValidate="txtClassificacao" Display="Dynamic" ValidationGroup='<%# this.ClientID %>'></asp:RequiredFieldValidator>
                                m²
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

            </table>
        </ItemTemplate>
    </asp:Repeater>
</div>
<script type="text/javascript">
    var <%= this.ClientID %> = new CapacidadeProducaoDiariaType("<%= this.ClientID %>");
</script>
