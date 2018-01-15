<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaProducaoProd.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaProducaoProd" Title="Produtos Produção" %>

<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel, agrupar) {
            var idLoja = FindControl("drpLoja", "select").value;
            var idsGrupos = FindControl("cbdGrupo", "select").itens();
            var situacaoProd = FindControl("cbdSituacaoProd", "select").itens();
            var idSubgrupo = FindControl("drpSubgrupo", "select").value;
            var codInterno = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescr", "input").value;
            var dtIni = FindControl("ctrlDataIniSit_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFimSit_txtData", "input").value;
            var dtIniPed = FindControl("ctrlDataIniPed_txtData", "input").value;
            var dtFimPed = FindControl("ctrlDataFimPed_txtData", "input").value;
            var dtIniEnt = FindControl("ctrlDataIniEnt_txtData", "input").value;
            var dtFimEnt = FindControl("ctrlDataFimEnt_txtData", "input").value;
            var situacao = FindControl("cbdSituacao", "select").itens();
            var agruparCorEsp = FindControl("chkAgruparCorEsp", "input").checked ? "1" : "0";
            var agruparGrupo = FindControl("chkAgruparGrupo", "input").checked ? "1" : "0";
            var agruparPedido = FindControl("chkAgruparPedido", "input").checked ? "1" : "0";
            var idFuncCliente = FindControl("drpVendedor", "select");
            idFuncCliente = idFuncCliente != null ? idFuncCliente.value : "0";
            var tipoPedido = FindControl("cbdTipoPedido", "select").itens();
            var tipoFastDelivery = FindControl("drpFastDelivery", "select").value;
            var incluirMateriaPrima = FindControl("chkMateriaPrima", "input").checked ? "1" : "0";
            var idPedido = FindControl("txtNumPedido", "input").value;
                        
            agrupar = agrupar == true;
            openWindow(600, 800, "RelBase.aspx?Rel=producaoProd&idLoja=" + idLoja + "&codInterno=" + codInterno + "&descrProd=" + descrProd +
                "&dtIni=" + dtIni + "&dtFim=" + dtFim + "&idsGrupos=" + idsGrupos + "&idSubgrupo=" + idSubgrupo + "&dtIniPed=" + dtIniPed +
                "&dtFimPed=" + dtFimPed + "&dtIniEnt=" + dtIniEnt + "&dtFimEnt=" + dtFimEnt + "&situacao=" + situacao + "&situacaoProd=" + situacaoProd +
                "&agruparGrupo=" + agruparGrupo + "&idFuncCliente=" + idFuncCliente + "&exportarExcel=" + exportarExcel + "&tipoFastDelivery=" + tipoFastDelivery +
                "&tipoPedido=" + tipoPedido + "&idPedido=" + idPedido + "&agruparPedido=" + agruparPedido + "&incluirMateriaPrima=" + 
                incluirMateriaPrima + "&agrupar=" + agrupar + "&agruparCorEsp=" + agruparCorEsp);

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
        
        function limparFiltros() {
            FindControl("drpLoja", "select").selectedIndex = 0;
            FindControl("drpGrupo", "select").selectedIndex = 0;
            FindControl("drpSubgrupo", "select").selectedIndex = 0;
            FindControl("txtCodProd", "input").value = "";
            FindControl("txtDescr", "input").value = "";
            FindControl("txtNumPedido", "input").value = "";
            FindControl("ctrlDataIniSit_txtData", "input").value = "";
            FindControl("ctrlDataFimSit_txtData", "input").value = "";
            FindControl("ctrlDataIniPed_txtData", "input").value = "";
            FindControl("ctrlDataFimPed_txtData", "input").value = "";
            FindControl("ctrlDataIniEnt_txtData", "input").value = "";
            FindControl("ctrlDataFimEnt_txtData", "input").value = "";
            FindControl("cbdSituacao", "select").itens();
            FindControl("chkAgruparGrupo", "input").checked = false;
            if (FindControl("drpVendedor", "select") != null) FindControl("drpVendedor", "select").selectedIndex = 0;
            FindControl("drpTipoPedido", "select").selectedIndex = 0;
            FindControl("drpFastDelivery", "select").selectedIndex = 0;
            return false;
        }

    </script>

    <table style="width: 100%">
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
                            <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                                <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkMateriaPrima" runat="server" Text="Buscar matéria-prima" />
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq0" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                                <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdGrupo" runat="server" CheckAll="False" 
                                DataSourceID="odsGrupo" DataTextField="Descricao" DataValueField="IdGrupoProd" 
                                ImageURL="~/Images/DropDown.png" 
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" 
                                OpenOnStart="False" Title="Selecione o grupo">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupo" runat="server" AutoPostBack="True" DataSourceID="odsSubgrupo"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd" 
                                ondatabound="drpSubgrupo_DataBound">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label7" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True" AutoPostBack="True">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Período (Pedido)" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniPed" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimPed" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq2" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq5" runat="server" OnClientClick="setProduto();" 
                                OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblSituacao" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSituacao" runat="server"
                                DataSourceID="odsSituacao" DataTextField="Descr" DataValueField="Id" 
                                Title="Selecione a situação" CheckAll="False" 
                                ondatabound="cbdSituacao_DataBound">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" onclick="imgPesq_Click" />
                        </td>
                        <td align="left">
                            <asp:Label ID="lblPeriodoSituacao" runat="server" Text="Período (Situação)" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td align="left">
                            <uc2:ctrlData ID="ctrlDataIniSit" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                            <uc2:ctrlData ID="ctrlDataFimSit" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        &nbsp;<asp:LinkButton ID="lnkPesq1" runat="server" OnClientClick="setProduto();"
                                            OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <asp:Label ID="lblFastDelivery" runat="server" Text="Fast Delivery" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFastDelivery" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Sim</asp:ListItem>
                                <asp:ListItem Value="2">Não</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblSituacaoProd" runat="server" ForeColor="#0066FF" Text="Situação Prod."></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSituacaoProd" runat="server" Title="Selecione a situação"
                                OpenOnStart="False" AltRowColor="" DataSourceID="odsSituacaoProd" DataTextField="Descr"
                                DataValueField="Id" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                ToolTip="Pesquisar" onclick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Tipo de pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <%--<asp:DropDownList ID="drpTipoVenda" runat="server" AppendDataBoundItems="True" 
                                DataSourceID="odsTipoVenda" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>--%>
                            
                            <sync:CheckBoxListDropDown ID="cbdTipoPedido" runat="server"
                                DataSourceID="odsTipoPedido" DataTextField="Descr" DataValueField="Id" 
                                Title="Selecione o tipo de pedido" CheckAll="False" 
                                AppendDataBoundItems="true" ImageURL="~/Images/DropDown.png" 
                                OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Período (Entrega)" 
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniEnt" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimEnt" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq4" runat="server" OnClientClick="setProduto();" 
                                OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Vendedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedor" runat="server" DataSourceID="odsVendedor" DataTextField="Nome"
                                DataValueField="IdFunc" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkLimparFiltros" runat="server" OnClientClick="return limparFiltros();"><img border="0" 
                                src="../Images/ExcluirGrid.gif" /> Limpar filtros</asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkAgruparPedido" runat="server" AutoPostBack="True" 
                                Text="Agrupar por pedido" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparGrupo" runat="server" Text="Agrupar por grupo de produto" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparCorEsp" runat="server" Text="Agrupar por cor/espessura" />
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
                <asp:GridView GridLines="None" ID="grdVendasProd" runat="server" AllowPaging="True" 
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" PageSize="20" AutoGenerateColumns="False" 
                    DataSourceID="odsProducaoProd" EmptyDataText="Nenhum registro encontrado">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" 
                            SortExpression="IdPedido" />
                        <asp:BoundField DataField="CodInterno" HeaderText="Cod." SortExpression="CodInterno" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="TotalQtde" HeaderText="Qtde" SortExpression="TotalQtde" />
                        <asp:BoundField DataField="TotalM2ML" HeaderText="Total M2/ML" SortExpression="TotalM2ML" />
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
                <sync:ObjectDataSource ID="odsProducaoProd" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetListaProducaoProdCount" SelectMethod="GetListaProducaoProd"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.ProdutoDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbdGrupo" Name="idsGrupos" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSubgrupo" Name="idSubgrupo" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInterno" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descrProd" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="chkMateriaPrima" Name="incluirMateriaPrima" 
                            PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="ctrlDataIniSit" Name="dtIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimSit" Name="dtFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniPed" Name="dtIniPed" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimPed" Name="dtFimPed" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniEnt" Name="dtIniEnt" 
                            PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimEnt" Name="dtFimEnt" 
                            PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="cbdSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbdSituacaoProd" Name="situacaoProd" 
                            PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="cbdTipoPedido" Name="tipoPedido" 
                            PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="drpVendedor" Name="idFuncCliente" 
                            PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFastDelivery" Name="tipoFastDelivery" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" 
                            PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="chkAgruparPedido" Name="agruparPedido" 
                            PropertyName="Checked" Type="Boolean" />
                    </SelectParameters>
                </sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="cbdGrupo" Name="idGrupos" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsSituacao" runat="server" SelectMethod="GetSituacaoPedido"
                    TypeName="Glass.Data.Helper.DataSources"></sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsGrupo" runat="server" SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.GrupoProdDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="false" Name="incluirTodos" Type="Boolean" />
                    </SelectParameters>
                </sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsVendedor" runat="server" SelectMethod="GetVendedoresComissao"
                    TypeName="Glass.Data.DAL.FuncionarioDAO"></sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsTipoPedido" runat="server" 
                    SelectMethod="GetTipoPedidoFilter" TypeName="Glass.Data.Helper.DataSources" 
                    >
                </sync:ObjectDataSource>
                <sync:ObjectDataSource ID="odsSituacaoProd" runat="server" SelectMethod="GetSituacaoProducao"
                    TypeName="Glass.Data.Helper.DataSources"></sync:ObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="setProduto(); return openRpt(false, false);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true, false); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                <br />
                <asp:LinkButton ID="lnkImprimirGrupo" runat="server" OnClientClick="setProduto(); return openRpt(false, true);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir (apenas grupos)</asp:LinkButton>
                    <br />
                
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
