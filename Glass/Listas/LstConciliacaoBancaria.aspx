<%@ Page Title="Conciliação Bancária" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstConciliacaoBancaria.aspx.cs" Inherits="Glass.UI.Web.Listas.LstConciliacaoBancaria" %>

<%@ Register src="../Controls/ctrlLogCancPopup.ascx" tagname="ctrlLogCancPopup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function cancelar(codigo)
        {
            openWindow(300, 450, "../Utils/SetMotivoCancConciliacao.aspx?codigoConciliacao=" + codigo);
        }

        function anexar(codigo)
        {
            openWindow(600, 800, "../Cadastros/CadFotos.aspx?tipo=ConciliacaoBancaria&id=" + codigo);
        }

        function openRpt(codigo, exportarExcel) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ConciliacaoBancaria&id=" + codigo + "&exportarExcel=" + (exportarExcel == true));
        }
    </script>
    <div class="filtro">
        <div>
            <span>
                <asp:Label ID="Label1" runat="server" Text="Conta Bancária" AssociatedControlID="drpContaBanco"></asp:Label>
                <asp:DropDownList ID="drpContaBanco" runat="server" 
                    AppendDataBoundItems="True" DataSourceID="odsContaBanco" 
                    DataTextField="Descricao" DataValueField="IdContaBanco">
                    <asp:ListItem Value="0">Todas</asp:ListItem>
                </asp:DropDownList>
                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" CssClass="botaoPesquisar" />
            </span>
        </div>
    </div>
    <div class="inserir" style="<%= ExibirConciliar() %>">
        <asp:HyperLink ID="lnkInserir" runat="server" NavigateUrl="~/Cadastros/CadConciliacaoBancaria.aspx">Realizar Conciliação Bancária</asp:HyperLink>
    </div>
    <asp:GridView ID="grdConciliacaoBancaria" runat="server" AllowPaging="True" AllowSorting="True"
        AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="Codigo" DataSourceID="odsConciliacaoBancaria"
        GridLines="None" 
        EmptyDataText="Não há conciliações bancárias cadastradas." 
        onrowdatabound="grdConciliacaoBancaria_RowDataBound">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton ID="imgRelatorio" runat="server" ImageUrl="~/Images/Relatorio.gif"
                        onclientclick='<%# Eval("Codigo", "openRpt({0}); return false") %>' ToolTip="Relatório" />
                         <asp:ImageButton ID="imgRelatorioExcel" runat="server" ImageUrl="~/Images/Excel.gif"
                        onclientclick='<%# Eval("Codigo", "openRpt({0}, true); return false") %>' ToolTip="Exportar relatório para Excel"
                        Visible='<%# Eval("ExibirExportarExel") %>' />
                    <asp:ImageButton ID="imgAnexar" runat="server" ImageUrl="~/Images/Clipe.gif" 
                        onclientclick='<%# Eval("Codigo", "anexar({0}); return false") %>' ToolTip="Anexos" />
                    <asp:ImageButton ID="imgCancelar" runat="server" 
                        ImageUrl="~/Images/ExcluirGrid.gif" Visible='<%# Eval("PodeCancelar") %>'
                        onclientclick='<%# Eval("Codigo", "cancelar({0}); return false") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Codigo" HeaderText="Código" SortExpression="Codigo" />
            <asp:BoundField DataField="DescricaoContaBancaria" HeaderText="Conta Bancária" 
                SortExpression="DescricaoContaBancaria" />
            <asp:BoundField DataField="DataConciliada" HeaderText="Data Conciliada" 
                SortExpression="DataConciliada" DataFormatString="{0:d}" />
            <asp:BoundField DataField="DescricaoSituacao" HeaderText="Situação" 
                SortExpression="Situacao" />
            <asp:BoundField DataField="NomeFuncionarioCadastro" HeaderText="Funcionário Responsável"
                ReadOnly="True" SortExpression="NomeFuncionarioCadastro" />
            <asp:BoundField DataField="DataCadastro" HeaderText="Data Conciliação" SortExpression="DataCadastro" />
            <asp:TemplateField>
                <ItemTemplate>
                    <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" 
                        IdRegistro='<%# Eval("Codigo") %>' Tabela="ConciliacaoBancaria" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <PagerStyle CssClass="pgr" />
        <AlternatingRowStyle CssClass="alt" />
    </asp:GridView>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsConciliacaoBancaria" runat="server" EnablePaging="True"
        MaximumRowsParameterName="pageSize" SelectCountMethod="ObtemNumeroConciliacoesBancarias"
        SelectMethod="ObtemListaConciliacoesBancarias" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="WebGlass.Business.ConciliacaoBancaria.Fluxo.CRUD"
        >
        <SelectParameters>
            <asp:ControlParameter ControlID="drpContaBanco" Name="codigoContaBancaria" 
                PropertyName="SelectedValue" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" 
    SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.ContaBancoDAO" 
    >
    </colo:VirtualObjectDataSource>
</asp:Content>
