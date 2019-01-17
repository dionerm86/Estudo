<%@ Page Title="Lista de Retalhos de Produção" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstRetalhoProducao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Producao.LstRetalhoProducao"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Producao/Retalhos/Templates/LstRetalhosProducao.Filtro.html")
    %>

    <div id="app">
        <retalhos-producao-filtros :filtro.sync="filtro"></retalhos-producao-filtros>
       <section>
           <span>
               <a :href="obterLinkInserirRetalhoProducao()">
                   Inserir Retalho Avulso
               </a>
           </span>
           <span>
               <a href="#" v-on:click.prevent="abrirJanelaDisponibilizarRetalhosProducao">
                   Disponibilizar Retalhos
               </a>
           </span>
       </section>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro"
                :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum retalho de produção encontrado.">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codInterno')">
                            Cód.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricao')">
                            Descrição
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('largura')">
                            Largura
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('altura')">
                            Altura
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataCad')">
                            Data Cad.
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">
                            Situação
                        </a>
                    </th>
                    <th>
                        Número de Etiqueta
                    </th>
                    <th>
                        Número NF-e
                    </th>
                    <th>
                        Lote
                    </th>
                    <th>
                        M² total
                    </th>
                    <th>
                        Etiquetas
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataUso')">
                            Data Uso
                        </a>
                    </th>
                    <th>
                        M² aproveitado
                    </th>
                    <th>
                        Funcionário
                    </th>
                    <th>
                        Obs
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item }">
                    <td>
                        <a href="#" @click.prevent="abrirRelatorioIndividual(item.id)">
                            <img border="0" src="../../Images/Relatorio.gif" />
                        </a>
                        <a href="#" @click.prevent="abrirDefinicaoMotivoCancelamento(item.id)">
                            <img border="0" src="../Images/ExcluirGrid.gif" />
                        </a>
                        <a href="#" @click.prevent="abrirDefinicaoMotivoPerda(item.id)">
                            <img border="0" src="../../Images/perda.png" style="width: 16px; height: 16px" />
                        </a>
                    </td>
                    <td>
                        {{ item.produto.codigo }}
                    </td>
                    <td>
                        {{ item.produto.descricao }}
                    </td>
                    <td>
                        {{ item.medidas.largura }}
                    </td>
                    <td>
                        {{ item.medidas.altura }}
                    </td>
                    <td>
                        {{ item.datas.cadastro | data }}
                    </td>
                    <td>
                        {{ item.situacao }}
                    </td>
                    <td>
                        {{ item.codigoEtiqueta }}
                    </td>
                    <td>
                        {{ item.numeroNotaFiscal }}
                    </td>
                    <td>
                        {{ item.lote }}
                    </td>
                    <td>
                        {{ item.medidas.metroQuadrado.total | decimal }}
                    </td>
                    <td>
                        <controle-tooltip :precisa-clicar="true" :titulo="'Leituras do retalho: ' + item.id"
                            v-if="item.permissoes.exibirEtiquetasUsando">
                            <template slot="botao">
                                <img src="../../Images/blocodenotas.png" :title="'Leituras do retalho: ' + item.id" />
                            </template>

                            {{ item.codigosEtiquetaUsandoRetalho }}
                        </controle-tooltip>
                    </td>
                    <td>
                        {{ item.datas.uso | data }}
                    </td>
                    <td>
                        {{ item.medidas.metroQuadrado.usando | decimal }}
                    </td>
                    <td>
                        {{ item.funcionario }}
                    </td>
                    <td>
                        {{ item.observacao }}
                    </td>
                    <td>
                        <log-alteracao tabela="RetalhoProducao" :id-item="item.id" :atualizar-ao-alterar="false"
                            v-if="item.permissoes.logAlteracoes"></log-alteracao>
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(false)">
                        <img border="0" src="../../Images/Printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(true)">
                        <img border="0" src="../../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Producao/Retalhos/Componentes/LstRetalhosProducao.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Producao/Retalhos/Componentes/LstRetalhosProducao.js" />
        </Scripts>
    </asp:ScriptManager>     
</asp:Content>
