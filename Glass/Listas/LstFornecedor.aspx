<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstFornecedor.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstFornecedor" Title="Fornecedores" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Fornecedores/Templates/LstFornecedores.Filtro.html")
    %>
    <div id="app">
        <fornecedor-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></fornecedor-filtros>
        <section>
            <a :href="obterLinkInserirFornecedor()" v-if="configuracoes && configuracoes.cadastrarFornecedor">
                Inserir fornecedor
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum fornecedor encontrado" :numero-registros="10">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Código</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nomeFantasia')">Nome fantasia</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cpfCnpj')">CPF/CNPJ</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('rgInscricaoEstadual')">RG/Insc. Est.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataUltimaCompra')">Ult. Compra</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('telefoneContato')">Tel. Cont.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('vendedor')">Vendedor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('celularVendedor')">Cel. Vend.</a>
                    </th>
                    <th v-if="configuracoes && configuracoes.usarCreditoFornecedor">
                        <a href="#" @click.prevent="ordenar('credito')">Crédito</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarFornecedor(item)" v-if="configuracoes && configuracoes.cadastrarFornecedor">
                            <img src="../Images/Edit.gif">
                        </a>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="configuracoes && configuracoes.cadastrarFornecedor">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                        <button @click.prevent="abrirPrecosFornecedor(item)" title="Preço de produto por fornecedor">
                            <img src="../Images/dinheiro.gif">
                        </button>
                        <button @click.prevent="abrirAnexos(item)" title="Anexos" v-if="configuracoes && configuracoes.anexarArquivosFornecedor">
                            <img src="../Images/Clipe.gif">
                        </button>
                        <button @click.prevent="alterarSituacao(item)" title="Alterar situação" v-if="configuracoes && configuracoes.ativarInativarFornecedor">
                            <img src="../Images/Inativar.gif">
                        </button>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.nomeFantasia }}</td>
                    <td>{{ item.cpfCnpj }}</td>
                    <td>{{ item.rgInscricaoEstadual }}</td>
                    <td>{{ item.dataUltimaCompra | data }}</td>
                    <td>{{ item.telefoneContato }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>{{ item.vendedor.nome }}</td>
                    <td>{{ item.vendedor.celular }}</td>
                    <td v-if="configuracoes && configuracoes.usarCreditoFornecedor">{{ item.credito | moeda }}</td>
                    <td style="white-space: nowrap">
                        <button @click.prevent="abrirFichaFornecedor(item)" title="Imprimir ficha">
                            <img src="../Images/printer.png">
                        </button>
                        <log-alteracao tabela="Fornecedor" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaFornecedores(false, false)" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaFornecedores(false, true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaFornecedores(true, false)" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir ficha
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaFornecedores(true, true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar ficha para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Fornecedores/Componentes/LstFornecedores.js" />
            <asp:ScriptReference Path="~/Vue/Fornecedores/Componentes/LstFornecedores.Filtro.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
