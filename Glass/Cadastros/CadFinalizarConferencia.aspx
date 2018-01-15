<%@ Page Title="Finalizar Conferências" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadFinalizarConferencia.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadFinalizarConferencia" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

    // Busca conferente pelo código do mesmo informado na textbox, ou se a mesma estiver em branco,
    // abre tela para selecioná-lo
    function getConferente(idConferente) {
        if (idConferente.value == "") {
            openWindow(500, 700, "../Utils/SelConferente.aspx");
            return false;
        }

        if (FindControl("txtNomeConferente", "input").value != "")
            return true;

        var retorno = MetodosAjax.GetConferente(idConferente.value).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idConferente.value = "";
            FindControl("txtNomeConferente", "input").value = "";
            return false;
        }

        FindControl("txtNomeConferente", "input").value = retorno[1];
    }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" ForeColor="#0066FF" Text="Num. Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" onkeydown="submitOnEnter(event);" onkeypress="return soNumeros(event, true, true);"
                                runat="server" Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" ForeColor="#0066FF" Text="Loja"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True" AutoPostBack="True" OnSelectedIndexChanged="drpLoja_SelectedIndexChanged">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="150px" onkeydown="submitOnEnter(event);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Data Conferência"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataConferencia" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgDataEfetuar" runat="server" ImageAlign="AbsMiddle" ToolTip="Alterar"
                                Width="16px" ImageUrl="~/Images/calendario.gif" OnClientClick="return SelecionaData('txtDataConferencia', this)" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" ForeColor="#0066FF" Text="Conferente"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumConferente" runat="server" Width="61px" onkeydown="if (isEnter(event)) getConferente(this);"
                                onblur="getConferente(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeConferente" runat="server" Width="217px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqMedidor" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="return getConferente(FindControl('txtNumConferente', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
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
                <asp:GridView GridLines="None" ID="grdPedidosConferencia" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsPedidosConferencia" EmptyDataText="Nenhum pedido em conferência encontrado."
                    OnRowCommand="grdPedidosConferencia_RowCommand" DataKeyNames="IdPedido" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="Vendedor" HeaderText="Vendedor" SortExpression="Vendedor" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="TelCli" HeaderText="Tel. Cliente" SortExpression="TelCli" />
                        <asp:BoundField DataField="LocalObra" HeaderText="Local Obra" SortExpression="LocalObra" />
                        <asp:BoundField DataField="DataEntrega" DataFormatString="{0:d}" HeaderText="Data Entrega"
                            SortExpression="DataEntrega" />
                        <asp:BoundField DataField="DataIni" DataFormatString="{0:d}" HeaderText="Início Confer."
                            SortExpression="DataIni" />
                        <asp:BoundField DataField="Conferente" HeaderText="Conferente" SortExpression="Conferente" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkFinalizar" runat="server" OnClientClick="return confirm('Tem certeza que deseja finalizar esta Conferência?');"
                                    CommandArgument='<%# Eval("IdPedido") %>' CommandName="Finalizar">Finalizar</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedidosConferencia" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SelectMethod="GetList"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.PedidoConferenciaDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumConferente" Name="idConferente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeCliente" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:Parameter DefaultValue="2" Name="situacao" Type="Int32" />
                        <asp:Parameter Name="sitPedido" Type="Int32" />
                        <asp:ControlParameter ControlID="txtDataConferencia" Name="dataConferencia" PropertyName="Text"
                            Type="String" />
                        <asp:Parameter Name="dataFinalIni" Type="String" />
                        <asp:Parameter Name="dataFinalFim" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
