<%@ Page Title="Perda de Chapa de Vidro" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstPerdaChapaVidro.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPerdaChapaVidro"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Produtos/ChapasVidro/Perdas/Templates/LstPerdasChapasVidro.Filtro.html")
    %>

    <div id="app">
        <perdas-chapas-vidro-filtros :filtro.sync="filtro"></perdas-chapas-vidro-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" 
                :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma perda de chapa de vidro encontrada.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigo')">
                            Cód.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('produto')">
                            Produto
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipoperda')">
                            Tipo da Perda
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('subtipoperda')">
                            Subtipo da Perda
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('etiqueta')">
                            Etiqueta
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('data')">
                            Data
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('funcionario')">
                            Func.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('observacao')">
                            Obs.
                        </a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td>
                        <span v-if="item.permissoes.cancelar">
                            <a href="#" @click.prevent="cancelar(item.id)">
                                <img border="0" src="../Images/ExcluirGrid.gif" />
                            </a>
                        </span>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.id }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.produto }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.dadosPerda.tipo }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.dadosPerda.subTipo }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.codigoEtiqueta }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.dadosPerda.data | dataHora }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.dadosPerda.funcionario }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.observacao }}
                    </td>
                    <td>
                        <log-cancelamento tabela="PerdaChapaVidro" :id-item="item.id" :atualizar-ao-alterar="false"
                            v-if="item.permissoes.logCancelamento"></log-cancelamento>
                    </td>
                </template>
            </lista-paginada>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Produtos/ChapasVidro/Perdas/Componentes/LstPerdasChapasVidro.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Produtos/ChapasVidro/Perdas/Componentes/LstPerdasChapasVidro.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
