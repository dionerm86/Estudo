<%@ Page Title="DRE" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="ListaDreCompetencia.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaDreCompetencia" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var idCategoriaConta = FindControl("drpCategoriaConta", "select").value;
            var idGrupoConta = FindControl("drpGrupoConta", "select").value;
            var idsPlanoConta = FindControl("cblPlanoConta", "select").itens();
            var idLoja = FindControl("drpLoja", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;

            if (!document.getElementById("<%= chkDetalhes.ClientID %>").checked) {
                openWindow(600, 800, "RelBase.aspx?rel=ListaDreCompetencia&idLoja=" + idLoja + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&idCategoriaConta=" + idCategoriaConta + "&idsPlanoConta=" + idsPlanoConta +
                    "&detalhes=" + false + "&idGrupoConta=" + idGrupoConta + "&exportarExcel=" + exportarExcel);
            }
            else {
                var chkDetalhes = FindControl("chkDetalhes", "input");
                var agrupar =  chkDetalhes.checked;

                openWindow(600, 800, "RelBase.aspx?rel=ListaDreCompetencia&idLoja=" + idLoja + "&dataIni=" + dataIni + "&dataFim=" + dataFim +"&idCategoriaConta=" + idCategoriaConta + "&idsPlanoConta=" + idsPlanoConta +
                    "&detalhes=" + agrupar + "&idGrupoConta=" + idGrupoConta + "&exportarExcel=" + exportarExcel);
            }

            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Categoria Conta" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpCategoriaConta" runat="server" AppendDataBoundItems="True"
                                AutoPostBack="True" DataSourceID="odsCategoriaConta" DataTextField="Descricao"
                                DataValueField="IdCategoriaConta" OnSelectedIndexChanged="drpCategoriaConta_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Grupo Conta" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupoConta" runat="server" AutoPostBack="True" DataSourceID="odsGrupoConta"
                                DataTextField="Name" DataValueField="Id" AppendDataBoundItems="True"
                                OnSelectedIndexChanged="drp_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Plano Conta" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td style="align-content:flex-start">
                            <sync:CheckBoxListDropDown ID="cblPlanoConta" runat="server"   Width="400px" CheckAll="False" Title="Selecione o Plano Conta" 
                                    DataSourceID="odsPlanoConta" DataTextField="Descricao" DataValueField="IdConta" ImageURL="~/Images/DropDown.png"
                                    OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsLoja" DataTextField="NomeFantasia" DataValueField="IdLoja" OnSelectedIndexChanged="drp_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:CheckBox ID="chkDetalhes" runat="server" Text="Relatório detalhado" AutoPostBack="True" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdPlanoContas" runat="server" AllowPaging="True"
                    AutoGenerateColumns="False" DataSourceID="odsListPlanoContas"
                    EmptyDataText="Nenhuma movimentação encontrada." CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:BoundField DataField="GrupoConta" HeaderText="Grupo" SortExpression="GrupoConta" />
                        <asp:BoundField DataField="PlanoConta" HeaderText="Plano Conta" SortExpression="PlanoConta" />
                        <asp:BoundField DataField="Referencial" HeaderText="Cliente/Fornecedor" SortExpression="Referencial" />
                        <asp:BoundField DataField="Data" DataFormatString="{0:d}" HeaderText="Data" SortExpression="Data" />
                        <asp:BoundField DataField="Valor" HeaderText="Valor" SortExpression="Valor" DataFormatString="{0:C}" />
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;">
                <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsListPlanoContas" runat="server" SelectMethod="PesquisarDreCompetencia"
                    TypeName="Glass.Data.RelDAL.PlanoContasDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="PesquisarDreCompetenciaCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpCategoriaConta" Name="idCategoriaConta" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpGrupoConta" Name="idGrupoConta" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="cblPlanoConta" Name="idsPlanoConta" PropertyName="SelectedValues" />
                        <asp:ControlParameter ControlID="chkDetalhes" Name="detalhado" PropertyName="Checked" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCategoriaConta" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.CategoriaContaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoConta" runat="server" 
                                              SelectMethod="ObtemGruposContaPorCategoria"
                                              TypeName="Glass.Financeiro.Negocios.IPlanoContasFluxo">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpCategoriaConta" Name="idCategoriaConta" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoConta" runat="server" SelectMethod="GetByGrupo"
                    TypeName="Glass.Data.DAL.PlanoContasDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpGrupoConta" Name="idGrupoConta" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfGrupos" runat="server" />
            </td>
        </tr>
    </table>

    <script type="text/javascript">       
        //if (document.getElementById(chkDetalhes.ClientID).checked)
        //document.getElementById("divDetalhes").style.display = "block";
    </script>

</asp:Content>
