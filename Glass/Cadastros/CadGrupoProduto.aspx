<%@ Page Title="Grupos de Produto" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadGrupoProduto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadGrupoProduto" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function exibirLog(idGrupo) {
            TagToTip("log_" + idGrupo, FADEIN, 300, COPYCONTENT, false, TITLE, 'Alterações no tipo cálculo', CLOSEBTN, true, CLOSEBTNTEXT, 'Fechar',
                CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true)
        }


        function openRpt(exportarExcel) {

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaSubgrupo&exportarExcel=" + exportarExcel);
        
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdGrupoProd" runat="server" SkinID="gridViewEditable"
                              DataSourceID="odsGrupoProd" DataKeyNames="IdGrupoProd" PageSize="15" OnRowCommand="grdGrupoProd_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif" alt="Editar" /></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" Visible='<%# !(bool)Eval("GrupoSistema") %>' OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Grupo de Produto?&quot;);" />
                                <asp:HyperLink ID="lnkSubgrupo" runat="server" NavigateUrl='<%# "CadSubgrupoProduto.aspx?IdGrupoProd=" + Eval("IdGrupoProd") + "&DescrGrupo=" + Eval("Descricao") %>'>
                                    <img src="../Images/subgrupo.png" border="0" alt="Subgrupos" /></asp:HyperLink>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="btnAtivarProdutos" runat="server" CommandName="AtivarProdutos" CommandArgument='<%# Eval("IdGrupoProd") %>' ImageUrl="~/Images/Ok.gif"
                                    ToolTip="Ativar todos os produtos deste grupo" OnClientClick="return confirm(&quot;Tem certeza que deseja ativar todos os produtos deste Grupo?&quot;);" />
                                <asp:ImageButton ID="btnInvativarProdutos" runat="server" CommandName="InativarProdutos" CommandArgument='<%# Eval("IdGrupoProd") %>' ImageUrl="~/Images/Inativar.gif"
                                    ToolTip="Inativar todos os produtos deste grupo" OnClientClick="return confirm(&quot;Tem certeza que deseja inativar todos os produtos deste Grupo?&quot;);" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código" SortExpression="IdGrupoProd">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdGrupoProd") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdGrupoProd") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricaoIns" runat="server" MaxLength="150" Width="150px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cálculo" SortExpression="TipoCalculo">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpCalculo" runat="server" SelectedValue='<%# Bind("TipoCalculo") %>'
                                    AppendDataBoundItems="True" DataSourceID="odsTipoCalc" DataTextField="Translation"
                                    DataValueField="Key">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpCalculoIns" runat="server" DataSourceID="odsTipoCalc"
                                    DataTextField="Translation" DataValueField="key">
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("TipoCalculo")).Format() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cálculo NF" SortExpression="TipoCalculoNf">
                            <FooterTemplate>
                                <asp:DropDownList ID="drpCalculoNfIns" runat="server" DataSourceID="odsTipoCalcNf"
                                    DataTextField="Translation" DataValueField="Key" >
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("TipoCalculoNf")).Format() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpCalculoNf" runat="server" SelectedValue='<%# Bind("TipoCalculoNf") %>'
                                    AppendDataBoundItems="True" DataSourceID="odsTipoCalcNf" DataTextField="Translation"
                                    DataValueField="Key">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Bloquear Estoque *" SortExpression="BloquearEstoque">
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkBloquearEstoque" runat="server" Checked='<%# Bind("BloquearEstoque") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkBloquearEstoque" runat="server" Checked="false" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkBloquearEstoque" runat="server" Checked='<%# Bind("BloquearEstoque") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Alterar Estoque" SortExpression="NaoAlterarEstoque">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkAlterarEstoque" runat="server" Enabled="false" Checked='<%# Eval("AlterarEstoque") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkAlterarEstoque" runat="server" Checked='<%# Bind("AlterarEstoque") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkAlterarEstoque" runat="server" Checked="true" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Alterar Estoque Fiscal" SortExpression="NaoAlterarEstoqueFiscal">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkAlterarEstoqueFiscal" runat="server" Enabled="false" Checked='<%# Eval("AlterarEstoqueFiscal") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkAlterarEstoqueFiscal" runat="server" Checked='<%# Bind("AlterarEstoqueFiscal") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkAlterarEstoqueFiscal" runat="server" Checked="true" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Exibir Mensagem Estoque" SortExpression="ExibirMensagemEstoque">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkExibirMensagemEstoque" runat="server" Checked='<%# Bind("ExibirMensagemEstoque") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkExibirMensagemEstoque" runat="server" Checked='<%# Bind("ExibirMensagemEstoque") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkExibirMensagemEstoque" runat="server" Checked="True" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Gera Volume?" SortExpression="GeraVolume">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkGeraVolume" runat="server" Checked='<%# Bind("GeraVolume") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkGeraVolume" runat="server" Checked='<%# Bind("GeraVolume") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkGeraVolume" runat="server" Checked="false" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo de Grupo" SortExpression="TipoGrupo">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("TipoGrupo")) %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="DropDownList1" runat="server" 
                                    DataSourceID="odsTiposGrupo" DataTextField="Translation" DataValueField="Key"
                                    SelectedValue='<%# Bind("TipoGrupo") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoGrupo" runat="server" 
                                                  DataSourceID="odsTiposGrupo" DataTextField="Translation" DataValueField="Key">
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>                       
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="GrupoProduto" IdRegistro='<%# (uint)(int)Eval("IdGrupoProd") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                </td> </tr>
                                <tr>
                                    <td colspan="3">
                                    </td>
                                    <td colspan="9" align="left" style="color: Red">
                                        os valores entre parênteses do "<%= Glass.Global.CalculosFluxo.NOME_MLAL%>" são valores
                                        de arredondamento para cálculo do valor
                                    </td>
                                </tr>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">
                                    <img border="0" src="../Images/insert.gif" alt="Inserir" /></asp:LinkButton>
                                </td> </tr>
                                <tr>
                                    <td colspan="3">
                                    </td>
                                    <td colspan="9" align="left" style="color: Red">
                                        os valores entre parênteses do "<%# Glass.Global.CalculosFluxo.NOME_MLAL %>" são valores
                                        de arredondamento para cálculo do valor
                                    </td>
                                </tr>
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <colo:VirtualObjectDataSource runat="server" ID="odsTiposGrupo"
                    TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoGrupoProd, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>

                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoProd" runat="server" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.GrupoProd"
                    DeleteMethod="ApagarGrupoProduto" EnablePaging="True" 
                    DeleteStrategy="GetAndDelete"
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarGruposProduto" SortParameterName="sortExpression"
                    SelectByKeysMethod="ObtemGrupoProduto"
                    TypeName="Glass.Global.Negocios.IGrupoProdutoFluxo"
                    UpdateMethod="SalvarGrupoProduto"
                    UpdateStrategy="GetAndUpdate">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCalc" runat="server" 
                    SelectMethod="ObtemTiposCalculo"
                    TypeName="Glass.Global.UI.Web.Process.GrupoProdutoDataSource">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="true" Name="exibirDecimal" Type="Boolean" />
                        <asp:Parameter DefaultValue="false" Name="notaFiscal" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCalcNf" runat="server" 
                    SelectMethod="ObtemTiposCalculo"
                    TypeName="Glass.Global.UI.Web.Process.GrupoProdutoDataSource">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="true" Name="exibirDecimal" Type="Boolean" />
                        <asp:Parameter DefaultValue="true" Name="notaFiscal" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>

                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCliente" runat="server" DataObjectTypeName="Glass.Data.Model.Cliente"
                    SelectMethod="GetForSel" TypeName="Glass.Data.DAL.ClienteDAO">                    
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                * Bloqueia venda dos produtos deste grupo se não houver em estoque
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" 
                    src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"> <img 
                    border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
