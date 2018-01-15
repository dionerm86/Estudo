<%@ Page Title="DIPJ (Entrada de Produtos)" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="EntradaProdutos.aspx.cs" Inherits="Glass.UI.Web.Relatorios.DIPJ.EntradaProdutos" %>
<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <script type="text/javascript">
        function imprimir(exportarExcel) {
            
            var dataInicial = FindControl("ctrlDataEmissaoInicial", "input").value;
            var dataFinal = FindControl("ctrlDataEmissaoFinal", "input").value;

            openWindow(600, 800, '../RelBase.aspx?rel=DIPJEntradaProdutos' + 
                '&dataEmissaoInicial=' + dataInicial + '&dataEmissaoFinal=' + dataFinal + '&exportarExcel=' + exportarExcel);
        }
    </script>
    <div>
        <label>Período de Entrada/Saída: </label>
        <uc2:ctrlData ID="ctrlDataEmissaoInicial" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" 
                      EnableViewState="false" />
        <uc2:ctrlData ID="ctrlDataEmissaoFinal" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                      EnableViewState="false" />
        <input type="submit" class="botaoPesquisar" />
    </div>
    <div style="margin-top: 20px;">
         <asp:GridView GridLines="None" ID="grdEntradaProdutos" runat="server" AutoGenerateColumns="False"
                    EnableViewState="false"
                    DataSourceID="odsEntradaProdutos" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Nenhum registro encontrado."
                    AllowPaging="True">
            <PagerSettings PageButtonCount="20" />
            <Columns>
                <asp:BoundField HeaderText="Classificação Fiscal" DataField="Id" />
                <asp:BoundField HeaderText="Mercadoria" DataField="Nome" />
                <asp:BoundField HeaderText="Valor Contábil" DataField="ValorContabil" DataFormatString="{0:C}" />
                <asp:BoundField HeaderText="Valor IPI" DataField="ValorIPI" DataFormatString="{0:C}" />
                <asp:BoundField HeaderText="Valor Contábil - IPI" DataField="ValorContabilIPI" DataFormatString="{0:C}" />
            </Columns>
        </asp:GridView>
        <colo:VirtualObjectDataSource ID="odsEntradaProdutos" Culture="pt-BR"
            runat="server" TypeName="Glass.Data.RelDAL.DipjDAO"
            SelectMethod="ObtemEntradaProdutos" MaximumRowsParameterName="pageSize" EnableViewState="false"
            EnablePaging="true">
            <SelectParameters>
                <asp:ControlParameter Name="dataEmissaoInicial" ControlID="ctrlDataEmissaoInicial" PropertyName="DataString" />
                <asp:ControlParameter Name="dataEmissaoFinal" ControlID="ctrlDataEmissaoFinal" PropertyName="DataString" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
    </div>
    <div>
        <a href="#" onclick="return imprimir(false);"><img border="0" src="<%= ResolveUrl("~") %>Images/printer.png" alt="Imprimir" /> Imprimir</a>
        <a href="#" onclick="return imprimir(true);"><img border="0" src="<%= ResolveUrl("~") %>Images/Excel.gif" alt="Exporta Excel" /> Exportar Excel</a>
    </div>
</asp:Content>