<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlTipoFuncionario.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlTipoFuncionario" %>

<asp:DropDownList ID="drpTipoFuncionario" runat="server" 
    DataSourceID="odsTipoFuncionario" DataTextField="Descricao" 
    DataValueField="TipoFuncComSetor" AppendDataBoundItems="True" 
    ondatabound="drpTipoFuncionario_DataBound">
    <asp:ListItem Value="0">Todos</asp:ListItem>
</asp:DropDownList>
<asp:HiddenField ID="hdfTipoFuncionario" runat="server" Value="0" />
<asp:HiddenField ID="hdfIdSetorFuncionario" runat="server" />
<colo:VirtualObjectDataSource culture="pt-BR" 
    ID="odsTipoFuncionario" runat="server" 
    SelectMethod="ObtemTiposFuncionarioSetor" 
    TypeName="Glass.Global.Negocios.IFuncionarioFluxo">
    <SelectParameters>
        <asp:ControlParameter ControlID="hdfIncluirSetor" DefaultValue="" 
            Name="incluirSetor" PropertyName="Value" Type="Boolean" />
        <asp:ControlParameter ControlID="hdfRemoverMarcadorProducaoSemSetor" 
            Name="removerMarcadorProducaoSemSetor" PropertyName="Value" Type="Boolean" />
    </SelectParameters>
</colo:VirtualObjectDataSource>
<asp:HiddenField ID="hdfIncluirSetor" runat="server" Value="True" />
<asp:HiddenField ID="hdfRemoverMarcadorProducaoSemSetor" runat="server" Value="False" />
