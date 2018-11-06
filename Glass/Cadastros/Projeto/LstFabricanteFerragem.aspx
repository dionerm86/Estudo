<%@ Page Title="Fabricante de Ferragem" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstFabricanteFerragem.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.LstFabricanteFerragem" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum fabricante de ferragem encontrado."
                :numero-registros="15" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Nome</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('site')">Site</a>
                    </th>
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
                    <td>{{ item.site }}</td>
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
                        <input type="text" v-model="fabricanteFerragem.nome" maxlength="60" style="width: 200px" required />
                    </td>
                    <td>
                        <input type="text" v-model="fabricanteFerragem.site" maxlength="60" style="width: 200px" required />
                    </td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Novo fabricante de ferragem..." v-if="!inserindo">
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
                        <input type="text" v-model="fabricanteFerragem.nome" maxlength="60" style="width: 200px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="text" v-model="fabricanteFerragem.site" maxlength="60" style="width: 200px" v-if="inserindo" required />
                    </td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Projetos/Ferragens/FabricantesFerragem/Componentes/LstFabricantesFerragem.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
