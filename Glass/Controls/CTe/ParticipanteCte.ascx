<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ParticipanteCte.ascx.cs"
    Inherits="Glass.UI.Web.Controls.CTe.ParticipanteCte" %>
<%@ Register Src="~/Controls/ctrlSelParticipante.ascx" TagName="ctrlPartCte" TagPrefix="uc1" %>

<script type="text/javascript">
    function CorrigeExibicaoLabel(idControle) {
        FindControl(idControle + "_lblDescrPart", "span").style.display = "inline-block";
        FindControl(idControle + "_lblDescrPart", "span").style.maxWidth = "175px";
        FindControl(idControle + "_lblDescrPart", "span").style.overflow = "hidden";
        FindControl(idControle + "_lblDescrPart", "span").style.maxHeight = "12px";
        FindControl(idControle + "_lblDescrPart", "span").style.position = "relative";
        FindControl(idControle + "_lblDescrPart", "span").style.top = "2px";
    }

    //Para selecionar o campo Tipo Tomador, o campo Tomador deve estar preenchido.
    function VerificaSelecaoTomador(idControleTomador)
    {
        if(FindControl(idControleTomador + "_lblDescrPart", "span").textContent == "" && FindControl("drpTipoTomador", "select").value == "selecione")
            alert("O campo Nome Tomador deve ser selecionado antes");
        else if(FindControl(idControleTomador + "_lblDescrPart", "span").textContent == "")
            alert("Os campos Nome Tomador e Tomador devem ser selecionados antes");
    }

    //Função responsável por carregar os dados do tomador de acordo com o tipo selecionado   .
    //Função acionada no evento onblur do drop de seleção do tipo de tomador.
    function AlteraExibicaoTomador(idControleTomador, idControleEmitente, idControleRemetente, idControleDestinatario, idControleExpedidor, idControleRecebedor, cteSaida, cteEntradaTerc) {
        if (FindControl("drpTipoTomador", "select").value != "selecione") {
            //Bloqueio do campo tomador para edição.     
            //Uma vez selecionado o tomador e o tipo, o campo Tomador só pode ser editado clicando na imagem de edição, 
            //a qual remove os dados de participante.
            FindControl(idControleTomador + "_drpPart", "select").disabled = true;
            FindControl(idControleTomador + "_imgPart", "input").disabled = true;

            //Restante do código da função garante que somente o campo selecionado no Tipo Tomador
            //será setado com os valores pertinentes e terá seus dados bloqueados para edição.
            if (!cteSaida) {
                FindControl(idControleEmitente + "_drpPart", "select").disabled = false;
                FindControl(idControleEmitente + "_imgPart", "input").style.visibility = "visible";
            }

            FindControl(idControleRemetente + "_drpPart", "select").disabled = false;
            FindControl(idControleRemetente + "_imgPart", "input").style.visibility = "visible";

            if (!cteEntradaTerc) {
                FindControl(idControleDestinatario + "_drpPart", "select").disabled = false;
                FindControl(idControleDestinatario + "_imgPart", "input").style.visibility = "visible";
            }

            FindControl(idControleExpedidor + "_drpPart", "select").disabled = false;
            FindControl(idControleExpedidor + "_imgPart", "input").style.visibility = "visible";

            FindControl(idControleRecebedor + "_drpPart", "select").disabled = false;
            FindControl(idControleRecebedor + "_imgPart", "input").style.visibility = "visible";

            var tipoTomador = FindControl("drpTipoTomador", "select").value;

            if (!cteSaida && tipoTomador == "emitente") {
                FindControl(idControleEmitente + "_drpPart", "select").disabled = true;
                FindControl(idControleEmitente + "_imgPart", "input").style.visibility = "hidden";
            }
            else if (tipoTomador == "remetente") {
                FindControl(idControleRemetente + "_drpPart", "select").disabled = true;
                FindControl(idControleRemetente + "_imgPart", "input").style.visibility = "hidden";
            }
            else if (tipoTomador == "destinatario") {
                FindControl(idControleDestinatario + "_drpPart", "select").disabled = true;
                FindControl(idControleDestinatario + "_imgPart", "input").style.visibility = "hidden";
            }
            else if (tipoTomador == "expedidor") {
                FindControl(idControleExpedidor + "_drpPart", "select").disabled = true;
                FindControl(idControleExpedidor + "_imgPart", "input").style.visibility = "hidden";
            }
            else if (tipoTomador == "recebedor") {
                FindControl(idControleRecebedor + "_drpPart", "select").disabled = true;
                FindControl(idControleRecebedor + "_imgPart", "input").style.visibility = "hidden";
            }
        }
    }
    
    //Função responsável por carregar os dados do tomador de acordo com o tipo selecionado   .
    //Função acionada no evento onblur do drop de seleção do tipo de tomador.
    function CarregaTomador(idControleTomador, idControleEmitente, idControleRemetente, idControleDestinatario, idControleExpedidor, idControleRecebedor, cteSaida, cteEntradaTerc) {
        if(FindControl("drpTipoTomador", "select").value != "selecione") {
            AlteraExibicaoTomador(idControleTomador, idControleEmitente, idControleRemetente, idControleDestinatario, idControleExpedidor, idControleRecebedor, cteSaida, cteEntradaTerc);
        
            //Bloqueio do campo tomador para edição.     
            //Uma vez selecionado o tomador e o tipo, o campo Tomador só pode ser editado clicando na imagem de edição, 
            //a qual remove os dados de participante.
            FindControl(idControleTomador + "_drpPart", "select").disabled = true;
            FindControl(idControleTomador + "_imgPart", "input").disabled = true;

            //Restante do código da função garante que somente o campo selecionado no Tipo Tomador
            //será setado com os valores pertinentes e terá seus dados bloqueados para edição.
            if (!cteSaida) {
                FindControl(idControleEmitente + "_drpPart", "select").value = "0";
                FindControl(idControleEmitente + "_lblDescrPart", "span").textContent = "";
                FindControl(idControleEmitente + "_hdfIdPart", "input").value = "";
            }
            
            FindControl(idControleRemetente + "_drpPart", "select").value = "0";
            FindControl(idControleRemetente + "_lblDescrPart", "span").textContent = "";
            FindControl(idControleRemetente + "_hdfIdPart", "input").value = "";

            if (!cteEntradaTerc) {
                FindControl(idControleDestinatario + "_drpPart", "select").value = "0";
                FindControl(idControleDestinatario + "_lblDescrPart", "span").textContent = "";
                FindControl(idControleDestinatario + "_hdfIdPart", "input").value = "";
            }
            
            FindControl(idControleExpedidor + "_drpPart", "select").value = "0";
            FindControl(idControleExpedidor + "_lblDescrPart", "span").textContent = "";
            FindControl(idControleExpedidor + "_hdfIdPart", "input").value = "";
            
            FindControl(idControleRecebedor + "_drpPart", "select").value = "0";
            FindControl(idControleRecebedor + "_lblDescrPart", "span").textContent = "";
            FindControl(idControleRecebedor + "_hdfIdPart", "input").value = "";
            
            FindControl("hdfTomadorSelecionado", "input").value = FindControl(idControleTomador + "_drpPart", "select").selectedIndex;
            
            var tipoTomador = FindControl("drpTipoTomador", "select").value;
            var idPart = FindControl(idControleTomador + "_hdfIdPart", "input").value;
            
            if(!cteSaida && tipoTomador == "emitente")
            {
                FindControl(idControleEmitente + "_drpPart", "select").value = FindControl(idControleTomador + "_drpPart", "select").value;
                FindControl(idControleEmitente + "_lblDescrPart", "span").textContent = FindControl(idControleTomador + "_lblDescrPart", "span").textContent;
                FindControl(idControleEmitente + "_hdfIdPart", "input").value = idPart;
            }
            else if(tipoTomador == "remetente")
            {
                FindControl(idControleRemetente + "_drpPart", "select").value = FindControl(idControleTomador + "_drpPart", "select").value;                
                FindControl(idControleRemetente + "_lblDescrPart", "span").textContent = FindControl(idControleTomador + "_lblDescrPart", "span").textContent;
                FindControl(idControleRemetente + "_hdfIdPart", "input").value = idPart;
            }
            else if(!cteEntradaTerc && tipoTomador == "destinatario")
            {
                FindControl(idControleDestinatario + "_drpPart", "select").value = FindControl(idControleTomador + "_drpPart", "select").value;                
                FindControl(idControleDestinatario + "_lblDescrPart", "span").textContent = FindControl(idControleTomador + "_lblDescrPart", "span").textContent;
                FindControl(idControleDestinatario + "_hdfIdPart", "input").value = idPart;
            }
            else if(tipoTomador == "expedidor")
            {
                FindControl(idControleExpedidor + "_drpPart", "select").value = FindControl(idControleTomador + "_drpPart", "select").value;
                FindControl(idControleExpedidor + "_lblDescrPart", "span").textContent = FindControl(idControleTomador + "_lblDescrPart", "span").textContent;
                FindControl(idControleExpedidor + "_hdfIdPart", "input").value = idPart;
            }
            else if(tipoTomador == "recebedor")
            {
                FindControl(idControleRecebedor + "_drpPart", "select").value = FindControl(idControleTomador + "_drpPart", "select").value;
                FindControl(idControleRecebedor + "_lblDescrPart", "span").textContent = FindControl(idControleTomador + "_lblDescrPart", "span").textContent;
                FindControl(idControleRecebedor + "_hdfIdPart", "input").value = idPart;
            }
        }
    }
    
    //Função responsável pela edição do tomador.
    //Esta função apaga todos os dados de participantes.
    function EditarTomador(idControleTomador, idControleEmitente, idControleRemetente, idControleDestinatario, idControleExpedidor, idControleRecebedor, cteSaida, cteEntradaTerc) {
        FindControl(idControleTomador + "_lblDescrPart", "span").textContent = "";
        FindControl(idControleTomador + "_drpPart", "select").disabled = false;
        FindControl(idControleTomador + "_imgPart", "input").disabled = false;
        
        FindControl("drpTipoTomador", "select").value = "selecione";

        if (!cteSaida) {
            FindControl(idControleEmitente + "_drpPart", "select").value = "0";
            FindControl(idControleEmitente + "_drpPart", "select").disabled = true;
            FindControl(idControleEmitente + "_lblDescrPart", "span").textContent = "";
            FindControl(idControleEmitente + "_imgPart", "input").style.visibility = "hidden";
        }

        FindControl(idControleRemetente + "_drpPart", "select").value = "0";
        FindControl(idControleRemetente + "_drpPart", "select").disabled = true;
        FindControl(idControleRemetente + "_lblDescrPart", "span").textContent = "";
        FindControl(idControleRemetente + "_imgPart", "input").style.visibility = "hidden";

        if (!cteEntradaTerc) {
            FindControl(idControleDestinatario + "_drpPart", "select").value = "0";
            FindControl(idControleDestinatario + "_drpPart", "select").disabled = true;
            FindControl(idControleDestinatario + "_lblDescrPart", "span").textContent = "";
            FindControl(idControleDestinatario + "_imgPart", "input").style.visibility = "hidden";
        }
            
        FindControl(idControleExpedidor + "_drpPart", "select").value = "0";
        FindControl(idControleExpedidor + "_drpPart", "select").disabled = true;
        FindControl(idControleExpedidor + "_lblDescrPart", "span").textContent = "";
        FindControl(idControleExpedidor + "_imgPart", "input").style.visibility = "hidden";
            
        FindControl(idControleRecebedor + "_drpPart", "select").value = "0";
        FindControl(idControleRecebedor + "_drpPart", "select").disabled = true;
        FindControl(idControleRecebedor + "_lblDescrPart", "span").textContent = "";
        FindControl(idControleRecebedor + "_imgPart", "input").style.visibility = "hidden";
    }
    
