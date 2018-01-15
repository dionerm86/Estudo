<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelObra.aspx.cs" Inherits="Glass.UI.Web.Utils.SelObra"
    Title="Selecione a Obra" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Tipo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoObra" runat="server">
                                <asp:ListItem Value="">Todos</asp:ListItem>
                                <asp:ListItem Value="False">Pagto. Antecipado</asp:ListItem>
                                <asp:ListItem Value="True">Crédito Gerado</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Aberta</asp:ListItem>
                                <asp:ListItem Value="5">Aguardando Financeiro</asp:ListItem>
                                <asp:ListItem Value="4">Confirmada</asp:ListItem>
                                <asp:ListItem Value="2">Cancelada</asp:ListItem>
                                <asp:ListItem Value="3">Finalizada</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Data de cadastro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido0" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataIni', this)" ToolTip="Alterar" />
                            <asp:TextBox ID="txtDataFim" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataFim', this)" ToolTip="Alterar" />
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnFechar" runat="server" Text="Fechar" OnClientClick="closeWindow();" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdObra" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsObra" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdObra"
                    EmptyDataText="Não há obras cadastradas.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton2" runat="server" OnClientClick='<%# "window.opener.setObra(0, " + Eval("IdObra") + ", \"" + Eval("Descricao").ToString().Replace("\r", "").Replace("\n", "").Replace("\"", "") + "\", \"" + (Eval("DescrSaldoPedidos") != null ? Eval("DescrSaldoPedidos").ToString().Replace("\n", "\\n") : "") + "\"); closeWindow(); return false;" %>'
                                    ImageUrl="~/Images/Insert.gif" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdObra" HeaderText="Cód." SortExpression="IdObra" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="Situacao" />
                        <asp:BoundField DataField="Saldo" DataFormatString="{0:C}" HeaderText="Saldo" SortExpression="Saldo">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data de cadastro"
                            SortExpression="DataCad" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsObra" runat="server" DataObjectTypeName="Glass.Data.Model.Obra"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ObraDAO" SelectCountMethod="GetListCount" 
                    CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
                    SkinID="" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeCliente" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:Parameter Name="idFunc" Type="UInt32" />
                        <asp:Parameter Name="idFuncCad" Type="UInt32" />
                        <asp:Parameter Name="idFormaPagto" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtDataIni" Name="dtIni" PropertyName="Text" Type="DateTime" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dtFim" PropertyName="Text" Type="DateTime" />
                        <asp:Parameter Name="dataFinIni" DefaultValue="" />
                        <asp:Parameter Name="dataFinFim" DefaultValue="" />
                        <asp:ControlParameter ControlID="drpTipoObra" DefaultValue="" Name="gerarCredito"
                            PropertyName="SelectedValue" Type="Boolean" />
                        <asp:QueryStringParameter Name="idsPedidosIgnorar" QueryStringField="idsPedidosIgnorar"
                            Type="String" />
                        <asp:Parameter Name="idObra" Type="UInt32" />
                        <asp:Parameter Name="idPedido" Type="UInt32" />
                        <asp:Parameter Name="descricao" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
