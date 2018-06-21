<%@ Control Language="C#" AutoEventWireup="true" 
    CodeBehind="ctrlConfigFaixasRentabilidadeComissao.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlConfigFaixasRentabilidadeComissao"%>
<%@ Register Src="~/Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:GridView ID="grdFaixasRentabilidadeComissao" runat="server" SkinID="gridViewEditable"
    DataSourceID="odsFaixaRentabilidadeComissao" DataKeyNames="IdFaixaRentabilidadeComissao"
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
            </ItemTemplate>
            <EditItemTemplate>
                <asp:LinkButton ID="lnkAtualizar" runat="server" CommandName="Update" ToolTip="Atualizar">
                    <img border="0" src="../Images/ok.gif" alt="Atualizar" />
                </asp:LinkButton>
                <asp:LinkButton ID="lnkCancelar" runat="server" CommandName="Cancel" ToolTip="Cancelar">
                    <img border="0" src="../Images/ExcluirGrid.gif" alt="Cancelar" />
                </asp:LinkButton>
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

        <asp:TemplateField HeaderText="Percentual Comissão (%)" ItemStyle-HorizontalAlign="Right">
            <EditItemTemplate>
                <asp:TextBox ID="txtPercentualComissao" runat="server" Text='<%# Bind("PercentualComissao") %>'></asp:TextBox>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="lblPercentualComissao" runat="server" Text='<%# Eval("PercentualComissao") %>'></asp:Label>
            </ItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txtPercentualComissao" runat="server" MaxLength="150" Width="150px"></asp:TextBox>
            </FooterTemplate>
        </asp:TemplateField>
                           
        <asp:TemplateField>
            <ItemTemplate>
                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="FaixaRentabilidadeComissao" IdRegistro='<%# (int)Eval("IdFaixaRentabilidadeComissao") %>' />
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
<asp:HiddenField runat="server" ID="hdfIdFuncFaixaRentabilidadeComissao" />
<asp:HiddenField runat="server" ID="hdfIdLojaFaixaRentabilidadeComissao" />

 <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFaixaRentabilidadeComissao" runat="server"
    DataObjectTypeName="Glass.Rentabilidade.Negocios.Entidades.FaixaRentabilidadeComissao"
    DeleteMethod="ApagarFaixaRentabilidadeComissao" EnablePaging="True"
    DeleteStrategy="GetAndDelete"
    SelectMethod="ObterFaixasRentabilidadeComissao"
    SelectByKeysMethod="ObterFaixaRentabilidadeComissao"
    TypeName="Glass.Rentabilidade.Negocios.IRentabilidadeFluxo"
    UpdateMethod="SalvarFaixaRentabilidadeComissao"
    UpdateStrategy="GetAndUpdate">
    <SelectParameters>
        <asp:ControlParameter ControlID="hdfIdLojaFaixaRentabilidadeComissao" Name="idLoja" />
        <asp:ControlParameter ControlID="hdfIdFuncFaixaRentabilidadeComissao" Name="idFunc" DefaultValue="" />
    </SelectParameters>
</colo:VirtualObjectDataSource>