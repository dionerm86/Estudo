<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlFormaPagto.ascx.cs"
    Inherits="Glass.UI.Web.Controls.ctrlFormaPagto" %>

<%@ Register src="ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc1" %>

<div id="<%= this.ClientID %>" align="center">

    <script type="text/javascript">
    function validaData(val, args)
    {
        args.IsValid = isDataValida(args.Value);
    }
    </script>

    <table id="<%= this.ClientID %>_tblCabecalho">
        <tr style="<%= !ExibirCliente ? "display: none": "" %>">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Cliente:
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px"></asp:TextBox>
                            <asp:LinkButton ID="lnkSelCliente" runat="server" Style="position: relative; top: 3px"
                                OnLoad="lnkSelCliente_Load">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <asp:CustomValidator ID="ctvCliente" runat="server" ErrorMessage="Selecione o cliente para a forma de pagamento"
                    ClientValidationFunction="validaCliente" ControlToValidate="txtNumCli" Display="None"
                    ValidateEmptyText="True"></asp:CustomValidator>
                <asp:HiddenField ID="hdfExibirCliente" runat="server" Value="False" />
            </td>
        </tr>
        <tr>
            <td align="center" style="padding-left: 3px; <%= !ExibirCredito ? "display: none": "" %>">
                <table cellpadding="1" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Label ID="lblInicioCredito" runat="server" Font-Size="10pt" Text="O cliente possui"></asp:Label>
                            <asp:Label ID="lblCredito" runat="server" Font-Size="10pt"></asp:Label>
                            <asp:Label ID="lblFimCredito" runat="server" Font-Size="10pt" Text="de Crédito"></asp:Label>
                            <span style="visibility: hidden">
                                <input type="text" style="width: 2px; border: 0px" />
                            </span>
                        </td>
                        <td style='<%= !ExibirUsarCredito ? "display: none": "" %>'>
                            <asp:CheckBox ID="chkUsarCredito" runat="server" Checked="True" Text="Usar crédito"
                                Style="display: none" />
                        </td>
                        <td style='<%= !ExibirUsarCredito ? "display: none": "" %>'>
                            <span runat="server" id="usarCredito" style="display: none">&nbsp;
                                <table cellpadding="0" cellspacing="0" style="display: inline-table">
                                    <tr>
                                        <td style="padding-right: 2px">
                                            R$
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCreditoUtilizado" runat="server" onkeypress="return soNumeros(event, false, true)"
                                                Width="70px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </span>
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfExibirCredito" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblValorObra" runat="server" Text="" Font-Size="10pt"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center" style="padding: 2px">
                <table cellpadding="1" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Label ID="lblTextoValorASerPago" runat="server" Font-Size="Large" Text="Valor a ser Pago:"></asp:Label>
                            <asp:Label ID="lblValorASerPago" runat="server" Font-Size="Large"></asp:Label>
                        </td>
                        <td style='<%= !ExibirGerarCredito ? "display: none": "" %>'>
                            &nbsp;
                            <asp:CheckBox ID="chkGerarCredito" runat="server" Text="Gerar Crédito" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table cellpadding="1" cellspacing="0">
                    <tr>
                        <td style='<%= !ExibirDataRecebimento ? "display: none": "" %>'>
                            <asp:Label ID="lblDataRecebimento" runat="server" Text="Data Receb."></asp:Label>
                        </td>
                        <td style='<%= !ExibirDataRecebimento ? "display: none": "" %>'>
                            <asp:TextBox ID="txtDataRecebimento" runat="server" onkeypress="return mascara_data(event, this), soNumeros(event, true, true);"
                                Width="70px" MaxLength="10"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataRecebimento', this)" ToolTip="Alterar" />
                            <asp:CustomValidator ID="ctvData" runat="server" ErrorMessage="*" ClientValidationFunction="validaData"
                                ControlToValidate="txtDataRecebimento" Display="Dynamic" ValidateEmptyText="true"></asp:CustomValidator>
                        </td>
                        <td style='<%= !ExibirJuros ? "display: none": "" %>'>
                            &nbsp; Juros (R$)
                        </td>
                        <td style='<%= !ExibirJuros ? "display: none": "" %>'>
                            <asp:TextBox ID="txtJuros" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            &nbsp;
                            <asp:CheckBox ID="chkRecebimentoParcial" runat="server" Text="Receb. Parcial" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="comissao" style="display: none" runat="server" visible="<%# ExibirComissaoComissionado %>">
            <td align="center">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            Valor da comissão (R$)
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorComissao" runat="server" Enabled="false" Width="50px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkComissaoComissionado" runat="server" Text="Descontar comissão do comissionado" />
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
    </table>
    <asp:Table ID="tblFormaPagto" runat="server" OnLoad="tblFormaPagto_Load" CellSpacing="-1"
        CellPadding="4">
    </asp:Table>
    <span id="<%= this.ClientID %>_Troco" style='<%= !lblTroco.Visible ? "display: none": "" %>'>
        <br />
        <asp:Label ID="lblTroco" runat="server" Text="" Visible="false"></asp:Label>
        <br />
    </span><span id="<%= this.ClientID %>_Restante" style='<%= !lblRestante.Visible ? "display: none": "" %>'>
        <br />
        <asp:Label ID="lblRestante" runat="server" Text="" ForeColor="Green"></asp:Label>
        <br />
    </span>
     <table id="<%= this.ClientID %>_TabelaCNI" style="margin-top: 8px; margin-bottom: 8px">
    </table>
    <table id="<%= this.ClientID %>_TabelaCheques" style="margin-top: 8px; margin-bottom: 8px">
    </table>
    <asp:TextBox ID="txtValorPago" runat="server" Style="display: none" EnableViewState="false"></asp:TextBox>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForRecebConta"
        TypeName="Glass.Data.DAL.FormaPagtoDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCartao" runat="server" SelectMethod="ObtemListaPorTipo"
        TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO">
        <SelectParameters>
            <asp:Parameter DefaultValue="0" Name="tipo" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.ContaBancoDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoBoleto" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.TipoBoletoDAO">
    </colo:VirtualObjectDataSource>
    <asp:HiddenField ID="hdfCheques" runat="server" />
    <asp:HiddenField ID="hdfCNI" runat="server" />
    <asp:HiddenField ID="hdfFormaPagtoCreditoCliente" runat="server" />
    <asp:HiddenField ID="hdfJurosMin" runat="server" />
    <asp:CustomValidator ID="ctvFormasPagto" runat="server" ErrorMessage="Selecione pelo menos uma forma de pagamento"
        Display="None" ClientValidationFunction="validaFormasPagto"></asp:CustomValidator>
    <asp:ValidationSummary ID="vsuSumario" runat="server" ShowMessageBox="true" ShowSummary="false" />
</div>
