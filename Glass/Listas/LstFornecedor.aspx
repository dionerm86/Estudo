<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstFornecedor.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstFornecedor" Title="Fornecedores" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function precoFornecedor(idFornec) {
            openWindow(600, 800, "../Utils/PrecoFornecedor.aspx?idFornec=" + idFornec);
        }

        function openRpt(exportarExcel, ficha, id) {
            var idFornec = FindControl("txtCodFornec", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var nome = FindControl("txtNome", "input").value;
            var cnpj = FindControl("txtCnpj", "input").value;
            var credito = FindControl("chkCredito", "input").checked;
            var idConta = FindControl("drpPlanoContas", "select").value;
            var tipoPagto = FindControl("drpTipoPagto", "select").value;
            var endereco = FindControl("txtEndereco", "input").value;
            var vendedor = FindControl("txtVendedor", "input").value;

            if (idFornec == "")
                idFornec = 0;

            if (id == 0) {
                if (ficha) {
                    openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=FichaFornecedor&idFornecedor=" + idFornec + "&nome=" + nome +
                        "&situacao=" + situacao + "&cnpj=" + cnpj + "&credito=" + credito + "&idConta=" + idConta + "&tipoPagto=" + tipoPagto +
                        "&endereco=" + endereco + "&vendedor=" + vendedor + "&exportarExcel=" + exportarExcel);
                }
                else {
                    openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=ListaFornecedores&idFornec=" + idFornec + "&nomeFornec=" + nome +
                        "&situacao=" + situacao + "&cnpj=" + cnpj + "&comCredito=" + credito + "&idConta=" + idConta + "&tipoPagto=" + tipoPagto +
                        "&endereco=" + endereco + "&vendedor=" + vendedor + "&exportarExcel=" + exportarExcel);
                }
            }
            else {
                openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=FichaFornecedor&idFornecedor=" + id);
            }
            return false;
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cód. Fornec." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodFornec" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Nome" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" onkeypress="if (isEnter(event)) return false;"
                                Width="170px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="CNPJ" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCnpj" runat="server" onkeypress="if (isEnter(event)) return false;"
                                Width="170px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Endereço" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEndereco" runat="server" onkeypress="if (isEnter(event)) return false;"
                                Width="170px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Vendedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtVendedor" runat="server" onkeypress="if (isEnter(event)) return false;"
                                Width="170px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Plano de Contas" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpPlanoContas" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsPlanoConta" DataTextField="DescrPlanoGrupo" DataValueField="IdConta">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList></td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Parcela Padrão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoPagto" runat="server" DataSourceID="odsParcelas" DataTextField="Descricao"
                            DataValueField="IdParcela" AppendDataBoundItems="true" >
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AppendDataBoundItems="True" DataSourceID="odsSituacaoFornecedor"
                                DataTextField="Translation" DataValueField="Key">
                                <asp:ListItem Value="">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkCredito" runat="server" Text="Fornecedores com crédito" AutoPostBack="True" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqFornComCredito" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
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
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Fornecedor</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdFornecedor" runat="server" DataKeyNames="IdFornec" DataSourceID="odsFornecedor"
                    EmptyDataText="Não há Fornecedores Cadastrados" SkinId="defaultGridView"
                    OnRowCommand="grdFornec_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadFornecedor.aspx?idFornec=" + Eval("IdFornec") %>' Visible='<%# PodeEditar() %>'>
                                    <img border="0" src="../Images/EditarGrid.gif" alt="Editar" /></asp:HyperLink>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Fornecedor?&quot;);"
                                    ToolTip="Excluir" Visible='<%# PodeApagar() %>' />
                                <asp:ImageButton ID="ImageButton1" runat="server" OnClientClick='<%# "precoFornecedor(" + Eval("IdFornec") + "); return false" %>'
                                    ImageUrl="~/Images/dinheiro.gif" ToolTip="Preço de Produto por Fornecedor" />
                                <asp:ImageButton ID="imbInativar" runat="server" CommandArgument='<%# Eval("IdFornec") %>'
                                    CommandName="Inativar" ImageUrl="~/Images/Inativar.gif" OnClientClick="if (!confirm(&quot;Deseja alterar a situação desse fornecedor?&quot;)) return false"
                                    ToolTip="Alterar situação" Visible='<%# PodeInativar() %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdFornec" HeaderText="Cód." SortExpression="IdFornec" />
                        <asp:BoundField DataField="NomeFantasia" HeaderText="Nome Fantasia" SortExpression="NomeFantasia" />
                        <asp:BoundField DataField="CpfCnpj" HeaderText="CPF/CNPJ" SortExpression="CpfCnpj" />
                        <asp:BoundField DataField="RgInscEst" HeaderText="RG/Insc. Est." SortExpression="RgInscEst" />
                        <asp:BoundField DataField="DtUltCompra" DataFormatString="{0:d}" HeaderText="Ult. Compra"
                            SortExpression="DtUltCompra" />
                        <asp:BoundField DataField="TelCont" HeaderText="Tel. Cont." SortExpression="TelCont" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Vendedor" HeaderText="Vendedor" SortExpression="Vendedor" />
                        <asp:BoundField DataField="TelCelVend" HeaderText="Cel. Vend." SortExpression="TelCelVend" />
                        <asp:BoundField DataField="Credito" DataFormatString="{0:C}" HeaderText="Crédito"
                            SortExpression="Credito" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgFicha" runat="server" ImageUrl="~/Images/printer.png" OnClientClick='<%# "openRpt(false, true, " + Eval("IdFornec") + "); return false;" %>' />
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# (uint)(int)Eval("IdFornec") %>'
                                    Tabela="Fornecedor" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <table>
                    <tr>
                        <td align="center">
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false, false, 0);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                            &nbsp;&nbsp;&nbsp;
                            <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="openRpt(true, false, 0); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="center">
                            <asp:LinkButton ID="lnkImprimirFicha" runat="server" OnClientClick="return openRpt(false, true, 0);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir ficha</asp:LinkButton>
                            &nbsp;&nbsp;&nbsp;
                            <asp:LinkButton ID="lnkExcel" runat="server" OnClientClick="openRpt(true, true, 0); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar ficha para o Excel</asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFornecedor" runat="server" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.Fornecedor"
                    DeleteMethod="ApagarFornecedor" 
                    DeleteStrategy="GetAndDelete"
                    SelectMethod="PesquisarFornecedores" 
                    SelectByKeysMethod="ObtemFornecedor"
                    TypeName="Glass.Global.Negocios.IFornecedorFluxo"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SortParameterName="sortExpression">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodFornec" Name="idFornec" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeFornec" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="txtCnpj" Name="cnpj" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="chkCredito" Name="comCredito" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="drpPlanoContas" Name="idPlanoConta" PropertyName="SelectedValue" Type="UInt32"/>
                        <asp:ControlParameter ControlID="drpTipoPagto" Name="idTipoPagto" PropertyName="SelectedValue" Type="UInt32"/>
                        <asp:ControlParameter ControlID="txtEndereco" Name="endereco" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtVendedor" Name="vendedor" PropertyName="Text" Type="String" />
                        <asp:Parameter Name="tipoPessoa" DefaultValue="" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSituacaoFornecedor" runat="server" 
                    SelectMethod="GetTranslatesFromTypeName"
                    TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.SituacaoFornecedor, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoConta" runat="server" 
                    SelectMethod="GetPlanoContas"
                    TypeName="Glass.Data.DAL.PlanoContasDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="2" Name="tipo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsParcelas" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.ParcelasDAO">
</colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
