<%@ Page Title="Estoque de Produtos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="LstEstoque.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEstoque" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Estoques/Templates/LstEstoques.Filtro.html")
    %>
    <div id="app">
        <estoque-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></estoque-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum estoque de produto encontrado." :numero-registros="20">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigoInternoProduto')">Cód.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricaoProduto')">Produto</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricaoGrupoProduto')">Grupo/Subgrupo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('estoqueMinimo')">Estoque mínimo</a>
                    </th>
                    <th v-if="!configuracoes.naoVendeVidro && !exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('estoqueM2')">m² em Estoque</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('quantidadeReserva')">Reserva</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal && configuracoes.usarLiberacaoPedido">
                        <a href="#" @click.prevent="ordenar('quantidadeLiberacao')">Liberação</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('quantidadeEstoque')">Qtd. em Estoque</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal">
                        <a href="#">Disponível</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('quantidadeDefeito')">Qtd. com Defeito</a>
                    </th>
                    <th v-if="exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('quantidadeEstoqueFiscal')">Qtd. em Estoque Fiscal</a>
                    </th>
                    <th v-if="exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('quantidadePosseTerceiros')">Qtde. Posse Terceiros</a>
                    </th>
                    <th v-if="exibirEstoqueFiscal">
                        <a href="#">Item em posse de</a>
                    </th>
                    <th v-if="exibirEstoqueFiscal"></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="editar(item, index)" title="Editar" v-if="item.permissoes.editar && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/Edit.gif">
                        </a>
                    </td>
                    <td>{{ item.codigoInternoProduto }}</td>
                    <td>{{ item.descricaoProduto }}</td>
                    <td>{{ item.descricaoGrupoProduto }} - {{ item.descricaoSubgrupoProduto }}</td>
                    <td>{{ item.estoqueMinimo }}</td>
                    <td v-if="!configuracoes.naoVendeVidro && !exibirEstoqueFiscal">{{ item.estoqueM2 | decimal }}{{ item.descricaoTipoCalculo }}</td>
                    <td v-if="!exibirEstoqueFiscal">
                        <a href="#" @click.prevent="abrirRelatorioReserva(item)" v-if="item.permissoes.exibirLinkReserva">
                            {{ item.quantidadeReserva }}
                        </a>
                    </td>
                    <td v-if="configuracoes.usarLiberacaoPedido && !exibirEstoqueFiscal">
                        <a href="#" @click.prevent="abrirRelatorioLiberacao(item)" v-if="item.permissoes.exibirLinkLiberacao">
                            {{ item.quantidadeLiberacao }}
                        </a>
                    </td>
                    <td v-if="!exibirEstoqueFiscal">{{ item.descricaoQuantidadeEstoque }}</td>
                    <td v-if="!exibirEstoqueFiscal">{{ item.descricaoEstoqueDisponivel }}</td>
                    <td v-if="!exibirEstoqueFiscal">{{ item.quantidadeDefeito }}</td>
                    <td v-if="exibirEstoqueFiscal">{{ item.quantidadeEstoqueFiscal }}</td>
                    <td v-if="exibirEstoqueFiscal">{{ item.quantidadePosseTerceiros }}</td>
                    <td v-if="exibirEstoqueFiscal">{{ item.descricaoTipoTerceiro }} {{ item.nomeTerceiro }}</td>
                    <td v-if="exibirEstoqueFiscal" style="white-space: nowrap">
                        <log-alteracao tabela="ProdutoLoja" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
                <template slot="itemEditando">
                    <td style="white-space: nowrap">
                        <button @click.prevent="atualizar" title="Atualizar">
                            <img src="../Images/ok.gif">
                        </button>
                        <button @click.prevent="cancelar" title="Cancelar">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ estoqueProdutoAtual.codigoInternoProduto }}</td>
                    <td>{{ estoqueProdutoAtual.descricaoProduto }}</td>
                    <td>{{ estoqueProdutoAtual.descricaoGrupoProduto }} - {{ estoqueProdutoAtual.descricaoSubgrupoProduto }}</td>
                    <td>
                        <input type="number" v-model.number="estoqueProduto.estoqueMinimo" style="width: 60px" />
                    </td>
                    <td v-if="!configuracoes.naoVendeVidro && !exibirEstoqueFiscal">
                        <input type="number" v-model.number="estoqueProduto.estoqueM2" style="width: 60px" />
                        {{ estoqueProdutoAtual.descricaoTipoCalculo }}
                    </td>
                    <td v-if="!exibirEstoqueFiscal">
                        {{ estoqueProdutoAtual.quantidadeReseva }}
                    </td>
                    <td v-if="configuracoes.usarLiberacaoPedido && !exibirEstoqueFiscal">
                        {{ estoqueProdutoAtual.quantidadeLiberacao }}
                    </td>
                    <td v-if="!exibirEstoqueFiscal">
                        <input type="number" v-model.number="estoqueProduto.quantidadeEstoque" style="width: 60px" />
                        {{ estoqueProdutoAtual.descricaoTipoCalculo }}
                    </td>
                    <td v-if="!exibirEstoqueFiscal">{{ estoqueProdutoAtual.descricaoEstoqueDisponivel }}</td>
                    <td v-if="!exibirEstoqueFiscal">
                        <input type="number" v-model.number="estoqueProduto.quantidadeDefeito" style="width: 60px" />
                        {{ estoqueProdutoAtual.descricaoTipoCalculo }}
                    </td>
                    <td v-if="exibirEstoqueFiscal">
                        <input type="number" v-model.number="estoqueProduto.quantidadeEstoqueFiscal" style="width: 60px" />
                    </td>
                    <td v-if="exibirEstoqueFiscal">
                        <input type="number" v-model.number="estoqueProduto.quantidadePosseTerceiros" style="width: 60px" />
                        {{ estoqueProdutoAtual.descricaoTipoCalculo }}
                    </td>
                    <td v-if="exibirEstoqueFiscal">

                    </td>
                    <td>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaEstoquesProduto(false)">
                        <img border="0" src="../Images/printer.png"> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaEstoquesProduto(true)">
                        <img border="0" src="../Images/Excel.gif"> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Estoques/Componentes/LstEstoques.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Estoques/Componentes/LstEstoques.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
