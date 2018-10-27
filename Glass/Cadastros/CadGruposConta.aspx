<%@ Page Title="Grupos do Plano de Contas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="CadGruposConta.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadGruposConta" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum grupo de conta encontrado."
                :numero-registros="15" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Num. Grupo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('categoriaConta')">Categoria</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('exibirPontoEquilibrio')">Exibir no ponto de equilíbrio</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="editar(item, index)" title="Editar" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Edit.gif">
                        </button>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="!inserindo && numeroLinhaEdicao === -1 && item.permissoes && item.permissoes.excluir">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ item.id }}</td>
                    <td v-if="item.categoriaConta">{{ item.categoriaConta.nome }}</td>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.exibirPontoEquilibrio | indicaMarcado }}</td>
                    <td v-if="item.situacao">{{ item.situacao.nome }}</td>
                    <td>
                        <button @click.prevent="alterarPosicao(item, true)" v-if="!inserindo && numeroLinhaEdicao === -1 && item.id > 1">
                            <img src="../Images/up.gif">
                        </button>
                        <button @click.prevent="alterarPosicao(item, false)" v-if="!inserindo && numeroLinhaEdicao === -1 && item.id > 1">
                            <img src="../Images/down.gif">
                        </button>
                        <log-alteracao tabela="GrupoConta" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes && item.permissoes.logAlteracoes"></log-alteracao>
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
                    <td>{{ grupoConta.id }}</td>
                    <td>
                        <template v-if="grupoConta.permissoes && !grupoConta.permissoes.editarApenasExibirPontoEquilibrio">
                            <lista-selecao-id-valor v-bind:item-selecionado.sync="categoriaContaAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCategoriaConta"
                                v-bind:ordenar="false" required></lista-selecao-id-valor>
                        </template>
                        <template v-else>
                            {{ grupoConta.nomeCategoriaConta }}
                        </template>
                    </td>
                    <td>
                        <template v-if="grupoConta.permissoes && !grupoConta.permissoes.editarApenasExibirPontoEquilibrio">
                            <input type="text" v-model="grupoConta.nome" maxlength="60" style="width: 250px" required />
                        </template>
                        <template v-else>
                            {{ grupoConta.nome }}
                        </template>
                    </td>
                    <td>
                        <input type="checkbox" v-model="grupoConta.exibirPontoEquilibrio" />
                    </td>
                    <td>
                        <template v-if="grupoConta.permissoes && !grupoConta.permissoes.editarApenasExibirPontoEquilibrio">
                            <lista-selecao-situacoes v-bind:situacao.sync="situacaoAtual" exibir-todas="false" required></lista-selecao-situacoes>
                        </template>
                        <template v-else>
                            {{ grupoConta.descricaoSituacao }}
                        </template>
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Novo grupo de conta..." v-if="!inserindo">
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
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="categoriaContaAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensCategoriaConta"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="text" v-model="grupoConta.nome" maxlength="60" style="width: 250px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="checkbox" v-model="grupoConta.exibirPontoEquilibrio" v-if="inserindo" />
                    </td>
                    <td>
                        <lista-selecao-situacoes v-bind:situacao.sync="situacaoAtual" exibir-todas="false" v-if="inserindo" required></lista-selecao-situacoes>
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/PlanosConta/GruposConta/Componentes/LstGruposConta.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
