<%@ Page Title="Estoque de Produtos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="LstEstoque.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEstoque" EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Estoques/Templates/LstEstoques.Filtro.html")
    %>
    <div id="app">
        <estoque-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></estoque-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum estoque de produto encontrado." :numero-registros="20" :linha-editando="numeroLinhaEdicao">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('codigoInternoProduto')">Cód.</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricaoProduto')">Produto</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('descricaoGrupo')">Grupo/Subgrupo</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('estoqueMinimo')">Estoque mínimo</a>
                    </th>
                    <th v-if="!configuracoes.naoVendeVidro && !exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('estoqueM2')">m² em Estoque</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('quantidadeReserva')">Reserva</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal && configuracoes.usarLiberacaoPedido">
                        <a href="#" @click.prevent="ordenar('quantidadeLiberacao')">Liberação</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('quantidadeEstoque')">Qtd. em Estoque</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal">
                        <a href="#">Disponível</a>
                    </th>
                    <th v-if="!exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('quantidadeDefeito')">Qtd. com Defeito</a>
                    </th>
                    <th v-if="exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('quantidadeEstoqueFiscal')">Qtd. em Estoque Fiscal</a>
                    </th>
                    <th v-if="exibirEstoqueFiscal">
                        <a href="#" @click.prevent="ordenar('quantidadePosseTerceiros')">Qtde. Posse Terceiros</a>
                    </th>
                    <th v-if="exibirEstoqueFiscal">
                        <a href="#">Item em posse de</a>
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <a href="#" @click.prevent="editar(item, index)" title="Editar" v-if="item.permissoes.editar && numeroLinhaEdicao === -1">
                            <img border="0" src="../Images/Edit.gif">
                        </a>
                    </td>
                    <td>{{ item.codigoInternoProduto }}</td>
                    <td>{{ item.descricaoProduto }}</td>
                    <td>{{ item.descricaoGrupoProduto }} - {{ item.descricaoSubgrupoProduto }}</td>
                    <td>{{ item.estoqueMinimo }}</td>
                    <td v-if="!configuracoes.naoVendeVidro && !exibirEstoqueFiscal">{{ item.estoqueM2 | decimal }}{{ item.descricaoTipoCalculo }}</td>
                    <td v-if="!exibirEstoqueFiscal">
                        <a href="#" @click.prevent="abrirRelatorioReserva(item)" v-if="item.permissoes.exibirLinkReserva">
                            {{ item.quantidadeReseva }}
                        </a>
                    </td>
                    <td v-if="configuracoes.usarLiberacaoPedido && !exibirEstoqueFiscal">
                        <a href="#" @click.prevent="abrirRelatorioLiberacao(item)" v-if="item.permissoes.exibirLinkLiberacao">
                            {{ item.quantidadeLiberacao }}
                        </a>
                    </td>
                    <td v-if="!exibirEstoqueFiscal">{{ item.descricaoQuantidadeEstoque }}</td>
                    <td v-if="!exibirEstoqueFiscal">{{ item.descricaoEstoqueDisponivel }}</td>
                    <td v-if="!exibirEstoqueFiscal">{{ item.quantidadeDefeito }}</td>
                    <td v-if="exibirEstoqueFiscal">{{ item.quantidadeEstoqueFiscal }}</td>
                    <td v-if="exibirEstoqueFiscal">{{ item.quantidadePosseTerceiros }}</td>
                    <td v-if="exibirEstoqueFiscal">{{ item.descricaoTipoTerceiro }} {{ item.nomeTerceiro }}</td>
                    <td v-if="exibirEstoqueFiscal" style="white-space: nowrap">
                        <log-alteracao tabela="ProdutoLoja" :id-item="item.id" :atualizar-ao-alterar="false" v-if="item.permissoes.logAlteracoes"></log-alteracao>
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
                    <td>{{ estoqueProdutoAtual.codigoInternoProduto }}</td>
                    <td>{{ estoqueProdutoAtual.descricaoProduto }}</td>
                    <td>{{ estoqueProdutoAtual.descricaoGrupoProduto }} - {{ estoqueProdutoAtual.descricaoSubgrupoProduto }}</td>
                    <td>
                        <input type="number" v-model.number="contaRecebida.estoqueMinimo" style="width: 60px" />
                    </td>
                    <td v-if="!configuracoes.naoVendeVidro && !exibirEstoqueFiscal">
                        <input type="number" v-model.number="contaRecebida.estoqueM2" style="width: 60px" />
                        {{ item.descricaoTipoCalculo }}
                    </td>
                    <td v-if="!exibirEstoqueFiscal">
                        {{ estoqueProdutoAtual.quantidadeReserva }}
                    </td>
                    <td v-if="configuracoes.usarLiberacaoPedido && !exibirEstoqueFiscal">
                        <a href="#" @click.prevent="abrirRelatorioLiberacao(item)" v-if="item.permissoes.exibirLinkLiberacao">
                            {{ item.quantidadeLiberacao }}
                        </a>
                    </td>
                    <td v-if="!exibirEstoqueFiscal">
                        <input type="number" v-model.number="contaRecebida.quantidadeEstoque" style="width: 60px" />
                        {{ item.descricaoTipoCalculo }}
                    </td>
                    <td v-if="!exibirEstoqueFiscal">{{ item.descricaoEstoqueDisponivel }}</td>
                    <td v-if="!exibirEstoqueFiscal">
                        <input type="number" v-model.number="contaRecebida.quantidadeDefeito" style="width: 60px" />
                        {{ item.descricaoTipoCalculo }}
                    </td>
                    <td v-if="exibirEstoqueFiscal">
                        <input type="number" v-model.number="contaRecebida.quantidadeEstoqueFiscal" style="width: 60px" />
                    </td>
                    <td v-if="exibirEstoqueFiscal">
                        <input type="number" v-model.number="contaRecebida.quantidadePosseTerceiros" style="width: 60px" />
                        {{ item.descricaoTipoCalculo }}
                    </td>
                    <td v-if="exibirEstoqueFiscal">

                    </td>
                    <td v-if="exibirEstoqueFiscal">
                    </td>
                </template>
            </lista-paginada>
        </section>
        <section class="links">
            <div>
                <span>
                    <a href="#" @click.prevent="abrirListaEstoquesProduto(false)">
                        <img border="0" src="../Images/printer.png"> Imprimir
                    </a>
                </span>
                <span>
                    <a href="#" @click.prevent="abrirListaEstoquesProduto(true)">
                        <img border="0" src="../Images/Excel.gif"> Exportar para o Excel
                    </a>
                </span>
            </div>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Estoques/Componentes/LstEstoques.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Estoques/Componentes/LstEstoques.js" />
        </Scripts>
    </asp:ScriptManager>






    
    <table align="center">
        <tr>
            <td>
                <asp:CheckBox ID="chkInsercaoRapidaEstoque" runat="server" Text="Inserção Rápida de Estoque"
                    AutoPostBack="true" OnCheckedChanged="chkInsercaoRapidaEstoque_CheckedChanged" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdEstoque" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdProd,IdLoja"
                    DataSourceID="odsEstoque" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" PageSize="20" OnDataBound="grdEstoque_DataBound">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif"
                                    CausesValidation="false" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif"
                                    Visible='<%# Eval("EditVisible") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInternoProd">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("CodInternoProd") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("CodInternoProd") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Grupo/Subgrupo" SortExpression="DescrGrupoProd, DescrSubgrupoProd">
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("DescrGrupoSubgrupoProd") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("DescrGrupoSubgrupoProd") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Estoque mínimo" SortExpression="EstMinimo">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEstMinimo" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("EstMinimo") %>' Width="70px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("EstoqueMinimoString") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="M² em Estoque" SortExpression="M2">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# (Eval("M2") ?? "").ToString() + (Eval("DescrEstoque") ?? "").ToString() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtM2Estoque" runat="server" Text='<%# Bind("M2") %>' Width="70px"
                                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:Label ID="Label11" runat="server" Text='<%# Eval("DescrEstoque") %>'></asp:Label>
                                <asp:RequiredFieldValidator ID="rfvM2Estoque" runat="server" ControlToValidate="txtM2Estoque"
                                    Display="Dynamic" ErrorMessage="Campo não pode ser vazio."></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Reserva" SortExpression="Reserva">
                            <EditItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("Reserva") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkReserva" runat="server" OnClientClick='<%# "abrirReserva(" + Eval("IdProd") + "); return false" %>'
                                    Text='<%# Eval("ReservaString") %>' Visible='<%# Eval("TipoCalc").ToString() == "1" || Eval("TipoCalc").ToString() == "5" %>'></asp:LinkButton>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Liberação" SortExpression="Liberacao">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkLiberacao" runat="server" OnClientClick='<%# "abrirLiberacao(" + Eval("IdProd") + "); return false" %>'
                                    Text='<%# Eval("LiberacaoString") %>' Visible='<%# Eval("TipoCalc").ToString() == "1" || Eval("TipoCalc").ToString() == "5" %>'></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("Liberacao") %>'></asp:Label>
                            </EditItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtd. em Estoque" SortExpression="QtdEstoque">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtdEstoque" runat="server" Text='<%# Bind("QtdEstoque") %>'
                                    Width="70px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("DescrEstoque") %>'></asp:Label>
                                <asp:RequiredFieldValidator ID="rfvQtdEstoque" runat="server" ControlToValidate="txtQtdEstoque"
                                    Display="Dynamic" ErrorMessage="Campo não pode ser vazio."></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("QtdEstoqueStringLabel") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Disponível">
                            <EditItemTemplate>
                                <asp:Label ID="Label20" runat="server" Text='<%# Eval("EstoqueDisponivel") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label20" runat="server" Text='<%# Bind("EstoqueDisponivel") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtd. em Estoque Fiscal" SortExpression="EstoqueFiscal">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtdEstoqueFiscal" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("EstoqueFiscal") %>' Width="70px"></asp:TextBox>
                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("DescrEstoque") %>'></asp:Label>
                                <asp:RequiredFieldValidator ID="rfvQtdEstoqueFiscal" runat="server" ControlToValidate="txtQtdEstoqueFiscal"
                                    Display="Dynamic" ErrorMessage="Campo não pode ser vazio."></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("EstoqueFiscal") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtd. com Defeito" SortExpression="Defeito">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtdDefeito" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("Defeito") %>' Width="70px"></asp:TextBox>
                                <asp:Label ID="Label12" runat="server" Text='<%# Eval("DescrEstoque") %>'></asp:Label>
                                <asp:RequiredFieldValidator ID="rfvQtdDefeito" runat="server" ControlToValidate="txtQtdDefeito"
                                    Display="Dynamic" ErrorMessage="Campo não pode ser vazio."></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("Defeito") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde. Posse Terceiros" SortExpression="QtdePosseTerceiros">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtdePosseTerc" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("QtdePosseTerceiros") %>' Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("QtdePosseTerceiros") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Item em posse de">
                            <EditItemTemplate>
                                <uc2:ctrlSelParticipante ID="ctrlSelParticipante1" runat="server" IdCliente='<%# Bind("IdCliente") %>'
                                    IdFornec='<%# Bind("IdFornec") %>' IdLoja='<%# Bind("IdLojaTerc") %>' IdTransportador='<%# Bind("IdTransportador") %>'
                                    IdAdminCartao='<%# Bind("IdAdminCartao") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label21" runat="server" Font-Italic="True" Text='<%# Eval("DescrTipoPart") %>'></asp:Label>
                                <asp:Label ID="Label22" runat="server" Text='<%# Eval("DescrPart") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <%-- <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# Eval("IdLog") %>'
                                    Tabela="ProdutoLoja" /> --%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfCodProduto" runat="server" Value='<%# Eval("IdLoja") + "," + Eval("IdProd") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtd. em Estoque" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtQtdEstoqueInsercaoRapida" runat="server" Text='<%# Bind("QtdEstoque") %>'
                                    Width="70px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvQtdEstoqueInsercaoRapida" runat="server" ControlToValidate="txtQtdEstoqueInsercaoRapida"
                                    Display="Dynamic" ErrorMessage="Campo deve ser preenchido."></asp:RequiredFieldValidator>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtd. em Estoque Fiscal" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtQtdEstoqueFiscalInsercaoRapida" runat="server" Text='<%# Bind("EstoqueFiscal") %>'
                                    Width="70px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvQtdEstoqueFiscalInsercaoRapida" runat="server"
                                    ControlToValidate="txtQtdEstoqueFiscalInsercaoRapida" Display="Dynamic" ErrorMessage="Campo deve ser preenchido."></asp:RequiredFieldValidator>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="ProdutoLoja" Campo="Qtde. Posse Terceiros" IdRegistro='<%# Eval("IdLog") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button runat="server" ID="btnSalvarInsercaoRapida" Text="Salvar" Width="100"
                    Visible="false" OnClick="SalvarInsercaoRapida" OnClientClick="bloquearPagina(); desbloquearPagina(false);" />
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEstoque" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectMethod="GetForEstoque" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
        TypeName="Glass.Data.DAL.ProdutoLojaDAO" DataObjectTypeName="Glass.Data.Model.ProdutoLoja"
        SelectCountMethod="GetForEstoqueCount" UpdateMethod="AtualizaEstoque" 
        OnUpdated="odsEstoque_Updated" >
        <SelectParameters>
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtCodProd" Name="codInternoProd" PropertyName="Text"
                Type="String" />
            <asp:ControlParameter ControlID="txtDescr" Name="descricao" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpGrupo" Name="idGrupoProd" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter ControlID="drpSubgrupo" Name="idSubgrupoProd" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter ControlID="chkComEstoque" DefaultValue="false" Name="exibirApenasComEstoque"
                PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="chkApenasPosseTerceiros" DefaultValue="" Name="exibirApenasPosseTerceiros"
                PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="chkApenasProdutosProjeto" 
                Name="exibirApenasProdutosProjeto" PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorVidro" PropertyName="IdCorVidro"
                Type="UInt32" />
            <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorFerragem" PropertyName="IdCorFerragem"
                Type="UInt32" />
            <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorAluminio" PropertyName="IdCorAluminio"
                Type="UInt32" />
            <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                Type="Int32" />
            <asp:QueryStringParameter Name="estoqueFiscal" QueryStringField="fiscal" Type="Int32" />
            <asp:ControlParameter ControlID="chkAguardSaidaEstoque" Name="aguardandoSaidaEstoque"
                PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="drpOrdenar" Name="ordenacao" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoProd" runat="server" SelectMethod="GetForFilter"
        TypeName="Glass.Data.DAL.GrupoProdDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupoProd" runat="server" SelectMethod="GetForFilter"
        TypeName="Glass.Data.DAL.SubgrupoProdDAO">
        <SelectParameters>
            <asp:ControlParameter ControlID="drpGrupo" Name="idGrupo" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
