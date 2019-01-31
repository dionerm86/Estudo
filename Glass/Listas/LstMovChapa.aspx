<%@ Page Title="Extrato de Mov. de Chapa de Vidro" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstMovChapa.aspx.cs" Inherits="Glass.UI.Web.Listas.LstMovChapa"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Produtos/MateriaPrima/Extrato/Totalizadores/Templates/LstMovimentacoesChapas.Filtro.html",
            "~/Vue/Produtos/MateriaPrima/Extrato/Totalizadores/Templates/LstMovimentacoesChapas.Movimentacoes.html")
    %>

    <div id="app">
        <movimentacoes-chapas-filtros :filtro.sync="filtro"></movimentacoes-chapas-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro"
                :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum registro encontrado." @atualizou-itens="atualizouItens">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        Cor
                    </th>
                    <th>
                        Espessura
                    </th>
                    <th>
                        Qtde. Utilizada
                    </th>
                    <th>
                        Qtde. Disponível
                    </th>
                    <th>
                        M² Lido
                    </th>
                    <th>
                        Sobra
                    </th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td>
                        <span v-if="!exibindoMovimentacoes(index)">
                            <a href="#" @click.prevent="alternarExibicaoMovimentacoes(index)">
                                <img src="../Images/mais.gif" />
                            </a>
                        </span>
                        <span v-if="exibindoMovimentacoes(index)">
                            <a href="#" @click.prevent="alternarExibicaoMovimentacoes(index)">
                                <img src="../Images/menos.gif" />
                            </a>
                        </span>
                    </td>
                    <td>
                        {{ item.corVidro.descricao }}
                    </td>
                    <td>
                        {{ item.espessura }} MM
                    </td>
                    <td>
                        {{ item.quantidades.utilizada }}
                    </td>
                    <td>
                        {{ item.quantidades.disponivel }}
                    </td>
                    <td>
                        {{ item.metroQuadradoLido | decimal }}
                    </td>
                    <td>
                        {{ item.sobra | decimal }}
                    </td>
                </template>
                <template slot="novaLinhaItem" slot-scope="{ item, index, classe }" v-if="exibindoMovimentacoes(index)">
                    <tr v-bind:class="classe" style="border-top: none">
                        <td v-bind:colspan="numeroColunasLista()">
                            <movimentacoes-chapas-movimentacoes :movimentacoes="item.extrato"></movimentacoes-chapas-movimentacoes>
                        </td>
                    </tr>
                </template>
            </lista-paginada>
        </section>
        <div style="text-align: center">
            <span style="color: red">
                Chapas em vermelho indicam leituras em dias diferentes.</br>
                Chapas em azul indicam revenda.</br>
                A quantidade disponível é baseada na inicial menos a utilizada.
            </span>
        </div>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(false, false)">
                        <img border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(true, false)">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
            <div>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(false, true)">
                        <img border="0" src="../Images/printer.png" /> Imprimir detalhado
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(true, true)">
                        <img border="0" src="../Images/Excel.gif" /> Exportar detalhado para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Produtos/MateriaPrima/Extrato/Totalizadores/Componentes/LstMovimentacoesChapas.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Produtos/MateriaPrima/Extrato/Totalizadores/Componentes/LstMovimentacoesChapas.Movimentacoes.js" />
            <asp:ScriptReference Path="~/Vue/Produtos/MateriaPrima/Extrato/Totalizadores/Componentes/LstMovimentacoesChapas.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
