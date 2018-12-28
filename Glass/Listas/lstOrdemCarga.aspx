<%@ Page Title="Ordens de Carga" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstOrdemCarga.aspx.cs" Inherits="Glass.UI.Web.Listas.lstOrdemCarga" 
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    
    <style>
    
    .destaque span
    {
        font-size:1.2em;
        margin: 0px 8px;
        display: inline-block;
        font-weight: bold;
    }
    
    </style>

    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Carregamentos/OrdensCarga/Templates/LstOrdensCarga.Filtro.html",
            "~/Vue/Carregamentos/OrdensCarga/Templates/LstOrdensCarga.Pedidos.html")
    %>

    <div id="app">
        <ordenscarga-filtros :filtro.sync="filtro" :configuracoes.sync="configuracoes"></ordenscarga-filtros>
        <section>
            <a :href="obterLinkInserirOrdemCarga()">
                Gerar Ordem de Carga
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma ordem de carga encontrada" v-on:atualizou-itens="atualizouItens">
                <template slot="cabecalho">
                    <th></th>
                    <th>Cód. OC</th>
                    <th>Cliente</th>
                    <th>Loja</th>
                    <th>Rota</th>
                    <th>Peso</th>
                    <th>Peso Pendente</th>
                    <th>Total M²</th>
                    <th>Itens</th>
                    <th>Total</th>
                    <th>Total M² Pendente</th>
                    <th>Itens Pendentes</th>
                    <th>Volumes</th>
                    <th>Tipo OC</th>
                    <th>Situação</th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="excluir(item)" title="Excluir">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                        <button v-on:click.prevent="alternarExibicaoPedidos(index)" v-if="!exibindoPedidos(index)">
                            <img src="../../Images/mais.gif" title="Exibir pedidos" />
                        </button>
                        <button v-on:click.prevent="alternarExibicaoPedidos(index)" v-if="exibindoPedidos(index)">
                            <img src="../../Images/menos.gif" title="Esconder pedidos" />
                        </button>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.cliente.id }} - {{ item.cliente.nome }}</td>
                    <td>{{ item.loja }}</td>
                    <td>{{ item.rota }}</td>
                    <td class="destaque">
                        <span>{{ item.peso | decimal }}</span>
                    </td>
                    <td>{{ item.pesoPendente | decimal }}</td>
                    <td>{{ item.totalMetroQuadrado | decimal }}</td>
                    <td>{{ item.quantidadePecas }}</td>
                    <td>{{ item.valorTotalPedidos | moeda }}</td>
                    <td>{{ item.totalMetroQuadradoPendente | decimal }}</td>
                    <td>{{ item.quantidadePecasPendentes }}</td>
                    <td>{{ item.quantidadeVolumes }}</td>
                    <td>{{ item.tipo }}</td>
                    <td>{{ item.situacao }}</td>
                    <td style="white-space: nowrap">
                        <button @click.prevent="abrirRelatorioOrdemCarga(item)" title="Visualizar ordem de carga">
                            <img src="../Images/Relatorio.gif">
                        </button>
                        <log-alteracao tabela="OrdemCarga" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                        <log-cancelamento tabela="OrdemCarga" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logCancelamento"></log-cancelamento>
                    </td>
                </template>
                <template slot="novaLinhaItem" slot-scope="{ item, index, classe }" v-if="exibindoPedidos(index)">
                    <tr v-bind:class="classe" style="border-top: none">
                        <td></td>
                        <td v-bind:colspan="numeroColunasLista() - 1">
                            <a href="#" @click.prevent="abrirInclusaoPedido(item)">
                                Adicionar Pedido
                            </a>
                            <ordens-carga-pedidos v-bind:filtro="{ idOrdemCarga: item.id, item }" v-bind:configuracoes="configuracoes"></ordens-carga-pedidos>
                        </td>
                    </tr>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(false)" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Carregamentos/OrdensCarga/Componentes/LstOrdensCarga.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Carregamentos/OrdensCarga/Componentes/LstOrdensCarga.Pedidos.js" />
            <asp:ScriptReference Path="~/Vue/Carregamentos/OrdensCarga/Componentes/LstOrdensCarga.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
