<%@ Page Title="Impressão de Etiquetas" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstEtiquetaBoxImprimir.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEtiquetaBoxImprimir" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <style type="text/css">
        .tabela
        {
            padding: 0;
            border-spacing: 0;
        }
        .tabela td
        {
            padding: 0 2px;
            margin: 0;
        }
    </style>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        var produtosJaAdicionados = false;
        var idsProdPedQtd = "";
        
        function verificaQtdeImprimir(idProdPed, qtdeProdPed, qtdeImpresso) {
            var qtdImprimir = FindControl("txtQtdImp_" + idProdPed, "input");
            
            if (qtdeProdPed - qtdeImpresso < qtdImprimir.value) {
                alert("A quantidade a ser impressa não pode ser maior que a quantidade do produto.");
                qtdImprimir.value = qtdeProdPed - qtdeImpresso;
                
                return false;
            }
        }

        function getProduto() {
            var numero = FindControl("txtNumero", "input");

            if (numero.value == "")
            {
                alert("Informe o número do pedido.");
                numero.focus();
                return false;
            }
            
            var idProcesso = FindControl("drpProcesso", "select").value;
            var idAplicacao = FindControl("drpAplicacao", "select").value;
            
            var idCorVidro = FindControl("drpCorVidro", "select").value;
            var espessura = FindControl("txtEspessura", "input").value;
            var idSubgrupoProd = FindControl("drpSubgrupoProd", "select").value;

            var noCache = new Date();
            var response = null;
            
            response = LstEtiquetaBoxImprimir.GetProdByPedido(numero.value, idProcesso, idAplicacao, idCorVidro, espessura, 
                idSubgrupoProd, noCache.getMilliseconds()).value;
                    
            if (response == null) {
                alert("Falha ao buscar Produtos. AJAX Error.");
                return false;
            }

            response = response.split('\t');

            if (response[0] == "Erro") {
                alert(response[1]);
                return false;
            }

            var produtos = response[1].split('|');

            produtosJaAdicionados = false;

            for (j = 0; j < produtos.length; j++)
            {
                if (produtos[j] == "")
                    continue;
                
                var items = produtos[j].split(';');
                
                setProdEtiqueta(items[1], items[0], items[2], items[3], items[4], items[5], items[6],
                    items[7], items[8], items[9]);
            }
            
            if (produtosJaAdicionados)
                alert("Alguns produtos já haviam sido adicionados.");
            
            numero.value = "";
            numero.focus();

            return false;
        }
        
        function setProdEtiqueta(idPedido, idProdPed, descrProd, codProc, codApl, qtd, qtdeImpresso, altura, largura, totM) {
            // Verifica se o produto já foi adicionado.
            var produtos = FindControl("hdfIdProdPedNf", "input").value.split(',');

            for (i = 0; i < produtos.length; i++) {
                // Verifica se o produto já foi adicionado pelo id.
                var txtQtdImp = FindControl("txtQtdImp_" + idProdPed, "input");

                if (idProdPed == produtos[i]) {
                    alert("Produto já adicionado.");

                    produtosJaAdicionados = true;
                    return false;
                }
            }
            
            // txtQtdImprimir (Qtd que o usuário planeja imprimir)
            var inputQtdImp = "<input name='txtQtdImp_" + idProdPed + "' type='text' id='txtQtdImp_" + idProdPed + "' " +
                " value='" + (qtd - qtdeImpresso) + "' style='width: 30px' onkeypress='return soNumeros(event, true, true)' onblur='verificaQtdeImprimir(" +
                idProdPed + ", " + qtd + ", " + qtdeImpresso + "); atualizaTotais();'/>";
            
            // Adiciona item à tabela
            addItem(new Array(idPedido, descrProd, altura + " x " + largura, totM, codProc, codApl, qtd, qtdeImpresso, inputQtdImp),
                new Array('Pedido', 'Produto', 'Altura x Largura', 'Tot. M²', 'Proc.', 'Apl.', 'Qtd.', 'Qtd. já impresso', 'Qtd. a imprimir'),
                'lstProd', idProdPed, "hdfIdProdPedNf", null, null, "callbackRemover", true);
            
            var linha = document.getElementById("lstProd_row" + (countItem["lstProd"] - 1));
            linha.cells[linha.cells.length - 1].width = "1px";

            atualizaTotais();
            return false;
        }

        function callbackRemover(linha)
        {
            var tabela = linha;
            while (tabela.nodeName.toLowerCase() != "table")
                tabela = tabela.parentNode;

            var numeroLinhas = 0;
            for (i = 1; i < tabela.rows.length; i++)
                if (tabela.rows[i].style.display != 'none')
                    numeroLinhas++;
            
            atualizaTotais();
        }

        function atualizaTotais() {
            var totM = 0;
            idsProdPedQtd = "";

            var tabela = document.getElementById("lstProd");
            for (t = 1; t < tabela.rows.length; t++) {
                if (tabela.rows[t].style.display == "none")
                    continue;
                    
                // Recupera o total de metro quadrado do prodPed.
                var totMProdPed = parseFloat(tabela.rows[t].cells[4].innerHTML.replace(",", "."));
                totMProdPed = !isNaN(totMProdPed) ? totMProdPed : 0;
                
                // Recupera a quantidade do prodPed.
                var qtdeProdPed = parseFloat(tabela.rows[t].cells[7].innerHTML.replace(",", "."));
                qtdeProdPed = !isNaN(qtdeProdPed) ? qtdeProdPed : 0;
                
                var idProdPed = tabela.rows[t].getAttribute("objId");
                var qtdImprimir = 0;
                
                if (idProdPed != "") {
                    // Recupera a quantidade a ser impressa.
                    qtdImprimir = document.getElementById("txtQtdImp_" + idProdPed).value;
                    totM += (totMProdPed / qtdeProdPed) * qtdImprimir;
                }
                else {
                    totM += totMProdPed;
                }
                
                idsProdPedQtd += idProdPed + ";" + qtdImprimir + "|";
            }

            FindControl("lblTotM", "span").innerHTML = totM.toFixed(2).replace(".", ",");
        }

        // Gera PDF das etiquetas
        function imprimir() {
            if (LstEtiquetaBoxImprimir.PermissaoParaImprimir().value != "true"){
                alert('Você não tem permissão para imprimir etiquetas.');
                return false;
            }
            
            if (idsProdPedQtd == null || idsProdPedQtd == "" || idsProdPedQtd == undefined) {
                alert("Selecione um pedido.");
                return false;
            }
            
            if (!confirm('Tem certeza que deseja imprimir etiquetas para os produtos selecionados?'))
                return false;
            
            // Abre tela de impressão de etiquetas
            openWindow(500, 700, '../Relatorios/RelBase.aspx?rel=EtiquetaBox&idsProdPedQtd=' + idsProdPedQtd);

            window.location.href = window.location.href;
            
            return false;
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td id="buscar2">
                                        <table class="tabela">
                                            <tr>
                                                <td id="pedido" style="color: #0066FF">
                                                    Pedido
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtNumero" runat="server" Width="60px" onkeydown="if (isEnter(event)) return getProduto();"
                                                        onkeypress="if (isEnter(event)) return false;"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                        OnClientClick="getProduto(); return false;" ToolTip="Pesquisar" />
                                                </td>
                                                <td id="processo1" style="color: #0066FF">
                                                    Processo
                                                </td>
                                                <td id="processo2">
                                                    <asp:DropDownList ID="drpProcesso" runat="server" AppendDataBoundItems="True" DataSourceID="odsProcesso"
                                                        DataTextField="CodInterno" DataValueField="IdProcesso">
                                                        <asp:ListItem Value="0">Todos</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td id="aplicacao1" style="color: #0066FF">
                                                    Aplicação
                                                </td>
                                                <td id="aplicacao2">
                                                    <asp:DropDownList ID="drpAplicacao" runat="server" AppendDataBoundItems="True" DataSourceID="odsAplicacao"
                                                        DataTextField="CodInterno" DataValueField="IdAplicacao">
                                                        <asp:ListItem Value="0">Todos</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td style="color: #0066FF">
                                        Cor
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpCorVidro" runat="server" AppendDataBoundItems="True" DataSourceID="odsCorVidro"
                                            DataTextField="Descricao" DataValueField="IdCorVidro">
                                            <asp:ListItem Value="0">Todas</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td style="color: #0066FF">
                                        Espessura
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEspessura" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                    </td>
                                    <td id="subgrupo1" style="color: #0066FF">
                                        Subgrupo
                                    </td>
                                    <td id="subgrupo2">
                                        <asp:DropDownList ID="drpSubgrupoProd" runat="server" DataSourceID="odsSubgrupo"
                                            DataTextField="Descricao" DataValueField="IdSubgrupoProd">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table id="lstProd" align="left" cellpadding="4" cellspacing="0" width="100%">
                </table>
                <table style="padding-top: 10px">
                    <tr>
                        <td style="font-weight: bold; font-size: 130%">
                            Total M²
                        </td>
                        <td style="font-size: 130%">
                            <asp:Label ID="lblTotM" runat="server" Text="0,00"></asp:Label>
                            m²
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <a href="#" id="lnkImprimir" onclick="imprimir(); return false;">
                    <img alt="" border="0" src="../Images/printer.png" />
                    Imprimir</a>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfIdProdPedNf" runat="server" />
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProcesso" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.EtiquetaProcessoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsAplicacao" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.EtiquetaAplicacaoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCorVidro" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.CorVidroDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="1" Name="idGrupo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
