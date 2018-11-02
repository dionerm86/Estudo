<%@ Page Title="Projetos Efetuados" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstProjeto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.LstProjeto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Projetos/Templates/LstProjetos.Filtro.html")
    %>
    <div id="app">
        <projetos-filtros :filtro.sync="filtro"></projetos-filtros>
        <section>
            <a :href="obterLinkInserirProjeto()">
                Efetuar projeto
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum projeto encontrado" :numero-registros="10">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Código</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('loja')">Loja</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('funcionario')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('total')">Total</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataCadastro')">Data</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarProjeto(item)">
                            <img src="../../Images/Edit.gif">
                        </a>
                        <button @click.prevent="excluir(item)" title="Excluir">
                            <img src="../../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.cliente.id }} - {{ item.cliente.nome }}</td>
                    <td>{{ item.loja }}</td>
                    <td>{{ item.funcionario }}</td>
                    <td>{{ item.total | moeda }}</td>
                    <td>{{ item.dataCadastro | data }}</td>
                    <td>{{ item.situacao }}</td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Projetos/Componentes/LstProjetos.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Projetos/Componentes/LstProjetos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>


