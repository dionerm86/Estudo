<%@ Page Title="Selecione o Roteiro de Produção" Language="C#" MasterPageFile="~/Layout.master"
     AutoEventWireup="true" CodeBehind="SelRoteiroProducao.aspx.cs" Inherits="Glass.UI.Web.Utils.SelRoteiroProducao" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        var selecionado = false;

        function setRoteiro(idRoteiro) {

            if (selecionado)
                return;

            selecionado = true;

            window.opener.setRoteiro(idRoteiro);

            closeWindow();
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:Panel ID="panPesquisar" runat="server" BorderColor="#EDE9DA" BorderWidth="2"
                    HorizontalAlign="Left" Style="white-space: nowrap" Width="377px">
                    <span style="display: block; text-align: center; font-weight: bold; background-color: #5D7B9D; color: #FFFFFF;">Pesquisar</span>
                    <div style="padding-right: 4px; padding-left: 4px; padding-bottom: 4px; padding-top: 4px; text-align:center">
                        <table border="0" cellspacing="1">
                            <tr align="center">
                                <td>Cód. Processo
                                </td>
                                <td>
                                    <asp:TextBox ID="txtCodProcesso" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                        ToolTip="Pesquisar" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>
                <br />
                <asp:GridView ID="grdRoteiroProducao" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataKeyNames="Codigo"
                    DataSourceID="odsRoteiroProducao"
                    EmptyDataText="Não há roteiros associados." GridLines="None">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="return setRoteiro('<%# Eval("Codigo") %>');">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CodigoInternoProcesso" HeaderText="Processo"
                            SortExpression="CodigoInternoProcesso" />
                        <asp:BoundField DataField="DescricaoSetores" HeaderText="Setores"
                            SortExpression="DescricaoSetores" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource ID="odsRoteiroProducao" runat="server"
                    Culture="pt-BR"
                    DataObjectTypeName="WebGlass.Business.RoteiroProducao.Entidade.RoteiroProducao"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="ObtemListaParaSelecaoCount" SelectMethod="ObtemListaParaSelecao"
                    StartRowIndexParameterName="startRow"
                    TypeName="WebGlass.Business.RoteiroProducao.Fluxo.RoteiroProducao"
                    SortParameterName="sortExpression">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodProcesso" PropertyName="Text" Name="codProcesso" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
