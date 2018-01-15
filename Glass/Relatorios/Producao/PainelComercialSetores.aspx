<%@ Page Title="Painel Comercial - Setores" Language="C#" MasterPageFile="~/PainelGraficos.master"
    AutoEventWireup="true" CodeBehind="PainelComercialSetores.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.PainelComercialSetores" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr align="center">
            <td>
                <table>
                    <tr>
                        <td>
                            <div style="width: 20px; height: 12px; background-color: #F08C41;">
                            </div>
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Capacidade"></asp:Label>
                        </td>
                        <td>&nbsp;</td>
                        <td>
                            <div style="width: 20px; height: 12px; background-color: #418CF0;">
                            </div>
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Previsto"></asp:Label>
                        </td>
                    </tr>
                </table>
                <br />
            </td>
        </tr>
        <tr align="center">
            <td>
                <div runat="server" id="divGraficos">
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
