<%@ Page Title="Sugestão de Compra" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadSugestaoCompra.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadSugestaoCompra"
    EnableEventValidation="false" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        var isProducao = '<%= Request["producao"] != null ? Request["producao"] : "0" %>';

        function openRpt(exportarExcel) {
            var idsProd = getIdsProd();
            if (idsProd.length == 1 && idsProd[0] == "") {
                alert("Escolha ao menos 1 produto para exibir o relatório.");
                return false;
            }

            var diasEstoque = FindControl("txtDiasEstoque", "input").value;
            var vendasMeses = FindControl("txtVendasMeses", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=SugestaoCompra&isProducao=" + isProducao +
                "&idLoja=" + FindControl("drpLoja", "select").value + "&idsProd=" + idsProd.join(",") + "&diasEstoque=" + diasEstoque + "&vendasMeses=" + vendasMeses);
        }

        function getSubgrupos(idGrupo) {
            var resposta = CadSugestaoCompra.GetSubgrupos(idGrupo, isProducao).value.split('#');
            if (resposta[0] == "Erro")
                alert(resposta[1]);
            else
                FindControl("drpSubgrupo", "select").innerHTML = resposta[1];
        }

        function verificarProd(idProd) {
            var ids = FindControl("hdfIdProdutos", "input").value.split(",");
            for (v = 0; v < ids.length; v++)
                if (idProd == ids[v])
                    return true;

            return false;
        }

        function adicionar() {
            var idLoja = FindControl("drpLoja", "select").value;
            var idGrupo = FindControl("drpGrupo", "select").value;
            var idSubgrupo = FindControl("drpSubgrupo", "select").value;
            var codProduto = FindControl("txtCodProd", "input").value;
            var descrProduto = FindControl("txtDescrProd", "input").value;
            var diasEstoque = FindControl("txtDiasEstoque", "input").value;
            var vendasMeses = FindControl("txtVendasMeses", "input").value;

            if (idGrupo == "0" && idSubgrupo == "0" && codProduto == "" && descrProduto == "")
                if (!confirm("Deseja incluir todos os produtos que estão com estoque abaixo do mínimo?")) {
                    alert("Selecione um filtro para adicionar os produtos.");
                    return;
                }

            var resposta = CadSugestaoCompra.GetProdutos(idLoja, idGrupo, idSubgrupo, codProduto, descrProduto, isProducao, vendasMeses, diasEstoque).value.split('#');
            if (resposta[0] == "Erro") {
                alert(resposta[1]);
                return;
            }

            var titulos = new Array("Cód.", "Descrição", "Grupo / Subgrupo", "Estoque Mínimo", "Qtde. sugerida " + (isProducao != "1" ? "compra" : "produção"), "Estoque Disponível", "Valor Unitário", "Total");

            if (vendasMeses > 0)
                titulos = new Array("Cód.", "Descrição", "Grupo / Subgrupo", "Média Mensal", "Qtde. sugerida compra", "Estoque Disponível", "Valor Unitário", "Total");

            var inserido = false;
            var produtos = resposta[1].split("|");

            for (add = 0; add < produtos.length; add++) {
                if (produtos[add] == "")
                    continue;

                var dadosProd = produtos[add].split(";");
                if (verificarProd(dadosProd[0]))
                    continue;

                var itensProd = new Array(dadosProd[1], dadosProd[2], dadosProd[3] + (dadosProd[4] != "" ? " / " + dadosProd[4] : ""), dadosProd[5], dadosProd[6], dadosProd[7], dadosProd[8], dadosProd[9]);

                if (vendasMeses > 0)
                    itensProd = new Array(dadosProd[1], dadosProd[2], dadosProd[3] + (dadosProd[4] != "" ? " / " + dadosProd[4] : ""), dadosProd[10], dadosProd[6], dadosProd[7], dadosProd[8], dadosProd[9]);

                addItem(itensProd, titulos, "tbProdutos", dadosProd[0], "hdfIdProdutos", null, null, "atualizaTotal", false);

                inserido = true;
            }

            atualizaTotal();

            if (!inserido)
                alert("Nenhum produto foi adicionado.");
        }

        function atualizaTotal() {
            var tabela = document.getElementById("tbProdutos");
            var total = 0;

            for (i = 1; i < tabela.rows.length; i++) {
                if (tabela.rows[i].style.display == "none")
                    continue;
                
                var totalItem = tabela.rows[i].cells[8].innerHTML;
                totalItem = parseFloat(totalItem.replace("R$", "").replace(" ", "").replace(/\./g, "").replace(",", "."));
                
                total += !isNaN(totalItem) ? totalItem : 0;
            }

            FindControl("lblTotalCompra", "span").innerHTML = total.toFixed(2).replace(".", ",");
        }

        function getIdsProd() {
            var idsProd = FindControl("hdfIdProdutos", "input").value.split(',');

            // Remove a última posição do vetor (produto em branco)
            if (idsProd.length > 0)
                idsProd.pop();

            return idsProd;
        }

        function gerarCompra() {
            var idsProd = getIdsProd();
            if (idsProd.length == 1 && idsProd[0] == "") {
                alert("Escolha ao menos 1 produto para gerar a compra.");
                return false;
            }

            if (!confirm("Deseja gerar uma compra com esses produtos?"))
                return false;

            var idLoja = FindControl("drpLoja", "select").value;
            var diasEstoque = FindControl("txtDiasEstoque", "input").value;
            var vendasMeses = FindControl("txtVendasMeses", "input").value;

            var resposta = CadSugestaoCompra.GerarCompra(idLoja, idsProd.join(","), vendasMeses, diasEstoque).value.split("#");
            if (resposta[0] == "Erro") {
                alert(resposta[1]);
                return false;
            }
            else {
                if (!confirm("Compra gerada com sucesso! Deseja gerar outra compra?"))
                    redirectUrl("CadCompra.aspx?idCompra=" + resposta[1]);
                else {

                    var idGrupo = FindControl("drpGrupo", "select");
                    var idSubgrupo = FindControl("drpSubgrupo", "select");

                    idGrupo.selectedIndex = 0;
                    idSubgrupo.selectedIndex = 0;

                    resetaGrid();
                }
            }
        }

        function resetaGrid() {
            resetGrid(document.getElementById("tbProdutos"));
        }

        function gerarPedido() {
            var idsProd = getIdsProd();
            if (idsProd.length == 1 && idsProd[0] == "") {
                alert("Escolha ao menos 1 produto para gerar o pedido.");
                return false;
            }

            if (!confirm("Deseja gerar um pedido com esses produtos?"))
                return false;

            var idLoja = FindControl("drpLoja", "select").value;

            var resposta = CadSugestaoCompra.GerarPedido(idLoja, idsProd.join(",")).value.split("#");
            if (resposta[0] == "Erro") {
                alert(resposta[1]);
                return false;
            }
            else {
                alert("Pedido gerado com sucesso!");
                redirectUrl("CadPedido.aspx?idPedido=" + resposta[1]);
            }
        }

        function tipoSugestaoChanged(control) {
            FindControl("tbSugestaoVendas", "table").style.display = control.value == "0" ? "none" : "";
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Tipo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoSugestao" runat="server" onchange="return tipoSugestaoChanged(this);">
                                <asp:ListItem Value="0">Sugestão de compras baseada no estoque mínimo</asp:ListItem>
                                <asp:ListItem Value="1">Sugestão de compras baseada nas vendas</asp:ListItem>
                            </asp:DropDownList></td>

                    </tr>
                </table>
                <table id="tbSugestaoVendas" style="display: none;">
                    <tr>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Número de meses previsto para manter estoque" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDiasEstoque" runat="server" Width="50px">
                            </asp:TextBox></td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Vendas realizadas nos últimos meses" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtVendasMeses" runat="server" Width="50px">
                            </asp:TextBox></td>

                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="false"
                                MostrarTodas="false" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupo" runat="server" DataSourceID="odsGrupo" onchange="getSubgrupos(this.value)"
                                DataTextField="Descricao" DataValueField="IdGrupoProd" OnDataBound="drpGrupo_DataBound">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupo" runat="server" DataSourceID="odsSubgrupo" DataTextField="Descricao"
                                DataValueField="IdSubgrupoProd">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px"></asp:TextBox>
                            <asp:TextBox ID="txtDescrProd" runat="server" Width="180px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <asp:LinkButton ID="lnkAdicionar" runat="server" OnClientClick="adicionar(); return false"> <img src="../Images/Insert.gif" border="0" /> Adicionar produto(s)</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table id="tbProdutos">
                </table>
                <span id="totalCompra" runat="server" style="font-size: medium">
                    <br />
                    <asp:Label ID="lblTituloTotalCompra" runat="server" Text="Valor total da compra: R$"></asp:Label>
                    <asp:Label ID="lblTotalCompra" runat="server">0,00</asp:Label>
                </span>
                <asp:HiddenField ID="hdfIdProdutos" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:Button ID="btnGerarCompra" runat="server" Text="Gerar Compra" OnClientClick="gerarCompra(); return false" />
                &nbsp;<asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false">
                    <img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.GrupoProdDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupo" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
