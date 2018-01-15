<%@ Page Title="Otimizações" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstOtimizacao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstOtimizacao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        function openRptOtimizacao(idOtimizacao) {
            if (idOtimizacao == null || idOtimizacao == "") {
                alert("Informe a otimizacao.");
                return false;
            }

            openWindow(500, 700, "../Relatorios/RelBase.aspx?rel=ResultadoOtimizacao&idOtimizacao=" + idOtimizacao, null, true, true);
            return false;
        }

        function openRptLstPecas(idOtimizacao) {
            if (idOtimizacao == null || idOtimizacao == "") {
                alert("Informe a otimizacao.");
                return false;
            }

            openWindow(500, 700, "../Relatorios/RelBase.aspx?rel=PecasOtimizacao&idOtimizacao=" + idOtimizacao, null, true, true);
            return false;
        }

        function exibirDados(botao, idOtimizacao, msg) {

            var boxObs = FindControl("boxObs", "div");
            var lblObs = FindControl("lblObs", "span");

            lblObs.innerHTML = msg;

            TagToTip('boxObs', FADEIN, 300, COPYCONTENT, false, TITLE, 'Pedidos da otimização: ' + idOtimizacao, CLOSEBTN, true,
                CLOSEBTNTEXT, 'Fechar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                FIX, [botao, 9 - getTableWidth('boxObs'), -41 - getTableHeight('boxObs')]);
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserirOtmizacao" runat="server" OnClick="lnkInserirOtmizacao_Click">Criar otimização</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdOtimizacao" runat="server" SkinID="gridViewEditable"
                    DataSourceID="odsOtimizacao" DataKeyNames="IdOtimizacao" AutoGenerateColumns="False"
                    EmptyDataText="Nenhuma otimização encontrada." AllowPaging="True" AllowSorting="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbImpOtimizacao" runat="server" ImageUrl="~/Images/printer.png"
                                    ToolTip="Imprimir Otimização" Visible="true"
                                    OnClientClick='<%# "return openRptOtimizacao("+ Eval("IdOtimizacao") + ");" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbImpOtimizacao1" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    ToolTip="Imprimir relação de peças" Visible="true"
                                    OnClientClick='<%# "return openRptLstPecas("+ Eval("IdOtimizacao") + ");" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Cód" SortExpression="IdOtimizacao" DataField="IdOtimizacao" />
                        <asp:BoundField HeaderText="Funcionário" SortExpression="NomeFuncionario" DataField="NomeFuncionario" />
                        <asp:BoundField HeaderText="Data" SortExpression="DataCadastro" DataField="DataCadastro" />
                        <asp:BoundField HeaderText="Peso Bruto" SortExpression="PesoBruto" DataField="PesoBruto" DataFormatString="{0:F3} Kg" />
                        <asp:BoundField HeaderText="Peso Liquido" SortExpression="PesoLiquido" DataField="PesoLiquido" DataFormatString="{0:F3} Kg" />
                        <asp:BoundField HeaderText="Peso Retalho" SortExpression="PesoRetalho" DataField="PesoRetalho" DataFormatString="{0:F3} Kg" />
                        <asp:BoundField HeaderText="Peso perda" SortExpression="PesoPerda" DataField="PesoPerda" DataFormatString="{0:F3} Kg" />
                        <asp:TemplateField HeaderText="Pedidos">
                            <ItemTemplate>
                                <asp:PlaceHolder ID="PlaceHolder2" runat="server">
                                    <a href="#" onclick='exibirDados(this, &#039;<%# Eval("IdOtimizacao") %>&#039;, &#039;<%# Eval("Pedidos") %>&#039;); return false;'>
                                        <img alt="" border="0" src="../Images/blocodenotas.png" title="Ver pedidos." /></a>
                                </asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                    <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsOtimizacao" runat="server"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarOtimizacoes"
                    SelectByKeysMethod="ObterOtimizacao"
                    SortParameterName="sortExpression"
                    TypeName="Glass.PCP.Negocios.IOtimizacaoFluxo"
                    DataObjectTypeName="Glass.PCP.Negocios.Entidades.Otimizacao">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
     <div id="boxObs" style="display: none; width: 350px;">
        <asp:Label ID="lblObs" runat="server" Text="Label"></asp:Label>
    </div>
</asp:Content>
