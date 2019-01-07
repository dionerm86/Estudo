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
                mensagem-lista-vazia="Nenhum preço de tabela por cliente encontrado, certifique-se de ter informado um cliente.">
                <template slot="cabecalho">
                    <th>
                        <a href="#" @click.prevent="ordenar('codigo')">
                            Cód.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricao')">
                            Descrição
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('grupo')">
                            Grupo
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipoValorTabela')">
                            Tipo de Valor de Tabela
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorOriginal')">
                            Valor Original
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorTabela')">
                            Valor de Tabela
                        </a>
                    </th>
                    <th>
                        Desconto/Acréscimo
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('altura')">
                            Altura
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('largura')">
                            Largura
                        </a>
                    </th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td>
                        {{ item.produto }}
                    </td>
                    <td>
                        {{ item.beneficiamentos }}
                    </td>
                    <td>
                        {{ item.grupo }} {{ item.subgrupo }}
                    </td>
                    <td>
                        {{ item.tipoValorTabela }}
                    </td>
                    <td>
                        {{ item.valorOriginal | moeda }}
                    </td>
                    <td>
                        {{ item.valorTabela | moeda }}
                    </td>
                    <td>
                        {{ item.fatorDescontoAcrescimo }}%
                    </td>
                    <td>
                        {{ item.altura }}
                    </td>
                    <td>
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
