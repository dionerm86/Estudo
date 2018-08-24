<%@ Page Title="Regras de Natureza de Operação" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstRegraNaturezaOperacao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstRegraNaturezaOperacao" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register src="../Controls/ctrlSelGrupoSubgrupoProd.ascx" tagname="ctrlSelGrupoSubgrupoProd" tagprefix="uc2" %>
<%@ Register src="../Controls/ctrlSelCorProd.ascx" tagname="ctrlSelCorProd" tagprefix="uc3" %>
<%@ Register src="../Controls/ctrlNaturezaOperacao.ascx" tagname="ctrlNaturezaOperacao" tagprefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function excluir(id)
        {
            openWindow(200, 400, "../Utils/SetMotivoCancRegraNatOp.aspx?id=" + id);
        }
    </script>
    <div class="filtro">
        <div>
            <span>
                <asp:Label ID="Label3" runat="server" Text="Loja" AssociatedControlID="drpLoja"></asp:Label>
                <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True" 
                    DataSourceID="odsLoja" DataTextField="NomeFantasia" DataValueField="IdLoja">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:ImageButton ID="imgPesq" runat="server" CssClass="botaoPesquisar"
                    ImageUrl="~/Images/Pesquisar.gif" onclick="imgPesq_Click" />
            </span>
            <span>
                <asp:Label ID="Label4" runat="server" Text="Tipo de Cliente" AssociatedControlID="drpTipoCliente"></asp:Label>
                <asp:DropDownList ID="drpTipoCliente" runat="server" 
                    AppendDataBoundItems="True" DataSourceID="odsTipoCliente" 
                    DataTextField="Name" DataValueField="Id">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:ImageButton ID="ImageButton1" runat="server" CssClass="botaoPesquisar"
                    ImageUrl="~/Images/Pesquisar.gif" onclick="imgPesq_Click" />
            </span>
            <span>
                <asp:Label ID="Label5" runat="server" Text="Grupo/subgrupo" AssociatedControlID="selGrupoSubgrupoProd"></asp:Label>
                <uc2:ctrlSelGrupoSubgrupoProd ID="selGrupoSubgrupoProd" runat="server" />
                <asp:ImageButton ID="ImageButton3" runat="server" CssClass="botaoPesquisar"
                    ImageUrl="~/Images/Pesquisar.gif" onclick="imgPesq_Click" />
            </span>
        </div>
        <div>
            <span>
                <asp:Label ID="Label6" runat="server" Text="Cor" AssociatedControlID="selCorProd"></asp:Label>
                <uc3:ctrlSelCorProd ID="selCorProd" runat="server" />
                <asp:ImageButton ID="ImageButton4" runat="server" CssClass="botaoPesquisar"
                    ImageUrl="~/Images/Pesquisar.gif" onclick="imgPesq_Click" />
            </span>
            <span>
                <asp:Label ID="Label7" runat="server" Text="Espessura" AssociatedControlID="txtEspessura"></asp:Label>
                <asp:TextBox ID="txtEspessura" runat="server" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                <asp:ImageButton ID="ImageButton5" runat="server" CssClass="botaoPesquisar"
                    ImageUrl="~/Images/Pesquisar.gif" onclick="imgPesq_Click" />
            </span>
            <span>
                <asp:Label ID="Label8" runat="server" Text="Natureza de Operação" AssociatedControlID="selNaturezaOperacao"></asp:Label>
                <uc4:ctrlNaturezaOperacao ID="selNaturezaOperacao" runat="server" FazerPostBackBotaoPesquisar="true" />
            </span>
            <span>
                <asp:Label ID="Label14" runat="server" Text="Uf Destino" ForeColor="#0066FF"></asp:Label>
            </span>
            <span>
                <sync:CheckBoxListDropDown ID="cblUfDestino" runat="server" CheckAll="False" DataSourceID="odsUf"
                    DataTextField="Name" DataValueField="Id" ImageURL="~/Images/DropDown.png" OpenOnStart="False" Width="160px">
                    <asp:ListItem></asp:ListItem>
                </sync:CheckBoxListDropDown>
            </span>
        </div>
    </div>
    <div class="inserir">
        <asp:HyperLink ID="lnkInserir" runat="server" NavigateUrl="~/Cadastros/CadRegraNaturezaOperacao.aspx"> Nova Regra de Natureza de Operação</asp:HyperLink>
    </div>
    <asp:GridView ID="grdRegraNaturezaOperacao" runat="server" DataSourceID="odsRegraNaturezaOperacao"
        AllowPaging="True" AutoGenerateColumns="False" CssClass="gridStyle"
        DataKeyNames="IdRegraNaturezaOperacao" GridLines="None">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:HyperLink ID="lnkEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif" NavigateUrl='<%# Eval("IdRegraNaturezaOperacao", "~/Cadastros/CadRegraNaturezaOperacao.aspx?id={0}") %>'></asp:HyperLink>
                    <asp:ImageButton ID="ImageButton2" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                        OnClientClick='<%# Eval("IdRegraNaturezaOperacao", "excluir({0}); return false") %>' />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="Loja" />
            <asp:BoundField DataField="DescricaoTipoCliente" HeaderText="Tipo de Cliente" SortExpression="TipoCliente" />
            <asp:TemplateField HeaderText="Grupo/Subgrupo" SortExpression="GrupoProduto, SubgrupoProduto">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# ObtemDescricaoGrupoSubgrupo(Eval("DescricaoGrupoProduto"), Eval("DescricaoSubgrupoProduto")) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Cor/Espessura" 
                SortExpression="CodigoCorVidro, CodigoCorAluminio, CodigoCorFerragem, Espessura">
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" 
                        Text='<%# ObtemCorEspessura(Eval("DescricaoCorVidro"), Eval("DescricaoCorAluminio"), Eval("DescricaoCorFerragem"), Eval("Espessura")) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Ufs Destino"
                SortExpression="UfDest">
                <ItemTemplate>
                    <asp:Label ID="lblUfDestino" runat="server"
                        Text='<%#Eval("UfDest") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="DescricaoNaturezaOperacaoProducaoIntra" HeaderText="Nat. Oper. Produção Intra."
                SortExpression="CodigoNaturezaOperacaoProducaoIntra" />
            <asp:BoundField DataField="DescricaoNaturezaOperacaoRevendaIntra" HeaderText="Nat. Oper. Revenda Intra."
                SortExpression="CodigoNaturezaOperacaoRevendaIntra" />
            <asp:BoundField DataField="DescricaoNaturezaOperacaoProducaoInter" 
                HeaderText="Nat. Oper. Produção Inter." 
                SortExpression="CodigoNaturezaOperacaoProducaoInter" />
            <asp:BoundField DataField="DescricaoNaturezaOperacaoRevendaInter" 
                HeaderText="Nat. Oper. Revenda Inter." 
                SortExpression="CodigoNaturezaOperacaoRevendaInter" />
            <asp:BoundField DataField="DescricaoNaturezaOperacaoProducaoStIntra" HeaderText="Nat. Oper. Produção ST Intra. *"
                SortExpression="CodigoNaturezaOperacaoProducaoStIntra" />
            <asp:BoundField DataField="DescricaoNaturezaOperacaoRevendaStIntra" HeaderText="Nat. Oper. Revenda ST Intra. *"
                SortExpression="CodigoNaturezaOperacaoRevendaStIntra" />
            <asp:BoundField DataField="DescricaoNaturezaOperacaoProducaoStInter" 
                HeaderText="Nat. Oper. Produção ST Inter. *" 
                SortExpression="CodigoNaturezaOperacaoProducaoStInter" />
            <asp:BoundField DataField="DescricaoNaturezaOperacaoRevendaStInter" 
                HeaderText="Nat. Oper. Revenda ST Inter. *" 
                SortExpression="CodigoNaturezaOperacaoRevendaStInter" />
            <asp:TemplateField>
                <ItemTemplate>
                    <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# (uint)(int)Eval("IdRegraNaturezaOperacao") %>'
                        Tabela="RegraNaturezaOperacao" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <PagerStyle CssClass="pgr" />
        <AlternatingRowStyle CssClass="alt" />
    </asp:GridView>
    <div style="color: blue; font-style: italic">
        * Estes campos serão utilizados quando o MVA do produto for maior que 0 (zero).
    </div>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRegraNaturezaOperacao" runat="server"
        SelectMethod="PesquisarRegrasNaturezaOperacao"
        TypeName="Glass.Fiscal.Negocios.ICfopFluxo"
        EnablePaging="True" MaximumRowsParameterName="pageSize" SortParameterName="sortExpression">
        <SelectParameters>
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" 
                PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="drpTipoCliente" Name="idTipoCliente" 
                PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="selGrupoSubgrupoProd" Name="idGrupoProd" 
                PropertyName="CodigoGrupoProduto" Type="Int32" />
            <asp:ControlParameter ControlID="selGrupoSubgrupoProd" Name="idSubgrupoProd" 
                PropertyName="CodigoSubgrupoProduto" Type="Int32" />
            <asp:ControlParameter ControlID="selCorProd" Name="idCorVidro" 
                PropertyName="IdCorVidro" Type="Int32" />
            <asp:ControlParameter ControlID="selCorProd" Name="idCorFerragem" 
                PropertyName="IdCorFerragem" Type="Int32" />
            <asp:ControlParameter ControlID="selCorProd" Name="idCorAluminio" 
                PropertyName="IdCorAluminio" Type="Int32" />
            <asp:ControlParameter ControlID="txtEspessura" Name="espessura" 
                PropertyName="Text" Type="Single" />
            <asp:ControlParameter ControlID="selNaturezaOperacao" Name="idNaturezaOperacao" 
                PropertyName="CodigoNaturezaOperacao" Type="Int32" />
            <asp:ControlParameter ControlID="cblUfDestino" Name="ufsDestino" PropertyName="SelectedItem"/>
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server"
        SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        MaximumRowsParameterName="" SkinID="" StartRowIndexParameterName="">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoCliente" runat="server"
        SelectMethod="ObtemDescritoresTipoCliente" TypeName="Glass.Global.Negocios.IClienteFluxo" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        MaximumRowsParameterName="" SkinID="" StartRowIndexParameterName="">
    </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource ID="odsUf" runat="server" Culture="pt-BR"
        SelectMethod="ObtemUfs" TypeName="Glass.Global.Negocios.ILocalizacaoFluxo">
    </colo:VirtualObjectDataSource>
</asp:Content>
