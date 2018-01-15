<%@ Page Title="Resumo do dia - Recebimentos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ResumoDiario.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ResumoDiario" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        
        function openRpt() {
            openWindow(600, 800, "RelBase.aspx?rel=ResumoDiario&" + obterQueryString());
        }

        function openRptCreditoGerado() {
            openWindow(600, 800, "RelBase.aspx?rel=ResumoDiarioCreditoGerado&" + obterQueryString());
        }

        function obterQueryString() {
            var data = FindControl("ctrlData_txtData", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;

            return "data=" + data + "&idLoja=" + idLoja;
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Data de consulta"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlData" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label20" runat="server" ForeColor="#0066FF" Text="Loja"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="True"/>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
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
                <asp:DetailsView ID="dtvResumoDiario" runat="server" AutoGenerateRows="False" CellPadding="4"
                    DataSourceID="odsResumoDiario" GridLines="None">
                    <Fields>
                        <asp:TemplateField HeaderText="Forma de pagamento">
                            <ItemTemplate>
                                Valor
                            </ItemTemplate>
                            <HeaderStyle Font-Bold="True" HorizontalAlign="Center" />
                            <ItemStyle Font-Bold="True" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="ChequeVista" DataFormatString="{0:c}" HeaderText="Cheque à vista"
                            SortExpression="ChequeVista">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ChequePrazo" DataFormatString="{0:c}" HeaderText="Cheque pré-datado"
                            SortExpression="ChequePrazo">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Dinheiro" DataFormatString="{0:c}" HeaderText="Dinheiro"
                            SortExpression="Dinheiro">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CartaoDebito" DataFormatString="{0:c}" HeaderText="Cartão de débito"
                            SortExpression="CartaoDebito">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CartaoCredito" DataFormatString="{0:c}" HeaderText="Cartão de crédito"
                            SortExpression="CartaoCredito">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Credito" DataFormatString="{0:c}" HeaderText="Crédito recebido"
                            SortExpression="Credito">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Troca" DataFormatString="{0:c}" HeaderText="Vale troca"
                            SortExpression="Troca">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Boleto" DataFormatString="{0:c}" HeaderText="Boleto" SortExpression="Boleto">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Deposito" DataFormatString="{0:c}" HeaderText="Depósito"
                            SortExpression="Deposito">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Total" DataFormatString="{0:c}" HeaderText="Total" ReadOnly="True"
                            SortExpression="Total">
                            <HeaderStyle Font-Bold="True" />
                            <ItemStyle Font-Bold="True" HorizontalAlign="Right" />
                        </asp:BoundField>                        
                        <asp:TemplateField HeaderText="Crédito gerado *" SortExpression="CreditoGerado">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkCreditoGerado" runat="server" OnClientClick="openRptCreditoGerado(); return false;" Text='<%# Eval("CreditoGerado", "{0:c}") %>'></asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Construcard" DataFormatString="{0:c}" HeaderText="Construcard *"
                            SortExpression="Construcard">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="PagtoChequeDevolvido" DataFormatString="{0:c}" HeaderText="Pagto. cheque devolvido *"
                            SortExpression="PagtoChequeDevolvido">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="PagtoChequeDevolvidoDinheiro" DataFormatString="{0:c}"
                            HeaderText="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Pagto. cheque devolvido (Dinheiro) *"
                            SortExpression="PagtoChequeDevolvidoDinheiro">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="PagtoChequeDevolvidoCheque" DataFormatString="{0:c}"
                            HeaderText="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Pagto. cheque devolvido (Cheque) *"
                            SortExpression="PagtoChequeDevolvidoCheque">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="PagtoChequeDevolvidoOutros" DataFormatString="{0:c}"
                            HeaderText="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Pagto. cheque devolvido (Outros) *"
                            SortExpression="PagtoChequeDevolvidoOutros">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NotaPromissoria" DataFormatString="{0:c}" HeaderText="Nota promissória *"
                            SortExpression="NotaPromissoria">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                        <asp:BoundField DataField="PagtoNotaPromissoria" DataFormatString="{0:c}" HeaderText="Pagto. nota promissória *"
                            SortExpression="PagtoNotaPromissoria">
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:BoundField>
                    </Fields>
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsResumoDiario" runat="server" SelectMethod="GetResumoDiario"
                    TypeName="Glass.Data.RelDAL.ResumoDiarioDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlData" Name="data" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <br />
                * Apenas para conferência. Não incluso no total.
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lkbImprimir" runat="server" OnClientClick="openRpt(); return false;"><img border="0" 
                    src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
