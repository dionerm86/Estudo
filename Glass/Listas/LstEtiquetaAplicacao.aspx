<%@ Page Title="Aplicações" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstEtiquetaAplicacao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEtiquetaAplicacao" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum processo encontrado"
                :numero-registros="15" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigo')">Código</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('decricao')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('destacarNaEtiqueta')">Destacar na Etiqueta?</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('gerarFormaInexistente')">Gerar Forma Inexistente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('naoPermitirFastDelivery')">Não Permitir Fast Delivery</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('diasMinimosEntrega')">Dias minimos p/ Entrega</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tiposPedido')">Tipo de Pedido</a>
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
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ item.codigo }}</td>
                    <td>{{ item.descricao }}</td>
                    <td>{{ item.destacarNaEtiqueta | indicaMarcado }}</td>
                    <td>{{ item.gerarFormaInexistente | indicaMarcado}}</td>
                    <td>{{ item.naoPermitirFastDelivery | indicaMarcado}}</td>
                    <td>{{ item.numeroDiasUteisDataEntrega }}</td>
                    <td>{{ obterDescricaoTiposPedidos(item) }}</td>
                    <td>
                        <span v-if="item.situacao">
                            {{ item.situacao.nome }}
                        </span>
                    </td>
                    <td>
                        <log-alteracao tabela="Aplicacao" :id-item="item.id" :atualizar-ao-alterar="false"
                            v-if="item.permissoes && item.permissoes.logAlteracoes"></log-alteracao>
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
                        <input type="text" v-model="aplicacao.codigo" maxlength="10" style="width: 50px" required />
                    </td>
                    <td>
                        <input type="text" v-model="aplicacao.descricao" maxlength="30" style="width: 150px" required />
                    </td>
                    <td>
                        <input type="checkbox" v-model="aplicacao.destacarNaEtiqueta" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="aplicacao.gerarFormaInexistente" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="aplicacao.naoPermitirFastDelivery" />
                    </td>
                    <td>
                        <input type="number" v-model.number="aplicacao.numeroDiasUteisDataEntrega" maxlength="10"
                            style="width: 60px" />
                    </td>
                    <td>
                        <lista-selecao-multipla :ids-selecionados.sync="aplicacao.tiposPedidos"
                            :funcao-recuperar-itens="obterTiposPedido"></lista-selecao-multipla>
                    </td>
                    <td>
                        <lista-selecao-id-valor :item-selecionado.sync="situacaoAtual"
                            :funcao-recuperar-itens="obterSituacoes" required></lista-selecao-id-valor>
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Novo produto..." v-if="!inserindo">
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
                        <input type="text" v-model="aplicacao.codigo" maxlength="10" style="width: 50px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="text" v-model="aplicacao.descricao" maxlength="30" style="width: 150px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="checkbox" v-model="aplicacao.destacarNaEtiqueta" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="aplicacao.gerarFormaInexistente" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="aplicacao.naoPermitirFastDelivery" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="number" v-model.number="aplicacao.numeroDiasUteisDataEntrega" maxlength="10"
                            style="width: 60px" v-if="inserindo" />
                    </td>
                    <td>
                        <lista-selecao-multipla :ids-selecionados.sync="aplicacao.tiposPedidos"
                            :funcao-recuperar-itens="obterTiposPedido" v-if="inserindo"></lista-selecao-multipla>
                    </td>
                    <td>
                        <lista-selecao-id-valor :item-selecionado.sync="situacaoAtual"
                            :funcao-recuperar-itens="obterSituacoes" required v-if="inserindo"></lista-selecao-id-valor>
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Aplicacoes/Componentes/LstAplicacoes.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
