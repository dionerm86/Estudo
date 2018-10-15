<%@ Page Title="Subgrupos de Produto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadSubgrupoProduto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadSubgrupoProduto" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum subgrupo de produto encontrado"
                :numero-registros="15" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Código</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipoCalculoPedido')">Cálculo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipoCalculoNotaFiscal')">Cálculo NF</a>
                    </th>
                    <th v-if="grupoVidro">
                        <a href="#" @click.prevent="ordenar('produtoParaEstoque')">Produtos para estoque</a>
                    </th>
                    <th v-if="grupoVidro">
                        <a href="#" @click.prevent="ordenar('vidroTemperado')">Vidro temperado</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('bloquearEstoque')">Bloquear estoque *</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('alterarEstoque')">Alterar estoque</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('alterarEstoqueFiscal')">Alterar estoque fiscal</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('exibirMensagemEstoque')">Exibir mensagem estoque</a>
                    </th>
                    <th v-if="configuracoes.usarControleOrdemDeCarga">
                        <a href="#" @click.prevent="ordenar('geraVolume')">Gera volume</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('liberarPendenteProducao')">Lib. pendende produção?</a>
                    </th>
                    <th v-if="!grupoVidro && configuracoes.bloquearItensVendaRevendaPedido">
                        <a href="#" @click.prevent="ordenar('permitirItemRevendaNaVenda')">Permitir item revenda na venda?</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('bloquearVendaECommerce')">Bloquear venda no e-commerce?</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('minimoDiasEntrega')">Núm. dias mín. entrega</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('diaSemanaEntrega')">Dia semana entrega</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipo')">Tipo de subgrupo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th>
                        Lojas associadas
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="editar(item, index)" title="Editar" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Edit.gif">
                        </button>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="!inserindo && numeroLinhaEdicao === -1 && item.permissoes.excluir">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                        <button @click.prevent="alterarSituacaoProdutos(item, configuracoes.situacaoAtiva)" title="Ativar todos os produtos deste subgrupo" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Ok.gif">
                        </button>
                        <button @click.prevent="alterarSituacaoProdutos(item, configuracoes.situacaoInativa)" title="Inativar todos os produtos deste subgrupo" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Inativar.gif">
                        </button>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.nome }}</td>
                    <td v-if="item.tiposCalculo && item.tiposCalculo.pedido">{{ item.tiposCalculo.pedido.nome }}</td>
                    <td v-if="item.tiposCalculo && item.tiposCalculo.notaFiscal">{{ item.tiposCalculo.notaFiscal.nome }}</td>
                    <td v-if="item.estoque && grupoVidro">{{ item.estoque.produtoParaEstoque | simNao }}</td>
                    <td v-if="grupoVidro">{{ item.vidroTemperado | simNao }}</td>
                    <td v-if="item.estoque">{{ item.estoque.bloquearEstoque | simNao }}</td>
                    <td v-if="item.estoque">{{ item.estoque.alterarEstoque | simNao }}</td>
                    <td v-if="item.estoque">{{ item.estoque.alterarEstoqueFiscal | simNao }}</td>
                    <td v-if="item.estoque">{{ item.estoque.exibirMensagemEstoque | simNao }}</td>
                    <td v-if="item.estoque && configuracoes.usarControleOrdemDeCarga">{{ item.estoque.geraVolume | simNao }}</td>
                    <td>{{ item.liberarPendenteProducao | simNao }}</td>
                    <td v-if="!grupoVidro && configuracoes.bloquearItensVendaRevendaPedido">{{ item.permitirItemRevendaNaVenda | simNao }}</td>
                    <td v-if="item.estoque">{{ item.estoque.bloquearVendaECommerce | simNao }}</td>
                    <td v-if="item.entrega">{{ item.entrega.diasMinimo }}</td>
                    <td v-if="item.entrega && item.entrega.diaSemana">{{ item.entrega.diaSemana.nome }}</td>
                    <td v-if="item.tipo">{{ item.tipo.nome }}</td>
                    <td v-if="item.cliente">{{ item.cliente.nome }}</td>
                    <td v-if="item.lojasAssociadas">{{ obterNomesLojasAssociadas(item) }}</td>
                    <td>
                        <log-alteracao tabela="SubgrupoProduto" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
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
                    <td>{{ subgrupoProduto.id }}</td>
                    <td>
                        <input type="text" v-model="subgrupoProduto.nome" maxlength="150" style="width: 150px" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoCalculoPedidoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipoCalculoPedido"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoCalculoNotaFiscalAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipoCalculoNotaFiscal"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td v-if="grupoVidro">
                        <input type="checkbox" v-model="subgrupoProduto.produtoParaEstoque" />
                    </td>
                    <td v-if="grupoVidro">
                        <input type="checkbox" v-model="subgrupoProduto.vidroTemperado" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="subgrupoProduto.bloquearEstoque" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="subgrupoProduto.alterarEstoque" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="subgrupoProduto.alterarEstoqueFiscal" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="subgrupoProduto.exibirMensagemEstoque" />
                    </td>
                    <td v-if="configuracoes.usarControleOrdemDeCarga">
                        <input type="checkbox" v-model="subgrupoProduto.geraVolume" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="subgrupoProduto.liberarPendenteProducao" />
                    </td>
                    <td v-if="!grupoVidro && configuracoes.bloquearItensVendaRevendaPedido">
                        <input type="checkbox" v-model="subgrupoProduto.permitirItemRevendaNaVenda" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="subgrupoProduto.bloquearVendaECommerce" />
                    </td>
                    <td>
                        <input type="number" v-model.number="subgrupoProduto.diasMinimoEntrega" maxlength="2" style="width: 50px" />
                    </td>
                    <td>
                        <select v-model="subgrupoProduto.diaSemanaEntrega">
                            <option></option>
                            <option Value="1">Domingo</option>
                            <option Value="2">Segunda-feira</option>
                            <option Value="3">Terça-feira</option>
                            <option Value="4">Quarta-feira</option>
                            <option Value="5">Quinta-feira</option>
                            <option Value="6">Sexta-feira</option>
                            <option Value="7">Sábado</option>
                        </select>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipo"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <campo-busca-cliente :cliente.sync="clienteAtual" :exibir-informacoes-compra="false"></campo-busca-cliente>
                    </td>
                    <td>
                        <lista-selecao-multipla v-bind:ids-selecionados.sync="subgrupoProduto.idsLojasAssociadas" texto-selecionar="Selecione as lojas" v-bind:funcao-recuperar-itens="obterItensLojas"
                            v-bind:ordenar="false" campo-id="id" campo-descricao="nome"></lista-selecao-multipla>
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Novo grupo de produto..." v-if="!inserindo">
                            <img src="../Images/Insert.gif">
                        </button>
                        <button v-on:click.prevent="inserir" title="Inserir" v-if="inserindo">
                            <img src="../Images/Ok.gif">
                        </button>
                        <button v-on:click.prevent="cancelar" title="Cancelar" v-if="inserindo">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td></td>
                    <td>
                        <input type="text" v-model="subgrupoProduto.nome" maxlength="150" style="width: 150px" v-if="inserindo" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoCalculoPedidoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipoCalculoPedido"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoCalculoNotaFiscalAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipoCalculoNotaFiscal"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td v-if="grupoVidro">
                        <input type="checkbox" v-model="subgrupoProduto.produtoParaEstoque" v-if="inserindo" />
                    </td>
                    <td v-if="grupoVidro">
                        <input type="checkbox" v-model="subgrupoProduto.vidroTemperado" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="subgrupoProduto.bloquearEstoque" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="subgrupoProduto.alterarEstoque" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="subgrupoProduto.alterarEstoqueFiscal" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="subgrupoProduto.exibirMensagemEstoque" v-if="inserindo" />
                    </td>
                    <td v-if="configuracoes.usarControleOrdemDeCarga">
                        <input type="checkbox" v-model="subgrupoProduto.geraVolume" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="subgrupoProduto.liberarPendenteProducao" v-if="inserindo" />
                    </td>
                    <td v-if="!grupoVidro && configuracoes.bloquearItensVendaRevendaPedido">
                        <input type="checkbox" v-model="subgrupoProduto.permitirItemRevendaNaVenda" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="subgrupoProduto.bloquearVendaECommerce" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="number" v-model.number="subgrupoProduto.diasMinimoEntrega" maxlength="2" style="width: 50px" v-if="inserindo" />
                    </td>
                    <td>
                        <select v-model="subgrupoProduto.diaSemanaEntrega" v-if="inserindo">
                            <option></option>
                            <option Value="1">Domingo</option>
                            <option Value="2">Segunda-feira</option>
                            <option Value="3">Terça-feira</option>
                            <option Value="4">Quarta-feira</option>
                            <option Value="5">Quinta-feira</option>
                            <option Value="6">Sexta-feira</option>
                            <option Value="7">Sábado</option>
                        </select>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipo"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <campo-busca-cliente :cliente.sync="clienteAtual" :exibir-informacoes-compra="false" v-if="inserindo"></campo-busca-cliente>
                    </td>
                    <td>
                        <lista-selecao-multipla v-bind:ids-selecionados.sync="subgrupoProduto.idsLojasAssociadas" texto-selecionar="Selecione as lojas" v-bind:funcao-recuperar-itens="obterItensLojas"
                            v-bind:ordenar="false" campo-id="id" campo-descricao="nome" v-if="inserindo"></lista-selecao-multipla>
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    * Bloqueia venda dos produtos deste subgrupo se não houver em estoque
                </span>
            </div>
            <div v-if="grupoVidro">
                <span style="color: red">
                    Ao alterar o cálculo dos subgrupos alguns dados poderão ser perdidos.
                </span>
            </div>
            <div>
                <span style="color: red">
                    Os valores entre parênteses do {{ configuracoes.descricaoMlal }} são valores de arredondamento para cálculo do valor
                </span>
            </div>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Produtos/SubgruposProduto/Componentes/LstSubgruposProduto.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
