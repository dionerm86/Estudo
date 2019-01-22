<%@ Page Title="Chapas Disponíveis" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstChapasDisponiveis.aspx.cs" Inherits="Glass.UI.Web.Listas.LstChapasDisponiveis"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Produtos/MateriaPrima/ChapasDisponiveis/Templates/LstChapasDisponiveis.Filtro.html")
    %>
    <div id="app">
        <chapas-disponiveis-filtros :filtro.sync="filtro"></chapas-disponiveis-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" mensagem-lista-vazia="Nenhuma chapa disponível encontrada.">
                <template slot="cabecalho">
                    <th>
                        Cor
                    </th>
                    <th>
                        Espessura
                    </th>
                    <th>
                        Fornecedor
                    </th>
                    <th>
                        NF-e
                    </th>
                    <th>
                        Lote
                    </th>
                    <th>
                        Produto
                    </th>
                    <th>
                        Etiqueta
                    </th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td>{{ item.cor }}</td>
                    <td>{{ item.espessura }}</td>
                    <td>{{ item.fornecedor }}</td>
                    <td>{{ item.numeroNotaFiscal }}</td>
                    <td>{{ item.lote }}</td>
                    <td>{{ item.produto }}</td>
                    <td>{{ item.codigoEtiqueta }}</td>
                </template>
            </lista-paginada>
        </section>
        <div style="text-align: center">
            <span style="color: red">
                São consideradas todas as chapas com etiquetas impressas e não cortadas/expedidas. 
            </span>
        </div>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(false)" title="Imprimir">
                        <img src="../Images/Printer.png"/> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(true)" title="Exportar para o Excel">
                        <img src="../Images/Excel.gif"/> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="false">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Produtos/MateriaPrima/ChapasDisponiveis/Componentes/LstChapasDisponiveis.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Produtos/MateriaPrima/ChapasDisponiveis/Componentes/LstChapasDisponiveis.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
    

