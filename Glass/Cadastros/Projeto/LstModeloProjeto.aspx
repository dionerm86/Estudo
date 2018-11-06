<%@ Page Title="Modelos de Projeto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="LstModeloProjeto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.LstModeloProjeto" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Projetos/ModelosProjeto/Templates/LstModelosProjeto.Filtro.html")
    %>
    <div id="app">
        <modelos-projeto-filtros :filtro.sync="filtro"></modelos-projeto-filtros>
        <section>
            <a :href="obterLinkInserirModeloProjeto()">
                Inserir modelo de projeto
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum modelo de projeto encontrado" :numero-registros="10">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigo')">Código</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarModeloProjeto(item)">
                            <img src="../../Images/Edit.gif">
                        </a>
                        <button @click.prevent="abrirTelaPosicaoPecasModeloProjeto(item)" title="Configurar posição das informações na figura">
                            <img src="../../Images/Coord.gif">
                        </button>
                        <button @click.prevent="abrirRelatorioModeloProjeto(item)" title="Impressão do modelo de projeto">
                            <img src="../../Images/Relatorio.gif">
                        </button>
                    </td>
                    <td>{{ item.codigo }}</td>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>
                        <log-alteracao tabela="ProjetoModelo" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaModelosProjeto(false)">
                        <img alt="" border="0" src="../../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaModelosProjeto(true)">
                        <img alt="" border="0" src="../../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Projetos/ModelosProjeto/Componentes/LstModelosProjeto.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Projetos/ModelosProjeto/Componentes/LstModelosProjeto.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>

