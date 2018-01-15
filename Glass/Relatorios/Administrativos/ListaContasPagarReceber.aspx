<%@ Page Title="Contas a Pagar / Receber" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ListaContasPagarReceber.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.ListaContasPagarReceber" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

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

        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNomeFornec", "input").value = "";
                return false;
            }

            FindControl("txtNomeFornec", "input").value = retorno[1];
        }

        function openRpt(exportarExcel) {

            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var idFornec = FindControl("txtNumFornec", "input").value;
            var nomeFornec = FindControl("txtNomeFornec", "input").value;
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var valorIni = FindControl("txtPrecoInicial", "input").value;
            var valorFin = FindControl("txtPrecoFinal", "input").value;


            openWindow(600, 800, "RelBase.aspx?rel=ContasPagarReceber&idCli=" + idCli + "&nomeCli=" + nomeCli + "&idFornec=" + idFornec + "&nomeFornec=" + nomeFornec
                + "&dtIni=" + dtIni + "&dtFim=" + dtFim + "&valorIni=" + valorIni + "&valorFim=" + valorFin + "&exportarExcel=" + exportarExcel);

            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click"
                                CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Fornecedor"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeFornec" runat="server" Width="170px" onkeypress="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" Text="Venc." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Valor Venc." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPrecoInicial" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            até
                        </td>
                        <td>
                            <asp:TextBox ID="txtPrecoFinal" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" CausesValidation="False"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdContaPagRec" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" EmptyDataText="Nenhuma conta a pagar / receber encontrada."
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdContaPagRec" DataSourceID="odsContasPagarReceber">
                    <Columns>
                        <asp:BoundField DataField="IdContaPagRec" HeaderText="Cód" />
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" />
                        <asp:BoundField DataField="IdNomeCliFornec" HeaderText="Cliente / Fornecedor" />
                        <asp:BoundField DataField="DataVencStr" HeaderText="Data de Venc." />
                        <asp:TemplateField HeaderText="Valor a pagar">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("ValorVencPagStr") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor a receber">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("ValorVencRecStr") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="TipoContaStr" HeaderText="Tipo de Conta" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContasPagarReceber" runat="server" EnablePaging="True"
                MaximumRowsParameterName="pageSize" SelectCountMethod="GetContasPagarReceberCount"
                SelectMethod="GetContasPagarReceber" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                TypeName="Glass.Data.RelDAL.ContasPagarReceberDAO" DataObjectTypeName="Glass.Data.RelModel.ContasPagarReceber"
                >
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                    <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="txtNumFornec" Name="idFornec" PropertyName="Text"
                        Type="UInt32" />
                    <asp:ControlParameter ControlID="txtNomeFornec" Name="nomeFornec" PropertyName="Text"
                        Type="String" />
                    <asp:ControlParameter ControlID="ctrlDataIni" Name="dtVecIni" PropertyName="DataString"
                        Type="String" />
                    <asp:ControlParameter ControlID="ctrlDataFim" Name="dtVecFim" PropertyName="DataString"
                        Type="String" />
                    <asp:ControlParameter ControlID="txtPrecoInicial" Name="valorIni" PropertyName="Text"
                        Type="Single" />
                    <asp:ControlParameter ControlID="txtPrecoFinal" Name="valorFim" PropertyName="Text"
                        Type="Single" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"
                    CausesValidation="False"> <img alt="" border="0" src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
