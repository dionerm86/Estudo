<%@ Page Title="Retificar Arquivo Remessa" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true" 
    CodeBehind="CadRetificarArquivoRemessa.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadRetificarArquivoRemessa" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>

<asp:Content ID="Pagina" ContentPlaceHolderID="Pagina" runat="server">

    <script type="text/javascript">

        function onChecked(control, recebida) {
            if (recebida == true)
                control.checked = true;
        }

    </script>

    <table>
       <%-- <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Num. Remessa: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtNumRemessa"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button runat="server" ID="btnBuscarContasReceber" Text="Buscar contas" /></td>
                    </tr>
                </table>
            </td>
        </tr>--%>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblContas" runat="server" Text="Selecione as contas recebidas que continuarão no arquivo remessa" Font-Bold="true"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView GridLines="None" ID="grdContasReceber" runat="server" AutoGenerateColumns="False"
                                DataSourceID="odsContasReceber" DataKeyNames="IdPedido" EmptyDataText="Não há contas à receber para o filtro especificado."
                                PageSize="20" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                                AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" OnDataBound="grdContasReceber_DataBound"
                                OnRowDataBound="grdContasReceber_RowDataBound">
                                <PagerSettings PageButtonCount="20" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSel" runat="server" Checked="True" onclick='<%# "onChecked(this, " + Eval("Recebida").ToString().ToLower() + ");" %>' />
                                            <asp:HiddenField ID="hdfIdContaR" runat="server" Value='<%# Eval("IdContaR") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Referencia" HeaderText="Ref." />
                                    <asp:BoundField DataField="NumParc" HeaderText="Parc." />
                                    <asp:BoundField DataField="NomeCli" HeaderText="Cliente" />
                                    <asp:BoundField DataField="DataCad" HeaderText="Data Cad." />
                                    <asp:BoundField DataField="ValorVec" HeaderText="Valor Venc." />
                                    <asp:BoundField DataField="DataVec" HeaderText="Data Venc." />
                                    <asp:BoundField DataField="DescricaoContaContabil" HeaderText="Tipo." />
                                    <asp:BoundField DataField="DescrFormaPagtoPlanoConta" HeaderText="Forma Pagto." />
                                    <asp:BoundField DataField="NomeLoja" HeaderText="Loja" />
                                    <asp:BoundField DataField="Obs" HeaderText="Obs." />
                                </Columns>
                                <PagerStyle CssClass="pgr"></PagerStyle>
                                <HeaderStyle Wrap="False" />
                                <EditRowStyle CssClass="edit"></EditRowStyle>
                                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblNaoRemover" runat="server" Text="*Contas em vermelho estão recebidas e não podem ser removidas." ForeColor="red"></asp:Label></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <br />
                <asp:Button ID="btnRetificarArquivoRemessa" runat="server" Visible="false"
                    OnClientClick="if (!confirm('Deseja retificar esse arquivo remessa?')) return false"
                    Text="Retificar Arquivo Remessa" OnClick="btnRetificarArquivoRemessa_Click" /></td>
        </tr>
    </table>

    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContasReceber" runat="server" SelectMethod="ObterContasReceberParaRetificarArquivoRemessa"
        TypeName="Glass.Data.DAL.ContasReceberDAO">
        <SelectParameters>
            <asp:QueryStringParameter QueryStringField="id" Name="idArquivoRemessa" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>

</asp:Content>
