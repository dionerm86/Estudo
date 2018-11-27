<%@ Page Title="Comissionados" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstComissionado.aspx.cs" Inherits="Glass.UI.Web.Listas.LstComissionado" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Comissionados/Templates/LstComissionados.Filtro.html")
    %>
    <div id="app">
        <comissionados-filtros :filtro.sync="filtro"></comissionados-filtros>
        <section>
            <a :href="obterLinkInserirComissionado()" v-if="configuracoes && configuracoes.cadastrarComissionado">
                Inserir comissionado
            </a>
        </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum comissionado encontrado" :numero-registros="10">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Código</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nome')">Nome</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cpfCnpj')">CPF/CNPJ</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('rgInscricaoEstadual')">RG/Insc. Est.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('telefoneResidencial')">Tel. Cont.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('telefoneCelular')">Celular</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('percentual')">Percentual</a>
                    </th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a :href="obterLinkEditarComissionado(item)" v-if="configuracoes && configuracoes.cadastrarComissionado">
                            <img src="../Images/Edit.gif">
                        </a>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="configuracoes && configuracoes.cadastrarComissionado">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ item.id }}</td>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.cpfCnpj }}</td>
                    <td>{{ item.rgInscricaoEstadual }}</td>
                    <td>{{ item.telefones.residencial }}</td>
                    <td>{{ item.telefones.celular }}</td>
                    <td>{{ item.percentual | percentual }}</td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaComissionados(false)">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaComissionados(true)">
                        <img alt="" border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Comissionados/Componentes/LstComissionados.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Comissionados/Componentes/LstComissionados.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
