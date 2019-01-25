<%@ Page Title="Encontro de Contas a Pagar/Receber" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstEncontroContas.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEncontroContas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
      Glass.UI.Web.IncluirTemplateTela.Script(
    "~/Vue/EncontrosContas/Templates/LstEncontrosContas.Filtro.html")
        %>
    <div id="app">
        <encontros-contas-filtros :filtro.sync="filtro" ></encontros-contas-filtros>
        <section>
            <a :href="obterLinkInserirEncontrosContas()">
                Inserir encontro contas a pagar/receber
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum encontro de contas a pagar/receber encontrado.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cod')">Cód.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('fornecedor')">Fornec.</a>
                    </th>
                    <th>
                        Contas a pagar                    
                    </th>
                    <th>
                        Contas a receber
                    </th>
                    <th>
                        Saldo
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataCadastro')">Data de Cad.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('observacao')">Obs</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="editar(item)" title="Editar" v-if="item.permissoes.editar">
                            <img src="../Images/EditarGrid.gif"/>
                        </button>
                        <button @click.prevent="imprimirEncontrosContas(item)" title="Imprimir" v-if="item.permissoes.imprimir">
                            <img src="../Images/Relatorio.gif"/>
                        </button>
                        <button @click.prevent="cancelar(item)" title="Cancelar" v-if="item.permissoes.excluir">
                            <img src="../Images/ExcluirGrid.gif"/>
                        </button>
                    </td>
                    <td>{{ item.id }}</td>
                    <td v-if="item.cliente">{{ item.cliente.id }} - {{ item.cliente.nome }}</td>
                    <td v-if="item.fornecedor">{{ item.fornecedor.id }} - {{ item.fornecedor.nome }}</td>
                    <td v-if="item.valores">{{ item.valores.pagar | moeda }}</td>
                    <td v-if="item.valores">{{ item.valores.receber | moeda }}</td>
                    <td v-if="item.valores">{{ item.valores.saldo | moeda }}</td>
                    <td>{{ item.dataCadastro | dataHora }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>{{ item.observacao }}</td>
                    <td style="white-space: nowrap">
                        <log-cancelamento tabela="EncontroContas" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logCancelamento"></log-cancelamento>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaEncontrosContas(false)" title="Imprimir">
                        <img src="../Images/printer.png"/> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaEncontrosContas(true)" title="Exportar para o Excel">
                        <img src="../Images/Excel.gif"/> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/EncontrosContas/Componentes/LstEncontrosContas.Filtro.js"/>
            <asp:ScriptReference Path="~/Vue/EncontrosContas/Componentes/LstEncontrosContas.js"/>
        </Scripts>
    </asp:ScriptManager>
</asp:Content>