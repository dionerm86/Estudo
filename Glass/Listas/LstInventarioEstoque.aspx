<%@ Page Title="Inventários de Estoque" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstInventarioEstoque.aspx.cs" Inherits="Glass.UI.Web.Listas.LstInventarioEstoque" %>

<%@ Register src="../Controls/ctrlLogCancPopup.ascx" tagname="ctrlLogCancPopup" tagprefix="uc1" %>

<%@ Register src="../Controls/ctrlSelGrupoSubgrupoProd.ascx" tagname="ctrlSelGrupoSubgrupoProd" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript">
        function abrirInventario(id)
        {
            var url = "../Cadastros/CadInventarioEstoque.aspx";
            if (!!id)
                url += "?id=" + id;

            openWindow(400, 800, url);
        }

        function finalizarInventario(id)
        {
            openWindow(600, 800, "../Cadastros/CadFinalizarInventario.aspx?id=" + id);
        }

        function confirmarInventario(id)
        {
            if (!confirm("Deseja confirmar esse inventário de estoque?"))
                return;

            bloquearPagina();
            var resposta = LstInventarioEstoque.Confirmar(id).value.split("|");

            desbloquearPagina(true);
            if (resposta[0] != "Ok")
                alert(resposta[1]);
            else
                atualizarPagina();
        }

        function abrirRelatorio(id)
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=InventarioEstoque&id=" + id);
        }

        function cancelar(id)
        {
            openWindow(200, 450, "../Utils/SetMotivoCancInventario.aspx?id=" + id);
        }
    </script>
    <div class="filtro">
        <div>
            <span>
                <asp:Label ID="Label2" runat="server" Text="Loja" AssociatedControlID="drpLoja"></asp:Label>
                <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" 
                DataTextField="NomeFantasia" DataValueField="IdLoja" 
                AppendDataBoundItems="True">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    CssClass="botaoPesquisar" onclick="imgPesq_Click" />
            </span>
            <span>
                <asp:Label ID="Label3" runat="server" Text="Situação" AssociatedControlID="drpSituacao"></asp:Label>
                <asp:DropDownList ID="drpSituacao" runat="server">
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem Value="1">Aberto</asp:ListItem>
                    <asp:ListItem Value="2">Em contagem</asp:ListItem>
                    <asp:ListItem Value="3">Finalizado</asp:ListItem>
                    <asp:ListItem Value="5">Confirmado</asp:ListItem>
                    <asp:ListItem Value="4">Cancelado</asp:ListItem>
                </asp:DropDownList>
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    CssClass="botaoPesquisar" onclick="imgPesq_Click" />
            </span>
        </div>
        <div>
            <span>
                <asp:Label ID="Label4" runat="server" Text="Grupo/Subgrupo" 
                    AssociatedControlID="ctrlSelGrupoSubgrupoProd"></asp:Label>
                <uc2:ctrlSelGrupoSubgrupoProd ID="ctrlSelGrupoSubgrupoProd" runat="server" 
                    ExibirGrupoProdutoVazio="True" ExibirSubgrupoProdutoVazio="True" />
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    CssClass="botaoPesquisar" onclick="imgPesq_Click" />
            </span>
        </div>
    </div>
    <div class="inserir">
        <asp:HyperLink ID="lnkInserir" runat="server" 
            NavigateUrl="~/Cadastros/CadInventarioEstoque.aspx"
            onclick="abrirInventario(); return false">Inserir inventário de estoque</asp:HyperLink>
    </div>
    <asp:GridView ID="grdInventarioEstoque" runat="server" AllowPaging="True" 
        AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
        DataKeyNames="Codigo" DataSourceID="odsInventarioEstoque" GridLines="None" 
        EmptyDataText="Não há inventários de estoque cadastrados." 
        onrowcommand="grdInventarioEstoque_RowCommand">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" 
                        ImageUrl="~/Images/EditarGrid.gif" 
                        onclientclick='<%# Eval("Codigo", "abrirInventario({0}); return false") %>' 
                        Visible='<%# ExibirEditar(Eval("Situacao")) %>' />
                    <asp:ImageButton ID="imgCancelar" runat="server" 
                        ImageUrl="~/Images/ExcluirGrid.gif" 
                        onclientclick='<%# Eval("Codigo", "cancelar({0}); return false;") %>' 
                        Visible='<%# ExibirCancelar(Eval("Situacao")) %>' />
                    <asp:ImageButton ID="imgRelatorio" runat="server" 
                        ImageUrl="~/Images/Relatorio.gif" 
                        onclientclick='<%# Eval("Codigo", "abrirRelatorio({0}); return false;") %>'
                        Visible='<%# ExibirRelatorio(Eval("Situacao")) %>' />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="Codigo" HeaderText="Código" 
                SortExpression="Codigo" />
            <asp:BoundField DataField="NomeLoja" HeaderText="Loja" ReadOnly="True" 
                SortExpression="NomeLoja" />
            <asp:TemplateField HeaderText="Grupo / Subgrupo" 
                SortExpression="DescricaoGrupoProduto">
                <EditItemTemplate>
                    <asp:Label ID="Label1" runat="server" 
                        Text='<%# Eval("DescricaoGrupoProduto") %>'></asp:Label>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" 
                        Text='<%# ObtemDescricaoGrupoSubgrupo(Eval("DescricaoGrupoProduto"), Eval("DescricaoSubgrupoProduto")) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="DescricaoSituacao" HeaderText="Situação" 
                SortExpression="DescricaoSituacao" />
            <asp:BoundField DataField="NomeFuncionarioCadastro" 
                HeaderText="Funcionário Cadastro" ReadOnly="True" 
                SortExpression="NomeFuncionarioCadastro" />
            <asp:BoundField DataField="DataCadastro" HeaderText="Data Cadastro" 
                SortExpression="DataCadastro" />
            <asp:BoundField DataField="NomeFuncionarioFinalizacao" 
                HeaderText="Funcionário Finalização" ReadOnly="True" 
                SortExpression="NomeFuncionarioFinalizacao" />
            <asp:BoundField DataField="DataFinalizacao" HeaderText="Data Finalização" 
                SortExpression="DataFinalizacao" />
            <asp:BoundField DataField="NomeFuncionarioConfirmacao" 
                HeaderText="Funcionário Confirmação" 
                SortExpression="NomeFuncionarioConfirmacao" />
            <asp:BoundField DataField="DataConfirmacao" HeaderText="Data Confirmação" 
                SortExpression="DataConfirmacao" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lnkIniciarContagem" runat="server" 
                        CommandArgument='<%# Eval("Codigo") %>' CommandName="IniciarContagem" 
                        onclientclick="if (!confirm(&quot;Deseja colocar esse inventário em contagem?&quot;)) return false;" 
                        Visible='<%# ExibirEmContagem(Eval("Situacao")) %>'>Iniciar Contagem</asp:LinkButton>
                    <asp:LinkButton ID="lnkFinalizar" runat="server" 
                        CommandArgument='<%# Eval("Codigo") %>'
                        OnClientClick='<%# Eval("Codigo", "finalizarInventario({0}); return false") %>'
                        Visible='<%# ExibirFinalizar(Eval("Situacao")) %>'>Finalizar</asp:LinkButton>
                    <asp:LinkButton ID="lnkConfirmar" runat="server" 
                        CommandArgument='<%# Eval("Codigo") %>'
                        OnClientClick='<%# Eval("Codigo", "confirmarInventario({0}); return false") %>'
                        Visible='<%# ExibirConfirmar(Eval("Situacao")) %>'>Confirmar</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" 
                        IdRegistro='<%# Eval("Codigo") %>' Tabela="InventarioEstoque" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <PagerStyle CssClass="pgr" />
        <AlternatingRowStyle CssClass="alt" />
    </asp:GridView>
    <colo:VirtualObjectDataSource ID="odsInventarioEstoque" runat="server" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        Culture="pt-BR" EnablePaging="True" 
        MaximumRowsParameterName="pageSize" SelectCountMethod="ObtemNumeroRegistros" 
        SelectMethod="ObtemItens" SortParameterName="sortExpression" 
        StartRowIndexParameterName="startRow" 
        TypeName="WebGlass.Business.InventarioEstoque.Fluxo.CRUD">
        <SelectParameters>
            <asp:ControlParameter ControlID="drpLoja" Name="codigoLoja" 
                PropertyName="SelectedValue" Type="UInt32" />
            <asp:ControlParameter ControlID="ctrlSelGrupoSubgrupoProd" Name="codigoGrupoProduto" 
                PropertyName="CodigoGrupoProduto" Type="UInt32" />
            <asp:ControlParameter ControlID="ctrlSelGrupoSubgrupoProd" Name="codigoSubgrupoProduto" 
                PropertyName="CodigoSubgrupoProduto" Type="UInt32" />
            <asp:ControlParameter ControlID="drpSituacao" Name="situacao" 
                PropertyName="SelectedValue" Type="Object" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource ID="odsLoja" runat="server" 
        Culture="pt-BR" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
    </colo:VirtualObjectDataSource>
</asp:Content>

