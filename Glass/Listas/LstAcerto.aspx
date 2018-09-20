<%@ Page Title="Consulta de Acertos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="LstAcerto.aspx.cs" Inherits="Glass.UI.Web.Listas.LstAcerto" EnableViewState="false" EnableViewStateMac="false"  %>


<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Acertos/Templates/LstAcertos.Filtro.html")
    %>
    <div id="app">
        <acertos-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></acertos-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum acerto encontrado.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Acerto</a>
                    </th>
                    <th>
                        Referência
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('funcionario')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('total')">Total</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataCadastro')">Data</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('observacao')">Observação</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="abrirCancelamento(item)" title="¨Cancelar">
                            <img border="0" src="../Images/ExcluirGrid.gif">
                        </a>
                        <a href="#" @click.prevent="abrirImpressaoAcerto(item)" title="¨Visualizar dados do Acerto">
                            <img border="0" src="../Images/relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="abrirImpressaoNotasPromissorias(item)" title="¨Nota promissória">
                            <img border="0" src="../Images/Nota.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexos(item)" title="¨Anexos">
                            <img border="0" src="../Images/Clipe.gif">
                        </a>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.referencia }}</td>
                    <td>
                        <template v-if="item.cliente && item.cliente.id">
                            {{ item.cliente.id }} - {{ item.cliente.nome }}
                        </template>
                    </td>
                    <td>{{ item.nomeFuncionario }}</td>
                    <td>{{ item.total | moeda }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>{{ item.dataCadastro | data }}</td>
                    <td>{{ item.observacao }}</td>
                    <td style="white-space: nowrap">
                        <log-cancelamento tabela="Acerto" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logCancelamento"></log-cancelamento>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div style="color: blue">
                Os acertos em azul foram renegociados.
            </div>
            <div style="color: gold">
                Os acertos em amarelo foram enviados para Jurídico/Cartório.
            </div>
            <div>
                Os acertos enviados para Jurídico/Cartório que também foram renegociados permaneceram em azul, para diferenciar utilize o filtro de Jurídico/Cartório.
            </div>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaAcertos(false)" title="Imprimir">
                        <img border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaAcertos(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Acertos/Componentes/LstAcertos.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Acertos/Componentes/LstAcertos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
