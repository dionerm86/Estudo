<%@ Page Title="Liberar Instalação Comum p/ Instalação Temperado" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadLiberarInst.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadLiberarInst" %>

<%@ Register src="../Controls/ctrlTextBoxFloat.ascx" tagname="ctrltextboxfloat" tagprefix="uc1" %>
<%@ Register src="../Controls/ctrlLinkQueryString.ascx" tagname="ctrllinkquerystring" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);" Width="60px"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                OnClick="imgPesq_Click" OnClientClick="if (FindControl('txtNumPedido', 'input').value == '') { alert('Informe o número do Pedido.'); return false; }" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvInstalacao" runat="server" AutoGenerateRows="False" 
                    DataSourceID="odsInstalacao" GridLines="None" DataKeyNames="IdInstalacao" 
                    HeaderText="Dados da Instalação">
        <Fields>
            <asp:TemplateField>
                <ItemTemplate>
                    <table align="left" style="width: 100%">
                        <tr>
                            <td align="left" class="dtvHeader">
                                <asp:Label ID="Label39" runat="server" Text="Pedido"></asp:Label>
                            </td>
                            <td align="left">
                                <asp:Label ID="lblPedido" runat="server" Text='<%# Eval("IdPedido") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" class="dtvHeader" nowrap="nowrap">
                                <asp:Label ID="Label44" runat="server" Text="Loja"></asp:Label>
                            </td>
                            <td class="dtvAlternatingRow" nowrap="nowrap">
                                <asp:Label ID="lblLoja" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" class="dtvHeader" nowrap="nowrap">
                                <asp:Label ID="Label41" runat="server" Text="Cliente"></asp:Label>
                            </td>
                            <td align="left">
                                <asp:Label ID="lblCliente" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" class="dtvHeader" nowrap="nowrap">
                                <asp:Label ID="Label45" runat="server" Text="Local"></asp:Label>
                            </td>
                            <td class="dtvAlternatingRow">
                                <asp:Label ID="lblLocal" runat="server" Text='<%# Eval("LocalObra") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" class="dtvHeader" nowrap="nowrap">
                                <asp:Label ID="Label42" runat="server" Text="Situação"></asp:Label>
                            </td>
                            <td align="left">
                                <asp:Label ID="lblSituacao" runat="server" Text='<%# Eval("DescrSituacao") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" class="dtvHeader" nowrap="nowrap">
                                <asp:Label ID="Label43" runat="server" Text="Data Conf. Ped."></asp:Label>
                            </td>
                            <td class="dtvAlternatingRow">
                                <asp:Label ID="lblDataConfPed" runat="server" 
                                    Text='<%# Eval("DataConfPedido", "{0:d}") %>'></asp:Label>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <InsertItemTemplate>
                    <asp:Button ID="btnInserir" runat="server" CommandName="Insert" 
                        Text="Inserir" />
                    <asp:Button ID="btnCancelar0" CausesValidation="false" runat="server" 
                        Text="Fechar" onclientclick="closeWindow();" />
                </InsertItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
        </Fields>
                    <HeaderStyle BackColor="White" Font-Bold="True" Font-Size="Medium" 
                        HorizontalAlign="Center" Height="40px" />
            </asp:DetailsView>
                    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInstalacao" runat="server" 
                    SelectMethod="BuscarParaLiberacao" TypeName="Glass.Data.DAL.InstalacaoDAO" 
                    onselected="odsInstalacao_Selected">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" 
                                PropertyName="Text" Type="UInt32" />
                        </SelectParameters>
                </colo:VirtualObjectDataSource>
                    </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnLiberar" runat="server" 
                    onclientclick="return confirm(&quot;Confirma liberação desta instalação para equipes de instalação temperado?&quot;);" 
                    Text="Liberar" Visible="False" onclick="btnLiberar_Click" />
            </td>
        </tr>
    </table>
</asp:Content>