</script>

<div class="dtvRow">
    <div class="dtvHeader">
        Nome Tomador
        <%= ObtemTextoCampoObrigatorio(ctrlParticipanteTomador.Validador) %>
    </div>
    <div class="dtvAlternatingRow">
        <uc1:ctrlPartCte runat="server" ID="ctrlParticipanteTomador" ExibirAdminCartao="false"
            PermitirVazio="false" ValidationGroup="c"  />
        &nbsp&nbsp&nbsp
        <asp:Image ID="imgEditarTomador" runat="server" Style="cursor: pointer;" 
            ToolTip="Editar participantes. A ação limpa os campos."
            ImageAlign="AbsMiddle" ImageUrl="~/Images/edit.gif" />
    </div>
    <div class="dtvHeader">
        Tomador
        <%= ObtemTextoCampoObrigatorio(cvDrpTomador)%>
    </div>
    <div class="dtvAlternatingRow">
        <asp:DropDownList ID="drpTipoTomador" runat="server" Width="180px">
            <asp:ListItem Value="selecione" Text="Selecione"></asp:ListItem>
            <asp:ListItem Value="emitente" Text="Emitente"></asp:ListItem>
            <asp:ListItem Value="remetente" Text="Remetente"></asp:ListItem>
            <asp:ListItem Value="expedidor" Text="Expedidor"></asp:ListItem>
            <asp:ListItem Value="recebedor" Text="Recebedor"></asp:ListItem>
            <asp:ListItem Value="destinatario" Text="Destinatário"></asp:ListItem>
        </asp:DropDownList>
        <asp:CompareValidator ID="cvDrpTomador" ControlToValidate="drpTipoTomador" runat="server"
            ErrorMessage="Selecione o tipo do tomador" ValueToCompare="selecione" Operator="NotEqual"
            ValidationGroup="c">*</asp:CompareValidator>
    </div>
