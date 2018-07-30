<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelEtiquetaProcesso.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelEtiquetaProcesso" Title="Selecione o Processo" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function setProc(idProcesso, codInterno, descr, codAplicacao) {

            if (GetQueryString("buscaComPopup") === "true") {
                var idControle = GetQueryString("id-controle");
                if (idControle) {
                    window.opener.Busca.Popup.atualizar(idControle, idProcesso, codInterno);
                    closeWindow();
                    return;
                }
            }

            if (GetQueryString("idProdPed") != "" && GetQueryString("idProdPed") != 'undefined' && GetQueryString("idProdPed") != null)
                window.opener.setProcComposicao(idProcesso, codInterno, codAplicacao, GetQueryString("idProdPed"));
            else
                window.opener.setProc(idProcesso, codInterno, codAplicacao, GetQueryString("idControle"));
            closeWindow();
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Código" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProcesso" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Aplicação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAplicacao" runat="server" DataSourceID="odsEtiquetaAplicacao"
                                DataTextField="Descricao" DataValueField="IdAplicacao" AppendDataBoundItems="true"
                                Width="150px">
                                <asp:ListItem Text="" Selected="True" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsEtiquetaAplicacao" runat="server"
                                SelectMethod="GetForSel" TypeName="Glass.Data.DAL.EtiquetaAplicacaoDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProcesso" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataSourceID="odsProcesso"
                    DataKeyNames="IdProcesso">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="setProc('<%# Eval("IdProcesso") %>', '<%# Eval("CodInterno") %>', '<%# Eval("Descricao") %>', '<%# Eval("CodAplicacao") %>');">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código" SortExpression="CodInterno">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrAplicacao" HeaderText="Aplicação" SortExpression="DescrAplicacao" />
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProcesso" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetForSelCount" SelectMethod="GetForSel"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.EtiquetaProcessoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodProcesso" Name="codProcesso" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtDescricao" Name="descricao" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpAplicacao" Name="idAplicacao" PropertyName="SelectedValue" Type="UInt32" />
                        <asp:QueryStringParameter Name="idSubgrupo" QueryStringField="idSubgrupo" Type="UInt32" DefaultValue="0" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
