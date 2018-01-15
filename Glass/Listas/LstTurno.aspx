<%@ Page Title="Turnos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstTurno.aspx.cs" Inherits="Glass.UI.Web.Listas.LstTurno" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaTurno&exportarExcel=" + exportarExcel);
        }
        
    </script>

    <div class="inserir">
        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkInserir_Click">Inserir Turno</asp:LinkButton>
    </div>
    <asp:GridView ID="grdTurno" runat="server" SkinID="defaultGridView"
        DataKeyNames="IdTurno" DataSourceID="odsTurno"
        EmptyDataText="Não existem turnos cadastrados">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:HyperLink ID="lnkEditar" runat="server" NavigateUrl='<%# "../Cadastros/CadTurno.aspx?idTurno=" + Eval("IdTurno") %>'
                        ToolTip="Editar">
                        <img alt="Editar" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                    <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                        OnClientClick="return confirm(&quot;Tem certeza que deseja excluir esta Turno?&quot;);"
                        ToolTip="Excluir" />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
            <asp:TemplateField HeaderText="Turno" SortExpression="Sequencia">
                <ItemTemplate>
                    <%# Colosoft.Translator.Translate(Eval("NumSeq")).Format() %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Inicio" HeaderText="Início" SortExpression="Inicio" />
            <asp:BoundField DataField="Termino" HeaderText="Término" SortExpression="Termino" />
        </Columns>
    </asp:GridView>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTurno" runat="server" 
        DataObjectTypeName="Glass.Global.Negocios.Entidades.Turno"
        DeleteMethod="ApagarTurno" DeleteStrategy="GetAndDelete"
        SelectMethod="PesquisarTurnos" 
        SelectByKeysMethod="ObtemTurno"
        TypeName="Glass.Global.Negocios.ITurnoFluxo"
        EnablePaging="True" MaximumRowsParameterName="pageSize"
        SortParameterName="sortExpression">
    </colo:VirtualObjectDataSource>
    <div class="imprimir">
        <span>
            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> 
            <img alt="Imprimir" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
        </span>
        <span>
            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;">
            <img alt="Exportar para o Excel" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
        </span>
    </div>
   
</asp:Content>
