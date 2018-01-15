<%@ Page Title="Planejamento de Produ��o - Setores" Language="C#" AutoEventWireup="true" CodeBehind="PainelPlanejamentoSetores.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.Producao.PainelPlanejamentoSetores" MasterPageFile="~/PainelGraficos.master" ValidateRequest="false" %>

<asp:Content runat="server" ID="conteudo" ContentPlaceHolderID="Conteudo">
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
                        <td>&nbsp;</td>
                        <td>
                            <div style="width: 20px; height: 12px; background-color: #01F08C;">
                            </div>
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Realizado"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr align="center">
            <td>&nbsp;
            </td>
        </tr>
        <tr align="center">
            <td>&nbsp;
            </td>
        </tr>
        <tr align="center">
            <td>
                <asp:Chart ID="chtPrevisaoProducao" runat="server" Width="1000px" />
            </td>
        </tr>
    </table>
</asp:Content>