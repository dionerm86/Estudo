<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstSugestaoCliente.aspx.cs"
    EnableEventValidation="false" Inherits="Glass.UI.Web.Listas.LstSugestaoCliente" Title="Sugestões / Reclamações"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Clientes/Sugestoes/Templates/LstSugestaoCliente.Filtro.html")
    %>
    <div id="app">
        <label v-if="cliente">
            Cliente: {{ cliente.nome }}
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
                            <img src="../Images/ExcluirGrid.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexos(item)" title="Anexos">
                            <img src="../Images/Clipe.gif">
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
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <button @click.prevent="voltar">
                        Voltar
                    </button>
                </span>
            </div>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaSugestoes(false)" title="Imprimir">
                        <img src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaSugestoes(true)" title="Exportar para o Excel">
                        <img src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Clientes/Sugestoes/Componentes/LstSugestaoCliente.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Clientes/Sugestoes/Componentes/LstSugestaoCliente.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
