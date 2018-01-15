<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaPassivos.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaPassivos" Title="Lista de Passivos" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(detalhes)
        {
            var idLoja = FindControl("drpLoja", "select").value;
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var planoConta = FindControl("txtPlanoConta", "input").value;
            var idGrupoConta = FindControl("drpGrupoConta", "select").value;

            openWindow(600, 800, "RelBase.aspx?rel=ListaPassivos&idLoja=" + idLoja + "&dtIni=" + dtIni + "&dtFim=" + dtFim + 
                "&idGrupoConta=" + idGrupoConta + "&planoConta=" + planoConta + "&detalhes=" + detalhes);
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label12" runat="server" ForeColor="#0066FF" Text="Loja"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="false"/>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Período"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Grupo de conta" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupoConta" runat="server" AppendDataBoundItems="True" DataSourceID="odsGrupoConta"
                                DataTextField="Name" DataValueField="Id">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Plano de conta" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPlanoConta" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
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
                <asp:GridView ID="grdListaPassivos" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsPassivos" GridLines="None"
                    PageSize="20">
                    <Columns>
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Plano de Conta" SortExpression="DescrPlanoConta" />
                        <asp:BoundField DataField="ValorPagar" DataFormatString="{0:c}" HeaderText="Valor a Pagar"
                            SortExpression="ValorPagar" />
                        <asp:BoundField DataField="ValorPago" DataFormatString="{0:c}" HeaderText="Valor Pago"
                            SortExpression="ValorPago" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:c}" HeaderText="Total" SortExpression="Total" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
        <tr align="center">
            <td>
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false">
                    <img src="../images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkImprimirDet" runat="server" OnClientClick="openRpt(true); return false">
                    <img src="../images/Printer.png" border="0" /> Imprimir (com detalhes)</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPassivos" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.RelDAL.ListaPassivosDAO"
                    >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpGrupoConta" Name="idGrupoConta" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtPlanoConta" Name="planoConta" PropertyName="Text"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoConta" runat="server" 
                                              SelectMethod="ObtemGruposConta"
                                              TypeName="Glass.Financeiro.Negocios.IPlanoContasFluxo">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
