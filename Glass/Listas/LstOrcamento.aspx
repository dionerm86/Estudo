<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstOrcamento.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstOrcamento" Title="Orçamentos" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Orcamentos/Templates/LstOrcamentos.Filtro.html")
    %>
    <div id="app">
        <orcamento-filtros :filtro.sync="filtro"></orcamento-filtros>
        <section v-if="configuracoes.cadastrarOrcamento">
            <a :href="obterLinkInserirOrcamento()">
                Inserir Orçamento
            </a>
        </section>
        <section>
            <lista-paginada :funcao-recuperar-itens="atualizarOrcamentos" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum orçamento encontrado">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Num.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('idProjeto')">Projeto</a>
                    </th>
                    <th v-if="configuracoes.exibirColunaIdPedidoEspelho">
                        <a href="#" @click.prevent="ordenar('idPedidoEspelho')">Pedido conf.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('idVendedor')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('telefoneOrcamento')">Tel. Res.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('total')">Total</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataCadastro')">Data</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarOrcamento(item)" title="Editar" v-if="item.permissoes.editar">
                            <img border="0" src="../Images/EditarGrid.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorio(item)" title="Imprimir" v-if="item.permissoes.imprimir">
                            <img border="0" src="../Images/Relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexosMedicao(item)" title="Anexos medições" v-if="item.idsMedicao && item.idsMedicao.length > 0">
                            <img border="0" src="../Images/Fotos.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorioMemoriaCalculo(item)" title="Memória de cálculo" v-if="item.permissoes.imprimirMemoriaCalculo">
                            <img border="0" src="../Images/calculator.gif">
                        </a>
                        <a href="#" @click.prevent="abrirRelatorioProjeto" title="Projeto" v-if="item.permissoes.imprimirProjeto">
                            <img border="0" src="../Images/clipboard.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexos(item)" title="Anexos orçamento">
                            <img border="0" src="../Images/Clipe.gif">
                        </a>
                        <a :href="obterLinkSugestoes(item)" title="Sugestões" v-if="item.permissoes.cadastrarSugestao">
                            <img border="0" src="../Images/Nota.gif">
                        </a>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.idProjeto }}</td>
                    <td v-if="configuracoes.exibirColunaIdPedidoEspelho">{{ item.idPedidoEspelho }}</td>
                    <td v-if="item.cliente.id">{{ item.cliente.id }} - {{ item.cliente.nome }}</td>
                    <td v-else>{{ item.cliente.nome }}</td>
                    <td>{{ item.vendedor.nome }}</td>
                    <td>{{ item.telefoneOrcamento }}</td>
                    <td>{{ item.total | moeda }}</td>
                    <td>{{ item.dataCadastro | data }}</td>
                    <td>{{ item.situacao }}</td>
                    <td style="white-space: nowrap">
                        <log-alteracao tabela="Orcamento" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                        <a href="#" @click.prevent="gerarPedido(item)" v-if="item.permissoes.gerarPedido">
                            Gerar Pedido
                        </a>
                        <a href="#" @click.prevent="enviarEmail(item)" title="Enviar e-mail do orçamento" v-if="item.permissoes.enviarEmail">
                            <img border="0" src="../Images/email.png">
                        </a>
                    </td>
                </template>
            </lista-paginada>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Orcamentos/Componentes/LstOrcamentos.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Orcamentos/Componentes/LstOrcamentos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
