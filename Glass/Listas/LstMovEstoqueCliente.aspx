<%@ Page Title="Extrato de Estoque de Cliente" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstMovEstoqueCliente.aspx.cs" Inherits="Glass.UI.Web.Listas.LstMovEstoqueCliente" %>

<%@ Register Src="../Controls/ctrlSelCorProd.ascx" TagName="ctrlSelCorProd" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register src="../Controls/ctrlData.ascx" tagname="ctrlData" tagprefix="uc3" %>
<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc4" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc5" %>
<%@ Register src="../Controls/ctrlSelCliente.ascx" tagname="ctrlSelCliente" tagprefix="uc6" %>
<%@ Register src="../Controls/ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc7" %>
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
            var idCliente = FindControl("ctrlSelCliente_ctrlSelClienteBuscar_hdfValor", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            
            var codInterno = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescrProd", "input").value;
            var situacaoProd = FindControl("drpSituacaoProd", "select").value;
            var tipoMov = FindControl("drpTipoMov", "select").value;
            var cfop = FindControl("selCfop_hdfValor", "input").value;

            var grupoProd = FindControl("drpGrupoProd", "select").value;
            var subgrupoProd = FindControl("drpSubgrupoProd", "select").value;
            var controleCor = <%= ctrlSelCorProd1.ClientID %>;
            var corVidro = controleCor.idCorVidro();
            var corFerragem = controleCor.idCorFerragem();
            var corAluminio = controleCor.idCorAluminio();
            
            return "idCliente=" + idCliente + "&idLoja=" + idLoja + "&dataIni=" + dataIni + 
                "&dataFim=" + dataFim + "&codInterno=" + codInterno + "&descricao=" + descrProd + 
                "&situacaoProd=" + situacaoProd + "&tipoMov=" + tipoMov +
                "&idGrupoProd=" + grupoProd + "&idSubgrupoProd=" + subgrupoProd +
                "&idCorVidro=" + corVidro + "&idCorFerragem=" + corFerragem +
                "&idCorAluminio=" + corAluminio;
        }
        
        function onInsert(botao)
        {
            var celula = botao;
            while (celula.nodeName.toLowerCase() != "td")
                celula = celula.parentNode;
            
            var idCliente = FindControl("hdfCodigoCliente", "input").value;
            var idProd = FindControl("hdfCodigoProduto", "input").value;
            var idLoja = FindControl("hdfCodigoLoja", "input").value;
            
            FindControl("hdfCodigoCliente", "input", celula).value = idCliente;
            FindControl("hdfCodigoProduto", "input", celula).value = idProd;
            FindControl("hdfCodigoLoja", "input", celula).value = idLoja;
        }

        function openRpt(exportarExcel)
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ExtratoEstoqueCliente&" +
                getQueryString() + "&exportarExcel=" + exportarExcel);
        }

        function openRptMov(mov, semValor, exportarExcel)
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=MovEstoque" + (!mov ? "Total" : "") + (semValor ? "SemValor" : "") + 
                "&cliente=1&" + getQueryString() + "&exportarExcel=" + exportarExcel);
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
                            <asp:Label ID="Label6" runat="server" ForeColor="#0066FF" Text="Cliente"></asp:Label>
                        </td>
                        <td>
                            <uc6:ctrlSelCliente ID="ctrlSelCliente" runat="server" FazerPostBackBotaoPesquisar="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc5:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="True" MostrarTodas="false"/>
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Período Movimentação"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="false"/>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="false"/>
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
                            <asp:Label ID="Label7" runat="server" ForeColor="#0066FF" Text="CFOP"></asp:Label>
                        </td>
                        <td>
                            <uc7:ctrlSelPopup ID="selCfop" runat="server" 
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
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdMovEstoque" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsMovEstoque" PageSize="20" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum registro encontrado (Certifique-se de ter informado o cliente, a loja e o produto)."
                    OnDataBound="grdMovEstoque_DataBound" DataKeyNames="Codigo" 
                    ShowFooter="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" 
                                    onclientclick="if (!confirm(&quot;Deseja excluir essa movimentação?&quot;)) return false" 
                                    Visible='<%# Eval("PodeExcluir") %>' />
                                <asp:HiddenField ID="hdfTipoMov" runat="server" Value='<%# Eval("TipoMovimentacao") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Codigo" HeaderText="Cód. Mov." SortExpression="Codigo" />
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia" />
                        <asp:BoundField DataField="DescricaoProduto" HeaderText="Produto" SortExpression="DescricaoProduto" />
                        <asp:BoundField DataField="NomeFuncionario" HeaderText="Funcionário" SortExpression="NomeFuncionario" />
                        <asp:TemplateField HeaderText="Data" SortExpression="DataMovimentacao">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DataMovimentacao") %>'></asp:Label>
                                <asp:Label ID="Label11" runat="server" 
                                    Text='<%# "<br />(Data Cad. " + Eval("DataCadastro") + ")" %>' 
                                    Visible='<%# Eval("DataCadastro") != null %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DataMovimentacao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc3:ctrlData ID="ctrlDataMov" runat="server" ExibirHoras="True" 
                                    ReadOnly="ReadWrite" ValidateEmptyText="True" ValidationGroup="insert" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde." SortExpression="QuantidadeMovimentacao">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("QuantidadeMovimentacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("QuantidadeMovimentacao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtQtde" runat="server" 
                                    onkeypress="return soNumeros(event, false, true)" Width="50px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvQtde" runat="server" 
                                    ControlToValidate="txtQtde" Display="Dynamic" ErrorMessage="*" 
                                    ValidationGroup="insert"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="UnidadeMedidaProduto" HeaderText="Unidade" 
                            SortExpression="UnidadeMedidaProduto" />
                        <asp:BoundField DataField="SaldoQuantidade" HeaderText="Saldo" SortExpression="SaldoQuantidade" />
                        <asp:TemplateField HeaderText="E/S" SortExpression="DescricaoTipoMovimentacao">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("DescricaoTipoMovimentacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("DescricaoTipoMovimentacao") %>'></asp:TextBox>
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
                        <asp:TemplateField HeaderText="Valor (Total)" SortExpression="ValorMovimentacao">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("ValorMovimentacao", "{0:c}") %>'></asp:Label>
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
                        <asp:BoundField DataField="SaldoValor" DataFormatString="{0:c}" 
                            HeaderText="Valor Acumulado" SortExpression="SaldoValor" />
                        <asp:TemplateField HeaderText="Observação" SortExpression="Observacao">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Observacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("Observacao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtObservacao" runat="server" Rows="3" 
                                    Text='<%# Bind("Observacao") %>' TextMode="MultiLine" Width="220px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc4:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="MovEstoqueCliente" IdRegistro='<%# Eval("Codigo") %>' />
                                <asp:HiddenField ID="hdfCodigoCliente" runat="server" Value='<%# Eval("CodigoCliente") %>' />
                                <asp:HiddenField ID="hdfCodigoProduto" runat="server" Value='<%# Eval("CodigoProduto") %>' />
                                <asp:HiddenField ID="hdfCodigoLoja" runat="server" Value='<%# Eval("CodigoLoja") %>' />
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" 
                                    onclick="imgAdd_Click" OnClientClick="onInsert(this)" 
                                    ValidationGroup="insert" />
                                <asp:HiddenField ID="hdfCodigoCliente" runat="server" />
                                <asp:HiddenField ID="hdfCodigoProduto" runat="server" />
                                <asp:HiddenField ID="hdfCodigoLoja" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle CssClass="rodapeGrid" />
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMovEstoque" runat="server" MaximumRowsParameterName=""
                    SelectMethod="ObtemLista" StartRowIndexParameterName="" 
                    TypeName="WebGlass.Business.MovimentacaoEstoqueCliente.Fluxo.CRUD"                      
                    DataObjectTypeName="WebGlass.Business.MovimentacaoEstoqueCliente.Entidade.MovimentacaoEstoqueCliente" 
                    DeleteMethod="Remover" CacheExpirationPolicy="Absolute" 
                    ConflictDetection="OverwriteChanges" SkinID="">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" Name="codigoLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlSelCliente" Name="codigoCliente" PropertyName="IdCliente"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codigoInternoProduto" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescrProd" Name="descricaoProduto" 
                            PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpTipoMov" Name="tipoMovimentacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpSituacaoProd" Name="situacaoProduto" 
                            PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpGrupoProd" Name="codigoCfop" 
                            PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpGrupoProd" Name="codigoGrupoProduto" 
                            PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSubgrupoProd" Name="codigoSubgrupoProdo" 
                            PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="codigoCorVidro" 
                            PropertyName="IdCorVidro" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="codigoCorFerragem" 
                            PropertyName="IdCorFerragem" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="codigoCorAluminio" 
                            PropertyName="IdCorAluminio" Type="UInt32" />
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
                <asp:LinkButton ID="lnkCanceladas" runat="server" OnClientClick="openLogCancelamento(); return false"> <img alt="" border="0" src="../Images/ExcluirGrid.gif" /> Movimentações Excluídas</asp:LinkButton>
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
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openRptMov(true, true, false); return false"><img src="../Images/Printer.png" border="0" /> Imprimir (Movimentação Sem Valor)</asp:LinkButton>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td align="left">
                            <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="openRptMov(true, true, true); return false;"><img border="0" 
                                src="../Images/Excel.gif" /> Exportar para o Excel (Movimentação Sem Valor)</asp:LinkButton>
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
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
