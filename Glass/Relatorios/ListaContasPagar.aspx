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
                        <a href="#" @click.prevent="ordenar('IdContaAPagar')">
                            Cód.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('Referencia')">
                            Referência
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('Fornecedor/Transportador/Funcionario')">
                            Fornecedor/Transportador/Func.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('Referente')">
                            Referente a
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('FormaPagamento')">
                            Forma Pagto.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('Parcelas')">
                            Parc.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('Valor')">
                            Valor
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('Vencimento')">
                            Vencimento
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('DataCadastro')">
                            Data Cad.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('BoletoChegou')">
                            Boleto Chegou?
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('Obs')">
                            Obs
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('Tipo')">
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
                    <td>
                        {{ item.id }}
                    </td>
                    <td>
                        {{ item.referencia }}
                    </td>
                    <td v-if="item.fornecedor && item.fornecedor.id">
                        {{ item.fornecedor.nome }}
                    </td>
                    <td v-if="item.transportador && item.transportador.id">
                        {{ item.transportador.nome }}
                    </td>
                    <td>
                        {{ item.descricaoContaAPagar }}
                    </td>
                    <td>
                        {{ item.formaPagamento.nome }}
                    </td>
                    <td>
                        <span v-if="item.parcela.exibir">
                            {{ item.parcela.numero }}/{{ item.parcela.total }}
                        </span>
                    </td>
                    <td>
                        {{ item.valorVencimento | moeda }}
                    </td>
                    <td>
                        {{ item.datas.vencimento | data }}
                    </td>
                    <td>
                        {{ item.datas.cadastro | data }}
                    </td>
                    <td>
                        {{ item.boletoChegou ? "Sim" : "Não" }}
                    </td>
                    <td>
                        {{ item.observacoes.contaAPagar }}
                        {{ item.observacoes.desconto || item.observacoes.acrescimo }}
                    </td>
                    <td>
                        {{ item.contaContabil }}
                    </td>
                    <td>
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
                    <td>
                        {{ contaAPagarAtual.id }}
                    </td>
                    <td>
                        {{ contaAPagarAtual.referencia }}
                    </td>
                    <td v-if="contaAPagarAtual.fornecedor && contaAPagarAtual.fornecedor.id">
                        {{ contaAPagarAtual.fornecedor.nome }}
                    </td>
                    <td v-if="contaAPagarAtual.transportador && contaAPagarAtual.transportador.id">
                        {{ contaAPagarAtual.transportador.nome }}
                    </td>
                    <td>
                        <lista-selecao-id-valor :funcao-recuperar-itens="obterPlanosConta"
                            :item-selecionado.sync="planoContaAtual"></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor :funcao-recuperar-itens="obterFormasPagamentoCompras"
                            :item-selecionado.sync="formaPagamentoAtual"></lista-selecao-id-valor>
                    </td>
                    <td>
                        <span v-if="contaAPagarAtual.parcela.exibir">
                            {{ contaAPagarAtual.parcela.numero }}/{{ contaAPagarAtual.parcela.total }}
                        </span>
                    </td>
                    <td>
                        {{ contaAPagarAtual.valorVencimento | moeda }}
                    </td>
                    <td v-if="contaAPagarAtual.permissoes.editarDataVencimento">
                        <campo-data-hora :data-hora.sync="contaAPagar.dataVencimento" :data-minima="Date.now()"></campo-data-hora>
                    </td>
                    <td v-else>
                        {{ contaAPagarAtual.datas.vencimento | data }}
                    </td>
                    <td>
                        {{ contaAPagarAtual.datas.cadastro | data }}
                    </td>
                    <td>
                        {{ contaAPagarAtual.boletoChegou ? "Sim" : "Não" }}
                    </td>
                    <td>
                        <input type="text" v-model="contaAPagar.observacao" />
                    </td>
                    <td>
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