<%@ Page Title="Monitoramento de Produção" Language="C#" AutoEventWireup="true" CodeBehind="PainelProducao.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.Producao.PainelProducao" MasterPageFile="~/PainelGraficos.master" ValidateRequest="false" %>

<asp:Content runat="server" ID="conteudo" ContentPlaceHolderID="Conteudo">
    <asp:Button ID="btnVoltarTelaMarcadorProducao" runat="server" Text="Clique aqui para voltar à tela de leitura da produção."
        OnClientClick="voltarTelaProducao(); return false;"></asp:Button>

    <table cellpadding="0" cellspacing="0">
        <tr>
            <td rowspan="2">
                <asp:Chart ID="chtProducaoDia" runat="server" Height="630px" />
            </td>
            <td align="right">
                <table runat="server" style="padding-left: 35px; padding-top: 55px; padding-right: 35px;" id="tbPercentualPerda" clientidmode="Static" visible="false">
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Produzido: " Font-Bold="True" Font-Size="18pt"></asp:Label>
                            <asp:Label ID="lblProduzido" runat="server" Text="" Font-Bold="True" Font-Size="22pt"></asp:Label>
                            <asp:Label ID="Label3" runat="server" Text="M²" Font-Bold="True" Font-Size="18pt"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div style="background-color: red; text-align: center;">
                                <asp:Label ID="lblPorcentagemPerda" runat="server" Text="" Font-Bold="True" ForeColor="White" Font-Size="60pt"></asp:Label>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Retrabalho: " Font-Bold="True" Font-Size="18pt"></asp:Label>
                            <asp:Label ID="lblRetrabalho" runat="server" Text="" Font-Bold="True" Font-Size="22pt"></asp:Label>
                            <asp:Label ID="Label6" runat="server" Text="M²" Font-Bold="True" Font-Size="18pt"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
            <td align="right" style="padding-right: 35px;">
                <asp:Chart ID="chtPerdaMensal" runat="server" Height="250px" Visible="false" />                
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Chart ID="chtProducaoPendente" runat="server" Width="900px" />
            </td>
        </tr>
    </table>

    <script type="text/javascript">

        // Seta nesta variável se quem abriu a tela foi a função de redirecionamento da tela de leitura de produção.
        var redirecionamentoProducao = '<%= Request["redirecionamentoProducao"] %>';
        var btnVoltarTela = FindControl("btnVoltarTelaMarcadorProducao", "input");

        // Caso a tela não tenha sido aberta pela função de redirecionamento da tela de leitura de produção, então
        // o botão de volta para a tela de marcador de produção não deve ser exibido.
        if (redirecionamentoProducao == null || redirecionamentoProducao == "" || redirecionamentoProducao == "false" || btnVoltarTela == null)
            FindControl("btnVoltarTelaMarcadorProducao", "input").style.display = "none";

        // Método que redireciona o usuário para a tela de leitura de peça.
        function voltarTelaProducao() {
            redirectUrl('<%=ResolveUrl("~/Cadastros/Producao/CadProducao.aspx")%>')
        }

    </script>

</asp:Content>
