<%@ Page Title="Contas a Pagar" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="ListaContasPagar.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaContasPagar" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function validarDataVenc(val, args) {
            var idCustoFixo = FindControl("hdfIdCustoFixo", "input").value;
            if (idCustoFixo == "")
                return;

            var dataVencOrig = toDate(FindControl("hdfDataVenc", "input").value);
            var dataVenc = toDate(args.Value);

            args.IsValid = dataVencOrig.getMonth() == dataVenc.getMonth() && dataVencOrig.getFullYear() == dataVenc.getFullYear();
        }

        function openRpt(exportarExcel) {
            var idContaPg = FindControl("txtIdContaPg", "input").value;
            var idCompra = FindControl("txtNumCompra", "input").value;
            var nf = FindControl("txtNF", "input").value;
            var idFornec = FindControl("txtNumFornec", "input").value;
            var nomeFornec = FindControl("txtNomeFornec", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idCustoFixo = FindControl("txtIdCustoFixo", "input").value;
            var idImpServ = FindControl("txtImpServ", "input").value;
            var idsFormaPagto = FindControl("ddlFiltroFormaPagto", "select").itens();
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var dtCadIni = FindControl("ctrlDataCadIni_txtData", "input").value;
            var dtCadFim = FindControl("ctrlDataCadFim_txtData", "input").value;
            var valorIni = FindControl("txtPrecoInicial", "input").value;
            var valorFin = FindControl("txtPrecoFinal", "input").value;
            var comissao = FindControl("chkComissao", "input").checked;
            var incluirCheques = FindControl("chkIncluirCheques", "input").checked;
            var tipo = FindControl("drpTipo", "select").value;
            var agrupar = FindControl("drpAgrupar", "select").value;
            var previsaoCustoFixo = FindControl("chkPrevisaoCustoFixo", "input").checked;
            var custoFixo = FindControl("chkCustoFixo", "input").checked;
            var planoConta = FindControl("txtPlanoConta", "input").value;
            var idPagtoRestante = FindControl("txtIdPagtoRestante", "input").value;
            var ordenar = FindControl("hdfOrdenar", "input").value;
            var contasSemValor = FindControl("chkContasSemValor", "input").checked;
            var dtBaixadoIni = FindControl("ctrlDataBaixadoIni_txtData", "input").value;
            var dtBaixadoFim = FindControl("ctrlDataBaixadoFim_txtData", "input").value;
            var dtNfCompraIni = FindControl("ctrlDataNfCompraIni_txtData", "input").value;
            var dtNfCompraFim = FindControl("ctrlDataNfCompraFim_txtData", "input").value;
            var numCte = FindControl("txtNumCte", "input").value;
            var idTransportadora = FindControl("txtNumTransportadora", "input").value;
            var nomeTransportadora = FindControl("txtNomeTransportadora", "input").value;
            var idFuncComissao = FindControl("drpFuncComissao", "select").value;
            var idComissao = FindControl("txtComissao", "input").value;

            var queryString = idCompra == "" ? "&idCompra=0" : "&idCompra=" + idCompra;
            queryString += idFornec == "" ? "&idFornec=0" : "&idFornec=" + idFornec;
            queryString += "&numCte=" + numCte;
            queryString += "&idTransportadora=" + idTransportadora;
            queryString += "&nomeTransportadora=" + nomeTransportadora;
            queryString += "&idFuncComissao=" + idFuncComissao;
            queryString += "&idComissao=" + idComissao;

            openWindow(600, 800, "RelBase.aspx?rel=ContasPagar&nomeFornec=" + nomeFornec + "&idLoja=" + idLoja + "&nf=" + nf + "&idContaPg=" + idContaPg +
                "&dtIni=" + dtIni + "&dtFim=" + dtFim + "&dataCadIni=" + dtCadIni + "&dataCadFim=" + dtCadFim + "&valorInicial=" + valorIni +
                "&idsFormaPagto=" + idsFormaPagto + "&valorFinal=" + valorFin + queryString + "&incluirCheques=" + incluirCheques +
                "&exportarExcel=" + exportarExcel + "&comissao=" + comissao + "&tipo=" + tipo + "&agrupar=" + agrupar +
                "&previsaoCustoFixo=" + previsaoCustoFixo + "&exibirSoPrevisaoCustoFixo=false" + "&planoConta=" + planoConta + "&idPagtoRestante=" + idPagtoRestante +
                "&custoFixo=" + custoFixo + "&idImpostoServ=" + idImpServ + "&ordenar=" + ordenar + "&contasSemValor=" + contasSemValor +
                "&dtBaixadoIni=" + dtBaixadoIni + "&dtBaixadoFim=" + dtBaixadoFim + "&dtNfCompraIni=" + dtNfCompraIni + "&dtNfCompraFim=" + dtNfCompraFim);

            return false;
        }

        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNomeFornec", "input").value = "";
                return false;
            }

            FindControl("txtNomeFornec", "input").value = retorno[1];
        }

        function openFornec() {
            if (FindControl("txtNumFornec", "input").value != "")
                return true;

            openWindow(500, 700, "../Utils/SelFornec.aspx");

            return false;
        }

        function getTransportadora(idTransportadora) {
            if (idTransportadora.value == "")
                return;

            var retorno = MetodosAjax.GetTransportadora(idTransportadora.value);

            if (retorno.error != null) {
                alert(retorno.error.description);
                idTransportadora.value = "";
                FindControl("txtNomeTransportadora", "input").value = "";
                return false;
            }

            FindControl("txtNomeTransportadora", "input").value = retorno.value;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>                        
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label24" runat="server" ForeColor="#0066FF" Text="Cód. Conta Pagar"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtIdContaPg" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label7" runat="server" ForeColor="#0066FF" Text="Num. Compra"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumCompra" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label17" runat="server" ForeColor="#0066FF" Text="NF/Pedido"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNF" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Rest. Pagto"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtIdPagtoRestante" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label12" runat="server" ForeColor="#0066FF" Text="Custo Fixo"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtIdCustoFixo" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label126" runat="server" ForeColor="#0066FF" Text="Imp./Serv."></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtImpServ" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label23" runat="server" ForeColor="#0066FF" Text="Comissão"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtComissao" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" ForeColor="#0066FF" Text="Loja"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="false" />
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" ForeColor="#0066FF" Text="Fornecedor"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeFornec" runat="server" Width="170px" onkeypress="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="return openFornec();"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Venc."></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Valor Venc." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPrecoInicial" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            até
                        </td>
                        <td>
                            <asp:TextBox ID="txtPrecoFinal" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq8" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Tipo"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpTipo" runat="server" OnLoad="drpTipo_Load">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq9" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label1" runat="server" Text="Plano de conta" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtPlanoConta" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label20" runat="server" ForeColor="#0066FF" Text="Data Cad."></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq10" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblNumCte" runat="server" Text="Num. CTe" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCte" runat="server" MaxLength="10" Width="60px"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label16" runat="server" ForeColor="#0066FF" Text="Transportadora"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumTransportadora" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getTransportadora(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeTransportadora" runat="server" Width="170px" onkeypress="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label22" runat="server" ForeColor="#0066FF" Text="Forma Pagto."></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="ddlFiltroFormaPagto" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsFormaPagto" DataTextField="Descricao" DataValueField="IdFormaPagto">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label14" runat="server" ForeColor="#0066FF" Text="Período Baixado"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataBaixadoIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataBaixadoFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label15" runat="server" ForeColor="#0066FF" Text="Período Emissão NF-e/Compra"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataNfCompraIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataNfCompraFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblFuncComissao" runat="server" ForeColor="#0066FF" Text="Func. Comissão"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncComissao" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="true">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbFuncComissao" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkIncluirCheques" runat="server" Text="Incluir cheques" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkPrevisaoCustoFixo" runat="server" Text="Exibir previsão de custo fixo" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkCustoFixo" runat="server" Text="Contas de custo fixo" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkComissao" runat="server" Text="Contas de comissão" AutoPostBack="True" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkContasSemValor" runat="server" Text="Contas sem valor" AutoPostBack="True" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label21" runat="server" Text="Agrupar impressão por:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpAgrupar" runat="server">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="1" Selected="True">Fornecedor</asp:ListItem>
                                <asp:ListItem Value="2">Data Venc.</asp:ListItem>
                                <asp:ListItem Value="3">Data Cad.</asp:ListItem>
                                <asp:ListItem Value="4">Plano de Conta</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;<asp:GridView GridLines="None" ID="grdConta" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdContaPg" DataSourceID="odsContasPagar"
                    EmptyDataText="Nenhuma conta a pagar encontrada." CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" OnRowDataBound="grdConta_RowDataBound"
                    OnSorted="grdConta_Sorted">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEditar" runat="server" Visible='<%# Eval("EditVisible") %>'
                                    ToolTip="Editar" CommandName="Edit">
                                    <img border="0" src="../Images/EditarGrid.gif" /></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" CausesValidation="False" />
                                <asp:HiddenField ID="hdfIdContaPg" runat="server" Value='<%# Bind("IdContaPg") %>' />
                                <asp:HiddenField ID="hdfIdNf" runat="server" Value='<%# Bind("IdNf") %>' />
                                <asp:HiddenField ID="hdfIdComissao" runat="server" Value='<%# Bind("IdComissao") %>' />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="IdContaPg">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("IdContaPg") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("IdContaPg") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referência" SortExpression="concat(coalesce(IdCompra,0), coalesce(IdPagto,0), coalesce(NumeroNf,0), coalesce(Nf,0), coalesce(NumBoleto,0), coalesce(IdCustoFixo,0))">
                            <ItemTemplate>
                                <asp:Label ID="label121" runat="server" Text='<%# Bind("Referencia") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="label123" runat="server" Text='<%# Eval("Referencia") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fornecedor/Transportador/Func." SortExpression="NomeExibir">
                            <EditItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("NomeExibir") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("NomeExibir") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referente a" SortExpression="concat(coalesce(DescrPlanoConta,''), coalesce(Obs,''), coalesce(ObsCompra,''), coalesce(IdPagtoRestante,''), coalesce(IdCustoFixo,''))">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpPlanoConta" runat="server" DataSourceID="odsPlanoConta" DataTextField="DescrPlanoGrupo"
                                    DataValueField="IdConta" SelectedValue='<%# Bind("IdConta") %>' AppendDataBoundItems="True" OnDataBinding="drpPlanoConta_DataBinding">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("DescrContaPagar") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Forma Pagto." SortExpression="FormaPagtoCompra">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpFormaPagto" runat="server" AppendDataBoundItems="True" DataSourceID="odsFormaPagto" 
                                    DataTextField="Descricao" DataValueField="IdFormaPagto" SelectedValue='<%# Bind("IdFormaPagto") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("FormaPagtoCompra") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Parc." SortExpression="NumParc, NumParcMax" ItemStyle-Wrap="false">
                            <EditItemTemplate>
                                <asp:Label ID="label124" runat="server" Text='<%# Eval("DescrNumParc") %>'></asp:Label>
                                <asp:HiddenField ID="hdfParc" runat="server" Value='<%# Bind("NumParc") %>' />
                                <asp:HiddenField ID="hdfParcMax" runat="server" Value='<%# Bind("NumParcMax") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="label125" runat="server" Text='<%# Eval("DescrNumParc") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False"></ItemStyle>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVenc">
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("ValorVenc", "{0:C}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("ValorVenc", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vencimento" SortExpression="DataVenc">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDataVenc" runat="server" Text='<%# Bind("DataVenc", "{0:d}") %>' Visible='<%# EditarDataVencimento() %>'
                                    onkeydown="return false;" oncut="return false;" onpaste="return false;" Width="80px"></asp:TextBox>
                                &nbsp;<asp:ImageButton ID="imgDataVenc" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                    OnClientClick="return SelecionaData('txtDataVenc', this)" ToolTip="Alterar" Visible='<%# EditarDataVencimento() %>' />

                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("DataVenc", "{0:d}") %>' Visible='<%# !EditarDataVencimento() %>' ></asp:Label>
                                <br />
                                <asp:CustomValidator ID="ctvData" runat="server" ClientValidationFunction="validarDataVenc"
                                    ControlToValidate="txtDataVenc" Display="Dynamic" ErrorMessage="Somente o dia do vencimento&lt;br&gt;pode ser alterado."
                                    ValidateEmptyText="True"></asp:CustomValidator>
                                <asp:HiddenField ID="hdfDataVenc" runat="server" Value='<%# Eval("DataVenc", "{0:d}") %>' />
                                <asp:HiddenField ID="hdfIdCustoFixo" runat="server" Value='<%# Bind("IdCustoFixo") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("DataVenc", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Cad." SortExpression="DataCad">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("DataCad", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("DataCad", "{0:d}") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Boleto Chegou?" SortExpression="BoletoChegou">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("BoletoChegouString") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("BoletoChegouString") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" Text='<%# Bind("Obs") %>' Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                                <asp:Label ID="Label32" runat="server" Text='<%# Bind("ObsDescAcresc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="DescricaoContaContabil">
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("DescricaoContaContabil") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("DescricaoContaContabil") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc3:ctrlLogPopup ID="ctrlLogContaPagar" runat="server" Tabela="ContaPagar" IdRegistro='<%# Eval("IdContaPg") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContasPagar" runat="server" SelectByKeysMethod="GetElement"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetPagtosCount"
                    SelectMethod="GetPagtos" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ContasPagarDAO" DataObjectTypeName="Glass.Data.Model.ContasPagar"
                    UpdateMethod="Update" OnUpdated="odsContasPagar_Updated" UpdateStrategy="GetAndUpdate">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtIdContaPg" Name="idContaPg" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumCompra" Name="idCompra" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNF" Name="nf" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtIdCustoFixo" Name="idCustoFixo" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtImpServ" Name="idImpostoServ" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumFornec" Name="idFornec" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeFornec" Name="nomeFornec" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ddlFiltroFormaPagto" Name="idsFormaPagto" PropertyName="SelectedValues"
                            Type="Object" />
                        <asp:ControlParameter ControlID="txtPrecoInicial" Name="valorInicial" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="txtPrecoFinal" Name="valorFinal" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="chkIncluirCheques" Name="incluirCheques" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="drpTipo" Name="tipo" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkPrevisaoCustoFixo" Name="previsaoCustoFixo" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="chkComissao" Name="comissao" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="txtPlanoConta" Name="planoConta" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtIdPagtoRestante" Name="idPagtoRestante" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="chkCustoFixo" Name="custoFixo" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="chkContasSemValor" Name="contasSemValor" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="ctrlDataBaixadoIni" Name="dtBaixadoIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataBaixadoFim" Name="dtBaixadoFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataNfCompraIni" Name="dtNfCompraIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataNfCompraFim" Name="dtNfCompraFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNumCte" Name="numCte" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumTransportadora" Name="idTransportadora" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeTransportadora" Name="nomeTransportadora"
                            PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpFuncComissao" Name="idFuncComissao"
                            PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="txtComissao" Name="idComissao" PropertyName="Text"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForCompra"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetVendedorForComissaoContasReceber" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPlanoConta" runat="server" SelectMethod="GetPlanoContas"
                    TypeName="Glass.Data.DAL.PlanoContasDAO">
                    <SelectParameters>
                        <asp:Parameter Name="tipo" Type="Int32" DefaultValue="2"/>
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfOrdenar" runat="server" />
            </td>
        </tr>
    </table>

    <script type="text/javascript">

        var comissao = FindControl("chkComissao", "input").checked;

        FindControl("lblFuncComissao", "span").style.display = comissao ? "" : "none";
        FindControl("drpFuncComissao", "select").style.display = comissao ? "" : "none";
        FindControl("imbFuncComissao", "input").style.display = comissao ? "" : "none";

        if (comissao)
            FindControl("drpNomeFunc", "select").value = "";

    </script>

</asp:Content>
