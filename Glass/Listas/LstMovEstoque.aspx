<%@ Page Title="Extrato de Estoque" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstMovEstoque.aspx.cs" Inherits="Glass.UI.Web.Listas.LstMovEstoque" %>

<%@ Register Src="../Controls/ctrlSelCorProd.ascx" TagName="ctrlSelCorProd" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <style type="text/css">
        .rodapeGrid td {
            padding-bottom: 39px;
        }
    </style>

    <script type="text/javascript">
        var click = false;

        function openLogCancelamento()
        {
            openWindow(600, 800, '../Utils/ShowLogCancelamento.aspx?tabela=<%= GetCodigoTabela() %>');
        }
        
        function getQueryString()
        {
            var idLoja = FindControl("drpLoja", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            
            var codInterno = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescrProd", "input").value;
            var situacaoProd = FindControl("drpSituacaoProd", "select").value;
            var codOtimizacao = FindControl("txtCodOtimizacao", "input").value;
            var tipoMov = FindControl("drpTipoMov", "select").value;

            var grupoProd = FindControl("cbdGrupoProd", "select").itens();
            var subgrupoProd = FindControl("cbdSubgrupoProd", "select").itens();
            var controleCor = <%= ctrlSelCorProd1.ClientID %>;
            var corVidro = controleCor.idCorVidro();
            var corFerragem = controleCor.idCorFerragem();
            var corAluminio = controleCor.idCorAluminio();
            var naoBuscarEstoqueZero = FindControl("chkNaoBuscarEstoqueZero", "input").checked;
            var lancManual = FindControl("chkLancManual", "input").checked;
            var usarValorFiscal = FindControl("chkUsarValorFiscal", "input").checked;
            
            return "idLoja=" + idLoja + "&dataIni=" + dataIni + "&dataFim=" + dataFim +
                "&codInterno=" + codInterno + "&descricao=" + descrProd + 
                "&situacaoProd=" + situacaoProd + "&codOtimizacao=" + codOtimizacao + "&tipoMov=" + tipoMov +
                "&idsGrupoProd=" + grupoProd + "&idSubgrupoProd=" + subgrupoProd +
                "&idCorVidro=" + corVidro + "&idCorFerragem=" + corFerragem +
                "&idCorAluminio=" + corAluminio + "&usarValorFiscal=" + usarValorFiscal +
                "&naoBuscarEstoqueZero=" + naoBuscarEstoqueZero + "&lancManual=" + lancManual;
        }
        
        function onInsert(botao)
        {
            if (click)
                return false;

            var celula = botao;
            while (celula.nodeName.toLowerCase() != "td")
                celula = celula.parentNode;
            
            var idProd = FindControl("hdfIdProd", "input").value;
            var idLoja = FindControl("hdfIdLoja", "input").value;
            
            FindControl("hdfIdProd", "input", celula).value = idProd;
            FindControl("hdfIdLoja", "input", celula).value = idLoja;

            click = true;
        }

        function openRpt(exportarExcel)
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ExtratoEstoque&" +
                getQueryString() + "&exportarExcel=" + exportarExcel);
        }

        function openRptMov(mov, exportarExcel)
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=MovEstoque" + (!mov ? "Total" : "") + "&" +
                getQueryString() + "&exportarExcel=" + exportarExcel);
        }

        function getProduto()
        {
            var codInterno = FindControl("txtCodProd", "input").value;
            var resposta = MetodosAjax.GetProd(codInterno).value.split(';');

            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                return;
            }

            FindControl("txtDescrProd", "input").value = resposta[2];
        }
        
        function mudaTipoMovIns(tipoMov)
        {
            var valor = FindControl("txtValor", "input");
            valor.disabled = tipoMov == 2;
            valor.value = tipoMov == 2 ? "(calculado)" : "";
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
                            <uc5:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="True" MostrarTodas="false" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" ForeColor="#0066FF" Text="Tipo"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoMov" runat="server">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Entradas</asp:ListItem>
                                <asp:ListItem Value="2">Saídas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Período Movimentação"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Style="width: 16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Produto"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="50px" onblur="getProduto()" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null)"></asp:TextBox>
                            <asp:TextBox ID="txtDescrProd" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" ForeColor="#0066FF" Text="Cod. Otimização"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodOtimizacao" runat="server" Width="80px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacaoProd" runat="server">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Ativo</asp:ListItem>
                                <asp:ListItem Value="2">Inativo</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdGrupoProd" runat="server" CheckAll="False" DataSourceID="odsGrupoProd"
                                DataTextField="Descricao" DataValueField="IdGrupoProd" Title="Selecione o grupo">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSubgrupoProd" runat="server" CheckAll="True" DataSourceID="odsSubgrupoProd"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd" Title="Selecione o Subgrupo" OnDataBound="cbdSubgrupoProd_DataBound">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlSelCorProd ID="ctrlSelCorProd1" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkNaoBuscarEstoqueZero" runat="server" ForeColor="#0066FF"
                                Text="Não Buscar Produtos com Estoque Zerado" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkLancManual" runat="server" ForeColor="#0066FF"
                                Text="Apenas Lançamentos Manuais" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkUsarValorFiscal" runat="server" ForeColor="#0066FF" Text="Usar Valor Fiscal do produto no inventário" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdMovEstoque" runat="server" AutoGenerateColumns="False" EnableViewState="false"
                    DataSourceID="odsMovEstoque" PageSize="20" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum registro encontrado (Certifique-se de ter informado a loja e o produto)."
                    OnDataBound="grdMovEstoque_DataBound" ShowFooter="True"
                    DataKeyNames="IdMovEstoque">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete"
                                    ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir essa movimentação?&quot;)) return false"
                                    Visible='<%# Eval("DeleteVisible") %>' />
                                <asp:HiddenField ID="hdfTipoMov" runat="server" Value='<%# Eval("TipoMov") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdMovEstoque" HeaderText="Cód. Mov." SortExpression="IdMovEstoque" />
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia" />
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:TemplateField HeaderText="Data" SortExpression="DataMov">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DataMov") %>'></asp:Label>
                                <asp:Label ID="Label11" runat="server"
                                    Text='<%# "<br />(Data Cad. " + Eval("DataCad") + ")" %>'
                                    Visible='<%# Eval("DataCad") != null %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DataMov") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc3:ctrlData ID="ctrlDataMov" runat="server" ExibirHoras="True"
                                    ReadOnly="ReadWrite" ValidateEmptyText="True" ValidationGroup="insert" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde." SortExpression="QtdeMov">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("QtdeMov") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("QtdeMov") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtQtde" runat="server"
                                    onkeypress="return soNumeros(event, false, true)" Width="50px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvQtde" runat="server"
                                    ControlToValidate="txtQtde" Display="Dynamic" ErrorMessage="*"
                                    ValidationGroup="insert"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CodUnidade" HeaderText="Unidade"
                            SortExpression="CodUnidade" />
                        <asp:BoundField DataField="SaldoQtdeMov" HeaderText="Saldo" SortExpression="SaldoQtdeMov" />
                        <asp:TemplateField HeaderText="E/S" SortExpression="DescrTipoMov">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("DescrTipoMov") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("DescrTipoMov") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" onchange="mudaTipoMovIns(this.value)">
                                    <asp:ListItem></asp:ListItem>
                                    <asp:ListItem Value="1">E</asp:ListItem>
                                    <asp:ListItem Value="2">S</asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvTipo" runat="server"
                                    ControlToValidate="drpTipo" Display="Dynamic" ErrorMessage="*"
                                    ValidationGroup="insert"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor (Total)" SortExpression="ValorMov">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("ValorMov", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValor" runat="server"
                                    Width="60px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvValor" runat="server"
                                    ControlToValidate="txtValor" Display="Dynamic" ErrorMessage="*"
                                    ValidationGroup="insert"></asp:RequiredFieldValidator>
                                <div style="position: absolute">
                                    <asp:Label ID="Label12" runat="server" Text="valor total da movimentação<br />(o valor unitário é calculado com<br />base nesse valor)" ForeColor="Red"></asp:Label>
                                </div>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="SaldoValorMov" DataFormatString="{0:c}"
                            HeaderText="Valor Acumulado" SortExpression="SaldoValorMov" />
                        <asp:TemplateField HeaderText="Obs." SortExpression="Obs">
                            <ItemTemplate>
                                <asp:Label ID="lblObs" runat="server" Text='<%# Eval("Obs") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtObs" runat="server"
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc4:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="MovEstoque" IdRegistro='<%# Eval("IdMovEstoque") %>' />
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Eval("IdProd") %>' />
                                <asp:HiddenField ID="hdfIdLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif"
                                    OnClick="imgAdd_Click" OnClientClick="onInsert(this)"
                                    ValidationGroup="insert" />
                                <asp:HiddenField ID="hdfIdProd" runat="server" />
                                <asp:HiddenField ID="hdfIdLoja" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle CssClass="rodapeGrid" />
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsMovEstoque" runat="server" MaximumRowsParameterName=""
                    SelectMethod="GetList" StartRowIndexParameterName="" EnableViewState="false"
                    TypeName="Glass.Data.DAL.MovEstoqueDAO"
                    DataObjectTypeName="Glass.Data.Model.MovEstoque" DeleteMethod="DeleteComTransacao">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInterno" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescrProd" Name="descricao" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpTipoMov" Name="tipoMov" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpSituacaoProd" Name="situacaoProd"
                            PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="cbdGrupoProd" Name="idsGrupoProd" 
                            PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="cbdSubgrupoProd" Name="idsSubgrupoProd"
                            PropertyName="SelectedValue" Type="string" />
                        <asp:ControlParameter ControlID="txtCodOtimizacao" Name="codOtimizacao" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorVidro"
                            PropertyName="IdCorVidro" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorFerragem"
                            PropertyName="IdCorFerragem" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorAluminio"
                            PropertyName="IdCorAluminio" Type="UInt32" />
                        <asp:ControlParameter ControlID="chkNaoBuscarEstoqueZero" Name="naoBuscarEstoqueZero" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="chkLancManual" Name="apenasLancManual"
                            PropertyName="Checked" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGrupoProd" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.GrupoProdDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="true" Name="incluirTodos" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubgrupoProd" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="cbdGrupoProd" Name="idGrupos" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkCanceladas" runat="server" OnClientClick="openLogCancelamento(); return false"> <img alt="" border="0" src="../Images/ExcluirGrid.gif" /> Movimentações Excluídas</asp:LinkButton>
                <br />
                <br />
                <table>
                    <tr>
                        <td align="right">
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false"><img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                        </td>
                        <td>&nbsp;
                        </td>
                        <td align="left">
                            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                                src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:LinkButton ID="lnkImprimirMov" runat="server" OnClientClick="openRptMov(true, false); return false"><img src="../Images/Printer.png" border="0" /> Imprimir (Movimentação)</asp:LinkButton>
                        </td>
                        <td>&nbsp;
                        </td>
                        <td align="left">
                            <asp:LinkButton ID="lnkExportarExcelMov" runat="server" OnClientClick="openRptMov(true, true); return false;"><img border="0" 
                                src="../Images/Excel.gif" /> Exportar para o Excel (Movimentação)</asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:LinkButton ID="lnkImprimirTotal" runat="server" OnClientClick="openRptMov(false, false); return false"><img src="../Images/Printer.png" border="0" /> Imprimir (Inventário)</asp:LinkButton>
                        </td>
                        <td>&nbsp;
                        </td>
                        <td align="left">
                            <asp:LinkButton ID="lnkExportarExcelTotal" runat="server" OnClientClick="openRptMov(false, true); return false;"><img border="0" 
                                src="../Images/Excel.gif" /> Exportar para o Excel (Inventário)</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
