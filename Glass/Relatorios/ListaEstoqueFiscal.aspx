<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaEstoqueFiscal.aspx.cs" Title="Relatório de Estoque Fiscal"
    Inherits="Glass.UI.Web.Relatorios.ListaEstoqueFiscal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(exportarExcel) {
            var idLoja = FindControl("drpLoja", "select").value;
            var codProd = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescr", "input").value;
            var idGrupo = FindControl("drpGrupo", "select").value;
            var idSubgrupo = FindControl("drpSubgrupo", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;           
            var apenasEstoqueFiscal = FindControl("chkApenasEstoqueFiscal", "input").checked;

            openWindow(600, 800, "RelBase.aspx?rel=EstoqueFiscal&idLoja=" + idLoja + "&codProd=" + codProd + "&descr=" + descrProd +
                "&idGrupo=" + idGrupo + "&idSubgrupo=" + idSubgrupo + "&apenasEstoqueFiscal=" + apenasEstoqueFiscal + "&situacao=" + situacao + 
                "&exportarExcel=" + exportarExcel);
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
                else {
                    FindControl("txtDescr", "input").value = retorno[2];
                }
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
                            <asp:Label ID="Label8" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();" 
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label6" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupo" runat="server" AutoPostBack="True" DataSourceID="odsGrupoProd"
                                DataTextField="Descricao" DataValueField="IdGrupoProd" OnSelectedIndexChanged="drpGrupo_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label7" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupo" runat="server" AutoPostBack="True" DataSourceID="odsSubgrupoProd"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd">
                            </asp:DropDownList>
                        </td>
                        <td><asp:Label ID="Label1" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label></td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Ativo</asp:ListItem>
                                <asp:ListItem Value="2">Inativo</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkApenasEstoqueFiscal" runat="server" Text="Apenas produtos com estoque fiscal"
                                Checked="True" AutoPostBack="True" />
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
                <asp:GridView GridLines="None" ID="grdEstoque" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsEstoque" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    PageSize="20">
                    <Columns>
                        <asp:BoundField DataField="CodInternoProd" HeaderText="Cód." 
                            SortExpression="CodInternoProd" />
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="NcmProd" HeaderText="NCM" SortExpression="NcmProd" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situacao" 
                            SortExpression="DescrSituacao" />
                        <asp:BoundField DataField="EstoqueFiscalUnidade" HeaderText="Qtd." SortExpression="EstoqueFiscalUnidade" />
                        <asp:BoundField DataField="CustoUnitProd" DataFormatString="{0:c}" HeaderText="Custo unit."
                            SortExpression="CustoUnitProd" />
                        <asp:BoundField DataField="ValorUnitProd" DataFormatString="{0:c}" HeaderText="Valor unit."
                            SortExpression="ValorUnitProd" />
                        <asp:BoundField DataField="CustoTotalProdFiscal" DataFormatString="{0:c}" HeaderText="Custo total"
                            SortExpression="CustoTotalProdFiscal" />
                        <asp:BoundField DataField="ValorTotalProdFiscal" DataFormatString="{0:c}" HeaderText="Valor total"
                            SortExpression="ValorTotalProdFiscal" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEstoque" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="GetForEstoque" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ProdutoLojaDAO" 
                    SelectCountMethod="GetForEstoqueCount" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInternoProd" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descricao" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupoProd" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSubgrupo" Name="idSubgrupoProd" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="chkApenasEstoqueFiscal" Name="exibirApenasComEstoque"
                            PropertyName="Checked" Type="Boolean" />
                        <asp:Parameter Name="exibirApenasPosseTerceiros" Type="Boolean" />
                        <asp:Parameter Name="exibirApenasProdutosProjeto" Type="Boolean" />
                        <asp:Parameter Name="idCorVidro" Type="UInt32" />
                        <asp:Parameter Name="idCorFerragem" Type="UInt32" />
                        <asp:Parameter Name="idCorAluminio" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" 
                            PropertyName="SelectedValue" Type="Int32" />
                        <asp:Parameter DefaultValue="1" Name="estoqueFiscal" Type="Int32" />
                        <asp:Parameter DefaultValue="false" Name="aguardandoSaidaEstoque" 
                            Type="Boolean" />
                        <asp:Parameter DefaultValue="1" Name="ordenacao" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoProd" runat="server" 
                    SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.GrupoProdDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupoProd" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupo" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false">
                    <img src="../Images/printer.png" border="0" /> Imprimir</asp:LinkButton>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" 
                    onclientclick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
