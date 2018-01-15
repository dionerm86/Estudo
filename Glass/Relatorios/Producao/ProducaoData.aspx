<%@ Page Title="Produção por Data de Entrega" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ProducaoData.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.ProducaoData" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var tipoData = FindControl("drpTipoData", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var tipo = FindControl("drpTipoSituacao", "select").itens();
            var idProcesso = FindControl("drpProcesso", "select").value;
            var idAplicacao = FindControl("drpAplicacao", "select").value;
            var naoCortados = FindControl("chkApenasNaoCortados", "input").checked;
            var codInternoMP = FindControl("txtCodProd", "input").value;
            var descrMP = FindControl("txtDescr", "input").value;
            var situacao = FindControl("drpSituacao", "select").itens();
            
            openWindow(600, 800, "RelBase.aspx?rel=ProducaoData&tipoData=" + tipoData + "&dataIni=" + dataIni + 
                "&dataFim=" + dataFim + "&tipo=" + tipo + "&situacao=" + situacao + "&idProcesso=" + idProcesso +
                "&idAplicacao=" + idAplicacao + "&naoCortados=" + naoCortados + 
                "&codInternoMP=" + codInternoMP + "&descrMP=" + descrMP + "&exportarExcel=" + exportarExcel);
        }
        
        // Carrega dados do produto com base no código do produto passado
        function setProduto() {
            var codInterno = FindControl("txtCodProd", "input").value;

            if (codInterno == "")
                return false;

            try {
                var retorno = MetodosAjax.GetProd(codInterno).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    FindControl("txtCodProd", "input").value = "";
                    return false;
                }

                FindControl("txtDescr", "input").value = retorno[2];
            }
            catch (err) {
                alert(err.value);
            }
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Tipo de Data"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoData" runat="server">
                                <asp:ListItem Value="1">Data Entrega</asp:ListItem>
                                <asp:ListItem Value="0">Data Fábrica</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" CausesValidation="False" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" ForeColor="#0066FF" Text="Período"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                CausesValidation="False" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkApenasNaoCortados" runat="server" Text="Apenas vidros não cortados" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" CausesValidation="False" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpTipoSituacao" runat="server" CheckAll="True" 
                                Title="Situação da produção">
                                <asp:ListItem Value="0">Pendente</asp:ListItem>
                                <asp:ListItem Value="1">Pronto</asp:ListItem>
                                <asp:ListItem Value="2">Etiqueta não impressa</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" CausesValidation="False" OnClick="imgPesq_Click" />
                        </td>
                          <td>
                            <asp:Label ID="Label7" runat="server" ForeColor="#0066FF" Text="Situação do Pedido"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpSituacao" runat="server" CheckAll="True" 
                                DataSourceID="odsSituacao" DataTextField="Descr" DataValueField="Id" 
                                ImageURL="~/Images/DropDown.png" OpenOnStart="False" 
                                Title="Selecione a situação" ondatabound="drpSituacao_DataBound">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" CausesValidation="False" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" ForeColor="#0066FF" Text="Processo"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpProcesso" runat="server" DataSourceID="odsProcesso" DataTextField="CodInterno"
                                DataValueField="IdProcesso" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" CausesValidation="False" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Aplicação"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAplicacao" runat="server" DataSourceID="odsAplicacao" DataTextField="CodInterno"
                                DataValueField="IdAplicacao" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" CausesValidation="False" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Matéria-Prima" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
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
                <asp:GridView GridLines="None" ID="grdProducaoData" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsProducaoData" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" AllowPaging="True"
                    PageSize="15" EmptyDataText="Nenhum registro encontrado">
                    <Columns>
                        <asp:BoundField DataField="DataHora" DataFormatString="{0:d}" HeaderText="Data" SortExpression="DataHora" />
                        <asp:BoundField DataField="DescrCorVidro" HeaderText="Cor" SortExpression="DescrCorVidro" />
                        <asp:BoundField DataField="DescrEspessura" HeaderText="Espessura" SortExpression="Espessura" />
                        <asp:BoundField DataField="CodAplicacao" HeaderText="Aplicação" SortExpression="CodAplicacao" />
                        <asp:BoundField DataField="CodProcesso" HeaderText="Processo" SortExpression="CodProcesso" />
                        <asp:BoundField DataField="TotM2" HeaderText="Tot. M²" SortExpression="TotM2" />
                        <asp:BoundField DataField="DescrPendente" HeaderText="Situação" SortExpression="Pendente" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProducaoData" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.RelDAL.ProducaoDataDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpTipoData" Name="tipoData" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpProcesso" Name="idProcesso" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpAplicacao" Name="idAplicacao" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipoSituacao" Name="tipo" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="chkApenasNaoCortados" Name="naoCortados" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInternoMP" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descrMP" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAplicacao" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.EtiquetaAplicacaoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProcesso" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.EtiquetaProcessoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSituacao" runat="server" SelectMethod="GetSituacaoPedido"
                    TypeName="Glass.Data.Helper.DataSources"></colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"><img border="0" 
                    src="../../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
