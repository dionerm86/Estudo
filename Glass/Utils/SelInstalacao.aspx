<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelInstalacao.aspx.cs" Inherits="Glass.UI.Web.Utils.SelInstalacao"
    Title="Selecionar Instalações" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function getCli(idCli)
        {
            if (idCli.value == "")
            {
                //openWindow(500, 800, 'SelCliente.aspx');
                return false;
            }

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCliente", "input").value = "";
                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
        }

        function validarInstalacao(idInstalacao, idPedido, nomeCliente, descrTipoInstalacao, nomeLoja, localObra, dataConfPedido)
        {
            var response = SelInstalacao.VerificarPedidoJaFinalizadoPCP(idPedido).value;

            if (response == null) {
                alert("Falha ao buscar Instalação. AJAX Error.");
                return false;
            }

            response = response.split('\t');

            if (response[0] == "Erro") {
                alert(response[1]);
                return false;
            }

           return window.opener.setInstalacao(idInstalacao, idPedido, nomeCliente, descrTipoInstalacao, nomeLoja, localObra, dataConfPedido)
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label11" runat="server" ForeColor="#0066FF" Text="Num. Pedido"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Período Entrega"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido0" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataIni', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataFim" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataFim', this)" ToolTip="Alterar" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" ForeColor="#0066FF" Text="Cliente"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" onblur="getCli(this);" onkeypress="return soNumeros(event, true, true);"
                                Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="return getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnFechar" runat="server" Text="Fechar" OnClientClick="closeWindow();" />
                <br />
                <asp:GridView GridLines="None" ID="grdInstalacao" runat="server" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsInstalacao"
                    EmptyDataText="Nenhuma instalação encontrada.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="validarInstalacao(<%# Eval("IdInstalacao") %>, <%# Eval("IdPedido") %>, '<%# Eval("NomeCliente").ToString().Replace("'", "") %>', '<%# Eval("DescrTipoInstalacao") %>', '<%# Eval("NomeLoja") %>', '<%# Eval("LocalObra").ToString().Replace("'", "") %>', '<%# Eval("DataConfPedido", "{0:d}") %>');">
                                    <img alt="Selecionar" border="0" src="../Images/insert.gif" title="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="LocalObra" HeaderText="Local Obra" SortExpression="LocalObra" />
                        <asp:BoundField DataField="DescrTipoInstalacao" HeaderText="Tipo Instalação" SortExpression="DescrTipoInstalacao" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="Situacao"
                            Visible="False" />
                        <asp:BoundField DataField="DataEntrega" HeaderText="Data Entrega" SortExpression="DataEntrega"
                            DataFormatString="{0:d}" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInstalacao" runat="server" SelectMethod="GetListAbertas"
                    TypeName="Glass.Data.DAL.InstalacaoDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountAbertas" SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeCliente" Name="nomeCli" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataIni" Name="dataIniConf" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFimConf" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
