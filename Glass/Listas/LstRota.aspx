<%@ Page Title="Rotas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstRota.aspx.cs" Inherits="Glass.UI.Web.Listas.LstRota" EnableViewState="False" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <a :href="obterLinkInserirRota()" v-if="configuracoes && configuracoes.cadastrarRota">
                Inserir Rota
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhua rota encontrada." :numero-registros="15">
                <template slot="cabecalho">
                    <th v-if="configuracoes && configuracoes.cadastrarRota"></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigo')">Código</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('distancia')">Distância</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('observacao')">Observação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('diasSemana')">Dias da rota</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('numeroDiasMinimosParaDataEntrega')">
                            Mínimo dias entrega
                            <label v-if="configuracoes && configuracoes.usarDiasCorridosCalculoRota">(corridos)</label>
                            <label v-else>(úteis)</label>
                        </a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap" v-if="configuracoes && configuracoes.cadastrarRota">
                        <a :href="obterLinkEditarRota(item)">
                            <img src="../Images/EditarGrid.gif">
                        </a>
                        <button @click.prevent="excluir(item)" title="Excluir">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                        <button @click.prevent="abrirDadosRota(item)" title="Detalhes">
                            <img src="../Images/Relatorio.gif">
                        </button>
                    </td>
                    <td>{{ item.codigo }}</td>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>{{ item.distancia }}</td>
                    <td>{{ item.observacao }}</td>
                    <td>{{ item.diasSemana }}</td>
                    <td>{{ item.numeroDiasMinimosParaDataEntrega }}</td>
                    <td>
                        <log-alteracao tabela="Rota" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes && item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaRotas(false)">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaRotas(true)">
                        <img alt="" border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Rotas/Componentes/LstRotas.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
