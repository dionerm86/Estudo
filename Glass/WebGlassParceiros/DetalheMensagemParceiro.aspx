<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DetalheMensagemParceiro.aspx.cs"
    Inherits="Glass.UI.Web.WebGlassParceiros.DetalheMensagemParceiro" Title="Mensagem" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvMensagem" runat="server" AutoGenerateRows="False" CellPadding="4"
                    DataSourceID="odsMensagemParceiro" ForeColor="#333333" GridLines="None" Height="50px"
                    Width="350px">
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Left" />
                    <FieldHeaderStyle BackColor="#E9ECF1" Font-Bold="True" HorizontalAlign="Left" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <Fields>
                        <asp:BoundField DataField="Remetente" HeaderText="Remetente" SortExpression="Remetente" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="Assunto" HeaderText="Assunto" SortExpression="Assunto" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descricao" SortExpression="Descricao" />
                    </Fields>
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#999999" HorizontalAlign="Left" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMensagemParceiro" runat="server" 
                    SelectMethod="ObtemDetalhesMensagemParceiro"
                    TypeName="Glass.Global.Negocios.IMensagemFluxo">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idMensagem" QueryStringField="idMsg" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnResponder" runat="server" Text="Responder" OnClick="btnResponder_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="Button1" runat="server" OnClientClick="closeWindow(); return false;"
                    Text="Fechar" />
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        window.opener.location.reload();
    </script>

</asp:Content>
