<%@ Page Title="Consulta de Pagamentos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstPagto.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPagto"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Pagamentos/Templates/LstPagamentos.Filtro.html")
    %>

    <div id="app">
        <pagamento-filtros :filtro.sync="filtro"></pagamento-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum pagamento encontrado.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('IdPagto')">Num. Pagto</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('DescrFormaPagto')">Forma Pagto</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('Desconto')">Desconto</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('Obs')">Obs</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('ValorPago')">Total</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('DataPagto')">Data</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('DescrSituacao')">Situação</a>
                    </th>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="abrirRelatorio(item)" title="Visualizar dados do pagamento">
                            <img border="0" src="../Images/Relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="abrirGerenciamentoDeFotosAnexas(item)" title="Gerenciar fotos anexas">
                            <img border="0" src="../Images/Clipe.gif" />
                        </a>
                        <a href="#" @click.prevent="abrirMotivoCancelamento(item)" title="Cancelar pagamento" v-if="item.permissoes.cancelar">
                            <img border="0" src="../Images/ExcluirGrid.gif" />
                        </a>
                    </td>
                    <td>
                        {{ item.id }}
                    </td>
                    <td>
                        {{ item.formaPagamento }}
                    </td>
                    <td>
                        {{ item.desconto | moeda }}
                    </td>
                    <td>
                        {{ item.observacao }}
                    </td>
                    <td>
                        {{ item.valorPagamento | moeda }}
                    </td>
                    <td>
                        {{ item.dataPagamento | data }}
                    </td>
                    <td>
                        {{ item.situacao }}
                    </td>
                    <td>
                        <log-alteracao tabela="Pagto" :id-item="item.id" :atualizar-ao-alterar="false" 
                            v-if="item.permissoes && item.permissoes.logAlteracoes"></log-alteracao>
                        <log-cancelamento tabela="Pagto" :id-item="item.id" :atualizar-ao-alterar="false"
                            v-if="item.permissoes.logCancelamento"></log-cancelamento>
                    </td>
                </template>
            </lista-paginada>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Pagamentos/Componentes/LstPagamentos.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Pagamentos/Componentes/LstPagamentos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
