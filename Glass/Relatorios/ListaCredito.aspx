<%@ Page Title="Movimentação de Crédito por Cliente" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ListaCredito.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaCredito" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = ListaCredito.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function openRpt() {
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idCli = FindControl("txtNumCli", "input").value;
            var sort = FindControl("hdfSort", "input").value;
            var totalGerado = FindControl("lblCreditoGerado", "span").innerHTML;
            var totalUtilizado = FindControl("lblCreditoUtilizado", "span").innerHTML;
            var tipoMovimentacao = FindControl("ddoTipoMov", "select").itens();

            if (idCli == "")
                idCli = 0;

            openWindow(600, 800, "RelBase.aspx?rel=MovCredito&idCliente=" + idCli + "&inicio=" + dtIni + "&fim=" + dtFim + "&sort=" + sort +
                "&gerado=" + totalGerado + "&utilizado=" + totalUtilizado + "&movimentacao=" + tipoMovimentacao);
        }
         
        function openRptReferencia(IdPedido,IdAcerto,IdLiberarPedido,IdObra,IdTrocaDevolucao,IdSinal,IdPagto,
            IdDevolucaoPagto,IdDeposito,IdCompra,IdAcertoCheque,IdContaR) {
        
            if (IdTrocaDevolucao > 0)
                openWindow(600, 800, "RelBase.aspx?rel=TrocaDevolucao&idTrocaDevolucao=" + IdTrocaDevolucao);
            else if (IdPedido > 0)
                openWindow(600, 800, "RelPedido.aspx?idPedido=" + IdPedido + "&tipo=0");
            else if (IdAcerto > 0)
                openWindow(600, 800, "RelBase.aspx?rel=Acerto&idAcerto=" + IdAcerto);
            else if (IdLiberarPedido > 0)
                openWindow(600, 800, "RelLiberacao.aspx?idLiberarPedido=" + IdLiberarPedido);
            else if (IdObra > 0)
                openWindow(600, 800, "RelBase.aspx?rel=Obra&idObra=" + IdObra);
            else if (IdSinal > 0)
                openWindow(600, 800, "RelBase.aspx?rel=Sinal&IdSinal=" + IdSinal);
            else if (IdPagto > 0)
                openWindow(600, 800, "RelBase.aspx?rel=Pagto&idPagto=" + IdPagto);
            else if (IdDevolucaoPagto > 0)
                openWindow(600, 800, "RelBase.aspx?rel=DevolucaoPagto&idDevolucaoPagto=" + IdDevolucaoPagto);
            else if (IdDeposito > 0)
                openWindow(600, 800, "RelBase.aspx?Rel=Deposito&idDeposito=" + IdDeposito + "&ordemCheque=1&exportarExcel=false");
            else if (IdCompra > 0)
                openWindow(600, 800, "RelBase.aspx?rel=Compra&idCompra=" + IdCompra);
            else if (IdAcertoCheque > 0)
                openWindow(600, 800, "RelBase.aspx?rel=AcertoCheque&idAcertoCheque=" + IdAcertoCheque);
            else if (IdContaR > 0) {
                debugger;
                var idAcertoParcial = ListaCredito.ObterIdAcertoParcial(IdContaR);
                var idObra = ListaCredito.ObterIdObra(IdContaR);

                if (idAcertoParcial.value != "")
                    openWindow(600, 800, "RelBase.aspx?rel=Acerto&idAcerto=" + idAcertoParcial.value);

                if (idObra.value != "")
                    openWindow(600, 800, "RelBase.aspx?rel=Obra&idObra=" + idObra.value);
            }
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Campo obrigatório"
                                ControlToValidate="txtNumCli">*</asp:RequiredFieldValidator>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="lnkSelCliente" runat="server" OnClick="imgPesq_Click" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="if (FindControl('txtNumCli', 'input').value == ''){ openWindow(590, 760, '../Utils/SelCliente.aspx'); return false;} else return true;">
                            </asp:ImageButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" ValidateEmptyText="true" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" ValidateEmptyText="true" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Movimentações de Crédito" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="ddoTipoMov" runat="server" CheckAll="True">
                                <asp:ListItem Value="1">Gerado</asp:ListItem>
                                <asp:ListItem Value="2">Estorno Utilizado</asp:ListItem>
                                <asp:ListItem Value="3">Utilizado</asp:ListItem>
                                <asp:ListItem Value="4">Estorno Gerado</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
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
                <asp:GridView GridLines="None" ID="grdCredito" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsCredito" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AllowPaging="True" AllowSorting="True" OnRowDataBound="grdCredito_RowDataBound"
                    OnSorting="grdCredito_Sorting" PageSize="15">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfTipoMov" runat="server" Value='<%# Eval("TipoMov") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfIdConta" runat="server" Value='<%# Eval("IdConta") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>                        
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbReferencia" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "openRptReferencia(" +
                                    (Eval("IdPedido") == "" || Eval("IdPedido") == null ? 0 : Eval("IdPedido")) + "," +
                                    (Eval("IdAcerto") == "" || Eval("IdAcerto") == null ? 0 : Eval("IdAcerto")) + "," +
                                    (Eval("IdLiberarPedido") == "" || Eval("IdLiberarPedido") == null ? 0 : Eval("IdLiberarPedido")) + "," +
                                    (Eval("IdObra") == "" || Eval("IdObra") == null ? 0 : Eval("IdObra")) + "," +
                                    (Eval("IdTrocaDevolucao") == "" || Eval("IdTrocaDevolucao") == null ? 0 : Eval("IdTrocaDevolucao")) + "," +
                                    (Eval("IdSinal") == "" || Eval("IdSinal") == null ? 0 : Eval("IdSinal")) + "," +
                                    (Eval("IdPagto") == "" || Eval("IdPagto") == null ? 0 : Eval("IdPagto")) + "," +
                                    (Eval("IdDevolucaoPagto") == "" || Eval("IdDevolucaoPagto") == null ? 0 : Eval("IdDevolucaoPagto")) + "," +
                                    (Eval("IdDeposito") == "" || Eval("IdDeposito") == null ? 0 : Eval("IdDeposito")) + "," +
                                    (Eval("IdCompra") == "" || Eval("IdCompra") == null ? 0 : Eval("IdCompra")) + "," +
                                    (Eval("IdAcertoCheque") == "" || Eval("IdAcertoCheque") == null ? 0 : Eval("IdAcertoCheque")) + "," +
                                    (Eval("IdContaR") == "" || Eval("IdContaR") == null ? 0 : Eval("IdContaR")) + "); return false" %>'
                                    ToolTip="Relatório Referência" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                        <asp:TemplateField HeaderText="Débito" SortExpression="Valor">
                            <ItemTemplate>
                                <asp:Label ID="lblDebito" runat="server" Text='<%# Bind("Valor") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Crédito" SortExpression="Valor">
                            <ItemTemplate>
                                <asp:Label ID="lblCredito" runat="server" Text='<%# Bind("Valor") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Saldo" HeaderText="Saldo" SortExpression="Saldo" />
                        <asp:BoundField DataField="Data" HeaderText="Data" SortExpression="Data" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <table>
                    <tr>
                        <td>
                            Total de crédito utilizado:
                        </td>
                        <td>
                            <asp:Label ID="lblCreditoUtilizado" runat="server" Font-Bold="True" Font-Size="120%"></asp:Label>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            Total de crédito gerado:
                        </td>
                        <td>
                            <asp:Label ID="lblCreditoGerado" runat="server" Font-Bold="True" Font-Size="120%"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            Crédito Atual:
                        </td>
                        <td>
                            <asp:Label ID="lblCreditoAtual" runat="server" Font-Bold="True" Font-Size="120%"></asp:Label>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"
                    Visible="False"><img border="0" 
                    src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCredito" runat="server" SelectMethod="GetCredito" TypeName="Glass.Data.RelDAL.CreditoDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCreditoCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
                    OnSelected="odsCredito_Selected" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="inicio" 
                            PropertyName="Data" Type="DateTime" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="fim" PropertyName="Data" 
                            Type="DateTime" />
                        <asp:ControlParameter ControlID="ddoTipoMov" Name="tipoMovimentacao" 
                            PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfSort" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
