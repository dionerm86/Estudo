<%@ Page Title="Tipos de Perda" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadTipoPerda.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadTipoPerda" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum tipo de perda encontrado."
                :numero-registros="15" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('setor')">Setor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('exibirNoPainelDeProducao')">Exibir no painel de produção</a>
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
                        <a :href="obterLinkSubtiposPerda(item)" title="Subtipos de perda" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/subgrupo.png">
                        </a>
                    </td>
                    <td>{{ item.nome }}</td>
                    <td v-if="item.setor">{{ item.setor.nome }}</td>
                    <td v-if="item.situacao">{{ item.situacao.nome }}</td>
                    <td>{{ item.exibirNoPainelDeProducao | indicaMarcado }}</td>
                    <td>
                        <log-alteracao tabela="TipoPerda" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
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
                        <input type="text" v-model="tipoPerda.nome" maxlength="150" style="width: 150px" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="setorAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensSetor"
                            v-bind:ordenar="false"></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="situacaoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensSituacao"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="checkbox" v-model="tipoPerda.exibirNoPainelDeProducao" />
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Novo tipo de perda..." v-if="!inserindo">
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
                        <input type="text" v-model="tipoPerda.nome" maxlength="150" style="width: 150px" v-if="inserindo" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="setorAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensSetor"
                            v-bind:ordenar="false" v-if="inserindo"></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="situacaoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensSituacao"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="checkbox" v-model="tipoPerda.exibirNoPainelDeProducao" v-if="inserindo" />
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Producao/TiposPerda/Componentes/LstTiposPerda.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>