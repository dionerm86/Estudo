<%@ Page Title="Cadastro de Regra de Natureza de Operação" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="CadRegraNaturezaOperacao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadRegraNaturezaOperacao" %>

<%@ Register src="../Controls/ctrlLoja.ascx" tagname="ctrlLoja" tagprefix="uc1" %>
<%@ Register src="../Controls/ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc2" %>
<%@ Register src="../Controls/ctrlNaturezaOperacao.ascx" tagname="ctrlNaturezaOperacao" tagprefix="uc3" %>

<%@ Register src="../Controls/ctrlSelGrupoSubgrupoProd.ascx" tagname="ctrlSelGrupoSubgrupoProd" tagprefix="uc4" %>

<%@ Register src="../Controls/ctrlSelCorProd.ascx" tagname="ctrlSelCorProd" tagprefix="uc5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <asp:DetailsView ID="dtvRegraNaturezaOperacao" runat="server" 
        AutoGenerateRows="False" CssClass="gridStyle detailsViewStyle" 
        DataKeyNames="IdRegraNaturezaOperacao" DataSourceID="odsRegraNaturezaOperacao" 
        DefaultMode="Insert" GridLines="None">
        <FieldHeaderStyle CssClass="dtvHeader" />
        <Fields>
            <asp:TemplateField HeaderText="Loja" SortExpression="Loja">
                <EditItemTemplate>
                    <uc1:ctrlLoja ID="ctrlLoja1" runat="server" MostrarTodas="False" 
                        SelectedValue='<%# Bind("IdLoja") %>' />
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Tipo Cliente" SortExpression="TipoCliente">
                <EditItemTemplate>
                    <asp:DropDownList ID="drpTipoCliente" runat="server" 
                        DataSourceID="odsTipoCliente" DataTextField="Name" 
                        DataValueField="Id" SelectedValue='<%# Bind("IdTipoCliente") %>'>
                    </asp:DropDownList>
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Grupo / Subgrupo" SortExpression="GrupoProduto">
                <EditItemTemplate>                    
                    <uc4:ctrlSelGrupoSubgrupoProd ID="ctrlSelGrupoSubgrupoProd" runat="server" 
                        IdGrupoProduto='<%# Bind("IdGrupoProd") %>' 
                        IdSubgrupoProduto='<%# Bind("IdSubgrupoProd") %>' 
                        ExibirGrupoProdutoVazio="True" ExibirSubgrupoProdutoVazio="true" />
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Cor">
                <EditItemTemplate>
                    <uc5:ctrlSelCorProd ID="ctrlSelCorProd" runat="server" 
                        OnLoad="ctrlSelCorProd_Load"
                        IdCorAluminioInt32='<%# Bind("IdCorAluminio") %>' 
                        IdCorFerragemInt32='<%# Bind("IdCorFerragem") %>' 
                        IdCorVidroInt32='<%# Bind("IdCorVidro") %>' />
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Espessura">
                <EditItemTemplate>
                    <asp:TextBox ID="txtEspessura" runat="server" Text='<%# Bind("Espessura") %>' 
                        Width="70px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                    mm
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Natureza Operação Produção Intraestadual" 
                SortExpression="CodigoNaturezaOperacaoProducaoIntra">
                <EditItemTemplate>
                    <uc3:ctrlNaturezaOperacao ID="ctrlNaturezaOperacaoProdIntra" runat="server" 
                        IdNaturezaOperacao='<%# Bind("IdNaturezaOperacaoProdIntra") %>' 
                        PermitirVazio="False" />
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Natureza Operação Revenda Intraestadual" 
                SortExpression="CodigoNaturezaOperacaoRevendaIntra">
                <EditItemTemplate>
                    <uc3:ctrlNaturezaOperacao ID="ctrlNaturezaOperacaoRevIntra" runat="server" 
                        IdNaturezaOperacao='<%# Bind("IdNaturezaOperacaoRevIntra") %>' 
                        PermitirVazio="False" />
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Natureza Operação Produção Interestadual" 
                SortExpression="CodigoNaturezaOperacaoProducaoInter">
                <EditItemTemplate>
                    <uc3:ctrlNaturezaOperacao ID="ctrlNaturezaOperacaoProdInter" runat="server" 
                        IdNaturezaOperacao='<%# Bind("IdNaturezaOperacaoProdInter") %>' 
                        PermitirVazio="False" />
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Natureza Operação Revenda Interestadual" 
                SortExpression="CodigoNaturezaOperacaoRevendaIntra">
                <EditItemTemplate>
                    <uc3:ctrlNaturezaOperacao ID="ctrlNaturezaOperacaoRevInter" runat="server" 
                        IdNaturezaOperacao='<%# Bind("IdNaturezaOperacaoRevInter") %>' 
                        PermitirVazio="False" />
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Natureza Operação Produção ST Intraestadual *" 
                SortExpression="CodigoNaturezaOperacaoProducaoStIntra">
                <EditItemTemplate>
                    <uc3:ctrlNaturezaOperacao ID="ctrlNaturezaOperacaoProdStIntra" runat="server" 
                        IdNaturezaOperacao='<%# Bind("IdNaturezaOperacaoProdStIntra") %>' 
                        PermitirVazio="False" />
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Natureza Operação Revenda ST Intraestadual *" 
                SortExpression="CodigoNaturezaOperacaoRevendaStIntra">
                <EditItemTemplate>
                    <uc3:ctrlNaturezaOperacao ID="ctrlNaturezaOperacaoRevStIntra" runat="server" 
                        IdNaturezaOperacao='<%# Bind("IdNaturezaOperacaoRevStIntra") %>' 
                        PermitirVazio="False" />
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Natureza Operação Produção ST Interestadual *" 
                SortExpression="CodigoNaturezaOperacaoProducaoStInter">
                <EditItemTemplate>
                    <uc3:ctrlNaturezaOperacao ID="ctrlNaturezaOperacaoProdStInter" runat="server" 
                        IdNaturezaOperacao='<%# Bind("IdNaturezaOperacaoProdStInter") %>' 
                        PermitirVazio="False" />
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Natureza Operação Revenda ST Interestadual *" 
                SortExpression="CodigoNaturezaOperacaoRevendaStInter">
                <EditItemTemplate>
                    <uc3:ctrlNaturezaOperacao ID="ctrlNaturezaOperacaoRevStInter" runat="server" 
                        IdNaturezaOperacao='<%# Bind("IdNaturezaOperacaoRevStInter") %>' 
                        PermitirVazio="False" />
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False">
                <EditItemTemplate>
                    <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" 
                        Text="Atualizar" />
                    <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" 
                        onclick="btnCancelar_Click" Text="Cancelar" />
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:Button ID="btnInserir" runat="server" CommandName="Insert" 
                        Text="Inserir" />
                    <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" 
                        onclick="btnCancelar_Click" Text="Cancelar" />
                </InsertItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
        </Fields>
        <AlternatingRowStyle CssClass="alt" />
    </asp:DetailsView>
    <div style="color: blue; font-style: italic">
        * Estes campos serão utilizados quando o MVA do produto for maior que 0 (zero).
    </div>
    <colo:VirtualObjectDataSource ID="odsRegraNaturezaOperacao" runat="server" 
        DataObjectTypeName="Glass.Fiscal.Negocios.Entidades.RegraNaturezaOperacao" 
        TypeName="Glass.Fiscal.Negocios.ICfopFluxo" 
        InsertMethod="SalvarRegraNaturezaOperacao" 
        SelectMethod="ObtemRegraNaturezaOperacao" 
        UpdateMethod="SalvarRegraNaturezaOperacao" 
        UpdateStrategy="GetAndUpdate"
        Culture="pt-BR">
        <SelectParameters>
            <asp:QueryStringParameter Name="idRegraNaturezaOperacao" 
                QueryStringField="id" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource ID="odsTipoCliente" runat="server" 
        SelectMethod="ObtemDescritoresTipoCliente" TypeName="Glass.Global.Negocios.IClienteFluxo" 
        Culture="pt-BR">
    </colo:VirtualObjectDataSource>
</asp:Content>

