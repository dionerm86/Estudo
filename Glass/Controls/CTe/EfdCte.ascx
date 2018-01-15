<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EfdCte.ascx.cs" Inherits="Glass.UI.Web.Controls.CTe.EfdCte" %>
<%@ Register Src="../ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<div class="dtvRow">
    <div class="dtvHeader">
        Natureza BC do Crédito
        <%= ObtemTextoCampoObrigatorio(selNatBcCred.Validador) %>
    </div>
    <div class="dtvAlternatingRow">
        <uc1:ctrlSelPopup ID="selNatBcCred" runat="server" DataSourceID="odsNaturezaBcCredito"
            DataTextField="Descr" DataValueField="Id" FazerPostBackBotaoPesquisar="False"
            TextWidth="200px" TituloTela="Selecione a Natureza da Base de Cálculo do Crédito"
            PermitirVazio="false" ErrorMessage="Selecione a natureza da base de cálculo do crédito"
            ValidationGroup="c" />
    </div>
    <div class="dtvHeader">
        Indicador Natureza Frete
        <%= ObtemTextoCampoObrigatorio(selIndNatFrete.Validador) %>
    </div>
    <div class="dtvAlternatingRow">
        <uc1:ctrlSelPopup ID="selIndNatFrete" runat="server" DataSourceID="odsIndNaturezaFrete"
            DataTextField="Descr" DataValueField="Id" FazerPostBackBotaoPesquisar="False"
            TextWidth="200px" TituloTela="Selecione o Indicador da Natureza do Frete" PermitirVazio="false"
            ErrorMessage="Selecione o indicador da natureza do frete" ValidationGroup="c" />
    </div>
</div>
<div class="dtvRow">
    <div class="dtvHeader">
        Tipo de Contribuição Social
        <%= ObtemTextoCampoObrigatorio(selCodCont.Validador) %>
    </div>
    <div class="dtvAlternatingRow">
        <uc1:ctrlSelPopup ID="selCodCont" runat="server" DataSourceID="odsCodCont" DataTextField="Descr"
            DataValueField="Id" FazerPostBackBotaoPesquisar="False" TextWidth="200px" TituloTela="Selecione o Tipo de Contribuição Social"
            ErrorMessage="Selecione o tipo de contribuição social" PermitirVazio="false"
            ValidationGroup="c" />
    </div>
    <div class="dtvHeader">
        Tipo de Crédito
        <%= ObtemTextoCampoObrigatorio(selCodCred.Validador) %>
    </div>
    <div class="dtvAlternatingRow">
        <uc1:ctrlSelPopup ID="selCodCred" runat="server" DataSourceID="odsCodCred" DataTextField="Descr"
            DataValueField="Id" FazerPostBackBotaoPesquisar="False" TextWidth="200px" TituloTela="Selecione o Tipo de Crédito"
            ErrorMessage="Selecione o tipo de crédito" PermitirVazio="false" ValidationGroup="c" />
    </div>
</div>
<div class="dtvRow">
    <div class="dtvHeader">
        Plano de Conta Contábil
    </div>
    <div class="dtvAlternatingRow">
        <asp:DropDownList ID="drpContaContabil" runat="server" AppendDataBoundItems="True"
            DataSourceID="odsPlanoContaContabil" DataTextField="Descricao" DataValueField="IdContaContabil">
            <asp:ListItem></asp:ListItem>
        </asp:DropDownList>
    </div>
</div>
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPlanoContaContabil" runat="server"
    SelectMethod="GetSorted" TypeName="Glass.Data.DAL.PlanoContaContabilDAO">
    <SelectParameters>
        <asp:Parameter DefaultValue="0" Name="natureza" Type="Int32" />
    </SelectParameters>
</colo:VirtualObjectDataSource>
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsNaturezaBcCredito" runat="server"
    SelectMethod="GetNaturezaBcCredito" TypeName="Glass.Data.EFD.DataSourcesEFD">
</colo:VirtualObjectDataSource>
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsIndNaturezaFrete" runat="server"
    SelectMethod="GetIndNaturezaFrete" TypeName="Glass.Data.EFD.DataSourcesEFD">
</colo:VirtualObjectDataSource>
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodCont" runat="server" SelectMethod="GetCodCont"
    TypeName="Glass.Data.EFD.DataSourcesEFD">
</colo:VirtualObjectDataSource>
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodCred" runat="server" SelectMethod="GetCodCred"
    TypeName="Glass.Data.EFD.DataSourcesEFD">
</colo:VirtualObjectDataSource>
