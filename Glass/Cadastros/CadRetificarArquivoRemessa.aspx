<%@ Page Title="Retificar Arquivo Remessa" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true"
    CodeBehind="CadRetificarArquivoRemessa.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadRetificarArquivoRemessa" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>

<asp:Content ID="Pagina" ContentPlaceHolderID="Pagina" runat="server">

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
                            <asp:Label ID="Label2" runat="server" Text="Num. Remessa: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtNumRemessa"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                 OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblContas" runat="server" Text="Selecione as contas que continuarão no arquivo remessa" Font-Bold="true"></asp:Label></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView GridLines="None" ID="grdContasReceber" runat="server" AutoGenerateColumns="False"
                                DataSourceID="odsContasReceber" DataKeyNames="IdPedido" EmptyDataText="Não há contas à receber para o filtro especificado."
                                PageSize="20" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                                AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" OnDataBound="grdContasReceber_DataBound">
                                <PagerSettings PageButtonCount="20" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSel" runat="server" Checked="True" />
                                            <asp:HiddenField ID="hdfIdContaR" runat="server" Value='<%# Eval("IdContaR") %>' />
                                            <asp:HiddenField ID="hdfIdArquivoRemessa" runat="server" Value='<%# Eval("IdArquivoRemessa") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="IdArquivoRemessa" HeaderText="Arq. Remessa" />
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
                            <asp:Label ID="lblNaoRemover" runat="server" Text="*Contas recebidas não serão exibidas para a retificação." ForeColor="red"></asp:Label></td>
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
            <asp:ControlParameter ControlID="txtNumRemessa" Name="idArquivoRemessa" PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>

</asp:Content>
