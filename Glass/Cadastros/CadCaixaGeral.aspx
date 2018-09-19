<%@ Page Title="Fechamento - Caixa Geral" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="CadCaixaGeral.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCaixaGeral" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Caixas/Geral/Templates/LstCaixaGeral.Filtro.html")
    %>
    <style>
        section.totalizadores {
          display: inline-block;
          text-align: center;
        }

          section.totalizadores div {
            padding-top: 14px;
          }

            section.totalizadores div:first-child {
              padding-top: 0;
            }

          section.totalizadores section.saldo {
            display: inline-grid;
            grid-template-columns: repeat(10, max-content);
            grid-gap: 4px 20px;
          }

          section.totalizadores section.credito {
            display: inline-grid;
            grid-template-columns: repeat(4, max-content);
            grid-gap: 4px 20px;
          }

          section.totalizadores section.parcelas {
            display: inline-grid;
            grid-template-columns: repeat(6, max-content);
            grid-gap: 4px 20px;
          }

          section.totalizadores section.cumulativo {
            display: inline-grid;
            grid-template-columns: repeat(6, max-content);
            grid-gap: 4px 20px;
          }

            section.totalizadores section.saldo label, section.parcelas label, section.totalizadores section.credito label, section.totalizadores section.cumulativo label {
              display: block;
              text-align: left;
              font-weight: bold;
              position: initial;
            }

            section.totalizadores section.saldo span, section.totalizadores section.credito span, section.totalizadores section.parcelas span, section.totalizadores section.cumulativo span {
              display: block;
              text-align: right;
            }
    </style>
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
        <section class="totalizadores" v-if="totalizadores && Object.keys(totalizadores).length !== 0">
            <div>
                <section class="saldo">
                    <label>Entrada dinheiro</label>
                    <span>
                        {{ totalizadores.entrada.dinheiro | moeda }}
                    </span>
                    <label>Entrada cheque</label>
                    <span>
                        {{ totalizadores.entrada.cheque | moeda }}
                    </span>
                    <label>Entrada cartão</label>
                    <span>
                        {{ totalizadores.entrada.cartao | moeda }}
                    </span>
                    <label>Entrada construcard</label>
                    <span>
                        {{ totalizadores.entrada.construcard | moeda }}
                    </span>
                    <label>Entrada permuta</label>
                    <span>
                        {{ totalizadores.entrada.permuta | moeda }}
                    </span>
                    <label style="color: red">Estorno/saída dinheiro</label>
                    <span>
                        {{ totalizadores.saida.dinheiro | moeda }}
                    </span>
                    <label style="color: red">Estorno/saída cheque</label>
                    <span>
                        {{ totalizadores.saida.cheque | moeda }}
                    </span>
                    <label style="color: red">Estorno/saída cartão</label>
                    <span>
                        {{ totalizadores.saida.cartao | moeda }}
                    </span>
                    <label style="color: red">Estorno/saída construcard</label>
                    <span>
                        {{ totalizadores.saida.construcard | moeda }}
                    </span>
                    <label style="color: red">Estorno/saída permuta</label>
                    <span>
                        {{ totalizadores.saida.permuta | moeda }}
                    </span>
                    <label>Saldo dinheiro</label>
                    <span>
                        {{ totalizadores.saldo.dinheiro | moeda }}
                    </span>
                    <label>Saldo cheque</label>
                    <span>
                        {{ totalizadores.saldo.cheque | moeda }}
                    </span>
                    <label>Saldo cartão</label>
                    <span>
                        {{ totalizadores.saldo.cartao | moeda }}
                    </span>
                    <label>Saldo construcard</label>
                    <span>
                        {{ totalizadores.saldo.construcard | moeda }}
                    </span>
                    <label>Saldo permuta</label>
                    <span>
                        {{ totalizadores.saldo.permuta | moeda }}
                    </span>
                </section>
            </div>
            <div>
                <section class="credito">
                    <label>Crédito utilizado</label>
                    <span>
                        {{ totalizadores.credito.recebido | moeda }}                    
                    </span>
                    <label>Crédito gerado</label>
                    <span>
                        {{ totalizadores.credito.gerado | moeda }}                    
                    </span>
                </section>
            </div>
            <div>
                <section class="parcelas" v-if="configuracoes && configuracoes.exibirInformacoesContasRecebidas">
                    <template>
                        <label>Notas promissórias geradas</label>
                        <span>
                            {{ totalizadores.parcelas.gerada | moeda }}                    
                        </span>
                        <label title="Este campo considera apenas o filtro de período">Contas recebidas {{ configuracoes.descricaoContaContabil }}</label>
                        <span>
                            {{ totalizadores.parcelas.recebidaContabil | moeda }}
                        </span>
                        <label title="Este campo considera apenas o filtro de período">Contas recebidas {{ configuracoes.descricaoContaNaoContabil }}</label>
                        <span>
                            {{ totalizadores.parcelas.recebidaNaoContabil | moeda }}
                        </span>
                    </template>
                </section>
            </div>
            <div>
                <section class="cumulativo">
                    <template v-if="configuracoes && configuracoes.exibirTotalCumulativo">
                        <label>Saldo cumulativo dinheiro</label>
                        <span>
                            {{ totalizadores.totaisAcumulados.dinheiro | moeda }}
                        </span>
                        <label>Saldo cumulativo cheque em aberto</label>
                        <span>
                            {{ totalizadores.totaisAcumulados.cheque | moeda }}                    
                        </span>
                        <label>Saldo cumulativo cheque reapres.</label>
                        <span>
                            {{ totalizadores.cheques.reapresentado | moeda }}                    
                        </span>
                        <label>Total cheque terc. utilizáveis</label>
                        <span>
                            {{ totalizadores.cheques.terceiro | moeda }}
                        </span>
                        <label>Saldo cumulativo cheque devolv.</label>
                        <span>
                            {{ totalizadores.cheques.devolvido | moeda }}
                        </span>
                    </template>
                </section>
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
