<%@ Page Title="Extrato de Estoque Fiscal" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstMovEstoqueFiscal.aspx.cs" Inherits="Glass.UI.Web.Listas.LstMovEstoqueFiscal" %>

<%@ Register Src="../Controls/ctrlSelCorProd.ascx" TagName="ctrlSelCorProd" TagPrefix="uc1" %>
<%@ Register src="../Controls/ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <style type="text/css">
        .rodapeGrid td { padding-bottom: 39px; }
    </style>
    
    <script type="text/javascript">

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
            var numeroNfe = FindControl("txtNumeroNfe", "input").value;
            var situacaoProd = FindControl("drpSituacaoProd", "select").value;
            var tipoMov = FindControl("drpTipoMov", "select").value;
            var cfop = FindControl("selCfop_hdfValor", "input").value;

            var grupoProd = FindControl("drpGrupoProd", "select").value;
            var subgrupoProd = FindControl("drpSubgrupoProd", "select").value;
            var controleCor = <%= ctrlSelCorProd1.ClientID %>;
            var corVidro = controleCor.idCorVidro();
            var corFerragem = controleCor.idCorFerragem();
            var corAluminio = controleCor.idCorAluminio();
            var lancManual = FindControl("chkLancManual", "input").checked;
            
            var naoBuscarEstoqueZero = FindControl("cboEstoque", "input").checked;
            var usarValorFiscal = FindControl("chkUsarValorFiscal", "input").checked;
            
            return "idLoja=" + idLoja + "&dataIni=" + dataIni + "&dataFim=" + dataFim +
                "&codInterno=" + codInterno + "&descricao=" + descrProd + 
                "&situacaoProd=" + situacaoProd + "&tipoMov=" + tipoMov +
                "&idCfop=" + cfop + "&numeroNfe=" + numeroNfe + "&idGrupoProd=" + grupoProd + 
                "&idSubgrupoProd=" + subgrupoProd + "&idCorVidro=" + corVidro + 
                "&idCorFerragem=" + corFerragem + "&idCorAluminio=" + corAluminio + 
                "&naoBuscarEstoqueZero=" + naoBuscarEstoqueZero + "&usarValorFiscal=" + usarValorFiscal + 
                "&lancManual=" + lancManual;
        }
        
        function onInsert(botao)
        {
            var celula = botao;
            while (celula.nodeName.toLowerCase() != "td")
                celula = celula.parentNode;
            
            var idProd = FindControl("hdfIdProd", "input").value;
            var idLoja = FindControl("hdfIdLoja", "input").value;
            
            FindControl("hdfIdProd", "input", celula).value = idProd;
            FindControl("hdfIdLoja", "input", celula).value = idLoja;
        }

        function openRpt(exportarExcel)
        {
            if (!validarQueryString())
                return false;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ExtratoEstoqueFiscal&" +
                getQueryString() + "&exportarExcel=" + exportarExcel);
        }

        function openRptMov(mov, comparativo, exportarExcel)
        {
            if (!validarQueryString())
                return false;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=MovEstoque" + (!mov ? "Total" : "") + (comparativo ? "Comparativo" : "") +
                "&fiscal=1&" + getQueryString() + "&exportarExcel=" + exportarExcel);
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

            FindControl("hdfIdProd", "input").value = resposta[1];
            FindControl("txtDescrProd", "input").value = resposta[2];
        }
        
        function mudaTipoMovIns(tipoMov)
        {
            var valor = FindControl("txtValor", "input");
            valor.disabled = tipoMov == 2;
            valor.value = tipoMov == 2 ? "(calculado)" : "";
        }

        function validarQueryString(){
            var dataIni = FindControl("ctrlDataIni_txtData", "input");
            var dataFim = FindControl("ctrlDataFim_txtData", "input");

            if (dataIni == null || dataIni == undefined || dataIni == "" || dataIni.value == ""){
                alert("Informe a data inicial.");
                return false;
            }

            if (dataFim == null || dataFim == undefined || dataFim == "" || dataFim.value == ""){
                alert("Informe a data final.");
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
                            <asp:Label ID="Label8" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Período Movimentação"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadOnly" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadOnly" ExibirHoras="False" />
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
                            <asp:Label ID="Label6" runat="server" ForeColor="#0066FF" Text="CFOP"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlSelPopup ID="selCfop" runat="server" 
                                ColunasExibirPopup="IdCfop|CodInterno|Descricao" DataSourceID="odsCfop" 
                                DataTextField="CodInterno" DataValueField="IdCfop" ExibirIdPopup="False" 
                                FazerPostBackBotaoPesquisar="True" PermitirVazio="True" TextWidth="50px" 
                                TitulosColunas="IdCfop|Cód.|Descrição" TituloTela="Selecione o CFOP" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" ForeColor="#0066FF" Text="Nota Fiscal"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumeroNfe" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupoProd" runat="server" DataSourceID="odsGrupoProd" DataTextField="Descricao"
                                DataValueField="IdGrupoProd">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupoProd" runat="server" DataSourceID="odsSubgrupoProd"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd">
                            </asp:DropDownList>
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
                            <asp:CheckBox ID="cboEstoque" runat="server" Text="Não Buscar Produtos com Estoque Zerado"
                                ForeColor="#0066FF" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            &nbsp;<asp:CheckBox ID="chkLancManual" runat="server" ForeColor="#0066FF" 
                                Text="Apenas Lançamentos Manuais" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkUsarValorFiscal" runat="server" ForeColor="#0066FF" Text="Usar Valor Fiscal do produto no inventário" />
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
                <asp:GridView GridLines="None" ID="grdMovEstoqueFiscal" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsMovEstoqueFiscal" PageSize="20" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum registro encontrado (Certifique-se de ter informado a loja e o produto)."
                    OnDataBound="grdMovEstoqueFiscal_DataBound" ShowFooter="True" 
                    DataKeyNames="IdMovEstoqueFiscal">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" 
                                    onclientclick="if (!confirm(&quot;Deseja excluir essa movimentação?&quot;)) return false" 
                                    Visible='<%# Eval("DeleteVisible") %>' />
                                <asp:HiddenField ID="hdfTipoMov" runat="server" Value='<%# Eval("TipoMov") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdMovEstoqueFiscal" HeaderText="Cód. Mov." SortExpression="IdMovEstoqueFiscal" />
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
                                <asp:TextBox ID="txtQtde" runat="server" Width="50px"
                                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
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
                                <asp:TextBox ID="txtValor" runat="server" Width="60px"
                                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
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
                                    Width="50px" ></asp:TextBox>                                                                
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc4:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="MovEstoqueFiscal" IdRegistro='<%# Eval("IdMovEstoqueFiscal") %>' />
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Eval("IdProd") %>' />
                                <asp:HiddenField ID="hdfIdLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" 
                                    onclick="imgAdd_Click" OnClientClick="onInsert(this)" 
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
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMovEstoqueFiscal" runat="server" MaximumRowsParameterName=""
                    SelectMethod="GetList" StartRowIndexParameterName="" 
                    TypeName="Glass.Data.DAL.MovEstoqueFiscalDAO"  
                    DataObjectTypeName="Glass.Data.Model.MovEstoqueFiscal" DeleteMethod="Delete">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInterno" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescrProd" Name="descricao" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNumeroNfe" Name="numeroNfe" PropertyName="Text" 
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpTipoMov" Name="tipoMov" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpSituacaoProd" Name="situacaoProd" 
                            PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="selCfop" Name="idCfop" PropertyName="Valor" 
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpGrupoProd" Name="idGrupoProd" 
                            PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSubgrupoProd" Name="idSubgrupoProd" 
                            PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorVidro"
                            PropertyName="IdCorVidro" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorFerragem" 
                            PropertyName="IdCorFerragem" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorAluminio" 
                            PropertyName="IdCorAluminio" Type="UInt32" />
                        <asp:ControlParameter ControlID="chkLancManual" Name="apenasLancManual" 
                            PropertyName="Checked" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoProd" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.GrupoProdDAO" >
                    <SelectParameters>
                        <asp:Parameter DefaultValue="true" Name="incluirTodos" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupoProd" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpGrupoProd" Name="idGrupo" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCfop" runat="server" 
                    SelectMethod="GetSortedByCodInterno" TypeName="Glass.Data.DAL.CfopDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkCanceladas" runat="server" OnClientClick="openLogCancelamento(); return false">
                    <img alt="" border="0" src="../Images/ExcluirGrid.gif" /> Movimentações Excluídas</asp:LinkButton>
                <br /><br />
                <table>
                    <tr>
                        <td align="right">
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false"><img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td align="left">
                            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                                src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:LinkButton ID="lnkImprimirMov" runat="server" OnClientClick="openRptMov(true, false, false); return false"><img src="../Images/Printer.png" border="0" /> Imprimir (Movimentação)</asp:LinkButton>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td align="left">
                            <asp:LinkButton ID="lnkExportarExcelMov" runat="server" OnClientClick="openRptMov(true, false, true); return false;"><img border="0" 
                                src="../Images/Excel.gif" /> Exportar para o Excel (Movimentação)</asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:LinkButton ID="lnkImprimirTotal" runat="server" OnClientClick="openRptMov(false, false, false); return false"><img src="../Images/Printer.png" border="0" /> Imprimir (Inventário)</asp:LinkButton>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td align="left">
                            <asp:LinkButton ID="lnkExportarExcelTotal" runat="server" OnClientClick="openRptMov(false, false, true); return false;"><img border="0" 
                                src="../Images/Excel.gif" /> Exportar para o Excel (Inventário)</asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:LinkButton ID="lnkImprimirTotalComparativo" runat="server" OnClientClick="openRptMov(false, true, false); return false"><img src="../Images/Printer.png" border="0" /> Imprimir (Inventário Comparativo)</asp:LinkButton>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td align="left">
                            <asp:LinkButton ID="lnkExportarExcelTotalComparativo" runat="server" OnClientClick="openRptMov(false, true, true); return false;"><img border="0" 
                                src="../Images/Excel.gif" /> Exportar para o Excel (Inventário Comparativo)</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
