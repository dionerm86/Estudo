<%@ Page Title="Pedidos em Conferência" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="LstPedidosEspelho.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPedidosEspelho" EnableViewState="false" EnableViewStateMac="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/PedidosConferencia/Templates/LstPedidosConferencia.Filtro.html")
    %>
    <div id="app">
        <pedidoconferencia-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></pedidoconferencia-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="atualizarPedidosConferencia" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum pedido em conferência encontrado">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Pedido</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nomeCliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nomeLoja')">Loja</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nomeConferente')">Conferente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('totalPedidoComercial')">Total pedido</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('totalPedidoConferencia')">Total conferência</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataCadastroConferencia')">Data conferência</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataFinalizacaoConferencia')">Finalização</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('totalM2')">Total m² / Qtde.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('peso')">Peso</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataEntregaPedidoComercial')">Data entrega</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataEntregaFabrica')">Data fábrica</a>
                    </th>
                    <th v-if="configuracoes.usarControleGerenciamentoProjetoCnc">
                        <a href="#" @click.prevent="ordenar('situacaoCnc')">Situação Proj. CNC</a>
                    </th>
                    <th v-if="configuracoes.permitirImpressaoDePedidosImportadosApenasConferidos">
                        <a href="#" @click.prevent="ordenar('pedidoConferido')">Pedido Conferido?</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td :style="{ color: item.corLinhaTabela, whiteSpace: 'nowrap' }">
                        <a :href="obterLinkEditarPedido(item)" title="Editar" v-if="item.permissoes.editar">
                            <img border="0" src="../Images/EditarGrid.gif">
                        </a>
                        <a href="#" @click.prevent="excluir(item)" title="Excluir conferência" v-if="item.permissoes.cancelar">
                            <img border="0" src="../Images/ExcluirGrid.gif">
                        </a>
                        <a href="#" @click.prevent="abrirImpressaoPedidoConferencia(item, item.permissoes.usarControleReposicao, 2)" title="Pedido conferência" v-if="item.permissoes.imprimir">
                            <img border="0" src="../Images/page_gear.png">
                        </a>
                        <a href="#" @click.prevent="abrirImpressaoPedidoConferencia(item, item.permissoes.usarControleReposicao, 0)" title="Pedido comercial" v-if="item.permissoes.imprimir">
                            <img border="0" src="../Images/Relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="abrirImpressaoPedidoConferencia(item, item.permissoes.usarControleReposicao, 3)" title="Memória de cálculo conferência" v-if="item.permissoes.imprimirMemoriaCalculo">
                            <img border="0" src="../Images/calculator.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexoPedidoConferencia(item)" title="Anexos" v-if="item.permissoes.anexarArquivos">
                            <img border="0" src="../Images/Clipe.gif">
                        </a>
                        <a href="#" @click.prevent="abrirImpressaoProjetos(item)" title="Projetos" v-if="item.permissoes.imprimirProjeto">
                            <img border="0" src="../Images/clipboard.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAssociacaoImagemAPeca(item)" title="Associar imagem à peça" v-if="item.permissoes.associarImagemAsPecas">
                            <img border="0" src="../Images/imagem.gif">
                        </a>
                        <a href="#" @click.prevent="abrirProdutosAComprar(item)" title="Produtos que ainda não foram comprados" v-if="item.permissoes.imprimirProdutosAComprar">
                            <img border="0" src="../Images/basket_go.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRentabilidade(item)" title="Rentabilidade" v-if="configuracoes.exibirRentabilidade">
                            <img border="0" src="../Images/cash_red.png">
                        </a>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        <label v-if="item.pedidoGeradoParceiro || item.pedidoImportado">W</label>{{ item.id }}
                    </td>
                    <td :style="{ color: item.corLinha }">{{ item.cliente.id }} - {{ item.cliente.nome }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.nomeLoja }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.nomeConferente }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.totalPedidoComercial | moeda }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.totalPedidoConferencia | moeda }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dataCadastroConferencia | data }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dataFinalizacaoConferencia | data }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.totalM2 }} ({{ item.quantidadePecas }}pç.)</td>
                    <td :style="{ color: item.corLinha }">{{ item.peso }}</td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.situacao }}
                        <a href="#" @click.prevent="reabrir(item)" title="Reabrir conferência" v-if="item.permissoes.reabrir">
                            <img border="0" src="../Images/cadeado.gif">
                        </a>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.dataEntregaPedidoComercial | data }}
                        <label v-if="item.dataEntregaOriginalPedidoComercial && item.dataEntregaOriginalPedidoComercial != item.dataEntregaPedidoComercial"> ({{ item.dataEntregaOriginalPedidoComercial }})</label>
                        <label v-if="item.fastDelivery"> - Fast Del.</label>
                    </td>
                    <td :style="{ color: item.corLinha }">{{ item.dataEntregaFabrica | data }}</td>
                    <td :style="{ color: item.corLinha }" v-if="configuracoes.usarControleGerenciamentoProjetoCnc">
                        {{ item.situacaoCnc.nome }}
                        <a href="#" @click.prevent="alterarSituacaoCnc(item)" title="Alterar situação projeto CNC" v-if="item.permissoes.exibirSituacaoCnc">
                            <img v-if="item.situacaoCnc.id == configuracoes.situacaoCncProjetado" border="0" src="../Images/projeto_deletar.png">
                            <img v-else border="0" src="../Images/projeto.png">
                        </a>
                        <a href="#" @click.prevent="alterarSituacaoCnc(item)" title="Alterar situação projeto CNC" v-if="item.permissoes.exibirSituacaoCncConferencia">
                            <img v-if="item.situacaoCnc.id == configuracoes.situacaoCncSemNecessidadeNaoConferido" border="0" src="../Images/ok.gif">
                            <img v-else border="0" src="../Images/Inativar.gif">
                        </a>
                    </td>
                    <td :style="{ color: item.corLinha }" v-if="configuracoes.permitirImpressaoDePedidosImportadosApenasConferidos">
                        <label v-if="item.pedidoConferido">Conferido</label>
                        <label v-else>Não conferido</label>
                        <a href="#" @click.prevent="marcarPedidoImportadoConferido(item)" title="Alterar pedido conferido/não conferido" v-if="item.permissoes.exibirConferirPedido">
                            <img v-if="item.pedidoConferido" border="0" src="../Images/ok.gif">
                            <img v-else border="0" src="../Images/Inativar.gif">
                        </a>
                    </td>
                    <td style="white-space: nowrap">
                        <img border="0" src="../Images/basket_go.gif" v-if="item.comprasGeradas.length > 0" :title="'Compra(s) gerada(s): ' + item.comprasGeradas">
                        <log-alteracao tabela="PedidoEspelho" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="exibirTotalPecasComSemMarcacao()" title="Exibe os valores de preço, peso e m² totais dos pedidos listados.">
                        <img border="0" src="../Images/detalhes.gif" /> Total marcação
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirAnexoVariosPedidosConferencia()" title="Anexar arquivos à vários pedidos.">
                        <img border="0" src="../Images/Clipe.gif" /> Anexar arquivos à vários pedidos
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="gerarArquivosMaquina(1)" v-if="configuracoes.usarControleGerenciamentoProjetoCnc">
                        <img border="0" src="../Images/blocodenotas.png" /> Gerar arquivo CNC
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="gerarArquivosMaquina(2)" v-if="configuracoes.gerarArquivoDxf">
                        <img border="0" src="../Images/blocodenotas.png" /> Gerar arquivo DXF
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="gerarArquivosMaquina(3)" v-if="configuracoes.gerarArquivoFml">
                        <img border="0" src="../Images/blocodenotas.png" /> Gerar arquivo FML
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="gerarArquivosMaquina(4)" v-if="configuracoes.gerarArquivoSGlass">
                        <img border="0" src="../Images/blocodenotas.png" /> Gerar arquivo SGlass
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="gerarArquivosMaquina(5)" v-if="configuracoes.gerarArquivoIntermac">
                        <img border="0" src="../Images/blocodenotas.png" /> Gerar arquivo Intermac
                    </a>
                </span>
            </div>
            <div>
                <span>
                    <a href="#" @click.prevent="abrirImpressaoListaPedidoConferencia()">
                        <img border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaSelecaoPedidoConferencia()">
                        <img border="0" src="../Images/printer.png" /> Impressão seletiva
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirImpressaoListaProdutosAComprar()">
                        <img border="0" src="../Images/basket_go.gif" /> Produtos a comprar
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/PedidosConferencia/Componentes/LstPedidosConferencia.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/PedidosConferencia/Componentes/LstPedidosConferencia.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
