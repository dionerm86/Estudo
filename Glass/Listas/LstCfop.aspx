<%@ Page Title="CFOP" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstCfop.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCfop" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function onSave(insert) {
            var descricao = FindControl(insert ? "txtDescricaoIns" : "txtDescricao", "input").value;
            var codInterno = FindControl(insert ? "txtCodInternoIns" : "txtCodInterno", "input").value;

            if (descricao == "") {
                alert("Informe a descrição.");
                return false;
            }

            if (codInterno == "") {
                alert("Informe o código.");
                return false;
            }
        }

        function openRpt(exportarExcel) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=ListaCfop&exportarExcel=" + exportarExcel);
            return false;
        }

        function alteraNaturezaOperacao(idCfop) {
            openWindow(600, 800, "LstNaturezaOperacao.aspx?idCfop=" + idCfop);
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cod. CFOP" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodCfop" runat="server" onkeypress="return event.charCode >= 48 && event.charCode <= 57"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtFiltroDescricao" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdCfop" runat="server" SkinID="gridViewEditable"
                              DataSourceID="odsCfop" DataKeyNames="IdCfop" EnableViewState="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm('Tem certeza que deseja excluir este CFOP?');" />
                                <asp:HyperLink ID="lnkNaturezaOperacao" runat="server" ImageUrl="~/Images/Subgrupo.png"
                                    NavigateUrl='<%# Eval("IdCfop", "~/Listas/LstNaturezaOperacao.aspx?idCfop={0}") %>'
                                    OnClick='<%# Eval("IdCfop", "alteraNaturezaOperacao({0}); return false;") %>'
                                    ToolTip="Naturezas de Operação"></asp:HyperLink>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onSave(false);" CausesValidation="true" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código" SortExpression="CodInterno">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodInterno" runat="server" MaxLength="20" Text='<%# Bind("CodInterno") %>'
                                    Width="70px" onkeypress="return event.charCode >= 48 && event.charCode <= 57"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodInternoIns" runat="server" MaxLength="20" Width="70px" onkeypress="return event.charCode >= 48 && event.charCode <= 57"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="53" Text='<%# Bind("Descricao") %>'
                                    Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricaoIns" runat="server" MaxLength="53"
                                    Width="300px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="IdTipoCfop">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoCfopEdit" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoCfop"
                                    DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdTipoCfop") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoCfop" runat="server" DataSourceID="odsTipoCfop" DataTextField="Name"
                                    DataValueField="Id">
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Tipo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Mercadoria" SortExpression="TipoMercadoria">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("TipoMercadoria")).Format() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoMercadoria" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsTipoMercadoria" DataTextField="Text" DataValueField="Key" SelectedValue='<%# Bind("TipoMercadoria") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoMercadoria" runat="server" DataSourceID="odsTipoMercadoria"
                                                  DataTextField="Text" DataValueField="Key" AppendDataBoundItems="true">
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Alterar Estoque Terceiros" SortExpression="AlterarEstoqueTerceiros">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkAlterarEstoqueTerceiros" runat="server" Enabled="false" Checked='<%# Eval("AlterarEstoqueTerceiros") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkAlterarEstoqueTerceiros" runat="server" Checked='<%# Bind("AlterarEstoqueTerceiros") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkAlterarEstoqueTerceiros" runat="server" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Alterar Estoque de Vidros de Cliente" SortExpression="AlterarEstoqueCliente">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkAlterarEstoqueCliente" runat="server" Checked='<%# Bind("AlterarEstoqueCliente") %>'
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkAlterarEstoqueCliente" runat="server" Checked='<%# Bind("AlterarEstoqueCliente") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkAlterarEstoqueCliente" runat="server" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Rows="1" Text='<%# Bind("Obs") %>'
                                    TextMode="MultiLine" Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Rows="1" TextMode="MultiLine"
                                    Width="200px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClientClick="return onSave(true);"
                                    OnClick="lnkInserir_Click"><img border="0" src="../Images/insert.gif" /></asp:LinkButton>
                            </FooterTemplate>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# (uint)(int)Eval("IdCfop") %>'
                                    Tabela="Cfop" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCfop" runat="server" 
                    DeleteMethod="ApagarCfop" EnableViewState="false" EnableCaching="false"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarCfops" 
                    SortParameterName="sortExpression"
                    TypeName="Glass.Fiscal.Negocios.ICfopFluxo" DataObjectTypeName="Glass.Fiscal.Negocios.Entidades.CfopPesquisa"
                    UpdateMethod="SalvarCfop">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodCfop" Name="codInterno" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtFiltroDescricao" Name="descricao" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource 
                    Culture="pt-BR" ID="odsTipoCfop" runat="server" SelectMethod="ObtemTiposCfop"
                    TypeName="Glass.Fiscal.Negocios.ICfopFluxo">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource 
                    Culture="pt-BR" ID="odsTipoMercadoria" runat="server"
                    SelectMethod="GetTranslatesFromTypeName"
                    TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" Type="String"
                             DefaultValue="Glass.Data.Model.TipoMercadoria, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
