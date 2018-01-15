<%@ Page Title="Movimentação de Crédito por Fornecedor" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ListaCreditoFornecedor.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaCreditoFornecedor" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function getFornecedor(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = ListaCreditoFornecedor.GetFornecedor(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeFornecedor", "input").value = "";
                return false;
            }

            FindControl("txtNomeFornecedor", "input").value = retorno[1];
        }

        function openRpt() {
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idFornec = FindControl("txtNumFornecedor", "input").value;
            var sort = FindControl("hdfSort", "input").value;
            var totalGerado = FindControl("lblCreditoGerado", "span").innerHTML;
            var totalUtilizado = FindControl("lblCreditoUtilizado", "span").innerHTML;
            var tipoMovimentacao = FindControl("ddoTipoMov", "select").itens();

            if (idFornec == "")
                idFornec = 0;

            openWindow(600, 800, "RelBase.aspx?rel=MovCreditoFornec&idFornec=" + idFornec + "&inicio=" + dtIni + "&fim=" + dtFim + "&sort=" + sort +
                "&gerado=" + totalGerado + "&utilizado=" + totalUtilizado + "&movimentacao=" + tipoMovimentacao);
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumFornecedor" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornecedor(this);"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Campo obrigatório"
                                ControlToValidate="txtNumFornecedor">*</asp:RequiredFieldValidator>
                            <asp:TextBox ID="txtNomeFornecedor" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="lnkSelFornecedor" runat="server" OnClick="imgPesq_Click" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="if (FindControl('txtNumFornecedor', 'input').value == ''){ openWindow(590, 760, '../Utils/SelFornec.aspx'); return false;} else return true;">
                            </asp:ImageButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                                ValidateEmptyText="true" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                                ValidateEmptyText="true" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Movimentações de Crédito" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="ddoTipoMov" runat="server" CheckAll="True">
                                <asp:ListItem Value="1">Gerado</asp:ListItem>
                                <asp:ListItem Value="2">Estorno Utilizado</asp:ListItem>
                                <asp:ListItem Value="3">Utilizado</asp:ListItem>
                                <asp:ListItem Value="4">Estorno Gerado</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
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
                <asp:GridView GridLines="None" ID="grdCredito" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsCredito" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AllowPaging="True" AllowSorting="True" OnRowDataBound="grdCredito_RowDataBound"
                    OnSorting="grdCredito_Sorting" PageSize="15">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfIdConta" runat="server" Value='<%# Eval("IdConta") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                        <asp:BoundField DataField="Valor" HeaderText="Valor" SortExpression="Valor" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="Saldo" DataFormatString="{0:c}" HeaderText="Saldo" SortExpression="Saldo" />
                        <asp:BoundField DataField="Data" HeaderText="Data" SortExpression="Data" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <table>
                    <tr>
                        <td>
                            Total de crédito utilizado:
                        </td>
                        <td>
                            <asp:Label ID="lblCreditoUtilizado" runat="server" Font-Bold="True" Font-Size="120%"></asp:Label>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            Total de crédito gerado:
                        </td>
                        <td>
                            <asp:Label ID="lblCreditoGerado" runat="server" Font-Bold="True" Font-Size="120%"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            Crédito Atual:
                        </td>
                        <td>
                            <asp:Label ID="lblCreditoAtual" runat="server" Font-Bold="True" Font-Size="120%"></asp:Label>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"
                    Visible="False"><img border="0" 
                    src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCredito" runat="server" SelectMethod="GetCreditoFornecedor"
                    TypeName="Glass.Data.RelDAL.CreditoDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCreditoFornecedorCount" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" OnSelected="odsCredito_Selected">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumFornecedor" Name="idFornec" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="inicio" PropertyName="DataString"
                            Type="DateTime" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="fim" PropertyName="DataString"
                            Type="DateTime" />
                        <asp:ControlParameter ControlID="ddoTipoMov" Name="tipoMovimentacao" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfSort" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
