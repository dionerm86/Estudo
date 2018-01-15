<%@ Page Title="Controle de Pagamentos Antecipados de Fornecedor" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstAntecipFornec.aspx.cs" Inherits="Glass.UI.Web.Listas.LstAntecipFornec" %>

<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = LstAntecipFornec.GetFornec(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNomeFornec", "input").value = "";
                return false;
            }

            FindControl("txtNomeFornec", "input").value = retorno[1];
        }

        function openRpt(idAntecip) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=AntecipFornec&idAntecipFornec=" + idAntecip);
            return false;
        }

        function openRptListaAntecipFornec(exportarExcel) {
            var idFornec = FindControl("txtNumFornec", "input").value;
            var nomeFornec = FindControl("txtNomeFornec", "input").value;
            var idFunc = FindControl("drpFuncionario", "select").value;
            var idFormaPagto = FindControl("drpFormaPagto", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaAntecipFornec&idFornec=" + idFornec + "&nomeFornec=" + nomeFornec +
            "&situacao=" + situacao + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&idFunc=" + idFunc + "&idFormaPagto=" + idFormaPagto +
            "&exportarExcel=" + exportarExcel);

            return false;
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" onblur="getFornec(this);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeFornec" runat="server" Width="200px" Style="margin-bottom: 0px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Funcionário"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" AppendDataBoundItems="True"
                                AutoPostBack="True" DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Aberta</asp:ListItem>
                                <asp:ListItem Value="5">Aguardando Financeiro</asp:ListItem>
                                <asp:ListItem Value="4">Confirmada</asp:ListItem>
                                <asp:ListItem Value="3">Finalizada</asp:ListItem>
                                <asp:ListItem Value="2">Cancelada</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Data de cadastro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Forma Pagto."></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpFormaPagto" runat="server" AppendDataBoundItems="True" DataSourceID="odsFormaPagto"
                                DataTextField="Descricao" DataValueField="IdFormaPagto">
                            </asp:DropDownList>
                        </td>
                        <td nowrap="nowrap">
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
                <asp:LinkButton ID="lbkInserir" runat="server" PostBackUrl="~/Cadastros/CadAntecipFornec.aspx">Inserir pagamento antecipado</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdAntecipFornec" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsAntecipacaoFornecedor"
                    DataKeyNames="IdAntecipFornec" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Não há antecipações cadastradas."
                    OnRowCommand="grdAntecipFornec_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" Visible='<%# Eval("EditVisible") %>'
                                    ImageUrl="~/Images/EditarGrid.gif" NavigateUrl='<%# "~/Cadastros/CadAntecipFornec" + ".aspx?idAntecipFornec=" + Eval("IdAntecipFornec") %>'></asp:HyperLink>
                                <asp:ImageButton ID="imbExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick='<%# "openWindow(200, 500, \"../Utils/SetMotivoCancPagto.aspx?idAntecipFornec=" + Eval("IdAntecipFornec") + "\"); return false" %>'
                                    Visible='<%# Eval("CancelVisible") %>' />
                                <a href="#" onclick='openRpt(&#039;<%# Eval("IdAntecipFornec") %>&#039;);'>
                                    <img border="0" src="../Images/Relatorio.gif" /></a>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="idAntecipFornec" HeaderText="Num. pag." SortExpression="idAntecipFornec" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="IdNomeFornec" HeaderText="Fornecedor" SortExpression="IdNomeFornec" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situacao" SortExpression="DescrSituacao" />
                        <asp:BoundField DataField="ValorAntecip" DataFormatString="{0:c}" HeaderText="Valor Antecipação"
                            SortExpression="ValorAntecip" />
                        <asp:BoundField DataField="Saldo" DataFormatString="{0:C}" HeaderText="Saldo" SortExpression="Saldo">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data de cadastro"
                            SortExpression="DataCad" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkFinalizar" runat="server" CommandArgument='<%# Eval("IdAntecipFornec") %>'
                                    CommandName="Finalizar" OnClientClick="return confirm(&quot;Tem certeza que deseja finalizar esta antecipação?&quot;);"
                                    Visible='<%# Eval("FinalizarVisible") %>'>Finalizar</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("IdAntecipFornec") %>'
                                    Tabela="AntecipFornec" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAntecipacaoFornecedor" runat="server" DataObjectTypeName="Glass.Data.Model.AntecipacaoFornecedor"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.AntecipacaoFornecedorDAO" SelectCountMethod="GetListCount"
                    OnDeleted="odsAntecipacaoFornecedor_Deleted" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumFornec" Name="idFornec" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeFornec" Name="nomeFornec" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFormaPagto" Name="idFormaPagto" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString"
                            Type="DateTime" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString"
                            Type="DateTime" />
                        <asp:Parameter Name="idsNotasIgnorar" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetFinanceiros"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForConsultaContasReceber"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfGerarCredito" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimirPagtoAnt" runat="server" OnClientClick="openRptListaAntecipFornec(); return false;"> <img 
                    border="0" src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;
                <asp:LinkButton ID="lnkExportarExcelPagtoAnt" runat="server" OnClientClick="openRptListaAntecipFornec(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
