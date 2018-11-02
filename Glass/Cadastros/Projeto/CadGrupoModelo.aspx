<%@ Page Title="Grupos de Projeto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadGrupoModelo.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.CadGrupoModelo" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum grupo de projeto encontrado."
                :numero-registros="15" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('boxPadrao')">Box padrão</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('esquadria')">Esquadria</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="editar(item, index)" title="Editar" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../../Images/Edit.gif">
                        </button>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.boxPadrao | indicaMarcado }}</td>
                    <td>{{ item.esquadria | indicaMarcado }}</td>
                    <td v-if="item.situacao">{{ item.situacao.nome }}</td>
                    <td>
                        <log-alteracao tabela="GrupoModelo" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
                <template slot="itemEditando">
                    <td style="white-space: nowrap">
                        <button @click.prevent="atualizar" title="Atualizar">
                            <img src="../../Images/ok.gif">
                        </button>
                        <button @click.prevent="cancelar" title="Cancelar">
                            <img src="../../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>
                        <input type="text" v-model="grupoProjeto.nome" maxlength="60" style="width: 300px" required />
                    </td>
                    <td>
                        <input type="checkbox" v-model="grupoProjeto.boxPadrao" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="grupoProjeto.esquadria" />
                    </td>
                    <td>
                        <lista-selecao-situacoes v-bind:situacao.sync="situacaoAtual" required></lista-selecao-situacoes>
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Novo turno..." v-if="!inserindo">
                            <img src="../../Images/Insert.gif">
                        </button>
                        <button v-on:click.prevent="inserir" title="Inserir" v-if="inserindo">
                            <img src="../../Images/Ok.gif">
                        </button>
                        <button v-on:click.prevent="cancelar" title="Cancelar" v-if="inserindo">
                            <img src="../../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>
                        <input type="text" v-model="grupoProjeto.nome" maxlength="60" style="width: 300px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="checkbox" v-model="grupoProjeto.boxPadrao" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="grupoProjeto.esquadria" v-if="inserindo" />
                    </td>
                    <td>
                        <lista-selecao-situacoes v-bind:situacao.sync="situacaoAtual" v-if="inserindo" required></lista-selecao-situacoes>
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Projetos/GruposProjeto/Componentes/LstGruposProjeto.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>

