<%@ Page Title="Grupo de Medida de Projeto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadGrupoMedidaProjeto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.CadGrupoMedidaProjeto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <div id="app">    
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum grupo de medida de projeto encontrado"
                :numero-registros="15" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricao')">Descrição</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td>
                        <button @click.prevent="editar(item, index)" title="Editar" v-if="item.permissoes.editar && !inserindo && numeroLinhaEdicao === -1">
                            <img src="../../Images/edit.gif" />
                        </button>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="item.permissoes.excluir && !inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/ExcluirGrid.gif" />
                        </button>
                    </td>
                    <td style="width: 300px;">
                        {{ item.nome }}
                    </td>
                    <td>
                        <log-alteracao tabela="GrupoMedidaProjeto" :atualizar-ao-alterar="false" :id-item="item.id" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
                <template slot="itemEditando">
                    <td>
                        <button @click.prevent="atualizar" title="Atualizar">
                            <img src="../../Images/ok.gif" />
                        </button>
                        <button @click.prevent="cancelar" title="Cancelar">
                            <img src="../Images/ExcluirGrid.gif" />
                        </button>
                    </td>
                    <td>
                        <input type="text" v-model="nomeGrupoMedidaProjetoAtual" maxlength="60" style="width: 300px">
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td>
                        <button v-on:click.prevent="iniciarCadastro" title="Novo grupo de medida de projeto..." v-if="!inserindo">
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
                        <input type="text" v-model="nomeGrupoMedidaProjetoAtual" maxlength="60" style="width: 300px" v-if="inserindo">
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Projetos/Medidas/Grupos/Componentes/LstGrupoMedidaProjetos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
