<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstProduto.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstProduto" Title="Produtos" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Produtos/Templates/LstProdutos.Filtro.html")
    %>
    <div id="app">
        <produtos-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></produtos-filtros>
        <section>
            <a :href="obterLinkInserirProduto()" v-if="configuracoes.cadastrarProduto">
                Inserir produto
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum produto encontrado.">
                <template slot="cabecalho">
                    <th v-if="configuracoes.cadastrarProduto"></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigo')">C�d.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricao')">Descri��o</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricaoGrupo')">Grupo/Subgrupo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('altura')">Altura</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('largura')">Largura</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('custofornecedor')">Custo forn.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('custocomimpostos')">Custo imp.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valoratacado')">Atacado</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorbalcao')">Balc�o</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorobra')">Obra</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorreposicao')">Reposi��o</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('quantidadereserva')">Reserva</a>
                    </th>
                    <th v-if="configuracoes.usarLiberacaoPedido">
                        <a href="#" @click.prevent="ordenar('quantidadeliberacao')">Libera��o</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('estoque')">Estoque</a>
                    </th>
                    <th>
                        <a href="#">Dispon�vel</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap" v-if="configuracoes && configuracoes.cadastrarProduto">
                        <a :href="obterLinkEditarProduto(item)" title="Editar">
                            <img border="0" src="../Images/EditarGrid.gif">
                        </a>
                        <a href="#" @click.prevent="excluir(item)" title="Excluir">
                            <img border="0" src="../Images/ExcluirGrid.gif">
                        </a>
                        <a href="#" @click.prevent="abrirPrecoFornecedor(item)" title="Pre�o por fornecedor">
                            <img border="0" src="../Images/dinheiro.gif">
                        </a>
                        <a href="#" @click.prevent="abrirDescontoPorQuantidade(item)" title="Desconto por Quantidade">
                            <img border="0" src="../Images/money_delete.gif">
                        </a>
                    </td>
                    <td>{{ item.codigo }}</td>
                    <td>{{ item.nome }} {{ item.descricaoBeneficiamentos }}</td>
                    <td>
                        {{ item.descricaoGrupo }}
                        <template v-if="item.descricaoSubgrupo">
                            -
                            {{ item.descricaoSubgrupo }}
                        </template>
                    </td>
                    <td>{{ item.altura }}</td>
                    <td>{{ item.largura }}</td>
                    <td>{{ item.custos.fornecedor | moeda }}</td>
                    <td>{{ item.custos.comImpostos | moeda }}</td>
                    <td>{{ item.valoresVenda.atacado | moeda }}</td>
                    <td>{{ item.valoresVenda.balcao | moeda }}</td>
                    <td>{{ item.valoresVenda.obra | moeda }}</td>
                    <td>{{ item.valoresVenda.reposicao | moeda }}</td>
                    <td>{{ item.estoque.reserva }}</td>
                    <td v-if="configuracoes && configuracoes.usarLiberacaoPedido">{{ item.estoque.liberacao }}</td>
                    <td>{{ item.estoque.real }}{{ item.estoque.unidade }}</td>
                    <td>{{ item.estoque.disponivel }}{{ item.estoque.unidade }}</td>
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="abrirPrecoAnterior(item)" title="Pre�o anterior" v-if="configuracoes.exibirPrecoAnterior">
                            <img border="0" src="../Images/money_hist.gif">
                        </a>
                        <a href="#" @click.prevent="abrirImpressaoProduto(item)" title="Imprimir" v-if="configuracoes.cadastrarProduto">
                            <img border="0" src="../Images/printer.png">
                        </a>
                        <controle-exibicao-imagem :id-item="item.id" tipo-item="Produto"></controle-exibicao-imagem>
                        <log-alteracao tabela="Produto" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div v-if="configuracoes.cadastrarProduto">
                <span>
                    <a href="#" @click.prevent="abrirListaProdutos(false, false)" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaProdutos(false, true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
            <div v-if="configuracoes.cadastrarProduto">
                <span>
                    <a href="#" @click.prevent="abrirListaProdutos(true, false)" title="Imprimir ficha">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir ficha
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaProdutos(true, true)" title="Exportar ficha para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar ficha para o Excel
                    </a>
                </span>
            </div>
            <div>
                <span>
                    <a href="#" @click.prevent="abrirImportacaoExportacaoPrecos()">
                        Exportar/importar pre�os de produtos
                    </a>
                </span>
            </div>
        </section>
    </div>
    <script type="text/javascript">
        function exportarPrecos() {
            app.abrirExportacaoPrecosProdutos();
        }
    </script>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Produtos/Componentes/LstProdutos.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Produtos/Componentes/LstProdutos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
