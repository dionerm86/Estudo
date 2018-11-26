<%@ Page Title="Notas Fiscais" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstNotaFiscal.aspx.cs" Inherits="Glass.UI.Web.Listas.LstNotaFiscal" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/NotasFiscais/Templates/LstNotasFiscais.Filtro.html")
    %>
    <div id="app">
        <notafiscal-filtros :filtro.sync="filtro"></notafiscal-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="atualizarNotasFiscais" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma nota fiscal encontrada">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('numeroNfe')">Num. NF</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('serie')">Série</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('modelo')">Modelo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigoCfop')">CFOP</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipoDocumento')">Tipo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('usuarioCadastro')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nomeEmitente')">Emitente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nomeDestinatario')">Destinatário/Remetente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataEmissao')">Data emissão</a>
                    </th>
                    <th v-if="notasAutorizadasEFinalizadas">
                        <a href="#" @click.prevent="ordenar('dataEntradaSaida')">Data entrada/saída</a>
                    </th>
                    <th v-if="notasAutorizadasEFinalizadas">
                        <a href="#" @click.prevent="ordenar('baseDeCalculoIcms')">Base Calc. ICMS</a>
                    </th>
                    <th v-if="notasAutorizadasEFinalizadas">
                        <a href="#" @click.prevent="ordenar('baseDeCalculoIcmsSt')">Base Calc. ICMS ST</a>
                    </th>
                    <th v-if="notasAutorizadasEFinalizadas">
                        <a href="#" @click.prevent="ordenar('valorIcms')">Valor ICMS</a>
                    </th>
                    <th v-if="notasAutorizadasEFinalizadas">
                        <a href="#" @click.prevent="ordenar('valorIcmsSt')">Valor ICMS ST</a>
                    </th>
                    <th v-if="notasAutorizadasEFinalizadas">
                        <a href="#" @click.prevent="ordenar('valorIpi')">Valor IPI</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('totalNota')">Total</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditar(item)" v-if="item.permissoes.editar">
                            <img v-if="verificarAlteracaoManual(item)" border="0" src="../Images/editarValor.gif" title="Alteração manual de valores">
                            <img v-else border="0" src="../Images/EditarGrid.gif" title="Editar">
                        </a>
                        <a href="#" @click.prevent="excluir(item)" title="Excluir" v-if="item.permissoes.cancelarNotasFiscaisDeTerceiros">
                            <img border="0" src="../Images/ExcluirGrid.gif">
                        </a>
                        <a href="#" @click.prevent="abrirLogEventos(item)" title="Log de eventos" v-if="item.permissoes.exibirLogDeEventos">
                            <img border="0" src="../Images/blocodenotas.png">
                        </a>
                        <a href="#" @click.prevent="abrirImpressaoDanfe(item)" title="Imprimir DANFE" v-if="item.permissoes.imprimirDanfe">
                            <img border="0" src="../Images/Relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="abrirImpressaoNotaFiscalTerceiros(item)" title="Imprimir Nota" v-if="item.permissoes.ImprimirDanfeTerceiros">
                            <img border="0" src="../Images/Relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="consultarSituacaoLote(item)" title="Consulta Situação do Lote" v-if="item.permissoes.consultarSituacaoLote">
                            <img border="0" src="../Images/ConsSitLoteNFe.gif">
                        </a>
                        <a href="#" @click.prevent="consultarSituacao(item)" title="Consulta Situação da NF-e" v-if="item.permissoes.consultarSituacaoNotaFiscal">
                            <img border="0" src="../Images/ConsSitNFe.gif">
                        </a>
                        <a href="#" @click.prevent="baixarXml(item, false)" title="Salvar arquivo da nota fiscal" v-if="item.permissoes.baixarXml">
                            <img border="0" src="../Images/disk.gif">
                        </a>
                        <a href="#" @click.prevent="baixarXml(item, true)" title="Salvar arquivo de inutilização da nota fiscal" v-if="item.permissoes.baixarArquivoInutilizacao">
                            <img border="0" src="../Images/disk.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexoXmlTerceiros(item)" title="Anexar XML Entrada Terceiros" v-if="item.permissoes.anexarXmlTerceiros">
                            <img border="0" src="../Images/page_attach.gif">
                        </a>
                        <a href="#" @click.prevent="baixarXmlTerceiros(item)" title="Baixar XML Entrada Terceiros" v-if="item.permissoes.baixarXmlTerceiros">
                            <img border="0" src="../Images/page_save.gif">
                        </a>
                        <a href="#" @click.prevent="abrirProcessosReferenciados(item)" title="Processos/Documentos Referenciados" v-if="item.permissoes.exibirProcessosReferenciados">
                            <img border="0" src="../Images/clipe.gif">
                        </a>
                        <a href="#" @click.prevent="abrirInformacoesAdicionais(item)" title="Informações adicionais" v-if="item.permissoes.exibirInformacoesAdicionais">
                            <img border="0" src="../Images/page_gear.png">
                        </a>
                        <a href="#" @click.prevent="emitirNotaFiscalFsda(item)" title="Emitir NF-e FS-DA" v-if="item.permissoes.emitirNotaFiscalFsda">
                            <img border="0" src="../Images/arrow_right.gif">
                        </a>
                        <controle-boleto :id-nota-fiscal="item.id" v-if="true || item.permissoes.exibirBoleto"></controle-boleto>
                        <a href="#" @click.prevent="abrirObservacoesLancamentoFiscal(item)" title="Observações do Lançamento Fiscal">
                            <img border="0" src="../Images/Nota.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexos(item)" title="Anexos">
                            <img border="0" src="../Images/Clipe.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAjustesDocumentoFiscal(item)" title="Ajustes do Documento Fiscal">
                            <img border="0" src="../Images/dinheiro.gif">
                        </a>
                        <a href="#" @click.prevent="reenviarEmail(item, false)" title="Reenviar e-mail XML/DANFE" v-if="item.permissoes.reenviarEmail">
                            <img border="0" src="../Images/reenvio_email.png">
                        </a>
                        <a href="#" @click.prevent="reenviarEmail(item, true)" title="Reenviar e-mail XML/DANFE (Cancelamento)" v-if="item.permissoes.reenviarEmailCancelamento">
                            <img border="0" src="../Images/reenvio_email.png">
                        </a>
                        <a href="#" @click.prevent="abrirRentabilidade(item)" title="Rentabilidade" v-if="item.permissoes.exibirRentabilidade">
                            <img border="0" src="../Images/cash_red.png">
                        </a>
                    </td>
                    <td :style="{ color: item.corLinha }">{{ item.numeroNfe }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.serie }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.modelo }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.codigoCfop }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.tipoDocumento.nome }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.usuarioCadastro }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.nomeEmitente }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.nomeDestinatario }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dataEmissao | data }}</td>
                    <td :style="{ color: item.corLinha }" v-if="notasAutorizadasEFinalizadas">{{ item.dataEntradaSaida | data }}</td>
                    <td :style="{ color: item.corLinha }" v-if="notasAutorizadasEFinalizadas">{{ item.baseDeCalculoIcms | moeda }}</td>
                    <td :style="{ color: item.corLinha }" v-if="notasAutorizadasEFinalizadas">{{ item.baseDeCalculoIcmsSt | moeda }}</td>
                    <td :style="{ color: item.corLinha }" v-if="notasAutorizadasEFinalizadas">{{ item.valorIcms | moeda }}</td>
                    <td :style="{ color: item.corLinha }" v-if="notasAutorizadasEFinalizadas">{{ item.valorIcmsSt | moeda }}</td>
                    <td :style="{ color: item.corLinha }" v-if="notasAutorizadasEFinalizadas">{{ item.valorIpi | moeda }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.totalNota | moeda }}</td>
                    <td :style="{ color: item.corLinha }">
                        <span style="white-space: nowrap">
                            <label :style="{ color: item.corSituacao }" v-if="item.permissoes.emitirNotaFiscalFsda">FS-DA</label>
                            <label :style="{ color: item.corSituacao }">{{ item.situacao.nome }}</label>
                            <a href="#" @click.prevent="reabrir(item)" title="Reabrir nota fiscal" v-if="item.permissoes.reabrir">
                                <img border="0" src="../Images/cadeado.gif">
                            </a>
                        </span>
                    </td>
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="gerarNotaFiscalComplementar(item)" title="Gerar NFe Complementar" v-if="item.permissoes.gerarNotaFiscalComplementar">
                            <img border="0" src="../Images/document_add.gif">
                        </a>
                        <a href="#" @click.prevent="abrirCartaCorrecao(item)" title="Criar Carta de Correção" v-if="item.permissoes.exibirCartaCorrecao">
                            <img border="0" src="../Images/email.png">
                        </a>
                        <span v-if="item.permissoes.exibirPedidos">
                            <img border="0" src="../Images/cart.png" :title="'Pedido(s): ' + item.idsPedido">
                        </span>
                        <span v-if="item.permissoes.exibirCompras">
                            <img border="0" src="../Images/cart.png" :title="'Compra(s): ' + item.idsCompra">
                        </span>
                        <a href="#" @click.prevent="abrirAjustesDocumentoFiscal(item)">
                            <img border="0" src="../Images/Nota.gif" title="Ajustes da apuração do ICMS">
                        </a>
                        <a href="#" @click.prevent="abrirCentroCusto(item)" title="Exibir Centro de Custos" v-if="item.permissoes.exibirCentroCusto">
                            <img v-if="item.centroCustoCompleto" border="0" src="../Images/cash_blue.png">
                            <img v-else border="0" src="../Images/cash_red.png">
                        </a>
                        <a href="#" @click.prevent="separarValores(item)" title="Vincular valores" v-if="item.permissoes.separarValores">
                            <img border="0" src="../Images/separar_valores.png">
                        </a>
                        <a href="#" @click.prevent="cancelarSeparacaoValores(item)" title="Desvincular valores" v-if="item.permissoes.cancelarSeparacaoDeValores">
                            <img border="0" src="../Images/separar_valores_cancelar.png">
                        </a>
                        <a href="#" @click.prevent="emitirNfce(item)" title="Emitir NFC-e" v-if="item.permissoes.emitirNfce">
                            <img border="0" src="../Images/arrow_right.gif">
                        </a>
                        <a href="#" @click.prevent="abrirLogMovimentacaoEstoque(item)" title="Log Movimentações de Estoque da NFe" v-if="item.permissoes.exibirLogEstoque">
                            <img border="0" src="../Images/info.gif">
                        </a>
                        <log-alteracao tabela="NotaFical" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div v-if="configuracoes.quantidadeNotasFsda > 0">
                Número de notas fiscais a emitir (Formulário de Segurança): {{ configuracoes.quantidadeNotasFsda }}
            </div>
            <div v-if="configuracoes.ativarContingencia">
                <span>
                    <a href="#" @click.prevent="alterarContingencia(configuracoes.tipoContingenciaScan)">
                        Habilitar Contingência da NF-e
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="alterarContingencia(configuracoes.tipoContingenciaFsda)">
                        <img border="0" src="../Images/fsda.gif" /> Habilitar Contingência FS-DA da NF-e
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="alterarContingencia(configuracoes.tipoContingenciaNaoUtilizar)">
                        Desabilitar Contingência da NF-e
                    </a>
                </span>
            </div>
            <div>
                <span>
                    <a href="#" @click.prevent="abrirImpressaoNotasFiscais(false)">
                        <img border="0" src="../Images/Printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirImpressaoNotasFiscais(true)">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="consultarDisponibilidadeNfe()">
                        <img border="0" src="../Images/nfe.png" height="24px" width="24px" /> Consultar Disponibilidade
                    </a>
                </span>
            </div>
            <div v-if="notasAutorizadasEFinalizadas">
                <span>
                    <a href="#" @click.prevent="abrirImpressaoProdutosNotasFiscais(false)">
                        <img border="0" src="../Images/Printer.png" /> Imprimir (Produtos)
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirImpressaoProdutosNotasFiscais(true)">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel (Produtos)
                    </a>
                </span>
            </div>
            <div>
                <span>
                    <a href="#" @click.prevent="baixarXmlEmLote(false)">
                        <img border="0" src="../Images/disk.gif" /> Baixar XMLs em Lote
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="baixarXmlEmLote(true)">
                        <img border="0" src="../Images/disk.gif" /> Baixar XMLs de inutilização em lote
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/NotasFiscais/Componentes/LstNotasFiscais.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/NotasFiscais/Componentes/LstNotasFiscais.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
