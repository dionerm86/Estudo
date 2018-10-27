<%@ Page Title="Categorias do Plano de Contas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="CadCategoriaConta.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCategoriaConta" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma categoria de conta encontrada."
                :numero-registros="15" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        Descrição
                    </th>
                    <th>
                        Tipo
                    </th>
                    <th>
                        Situação
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="editar(item, index)" title="Editar" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Edit.gif">
                        </button>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ item.nome }}</td>
                    <td v-if="item.tipo">{{ item.tipo.nome }}</td>
                    <td v-if="item.situacao">{{ item.situacao.nome }}</td>
                    <td>
                        <button @click.prevent="alterarPosicao(item, true)" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/up.gif">
                        </button>
                        <button @click.prevent="alterarPosicao(item, false)" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/down.gif">
                        </button>
                        <log-alteracao tabela="CategoriaConta" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes && item.permissoes.logAlteracoes"></log-alteracao>
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
                    <td>
                        <input type="text" v-model="categoriaConta.nome" maxlength="60" style="width: 250px" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipo"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-situacoes v-bind:situacao.sync="situacaoAtual" exibir-todas="false" required></lista-selecao-situacoes>
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Nova categoria de conta..." v-if="!inserindo">
                            <img src="../Images/Insert.gif">
                        </button>
                        <button v-on:click.prevent="inserir" title="Inserir" v-if="inserindo">
                            <img src="../Images/Ok.gif">
                        </button>
                        <button v-on:click.prevent="cancelar" title="Cancelar" v-if="inserindo">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>
                        <input type="text" v-model="categoriaConta.nome" maxlength="60" style="width: 250px" v-if="inserindo" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipo"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
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
            <asp:ScriptReference Path="~/Vue/PlanosConta/GruposConta/CategoriasConta/Componentes/LstCategoriasConta.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>