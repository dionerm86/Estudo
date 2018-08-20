<%@ Page Title="Exportação de Pedidos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstPedidoExportacao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPedidoExportacao" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(idExportacao) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Exportacao&idExportacao=" + idExportacao);
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Cód." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdExportacao" runat="server" Width="70px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" OnClick="lnkPesquisar_Click" ImageUrl="~/Images/Pesquisar.gif" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumeroPedido" runat="server" Width="70px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" OnClick="lnkPesquisar_Click" ImageUrl="~/Images/Pesquisar.gif" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSituacao" runat="server" AppendDataBoundItems="True" DataSourceID="odsSituacao"
                                DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" OnClick="lnkPesquisar_Click" ImageUrl="~/Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label5" runat="server" Text="Data Exportação" 
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <uc1:ctrlData ID="txtDataInicial" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td align="left">
                            <uc1:ctrlData ID="txtDataFinal" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" OnClick="lnkPesquisar_Click" ImageUrl="~/Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdPedido" runat="server" AllowPaging="True" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsExportacao" 
                    EmptyDataText="Nenhum registro encontrado" 
                    onrowcommand="grdPedido_RowCommand">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgRelatorio" runat="server" 
                                    ImageUrl="~/Images/Relatorio.gif" 
                                    onclientclick='<%# "openRpt(" + Eval("IdExportacao") + "); return false" %>' />
                                <asp:LinkButton ID="lnkConsulta" runat="server" CommandName="Consultar"
                                     CommandArgument='<%# Eval("IdExportacao") %>'>
                                    <img border="0" src="../Images/Pesquisar.gif" title="Consultar Situação dos Pedidos" />
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdExportacao" HeaderText="Cód." 
                            SortExpression="IdExportacao" />
                        <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" 
                            SortExpression="NomeFornec" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" 
                            SortExpression="NomeFunc" />
                        <asp:BoundField DataField="DataExportacao" HeaderText="Data Exportação" 
                            SortExpression="DataExportacao" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsExportacao" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.ExportacaoDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtIdExportacao" Name="idExportacao" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumeroPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ddlSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtDataInicial" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFinal" Name="dataFim" 
                            PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSituacao" runat="server" SelectMethod="GetSituacaoExportacao"
                    TypeName="Glass.Data.Helper.DataSources" >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
