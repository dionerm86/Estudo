<%@ Page Title="Tipos de Funcionário" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadTipoFuncionario.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadTipoFuncionario" EnableViewState="false" EnableViewStateMac="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum tipo de funcionário encontrado"
                :numero-registros="15" :exibir-inclusao="true">
                <template slot="cabecalho">
                    <th></th>
                    <th><a href="#" @click.prevent="ordenar('id')">Cod.</a></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricao')">Descrição</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="item.permissoes.excluir">
                            <img src="../Images/ExcluirGrid.gif" />
                        </button>
                    </td>
                    <td>
                        {{ item.id }}
                    </td>
                    <td style="width: 170px;">
                        {{ item.descricao }}
                    </td>
                    <td>
                        <log-alteracao tabela="TipoFuncionario" :atualizar-ao-alterar="false" :id-item="item.id" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>              
                <template slot="itemIncluir">
                    <td colspan="2">
                        <button v-on:click.prevent="iniciarCadastro" title="Novo tipo de funcionário..." v-if="!inserindo">
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
                        <input type="text" v-model="tipoFuncionario.descricao" maxlength="35" style="width: 170px" v-if="inserindo">
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Funcionarios/Tipos/Componentes/LstTiposFuncionario.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
