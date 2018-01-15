<%@ Page Title="Planejamento de Produção - Setores" Language="C#" AutoEventWireup="true" CodeBehind="PainelPlanejamentoSetores.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.Producao.PainelPlanejamentoSetores" MasterPageFile="~/PainelGraficos.master" ValidateRequest="false" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

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
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblData" runat="server" ForeColor="#0066FF" Text="Período"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlData" runat="server" ReadOnly="ReadOnly" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imbData" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" ToolTip="Pesquisar" />
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