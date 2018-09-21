<%@ Page Title="Cheques" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstCheque.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCheque" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Cheques/Templates/LstCheque.Filtro.html")
    %>
    <div id="app">
        <cheques-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></cheques-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum cheque encontrado." :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#">Referência</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('loja')">Loja</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('fornecedor')">Fornecedor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('numero')">Número</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('banco')">Banco</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('agencia')">Agência</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('conta')">Conta</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('titular')">Titular</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cpfCnpj')">CPF/CNPJ</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valor')">Valor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataVencimento')">Data vencimento</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('observacao')">Obervação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="editar(item, index)" title="Editar" v-if="numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/Edit.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexos(item)" title="Anexos" v-if="numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/Clipe.gif">
                        </a>
                    </td>
                    <td>{{ item.referencia }}</td>
                    <td>
                        <template v-if="item.loja">
                            {{ item.loja.nome }}
                        </template>
                    </td>
                    <td>
                        <template v-if="item.cliente && item.cliente.id">
                            {{ item.cliente.id }} - {{ item.cliente.nome }}
                        </template>
                    </td>
                    <td>
                        <template v-if="item.fornecedor && item.fornecedor.id">
                            {{ item.fornecedor.id }} - {{ item.fornecedor.nome }}
                        </template>
                    </td>
                    <td style="white-space: nowrap">
                        {{ item.numeroCheque }}
                        <template v-if="item.digitoNumeroCheque">
                             - {{ item.digitoNumeroCheque }}
                        </template>
                    </td>
                    <td>{{ item.banco }}</td>
                    <td>{{ item.agencia }}</td>
                    <td>{{ item.conta }}</td>
                    <td>{{ item.titular }}</td>
                    <td>{{ item.cpfCnpj }}</td>
                    <td>{{ item.valorRecebido }}</td>
                    <td>
                        {{ item.dataVencimento | data }}
                        <template v-if="item.dataVencimentoOriginal">
                            <br />(Data Venc. Orig. {{ item.dataVencimentoOriginal | data }})
                        </template>
                    </td>
                    <td>{{ item.observacao }}</td>
                    <td>{{ item.situacao }}</td>
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="cancelarReapresentacao(item)" v-if="numeroLinhaEdicao === -1 && item.permissoes.cancelarReapresentacao">
                            Cancelar reapresentação
                        </a>
                        <a href="#" @click.prevent="cancelarDevolucao(item)" v-if="numeroLinhaEdicao === -1 && item.permissoes.cancelarDevolucao">
                            Cancelar devolução
                        </a>
                        <a href="#" @click.prevent="cancelarProtesto(item)" v-if="numeroLinhaEdicao === -1 && item.permissoes.cancelarProtesto">
                            Cancelar protesto
                        </a>
                        <a href="#" @click.prevent="abrirLocalizacaoCheque(item)" title="Localização" v-if="numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/environment.gif" />
                        </a>
                        <log-alteracao tabela="Cheque" :id-item="item.id" :atualizar-ao-alterar="false" v-if="numeroLinhaEdicao === -1 && item.permissoes.logAlteracoes"></log-alteracao>
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
                    <td>{{ chequeAtual.referencia }}</td>
                    <td>
                        <template v-if="chequeAtual.loja">
                            {{ chequeAtual.loja.nome }}
                        </template>
                    </td>
                    <td>
                        <template v-if="chequeAtual.cliente && chequeAtual.cliente.id">
                            {{ chequeAtual.cliente.id }} - {{ chequeAtual.cliente.nome }}
                        </template>
                    </td>
                    <td>
                        <template v-if="chequeAtual.fornecedor && chequeAtual.fornecedor.id">
                            {{ chequeAtual.fornecedor.id }} - {{ chequeAtual.fornecedor.nome }}
                        </template>
                    </td>
                    <td style="white-space: nowrap">
                        <span v-if="chequeAtual && chequeAtual.permissoes && chequeAtual.permissoes.alterarDadosCheque">
                            <input type="number" v-model.number="cheque.numeroCheque" style="width: 60px" />
                            <input type="text" v-model="cheque.digitoNumeroCheque" style="width: 10px" maxlength="1" />
                        </span>
                        <span v-else style="white-space: nowrap">
                            {{ chequeAtual.numeroCheque }}
                            <template v-if="chequeAtual.digitoNumeroCheque">
                                 - {{ chequeAtual.digitoNumeroCheque }}
                            </template>
                        </span>
                    </td>
                    <td>
                        <span v-if="chequeAtual && chequeAtual.permissoes && chequeAtual.permissoes.alterarDadosCheque">
                            <input type="text" v-model="cheque.banco" style="width: 50px" />
                        </span>
                        <span v-else>
                            {{ chequeAtual.banco }}
                        </span>
                    </td>
                    <td>
                        <span v-if="chequeAtual && chequeAtual.permissoes && chequeAtual.permissoes.alterarDadosCheque">
                            <input type="text" v-model="cheque.agencia" style="width: 50px" />
                        </span>
                        <span v-else>
                            {{ chequeAtual.agencia }}
                        </span>
                    </td>
                    <td>
                        <span v-if="chequeAtual && chequeAtual.permissoes && chequeAtual.permissoes.alterarDadosCheque">
                            <input type="text" v-model="cheque.conta" style="width: 80px" />
                        </span>
                        <span v-else>
                            {{ chequeAtual.conta }}
                        </span>
                    </td>
                    <td>
                        <input type="text" v-model="cheque.titular" style="width: 200px" maxlength="45" />
                    </td>
                    <td>{{ chequeAtual.cpfCnpj }}</td>
                    <td>{{ chequeAtual.valorRecebido }}</td>
                    <td>
                        <span v-if="chequeAtual && chequeAtual.permissoes && chequeAtual.permissoes.alterarDataVencimento">
                            <campo-data-hora :data-hora.sync="cheque.dataVencimento"></campo-data-hora>
                        </span>
                        <span v-else>
                            {{ chequeAtual.dataVencimento | data }}
                            <template v-if="chequeAtual.dataVencimentoOriginal">
                                <br />(Data Venc. Orig. {{ chequeAtual.dataVencimentoOriginal | data }})
                            </template>
                        </span>
                    </td>
                    <td>
                        <input type="text" v-model="cheque.observacao" style="width: 200px" maxlength="300" />
                    </td>
                    <td>{{ chequeAtual.situacao }}</td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaCheques(false)" title="Imprimir">
                        <img border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaCheques(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Cheques/Componentes/LstCheque.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Cheques/Componentes/LstCheque.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
