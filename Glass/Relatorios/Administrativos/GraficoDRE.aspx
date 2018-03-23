<%@ Page Title="Gráfico DRE" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="GraficoDRE.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.GraficoDRE" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        var data;

        function openRpt() {
            var idCategoriaConta = FindControl("drpCategoriaConta", "select").value;
            var idGrupoConta = FindControl("drpGrupoConta", "select").value;
            var idsPlanoConta = FindControl("cblPlanoConta", "select").itens();
            var idLoja = FindControl("drpLoja", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var ajustado = FindControl("chkAjustado", "input").checked;
            var tipoMov = FindControl("drpTipoMov", "select").value;
            var tipoConta = FindControl("drpContabil", "select").value;

            var tempFile = FindControl("hdfTempFile", "input").value;
            var rel = openWindow(600, 800, "RelBase.aspx?postData=getPostData()");
            
            data = new Object();
            data["rel"] = "GraficoDRE";
            data["dataIni"] = dataIni;
            data["dataFim"] = dataFim;
            data["idLoja"] = idLoja;
            data["idCategoriaConta"] = idCategoriaConta;
            data["idGrupoConta"] = idGrupoConta;
            data["idsPlanoConta"] = idsPlanoConta;
            data["ajustado"] = ajustado;
            data["tipoMov"] = tipoMov;
            data["tipoConta"] = tipoConta;
            data["tempFile"] = tempFile;
            
            return false;
        }
        function getPostData() {
            return data;
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
                                DataTextField="Name" DataValueField="Id" AppendDataBoundItems="True">
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
                                DataSourceID="odsLoja" DataTextField="NomeFantasia" DataValueField="IdLoja">
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
                            <asp:CheckBox ID="chkAjustado" runat="server" Text="Relatório ajustado" AutoPostBack="True" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <div id="msChart">
                    <asp:Chart ID="Chart1" runat="server">
                    </asp:Chart>
                </div>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdDRE" runat="server" AutoGenerateColumns="False"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EnableViewState="False">
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false">
                    <img alt="" border="0" src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <%--<colo:VirtualObjectDataSource culture="pt-BR" ID="odsListPlanoContas" runat="server" SelectMethod="GetList"
                    TypeName="Glass.Data.RelDAL.PlanoContasDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    OnSelecting="odsListPlanoContas_Selecting">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpCategoriaConta" Name="idCategoriaConta" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpGrupoConta" Name="idGrupoConta" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpPlanoConta" Name="idPlanoConta" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="hdfGrupos" Name="grupos" PropertyName="Value" Type="String" />
                        <asp:ControlParameter ControlID="drpTipoMov" Name="tipoMov" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpContabil" Name="tipoConta" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkAjustado" Name="ajustado" PropertyName="Checked"
                            Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>--%>
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
                <asp:HiddenField ID="hdfTempFile" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
