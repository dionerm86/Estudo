<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstCfopMigrando.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstCfopMigrando" Title="CFOP" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Cfops/Templates/LstCfops.Filtro.html")
    %>
    <div id="app">
        <cfops-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></cfops-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum CFOP encontrado.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigo')">Cód.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricao')">Descrição</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('idTipoCfop')">Tipo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipoMercadoria')">Tipo Mercadoria</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('alterarEstoqueTerceiros')">Alterar estoque terceiros</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('alterarEstoqueCliente')">Alterar estoque cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('obs')">Obs</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button @click.prevent="editar(item, index)" title="Editar" v-if="!inserindo && numeroLinhaEdicao === -1">
                            <img src="../Images/Edit.gif">
                        </button>
                        <button @click.prevent="excluir(item)" title="Excluir" v-if="!inserindo && numeroLinhaEdicao === -1 && item.permissoes.excluir">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                        <a href="#" @click.prevent="abrirNaturezaOperacao(item)" title="Natureza Operação">
                            <img border="0" src="../Images/Subgrupo.png">
                        </a>
                    </td>
                    <td>{{ item.codigo }}</td>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.idTipoCfop }}</td>
                    <td>{{ item.tipoMercadoria }}</td>
                    <td>{{ item.alterarEstoqueTerceiros | simNao }}</td>
                    <td v-if="configuracoes && configuracoes.controlarEstoqueVidrosClientes">{{ item.alterarEstoqueCliente | simNao }}</td>
                    <td>{{ item.obs }}</td>
                    <td style="white-space: nowrap">
                        <controle-exibicao-imagem :id-item="item.id" tipo-item="Cfop"></controle-exibicao-imagem>
                        <log-alteracao tabela="Cfop" :id-item="item.id" :atualizar-ao-alterar="false"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaCfops(false)" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaCfops(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Cfops/Componentes/LstCfops.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Cfops/Componentes/LstCfops.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
