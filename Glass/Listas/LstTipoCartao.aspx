<%@ Page Title="Tipos de Cartão" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstTipoCartao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstTipoCartao" %>

<%@ Register Src="~/Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function onSave(insert) {
            var descricao = FindControl(insert ? "txtDescricaoIns" : "txtDescricao", "input").value;

            if (descricao == "") {
                alert("Informe a descrição.");
                return false;
            }
        }

    </script>

    <table>
        <tr><td>&nbsp;</td></tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdTipoCartao" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    DataSourceID="odsTipoCartao" ShowFooter="True" DataKeyNames="IdTipoCartao">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                               <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadTipoCartao.aspx?idTipoCartao=" + Eval("IdTipoCartao") %>'>
                                    <img border="0" src="../Images/gear.gif" /></asp:HyperLink>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" Visible='<%# Eval("PodeExcluir") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onSave(false);" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="IdTipoCartao">
                            <EditItemTemplate>
                                <asp:Label ID="IdTipoCartao" runat="server" Text='<%# Bind("IdTipoCartao") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="IdTipoCartao" runat="server" Text='<%# Bind("IdTipoCartao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Operadora" SortExpression="Operadora">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpOperadora" runat="server" DataSourceID="odsOperadoraCartao"
                                    DataTextField="Descricao" DataValueField="IdOperadoraCartao" SelectedValue='<%# Bind("Operadora") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblOperadora" runat="server" Text='<%# Eval("DescOperadora") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpOperadora" runat="server" DataSourceID="odsOperadoraCartao"
                                    DataTextField="Descricao" DataValueField="IdOperadoraCartao">
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Bandeira" SortExpression="Bandeira">
                            <ItemTemplate>
                                <asp:Label ID="lblBandeira" runat="server" Text='<%# Eval("DescBandeira") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpBandeira" runat="server" DataSourceID="odsBandeiraCartao"
                                    DataTextField="Descricao" DataValueField="IdBandeiraCartao" SelectedValue='<%# Bind("Bandeira") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpBandeira" runat="server" DataSourceID="odsBandeiraCartao"
                                    DataTextField="Descricao" DataValueField="IdBandeiraCartao">
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="Tipo">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" DataSourceID="odsTipo"
                                    DataTextField="Translation" DataValueField="Key" SelectedValue='<%# Bind("Tipo") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" DataSourceID="odsTipo"
                                    DataTextField="Translation" DataValueField="Key">
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblTipo" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Tipo")).Format() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopupTipoCartaoParc" runat="server" Tabela="TipoCartao" IdRegistro='<%# (uint)(int)Eval("IdTipoCartao") %>'/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClientClick="return onSave(true);"
                                    OnClick="lnkInserir_Click"><img border="0" src="../Images/insert.gif" /></asp:LinkButton>
                            </FooterTemplate>
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
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoCartao" runat="server" DeleteMethod="Delete" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" OnDeleted="odsTipoCartao_Deleted" SelectCountMethod="GetCount"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO" DataObjectTypeName="Glass.Data.Model.TipoCartaoCredito"
                    UpdateMethod="Update">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource runat="server" ID="odsTipo"
                    SelectMethod="GetTranslatesFromTypeName" EnableViewState="false"
                    TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" Type="String"
                            DefaultValue="Glass.Data.Model.TipoCartaoEnum, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource runat="server" ID="odsOperadoraCartao" Culture="pt-BR"
                    SelectMethod="PesquisarOperadoraCartaoPelaSituacao" TypeName="Glass.Data.DAL.OperadoraCartaoDAO">
                    <SelectParameters>
                        <asp:Parameter Name="situacao" Type="Int32" DefaultValue="1" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource runat="server" ID="odsBandeiraCartao" Culture="pt-BR"
                    SelectMethod="PesquisarBandeiraCartaoPelaSituacao" TypeName="Glass.Data.DAL.BandeiraCartaoDAO">
                    <SelectParameters>
                        <asp:Parameter Name="situacao" Type="Int32" DefaultValue="1" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
