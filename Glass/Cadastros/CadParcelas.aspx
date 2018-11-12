<%@ Page Title="Parcelas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadParcelas.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadParcelas" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <div v-if="editando || inserindo">
            <section class="edicao">
                <span class="cabecalho">
                    <label>
                        Descrição
                    </label>
                </span>
                <span class="colspan3">
                    <input type="text" v-model="parcela.descricao" required />
                </span>
                <span class="cabecalho">
                    <label>
                        Tipo Pagto.
                    </label>
                </span>
                <span class="colspan3">
                    <lista-selecao-id-valor :item-selecionado.sync="formaPagamentoAtual" :funcao-recuperar-itens="obterFormaPagto" required></lista-selecao-id-valor>
                </span>
                <template v-if="exibirDias">
                    <span class="cabecalho">
                        <label>
                            Dias
                        </label>
                    </span>
                    <span class="colspan3">
                        <input type="number" v-model="diaAtual"/>
                        <button v-on:click.prevent="inserirDia" title="Inserir dia">
                            <img v-bind:src="caminhoRelativo('/Images/Insert.gif')">
                        </button>
                        <div v-for="dia in parcela.dias" style="padding: 4px 0">
                            <a href="#" @click.prevent="excluirDia(dia)" title="Remover">
                                <img border="0" src="../Images/ExcluirGrid.gif">
                            </a>
                            {{dia}}
                        </div>
                    </span>
                </template>
                <span class="cabecalho" v-if="configuracoes.usarDescontoEmParcela">
                    <label>
                        Desconto (%)
                    </label>
                </span>
                <span class="colspan3" v-if="configuracoes.usarDescontoEmParcela">
                    <input type="text" v-model="parcela.desconto" />
                </span>
                <span class="colspan4" style="padding: 4px 0">
                        <input v-model="parcela.parcelaPadrao" type="checkbox" />
                        <label for="parcelaPadrao">
                            Exibir marcado como padrão?
                        </label>
                </span>
                <span class="colspan4" style="padding: 4px 0" v-if="configuracoes.usarTabelaDescontoAcrescimoPedidoAVista">
                        <input id="parcelaAvista" type="checkbox" v-model="parcela.parcelaAvista" />
                        <label for="parcelaAvista">
                            Parcela à vista?
                        </label>
                </span>
                <span class="cabecalho">
                    <label>
                        Situação
                    </label>
                </span>
                <span>
                    <lista-selecao-situacoes v-bind:situacao.sync="situacaoAtual" required></lista-selecao-situacoes>
                </span>
                <span class="botoes">
                    <span>
                        <button @click.prevent="inserirParcela" v-if="inserindo">
                            Inserir
                        </button>
                        <button @click.prevent="atualizarParcela" v-if="editando">
                            Atualizar
                        </button>
                        <button @click.prevent="cancelar">
                            Cancelar
                        </button>
                    </span>
                </span>
            </section>
        </div>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Parcelas/Componentes/CadParcelas.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
