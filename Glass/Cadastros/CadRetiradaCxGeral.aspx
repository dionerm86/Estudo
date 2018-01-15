<%@ Page Title="Retirada Cx. Geral" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadRetiradaCxGeral.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadRetiradaCxGeral" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

    function onSave(btn) {
        var idFornec = FindControl("hdfFornec", "input").value;
        var idConta = FindControl("drpPlanoConta", "select").value;
        var valor = FindControl("txtValor", "input").value;
        var formaSaida = FindControl("drpFormaSaida", "select").value;
        var idCheque = FindControl("hdfIdCheque", "input").value;
        var obs = FindControl("txtObs", "textarea").value;

        if (idConta == "" || idConta == "0") {
            alert("Informe o plano de conta.");
            return false;
        }

        if (valor == "" || parseFloat(valor.replace(",", ".")) == 0) {
            alert("Informe o valor da retirada");
            return false;
        }

        if (obs == "") {
            alert("Informe o motivo da retirada.");
            return false;
        }

        btn.disabled = true;

        var response = CadRetiradaCxGeral.Retirar(idFornec, idConta, valor, formaSaida, idCheque, obs).value;

        btn.disabled = false;

        if (response == null) {
            alert("Falha na requisição, recarregue a página e tente novamente.");
            return false;
        }

        response = response.split('\t');

        if (response[0] == "Erro") {
            alert(response[1]);
            return false;
        }

        alert(response[1]);

        FindControl("drpPlanoConta", "select").selectedIndex = 0;
        FindControl("txtValor", "input").value = "";
        FindControl("txtObs", "textarea").value = "";

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

    function getFornec(idFornec) {
        var retorno = MetodosAjax.GetFornec(idFornec.value).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idFornec.value = "";
            FindControl("txtNomeFornec", "input").value = "";
            FindControl("hdfFornec", "input").value = "";
            return false;
        }

        FindControl("txtNomeFornec", "input").value = retorno[1];
        FindControl("hdfFornec", "input").value = idFornec.value;
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Plano de Conta:"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpPlanoConta" runat="server" DataSourceID="odsPlanoConta"
                                DataTextField="DescrPlanoGrupo" DataValueField="IdConta" AppendDataBoundItems="True">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Fornecedor:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" 
                                onkeypress="return soNumeros(event, true, true);" onblur="getFornec(this);" 
                                Text='<%# Eval("IdFornec") %>'></asp:TextBox>
                            <asp:TextBox ID="txtNomeFornec" runat="server" onkeypress="return false"
                                Text='<%# Eval("NomeFornecedor") %>' Width="250px"></asp:TextBox>
                            <asp:LinkButton ID="lnkSelFornec" runat="server" 
                                
                                
                                OnClientClick="openWindow(570, 760, '../Utils/SelFornec.aspx'); return false;">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                            <asp:HiddenField ID="hdfFornec" runat="server" 
                                Value='<%# Bind("IdFornec") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Valor:" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtValor" runat="server" Width="100px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                    </td>
                                    <td nowrap="nowrap">
                                        &nbsp;
                                        <asp:Label ID="Label6" runat="server" Text="Forma saída:"></asp:Label>
                                        &nbsp;
                                    </td>
                                    <td align="left" width="100%">
                                        <asp:DropDownList ID="drpFormaSaida" runat="server" onchange="alteraFormaSaida(this.value)">
                                            <asp:ListItem Value="1">Dinheiro</asp:ListItem>
                                            <asp:ListItem Value="2">Cheque</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:ImageButton ID="imbSelCheque" runat="server" 
                                            ImageUrl="~/Images/Insert.gif" Style="display: none"
                                            onclientclick="openWindow(600, 850, &quot;../Utils/SelCheque.aspx?tipo=2&situacao=1&unico=1&adicionarTodosVisible=false&quot;); return false;" 
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
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="Label3" runat="server" Text="Observação" Font-Bold="False"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Rows="3" TextMode="MultiLine"
                    Width="100%"></asp:TextBox>
            </td>
        </tr>
    </table>
    <br />
    <asp:Button ID="btnRetirar" runat="server" Text="Retirar"
        OnClientClick="return onSave(this);" />
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoConta" runat="server" SelectMethod="GetPlanoContas"
        TypeName="Glass.Data.DAL.PlanoContasDAO">
        <SelectParameters>
            <asp:Parameter DefaultValue="2" Name="tipo" Type="Int32" />
        </SelectParameters>
                </colo:VirtualObjectDataSource>
    <script type="text/javascript">
        alteraFormaSaida(FindControl("drpFormaSaida", "select").value);
    </script>
</asp:Content>
