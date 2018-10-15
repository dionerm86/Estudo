<%@ Page Title="Consulta Produção" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstProducao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Producao.LstProducao"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Producao/Templates/LstProducao.Filtros.html",
            "~/Vue/Producao/Templates/LstProducao.Pecas.html")
    %>
    <div id="app">
        <producao-filtros :filtro.sync="filtro" :configuracoes="configuracoes" :agrupar-impressao="agruparImpressao"></producao-filtros>
        <producao-pecas :filtro="filtro" :configuracoes="configuracoes" :buscar-pecas="buscarPecas" @atualizou-itens="atualizouItens"></producao-pecas>
        <section style="font-size: medium" v-if="exibirContagem">
            <span>
                <label style="color: blue; font-weight: bold">Peças Prontas:</label>
                <label>
                    <template v-if="contagem && contagem.prontas">
                        {{ contagem.prontas.numero }}
                        ({{ contagem.prontas.areaEmM2.real }} / {{ contagem.prontas.areaEmM2.paraCalculo }} m²)
                    </template>
                    <template v-else>
                        0 (0 / 0 m²)
                    </template>
                </label>
            </span>
            <span>
                <label style="color: red; font-weight: bold">Peças Pendentes:</label>
                <label>
                    <template v-if="contagem && contagem.pendentes">
                        {{ contagem.pendentes.numero }}
                        ({{ contagem.pendentes.areaEmM2.real }} / {{ contagem.pendentes.areaEmM2.paraCalculo }} m²)
                    </template>
                    <template v-else>
                        0 (0 / 0 m²)
                    </template>
                </label>
            </span>
            <span>
                <label style="color: #009933; font-weight: bold">Peças Entregues:</label>
                <label>
                    <template v-if="contagem && contagem.entregues">
                        {{ contagem.entregues.numero }}
                        ({{ contagem.entregues.areaEmM2.real }} / {{ contagem.entregues.areaEmM2.paraCalculo }} m²)
                    </template>
                    <template v-else>
                        0 (0 / 0 m²)
                    </template>
                </label>
            </span>
            <span>
                <label style="color: gray; font-weight: bold">Peças Perdidas:</label>
                <label>
                    <template v-if="contagem && contagem.perdidas">
                        {{ contagem.perdidas.numero }}
                        ({{ contagem.perdidas.areaEmM2.real }} / {{ contagem.perdidas.areaEmM2.paraCalculo }} m²)
                    </template>
                    <template v-else>
                        0 (0 / 0 m²)
                    </template>
                </label>
            </span>
            <span>
                <label style="color: black; font-weight: bold">Peças Canceladas:</label>
                <label>
                    <template v-if="contagem && contagem.canceladas">
                        {{ contagem.canceladas.numero }}
                        ({{ contagem.canceladas.areaEmM2.real }} / {{ contagem.canceladas.areaEmM2.paraCalculo }} m²)
                    </template>
                    <template v-else>
                        0 (0 / 0 m²)
                    </template>
                </label>
            </span>
        </section>
        <section v-if="dadosProducao">
            <div style="color: red; font-weight: bold; font-size: medium">
                <template v-if="dadosProducao.quantidadePecasVidroParaEstoque > 0 && !dadosProducao.pedidoProducao">
                    Este pedido possui {{ dadosProducao.quantidadePecasVidroParaEstoque }} peça(s) de estoque.
                </template>
                <template v-if="dadosProducao.possuiEtiquetasNaoImpressas">
                    Este pedido possui peças não impressas.
                </template>
            </div>
        </section>
        <section class="links">
            <div v-if="configuracoes.exibirRelatorios">
                <span style="text-align: right">
                    <a href="#" @click.prevent="abrirRelatorioGeral(false)">
                        <img src="../../Images/Printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorioGeral(true)">
                        <img src="../../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
            <div v-if="exibirImpressoesSetor">
                <span style="text-align: right">
                    <a href="#" @click.prevent="abrirRelatorioSetor(false)">
                        <img src="../../Images/Printer.png" /> Imprimir (Setor Selecionado)
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorioSetor(true)">
                        <img src="../../Images/Excel.gif" /> Exportar para o Excel (Setor Selecionado)
                    </a>
                </span>
            </div>
            <div v-if="exibirImpressoesRoteiro">
                <span style="text-align: right">
                    <a href="#" @click.prevent="abrirRelatorioRoteiro(false)">
                        <img src="../../Images/Printer.png" /> Imprimir (Roteiro)
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorioRoteiro(true)">
                        <img src="../../Images/Excel.gif" /> Exportar para o Excel (Roteiro)
                    </a>
                </span>
            </div>
            <div>
                <span style="text-align: right">
                    <a href="#" @click.prevent="abrirRelatorioPedidos(false)">
                        <img src="../../Images/Printer.png" /> Imprimir (Pedidos)
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorioPedidos(true)">
                        <img src="../../Images/Excel.gif" /> Exportar para o Excel (Pedidos)
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Producao/Componentes/LstProducao.Filtros.js" />
            <asp:ScriptReference Path="~/Vue/Producao/Componentes/LstProducao.Pecas.js" />
            <asp:ScriptReference Path="~/Vue/Producao/Componentes/LstProducao.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
