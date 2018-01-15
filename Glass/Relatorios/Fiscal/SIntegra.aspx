<%@ Page Title="Arquivo do Sintegra" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="SIntegra.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Fiscal.SIntegra" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        // -----------------------------------------------------------
        // Função que disponibiliza o download do arquivo do SIntegra.
        // -----------------------------------------------------------
        function baixarArquivo()
        {
            // recupera os valores selecionados nos campos
            var loja = document.getElementById("<%= ddlLoja.ClientID %>").value;
            var inicio = FindControl('ctrlDataIni_txtData', 'input').value;
            var fim = FindControl('ctrlDataFim_txtData', 'input').value;
            
            // garante que os campos de data não estejam vazios
            if (inicio == "" || fim == "") 
            {
                alert("Preencha o intervalo de data antes de continuar.");
                return false;
            }
            
            // converte os objetos de data
            var dataInicio = new Date(inicio.substr(6, 4), parseFloat(inicio.substr(3, 2)) - 1, inicio.substr(0, 2));
            var dataFim = new Date(fim.substr(6, 4), parseFloat(fim.substr(3, 2)) - 1, fim.substr(0, 2), 23, 59, 59, 999);
                        
            if (dataInicio.getDate() != 1)
            {
                if (!confirm("Tem certeza que deseja gerar o arquivo mesmo que a data não seja a partir do primeiro dia do mês?"))
                    return false;
            }
            
            if (!isUltimoDiaMes(dataFim.getDate(), dataFim.getMonth(), dataFim.getFullYear()))
            {
                if (!confirm("Tem certeza que deseja gerar o arquivo mesmo que a data não seja o último dia do mês?"))
                    return false;
            }
        }
        
        // -------------------------------------------------------------
        // Função que verifica se o dia informado é o último dia do mês.
        // -------------------------------------------------------------
        function isUltimoDiaMes(dia, mes, ano)
        {
            var diasMeses = new Array(31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31);
            if ((ano % 4 == 0) || (ano % 100 == 0) || (ano % 400 == 0))
                diasMeses[1] = 29;
            
            return dia == diasMeses[mes];
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left" colspan="2">
                            <asp:DropDownList ID="ddlLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" OnDataBound="ddlLoja_DataBound">
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                            </colo:VirtualObjectDataSource>
                            <asp:HiddenField ID="hdfIdLoja" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Label ID="lblPeriodo" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right" colspan="3">
                            &nbsp;
                        </td>
                    </tr>
                </table>
                <br />
                <table>
                    <tr>
                        <td class="subtitle1">
                            <asp:Label ID="lblRegistros" runat="server" Text="Registros"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBoxList ID="cblRegistros" runat="server">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <asp:Button ID="btnBaixar" runat="server" Text="Gerar arquivo" OnClientClick="return baixarArquivo();"
                    OnClick="btnBaixar_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
