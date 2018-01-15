<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlSelParticipante.ascx.cs"
    Inherits="Glass.UI.Web.Controls.ctrlSelParticipante" %>

<asp:DropDownList ID="drpPart" runat="server" DataSourceID="odsTipoPart" DataTextField="Descr"
    DataValueField="Id" ondatabound="drpPart_DataBound">
</asp:DropDownList>
<asp:Label ID="lblDescrPart" runat="server"></asp:Label>
<asp:ImageButton ID="imgPart" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/Pesquisar.gif" />
<asp:CustomValidator ID="ctvPart" runat="server" ErrorMessage="Campo obrigatório"
    ControlToValidate="drpPart" ClientValidationFunction="ctrlSelParticipante_validaPart" ValidateEmptyText="true"
    Enabled="false" Display="Dynamic">*</asp:CustomValidator>
<asp:HiddenField ID="hdfIdPart" runat="server" />
<asp:HiddenField ID="hdfExibirAdminCartao" runat="server" Value="True" />
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoPart" runat="server" SelectMethod="GetTipoParticipante" 
    TypeName="Glass.Data.EFD.DataSourcesEFD" >
    <SelectParameters>
        <asp:ControlParameter ControlID="hdfExibirAdminCartao" Name="exibirAdminCartao" 
            PropertyName="Value" Type="Boolean" />
    </SelectParameters>
</colo:VirtualObjectDataSource>
