<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlSelGrupoSubgrupoProd.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlSelGrupoSubgrupoProd" %>

Grupo:
<asp:DropDownList ID="drpGrupoProd" runat="server" AppendDataBoundItems="True" 
    DataSourceID="odsGrupoProd" DataTextField="Descricao" DataValueField="IdGrupoProd">
    <asp:ListItem></asp:ListItem>
</asp:DropDownList>
&nbsp;
Subgrupo:
<select ID="drpSubgrupoProd" name="drpSubgrupoProd" runat="server">
</select>
<asp:HiddenField ID="hdfSubgrupoProd" runat="server" />
<colo:VirtualObjectDataSource ID="odsGrupoProd" runat="server" Culture="pt-BR"
    SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.GrupoProdDAO">
    <SelectParameters>
        <asp:Parameter DefaultValue="false" Name="incluirTodos" Type="Boolean" />
    </SelectParameters>
</colo:VirtualObjectDataSource>
<script type="text/javascript">
    var subgrupo = document.getElementById("<%= hdfSubgrupoProd.ClientID %>").value;
    document.getElementById("<%= drpGrupoProd.ClientID %>").onchange();
    
    document.getElementById("<%= drpSubgrupoProd.ClientID %>").value = subgrupo;
    document.getElementById("<%= drpSubgrupoProd.ClientID %>").onchange();
</script>