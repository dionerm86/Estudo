<%@ Page Title="Classificação - Roteiro da Produção" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="LstClassificacaoRoteiroProducao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstClassificacaoRoteiroProducao" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <div id="app">
        <section>
            <a :href="obterLinkInserirClassificacaoRoteiro()">
                Inserir classificação
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma classificação de roteiro encontrada.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Cód.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('capacidadeDiaria')">Capacidade diária</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('metaDiaria')">Meta diária</a>
                    </th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarClassificacaoRoteiro(item)" title="Editar">
                            <img border="0" src="../../Images/EditarGrid.gif">
                        </a>
                        <a href="#" @click.prevent="excluir(item)" title="Excluir">
                            <img border="0" src="../../Images/ExcluirGrid.gif">
                        </a>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.capacidadeDiaria }}</td>
                    <td>{{ item.metaDiaria }}</td>
                </template>
            </lista-paginada>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Producao/Roteiros/Classificacoes/Componentes/LstClassificacoesRoteiro.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
