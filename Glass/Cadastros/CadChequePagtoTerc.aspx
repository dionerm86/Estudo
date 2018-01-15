<%@ Page Title="Cheques de Terceiros" Language="C#" AutoEventWireup="true" CodeBehind="CadChequePagtoTerc.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadChequePagtoTerc" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>

<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <script type="text/javascript">
        function setCheque(idCheque, numCheque, titular, valor, dataVenc, banco, agencia, conta, situacao, origem, idAcertoCheque, idContaR,
            idPedido, idSinal, idAcerto, idLiberarPedido, idTrocaDevolucao, cpfCnpj)
        {
            var nomeTabelaChequesOpener = <%= !String.IsNullOrEmpty(Request["tabelaCheque"]) ? Request["tabelaCheque"] : "'tbChequePagto'" %>;
            var callbackIncluir = <%= Request["callbackIncluir"] %>;
            var callbackExcluir = <%= Request["callbackExcluir"] %>;
            var nomeControleFormaPagto = <%= Request["nomeControleFormaPagto"] != null ? Request["nomeControleFormaPagto"] : "''" %>;
            
            window.opener.setCheque(nomeTabelaChequesOpener, idCheque, null, numCheque, null, titular, valor, dataVenc, banco, agencia, conta, situacao, '', window, "terceiro",
                origem, idAcertoCheque, idContaR, idPedido, idSinal, idAcerto, idLiberarPedido, idTrocaDevolucao, cpfCnpj, '', '', <%= Request["controlPagto"] %>, "",
                callbackIncluir, callbackExcluir, nomeControleFormaPagto);
            
            window.opener.loadDropCheque();
        }
    </script>

    <table style="width: 100%;">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" ForeColor="#0066FF" Text="Num. Acerto"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumAcerto" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label14" runat="server" ForeColor="#0066FF" Text="Num. Cheque"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCheque" runat="server" Width="100px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label16" runat="server" ForeColor="#0066FF" Text="Titular"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTitular" runat="server" MaxLength="40" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" ForeColor="#0066FF" Text="Agência"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAgencia" runat="server" MaxLength="25" Width="70px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" ForeColor="#0066FF" Text="Conta"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtConta" runat="server" MaxLength="20" Width="90px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Período Venc."></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataIni" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataIni', this)" ToolTip="Alterar" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtDataFim" runat="server" onkeypress="return false;" Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataFim" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataFim', this)" ToolTip="Alterar" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
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
                <asp:Button ID="Button1" runat="server" OnClientClick="closeWindow();" Text="Fechar" />
                <br />
                <asp:GridView GridLines="None" ID="grdCheque" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdCheque" DataSourceID="odsCheques"
                    EmptyDataText="Nenhum cheque cadastrado." CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick='setCheque(<%# Eval("IdCheque") %>, <%# Eval("Num") %>, "<%# Eval("Titular") %>", "<%# Eval("Valor", "{0:C}") %>", "<%# Eval("DataVenc", "{0:d}") %>", "<%# Eval("Banco") %>", "<%# Eval("Agencia") %>", "<%# Eval("Conta") %>", "<%# Eval("Situacao") %>", "<%# Eval("Origem") %>", "<%# Eval("IdAcertoCheque") %>", "<%# Eval("IdContaR") %>", "<%# Eval("IdPedido") %>", "<%# Eval("IdSinal") %>", "<%# Eval("IdAcerto") %>", "<%# Eval("IdLiberarPedido") %>", "<%# Eval("IdTrocaDevolucao") %>", "<%# Eval("CpfCnpj") %>");'>
                                    <img src="../Images/Insert.gif" border="0" title="Incluir cheque no pagamento" /></a>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num." SortExpression="Num">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Num") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Banco" SortExpression="Banco">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Banco") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Agência" SortExpression="Agencia">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Agencia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Conta" SortExpression="Conta">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Conta") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Titular" SortExpression="Titular">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Titular") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Valor", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Venc." SortExpression="DataVenc">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DataVenc", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCheques" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCountTerceiros" SelectMethod="GetTerceiros" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ChequesDAO" DataObjectTypeName="Glass.Data.Model.Cheques"
        DeleteMethod="Delete" UpdateMethod="Update">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtNumAcerto" Name="idAcerto" PropertyName="Text"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtNumCheque" Name="numCheque" PropertyName="Text"
                Type="Int32" />
            <asp:ControlParameter ControlID="txtTitular" Name="titular" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtAgencia" Name="agencia" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtConta" Name="conta" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtDataIni" Name="dataIni" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="Text" Type="String" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
