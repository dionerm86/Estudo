<%@ Page Title="Conferências" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="ListaPedidosConferencia.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaPedidosConferencia" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        // Abre relatório de conferência
        function openRpt() {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var nomeCliente = FindControl("txtNomeCliente", "input").value;
            var sitPedido = FindControl("drpSitPedido", "select").value;
            var dataConf = FindControl("ctrlDataConferencia_txtData", "input").value;
            var dataFinalIni = FindControl("ctrlDataFinalIni_txtData", "input").value;
            var dataFinalFim = FindControl("ctrlDataFinalFim_txtData", "input").value;
            var numConferente = FindControl("txtNumConferente", "input").value;

            if (idPedido == "")
                idPedido = 0;

            if (idLoja == "")
                idLoja = 0;

            if (numConferente == "")
                numConferente = 0;

            var queryString = "?Rel=ListaConferencia&IdPedido=" + idPedido + "&idLoja=" + idLoja + "&NomeCliente=" + nomeCliente +
                "&situacao=" + situacao + "&IdConferente=" + numConferente + "&dataConferencia=" + dataConf + "&dataFinalIni=" + dataFinalIni +
                "&dataFinalFim=" + dataFinalFim + "&sitPedido=" + sitPedido;

            openWindow(600, 800, "../Relatorios/RelBase.aspx" + queryString);
            return false;
        }

        // Busca conferente pelo código do mesmo informado na textbox, ou se a mesma estiver em branco,
        // abre tela para selecioná-lo
        function getConferente(idConferente) {
            if (idConferente.value == "") {
                openWindow(500, 700, "../Utils/SelConferente.aspx");
                return false;
            }

            var retorno = MetodosAjax.GetConferente(idConferente.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idConferente.value = "";
                FindControl("txtNomeConferente", "input").value = "";
                return false;
            }

            FindControl("txtNomeConferente", "input").value = retorno[1];
        }

        // Função utilizada após selecionar conferente no popup, para preencher o id e o nome do mesmo
        // Nas respectivas textboxes deste form
        function setConferente(id, nome) {
            FindControl("txtNumConferente", "input").value = id;
            FindControl("txtNomeConferente", "input").value = nome;
            return false;
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" ForeColor="#0066FF" Text="Num. Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" runat="server" Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Sit. Confer."></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpSituacao_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Aberta</asp:ListItem>
                                <asp:ListItem Value="2">Em Andamento</asp:ListItem>
                                <asp:ListItem Value="3">Finalizada</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" ForeColor="#0066FF" Text="Sit. Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSitPedido" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpSituacao_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="2">Ativo/Em Conferência</asp:ListItem>
                                <asp:ListItem Value="3">Em Conferência</asp:ListItem>
                                <asp:ListItem Value="4">Conferido</asp:ListItem>
                                <asp:ListItem Value="5">Confirmado</asp:ListItem>
                                <asp:ListItem Value="6">Cancelado</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label12" runat="server" ForeColor="#0066FF" Text="Conferente"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumConferente" runat="server" Width="50px" onblur="getConferente(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeConferente" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqMedidor" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="getConferente(FindControl('txtNumConferente', 'input')); return false;"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" ForeColor="#0066FF" Text="Loja"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True" AutoPostBack="True" OnSelectedIndexChanged="drpSituacao_SelectedIndexChanged">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
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
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Data Início Confer."></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataConferencia" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label14" runat="server" ForeColor="#0066FF" Text="Período Finalização"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFinalIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFinalFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
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
                <asp:GridView GridLines="None" ID="grdPedidosConferencia" runat="server" AllowPaging="True"
                    AllowSorting="True" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AutoGenerateColumns="False" DataSourceID="odsPedidosConferencia"
                    EmptyDataText="Nenhum pedido em conferência encontrado.">
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
                        <asp:BoundField DataField="DataFim" DataFormatString="{0:d}" HeaderText="Fim Confer."
                            SortExpression="DataFim" />
                        <asp:BoundField DataField="Conferente" HeaderText="Conferente" SortExpression="Conferente" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Sit. Confer." ReadOnly="True"
                            SortExpression="DescrSituacao" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <a id="lnkImprimir" href="#" onclick="return openRpt();">
                    <img alt="" border="0" src="../Images/printer.png" />
                    Imprimir</a><br />
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
                        <asp:ControlParameter ControlID="drpSituacao" DefaultValue="0" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpSitPedido" DefaultValue="" Name="sitPedido" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataConferencia" Name="dataConferencia" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFinalIni" Name="dataFinalIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFinalFim" Name="dataFinalFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
