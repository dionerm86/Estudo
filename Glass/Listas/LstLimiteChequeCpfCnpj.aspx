<%@ Page Title="Limite de Cheques por CPF/CNPJ" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstLimiteChequeCpfCnpj.aspx.cs" Inherits="Glass.UI.Web.Listas.LstLimiteChequeCpfCnpj" %>

<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>

<%@ Register src="../Controls/ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="CPF/CNPJ" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlSelPopup ID="selCpfCnpj" runat="server" ColunasExibirPopup="Key|Value" 
                                DataSourceID="odsCpfCnpj" DataTextField="Value" DataValueField="Key" 
                                ExibirIdPopup="False" FazerPostBackBotaoPesquisar="True" 
                                TitulosColunas="Cpf|CPF/CNPJ" TituloTela="Selecione o CPF/CNPJ"  />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdLimiteCheque" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
                    DataKeyNames="Codigo" DataSourceID="odsLimiteCheque" GridLines="None" 
                    PageSize="30" 
                    EmptyDataText="Ainda não há CPF/CNPJ cadastrados nos cheques. Ao cadastrar, serão gerados automaticamente registros para controle do limite.">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" 
                                    ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" 
                                    CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" 
                                    ImageUrl="~/Images/EditarGrid.gif" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CPF/CNPJ" SortExpression="CpfCnpj">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("CpfCnpj") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("CpfCnpj") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Limite Configurado" SortExpression="Limite">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLimite" runat="server" Text='<%# Bind("Limite") %>'
                                    onkeypress="return soNumeros(event, false, true)" Width="100px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Limite", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Utilizado do Limite" 
                            SortExpression="ValorUtilizado">
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" 
                                    Text='<%# Eval("ValorUtilizado", "{0:c}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" 
                                    Text='<%# Bind("ValorUtilizado", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Restante do Limite" 
                            SortExpression="ValorRestante">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" onprerender="Label4_PreRender" 
                                    Text='<%# Eval("ValorRestante", "{0:c}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" onprerender="Label4_PreRender" 
                                    Text='<%# Bind("ValorRestante", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Observacao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObservacao" runat="server" Text='<%# Bind("Observacao") %>' Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblObservacao" runat="server" Text='<%# Bind("Observacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                                    IdRegistro='<%# Eval("Codigo") %>' Tabela="LimiteChequeCpfCnpj" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLimiteCheque" runat="server" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize" SelectCountMethod="ObtemNumeroItens" 
                    SelectMethod="ObtemItens" SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" 
                    TypeName="WebGlass.Business.Cheque.Fluxo.LimiteCheque"                      
                    DataObjectTypeName="WebGlass.Business.Cheque.Entidade.LimiteCheque" 
                    UpdateMethod="SalvaLimite">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="selCpfCnpj" Name="cpfCnpj" PropertyName="Valor" 
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCpfCnpj" runat="server" 
                    SelectMethod="ObtemCpfCnpj" 
                    TypeName="WebGlass.Business.Cheque.Fluxo.LimiteCheque" >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

