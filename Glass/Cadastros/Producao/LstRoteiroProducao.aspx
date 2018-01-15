<%@ Page Title="Roteiros de Produção" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstRoteiroProducao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Producao.LstRoteiroProducao" %>

<%@ Register src="../../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>
<%@ Register src="../../Controls/ctrlSelGrupoSubgrupoProd.ascx" tagname="ctrlSelGrupoSubgrupoProd" tagprefix="uc2" %>
<%@ Register src="../../Controls/ctrlSelProcesso.ascx" tagname="ctrlSelProcesso" tagprefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <script type="text/javascript">
        function openRpt(exportarExcel) {
            var grupoProd = FindControl("selGrupoSubgrupo_drpGrupoProd", "select");
            grupoProd = grupoProd ? grupoProd.value : 0;

            var subgrupoProd = FindControl("selGrupoSubgrupo_hdfSubgrupoProd", "input");
            subgrupoProd = subgrupoProd ? subgrupoProd.value : 0;

            var processo = FindControl("selProcesso_hdfValor", "input");
            processo = processo ? processo.value : 0;

            openWindow(600, 800, "../../Relatorios/Producao/RelBase.aspx?rel=RoteiroProducao&grupoProd=" + grupoProd +
                "&subgrupoProd=" + subgrupoProd + "&processo=" + processo + "&exportarExcel=" + (!!exportarExcel));
        }
    </script>
    <div class="filtro">
        <div>
            <span runat="server" visible="false">
                <asp:Label ID="Label1" runat="server" Text="Grupo/Subgrupo de Produto" 
                    AssociatedControlID="selGrupoSubgrupo"></asp:Label>
                <uc2:ctrlSelGrupoSubgrupoProd ID="selGrupoSubgrupo" runat="server" 
                    ApenasVidros="True" ExibirSubgrupoProdutoVazio="True" />
                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    CssClass="botaoPesquisar" onclick="imgPesq_Click" />
            </span>
            <span>
                <asp:Label ID="Label2" runat="server" Text="Processo" 
                    AssociatedControlID="selProcesso"></asp:Label>
                <uc3:ctrlSelProcesso ID="selProcesso" runat="server" FazerPostBackBotaoPesquisar="true" />
            </span>
        </div>
    </div>
    <div class="inserir">
        <asp:HyperLink ID="lnkInserir" runat="server" 
            NavigateUrl="~/Cadastros/Producao/CadRoteiroProducao.aspx">Inserir Roteiro de Produção</asp:HyperLink>
    </div>
    <asp:GridView ID="grdRoteiroProducao" runat="server" AllowPaging="True" 
        AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
        DataKeyNames="Codigo" 
        DataSourceID="odsRoteiroProducao" 
        EmptyDataText="Não há roteiros de produção cadastrados." GridLines="None">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton ID="imgEditar" runat="server" 
                        ImageUrl="~/Images/EditarGrid.gif"                         
                        PostBackUrl='<%# "~/Cadastros/Producao/CadRoteiroProducao.aspx?id=" + Eval("Codigo") %>' />
                    <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" 
                        ImageUrl="~/Images/ExcluirGrid.gif" 
                        onclientclick="if (!confirm(&quot;Deseja excluir esse roteiro de produção?&quot;)) return false;" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CodigoInternoProcesso" HeaderText="Processo" 
                SortExpression="CodProcesso" />
            <asp:BoundField DataField="DescricaoSetores" HeaderText="Setores"  />
            <asp:TemplateField>
                <ItemTemplate>
                    <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                        IdRegistro='<%# Convert.ToUInt32(Eval("Codigo")) %>' Tabela="RoteiroProducao" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <PagerStyle CssClass="pgr" />
        <AlternatingRowStyle CssClass="alt" />
    </asp:GridView>
    <br />
    <div class="imprimir">
        <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false">
            <img src="../../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
        &nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openRpt(true); return false">
            <img src="../../Images/Excel.gif" border="0" /> Exportar para o Excel</asp:LinkButton>
    </div>
    <colo:VirtualObjectDataSource ID="odsRoteiroProducao" runat="server" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        Culture="pt-BR" 
        DataObjectTypeName="WebGlass.Business.RoteiroProducao.Entidade.RoteiroProducao" 
        DeleteMethod="Excluir" EnablePaging="True" MaximumRowsParameterName="pageSize" 
        SelectCountMethod="ObtemNumeroRegistros" SelectMethod="ObtemLista" 
        StartRowIndexParameterName="startRow" 
        TypeName="WebGlass.Business.RoteiroProducao.Fluxo.CRUD" 
    SortParameterName="sortExpression">
        <SelectParameters>
            <asp:Parameter Name="codigoRoteiroProducao" Type="UInt32" />
            <asp:ControlParameter Name="codigoGrupoProduto" Type="UInt32" ControlID="selGrupoSubgrupo" PropertyName="CodigoGrupoProduto" />
            <asp:ControlParameter Name="codigoSubgrupoProduto" Type="UInt32" ControlID="selGrupoSubgrupo" PropertyName="CodigoSubgrupoProduto" />
            <asp:ControlParameter Name="codigoProcesso" Type="UInt32" ControlID="selProcesso" PropertyName="CodigoProcesso" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>

