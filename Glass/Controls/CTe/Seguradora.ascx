<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Seguradora.ascx.cs" Inherits="Glass.UI.Web.Controls.CTe.Seguradora" %>
<div class="dtvRow">
    <div class="dtvHeader">
        <asp:Label ID="lblSeguradora" runat="server" Text="Nome Seguradora *" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:DropDownList ID="drpSeguradora" runat="server" DataSourceID="odsSeguradora"
            DataTextField="NomeSeguradora" DataValueField="IdSeguradora" Height="20px" Width="150px"
            AppendDataBoundItems="True" Enabled="true" Visible="true" >
            <asp:ListItem Value="selecione" Text="Selecione"></asp:ListItem>
        </asp:DropDownList>
        <asp:RequiredFieldValidator ID="rfvSeguradora" runat="server" ErrorMessage="*" ValidationGroup="c"
            ControlToValidate="drpSeguradora" Display="Dynamic"></asp:RequiredFieldValidator>
    </div>
</div>
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsSeguradora" runat="server" DataObjectTypeName="Glass.Data.Model.CTe.Seguradora"
    SelectMethod="GetList" TypeName="Glass.Data.DAL.CTe.SeguradoraDAO" EnablePaging="True"
    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SortParameterName="sortExpression"
    StartRowIndexParameterName="startRow">
</colo:VirtualObjectDataSource>
