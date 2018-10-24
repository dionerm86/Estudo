<%@ Page Title="Contas Bancárias" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstContaBanco.aspx.cs" Inherits="Glass.UI.Web.Listas.LstContaBanco" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma conta bancária encontrada."
                :numero-registros="10" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('loja')">Loja</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Banco</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigoBanco')">Cód. banco</a>
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
                        <a href="#" @click.prevent="ordenar('codigoConvenio')">Cód. convênio</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigoCliente')">Cód. cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('posto')">Posto/variação (CNAB)</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="editar(item, index)" title="Editar" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Edit.gif">
                        </button>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="!inserindo && numeroLinhaEdicao === -1 && item.permissoes && item.permissoes.excluir">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td v-if="item.loja">{{ item.loja.nome }}</td>
                    <td>{{ item.nome }}</td>
                    <td v-if="item.dadosBanco">{{ item.dadosBanco.banco.id }}</td>
                    <td v-if="item.dadosBanco">{{ item.dadosBanco.agencia }}</td>
                    <td v-if="item.dadosBanco">{{ item.dadosBanco.conta }}</td>
                    <td v-if="item.dadosBanco">{{ item.dadosBanco.titular }}</td>
                    <td v-if="item.dadosBanco">{{ item.dadosBanco.codigoConvenio }}</td>
                    <td v-if="item.cnab">{{ item.cnab.codigoCliente }}</td>
                    <td v-if="item.cnab">{{ item.cnab.posto }}</td>
                    <td v-if="item.situacao">{{ item.situacao.nome }}</td>
                    <td>
                        <log-alteracao tabela="ContaBanco" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
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
                        <lista-selecao-lojas :loja.sync="lojaAtual" :ativas="true" required></lista-selecao-lojas>
                    </td>
                    <td>
                        <input type="text" v-model="contaBancaria.nome" maxlength="45" style="width: 180px" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="bancoAtual" texto-selecionar="Selecionar" v-bind:funcao-recuperar-itens="obterItensBanco"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="text" v-model="contaBancaria.dadosBanco.agencia" maxlength="15" style="width: 60px" v-if="contaBancaria.dadosBanco" required />
                    </td>
                    <td>
                        <input type="text" v-model="contaBancaria.dadosBanco.conta" maxlength="15" style="width: 100px" v-if="contaBancaria.dadosBanco" required />
                    </td>
                    <td>
                        <input type="text" v-model="contaBancaria.dadosBanco.titular" maxlength="15" style="width: 180px" v-if="contaBancaria.dadosBanco" required />
                    </td>
                    <td>
                        <input type="number" v-model.number="contaBancaria.dadosBanco.codigoConvenio" maxlength="20" style="width: 60px" v-if="contaBancaria.dadosBanco" />
                    </td>
                    <td>
                        <input type="text" v-model="contaBancaria.cnab.codigoCliente" maxlength="21" style="width: 100px" v-if="contaBancaria.cnab" />
                    </td>
                    <td>
                        <input type="number" v-model.number="contaBancaria.cnab.posto" maxlength="20" style="width: 60px" v-if="contaBancaria.cnab" />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="situacaoAtual" texto-selecionar="Selecionar" v-bind:funcao-recuperar-itens="obterItensSituacao"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Nova conta bancária..." v-if="!inserindo">
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
                        <lista-selecao-lojas :loja.sync="lojaAtual" :ativas="true" v-if="inserindo" required></lista-selecao-lojas>
                    </td>
                    <td>
                        <input type="text" v-model="contaBancaria.nome" maxlength="45" style="width: 180px" v-if="inserindo" required />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="bancoAtual" texto-selecionar="Selecionar" v-bind:funcao-recuperar-itens="obterItensBanco"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td>
                        <input type="text" v-model="contaBancaria.dadosBanco.agencia" maxlength="15" style="width: 60px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="text" v-model="contaBancaria.dadosBanco.conta" maxlength="15" style="width: 100px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="text" v-model="contaBancaria.dadosBanco.titular" maxlength="15" style="width: 180px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="number" v-model.number="contaBancaria.dadosBanco.codigoConvenio" maxlength="20" style="width: 60px" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="text" v-model="contaBancaria.cnab.codigoCliente" maxlength="21" style="width: 100px" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="number" v-model.number="contaBancaria.cnab.posto" maxlength="20" style="width: 60px" v-if="inserindo" />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="situacaoAtual" texto-selecionar="Selecionar" v-bind:funcao-recuperar-itens="obterItensSituacao"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/ContasBancarias/Componentes/LstContasBancarias.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
