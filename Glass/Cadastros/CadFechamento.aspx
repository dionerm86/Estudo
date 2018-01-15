<%@ Page Title="Fechamento de Caixa Diário" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadFechamento.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadFechamento" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(somenteTotais)
        {
            var idFunc = FindControl("drpFunc", "select") != null ? FindControl("drpFunc", "select").value : 0;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=CaixaDiario&idLoja=" + FindControl("ddlLoja", "select").value +
                "&Data=" + FindControl("txtDataIni", "input").value + "&idFunc=" + idFunc + "&somenteTotais=" + somenteTotais);
            return false;
        }

        function onFecha() {
            var saldoAnterior = FindControl("lblSaldoCaixa", "span").innerHTML.replace("R$", "").replace("R$ ", "").replace(".", "");
            if (saldoAnterior == undefined || saldoAnterior == null)
                saldoAnterior = FindControl("lblSaldoCaixaAtraso", "span").innerHTML.replace("R$ ", "").replace(".", "");

            var idLojaSelecionada = FindControl("ddlLoja", "select").value;
            var saldoAtual = CadFechamento.ObtemSaldoAtual(idLojaSelecionada).value;
            
            if (parseFloat(saldoAnterior) != parseFloat(saldoAtual)) {
                if (confirm("Foram feitas movimentações no caixa diário que não estão sendo exibidas na tela, pois, a tela está desatualizada." +
                    "\n\nClique em OK para a não transferir o valor. A tela será atualizada para que as movimentações recentes sejam buscadas." +
                    "\nClique em CANCELAR para concluir a transferência. As movimentações recentes não serão consideradas no valor transferido para o caixa geral.")) {
                    redirectUrl(window.location.href);
                    return false;
                }

                if (!confirm('Confirmar transferência para Caixa Geral com as movimentações desatualizadas?')) {
                    redirectUrl(window.location.href);
                    return false;
                }
            }
            else if (!confirm('Confirmar transferência para Caixa Geral?'))
                return false;

            return true;
        }

    </script>

    <table align="center" style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlLoja runat="server" ID="ddlLoja" SomenteAtivas="true" AutoPostBack="True"
                                MostrarTodas="false" OnSelectedIndexChanged="ddlLoja_SelectedIndexChanged" Enabled="False" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataIni" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataIni', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td runat="server" id="tituloFunc">
                            <asp:Label ID="Label5" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td runat="server" id="filtroFunc">
                            <asp:DropDownList ID="drpFunc" runat="server" AppendDataBoundItems="True" 
                                DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td runat="server" id="pesqFunc">
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
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
                <asp:GridView GridLines="None" ID="grdFechamento" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsFechamento" OnDataBound="grdFechamento_DataBound" EmptyDataText="Nenhuma movimentação feita."
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField HeaderText="Cod. Mov." SortExpression="IdCaixaDiario">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdCaixaDiario") %>'></asp:Label>
                                <asp:HiddenField ID="hdfTipoMov" runat="server" Value='<%# Eval("TipoMov") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("IdCaixaDiario") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia" />
                        <asp:BoundField DataField="ClienteFornecedor" HeaderText="Cli./Forn." SortExpression="ClienteFornecedor" />
                        <asp:BoundField DataField="Valor" HeaderText="Valor" SortExpression="Valor" DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Juros" DataFormatString="{0:C}" HeaderText="Juros" SortExpression="Juros" />
                        <asp:BoundField DataField="DescrUsuCad" HeaderText="Funcionário" 
                            SortExpression="DescrUsuCad" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                        <asp:BoundField DataField="Saldo" HeaderText="Saldo" SortExpression="Saldo" DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td><a href="#" onclick="openRpt();">
                            <img alt="" src="../Images/printer.png" border="0" />
                            Imprimir Fechamento</a></td>
                        <td><a href="#" onclick="openRpt(true);">
                            <img alt="" src="../Images/printer.png" border="0" />
                            Imprimir Totais</a></td>
                    </tr>
                </table>
               
                <br />
                <br />
                <div id="divFecharCaixa" runat="server">
                    <asp:LinkButton ID="lnkTransferir" runat="server" OnClientClick="document.getElementById('tbFecharCaixa').style.display='inline'; return false;">
                         <img alt="" src="../Images/book_go.png" border="0" />
                        Fechar Caixa Diário</asp:LinkButton>
                    <br />
                    <table id="tbFecharCaixa" style="display: none;">
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label3" runat="server" Text="Saldo do caixa:"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblSaldoCaixa" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label7" runat="server" Text="Saldo em dinheiro:"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblSaldoDinheiro" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Text="Valor a ser transferido para o Cx. Geral:"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblValorTransf" runat="server"></asp:Label>
                                            <asp:TextBox ID="txtValorTransf" runat="server" Width="100px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button ID="btnFecharCaixa" runat="server" OnClick="btnFecharCaixa_Click" OnClientClick="return onFecha();"
                                    Text="Fechar Caixa" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="divFecharCaixaAtraso" runat="server" visible="False">
                    <asp:LinkButton ID="lnkTransfAtraso" runat="server" OnClientClick="document.getElementById('tbFecharCaixaAtraso').style.display='inline'; return false;">
                         <img alt="" src="../Images/book_go.png" border="0" />
                        Fechar Caixa Diário (Dia anterior não foi fechado)</asp:LinkButton>
                    <br />
                    <table id="tbFecharCaixaAtraso" style="display: none;">
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label4" runat="server" Text="Saldo do caixa (dia não fechado):"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblSaldoCaixaAtraso" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label8" runat="server" Text="Saldo em dinheiro (dia não fechado):"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblSaldoDinheiroAtraso" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label6" runat="server" Text="Valor a ser transferido para o Cx. Geral:"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblValorTransfAtraso" runat="server"></asp:Label>
                                            <asp:TextBox ID="txtValorTransfAtraso" runat="server" Width="100px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button ID="btnFecharCaixaAtraso" runat="server" OnClientClick="return onFecha();"
                                    Text="Fechar Caixa" OnClick="btnFecharCaixaAtraso_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFechamento" runat="server" SelectMethod="GetMovimentacoes"
                    TypeName="Glass.Data.DAL.CaixaDiarioDAO" MaximumRowsParameterName="" 
                    StartRowIndexParameterName="" onselected="odsFechamento_Selected">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFunc" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtDataIni" Name="data" PropertyName="Text" Type="DateTime" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource ID="odsFuncionario" runat="server" 
                    CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
                    Culture="" MaximumRowsParameterName="" SelectMethod="GetCaixaDiario" SkinID="" 
                    StartRowIndexParameterName="" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