</div>
<div class="dtvRow">
    <div class="dtvHeader">
        Emitente
        <%= ObtemTextoCampoObrigatorio(ctrlParticipanteEmitente.Validador)%>
    </div>
    <div class="dtvAlternatingRow">
        <uc1:ctrlPartCte runat="server" ID="ctrlParticipanteEmitente" ExibirAdminCartao="false"
            PermitirVazio="false" ValidationGroup="c" />
    </div>
    <div class="dtvHeader">
        Remetente
        <%= ObtemTextoCampoObrigatorio(ctrlParticipanteRemetente.Validador)%>
    </div>
    <div class="dtvAlternatingRow">
        <uc1:ctrlPartCte runat="server" ID="ctrlParticipanteRemetente" ExibirAdminCartao="false"
            PermitirVazio="false" ValidationGroup="c" />
    </div>
</div>
<div class="dtvRow">
    <div class="dtvHeader">
        Destinatário
        <%= ObtemTextoCampoObrigatorio(ctrlParticipanteDestinatario.Validador)%>
    </div>
    <div class="dtvAlternatingRow">
        <uc1:ctrlPartCte runat="server" ID="ctrlParticipanteDestinatario" ExibirAdminCartao="false"
            PermitirVazio="false" ValidationGroup="c" />
    </div>
    <div class="dtvHeader">
        Expedidor
    </div>
    <div class="dtvAlternatingRow">
        <uc1:ctrlPartCte runat="server" ID="ctrlParticipanteExpedidor" ExibirAdminCartao="false" />
    </div>
