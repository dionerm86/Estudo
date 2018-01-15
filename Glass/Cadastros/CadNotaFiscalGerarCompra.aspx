<%@ Page Title="Gerar Nota Fiscal de Compra" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadNotaFiscalGerarCompra.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadNotaFiscalGerarCompra" %>

<%@ Register Src="../Controls/ctrlNaturezaOperacao.ascx" TagName="ctrlNaturezaOperacao"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        var fornecedores = new Array();
        var buscandoFornec = false;

        function addCompra(idCompra) {
            if (buscandoFornec)
                return;

            if (idCompra == null || idCompra == 0)
                idCompra = FindControl("txtNumCompra", "input").value;

            if (Trim(idCompra) == "") {
                alert("Selecione uma compra para continuar.");
                FindControl("txtNumCompra", "input").value = "";
                FindControl("txtNumCompra", "input").focus();
                return;
            }

            var valida = CadNotaFiscalGerarCompra.ValidaCompra(idCompra).value.split(';');
            if (valida[0] == "Erro") {
                alert(valida[1]);
                FindControl("txtNumCompra", "input").value = "";
                FindControl("txtNumCompra", "input").focus();
                return;
            }

            var idsCompras = FindControl("hdfBuscarIdsCompras", "input").value.split(',');
            var novosIds = new Array();

            novosIds.push(idCompra);
            for (i = 0; i < idsCompras.length; i++)
                if (idsCompras[i] != idCompra && idsCompras[i].length > 0)
                    novosIds.push(idsCompras[i]);

            FindControl("hdfBuscarIdsCompras", "input").value = novosIds.join(',');

            if (FindControl("txtNumCompra", "input")) {
                FindControl("txtNumCompra", "input").value = "";
                cOnClick("btnBuscarCompras", null);
            }
            else
                atualizarPagina();
        }

        function removeCompra(idCompra, atualiza) {
            var idsCompras = FindControl("hdfBuscarIdsCompras", "input").value.split(',');
            var novosIds = new Array();

            for (i = 0; i < idsCompras.length; i++)
                if (idsCompras[i] != idCompra && idsCompras[i].length > 0)
                    novosIds.push(idsCompras[i]);

            FindControl("hdfBuscarIdsCompras", "input").value = novosIds.join(',');
            
            if (atualiza)
                cOnClick("btnBuscarCompras", null);
        }

        function buscarCompras() {
            var idFornec = "";
            var nomeFornec = "";
            
            if (FindControl("txtIdFornec", "input") != null && FindControl("txtIdFornec", "input") != undefined &&
                FindControl("txtNomeFornec", "input") != null && FindControl("txtNomeFornec", "input") != undefined) {
                if (FindControl("txtIdFornec", "input").value != "" && FindControl("txtNomeFornec", "input").value != "") {
                    idFornec = FindControl("txtIdFornec", "input").value;
                    nomeFornec = FindControl("txtNomeFornec", "input").value;
                }
                else
                    return;
            }
            else
                return;

            buscandoFornec = true;

            if (idFornec == "")
                idFornec = "0";

            FindControl("hdfBuscarIdsCompras", "input").value = CadNotaFiscalGerarCompra.GetComprasByFornecedor(idFornec, nomeFornec).value;
        }

        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornec(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNomeFornec", "input").value = "";
                return false;
            }

            FindControl("txtNomeFornec", "input").value = retorno[1];
        }

        function gerarNf(botao, dadosNaturezasOperacao) {
            if (!validate())
                return false;

            // Informa sobre os pedidos que já contém notas fiscais
            var tabela = document.getElementById("<%= grdCompras.ClientID %>");
            var comprasNf = new Array();

            for (i = 1; i < tabela.rows.length; i++) {
                var inputs = tabela.rows[i].cells[0].getElementsByTagName("input");
                var idCompra;
                var notasGeradas;

                for (j = 0; j < inputs.length; j++)
                    if (inputs[j].id.indexOf("hdfIdCompra") > -1)
                        idCompra = inputs[j].value;
                    else if (inputs[j].id.indexOf("hdfNotasGeradas") > -1)
                        notasGeradas = inputs[j].value;

                if (notasGeradas.length > 0)
                    comprasNf.push(new Array(idCompra, notasGeradas));
            }

            if (!dadosNaturezasOperacao && comprasNf.length > 0) {
                var compras = "";
                for (i = 0; i < comprasNf.length; i++)
                    compras += ", " + comprasNf[i][0] + " (NF " + comprasNf[i][1] + ")";

                if (!confirm("As seguintes compras já possuem notas fiscais geradas para elas:\n" +
                    compras.substr(2) + ".\n\nDeseja continuar com a geração das notas?"))
                    return false;
            }

            var idFornec = FindControl("hdfIdFornec", "input").value;
            var idsCompras = FindControl("hdfBuscarIdsCompras", "input").value;
            var idNaturezaOperacao = FindControl("ctrlNaturezaOperacao_selNaturezaOperacao_hdfValor", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var numeroNFe = FindControl("txtNumeroNFe", "input").value;
            var tipoCompra = FindControl("drpTipoCompra", "select").value;
            var planoConta = FindControl("drpPlanoContas", "select").value;
            
            if (idLoja == "" || idLoja == "0") {
                alert("Informe a loja.");
                return false;
            }

            if (numeroNFe == "") {
                alert("Informe o número da NF-e.");
                return false;
            }

            if (tipoCompra == "") {
                alert("Informe o tipo de compra.");
                return false;
            }

            if (planoConta == "") {
                alert("Informe o plano de contas.");
                return false;
            }
            
            botao.disabled = true;
            bloquearPagina();

            if (FindControl("chkNaturezaOperacaoPorProduto", "input").checked && !dadosNaturezasOperacao) {
                var agruparProduto = CadNotaFiscalGerarCompra.GetAgruparProdutoNf().value;

                openWindow(500, 750, "../Utils/SetNaturezaOperacaoProdutoGerarNf.aspx?idFornec=" + idFornec + "&idsCompras=" + idsCompras +
                    "&idNaturezaOperacao=" + idNaturezaOperacao + "&idLoja=" + idLoja + "&agruparProdutos=" + agruparProduto);

                return false;
            }

            var retorno = CadNotaFiscalGerarCompra.GerarNf(idsCompras, idNaturezaOperacao,
                idLoja, dadosNaturezasOperacao, idFornec, tipoCompra, planoConta, numeroNFe).value.split(';');
                
            if (retorno[0] == "Erro")
                FalhaGerarNf(retorno[1]);
            else
                NfGerada(retorno[1], true);
        }

        function NfGerada(idNf, popup) {
            desbloquearPagina(true);
            alert("Nota fiscal gerada com sucesso!");
            redirectUrl((popup ? "../Cadastros/" : "") + "CadNotaFiscal.aspx?idNf=" + idNf);
        }

        function FalhaGerarNf(erro, fechouJanela) {
            desbloquearPagina(true);
            
            if (!fechouJanela)
                alert(erro);

            FindControl("btnGerarNf", "input").disabled = false;
            return;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table runat="server" id="buscar">
                    <tr >
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="center" id="compra_titulo" runat="server">
                                        <asp:Label ID="lblCompra" runat="server" Text="Compra" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td id="compra_campo" runat="server">
                                        <asp:TextBox ID="txtNumCompra" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                            onkeydown="if (isEnter(event)) cOnClick('imbAddCompra', null);"></asp:TextBox>
                                    </td>
                                    <td id="compra_buscar" runat="server">
                                        <asp:ImageButton ID="imbAddCompra" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addCompra(); return false;" />
                                    </td>
                                </tr>
                            </table>
                            <table id="fornecedor" runat="server">
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="lblFornec" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtIdFornec" onkeypress="return soNumeros(event, true, true);" runat="server"
                                            Width="50px" onblur="getFornec(this)"></asp:TextBox>
                                        <asp:TextBox ID="txtNomeFornec" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('btnBuscarCompras', null);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="lnkSelFornec" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelFornec.aspx'); return false;">
                                            <img border="0" src="../Images/Pesquisar.gif" />
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                            <asp:Button ID="btnBuscarCompras" runat="server" Text="Buscar Compras" OnClick="btnBuscarCompras_Click"
                                OnClientClick="buscarCompras()" CausesValidation="False" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfBuscarIdsCompras" runat="server" />
                <asp:HiddenField ID="hdfIdFornec" runat="server" />
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkNaturezaOperacaoPorProduto" runat="server" Text="Selecionar natureza de operação por produto" />
                        </td>
                    </tr>
                </table>
                <br />
                <table id="gerar" runat="server" visible="false">
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdCompras" runat="server" AutoGenerateColumns="False"
                                DataSourceID="odsCompras" DataKeyNames="IdCompra" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                                AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Não foram encontrados compras confirmadas para esse fornecedor ou com esse número."
                                EnableViewState="False" OnDataBound="grdCompras_DataBound">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imbExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                                OnClientClick='<%# "removeCompra(" + Eval("IdCompra") + ", true); return false;" %>'
                                                ToolTip="Remover pedido" Visible='<%# buscar.Visible %>' />
                                            <asp:HiddenField ID="hdfIdCompra" runat="server" Value='<%# Eval("IdCompra") %>' />
                                            <asp:HiddenField ID="hdfNotasGeradas" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="IdCompra" HeaderText="Compra" SortExpression="IdCompra" />
                                    <asp:BoundField DataField="DescrUsuCad" HeaderText="Funcionário" SortExpression="DescrUsuCad" />
                                    <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                                    <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                                    <asp:BoundField DataField="Obs" HeaderText="Obs. Compra" SortExpression="Obs" />
                                    <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTotalCompra" runat="server" Text='<%# Bind("Total", "{0:C}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data Compra"
                                        SortExpression="DataCad" />
                                </Columns>
                                <PagerStyle CssClass="pgr"></PagerStyle>
                                <EditRowStyle CssClass="edit"></EditRowStyle>
                                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                            </asp:GridView>
                            <br />
                            <div style="font-size: medium; text-align: center">
                                Total:
                                <asp:Label ID="lblTotal" runat="server" Text=""></asp:Label>
                            </div>
                            <asp:Label ID="lblInfoBloqueioCompras" runat="server" Text="<br />Compras em Vermelho já possuem notas fiscais geradas."
                                ForeColor="Red"></asp:Label>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCompras" runat="server" SelectMethod="GetForNFe"
                                TypeName="Glass.Data.DAL.CompraDAO" OnSelected="odsCompras_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="hdfBuscarIdsCompras" Name="idsCompras" PropertyName="Value"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="txtIdFornec" Name="idFornec" PropertyName="Text" Type="UInt32" />
                                    <asp:ControlParameter ControlID="txtNomeFornec" Name="nomeFornec" PropertyName="Text"
                                        Type="String" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                            <br />
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td>
                                        Natureza de Operação:
                                    </td>
                                    <td>
                                        <uc1:ctrlNaturezaOperacao ID="ctrlNaturezaOperacao" runat="server" 
                                            PermitirVazio="False" />
                                    </td>
                                    <td>
                                        &nbsp;&nbsp;
                                    </td>
                                    <td>
                                        Loja:
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                            DataValueField="IdLoja">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Número NF-e:
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtNumeroNFe" Width="80px"
                                            onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                                    </td>
                                    <td>
                                        &nbsp;&nbsp;
                                    </td>
                                    <td>
                                        Tipo de Compra:
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpTipoCompra" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Value="1">À vista</asp:ListItem>
                                            <asp:ListItem Value="2">À prazo</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Plano de contas
                                    </td>
                                    <td colspan="4" align="center">
                                        <asp:DropDownList ID="drpPlanoContas" runat="server" DataSourceID="odsPlanoContas"
                                            DataTextField="DescrPlanoGrupo" DataValueField="IdConta" AppendDataBoundItems="true">
                                            <asp:ListItem></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="5" align="center">
                                        &nbsp
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="5" align="center">
                                        <asp:Button ID="btnGerarNf" runat="server" OnClientClick="gerarNf(this, ''); return false;"
                                            Text="Gerar NF" />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblMensagem" runat="server"></asp:Label>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" OnSelected="odsLoja_Selected"
                                            SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                                        </colo:VirtualObjectDataSource>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPlanoContas" runat="server"
                                            SelectMethod="GetPlanoContasCompra" TypeName="Glass.Data.DAL.PlanoContasDAO">
                                        </colo:VirtualObjectDataSource>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <script type="text/javascript">

        if (FindControl("txtNumCompra", "input") != null && FindControl("txtNumCompra", "input") != undefined) {
            if (!FindControl("txtNumCompra", "input").disabled)
                FindControl("txtNumCompra", "input").focus();
        }
        
    </script>

</asp:Content>
