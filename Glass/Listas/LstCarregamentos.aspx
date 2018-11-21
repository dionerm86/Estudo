<%@ Page Title="Carregamentos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstCarregamentos.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCarregamentos" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Carregamentos/Templates/LstCarregamentos.Filtro.html",
            "~/Vue/Carregamentos/Templates/LstCarregamentos.OrdensCarga.html")
    %>
    <div id="app">
        <carregamentos-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></carregamentos-filtros>
        <section>
            <a :href="obterLinkGerarCarregamento()">
                Gerar carregamento
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum carregamento encontrado."
                :linha-editando="numeroLinhaEdicao" v-on:atualizou-itens="atualizouItens">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        Carregamento
                    </th>
                    <th>
                        Motorista
                    </th>
                    <th>
                        Veículo
                    </th>
                    <th>
                        Data prev. saída
                    </th>
                    <th>
                        Loja
                    </th>
                    <th>
                        Situação
                    </th>
                    <th>
                        Rotas
                    </th>
                    <th>
                        Peso
                    </th>
                    <th>
                        Total
                    </th>
                    <th>
                        Situação faturamento
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="editar(item, index)" title="Editar" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Edit.gif">
                        </button>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                        <button v-on:click.prevent="alternarExibicaoOrdensCarga(index)" v-if="!inserindo && numeroLinhaEdicao === -1 && !exibindoOrdensCarga(index)">
                            <img src="../../Images/mais.gif" title="Exibir ordens de carga" />
                        </button>
                        <button v-on:click.prevent="alternarExibicaoOrdensCarga(index)" v-if="!inserindo && numeroLinhaEdicao === -1 && exibindoOrdensCarga(index)">
                            <img src="../../Images/menos.gif" title="Esconder ordens de carga" />
                        </button>
                    </td>
                    <td>{{ item.id }}</td>
                    <td v-if="item.motorista">{{ item.motorista.nome }}</td>
                    <td v-if="item.veiculo">{{ item.veiculo.nome }}</td>
                    <td>{{ item.dataPrevisaoSaida | dataHora }}</td>
                    <td>{{ item.loja }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>{{ obterRotas(item) }}</td>
                    <td>{{ item.peso }}</td>
                    <td>{{ item.valorTotalPedidos | moeda }}</td>
                    <td>{{ item.situacaoFaturamento }}</td>
                    <td style="white-space: nowrap">
                        <button @click.prevent="faturar(item)" title="Faturar carregamento" v-if="!inserindo && numeroLinhaEdicao === -1 && item.permissoes.faturarCarregamento">
                            <img src="../Images/Faturamento.gif">
                        </button>
                        <button @click.prevent="exibirFaturamento(null, item)" title="Imprimir faturamento" v-if="!inserindo && numeroLinhaEdicao === -1 && item.permissoes.imprimirFaturamento">
                            <img src="../Images/printer.png">
                        </button>
                        <button @click.prevent="abrirRelatorioCarregamento(item, false)" title="Visualizar carregamento" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Relatorio.gif">
                        </button>
                        <button @click.prevent="abrirRelatorioCarregamento(item, true)" title="Visualizar carregamento" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Excel.gif">
                        </button>                        
                        <log-alteracao tabela="Carregamento" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                        <log-cancelamento tabela="OrdemCarga" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logCancelamento"></log-cancelamento>
                    </td>
                </template>
                <template slot="itemEditando">
                    <td style="white-space: nowrap">
                        <button @click.prevent="atualizar" title="Atualizar">
                            <img src="../Images/ok.gif">
                        </button>
                        <button @click.prevent="cancelar" title="Cancelar">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ carregamento.id }}</td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="motoristaAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensFiltroMotoristas"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="veiculoAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensFiltroVeiculos"
                            v-bind:ordenar="false" campo-id="codigo" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <campo-data-hora :data-hora.sync="carregamento.dataPrevisaoSaida" :exibir-horas="true" required></campo-data-hora>
                    </td>
                    <td>{{ carregamento.loja }}</td>
                    <td>{{ carregamento.situacao }}</td>
                    <td>{{ carregamento.rotas }}</td>
                    <td>{{ carregamento.peso }}</td>
                    <td>{{ carregamento.valorTotalPedidos | moeda }}</td>
                    <td>{{ carregamento.situacaoFaturamento }}</td>
                    <td></td>
                </template>
                <template slot="novaLinhaItem" slot-scope="{ item, index, classe }" v-if="exibindoOrdensCarga(index)">
                    <tr v-bind:class="classe" style="border-top: none">
                        <td></td>
                        <td v-bind:colspan="numeroColunasLista() - 1">
                            <a href="#" @click.prevent="abrirInclusaoOrdemCarga(item)">
                                Adicionar OC
                            </a>
                            <carregamentos-ordens-carga v-bind:filtro="{ idCarregamento: item.id, item }" v-bind:configuracoes="configuracoes"></carregamentos-ordens-carga>
                        </td>
                    </tr>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaCarregamentos(false)" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaCarregamentos(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Carregamentos/Componentes/LstCarregamentos.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Carregamentos/Componentes/LstCarregamentos.OrdensCarga.js" />
            <asp:ScriptReference Path="~/Vue/Carregamentos/Componentes/LstCarregamentos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
