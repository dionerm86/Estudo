<%@ Page Title="Pendências de Carregamento" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaPendenciaCarregamento.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaPendenciaCarregamento" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Carregamentos/Itens/Pendencias/Templates/ListaPendenciaCarregamento.Filtro.html")
    %>
    <div id="app">
        <pendencia-carregamento-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></pendencia-carregamento-filtros>
        <section>
              <lista-paginada ref="lista" :funcao-recuperar-itens="obter" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum carregamento pendente encontrado">
                <template slot="cabecalho">
                    <th>
                        Carregamento
                    </th>
                    <th>
                        Cliente
                    </th>
                    <th>
                        Peso Pendente
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td>
                        {{ item.id }}
                    </td>
                    <td v-if="configuracoes.controlarPedidosImportados && item.clienteExterno.id != 0">
                        {{ item.clienteExterno.id }} - {{ item.clienteExterno.nome }}
                    </td>
                    <td v-else>
                        {{ item.cliente.id }} - {{ item.cliente.nome }}
                    </td>
                    <td>
                        {{ item.peso }}
                    </td>
                    <td>
                        <a href="#" @click.prevent="abrirRelatorioCarregamentoPendente(item)">
                            <img src="../Images/Relatorio.gif" />
                        </a>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="link">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaCarregamentosPendentes(false)">
                        <img border="0" src="../Images/Printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaCarregamentosPendentes(true)">
                       <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>        
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Carregamentos/Itens/Pendencias/Componentes/ListaPendenciaCarregamento.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Carregamentos/Itens/Pendencias/Componentes/ListaPendenciaCarregamento.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
