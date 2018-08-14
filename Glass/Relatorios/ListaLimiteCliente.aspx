<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaLimiteCliente.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaLimiteCliente" Title="Débitos do Limite do Cliente" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

    function openRpt(limite)
    {
        var idCliente = FindControl("txtNumCli", "input").value;
        var idPedido = FindControl("txtIdPedido", "input").value;
        var idLiberarPedido = FindControl("txtIdLiberarPedido", "input");
        var buscarItens = FindControl("cbdBuscar", "select").itens();
        var agrupar = FindControl("drpAgrupar", "select");
        var ordenar = FindControl("drpOrdenar", "select");
        var tipoBuscaData = FindControl("drpTipoData", "select").value;
        var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
        var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
        idCliente = idCliente.length > 0 ? idCliente : "0";
        idPedido = idPedido.length > 0 ? idPedido : "0";
        idLiberarPedido = idLiberarPedido != null && idLiberarPedido.value.length > 0 ? idLiberarPedido.value : "0";
        agrupar = agrupar != null ? agrupar.value : "";
        ordenar = ordenar != null ? ordenar.value : 0;
                
        openWindow(600, 800, "RelBase.aspx?rel=LimiteCliente&idCliente=" + idCliente + "&idPedido=" + 
            idPedido + "&idLiberarPedido=" + idLiberarPedido + "&buscarItens=" + buscarItens + "&agrupar=" + agrupar +
            "&ordenar=" + ordenar + "&tipoBuscaData=" + tipoBuscaData + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&limite=" + (limite == true ? "true" : "false"));
    }

    function openRptReferencia(IdPedido, IdAcerto, IdLiberarPedido, IdObra, IdTrocaDevolucao, IdSinal, IdPagto,
        IdDevolucaoPagto, IdDeposito, IdCompra, IdAcertoCheque, IdNf) {

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
        else if (IdDevolucaoPagto > 0)
            openWindow(600, 800, "RelBase.aspx?rel=DevolucaoPagto&idDevolucaoPagto=" + IdDevolucaoPagto);
        else if (IdDeposito > 0)
            openWindow(600, 800, "RelBase.aspx?Rel=Deposito&idDeposito=" + IdDeposito + "&ordemCheque=1&exportarExcel=false");
        else if (IdAcertoCheque > 0)
            openWindow(600, 800, "RelBase.aspx?rel=AcertoCheque&idAcertoCheque=" + IdAcertoCheque);
        else if (IdNf > 0)
            openWindow(600, 800, "NFe/RelBase.aspx?rel=Danfe&idNf=" + IdNf);
    }

    function getCli(idCli)
    {
        if (idCli.value == "") {
            openWindow(600, 800, "../Utils/SelCliente.aspx");
            return false;
        }

        var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idCli.value = "";
            FindControl("txtNomeCliente", "input").value = "";
            return false;
        }

        FindControl("txtNomeCliente", "input").value = retorno[1];
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Agrupar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAgrupar" runat="server">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="1" Selected="True">Cliente</asp:ListItem>
                                <asp:ListItem Value="2">Liberação</asp:ListItem>
                                <asp:ListItem Value="3">Rota</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdPedido" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq0', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdLiberarPedido" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq0', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Buscar" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdBuscar" runat="server" OnLoad="cbdBuscar_Load" Title="Selecione os itens">
                                <asp:ListItem Value="1">Contas a receber</asp:ListItem>
                                <asp:ListItem Value="2">Pedidos Ativos</asp:ListItem>
                                <asp:ListItem Value="3">Pedidos Conferidos</asp:ListItem>
                                <asp:ListItem Value="4">Pedidos Confirmados</asp:ListItem>
                                <asp:ListItem Value="5">Cheques</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoData" runat="server">
                                <asp:ListItem Value="1">Débito</asp:ListItem>
                                <asp:ListItem Value="2">Venc.</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            a
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblOrdenar" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server">
                                <asp:ListItem Value="1">Cliente</asp:ListItem>
                                <asp:ListItem Value="2">Débito</asp:ListItem>
                                <asp:ListItem Value="3">Pedido</asp:ListItem>
                                <asp:ListItem Value="4">Liberação</asp:ListItem>
                                <asp:ListItem Value="5">Data Venc.</asp:ListItem>
                                <asp:ListItem Value="6">Data Venc Cresc.</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
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
                <asp:GridView GridLines="None" ID="grdLimiteCliente" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsLimiteCliente" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" AllowPaging="True"
                    AllowSorting="True" PageSize="30" OnDataBound="grdLimiteCliente_DataBound">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbReferencia" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "openRptReferencia(" +
                                    (Eval("IdPedido") == null || Eval("IdPedido").ToString() == "" ? 0 : Eval("IdPedido")) + "," +
                                    (Eval("IdAcerto") == null || Eval("IdAcerto").ToString() == "" ? 0 : Eval("IdAcerto")) + "," +
                                    (Eval("IdLiberarPedido") == null || Eval("IdLiberarPedido").ToString() == "" ? 0 : Eval("IdLiberarPedido")) + "," +
                                    (Eval("IdObra") == null || Eval("IdObra").ToString() == "" ? 0 : Eval("IdObra")) + "," +
                                    (Eval("IdTrocaDevolucao") == null || Eval("IdTrocaDevolucao").ToString() == "" ? 0 : Eval("IdTrocaDevolucao")) + "," +
                                    (Eval("IdSinal") == null || Eval("IdSinal").ToString() == "" ? 0 : Eval("IdSinal")) + ",0," +
                                    (Eval("IdDevolucaoPagto") == null || Eval("IdDevolucaoPagto").ToString() == "" ? 0 : Eval("IdDevolucaoPagto")) + ",0,0," +
                                    (Eval("IdAcertoCheque") == null || Eval("IdAcertoCheque").ToString() == "" ? 0 : Eval("IdAcertoCheque")) + "," + 
                                    (Eval("IdNf") == null || Eval("IdNf").ToString() == "" ? 0 : Eval("IdNf")) + "); return false" %>'
                                    ToolTip="Relatório Referência" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" 
                            SortExpression="Referencia" />
                        <asp:BoundField DataField="IdNomeCli" HeaderText="Cliente" SortExpression="IdNomeCli" />
                        <asp:BoundField DataField="ValorVec" HeaderText="Débito" SortExpression="ValorVec"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data Débito" SortExpression="DataCad"
                            DataFormatString="{0:d}" />
                        <asp:BoundField DataField="DataVec" DataFormatString="{0:d}" HeaderText="Data Venc."
                            SortExpression="DataVec" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLimiteCliente" runat="server" SelectMethod="GetDebitosList"
                    TypeName="Glass.Data.DAL.ContasReceberDAO" OnSelected="odsLimiteCliente_Selected"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetDebitosCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtIdPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtIdLiberarPedido" Name="idLiberarPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="cbdBuscar" Name="buscarItens" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpOrdenar" Name="ordenar" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpTipoData" Name="tipoBuscaData" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblTituloChequesEmAberto" runat="server" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblTotalChequesEmAberto" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblTituloChequesDevolvidos" runat="server" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblTotalChequesDevolvidos" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblTituloChequesProtestados" runat="server" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblTotalChequesProtestados" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblTituloCheques" runat="server" Font-Bold="True" 
                                ForeColor="#3366FF"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblTotalCheques" runat="server" Font-Bold="False" 
                                ForeColor="#3366FF"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblTituloContasReceber" runat="server" Font-Bold="True" 
                                ForeColor="#3366FF"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblTotalContasReceber" runat="server" Font-Bold="False" 
                                ForeColor="#3366FF"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblTituloPedidosEmAberto" runat="server" Font-Bold="True" 
                                ForeColor="#3366FF"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblTotalPedidosEmAberto" runat="server" Font-Bold="False" 
                                ForeColor="#3366FF"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblTituloTotalDebitos" runat="server" Font-Bold="True" 
                                ForeColor="Blue" Font-Size="Medium"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblTotalDebitos" runat="server" Font-Bold="False" 
                                Font-Size="Medium" ForeColor="Blue"></asp:Label>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table>
                     <tr>
                        <td>
                            <asp:Label ID="lblTituloCreditoCliente" runat="server" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblCreditoCliente" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                     <tr>
                        <td>
                            <asp:Label ID="lblTituloLimiteUtilizado" runat="server" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblLimiteUtilizado" runat="server" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                     <tr>
                        <td>
                            <asp:Label ID="lblTituloLimiteConfigurado" runat="server" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblLimiteConfigurado" runat="server" Font-Bold="False"></asp:Label>
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
                <asp:LinkButton ID="lkbImprimir" runat="server" OnClientClick="openRpt(false); return false;"><img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lkbImprimirLimite" runat="server" OnClientClick="openRpt(true); return false;"><img src="../Images/Printer.png" border="0" /> Imprimir Resumo</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
