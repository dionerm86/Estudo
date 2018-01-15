<%@ Page Title="DRE" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="ListaPlanoConta.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaPlanoConta" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var idCategoriaConta = FindControl("drpCategoriaConta", "select").value;
            var idGrupoConta = FindControl("drpGrupoConta", "select").value;
            var idPlanoConta = FindControl("drpPlanoConta", "select").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var ajustado = FindControl("chkAjustado", "input").checked;
            var tipoMov = FindControl("drpTipoMov", "select").value;
            var tipoConta = FindControl("drpContabil", "select").value;
            var exibirChequeDevolvido = FindControl("chkExibirChequeDevolvido", "input").checked;
            var mes = FindControl("chkMes", "input");
            mes = mes.disabled ? "false" : mes.checked;
            var ordenar = FindControl("drpOrdenar", "select").value;

            if (!document.getElementById("<%= chkDetalhes.ClientID %>").checked) {
                openWindow(600, 800, "RelBase.aspx?rel=ListaPlanoContas&idGrupoConta=" + idGrupoConta + "&idPlanoConta=" + idPlanoConta +
                    "&idLoja=" + idLoja + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&idCategoriaConta=" + idCategoriaConta +
                    "&exportarExcel=" + exportarExcel + "&ajustado=" + ajustado + "&tipoMov=" + tipoMov + "&tipoConta=" + tipoConta + "&mes=" + mes + 
                    "&exibirChequeDevolvido=" + exibirChequeDevolvido + "&ordenar=" + ordenar);
            }
            else {
                var agruparDetalhes = FindControl("chkAgruparDetalhes", "input");
                agruparDetalhes = agruparDetalhes != null ? agruparDetalhes.checked : false;

                openWindow(600, 800, "RelBase.aspx?rel=ListaPlanoContasDet&idGrupoConta=" + idGrupoConta + "&idPlanoConta=" + idPlanoConta +
                    "&agruparDetalhes=" + agruparDetalhes + "&idLoja=" + idLoja + "&dataIni=" + dataIni + "&dataFim=" + dataFim +
                    "&idCategoriaConta=" + idCategoriaConta + "&exportarExcel=" + exportarExcel + "&ajustado=" + ajustado +
                    "&tipoMov=" + tipoMov + "&tipoConta=" + tipoConta + "&exibirChequeDevolvido=" + exibirChequeDevolvido + "&ordenar=" + ordenar);
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
                        <td>
                            <asp:DropDownList ID="drpPlanoConta" runat="server" AutoPostBack="True" DataSourceID="odsPlanoConta"
                                DataTextField="Descricao" DataValueField="IdConta" OnSelectedIndexChanged="drp_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Movimentações" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoMov" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Entradas</asp:ListItem>
                                <asp:ListItem Value="2">Saídas</asp:ListItem>
                            </asp:DropDownList>
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
                        <td>
                            <asp:CheckBox ID="chkAjustado" runat="server" Text="Relatório ajustado" AutoPostBack="True" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:CheckBox ID="chkDetalhes" runat="server" Text="Relatório detalhado" AutoPostBack="True" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:CheckBox ID="chkAgruparDetalhes" runat="server" Text="Agrupar impressão detalhada" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Tipos de contas" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContabil" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Contábeis</asp:ListItem>
                                <asp:ListItem Value="2">Não contábeis</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkMes" runat="server" Text="Agrupar impressão por mês" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkExibirChequeDevolvido" runat="server" Text="Exibir movimentação de cheque devolvido" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
                <table runat="server" ID="tbOrdenar">
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0" Selected="True">Plano de Contas</asp:ListItem>
                                <asp:ListItem Value="1">Data</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdPlanoContas" runat="server" AllowPaging="True"
                    AutoGenerateColumns="False" DataSourceID="odsListPlanoContas" OnLoad="grdPlanoContas_Load"
                    EmptyDataText="Nenhuma movimentação encontrada." CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:BoundField DataField="GrupoConta" HeaderText="Grupo" SortExpression="GrupoConta" />
                        <asp:BoundField DataField="PlanoConta" HeaderText="Plano Conta" SortExpression="PlanoConta" />
                        <asp:BoundField DataField="DescrTipo" HeaderText="Movimentação" SortExpression="DescrTipo" />
                        <asp:BoundField DataField="Referencial" HeaderText="Cliente/Fornecedor" SortExpression="Referencial" />
                        <asp:BoundField DataField="Data" DataFormatString="{0:d}" HeaderText="Data" SortExpression="Data" />
                        <asp:BoundField DataField="Valor" HeaderText="Valor" SortExpression="Valor" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="VencPeriodoNaoPagas" DataFormatString="{0:c}" HeaderText="Venc. não pagas"
                            SortExpression="VencPeriodoNaoPagas" />
                        <asp:BoundField DataField="VencPassadoPagasPeriodo" DataFormatString="{0:c}" HeaderText="Pagas venc. passado"
                            SortExpression="VencPassadoPagasPeriodo" />
                        <asp:BoundField DataField="ValorAjustado" DataFormatString="{0:c}" HeaderText="Valor ajustado"
                            SortExpression="ValorAjustado" />
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
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsListPlanoContas" runat="server" SelectMethod="GetList"
                    TypeName="Glass.Data.RelDAL.PlanoContasDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    OnSelecting="odsListPlanoContas_Selecting" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpCategoriaConta" Name="idCategoriaConta" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpGrupoConta" Name="idGrupoConta" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpPlanoConta" Name="idPlanoConta" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpTipoMov" Name="tipoMov" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpContabil" Name="tipoConta" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkAjustado" Name="ajustado" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="chkExibirChequeDevolvido" 
                            Name="exibirChequeDevolvido" PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="drpOrdenar" Name="ordenar" PropertyName="SelectedValue"
                            Type="Int32" />
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
