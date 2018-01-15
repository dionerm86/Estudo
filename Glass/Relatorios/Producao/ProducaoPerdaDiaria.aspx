<%@ Page Title="Controle de Produção e Perda Diária" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ProducaoPerdaDiaria.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.ProducaoPerdaDiaria" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        var dadosPost;

        function openRpt() {
            var mes = FindControl("drpMes", "select").value;
            var idSetor = FindControl("drpSetor", "select").value;
            var incluirTrocaDevolucao =
                FindControl("chkIncluirTrocaDevolucao", "input") != null ?
                    FindControl("chkIncluirTrocaDevolucao", "input").checked : false;
            var ano = FindControl("drpAno", "select").value;
            var tipoRelatorio = FindControl("drpTipoGrafico", "select").value;

            dadosPost = new Object();
            dadosPost["rel"] = "GraficoProducaoPerdaDiaria";
            dadosPost["mes"] = mes;
            dadosPost["ano"] = ano;
            dadosPost["setor"] = idSetor;
            dadosPost["incluirTrocaDevolucao"] = incluirTrocaDevolucao;
            dadosPost["grfProdAcumulada"] = "";
            dadosPost["grfIndicePerda"] = "";
            dadosPost["grfProdDiaria"] = "";
            dadosPost["grfPerdaMensal"] = "";
            dadosPost["grfPerdaSetores"] = "";
            dadosPost["grf10mm"] = "";
            dadosPost["grf8mm"] = "";
            dadosPost["grf6mm"] = "";

            if (tipoRelatorio == "1" && FindControl("hdfGrafProducaoAcumulada", "input") != null) {
                dadosPost["tipoGrafico"] = "1";
                dadosPost["grfProdAcumulada"] = FindControl("hdfGrafProducaoAcumulada", "input").value;
                dadosPost["grfIndicePerda"] = FindControl("hdfGrafIndicePerda", "input").value;
                dadosPost["grfProdDiaria"] = FindControl("hdfGrafProducaoDiaria", "input").value;
                dadosPost["grfPerdaMensal"] = FindControl("hdfGrafPerdaMensal", "input").value;
                dadosPost["grfPerdaSetores"] = FindControl("hdfGrafPerdaSetores", "input").value;
            }
            else if (tipoRelatorio == "2" && FindControl("hdfGraf10mm", "input") != null) {
                dadosPost["tipoGrafico"] = "2";
                dadosPost["grf10mm"] = FindControl("hdfGraf10mm", "input").value;
                dadosPost["grf8mm"] = FindControl("hdfGraf8mm", "input").value;
                dadosPost["grf6mm"] = FindControl("hdfGraf6mm", "input").value;
            }

            if ((tipoRelatorio == "1" && (dadosPost["grfProdAcumulada"] == "" && dadosPost["grfIndicePerda"] == "" && dadosPost["grfProdDiaria"] == ""))
                || (tipoRelatorio == "2" && dadosPost["grf10mm"] == "" && dadosPost["grf8mm"] == "" && dadosPost["grf6mm"] == "")) {
                alert("Antes de imprimir, voce deve gerar os gráficos.");
                return false;
            }

            openWindow(600, 800, "RelBase.aspx?postData=getPostData()");
            return false;
        }

        function getPostData() {
            return dadosPost;
        }
 
        function atualizarChkIncluirTrocaDevolucao(controle) {
            var chkIncluirTrocaDevolucao = FindControl("chkIncluirTrocaDevolucao", "input");
            
            if (chkIncluirTrocaDevolucao != null) {
                if (controle.value === "2") {
                        chkIncluirTrocaDevolucao.parentNode.style.display = "";
                }
                else {
                    chkIncluirTrocaDevolucao.parentNode.style.display = "none";
                    chkIncluirTrocaDevolucao.checked = false;
                }
            }
        }

        function validarSetorEscolhido() {
            var idSetor = FindControl("drpSetor", "select").value;
            var tipoRelatorio = FindControl("drpTipoGrafico", "select").value;

            if (idSetor <= 0 && tipoRelatorio == "1") {
                alert("Este tipo de gráfico necessita que um único setor seja informado.");
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
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Mês Referência" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpMes" runat="server">
                                <asp:ListItem Text="Janeiro" Value="1" />
                                <asp:ListItem Text="Fevereiro" Value="2" />
                                <asp:ListItem Text="Março" Value="3" />
                                <asp:ListItem Text="Abril" Value="4" />
                                <asp:ListItem Text="Maio" Value="5" />
                                <asp:ListItem Text="Junho" Value="6" />
                                <asp:ListItem Text="Julho" Value="7" />
                                <asp:ListItem Text="Agosto" Value="8" />
                                <asp:ListItem Text="Setembro" Value="9" />
                                <asp:ListItem Text="Outubro" Value="10" />
                                <asp:ListItem Text="Novembro" Value="11" />
                                <asp:ListItem Text="Dezembro" Value="12" />
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAno" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imbPesquisar" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imbPesquisar_Click" OnClientClick="return validarSetorEscolhido();" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Setor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSetor" runat="server" AppendDataBoundItems="True" AutoPostBack="False"
                                DataSourceID="odsSetor" DataTextField="Descricao" DataValueField="IdSetor">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblTipoGrafico" runat="server" Text="Tipo de Gráfico" ForeColor="#0066FF" />
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoGrafico" runat="server" onclick="atualizarChkIncluirTrocaDevolucao(this);">
                                <asp:ListItem Text="Produção Acumulada" Value="1" />
                                <asp:ListItem Text="Produção e Perda por produto" Value="2" />
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkIncluirTrocaDevolucao" runat="server" Text="Incluir troca/devolução">
                            </asp:CheckBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table id="tbGraficosPerdaAcumulada" runat="server" visible="false" width="100%">
                    <tr id="trGrafico1">
                        <td colspan="2">
                            <asp:Chart ID="chtProdAcumulada" runat="server" Visible="false" />
                        </td>
                    </tr>
                    <tr id="trGrafico2">
                        <td colspan="2">
                            <asp:Chart ID="chtIndicePerda" runat="server" Visible="false" />
                        </td>
                    </tr>
                    <tr id="trGrafico3">
                        <td>
                            <asp:Chart ID="chtProducaoDiaria" runat="server" Visible="false" />
                        </td>
                        <td>
                            <asp:Chart ID="chtPerdaMensal" runat="server" Visible="false" />
                        </td>
                    </tr>
                    <tr id="trGrafico5">
                        <td colspan="2">
                            <asp:Chart ID="chtPerdaSetores" runat="server" Visible="false" />
                        </td>
                    </tr>
                    <tr id="trHiddens">
                        <td colspan="2">
                            <asp:HiddenField ID="hdfGrafProducaoAcumulada" runat="server" />
                            <asp:HiddenField ID="hdfGrafIndicePerda" runat="server" />
                            <asp:HiddenField ID="hdfGrafProducaoDiaria" runat="server" />
                            <asp:HiddenField ID="hdfGrafPerdaMensal" runat="server" />
                            <asp:HiddenField ID="hdfGrafPerdaSetores" runat="server" />
                        </td>
                    </tr>
                </table>
                <table id="tbGraficosProduto" runat="server" visible="false">
                    <tr>
                        <td id="tdGraf10mm">
                            <asp:Chart ID="chtProd10mm" runat="server" Visible="false" />
                        </td>
                    
                        <td>
                            <asp:Chart ID="chtProd8mm" runat="server" Visible="false" />
                        </td>
                    </tr>
                    <tr valign="top">
                        <td>
                            <asp:Chart ID="chtProd6mm" runat="server" Visible="false" />
                        </td>
                    
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdProdutoProducao" runat="server" AutoGenerateColumns="False"
                                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EmptyDataText="Não foram encontradas vendas para esse filtro específico." DataSourceID="odsProdutoProducao"
                                Width="600px">
                                <Columns>
                                    <asp:BoundField DataField="CorVidro" HeaderText="Cor" ItemStyle-HorizontalAlign="Center">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="EspessuraFormat" HeaderText="Espessura" ItemStyle-HorizontalAlign="Center">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="TotProdM2" HeaderText="Produção" ItemStyle-HorizontalAlign="Center">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="TotPerdaM2" HeaderText="Perda" ItemStyle-HorizontalAlign="Center">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="IndicePerdaProdFormat" HeaderText="Índice" ItemStyle-HorizontalAlign="Center">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                    </asp:BoundField>
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutoProducao" runat="server"
                                MaximumRowsParameterName="pageSize" SelectMethod="GetPerdaProduto" StartRowIndexParameterName="startRow"
                                TypeName="Glass.Data.RelDAL.GraficoProdPerdaDiariaDAO" EnablePaging="False">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="drpSetor" Name="setor" PropertyName="SelectedValue"
                                        Type="Int32" />
                                    <asp:ControlParameter ControlID="chkIncluirTrocaDevolucao" Name="incluirTrocaDevolucao" PropertyName="Checked"
                                        Type="Boolean" />
                                    <asp:ControlParameter ControlID="drpMes" Name="mes" PropertyName="SelectedValue"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="drpAno" Name="ano" PropertyName="SelectedValue"
                                        Type="String" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:HiddenField ID="hdfGraf10mm" runat="server" />
                            <asp:HiddenField ID="hdfGraf8mm" runat="server" />
                            <asp:HiddenField ID="hdfGraf6mm" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"> <img src="../../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSetor" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.SetorDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script>
                
        var chkIncluirTrocaDevolucao = FindControl("chkIncluirTrocaDevolucao", "input");
         
        if (chkIncluirTrocaDevolucao != null) {
            if (FindControl("drpTipoGrafico", "select").value === "2") {
                    chkIncluirTrocaDevolucao.parentNode.style.display = "";
            }
            else {
                chkIncluirTrocaDevolucao.parentNode.style.display = "none";
                chkIncluirTrocaDevolucao.checked = false;
            }
        }
         
    </script>

</asp:Content>
