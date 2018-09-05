<%@ Page Title="Consultar Liberações de Pedidos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstLiberarPedido.aspx.cs" Inherits="Glass.UI.Web.Listas.LstLiberarPedido" EnableViewState="false" EnableViewStateMac="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Liberacoes/Templates/LstLiberacoes.Filtro.html")
    %>
    <div id="app">
        <liberacao-filtros :filtro.sync="filtro"></liberacao-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="atualizarLiberacoes" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma liberação encontrada">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Liberação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nomeCliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nomeFuncionario')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricaoPagamento')">Pagamento</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('total')">Total</a>
                    </th>
                    <th v-if="configuracoes.exibirIcms">
                        <a href="#" @click.prevent="ordenar('valorIcms')">Valor ICMS</a>
                    </th>
                    <th v-if="configuracoes.exibirIcms">
                        <a href="#" @click.prevent="ordenar('total')">Total sem ICMS</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataLiberacao')">Data</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="abrirRelatorio(item, false, false)" title="Relatório" v-if="item.permissoes.imprimir">
                            <img border="0" src="../Images/Relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorio(item, true, false)" title="Relatório completo" v-if="configuracoes.imprimirRelatorioCompleto">
                            <img border="0" src="../Images/Report.png">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorio(item, false, true)" title="Relatório do cliente" v-if="configuracoes.imprimirRelatorioCliente">
                            <img border="0" src="../Images/RelatorioCliente.png">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorioNotasPromissorias(item)" title="Notas promissórias" v-if="item.permissoes.exibirNotaPromissoria">
                            <img border="0" src="../Images/Nota.gif">
                        </a>
                        <a :href="obterLinkGerarNfe(item)" title="Gerar NF-e">
                            <img border="0" src="../Images/script_go.gif">
                        </a>
                        <a href="#" @click.prevent="abrirCancelamentoLiberacao(item)" title="Cancelar liberação" v-if="item.permissoes.cancelar && !configuracoes.apenasConsultaLiberacao">
                            <img border="0" src="../Images/ExcluirGrid.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexos(item)" title="Anexos" v-if="!configuracoes.apenasConsultaLiberacao">
                            <img border="0" src="../Images/Clipe.gif">
                        </a>
                        <controle-boleto :id-liberacao="item.id" v-if="item.permissoes.exibirBoleto"></controle-boleto>
                        <a href="#" @click.prevent="reenviarEmail(item)" title="Reenviar e-mail da liberação" v-if="item.permissoes.exibirReenvioEmail && !configuracoes.apenasConsultaLiberacao">
                            <img border="0" src="../Images/email.png">
                        </a>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.cliente.id }} - {{ item.cliente.nome }}</td>
                    <td>{{ item.funcionario.nome }}</td>
                    <td>{{ item.descricaoPagamento }}</td>
                    <td>{{ item.total | moeda }}</td>
                    <td v-if="configuracoes.exibirIcms">{{ item.valorIcms | moeda }}</td>
                    <td v-if="configuracoes.exibirIcms">{{ item.total - item.valorIcms | moeda }}</td>
                    <td>{{ item.dataLiberacao | data }}</td>
                    <td>{{ item.situacao }}</td>
                    <td style="white-space: nowrap">
                        <img border="0" src="../Images/blocodenotas.png" v-if="item.permissoes.exibirNfeGerada" :title="'Notas fiscais geradas: ' + item.notasGeradas" style="cursor: pointer">
                        <log-alteracao tabela="LiberacaoReenvioEmail" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div v-if="!configuracoes.apenasConsultaLiberacao">
                <span>
                    <img border="0" src="../Images/Clipe.gif">
                    <a href="#" @click.prevent="abrirAnexosVariasLiberacoes()" title="Anexar arquivos à várias liberações">
                        Anexar arquivos à várias liberações
                    </a>
                </span>
            </div>
            <div v-if="!configuracoes.apenasConsultaLiberacao">
                <span>
                    <a href="#" @click.prevent="abrirListaLiberacoes(false)">
                        <img border="0" src="../Images/printer.png"> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaLiberacoes(true)">
                        <img border="0" src="../Images/Excel.gif"> Exportar para o Excel
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirTotaisLiberacoes()" title="Exibe os valores de preço, peso e m² totais das liberações listadas.">
                        <img border="0" src="../Images/detalhes.gif"> Totais das liberações
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Liberacoes/Componentes/LstLiberacoes.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Liberacoes/Componentes/LstLiberacoes.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
