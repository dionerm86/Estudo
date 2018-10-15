<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LoginSistema.aspx.cs" Inherits="Glass.UI.Web.Utils.LoginSistema" Title="Utilização do Sistema" %>

<%@ Register src="../Controls/ctrlData.ascx" tagname="ctrlData" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var idFuncionario = FindControl("drpFuncionario", "select").value;
            var tipoAtividade = FindControl("drpTipo", "select").value;
            var periodoIni = FindControl("ctrlDataIni", "input").value;
            var periodoFim = FindControl("ctrlDataFim", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=UtilizacaoSistema&idFuncionario=" +
                idFuncionario + "&tipoAtividade=" + tipoAtividade + "&periodoIni=" + periodoIni + "&periodoFim=" + periodoFim + "&exportarExcel=" + exportarExcel);

            return false;
        }

    </script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" 
                                AppendDataBoundItems="True" DataSourceID="odsFuncionario" DataTextField="Nome" 
                                DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                onclick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Tipo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipo" runat="server">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Entrou</asp:ListItem>
                                <asp:ListItem Value="2">Saiu</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                onclick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ExibirHoras="True" 
                                ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            a
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ExibirHoras="True" 
                                ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                onclick="imgPesq_Click" />
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
                <asp:GridView ID="grdLogin" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
                    DataKeyNames="IdLogin" DataSourceID="odsLogin" GridLines="None" PageSize="25">
                    <Columns>
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="Data" HeaderText="Data" SortExpression="Data" />
                        <asp:TemplateField HeaderText="Tipo" SortExpression="Tipo">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Tipo") %>'></asp:Label>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("DescrSaidaManual") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="UsuarioSync" HeaderText="UWG" SortExpression="UsuarioSync" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <tr>
                    <td align="center">
                        <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                        &nbsp;&nbsp;&nbsp;
                    <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"> <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                    </td>
             </tr>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLogin" runat="server" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" 
                    SelectMethod="GetList" SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.LoginSistemaDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" 
                            PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipo" Name="tipo" 
                            PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" 
                            PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" 
                            PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" 
                    SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

