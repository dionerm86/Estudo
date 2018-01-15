<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Proprietario.ascx.cs"
    Inherits="Glass.UI.Web.Controls.CTe.Proprietario" %>
<div class="dtvRow">
    <div class="dtvHeader">
        <asp:Label ID="Label17" runat="server" Text="Cpf"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox ID="txtCpf" CssClass="cpf" runat="server" MaxLength="50" Width="120px"
            onkeypress="maskCPF(event, this)"></asp:TextBox>
    </div>
    <div class="dtvHeader">
        <asp:Label ID="Label18" runat="server" Text="Cnpj"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox ID="txtCnpj" CssClass="cnpj" runat="server" MaxLength="50" Width="120px"
            onkeypress="maskCNPJ(event, this)"></asp:TextBox>
    </div>
</div>
<div class="dtvRow">
    <div class="dtvHeader">
        <asp:Label ID="Label36" runat="server" Text="RNTRC"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox ID="txtRntrc" runat="server" MaxLength="50" Width="200px"></asp:TextBox>
    </div>
    <div class="dtvHeader">
        <asp:Label ID="Label54" runat="server" Text="Nome"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox ID="txtNomeProprietario" runat="server" MaxLength="50" Width="300px"></asp:TextBox>
    </div>
</div>
<div class="dtvRow">
    <div class="dtvHeader">
        <asp:Label ID="Label69" runat="server" Text="Inscrição Estadual"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox ID="txtInscricaoEstadual" runat="server" MaxLength="50" Width="200px"></asp:TextBox>
    </div>
    <div class="dtvHeader">
        <asp:Label ID="Label1" runat="server" Text="Uf"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:DropDownList ID="drpUf" runat="server" Height="20px" Width="45px" AppendDataBoundItems="True"
            Enabled="true" Visible="true">
            <asp:ListItem>AC</asp:ListItem>
            <asp:ListItem>AL</asp:ListItem>
            <asp:ListItem>AM</asp:ListItem>
            <asp:ListItem>AP</asp:ListItem>
            <asp:ListItem>BA</asp:ListItem>
            <asp:ListItem>CE</asp:ListItem>
            <asp:ListItem>DF</asp:ListItem>
            <asp:ListItem>ES</asp:ListItem>
            <asp:ListItem>GO</asp:ListItem>
            <asp:ListItem>MA</asp:ListItem>
            <asp:ListItem>MG</asp:ListItem>
            <asp:ListItem>MS</asp:ListItem>
            <asp:ListItem>MT</asp:ListItem>
            <asp:ListItem>PB</asp:ListItem>
            <asp:ListItem>PA</asp:ListItem>
            <asp:ListItem>PE</asp:ListItem>
            <asp:ListItem>PI</asp:ListItem>
            <asp:ListItem>PR</asp:ListItem>
            <asp:ListItem>RJ</asp:ListItem>
            <asp:ListItem>RN</asp:ListItem>
            <asp:ListItem>RO</asp:ListItem>
            <asp:ListItem>RR</asp:ListItem>
            <asp:ListItem>RS</asp:ListItem>
            <asp:ListItem>SC</asp:ListItem>
            <asp:ListItem>SP</asp:ListItem>
            <asp:ListItem>SE</asp:ListItem>
            <asp:ListItem>TO</asp:ListItem>
        </asp:DropDownList>
    </div>
</div>
<div class="dtvRow">
    <div class="dtvHeader">
        <asp:Label ID="Label2" runat="server" Text="Tipo Proprietário"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:DropDownList ID="drpTipoProp" runat="server" Height="20px" Width="150px" AppendDataBoundItems="True"
            Enabled="true" Visible="true">
            <asp:ListItem Text="TAC - Agregado" Value="0"></asp:ListItem>
            <asp:ListItem Text="TAC Independente" Value="1"></asp:ListItem>
            <asp:ListItem Text="Outros" Value="2"></asp:ListItem>
        </asp:DropDownList>
    </div>
</div>
