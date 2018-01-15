<%@ Page Title="Dados do Carregamento" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadDadosCarregamento.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadDadosCarregamento" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function finalizaCarregamento() {

            var veiculo = FindControl("drpVeiculo", "select").value;
            var idMotorista = FindControl("drpMotorista", "select").value;
            var dtPrevSaida = FindControl("txtDtPrevSaida_txtData", "input").value 
                + " " + FindControl("txtDtPrevSaida_txtHora", "input").value;

            var idsOCs = GetQueryString("idsOCs");
            var idLoja = GetQueryString("idLoja");

            var valido = CadDadosCarregamento.ValidaCarregamentoAcimaCapacidadeVeiculo(veiculo, idsOCs);

            if (valido.error != null) {
                alert(valido.error.description);
                return false;
            }

            valido = valido.value.split(";");

            if (valido[0] == "Erro") {
                alert(valido[1]);
                return false;
            }

            if (valido[0] == "Excedeu") {
                if(!confirm(valido[1]))
                    return false;
            }

            var enviarEmail = false;
            
             
            if(<%= EnviarEmailAoFinalizar().ToString().ToLower() %>)
                enviarEmail = confirm("Enviar e-mail para os clientes?");

            var retorno = CadDadosCarregamento.FinalizaCarregamento(veiculo, idMotorista, dtPrevSaida, idLoja, idsOCs, enviarEmail);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return false;
            }

            window.opener.redirectUrl("../Listas/lstCarregamentos.aspx");
            alert("Carregamento " + retorno.value + " gerado com sucesso!");
            window.close();
        }
    
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td class="dtvHeader">
                            Veículo
                        </td>
                        <td class="dtvAlternatingRow">
                            <asp:DropDownList ID="drpVeiculo" runat="server" DataSourceID="odsVeiculo" DataValueField="Placa"
                                DataTextField="DescricaoCompleta">
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVeiculo" runat="server" SelectMethod="GetOrdered"
                                TypeName="Glass.Data.DAL.VeiculoDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="dtvHeader">
                            Motorista
                        </td>
                        <td class="dtvAlternatingRow">
                            <asp:DropDownList ID="drpMotorista" runat="server" DataSourceID="odsMotorista" DataValueField="IdFunc"
                                DataTextField="Nome">
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMotorista" runat="server" SelectMethod="GetMotoristas"
                                TypeName="Glass.Data.DAL.FuncionarioDAO">
                                <SelectParameters>
                                    <asp:Parameter Name="nome" Type="String" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="dtvHeader">
                            Data prevista de saída
                        </td>
                        <td class="dtvAlternatingRow">
                            <uc1:ctrlData ID="txtDtPrevSaida" runat="server" ReadOnly="ReadWrite" ExibirHoras="true" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnFinalizar" runat="server" Text="Finalizar Carregamento" OnClientClick="finalizaCarregamento();return false;" />
            </td>
        </tr>
    </table>
</asp:Content>
