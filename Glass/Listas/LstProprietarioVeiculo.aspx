<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="LstProprietarioVeiculo.aspx.cs" Inherits="Glass.UI.Web.Listas.LstProprietarioVeiculo"
    Title="Proprietários Veículos" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Conteudo" runat="Server">
    <div id="app">
        <div>
            <a v-bind:href="obterLinkInserirProprietarioVeiculo()">Inserir Proprietário</a>
        </div>
        <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" 
            mensagem-lista-vazia="Nenhum proprietário de veículo encontrado." :numero-registros="15">
            <template slot="cabecalho">
                <th></th>
                <th>
                    <a href="#" @click.prevent="ordenar('nome')">
                        Nome
                    </a>
                </th>
                <th>
                    <a href="#" @click.prevent="ordenar('rntrc')">
                        RNTRC
                    </a>
                </th>
                <th>
                    <a href="#" @click.prevent="ordenar('ie')">
                        Insc. Estadual
                    </a>
                </th>
                <th>
                    <a href="#" @click.prevent="ordenar('uf')">
                        UF
                    </a>
                </th>
                <th>
                    <a href="#" @click.prevent="ordenar('tipoProp')">
                        TipoProp
                    </a>
                </th>
            </template>
            <template slot="item" slot-scope="{ item }">
                <td style="white-space: nowrap">
                    <a v-bind:href="obterLinkEditarProprietarioVeiculo(item.id)">
                        <img src="../Images/edit.gif" />
                    </a>
                    <button @click.prevent="excluir(item)">
                        <img src="../Images/ExcluirGrid.gif" />
                    </button>
                </td>
                <td>
                    {{ item.nome }}
                </td>
                <td>
                    {{ item.rntrc }}
                </td>
                <td>
                    {{ item.inscricaoEstadual }}
                </td>
                <td>
                    {{ item.uf }}
                </td>
                <td>
                    {{ item.tipo }}
                </td>
            </template>
        </lista-paginada>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/ConhecimentosTransporte/Veiculos/Proprietarios/Componentes/ListaProprietariosVeiculo.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
