<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstSinaisPagos.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstSinaisPagos" Title="Sinais Pagos"%>

<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

    function getFornec(idFornec)
    {
        if (idFornec.value == "")
            return;

        var retorno = MetodosAjax.GetFornec(idFornec.value).value.split(';');
        
        if (retorno[0] == "Erro")
        {
            alert(retorno[1]);
            idFornec.value = "";
            FindControl("txtNome", "input").value = "";
            return false;
        }
        
        FindControl("txtNome", "input").value = retorno[1];
    }

    function openRpt()
    {
        var idFornec = FindControl("txtNumFornec", "input").value;
        var idCompra = FindControl("txtNumCompra", "input").value;
        var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
        var horaIni = FindControl("ctrlDataIni_txtHora", "input").value;
        var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
        var horaFim = FindControl("ctrlDataFim_txtHora", "input").value;

        if (horaIni != "") dataIni = dataIni + " " + horaIni;
        if (horaFim != "") dataFim = dataFim + " " + horaFim;
        idFornec = idFornec == "" ? 0 : idFornec;
        idCompra = idCompra == "" ? 0 : idCompra;

        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=SinaisPagos&IdFornec=" + idFornec + "&IdCompra=" + idCompra +
            "&dataIni=" + dataIni + "&dataFim=" + dataFim);
            
        return false;
    }

    function openRptInd(idSinal)
    {
        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=SinalCompra&IdSinalCompra=" + idSinal);
    }

    </script>
    
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Num. Compra" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCompra" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" style="height: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
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
                <asp:GridView GridLines="None" ID="grdSinaisReceber" runat="server" AllowPaging="True"
                    AllowSorting="True" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataSourceID="odsSinaisReceber" AutoGenerateColumns="False"
                    PageSize="15" EmptyDataText="Nenhum sinal encontrado.">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "openRptInd(" + Eval("IdSinalCompra") + "); return false" %>' />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancelar" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick='<%# "openWindow(200, 500, \"../Utils/SetMotivoCancPagtoSinalCompra.aspx?&IdSinalCompra=" + Eval("IdSinalCompra") + "\"); return false" %>'
                                    ToolTip="Cancelar recebimento" Visible='<%# Eval("CancelarVisible") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdSinalCompra" HeaderText="Núm. Sinal" SortExpression="IdSinalCompra" />
                        <asp:BoundField DataField="IdNomeFornecedor" HeaderText="Fornecedor" ReadOnly="True" SortExpression="NomeFornec" />
                        <asp:BoundField DataField="TotalSinal" DataFormatString="{0:C}" HeaderText="Valor Sinal"
                            SortExpression="TotalSinal" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data Rec." SortExpression="DataCad" />
                       
                        <asp:TemplateField>
                            <ItemTemplate>
                            <uc3:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                                    IdRegistro='<%# Eval("IdSinalCompra") %>' Tabela="SinalCompra" />
                                <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("IdSinalCompra") %>'
                                    Tabela="SinalCompra" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Obs" HeaderText="Obs" SortExpression="Obs" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSinaisReceber" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.SinalCompraDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumSinal" Name="idSinal" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCompra" Name="idCompra" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumFornec" Name="idFornec" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:Parameter Name="idFormaPagto" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>

</asp:Content>
