<%@ Page Title="Consulta de Acertos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstAcerto.aspx.cs" Inherits="Glass.UI.Web.Listas.LstAcerto" %>

<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

    function openRpt(exportarExcel) {
        var idPedido = FindControl("txtNumPedido", "input").value;
        var idLiberarPedido = FindControl("txtNumLiberarPedido", "input").value;
        var idAcerto = FindControl("txtNumAcerto", "input").value;
        var idCliente = FindControl("txtNumCli", "input").value;
        var nomeCliente = FindControl("txtNomeCliente", "input").value;
        var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
        var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
        var idFormaPagto = FindControl("drpFormaPagto", "select").value;
        var numNotaFiscal = FindControl("txtNumNotaFiscal", "input").value;
    
        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaAcerto&idPedido=" + idPedido + "&idLiberarPedido=" + idLiberarPedido +
            "&idAcerto=" + idAcerto + "&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente + "&dataIni=" + dataIni + "&dataFim=" + dataFim +
            "&idFormaPagto=" + idFormaPagto + "&numNotaFiscal=" + numNotaFiscal + "&exportarExcel=" + exportarExcel);
            
        return false;
    }

    function openRptAcerto(idAcerto) {
        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Acerto&idAcerto=" + idAcerto);
        return false;
    }

    function getCli(idCli)
    {
        if (idCli.value == "") {
            openWindow(570, 760, '../Utils/SelCliente.aspx');
            return false;
        }

        var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');
        
        if (retorno[0] == "Erro")
        {
            alert(retorno[1]);
            idCli.value = "";
            FindControl("txtNomeCliente", "input").value = "";
            return false;
        }

        FindControl("txtNomeCliente", "input").value = retorno[1];
    }

    function openRptProm(idAcerto) 
    {
        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=NotaPromissoria&idAcerto=" + idAcerto);
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumLiberarPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Num. Acerto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumAcerto" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Nota Fiscal" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumNotaFiscal" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Período"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" 
                                ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" 
                                ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Forma Pagto." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFormaPagto" runat="server" DataSourceID="odsFormaPagto"
                                DataTextField="Descricao" DataValueField="IdFormaPagto" AutoPostBack="True" OnSelectedIndexChanged="drpFormaPagto_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoBoleto" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoBoleto"
                                DataTextField="Descricao" DataValueField="IdTipoBoleto" Visible="False" AutoPostBack="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
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
                <asp:GridView GridLines="None" ID="grdAcerto" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdAcerto" DataSourceID="odsAcerto"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Nenhum acerto encontrado com o filtro aplicado."
                    OnPreRender="grdAcerto_PreRender">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkCancelar" runat="server" CommandName="Cancelar" OnClientClick='<%# "openWindow(200, 500, \"../Utils/SetMotivoCancReceb.aspx?tipo=acerto&id=" + Eval("IdAcerto") + "\"); return false" %>'
                                    Visible='<%# Eval("CancelarVisible") %>'>
                                     <img src="../Images/ExcluirGrid.gif" border="0" /></asp:LinkButton>
                                <a href="#" onclick="return openRptAcerto(<%# Eval("IdAcerto") %>);">
                                    <img src="../Images/relatorio.gif" border="0" title="Visualizar dados do Acerto"></a>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Nota.gif" OnClientClick='<%# "openRptProm(" + Eval("IdAcerto") + "); return false" %>'
                                    ToolTip="Nota promissória" Visible='<%# Eval("ExibirNotaPromissoria") %>' />
                                <asp:HiddenField ID="hdfRenegociacao" runat="server" Value='<%# Eval("Renegociacao") %>' />
                                 <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdAcerto") %>&tipo=acerto&#039;); return false;'>
                                    <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdAcerto" HeaderText="Acerto" SortExpression="IdAcerto" />
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="IdPedido, IdLiberarPedido" />
                        <asp:BoundField DataField="IdNomeCliente" HeaderText="Cliente" SortExpression="IdNomeCliente" />
                        <asp:BoundField DataField="Funcionario" HeaderText="Funcionário" SortExpression="Funcionario" />
                        <asp:BoundField DataField="TotalAcerto" DataFormatString="{0:C}" HeaderText="Total"
                            SortExpression="TotalAcerto" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="Situacao" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="Obs" HeaderText="Obs" SortExpression="Obs" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("IdAcerto") %>'
                                    Tabela="Acerto" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <div style="color: blue; text-align: center">
                    Os acertos em azul foram renegociados.
                </div>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAcerto" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetByCliListCount" SelectMethod="GetByCliList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.AcertoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumAcerto" Name="numAcerto" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumLiberarPedido" Name="idLiberarPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpFormaPagto" Name="idFormaPagto" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipoBoleto" Name="idTipoBoleto" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumNotaFiscal" Name="numNotaFiscal" PropertyName="Text  "
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForConsultaConta"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoBoleto" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.TipoBoletoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