</div>
<div class="dtvRow">
    <div class="dtvHeader">
        Recebedor
    </div>
    <div class="dtvAlternatingRow">
        <uc1:ctrlPartCte runat="server" ID="ctrlParticipanteRecebedor" ExibirAdminCartao="false" />
    </div>
</div>
<asp:HiddenField runat="server" ID="hdfTomadorSelecionado" />
<script type="text/javascript">
    CorrigeExibicaoLabel("<%= ctrlParticipanteTomador.ClientID %>");
    CorrigeExibicaoLabel("<%= ctrlParticipanteEmitente.ClientID %>");
    CorrigeExibicaoLabel("<%= ctrlParticipanteRemetente.ClientID %>");
    CorrigeExibicaoLabel("<%= ctrlParticipanteDestinatario.ClientID %>");
    CorrigeExibicaoLabel("<%= ctrlParticipanteExpedidor.ClientID %>");
    CorrigeExibicaoLabel("<%= ctrlParticipanteRecebedor.ClientID %>");
        
    AlteraExibicaoTomador("<%= ctrlParticipanteTomador.ClientID %>", "<%= ctrlParticipanteEmitente.ClientID %>",
        "<%= ctrlParticipanteRemetente.ClientID %>", "<%= ctrlParticipanteDestinatario.ClientID %>",
        "<%= ctrlParticipanteExpedidor.ClientID %>", "<%= ctrlParticipanteRecebedor.ClientID %>",
        <%= CteSaida.ToString().ToLower() %>, <%= CteEntradaTerceiros.ToString().ToLower() %>);
</script>