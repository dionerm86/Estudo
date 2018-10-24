<%@ Page Title="Roteiros de Produção" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="LstRoteiroProducao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Producao.LstRoteiroProducao" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Producao/Roteiros/Templates/LstRoteiros.Filtro.html")
    %>
    <div id="app">
        <roteiros-filtros :filtro.sync="filtro"></roteiros-filtros>
        <section>
            <a :href="obterLinkInserirRoteiro()">
                Inserir roteiro
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum roteiro encontrado.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('processo')">Processo</a>
                    </th>
                    <th>
                        Setores
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarRoteiro(item)" title="Editar">
                            <img border="0" src="../../Images/EditarGrid.gif">
                        </a>
                        <a href="#" @click.prevent="excluir(item)" title="Excluir">
                            <img border="0" src="../../Images/ExcluirGrid.gif">
                        </a>
                    </td>
                    <td>{{ item.processo }}</td>
                    <td>{{ obterNomesSetores(item) }}</td>
                    <td style="white-space: nowrap">
                        <log-alteracao tabela="RoteiroProducao" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaRoteiros(false)" title="Imprimir">
                        <img alt="" border="0" src="../../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaRoteiros(true)" title="Exportar para o Excel">
                        <img border="0" src="../../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Producao/Roteiros/Componentes/LstRoteiros.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Producao/Roteiros/Componentes/LstRoteiros.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>

