<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlLojaNCM.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlLojaNCM" %>

<%@ Register Src="ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc1" %>

<table runat="server" id="tabela" class="pos" cellpadding="1" cellspacing="1">
    <tr>
        <td>
            <uc1:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="false" MostrarTodas="false" 
                OnChange='<%# this.ID +".ncmCallback();" %>' MostrarVazia="true"/>
        </td>
        <td nowrap="nowrap" valign="top" style="padding-top: 7px"><asp:Label ID="lblNCM" runat="server" Text="NCM: "></asp:Label></td>
        <td valign="top" style="padding: 1px">
            <asp:TextBox ID="txtNCM" runat="server" OnChange='<%# this.ID +".ncmCallback();" %>'></asp:TextBox>
        </td>
        <td valign="top" style="padding-top: 3px">
            <asp:ImageButton ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfIdLoja" runat="server" />
<asp:HiddenField ID="hdfNCM" runat="server" />