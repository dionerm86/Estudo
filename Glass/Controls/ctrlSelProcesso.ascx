<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlSelProcesso.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlSelProcesso" %>

<%@ Register Src="~/Controls/ctrlSelPopup.ascx" TagPrefix="uc1" TagName="ctrlSelPopup" %>
<%@ Register Src="~/Controls/ctrlSelAplicacao.ascx" TagPrefix="uc2" TagName="ctrlSelAplicacao" %>

<uc1:ctrlSelPopup ID="selProcesso" runat="server" DataSourceID="odsProcesso"
    DataTextField="CodInterno" DataValueField="IdProcesso" FazerPostBackBotaoPesquisar="false"
    ColunasExibirPopup="IdProcesso|CodInterno|Descricao" CallbackSelecao="alteraAplicacaoControleProcesso"
    ExibirIdPopup="false" TitulosColunas="Id|Cód.|Descrição" TituloTela="Selecione o processo"
    TextWidth="50px" />
<colo:VirtualObjectDataSource ID="odsProcesso" runat="server" SortParameterName="sortExpression"
    CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
    Culture="pt-BR" MaximumRowsParameterName="pageSize" SelectMethod="GetForSel"
    StartRowIndexParameterName="startRow" 
    TypeName="Glass.Data.DAL.EtiquetaProcessoDAO" SkinID="" 
    EnablePaging="True" SelectCountMethod="GetForSelCount">
</colo:VirtualObjectDataSource>

<script type="text/javascript">
    var <%= this.ClientID %>_controleAplicacao = '<%= ControleAplicacao != null ? ControleAplicacao.ClientID : "" %>';
    
    function alteraAplicacaoControleProcesso(nomeControle, id) {
        if (!!id) {
            var idAplicacao = ctrlSelProcesso.ObtemAplicacao(id).value;
            
            if (!!idAplicacao && !!<%= this.ClientID %>_controleAplicacao) {
                idAplicacao = idAplicacao.split('|');
                document.getElementById(<%= this.ClientID %>_controleAplicacao + "_selAplicacao_txtDescr").value = idAplicacao[1];
                document.getElementById(<%= this.ClientID %>_controleAplicacao + "_selAplicacao_hdfValor").value = idAplicacao[0];
            }
        }
    }
</script>