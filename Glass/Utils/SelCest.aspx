<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelCest.aspx.cs" Inherits="Glass.UI.Web.Utils.SelCest"
    Title="Selecione o CEST" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function setNf(idCest, Codigo)
        {
            window.opener.setCest(idCest, Codigo);
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Código" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodigo" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>                        
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCest" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    DataSourceID="odsCest" DataKeyNames="IdCest" EmptyDataText="Nenhum CEST encontrado."
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" Width="100%">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfIdNf" runat="server" Value='<%# Eval("IdCest") %>' />
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick='setNf(&#039;<%# Eval("IdCest") %>&#039;,&#039;<%# Eval("Codigo") %>&#039;);closeWindow();return false;'>
                                    <img alt="Selecionar" border="0" src="../Images/ok.gif" title="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Codigo" HeaderText="Código" SortExpression="Codigo" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource ID="odsCest" runat="server"
                    SelectMethod="ObtemCESTs" Culture="pt-BR"
                    EnablePaging="true"
                    MaximumRowsParameterName="pageSize"
                    SortParameterName="sortExpression"
                    TypeName="Glass.Fiscal.Negocios.ICestFluxo">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodigo" Name="Codigo" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
