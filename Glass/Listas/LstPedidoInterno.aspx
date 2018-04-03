<%@ Page Title="Pedidos Internos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstPedidoInterno.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPedidoInterno" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRptPedido(idPedido, exportarExcel) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=PedidoInterno&idPedido=" + idPedido +
                "&exportarexcel=" + exportarExcel);
        }

        function openRpt(exportarExcel) {
            var idPedido = FindControl("txtIdPedido", "input").value;
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idFunc = FindControl("drpFunc", "select").value;
            var idFuncAut = FindControl("drpFuncAut", "select").value;
            var agruparImpressao = FindControl("ckbAgruparImpressao", "input").checked;

            if (idPedido == "") idPedido = 0;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaPedidoInterno&idPedido=" + idPedido + "&dataIni=" + dtIni +
                "&dataFim=" + dtFim + "&idFunc=" + idFunc + "&idFuncAut=" + idFuncAut + "&exportarExcel=" + exportarExcel + "&agrupar=" + agruparImpressao);

            return false;
        }

    </script>

    <div class="filtro">
        <div>
            <span>
                <asp:Label ID="Label1" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                <asp:TextBox ID="txtIdPedido" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label2" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <sync:CheckBoxListDropDown ID="cbdSituacao" runat="server" DataSourceID="odsSituacao"
                    DataTextField="Translation" DataValueField="Value" Width="200px">
                </sync:CheckBoxListDropDown>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
        </div>
        <div>
            <span>
                <asp:Label ID="Label3" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                <asp:DropDownList ID="drpFunc" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                    DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc">
                    <asp:ListItem Value="0">Todos</asp:ListItem>
                </asp:DropDownList>
            </span>
        </div>
        <div>
            <span>
                <asp:Label ID="Label4" runat="server" Text="Funcionário Autorizador" ForeColor="#0066FF"></asp:Label>
                <asp:DropDownList ID="drpFuncAut" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                    DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc">
                    <asp:ListItem Value="0">Todos</asp:ListItem>
                </asp:DropDownList>
            </span>
        </div>
    </div>
    <div class="inserir">
        <asp:LinkButton ID="lkbInserir" runat="server" OnClientClick="redirectUrl('../Cadastros/CadPedidoInterno.aspx'); return false">
            Inserir pedido</asp:LinkButton>
        <span style="<%= ExibirAutorizar() ? "": "display: none" %>">&nbsp;&nbsp;
            <asp:HyperLink ID="lnkAutorizar" runat="server" NavigateUrl="~/Cadastros/CadAutorizarPedidoInterno.aspx">Autorizar pedido</asp:HyperLink>
        </span>
    </div>
    <asp:GridView GridLines="None" ID="grdPedidos" runat="server" AutoGenerateColumns="False"
        DataKeyNames="IdPedidoInterno" DataSourceID="odsPedidoInterno" CssClass="gridStyle"
        PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
        EmptyDataText="Não há pedidos internos cadastrados." AllowPaging="True" AllowSorting="True"
        OnRowCommand="grdPedidos_RowCommand">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton ID="imgEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                        OnClientClick='<%# "redirectUrl(\"../Cadastros/CadPedidoInterno.aspx?idPedidoInterno=" + Eval("IdPedidoInterno") + (Request["producao"] == "1" ? "&producao=1&popup=true" : "") + "\""+ "); return false" %>'
                        Visible='<%# Eval("EditVisible") %>' />
                    <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                        OnClientClick="if (!confirm(&quot;Deseja cancelar esse pedido interno?&quot;)) return false;"
                        Visible='<%# Eval("DeleteVisible") %>' />
                    <asp:ImageButton ID="imgRelatorio" runat="server" ImageUrl="~/Images/Relatorio.gif"
                        OnClientClick='<%# "openRptPedido(" + Eval("IdPedidoInterno") + ", false); return false" %>' />
                    <asp:ImageButton ID="imgExcel" runat="server" ImageUrl="~/Images/Excel.gif"
                        OnClientClick='<%# "openRptPedido(" + Eval("IdPedidoInterno") + ",true); return false" %>' />
                    <asp:PlaceHolder ID="pchFotos" runat="server" Visible='true'>
                        <a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdPedidoInterno") %>&tipo=pedidointerno&#039;); return false;'>
                            <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="IdPedidoInterno" HeaderText="Pedido" SortExpression="IdPedidoInterno" />
            <asp:BoundField DataField="NomeFuncCad" HeaderText="Funcionário" SortExpression="NomeFuncCad" />
            <asp:BoundField DataField="DataPedido" DataFormatString="{0:d}" HeaderText="Data"
                SortExpression="DataPedido" />
            <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                <ItemTemplate>
                    <table class="pos">
                        <tr>
                            <td>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrSituacao") %>'></asp:Label>
                            </td>
                            <td>
                                <asp:ImageButton ID="imgReabrir" runat="server" Visible='<%# Eval("ReabrirVisible") %>'
                                    CommandArgument='<%# Eval("IdPedidoInterno") %>' CommandName="Reabrir" ImageUrl="~/Images/cadeado.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja reabrir esse pedido interno?&quot;)) return false" />
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("DescrSituacao") %>'></asp:Label>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="NomeFuncAut" HeaderText="Funcionário Autorizador" SortExpression="NomeFuncAut" />
            <asp:BoundField DataField="DataAut" HeaderText="Data Autorização"
                SortExpression="DataAut" />
            <asp:BoundField DataField="NomeFuncConf" HeaderText="Funcionário Confirmação"
                SortExpression="NomeFuncConf" />
            <asp:BoundField DataField="DataConf" HeaderText="Data Confirmação"
                SortExpression="DataConf" />
        </Columns>
        <PagerStyle CssClass="pgr"></PagerStyle>
        <EditRowStyle CssClass="edit"></EditRowStyle>
        <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
    </asp:GridView>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedidoInterno" runat="server" DeleteMethod="Cancelar"
        EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
        SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
        TypeName="Glass.Data.DAL.PedidoInternoDAO">
        <DeleteParameters>
            <asp:Parameter Name="idPedidoInterno" Type="UInt32" />
        </DeleteParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="txtIdPedido" Name="idPedidoInterno" PropertyName="Text" Type="UInt32" />
            <asp:ControlParameter ControlID="drpFunc" Name="idFunc" PropertyName="SelectedValue" Type="UInt32" />
            <asp:ControlParameter ControlID="drpFuncAut" Name="idFuncAut" PropertyName="SelectedValue" Type="UInt32" />
            <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
            <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
            <asp:ControlParameter ControlID="cbdSituacao" Name="situacao" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.FuncionarioDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacao" runat="server"
        TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.SituacaoPedidoInt, Glass.Data" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <asp:CheckBox ID="ckbAgruparImpressao" runat="server" Text="Agrupar impressão por pedido"
        ToolTip="Agrupa os pedidos e exibe os produtos" />
    <div>
        <br />
        <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);">
            <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
        &nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;">
            <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
    </div>
</asp:Content>
