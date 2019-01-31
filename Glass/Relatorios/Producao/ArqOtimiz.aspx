<%@ Page Title="Arquivos de Otimização" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ArqOtimiz.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.ArqOtimiz" EnableViewState ="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/ArquivosOtimizacao/Templates/LstArquivosOtimizacao.Filtro.html")
    %>

    <div id="app">
        <arquivos-otimizacao-filtros v-bind:filtro.sync="filtro"></arquivos-otimizacao-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum arquivo de otimização encontrado.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('funcionario')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('data')">Data</a>
                    </th>
                    <th>
                        <a href="#" @click.prevente="ordenar('direcao')">Direção</a>
                    </th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="abrirLinkDownload(item)" title="Download do arquivo" v-if="item.permissoes.exibirLinkDownload">
                            <img src="../../Images/Relatorio.gif" />
                        </button>
                        <button @click.prevent="abrirLinkDownloadECutter(item.id)" title="Download do arquivo" v-if="item.permissoes.exibirLinkECutter">
                            <img src="../../Images/Relatorio.gif" />
                        </button>
                    </td>
                    <td>{{ item.funcionario }}</td>
                    <td>{{ item.dataCadastro | dataHora }}</td>
                    <td>{{ item.direcao.descricao }}</td>
                </template>
            </lista-paginada>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/ArquivosOtimizacao/Componentes/LstArquivosOtimizacao.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/ArquivosOtimizacao/Componentes/LstArquivosOtimizacao.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
