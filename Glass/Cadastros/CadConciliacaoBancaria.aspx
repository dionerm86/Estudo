<%@ Page Title="Realizar Conciliação Bancária" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadConciliacaoBancaria.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadConciliacaoBancaria" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register src="../Controls/ctrlDivFlutuante.ascx" tagname="ctrlDivFlutuante" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    
    <script type="text/javascript">
        function conciliar()
        {
            var data = FindControl("hdfDataConciliacao", "input").value;
            if (!confirm("A conciliação até o dia " + data + " está correta?"))
                return false;

            bloquearPagina();
            desbloquearPagina(false);
            return true;
        }
    </script>
    <div class="filtro">
        <div>
            <span>
                <asp:Label ID="Label2" runat="server" Text="Conta bancária" AssociatedControlID="drpContaBanco"></asp:Label>
                <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                    DataTextField="Descricao" DataValueField="IdContaBanco" 
                ValidationGroup="filtro">
                </asp:DropDownList>
                <asp:ImageButton ID="imgPesq" runat="server" CssClass="botaoPesquisar"
                    ImageUrl="~/Images/Pesquisar.gif" onclick="imgPesq_Click" 
                ValidationGroup="filtro" />
            </span>
            <span>
                <asp:Label ID="Label1" runat="server" Text="Data de Término" AssociatedControlID="ctrlData"></asp:Label>
                <uc1:ctrlData ID="ctrlData" runat="server" ValidateEmptyText="True" 
                ValidationGroup="filtro" />
                <asp:ImageButton ID="imgPesq0" runat="server" CssClass="botaoPesquisar"
                    ImageUrl="~/Images/Pesquisar.gif" onclick="imgPesq_Click" 
                ValidationGroup="filtro" />
            </span>
        </div>
    </div>
    <asp:GridView ID="grdMovBanco" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
        DataKeyNames="CodigoMovimentacao" DataSourceID="odsMovBanco" 
        GridLines="None" ondatabound="grdMovBanco_DataBound" 
        EmptyDataText="Não há movimentação nesta conta.">
        <Columns>
            <asp:BoundField DataField="ReferenciaMovimentacao" HeaderText="Ref." ReadOnly="True"
                SortExpression="ReferenciaMovimentacao" />
            <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" ReadOnly="True" SortExpression="NomeCliente" />
            <asp:BoundField DataField="NomeFornecedor" HeaderText="Fornecedor" ReadOnly="True"
                SortExpression="NomeFornecedor" />
            <asp:BoundField DataField="NomePlanoContas" HeaderText="Plano de Conta" ReadOnly="True"
                SortExpression="NomePlanoContas" />
            <asp:BoundField DataField="ValorMovimentacao" DataFormatString="{0:c}" HeaderText="Valor"
                ReadOnly="True" SortExpression="ValorMovimentacao" />
            <asp:BoundField DataField="ValorJuros" DataFormatString="{0:c}" HeaderText="Juros"
                ReadOnly="True" SortExpression="ValorJuros" />
            <asp:BoundField DataField="DataMovimentacao" HeaderText="Data" ReadOnly="True" SortExpression="DataMovimentacao" />
            <asp:BoundField DataField="SaldoAposMovimentacao" HeaderText="Saldo" ReadOnly="True"
                SortExpression="SaldoAposMovimentacao" DataFormatString="{0:c}" />
            <asp:BoundField DataField="Observacao" HeaderText="Observação" ReadOnly="True" SortExpression="Observacao" />
        </Columns>
        <AlternatingRowStyle CssClass="alt" />
    </asp:GridView>
    <asp:Button ID="btnConciliar" runat="server" Text="Realizar conciliação" 
        onclientclick="if (!conciliar()) return false;" 
        onclick="btnConciliar_Click" />
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMovBanco" runat="server" SelectMethod="ObtemMovimentacoesParaConciliacao"
        TypeName="WebGlass.Business.MovimentacaoBancaria.Fluxo.BuscarEValidar" >
        <SelectParameters>
            <asp:ControlParameter ControlID="drpContaBanco" Name="codigoContaBanco" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter ControlID="ctrlData" Name="dataTermino" PropertyName="Data"
                Type="DateTime" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.ContaBancoDAO" >
    </colo:VirtualObjectDataSource>
    <asp:HiddenField ID="hdfCodigoContaBancaria" runat="server" />
    <asp:HiddenField ID="hdfDataConciliacao" runat="server" />
</asp:Content>
