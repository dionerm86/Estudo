<%@ Page Title="Contas Pagas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="ListaContasPagas.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaContasPagas" 
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/ContasPagar/Pagas/Templates/ListaContasPagas.Filtro.html")
    %>    
    <div id="app">
        <contaspagas-filtro :filtro.sync="filtro"></contaspagas-filtro>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro"
                :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma conta paga encontrada."
                :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">
                            Cód.
                        </a>
                    </th>
                    <th>Referência</th>
                    <th>
                        <a href="#" @click.prevent="ordenar('fornecedor/transportador/funcionario')">
                            Fornecedor/Transportador/Func.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('referente')">
                            Referente a
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('formaPagamento')">
                            Forma Pagto.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('parcelas')">
                            Parc.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valor')">
                            Valor
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorPago')">
                            Valor Pago
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('vencimento')">
                            Vencimento
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('pagamento')">
                            Pagto.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('localizacao')">
                            Localização
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('obs')">
                            Obs
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipo')">
                            Tipo
                        </a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="editar(item, index)" title="Editar" v-if="item.permissoes.editar && numeroLinhaEdicao === -1">
                            <img src="../Images/edit.gif" />
                        </button>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.id }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.referencia }}
                    </td>
                    <td v-if="item.fornecedor && item.fornecedor.id" :style="{ color: item.corLinha }">
                        {{ item.fornecedor.nome }}
                    </td>
                    <td v-if="item.transportador && item.transportador.id" :style="{ color: item.corLinha }">
                        {{ item.transportador.nome }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.descricaoContaAPagar }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.formaPagamento }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        <span v-if="item.parcela.exibir">
                            {{ item.parcela.numero }}/{{ item.parcela.total }}
                        </span>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.valorVencimento | moeda }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.valoresPagamento.valorPago | moeda }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.datas.vencimento | data }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.datas.pagamento | data }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.destinoPagamento }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.observacoes.contaPaga }}
                        <template v-if="item.observacoes && item.observacoes.desconto">
                            {{ item.observacoes.desconto }}
                        </template>
                        <template v-else-if="item.observacoes && item.observacoes.acrescimo">
                            {{ item.observacoes.acrescimo }}
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.descricaoContabil }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        <log-alteracao tabela="ContaPagar" :id-item="item.id" :atualizar-ao-alterar="false"
                            v-if="item.permissoes && item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
                <template slot="itemEditando">
                    <td style="white-space: nowrap">
                        <button @click.prevent="atualizar(contaPagaAtual.id, $event)" title="Atualizar">
                            <img src="../../Images/ok.gif" />
                        </button>
                        <button @click.prevent="cancelar" title="Cancelar">
                            <img src="../Images/ExcluirGrid.gif" />
                        </button>
                    </td>
                    <td :style="{ color: contaPagaAtual.corLinha }">
                        {{ contaPagaAtual.id }}
                    </td>
                    <td :style="{ color: contaPagaAtual.corLinha }">
                        {{ contaPagaAtual.referencia }}
                    </td>
                    <td v-if="contaPagaAtual.fornecedor && contaPagaAtual.fornecedor.id" :style="{ color: contaPagaAtual.corLinha }">
                        {{ contaPagaAtual.fornecedor.nome }}
                    </td>
                    <td v-if="contaPagaAtual.transportador && contaPagaAtual.transportador.id" :style="{ color: contaPagaAtual.corLinha }">
                        {{ contaPagaAtual.transportador.nome }}
                    </td>
                    <td :style="{ color: contaPagaAtual.corLinha }">
                        <lista-selecao-id-valor :funcao-recuperar-itens="obterPlanosConta"
                            :item-selecionado.sync="planoContaAtual"></lista-selecao-id-valor>
                    </td>
                    <td :style="{ color: contaPagaAtual.corLinha }">
                        {{ contaPagaAtual.formaPagamento }}
                    </td>
                    <td :style="{ color: contaPagaAtual.corLinha }">
                        <span v-if="contaPagaAtual.parcela.exibir">
                            {{ contaPagaAtual.parcela.numero }}/{{ contaPagaAtual.parcela.total }}
                        </span>
                    </td>
                    <td :style="{ color: contaPagaAtual.corLinha }">
                        {{ contaPagaAtual.valorVencimento | moeda }}
                    </td>
                    <td :style="{ color: contaPagaAtual.corLinha }">
                        {{ contaPagaAtual.valoresPagamento.valorPago | moeda }}
                    </td>
                    <td :style="{ color: contaPagaAtual.corLinha }">
                        {{ contaPagaAtual.datas.vencimento | data }}
                    </td>
                    <td :style="{ color: contaPagaAtual.corLinha }">
                        {{ contaPagaAtual.datas.pagamento | data }}
                    </td>
                    <td :style="{ color: contaPagaAtual.corLinha }">
                        {{ contaPagaAtual.destinoPagamento }}
                    </td>
                    <td :style="{ color: contaPagaAtual.corLinha }">
                        <input type="text" v-model="contaPaga.observacao" />
                    </td>
                    <td :style="{ color: contaPagaAtual.corLinha }">
                        {{ contaPagaAtual.descricaoContabil }}
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
            <section class="links">
                <div>
                    <span>
                        <a href="#" @click.prevent="abrirRelatorio(false, false, false, false)">
                            <img border="0" src="../Images/printer.png" /> Imprimir
                        </a>
                    </span>
                    <span>
                        <a href="#" @click.prevent="abrirRelatorio(true, false, false, false)">
                            <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                        </a>
                    </span>
                </div>
                <div>
                    <span>
                        <a href="#" @click.prevent="abrirRelatorio(false, true, false, false)" v-if="configuracoes.gerarArquivoGCon">
                            <img border="0" src="../Images/blocodenotas.png" /> Exportar para o GCON
                        </a>
                    </span>
                    <span>
                        <a href="#" @click.prevent="abrirRelatorio(false, false, true, false)" v-if="configuracoes.gerarArquivoProsoft">
                            <img border="0" src="../Images/blocodenotas.png" /> Exportar para PROSOFT
                        </a>
                    </span>
                    <span>
                        <a href="#" @click.prevent="abrirRelatorio(false, false, false, true)" v-if="configuracoes.gerarArquivoDominio">
                            <img border="0" src="../Images/blocodenotas.png" /> Exportar para DOMÍNIO SISTEMAS
                        </a>
                    </span>
                </div>
            </section>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/ContasPagar/Pagas/Componentes/ListaContasPagas.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/ContasPagar/Pagas/Componentes/ListaContasPagas.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
