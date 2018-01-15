<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlParticipanteMDFe.ascx.cs"
    Inherits="Glass.UI.Web.Controls.MDFe.ctrlParticipanteMDFe" %>

<%@ Register Src="~/Controls/ctrlSelParticipante.ascx" TagName="ctrlPart" TagPrefix="uc2" %>

<div class="dtvRow">
    <div class="dtvHeader">
        <asp:Label ID="Label3" runat="server" Text="Emitente *"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <uc2:ctrlpart runat="server" id="ctrlParticipanteEmitente" exibiradmincartao="false"
            permitirvazio="false" validationgroup="c" />
    </div>
    <div class="dtvHeader">
        <asp:Label ID="Label4" runat="server" Text="Contratante"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <uc2:ctrlpart runat="server" id="ctrlParticipanteContratante" exibiradmincartao="false"
            permitirvazio="false" validationgroup="c" />
    </div>
</div>

<script type="text/javascript">

    // Desabilita o campo Tipo Emitente para só permitir selecionar loja
    FindControl("dtvManifestoEletronico_ctrlParticipantes_ctrlParticipanteEmitente_drpPart", "select").value = 3;
    FindControl("dtvManifestoEletronico_ctrlParticipantes_ctrlParticipanteEmitente_drpPart", "select").disabled = true;

</script>
