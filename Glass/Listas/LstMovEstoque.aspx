<%@ Page Title="Extrato de Estoque" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstMovEstoque.aspx.cs" Inherits="Glass.UI.Web.Listas.LstMovEstoque"
    EnableViewState="false" EnableViewStateMac="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Estoques/Movimentacoes/Reais/Templates/ListaMovimentacoesEstoqueReal.Filtro.html")
    %>
    <div id="app">
        <movimentacoesestoquereal-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></movimentacoesestoquereal-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum estoque de produto encontrado." :exibir-inclusao="listaNaoVazia" @atualizou-itens="atualizouItens">
                <template slot="cabecalho">
                    <th></th>
                    <th>Cód. Mov.</th>
                    <th>Referência</th>
                    <th>Produto</th>
                    <th>Fornecedor</th>
                    <th>Funcionário</th>
                    <th>Data</th>
                    <th>Qtde.</th>
                    <th>Unidade</th>
                    <th>Saldo</th>
                    <th>E/S</th>
                    <th>Valor (Total)</th>
                    <th>Valor Acumulado</th>                    
                    <th>Obs.</th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td>
                        <a href="#" v-show="item.permissoes.excluir" title="Excluir Movimentação">
                            <img src="../Images/ExcluirGrid.gif" @click.prevent="excluiMovimentacaoEstoqueReal(item)" />
                        </a>
                    </td>
                    <td v-bind:style="{ color: item.corLinha }">
                        {{ item.id }}
                    </td>
                    <td v-bind:style="{ color: item.corLinha }">
                        {{ item.referencia }}
                    </td>
                    <td v-bind:style="{ color: item.corLinha }">
                        {{ item.produto.nome }}
                    </td>
                    <td v-bind:style="{ color: item.corLinha }">
                        {{ item.fornecedor }}
                    </td>
                    <td v-bind:style="{ color: item.corLinha }">
                        {{ item.funcionario }}
                    </td>
                    <td v-if="!item.datas.cadastro" v-bind:style="{ color: item.corLinha }">
                        {{ item.datas.movimentacao | dataHora }}
                    </td>
                    <td v-else v-bind:style="{ color: item.corLinha }">
                        {{ item.datas.movimentacao | data }}</br>
                        (Data Cad. {{ item.datas.cadastro | dataHora }})
                    </td>
                    <td v-bind:style="{ color: item.corLinha }">
                        {{ item.dadosEstoque.quantidade }}
                    </td>
                    <td v-bind:style="{ color: item.corLinha }">
                        {{ item.dadosEstoque.unidade }}
                    </td>
                    <td v-bind:style="{ color: item.corLinha }">
                        {{ item.dadosEstoque.saldoQuantidade }}
                    </td>
                    <td v-bind:style="{ color: item.corLinha }">
                        {{ item.dadosEstoque.tipoMovimentacao }}
                    </td>               
                    <td v-bind:style="{ color: item.corLinha }">
                        {{ item.dadosEstoque.valor | moeda }}
                    </td>
                    <td v-bind:style="{ color: item.corLinha }">
                        {{ item.dadosEstoque.saldoValor | moeda }}
                    </td>
                    <td v-bind:style="{ color: item.corLinha }">
                        {{ item.observacao }}
                    </td>
                    <td>
                        <log-alteracao tabela="MovEstoque" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes && item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Nova movimentação..." v-if="!inserindo">
                            <img src="../Images/Insert.gif">
                        </button>
                        <button v-on:click.prevent="inserir" title="Inserir" v-if="inserindo">
                            <img src="../Images/Ok.gif">
                        </button>
                        <button v-on:click.prevent="cancelar" title="Cancelar" v-if="inserindo">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td colspan="5"></td>
                    <td style="white-space: nowrap">
                        <campo-data-hora :data-hora.sync="movimentacao.dataMovimentacao" :exibir-horas="true" v-if="inserindo"></campo-data-hora>
                    </td>
                    <td>
                        <input type="number" v-model="movimentacao.quantidade" min="0" style="width: 40px" v-if="inserindo" />
                    </td>
                    <td colspan="2"></td>
                    <td>
                        <lista-selecao-id-valor :item-selecionado.sync="tipoMovimentacaoAtual" texto-selecionar="Todas" 
                                                :funcao-recuperar-itens="obterItensFiltroTiposMovimentacao" v-if="inserindo"></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="number" v-model="movimentacao.valor" min="0" style="width: 50px" v-if="inserindo" />
                    </td>
                    <td></td>                    
                    <td>
                        <input type="text" v-model="movimentacao.observacao" v-if="inserindo" />
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
            <section class="link">
                <div>
                    <span>
                        <a href="#" @click.prevent="abrirLogCancelamento()">
                            <img alt="" border="0" src="../Images/ExcluirGrid.gif" /> Movimentações Excluidas
                        </a>
                    </span>
                </div>
                <div>
                    <span>
                        <a href="#" @click.prevent="abrirRelatorio(false)">
                            <img border="0" src="../Images/printer.png"> Imprimir
                        </a>
                    </span>
                    <span>
                        <a href="#" @click.prevent="abrirRelatorio(true)">
                            <img border="0" src="../Images/Excel.gif"> Exportar para o Excel
                        </a>
                    </span>
                </div>
                <div>
                    <span>
                        <a href="#" @click.prevent="abrirRelatorioMovimentacao(false)">
                            <img border="0" src="../Images/printer.png"> Imprimir (Movimentação)
                        </a>
                    </span>
                    <span>
                        <a href="#" @click.prevent="abrirRelatorioMovimentacao(true)">
                            <img border="0" src="../Images/Excel.gif"> Exportar para o Excel (Movimentação)
                        </a>
                    </span>
                </div>
                <div>
                    <span>
                        <a href="#" @click.prevent="abrirRelatorioInventario(false)">
                            <img border="0" src="../Images/printer.png"> Imprimir (Inventário)
                        </a>
                    </span>
                    <span>
                        <a href="#" @click.prevent="abrirRelatorioInventario(true)">
                            <img border="0" src="../Images/Excel.gif"> Exportar para o Excel (Inventário)
                        </a>
                    </span>
                </div>
            </section>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Estoques/Movimentacoes/Reais/Componentes/ListaMovimentacoesEstoqueReal.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Estoques/Movimentacoes/Reais/Componentes/ListaMovimentacoesEstoqueReal.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
