<%@ Page Title="Processos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstEtiquetaProcesso.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEtiquetaProcesso" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">




        function onSave(insert) {
            var descricao = FindControl(insert ? "txtDescricaoIns" : "txtDescricao", "input").value;
            var codInterno = FindControl(insert ? "txtCodInternoIns" : "txtCodInterno", "input").value;

            if (descricao == "") {
                alert("Informe a descrição.");
                return false;
            }

            if (codInterno == "") {
                alert("Informe o código.");
                return false;
            }
        }

    </script>

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
                        <a href="#" @click.prevent="ordenar('descricao')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('aplicacao')">Aplicação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('destacarNaEtiqueta')">Destacar na Etiqueta?</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('gerarFormaInexistente')">Gerar forma inexistente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('gerarArquivoDeMesa')">Gerar Arquivo de Mesa</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('numeroDiasUteisDataEntrega')">Número dias úteis data entrega</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipoProcesso')">Tipo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tiposPedidos')">Tipo de Pedido</a>
                    </th>
                    <th>
                        Situação
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
                    <td>
                        <span v-if="item.aplicacao">
                            {{ item.aplicacao.codigo }}
                        </span>
                    </td>
                    <td>{{ item.destacarNaEtiqueta | simNao }}</td>
                    <td>{{ item.gerarFormaInexistente | simNao }}</td>
                    <td>{{ item.gerarArquivoDeMesa | simNao }}</td>
                    <td>{{ item.numeroDiasUteisDataEntrega }}</td>
                    <td>
                        <span v-if="item.tipoProcesso">
                            {{ item.tipoProcesso.nome }}
                        </span>
                    </td>
                    <td>{{ obterDescricaoTiposPedidos(item) }}</td>
                    <td>
                        <span v-if="item.situacao">
                            {{ item.situacao.nome }}
                        </span>
                    </td>
                    <td>
                        <log-alteracao tabela="Processo" :id-item="item.id" :atualizar-ao-alterar="false"
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
                        <input type="text" v-model="processo.codigo" maxlength="10" style="width: 50px" />
                    </td>
                    <td>
                        <input type="text" v-model="processo.descricao" maxlength="30" style="width: 150px" />
                    </td>
                    <td>
                        <campo-busca-etiqueta-aplicacao :aplicacao.sync="aplicacaoAtual"></campo-busca-etiqueta-aplicacao>
                    </td>
                    <td>
                        <input type="checkbox" v-model="processo.destacarNaEtiqueta" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="processo.gerarFormaInexistente" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="processo.gerarArquivoDeMesa" />
                    </td>
                    <td>
                        <input type="number" v-model.number="processo.numeroDiasUteisDataEntrega" maxlength="10"
                            style="width: 60px" />
                    </td>
                    <td>
                        <lista-selecao-id-valor :item-selecionado.sync="tipoProcessoAtual"
                            :funcao-recuperar-itens="obterTiposProcesso"></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-multipla :ids-selecionados.sync="processo.tiposPedidos"
                            :funcao-recuperar-itens="obterTiposPedido"></lista-selecao-multipla>
                    </td>
                    <td>
                        <lista-selecao-id-valor :item-selecionado.sync="situacaoAtual"
                            :funcao-recuperar-itens="obterSituacoes"></lista-selecao-id-valor>
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
                        <input type="text" v-model="processo.codigo" maxlength="10" style="width: 50px" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="text" v-model="processo.descricao" maxlength="30" style="width: 150px" v-if="inserindo" />
                    </td>
                    <td>
                        <campo-busca-etiqueta-aplicacao :aplicacao.sync="aplicacaoAtual" v-if="inserindo"></campo-busca-etiqueta-aplicacao>
                    </td>
                    <td>
                        <input type="checkbox" v-model="processo.destacarNaEtiqueta" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="processo.gerarFormaInexistente" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="processo.gerarArquivoDeMesa" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="number" v-model.number="processo.numeroDiasUteisDataEntrega" maxlength="10"
                            style="width: 60px" v-if="inserindo" />
                    </td>
                    <td>
                        <lista-selecao-id-valor :item-selecionado.sync="tipoProcessoAtual"
                            :funcao-recuperar-itens="obterTiposProcesso" v-if="inserindo"></lista-selecao-id-valor>
                    </td>
                    <td>
                        <lista-selecao-multipla :ids-selecionados.sync="processo.tiposPedidos"
                            :funcao-recuperar-itens="obterTiposPedido" v-if="inserindo"></lista-selecao-multipla>
                    </td>
                    <td>
                        <lista-selecao-id-valor :item-selecionado.sync="situacaoAtual"
                            :funcao-recuperar-itens="obterSituacoes" v-if="inserindo"></lista-selecao-id-valor>
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Processos/Componentes/LstProcessos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
