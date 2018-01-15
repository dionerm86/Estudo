<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImprimirProjeto.aspx.cs"
    Inherits="Glass.UI.Web.WebGlassParceiros.ImprimirProjeto" Title="Impressão dos Cálculos do Pedido n.º "
    MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function openRpt()
        {
            var itens = new Array();

            var inputs = document.getElementById("<%= grdItemProjeto.ClientID %>").getElementsByTagName("input");
            for (i = 0; i < inputs.length; i++)
            {
                if (inputs[i].type != "checkbox" || inputs[i].id.indexOf("chkImprimirTodas") > -1 || !inputs[i].checked)
                    continue;

                var idItemProjeto = inputs[i].parentNode.getElementsByTagName("input");
                for (j = 0; j < idItemProjeto.length; j++)
                    if (idItemProjeto[j].id.indexOf("hdfIdItemProjeto") > -1)
                {
                    idItemProjeto = idItemProjeto[j].value;
                    break;
                }

                itens.push(idItemProjeto);
            }

            if (itens.length == 0)
            {
                alert("Selecione ao menos 1 item de projeto para ser impresso.");
                return false;
            }

            var pcp = '<%= !String.IsNullOrEmpty(Request["pcp"]) ? "&pcp=" + Request["pcp"] : "" %>';
            redirectUrl("../Relatorios/Projeto/RelBase.aspx?rel=imagemProjeto&idProjeto=&idItemProjeto=" + itens.join(",") + pcp);

            //openWindow(600, 800, "../../Relatorios/Projeto/RelBase.aspx?rel=imagemProjeto&idProjeto=" + idProjeto + "&idItemProjeto=" + idItemProjeto);

            return false;
        }

        function marcarTodas(marcar)
        {
            var inputs = document.getElementById("<%= grdItemProjeto.ClientID %>").getElementsByTagName("input");
            for (i = 0; i < inputs.length; i++)
            {
                if (inputs[i].type != "checkbox" || inputs[i].id.indexOf("chkImprimirTodas") > -1)
                    continue;

                inputs[i].checked = marcar;
            }
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdItemProjeto" runat="server" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdItemProjeto" DataSourceID="odsItemProjeto"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfIdProjeto" runat="server" Value='<%# Eval("IdProjeto") %>' />
                                <asp:HiddenField ID="hdfIdItemProjeto" runat="server" Value='<%# Eval("IdItemProjeto") %>' />
                                <asp:CheckBox ID="chkImprimir" runat="server" />
                            </ItemTemplate>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkImprimirTodas" runat="server" OnClick="marcarTodas(this.checked)" />
                            </HeaderTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="CodigoModelo" HeaderText="Cód." SortExpression="CodigoModelo">
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Ambiente" HeaderText="Ambiente" SortExpression="Ambiente" />
                        <asp:BoundField DataField="DescrCorVidro" HeaderText="Cor Vidro" SortExpression="DescrCorVidro" />
                        <asp:BoundField DataField="DescrCorAluminio" HeaderText="Cor Alumínio" SortExpression="DescrCorAluminio" />
                        <asp:BoundField DataField="DescrCorFerragem" HeaderText="Cor Ferragem" SortExpression="DescrCorFerragem" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Total" SortExpression="Total">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <asp:LinkButton ID="lnkImprmir" runat="server" OnClientClick="openRpt(); return false">
                    <img border="0" src="../Images/Relatorio.gif" /> Imprimir</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsItemProjeto" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountImpr" SelectMethod="GetListImpr" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ItemProjetoDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idProjeto" QueryStringField="idProjeto" Type="UInt32" />
                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                        <asp:QueryStringParameter Name="idOrcamento" QueryStringField="idOrcamento" Type="UInt32" />
                        <asp:QueryStringParameter Name="pcp" QueryStringField="pcp" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
