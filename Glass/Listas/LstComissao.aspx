<%@ Page Title="Comissões" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstComissao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstComissao" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRptIndividual(idComissao) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ComissaoDetalhada&idComissao=" + idComissao);
            return false;
        }

        function openRptGeral() {
            var tipoFunc = FindControl("drpTipo", "select").value;
            var idFunc = FindControl("drpNome", "select").value;
            var idPedido = FindControl("txtPedido", "input").value;
            idPedido = idPedido != "" ? idPedido : "0";
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Comissao&tipoFunc=" + tipoFunc + "&idFunc=" + idFunc +
                "&idPedido=" + idPedido + "&dataIni=" + dataIni + "&dataFim=" + dataFim);

            return false;
        }

        function openRptRecibos()
        {
            var tipoFunc = FindControl("drpTipo", "select").value;
            var idFunc = FindControl("drpNome", "select").value;
            var idPedido = FindControl("txtPedido", "input").value;
            idPedido = idPedido != "" ? idPedido : "0";
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=RecibosComissao&tipoFunc=" + tipoFunc + "&idFunc=" + idFunc +
                "&idPedido=" + idPedido + "&dataIni=" + dataIni + "&dataFim=" + dataFim);

            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Tipo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipo" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpTipo_SelectedIndexChanged">
                                <asp:ListItem Value="0">Funcionário</asp:ListItem>
                                <asp:ListItem Value="1">Comissionado</asp:ListItem>
                                <asp:ListItem Value="2">Instalador</asp:ListItem>
                                <asp:ListItem Value="3">Gerente</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Nome" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpNome" runat="server" DataSourceID="odsFuncionario" DataTextField="Nome"
                                DataValueField="IdFunc" AutoPostBack="True" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="70px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap" align="right">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
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
                <asp:GridView GridLines="None" ID="grdComissao" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdComissao" DataSourceID="odsComissao"
                    EmptyDataText="Nenhuma comissão encontrada." CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="openRptIndividual('<%# Eval("IdComissao") %>');">
                                    <img border="0" src="../Images/Relatorio.gif" title="Pedidos desta Comissão" /></a>
                                <asp:LinkButton ID="lnkExcluir" runat="server" OnClientClick="return confirm('Tem certeza que deseja excluir esta comissão?')"
                                    CommandName="Delete">
                                     <img border="0" src="../Images/ExcluirGrid.gif" title="Excluir comissão" /></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>                        
                        <asp:BoundField DataField="IdComissao" HeaderText="Cod.Comissao" ReadOnly="True" SortExpression="IdComissao" />
                        <asp:BoundField DataField="Nome" HeaderText="Nome" ReadOnly="True" SortExpression="Nome" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Comissão"
                            SortExpression="Total" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRptGeral();">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkRecibos" runat="server" OnClientClick="return openRptRecibos();">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir recibos de comissão</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsComissao" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ComissaoDAO" DataObjectTypeName="Glass.Data.Model.Comissao"
                    DeleteMethod="Delete" OnDeleted="odsComissao_Deleted">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpTipo" Name="tipoFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpNome" Name="idFuncComissionado" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtPedido" Name="idPedido" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetVendedores"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsComissionado" runat="server" SelectMethod="GetAllOrdered" TypeName="Glass.Data.DAL.ComissionadoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInstalador" runat="server" SelectMethod="GetColocadores"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
               <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGerente" runat="server" SelectMethod="GetGerentesForComissao"
                            TypeName="Glass.Data.DAL.FuncionarioDAO">                           
                        </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
