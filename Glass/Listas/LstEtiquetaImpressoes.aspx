<%@ Page Title="Consulta de Impressões de Etiquetas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstEtiquetaImpressoes.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEtiquetaImpressoes" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/ImpressoesEtiquetas/Templates/LstImpressoesEtiquetas.Filtro.html")
    %>
    <div id="app">
        <impressoes-etiquetas-filtros :filtro.sync="filtro"></impressoes-etiquetas-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma impressão encontrada" :numero-registros="10">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('id')">Impressão</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('loja')">Loja</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('funcionario')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataImpressao')">Data</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipoImpressao')">Tipo de impressão</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="abrirItensImpressos(item)" title="Itens impressos" v-if="item.permissoes.imprimir">
                            <img src="../Images/relatorio.gif">
                        </button>
                        <button @click.prevent="baixarArquivoOtimizacao(item)" title="Arquivo de otimização" v-if="item.permissoes.baixarArquivoOtimizacao">
                            <img src="../Images/blocodenotas.png">
                        </button>
                        <button @click.prevent="abrirOtimizacaoECutter(item)" title="Abrir otimização" v-if="item.permissoes.abrirECutter">
                            <img src="../Images/puzzle_arrow.png">
                        </button>
                        <button @click.prevent="abrirCancelamentoImpressao(item)" title="Cancelar impressão" v-if="item.permissoes.cancelar">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td :style="{ color: item.corLinha }">{{ item.id }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.loja }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.funcionario }}</td>
                    <td :style="{ color: item.corLinha }" :style="{ color: item.corLinha }">{{ item.dataImpressao | dataHora }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.situacao }}</td>
                    <td :style="{ color: item.corLinha }" v-if="item.tipoImpressao">{{ item.tipoImpressao.nome }}</td>
                    <td style="white-space: nowrap">
                        <log-cancelamento tabela="ImpressaoEtiqueta" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logCancelamento"></log-cancelamento>
                    </td>
                </template>
            </lista-paginada>
        </section>
    </div>
     <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/ImpressoesEtiquetas/Componentes/LstImpressoesEtiquetas.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/ImpressoesEtiquetas/Componentes/LstImpressoesEtiquetas.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
