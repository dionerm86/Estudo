<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstSinaisNaoPagos.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstSinaisNaoPagos" Title="Sinais a Pagar" %>

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

            idFornec = idFornec == "" ? 0 : idFornec;
            idCompra = idCompra == "" ? 0 : idCompra;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=SinaisPagar&IdFornec=" + idFornec + "&IdCompra=" + idCompra);
            return false;
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
                                OnClick="imgPesq_Click" Style="width: 16px" />
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
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdSinaisReceber" runat="server" AllowPaging="True"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AllowSorting="True" DataSourceID="odsSinaisReceber"
                    EmptyDataText="Nenhum sinal encontrado." AutoGenerateColumns="False" PageSize="15">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="IdCompra" HeaderText="Num. Compra" SortExpression="IdCompra" />
                        <asp:BoundField DataField="IdNomeFornec" HeaderText="Fornecedor" ReadOnly="True"
                            SortExpression="NomeFornec" />
                        <asp:BoundField DataField="ValorEntrada" DataFormatString="{0:C}" HeaderText="Valor Sinal"
                            SortExpression="ValorEntrada" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data Compra"
                            SortExpression="DataCad" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Total Compra."
                            SortExpression="Total" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();">
                <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSinaisReceber" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetSinaisNaoPagosCount" SelectMethod="GetSinaisNaoPagos" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.CompraDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumFornec" Name="idFornec" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCompra" Name="idCompra" PropertyName="Text"
                            Type="UInt32" />
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
