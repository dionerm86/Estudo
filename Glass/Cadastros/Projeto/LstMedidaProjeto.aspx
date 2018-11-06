<%@ Page Title="Medidas de Projeto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstMedidaProjeto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.LstMedidaProjeto" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Projetos/MedidasProjeto/Templates/LstMedidasProjeto.Filtro.html")
    %>
    <div id="app">
        <medidas-projeto-filtros :filtro.sync="filtro"></medidas-projeto-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma medida de projeto encontrada."
                :numero-registros="15" :exibir-inclusao="true" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorPadrao')">Valor padrão</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('exibirApenasEmCalculosDeMedidaExata')">Exibir apenas em cálculos de medida exata</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('exibirApenasEmCalculosDeFerragensEAluminios')">Exibir apenas em cálculos de ferragens e alumínios</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('grupoMedidaProjeto')">Grupo</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="editar(item, index)" title="Editar" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../../Images/Edit.gif">
                        </button>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="!inserindo && numeroLinhaEdicao === -1 && item.permissoes && item.permissoes.excluir">
                            <img src="../../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.valorPadrao }}</td>
                    <td>{{ item.exibirApenasEmCalculosDeMedidaExata | indicaMarcado }}</td>
                    <td>{{ item.exibirApenasEmCalculosDeFerragensEAluminios | indicaMarcado }}</td>
                    <td v-if="item.grupoMedidaProjeto">{{ item.grupoMedidaProjeto.nome }}</td>
                    <td>
                        <log-alteracao tabela="MedidaProjeto" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
                <template slot="itemEditando">
                    <td style="white-space: nowrap">
                        <button @click.prevent="atualizar" title="Atualizar">
                            <img src="../../Images/ok.gif">
                        </button>
                        <button @click.prevent="cancelar" title="Cancelar">
                            <img src="../../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>
                        <input type="text" v-model="medidaProjeto.nome" maxlength="60" style="width: 300px" required />
                    </td>
                    <td>
                        <input type="number" v-model.number="medidaProjeto.valorPadrao" maxlength="6" style="width: 60px" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="medidaProjeto.exibirApenasEmCalculosDeMedidaExata" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="medidaProjeto.exibirApenasEmCalculosDeFerragensEAluminios" />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="grupoMedidaAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensGrupoMedida"
                            v-bind:ordenar="false" required></lista-selecao-id-valor>
                    </td>
                    <td></td>
                </template>
                <template slot="itemIncluir">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="iniciarCadastro" title="Nova medida de projeto..." v-if="!inserindo">
                            <img src="../../Images/Insert.gif">
                        </button>
                        <button v-on:click.prevent="inserir" title="Inserir" v-if="inserindo">
                            <img src="../../Images/Ok.gif">
                        </button>
                        <button v-on:click.prevent="cancelar" title="Cancelar" v-if="inserindo">
                            <img src="../../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>
                        <input type="text" v-model="medidaProjeto.nome" maxlength="60" style="width: 300px" v-if="inserindo" required />
                    </td>
                    <td>
                        <input type="number" v-model.number="medidaProjeto.valorPadrao" maxlength="6" style="width: 60px" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="medidaProjeto.exibirApenasEmCalculosDeMedidaExata" v-if="inserindo" />
                    </td>
                    <td>
                        <input type="checkbox" v-model="medidaProjeto.exibirApenasEmCalculosDeFerragensEAluminios" v-if="inserindo" />
                    </td>
                    <td>
                        <lista-selecao-id-valor v-bind:item-selecionado.sync="grupoMedidaAtual" texto-selecionar="Todos" v-bind:funcao-recuperar-itens="obterItensGrupoMedida"
                            v-bind:ordenar="false" v-if="inserindo" required></lista-selecao-id-valor>
                    </td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Projetos/MedidasProjeto/Componentes/LstMedidasProjeto.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Projetos/MedidasProjeto/Componentes/LstMedidasProjeto.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
