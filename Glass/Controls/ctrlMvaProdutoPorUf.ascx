<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlMvaProdutoPorUf.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlMvaProdutoPorUf" %>
<div id="<%= this.ClientID %>" style="display: inline-block">
    <div id="<%= this.ClientID %>_geral">
        <span>
            <asp:Label ID="lblMvaOriginal" runat="server" Text="MVA Original"></asp:Label>
            <asp:TextBox ID="txtMvaOriginal" runat="server" Width="60px"
                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
        </span>
        <span style="margin-left: 10px">
            <asp:Label ID="lblMvaSimples" runat="server" Text="MVA Clientes Optantes pelo Simples"></asp:Label>
            <asp:TextBox ID="txtMvaSimples" runat="server" Width="60px"
                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
        </span>
        <asp:CustomValidator ID="ctvGeral" runat="server" ErrorMessage="MVA - Geral: Selecione os dados de MVA Original e para Clientes Optantes pelo Simples"
            Text="*" ControlToValidate="txtMvaOriginal" ValidateEmptyText="true"></asp:CustomValidator>
    </div>
    <div id="<%= this.ClientID %>_excecoes" style="border-top: 1px dotted gray; padding-top: 3px; margin-top: 3px">
        <span style="position: absolute; margin-top: 7px">
            <asp:Label ID="lblExcecoes" runat="server" Text="Exceções" Font-Bold="true" EnableViewState="false"></asp:Label>
        </span>
        <div id="<%= this.ClientID %>_excecoes_item0" style="margin-left: 70px">
            <span>
                <asp:Label ID="item0_lblUfOrigemExcecao" runat="server" Text="UF Origem" EnableViewState="false"></asp:Label>
                <asp:DropDownList ID="item0_drpUfOrigemExcecao" runat="server" DataSourceID="odsUf" EnableViewState="false" 
                    DataTextField="Name" DataValueField="Name" AppendDataBoundItems="true">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
            </span>
            <span style="margin-left: 10px">
                <asp:Label ID="item0_lblUfDestinoExcecao" runat="server" Text="UF Destino" EnableViewState="false"></asp:Label>
                <asp:DropDownList ID="item0_drpUfDestinoExcecao" runat="server" DataSourceID="odsUf" EnableViewState="false" 
                    DataTextField="Name" DataValueField="Name" AppendDataBoundItems="true">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
            </span>
            <span style="margin-left: 10px">
                <asp:Label ID="item0_lblMvaOriginalExcecao" runat="server" Text="MVA Orig." EnableViewState="false"></asp:Label>
                <asp:TextBox ID="item0_txtMvaOriginalExcecao" runat="server" Width="60px" EnableViewState="false"
                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
            </span>
            <span style="margin-left: 10px">
                <asp:Label ID="item0_lblMvaSimplesExcecao" runat="server" Text="MVA Simples" EnableViewState="false"></asp:Label>
                <asp:TextBox ID="item0_txtMvaSimplesExcecao" runat="server" Width="60px" EnableViewState="false"
                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
            </span>
            <span>
                <asp:CustomValidator ID="item0_ctvExcecao" runat="server" ErrorMessage="MVA - Exceção 1: Selecione todos os itens desta linha." ValidateEmptyText="true" 
                    Text="*" ControlToValidate="item0_drpUfOrigemExcecao" Display="Dynamic"></asp:CustomValidator>
                <asp:ImageButton ID="item0_imgAdicionarExcecao" runat="server" ImageUrl="~/Images/Insert.gif"
                    ImageAlign="AbsBottom" />
                <asp:ImageButton ID="item0_imgExcluirExcecao" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                    ImageAlign="AbsBottom" style="visibility: hidden" />
            </span>
        </div>
    </div>
</div>
<colo:VirtualObjectDataSource ID="odsUf" runat="server" culture="pt-BR"
    SelectMethod="ObtemUfs" 
    TypeName="Glass.Global.Negocios.ILocalizacaoFluxo">
</colo:VirtualObjectDataSource>
<asp:HiddenField ID="hdfExcecoes" runat="server" />
<script type="text/javascript">
    var <%= this.ClientID %> = new MvaProdutoPorUfType("<%= this.ClientID %>");
</script>