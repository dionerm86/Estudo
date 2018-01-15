<%@ Page Title="Relatório de Produção por Setor" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ProducaoFornoResumo.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.ProducaoFornoResumo" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var setor = FindControl("ddlSetor", "select").value;
            var idTurno = FindControl("ddlTurno", "select").value;

            openWindow(600, 800, "RelBase.aspx?rel=ProducaoFornoResumo&setor=" + setor + "&dtIni=" + dtIni + "&dtFim=" + dtFim + "&idTurno=" + idTurno + "&exportarExcel=" + exportarExcel);
        }

        function validaPeriodo() {
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;

            if (dtIni.split('/')[1] != dtFim.split('/')[1]) {
                alert("Não é possivel escolher o período de meses diferentes");
                return false;
            }

            return true;
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="lblMesAno" runat="server" ForeColor="#0066FF" Text="Período: "></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadOnly" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadOnly" ExibirHoras="False" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                CausesValidation="False" OnClick="imgPesq_Click" OnClientClick="if (!validaPeriodo()) return false;"/>
                        </td>
                        <td align="left">
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Setor"></asp:Label>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:DropDownList ID="ddlSetor" runat="server" AppendDataBoundItems="True" 
                                AutoPostBack="false" DataSourceID="odsSetor" DataTextField="Descricao" 
                                DataValueField="IdSetor"></asp:DropDownList>
                            
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                CausesValidation="False" OnClick="imgPesq_Click" OnClientClick="if (!validaPeriodo()) return false;"/>
                        </td>
                        <td align="left">
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Turno"></asp:Label>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:DropDownList ID="ddlTurno" runat="server" AppendDataBoundItems="True" 
                                AutoPostBack="false" DataSourceID="odsTUrno" DataTextField="Descricao" 
                                DataValueField="IdTurno">
                                <asp:ListItem Text="Todos" Value="0" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                            
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                CausesValidation="False" OnClick="imgPesq_Click" OnClientClick="if (!validaPeriodo()) return false;"/>
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
                <asp:GridView GridLines="None" ID="grdProducaoFornoResumo" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsProducaoFornoResumo" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" AllowPaging="True"
                    PageSize="15">
                    <Columns>
                        <asp:BoundField DataField="Dia" HeaderText="Dia" SortExpression="Dia" ReadOnly="True" />
                        <asp:BoundField DataField="Turno" HeaderText="Turno" SortExpression="Turno" ReadOnly="True" />
                        <asp:BoundField DataField="DescrCorVidro" HeaderText="Cor" SortExpression="DescrCorVidro" />
                        <asp:BoundField DataField="Espessura" HeaderText="Espessura" SortExpression="Espessura" />
                        <asp:BoundField DataField="Tipo" HeaderText="Tipo" SortExpression="Tipo" />
                        <asp:BoundField DataField="TotM2" HeaderText="Total m²" SortExpression="TotM2" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProducaoFornoResumo" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SelectMethod="GetList"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.RelDAL.ProducaoFornoResumoDAO">
                    <SelectParameters>
                         <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="DateTime" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="DateTime" />
                         <asp:ControlParameter ControlID="ddlSetor" Name="setor" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="ddlTurno" Name="idTurno" PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSetor" runat="server" SelectMethod="GetAll" 
                                TypeName="Glass.Data.DAL.SetorDAO"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTurno" runat="server" SelectMethod="GetList" 
                                TypeName="Glass.Data.DAL.TurnoDAO"></colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="if (!validaPeriodo()) return false; openRpt(false); return false;"><img border="0" 
                    src="../../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="if (!validaPeriodo()) return false; openRpt(true); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
