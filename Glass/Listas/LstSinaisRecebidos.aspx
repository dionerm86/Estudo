<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstSinaisRecebidos.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstSinaisRecebidos" Title="Sinais Recebidos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Sinais/Templates/LstSinais.Filtro.html")
    %>
    <div id="app">
        <sinais-filtros :filtro.sync="filtro"></sinais-filtros>
        <section>
            <a :href="obterLinkPagarSinal()" v-if="pagamentoAntecipado">
                Efetuar Pagamento Antecipado
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum registro encontrado.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">
                            <template v-if="!pagamentoAntecipado">
                                Sinal
                            </template>
                            <template v-else>
                                Pagto. Antecipado
                            </template>
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th>
                        Valor
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataCadastro')">Data Rec.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('observacao')">Observação</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="abrirCancelamento(item)" title="¨Cancelar" v-if="item.permissoes.cancelar">
                            <img border="0" src="../Images/ExcluirGrid.gif">
                        </a>
                        <a href="#" @click.prevent="abrirImpressaoSinal(item)" title="¨Visualizar dados do recebimento">
                            <img border="0" src="../Images/relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexos(item)" title="¨Anexos">
                            <img border="0" src="../Images/Clipe.gif">
                        </a>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>
                        <template v-if="item.cliente && item.cliente.id">
                            {{ item.cliente.id }} - {{ item.cliente.nome }}
                        </template>
                    </td>
                    <td>{{ item.total | moeda }}</td>
                    <td>{{ item.dataCadastro | data }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>{{ item.observacao }}</td>
                    <td style="white-space: nowrap">
                        <log-alteracao tabela="Sinal" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                        <log-cancelamento tabela="Sinal" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logCancelamento"></log-cancelamento>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaSinais(false)" title="Imprimir">
                        <img border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaSinais(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Sinais/Componentes/LstSinais.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Sinais/Componentes/LstSinais.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
