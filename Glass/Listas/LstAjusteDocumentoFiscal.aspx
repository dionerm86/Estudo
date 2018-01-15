<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LstAjusteDocumentoFiscal.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstAjusteDocumentoFiscal" Title="Ajuste do Documento Fiscal"
    MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlSelProduto.ascx" TagName="ctrlSelProduto" TagPrefix="uc2" %>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function verificaProdutoNota(nomeControle, prod) {
            if (!prod) return;

            var idNf = parseInt('<%= Request["idNf"] %>', 10);
            if (idNf == 0 || isNaN(idNf))
                return;

            var ok = LstAjusteDocumentoFiscal.VerificaProdutoNota(prod, idNf);

            if (ok.error != null) {
                alert(ok.error.description);

                var descr = document.getElementById(nomeControle + "_txtDescr");
                descr.value = "";
                descr.onchange();
            }
        }
    </script>

    <asp:GridView ID="grdAjusteDocumentoFiscal" runat="server" AutoGenerateColumns="False"
        DataSourceID="odsAjusteDocumentoFiscal" CssClass="gridStyle" DataKeyNames="IdAjusteDocumentoFiscal"
        GridLines="None" ShowFooter="True" OnDataBound="grdAjusteDocumentoFiscal_DataBound"
        OnRowCommand="grdAjusteDocumentoFiscal_RowCommand">
        <Columns>
            <asp:TemplateField>
                <EditItemTemplate>
                    <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/Ok.gif" />
                    <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                        ImageUrl="~/Images/ExcluirGrid.gif" />
                    <asp:HiddenField ID="hdfIdNf" runat="server" Value='<%# Bind("IdNf") %>' />
                    <asp:HiddenField ID="hdfIdCte" runat="server" Value='<%# Bind("IdCte") %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif"
                        CausesValidation="false" />
                    <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                        CausesValidation="false" OnClientClick="if (!confirm(&quot;Deseja excluir esse ajuste?&quot;)) return false;" />
                    <asp:CheckBox ID="chkExibirLinha" runat="server" Visible="false" Checked='<%# (uint)Eval("IdAjusteDocumentoFiscal") > 0 %>' />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Obs. Lanc. Fiscal" SortExpression="IdObsLancFiscal">
                <EditItemTemplate>
                    <asp:DropDownList ID="drpObsLancFiscal" runat="server" AppendDataBoundItems="True"
                        DataSourceID="odsObsLancFiscal" DataTextField="Descricao" DataValueField="IdObsLancFiscal"
                        SelectedValue='<%# Bind("IdObsLancFiscal") %>'>
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvObsLancFiscal" runat="server" ControlToValidate="drpObsLancFiscal"
                        Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="drpObsLancFiscal" runat="server" AppendDataBoundItems="True"
                        DataSourceID="odsObsLancFiscal" DataTextField="Descricao" DataValueField="IdObsLancFiscal"
                        SelectedValue='<%# Bind("IdObsLancFiscal") %>'>
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvObsLancFiscal" runat="server" ControlToValidate="drpObsLancFiscal"
                        Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label9" runat="server" Text='<%# Bind("DescricaoObsLancFiscal") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Ajuste" SortExpression="CodigoAjuste">
                <EditItemTemplate>
                    <uc1:ctrlSelPopup ID="selAjuste" runat="server" DataSourceID="odsAjusteBeneficioIncentivo"
                        DataTextField="Codigo" DataValueField="IdAjBenInc" Descricao='<%# Eval("CodigoAjuste") %>'
                        PermitirVazio="false" ColunasExibirPopup="IdAjBenInc|Codigo|Descricao" ExibirIdPopup="false"
                        TitulosColunas="Id|Código|Descrição" Valor='<%# Bind("IdAjBenInc") %>' TextWidth="100px"
                        TituloTela="Selecione o ajuste/benefício/incentivo" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("CodigoAjuste") %>'></asp:Label>
                    <%-- -
                    <asp:Label ID="Label7" runat="server" Text='<%# Eval("DescricaoAjuste") %>'></asp:Label> --%>
                </ItemTemplate>
                <FooterTemplate>
                    <uc1:ctrlSelPopup ID="selAjuste" runat="server" DataSourceID="odsAjusteBeneficioIncentivo"
                        DataTextField="Codigo" DataValueField="IdAjBenInc" PermitirVazio="false" ColunasExibirPopup="IdAjBenInc|Codigo|Descricao"
                        ExibirIdPopup="false" TitulosColunas="Id|Código|Descrição" TituloTela="Selecione o ajuste/benefício/incentivo" />
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Produto" SortExpression="CodInternoProd">
                <EditItemTemplate>
                    <uc2:ctrlSelProduto ID="selProduto" runat="server" IdProd='<%# Bind("IdProd") %>'
                        PermitirVazio="true" Callback="verificaProdutoNota" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("CodInternoProd") %>'></asp:Label>
                </ItemTemplate>
                <FooterTemplate>
                    <uc2:ctrlSelProduto ID="selProduto" runat="server" PermitirVazio="true" Callback="verificaProdutoNota" />
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Valor Base Cálculo Imposto" SortExpression="ValorBaseCalculoImposto">
                <EditItemTemplate>
                    <asp:TextBox ID="txtValorBaseCalculoImposto" runat="server" Text='<%# Bind("ValorBaseCalculoImpostoString") %>'
                        onkeypress="return soNumeros(event, false, true)" Width="90px"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("ValorBaseCalculoImposto", "{0:C}") %>'></asp:Label>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtValorBaseCalculoImposto" runat="server" Width="90px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Alíquota Imposto" SortExpression="AliquotaImposto">
                <EditItemTemplate>
                    <asp:TextBox ID="txtAliquotaImposto" runat="server" Text='<%# Bind("AliquotaImpostoString") %>'
                        onkeypress="return soNumeros(event, false, true)" Width="40px"></asp:TextBox>
                    %
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("AliquotaImposto", "{0}%") %>'></asp:Label>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtAliquotaImposto" runat="server" Width="40px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                    %
                </FooterTemplate>
                <FooterStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Valor Imposto" SortExpression="ValorImposto">
                <EditItemTemplate>
                    <asp:TextBox ID="txtValorImposto" runat="server" Text='<%# Bind("ValorImpostoString") %>'
                        onkeypress="return soNumeros(event, false, true)" Width="90px"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label5" runat="server" Text='<%# Bind("ValorImposto", "{0:C}") %>'></asp:Label>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtValorImposto" runat="server" Width="90px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Outros Valores" SortExpression="OutrosValores">
                <EditItemTemplate>
                    <asp:TextBox ID="txtOutrosValores" runat="server" Text='<%# Bind("OutrosValoresString") %>'
                        onkeypress="return soNumeros(event, false, true)" Width="90px"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label6" runat="server" Text='<%# Bind("OutrosValores", "{0:C}") %>'></asp:Label>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtOutrosValores" runat="server" Width="90px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Observação" SortExpression="Obs">
                <EditItemTemplate>
                    <asp:TextBox ID="txtObservacao" runat="server" MaxLength="255" Text='<%# Bind("Obs") %>'
                        Width="200px"></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label8" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtObservacao" runat="server" MaxLength="255" Width="200px"></asp:TextBox>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <FooterTemplate>
                    <asp:ImageButton ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif"
                        OnClick="imgAdicionar_Click" />
                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
        <AlternatingRowStyle CssClass="alt" />
    </asp:GridView>
    <colo:VirtualObjectDataSource runat="server" ID="odsAjusteDocumentoFiscal" DataObjectTypeName="Glass.Data.Model.AjusteDocumentoFiscal"
        DeleteMethod="Delete" SelectMethod="ObtemPorNf" TypeName="Glass.Data.DAL.AjusteDocumentoFiscalDAO"
        UpdateMethod="Update">
        <SelectParameters>
            <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource ID="odsAjusteBeneficioIncentivo" runat="server" SelectMethod="GetForSelPopup"
        TypeName="Glass.Data.DAL.AjusteBeneficioIncentivoDAO">
        <SelectParameters>
            <asp:Parameter DefaultValue="0" Name="tipoImposto" Type="Int32" />
            <asp:QueryStringParameter DefaultValue="" Name="idNf" QueryStringField="idNf" Type="UInt32" />
            <asp:QueryStringParameter DefaultValue="" Name="idCte" QueryStringField="idCte" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource ID="odsObsLancFiscal" runat="server" SelectMethod="GetByNf"
        TypeName="Glass.Data.DAL.ObsLancFiscalNfDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
