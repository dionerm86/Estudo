<%@ Page Title="Compra de Mercadoria" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstCompraPcp.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstCompraPcp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
                "~/Vue/Compras/Mercadorias/Templates/LstComprasMercadorias.Filtro.html")
    %>
    <div id="app">
        <compras-mercadorias-filtros :filtro.sync="filtro"></compras-mercadorias-filtros>
        <section>
            <a :href="obterLinkInserirCompraMercadoria()">
                Nova Compra
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Não há compras cadastradas.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('num')">Num</a>
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
                        <a href="#" @click.prevent="ordenar('funcionario')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('total')">Total</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('pagto')">Pagto</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('data')">Data</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('contabil')">Contábil</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarCompraMercadoria(item.id)" title="Editar" v-if="item.permissoes.editar">
                            <img src="../Images/EditarGrid.gif" />
                        </a>
                        <button @click.prevent="abrirRelatorioComprasMercadorias(item)" title="Visualizar dados da compra">
                            <img src="../Images/relatorio.gif" />
                        </button>
                        <button @click.prevent="abrirGerenciamentoDeFotos(item)" title="Gerenciar Fotos">
                            <img src="../Images/Clipe.gif"/>
                        </button>
                        <button @click.prevent="cancelar(item)" title="Cancelar">
                            <img src="../Images/ExcluirGrid.gif" />
                        </button>
                        <button @click.prevent="gerarNotaFiscal(item)" v-if="item.permissoes.gerarNotaFiscal" title="Gerar NF de entrada">
                            <img src="../Images/script_go.gif" />
                        </button>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.idPedido }}</td>
                    <td>{{ item.fornecedor }}</td>
                    <td>{{ item.loja }}</td>
                    <td>{{ item.usuarioCadastro }}</td>
                    <td>{{ item.total | moeda }}</td>
                    <td>{{ item.tipoCompra }}</td>
                    <td>{{ item.dataCadastro | data }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>{{ item.contabil | indicaMarcado }}</td>
                    <td style="white-space: nowrap">
                        <div>
                            <button @click.prevent="reabrir(item)" title="Reabrir Compra" v-if="item.permissoes.reabrir">
                                <img src="../Images/cadeado.gif"/>
                            </button>
                            <img src="../Images/basket_add.gif" title="Estoque Creditado" v-if="item.estoqueCreditado"/>
                        </div>
                        <div>
                            <a href="#" @click.prevent="produtoChegou(item)" v-if="item.permissoes.exibirProdutoChegou">
                                Produto chegou
                            </a>
                        </div>
                    </td>
                </template>
            </lista-paginada>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Compras/Mercadorias/Componentes/LstComprasMercadorias.Filtro.js" /> 
            <asp:ScriptReference Path="~/Vue/Compras/Mercadorias/Componentes/LstComprasMercadorias.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
