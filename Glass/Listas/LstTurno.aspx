<%@ Page Title="Turnos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstTurno.aspx.cs" Inherits="Glass.UI.Web.Listas.LstTurno" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum turno encontrado."
                :numero-registros="15" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('sequencia')">Turno</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('inicio')">Início</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('termino')">Término</a>
                    </th>
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
                    <td v-if="item.sequencia">{{ item.sequencia.nome }}</td>
                    <td>{{ item.inicio }}</td>
                    <td>{{ item.termino }}</td>
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
                        <input type="text" v-model="turno.nome" maxlength="150" style="width: 150px" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="sequenciaAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensSequencia"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="text" v-model="turno.inicio" maxlength="5" style="width: 50px" required />
                    </td>
                    <td>
                        <input type="text" v-model="turno.termino" maxlength="5" style="width: 50px" required />
                    </td>
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
                    <td>
                        <input type="text" v-model="turno.nome" maxlength="150" style="width: 150px" v-if="inserindo" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="sequenciaAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensSequencia"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="text" v-model="turno.inicio" maxlength="5" style="width: 50px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="text" v-model="turno.termino" maxlength="5" style="width: 50px" v-if="inserindo" required />
                    </td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Producao/Turnos/Componentes/LstTurnos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
