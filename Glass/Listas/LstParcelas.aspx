<%@ Page Title="Parcelas  " Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstParcelas.aspx.cs" Inherits="Glass.UI.Web.Listas.LstParcelas" EnableViewState="False" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <a :href="obterLinkInserirParcela()">
                Inserir Parcela
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhua parcela encontrada." :numero-registros="15">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('numParcelas')">Núm. Parcelas</a>
                    </th>
                    <th v-if="configuracoes.usarTabelaDescontoAcrescimoPedidoAVista">
                        <a href="#" @click.prevent="ordenar('desconto')">Desconto</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dias')">Dias</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('parcelaPadrao')">Exibir marcado como padrão?</a>
                    </th>
                    <th v-if="configuracoes.usarDescontoEmParcela" >
                        <a href="#" @click.prevent="ordenar('parcelaAVista')">Parcela á vista</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarParcela(item)">
                            <img src="../Images/EditarGrid.gif">
                        </a>
                        <button @click.prevent="excluir(item)" title="Excluir">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.numParcelas }}</td>
                    <td v-if="configuracoes.usarTabelaDescontoAcrescimoPedidoAVista">{{ item.desconto }}</td>
                    <td>{{ item.dias }}</td>
                    <td>{{ item.parcelaPadrao | indicaMarcado }}</td>
                    <td v-if="configuracoes.usarDescontoEmParcela">{{ item.parcelaAVista }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>
                        <log-alteracao tabela="Parcelas" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes && item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaParcelas(false)">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaParcelas(true)">
                        <img alt="" border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Parcelas/Componentes/LstParcelas.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
