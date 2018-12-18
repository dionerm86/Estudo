<%@ Page Title="Ordens de Carga" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstOrdemCarga.aspx.cs" Inherits="Glass.UI.Web.Listas.lstOrdemCarga" %>

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
            "~/Vue/Carregamentos/OrdensCarga/Templates/LstOrdensCarga.Filtro.html")
    %>

    <div id="app">
        <ordenscarga-filtros :filtro.sync="filtro" :configuracoes.sync="configuracoes"></ordenscarga-filtros>
        <section>
            <a :href="obterLinkInserirOrdemCarga()" v-if="configuracoes.emitirPedido">
                Gerar Ordem de Carga
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma ordem de carga encontrada">
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
                <template slot="item" slot-scope="{ item }">
                    <td>
                        
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.idCliente }} - {{ item.nomeCliente }}</td>
                    <td>{{ item.loja }}</td>
                    <td>{{ item.rota }}</td>
                    <td class="destaque">{{ item.peso }}</td>
                    <td>{{ item.pesoPendente }}</td>
                    <td>{{ item.totalMetroQuadrado }}</td>
                    <td>{{ item.quantidadePecas }}</td>
                    <td>{{ item.valorTotalPedidos}}</td>
                    <td>{{ item.totalMetroQuadradoPendente}}</td>
                    <td>{{ item.quantidadePecasPendentes }}</td>
                    <td>{{ item.quantidadeVolumes }}</td>
                    <td>{{ item.tipo }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>

                    </td>
                </template>
                <template slot="novaLinhaItem" slot-scope="{ item, index, classe }" v-if="exibindoOrdensCarga(index)">
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
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Carregamentos/OrdensCarga/Componentes/LstOrdensCarga.js" />
            <asp:ScriptReference Path="~/Vue/Carregamentos/OrdensCarga/Componentes/LstOrdensCarga.Filtro.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
