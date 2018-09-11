<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaContasRecebidas.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaContasRecebidas" Title="Contas Recebidas" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/ContasReceber/Templates/LstContasRecebidas.Filtro.html")
    %>
    <div id="app">
        <contarecebida-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></contarecebida-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma conta recebida encontrada." :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#">Referência</a>
                    </th>
                    <th v-if="configuracoes.comissaoPorContasRecebida">
                        <a href="#" @click.prevent="ordenar('idComissao')">Comissão</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('numeroParcela')">Parc.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nomeCliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('formaPagamento')">Forma Pagamento</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorVencimento')">Valor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataVencimento')">Data vencimento</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorRecebido')">Valor recebido</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataRecebimento')">Data recebimento</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('recebidaPor')">Recebida por</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('numeroNFe')">Número NF-e</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('localizacao')">Localização</a>
                    </th>
                    <th v-if="configuracoes.utilizarCnab">
                        <a href="#" @click.prevent="ordenar('numeroArquivoRemessa')">Num. Arquivo Remessa</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('observacao')">Observação</a>
                    </th>
                    <th v-if="configuracoes.exibirComissao">
                        <a href="#" @click.prevent="ordenar('descricaoComissao')">Comissão</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipoContabil')">Tipo</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="editar(item, index)" title="Editar" v-if="item.permissoes.editar && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/Edit.gif">
                        </a>
                        <a href="#" @click.prevent="abrirCancelamentoContaRecebida(item)" title="Cancelar" v-if="item.permissoes.cancelar && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/ExcluirGrid.gif">
                        </a>

                        <a href="#" @click.prevent="abrirRelatorioLiberacao(item)" title="Liberação" v-if="item.idLiberarPedido > 0 && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorioPedido(item)" title="Pedido" v-if="item.idPedido > 0 && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorioAcertoParcial(item)" title="Acerto parcial" v-if="item.idAcertoParcial > 0 && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorioSinal(item)" title="Sinal/Pagto. antecipado" v-if="item.idSinal > 0 && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorioEncontroContas(item)" title="Encontro de contas" v-if="item.idEncontroContas > 0 && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorioObra(item)" title="Obra" v-if="item.idObra > 0 && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/relatorio.gif">
                        </a>

                        <a href="#" @click.prevent="abrirRelatorioAcerto(item)" title="Dados do recebimento" v-if="item.idAcerto > 0 && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/script_go.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorioContaRecebida(item)" title="Dados do recebimento" v-else-if="numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/script_go.gif">
                        </a>
                    </td>
                    <td :style="{ color: item.corLinha }">{{ item.referencia }}</td>
                    <td :style="{ color: item.corLinha }" v-if="configuracoes.comissaoPorContasRecebida">{{ item.idComissao }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.numeroParcela }}/{{ item.numeroMaximoParcelas }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.cliente.id }} - {{ item.cliente.nome }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.formaPagamento }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.valorVencimento | moeda }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dataVencimento | data }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.valorRecebimento | moeda }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dataRecebimento | data }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.recebidaPor }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.numeroNfe }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.localizacao }}</td>
                    <td :style="{ color: item.corLinha }" v-if="configuracoes.utilizarCnab">{{ item.numeroArquivoRemessaCnab }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.observacao }} {{ item.observacaoDescontoAcrescimo }}</td>
                    <td :style="{ color: item.corLinha }" v-if="configuracoes.exibirComissao">{{ item.detalhamentoComissao }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.descricaoContaContabil }}</td>
                    <td :style="{ color: item.corLinha, whiteSpace: 'nowrap' }">
                        <log-alteracao tabela="ContasReceber" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                        <log-cancelamento tabela="ContasReceber" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logCancelamento"></log-cancelamento>
                    </td>
                </template>
                <template slot="itemEditando">
                    <td style="white-space: nowrap">
                        <button @click.prevent="atualizar" title="Atualizar">
                            <img src="../Images/ok.gif">
                        </button>
                        <button @click.prevent="cancelar" title="Cancelar">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ contaRecebidaAtual.referencia }}</td>
                    <td v-if="configuracoes.comissaoPorContasRecebida">{{ contaRecebidaAtual.idComissao }}</td>
                    <td>{{ contaRecebidaAtual.numeroParcela }}/{{ contaRecebidaAtual.numeroMaximoParcelas }}</td>
                    <td v-if="contaRecebidaAtual && contaRecebidaAtual.cliente">
                        {{ contaRecebidaAtual.cliente.id }} - {{ contaRecebidaAtual.cliente.nome }}
                    </td>
                    <td>{{ contaRecebidaAtual.formaPagamento }}</td>
                    <td>{{ contaRecebidaAtual.valorVencimento | moeda }}</td>
                    <td>{{ contaRecebidaAtual.dataVencimento | data }}</td>
                    <td>{{ contaRecebidaAtual.valorRecebimento | moeda }}</td>
                    <td>{{ contaRecebidaAtual.dataRecebimento | data }}</td>
                    <td>{{ contaRecebidaAtual.recebidaPor }}</td>
                    <td>{{ contaRecebidaAtual.numeroNfe }}</td>
                    <td>{{ contaRecebidaAtual.localizacao }}</td>
                    <td v-if="configuracoes.utilizarCnab">{{ contaRecebidaAtual.numeroArquivoRemessaCnab }}</td>
                    <td>
                        <input type="text" v-model="contaRecebida.observacao" style="width: 200px" />
                    </td>
                    <td v-if="configuracoes.exibirComissao">{{ contaRecebidaAtual.detalhamentoComissao }}</td>
                    <td>{{ contaRecebidaAtual.descricaoContaContabil }}</td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaContasRecebidas(false)" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaContasRecebidas(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
            <div>
                <span v-if="configuracoes.gerarArquivoGCon">
                    <a href="#" @click.prevent="gerarArquivoGCon(true)">
                        <img border="0" src="../Images/blocodenotas.png" /> Exportar para o GCON
                    </a>
                </span>
                <span v-if="configuracoes.gerarArquivoProsoft">
                    <a href="#" @click.prevent="gerarArquivoProsoft(true)">
                        <img border="0" src="../Images/blocodenotas.png" /> Exportar para o Prosoft
                    </a>
                </span>
                <span v-if="configuracoes.gerarArquivoDominio">
                    <a href="#" @click.prevent="gerarArquivoDominio(true)">
                        <img border="0" src="../Images/blocodenotas.png" /> Exportar para arquivo da Domínio Sistemas
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/ContasReceber/Componentes/LstContasRecebidas.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/ContasReceber/Componentes/LstContasRecebidas.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
