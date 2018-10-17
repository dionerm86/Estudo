<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstSugestaoCliente.aspx.cs"
    EnableEventValidation="false" Inherits="Glass.UI.Web.Listas.LstSugestaoCliente" Title="Sugestões / Reclamações" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlTipoFuncionario.ascx" TagName="ctrlTipoFuncionario"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Clientes/Templates/LstSugestaoCliente.Filtro.html")
    %>
    <div id="app">
        <label v-if="verificarOrigemCliente()">
            {{ obterTitulo() }}
        </label>
        <sugestao-cliente-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></sugestao-cliente-filtros>
        <section v-if="configuracoes.cadastrarSugestaoCliente">
            <a :href="obterLinkInserirSugestao()">
                Inserir Sugestão
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma sugestão encontrada.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Cód.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('idPedido')">Pedido</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('rota')">Rota</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataCadastro')">Data Cadastro</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nomeFuncionario')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipo')">Tipo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricao')">Descrição.</a>
                    </th>
                    <th>
                        <a href="#">Situação</a>
                    </th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="cancelarSugestao(item)" title="Cancelar" v-if="item.permissoes.cancelar">
                            <img border="0" src="../Images/ExcluirGrid.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexos(item)" title="Anexos">
                            <img border="0" src="../Images/Clipe.gif">
                        </a>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.cliente.id }} - {{ item.cliente.nome }}</td>
                    <td>{{ item.idPedido }}</td>
                    <td>{{ item.rota }}</td>
                    <td>{{ item.cadastro.data | data }}</td>
                    <td>{{ item.cadastro.funcionario }}</td>
                    <td>{{ item.tipo }}</td>
                    <td>{{ item.descricao }}</td>
                    <td>{{ item.situacao }}</td>
                </template>
                <span>
                    <span>
                        <button @click.prevent="voltar">
                            Voltar
                        </button>
                    </span>
                </span>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaSugestoes(false)" title="Imprimir">
                        <img border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaSugestoes(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Clientes/Componentes/LstSugestaoCliente.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Clientes/Componentes/LstSugestaoCliente.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
