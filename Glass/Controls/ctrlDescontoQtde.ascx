<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlDescontoQtde.ascx.cs"
    Inherits="Glass.UI.Web.Controls.ctrlDescontoQtde" %>
<div id="divDescontoQtde" style="display: none; position: absolute; border: 1px solid;
    background-color: white" runat="server">
    <table cellpadding="3" style="margin-top: 3px">
        <tr>
            <td align="left">
                Desconto máx.:
            </td>
            <td align="left">
                <asp:Label ID="lblPercDescQtde" runat="server"></asp:Label>
                %
            </td>
        </tr>
        <tr id="trDescontoTabela">
            <td align="left">
                Desconto de tabela:
            </td>
            <td align="left">
                <asp:Label ID="lblDescTabela" runat="server"></asp:Label>
                %
            </td>
        </tr>
        <tr>
            <td align="left">
                Desconto aplic.:
            </td>
            <td align="left">
                <asp:TextBox ID="txtPercDescQtde" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                %
                <asp:CustomValidator ID="ctvPercDesconto" runat="server" 
                    ClientValidationFunction="validaPercDesconto" 
                    ControlToValidate="txtPercDescQtde" Display="None" ValidateEmptyText="True"></asp:CustomValidator>
            </td>
        </tr>
    </table>
</div>
<asp:HiddenField ID="hdfValorDescontoQtde" runat="server" />