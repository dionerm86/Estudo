<%@ Page Title="Fechamento - Caixa Geral" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="CadCaixaGeral.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCaixaGeral" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Caixas/Geral/Templates/LstCaixaGeral.Filtro.html")
    %>
    <div id="app">
        <caixa-geral-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></caixa-geral-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma movimentação encontrada." v-bind:numero-registros="500">
                <template slot="cabecalho">
                    <th>Cód. Mov.</th>
                    <th>Referência</th>
                    <th>Cliente/Fornecedor</th>
                    <th>Valor</th>
                    <th>Juros</th>
                    <th>Data</th>
                    <th>Loja</th>
                    <th>Referente a</th>
                    <th>Saldo</th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td :style="{ color: item.corLinha }">
                        <template v-if="item.id">
                            {{ item.id }}
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">{{ item.referencia }}</td>
                    <td :style="{ color: item.corLinha }">
                        <template v-if="item.cliente && item.cliente.id">
                            {{ item.cliente.id }} - {{ item.cliente.nome }}
                        </template>
                        <template v-else-if="item.fornecedor && item.fornecedor.id">
                            {{ item.fornecedor.id }} - {{ item.fornecedor.nome }}
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        <template v-if="item.id">
                            {{ item.valor | moeda }}
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        <template v-if="item.id">
                            {{ item.juros | moeda }}
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        <template v-if="item.id">
                            {{ item.dataMovimentacao | dataHora }}
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">{{ item.loja }}</td>
                    <td>
                        <label v-if="item.id" :style="{ color: item.corLinha }">
                            {{ item.planoDeConta }}
                            <template v-if="item.contaBanco && item.contaBanco.length">
                                 ({{ item.contaBanco }})
                            </template>
                            <template v-if="item.observacao && item.observacao.length">
                                  Obs. ({{ item.observacao }})
                            </template>
                        </label>
                        <label v-else style="color: black; font-weight: bold">
                            {{ item.observacao }}
                        </label>
                    </td>
                    <td :style="{ color: item.corLinha }">{{ item.saldo | moeda }}</td>
                </template>
            </lista-paginada>
        </section>
        <section v-if="totalizadores && Object.keys(totalizadores).length !== 0">
            <div>
                <span>
                    <label>Entrada dinheiro</label>
                    {{ totalizadores.entrada.dinheiro | moeda }}
                </span>
                <span>
                    <label>Entrada cheque</label>
                    {{ totalizadores.entrada.cheque | moeda }}
                </span>
                <span>
                    <label>Entrada cartão</label>
                    {{ totalizadores.entrada.cartao | moeda }}
                </span>
                <span>
                    <label>Entrada construcard</label>
                    {{ totalizadores.entrada.construcard | moeda }}
                </span>
                <span>
                    <label>Entrada permuta</label>
                    {{ totalizadores.entrada.permuta | moeda }}
                </span>
            </div>
            <div>
                <span>
                    <label style="color: red">Estorno/saída dinheiro</label>
                    {{ totalizadores.saida.dinheiro | moeda }}
                </span>
                <span>
                    <label style="color: red">Estorno/saída cheque</label>
                    {{ totalizadores.saida.cheque | moeda }}
                </span>
                <span>
                    <label style="color: red">Estorno/saída cartão</label>
                    {{ totalizadores.saida.cartao | moeda }}
                </span>
                <span>
                    <label style="color: red">Estorno/saída construcard</label>
                    {{ totalizadores.saida.construcard | moeda }}
                </span>
                <span>
                    <label style="color: red">Estorno/saída permuta</label>
                    {{ totalizadores.saida.permuta | moeda }}
                </span>
            </div>
            <div>
                <span>
                    <label>Saldo dinheiro</label>
                    {{ totalizadores.saldo.dinheiro | moeda }}
                </span>
                <span>
                    <label>Saldo cheque</label>
                    {{ totalizadores.saldo.cheque | moeda }}
                </span>
                <span>
                    <label>Saldo cartão</label>
                    {{ totalizadores.saldo.cartao | moeda }}
                </span>
                <span>
                    <label>Saldo construcard</label>
                    {{ totalizadores.saldo.construcard | moeda }}
                </span>
                <span>
                    <label>Saldo permuta</label>
                    {{ totalizadores.saldo.permuta | moeda }}
                </span>
            </div>
            <div>
                <span>
                    <label>Crédito utilizado</label>
                    {{ totalizadores.credito.recebido | moeda }}                    
                </span>
                <span>
                    <label>Notas promissórias geradas</label>
                    {{ totalizadores.parcelas.gerada | moeda }}                    
                </span>

                <span>
                    <label>Crédito gerado</label>
                    {{ totalizadores.credito.gerado | moeda }}                    
                </span>
            </div>
            <div>
                <template v-if="configuracoes && configuracoes.exibirInformacoesContasRecebidas">
                    <span>
                        <label title="Este campo considera apenas o filtro de período">Contas recebidas {{ configuracoes.descricaoContaContabil }}</label>
                        {{ totalizadores.parcelas.recebidaContabil | moeda }}
                    </span>
                    <span>
                        <label title="Este campo considera apenas o filtro de período">Contas recebidas {{ configuracoes.descricaoContaNaoContabil }}</label>
                        {{ totalizadores.parcelas.recebidaNaoContabil | moeda }}
                    </span>
                </template>
            </div>
            <div>
                <template v-if="configuracoes && configuracoes.exibirTotalCumulativo">
                    <span>
                        <label>Saldo cumulativo dinheiro</label>
                        {{ totalizadores.totaisAcumulados.dinheiro | moeda }}
                    </span>
                    <span>
                        <label>Saldo cumulativo cheque em aberto</label>
                        {{ totalizadores.totaisAcumulados.cheque | moeda }}                    
                    </span>
                    <span>
                        <label>Saldo cumulativo cheque reapres.</label>
                        {{ totalizadores.cheques.reapresentado | moeda }}                    
                    </span>
                </template>
                <template v-if="configuracoes && configuracoes.exibirTotalCumulativo">
                    <span>
                        <label>Total cheque terc. utilizáveis</label>
                        {{ totalizadores.cheques.terceiro | moeda }}
                    </span>
                    <span>
                        <label>Saldo cumulativo cheque devolv.</label>
                        {{ totalizadores.cheques.devolvido | moeda }}
                    </span>
                </template>
            </div>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirMovimentacoes(false, false)">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir fechamento
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirMovimentacoes(false, true)">
                        <img alt="" border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
            <div>
                <span>
                    <a href="#" @click.prevent="abrirMovimentacoes(true, false)">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir Totais
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Caixas/Geral/Componentes/LstCaixaGeral.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Caixas/Geral/Componentes/LstCaixaGeral.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
