<%@ Page Title="Estoque de Produtos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstEstoque.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEstoque" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Estoques/Templates/LstEstoque.Filtro.html")
    %>
    <div id="app">
        <estoque-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></estoque-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum estoque de produto encontrado." :numero-registros="20" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th v-if="!insercaoRapidaEstoque"></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigoInternoProduto')">C�d.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricaoProduto')">Produto</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricaoGrupo')">Grupo/Subgrupo</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('estoqueMinimo')">Estoque m��nimo</a>
                    </th>
                    <th v-if="!configuracoes.naoVendeVidro && !exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('estoqueM2')">m� em Estoque</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('quantidadeReserva')">Reserva</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal && configuracoes.usarLiberacaoPedido">
                        <a href="#" @click.prevent="ordenar('quantidadeLiberacao')">Libera��o</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('quantidadeEstoque')">Qtd. em Estoque</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal">
                        <a href="#">Dispon�vel</a>
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
                    <td style="white-space: nowrap" v-if="!insercaoRapidaEstoque">
                        <a href="#" @click.prevent="editar(item, index)" title="Editar" v-if="item.permissoes.editar && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/Edit.gif">
                        </a>
                    </td>
                    <td>{{ item.codigoInternoProduto }}</td>
                    <td>{{ item.descricaoProduto }}</td>
                    <td>{{ item.descricaoGrupoProduto }} - {{ item.descricaoSubgrupoProduto }}</td>
                    <td v-if="!exibirEstoqueFiscal">{{ item.estoqueMinimo }}</td>
                    <td v-if="!configuracoes.naoVendeVidro && !exibirEstoqueFiscal">{{ item.estoqueM2 | decimal }}{{ item.descricaoTipoCalculo }}</td>
                    <td v-if="!exibirEstoqueFiscal">
                        <a href="#" @click.prevent="abrirRelatorioReserva(item)" v-if="item.permissoes.exibirLinkReserva">
                            {{ item.quantidadeReseva }}
                        </a>
                    </td>
                    <td v-if="configuracoes.usarLiberacaoPedido && !exibirEstoqueFiscal">
                        <a href="#" @click.prevent="abrirRelatorioLiberacao(item)" v-if="item.permissoes.exibirLinkLiberacao">
                            {{ item.quantidadeLiberacao }}
                        </a>
                    </td>
                    <td v-if="!exibirEstoqueFiscal">
                        <label v-if="!insercaoRapidaEstoque">{{ item.descricaoQuantidadeEstoque }}</label>
                        <span v-else>
                            <input type="number" @change.prevent="atualizarCampoUnico(item)" v-model.number="item.quantidadeEstoque" style="width: 60px" />
                            <img border="0" title="Atualizando..." src="../Images/load.gif" style="height: 16px; width:16px" v-if="item.idProduto == idProdutoEmAtualizacao" />
                            <img border="0" title="Atualizado" src="../Images/check.gif" style="height: 16px; width:16px" v-else />
                        </span>
                    </td>
                    <td v-if="!exibirEstoqueFiscal">{{ item.descricaoEstoqueDisponivel }}</td>
                    <td v-if="!exibirEstoqueFiscal">{{ item.quantidadeDefeito }}</td>
                    <td v-if="exibirEstoqueFiscal">
                        <label v-if="!insercaoRapidaEstoque">{{ item.quantidadeEstoqueFiscal }}</label>
                        <span v-else>
                            <input type="number" @change.prevent="atualizarCampoUnico(item)" v-model.number="item.quantidadeEstoqueFiscal" style="width: 60px" />
                            <img border="0" title="Atualizando..." src="../Images/load.gif" style="height: 16px; width:16px" v-if="item.idProduto == idProdutoEmAtualizacao" />
                            <img border="0" title="Atualizado" src="../Images/check.gif" style="height: 16px; width:16px" v-else />
                        </span>
                    </td>
                    <td v-if="exibirEstoqueFiscal">{{ item.quantidadePosseTerceiros }}</td>
                    <td v-if="exibirEstoqueFiscal">
                        <span v-if="item.tipoParticipante" style="font-style: italic">
                            {{ item.tipoParticipante.nome }}
                        </span>
                        <span v-if="item.participante">
                            {{ item.participante.nome }}
                        </span>
                    </td>
                    <td v-if="exibirEstoqueFiscal" style="white-space: nowrap">
                        <log-alteracao tabela="ProdutoLoja" :id-item="item.idLog" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
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
                    <td v-if="!exibirEstoqueFiscal">
                        <input type="number" v-model.number="estoqueProduto.estoqueMinimo" style="width: 60px" />
                        {{ estoqueProdutoAtual.descricaoTipoCalculo }}
                    </td>
                    <td v-if="!configuracoes.naoVendeVidro && !exibirEstoqueFiscal">
                        <input type="number" v-model.number="estoqueProduto.estoqueM2" style="width: 60px" />
                        {{ estoqueProdutoAtual.descricaoTipoCalculo }}
                    </td>
                    <td v-if="!exibirEstoqueFiscal">
                        {{ estoqueProdutoAtual.quantidadeReserva }}
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
                        <controle-selecao-participante-fiscal :participante.sync="participanteAtual"
                            :tipo-participante.sync="estoqueProduto.tipoParticipante"></controle-selecao-participante-fiscal>
                    </td>
                    <td v-if="exibirEstoqueFiscal">
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div v-if="configuracoes.alterarEstoqueManualmente">
                <span>
                    <a href="#" @click.prevent="ativarDesativarInsercaoRapidaEstoque()" title="Inser��o r�pida">
                        <img src="../Images/addMany.gif"> Inser��o r�pida de estoque
                    </a>
                </span>
            </div>
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
            <asp:ScriptReference Path="~/Vue/Estoques/Componentes/LstEstoque.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Estoques/Componentes/LstEstoque.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
