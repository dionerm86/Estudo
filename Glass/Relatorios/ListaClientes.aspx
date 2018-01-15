<%@ Page Title="Clientes" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="ListaClientes.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaClientes" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var dataCompraIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataCompraFim = FindControl("ctrlDataFim_txtData", "input").value;
            var tipoPessoa = FindControl("drpTipoPessoa", "select").value;
            var revenda = FindControl("chkRevenda", "input").checked ? 1 : 0;
            var compra = FindControl("chkCompra", "input").checked;
            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var codRota = FindControl("txtRota", "input").value;
            var idFunc = FindControl("drpFuncionario", "select").value;
            var agruparVend = FindControl("chkAgruparVend", "input").checked;
            var apenasSemRota = FindControl("chkApenasSemRota", "input") == null ? "false" : FindControl("chkApenasSemRota", "input").checked;
            var apenasSemPrecoTabela = FindControl("chkApenasSemPrecoTabela", "input") == null ? "false" : FindControl("chkApenasSemPrecoTabela", "input").checked;
            var limite = FindControl("ddlLimite", "select").value;

            if (idCli == "")
                idCli = 0;

            openWindow(600, 800, "RelBase.aspx?rel=ListaClientes" +
                "&idCli=" + idCli + "&nomeCli=" + nomeCli +
                "&tipoPessoa=" + tipoPessoa +
                "&Revenda=" + revenda +
                "&dataCompraIni=" + dataCompraIni + "&dataCompraFim=" + dataCompraFim +
                "&comCompra=" + compra +
                "&situacao=" + situacao +
                "&idFunc=" + idFunc +
                "&agruparVend=" + agruparVend +
                "&limite=" + limite +
                "&semTabela=" + apenasSemPrecoTabela +
                "&apenasSemRota=" + apenasSemRota +
                "&codRota=" + codRota + "&exportarExcel=" + exportarExcel);

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

        function openRota() {
            if (FindControl("txtRota", "input").value != "")
                return true;

            openWindow(500, 700, "../Utils/SelRota.aspx");
            return false;
        }

        function setRota(codInterno) {
            FindControl("txtRota", "input").value = codInterno;
        }

        function disableRota() {
            FindControl("txtRota", "input").value = "";

            if (FindControl("chkApenasSemRota", "input").checked) {
                FindControl("lblRota", "span").style.visibility = 'hidden';
                FindControl("txtRota", "input").style.visibility = 'hidden';
                FindControl("imgPesqRota", "input").style.visibility = 'hidden';
            }
            else {
                FindControl("lblRota", "span").style.visibility = 'visible';
                FindControl("txtRota", "input").style.visibility = 'visible';
                FindControl("imgPesqRota", "input").style.visibility = 'visible';
            }
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
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
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
                            <asp:Label ID="Label11" runat="server" Text="Revenda Somente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkRevenda" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="left">
                            <asp:Label ID="Label2" runat="server" Text="Data de Aniversário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <uc1:ctrlData ID="ctrlDataNiverIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataNiverFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label10" runat="server" Text="Período de Compra" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Com Compras" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkCompra" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AppendDataBoundItems="True" DataSourceID="odsSituacaoCliente"
                                DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label14" runat="server" Text="Vendedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq8" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparVend" runat="server" Text="Agrupar por Vendedor" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Limite" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlLimite" runat="server" AppendDataBoundItems="True"
                                >
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Acima do Limite</asp:ListItem>
                                <asp:ListItem Value="2">Dentro do Limite</asp:ListItem>
                                <asp:ListItem Value="3">Sem Limite Cadastrado</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblApenasSemPrecoTabela" runat="server" Text="Sem Preço de Tabela Definido"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkApenasSemPrecoTabela" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqSemTabela" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblApenasSemRota" runat="server" Text="Apenas Clientes sem Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkApenasSemRota" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqSemRota" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblRota" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtRota" runat="server" MaxLength="20" Width="80px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqRota" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" OnClientClick="return openRota();" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdClientes" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdCli" DataSourceID="odsClientes"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" PageSize="15" EmptyDataText="Nenhum cliente encontrado">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="IdNome" HeaderText="Nome" SortExpression="IdNome" />
                        <asp:BoundField DataField="CpfCnpj" HeaderText="CPF/CNPJ" SortExpression="CpfCnpj" />
                        <asp:BoundField DataField="EnderecoCompleto" HeaderText="Endereço" SortExpression="EnderecoCompleto" />
                        <asp:BoundField DataField="Telefone" HeaderText="Tel. Cont." SortExpression="Telefone" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="DescrSituacao">
                        </asp:BoundField>
                        <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                        <asp:BoundField DataField="DtUltCompra" DataFormatString="{0:d}" HeaderText="Ult. Compra"
                            SortExpression="DtUltCompra" />
                        <asp:BoundField DataField="DataNascString" DataFormatString="{0:d}" HeaderText="Data Nasc."
                            SortExpression="DataNasc" />
                        <asp:BoundField DataField="TotalComprado" DataFormatString="{0:C}" HeaderText="Total Comprado"
                            SortExpression="TotalComprado" />
                        <asp:BoundField DataField="Limite" DataFormatString="{0:C}" HeaderText="Limite"
                            SortExpression="Limite" />
                        <asp:BoundField DataField="UsoLimite" DataFormatString="{0:C}" HeaderText="Limite Utilizado"
                            SortExpression="UsoLimite" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsClientes" runat="server" 
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetRptCount" SelectMethod="GetForListaRpt" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ClienteDAO" 
                    CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
                    SkinID="" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtRota" Name="codRota" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpTipoPessoa" Name="tipoPessoa" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkRevenda" Name="revenda" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="chkCompra" Name="compra" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkApenasSemRota" Name="apenasSemRota" 
                            PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="chkApenasSemPrecoTabela" 
                            Name="apenasSemPrecoTabela" PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="ctrlDataNiverIni" Name="dataNiverIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataNiverFim" Name="dataNiverFim" PropertyName="DataString"
                            Type="String" />
                        <asp:Parameter Name="dataCadIni" Type="String" />
                        <asp:Parameter Name="dataCadFim" Type="String" />
                        <asp:Parameter Name="dataSemCompraIni" Type="String" />
                        <asp:Parameter Name="dataSemCompraFim" Type="String" />
                        <asp:Parameter Name="dataInativadoIni" Type="String" />
                        <asp:Parameter Name="dataInativadoFim" Type="String" />
                        <asp:ControlParameter ControlID="ddlLimite" Name="limite" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSituacaoCliente" runat="server" SelectMethod="GetSituacaoCliente"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:Label ID="Label4" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>&nbsp;
                <asp:Label ID="Label5" runat="server" Text="A informação de limite utilizado é baseado no dia anterior de trabalho." Font-Bold="True" ></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;">
                    <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
