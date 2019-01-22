<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaLiberarPedidoMov.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaLiberarPedidoMov" Title="Movimentações de Liberações de Pedidos" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Contentl" ContentPlaceHolderID="Conteudo" runat="server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Liberacoes/Movimentacoes/Templates/LstMovimentacoesLiberacoes.Filtro.html")
    %>
    <div id="app">
        <movimentacoes-liberacoes :filtro.sync="filtro"></movimentacoes-liberacoes>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" :numero-registros="15" mensagem-lista-vazia="Nenhuma movimentação de liberação encontrada">
                <template slot="cabecalho">
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">
                            Liberação
                        </a>                        
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">
                            Cliente
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">
                            Situação
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('total')">
                            Total
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('desconto')">
                            Desconto
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dinheiro')">
                            Dinheiro
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cheque')">
                            Cheque
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('prazo')">
                            Prazo
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('boleto')">
                            Boleto
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('deposito')">
                            Depósito
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cartao')">
                            Cartão
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('outros')">
                            Outros
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('debito')">
                            Débito
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('credito')">
                            Crédito
                        </a>
                    </th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td>{{ item.id }}</td>
                    <td>{{ item.cliente }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>{{ item.total | moeda }}</td>
                    <td>{{ item.desconto | moeda }}</td>
                    <td>{{ item.formasPagamento.dinheiro | moeda }}</td>
                    <td>{{ item.formasPagamento.cheque | moeda }}</td>
                    <td>{{ item.formasPagamento.boleto | moeda }}</td>
                    <td>{{ item.formasPagamento.deposito | moeda }}</td>
                    <td>{{ item.formasPagamento.cartao | moeda }}</td>
                    <td>{{ item.formasPagamento.prazo | moeda }}</td>
                    <td>{{ item.formasPagamento.outros | moeda }}</td>
                    <td>{{ item.formasPagamento.debito | moeda }}</td>
                    <td>{{ item.formasPagamento.credito | moeda }}</td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaMovimentacoesLiberacoes(false, false)" title="Imprimir">
                        <img src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaMovimentacoesLiberacoes(false, true)" title="Exportar para o Excel">
                        <img src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaMovimentacoesLiberacoes(true, false)" title="Imprimir (sem valores)">
                        <img src="../Images/printer.png" /> Imprimir (sem valores)
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="false">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Liberacoes/Movimentacoes/Componentes/LstMovimentacoesLiberacoes.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Liberacoes/Movimentacoes/Componentes/LstMovimentacoesLiberacoes.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
