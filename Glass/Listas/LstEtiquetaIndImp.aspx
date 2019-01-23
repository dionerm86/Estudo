<%@ Page Title="Impressão Individual de Etiqueta" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstEtiquetaIndImp.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEtiquetaIndImp"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/ImpressoesEtiquetas/Individual/Templates/LstImpressoesIndividuaisEtiquetas.Filtro.html")
    %>
    
    <div id="app">
        <impressoes-individuais-etiquetas-filtros :filtro.sync="filtro">
        </impressoes-individuais-etiquetas-filtros>
        <section>
            <lista-paginada :filtro="filtro" :funcao-recuperar-itens="obterLista" 
                :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum produto encontrado ou nenhum filtro utilizado.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('pedido')">
                            Pedido
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('notaFiscal')">
                            Número NF-e
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('produto')">
                            Produto
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('altura')">
                            Altura
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('largura')">
                            Largura
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('processo')">
                            Proc.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('aplicacao')">
                            Apl.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('quantidade')">
                            Qtd.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('quantidadeImpresso')">
                            Qtd. Já Impresso
                        </a>
                    </th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td>
                        <a href="#" @click.prevent="selecionarEtiquetaProduto(item)">
                            <img border="0" src="../Images/ok.gif" title="Selecionar" alt="Selecionar" />
                        </a>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        <span v-if="item.idPedido > 0">
                            {{ item.idPedido }}
                        </span>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        <span v-if="item.numeroNotaFiscal > 0">
                            {{ item.numeroNotaFiscal }}
                        </span>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.produto }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.medidas.altura }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.medidas.largura }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.processo }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.aplicacao }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.quantidade.total }}
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.quantidade.impressa }}
                    </td>
                </template>
            </lista-paginada>
            <div style="margin-top: 10px">
                <span style="color: red">
                    Etiquetas em Vermelho são de pedidos de reposição.
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/ImpressoesEtiquetas/Individual/Componentes/LstImpressoesIndividuaisEtiquetas.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/ImpressoesEtiquetas/Individual/Componentes/LstImpressoesIndividuaisEtiquetas.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>