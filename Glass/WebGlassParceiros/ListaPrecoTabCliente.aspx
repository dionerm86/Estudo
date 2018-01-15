<%@ Page Title="Preços de Tabela" Language="C#" MasterPageFile="~/WebGlassParceiros/PainelParceiros.master"
    AutoEventWireup="true" CodeBehind="ListaPrecoTabCliente.aspx.cs" Inherits="Glass.UI.Web.WebGlassParceiros.ListaPrecoTabCliente" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

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
                            <asp:Label ID="Label6" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();"></asp:TextBox>
                            <asp:TextBox ID="txtDescr" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq0" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSubgrupo" runat="server" CheckAll="False" DataSourceID="odsSubgrupoProd"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd" ImageURL="~/Images/DropDown.png"
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OpenOnStart="False"
                                Title="Selecione o subgrupo" OnDataBound="cbdSubgrupo_DataBound">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:Label ID="lblTipoValor" runat="server" Text="Tipo Valor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoValor" runat="server">
                                <asp:ListItem Value="0">Padrão</asp:ListItem>
                                <asp:ListItem Value="1">Atacado</asp:ListItem>
                                <asp:ListItem Value="2">Balcão</asp:ListItem>
                                <asp:ListItem Value="3">Obra</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq2" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>
                    </table>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label7" runat="server" Text="Altura" ForeColor="#0066FF"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtAlturaInicio" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);">
                                </asp:TextBox>
                                <asp:Label runat="server" Text="Até"></asp:Label>
                                <asp:TextBox ID="txtAlturaFim" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);">
                                </asp:TextBox>
                                <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                            </td>
                            <td></td>
                            <td>
                                <asp:Label ID="Label8" runat="server" Text="Largura" ForeColor="#0066FF"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtLarguraInicio" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);">
                                </asp:TextBox>
                                <asp:Label runat="server" Text="Até"></asp:Label>
                                <asp:TextBox ID="txtLarguraFim" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);">
                                </asp:TextBox>
                                <asp:LinkButton ID="LinkButton3" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenacao" runat="server">
                                <asp:ListItem Value="0">Cód. Produto</asp:ListItem>
                                <asp:ListItem Value="1">Descr. Produto</asp:ListItem>
                                <asp:ListItem Value="2">Grupo</asp:ListItem>
                                <asp:ListItem Value="3">Subgrupo</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
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
                <asp:GridView ID="grdProduto" runat="server" AllowPaging="True" AllowSorting="True"
                    CssClass="gridStyle" GridLines="None" AutoGenerateColumns="False" DataSourceID="odsProduto"
                    EmptyDataText="Selecione um cliente">
                    <Columns>
                        <asp:BoundField DataField="CodInterno" HeaderText="Cód." SortExpression="CodInterno" />
                        <asp:BoundField DataField="DescricaoProdutoBeneficiamento" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:TemplateField HeaderText="Grupo" SortExpression="DescrGrupo">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("DescrGrupo") + (Eval("DescrSubgrupo") != null && Eval("DescrSubgrupo") != "" ? " " + Eval("DescrSubgrupo") : "") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrGrupo") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="TituloValorTabelaUtilizado" HeaderText="Tipo de Valor de Tabela"
                            SortExpression="TituloValorTabelaUtilizado" />
                        <asp:BoundField DataField="ValorTabelaUtilizado" DataFormatString="{0:c}" HeaderText="Valor de Tabela"
                            SortExpression="ValorTabelaUtilizado" />
                        <asp:BoundField DataField="Altura" HeaderText="Altura"
                            SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura"
                            SortExpression="Largura" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
               <%-- <asp:LinkButton ID="lkbImprimir" runat="server" OnClientClick="openRpt(false); return false">
                    <img src="../Images/Printer.png" border="0" /> Imprimir
                </asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>--%>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProduto" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountPrecoTab" SelectMethod="GetListPrecoTab" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutoDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfIdCli" Name="idCliente" PropertyName="Value" />
                        <asp:Parameter Name="nomeCliente" />
                        <asp:Parameter Name="idGrupoProd" />
                        <asp:ControlParameter ControlID="cbdSubgrupo" Name="idsSubgrupoProd" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInterno" PropertyName="Text" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descrProduto" PropertyName="Text" />
                        <asp:ControlParameter ControlID="drpTipoValor" Name="tipoValor" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="txtAlturaInicio" Name="alturaInicio" PropertyName="Text" />
                        <asp:ControlParameter ControlID="txtAlturaFim" Name="alturaFim" PropertyName="Text" />
                        <asp:ControlParameter ControlID="txtLArguraInicio" Name="larguraInicio" PropertyName="Text" />
                        <asp:ControlParameter ControlID="txtLArguraFim" Name="larguraFim" PropertyName="Text" />
                        <asp:ControlParameter ControlID="drpOrdenacao" Name="ordenacao" PropertyName="SelectedValue" />
                        <asp:Parameter Name="produtoDesconto" DefaultValue="false" />
                        <asp:Parameter  Name="ecommerce" DefaultValue="true" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupoProd" runat="server" SelectMethod="GetForTabelaClienteEcommerce"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfIdCli" Name="idCli" PropertyName="Value" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="hdfIdCli" />
</asp:Content>
