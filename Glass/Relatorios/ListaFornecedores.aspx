<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" Title="Fornecedores"
    CodeBehind="ListaFornecedores.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaFornecedores" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(exportarExcel) {
            var tipoPessoa = FindControl("drpTipoPessoa", "select").value;
            var idFornec = FindControl("txtNumFornec", "input").value;
            var nomeFornec = FindControl("txtNome", "input").value;
            var cnpj = FindControl("txtCnpj", "input").value;
            var comCredito = FindControl("chkCredito", "input").checked;
            var situacao = FindControl("drpSituacao", "select").value;

            if (idFornec == "")
                idFornec = 0;

            openWindow(600, 800, "RelBase.aspx?rel=ListaFornecedores&tipoPessoa=" + tipoPessoa + "&idFornec=" + idFornec + "&nomeFornec=" +
                nomeFornec + "&cnpj=" + cnpj + "&comCredito=" + comCredito + "&situacao=" + situacao + "&exportarExcel=" + exportarExcel);

            return false;
        }

        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
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
                            <asp:Label ID="Label3" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumFornec" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getFornec(FindControl('txtNumFornec', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Tipo Pessoa" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoPessoa" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Física</asp:ListItem>
                                <asp:ListItem Value="2">Jurídica</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                        <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                            OnClick="imgPesq_Click" />
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Ativo</asp:ListItem>
                                <asp:ListItem Value="2">Inativo</asp:ListItem>
                            </asp:DropDownList>
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
                <asp:Label ID="Label5" runat="server" Text="CNPJ" ForeColor="#0066FF"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtCnpj" runat="server" onkeypress="if (isEnter(event)) return false;"
                    Width="170px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
            </td>
            <td>
                <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getFornec(FindControl('txtNumFornec', 'input'));" OnClick="imgPesq_Click" />
            </td>
            <td>
                <asp:CheckBox ID="chkCredito" runat="server" Text="Fornecedores com crédito" AutoPostBack="True" />
            </td>
            <td>
                <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getFornec(FindControl('txtNumFornec', 'input'));" OnClick="imgPesq_Click" />
            </td>
        </tr>
    </table>
    </td> </tr>
    <tr>
        <td align="center">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td align="center">
            <asp:GridView GridLines="None" ID="grdClientes" runat="server" AllowPaging="True"
                AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsFornecedores"
                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                EditRowStyle-CssClass="edit" PageSize="15" Style="margin-top: 0px">
                <PagerSettings PageButtonCount="20" />
                <Columns>
                    <asp:BoundField DataField="Idfornec" HeaderText="Num." SortExpression="Idfornec" />
                    <asp:BoundField DataField="Nome" HeaderText="Razão Social ou Nome Fantasia" SortExpression="Razaosocial, NomeFantasia" />
                    <asp:BoundField DataField="CpfCnpj" HeaderText="CPF/CNPJ" SortExpression="CpfCnpj" />
                    <asp:BoundField DataField="RgInscEst" HeaderText="Insc. Est." SortExpression="RgInscEst" />
                    <asp:BoundField DataField="EnderecoCompleto" HeaderText="Endereço" SortExpression="Endereco, Numero, Bairro" />
                    <asp:BoundField DataField="Telcont" HeaderText="Tel. Cont." SortExpression="Telcont">
                    </asp:BoundField>
                </Columns>
                <PagerStyle />
                <EditRowStyle />
                <AlternatingRowStyle />
            </asp:GridView>
            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFornecedores" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                SelectCountMethod="GetRptCount" SelectMethod="GetForListaRpt" SortParameterName="sortExpression"
                StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.FornecedorDAO"
                >
                <SelectParameters>
                    <asp:ControlParameter ControlID="drpTipoPessoa" Name="tipoPessoa" PropertyName="SelectedValue"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="txtNumFornec" Name="idFornec" PropertyName="Text"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtNome" Name="nomeFornec" PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                        Type="Int32" />
                    <asp:ControlParameter ControlID="txtCnpj" Name="cnpj" PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="chkCredito" Name="comCredito" PropertyName="Checked"
                        Type="Boolean" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
        </td>
    </tr>
    <tr>
        <td align="center">
            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;">
                    <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
        </td>
    </tr>
    </table>
</asp:Content>
