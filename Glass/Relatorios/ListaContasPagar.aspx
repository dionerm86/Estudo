<%@ Page Title="Contas a Pagar" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="ListaContasPagar.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaContasPagar"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/ContasPagar/Templates/ListaContasPagar.Filtro.html")
    %>
    <div id="app">
        <contaspagar-filtro :filtro.sync="filtro"></contaspagar-filtro>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro"
                :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma conta a pagar encontrada."
                :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">
                            Cód.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('referencia')">
                            Referência
                        </a>
                    </th>
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
                        <a href="#" @click.prevent="ordenar('vencimento')">
                            Vencimento
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataCadastro')">
                            Data Cad.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('boletoChegou')">
                            Boleto Chegou?
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
                        {{ item.formaPagamento.nome }}
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
                        {{ item.datas.vencimento | data }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.datas.cadastro | data }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.boletoChegou ? "Sim" : "Não" }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.observacoes.contaAPagar }}
                        <template v-if="item && item.observacoes && item.observacoes.desconto">
                            {{ item.observacoes.desconto }}
                        </template>
                        <template v-else-if="item && item.observacoes && item.observacoes.acrescimo">
                            {{ item.observacoes.acrescimo }}
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.contaContabil }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        <log-alteracao tabela="ContaPagar" :id-item="item.id" :atualizar-ao-alterar="false"
                            v-if="item.permissoes && item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
                <template slot="itemEditando">
                    <td style="white-space: nowrap">
                        <button @click.prevent="atualizar(contaAPagarAtual.id, $event)" title="Atualizar">
                            <img src="../../Images/ok.gif" />
                        </button>
                        <button @click.prevent="cancelar" title="Cancelar">
                            <img src="../Images/ExcluirGrid.gif" />
                        </button>
                    </td>
                    <td :style="{ color: contaAPagarAtual.corLinha }">
                        {{ contaAPagarAtual.id }}
                    </td>
                    <td :style="{ color: contaAPagarAtual.corLinha }">
                        {{ contaAPagarAtual.referencia }}
                    </td>
                    <td v-if="contaAPagarAtual.fornecedor && contaAPagarAtual.fornecedor.id" :style="{ color: contaAPagarAtual.corLinha }">
                        {{ contaAPagarAtual.fornecedor.nome }}
                    </td>
                    <td v-if="contaAPagarAtual.transportador && contaAPagarAtual.transportador.id" :style="{ color: contaAPagarAtual.corLinha }">
                        {{ contaAPagarAtual.transportador.nome }}
                    </td>
                    <td :style="{ color: contaAPagarAtual.corLinha }">
                        <lista-selecao-id-valor :funcao-recuperar-itens="obterPlanosConta"
                            :item-selecionado.sync="planoContaAtual"></lista-selecao-id-valor>
                    </td>
                    <td :style="{ color: contaAPagarAtual.corLinha }">
                        <lista-selecao-id-valor :funcao-recuperar-itens="obterFormasPagamentoCompras"
                            :item-selecionado.sync="formaPagamentoAtual"></lista-selecao-id-valor>
                    </td>
                    <td :style="{ color: contaAPagarAtual.corLinha }">
                        <span v-if="contaAPagarAtual.parcela.exibir">
                            {{ contaAPagarAtual.parcela.numero }}/{{ contaAPagarAtual.parcela.total }}
                        </span>
                    </td>
                    <td :style="{ color: contaAPagarAtual.corLinha }">
                        {{ contaAPagarAtual.valorVencimento | moeda }}
                    </td>
                    <td v-if="contaAPagarAtual.permissoes.editarDataVencimento" :style="{ color: contaAPagarAtual.corLinha }">
                        <campo-data-hora :data-hora.sync="contaAPagar.dataVencimento" :data-minima="Date.now()"></campo-data-hora>
                    </td>
                    <td v-else :style="{ color: contaAPagarAtual.corLinha }">
                        {{ contaAPagarAtual.datas.vencimento | data }}
                    </td>
                    <td :style="{ color: contaAPagarAtual.corLinha }">
                        {{ contaAPagarAtual.datas.cadastro | data }}
                    </td>
                    <td :style="{ color: contaAPagarAtual.corLinha }">
                        {{ contaAPagarAtual.boletoChegou ? "Sim" : "Não" }}
                    </td>
                    <td :style="{ color: contaAPagarAtual.corLinha }">
                        <input type="text" v-model="contaAPagar.observacao" />
                    </td>
                    <td :style="{ color: contaAPagarAtual.corLinha }">
                        {{ contaAPagarAtual.contaContabil }}
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
            <section class="links">
                <div>
                    <span>
                        <a href="#" @click.prevent="abrirRelatorio(false)">
                            <img border="0" src="../Images/printer.png"> Imprimir
                        </a>
                    </span>
                    <span>
                        <a href="#" @click.prevent="abrirRelatorio(true)">
                            <img border="0" src="../Images/Excel.gif"> Exportar para o Excel
                        </a>
                    </span>
                </div>
            </section>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/ContasPagar/Componentes/ListaContasPagar.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/ContasPagar/Componentes/ListaContasPagar.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>