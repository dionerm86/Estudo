<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstAssociarPropVeic.aspx.cs" 
    Inherits="Glass.UI.Web.Listas.LstAssociarPropVeic" Title="Associar Proprietário/Veículo"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Conteudo" runat="Server">    
    <div id="app">
        <section>
            <a :href="obterLinkInserirAssociacaoProprietarioVeiculo()">Associar Proprietário/Veículo</a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma associação entre proprietário de veículo e veículo encontrada.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">
                            Nome
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('placa')">
                            Placa
                        </a>
                    </th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td>
                        <a :href="obterLinkEditarAssociacaoProprietarioVeiculo(item)">
                            <img src="../Images/EditarGrid.gif" />
                        </a>
                        <a href="#" @click.prevent="excluir(item)">
                            <img src="../Images/ExcluirGrid.gif" />
                        </a>
                    </td>
                    <td>
                        {{ item.proprietario.nome }}
                    </td>
                    <td>
                        {{ item.placaVeiculo }}
                    </td>
                </template>
            </lista-paginada>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/ConhecimentosTransporte/Veiculos/Proprietarios/Associacoes/Componentes/ListaAssociacaoProprietariosVeiculos.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>

