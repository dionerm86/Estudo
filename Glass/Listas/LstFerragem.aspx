<%@ Page Title="Ferragens" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstFerragem.aspx.cs" Inherits="Glass.UI.Web.Listas.LstFerragem" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Projetos/Ferragens/Templates/LstFerragens.Filtro.html")
    %>
    <div id="app">
        <ferragens-filtros :filtro.sync="filtro"></ferragens-filtros>
        <section v-if="configuracoes && configuracoes.cadastrarFerragem">
            <a :href="obterLinkInserirFerragem()">
                Inserir ferragem
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma ferragem encontrada" :numero-registros="10">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Nome</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('fabricante')">Fabricante</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataAlteracao')">Data alteração</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarFerragem(item)">
                            <img src="../../Images/Edit.gif">
                        </a>
                        <button @click.prevent="alterarSituacao(item)" title="Ativar/Inativar">
                            <img src="../../Images/Inativar.gif">
                        </button>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="item.permissoes.excluir">
                            <img src="../../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.fabricante }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>{{ item.dataAlteracao | dataHora }}</td>
                    <td>
                        <log-alteracao tabela="Ferragem" :id-item="item.id" :atualizar-ao-alterar="false"
                            v-if="item.permissoes && item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Projetos/Ferragens/Componentes/LstFerragens.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Projetos/Ferragens/Componentes/LstFerragens.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
