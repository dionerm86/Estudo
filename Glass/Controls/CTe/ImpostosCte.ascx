<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImpostosCte.ascx.cs" Inherits="Glass.UI.Web.Controls.CTe.ImpostosCte" %>
<%@ Register src="ImpostoCte.ascx" tagname="ImpostoCte" tagprefix="uc1" %>
<style title="text/css">
    .aba, .painel {
    	width: 98,5%;
    }
    
    .aba {
    	position: relative;
    	padding-bottom: 6px;
    }
    
    .aba span {
        padding: 6px;
        margin-right: 3px;
        cursor: pointer;
    }
    
    .painel {
    	border: 1px solid silver;
    	vertical-align: top;
    	padding: 8px;
    	overflow: auto;
    }
</style>
<script type="text/javascript">
    // -----------------------------------
    // Função que muda a aba ativa na tela
    // -----------------------------------
    function mudaAba(nomeAba) {
        // variável que contém os identificadores das abas
        var abas = new Array("icms", "pis", "cofins");

        // percorre todas as abas
        for (i = 0; i < abas.length; i++) {
            // recupera o título de aba atual e altera dependendo do parâmetro
            var aba = document.getElementById("aba_" + abas[i]);
            var borda = (abas[i] == nomeAba) ? "1px solid silver" : "1px solid #ddd";
            var bordaInferior = (abas[i] == nomeAba) ? "1px solid white" : "";
            aba.style.fontWeight = (abas[i] == nomeAba) ? "bold" : "normal";
            aba.style.borderTop = borda;
            aba.style.borderLeft = borda;
            aba.style.borderRight = borda;
            aba.style.borderBottom = bordaInferior;

            // recupera a aba atual e exibe/esconde dependendo do parâmetro
            var aba = document.getElementById(abas[i]);
            aba.style.display = (abas[i] == nomeAba) ? "block" : "none";
        }

        // altera o hiddenfield que guarda a aba atual
        document.getElementById("<%= hdfAba.ClientID %>").value = nomeAba;
    }
</script>

<div align="left" class="aba">
    <span id="aba_icms" onclick="mudaAba('icms')">
        ICMS
    </span>
    <span id="aba_pis" onclick="mudaAba('pis')">
        PIS
    </span>
    <span id="aba_cofins" onclick="mudaAba('cofins')">
        Cofins
    </span>
</div>
<div class="painel">
    <div id="icms">
        <uc1:ImpostoCte ID="imp_icms" runat="server" TipoImposto="Icms" AtivarValidadores='<%# ICMSObrigatorio %>' />
    </div>
    <div id="pis">
        <uc1:ImpostoCte ID="imp_pis" runat="server" TipoImposto="Pis" AtivarValidadores='<%# PISObrigatorio %>' />
    </div>
    <div id="cofins">
        <uc1:ImpostoCte ID="imp_cofins" runat="server" TipoImposto="Cofins" AtivarValidadores='<%# COFINSObrigatorio %>' />
    </div>
</div>

<asp:HiddenField ID="hdfAba" runat="server" Value="icms" />

<script type="text/javascript">
    mudaAba(document.getElementById("<%= hdfAba.ClientID %>").value);
</script>