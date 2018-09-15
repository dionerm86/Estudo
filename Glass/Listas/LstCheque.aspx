<%@ Page Title="Cheques" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstCheque.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCheque" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Cheques/Templates/LstCheque.Filtro.html")
    %>
    <div id="app">
        <cheques-filtros :filtro.sync="filtro"></cheques-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum cheque encontrado." :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#">Referência</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('loja')">Loja</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('fornecedor')">Fornecedor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('numero')">Número</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('banco')">Banco</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('agencia')">Agência</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('conta')">Conta</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('titular')">Titular</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cpfCnpj')">CPF/CNPJ</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('valor')">Valor</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataVencimento')">Data vencimento</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('observacao')">Obervação</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('situacao')">Situação</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="editar(item, index)" title="Editar" v-if="numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/Edit.gif">
                        </a>
                        <a href="#" @click.prevent="abrirAnexos(item)" title="Anexos" v-if="numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/Clipe.gif">
                        </a>
                    </td>
                    <td>{{ item.referencia }}</td>
                    <td>
                        <label v-if="item.loja">
                            {{ item.loja.nome }}
                        </label>
                    </td>
                    <td>
                        <label v-if="item.cliente">
                            {{ item.cliente.id }} - {{ item.cliente.nome }}
                        </label>
                    </td>
                    <td>
                        <label v-if="item.fornecedor">
                            {{ item.fornecedor.id }} - {{ item.fornecedor.nome }}
                        </label>
                    </td>
                    <td>
                        {{ item.numeroCheque }}
                        <label v-if="item.digitoNumeroCheque">
                             - {{ item.digitoNumeroCheque }}
                        </label>
                    </td>
                    <td>{{ item.banco }}</td>
                    <td>{{ item.agencia }}</td>
                    <td>{{ item.conta }}</td>
                    <td>{{ item.titular }}</td>
                    <td>{{ item.cpfCnpj }}</td>
                    <td>{{ item.valoRecebido }}</td>
                    <td>
                        {{ item.dataVencimento | Data }}
                        <label v-if="item.dataVencimentoOriginal">
                            <br />(Data Venc. Orig. {{ item.dataVencimentoOriginal | Data }})
                        </label>
                    </td>
                    <td>{{ item.observacao }}</td>
                    <td>{{ item.situacao }}</td>
                    <td>
                        <a href="#" @click.prevent="cancelarReapresentacao(item)" v-if="numeroLinhaEdicao === -1 && item.permissoes.cancelarReapresentacao">
                            Cancelar reapresentação
                        </a>
                        <a href="#" @click.prevent="cancelarDevolucao(item)" v-if="numeroLinhaEdicao === -1 && item.permissoes.cancelarDevolucao">
                            Cancelar devolução
                        </a>
                        <a href="#" @click.prevent="cancelarProtesto(item)" v-if="numeroLinhaEdicao === -1 && item.permissoes.cancelarProtesto">
                            Cancelar protesto
                        </a>
                        <a href="#" @click.prevent="abrirLocalizacaoCheque(item)" title="Localização" v-if="numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/environment.gif" />
                        </a>
                        <log-alteracao tabela="Cheque" :id-item="item.id" :atualizar-ao-alterar="false" v-if="numeroLinhaEdicao === -1 && item.permissoes.logAlteracoes"></log-alteracao>
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
                    <td>{{ chequeAtual.referencia }}</td>
                    <td>
                        <label v-if="chequeAtual.loja">
                            {{ chequeAtual.loja.nome }}
                        </label>
                    </td>
                    <td>
                        <label v-if="chequeAtual.cliente">
                            {{ chequeAtual.cliente.id }} - {{ chequeAtual.cliente.nome }}
                        </label>
                    </td>
                    <td>
                        <label v-if="chequeAtual.fornecedor">
                            {{ chequeAtual.fornecedor.id }} - {{ chequeAtual.fornecedor.nome }}
                        </label>
                    </td>
                    <td>
                        <span v-if="chequeAtual.permissoes.alterarDadosCheque">
                            <input type="number" v-model.number="cheque.numeroCheque" style="width: 50px" />
                            <input type="text" v-model="cheque.digitoNumeroCheque" style="width: 10px" maxlength="1" />
                        </span>
                        <span v-else>
                            {{ chequeAtual.numeroCheque }}
                            <label v-if="chequeAtual.digitoNumeroCheque">
                                 - {{ chequeAtual.digitoNumeroCheque }}
                            </label>
                        </span>
                    </td>
                    <td>
                        <span v-if="chequeAtual.permissoes.alterarDadosCheque">
                            <input type="text" v-model="cheque." style="width: 50px" />
                        <span>
                        <span v-else>
                            {{ chequeAtual.banco }}
                        </span>
                    </td>
                    <td>
                        <span v-if="chequeAtual.permissoes.alterarDadosCheque">
                            <input type="text" v-model="cheque.agencia" style="width: 50px" />
                        <span>
                        <span v-else>
                            {{ chequeAtual.agencia }}
                        </span>
                    </td>
                    <td>
                        <span v-if="chequeAtual.permissoes.alterarDadosCheque">
                            <input type="text" v-model="cheque.conta" style="width: 50px" />
                        <span>
                        <span v-else>
                            {{ chequeAtual.conta }}
                        </span>
                    </td>
                    <td>
                        <input type="text" v-model="cheque.titular" style="width: 200px" maxlength="45" />
                    </td>
                    <td>{{ chequeAtual.cpfCnpj }}</td>
                    <td>{{ chequeAtual.valoRecebido }}</td>
                    <td>
                        <span v-if="chequeAtual.permissoes.alterarDataVencimento">
                            <campo-data-hora :data-hora.sync="cheque.dataVencimento"></campo-data-hora>
                        </span>
                        <span v-else>
                            {{ chequeAtual.dataVencimento | Data }}
                            <label v-if="chequeAtual.dataVencimentoOriginal">
                                <br />(Data Venc. Orig. {{ chequeAtual.dataVencimentoOriginal | Data }})
                            </label>
                        </span>
                    </td>
                    <td>
                        <input type="text" v-model="cheque.obervacao" style="width: 200px" maxlength="300" />
                    </td>
                    <td>{{ chequeAtual.situacao }}</td>
                    <td></td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaCheques(false)" title="Imprimir">
                        <img border="0" src="../Images/printer.png" /> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaCheques(true)" title="Exportar para o Excel">
                        <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Cheques/Componentes/LstCheques.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Cheques/Componentes/LstCheques.js" />
        </Scripts>
    </asp:ScriptManager>


















    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCheque" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    DataKeyNames="IdCheque" DataSourceID="odsCheques" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum cheque encontrado."
                    OnRowCommand="grdCheque_RowCommand">
                    <PagerSettings PageButtonCount="30" />
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="ImageButton4" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="ImageButton5" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                                <asp:HiddenField ID="hdfTipo" runat="server" Value='<%# Bind("Tipo") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referência" SortExpression="Referencia">
                            <EditItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("Referencia") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Loja" SortExpression="IdLoja">
                            <EditItemTemplate>
                                <asp:Label ID="Label24" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente">
                            <EditItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("IdNomeCliente") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fornecedor" SortExpression="IdFornecedor">
                            <EditItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Eval("IdNomeFornecedor") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num." SortExpression="Num">
                            <EditItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("Num") %>' Visible='<%# !(bool)Eval("EditarAgenciaConta") %>'></asp:Label>
                                <asp:TextBox ID="txtNum" runat="server"  Visible='<%# Eval("EditarAgenciaConta") %>' onchange="FindControl('hdfNum', 'input').value = this.value"
                                    Text='<%# Eval("Num") %>' Width="50px"></asp:TextBox>
                                <asp:TextBox ID="txtDigitoNum" runat="server" onchange="FindControl('hdfDigitoNum', 'input').value = this.value"
                                    Text='<%# Eval("DigitoNum") %>'  Visible='<%# Eval("EditarAgenciaConta") %>' Width="10px" MaxLength="1" ></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Banco" SortExpression="Banco">
                            <EditItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("Banco") %>' Visible='<%# !(bool)Eval("EditarAgenciaConta") %>'></asp:Label>
                                  <asp:TextBox ID="txtBanco" runat="server"  Visible='<%# Eval("EditarAgenciaConta") %>' onchange="FindControl('hdfBanco', 'input').value = this.value"
                                       Text='<%# Eval("Banco") %>'  Width="50px"></asp:TextBox>
                                <asp:HiddenField ID="hdfBanco" runat="server" Value='<%# Bind("Banco") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Agência" SortExpression="Agencia">
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("Agencia") %>' Visible='<%# !(bool)Eval("EditarAgenciaConta") %>'></asp:Label>
                                <asp:TextBox ID="txtAgencia" runat="server" onchange="FindControl('hdfAgencia', 'input').value = this.value"
                                    Text='<%# Eval("Agencia") %>' Visible='<%# Eval("EditarAgenciaConta") %>' Width="50px"></asp:TextBox>
                                <asp:HiddenField ID="hdfAgencia" runat="server" Value='<%# Bind("Agencia") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Conta" SortExpression="Conta">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("Conta") %>' Visible='<%# !(bool)Eval("EditarAgenciaConta") %>'></asp:Label>
                                <asp:TextBox ID="txtConta" runat="server" onchange="FindControl('hdfConta', 'input').value = this.value"
                                    Text='<%# Eval("Conta") %>' Visible='<%# Eval("EditarAgenciaConta") %>' Width="70px"></asp:TextBox>
                                <asp:HiddenField ID="hdfConta" runat="server" Value='<%# Bind("Conta") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Titular" SortExpression="Titular">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtTitular" runat="server" MaxLength="45" Text='<%# Bind("Titular") %>'
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CPF/CNPJ" SortExpression="CpfCnpj">
                            <EditItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("CpfCnpjFormatado") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <EditItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("ValorRecebido") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Venc." SortExpression="DataVenc">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDataVencGrid" runat="server" onkeypress="return false;" Text='<%# Bind("DataVenc") %>'
                                    Width="70px" Visible='<%# Eval("AlterarDataVenc") %>'></asp:TextBox>
                                <asp:ImageButton ID="imgDataVencGrid" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                    ToolTip="Alterar" OnClientClick="return SelecionaData('txtDataVencGrid', this)"
                                    Visible='<%# Eval("AlterarDataVenc") %>' />
                                <asp:Label ID="lblDataVenc" runat="server" Text='<%# Eval("DataVencLista") %>' Visible='<%# !(bool)Eval("AlterarDataVenc") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Text='<%# Bind("Obs") %>'
                                    Width="200px" Rows="2" TextMode="MultiLine"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="DescrSituacao">
                            <EditItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("DescrSituacao") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>

                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
