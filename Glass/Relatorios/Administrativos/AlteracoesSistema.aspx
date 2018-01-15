<%@ Page Title="Alterações no Sistema" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="AlteracoesSistema.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.AlteracoesSistema" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register src="../../Controls/ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt()
        {
            var tipo = FindControl("drpTipo", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var tabela = FindControl("drpLocal", "select").value;
            var idFunc = FindControl("drpFunc", "select").value;
            var campo = FindControl("selCampo_hdfValor", "input").value;

            openWindow(600, 800, "RelBase.aspx?rel=AlteracoesSistema&tipo=" + tipo + "&dataIni=" + dataIni + 
                "&dataFim=" + dataFim + "&tabela=" + tabela + "&idFunc=" + idFunc + "&campo=" + campo);
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
                            <asp:DropDownList ID="drpTipo" runat="server">
                                <asp:ListItem Value="">Todos</asp:ListItem>
                                <asp:ListItem Value="Alt">Alteração</asp:ListItem>
                                <asp:ListItem Value="Canc">Cancelamento</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Local" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLocal" runat="server" DataSourceID="odsTabelas" DataTextField="Value"
                                DataValueField="Key" ondatabinding="ItemFiltro_DataBinding" 
                                ondatabound="ItemFiltro_DataBound">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionarios"
                                DataTextField="Value" DataValueField="Key" 
                                ondatabinding="ItemFiltro_DataBinding" ondatabound="ItemFiltro_DataBinding">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Campo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlSelPopup ID="selCampo" runat="server" ColunasExibirPopup="Key|Value" 
                                DataSourceID="odsCampos" DataTextField="Value" DataValueField="Key" 
                                FazerPostBackBotaoPesquisar="True" PermitirVazio="True" 
                                TitulosColunas="Cód.|Campo" TituloTela="Selecione o campo" 
                                TextWidth="200px" OnDataBinding="ItemFiltro_DataBinding" OnDataBound="ItemFiltro_DataBound" />
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
                <asp:GridView ID="grdAlteracoesSistema" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsAlteracoesSistema"
                    GridLines="None" PageSize="20">
                    <Columns>
                        <asp:BoundField DataField="DescrTipo" HeaderText="Tipo" ReadOnly="True" SortExpression="Tipo" />
                        <asp:BoundField DataField="NomeTabela" HeaderText="Local" ReadOnly="True" SortExpression="Tabela" />
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia" />
                        <asp:BoundField DataField="Data" HeaderText="Data" SortExpression="Data" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="Campo" HeaderText="Campo" SortExpression="Campo" />
                        <asp:BoundField DataField="Valor" HeaderText="Alteração" SortExpression="Valor" />
                        <asp:BoundField DataField="InfoAdicional" HeaderText="Info. Adicional" SortExpression="InfoAdicional" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAlteracoesSistema" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SelectMethod="GetList"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.RelDAL.AlteracoesSistemaDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpTipo" Name="tipo" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpLocal" Name="tabela" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="selCampo" Name="campo" 
                            PropertyName="Valor" Type="String" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTabelas" runat="server" SelectMethod="GetTabelas" 
                    TypeName="Glass.Data.RelDAL.AlteracoesSistemaDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpTipo" Name="tipo" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="selCampo" Name="campo" 
                            PropertyName="Valor" Type="String" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionarios" runat="server" SelectMethod="GetFuncionarios"
                    TypeName="Glass.Data.RelDAL.AlteracoesSistemaDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpTipo" Name="tipo" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpLocal" Name="tabela" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="selCampo" Name="campo" 
                            PropertyName="Valor" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCampos" runat="server" SelectMethod="GetCampos"
                    TypeName="Glass.Data.RelDAL.AlteracoesSistemaDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpTipo" Name="tipo" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpLocal" Name="tabela" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" 
                            PropertyName="SelectedValue" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false">
                    <img src="../../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
