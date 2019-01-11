<%@ Page Title="Exportação de Pedidos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstPedidoExportacao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPedidoExportacao"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Exportacao/Lista/Templates/LstPedidosExportacao.Filtro.html")
    %>
    <div id="app">
        <pedidos-exportacao-filtros :filtro.sync="filtro"></pedidos-exportacao-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma exportação de pedido encontrada.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('IdExportacao')">Cód.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('NomeFornec')">Fornecedor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('NomeFunc')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('DataExportacao')">Data Exportação</a>
                    </th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td>
                        <a href="#" @click.prevent="abrirRelatorio(item.id)">
                            <img src="../Images/Relatorio.gif" border="0">
                        </a>
                        <a href="#" @click.prevent="obterSituacaoPedidoExportacao(item.id)">
                            <img src="../Images/Pesquisar.gif" border="0">
                        </a>                        
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.fornecedor }}</td>
                    <td>{{ item.funcionario }}</td>
                    <td>{{ item.dataExportacao }}</td>
                </template>
            </lista-paginada>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Exportacao/Lista/Componentes/LstPedidosExportacao.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Exportacao/Lista/Componentes/LstPedidosExportacao.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
