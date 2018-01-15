<%@ Page Title="Encontro de Contas a Pagar/Receber" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstEncontroContas.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEncontroContas" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCliente", "input").value = "";
                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
        }

        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNomeFornecedor", "input").value = "";
                return false;
            }

            FindControl("txtNomeFornecedor", "input").value = retorno[1];
        }

        function cancelar(idEncontroContas) {
            openWindow(150, 450, "../Utils/SetMotivoCancEncontroContas.aspx?idEncontroContas=" + idEncontroContas);
        }

        function openRptInd(idEncontroContas) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=EncontroContas&IdEncontroContas=" + idEncontroContas);
        }

        function openRpt(exportarExcel) {

            var idEncontroContas = FindControl("txtIdEncontroContas", "input").value;
            var idCliente = FindControl("txtIdCliente", "input").value;
            var nomeCliente = FindControl("txtNomeCliente", "input").value;
            var idFornecedor = FindControl("txtIdFornecedor", "input").value;
            var nomeFornecedor = FindControl("txtNomeFornecedor", "input").value;
            var obs = FindControl("txtObs", "input").value;
            var dataCadIni = FindControl("ctrlDataCadIni_txtData", "input").value;
            var dataCadFim = FindControl("ctrlDataCadFim_txtData", "input").value;

            var queryString = "&idEncontroContas=" + idEncontroContas +
            "&idCliente=" + idCliente +
            "&nomeCliente=" + nomeCliente +
            "&idFornecedor=" + idFornecedor +
            "&nomeFornecedor=" + nomeFornecedor +
            "&obs=" + obs +
            "&dataCadIni=" + dataCadIni +
            "&dataCadFim=" + dataCadFim +
            "&exportarexcel=" + exportarExcel;


            openWindow(600, 800, '../Relatorios/RelBase.aspx?rel=ListaEncontroContas' + queryString);
            return false;
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Cód" ForeColor="#0066FF" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdEncontroContas" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdCliente" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>&nbsp;<asp:TextBox ID="txtNomeCliente" runat="server"
                                    Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdFornecedor" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>&nbsp;<asp:TextBox ID="txtNomeFornecedor"
                                    runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label27" runat="server" ForeColor="#0066FF" Text="Período Cad."></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
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
                            <asp:Label ID="Label29" runat="server" Text="Obs.:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtObs" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
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
                <asp:HyperLink ID="lnkInserir" runat="server" NavigateUrl="~/Cadastros/CadEncontroContas.aspx"> Inserir encontro contas a pagar/receber</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:GridView GridLines="None" ID="grdEncontroContas" runat="server" AllowPaging="True"
                                AllowSorting="True" AutoGenerateColumns="False" EmptyDataText="Nenhum encontro de contas a pagar/receber encontrado."
                                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" DataSourceID="odsEncontroContas">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imbRel" runat="server" ImageUrl="~/Images/Relatorio.gif" OnClientClick='<%# "openRptInd(" + Eval("IdEncontroContas") + "); return false" %>'
                                                Visible='<%# Eval("RelIndVisible") %>' />
                                            <asp:HyperLink ID="lnkEditar" runat="server" Visible='<%# Eval("EditarVisible") %>'
                                                ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadEncontroContas.aspx?idEncontroContas=" + Eval("IdEncontroContas") %>'><img border="0" src="../Images/EditarGrid.gif" alt=""/></asp:HyperLink>
                                            <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Cancelar" ImageUrl="~/Images/ExcluirGrid.gif"
                                                Visible='<%# Eval("ExcluirVisible") %>' OnClientClick='<%# "cancelar(" + Eval("IdEncontroContas") + "); return false;" %>'
                                                ToolTip="Cancelar" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="IdEncontroContas" HeaderText="Cód." SortExpression="IdEncontroContas" />
                                    <asp:BoundField DataField="IdNomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                                    <asp:BoundField DataField="IdNomeFornecedor" HeaderText="Fornec." SortExpression="NomeFornecedor" />
                                    <asp:TemplateField HeaderText="Contas a pagar">
                                        <ItemTemplate>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Eval("ValorPagar","{0:C}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Contas a receber">
                                        <ItemTemplate>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Eval("ValorReceber","{0:C}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Saldo">
                                        <ItemTemplate>
                                            <asp:Label ID="Label5" runat="server" Text='<%# Eval("Saldo","{0:C}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="DataCad" HeaderText="Data de Cad." SortExpression="DataCad" />
                                    <asp:BoundField DataField="SituacaoStr" HeaderText="Situação" ReadOnly="True" SortExpression="SituacaoStr" />
                                    <asp:BoundField DataField="Obs" HeaderText="Obs" SortExpression="Obs" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <uc3:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("IdEncontroContas") %>'
                                                Tabela="EncontroContas" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="pgr"></PagerStyle>
                                <EditRowStyle CssClass="edit"></EditRowStyle>
                                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEncontroContas" runat="server" EnablePaging="True"
                            MaximumRowsParameterName="pageSize" SelectCountMethod="GetListCount" SelectMethod="GetList"
                            SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.EncontroContasDAO"
                            >
                            <SelectParameters>
                                <asp:ControlParameter ControlID="txtIdEncontroContas" Name="idEncontroContas" PropertyName="Text"
                                    Type="UInt32" />
                                <asp:ControlParameter ControlID="txtIdCliente" Name="idCliente" PropertyName="Text"
                                    Type="UInt32" />
                                <asp:ControlParameter ControlID="txtNomeCliente" Name="nomeCliente" PropertyName="Text"
                                    Type="String" />
                                <asp:ControlParameter ControlID="txtIdFornecedor" Name="idFornecedor" PropertyName="Text"
                                    Type="UInt32" />
                                <asp:ControlParameter ControlID="txtNomeFornecedor" Name="nomeFornecedor" PropertyName="Text"
                                    Type="String" />
                                <asp:ControlParameter ControlID="txtObs" Name="obs" PropertyName="Text" Type="String" />
                                <asp:Parameter Name="situacao" Type="Int32" />
                                <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadIni" PropertyName="DataString"
                                    Type="String" />
                                <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadFim" PropertyName="DataString"
                                    Type="String" />
                                <asp:Parameter Name="usuCad" Type="UInt32" />
                                <asp:Parameter Name="dataFinIni" Type="DateTime" />
                                <asp:Parameter Name="dataFinFim" Type="DateTime" />
                                <asp:Parameter Name="usuFin" Type="UInt32" />
                            </SelectParameters>
                        </colo:VirtualObjectDataSource>
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
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"
                    CausesValidation="False"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
