<%@ Page Title="Volumes" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadVolume.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadVolume" EnableViewState="false" EnableViewStateMac="false" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Volumes/Templates/LstVolumes.Filtro.html",
            "~/Vue/Volumes/Templates/LstVolumes.Itens.html")
    %>
    <div id="app">
        <volumes-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></volumes-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum pedido encontrado para o filtro informado."
                v-on:atualizou-itens="atualizouItens">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('pedido')">Pedido</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('loja')">Loja</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('funcionario')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataEntrega')">Entrega</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('rota')">Rota</a>
                    </th>
                    <th>
                        Total m²
                    </th>
                    <th>
                        Peso total
                    </th>
                    <th>
                        Itens pedido
                    </th>
                    <th>
                        Itens volume
                    </th>
                    <th>
                        Itens pendentes
                    </th>
                    <th>
                        Situação
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="alternarExibicaoVolumes(index)" v-if="!exibindoVolumes(index)">
                            <img src="../../Images/mais.gif" title="Exibir volumes" />
                        </button>
                        <button v-on:click.prevent="alternarExibicaoVolumes(index)" v-if="exibindoVolumes(index)">
                            <img src="../../Images/menos.gif" title="Esconder volumes" />
                        </button>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.id }} 
                        <template v-if="configuracoes.controlarPedidosImportados && item.importado && item.pedidoExterno">
                            ({{ item.pedidoExterno.id }})
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.cliente.id }} - {{ item.cliente.nome }} 
                        <template v-if="configuracoes.controlarPedidosImportados && item.importado && item.pedidoExterno && item.pedidoExterno.cliente">
                            ({{ item.pedidoExterno.cliente.id }} - {{ item.pedidoExterno.cliente.nome }})
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">{{ item.loja }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.funcionario }}</td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.dataEntrega | data }} 
                        <template v-if="item.dataEntregaOriginal">
                            ({{ item.dataEntregaOriginal | data }})
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.rota }}
                        <template v-if="configuracoes.controlarPedidosImportados && item.importado && item.pedidoExterno">
                            ({{ item.pedidoExterno.rota }})
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">{{ item.dadosVolume.metroQuadrado | decimal }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dadosVolume.peso | decimal }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.quantidadePecasPedido | decimal }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dadosVolume.quantidadePecas | decimal }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dadosVolume.quantidadePecasPendentes | decimal }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dadosVolume.situacao }}</td>
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="abrirTelaGerarVolume(item)" title="Gerar volume" v-if="item.permissoes.gerarVolume">
                            Gerar volume
                        </a>
                        <button @click.prevent="abrirTelaVisualizarVolume(item)" title="Visualizar volume" v-if="item.permissoes.exibirRelatorioVolume">
                            <img src="../Images/Relatorio.gif">
                        </button>
                    </td>
                </template>
                <template slot="novaLinhaItem" slot-scope="{ item, index, classe }" v-if="exibindoVolumes(index)">
                    <tr v-bind:class="classe" style="border-top: none">
                        <td></td>
                        <td v-bind:colspan="numeroColunasLista() - 1">
                            <volumes-itens v-bind:filtro="{ idPedido: item.id, idVolume: filtro.idVolume, situacoesVolume: filtro.situacoesVolume }" v-bind:configuracoes="configuracoes"></volumes-itens>
                        </td>
                    </tr>
                </template>
            </lista-paginada>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Volumes/Componentes/LstVolumes.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Volumes/Componentes/LstVolumes.Itens.js" />
            <asp:ScriptReference Path="~/Vue/Volumes/Componentes/LstVolumes.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
