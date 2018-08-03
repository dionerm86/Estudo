<%@ Page Title="Contas Pagas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="ListaContasPagas.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaContasPagas" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(exportarExcel, exportarGCON, exportarProsoft, exportarDominio) {
            var idContaPg = FindControl("txtIdContaPg", "input").value;
            var idCompra = FindControl("txtNumCompra", "input").value;
            var nf = FindControl("txtNF", "input").value;
            var idFornec = FindControl("txtFornecedor", "input").value;
            var nomeFornec = FindControl("txtNome", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idCustoFixo = FindControl("txtIdCustoFixo", "input").value;
            var idImpServ = FindControl("txtImpServ", "input").value;
            var dataIniCad = FindControl("ctrlDataIniCad_txtData", "input").value;
            var dataFimCad = FindControl("ctrlDataFimCad_txtData", "input").value;
            var dtIniPago = FindControl("ctrlDataIniPago_txtData", "input").value;
            var dtFimPago = FindControl("ctrlDataFimPago_txtData", "input").value;
            var dtIniVenc = FindControl("ctrlDataIniVenc_txtData", "input").value;
            var dtFimVenc = FindControl("ctrlDataFimVenc_txtData", "input").value;
            var formaPagto = FindControl("cblFormaPagto", "select").itens();
            var valorIni = FindControl("txtPrecoInicial", "input").value;
            var valorFin = FindControl("txtPrecoFinal", "input").value;
            var tipo = FindControl("drpTipo", "select").value;
            var comissao = FindControl("chkComissao", "input").checked;
            var renegociadas = FindControl("chkRenegociadas", "input").checked;
            var agrupar = FindControl("drpAgrupar", "select").value;
            var custoFixo = FindControl("chkCustoFixo", "input").checked;
            var ordenar = FindControl("hdfOrdenar", "input").value;
            var planoConta = FindControl("txtPlanoConta", "input").value;
            var exibirAPagar = FindControl("chkContasAPagar", "input").checked;
            var jurosMulta = FindControl("chkJurosMulta", "input").checked;
            var idComissao = FindControl("txtComissao", "input").value;
            var numCte = FindControl("txtNumCte", "input").value;
            var observacao = FindControl("txtObservacao", "input").value;

            valorIni = valorIni.replace('.', '');
            valorFin = valorFin.replace('.', '');

            var queryString = idCompra == "" ? "&idCompra=0" : "&idCompra=" + idCompra;
            queryString += idFornec == "" ? "&idFornec=0" : "&idFornec=" + idFornec;
            queryString += "&idComissao=" + idComissao;
            queryString += "&numCte=" + numCte;

            if (!exportarGCON && !exportarProsoft && !exportarDominio) {
                openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=ContasPagas&nomeFornec=" + nomeFornec + "&idLoja=" + idLoja + "&nf=" + nf +
                    "&idContaPg=" + idContaPg + "&dataIniCad=" + dataIniCad + "&dataFimCad=" + dataFimCad +
                    "&dtIniPago=" + dtIniPago + "&dtFimPago=" + dtFimPago + "&dtIniVenc=" + dtIniVenc + "&dtFimVenc=" + dtFimVenc +
                    "&formaPagto=" + formaPagto + "&valorInicial=" + valorIni + "&valorFinal=" + valorFin + "&tipo=" + tipo + queryString +
                    "&planoConta=" + planoConta + "&exportarExcel=" + exportarExcel + "&comissao=" + comissao + "&renegociadas=" + renegociadas + "&jurosMulta=" + jurosMulta +
                    "&custoFixo=" + custoFixo + "&idCustoFixo=" + idCustoFixo + "&idImpostoServ=" + idImpServ + "&agrupar=" + agrupar +
                    "&ordenar=" + ordenar + "&exibirAPagar=" + exibirAPagar + "&observacao=" + observacao);
            }
            else if (exportarGCON) {
                window.open("../Handlers/ArquivoGCon.ashx?nomeFornec=" + nomeFornec + "&idLoja=" + idLoja + "&numeroNFe=" + nf +
                    "&idContaPg=" + idContaPg + "&dataIniCad=" + dataIniCad + "&dataFimCad=" + dataFimCad +
                    "&dtIniRec=" + dtIniPago + "&dtFimRec=" + dtFimPago + "&dtIniVenc=" + dtIniVenc + "&dtFimVenc=" + dtFimVenc +
                    "&idFormaPagto=" + formaPagto + "&valorInicial=" + valorIni + "&valorFinal=" + valorFin + "&tipo=" + tipo + queryString +
                    "&idConta=" + planoConta + "&exportarExcel=" + exportarExcel + "&comissao=" + comissao + "&renegociadas=" + renegociadas + "&jurosMulta=" + jurosMulta +
                    "&custoFixo=" + custoFixo + "&idCustoFixo=" + idCustoFixo + "&idImpServ=" + idImpServ + "&receber=false" + "&observacao=" + observacao);
            }
            else if (exportarProsoft) {
                window.open("../Handlers/ArquivoProsoft.ashx?nomeFornec=" + nomeFornec + "&idLoja=" + idLoja + "&numeroNFe=" + nf +
                   "&idContaPg=" + idContaPg + "&dtIniRec=" + dtIniPago + "&dtFimRec=" + dtFimPago + "&dtIniVenc=" + dtIniVenc + "&dtFimVenc=" + dtFimVenc +
                   "&idFormaPagto=" + formaPagto + "&valorInicial=" + valorIni + "&valorFinal=" + valorFin + "&tipo=" + tipo + queryString +
                   "&idConta=" + planoConta + "&exportarExcel=" + exportarExcel + "&comissao=" + comissao + "&renegociadas=" + renegociadas + "&jurosMulta=" + jurosMulta +
                   "&custoFixo=" + custoFixo + "&idCustoFixo=" + idCustoFixo + "&idImpServ=" + idImpServ + "&receber=false" + "&observacao=" + observacao);
            }
            else if (exportarDominio) {
                window.open("../Handlers/ArquivoDominio.ashx?nomeFornec=" + nomeFornec + "&idLoja=" + idLoja + "&nf=" + nf +
                    "&idContaPg=" + idContaPg + "&dataIniCad=" + dataIniCad + "&dataFimCad=" + dataFimCad +
                    "&dtIniPago=" + dtIniPago + "&dtFimPago=" + dtFimPago + "&dtIniVenc=" + dtIniVenc + "&dtFimVenc=" + dtFimVenc +
                    "&formaPagto=" + formaPagto + "&valorInicial=" + valorIni + "&valorFinal=" + valorFin + "&tipo=" + tipo + queryString +
                    "&planoConta=" + planoConta + "&exportarExcel=" + exportarExcel + "&comissao=" + comissao + "&renegociadas=" + renegociadas +
                    "&custoFixo=" + custoFixo + "&idCustoFixo=" + idCustoFixo + "&idImpostoServ=" + idImpServ + "&agrupar=" + agrupar +
                    "&exibirAPagar=" + exibirAPagar + "&receber=false" + "&observacao=" + observacao);
            }

            return false;
        }

        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function openFornec() {
            if (FindControl("txtFornecedor", "input").value != "")
                return true;

            openWindow(500, 700, "../Utils/SelFornec.aspx");

            return false;
        }

        function openPlanoConta() {
        
            var planoConta = FindControl("txtPlanoConta", "input");
            
            if (planoConta == null)
                return false;

            if (planoConta.value.trim() != "")
                return true;

            openWindow(500, 700, '../Utils/SelPlanoConta.aspx');
            return false;
        }

        function setPlanoConta(idConta, descricao) {
            var planoConta = FindControl("txtPlanoConta", "input");

            if (planoConta == null)
                return false;

            planoConta.value = descricao.split('-')[descricao.split('-').length - 1].trim();
            cOnClick("imgPesq");
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>                        
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label16" runat="server" ForeColor="#0066FF" Text="Cód. Conta Paga"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtIdContaPg" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label7" runat="server" ForeColor="#0066FF" Text="Num. Compra"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumCompra" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label17" runat="server" ForeColor="#0066FF" Text="NF/Pedido"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNF" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Custo Fixo"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtIdCustoFixo" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label126" runat="server" ForeColor="#0066FF" Text="Imp./Serv."></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtImpServ" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq12" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label23" runat="server" ForeColor="#0066FF" Text="Comissão"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtComissao" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label14" runat="server" ForeColor="#0066FF" Text="CT-e"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumCte" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Cad."></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIniCad" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFimCad" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label15" runat="server" ForeColor="#0066FF" Text="Venc."></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIniVenc" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFimVenc" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Pagto."></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIniPago" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFimPago" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq10" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" ForeColor="#0066FF" Text="Loja"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="false"/>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" ForeColor="#0066FF" Text="Fornecedor"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtFornecedor" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" Width="170px" onkeypress="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label11" runat="server" Text="Forma Pagto." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblFormaPagto" runat="server" DataSourceID="odsFormaPagto"
                                DataTextField="Descricao" DataValueField="IdFormaPagto" AutoPostBack="True" >
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq8" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label18" runat="server" Text="Valor Pago" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtPrecoInicial" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            até
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtPrecoFinal" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq9" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                         <td nowrap="nowrap">
                            <asp:Label ID="Label20" runat="server" ForeColor="#0066FF" Text="Tipo"></asp:Label>
                        </td>
                         <td nowrap="nowrap">
                            <asp:DropDownList ID="drpTipo" runat="server" OnLoad="drpTipo_Load">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                         <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                         <td nowrap="nowrap">
                            <asp:Label ID="Label22" runat="server" Text="Obs." ForeColor="#0066FF"></asp:Label>
                        </td>
                         <td nowrap="nowrap">
                            <asp:TextBox ID="txtObservacao" runat="server" Width="150px"></asp:TextBox>
                        </td>
                         <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                         <td nowrap="nowrap">
                            <asp:Label ID="Label25" runat="server" Text="Plano de conta" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label1" runat="server" Text="Plano de conta" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtPlanoConta" runat="server" Width="150px"></asp:TextBox>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            OnClientClick="return openPlanoConta();" ToolTip="Pesquisar"
                                            OnClick="imgPesq_Click" />
                                    </td>
                                    <td nowrap="nowrap">
                            <asp:Label ID="Label21" runat="server" Text="Agrupar impressão por:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpAgrupar" runat="server">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="1">Fornecedor</asp:ListItem>
                                <asp:ListItem Value="2">Data Venc.</asp:ListItem>
                                <asp:ListItem Value="3">Data Cad.</asp:ListItem>
                                <asp:ListItem Value="4">Plano de Conta</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td nowrap="nowrap">
                            <asp:CheckBox ID="chkContasAPagar" runat="server" Text="Exibir contas a pagar"
                            AutoPostBack="true" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:CheckBox ID="chkCustoFixo" runat="server" Text="Contas de custo fixo" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:CheckBox ID="chkComissao" runat="server" Text="Contas de comissão" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:CheckBox ID="chkRenegociadas" runat="server" Text="Exibir contas renegociadas" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:CheckBox ID="chkJurosMulta" runat="server" Text="Exibir contas com juros/multas" />
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
                <asp:GridView GridLines="None" ID="grdConta" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdContaPg" DataSourceID="odsContasPagar"
                    EmptyDataText="Nenhuma conta paga encontrada." CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" OnSorted="grdConta_Sorted"
                    OnRowDataBound="grdConta_RowDataBound">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />
                                <asp:HiddenField ID="hdfIdContaPg" runat="server" Value='<%# Bind("IdContaPg") %>' />
                                <asp:HiddenField ID="hdfPaga" runat="server" Value='<%# Bind("Paga") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEditar" runat="server" CommandName="Edit" ToolTip="Editar"
                                    Visible='<%# Eval("EditVisible") %>'>
                                <img border="0" src="../Images/EditarGrid.gif" /></asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="IdContaPg">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("IdContaPg") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdContaPg") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referência" SortExpression="concat(coalesce(c.IdCompra,0), coalesce(c.IdPagto,0), coalesce(nf.NumeroNfe,0), coalesce(cmp.Nf,0), coalesce(c.NumBoleto,0), coalesce(c.IdCustoFixo,0))">
                            <EditItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Referencia") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Referencia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fornecedor/Transportador/Func." SortExpression="NomeExibir">
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("NomeExibir") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("NomeExibir") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referente a" SortExpression="concat(coalesce(g.descricao,''), coalesce(pl.descricao,''), coalesce(c.Obs,''), coalesce(cmp.Obs,''), coalesce(c.IdPagtoRestante,''), coalesce(c.IdCustoFixo,''))">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpPlanoConta" runat="server" DataSourceID="odsPlanoConta" DataTextField="DescrPlanoGrupo"
                                    DataValueField="IdConta" SelectedValue='<%# Bind("IdConta") %>' AppendDataBoundItems="True">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DescrContaPagar") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Forma Pagto." SortExpression="FormaPagto">
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("DescrFormaPagto") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("DescrFormaPagto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Parc." SortExpression="NumParc, NumParcMax" ItemStyle-Wrap="false">
                            <EditItemTemplate>
                                <asp:Label ID="Label30" runat="server" Text='<%# Eval("DescrNumParc") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label30" runat="server" Text='<%# Eval("DescrNumParc") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False"></ItemStyle>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVenc">
                            <EditItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("ValorVenc", "{0:C}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("ValorVenc", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Pago" SortExpression="cast(concat(coalesce(c.ValorPago,0), coalesce(c.Juros,0), coalesce(c.Multa,0)) as signed)">
                            <EditItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("DescrValorPagto") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("DescrValorPagto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vencimento" SortExpression="DataVenc">
                            <EditItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("DataVenc", "{0:d}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("DataVenc", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pagto." SortExpression="DataPagto">
                            <EditItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("DataPagto", "{0:d}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("DataPagto", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Localização">
                            <EditItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("DestinoPagto") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("DestinoPagto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox11" runat="server" Text='<%# Bind("Obs") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                                <asp:Label ID="Label32" runat="server" Text='<%# Bind("ObsDescAcresc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="DescricaoContaContabil">
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" 
                                    Text='<%# Bind("DescricaoContaContabil") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label12" runat="server" 
                                    Text='<%# Bind("DescricaoContaContabil") %>'></asp:Label>
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
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkExportarGCON" runat="server" OnClientClick="openRpt(false, true, false, false); return false;"><img border="0" 
                    src="../Images/blocodenotas.png" /> Exportar para o GCON</asp:LinkButton>
                <asp:LinkButton ID="lnkExportarProsoft" runat="server" OnClientClick="openRpt(false, false, true, false); return false;"><img border="0" 
                    src="../Images/blocodenotas.png" /> Exportar para PROSOFT</asp:LinkButton>
                <asp:LinkButton ID="lnkExportarDominio" runat="server" OnClientClick="openRpt(false, false, false, true); return false;"><img border="0" 
                    src="../Images/blocodenotas.png" /> Exportar para DOMÍNIO SISTEMAS</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContasPagar" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetPagasCount" SelectMethod="GetPagas" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ContasPagarDAO" UpdateStrategy="GetAndUpdate"
                    DataObjectTypeName="Glass.Data.Model.ContasPagar" UpdateMethod="Update" SelectByKeysMethod="GetElement">
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
                        <asp:ControlParameter ControlID="txtFornecedor" Name="idFornec" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeFornec" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="cblFormaPagto" Name="formaPagto" PropertyName="SelectedValue"
                            Type="string" />
                        <asp:ControlParameter ControlID="ctrlDataIniCad" Name="dataIniCad" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimCad" Name="dataFimCad" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniPago" Name="dtIniPago" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimPago" Name="dtFimPago" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniVenc" Name="dtIniVenc" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimVenc" Name="dtFimVenc" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtPrecoInicial" Name="valorInicial" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="txtPrecoFinal" Name="valorFinal" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="drpTipo" Name="tipo" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkComissao" Name="comissao" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="chkRenegociadas" Name="renegociadas" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="chkJurosMulta" Name="jurosMulta" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="txtPlanoConta" Name="planoConta" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="chkCustoFixo" Name="custoFixo" PropertyName="Checked"
                            Type="Boolean" />
                            <asp:ControlParameter ControlID="chkContasAPagar" Name="exibirAPagar" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="txtComissao" Name="idComissao" PropertyName="Text"
                            Type="Int32" />
                         <asp:ControlParameter ControlID="txtNumCte" Name="numCte" PropertyName="Text"
                            Type="Int32" />
                         <asp:ControlParameter ControlID="txtObservacao" Name="observacao" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForFiltroPagto"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO">
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
</asp:Content>
