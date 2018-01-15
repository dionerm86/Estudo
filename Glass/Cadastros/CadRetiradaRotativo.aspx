<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadRetiradaRotativo.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadRetiradaRotativo" Title="Retirada de Caixa Diário" %>

<%@ Register src="../Controls/ctrlTextBoxFloat.ascx" tagname="ctrlTextBoxFloat" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

<script type="text/javascript">
    var clicked = false;

    function onSave(btn) {
        if (clicked)
            return false;

        btn.disabled = true;

        clicked = true;
    
        var idConta = FindControl("drpPlanoConta", "select").value;
        var valor = FindControl("txtValor", "input").value;
        var formaSaida = FindControl("drpFormaSaida", "select").value;
        var idCheque = FindControl("hdfIdCheque", "input").value;
        var obs = FindControl("txtObs", "textarea").value;

        if (idConta == "" || idConta == "0") {
            alert("Informe o plano de conta.");
            btn.disabled = false;
            clicked = false;
            return false;
        }

        if (formaSaida == "" || formaSaida == "0") {
            alert("Informe a forma de retirada.");
            btn.disabled = false;
            clicked = false;
            return false;
        }

        if (valor == "" || parseFloat(valor.replace(",", ".")) == 0) {
            alert("Informe o valor do crédito");
            btn.disabled = false;
            clicked = false;
            return false;
        }

        var response = CadRetiradaRotativo.Retirar(idConta, valor, formaSaida, idCheque, obs).value;

        btn.disabled = false;

        if (response == null) {
            alert("Falha na requisição, recarregue a página e tente novamente.");
            clicked = false;
            return false;
        }

        response = response.split('\t');

        if (response[0] == "Erro") {
            alert(response[1]);
            clicked = false;
            return false;
        }

        alert(response[1]);

        FindControl("drpPlanoConta", "select").selectedIndex = 0;
        FindControl("drpFormaSaida", "select").selectedIndex = 0;
        FindControl("txtValor", "input").value = "";
        FindControl("txtObs", "textarea").value = "";

        clicked = false;

        return false;
    }

    function alteraFormaSaida(forma)
    {
        var valor = FindControl("txtValor", "input");
        var selCheque = FindControl("imbSelCheque", "input");
        var dadosCheque = FindControl("lblDadosCheque", "span");
        var idCheque = FindControl("hdfIdCheque", "input");
        
        valor.readOnly = forma == 2;
        valor.value = forma == 2 ? "" : valor.value;
        selCheque.style.display = forma == 2 ? "" : "none";
        dadosCheque.style.display = forma == 2 ? "" : "none";
        dadosCheque.innerHTML = "";
        idCheque.value = "";
    }

    function setCheque(idCheque, num, titular, banco, agencia, conta, valor, dataVenc, janela)
    {
        var campoIdCheque = FindControl("hdfIdCheque", "input");
        var dadosCheque = FindControl("lblDadosCheque", "span");
        var campoValor = FindControl("txtValor", "input");
        
        campoIdCheque.value = idCheque;
        dadosCheque.innerHTML = "Cheque núm. " + num + " / Banco " + banco + " / Agência " + agencia + " / Conta " +  conta;
        campoValor.value = valor.replace("R$", "").replace(" ", "").replace(/\./g, "");
    }
</script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left" nowrap="nowrap" width="50">
                            <asp:Label ID="Label4" runat="server" Text="Plano de Conta:"></asp:Label>
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
                            <asp:Label ID="Label5" runat="server" Text="Valor:"></asp:Label>
                        </td>
                        <td align="left">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtValor" runat="server" Width="70px"
                                            onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                    </td>
                                    <td nowrap="nowrap">
                                        &nbsp;
                                        <asp:Label ID="Label6" runat="server" Text="Forma saída:"></asp:Label>
                                        &nbsp;
                                    </td>
                                    <td align="left" width="100%">
                                        <asp:DropDownList ID="drpFormaSaida" runat="server" onchange="alteraFormaSaida(this.value)">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Value="1">Dinheiro</asp:ListItem>
                                            <asp:ListItem Value="2">Cheque</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:ImageButton ID="imbSelCheque" runat="server" 
                                            ImageUrl="~/Images/Insert.gif" Style="display: none"
                                            onclientclick="openWindow(600, 850, &quot;../Utils/SelCheque.aspx?tipo=2&situacao=1&unico=1&quot;); return false;" 
                                            ToolTip="Selecionar cheque" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <div style="padding-top: 4px">
                                            <asp:Label ID="lblDadosCheque" runat="server"></asp:Label>
                                            <asp:HiddenField ID="hdfIdCheque" runat="server" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="Label3" runat="server" Text="Observação"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Rows="3" 
                                TextMode="MultiLine" Width="100%"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnRetirar" runat="server" 
                    Text="Retirar" OnClientClick="return onSave(this);" />
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoConta" runat="server" 
                    SelectMethod="GetPlanoContas" TypeName="Glass.Data.DAL.PlanoContasDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="2" Name="tipo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        alteraFormaSaida(FindControl("drpFormaSaida", "select").value);
    </script>
</asp:Content>

