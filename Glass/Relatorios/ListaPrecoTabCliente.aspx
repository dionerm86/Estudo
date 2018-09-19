<%@ Page Title="Preços de Tabela por Cliente" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ListaPrecoTabCliente.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaPrecoTabCliente" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(exportarExcel) {
            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var idGrupo = FindControl("drpGrupo", "select").value;
            var idsSubgrupo = FindControl("cbdSubgrupo", "select").itens();
            var codInterno = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescr", "input").value;
            var tipoValor = FindControl("drpTipoValor", "select").value;
            var ordenacao = FindControl("drpOrdenacao", "select").value;
            var exibirPerc = FindControl("chkExibirPerc", "input").checked;
            var alturaInicio = FindControl("txtAlturaInicio", "input").value;
            var alturaFim = FindControl("txtAlturaFim", "input").value;
            var larguraInicio = FindControl("txtLarguraInicio", "input").value;
            var larguraFim = FindControl("txtLarguraFim", "input").value;
            var produtoDesconto = FindControl("chkProdutoDesconto", "input").checked;
            var incluirBeneficiamento = FindControl("chkIncluirBeneficiamento", "input").checked;
            var exibirValorOriginal = FindControl("chkExibirValorOriginal", "input").checked;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=PrecoTabCliente&idCli=" + idCli + "&nomeCli=" + nomeCli + "&idGrupo=" + idGrupo +
                "&idsSubgrupo=" + idsSubgrupo + "&codInterno=" + codInterno + "&descrProd=" + descrProd + "&tipoValor=" + tipoValor +
                "&alturaInicio=" + alturaInicio + "&alturaFim=" + alturaFim + "&larguraInicio=" + larguraInicio + "&larguraFim=" + larguraFim +
                "&ordenacao=" + ordenacao + "&exibirPerc=" + exibirPerc + "&exportarExcel=" + exportarExcel + "&produtoDesconto=" + produtoDesconto +
                "&incluirBeneficiamento=" + incluirBeneficiamento + "&exibirValorOriginal=" + exibirValorOriginal);
        }

        function getCli(idCli) {
            if (idCli.value == "") {
                openWindow(570, 760, '../Utils/SelCliente.aspx');
                return false;
            }

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            var ids = ListaPrecoTabCliente.ObtemIdsSubGrupoCliente(idCli.value).value;
            var cdbSubgrupo = FindControl("cbdSubgrupo", "select");
            if(cdbSubgrupo != null)
                cdbSubgrupo.itens(ids);
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
                            <asp:Label ID="Label3" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                    OnClientClick="return getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <div id="filtros" runat="server">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupo" runat="server" AutoPostBack="True" DataSourceID="odsGrupoProd"
                                DataTextField="Descricao" DataValueField="IdGrupoProd" OnDataBound="drpGrupo_DataBound">
                            </asp:DropDownList>
                        </td>
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
                                <asp:Label ID="Label6" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();"></asp:TextBox>
                                <asp:TextBox ID="txtDescr" runat="server" Width="150px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:LinkButton ID="lnkPesq0" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                            </td>
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
                        <td>
                            <asp:CheckBox ID="chkExibirPerc" runat="server" Checked="True" Text="Exibir percentual de desconto/ascréscimo" />
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq1" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>                        
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                             <asp:CheckBox ID="chkProdutoDesconto" runat="server" Checked="false" Text="Exibir apenas produtos com desconto" />
                        </td>
                        <td>
                             <asp:CheckBox ID="chkIncluirBeneficiamento" runat="server" Checked="false" Text="Incluir beneficiamentos no relatorio" />
                        </td>
                        <td>
                             <asp:CheckBox ID="chkExibirValorOriginal" runat="server" Checked="true"  OnCheckedChanged="grdProduto_DataBound" Text="Não exibir a coluna Valor Original"/>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" align="center">
                            <asp:Button ID="btnFiltrarNovoCliente" OnClick="btnFiltrarNovoCliente_Click" runat="server" Text="Filtrar novo cliente" />
                        </td>
                    </tr>
                </table>
            </div>
            </td>
            <td>
                
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdProduto" runat="server" AllowPaging="True" AllowSorting="True" OnDataBound="grdProduto_DataBound"
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
                        <asp:BoundField DataField="ValorOriginalUtilizado" DataFormatString="{0:c}" HeaderText="Valor Original" 
                            SortExpression="ValorOriginalUtilizado" />
                        <asp:BoundField DataField="ValorTabelaUtilizado" DataFormatString="{0:c}" HeaderText="Valor de Tabela"
                            SortExpression="ValorTabelaUtilizado" />
                        <asp:BoundField DataField="DescrPercDescAcrescimo" HeaderText="Desconto/Acréscimo" />
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
                <asp:LinkButton ID="lkbImprimir" runat="server" OnClientClick="openRpt(false); return false">
                    <img src="../Images/Printer.png" border="0" /> Imprimir
                </asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProduto" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountPrecoTab" SelectMethod="GetListPrecoTab" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutoDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeCliente" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupoProd" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="cbdSubgrupo" Name="idsSubgrupoProd" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInterno" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descrProduto" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpTipoValor" Name="tipoValor" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtAlturaInicio" Name="alturaInicio" PropertyName="Text"
                            Type="Decimal" />
                        <asp:ControlParameter ControlID="txtAlturaFim" Name="alturaFim" PropertyName="Text"
                            Type="Decimal" />
                        <asp:ControlParameter ControlID="txtLArguraInicio" Name="larguraInicio" PropertyName="Text"
                            Type="Decimal" />
                        <asp:ControlParameter ControlID="txtLArguraFim" Name="larguraFim" PropertyName="Text"
                            Type="Decimal" />
                        <asp:ControlParameter ControlID="drpOrdenacao" Name="ordenacao" 
                            PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="chkProdutoDesconto" Name="produtoDesconto" 
                            PropertyName="Checked" Type="Boolean" />
                        <asp:Parameter  Name="ecommerce" DefaultValue="false" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoProd" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.GrupoProdDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupoProd" runat="server" SelectMethod="GetForTabelaCliente"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupo" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
