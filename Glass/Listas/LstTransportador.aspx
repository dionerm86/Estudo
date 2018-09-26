<%@ Page Title="Transportadoras" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstTransportador.aspx.cs" Inherits="Glass.UI.Web.Listas.LstTransportador" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Transportadores/Templates/LstTransportadores.Filtro.html")
    %>
    <div id="app">
        <transportadores-filtros :filtro.sync="filtro"></transportadores-filtros>
        <section>
            <a :href="obterLinkInserirTransportador()" v-if="configuracoes && configuracoes.cadastrarTransportador">
                Inserir transportador
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum transportador encontrado" :numero-registros="10">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Código</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('razaoSocial')">Nome</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nomeFantasia')">Nome fantasia</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cpfCnpj')">CPF/CNPJ</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('placa')">Placa</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('telefone')">Telefone</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarTransportador(item)" v-if="configuracoes && configuracoes.cadastrarTransportador">
                            <img src="../Images/Edit.gif">
                        </a>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="configuracoes && configuracoes.cadastrarTransportador">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.razaoSocial }}</td>
                    <td>{{ item.nomeFantasia }}</td>
                    <td>{{ item.cpfCnpj }}</td>
                    <td>{{ item.placa }}</td>
                    <td>{{ item.telefone }}</td>
                    <td style="white-space: nowrap">
                        <log-alteracao tabela="Transportador" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Transportadores/Componentes/LstTransportadores.js" />
            <asp:ScriptReference Path="~/Vue/Transportadores/Componentes/LstTransportadores.Filtro.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
