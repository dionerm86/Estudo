<%@ Page Title="Clientes com Crédito" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstClienteCredito.aspx.cs" Inherits="Glass.UI.Web.Listas.LstClienteCredito" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
<script type="text/javascript">

    function openRpt(exportarExcel) {
        var idCli = FindControl("txtNumCli", "input").value;
        var nomeCli = FindControl("txtNome", "input").value;
        var telefone = FindControl("txtTelefone", "input").value;
        var cpfCnpj = FindControl("txtCnpj", "input").value

        if (idCli == "") idCli = 0;

        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ClientesCredito&idCliente=" + idCli + "&nomeCli=" + nomeCli + "&telefone=" + telefone +
            "&cpfCnpj=" + cpfCnpj + "&exportarExcel=" + exportarExcel);

        return false;
    }


    function getCli(idCli) {
        if (idCli.value == "")
            return;

        var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idCli.value = "";
            FindControl("txtNome", "input").value = "";
            return false;
        }

        FindControl("txtNome", "input").value = retorno[1];
    }

</script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                  <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" 
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Telefone" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTelefone" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="CPF/CNPJ" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCnpj" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
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
                <asp:GridView GridLines="None" ID="grdCliente" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsCliente"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="Idcli" 
                    EmptyDataText="Nenhum cliente com crédito encontrado.">
                    <Columns>
                        <asp:BoundField DataField="IdNome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="EnderecoCompleto" HeaderText="Endereço" SortExpression="EnderecoCompleto" />
                        <asp:BoundField DataField="CpfCnpj" HeaderText="CPF/CNPJ" SortExpression="CpfCnpj" />
                        <asp:BoundField DataField="Telefone" HeaderText="Tel. Cont." SortExpression="Telefone">
                        </asp:BoundField>
                        <asp:BoundField DataField="TelCel" HeaderText="Celular" SortExpression="TelCel" />
                        <asp:BoundField DataField="Credito" DataFormatString="{0:C}" HeaderText="Crédito"
                            SortExpression="Credito" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;">
                    <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCliente" runat="server" DataObjectTypeName="Glass.Data.Model.Cliente"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountListCredito" SelectMethod="GetListCredito" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ClienteDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="codCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNome" Name="nome" PropertyName="Text" Type="String" />
                        <asp:Parameter Name="bairro" Type="String" />
                        <asp:ControlParameter ControlID="txtTelefone" Name="telefone" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtCnpj" Name="cpfCnpj" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
