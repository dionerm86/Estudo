<%@ Page Title="Planos de Conta" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadPlanoConta.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadPlanoConta" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/PlanosConta/Templates/LstPlanosConta.Filtro.html")
    %>
    <div id="app">
        <planos-conta-filtros :filtro.sync="filtro"></planos-conta-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum plano de conta encontrado."
                :numero-registros="15" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigo')">Num. Conta</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('grupoConta')">Grupo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('exibirDre')">Exibir no DRE</a>
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
                    <td>{{ item.codigo }}</td>
                    <td v-if="item.grupoConta">{{ item.grupoConta.nome }}</td>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.exibirDre | indicaMarcado }}</td>
                    <td v-if="item.situacao">{{ item.situacao.nome }}</td>
                    <td>
                        <log-alteracao tabela="PlanoContas" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes && item.permissoes.logAlteracoes"></log-alteracao>
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
                    <td>{{ planoConta.codigo }}</td>
                    <td>
                        <template v-if="planoConta.permissoes && !planoConta.permissoes.editarApenasExibirDre">
                            <lista-selecao-id-valor v-bind:item-selecionado.sync="grupoContaAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensGrupoConta"
                                v-bind:ordenar="false" required></lista-selecao-id-valor>
                        </template>
                        <template v-else>
                            {{ planoConta.nomeGrupoConta }}
                        </template>
                    </td>
                    <td>
                        <template v-if="planoConta.permissoes && !planoConta.permissoes.editarApenasExibirDre">
                            <input type="text" v-model="planoConta.nome" maxlength="60" style="width: 250px" required />
                        </template>
                        <template v-else>
                            {{ planoConta.nome }}
                        </template>
                    </td>
                    <td>
                        <input type="checkbox" v-model="planoConta.exibirDre" />
                    </td>
                    <td>
                        <template v-if="planoConta.permissoes && !planoConta.permissoes.editarApenasExibirDre">
                            <lista-selecao-id-valor v-bind:item-selecionado.sync="situacaoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensSituacao"
                                v-bind:ordenar="false" required></lista-selecao-id-valor>
                        </template>
                        <template v-else>
                            {{ planoConta.descricaoSituacao }}
                        </template>
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Novo plano de conta..." v-if="!inserindo">
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
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="grupoContaAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensGrupoConta"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="text" v-model="planoConta.nome" maxlength="60" style="width: 250px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="checkbox" v-model="planoConta.exibirDre" v-if="inserindo" />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="situacaoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensSituacao"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaPlanosConta(false)" title="Imprimir">
                        <img alt="" border="0" src="../../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaPlanosConta(true)" title="Exportar para o Excel">
                        <img border="0" src="../../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/PlanosConta/Componentes/LstPlanosConta.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/PlanosConta/Componentes/LstPlanosConta.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
