<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstProduto.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstProduto" Title="Produtos" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
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
                        <a href="#" @click.prevent="ordenar('codigo')">Cód.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricao')">Descrição</a>
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
                        <a href="#" @click.prevent="ordenar('valorbalcao')">Balcão</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorobra')">Obra</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorreposicao')">Reposição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('quantidadereserva')">Reserva</a>
                    </th>
                    <th v-if="configuracoes.usarLiberacaoPedido">
                        <a href="#" @click.prevent="ordenar('quantidadeliberacao')">Liberação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('estoque')">Estoque</a>
                    </th>
                    <th>
                        <a href="#">Disponível</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap" v-if="configuracoes.cadastrarProduto">
                        <a :href="obterLinkEditarProduto(item)" title="Editar">
                            <img border="0" src="../Images/EditarGrid.gif">
                        </a>
                        <a href="#" @click.prevent="excluir(item)" title="Excluir">
                            <img border="0" src="../Images/ExcluirGrid.gif">
                        </a>
                        <a href="#" @click.prevent="abrirPrecoFornecedor(item)" title="Preço por fornecedor">
                            <img border="0" src="../Images/dinheiro.gif">
                        </a>
                        <a href="#" @click.prevent="abrirDescontoPorQuantidade(item)" title="Desconto por Quantidade">
                            <img border="0" src="../Images/money_delete.gif">
                        </a>
                    </td>
                    <td>{{ item.codigo }}</td>
                    <td>{{ item.descricao }} {{ item.descricaoBeneficiamentos }}</td>
                    <td>{{ item.descricaoGrupo }} - {{ item.descricaoSubgrupo }}</td>
                    <td>{{ item.altura }}</td>
                    <td>{{ item.largura }}</td>
                    <td>{{ item.custoFornecedor }}</td>
                    <td>{{ item.custoComImpostos }}</td>
                    <td>{{ item.valorAtacado }}</td>
                    <td>{{ item.valorBalcao }}</td>
                    <td>{{ item.valorObra }}</td>
                    <td>{{ item.valorReposicao }}</td>
                    <td>{{ item.quantidadeReserva }}</td>
                    <td v-if="configuracoes.usarLiberacaoPedido">{{ item.quantidadeLiberacao }}</td>
                    <td>{{ item.estoque }}</td>
                    <td>{{ item.estoqueDisponivel }}</td>
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="abrirPrecoAnterior(item)" title="Preço anterior" v-if="configuracoes.exibirPrecoAnterior">
                            <img border="0" src="../Images/money_hist.gif">
                        </a>
                        <a href="#" @click.prevent="abrirImpressaoProduto(item)" title="Imprimir" v-if="configuracoes.cadastrarProduto">
                            <img border="0" src="../Images/printer.png">
                        </a>
                        <!--ctrlImagemPopup ID="ctrlImagemPopup1" ImageUrl='Glass.Global.UI.Web.Process.ProdutoRepositorioImagens.Instance.ObtemUrl((int)Eval("IdProd"))' /-->
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
                        Exportar/importar preços de produtos
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Produtos/Componentes/LstProdutos.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Produtos/Componentes/LstProdutos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
