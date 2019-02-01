<%@ Page Title="Acerto de Cheques Devolvidos/Abertos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstAcertoCheque.aspx.cs" Inherits="Glass.UI.Web.Listas.LstAcertoCheque" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
           "~/Vue/AcertosCheques/Templates/LstAcertosCheques.Filtro.html")
    %>
    <div id="app">
        <acertos-cheques-filtros :filtro.sync="filtro"></acertos-cheques-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Não existem acertos de cheques cadastrados">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Núm.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('funcionario')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('data')">Data</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valor')">Valor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('observacao')">Observação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td>
                        <button @click.prevent="abrirRelatorio(item)">
                            <img src="../Images/Relatorio.gif"/>
                        </button>
                        <button @click.prevent="cancelar(item)" v-if="item.permissoes.cancelar">
                            <img src="../Images/ExcluirGrid.gif"/>
                        </button>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.funcionario }}</td>
                    <td>{{ item.data | data }}</td>
                    <td>{{ item.valor | moeda }}</td>
                    <td>{{ item.observacao }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>
                        <div>
                            <log-cancelamento tabela="AcertoCheque" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logCancelamento"></log-cancelamento>
                        </div>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaAcertosCheques(false)" title="Imprimir">
                        <img src="../images/Printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaAcertosCheques(true)" title="Exportar para o Excel">
                        <img src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/AcertosCheques/Componentes/LstAcertosCheques.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/AcertosCheques/Componentes/LstAcertosCheques.js" />
        </Scripts>            
    </asp:ScriptManager>
</asp:Content>