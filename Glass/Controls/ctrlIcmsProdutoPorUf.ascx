<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlIcmsProdutoPorUf.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlIcmsProdutoPorUf" %>
<div id="<%= this.ClientID %>" style="display: inline-block">
    <div id="<%= this.ClientID %>_geral">
        <span>
            <asp:Label ID="lblIcmsIntra" runat="server" Text="Alíquota ICMS Intraestadual"></asp:Label>
            <asp:TextBox ID="txtIcmsIntra" runat="server" Width="60px"
                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
        </span>
        <span style="margin-left: 10px">
            <asp:Label ID="lblIcmsInter" runat="server" Text="Alíquota ICMS Interestadual"></asp:Label>
            <asp:TextBox ID="txtIcmsInter" runat="server" Width="60px"
                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
        </span>
        <span style="margin-left: 10px">
            <asp:Label ID="lblIcmsInternaDest" runat="server" Text="Alíquota ICMS Interna Destinatário"></asp:Label>
            <asp:TextBox ID="txtIcmsInternaDest" runat="server" Width="60px"
                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
        </span>
        <asp:CustomValidator ID="ctvGeral" runat="server" ErrorMessage="ICMS - Geral: Indique as alíquotas de ICMS intraestadual e interestadual"
            Text="*" ControlToValidate="txtIcmsIntra" ValidateEmptyText="true"></asp:CustomValidator>
        <span>
            <asp:Label ID="lblFCPIntra" runat="server" Text="Alíquota FCP Intraestadual"></asp:Label>
            <asp:TextBox ID="txtFCPIntra" runat="server" Width="60px"
                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
        </span>
        <span style="margin-left: 10px">
            <asp:Label ID="lblFCPInter" runat="server" Text="Alíquota FCP Interestadual"></asp:Label>
            <asp:TextBox ID="txtFCPInter" runat="server" Width="60px"
                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
        </span>
    </div>
    <div id="<%= this.ClientID %>_excecoes" style="border-top: 1px dotted gray; padding-top: 3px; margin-top: 3px">
        <span style="position: absolute; margin-top: 7px">
            <asp:Label ID="lblExcecoes" runat="server" Text="Exceções" Font-Bold="true" EnableViewState="false"></asp:Label>
        </span>
        <div id="<%= this.ClientID %>_excecoes_item0" style="margin-left: 70px">
            <span>
                <asp:Label ID="item0_lblTipoCliente" runat="server" Text="Tipo Cliente" EnableViewState="false"></asp:Label>
                <asp:DropDownList ID="item0_drpTipoCliente" runat="server" DataSourceID="odsTipoCliente" EnableViewState="false" 
                    DataTextField="Name" DataValueField="Id" AppendDataBoundItems="true">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
            </span>
            <span style="margin-left: 10px">
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
                <asp:Label ID="item0_lblIcmsIntraExcecao" runat="server" Text="Alíq. ICMS Intra." EnableViewState="false"></asp:Label>
                <asp:TextBox ID="item0_txtIcmsIntraExcecao" runat="server" Width="60px" EnableViewState="false"
                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
            </span>
            <span style="margin-left: 10px">
                <asp:Label ID="item0_lblIcmsInterExcecao" runat="server" Text="Alíq. ICMS Inter." EnableViewState="false"></asp:Label>
                <asp:TextBox ID="item0_txtIcmsInterExcecao" runat="server" Width="60px" EnableViewState="false"
                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
            </span>
            <span style="margin-left: 10px">
                <asp:Label ID="item0_lblIcmsInternaDestExcecao" runat="server" Text="Alíq. ICMS Interna Dest." EnableViewState="false"></asp:Label>
                <asp:TextBox ID="item0_txtIcmsInternaDestExcecao" runat="server" Width="60px" EnableViewState="false"
                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
            </span>
            <span style="margin-left: 10px">
                <asp:Label ID="item0_lblFCPIntraExcecao" runat="server" Text="Alíq. FCP Intra." EnableViewState="false"></asp:Label>
                <asp:TextBox ID="item0_txtFCPIntraExcecao" runat="server" Width="60px" EnableViewState="false"
                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
            </span>
            <span style="margin-left: 10px">
                <asp:Label ID="item0_lblFCPInterExcecao" runat="server" Text="Alíq. FCP Inter." EnableViewState="false"></asp:Label>
                <asp:TextBox ID="item0_txtFCPInterExcecao" runat="server" Width="60px" EnableViewState="false"
                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
            </span>
            <span>
                <asp:CustomValidator ID="item0_ctvExcecao" runat="server" ErrorMessage="ICMS - Exceção 1: Selecione todos os itens desta linha." 
                    ValidateEmptyText="true" Text="*" ControlToValidate="item0_drpUfOrigemExcecao" Display="Dynamic"></asp:CustomValidator>
                <asp:ImageButton ID="item0_imgAdicionarExcecao" runat="server" ImageUrl="~/Images/Insert.gif" ImageAlign="AbsBottom" />
                <asp:ImageButton ID="item0_imgExcluirExcecao" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                    ImageAlign="AbsBottom" style="visibility: hidden" />
            </span>
        </div>
    </div>
</div>
<colo:VirtualObjectDataSource ID="odsTipoCliente" runat="server" culture="pt-BR"
    SelectMethod="ObtemDescritoresTipoCliente" TypeName="Glass.Global.Negocios.IClienteFluxo">
</colo:VirtualObjectDataSource>
<colo:VirtualObjectDataSource ID="odsUf" runat="server" culture="pt-BR"
    SelectMethod="ObtemUfs" TypeName="Glass.Global.Negocios.ILocalizacaoFluxo">
</colo:VirtualObjectDataSource>
<asp:HiddenField ID="hdfExcecoes" runat="server" />
<script type="text/javascript">
    var <%= this.ClientID %> = new IcmsProdutoPorUfType("<%= this.ClientID %>");
</script>