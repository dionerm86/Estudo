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
                    <lista-selecao-id-valor :item-selecionado.sync="parcela.tipoPagto" :funcao-recuperar-itens="obterFormaPagto" required></lista-selecao-id-valor>
                </span>
                <span class="cabecalho">
                    <label>
                        Dias
                    </label>
                </span>
                <span class="colspan3">
                    <input type="text"/>
                <button v-on:click.prevent="iniciarCadastro" title="Novo produto...">
                    <img v-bind:src="caminhoRelativo('/Images/Insert.gif')">
                </button>
                </span>
                <span class="colspan4" style="margin-left: 8px">
                        <input id="parcelaPadrao" type="checkbox" v-model="parcela.parcelaPadrao" />
                        <label for="parcelaPadrao">
                            Exibir marcado como padrão?
                        </label>
                    </span>
                <span class="cabecalho">
                    <label>
                        Situação
                    </label>
                </span>
                <span>
                    <lista-selecao-id-valor :item-selecionado.sync="parcela.situacoes" :funcao-recuperar-itens="obterSituacoes" required></lista-selecao-id-valor>
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
