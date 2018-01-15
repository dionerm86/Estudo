<%@ Page Title="Beneficiamentos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstBenef.aspx.cs" Inherits="Glass.UI.Web.Listas.LstBenef" %>

<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Beneficiamento</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdBenef" runat="server" AllowPaging="True" 
                    AutoGenerateColumns="False" DataKeyNames="IdBenefConfig" DataSourceID="odsBenefConfig"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" PageSize="15" EmptyDataText="Nenhum beneficiamento cadastrado."
                    OnRowCommand="grdBenef_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadBenef.aspx?idBenefConfig=" + Eval("IdBenefConfig") %>'>
                                    <img border="0" src="../Images/EditarGrid.gif" alt="Editar" /></asp:HyperLink>
                                <asp:LinkButton ID="lnkExcluir" runat="server" CommandName="Delete" 
                                    OnClientClick="return confirm('Deseja excluir/inativar este beneficiamento?');" 
                                    Visible='<%# (string)Eval("Nome") != "Redondo" %>'>
                                     <img border="0" src="../Images/ExcluirGrid.gif" title="Excluir/Inativar" alt="Excluir" /></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:TemplateField HeaderText="Controle" SortExpression="TipoControle">
                            <ItemTemplate>
                                <%# Colosoft.Translator.Translate(Eval("TipoControle")).Format() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cálculo" SortExpression="TipoCalculo">
                            <ItemTemplate>
                                <%# Colosoft.Translator.Translate(Eval("TipoCalculo")).Format() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CodAplicacao" HeaderText="Aplicação" 
                            SortExpression="CodAplicacao" />
                        <asp:BoundField DataField="CodProcesso" HeaderText="Processo" 
                            SortExpression="CodProcesso" />
                        <asp:CheckBoxField DataField="CobrancaOpcional" HeaderText="Cobr. Opcional" SortExpression="CobrancaOpcional" />
                        <asp:CheckBoxField DataField="NaoExibirEtiqueta" HeaderText="Não exibir desc. na imp. etiqueta" 
                        SortExpression="NaoExibirEtiqueta" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="TipoBenef">
                            <ItemTemplate>
                                <%# Colosoft.Translator.Translate(Eval("TipoBenef")).Format() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgUp" runat="server" CommandArgument='<%# Eval("IdBenefConfig") %>'
                                    CommandName="Up" ImageUrl="~/Images/up.gif" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgDown" runat="server" CommandArgument='<%# Eval("IdBenefConfig") %>'
                                    CommandName="Down" ImageUrl="~/Images/down.gif" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                                    IdRegistro='<%# (uint)(int)Eval("IdBenefConfig") %>' Tabela="BenefConfig" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsBenefConfig" runat="server" 
                    SelectMethod="PesquisarConfiguracoesBeneficiamento" 
                    SelectByKeysMethod="ObtemBenefConfig"
                    TypeName="Glass.Global.Negocios.IBeneficiamentoFluxo"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SortParameterName="sortExpression"
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.BenefConfig" 
                    DeleteMethod="ApagarBenefConfig"
                    DeleteStrategy="GetAndDelete"></colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
