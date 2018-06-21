<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlConfigFaixasRentabilidadeLiberacao.ascx.cs" 
    Inherits="Glass.UI.Web.Controls.ctrlConfigFaixasRentabilidadeLiberacao"%>
<%@ Register Src="~/Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:GridView ID="grdFaixasRentabilidadeLiberacao" runat="server" SkinID="gridViewEditable"
    DataSourceID="odsFaixaRentabilidadeLiberacao" DataKeyNames="IdFaixaRentabilidadeLiberacao"
    AllowPaging="false" AllowSorting="false"> 
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                <img border="0" src="../Images/Edit.gif" alt="Editar" /></asp:LinkButton>
                <asp:LinkButton ID="lnkExcluir" runat="server" CommandName="Delete"
                    OnClientClick="return confirm(&quot;Tem certeza que deseja excluir esta faixa?&quot;);"
                    ToolTip="Excluir">
                <img border="0" src="../Images/ExcluirGrid.gif" alt="Excluir" /></asp:LinkButton>
                <asp:HiddenField runat="server" ID="hdfId" Value='<%# Eval("IdFaixaRentabilidadeLiberacao") %>' />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                    ToolTip="Cancelar" />
            </EditItemTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Percentual Rentabilidade (%)" ItemStyle-HorizontalAlign="Right">
            <EditItemTemplate>
                <asp:TextBox ID="txtPercentualRentabilidade" runat="server" Text='<%# Bind("PercentualRentabilidade") %>'></asp:TextBox>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="lblPercentualRentabilidade" runat="server" Text='<%# Eval("PercentualRentabilidade") %>'></asp:Label>
            </ItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txtPercentualRentabilidade" runat="server" MaxLength="150" Width="150px"></asp:TextBox>
            </FooterTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Requer Liberação" SortExpression="">
            <EditItemTemplate>
                <asp:CheckBox ID="chkRequerLiberacao" runat="server" Checked='<%# Bind("RequerLiberacao") %>' />
            </EditItemTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="chkRequerLiberacao" runat="server" Checked='<%# Bind("RequerLiberacao") %>'
                    Enabled="False" />
            </ItemTemplate>
            <FooterTemplate>
                <asp:CheckBox ID="chkRequerLiberacao" runat="server" Checked="false" />
            </FooterTemplate>
            <FooterStyle HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Tipos Funcionário" ItemStyle-HorizontalAlign="Right">
            <EditItemTemplate>
                <sync:CheckBoxListDropDown 
                    ID="drpTiposFuncionario" runat="server" DataSourceID="odsTiposFuncionario"
                    DataTextField="Name" DataValueField="Id"
                    SelectedValues='<%# ((Glass.Rentabilidade.Negocios.Entidades.FaixaRentabilidadeLiberacao)Page.GetDataItem()).TiposFuncionarioFaixa.Select(f => f.IdTipoFuncionario).ToArray() %>'>
                </sync:CheckBoxListDropDown>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="lblTiposFuncionario" runat="server" Text='<%# string.Join(",", ((Glass.Rentabilidade.Negocios.Entidades.FaixaRentabilidadeLiberacao)Page.GetDataItem()).TiposFuncionario.Select(f => f.Descricao)) %>'></asp:Label>
            </ItemTemplate>
            <FooterTemplate>
               <%-- <sync:CheckBoxListDropDown 
                    ID="drpTiposFuncionarioFaixasRentabilidadeLiberacao" runat="server" DataSourceID="odsTiposFuncionario"
                    EnableViewState="false"
                    DataTextField="Name" DataValueField="Id">
                </sync:CheckBoxListDropDown>--%>
            </FooterTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Funcionários" ItemStyle-HorizontalAlign="Right">
            <EditItemTemplate>
                <sync:CheckBoxListDropDown 
                    ID="drpFuncionarios" runat="server" DataSourceID="odsFunc"
                    DataTextField="Nome" DataValueField="IdFunc"
                    SelectedValues='<%# ((Glass.Rentabilidade.Negocios.Entidades.FaixaRentabilidadeLiberacao)Page.GetDataItem()).FuncionariosFaixa.Select(f => f.IdFunc).ToArray() %>'>
                </sync:CheckBoxListDropDown>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="lblFuncionarios" runat="server" Text='<%# string.Join(",", ((Glass.Rentabilidade.Negocios.Entidades.FaixaRentabilidadeLiberacao)Page.GetDataItem()).Funcionarios.Select(f => f.Nome)) %>'></asp:Label>
            </ItemTemplate>
            <FooterTemplate>
                <%--<sync:CheckBoxListDropDown 
                    ID="drpFuncionariosFaixasRentabilidadeLiberacao" runat="server" DataSourceID="odsFunc"
                    EnableViewState="false"
                    DataTextField="Nome" DataValueField="IdFunc">
                </sync:CheckBoxListDropDown>--%>
            </FooterTemplate>
        </asp:TemplateField>
                           
        <asp:TemplateField>
            <ItemTemplate>
                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="FaixaRentabilidadeLiberacao" IdRegistro='<%# (int)Eval("IdFaixaRentabilidadeLiberacao") %>' />
            </ItemTemplate>
            <FooterTemplate>
                        <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">
                        <img border="0" src="../Images/insert.gif" alt="Inserir" /></asp:LinkButton>
                    </td> 
                </tr>
            </FooterTemplate>
            <FooterStyle HorizontalAlign="Center" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:HiddenField runat="server" ID="hdfIdLojaFaixaRentabilidadeLiberacao" />

<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFunc" runat="server" 
    SelectMethod="GetVendedoresComissao" EnableViewState="false"
    TypeName="Glass.Data.DAL.FuncionarioDAO">
    <SelectParameters>
        <asp:Parameter DefaultValue="false" Name="incluirInstaladores" Type="Boolean" />
    </SelectParameters>
</colo:VirtualObjectDataSource>
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTiposFuncionario" runat="server" 
    SelectMethod="ObtemTiposFuncionario" EnableViewState="false"
    TypeName="Glass.Global.Negocios.IFuncionarioFluxo">
</colo:VirtualObjectDataSource>
 <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFaixaRentabilidadeLiberacao" runat="server"
    DataObjectTypeName="Glass.Rentabilidade.Negocios.Entidades.FaixaRentabilidadeLiberacao"
    DeleteMethod="ApagarFaixaRentabilidadeLiberacao" EnablePaging="True"
    DeleteStrategy="GetAndDelete"
    SelectMethod="ObterFaixasRentabilidadeLiberacao"
    SelectByKeysMethod="ObterFaixaRentabilidadeLiberacao"
    TypeName="Glass.Rentabilidade.Negocios.IRentabilidadeFluxo"
    UpdateMethod="SalvarFaixaRentabilidadeLiberacao"
    UpdateStrategy="GetAndUpdate">
    <SelectParameters>
        <asp:ControlParameter ControlID="hdfIdLojaFaixaRentabilidadeLiberacao" Name="idLoja" />
    </SelectParameters>
</colo:VirtualObjectDataSource>