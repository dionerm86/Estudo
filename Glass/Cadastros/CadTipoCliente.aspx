<%@ Page Title="Tipos de Cliente" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="CadTipoCliente.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadTipoCliente" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum tipo de cliente encontrado."
                :numero-registros="15" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricao')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cobrarAreaMinima')">Cobrar área mínima</a>
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
                    <td>{{ item.descricao }}</td>
                    <td>{{ item.cobrarAreaMinima | simNao }}</td>
                    <td>
                        <log-alteracao tabela="TipoCliente" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
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
                        <input type="text" v-model="tipoCliente.descricao" maxlength="30" style="width: 150px" required />
                    </td>
                    <td>
                        <input type="checkbox" v-model="tipoCliente.cobrarAreaMinima" />
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Novo tipo de cliente..." v-if="!inserindo">
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
                        <input type="text" v-model="tipoCliente.descricao" maxlength="30" style="width: 150px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="checkbox" v-model="tipoCliente.cobrarAreaMinima" v-if="inserindo" />
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Clientes/Tipos/Componentes/LstTipos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>