<%@ Page Title="Finalizar/Continuar Instalação" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadFinalizarInstalacao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadFinalizarInstalacao" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script>
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

    function continuar(idInstalacao) {
        if (!confirm('Tem certeza que deseja continuar esta Instalação?'))
            return false;

        var obs = "";

        while (obs == "")
            obs = prompt("Observação:");

        var retorno = CadFinalizarInstalacao.Continuar(idInstalacao, obs).value.split('|');

        if (retorno[0] == "Erro") {
            alert("Falha ao continuar instalação.");
            return false;
        }

        alert("Instalação continuada.");

        // Atualiza página
        cOnClick('imgPesq', null);
    }

    function cancelar(idInstalacao) {
        if (!confirm('Tem certeza que deseja cancelar esta Instalação?'))
            return false;

        var obs = "";

        while (obs == "")
            obs = prompt("Observação:");

        var retorno = CadFinalizarInstalacao.Cancelar(idInstalacao, obs).value.split('|');

        if (retorno[0] == "Erro") {
            alert("Falha ao cancelar instalação.");
            return false;
        }

        alert("Instalação cancelada.");

        // Atualiza página
        cOnClick('imgPesq', null);
    }
    
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Ordem Inst." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtOrdemInst" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" Width="140px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Equipe" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsEquipe" DataTextField="NomeEstendido" DataValueField="IdEquipe">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkConfirmadas" runat="server" Text="Buscar confirmadas no PDA" />
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
                <asp:GridView GridLines="None" ID="grdColocacoes" runat="server" AllowPaging="True"
                    AutoGenerateColumns="False" DataSourceID="odsInstalacao" DataKeyNames="IdInstalacao"
                    OnRowCommand="grdColocacoes_RowCommand" EmptyDataText="Nenhuma instalação encontrada."
                    AllowSorting="True" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="IdOrdemInstalacao" HeaderText="Ordem Inst." SortExpression="IdOrdemInstalacao" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomesEquipes" HeaderText="Equipe" SortExpression="NomesEquipes" />
                        <asp:BoundField DataField="DataInstalacao" DataFormatString="{0:d}" HeaderText="Data Inst."
                            SortExpression="DataInstalacao" />
                        <asp:BoundField DataField="DataFinal" DataFormatString="{0:d}" HeaderText="Data Final."
                            SortExpression="DataFinal" />
                        <asp:BoundField DataField="DescrTipoInstalacao" HeaderText="Tipo" ReadOnly="True"
                            SortExpression="DescrTipoInstalacao" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" ReadOnly="True" SortExpression="DescrSituacao" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# Bind("CancelarVisible") %>'>
                                    <a href="#" onclick='return cancelar(<%# Eval("IdInstalacao") %>);'>Cancelar</a>
                                </asp:PlaceHolder>
                                &nbsp;&nbsp;
                                <asp:LinkButton ID="lnkFinalizar" CommandArgument='<%# Eval("IdInstalacao") %>' OnClientClick="return confirm('Tem certeza que deseja finalizar esta Instalação?');"
                                    runat="server" CommandName="Finalizar" Visible='<%# Eval("FinalizarVisible") %>'>Finalizar</asp:LinkButton>
                                <asp:PlaceHolder ID="pchContinuar1" runat="server" Visible='<%# Bind("ContinuarVisible") %>'>
                                    &nbsp;&nbsp; <a href="#" onclick='return continuar(<%# Eval("IdInstalacao") %>);'>Continuar</a>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="pchContinuar" runat="server" Visible='<%# Bind("ContinuarConfirmadaVisible") %>'>
                                    <a href="#" onclick='openWindow(500, 700, "../Utils/SetProdInst.aspx?idPedido=<%# Eval("IdPedido") %>&idInstalacao=<%# Eval("IdInstalacao") %>");'>
                                        <%# Eval("ContinuarConfirmadaText") %></a> </asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInstalacao" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountEmAndamento" SelectMethod="GetListEmAndamento" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.InstalacaoDAO" SortParameterName="sortExpression">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtOrdemInst" Name="idOrdemInstalacao" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpEquipe" Name="idEquipe" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="chkConfirmadas" Name="confirmadasPda" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEquipe" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.EquipeDAO"
                    MaximumRowsParameterName="" StartRowIndexParameterName="">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
