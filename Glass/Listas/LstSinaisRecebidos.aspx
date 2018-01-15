<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstSinaisRecebidos.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstSinaisRecebidos" Title="Sinais Recebidos" %>

<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
    function getCli(idCli)
    {
        if (idCli.value == "")
            return;

        var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');
        
        if (retorno[0] == "Erro")
        {
            alert(retorno[1]);
            idCli.value = "";
            FindControl("txtNome", "input").value = "";
            return false;
        }
        
        FindControl("txtNome", "input").value = retorno[1];
    }

    function openRpt(exportarExcel)
    {
        var idCli = FindControl("txtNumCli", "input").value;
        var idPedido = FindControl("txtNumPedido", "input").value;
        var isPagtoAntecipado = FindControl("hdfPagtoAntecipado", "input").value;
        var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
        var horaIni = FindControl("ctrlDataIni_txtHora", "input").value;
        var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
        var horaFim = FindControl("ctrlDataFim_txtHora", "input").value;
        var ordenacao = FindControl("drpOrdenacao", "select").value;

        if (horaIni != "") dataIni = dataIni + " " + horaIni;
        if (horaFim != "") dataFim = dataFim + " " + horaFim;
        idCli = idCli == "" ? 0 : idCli;
        idPedido = idPedido == "" ? 0 : idPedido;

        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=SinaisRecebidos&IdCli=" + idCli + "&IdPedido=" + idPedido +
            "&pagtoAntecipado=" + isPagtoAntecipado + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&ordenacao=" + ordenacao + "&exportarExcel=" + exportarExcel);
            
        return false;
    }

    function openRptInd(idSinal)
    {
        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Sinal&IdSinal=" + idSinal);
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Num. Sinal" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumSinal" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Data Rec." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="true" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenacao" runat="server" >
                                <asp:ListItem Value="0" Text="Nenhum"></asp:ListItem>
                                <asp:ListItem Value="1" Text="Pedido"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Cliente"></asp:ListItem>
                                <asp:ListItem Value="3" Text="Data Recebimento"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
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
        <tr runat="server" id="inserirPagtoAntecipado">
            <td align="center">
                <asp:LinkButton ID="lnkInserirPagto" runat="server" onclick="lnkInserirPagto_Click">Efetuar Pagamento Antecipado</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdSinaisReceber" runat="server" AllowPaging="True"
                    AllowSorting="True" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataSourceID="odsSinaisReceber" AutoGenerateColumns="False"
                    PageSize="15">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "openRptInd(" + Eval("IdSinal") + "); return false" %>' />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancelar" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick='<%# "openWindow(200, 500, \"../Utils/SetMotivoCancReceb.aspx?tipo=sinal&id=" + Eval("IdSinal") + "\"); return false" %>'
                                    ToolTip="Cancelar recebimento" OnLoad="imbCancelar_Load" Visible='<%# Eval("CancelarVisible") %>' />
                                <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("idSinal") %>&tipo=pagtoAntecipado&#039;); return false;'>
                                    <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdSinal" HeaderText="Núm. Sinal" SortExpression="IdSinal" />
                        <asp:BoundField DataField="IdNomeCliente" HeaderText="Cliente" ReadOnly="True" SortExpression="IdCliente" />
                        <asp:BoundField DataField="TotalSinal" DataFormatString="{0:C}" HeaderText="Valor Sinal"
                            SortExpression="TotalSinal" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data Rec." SortExpression="DataCad" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="Situacao" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc3:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                                    IdRegistro='<%# Eval("IdSinal") %>' Tabela="Sinal" />
                                <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("IdSinal") %>'
                                    Tabela="Sinal" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Obs" HeaderText="Obs" SortExpression="Obs" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(); return false;"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSinaisReceber" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.SinalDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumSinal" Name="idSinal" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:Parameter Name="idFormaPagto" Type="UInt32" />
                        <asp:ControlParameter ControlID="hdfPagtoAntecipado" Name="isPagtoAntecipado" PropertyName="Value" Type="Boolean" />
                        <asp:ControlParameter ControlID="drpOrdenacao" Name="ordenacao" PropertyName="SelectedValue" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfPagtoAntecipado" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
