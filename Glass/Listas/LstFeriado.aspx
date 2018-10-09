<%@ Page Title="Lista de Feriados" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstFeriado.aspx.cs" Inherits="Glass.UI.Web.Listas.LstFeriado" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum feriado encontrado."
                :numero-registros="15" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricao')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dia')">Dia</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('mes')">Mês</a>
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
                    <td>{{ item.descricao }}</td>
                    <td>{{ item.dia }}</td>
                    <td>{{ item.mes }}</td>
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
                        <input type="text" v-model="feriado.descricao" maxlength="30" style="width: 150px" required />
                    </td>
                    <td>
                        <input type="number" v-model.number="feriado.dia" style="width: 50px" v-bind:min="1" v-bind:max="31" required />
                    </td>
                    <td>
                        <input type="number" v-model.number="feriado.mes" style="width: 50px" v-bind:min="1" v-bind:max="12" required />
                    </td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Novo feriado..." v-if="!inserindo">
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
                        <input type="text" v-model="feriado.descricao" maxlength="30" style="width: 150px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="number" v-model.number="feriado.dia" style="width: 50px" v-bind:min="1" v-bind:max="31" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="number" v-model.number="feriado.mes" style="width: 50px" v-bind:min="1" v-bind:max="12" v-if="inserindo" required />
                    </td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Feriados/Componentes/LstFeriados.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
