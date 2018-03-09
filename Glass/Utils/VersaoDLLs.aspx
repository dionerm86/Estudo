<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VersaoDLLs.aspx.cs" Inherits="Glass.UI.Web.Utils.VersaoDLLs"
    Title="Versões DLLs" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdDLL" runat="server" AutoGenerateColumns="False"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit"   >
                    <Columns>                        
                        <asp:BoundField DataField="Nome" HeaderText="DLL" />    
                        <asp:BoundField DataField="Versao" HeaderText="Versão" /> 
                    </Columns>                   
                </asp:GridView>
                <br />                
            </td>
        </tr>
    </table>
</asp:Content>
