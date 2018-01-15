<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlRodoviario.ascx.cs"
    Inherits="Glass.UI.Web.Controls.MDFe.ctrlRodoviario" %>

<%@ Register Src="~/Controls/MDFe/ctrlLacreRodoviarioMDFe.ascx" TagName="ctrlLacreRodoviario" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/MDFe/ctrlCIOTRodoviarioMDFe.ascx" TagName="ctrlCIOT" TagPrefix="uc3" %>
<%@ Register Src="~/Controls/MDFe/ctrlPedagioRodoviarioMDFe.ascx" TagName="ctrlPedagio" TagPrefix="uc4" %>
<%@ Register Src="~/Controls/MDFe/ctrlCondutorVeiculoMDFe.ascx" TagName="ctrlCondutorVeiculo" TagPrefix="uc5" %>
<%@ Register Src="~/Controls/MDFe/ctrlVeiculoRodoviarioMDFe.ascx" TagName="ctrlVeiculoReboque" TagPrefix="uc6" %>

<asp:DetailsView ID="dtvRodoviario" runat="server" AutoGenerateRows="false" GridLines="None" Width="100%" DefaultMode="Insert"
    CellPadding="0" CellSpacing="0" HeaderStyle-BorderWidth="0">
    <Fields>
        <asp:TemplateField ShowHeader="false">
            <InsertItemTemplate>
<div class="dtvSubTitulo">
    CIOT
</div>
<uc3:ctrlCIOT ID="ctrlCIOT" runat="server" IdRodoviario='<%# Bind("IdRodoviario") %>' CiotRodoviario='<%# Bind("CiotRodoviario") %>' />

<div class="dtvSubTitulo">
    Vale Pedágio
</div>
<uc4:ctrlPedagio ID="ctrlPedagio" runat="server" IdRodoviario='<%# Bind("IdRodoviario") %>' PedagioRodoviario='<%# Bind("PedagioRodoviario") %>' />
<div class="dtvSubTitulo">
    Veículo
</div>
<div class="dtvRow">
        <div class="dtvHeader">
            <asp:Label ID="Label1" runat="server" Text="Veiculo Tração *"></asp:Label>
        </div>
        <div class="dtvAlternatingRow">
            <asp:DropDownList ID="drpVeiculoTracao" runat="server" SelectedValue='<%# Bind("PlacaTracao") %>' Width="250px"
                DataSourceID="odsVeiculoTracao" DataTextField="Placa" DataValueField="Placa">
            </asp:DropDownList>
        </div>
</div>
<uc5:ctrlCondutorVeiculo ID="ctrlCondutorVeiculo" runat="server" IdRodoviario='<%# Bind("IdRodoviario") %>' CondutorVeiculo='<%# Bind("CondutorVeiculo") %>' />
<uc6:ctrlVeiculoReboque ID="ctrlVeiculoReboque" runat="server" IdRodoviario='<%# Bind("IdRodoviario") %>' VeiculoRodoviario='<%# Bind("VeiculoRodoviario") %>' />
<div class="dtvSubTitulo">
    Lacres
</div>
<uc2:ctrlLacreRodoviario ID="ctrlLacreRodoviario" runat="server" IdRodoviario='<%# Bind("IdRodoviario") %>' LacreRodoviario='<%# Bind("LacreRodoviario") %>' />
            </InsertItemTemplate>
            <EditItemTemplate>
<div class="dtvSubTitulo">
    CIOT
</div>
<uc3:ctrlCIOT ID="ctrlCIOT" runat="server" IdRodoviario='<%# Bind("IdRodoviario") %>' CiotRodoviario='<%# Bind("CiotRodoviario") %>' />

<div class="dtvSubTitulo">
    Vale Pedágio
</div>
<uc4:ctrlPedagio ID="ctrlPedagio" runat="server" IdRodoviario='<%# Bind("IdRodoviario") %>' PedagioRodoviario='<%# Bind("PedagioRodoviario") %>' />
<div class="dtvSubTitulo">
    Veículo
</div>
<div class="dtvRow">
        <div class="dtvHeader">
            <asp:Label ID="Label1" runat="server" Text="Veiculo Tração *"></asp:Label>
        </div>
        <div class="dtvAlternatingRow">
            <asp:DropDownList ID="drpVeiculoTracao" runat="server" SelectedValue='<%# Bind("PlacaTracao") %>' Width="250px"
                DataSourceID="odsVeiculoTracao" DataTextField="Placa" DataValueField="Placa">
            </asp:DropDownList>
        </div>
</div>
<uc5:ctrlCondutorVeiculo ID="ctrlCondutorVeiculo" runat="server" IdRodoviario='<%# Bind("IdRodoviario") %>' CondutorVeiculo='<%# Bind("CondutorVeiculo") %>' />
<uc6:ctrlVeiculoReboque ID="ctrlVeiculoReboque" runat="server" IdRodoviario='<%# Bind("IdRodoviario") %>' VeiculoRodoviario='<%# Bind("VeiculoRodoviario") %>' />
<div class="dtvSubTitulo">
    Lacres
</div>
<uc2:ctrlLacreRodoviario ID="ctrlLacreRodoviario" runat="server" IdRodoviario='<%# Bind("IdRodoviario") %>' LacreRodoviario='<%# Bind("LacreRodoviario") %>' />
            </EditItemTemplate>
        </asp:TemplateField>
    </Fields>
</asp:DetailsView>


<asp:ObjectDataSource ID="odsVeiculoTracao" runat="server"
    TypeName="Glass.Data.DAL.VeiculoDAO" DataObjectTypeName="Glass.Data.Model.Veiculo"
    SelectMethod="ObterVeiculoPorTipo">
    <SelectParameters>
        <asp:Parameter Name="tipoVeiculo" DefaultValue="0" />
    </SelectParameters>
</asp:ObjectDataSource>