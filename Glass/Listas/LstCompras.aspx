
<%@ Page Title="Compras" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstCompras.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCompras" 
    EnableViewState="false" EnableViewStateMac="false"%>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Compras/Templates/LstCompras.Filtro.html")
    %>

    <div id="app">
        <compras-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></compras-filtros>
        <section>
            <a :href="obterLinkInserirCompra()" v-if="configuracoes.controleFinanceiroPagamento">
                Nova Compra
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma compra encontrada.">
                <template slot="cabecalho" :numero-registros="15">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('num')">Num</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cotacao')">Cota��o</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('pedido')">Pedido</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('fornecedor')">Fornecedor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('loja')">Loja</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('funcionario')">Funcion�rio</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('total')">Total</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataEntradaFabrica')">Data Ent. F�brica</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('pagto')">Pagto</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('data')">Data</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situa��o</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('contabil')">Contabil</a>
                    </th>
                    <th></th>
                    <th></th>             
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarCompra(item.id)" title="Editar" v-if="item.permissoes.editar">
                            <img src="../Images/Edit.gif">
                        </a>
                        <button @click.prevent="abrirRelatorioCompra(item, false)" title="Visualizar dados da compra">
                            <img src="../Images/Relatorio.gif" />
                        </button>
                        <button @click.prevent="abrirRelatorioCompra(item, true)" title="Exportar dados da compra para o excel">
                            <img src="../Images/Excel.gif" />
                        </button>
                        <button @click.prevent="cancelar(item)" v-if="item.permissoes.cancelar" title="Cancelar compra">
                            <img src="../Images/ExcluirGrid.gif" />
                        </button>
                        <button @click.prevent="abrirGerenciamentoDeFotos(item)" v-if="item.permissoes.gerenciarFotos" title="Gerenciar fotos">
                            <img src="../Images/Clipe.gif" />
                        </button>
                        <button @click.prevent="gerarNotaFiscal(item)" v-if="item.permissoes.gerarNotaFiscal" title="Gerar NF de entrada" >
                            <img src="../Images/script_go.gif" />
                        </button>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>
                        <span v-if="item.idCotacaoCompra">
                            {{ item.idCotacaoCompra }}
                        </span>
                    </td>
                    <td>
                        <span v-if="item.idsPedido">
                            {{ item.idsPedido }}
                        </span>
                        <span v-if="item.idPedidoEspelho">
                            {{ item.idPedidoEspelho }}
                        </span>
                    </td>
                    <td>{{ item.fornecedor.id }} - {{ item.fornecedor.nome }}</td>
                    <td>{{ item.loja }}</td>
                    <td>{{ item.usuarioCadastro }}</td>
                    <td>{{ item.total | moeda }}</td>
                    <td>{{ item.datas.fabrica | data }}</td>
                    <td style="white-space: nowrap">{{ item.tipo }}</td>
                    <td>{{ item.datas.cadastro | data }}</td>
                    <td>{{ item.situacao }}</td>
                    <td style="text-align: center">{{ item.contabil | indicaMarcado }}</td>
                    <td style="white-space: nowrap">
                        <div>
                            <controle-tooltip titulo="Nota fiscal gerada:" v-if="item.permissoes.exibirNotasFiscaisGeradas">
                                <template slot="botao">
                                    <img src="../Images/blocodenotas.png" />
                                </template>

                                <div>
                                    {{ item.numeroNotaFiscal }}
                                </div>
                            </controle-tooltip>                        
                            <button @click.prevent="reabrir(item)" title="Reabrir compra" v-if="item.permissoes.reabrir">
                                <img src="../Images/cadeado.gif">
                            </button>                        
                            <img src="/Images/basket_add.gif" title="Estoque creditado" v-if="item.estoqueCreditado">
                        </div>
                        <div>
                            <a href="#" @click.prevent="produtoChegou(item)" v-if="item.permissoes.exibirLinkProdutoChegou">
                                Produto Chegou
                            </a>
                        </div>
                        <div>
                            <a href="#" @click.prevent="finalizar(item)" title="Finalizar compra" v-if="item.permissoes.exibirFinalizacaoEntrega">
                                Finalizar
                            </a>
                        </div>
                    </td>
                    <td>
                        <log-alteracao tabela="Compra" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirRelatorioCompras(false)" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorioCompras(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Compras/Componentes/LstCompras.Filtro.js" /> 
            <asp:ScriptReference Path="~/Vue/Compras/Componentes/LstCompras.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
