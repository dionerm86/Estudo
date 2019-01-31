<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstArquivoRemessa.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstArquivoRemessa" Title="Arquivos de Remessa" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/ArquivosRemessa/Templates/LstArquivosRemessa.Filtro.html")
    %>

    <div id="app">
        <arquivos-remessa-filtros :filtro.sync="filtro"></arquivos-remessa-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro"
                :ordenacao="ordenacao" mensagem-lista-vazia="Não há arquivos de remessa.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">
                            Cód.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('numeroRemessa')">
                            Núm. Remessa
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipo')">
                            Tipo
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('funcionario')">
                            Funcionário
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataCadastro')">
                            Data Cad.
                        </a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td style="white-space: nowrap">
                        <template v-if="item.permissoes && item.permissoes.excluir">
                            <button @click.prevent="excluir(item.id)">
                                <img src="../Images/ExcluirGrid.gif" />
                            </button>
                        </template>
                        <template>
                            <a :href="obterLinkDownload(item.id)">
                                <img src="../Images/disk.gif" />
                            </a>
                        </template>
                        <template>
                            <a :href="obterLinkRetificar(item.id)">
                                <img src="../Images/retificar.png" style="width: 16px; height: 16px" />
                            </a>
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.id }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.numeroArquivoRemessa }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.tipo }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.usuarioCadastro }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.dataCadastro | dataHora }}
                    </td>
                    <td>
                        <template v-if="item.permissoes && item.permissoes.logImportacao">
                            <a :href="obterLinkLogImportacao(item.id)">
                                <img src="../Images/blocodenotas.png" />
                            </a>
                        </template>
                    </td>
                </template>
            </lista-paginada>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/ArquivosRemessa/Componentes/LstArquivosRemessa.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/ArquivosRemessa/Componentes/LstArquivosRemessa.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>

