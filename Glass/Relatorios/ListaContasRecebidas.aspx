<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaContasRecebidas.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaContasRecebidas" Title="Contas Recebidas" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/ContasReceber/Templates/LstContasRecebidas.Filtro.html")
    %>
    <div id="app">
        <contarecebida-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></contarecebida-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhuma conta recebida encontrada." :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="return false">Referência</a>
                    </th>
                    <th v-if="configuracoes.comissaoPorContasRecebida">
                        <a href="#" @click.prevent="ordenar('idComissao')">Comissão</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('numeroParcela')">Parc.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('nomeCliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('formaPagamento')">Forma Pagamento</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorVencimento')">Valor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataVencimento')">Data vencimento</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valorRecebido')">Valor recebido</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataRecebimento')">Data recebimento</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('recebidaPor')">Recebida por</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('numeroNFe')">Número NF-e</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('localizacao')">Localização</a>
                    </th>
                    <th v-if="configuracoes.utilizarCnab">
                        <a href="#" @click.prevent="ordenar('numeroArquivoRemessa')">Num. Arquivo Remessa</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('observacao')">Observação</a>
                    </th>
                    <th v-if="configuracoes.exibirComissao">
                        <a href="#" @click.prevent="ordenar('descricaoComissao')">Comissão</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('tipoContabil')">Tipo</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="editar(item, index)" title="Editar" v-if="item.permissoes.editar && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/Edit.gif">
                        </a>
                        <a href="#" @click.prevent="abrirCancelamentoContaRecebida(item)" title="Cancelar" v-if="item.permissoes.cancelar && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/ExcluirGrid.gif">
                        </a>
                    </td>
                    <td :style="{ color: item.corLinha }">{{ item.referencia }}</td>
                    <td :style="{ color: item.corLinha }" v-if="configuracoes.comissaoPorContasRecebida">{{ item.idComissao }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.numeroParcela }}/{{ item.numeroMaximoParcelas }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.cliente.id }} - {{ item.cliente.nome }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.formaPagamento }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.valorVencimento | moeda }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dataVencimento | data }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.valorRecebimento | moeda }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dataRecebimento | data }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.recebidaPor }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.numeroNfe }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.localizacao }}</td>
                    <td :style="{ color: item.corLinha }" v-if="configuracoes.utilizarCnab">{{ item.numeroArquivoRemessaCnab }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.observacao }} {{ item.observacaoDescontoAcrescimo }}</td>
                    <td :style="{ color: item.corLinha }" v-if="configuracoes.exibirComissao">{{ item.detalhamentoComissao }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.descricaoContaContabil }}</td>
                    <td :style="{ color: item.corLinha, whiteSpace: 'nowrap' }">
                        <log-alteracao tabela="ContasReceber" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
                        <log-cancelamento tabela="ContasReceber" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logCancelamento"></log-cancelamento>
                    </td>
                </template>
                <template slot="itemEditando">
                    <td style="white-space: nowrap">
                        <button @click.prevent="atualizar" title="Atualizar">
                            <img src="../Images/ok.gif">
                        </button>
                        <button @click.prevent="cancelar" title="Cancelar">
                            <img src="../Images/ExcluirGrid.gif">
                        </button>
                    </td>
                    <td>{{ contaRecebidaAtual.referencia }}</td>
                    <td v-if="configuracoes.comissaoPorContasRecebida">{{ contaRecebidaAtual.idComissao }}</td>
                    <td>{{ contaRecebidaAtual.numeroParcela }}/{{ contaRecebidaAtual.numeroMaximoParcelas }}</td>
                    <td>{{ contaRecebidaAtual.cliente.id }} - {{ contaRecebidaAtual.cliente.nome }}</td>
                    <td>{{ contaRecebidaAtual.formaPagamento }}</td>
                    <td>{{ contaRecebidaAtual.valorVencimento | moeda }}</td>
                    <td>{{ contaRecebidaAtual.dataVencimento | data }}</td>
                    <td>{{ contaRecebidaAtual.valorRecebimento | moeda }}</td>
                    <td>{{ contaRecebidaAtual.dataRecebimento | data }}</td>
                    <td>{{ contaRecebidaAtual.recebidaPor }}</td>
                    <td>{{ contaRecebidaAtual.numeroNfe }}</td>
                    <td>{{ contaRecebidaAtual.localizacao }}</td>
                    <td v-if="configuracoes.utilizarCnab">{{ contaRecebidaAtual.numeroArquivoRemessaCnab }}</td>
                    <td>
                        <input type="text" v-model="contaRecebida.observacao" style="width: 200px" />
                    </td>
                    <td v-if="configuracoes.exibirComissao">{{ contaRecebidaAtual.detalhamentoComissao }}</td>
                    <td>{{ contaRecebidaAtual.descricaoContaContabil }}</td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaContasRecebidas(false)" title="Imprimir">
                        <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaContasRecebidas(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
            <div>
                <span v-if="configuracoes.gerarArquivoGCon">
                    <a href="#" @click.prevent="gerarArquivoGCon(true)">
                        <img border="0" src="../Images/blocodenotas.png" /> Exportar para o GCON
                    </a>
                </span>
                <span v-if="configuracoes.gerarArquivoProsoft">
                    <a href="#" @click.prevent="gerarArquivoProsoft(true)">
                        <img border="0" src="../Images/blocodenotas.png" /> Exportar para o Prosoft
                    </a>
                </span>
                <span v-if="configuracoes.gerarArquivoDominio">
                    <a href="#" @click.prevent="gerarArquivoDominio(true)">
                        <img border="0" src="../Images/blocodenotas.png" /> Exportar para arquivo da Domínio Sistemas
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/ContasReceber/Componentes/LstContasRecebidas.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/ContasReceber/Componentes/LstContasRecebidas.js" />
        </Scripts>
    </asp:ScriptManager>




    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdConta" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="IdContaR" DataSourceID="odsContasReceber" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Nenhuma conta recebida encontrada." AllowPaging="True" AllowSorting="True"
                    OnRowDataBound="grdConta_RowDataBound">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit"
                                    Visible='<%# !(bool)Eval("IsParcelaCartao") %>'>
                                      <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CausesValidation="false" CommandName="Update"
                                    Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:PlaceHolder runat="server" ID="rptRef" Visible='<%# !String.IsNullOrEmpty(Eval("RelatorioPedido") as string) %>'>
                                    <a href="#" onclick="openRptUnico('<%# Eval("RelatorioPedido") %>');">
                                        <img border="0" src="../Images/Relatorio.gif" /></a> </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" ID="rptRec">
                                    <a href="#" onclick="openRptUnico('<%# Eval("UrlRelatorio") %>');">
                                        <img border="0" src="../Images/script_go.gif" title="Dados do recebimento" /></a>
                                </asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referência" SortExpression="Referencia">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Referencia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Comissão" SortExpression="IdComissao">
                            <ItemTemplate>
                                <asp:Label ID="Label22" runat="server" Text='<%# Bind("IdComissao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Parc." SortExpression="NumParcString">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("NumParcString") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente" SortExpression="IdNomeCli">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("IdNomeCli") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Forma Pagto." SortExpression="DescrFormaPagto">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DescrFormaPagto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVec">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("ValorVec", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data. Venc." SortExpression="DataVec">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("DataVec", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Rec." SortExpression="ValorRec">
                            <ItemTemplate>
                                <asp:Label ID="Label71" runat="server" Text='<%# Eval("ValorRec", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Rec." SortExpression="DataRec">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("DataRec", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Recebida por" SortExpression="NomeFunc">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("NomeFunc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num. NF" SortExpression="NumeroNFe">
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("NumeroNFe") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Localização" SortExpression="DestinoRec">
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("DestinoRec") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num. Arquivo Remessa" SortExpression="NumeroArquivoRemessaCnab">
                            <ItemTemplate>
                                <asp:Label ID="Label72" runat="server" Text='<%# Bind("NumeroArquivoRemessaCnab") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <ItemTemplate>
                                <asp:Label ID="lblObs" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                                <asp:Label ID="lblObsDescAcresc" runat="server" Text='<%# Bind("ObsDescAcresc") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Text='<%# Bind("Obs") %>'
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrComissao" HeaderText="Comissão" SortExpression="DescrComissao"></asp:BoundField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="DescricaoContaContabil">
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("DescricaoContaContabil") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("IdContaR") %>'
                                    Tabela="ContasReceber" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc4:ctrlLogPopup ID="ctrlLogContasReceber" runat="server" Tabela="ContasReceber" IdRegistro='<%# Eval("IdContaR") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Content>
