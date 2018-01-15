<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlTipoPerda.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlTipoPerda" %>

<table class="pos" cellpadding="0" cellspacing="0" style="display: inline-table">
    <tr>
        <td nowrap="nowrap">
            <asp:DropDownList ID="drpTipoPerda" runat="server" DataSourceID="odsTipoPerda" 
                DataTextField="Descricao" DataValueField="IdTipoPerda">
                <asp:ListItem></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvTipoPerda" runat="server" ErrorMessage="*" 
                ControlToValidate="drpTipoPerda" Display="Dynamic"></asp:RequiredFieldValidator>
        </td>
        <td nowrap="nowrap">
            &nbsp;
            Subtipo:
            <select id="<%= this.ClientID %>_drpSubtipoPerda"
                onchange="document.getElementById('<%= hdfIdSubtipoPerda.ClientID %>').value = this.value">
            </select>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfIdSubtipoPerda" runat="server" />
<asp:HiddenField ID="hdfIdSetor" runat="server" />
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoPerda" runat="server" 
    SelectMethod="GetOrderedList" TypeName="Glass.Data.DAL.TipoPerdaDAO">
    <SelectParameters>
        <asp:ControlParameter ControlID="hdfIdSetor" Name="idSetor" PropertyName="Value"
            Type="Int32" />
    </SelectParameters>
</colo:VirtualObjectDataSource>
<script type="text/javascript">
    getSubtipos("<%= this.ClientID %>", document.getElementById("<%= drpTipoPerda.ClientID %>").value);
    <%= Init() %>
</script>