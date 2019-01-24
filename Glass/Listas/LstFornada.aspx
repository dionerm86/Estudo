<%@ Page Title="Gerenciamento de Fornada" Language="C#" MasterPageFile="~/Painel.master" 
    AutoEventWireup="true" CodeBehind="LstFornada.aspx.cs" Inherits="Glass.UI.Web.Listas.LstFornada" 
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Producao/Fornadas/Templates/LstFornadas.Filtro.html",
            "~/Vue/Producao/Fornadas/Templates/LstFornadas.Pecas.html")
    %>
    
    <div id="app">
        <fornadas-filtros :filtro.sync="filtro"></fornadas-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro"
                ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma fornada encontrada." @atualizou-itens="atualizouItens">
                <template slot="cabecalho">
                    <th></th>
                    <th style="text-align: center">
                        <a href="#" @click.prevent="ordenar('id')">
                            Cód
                        </a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('funcionario')">
                            Funcionário
                        </a>
                    </th>
                    <th style="text-align: center">
                        <a href="#" @click.prevent="ordenar('dataCadastro')">
                            Data
                        </a>
                    </th>
                    <th style="text-align: center">
                        <a href="#" @click.prevent="ordenar('capacidade')">
                            Capacidade
                        </a>
                    </th>
                    <th style="text-align: center">
                        <a href="#" @click.prevent="ordenar('metroQuadradoLido')">
                            Lido
                        </a>
                    </th>
                    <th style="text-align: center">
                        <a href="#" @click.prevent="ordenar('aproveitamento')">
                            Aproveitamento
                        </a>
                    </th>
                    <th style="text-align: center">
                        Etiquetas
                    </th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td>
                        <span>
                            <a href="#" @click.prevent="alternarExibicaoPecas(index)">
                                <img border="0" :src="exibindoPecasFornada(index) ? '../Images/menos.gif' : '../Images/mais.gif'" />
                            </a>
                        </span>
                    </td>
                    <td>
                        {{ item.id }}
                    </td>
                    <td>
                        {{ item.usuarioCadastro }}
                    </td>
                    <td style="text-align: center">
                        {{ item.dataCadastro | dataHora }}
                    </td>
                    <td style="text-align: center">
                        {{ item.capacidade | decimal }} M²
                    </td>
                    <td style="text-align: center">
                        {{ item.metroQuadradoLido | decimal }} M² ({{ item.quantidadeLida }})
                    </td>
                    <td style="text-align: center">
                        {{ item.aproveitamento | decimal }}%
                    </td>
                    <td style="text-align: center">
                        <span title="Ver etiquetas.">
                            <controle-tooltip :precisa-clicar="true" :titulo="'Etiquetas da fornada: ' + item.id">
                                <template slot="botao">
                                    <img src="../Images/blocodenotas.png" />
                                </template>

                                <div style="max-width: 400px; font-size: 1.1em">
                                    {{ item.etiquetas.toString() }}
                                </div>
                            </controle-tooltip>
                        </span>
                    </td>
                </template>
                <template slot="novaLinhaItem" slot-scope="{ item, index, classe }" v-if="exibindoPecasFornada(index)">
                    <tr>
                        <td></td>
                        <td :colspan="numeroColunasLista() - 1">
                            <fornadas-pecas :filtro="{ idFornada: item.id }"></fornadas-pecas>
                        </td>
                    </tr>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(true, false)">
                        <img border="0" src="../Images/printer.png" /> Relatório Analítico
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(true, true)">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
            <div>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(false, false)">
                        <img border="0" src="../Images/printer.png" /> Relatório Sintético
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirRelatorio(false, true)">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Producao/Fornadas/Componentes/LstFornadas.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Producao/Fornadas/Componentes/LstFornadas.Pecas.js" />
            <asp:ScriptReference Path="~/Vue/Producao/Fornadas/Componentes/LstFornadas.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
