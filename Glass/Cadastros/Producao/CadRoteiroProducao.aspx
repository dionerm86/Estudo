<%@ Page Title="Cadastro de Roteiro de Produção" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="CadRoteiroProducao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Producao.CadRoteiroProducao" %>

<%@ Register src="../../Controls/ctrlSelGrupoSubgrupoProd.ascx" tagname="ctrlSelGrupoSubgrupoProd" tagprefix="uc1" %>
<%@ Register src="../../Controls/ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc2" %>
<%@ Register src="../../Controls/ctrlSelProcesso.ascx" tagname="ctrlSelProcesso" tagprefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <script type="text/javascript">
        function alteraSetores()
        {
            var setores = [];
            var campos = FindControl("cblSetores", "table").getElementsByTagName("input");

            for (var i = 0; i < campos.length; i++)
                if (campos[i].type == "checkbox" && campos[i].checked)
                {
                    var valor = campos[i].parentNode.getAttribute("valor");
                    setores.push(valor);
                }
        
            var campoSetores = FindControl("hdfCodigosSetores", "input");
            campoSetores.value = setores.join(",");
        }
    </script>
    <asp:DetailsView ID="dtvRoteiroProducao" runat="server" 
        AutoGenerateRows="False" DataSourceID="odsRoteiroProducao" 
        CssClass="gridStyle detailsViewStyle" 
        DataKeyNames="Codigo" DefaultMode="Insert" 
        GridLines="None">
        <FieldHeaderStyle CssClass="dtvHeader" />
        <Fields>
            <asp:TemplateField HeaderText="Grupo / Subgrupo" 
                SortExpression="CodigoGrupoProduto" Visible="False">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("CodigoGrupoProduto") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <uc1:ctrlSelGrupoSubgrupoProd ID="ctrlSelGrupoSubgrupoProd1" runat="server" 
                        CodigoGrupoProduto='<%# Bind("CodigoGrupoProduto") %>' ApenasVidros="true"
                        CodigoSubgrupoProduto='<%# Bind("CodigoSubgrupoProduto") %>'
                        ExibirGrupoProdutoVazio="True" ExibirSubgrupoProdutoVazio="true" />
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Processo" SortExpression="CodigoProcesso">
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("CodigoProcesso") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <uc3:ctrlSelProcesso ID="ctrlSelProcesso1" runat="server" 
                        CodigoProcesso='<%# Bind("CodigoProcesso") %>' PermitirVazio="False" />
                    <asp:HiddenField ID="hdfIdClassificacao" Value='<%# Bind("IdClassificacaoRoteiroProducao") %>' runat="server"/>    
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Setores do roteiro" 
                SortExpression="CodigosSetoresString">
                <ItemTemplate>
                    <asp:Label ID="Label3" runat="server" 
                        Text='<%# Bind("CodigosSetoresString") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:HiddenField ID="hdfCodigosSetores" runat="server" 
                        Value='<%# Bind("CodigosSetoresString") %>' />
                    <%-- <div style="float: right; padding-left: 16px;">
                        <br />
                        * setores de beneficiamento (podem ser adicionados <br />
                        ao roteiro se o beneficiamento estiver vinculado ao <br />
                        produto, mesmo se o setor não estiver no roteiro)
                    </div> --%>
                    <asp:CheckBoxList ID="cblSetores" runat="server" DataSourceID="odsSetor" 
                        DataTextField="Descricao" DataValueField="Codigo" RepeatColumns="2" 
                        ondatabound="cblSetores_DataBound">
                    </asp:CheckBoxList>
                </EditItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False">
                <EditItemTemplate>
                    <br />
                    <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" 
                        Text="Atualizar" />
                    <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" 
                        onclick="btnCancelar_Click" Text="Cancelar" />
                </EditItemTemplate>
                <InsertItemTemplate>
                    <br />
                    <asp:Button ID="btnInserir" runat="server" CommandName="Insert" 
                        Text="Inserir" />
                    <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" 
                        onclick="btnCancelar_Click" Text="Cancelar" />
                </InsertItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>
    <colo:VirtualObjectDataSource ID="odsRoteiroProducao" runat="server" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        DataObjectTypeName="WebGlass.Business.RoteiroProducao.Entidade.RoteiroProducao" 
        InsertMethod="NovoRoteiroProducao" MaximumRowsParameterName="" 
        SelectMethod="ObtemItem" StartRowIndexParameterName="" 
        TypeName="WebGlass.Business.RoteiroProducao.Fluxo.CRUD" 
        UpdateMethod="Atualizar" oninserted="odsRoteiroProducao_Inserted" 
        onupdated="odsRoteiroProducao_Updated" Culture="" SkinID="">
        <SelectParameters>
            <asp:QueryStringParameter Name="codigoRoteiroProducao" QueryStringField="id" 
                Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource ID="odsSetor" runat="server" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" MaximumRowsParameterName="" 
        SelectMethod="ObtemSetoresRoteiroProducao" 
        StartRowIndexParameterName="" 
        TypeName="WebGlass.Business.Setor.Fluxo.BuscarEValidar">
    </colo:VirtualObjectDataSource>
    <br />
    <div style="color: Blue; text-align: center">
        As alterações feitas no roteiro de produção só serão válidas
        para as etiquetas impressas a partir da alteração. <br />
        As etiquetas já impressas continuam com o seu próprio roteiro.
    </div>
</asp:Content>
