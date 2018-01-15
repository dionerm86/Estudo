<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelCustoFixo.aspx.cs" Inherits="Glass.UI.Web.Utils.SelCustoFixo"
    Title="Selecione o Custo Fixo" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
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
                <asp:GridView GridLines="None" ID="grdCustoFixo" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsCustoFixo" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    DataKeyNames="IdCustoFixo" EmptyDataText="Nenhum custo fixo encontrado.">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="window.opener.setCustoFixo(<%# Eval("IdCustoFixo") %>, '<%# Eval("Descricao") != null ? Eval("Descricao").ToString().Replace("'", "") : "" %>', '<%# Eval("NomeFornec") != null ? Eval("NomeFornec").ToString().Replace("'", "") : "" %>', '<%# Eval("NomeLoja") %>', '<%# Eval("DescrPlanoConta") != null ? Eval("DescrPlanoConta").ToString().Replace("'", "") : "" %>', '<%# Eval("ValorVenc") %>', <%# Eval("DiaVenc") %>, window);">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Rerente a" SortExpression="DescrPlanoConta" />
                        <asp:BoundField DataField="ValorVenc" HeaderText="Valor" SortExpression="ValorVenc"
                            DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DiaVenc" HeaderText="Dia Venc." SortExpression="DiaVenc">
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataUltPagto" DataFormatString="{0:d}" HeaderText="Ult. Pagto"
                            SortExpression="DataUltPagto">
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="DescrSituacao" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCustoFixo" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountSel" SelectMethod="GetListSel" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.CustoFixoDAO"
                    DataObjectTypeName="Glass.Data.Model.CustoFixo" DeleteMethod="Delete">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="idCustoFixo" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:Parameter DefaultValue="" Name="idFornec" Type="UInt32" />
                        <asp:Parameter Name="nomeFornec" Type="String" />
                        <asp:Parameter Name="diaVencIni" Type="Int32" />
                        <asp:Parameter Name="diaVencFim" Type="Int32" />
                        <asp:ControlParameter ControlID="txtNome" Name="descricao" PropertyName="Text" Type="String" />
                        <asp:Parameter DefaultValue="1" Name="situacao" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
