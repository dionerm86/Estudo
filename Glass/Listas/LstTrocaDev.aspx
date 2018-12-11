<%@ Page Title="Troca/Devolução  " Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstTrocaDev.aspx.cs" Inherits="Glass.UI.Web.Listas.LstTrocaDev" EnableViewState="False" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Estoques/TrocasDevolucoes/Templates/LstTrocaDevolucao.Filtro.html")
    %>
    <div id="app">
        <trocas-devolucoes-filtros :filtro.sync="filtro" :configuracoes="configuracoes" :agrupar-por-funcionario.sync="agruparPorFuncionario"
            :agrupar-por-funcionario-associado.sync="agruparPorFuncionarioAssociado"></trocas-devolucoes-filtros>
        <section v-if="configuracoes.cadastrarTrocaDevolucao">
            <a :href="obterLinkInserirTrocaDevolucao()">
                Inserir troca/devolução
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhua troca/devolução encontrada." :numero-registros="15">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Cód</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('pedido')">Pedido</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('funcionario')">Func. Solic.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipo')">Tipo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('data')">Data</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataErro')">Data Erro</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('loja')">Loja</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('creditoGerado')">Crédito Gerado</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorExcedente')">Valor Excedente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('origem')">Origem</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('setor')">Setor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('observacao')">Obs</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('usuarioCadastro')">Func. Cad.</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarTrocaDevoluca(item)" title="Editar" v-if="item.permissoes.editar">
                            <img src="../Images/EditarGrid.gif">
                        </a>
                        <button @click.prevent="cancelar(item)" title="Cancelar" v-if="item.permissoes.editar">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                        <a href="#" @click.prevent="abrirRelatorioTrocaDevolucao(item)" title="Troca/Devolução">
                            <img border="0" src="../Images/Relatorio.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexosTrocaDevolucao(item)" title="Anexos">
                            <img border="0" src="../Images/Clipe.gif">
                        </a>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.idPedido }}</td>
                    <td>{{ item.funcionario }}</td>
                    <td>
                        <span v-if="item.cliente">
                            {{ item.cliente.id }} - {{ item.cliente.nome }}
                        </span>
                    </td>
                    <td>{{ item.tipo }}</td>
                    <td>{{ item.dataTrocaDevolucao | data }}</td>
                    <td>{{ item.dataErro | data }}</td>
                    <td>{{ item.loja }}</td>
                    <td>{{ item.creditoGerado | moeda }}</td>
                    <td>{{ item.valorExcedente | moeda}}</td>
                    <td>{{ item.situacao }}</td>
                    <td>{{ item.origem }}</td>
                    <td>{{ item.setor }}</td>
                    <td>{{ item.observacao }}</td>
                    <td>{{ item.usuarioCadastro }}</td>
                    <td>
                        <log-alteracao tabela="TrocaDev" :id-item="item.id" :atualizar-ao-alterar="false"
                            v-if="item.permissoes && item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaTrocaDevolucao(false)">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaTrocaDevolucao(true)">
                        <img alt="" border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaControlePerdasExternas()">
                        <img alt="" border="0" src="../Images/printer.png" /> Controle de perdas externas
                    </a>
                </span>
            </div>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Estoques/TrocasDevolucoes/Componentes/LstTrocaDevolucao.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Estoques/TrocasDevolucoes/Componentes/LstTrocaDevolucao.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
