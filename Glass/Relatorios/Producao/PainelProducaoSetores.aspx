<%@ Page Title="Monitoramento de Produção por Setor" Language="C#" AutoEventWireup="true" CodeBehind="PainelProducaoSetores.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.Producao.PainelProducaoSetores" MasterPageFile="~/PainelGraficos.master" ValidateRequest="false" %>

<asp:Content runat="server" ID="conteudo" ContentPlaceHolderID="Conteudo">
    <table cellpadding="0" cellspacing="0">
        <asp:Button ID="btnVoltarTelaMarcadorProducao" runat="server" Text="Clique aqui para voltar à tela de leitura da produção."
            OnClientClick="voltarTelaProducao(); return false;"></asp:Button>
        <tr>
            <td rowspan="2">
                <asp:Chart ID="chtMeta" runat="server" Height="670px" Width="150px" />
                <asp:Chart ID="chtProducaoDiaSetores" runat="server" Height="670px" Width="1200px" />
            </td>
            <td align="right">
                <asp:Chart ID="chtPerdaMensal" runat="server" Height="300px" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Chart ID="chtProducaoPendente" runat="server" Width="600px" />
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
        function voltarTelaProducao () {
            redirectUrl('<%=ResolveUrl("~/Cadastros/Producao/CadProducao.aspx")%>')
        }
        
    </script>

</asp:Content>