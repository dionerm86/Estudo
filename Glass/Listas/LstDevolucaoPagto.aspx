<%@ Page Title="Devolu��o de Pagamento" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstDevolucaoPagto.aspx.cs" Inherits="Glass.UI.Web.Listas.LstDevolucaoPagto" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/DevolucoesPagamento/Templates/LstDevolucoesPagamento.Filtro.html")
    %>
    <div id="app">
        <devolucoes-pagamento-filtros :filtro.sync="filtro"></devolucoes-pagamento-filtros>
        <section>
            <a :href="obterLinkInserirDevolucao()">
                Inserir devolu��o de pagamento
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="N�o h� devolu��es de pagamento para esse filtro.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">C�d.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('data')">Data</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valor')">Valor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situa��o</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('funcionario')">Funcion�rio</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="cancelar(item)" title="Cancelar" v-if="item.permissoes.cancelar">
                            <img src="../Images/ExcluirGrid.gif"/>
                        </button>
                        <button @click.prevent="abrirRelatorioDevolucao(item.id)">
                            <img src="../Images/Relatorio.gif" />
                        </button>
                        <button @click.prevent="abrirGerenciamentoDeFotos(item)">
                            <img src="../Images/Clipe.gif"/>
                        </button>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>
                        <template v-if="item.cliente">
                            {{ item.cliente.nome }}
                        </template>
                    </td>
                    <td>{{ item.dataCadastro | data }}</td>
                    <td>{{ item.valor | moeda }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>{{ item.usuarioCadastro }}</td>
                    <td style="white-space: nowrap">
                        <log-cancelamento tabela="DevolucaoPagamento" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logCancelamento"></log-cancelamento>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaDevolucoes()" title="Imprimir">
                        <img src="../Images/Printer.png"/> Imprimir
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/DevolucoesPagamento/Componentes/LstDevolucoesPagamento.Filtro.js" /> 
            <asp:ScriptReference Path="~/Vue/DevolucoesPagamento/Componentes/LstDevolucoesPagamento.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
