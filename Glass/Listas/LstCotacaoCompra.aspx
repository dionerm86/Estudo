<%@ Page Title="Cotações de Compra" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstCotacaoCompra.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCotacaoCompra" %>

<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function cancelar(id) {
            openWindow(200, 450, "../Utils/SetMotivoCancCotacaoCompra.aspx?idCotacaoCompra=" + id);
        }

        function openRpt(id, tipo, exibirValor) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=CotacaoCompraCalculada" +
                "&id=" + id + "&tipo=" + tipo + "&exibirValor=" + exibirValor);
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
                            <asp:TextBox ID="txtCodigo" runat="server" Width="50px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Aberta</asp:ListItem>
                                <asp:ListItem Value="2">Finalizada</asp:ListItem>
                                <asp:ListItem Value="3">Cancelada</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Período Cadastro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataCadIni" runat="server" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataCadFim" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
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
                <asp:HyperLink ID="lnkInserir" runat="server" NavigateUrl="~/Cadastros/CadCotacaoCompra.aspx">Inserir cotação de compra</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdCotacaoCompra" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="Codigo" DataSourceID="odsCotacaoCompra"
                    GridLines="None" EmptyDataText="Não há cotação de compra cadastrada." OnRowCommand="grdCotacaoCompra_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Relatorio_menos.jpg"
                                    OnClientClick='<%# "openRpt(" + Eval("Codigo")+ "," + Eval("PrioridadeCalculoFinalizacao") + ", false); return false;" %>'
                                    Visible='<%# Eval("RelatorioVisivel") %>' ToolTip="Visualizar Cotação sem Valores" />
                                <asp:ImageButton ID="imgRelatorio" runat="server" ImageUrl="~/Images/relatorio.gif"
                                    OnClientClick='<%# "openRpt(" + Eval("Codigo")+ "," + Eval("PrioridadeCalculoFinalizacao") + ", true); return false;" %>'
                                    Visible='<%# Eval("RelatorioVisivel") %>' ToolTip="Visualizar Cotação" />
                                <asp:HyperLink ID="lnkEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif" NavigateUrl='<%# Eval("Codigo", "~/Cadastros/CadCotacaoCompra.aspx?id={0}") %>'
                                    Visible='<%# Eval("PodeEditar") %>'></asp:HyperLink>
                                <asp:ImageButton ID="imgExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick='<%# Eval("Codigo", "cancelar({0}); return false") %>' Visible='<%# Eval("PodeCancelar") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Codigo" HeaderText="Cód." SortExpression="Codigo" />
                        <asp:BoundField DataField="Observacao" HeaderText="Observação" SortExpression="Observacao" />
                        <asp:BoundField DataField="NomeFuncCadastro" HeaderText="Func. Cadastro" ReadOnly="True"
                            SortExpression="CodFuncCadastro" />
                        <asp:BoundField DataField="DataCadastro" DataFormatString="{0:d}" HeaderText="Data Cadastro"
                            SortExpression="DataCadastro" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <table class="pos" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescricaoSituacao") %>'></asp:Label>
                                        </td>
                                        <td style="padding-left: 3px">
                                            <asp:ImageButton ID="imgReabrir" runat="server" ImageUrl="~/images/Cadeado.gif" ToolTip="Reabrir"
                                                Visible='<%# Eval("PodeReabrir") %>' CommandArgument='<%# Eval("Codigo") %>'
                                                CommandName="Reabrir" OnClientClick="if (!confirm(&quot;Deseja reabrir esta cotação de compra?&quot;)) return false" />
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="NomeFuncFinalizacao" HeaderText="Func. Finalização" SortExpression="NomeFuncFinalizacao" />
                        <asp:BoundField DataField="DataFinalizacao" DataFormatString="{0:d}" HeaderText="Data Finalização"
                            SortExpression="DataFinalizacao" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("Codigo") %>'
                                    Tabela="CotacaoCompras" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCotacaoCompra" runat="server"
                    DataObjectTypeName="WebGlass.Business.CotacaoCompra.Entidade.CotacaoCompra" DeleteMethod="ExcluirCotacaoCompra"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="ObtemNumeroCotacoesCompra"
                    SelectMethod="ObtemCotacoesCompra" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="WebGlass.Business.CotacaoCompra.Fluxo.CRUD">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodigo" Name="idCotacaoCompra" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
