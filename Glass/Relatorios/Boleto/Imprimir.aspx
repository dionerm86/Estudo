<%@ Page Title="Imprimir Boleto" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true"
    CodeBehind="Imprimir.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Boleto.Imprimir" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Menu" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Pagina" runat="Server">

    <script type="text/javascript">
        function gerarBoleto() {
            var contaBanco = FindControl("drpContaBanco", "select").value;
            var carteira = FindControl("drpCarteira", "select").value;
            var especieDoc = FindControl("drpEspecieDocumento", "select").value;
            var codigoLiberacao = '<%= Request["codigoLiberacao"] %>'

            var instrucoes = FindControl("txtInstrucoes", "textarea").value.replace(/\n/g, ";");

            var codigoNotaFiscal = '<%= Request["codigoNotaFiscal"] %>';
            var codigoContaReceber = '<%= Request["codigoContaReceber"] %>';
            var codigoCte = '<%= Request["codigoCte"] %>';

            var base = '<%= ResolveUrl("~/") %>';
            window.opener.openWindow(600, 800, base + '../../Handlers/Boleto.ashx?codigoNotaFiscal=' + codigoNotaFiscal + '&codigoCte=' + codigoCte +
                '&codigoContaReceber=' + codigoContaReceber + '&codigoLiberacao=' + codigoLiberacao + '&codigoContaBanco=' + contaBanco +
                '&carteira=' + carteira + '&especieDocumento=' + especieDoc + '&instrucoes=' + instrucoes);

            if (window.opener.controlePopup.length == 0)
                closeWindow();
        }
    </script>

    <asp:HiddenField runat="server" ID="hdfTipoArquivo" Value="1" />

    <table width="100%">
        <tr>
            <td colspan="2" style="color: blue; text-align: center; font-size: medium; padding-bottom: 8px">
                <%= BoletoJaImpresso() %>
            </td>
        </tr>
        <tr>
            <td align="right" style="padding-right: 4px">
                <asp:Label ID="Label1" runat="server" Text="Conta bancária" ForeColor="#0066FF"></asp:Label>
            </td>
            <td align="left">
                <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                    DataTextField="Name" DataValueField="Id" AutoPostBack="True" OnDataBound="drpContaBanco_DataBound"
                    OnSelectedIndexChanged="drpContaBanco_SelectedIndexChanged" AppendDataBoundItems="true">
                </asp:DropDownList>
                <asp:HiddenField ID="hdfBanco" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="right" style="padding-right: 4px">
                <asp:Label ID="Label2" runat="server" Text="Carteira" ForeColor="#0066FF"></asp:Label>
            </td>
            <td align="left">
                <asp:DropDownList ID="drpCarteira" runat="server" DataSourceID="odsCarteira" DataTextField="Value"
                    DataValueField="Key">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="right" nowrap="nowrap" style="padding-right: 4px">
                <asp:Label ID="Label3" runat="server" Text="Espécie Documento" ForeColor="#0066FF"></asp:Label>
            </td>
            <td align="left">
                <asp:DropDownList ID="drpEspecieDocumento" runat="server" DataSourceID="odsEspecieDocumento"
                    DataTextField="Value" DataValueField="Key">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="right" style="vertical-align: top; padding-top: 3px; padding-right: 4px">
                <asp:Label ID="lblInstrucoes" runat="server" Text="Instruções" ForeColor="#0066FF"></asp:Label>
            </td>
            <td align="left" width="100%">
                <asp:TextBox ID="txtInstrucoes" runat="server" TextMode="MultiLine" Rows="2" Columns="20" Width="480px" Height="58px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <br />
                <asp:Button ID="btnGerar" runat="server" Text="Gerar boleto" OnClientClick="gerarBoleto(); return false;" />
            </td>
        </tr>
    </table>
    <asp:Label ID="lblSemConta" runat="server" ForeColor="Red" Text="<br />Não há contas bancárias configuradas para geração de boletos.<br />Indique no cadastro de contas bancárias o número do banco e o código do convênio."></asp:Label>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContaBanco" runat="server"
        SelectMethod="ObtemContasBanco" TypeName="Glass.Financeiro.Negocios.IContaBancariaFluxo">
        <SelectParameters>
            <asp:QueryStringParameter QueryStringField="codigoContaReceber" Name="idContaR" Type="Int32" />
            <asp:QueryStringParameter Name="idNf" QueryStringField="codigoNotaFiscal" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>


    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsEspecieDocumento" runat="server" SelectMethod="ObterEspecieDocumento"
        TypeName="Sync.Utils.Boleto.DataSourceHelper">
        <SelectParameters>
            <asp:ControlParameter ControlID="hdfBanco" Name="codBanco" PropertyName="Value" Type="Int32" />
            <asp:ControlParameter ControlID="hdfTipoArquivo" Name="tipoArquivo" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCarteira" runat="server" SelectMethod="ObterCarteiras"
        TypeName="Sync.Utils.Boleto.DataSourceHelper">
        <SelectParameters>
            <asp:ControlParameter ControlID="hdfBanco" Name="codBanco" PropertyName="Value" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
