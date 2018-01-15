<%@ Page Title="Lista de Crédito Fornecedor" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstCreditoFornecedor.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCreditoFornecedor" %>

<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRptItem(idCreditoFornecedor) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=CredFornec&idCreditoFornecedor=" + idCreditoFornecedor);
            return false;
        }

        function openRpt(exportExcel) {

            var idFornecedor = FindControl("txtNumFornecedor", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaCredFornec&idFornecedor=" + idFornecedor + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&exportarExcel=" + exportExcel);
            return false;
        }
        
        function getFornec(id) {
            if (id.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(id.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                id.value = "";
                FindControl("txtNomeFornecedor", "input").value = "";
                return false;
            }

            FindControl("txtNomeFornecedor", "input").value = retorno[1];
        }

        function selFornec() {

            if (FindControl("txtNomeFornecedor", "input").value == "") {
                openWindow(590, 760, '../Utils/SelFornec.aspx'); return false;
            }
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
                            <asp:TextBox ID="txtNumFornecedor" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);" Text='<%# Bind("IdFornecedor") %>'></asp:TextBox>
                            <asp:TextBox ID="txtNomeFornecedor" runat="server" Width="320px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="selFornec();" onclick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Data de cadastro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
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
                <asp:LinkButton ID="lbkInserir" runat="server" PostBackUrl="~/Cadastros/CadCreditoFornecedor.aspx">Gerar Crédito</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCredFornec" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsCreditoFornecedor"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Registros não encontrados.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
<%--                                <asp:ImageButton ID="imbEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                                    Visible='<%# Eval("EditVisible") %>' PostBackUrl='<%# "~/Cadastros/CadCreditoFornecedor.aspx?id=" + Eval("IdCreditoFornecedor") %>' />
--%>                                <asp:ImageButton ID="imbExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                    Visible='<%# Eval("CancelVisible") %>' OnClientClick='<%# "openWindow(200, 500, \"../Utils/SetMotivoCancReceb.aspx?tipo=credFornec&id=" + Eval("IdCreditoFornecedor") + "\"); return false" %>' />
                                <a href="#" onclick='openRptItem(&#039;<%# Eval("IdCreditoFornecedor") %>&#039;);'>
                                    <img border="0" src="../Images/Relatorio.gif" /></a>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="NomeFornecedor" HeaderText="Fornecedor" SortExpression="NomeFornecedor" />
                        <asp:BoundField DataField="NomeUsuarioCad" HeaderText="Usuário Cad." SortExpression="NomeUsuarioCad" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data Cad." SortExpression="DataCad"
                            DataFormatString="{0:d}" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("IdCreditoFornecedor") %>'
                                    Tabela="CreditoFornecedor" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCreditoFornecedor" runat="server" DataObjectTypeName="Glass.Data.Model.CreditoFornecedor"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.CreditoFornecedorDAO" SelectCountMethod="GetListCount">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumFornecedor" Name="idFornecedor" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString" Type="DateTime" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString" Type="DateTime" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false;"> 
                   <img border="0" src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" Visible="false" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
