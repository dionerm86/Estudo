<%@ Page Title="Exportar Pedido" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstExportarPedido.aspx.cs" Inherits="Glass.UI.Web.Listas.LstExportarPedido"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Pedidos/Exportacao/Templates/LstExportarPedidos.Filtro.html",
            "~/Vue/Pedidos/Exportacao/Templates/LstExportarPedidos.Produtos.html")
    %>

    <div id="app">
        <exportacao-pedidos-filtros :filtro.sync="filtro"></exportacao-pedidos-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao"
                 mensagem-lista-vazia="Nenhum pedido para exportação encontrado.">
                <template slot="cabecalho">
                    <th>
                        <input type="checkbox" v-model="marcarTodosOsPedidosParaExportacao" />
                    </th>
                    <th></th>
                    <th>
                        Num
                    </th>
                    <th>
                        Proj.
                    </th>
                    <th>
                        Orça.
                    </th>
                    <th>
                        Cliente
                    </th>
                    <th>
                        Loja
                    </th>
                    <th>
                        Funcionário
                    </th>
                    <th>
                        Total
                    </th>
                    <th>
                        Pagto
                    </th>
                    <th>
                        Data
                    </th>
                    <th>
                        Entrega
                    </th>
                    <th>
                        Situação
                    </th>
                    <th>
                        Tipo
                    </th>
                    <th>
                        <input type="checkbox" />
                        Exportar Beneficiamentos (todos)?
                    </th>
                    <th>
                        Situação Exportação
                    </th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td>
                        <span>
                            <input type="checkbox" :value="item.id" v-model="pedidosExportar" />
                        </span>
                        <span v-if="item.permissoes.consultarSituacao">
                            <button @click.prevent="consultarSituacao(item.id)" title="Consultar situação">
                                <img border="0" src="../Images/Pesquisar.gif" />
                            </button>
                        </span>
                    </td>
                    <td>
                        <span v-if="configuracoes.exibirProdutos">
                            <span v-if="!exibindoProdutos(index)">
                                <button @click.prevent="alternarExibicaoProdutos(index)" title="Exibir Produtos">
                                    <img border="0" src="../Images/mais.gif" />
                                </button>
                            </span>
                            <span v-if="exibindoProdutos(index)">
                                <button @click.prevent="alternarExibicaoProdutos(index)" title="Esconder Produtos">
                                    <img border="0" src="../Images/menos.gif" />
                                </button>
                            </span>
                        </span>
                    </td>
                    <td>
                        {{ item.id }}
                    </td>
                    <td>
                        <span v-if="item && item.idProjeto">
                            {{ item.idProjeto }}
                        </span>
                    </td>
                    <td>
                        <span v-if="item && item.idOrcamento">
                            {{ item.idOrcamento }}
                        </span>
                    </td>
                    <td>
                        {{ item.cliente.id }} - {{ item.cliente.nome }}
                    </td>
                    <td>
                        {{ item.loja }}
                    </td>
                    <td>
                        {{ item.funcionario }}
                    </td>
                    <td>
                        {{ item.total | moeda }}
                    </td>
                    <td style="white-space: nowrap">
                        {{ item.tipoVenda }}
                    </td>
                    <td>
                        {{ item.datas.pedido | data }}
                    </td>
                    <td>
                        {{ item.datas.entrega | data }}
                    </td>
                    <td>
                        {{ item.situacoes.pedido }}
                    </td>
                    <td>
                        {{ item.tipoPedido }}
                    </td>
                    <td>
                        <input type="checkbox" :value="item.id" v-model="beneficiamentosExportar" :enabled="pedidosExportar.includes(item.id)" />
                        Exportar Beneficiamentos?
                    </td>
                    <td>
                        {{ item.situacoes.exportacao }}
                    </td>
                </template>
                <template slot="novaLinhaItem" slot-scope="{ item, index, classe }" v-if="exibindoProdutos(index)">
                    <tr>
                        <td></td>
                        <td :colspan="numeroColunasLista() - 1">
                            <exportacao-pedidos-produtos :filtro="{ idPedido: item.id }"></exportacao-pedidos-produtos>
                        </td>
                    </tr>
                </template>
            </lista-paginada>
            <section>
                <div>
                    <span class="form-group">
                        <label>Fornecedor</label>
                        <lista-selecao-id-valor :funcao-recuperar-itens="obterItensControleFornecedores"
                            :item-selecionado.sync="fornecedorAtual"></lista-selecao-id-valor>
                    </span>
                </div>
                <div style="margin-top: 5px">
                    <input type="button" value="Exportar Pedidos Selecionados" @click.prevent="exportar" />
                </div>
            </section>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Pedidos/Exportacao/Componentes/LstExportarPedidos.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Pedidos/Exportacao/Componentes/LstExportarPedidos.Produtos.js" />
            <asp:ScriptReference Path="~/Vue/Pedidos/Exportacao/Componentes/LstExportarPedidos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
