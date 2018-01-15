<%@ Page Title="Quitar Débito de Funcionário " Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadQuitarDebitoFunc.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadQuitarDebitoFunc" %>

<%@ Register src="../Controls/ctrlFormaPagto.ascx" tagname="ctrlFormaPagto" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
<script type="text/javascript">
        function atualizarDadosFunc(idFunc)
        {
            if (idFunc == "")
            {
                limpar();
                return;
            }
            
            var resposta = CadQuitarDebitoFunc.GetDadosFuncionario(idFunc).value.split(';');
            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                limpar();
                return;
            }
            
            FindControl("hdfValorDebito", "input").value = resposta[1];
            usarCredito("<%= ctrlFormaPagto1.ClientID %>", "");
        }
        
        function limpar()
        {
            var controle = <%= ctrlFormaPagto1.ClientID %>;
            FindControl("hdfValorDebito", "input").value = "0";
            FindControl("txtObs", "input").value = "";
            controle.Limpar();
        }
        
        function confirmar(botao)
        {
            if (!validate())
                return;
            
            if (!confirm("Quitar débito?"))
                return;
            
            var idFunc = FindControl("drpFunc", "select").value;
            
            var controle = <%= ctrlFormaPagto1.ClientID %>;
            var valores = controle.Valores();
            var formasPagto = controle.FormasPagamento();
            var tiposCartao = controle.TiposCartao();
            var parcelasCartao = controle.ParcelasCartao();
            var contasBanco = controle.ContasBanco();
            var tiposBoleto = controle.TiposBoleto();
            var taxaAntecip = controle.TaxasAntecipacao();
            var numAutConstrucard = controle.NumeroConstrucard();
            var recebimentoParcial = controle.RecebimentoParcial();
            var cheques = controle.Cheques();
            var gerarCredito = controle.GerarCredito();
            var obs = FindControl("txtObs", "input").value;
            var depositoNaoIdentificado = controle.DepositosNaoIdentificados();
            
            var resposta = CadQuitarDebitoFunc.Confirmar(idFunc, valores, formasPagto, tiposCartao,
                parcelasCartao, contasBanco, depositoNaoIdentificado, tiposBoleto, taxaAntecip, numAutConstrucard, 
                recebimentoParcial, gerarCredito, cheques, obs).value.split(';');
            
            alert(resposta[1]);
            
            if (resposta[0] != "Erro")
            {
                limpar();
                atualizarDadosFunc(idFunc);
            }
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFunc" runat="server" AppendDataBoundItems="True" 
                                DataSourceID="odsFunc" DataTextField="Nome" DataValueField="IdFunc"
                                onchange="atualizarDadosFunc(this.value)">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <uc1:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server" 
                    OnLoad="ctrlFormaPagto1_Load" ExibirGerarCredito="False" ExibirCredito="False"
                    ExibirJuros="False" ExibirDataRecebimento="False" 
                    CobrarJurosCartaoClientes="False" />
                <br />
                <table>
                    <tr>
                        <td>
                            Observações
                        </td>
                        <td>
                            <asp:TextBox ID="txtObs" runat="server" Columns="75" Rows="4" 
                                TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnConfirmar" runat="server" Text="Quitar" OnClientClick="confirmar(this); return false" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFunc" runat="server" 
                    SelectMethod="GetFuncionariosDebito" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfValorDebito" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>

