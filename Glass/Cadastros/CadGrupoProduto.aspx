<%@ Page Title="Grupos de Produto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadGrupoProduto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadGrupoProduto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum grupo de produto encontrado"
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
                        <a href="#" @click.prevent="ordenar('tipo')">Tipo de grupo</a>
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
                        <a :href="obterLinkSubgrupos(item)" title="Subgrupos"  v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/subgrupo.png">
                        </a>
                        <button @click.prevent="alterarSituacaoProdutos(item, configuracoes.situacaoAtiva)" title="Ativar todos os produtos deste grupo" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Ok.gif">
                        </button>
                        <button @click.prevent="alterarSituacaoProdutos(item, configuracoes.situacaoInativa)" title="Inativar todos os produtos deste grupo" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Inativar.gif">
                        </button>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.nome }}</td>
                    <td v-if="item.tiposCalculo && item.tiposCalculo.pedido">{{ item.tiposCalculo.pedido.nome }}</td>
                    <td v-if="item.tiposCalculo && item.tiposCalculo.notaFiscal">{{ item.tiposCalculo.notaFiscal.nome }}</td>
                    <td v-if="item.estoque">{{ item.estoque.bloquearEstoque | simNao }}</td>
                    <td v-if="item.estoque">{{ item.estoque.alterarEstoque | simNao }}</td>
                    <td v-if="item.estoque">{{ item.estoque.alterarEstoqueFiscal | simNao }}</td>
                    <td v-if="item.estoque">{{ item.estoque.exibirMensagemEstoque | simNao }}</td>
                    <td v-if="item.estoque && configuracoes.usarControleOrdemDeCarga">{{ item.estoque.geraVolume | simNao }}</td>
                    <td v-if="item.tipo">{{ item.tipo.nome }}</td>
                    <td>
                        <log-alteracao tabela="GrupoProduto" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
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
                    <td>{{ grupoProduto.id }}</td>
                    <td>
                        <input type="text" v-model="grupoProduto.nome" maxlength="150" style="width: 150px" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoCalculoPedidoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipoCalculoPedido"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoCalculoNotaFiscalAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipoCalculoNotaFiscal"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="checkbox" v-model="grupoProduto.bloquearEstoque" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="grupoProduto.alterarEstoque" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="grupoProduto.alterarEstoqueFiscal" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="grupoProduto.exibirMensagemEstoque" />
                    </td>
                    <td v-if="configuracoes.usarControleOrdemDeCarga">
                        <input type="checkbox" v-model="grupoProduto.geraVolume" />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipo"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
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
                        <input type="text" v-model="grupoProduto.nome" maxlength="150" style="width: 150px" v-if="inserindo" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoCalculoPedidoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipoCalculoPedido"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoCalculoNotaFiscalAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipoCalculoNotaFiscal"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="checkbox" v-model="grupoProduto.bloquearEstoque" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="grupoProduto.alterarEstoque" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="grupoProduto.alterarEstoqueFiscal" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="grupoProduto.exibirMensagemEstoque" v-if="inserindo" />
                    </td>
                    <td v-if="configuracoes.usarControleOrdemDeCarga">
                        <input type="checkbox" v-model="grupoProduto.geraVolume" v-if="inserindo" />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipo"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    * Bloqueia venda dos produtos deste grupo se não houver em estoque
                </span>
            </div>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaGruposProduto(false)" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaGruposProduto(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/GruposProduto/Componentes/LstGruposProduto.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>