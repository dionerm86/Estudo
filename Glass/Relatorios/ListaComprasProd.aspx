<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaComprasProd.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaComprasProd" Title="Compras de Produtos" %>

<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var idLoja = FindControl("drpLoja", "select").value;
            var idGrupo = FindControl("drpGrupo", "select").value;
            var idSubgrupo = FindControl("drpSubgrupo", "select").value;
            var codInterno = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescr", "input").value;
            var idFornec = FindControl("txtNumFornec", "input").value;
            var nomeFornec = FindControl("txtNome", "input").value;
            var dtIni = FindControl("ctrlDataIniSit_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFimSit_txtData", "input").value;
            var agruparGrupo = FindControl("chkAgruparGrupo", "input").checked ? "1" : "0";
            var agruparFornec = FindControl("chkAgruparFornec", "input").checked ? "1" : "0";
            var idFunc = FindControl("drpVendedor", "select").value;
            var tipoCfop = FindControl("cbdTipoCfop", "select").itens();
            var exibirDetalhes = FindControl("chkExibirDetalhes", "input").checked ? "1" : "0";
            var comSemNf = FindControl("cblNotaFiscal", "select").itens();
            
            idFornec = idFornec == "" ? 0 : idFornec;

            openWindow(600, 800, "RelBase.aspx?Rel=comprasProd&idLoja=" + idLoja + "&idFornec=" + idFornec + "&nomeFornec=" + nomeFornec +
                "&codInterno=" + codInterno + "&descrProd=" + descrProd + "&dtIni=" + dtIni + "&dtFim=" + dtFim + "&idGrupo=" + idGrupo +
                "&idSubgrupo=" + idSubgrupo + "&agruparFornec=" + agruparFornec + "&agruparGrupo=" + agruparGrupo + "&idFunc=" + idFunc + 
                "&exibirDetalhes=" + exibirDetalhes + "&tipoCfop=" + tipoCfop + "&comSemNf=" + comSemNf + "&exportarExcel=" + exportarExcel);

            return false;
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

        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq0" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupo" runat="server" AutoPostBack="True" DataSourceID="odsGrupo"
                                DataTextField="Descricao" DataValueField="IdGrupoProd">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupo" runat="server" AutoPostBack="True" DataSourceID="odsSubgrupo"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label12" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('lnkPesq3', 'a');"></asp:TextBox>
                        </td>
                        <td align="left">
                            <asp:LinkButton ID="lnkPesq3" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td align="left">
                            <asp:Label ID="Label7" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <uc3:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true"/>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblPeriodoSituacao" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td align="left">
                                        <uc2:ctrlData ID="ctrlDataIniSit" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                                    </td>
                                    <td>
                                        <uc2:ctrlData ID="ctrlDataFimSit" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                                    </td>
                                    <td>
                                        &nbsp;<asp:LinkButton ID="lnkPesq1" runat="server" OnClientClick="setProduto();"
                                            OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Vendedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedor" runat="server" DataSourceID="odsVendedor" DataTextField="Nome"
                                DataValueField="IdFunc" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Tipo de CFOP" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdTipoCfop" runat="server" CheckAll="False" DataSourceID="odsTipoCfop"
                                DataTextField="Descricao" DataValueField="IdTipoCfop" ImageURL="~/Images/DropDown.png"
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OpenOnStart="False"
                                Title="Selecione o tipo de CFOP">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Nota Fiscal" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblNotaFiscal" runat="server" CheckAll="True">
                                <asp:ListItem Value="1">Com NF gerada</asp:ListItem>
                                <asp:ListItem Value="2">Sem NF gerada</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkAgruparGrupo" runat="server" Text="Agrupar por grupo de produto" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparFornec" runat="server" Text="Agrupar por fornecedor"
                                AutoPostBack="True" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkExibirDetalhes" runat="server" Text="Exibir detalhes no relatório" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdComprasProd" runat="server" AllowPaging="True"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" PageSize="20" AutoGenerateColumns="False" DataSourceID="odsComprasProd">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="CodInterno" HeaderText="Cod." SortExpression="CodInterno" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:TemplateField HeaderText="Fornecedor" SortExpression="IdFornecComp">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("IdClienteVend") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("IdFornecComp") + " - " + Eval("NomeFornecComp") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="TotalQtde" HeaderText="Qtde" SortExpression="TotalQtde" />
                        <asp:BoundField DataField="TotalM2" HeaderText="Total M2" SortExpression="TotalM2" />
                        <asp:BoundField DataField="TotalCusto" DataFormatString="{0:C}" HeaderText="Custo Total"
                            SortExpression="TotalCusto" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Eval("ImagemUrl") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsComprasProd" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetListaComprasProdCount" SelectMethod="GetListaComprasProd"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutoDAO"
                    >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumFornec" Name="idFornec" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeFornec" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupo" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSubgrupo" Name="idSubgrupo" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInterno" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descrProd" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniSit" Name="dtIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimSit" Name="dtFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpVendedor" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="chkAgruparFornec" Name="agruparFornecedor" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="cbdTipoCfop" Name="tipoCfop" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cblNotaFiscal" Name="comSemNf" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupo" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSituacao" runat="server" SelectMethod="GetSituacaoPedidoFiltro"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupo" runat="server" SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.GrupoProdDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVendedor" runat="server" SelectMethod="GetVendedoresComissao"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoVenda" runat="server" SelectMethod="GetTipoVenda"
                    TypeName="Glass.Data.Helper.DataSources">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="false" Name="incluirVazio" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCfop" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.TipoCfopDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="setProduto(); return openRpt();">
                <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
