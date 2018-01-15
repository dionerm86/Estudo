<%@ Page Title="Transportadoras" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstTransportador.aspx.cs" Inherits="Glass.UI.Web.Listas.LstTransportador" %>

<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cód. Transp." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdTransp" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Nome" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" onkeypress="if (isEnter(event)) return false;"
                                Width="170px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Transportadora</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdTransportador" runat="server" SkinID="defaultGridView"
                    DataKeyNames="IdTransportador" DataSourceID="odsTransportador" 
                    EmptyDataText="Não há transportadoras cadastradas.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadTransportador.aspx?idTransp=" + Eval("IdTransportador") %>' Visible='<%# PodeEditar() %>'>
                                    <img border="0" src="../Images/EditarGrid.gif" alt="Editar" /></asp:HyperLink>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja excluir esta Transportadora?&quot;);"
                                    ToolTip="Excluir" Visible='<%# PodeApagar() %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdTransportador" HeaderText="Cód." SortExpression="IdTransportador" />
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="NomeFantasia" HeaderText="Nome Fantasia" SortExpression="NomeFantasia" />
                        <asp:BoundField DataField="CpfCnpj" HeaderText="CPF/CNPJ" SortExpression="CpfCnpj" />
                        <asp:BoundField DataField="Placa" HeaderText="Placa" SortExpression="Placa" />
                        <asp:BoundField DataField="Telefone" HeaderText="Telefone" SortExpression="Telefone" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="Transportador" IdRegistro='<%# (uint)(int)Eval("IdTransportador") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTransportador" runat="server" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.Transportador"
                    DeleteMethod="ApagarTransportador" 
                    DeleteStrategy="GetAndDelete"
                    SelectMethod="PesquisarTransportadores" 
                    SelectByKeysMethod="ObtemTransportador"
                    TypeName="Glass.Global.Negocios.ITransportadorFluxo"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SortParameterName="sortExpression">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtIdTransp" Name="idTransportador" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nome" PropertyName="Text" Type="String" />
                        <asp:Parameter Name="cpfCnpj" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
