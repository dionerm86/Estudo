<%@ Page Title="Lançar Débito de Funcionário" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadLancarDebitoFunc.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadLancarDebitoFunc" %>

<%@ Register src="../Controls/ctrlFormaPagto.ascx" tagname="ctrlFormaPagto" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript">
        function confirmar()
        {
            if (!validate())
                return false;

            if (FindControl("drpFunc", "select").value == "")
            {
                alert("Selecione o funcionário.");
                return false;
            }

            if (FindControl("drpPlanoConta", "select").value == "")
            {
                alert("Selecione o plano de contas.");
                return false;
            }

            if (FindControl("txtValor", "input").value == "")
            {
                alert("Digite o valor.");
                return false;
            }
            
            if (!confirm("Lançar débito?"))
                return false;
            
            return true;
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            Funcionário
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpFunc" runat="server" AppendDataBoundItems="True" 
                                DataSourceID="odsFunc" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            Plano de conta
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpPlanoConta" runat="server" 
                                DataSourceID="odsPlanoConta" DataTextField="DescrPlanoGrupo" 
                                DataValueField="IdConta" AppendDataBoundItems="True">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            Valor (R$)
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtValor" runat="server" Width="100px"
                                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            Observações
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtObs" runat="server" Columns="75" Rows="4" 
                                TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:Button ID="btnConfirmar" runat="server" Text="Debitar" 
                    OnClientClick="return confirmar()" onclick="btnConfirmar_Click" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFunc" runat="server" 
                    SelectMethod="GetOrdered" 
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoConta" runat="server" 
                    SelectMethod="GetPlanoContas" TypeName="Glass.Data.DAL.PlanoContasDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="2" Name="tipo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

