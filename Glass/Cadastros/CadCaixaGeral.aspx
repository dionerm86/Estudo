<%@ Page Title="Fechamento - Caixa Geral" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadCaixaGeral.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCaixaGeral" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <style type="text/css">
        .valor {
            text-align: right;
            padding-left: 4px;
            padding-right: 8px;
        }
    </style>

    <script type="text/javascript">
        function openRpt(exportarExcel, somenteTotais) {
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var horaIni = FindControl("ctrlDataIni_txtHora", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var horaFim = FindControl("ctrlDataFim_txtHora", "input").value;
            var idFunc = FindControl("drpFuncionario", "select").value;
            var apenasDinheiro = FindControl("chkApenasDinheiro", "input");
            var tipoMov = FindControl("drpTipoMov", "select").value;
            var tipoConta = FindControl("drpContabil", "select").value;
            var id = FindControl("txtIdCaixaGeral", "input");
            var valor = FindControl("txtValorMov", "input");
            var semEstorno = FindControl("chkSemEstorno", "input");
            var idLoja = FindControl("drpLoja", "select").value;
            var apenasCheque = FindControl("chkApenasCheque", "input");

            if (horaIni != "") dataIni = dataIni + " " + horaIni;
            if (horaFim != "") dataFim = dataFim + " " + horaFim;

            var queryString = "&DtIni=" + dataIni + "&DtFim=";
            queryString += FindControl("ctrlDataFim_txtData", "input") ? dataFim : dataIni;
            queryString += "&idFunc=" + idFunc;
            queryString += "&apenasDinheiro=" + (apenasDinheiro != null ? apenasDinheiro.checked : "false");
            queryString += "&tipoMov=" + tipoMov;
            queryString += "&tipoConta=" + tipoConta;
            queryString += "&id=" + (id != null && id.value.length > 0 ? id.value : "0");
            queryString += "&valorIni=" + (valor != null ? valor.value : "");
            queryString += "&valorFim=" + (valor != null ? valor.value : "");
            queryString += "&semEstorno=" + (semEstorno != null ? semEstorno.checked : "false");
            queryString += "&idLoja=" + idLoja;
            queryString += "&apenasCheque=" + (apenasCheque != null ? apenasCheque.checked : "false");
            queryString += "&somenteTotais=" + somenteTotais;
            queryString += "&exportarExcel=" + exportarExcel;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=CaixaGeral" + queryString);
            return false;
        }

        // Utilizado para copiar a dataIni para a dataFim, se a pessoa não tiver acesso à consulta por período
        function setDataFim(txtDataIni) {
            FindControl("ctrlDataFim_txtData", "input").value = txtDataIni.value;
        }



    </script>

    <table style="width: 100%" runat="server" id="tbCaixaGeral">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="lblFuncionario" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:DropDownList ID="drpFuncionario" runat="server" AppendDataBoundItems="True"
                                AutoPostBack="True" DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            <asp:Label ID="lblPeriodo" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="True" />
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="True" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Código Mov." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtIdCaixaGeral" runat="server" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('ImageButton1')" Width="70px"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label2" runat="server" Text="Valor R$" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtValorMov" runat="server" onkeypress="return soNumeros(event, false, true)"
                                onkeydown="if (isEnter(event)) cOnClick('ImageButton2')" Width="100px"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="true" MostrarTodas="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Tipos de contas" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContabil" runat="server" AutoPostBack="True" OnTextChanged="drpContabil_IndexChanged">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Contábeis</asp:ListItem>
                                <asp:ListItem Value="2">Não contábeis</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblTipoMov" runat="server" Text="Movimentações" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoMov" runat="server">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Entradas</asp:ListItem>
                                <asp:ListItem Value="2">Saídas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkApenasDinheiro" runat="server" Text="Apenas movimentações em dinheiro" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkSemEstorno" runat="server" Text="Movimentações de Entrada sem Estorno" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkApenasCheque" runat="server" Text="Apenas movimentações em cheque" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCaixaGeral" runat="server" AutoGenerateColumns="false"
                    EnableViewState="false" DataSourceID="odsCaixaGeral" OnDataBound="grdCaixaGeral_DataBound"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField HeaderText="Cod. Mov." SortExpression="IdCaixaGeral">
                            <ItemTemplate>
                                <asp:Label ID="lblCodMov" runat="server" Text='<%# Bind("CodMov") %>'></asp:Label>
                                <asp:HiddenField ID="hdfTipoMov" runat="server" Value='<%# Eval("TipoMov") %>' />
                                <asp:HiddenField ID="hdfTotalCheque" runat="server" Value='<%# Eval("TotalCheque", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfTotalChequeDev" runat="server" Value='<%# Eval("TotalChequeDevolvido", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfTotalChequeReapres" runat="server" Value='<%# Eval("TotalChequeReapresentado", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfTotalDinheiro" runat="server" Value='<%# Eval("TotalDinheiro", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfSaldoDinheiro" runat="server" Value='<%# Eval("SaldoDinheiro", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfSaldoCheque" runat="server" Value='<%# Eval("SaldoCheque", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfSaldoCartao" runat="server" Value='<%# Eval("SaldoCartao", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfSaldoConstrucard" runat="server" Value='<%# Eval("SaldoConstrucard", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfSaldoPermuta" runat="server" Value='<%# Eval("SaldoPermuta", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfTotalTercVenc" runat="server" Value='<%# Eval("TotalChequeTerc", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfSaidaCheque" runat="server" Value='<%# Eval("TotalSaidaCheque", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfSaidaCartao" runat="server" Value='<%# Eval("TotalSaidaCartao", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfSaidaDinheiro" runat="server" Value='<%# Eval("TotalSaidaDinheiro", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfSaidaConstrucard" runat="server" Value='<%# Eval("TotalSaidaConstrucard", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfSaidaPermuta" runat="server" Value='<%# Eval("TotalSaidaPermuta", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfCreditoGerado" runat="server" Value='<%# Eval("TotalCreditoGerado", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfEntradaDinheiro" runat="server" Value='<%# Eval("TotalEntradaDinheiro", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfEntradaCheque" runat="server" Value='<%# Eval("TotalEntradaCheque", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfEntradaCartao" runat="server" Value='<%# Eval("TotalEntradaCartao", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfEntradaConstrucard" runat="server" Value='<%# Eval("TotalEntradaConstrucard", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfEntradaPermuta" runat="server" Value='<%# Eval("TotalEntradaPermuta", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfCreditoRecebido" runat="server" Value='<%# Eval("TotalCreditoRecebido", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfContasReceberGeradas" runat="server" Value='<%# Eval("ContasReceberGeradas", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfContasRecebidasContabeis" runat="server" Value='<%# Eval("TotalContasRecebidasContabeis", "{0:C}") %>' />
                                <asp:HiddenField ID="hdfContasRecebidasNaoContabeis" runat="server" Value='<%# Eval("TotalContasRecebidasNaoContabeis", "{0:C}") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("IdCaixaGeral") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia" />
                        <asp:BoundField DataField="ClienteFornecedor" HeaderText="Cli./Forn." SortExpression="ClienteFornecedor" />
                        <asp:BoundField DataField="ValorString" DataFormatString="{0:C}" HeaderText="Valor"
                            SortExpression="ValorString" />
                        <asp:BoundField DataField="JurosString" DataFormatString="{0:C}" HeaderText="Juros"
                            SortExpression="JurosString" />
                        <asp:BoundField DataField="DataMovString" DataFormatString="{0:g}" HeaderText="Data"
                            SortExpression="DataMovString" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="PlanoContaObs" HeaderText="Referente a" SortExpression="PlanoContaObs" />
                        <asp:BoundField DataField="Saldo" DataFormatString="{0:C}" HeaderText="Saldo" SortExpression="Saldo" />
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCaixaGeral" runat="server" SelectMethod="GetMovimentacoes"
                    TypeName="Glass.Data.DAL.CaixaGeralDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtIdCaixaGeral" Name="idCaixaGeral" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtValorMov" Name="valorIni" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtValorMov" Name="valorFim" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="chkApenasDinheiro" Name="apenasDinheiro" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="chkApenasCheque" Name="apenasCheque" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="drpTipoMov" Name="tipoMov" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkSemEstorno" Name="semEstorno" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="drpContabil" Name="tipoConta" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetFinanceiros" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center" id="resumoCaixaGeral">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Font-Bold="True" Text="Entrada Dinheiro"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblEntradaDinheiro" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Font-Bold="True" Text="Entrada Cheque"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblEntradaCheque" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Font-Bold="True" Text="Entrada Cartão"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblEntradaCartao" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Font-Bold="True" Text="Entrada Construcard"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblEntradaConstrucard" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Font-Bold="True" Text="Entrada Permuta"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblEntradaPermuta" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Red" Text="Estorno/Saída Dinheiro"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblSaidaDinheiro" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Font-Bold="True" ForeColor="Red" Text="Estorno/Saída Cheque"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblSaidaCheque" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label15" runat="server" Font-Bold="True" ForeColor="Red" Text="Estorno/Saída Cartão"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblSaidaCartao" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label14" runat="server" Font-Bold="True" ForeColor="Red" Text="Estorno/Saida Construcard"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblSaidaConstrucard" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                                                <td>
                            <asp:Label ID="Label19" runat="server" Font-Bold="True" ForeColor="Red" Text="Estorno/Saida Permuta"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblSaidaPermuta" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Font-Bold="True" Text="Saldo Dinheiro"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblSaldoDinheiro" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Font-Bold="True" Text="Saldo Cheque"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblSaldoCheque" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblTxtTotalCartao" runat="server" Font-Bold="True" Text="Saldo Cartão"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblSaldoCartao" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" Font-Bold="True" Text="Saldo Construcard"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblSaldoConstrucard" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label20" runat="server" Font-Bold="True" Text="Saldo Permuta"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblSaldoPermuta" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                </table>
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Font-Bold="True" Text="Crédito Utilizado"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblCreditoRecebido" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label16" runat="server" Font-Bold="True" Text="Notas Promissórias Geradas"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblContasReceberGeradas" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label13" runat="server" Font-Bold="True" Text="Crédito Gerado"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblCreditoGerado" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                </table>
                <br />
                <table style="<%= ExibirColunasContaRecebida() %>">
                    <tr>
                        <td>
                            <asp:Label ID="lblTituloContasContabeis" runat="server" Font-Bold="True" Text="Contas Recebidas"
                                ToolTip="Este campo considera apenas o filtro de período"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblContasContabeis" runat="server" Font-Bold="False"
                                ToolTip="Este campo considera apenas o filtro de período"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblTituloContasNaoContabeis" runat="server" Font-Bold="True"
                                Text="Contas Recebidas"
                                ToolTip="Este campo considera apenas o filtro de período"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblContasNaoContabeis" runat="server" Font-Bold="False"
                                ToolTip="Este campo considera apenas o filtro de período"></asp:Label>
                        </td>
                    </tr>
                </table>
                <br />
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="lblTitleTotalDinheiro" runat="server" Font-Bold="True" Text="Saldo Cumulativo Dinheiro"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblTotalDinheiro" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblTitleTotalCheque" runat="server" Font-Bold="True" Text="Saldo Cumulativo Cheque Em aberto"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblTotalCheque" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblTitleTotalChequeReapresentado" runat="server" Font-Bold="True"
                                Text="Saldo Cumulativo Cheque Reapres."></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblTotalChequeReapresentado" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="lblTitleTotalChequeTerc" runat="server" Font-Bold="True" Text="Total Cheque Terc. Utilizáveis"></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblTotalTercVenc" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblTitleTotalChequeDev" runat="server" Font-Bold="True" Text="Saldo Cumulativo Cheque Devolv."></asp:Label>
                        </td>
                        <td class="valor">
                            <asp:Label ID="lblTotalChequeDev" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <a href="#" onclick="openRpt(false);">
                    <img alt="" border="0" src="../Images/printer.png" />
                    Imprimir Fechamento</a> &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <a href="#" onclick="openRpt(false, true);">
                    <img alt="" border="0" src="../Images/printer.png" />
                    Imprimir Totais</a>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
