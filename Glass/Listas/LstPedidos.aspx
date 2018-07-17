﻿<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstPedidos.aspx.cs"
Inherits="Glass.UI.Web.Listas.LstPedidos" Title="Pedidos" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Pedidos/Templates/LstPedidos.Filtro.html")
    %>

    <div id="app">
        <pedido-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></pedido-filtros>
        <section>
            <a :href="obterLinkInserirPedido()" v-if="configuracoes.emitirPedido">
                Inserir Pedido
            </a>
        </section>
        <section>
            <lista-paginada :funcao-recuperar-itens="atualizarPedidos" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum pedido encontrado.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Num</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('idProjeto')">Proj.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('idOrcamento')">Orça.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigoPedidoCliente')">Pedido Cli.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th v-if="configuracoes.exibirColunaLoja">
                        <a href="#" @click.prevent="ordenar('loja')">Loja</a>
                    </th>
                    <th v-if="!vendedorFixo">
                        <a href="#" @click.prevent="ordenar('vendedor')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('total')">Total</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipoVenda')">Pagto</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataPedido')">Data</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataFinalizacao')">Finalização</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataEntrega')">Entrega</a>
                    </th>
                    <th v-if="configuracoes.exibirColunaConfirmacao">
                        <a href="#" @click.prevent="ordenar('dataConfirmacao')">Confirm.</a>
                    </th>
                    <th v-if="configuracoes.exibirColunaPedidoPronto">
                        <a href="#" @click.prevent="ordenar('dataPronto')">Pronto</a>
                    </th>
                    <th v-if="configuracoes.exibirColunaLiberacao">
                        <a href="#" @click.prevent="ordenar('dataLiberacao')">Liberação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th v-if="configuracoes.exibirColunaSituacaoProducao">
                        <a href="#" @click.prevent="ordenar('situacaoProducao')">Situação Produção</a>
                    </th>
                    <th v-if="!tipoPedidoFixo">
                        <a href="#" @click.prevent="ordenar('tipoPedido')">Tipo</a>
                    </th>
                    <th v-if="configuracoes.exibirColunaLiberadoFinanceiro">
                        <a href="#" @click.prevent="ordenar('liberadoEntrega')">Liberado p/ Entrega</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarPedido(item)" title="Editar" v-if="item.permissoes.editar">
                            <img border="0" src="../Images/EditarGrid.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorio(item, 2)" title="Pedido PCP" v-if="item.permissoes.imprimirPcp">
                            <img border="0" src="../Images/page_gear.png">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorio(item, 0)" title="Pedido" v-if="item.permissoes.imprimir">
                            <img border="0" :src="item.permissoes.imprimirPcp ? '../Images/Relatorio_menos.jpg' : '../Images/Relatorio.gif'">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorio(item, 1, false)" title="Memória de cálculo" v-if="item.permissoes.imprimirMemoriaCalculo">
                            <img border="0" src="../Images/calculator.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorioNotasPromissorias(item)" title="Nota promissória" v-if="item.permissoes.imprimirNotaPromissoria">
                            <img border="0" src="../Images/Nota.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorioProjeto(item)" title="Projeto" v-if="item.permissoes.imprimirProjeto">
                            <img border="0" src="../Images/clipboard.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexos(item)" title="Anexos">
                            <img border="0" src="../Images/Clipe.gif">
                        </a>
                        <a :href="obterLinkSugestoes(item)" title="Sugest�es" v-if="configuracoes.exibirSugestoes">
                            <img border="0" src="../Images/Nota.gif">
                        </a>
                        <a href="#" @click.prevent="abrirCancelarPedido(item)" title="Cancelar" v-if="item.permissoes.cancelar">
                            <img border="0" src="../Images/ExcluirGrid.gif">
                        </a>
                        <a href="#" @click.prevent="abrirDesconto(item)" title="Alterações" v-if="item.permissoes.desconto">
                            <img border="0" src="../Images/money_delete.gif">
                        </a>
                        <a href="#" @click.prevent="abrirImagemPeca(item)" title="Exibir imagens das peças" v-if="item.permissoes.imagemPeca">
                            <img border="0" src="../Images/imagem.gif">
                        </a>
                        <a href="#" @click.prevent="abrirItensFaltamLiberar(item)" title="Itens que ainda faltam liberar" v-if="item.permissoes.impressaoItensLiberar">
                            <img border="0" src="../Images/book_go.gif">
                        </a>
                        <a :href="obterLinkAlterarProcessoEAplicacao(item)" title="Alterar Processo/Aplicação das peças" v-if="item.permissoes.alterarProcessoEAplicacao">
                            <img border="0" src="../Images/application_edit.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRentabilidade(item)" title="Rentabilidade" v-if="configuracoes.exibirRentabilidade">
                            <img border="0" src="../Images/cash_red.png">
                        </a>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.idProjeto }}</td>
                    <td>{{ item.idOrcamento }}</td>
                    <td>{{ item.codigoPedidoCliente }}</td>
                    <td>{{ item.cliente.id }} - {{ item.cliente.nome }}</td>
                    <td v-if="configuracoes.exibirColunaLoja">{{ item.loja.nome }}</td>
                    <td v-if="!vendedorFixo">
                        {{ item.vendedor.nome }}
                    </td>
                    <td>{{ item.total | moeda }}</td>
                    <td>{{ item.tipoVenda }}</td>
                    <td>{{ item.dataPedido | dataHora }}</td>
                    <td>
                        <span v-if="item.finalizacao">
                            {{ item.finalizacao.data | data }} (Func.: {{ item.finalizacao.funcionario }})
                        </span>
                    </td>
                    <td>
                        {{ item.dataEntrega.atual | data }}
                        <span v-if="item.dataEntrega.original">
                            ({{ item.dataEntrega.original | data }})
                        </span>
                    </td>
                    <td v-if="configuracoes.exibirColunaConfirmacao">
                        <span v-if="item.confirmacao">
                            {{ item.confirmacao.data | data }} (Func.: {{ item.confirmacao.funcionario }})
                        </span>
                    </td>
                    <td v-if="configuracoes.exibirColunaPedidoPronto">
                        {{ item.dataPronto | data }}
                    </td>
                    <td v-if="configuracoes.exibirColunaLiberacao">
                        <span v-if="item.liberacao">
                            {{ item.liberacao.data | data }} (Func.: {{ item.liberacao.funcionario }})
                        </span>
                    </td>
                    <td style="white-space: nowrap">
                        <span v-if="!item.permissoes.anexosLiberacao || !item.liberacao">{{ item.situacao }}</span>
                        <a href="#" @click.prevent="abrirAnexosLiberacao(item)" v-else>{{ item.situacao }}</a>

                        <a href="#" @click.prevent="reabrirPedido(item)" title="Reabrir pedido" v-if="item.permissoes.reabrir">
                            <img src="../Images/cadeado.gif">
                        </a>
                    </td>
                    <td v-if="configuracoes.exibirColunaSituacaoProducao" style="white-space: nowrap">
                        <a :href="obterLinkConsultaProducao(item)" v-if="configuracoes.controlarProducao && item.producao.situacao !== '-'">{{ item.producao.situacao }}</a>
                        <span v-else>{{ item.producao.situacao }}</span>

                        <a :href="obterLinkConsultaProducao(item)" v-if="configuracoes.controlarProducao && item.producao.pronto">
                            <img src="../Images/curtir.gif">
                        </a>
                        <a :href="obterLinkConsultaProducao(item)" v-if="configuracoes.controlarProducao && item.producao.pendente">
                            <img src="../Images/nao curtir.gif">
                        </a>
                    </td>
                    <td v-if="!tipoPedidoFixo">
                        {{ item.tipo }}
                    </td>
                    <td v-if="configuracoes.exibirColunaLiberadoFinanceiro" style="white-space: nowrap">
                        {{ item.liberadoFinanceiro | simNao }}
                        <a href="#" @click.prevent="alterarLiberacaoFinanceira(item, true)" v-if="!item.liberadoFinanceiro">
                            Liberar
                        </a>
                        <a href="#" @click.prevent="alterarLiberacaoFinanceira(item, false)" v-else>
                            Desfazer
                        </a>
                    </td>
                    <td style="white-space: nowrap">
                        <log-alteracao tabela="Pedido" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                        <img src="../Images/cifrao.png" title="Sinal e Pagamento Antecipado" key="sinalEPagamentoAntecipado" v-if="item.sinalEPagamentoAntecipado.temPagamentoAntecipado && item.sinalEPagamentoAntecipado.idSinal">
                        <img src="../Images/cifrao.png" title="Pagamento Antecipado" key="sinalEPagamentoAntecipado" v-else-if="item.sinalEPagamentoAntecipado.temPagamentoAntecipado">
                        <img src="../Images/cifrao.png" :title="item.sinalEPagamentoAntecipado.valorSinal | moeda | textoSinal" v-else-if="item.sinalEPagamentoAntecipado.idSinal"
                            key="sinalEPagamentoAntecipado">
                        <span v-if="item.liberacao && item.liberacao.observacao" @mouseover="exibirDivComoPopup(`obsLiberacao_${item.id}`, $event)"
                            onmouseout="UnTip()">
                            <img src="../Images/Nota.gif" title="Observação da Liberação">
                            <div :id="`obsLiberacao_${item.id}`" style="display: none">
                                Observação da Liberação:
                                <div>
                                    {{ item.liberacao.observacao }}
                                </div>
                            </div>
                        </span>
                        <span v-if="item.permissoes.finalizacoesFinanceiro" @mouseover="exibirDivComoPopup(`obsFinFinanc_${item.id}`, $event)" onmouseout="UnTip()">
                            <img src="../Images/money_hist.gif" title="Finalizações financeiro">
                            <div :id="`obsFinFinanc_${item.id}`" style="display: none">
                                <lista-paginada :funcao-recuperar-itens="buscarObservacoesFinanceiro" :filtro="{ id: item.id }" :ordenacao="ordenacaoObservacoesFinanceiro"
                                    :numero-registros="5">
                                    <template slot="cabecalho">
                                        <th>
                                            <a href="#" @click.prevent="ordenarObservacoesFinanceiro('motivo')">Ação</a>
                                        </th>
                                        <th>
                                            <a href="#" @click.prevent="ordenarObservacoesFinanceiro('observacao')">Observação</a>
                                        </th>
                                        <th>
                                            <a href="#" @click.prevent="ordenarObservacoesFinanceiro('cadastro.funcionario')">Funcionário</a>
                                        </th>
                                        <th>
                                            <a href="#" @click.prevent="ordenarObservacoesFinanceiro('cadastro.data')">Data Cadastro</a>
                                        </th>
                                    </template>
                                    <template slot="item" slot-scope="{ item }">
                                        <td :style="{ color: item.corLinha }">{{ item.motivo }}</td>
                                        <td :style="{ color: item.corLinha }">{{ item.observacao }}</td>
                                        <td :style="{ color: item.corLinha }">{{ item.cadastro.data }}</td>
                                        <td :style="{ color: item.corLinha }">{{ item.cadastro.funcionario }}</td>
                                    </template>
                                </lista-paginada>
                            </div>
                        </span>
                        <img src="../Images/carregamento.png" :title='`Ordem de Carga: ${exibirOrdensDeCarga(item)}`' v-if="item.idsOrdensDeCarga && item.idsOrdensDeCarga.length > 0"
                            style="width: 16px; height: 16px">
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section>
            <span>
                <a href="#" @click.prevent="anexarArquivosAVariosPedidos()" title="Anexar arquivos a vários pedidos">
                    <img src="../Images/Clipe.gif"> Anexar arquivos a vários pedidos
                </a>
            </span>
            <span v-if="configuracoes.exibirBotoesTotais">
                <span>
                    <a href="#" @click.prevent="abrirListaTotais()" title="Exibe os valores de preço, peso e m² totais dos pedidos listados">
                        <img src="../Images/detalhes.gif"> Total
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirGraficoTotaisDiarios()" title="Exibe os valores de preço, peso e m² totais dos pedidos listados">
                        <img src="../Images/detalhes.gif"> Gráfico Totais Diários
                    </a>
                </span>
            </span>
        </section>
    </div>

    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Pedidos/Componentes/LstPedidos.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Pedidos/Componentes/LstPedidos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
