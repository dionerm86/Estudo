<%@ Page Title="Posição de Matéria-Prima" Language="C#" MasterPageFile="~/Painel.master"
        AutoEventWireup="true" CodeBehind="LstPosicaoMateriaPrima.aspx.cs"
        Inherits="Glass.UI.Web.Listas.LstPosicaoMateriaPrima" EnableViewState="false"
        EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Produtos/MateriaPrima/Posicao/Templates/LstPosicoesMateriaPrima.Filtro.html",
            "~/Vue/Produtos/MateriaPrima/Posicao/Templates/LstPosicoesMateriaPrima.Chapas.html")
    %>
    
    <div id="app">
        <posicoes-materia-prima-filtros :filtro.sync="filtro"></posicoes-materia-prima-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro"
                :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma posição de matéria prima encontrada."
                @atualizou-itens="atualizouItens">
                <template slot="cabecalho">
                    <th></th>
                    <th style="white-space: nowrap">
                        Cor
                    </th>
                    <th style="white-space: nowrap">
                        Espessura
                    </th>
                    <th style="white-space: nowrap">
                        Total M2
                    </th>
                    <th style="white-space: nowrap">
                        M2 Com Etiqueta Impressa
                    </th>
                    <th style="white-space: nowrap">
                        M2 Sem Etiqueta Impressa
                    </th>
                    <th style="white-space: nowrap">
                        M2 Pedido de Venda
                    </th>
                    <th style="white-space: nowrap">
                        M2 Pedido de Produção
                    </th>
                    <th style="white-space: nowrap">
                        M2 em Estoque
                    </th>
                    <th style="white-space: nowrap">
                        M2 Disponível
                    </th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td>
                        <span v-if="item.permissoes.exibirChapas && !exibindoChapas(index)">
                            <button @click.prevent="alternarExibicaoChapas(index)">
                                <img border="0" src="../Images/mais.gif" />
                            </button>
                        </span>
                        <span v-if="exibindoChapas(index)">
                            <button @click.prevent="alternarExibicaoChapas(index)">
                                <img border="0" src="../Images/menos.gif" />
                            </button>
                        </span>
                    </td>
                    <td style="text-align: center">
                        {{ item.corVidro.descricao }}
                    </td>
                    <td style="text-align: center">
                        {{ item.espessura | decimal }} MM
                    </td>
                    <td style="text-align: center">
                        {{ item.metroQuadrado.total | decimal }}
                    </td>
                    <td style="text-align: center">
                        {{ item.metroQuadrado.comEtiquetaImpressa | decimal }}
                    </td>
                    <td style="text-align: center">
                        {{ item.metroQuadrado.semEtiquetaImpressa | decimal }}
                    </td>
                    <td style="text-align: center">
                        {{ item.metroQuadrado.pedidoDeVenda | decimal }}
                    </td>
                    <td style="text-align: center">
                        {{ item.metroQuadrado.pedidoDeProducao | decimal }}
                    </td>
                    <td style="text-align: center; font-weight: bold">
                        {{ item.metroQuadrado.emEstoque | decimal }}
                    </td>
                    <td style="text-align: center; font-weight: bold">
                        {{ item.metroQuadrado.disponivel | decimal }}
                    </td>
                </template>
                <template slot="novaLinhaItem" slot-scope="{ item, index, classe }" v-if="exibindoChapas(index)">
                    <tr :class="classe">
                        <td :colspan="numeroColunasLista()">
                            <posicoes-materia-prima-chapas :filtro="{ idCorVidro: item.corVidro.id, espessura: item.espessura }">
                            </posicoes-materia-prima-chapas>
                        </td>
                    </tr>
                </template>
            </lista-paginada>
        </section>
        <div style="margin-top: 10px">
            <span style="color:red">
                <label>São consideradas todas as chapas de notas finalizadas (com ou sem etiqueta impressa)</label>
            </span>
        </div>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(false)" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Produtos/MateriaPrima/Posicao/Componentes/LstPosicoesMateriaPrima.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Produtos/MateriaPrima/Posicao/Componentes/LstPosicoesMateriaPrima.Chapas.js" />
            <asp:ScriptReference Path="~/Vue/Produtos/MateriaPrima/Posicao/Componentes/LstPosicoesMateriaPrima.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
