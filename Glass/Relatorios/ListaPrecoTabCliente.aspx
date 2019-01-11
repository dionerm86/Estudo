<%@ Page Title="Preços de Tabela por Cliente" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ListaPrecoTabCliente.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaPrecoTabCliente"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Produtos/PrecosTabelaCliente/Templates/ListaPrecoTabelaCliente.Filtro.html")
    %>

    <div id="app">
        <preco-tabela-cliente-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></preco-tabela-cliente-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" 
                mensagem-lista-vazia="Nenhum preço de tabela por cliente encontrado (Certifique-se de ter informado o cliente).">
                <template slot="cabecalho">
                    <th style="white-space: nowrap">
                        <a href="#" @click.prevent="ordenar('codigo')">
                            Cód.
                        </a>
                    </th>
                    <th style="white-space: nowrap">
                        <a href="#" @click.prevent="ordenar('descricao')">
                            Descrição
                        </a>
                    </th>
                    <th style="white-space: nowrap">
                        <a href="#" @click.prevent="ordenar('grupo')">
                            Grupo
                        </a>
                    </th>
                    <th v-if="!configuracoes.usarLiberacaoPedido" style="white-space: nowrap">
                        <a href="#" @click.prevent="ordenar('tipoValorTabela')">
                            Tipo de Valor de Tabela
                        </a>
                    </th>
                    <th v-if="filtro && !filtro.naoExibirColunaValorOriginal" style="white-space: nowrap">
                        <a href="#" @click.prevent="ordenar('valorOriginal')">
                            Valor Original
                        </a>
                    </th>
                    <th style="white-space: nowrap">
                        <a href="#" @click.prevent="ordenar('valorTabela')">
                            Valor de Tabela
                        </a>
                    </th>
                    <th v-if="filtro && filtro.exibirPercentualDescontoAcrescimo" style="white-space: nowrap">
                        Desconto/Acréscimo
                    </th>
                    <th style="white-space: nowrap">
                        <a href="#" @click.prevent="ordenar('altura')">
                            Altura
                        </a>
                    </th>
                    <th style="white-space: nowrap">
                        <a href="#" @click.prevent="ordenar('largura')">
                            Largura
                        </a>
                    </th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td style="white-space: nowrap">
                        {{ item.produto }}
                    </td>
                    <td style="white-space: nowrap">
                        {{ item.beneficiamentos }}
                    </td>
                    <td style="white-space: nowrap">
                        {{ item.grupo }} {{ item.subgrupo }}
                    </td>
                    <td v-if="!configuracoes.usarLiberacaoPedido" style="white-space: nowrap">
                        {{ item.tipoValorTabela }}
                    </td>
                    <td v-if="filtro && !filtro.naoExibirColunaValorOriginal" style="white-space: nowrap">
                        {{ item.valorOriginal | moeda }}
                    </td>
                    <td style="white-space: nowrap">
                        {{ item.valorTabela | moeda }}
                    </td>
                    <td v-if="filtro && filtro.exibirPercentualDescontoAcrescimo" style="white-space: nowrap">
                        {{ item.fatorDescontoAcrescimo }}%
                    </td>
                    <td style="white-space: nowrap">
                        {{ item.altura }}
                    </td>
                    <td style="white-space: nowrap">
                        {{ item.largura }}
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(false)">
                        <img src="../Images/Printer.png"> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(true)">
                        <img src="../Images/Excel.gif"> Exportar para o excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Produtos/PrecosTabelaCliente/Componentes/ListaPrecoTabelaCliente.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Produtos/PrecosTabelaCliente/Componentes/ListaPrecoTabelaCliente.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
