<%@ Page Title="Fechamento de Caixa Diário" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="CadFechamento.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadFechamento" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Caixas/Diario/Templates/LstCaixaDiario.Filtro.html")
    %>
    <div id="app">
        <caixa-diario-filtros :filtro.sync="filtro" :configuracoes="configuracoes" @loja-alterada="lojaAlterada"></caixa-diario-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma movimentação realizada.">
                <template slot="cabecalho">
                    <th>Cód. Mov.</th>
                    <th>Referência</th>
                    <th>Cliente/Fornecedor</th>
                    <th>Valor</th>
                    <th>Juros</th>
                    <th>Funcionário</th>
                    <th>Data</th>
                    <th>Referente a</th>
                    <th>Saldo</th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td :style="{ color: item.corLinha }">{{ item.id }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.referencia }}</td>
                    <td :style="{ color: item.corLinha }">
                        <template v-if="item.cliente && item.cliente.id">
                            {{ item.cliente.id }} - {{ item.cliente.nome }}
                        </template>
                        <template v-else-if="item.fornecedor && item.fornecedor.id">
                            {{ item.fornecedor.id }} - {{ item.fornecedor.nome }}
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">{{ item.valor | moeda }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.juros | moeda }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.usuarioCadastro }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dataCadastro | dataHora }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.planoDeConta }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.saldo | moeda }}</td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div v-if="configuracoes.cadastrarProduto">
                <span>
                    <a href="#" @click.prevent="abrirMovimentacoes(false)">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirMovimentacoes(true)">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir totais
                    </a>
                </span>
            </div>
        </section>
        <section v-if="diaAnterior && diaAnterior.caixaFechado && !diaAtual.caixaFechado && configuracoes && configuracoes.controleCaixaDiario">
            <div>
                <span>
                    <a href="#" @click.prevent="exibirFechamentoCaixa()">
                        <img alt="" border="0" src="../Images/book_go.png" /> Fechar caixa diário
                    </a>
                </span>
            </div>
            <div>
                <span>
                    <label>Saldo do caixa:</label>
                    {{ diaAtual.saldo | moeda }}
                </span>
            </div>
            <div>
                <span>
                    <label>Saldo em dinheiro:</label>
                    {{ diaAtual.saldoDinheiro | moeda }}
                </span>
            </div>
            <div>
                <span>
                    <label>Valor a ser transferido para o caixa geral:</label>
                    <input type="number" v-model.number="valorATransferirCaixaGeral" style="width: 100px;"/>
                </span>
            </div>
            <div>
                <span>
                    <button @click.prevent="fechar()">Fechar caixa</button>
                </span>
            </div>
        </section>
        <section v-else-if="diaAnterior && !diaAnterior.caixaFechado && configuracoes && configuracoes.controleCaixaDiario">
            <div>
                <span>
                    <a href="#" @click.prevent="exibirFechamentoCaixaDiaAnterior()">
                        <img alt="" border="0" src="../Images/book_go.png" /> Fechar caixa diário (dia anterior não foi fechado)
                    </a>
                </span>
            </div>
            <div>
                <span>
                    <label>Saldo do caixa (dia não fechado):</label>
                    {{ diaAnterior.saldo | moeda }}
                </span>
            </div>
            <div>
                <span>
                    <label>Saldo em dinheiro (dia não fechado):</label>
                    {{ diaAnterior.saldoDinheiro | moeda }}
                </span>
            </div>
            <div>
                <span>
                    <label>Valor a ser transferido para o caixa geral:</label>
                    <input type="number" v-model.number="valorATransferirCaixaGeral" style="width: 100px;"/>
                </span>
            </div>
            <div>
                <span>
                    <button @click.prevent="fechar()">Fechar caixa</button>
                </span>
            </div>            
        </section>
        <section>
            <div>
                <span v-if="diaAtual && diaAtual.caixaFechado">
                    <a href="#" @click.prevent="reabrir()" v-if="configuracoes && configuracoes.usuarioAdministrador">
                        <img alt="" border="0" src="../Images/cadeado.gif" /> Reabrir caixa diário
                    </a>
                    <label v-if="configuracoes && !configuracoes.usuarioAdministrador" style="color: red">
                        Apenas administradores podem reabrir o caixa. Caso seja necessário realizar a reabertura, favor solicitar a um administrador do sistema.
                    </label>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Caixas/Diario/Componentes/LstCaixaDiario.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Caixas/Diario/Componentes/LstCaixaDiario.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
