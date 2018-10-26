<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstCfop.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstCfopMigrando" Title="CFOP" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Cfops/Templates/LstCfops.Filtro.html")
    %>
    <div id="app">
        <cfops-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></cfops-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum CFOP encontrado."
                 :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigo')">Cód.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipoCfop')">Tipo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipoMercadoria')">Tipo Mercadoria</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('alterarEstoqueTerceiros')">Alterar estoque terceiros</a>
                    </th>
                    <th v-if="configuracoes && configuracoes.controlarEstoqueVidrosClientes">
                        <a href="#" @click.prevent="ordenar('alterarEstoqueCliente')">Alterar estoque cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('observacao')">Obs</a>
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
                        <a href="#" @click.prevent="abrirNaturezaOperacao(item)" title="Natureza Operação">
                            <img border="0" src="../Images/Subgrupo.png">
                        </a>
                    </td>
                    <td>{{ item.codigo }}</td>
                    <td>{{ item.nome }}</td>
                    <td v-if="item.tipoCfop">{{ item.tipoCfop.nome }}</td>
                    <td v-if="item.tipoMercadoria">{{ item.tipoMercadoria.nome }}</td>
                    <td>{{ item.alterarEstoqueTerceiros | indicaMarcado }}</td>
                    <td v-if="configuracoes && configuracoes.controlarEstoqueVidrosClientes">{{ item.alterarEstoqueCliente | indicaMarcado }}</td>
                    <td>{{ item.observacao }}</td>
                    <td style="white-space: nowrap">
                        <log-alteracao tabela="Cfop" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
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
                    <td>
                        <input type="number" v-model.number="cfop.codigo" maxlength="4" style="width: 70px" required />
                    </td>
                    <td>
                        <input type="text" v-model="cfop.nome" maxlength="150" style="width: 200px" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoCfopAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipoCfop"
                            v-bind:ordenar="false"></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoMercadoriaAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipoMercadoria"
                            v-bind:ordenar="false"></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="checkbox" v-model="cfop.alterarEstoqueTerceiros" />
                    </td>
                    <td v-if="configuracoes && configuracoes.controlarEstoqueVidrosClientes">
                        <input type="checkbox" v-model="cfop.alterarEstoqueCliente" />
                    </td>
                    <td>
                        <input type="text" v-model="cfop.observacao" maxlength="150" style="width: 200px"/>
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Novo CFOP..." v-if="!inserindo">
                            <img src="../Images/Insert.gif">
                        </button>
                        <button v-on:click.prevent="inserir" title="Inserir" v-if="inserindo">
                            <img src="../Images/Ok.gif">
                        </button>
                        <button v-on:click.prevent="cancelar" title="Cancelar" v-if="inserindo">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>
                        <input type="number" v-model.number="cfop.codigo" maxlength="4" style="width: 70px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="text" v-model="cfop.nome" maxlength="150" style="width: 150px" v-if="inserindo" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoCfopAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipoCfop"
                            v-bind:ordenar="false" v-if="inserindo"></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="tipoMercadoriaAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensTipoMercadoria"
                            v-bind:ordenar="false" v-if="inserindo"></lista-selecao-id-valor>
                    </td>                    
                    <td>
                        <input type="checkbox" v-model="cfop.alterarEstoqueTerceiros" v-if="inserindo" />
                    </td>
                    <td v-if="configuracoes && configuracoes.controlarEstoqueVidrosClientes">
                        <input type="checkbox" v-model="cfop.alterarEstoqueCliente" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="text" v-model="cfop.observacao" maxlength="150" style="width: 200px" v-if="inserindo"/>
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaCfops(false)" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaCfops(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Cfops/Componentes/LstCfops.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Cfops/Componentes/LstCfops.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
