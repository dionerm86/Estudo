<%@ Page Title="Produção por Setor/Pedido" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="PedidosCapacidadeProducao.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.PedidosCapacidadeProducao" %>

<%@ Register src="../../Controls/ctrlData.ascx" tagname="ctrlData" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    
    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var data = FindControl("ctrlData_txtData", "input").value;
            var horaInicial = FindControl("txtHoraInicial", "input").value;
            var horaFinal = FindControl("txtHoraFinal", "input").value;
            var idSetor = FindControl("drpSetor", "select").value;

            openWindow(600, 800, "RelBase.aspx?rel=CapacidadeProducaoPedido&data=" + data + "&horaInicial=" + horaInicial + "&horaFinal=" + horaFinal + "&idSetor=" + idSetor + "&exportarExcel=" + exportarExcel);
        }

        function validaHora(controle) {
            if (controle != null && controle.value != null && controle.value != "" && !isHoraValida(controle.value)) {
                alert('Horário inválido');
                controle.value = "";
            }
        }
            
    </script>

    <div class="filtro">
        <div>
            <span>
                <asp:Label ID="Label1" runat="server" Text="Data" AssociatedControlID="ctrlData"></asp:Label>
                <uc1:ctrlData ID="ctrlData" runat="server" />
                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" CssClass="botaoPesquisar" onclick="imgPesq_Click" />
            </span>
            <span>
                <asp:Label ID="Label3" runat="server" Text="Horário" AssociatedControlID="txtHoraInicial"></asp:Label>
                <asp:TextBox ID="txtHoraInicial" runat="server" Width="44px" MaxLength="5" onkeydown="return mascara_hora(event, this)"
                    onkeypress="return soNumeros(event, true, true)" onblur="validaHora(this); return false;"></asp:TextBox>
                <asp:Label ID="Label4" runat="server" Text="até" AssociatedControlID="txtHoraFinal"></asp:Label>
                <asp:TextBox ID="txtHoraFinal" runat="server" Width="44px" MaxLength="5" onkeydown="return mascara_hora(event, this)"
                    onkeypress="return soNumeros(event, true, true)" onblur="validaHora(this); return false;"></asp:TextBox>
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" CssClass="botaoPesquisar" onclick="imgPesq_Click" />
            </span>
            <span>
                <asp:Label ID="Label2" runat="server" Text="Setor" AssociatedControlID="drpSetor"></asp:Label>
                <asp:DropDownList ID="drpSetor" runat="server" AppendDataBoundItems="True" 
                DataSourceID="odsSetor" DataTextField="Descricao" DataValueField="IdSetor">
                    <asp:ListItem Value="0">Todos</asp:ListItem>
                </asp:DropDownList>
                <asp:ImageButton ID="ImageButton1" runat="server" 
                    ImageUrl="~/Images/Pesquisar.gif" CssClass="botaoPesquisar" 
                    onclick="imgPesq_Click" />
            </span>
        </div>
    </div>
    <asp:GridView ID="grdCapacidadeProducaoPedido" runat="server" 
        AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" 
        CssClass="gridStyle" DataSourceID="odsCapacidadeProducaoPedido" 
        GridLines="None" PageSize="15">
        <Columns>
            <asp:BoundField DataField="IdPedido" HeaderText="Pedido" 
                SortExpression="IdPedido" />
            <asp:BoundField DataField="NomeSetor" HeaderText="Setor" 
                SortExpression="IdSetor" />
            <asp:BoundField DataField="TotM" HeaderText="M²" SortExpression="TotM" />
        </Columns>
        <PagerStyle CssClass="pgr" />
        <AlternatingRowStyle CssClass="alt" />
    </asp:GridView>
    <div>
        <br />
        <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false">
            <img src="../../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
        &nbsp;&nbsp;&nbsp;
        <asp:LinkButton ID="lnkExcel" runat="server" OnClientClick="openRpt(true); return false">
            <img src="../../Images/Excel.gif" border="0" /> Exportar para o Excel</asp:LinkButton>
    </div>
    <colo:VirtualObjectDataSource ID="odsCapacidadeProducaoPedido" runat="server" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        Culture="pt-BR" MaximumRowsParameterName="pageSize" 
        SelectCountMethod="ObtemNumeroPedidosCapacidadeProducao" 
        SelectMethod="ObtemListaPedidosCapacidadeProducao" SkinID="" 
        SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
        TypeName="Glass.Data.RelDAL.CapacidadeProducaoPedidoDAO" 
        EnablePaging="True">
        <SelectParameters>
            <asp:ControlParameter ControlID="ctrlData" Name="dataProducao" PropertyName="Data" Type="DateTime" />
            <asp:ControlParameter ControlID="txtHoraInicial" Name="horaInicial" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtHoraFinal" Name="horaFinal" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpSetor" Name="idSetor" PropertyName="SelectedValue" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource ID="odsSetor" runat="server" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        Culture="" MaximumRowsParameterName="" SelectMethod="GetOrdered" SkinID="" 
        StartRowIndexParameterName="" TypeName="Glass.Data.DAL.SetorDAO">
    </colo:VirtualObjectDataSource>
</asp:Content>

