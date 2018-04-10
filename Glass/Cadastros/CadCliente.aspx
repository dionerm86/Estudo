<%@ Page Title="Cadastro de Clientes" Language="C#" MasterPageFile="~/Painel.master" EnableViewState="false"
    AutoEventWireup="true" CodeBehind="CadCliente.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCliente" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlParcelasUsar.ascx" TagName="ctrlParcelasUsar" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlFormasPagtoUsar.ascx" TagName="ctrlFormasPagtoUsar" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlLimiteTexto.ascx" TagName="ctrlLimiteTexto" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc6" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
    
        function cidadeUf()
        {
            var cidade = FindControl('txtCidade', 'input');
            var nomeUf = FindControl('hdfNomeUf', 'input').value;
            
            if (nomeUf != "" && nomeUf != null) {
                cidade.value += " - " + nomeUf;
            }
            
            var cidadeCobranca = FindControl('txtCidadeCobranca', 'input');
            var nomeUfCobranca = FindControl('hdfNomeUfCobranca', 'input').value;
            
            if (nomeUfCobranca != "" && nomeUfCobranca != null) {
                cidadeCobranca.value += " - " + nomeUfCobranca;
            }
            
            var cidadeEntrega = FindControl('txtCidadeEntrega', 'input');
            var nomeUfEntrega = FindControl('hdfNomeUfEntrega', 'input').value;
            
            if (nomeUfEntrega != "" && nomeUfEntrega != null) {
                cidadeEntrega.value += " - " + nomeUfEntrega;
            }
        }
        
        function getComissionado()
        {
            var idComissionado = FindControl("txtIdComissionado", "input").value;
            setComissionado(idComissionado);
        }

        function openComissionado()
        {
            openWindow(600, 800, "../Utils/SelComissionado.aspx");
        }

        function setComissionado(idComissionado)
        {
            var resposta = MetodosAjax.GetComissionado(idComissionado, "").value.split(';');
            
            if (resposta[0] == "Erro")
            {
                FindControl("txtIdComissionado", "input").value = "";
                FindControl("lblComissionado", "span").innerHTML = "";
                return;
            }

            FindControl("txtIdComissionado", "input").value = idComissionado;
            FindControl("lblComissionado", "span").innerHTML = resposta[1];
        }

        function iniciaPesquisaCep(cep)
        {
            if(FindControl("hdfCampoCidade", "input").value == "cliente")
            {
                var logradouro = FindControl("txtEndereco", "input");
                var bairro = FindControl("txtBairro", "input");
                var cidade = FindControl("txtCidade", "input");
                var idCidade = FindControl("hdfCidade", "input");
                pesquisarCep(cep, null, logradouro, bairro, cidade, null, idCidade);
                
                if (logradouro != null &&
                    logradouro.value != null &&
                    logradouro.value.length >= 100)
                    logradouro.value = logradouro.value.toString().substring(0, 100);
            }
            else if (FindControl("hdfCampoCidade", "input").value == "cobranca") 
            {
                var logradouro2 = FindControl("txtEnderecoCobranca", "input");
                var bairro2 = FindControl("txtBairroCobranca", "input");
                var cidade2 = FindControl("txtCidadeCobranca", "input");
                var idCidade2 = FindControl("hdfCidadeCobranca", "input");
                pesquisarCep(cep, null, logradouro2, bairro2, cidade2, null, idCidade2);
                
                if (logradouro2 != null &&
                    logradouro2.value != null &&
                    logradouro2.value.length >= 100)
                    logradouro2.value = logradouro2.value.toString().substring(0, 100);
            }
            else if (FindControl("hdfCampoCidade", "input").value == "entrega") 
            {
                var logradouro3 = FindControl("txtEnderecoEntrega", "input");
                var bairro3 = FindControl("txtBairroEntrega", "input");
                var cidade3 = FindControl("txtCidadeEntrega", "input");
                var idCidade3 = FindControl("hdfCidadeEntrega", "input");
                pesquisarCep(cep, null, logradouro3, bairro3, cidade3, null, idCidade3);
                
                if (logradouro3 != null &&
                    logradouro3.value != null &&
                    logradouro3.value.length >= 100)
                    logradouro3.value = logradouro3.value.toString().substring(0, 100);
            }            
        }
        
        function onInsert(){
            bloquearPagina();
            
            if(!validaInsercao()){
                desbloquearPagina(true);
                return false;
            }
            else{
                desbloquearPagina(false);
                return true;
            }
        }

        function validaInsercao()
        {
            if (verificaCampos() == false)
                return false;

            var cpfCnpj = FindControl("txtCpfCnpj", "input").value;
            var produtorRural = FindControl("chkProdutorRural", "input").checked;
            var urlSistema = FindControl("txtUrlSistema", "input");
            urlSistema = urlSistema != null ? urlSistema.value : "";

            if (<%= ExigirFuncionarioAoInserir().ToString().ToLower() %> && FindControl("drpFuncionario", "select").value == "") {
                alert("Informe o vendedor associado à este cliente.");
                return false;
            }
            
            if (urlSistema != "" && urlSistema.toUpperCase().indexOf("WEBGLASS") == -1) {
                alert("A URL do Sistema é inválida.");
                return false;
            }

            if ((cpfCnpj == "999.999.999-99" || cpfCnpj == "99.999.999/9999-99") && <%= PermitirCpfCnpjTudo9AoInserir().ToString().ToLower() %>)
                FindControl("valCpfCnpj", "span").enabled = false;
            else if (cpfCnpj == "")
            {
                alert("Informe o CPF/CNPJ do cliente.");
                return false;
            }
            else if (FindControl("valCpfCnpj", "span").style.visibility == "visible")
                return false;
            else if (!produtorRural && CadCliente.CheckIfExists(cpfCnpj).value == "true")
            {
                alert("Já existe um cliente cadastrado com o CPF/CNPJ informado");
                return false;
            }
            
            FindControl("txtLimite", "input").value = FindControl("txtLimite", "input").value.replace(".", "");
            FindControl("txtLimiteCheques", "input").value = FindControl("txtLimiteCheques", "input").value.replace(".", "");

            return true;
        }

        function onUpdate(){
        
            bloquearPagina();
            
            if(!validaUpdate()){
                desbloquearPagina(true);
                return false;
            }
            else{
                desbloquearPagina(false);
                return true;
            }
        }
        
        function validaUpdate()
        {            
            var tipoUsuario = FindControl("hdfTipoUsuario", "input").value;
            var produtorRural = FindControl("chkProdutorRural", "input").checked;
            var urlSistema = FindControl("txtUrlSistema", "input");
            urlSistema = urlSistema != null ? urlSistema.value : "";

            if (tipoUsuario != "19") {
                if (verificaCampos() == false)
                    return false;
            }
                        
            var cpfCnpj = FindControl("txtCpfCnpj", "input").value;
            
            if ((cpfCnpj == "999.999.999-99" || cpfCnpj == "99.999.999/9999-99") && <%= PermitirCpfCnpjTudo9AoInserir().ToString().ToLower() %>)
                FindControl("valCpfCnpj", "span").enabled = false;
            else if (cpfCnpj == "")
            {
                alert("Informe o CPF/CNPJ do cliente.");
                return false;
            }
            else if (FindControl("valCpfCnpj", "span").style.visibility == "visible")
                return false;            
            else if (!produtorRural && CadCliente.CheckIfExists(cpfCnpj).value == "true") {
                var cpfCnpjNovo = FindControl("txtCpfCnpj", "input").value;
                var cpfCnpjSalvo = FindControl("hdfCNPJ", "input").value;

                if (CadCliente.ComparaCpfCnpj(cpfCnpjSalvo, cpfCnpjNovo).value == "false") {
                    alert("CPF/CNPJ inserido já está cadastrado no sistema para outro cliente");
                    return false;
                }
            }
            
            if (FindControl("txtValorMediaIni", "input").value != "" || FindControl("txtValorMediaFim", "input").value != ""){
                var valorMediaIni = FindControl("txtValorMediaIni", "input");
                var valorMediaFim = FindControl("txtValorMediaFim", "input");
                
                if (valorMediaIni.value != "") {
                    while(valorMediaIni.value.indexOf(".") >= 0) {
                        valorMediaIni.value = valorMediaIni.value.replace(".","");
                    }
                }
                else {
                    valorMediaIni.value = "0";
                }
                
                if (valorMediaFim.value != "") {
                    while(valorMediaFim.value.indexOf(".") >= 0) {
                        valorMediaFim.value = valorMediaFim.value.replace(".","");
                    }
                }
                else {
                    valorMediaFim.value = "0";
                }
                
                if (parseFloat(valorMediaIni.value) > parseFloat(valorMediaFim.value)){
                    alert("O valor inicial da média de compra não pode ser maior que o valor final da mesma.");
                    return false;
                }
            }

            if (FindControl("drpTipoCliente", "select") != null && 
                (FindControl("drpTipoCliente", "select").value == "" || FindControl("drpTipoCliente", "select").value == null)) {
                alert("Informe o tipo do cliente.");
                return false;                
            }
            
            if (urlSistema != "" && urlSistema.toUpperCase().indexOf("WEBGLASS") == -1) {
                alert("A URL do Sistema é inválida.");
                return false;
            }

            var toolTip = document.getElementById("WzTtDiV");
            if (toolTip != null && toolTip.style.visibility == "visible")
            {
                alert("Aplique as alterações feitas nas formas de pagamento e/ou parcelas.");
                return false;
            }

            FindControl("txtLimite", "input").value = FindControl("txtLimite", "input").value.replace(".", "");
            FindControl("txtLimiteCheques", "input").value = FindControl("txtLimiteCheques", "input").value.replace(".", "");
            
            return true;
        }

        function verificaCampos()
        {
            if (FindControl("txtNome", "input").value == "")
            {
                alert("Informe o nome do cliente.");
                return false;
            }
            
            if (!(<%= NaoExigirEnderecoConsumidorFinal().ToString().ToLower() %> && getTipoFiscal() == "1"))
            {
                if (FindControl("hdfCidade", "input").value == "")
                {
                    alert("Informe a cidade do Cliente.");
                    return false;
                }

                if (FindControl("txtEndereco", "input").value == "")
                {
                    alert("Informe o endereço do Cliente.");
                    return false;
                }

                if (FindControl("txtNum", "input").value == "")
                {
                    alert("Informe o número do endereço do Cliente.");
                    return false;
                }

                if (FindControl("txtBairro", "input").value == "")
                {
                    alert("Informe o bairro do Cliente.");
                    return false;
                }
            }            

            if (FindControl("drpTipoCliente", "select") != null && 
                (FindControl("drpTipoCliente", "select").value == "" || FindControl("drpTipoCliente", "select").value == null)) {
                alert("Informe o tipo do cliente.");
                return false;                
            }
            
            var exigirEmailCliente = <%= ExigirEmailClienteAoInserirOuAtualizar().ToString().ToLower() %>;
            
            // Email Comercial
            if (FindControl("txtEmail", "textArea") != null)
            {     
                var email = FindControl("txtEmail", "textArea").value;

                if (email == "" && exigirEmailCliente) {
                    alert("Informe o email do cliente.");
                    return false;
                }
                
                if (email != "") 
                {
                    for (var i = 0; i < email.split(';').length; i++)
                        if (email.split(';')[i].trim() != "" && !validaEmail(email.split(';')[i])) {
                            alert("Email inválido.");
                            return false;
                        }
                }
            }

            // Email Fiscal
            if (FindControl("txtEmailFiscal", "input") != null)
            {                
                var emailFiscal = FindControl("txtEmailFiscal", "input").value;

                if (emailFiscal == "" && <%= ExigirEmailClienteAoInserirOuAtualizar().ToString().ToLower() %>) {
                    alert("Informe o email fiscal do cliente.");
                    return false;
                }

                    for (var i = 0; i < emailFiscal.split(';').length; i++)
                        if (emailFiscal.split(';')[i].trim() != "" && !validaEmail(emailFiscal.split(';')[i])) {
                            alert("Email Fiscal inválido.");
                        return false;
                    }
            }
            
            // Email Cobrança
            if (FindControl("txtEmailCobranca", "input") != null)
            {                
                var emailCobranca = FindControl("txtEmailCobranca", "input").value;

                if (emailCobranca == "" && exigirEmailCliente) {
                    alert("Informe o email de cobrança do cliente.");
                    return false;
                }

                if (emailCobranca != "")
                {
                    for (var i = 0; i < emailCobranca.split(';').length; i++)
                        if (emailCobranca.split(';')[i].trim() != "" && !validaEmail(emailCobranca.split(';')[i])) {
                            alert("Email cobrança inválido.");
                            return false;
                        }
                }
            }

            var telRes = FindControl("txtTelRes", "input").value;
            var telCont = FindControl("txtTelCont", "input").value;
            var telCel = FindControl("txtTelCel", "input").value;
            
            if (telRes == "" && telCont == "" && telCel == "")
            {
                alert("Informe pelo menos um telefone de contato do Cliente.");
                return false;
            }

            if (telRes != "" && telRes.replace(' ', '').length < 13)
            {
                alert("Telefone residencial inválido.");
                return false;
            }

            if (telCont != "" && telCont.replace(' ', '').length < 13)
            {
                alert("Telefone de contato inválido.");
                return false;
            }

            if (telCel != "" && telCel.replace(' ', '').length < 13)
            {
                alert("Celular inválido.");
                return false;
            }

            var cpfCnpj = FindControl("txtCpfCnpj", "input").value;

            if (!(<%= NaoExigirEnderecoConsumidorFinal().ToString().ToLower() %> && getTipoFiscal() == "1")) {
                var cep = FindControl("txtCep", "input").value;
                if (FindControl("txtCep", "input").value == "") {
                    alert("Informe o CEP do Cliente.");
                    return false;
                }

                if (cep.length > 0 && (cep.length < 8 || cep == "00000000" || cep == "00000-000")) {
                    alert("Cep inválido.");
                    return false;
                }
        }
            
        // Se for PJ, Inscrição Estadual/Contato não pode ficar em branco
        if (getTipoPessoa() == "J")
        {
            if (FindControl("txtContato1", "input").value == "" && FindControl("txtContato", "input").value == "")
            {
                alert("O campo contato deve ser informado.");
                return false;
            }
        }

        if (getTipoFiscal() == "") 
        {
            alert("Informe o tipo fiscal do cliente.");
            return false;
        }

        if (FindControl("txtLogin", "input").value != "" && FindControl("drpFuncionario", "select").value == "")
        {
            alert("O campo Vendedor deve ser informado porque o cliente possui login.");
            return false;
        }
            
        return true;
        }

        function drpTipoPessoaChanged(apagarCpfCnpj) 
        {
            if (apagarCpfCnpj){
                FindControl("txtCpfCnpj", "input").value = "";
                FindControl("txtRgEsc", "input").value = "";
            }

            if (getTipoPessoa() == "J") {
                FindControl("txtCpfCnpj", "input").maxLength = 18;
                FindControl("lblDataNasc", "span").innerHTML = "Data Fundação";
                FindControl("lblRgInscEst", "span").innerHTML = "Insc. Est.";
            }
            else {
                FindControl("txtCpfCnpj", "input").maxLength = 14;
                FindControl("lblDataNasc", "span").innerHTML = "Data Nasc.";
                FindControl("lblRgInscEst", "span").innerHTML = FindControl("chkProdutorRural", "input").checked ? "Insc. Est." : "RG";
            }

            FindControl("txtRgEsc", "input").onkeypress = getTipoPessoa() == "J" || FindControl("chkProdutorRural", "input").checked ? null : function(event){
                return soNumeros(event, true, false);
            }
        }

        function getTipoPessoa()
        {
            return FindControl("ddlTipoPessoa", "select").value == "Juridica" ? "J" : "F";
        }

        function getTipoFiscal() {
            switch (FindControl("drpTipoFiscal", "select").value)
            {
                case "ConsumidorFinal": return "1"
                case "Revenda": return "2";
                default: return "";
            }
        }

        function setCidade(idCidade, nomeCidade, nomeUf)
        {
            if (FindControl("hdfCampoCidade", "input").value == "cliente")
            {
                FindControl('hdfCidade', 'input').value = idCidade;
                FindControl('txtCidade', 'input').value = nomeCidade + " - " + nomeUf;
            }
            else if (FindControl("hdfCampoCidade", "input").value == "cobranca") 
            {
                FindControl('hdfCidadeCobranca', 'input').value = idCidade;
                FindControl('txtCidadeCobranca', 'input').value = nomeCidade + " - " + nomeUf;
            }
            else if (FindControl("hdfCampoCidade", "input").value == "entrega") 
            {
                FindControl('hdfCidadeEntrega', 'input').value = idCidade;
                FindControl('txtCidadeEntrega', 'input').value = nomeCidade + " - " + nomeUf;
            }            
        }
             
        function setCampoCidade(campo) {
            if (campo == "cliente")
                FindControl("hdfCampoCidade", "input").value = "cliente";
            else if (campo == "cobranca")
                FindControl("hdfCampoCidade", "input").value = "cobranca";
            else if (campo == "entrega")
                FindControl("hdfCampoCidade", "input").value = "entrega";
        }

        // Máscara para valores monetários
        function maskCurrency(o, n, dig, dec) {
            new function(c, dig, dec, m) {
                addEvent(o, "keypress", function(e, _) {
                    if ((_ = e.key == 45) || e.key > 47 && e.key < 58) {
                        var o = this, d = 0, n, s, h = o.value.charAt(0) == "-" ? "-" : "",
                    l = (s = (o.value.replace(/^(-?)0+/g, "$1") + String.fromCharCode(e.key)).replace(/\D/g, "")).length;
                        m + 1 && (o.maxLength = m + (d = o.value.length - l + 1));
                        if (m + 1 && l >= m && !_) return false;
                        l <= (n = c) && (s = new Array(n - l + 2).join("0") + s);
                        for (var i = (l = (s = s.split("")).length) - n; (i -= 3) > 0; s[i - 1] += dig);
                        n && n < l && (s[l - ++n] += dec);
                        _ ? h ? m + 1 && (o.maxLength = m + d) : s[0] = "-" + s[0] : s[0] = h + s[0];
                        o.value = s.join("");
                    }
                    e.key > 30 && e.preventDefault();
                });
            } (!isNaN(n) ? Math.abs(n) : 2, "" /*typeof dig != "string" ? "." : dig*/, typeof dec != "string" ? "," : dec, o.maxLength);
        }

        addEvent = function(o, e, f, s) {
            var r = o[r = "_" + (e = "on" + e)] = o[r] || (o[e] ? [[o[e], o]] : []), a, c, d;
            r[r.length] = [f, s || o], o[e] = function(e) {
                try {
                    (e = e || event).preventDefault || (e.preventDefault = function() { e.returnValue = false; });
                    e.stopPropagation || (e.stopPropagation = function() { e.cancelBubble = true; });
                    e.target || (e.target = e.srcElement || null);
                    e.key = (e.which + 1 || e.keyCode + 1) - 1 || 0;
                } catch (f) { }
                for (d = 1, f = r.length; f; r[--f] && (a = r[f][0], o = r[f][1], a.call ? c = a.call(o, e) : (o._ = a, c = o._(e), o._ = null), d &= c !== false));
                return e = null, !!d;
            }
        };
        
        function bloquearEspeciais(e)
        {
            if (!((e.key >= 'a' && e.key <= 'z') || (e.key >= 'A' && e.key <= 'Z')) &&  
                isNaN(parseFloat(e.key)))     
            {     
                e.returnValue = false;     
            } 
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvCliente" runat="server" AutoGenerateRows="False" DataSourceID="odsCliente"
                    DefaultMode="Insert" GridLines="None" DataKeyNames="IdCli" SkinID="defaultDetailsView">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <InsertItemTemplate>
                                <table>
                                    <tr>
                                        <td style="width: 1000px">
                                            <table id="tblDadosCadastrais" width="100%">
                                                <tr>
                                                    <td colspan="4" align="center" bgcolor="#D2D2D2">
                                                        <asp:Label ID="Label20" runat="server" Text="Dados Cadastrais" Font-Bold="True"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label21" runat="server" Text="Nome/Razão Social"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="width: 380px">
                                                        <asp:TextBox ID="txtNome" runat="server" MaxLength="75" Text='<%# Bind("Nome") %>'
                                                            Width="300px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label30" runat="server" Text="Tipo Pessoa"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:DropDownList ID="ddlTipoPessoa" runat="server" onchange="drpTipoPessoaChanged(true)"
                                                            SelectedValue='<%# Bind("TipoPessoa") %>'>
                                                            <asp:ListItem Value="Fisica">Pessoa Física</asp:ListItem>
                                                            <asp:ListItem Value="Juridica">Pessoa Jurídica</asp:ListItem>
                                                        </asp:DropDownList>
                                                        &nbsp;<asp:CheckBox ID="chkProdutorRural" runat="server" onclick="drpTipoPessoaChanged(true)"
                                                            Checked='<%# Bind("ProdutorRural") %>' Text="Produtor Rural" />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" nowrap="nowrap" style="width: 170px">
                                                        <asp:Label ID="Label4" runat="server" Text="Apelido/Nome Fantasia"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtNomeFantasia" runat="server" Text='<%# Bind("NomeFantasia") %>'
                                                            MaxLength="100" Width="300px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="lblRgInscEst" runat="server" Text="RG/Insc. Est. "></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtRgEsc" runat="server" MaxLength="22" Text='<%# Bind("RgEscinst") %>'
                                                            Width="120px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label2" runat="server" Text="CPF/CNPJ"></asp:Label>
                                                        <asp:Label ID="Label96" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtCpfCnpj" runat="server" MaxLength="18" Text='<%# Bind("CpfCnpj") %>'
                                                            Width="150px" onkeypress="getTipoPessoa()=='J' ? maskCNPJ(event, this) : maskCPF(event, this);"></asp:TextBox>
                                                        <asp:CustomValidator ID="valCpfCnpj" runat="server" ClientValidationFunction="validarCpfCnpj"
                                                            ControlToValidate="txtCpfCnpj" ErrorMessage="CPF/CNPJ Inválido"></asp:CustomValidator>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="lblSuframa" runat="server" Text="SUFRAMA "></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtSuframa" runat="server" MaxLength="9" Text='<%# Bind("Suframa") %>'
                                                            Width="80px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label92" runat="server" Text="Id. Estrangeiro"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtEstrangeiroNum" runat="server" MaxLength="20" Text='<%# Bind("NumEstrangeiro") %>'
                                                            Width="150px"></asp:TextBox></td>
                                                    <td align="left" class="dtvHeader" style="width: 160px"></td>
                                                    <td align="left"></td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label34" runat="server" Text="Email Comercial"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtEmail" runat="server" TextMode="MultiLine" Text='<%# Bind("Email") %>'
                                                            Width="300px" Height="35px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="lblDataNasc" runat="server" Text="Data Nasc."></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <uc5:ctrlData ID="ctrlDataBaseVenc" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataNasc") %>' />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="lblEmailFiscal" runat="server" Text="Email Fiscal"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtEmailFiscal" runat="server" Text='<%# Bind("EmailFiscal") %>' Width="300px"></asp:TextBox>
                                                        <br />
                                                        <asp:CheckBox ID="CheckBox6" runat="server" Checked='<%# Bind("NaoReceberEmailFiscal") %>'
                                                            Text="Não receber e-mail fiscal" />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px"></td>
                                                    <td align="left"></td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label22" runat="server" Text="Tel. Cont."></asp:Label>
                                                        <asp:Label ID="Label111" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtTelCont" runat="server" MaxLength="14" onkeydown="return maskTelefone(event, this);"
                                                            onkeypress="return soTelefone(event)" Text='<%# Bind("TelCont") %>' Width="100px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label23" runat="server" Text="Tel. Cel."></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtTelCel" runat="server" MaxLength="14" onkeydown="return maskTelefone(event, this);"
                                                            onkeypress="return soTelefone(event)" Text='<%# Bind("TelCel") %>' Width="100px"></asp:TextBox>
                                                        <br />
                                                        <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("NaoReceberSms") %>'
                                                            Text="Não receber SMS pedido pronto" Visible="<%# Glass.Configuracoes.PCPConfig.EmailSMS.EnviarSMSPedidoPronto %>" />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label25" runat="server" Text="Tel. Res."></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtTelRes" runat="server" MaxLength="14" onkeydown="return maskTelefone(event, this);"
                                                            onkeypress="return soTelefone(event)" Text='<%# Bind("TelRes") %>' Width="100px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label26" runat="server" Text="Fax"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtTelFax" runat="server" MaxLength="14" onkeydown="return maskTelefone(event, this);"
                                                            onkeypress="return soTelefone(event)" Text='<%# Bind("Fax") %>' Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label322" runat="server" Text="Tipo Fiscal"></asp:Label>
                                                        <asp:Label ID="Label97" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:DropDownList ID="drpTipoFiscal" runat="server" SelectedValue='<%# Bind("TipoFiscal") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                            <asp:ListItem Value="ConsumidorFinal">Consumidor Final</asp:ListItem>
                                                            <asp:ListItem Value="Revenda">Revenda</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">CRT
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpCrt" runat="server" SelectedValue='<%# Bind("Crt") %>'
                                                            DataSourceID="odsCrt" DataValueField="Key" DataTextField="Translation">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label88" runat="server" Text="Tipo contribuinte"></asp:Label>
                                                        <asp:Label ID="Label93" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:DropDownList ID="drpIndicadorIEDestinatario" runat="server" SelectedValue='<%# Bind("IndicadorIEDestinatario") %>'
                                                            DataSourceID="odsIndicadorIEDestinatario" DataTextField="Translation" DataValueField="Key">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label84" runat="server" Text="CNAE"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtCnae" runat="server" MaxLength="10"
                                                            Text='<%# Bind("Cnae") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 1000px">
                                            <table id="tblEnderecos" style="width: 100%">
                                                <tr>
                                                    <td colspan="4" align="center" bgcolor="#D2D2D2">
                                                        <asp:Label ID="Label27" runat="server" Text="Endereços" Font-Bold="True"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label28" runat="server" Text="Endereço"></asp:Label>
                                                        <asp:Label ID="Label100" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtEndereco" runat="server" MaxLength="100" Text='<%# Bind("Endereco") %>' Width="230px"></asp:TextBox>
                                                        &nbsp;<asp:Label ID="Label52" runat="server" Text="N.º"></asp:Label>&nbsp;
                                                        <asp:Label ID="Label106" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                        <asp:TextBox ID="txtNum" onKeyPress='bloquearEspeciais(event)' runat="server" Width="50px" Text='<%# Bind("Numero") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label51" runat="server" Text="Complemento"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtCompl" runat="server" MaxLength="50" Text='<%# Bind("Compl") %>'
                                                            Width="150px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label53" runat="server" Text="Bairro"></asp:Label>
                                                        <asp:Label ID="Label101" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtBairro1" runat="server" MaxLength="100" Text='<%# Bind("Bairro") %>'
                                                            Width="200px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label55" runat="server" Text="CEP"></asp:Label>
                                                        <asp:Label ID="Label107" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtCep" runat="server" MaxLength="9" Text='<%# Bind("Cep") %>' onkeypress="return soCep(event)"
                                                            onkeydown="return maskCep(event, this);"></asp:TextBox>
                                                        <asp:ImageButton ID="imgPesquisarCep" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="setCampoCidade('cliente'); iniciaPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label54" runat="server" Text="Cidade"></asp:Label>
                                                        <asp:Label ID="Label104" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtCidade" runat="server" MaxLength="50" Text='<%# Eval("Cidade.NomeCidade") %>'
                                                            Width="200px" ReadOnly="True"></asp:TextBox>
                                                        <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="setCampoCidade('cliente'); openWindow(500, 700, '../Utils/SelCidade.aspx?retUf=1'); return false;" />
                                                        <asp:HiddenField ID="hdfCidade" runat="server" Value='<%# Bind("IdCidade") %>' />
                                                        <asp:HiddenField ID="hfdNomeUf" runat="server" Value='<%# Eval("Cidade.NomeUf") %>' />
                                                    </td>
                                                     <td align="left" class="dtvHeader" style="width: 160px"><asp:Label ID="lblPais" runat="server" Text="País"></asp:Label></td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpPais" runat="server" DataSourceID="odsPais" DataTextField="NomePais"
                                                            DataValueField="IdPais" SelectedValue='<%# Bind("IdPais") %>'>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label46" runat="server" Text="Endereço de Cobrança"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtEnderecoCobranca" runat="server" MaxLength="100" Text='<%# Bind("EnderecoCobranca") %>'
                                                            Width="230px"></asp:TextBox>
                                                        &nbsp;<asp:Label ID="Label12" runat="server" Text="N.º"></asp:Label>&nbsp;
                                                        <asp:TextBox ID="txtNumCobranca" runat="server" onKeyPress='bloquearEspeciais(event)' Width="50px" Text='<%# Bind("NumeroCobranca") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label47" runat="server" Text="Complemento (cobr.)"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtComplCobranca" runat="server" MaxLength="50" Text='<%# Bind("ComplCobranca") %>'
                                                            Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label48" runat="server" Text="Bairro (cobr.)"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtBairroCobranca" runat="server" MaxLength="100" Text='<%# Bind("BairroCobranca") %>'
                                                            Width="200px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label49" runat="server" Text="CEP (cobr.)"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtCepCobranca" runat="server" MaxLength="9" Text='<%# Bind("CepCobranca") %>'
                                                            onkeypress="return soCep(event)" onkeydown="return maskCep(event, this);"></asp:TextBox>
                                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="setCampoCidade('cobranca'); iniciaPesquisaCep(FindControl('txtCepCobranca', 'input').value); return false" />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label14" runat="server" Text="Cidade (cobr.)"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtCidadeCobranca" runat="server" MaxLength="50" Text='<%# Eval("CidadeCobranca.NomeCidade") %>'
                                                            Width="200px" ReadOnly="True"></asp:TextBox>
                                                        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="setCampoCidade('cobranca'); openWindow(500, 700, '../Utils/SelCidade.aspx?retUf=1'); return false;" />
                                                        <asp:HiddenField ID="hdfCidadeCobranca" runat="server" Value='<%# Bind("IdCidadeCobranca") %>' />
                                                        <asp:HiddenField ID="hdfNomeUfCobranca" runat="server" Value='<%# Eval("CidadeCobranca.NomeUf") %>' />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px"></td>
                                                    <td align="left"></td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label1" runat="server" Text="Endereço de Entrega"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtEnderecoEntrega" runat="server" MaxLength="100" Text='<%# Bind("EnderecoEntrega") %>'
                                                            Width="230px"></asp:TextBox>
                                                        &nbsp;<asp:Label ID="Label5" runat="server" Text="N.º"></asp:Label>&nbsp;
                                                        <asp:TextBox ID="txtNumeroEntrega" runat="server" onKeyPress='bloquearEspeciais(event)' Width="50px" Text='<%# Bind("NumeroEntrega") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label6" runat="server" Text="Complemento (entr.)"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtComplEntrega" runat="server" MaxLength="50" Text='<%# Bind("ComplEntrega") %>'
                                                            Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label7" runat="server" Text="Bairro (entr.)"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtBairroEntrega" runat="server" MaxLength="100" Text='<%# Bind("BairroEntrega") %>'
                                                            Width="200px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label8" runat="server" Text="CEP (entr.)"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtCepEntrega" runat="server" MaxLength="9" Text='<%# Bind("CepEntrega") %>'
                                                            onkeypress="return soCep(event)" onkeydown="return maskCep(event, this);"></asp:TextBox>
                                                        <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="setCampoCidade('entrega'); iniciaPesquisaCep(FindControl('txtCepEntrega', 'input').value); return false" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label11" runat="server" Text="Cidade (entr.)"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtCidadeEntrega" runat="server" MaxLength="50" Text='<%# Eval("CidadeEntrega.NomeCidade") %>'
                                                            Width="200px" ReadOnly="True"></asp:TextBox>
                                                        <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="setCampoCidade('entrega'); openWindow(500, 700, '../Utils/SelCidade.aspx?retUf=1'); return false;" />
                                                        <asp:HiddenField ID="hdfCidadeEntrega" runat="server" Value='<%# Bind("IdCidadeEntrega") %>' />
                                                        <asp:HiddenField ID="hdfNomeUfEntrega" runat="server" Value='<%# Eval("CidadeEntrega.NomeUf") %>' />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px"></td>
                                                    <td align="left"></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 1000px">
                                            <table id="tblDadosFinanc" style='<%# "width: 100%;" + (ExibirInformacoesFinanceiras() ? "": "display: none") %>'>
                                                <tr>
                                                    <td align="center" bgcolor="#D2D2D2" colspan="4">
                                                        <asp:Label ID="Label9" runat="server" Font-Bold="True" Text="Dados Financeiros"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label36" runat="server" Text="Média de compra mensal"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">De
                                                        <asp:TextBox ID="txtValorMediaIni" runat="server" onkeypress="return soNumeros(event, false, false);"
                                                            Text='<%# Bind("ValorMediaIni") %>' Width="80px"></asp:TextBox>
                                                        até
                                                        <asp:TextBox ID="txtValorMediaFim" runat="server" onkeypress="return soNumeros(event, false, false);"
                                                            Text='<%# Bind("ValorMediaFim") %>' Width="80px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style='<%# ExibirPercentualComissao() %>'>
                                                        <asp:Label ID="Label72" runat="server" Text="Percentual de comissão"></asp:Label>
                                                    </td>
                                                    <td align="left" style='<%# ExibirPercentualComissao() %>'>
                                                        <asp:TextBox ID="txtPercentualComissao" runat="server" MaxLength="3" onkeypress="return soNumeros(event, false, false);"
                                                            Text='<%# Bind("PercentualComissao") %>' Width="50px"></asp:TextBox>
                                                        <asp:Label ID="Label73" runat="server" Text="%"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label32" runat="server" Text="Limite"
                                                            ToolTip="Caso o limite esteja zerado o Cliente será ilimitado."></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <table class="pos" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:TextBox ID="txtLimite" runat="server" onkeypress="return soNumeros(event, false, false);"
                                                                        OnLoad="txtLimite_Load" Text='<%# Bind("Limite") %>' Width="80px"
                                                                        ToolTip="Caso o limite esteja zerado o Cliente será ilimitado."></asp:TextBox>
                                                                    <br />
                                                                    <asp:CheckBox ID="chkBloquearCheques" runat="server" Checked='<%# Bind("BloquearRecebChequeLimite") %>'
                                                                        OnLoad="chkBloquearCheques_Load" Text="Bloquear recebimento de cheque de terceiro acima de 50% do limite" />
                                                                    <br />
                                                                    <asp:CheckBox ID="chkBloquearChequesProprio" runat="server" Checked='<%# Bind("BloquearRecebChequeProprioLimite") %>'
                                                                        OnLoad="chkBloquearCheques_Load" Text="Bloquear recebimento de cheque próprio acima de 50% do limite" />
                                                                </td>
                                                                <td style='<%# ExibirLimiteCheques() %>'>&nbsp;
                                                                </td>
                                                                <td style='<%# ExibirLimiteCheques() %>' class="dtvHeader">Limite de cheques por CPF/CNPJ
                                                                </td>
                                                                <td style='<%# ExibirLimiteCheques() %>'>
                                                                    <asp:TextBox ID="txtLimiteCheques" runat="server" onkeypress="return soNumeros(event, false, false);"
                                                                        OnLoad="txtLimite_Load" Text='<%# Bind("LimiteCheques") %>' Width="80px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label37" runat="server" Text="Formas Pagto."></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <uc3:ctrlFormasPagtoUsar ID="ctrlFormasPagtoUsar1" runat="server" FormaPagtoPadrao='<%# Bind("IdFormaPagto") %>'
                                                            FormasPagto='<%# Bind("FormasPagto") %>' />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label83" runat="server" Text="Data Limite do Cad."></asp:Label>
                                                    </td>
                                                    <td>
                                                        <uc5:ctrlData ID="ctrlDataLimiteCad" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataLimiteCad") %>' />
                                                    </td>
                                                    <td nowrap="nowrap" class="dtvHeader" align="left"></td>
                                                    <td align="left"></td>
                                                </tr>
                                                <tr>
                                                    <td nowrap="nowrap" class="dtvHeader" align="left">
                                                        <asp:Label ID="Label56" runat="server" Text="Parcelas"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <uc2:ctrlParcelasUsar ID="ctrlParcelasUsar1" runat="server" BloquearPagto='<%# Bind("BloquearPagto") %>'
                                                            FormaPagtoPadrao='<%# Bind("TipoPagto") %>' ParcelasNaoUsar='<%# Bind("Parcelas") %>' />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label33" runat="server" Text="Revenda"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:CheckBox ID="chkRevenda" runat="server" Checked='<%# Bind("Revenda") %>' Enabled="<%# Glass.Data.Helper.Config.PossuiPermissao(Glass.Data.Helper.Config.FuncaoMenuCadastro.MarcarClienteRevenda) %>" />
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap" style='<%# Glass.Configuracoes.PedidoConfig.LiberarPedido ? "": "display: none" %>'>
                                                        <asp:Label ID="lblPercSinal" runat="server" Text="Perc. Sinal Mín. Pedido"></asp:Label>
                                                    </td>
                                                    <td align="left" style='<%# Glass.Configuracoes.PedidoConfig.LiberarPedido ? "": "display: none" %>'>
                                                        <asp:TextBox ID="txtPercSinalMin" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                            OnLoad="txtPercSinalMin_Load" Text='<%# Bind("PercSinalMinimo") %>' Width="50px"></asp:TextBox>
                                                        <asp:Label ID="Label10" runat="server" Text="%"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="lblPercRedNfeVenda" runat="server" Text="Perc. Desconto (Venda)" Visible='<%# (bool)ExibirPercRedNfe() %>'></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtPercReducaoNfe" runat="server" Text='<%# Bind("PercReducaoNFe") %>'
                                                            Visible='<%# (bool)ExibirPercRedNfe() %>' Width="70px">
                                                        </asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="lblPercRedNfeRevenda" runat="server" Text="Perc. Desconto (Revenda)"
                                                            Visible='<%# (bool)ExibirPercRedNfe() %>'></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtPercReducaoNfeRevenda" runat="server" Text='<%# Bind("PercReducaoNFeRevenda") %>'
                                                            Visible='<%# (bool)ExibirPercRedNfe() %>' Width="70px">
                                                        </asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style='<%# Glass.Configuracoes.Geral.UsarTabelasDescontoAcrescimoCliente ? "": "display: none" %>'>Tabela Desconto/Acréscimo
                                                    </td>
                                                    <td align="left" style='<%# Glass.Configuracoes.Geral.UsarTabelasDescontoAcrescimoCliente ? "": "display: none" %>'>
                                                        <asp:DropDownList ID="drpTabelaDescontoAcrescimo" runat="server" AppendDataBoundItems="true"
                                                            DataSourceID="odsTabelaDescontoAcrescimo" DataTextField="Name" DataValueField="Id"
                                                            SelectedValue='<%# Bind("IdTabelaDesconto") %>' OnLoad="drpTabelaDescontoAcrescimo_Load">
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap" style='<%# Glass.Configuracoes.PedidoConfig.LiberarPedido ? "": "display: none" %>'>Pagar antes da produção?
                                                    </td>
                                                    <td align="left" style='<%# Glass.Configuracoes.PedidoConfig.LiberarPedido ? "": "display: none" %>'>
                                                        <asp:CheckBox ID="chkPagamentoAntesProducao" runat="server" Checked='<%# Bind("PagamentoAntesProducao") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style='<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "display: none": "" %>; padding: 4px; width: 170px;'>
                                                        <asp:Label ID="Label3" runat="server" Text="Cobrar ICMS ST no pedido"></asp:Label>
                                                    </td>
                                                    <td style='<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "display: none": "" %>; width: 380px;'
                                                        align="left">
                                                        <asp:CheckBox ID="chkCobrarIcmsSt" runat="server" Checked='<%# Bind("CobrarIcmsSt") %>' />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style='<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIpiPedido ? "display: none": "" %>; padding: 4px; width: 170px;'>
                                                        <asp:Label ID="Label38" runat="server" Text="Cobrar IPI no pedido"></asp:Label>
                                                    </td>
                                                    <td align="left" style='<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIpiPedido ? "display: none": "" %>; padding: 4px; width: 170px;'>
                                                        <asp:CheckBox ID="chkCobrarIpi" runat="server" Checked='<%# Bind("CobrarIpi") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label86" runat="server" Text="Conta Bancária"></asp:Label></td>
                                                    <td>
                                                        <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                                                            DataTextField="Descricao" DataValueField="IdContaBanco" AppendDataBoundItems="True"
                                                            SelectedValue='<%# Bind("IdContaBanco") %>'>
                                                            <asp:ListItem Text="" Value="" Selected="True"></asp:ListItem>
                                                        </asp:DropDownList><asp:Image ID="imgConta" runat="server" ImageUrl="~/Images/Help.gif" ToolTip='<%# BancosDisponiveis() %>'></asp:Image></td>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label89" runat="server" Text="Plano conta contábil"></asp:Label></td>
                                                    <td>
                                                        <asp:DropDownList ID="drpPlanoContaContabil" runat="server" AppendDataBoundItems="True" DataSourceID="odsPlanoContaContabil"
                                                            DataTextField="Descricao" DataValueField="IdContaContabil" SelectedValue='<%# Bind("IdContaContabil") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader"  style="width: 170px">
                                                        <asp:Label ID="lblEmailCobranca" runat="server" Text="Email Cobrança"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtEmailCobranca" runat="server" Text='<%# Bind("EmailCobranca") %>' Width="300px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="lblDescontoEcommerce" Text="Desconto em pedidos abertos no E-commerce" runat="server" />
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtDescontoEcommerce" runat="server" Text='<%# Bind("DescontoEcommerce")%>' Width="70px" />
                                                        <asp:Label ID="Label91" runat="server" Text="%"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 1000px">
                                            <table id="tblDadosCompl" style="width: 100%">
                                                <tr>
                                                    <td colspan="4" align="center" bgcolor="#D2D2D2">
                                                        <asp:Label ID="Label57" runat="server" Text="Dados Complementares" Font-Bold="True"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label19" runat="server" Text="Situação"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'
                                                            OnLoad="drpSituacao_Load" DataSourceID="odsSituacaoCliente" DataTextField="Translation"
                                                            DataValueField="Key">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label75" runat="server" Text="Tipo"></asp:Label>
                                                        <asp:Label ID="Label110" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpTipoCliente" runat="server" SelectedValue='<%# Bind("IdTipoCliente") %>'
                                                            DataSourceID="odsTipoCliente" DataTextField="Name" DataValueField="Id"
                                                            AppendDataBoundItems="True">
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label24" runat="server" Text="Login"></asp:Label>
                                                        <br />
                                                        <br />
                                                        <asp:Label ID="Label31" runat="server" Text="Senha"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:TextBox ID="txtLogin" runat="server" MaxLength="20" Text='<%# Bind("Login") %>'></asp:TextBox>
                                                        <br />
                                                        <asp:TextBox ID="txtSenha" runat="server" MaxLength="20" Text='<%# Bind("Senha") %>'
                                                            TextMode="Password"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label40" runat="server" Text="Loja"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True" DataSourceID="odsLoja"
                                                            DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdLoja") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <uc1:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdCli"
                                                            Text='<%# Bind("IdCli") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label39" runat="server" Text="Contato"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:TextBox ID="txtContato" runat="server" MaxLength="50" Text='<%# Bind("Contato") %>'
                                                            Width="140px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label70" runat="server" Text="E-mail Contato"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:TextBox ID="txtEmailContato" runat="server" MaxLength="50" Text='<%# Bind("EmailContato") %>'
                                                            Width="140px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label42" runat="server" Text="Vendedor"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:DropDownList ID="drpFuncionario" runat="server" AppendDataBoundItems="True"
                                                            DataSourceID="odsFuncionario" DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdFunc") %>'
                                                            OnDataBound="drpFuncionario_DataBound">
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label82" runat="server" Text="Setor Contato"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:TextBox ID="txtSetorContato" runat="server" MaxLength="50" Text='<%# Bind("SetorContato") %>'
                                                            Width="140px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label44" runat="server" OnLoad="chkIgnorarBloqueio_Load" Text="Pedidos Prontos"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:CheckBox ID="chkIgnorarBloqueio" runat="server" Checked='<%# Bind("IgnorarBloqueioPedPronto") %>'
                                                            OnLoad="chkIgnorarBloqueio_Load" Text="Ignorar bloqueio de emissão de pedidos caso haja pedidos não entregues" />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label58" runat="server" Text="Comissionado"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtIdComissionado" runat="server" onblur="getComissionado()" onkeypress="return soNumeros(event, true, true)"
                                                            Text='<%# Bind("IdComissionado") %>' Width="50px"></asp:TextBox>
                                                        <asp:ImageButton ID="imgComissionado" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="openComissionado(); return false" />
                                                        &nbsp;
                                                        <asp:Label ID="lblComissionado" runat="server" Text='<%# Eval("Comissionado.Nome") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label50" runat="server" OnLoad="chkIgnorarBloqueio_Load" Text="Bloquear Pedido"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:CheckBox ID="chkBloquearPedidoContaVenc" runat="server" Checked='<%# Bind("BloquearPedidoContaVencida") %>'
                                                            Text="Bloquear emissão de pedido se houver conta vencida" />
                                                    </td>
                                                    <td class="dtvHeader" style='<%= Glass.Configuracoes.ComissaoConfig.UsarPercComissaoCliente ? "": "display: none" %>;'
                                                        align="left">Perc. Comissão Vendedor
                                                    </td>
                                                    <td style='<%= Glass.Configuracoes.ComissaoConfig.UsarPercComissaoCliente ? "": "display: none" %>'
                                                        align="left">
                                                        <asp:TextBox ID="txtPercComissaoFunc" runat="server" Text='<%# Bind("PercComissaoFunc") %>'
                                                            Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                                        %
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class='dtvHeader' style="width: 170px; display: '<%= Glass.Configuracoes.FinanceiroConfig.UsarControleCobrancaEmail ? "" : "none" %>'">
                                                        <asp:Label ID="Label16" runat="server" Text="E-mail Cobrança" Visible='<%# Glass.Configuracoes.FinanceiroConfig.UsarControleCobrancaEmail %>'></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px;">
                                                        <asp:CheckBox ID="chkNaoReceberEmailCobrancaVencidas" runat="server" Checked='<%# Bind("NaoReceberEmailCobrancaVencida") %>'
                                                            Text='<%# "Não receber e-mail de cobrança (contas vencidas)" %>' Visible="<%# Glass.Configuracoes.FinanceiroConfig.UsarControleCobrancaEmail %>" />
                                                        <br />
                                                        <asp:CheckBox ID="chkNaoReceberEmailCobrancaVencer" runat="server" Checked='<%# Bind("NaoReceberEmailCobrancaVencer") %>'
                                                            Text='<%# "Não receber e-mail de cobrança (contas a vencer)" %>' Visible="<%# Glass.Configuracoes.FinanceiroConfig.UsarControleCobrancaEmail %>" />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="<%= ExibirEstoqueClientes() %>">
                                                        <asp:Label ID="Label45" runat="server" Text="Estoque do Cliente"></asp:Label>
                                                    </td>
                                                    <td align="left" style="<%= ExibirEstoqueClientes() %>">
                                                        <asp:CheckBox ID="chkEstoqueCliente" runat="server" Checked='<%# Bind("ControlarEstoqueVidros") %>'
                                                            Text="Controlar Estoque de Vidros do Cliente" />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px; <%# !Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoPronto ? "display: none": "" %>">
                                                        <asp:Label ID="Label130" runat="server" Text="E-mail Ped. Pronto"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px; <%# !Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoPronto ? "display: none": "" %>">
                                                        <asp:CheckBox ID="chkNaoRecebeEmailPedPronto" runat="server" Checked='<%# Bind("NaoReceberEmailPedPronto") %>'
                                                            Text='<%# "Não receber e-mail pedido " + (Glass.Configuracoes.PedidoConfig.LiberarPedido ? "conf./" : "") + "pronto" %>'
                                                            Visible="<%# Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoPronto %>" />
                                                    </td>
                                                    <td align="left" class='dtvHeader' style='<%= Glass.Configuracoes.PCPConfig.GerarOrcamentoFerragensAluminiosPCP ? "": "none" %>'>
                                                        <asp:Label ID="Label68" runat="server" OnLoad="chkGerarOrcamento_Load" Text="Gerar orçamento"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:CheckBox ID="chkGerarOrcamento" runat="server" Checked='<%# Bind("GerarOrcamentoPcp") %>'
                                                            OnLoad="chkGerarOrcamento_Load" Text="Gerar orçamento de alumínios e ferragens ao finalizar conferência de pedido." />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px; <%# !Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoConfirmado ? "display: none": "" %>">
                                                        <asp:Label ID="Label15" runat="server" Text="E-mail Ped. Finalizado PCP"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px; <%# !Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoConfirmado ? "display: none": "" %>">
                                                        <asp:CheckBox ID="chkNaoRecebeEmailPedPcp" runat="server" Checked='<%# Bind("NaoReceberEmailPedPcp") %>'
                                                            Text="Não enviar e-mail quando o pedido for finalizado no PCP." />
                                                        <br />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="<%= ExibirNaoEnviarEmailLiberacao() %>">
                                                        <asp:Label ID="Label78" runat="server" Text="Não enviar e-mail ao liberar pedido"></asp:Label>
                                                    </td>
                                                    <td align="left" style="<%= ExibirNaoEnviarEmailLiberacao() %>">
                                                        <asp:CheckBox ID="chkNaoEnviarEmailLiberacao" runat="server" Checked='<%# Bind("NaoEnviarEmailLiberacao") %>' />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label71" runat="server" Text="Transportador"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpTransportador" runat="server" AppendDataBoundItems="True"
                                                            DataSourceID="odsTransportador" DataTextField="Name" DataValueField="Id"
                                                            SelectedValue='<%# Bind("IdTransportador") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="lblRota" runat="server" OnLoad="lblRota_Load" Text="Rota"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpRota" runat="server" AppendDataBoundItems="True" DataSourceID="odsRota"
                                                            DataTextField="CodInterno" DataValueField="IdRota" OnLoad="drpRota_Load" SelectedValue='<%# Bind("IdRota") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="<%= UsarControleOrdemCarga() %>">
                                                        <asp:Label ID="Label79" runat="server" Text="Ordem de Carga"></asp:Label>
                                                    </td>
                                                    <td align="left" style="<%= UsarControleOrdemCarga() %>">
                                                        <asp:CheckBox ID="CheckBox1" runat="server" Text="Pode gerar somente OC de transferência?"
                                                            Checked='<%# Bind("SomenteOcTransferencia") %>' />
                                                    </td>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="lblUrlSistema" runat="server" OnLoad="UrlSistema_Load" Text="URL do Sistema WebGlass"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtUrlSistema" runat="server" Text='<%# Bind("UrlSistema") %>' Width="200px"
                                                            OnLoad="UrlSistema_Load"></asp:TextBox>
                                                        <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtUrlSistema"
                                                            ErrorMessage="Informe uma URL válida." SetFocusOnError="True" OnLoad="UrlSistema_Load"
                                                            ValidationExpression="((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)"
                                                            ValidationGroup="c" ToolTip="Informe uma URL válida.">*</asp:RegularExpressionValidator>--%>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label29" runat="server" Text="Observação"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <asp:TextBox ID="txtObservacao" runat="server" MaxLength="200" Text='<%# Bind("Obs") %>'
                                                                        TextMode="MultiLine" Width="300px" />
                                                                </td>
                                                                <td>
                                                                    <uc4:ctrlLimiteTexto ID="lmtObsDataEntr" runat="server" IdControlToValidate="txtObservacao" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label81" runat="server" Text="Observação da Liberação"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="TextBox3" runat="server" MaxLength="1000" Text='<%# Bind("ObsLiberacao") %>'
                                                            TextMode="MultiLine" Width="300px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="<%= ControlarPedidosImportados() %>">
                                                        <asp:Label ID="Label87" runat="server" Text="Importação"></asp:Label>
                                                    </td>
                                                    <td align="left" style="<%= ControlarPedidosImportados() %>">
                                                        <asp:CheckBox ID="CheckBox4" runat="server" Text="Cliente de importação?"
                                                            Checked='<%# Bind("Importacao") %>' /></td>
                                                    <td align="left" class="dtvHeader" style='<%= Glass.Configuracoes.ProjetoConfig.UtilizarEditorCADImagensProjeto ? "" : "display: none;"  %>'>
                                                        <asp:Label ID="Label43" runat="server" Text="Editor CAD"></asp:Label></td>
                                                    <td align="left" style='<%= Glass.Configuracoes.ProjetoConfig.UtilizarEditorCADImagensProjeto ? "" : "display: none;"  %>'>
                                                        <asp:CheckBox ID="CheckBox5" runat="server" Text="Habilitar editor CAD no E-Commerce?"
                                                            Checked='<%# Bind("HabilitarEditorCad") %>' />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label90" runat="server" Text="Subgrupo Prod."></asp:Label>
                                                    </td>
                                                    <td align="left" colspan="3">
                                                        <sync:CheckBoxListDropDown ID="drpSubgrupo" runat="server" DataSourceID="odsSubgrupo"
                                                            DataTextField="DescrGrupoSubGrupo" DataValueField="IdSubgrupoProd" Width="300px"
                                                            SelectedValue='<%# Bind("IdsSubgrupoProd") %>'>
                                                        </sync:CheckBoxListDropDown>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="lblAtendente" runat="server" Text="Atendente"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:DropDownList ID="drpAtendente" runat="server" AppendDataBoundItems="True"
                                                            DataSourceID="odsFuncionario" DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdFuncAtendente") %>'
                                                            OnDataBound="drpFuncionario_DataBound">
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" style="display: none"></td>
                                                    <td align="left" style="display: none"></td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label17" runat="server" Text="Histórico" />
                                                    </td>
                                                    <td align="left" colspan="3">
                                                        <asp:TextBox ID="txtHistorico" runat="server" Text='<%# Bind("Historico") %>' TextMode="MultiLine"
                                                            Width="600px" Height="70px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label77" runat="server" Text="Observação para NF-e" />
                                                    </td>
                                                    <td align="left" colspan="3">
                                                        <asp:TextBox ID="txtObsNfe" runat="server" Text='<%# Bind("ObsNfe") %>' TextMode="MultiLine"
                                                            Width="600px" Height="70px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="lblLogoCliente" runat="server" Text="Logo do Cliente" />
                                                    </td>
                                                    <td align="left" colspan="3">
                                                        <asp:FileUpload ID="filLogoCliente" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 1000px">
                                            <table id="tblContatos" style="width: 100%">
                                                <tr>
                                                    <td colspan="4" align="center" bgcolor="#D2D2D2">
                                                        <asp:Label ID="Label59" runat="server" Text="Dados dos Contatos" Font-Bold="True"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label60" runat="server" Text="Contato 1"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtContato1" runat="server" Text='<%# Bind("Contato1") %>' MaxLength="50"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label61" runat="server" Text="Contato 2"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtContato2" runat="server" Text='<%# Bind("Contato2") %>' MaxLength="50"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label62" runat="server" Text="Cel. Contato 1"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtCelContato1" runat="server" MaxLength="14" onkeydown="return maskTelefone(event, this);"
                                                            onkeypress="return soTelefone(event)" Text='<%# Bind("CelContato1") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label63" runat="server" Text="Cel. Contato 2"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtCelContato2" runat="server" MaxLength="14" onkeydown="return maskTelefone(event, this);"
                                                            onkeypress="return soTelefone(event)" Text='<%# Bind("CelContato2") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label64" runat="server" Text="Ramal Contato 1"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtRamalContato1" runat="server" Text='<%# Bind("RamalContato1") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label65" runat="server" Text="Ramal Contato 2"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtRamalContato2" runat="server" Text='<%# Bind("RamalContato2") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label66" runat="server" Text="Email Contato 1"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtEmailContato1" runat="server" Text='<%# Bind("EmailContato1") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label67" runat="server" Text="Email Contato 2"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtEmailContato2" runat="server" Text='<%# Bind("EmailContato2") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <table>
                                    <tr>
                                        <td style="width: 1000px">
                                            <table id="tblDadosCadastrais" width="100%">
                                                <tr>
                                                    <td colspan="4" align="center" bgcolor="#D2D2D2">
                                                        <asp:Label ID="Label20" runat="server" Text="Dados Cadastrais" Font-Bold="True"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label21" runat="server" Text="Nome/Razão Social"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="width: 380px">
                                                        <asp:TextBox ID="txtNome" runat="server" MaxLength="75" Text='<%# Bind("Nome") %>'
                                                            Width="300px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label30" runat="server" Text="Tipo Pessoa"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:DropDownList ID="ddlTipoPessoa" runat="server" onchange="drpTipoPessoaChanged(true)"
                                                            SelectedValue='<%# Bind("TipoPessoa") %>'>
                                                            <asp:ListItem Value="Fisica">Pessoa Física</asp:ListItem>
                                                            <asp:ListItem Value="Juridica">Pessoa Jurídica</asp:ListItem>
                                                        </asp:DropDownList>
                                                        &nbsp;<asp:CheckBox ID="chkProdutorRural" runat="server" onclick="drpTipoPessoaChanged(true)"
                                                            Checked='<%# Bind("ProdutorRural") %>' Text="Produtor Rural" />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" nowrap="nowrap" style="width: 170px">
                                                        <asp:Label ID="Label4" runat="server" Text="Apelido/Nome Fantasia"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtNomeFantasia" runat="server" Text='<%# Bind("NomeFantasia") %>'
                                                            MaxLength="100" Width="300px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="lblRgInscEst" runat="server" Text="RG/Insc. Est. "></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtRgEsc" runat="server" MaxLength="22" Text='<%# Bind("RgEscinst") %>'
                                                            Width="120px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label2" runat="server" Text="CPF/CNPJ"></asp:Label>
                                                        <asp:Label ID="Label93" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtCpfCnpj" runat="server" MaxLength="18" Text='<%# Bind("CpfCnpj") %>'
                                                            Width="150px" onkeypress="getTipoPessoa()=='J' ? maskCNPJ(event, this) : maskCPF(event, this);"></asp:TextBox>
                                                        <asp:CustomValidator ID="valCpfCnpj" runat="server" ClientValidationFunction="validarCpfCnpj"
                                                            ControlToValidate="txtCpfCnpj" ErrorMessage="CPF/CNPJ Inválido"></asp:CustomValidator>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="lblSuframa" runat="server" Text="SUFRAMA "></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtSuframa" runat="server" MaxLength="9" Text='<%# Bind("Suframa") %>'
                                                            Width="80px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label92" runat="server" Text="Id. Estrangeiro"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtEstrangeiroNum" runat="server" MaxLength="20" Text='<%# Bind("NumEstrangeiro") %>'
                                                            Width="150px"></asp:TextBox></td>
                                                    <td align="left" class="dtvHeader" style="width: 160px"></td>
                                                    <td align="left"></td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label34" runat="server" Text="Email Comercial"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtEmail" runat="server" TextMode="MultiLine" Text='<%# Bind("Email") %>'
                                                            Width="300px" Height="35px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="lblDataNasc" runat="server" Text="Data Nasc."></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <uc5:ctrlData ID="ctrlDataNasc2" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataNasc") %>'
                                                            ExibirHoras="False" />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="lblEmailFiscal" runat="server" Text="Email Fiscal"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtEmailFiscal" runat="server" Text='<%# Bind("EmailFiscal") %>' Width="300px"></asp:TextBox>
                                                        <br />
                                                        <asp:CheckBox ID="CheckBox6" runat="server" Checked='<%# Bind("NaoReceberEmailFiscal") %>'
                                                            Text="Não receber e-mail fiscal" />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px"></td>
                                                    <td align="left"></td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label22" runat="server" Text="Tel. Cont."></asp:Label>
                                                        <asp:Label ID="Label94" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtTelCont" runat="server" MaxLength="14" onkeydown="return maskTelefone(event, this);"
                                                            onkeypress="return soTelefone(event)" Text='<%# Bind("TelCont") %>' Width="100px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label23" runat="server" Text="Tel. Cel."></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtTelCel" runat="server" MaxLength="14" onkeydown="return maskTelefone(event, this);"
                                                            onkeypress="return soTelefone(event)" Text='<%# Bind("TelCel") %>' Width="100px"></asp:TextBox>
                                                        <br />
                                                        <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("NaoReceberSms") %>'
                                                            Text="Não receber SMS pedido pronto" Visible="<%# Glass.Configuracoes.PCPConfig.EmailSMS.EnviarSMSPedidoPronto %>" />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label25" runat="server" Text="Tel. Res."></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtTelRes" runat="server" MaxLength="14" onkeydown="return maskTelefone(event, this);"
                                                            onkeypress="return soTelefone(event)" Text='<%# Bind("TelRes") %>' Width="100px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label26" runat="server" Text="Fax"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtTelFax" runat="server" MaxLength="14" onkeydown="return maskTelefone(event, this);"
                                                            onkeypress="return soTelefone(event)" Text='<%# Bind("Fax") %>' Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label322" runat="server" Text="Tipo Fiscal"></asp:Label>
                                                        <asp:Label ID="Label98" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:DropDownList ID="drpTipoFiscal" runat="server" SelectedValue='<%# Bind("TipoFiscal") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                            <asp:ListItem Value="ConsumidorFinal">Consumidor Final</asp:ListItem>
                                                            <asp:ListItem Value="Revenda">Revenda</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">CRT
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpCrt" runat="server" SelectedValue='<%# Bind("Crt") %>'
                                                            DataSourceID="odsCrt" DataValueField="Key" DataTextField="Translation">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label88" runat="server" Text="Tipo contribuinte"></asp:Label>
                                                        <asp:Label ID="Label95" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:DropDownList ID="drpIndicadorIEDestinatario" runat="server" SelectedValue='<%# Bind("IndicadorIEDestinatario") %>'
                                                            DataSourceID="odsIndicadorIEDestinatario" DataTextField="Translation" DataValueField="Key">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label84" runat="server" Text="CNAE"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtCnae" runat="server" MaxLength="10"
                                                            Text='<%# Bind("Cnae") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 1000px">
                                            <table id="tblEnderecos" style="width: 100%">
                                                <tr>
                                                    <td colspan="4" align="center" bgcolor="#D2D2D2">
                                                        <asp:Label ID="Label27" runat="server" Text="Endereços" Font-Bold="True"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label28" runat="server" Text="Endereço"></asp:Label>
                                                        <asp:Label ID="Label99" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtEndereco" runat="server" MaxLength="100" Text='<%# Bind("Endereco") %>'
                                                            Width="230px"></asp:TextBox>
                                                        &nbsp;<asp:Label ID="Label52" runat="server" Text="N.º"></asp:Label>&nbsp;
                                                        <asp:Label ID="Label105" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                        <asp:TextBox ID="txtNum" runat="server" Width="50px" onKeyPress='bloquearEspeciais(event)' Text='<%# Bind("Numero") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label51" runat="server" Text="Complemento"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtCompl" runat="server" MaxLength="50" Text='<%# Bind("Compl") %>'
                                                            Width="150px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label53" runat="server" Text="Bairro"></asp:Label>
                                                        <asp:Label ID="Label102" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtBairro5" runat="server" MaxLength="100" Text='<%# Bind("Bairro") %>'
                                                            Width="200px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label55" runat="server" Text="CEP"></asp:Label>
                                                        <asp:Label ID="Label108" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtCep1" runat="server" MaxLength="9" Text='<%# Bind("Cep") %>'
                                                            onkeypress="return soCep(event)" onkeydown="return maskCep(event, this);"></asp:TextBox>
                                                        <asp:ImageButton ID="imgPesquisarCep1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="setCampoCidade('cliente'); iniciaPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label54" runat="server" Text="Cidade"></asp:Label>
                                                        <asp:Label ID="Label103" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtCidade1" runat="server" MaxLength="50" Text='<%# Eval("Cidade.NomeCidade") %>'
                                                            Width="200px" ReadOnly="True"></asp:TextBox>
                                                        <asp:ImageButton ID="imgPesq111" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="setCampoCidade('cliente'); openWindow(500, 700, '../Utils/SelCidade.aspx?retUf=1'); return false;" />
                                                        <asp:HiddenField ID="hdfCidade1" runat="server" Value='<%# Bind("IdCidade") %>' />
                                                        <asp:HiddenField ID="hdfNomeUf" runat="server" Value='<%# Eval("Cidade.NomeUf") %>' />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px"><asp:Label ID="lblPaisIns" runat="server" Text="País"></asp:Label></td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpPais" runat="server" DataSourceID="odsPais" DataTextField="NomePais"
                                                            DataValueField="IdPais" SelectedValue='<%# Bind("IdPais") %>'>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label46" runat="server" Text="Endereço de Cobrança"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtEnderecoCobranca" runat="server" MaxLength="100" Text='<%# Bind("EnderecoCobranca") %>'
                                                            Width="230px"></asp:TextBox>
                                                        &nbsp;<asp:Label ID="Label12" runat="server" Text="N.º"></asp:Label>&nbsp;
                                                        <asp:TextBox ID="txtNumCobranca" runat="server" onKeyPress='bloquearEspeciais(event)' Width="50px" Text='<%# Bind("NumeroCobranca") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label47" runat="server" Text="Complemento (cobr.)"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtComplCobranca" runat="server" MaxLength="50" Text='<%# Bind("ComplCobranca") %>'
                                                            Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label48" runat="server" Text="Bairro (cobr.)"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtBairroCobranca" runat="server" MaxLength="100" Text='<%# Bind("BairroCobranca") %>'
                                                            Width="200px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label49" runat="server" Text="CEP (cobr.)"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtCepCobranca" runat="server" MaxLength="9" Text='<%# Bind("CepCobranca") %>'
                                                            onkeypress="return soCep(event)" onkeydown="return maskCep(event, this);"></asp:TextBox>
                                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="setCampoCidade('cobranca'); iniciaPesquisaCep(FindControl('txtCepCobranca', 'input').value); return false" />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label14" runat="server" Text="Cidade (cobr.)"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtCidadeCobranca" runat="server" MaxLength="50" Text='<%# Eval("CidadeCobranca.NomeCidade") %>'
                                                            Width="200px" ReadOnly="True"></asp:TextBox>
                                                        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="setCampoCidade('cobranca'); openWindow(500, 700, '../Utils/SelCidade.aspx?retUf=1'); return false;" />
                                                        <asp:HiddenField ID="hdfCidadeCobranca" runat="server" Value='<%# Bind("IdCidadeCobranca") %>' />
                                                        <asp:HiddenField ID="hdfNomeUfCobranca" runat="server" Value='<%# Eval("CidadeCobranca.NomeUf") %>' />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px"></td>
                                                    <td align="left"></td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label1" runat="server" Text="Endereço de Entrega"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtEnderecoEntrega" runat="server" MaxLength="100" Text='<%# Bind("EnderecoEntrega") %>'
                                                            Width="230px"></asp:TextBox>
                                                        &nbsp;<asp:Label ID="Label5" runat="server" Text="N.º"></asp:Label>&nbsp;
                                                        <asp:TextBox ID="txtNumeroEntrega" runat="server" onKeyPress='bloquearEspeciais(event)' Width="50px" Text='<%# Bind("NumeroEntrega") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label6" runat="server" Text="Complemento (entr.)"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtComplEntrega" runat="server" MaxLength="50" Text='<%# Bind("ComplEntrega") %>'
                                                            Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label7" runat="server" Text="Bairro (entr.)"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtBairroEntrega" runat="server" MaxLength="100" Text='<%# Bind("BairroEntrega") %>'
                                                            Width="200px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label8" runat="server" Text="CEP (entr.)"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtCepEntrega" runat="server" MaxLength="9" Text='<%# Bind("CepEntrega") %>'
                                                            onkeypress="return soCep(event)" onkeydown="return maskCep(event, this);"></asp:TextBox>
                                                        <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="setCampoCidade('entrega'); iniciaPesquisaCep(FindControl('txtCepEntrega', 'input').value); return false" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label11" runat="server" Text="Cidade (entr.)"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtCidadeEntrega" runat="server" MaxLength="50" Text='<%# Eval("CidadeEntrega.NomeCidade") %>'
                                                            Width="200px" ReadOnly="True"></asp:TextBox>
                                                        <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="setCampoCidade('entrega'); openWindow(500, 700, '../Utils/SelCidade.aspx?retUf=1'); return false;" />
                                                        <asp:HiddenField ID="hdfCidadeEntrega" runat="server" Value='<%# Bind("IdCidadeEntrega") %>' />
                                                        <asp:HiddenField ID="hdfNomeUfEntrega" runat="server" Value='<%# Eval("CidadeEntrega.NomeUf") %>' />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px"></td>
                                                    <td align="left"></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 1000px">
                                            <table id="tblDadosFinanc" style='<%# "width: 100%;" + (ExibirInformacoesFinanceiras() ? "": "display: none") %>'>
                                                <tr>
                                                    <td align="center" bgcolor="#D2D2D2" colspan="4">
                                                        <asp:Label ID="Label9" runat="server" Font-Bold="True" Text="Dados Financeiros"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label18" runat="server" Text="Média de compra mensal"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">De
                                                        <asp:TextBox ID="txtValorMediaIni" runat="server" onkeypress="return soNumeros(event, false, false);"
                                                            Text='<%# Bind("ValorMediaIni") %>' Width="80px"></asp:TextBox>
                                                        até
                                                        <asp:TextBox ID="txtValorMediaFim" runat="server" onkeypress="return soNumeros(event, false, false);"
                                                            Text='<%# Bind("ValorMediaFim") %>' Width="80px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style='<%# ExibirPercentualComissao() %>'>
                                                        <asp:Label ID="Label72" runat="server" Text="Percentual de comissão"></asp:Label>
                                                    </td>
                                                    <td align="left" style='<%# ExibirPercentualComissao() %>'>
                                                        <asp:TextBox ID="txtPercentualComissao" runat="server" MaxLength="3" onkeypress="return soNumeros(event, false, false);"
                                                            Text='<%# Bind("PercentualComissao") %>' Width="50px"></asp:TextBox>
                                                        <asp:Label ID="Label73" runat="server" Text="%"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label321" runat="server" Text="Limite"
                                                            ToolTip="Caso o limite esteja zerado o Cliente será ilimitado."></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <table class="pos" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:TextBox ID="txtLimite" runat="server" onkeypress="return soNumeros(event, false, false);"
                                                                        OnLoad="txtLimite_Load" Text='<%# Bind("Limite") %>' Width="80px"
                                                                        ToolTip="Caso o limite esteja zerado o Cliente será ilimitado."></asp:TextBox>
                                                                    <br />
                                                                    <asp:CheckBox ID="chkBloquearCheques" runat="server" Checked='<%# Bind("BloquearRecebChequeLimite") %>'
                                                                        Text="Bloquear recebimento de cheque de terceiro acima de 50% do limite" />
                                                                    <br />
                                                                    <asp:CheckBox ID="chkBloquearChequesProprio" runat="server" Checked='<%# Bind("BloquearRecebChequeProprioLimite") %>'
                                                                        Text="Bloquear recebimento de cheque próprio acima de 50% do limite" />
                                                                </td>
                                                                <td style='<%# ExibirLimiteCheques() %>'>&nbsp;
                                                                </td>
                                                                <td style='<%# ExibirLimiteCheques() %>' class="dtvHeader">Limite de cheques por CPF/CNPJ
                                                                </td>
                                                                <td style='<%# ExibirLimiteCheques() %>'>
                                                                    <asp:TextBox ID="txtLimiteCheques" runat="server" onkeypress="return soNumeros(event, false, false);"
                                                                        OnLoad="txtLimite_Load" Text='<%# Bind("LimiteCheques") %>' Width="80px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="lblPercSinal" runat="server" Text="Perc. Sinal Mín. Pedido"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtPercSinalMin" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                            OnLoad="txtPercSinalMin_Load" Text='<%# Bind("PercSinalMinimo") %>' Width="50px"></asp:TextBox>
                                                        <asp:Label ID="Label10" runat="server" Text="%"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label35" runat="server" Text="Crédito"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtCredito" runat="server" Enabled='<%# HabilitarCampoCredito() %>'
                                                            Text='<%# Bind("Credito") %>' Width="70px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label41" runat="server" Text="Perc. Desconto (Venda)" Visible='<%# PercReducaoNfeVisible() %>'></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtPercReducaoNfe" runat="server" Text='<%# Bind("PercReducaoNFe") %>'
                                                            Visible='<%# PercReducaoNfeVisible() %>' Width="70px">
                                                        </asp:TextBox>
                                                        <asp:Label ID="lblPercNfe" runat="server" Text="%" Visible='<%# PercReducaoNfeVisible() %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td class="dtvHeader">
                                                        <asp:Label ID="Label83" runat="server" Text="Data Limite do Cad."></asp:Label>
                                                    </td>
                                                    <td>
                                                        <uc5:ctrlData ID="ctrlDataLimiteCad" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataLimiteCad") %>' />
                                                    </td>
                                                    <td class="dtvHeader">
                                                        <asp:Label ID="Label15" runat="server" Text="Perc. Desconto (Revenda)" Visible='<%# PercReducaoNfeVisible() %>'></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtPercReducaoNfeRevenda" runat="server" Text='<%# Bind("PercReducaoNFeRevenda") %>'
                                                            Visible='<%# PercReducaoNfeVisible() %>' Width="70px">
                                                        </asp:TextBox>
                                                        <asp:Label ID="Label13" runat="server" Text="%" Visible='<%# PercReducaoNfeVisible() %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style='<%# Glass.Configuracoes.Geral.UsarTabelasDescontoAcrescimoCliente ? "": "display: none" %>'>Tabela Desconto/Acréscimo
                                                    </td>
                                                    <td align="left" style='<%# Glass.Configuracoes.Geral.UsarTabelasDescontoAcrescimoCliente ? "": "display: none" %>'>
                                                        <asp:DropDownList ID="drpTabelaDescontoAcrescimo" runat="server" DataSourceID="odsTabelaDescontoAcrescimo"
                                                            DataTextField="Name" DataValueField="Id" AppendDataBoundItems="true"
                                                            SelectedValue='<%# Bind("IdTabelaDesconto") %>' OnLoad="drpTabelaDescontoAcrescimo_Load">
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td nowrap="nowrap" class="dtvHeader" style='<%# Glass.Configuracoes.PedidoConfig.LiberarPedido ? "": "display: none" %>;'>Pagar antes da produção?
                                                    </td>
                                                    <td style='<%# Glass.Configuracoes.PedidoConfig.LiberarPedido ? "": "display: none" %>'
                                                        align="left">
                                                        <asp:CheckBox ID="chkPagamentoAntesProducao" runat="server" Checked='<%# Bind("PagamentoAntesProducao") %>' />
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label33" runat="server" Text="Revenda"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:CheckBox ID="chkRevenda" runat="server" Checked='<%# Bind("Revenda") %>' Enabled="<%# Glass.Data.Helper.Config.PossuiPermissao(Glass.Data.Helper.Config.FuncaoMenuCadastro.MarcarClienteRevenda) %>" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style='padding: 4px; <%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIpiPedido ? "visibility: hidden": "" %>; width: 170px;'>
                                                        <asp:Label ID="Label38" runat="server" Text="Cobrar IPI no pedido"></asp:Label>
                                                    </td>
                                                    <td style='<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIpiPedido ? "visibility: hidden": "" %>; width: 380px;'
                                                        align="left">
                                                        <asp:CheckBox ID="chkCobrarIpi" runat="server" Checked='<%# Bind("CobrarIpi") %>' />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label37" runat="server" Text="Formas Pagto."></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <uc3:ctrlFormasPagtoUsar ID="ctrlFormasPagtoUsar1" runat="server" FormaPagtoPadrao='<%# Bind("IdFormaPagto") %>'
                                                            FormasPagto='<%# Bind("FormasPagto") %>' />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style='padding: 4px; <%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "visibility: hidden": "" %>; width: 170px;'>
                                                        <asp:Label ID="Label3" runat="server" Text="Cobrar ICMS ST no pedido"></asp:Label>
                                                    </td>
                                                    <td style='<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "visibility: hidden": "" %>; width: 380px;'
                                                        align="left">
                                                        <asp:CheckBox ID="chkCobrarIcmsSt" runat="server" Checked='<%# Bind("CobrarIcmsSt") %>' />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label56" runat="server" Text="Parcelas"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <uc2:ctrlParcelasUsar ID="ctrlParcelasUsar1" runat="server" BloquearPagto='<%# Bind("BloquearPagto") %>'
                                                            ParcelasNaoUsar='<%# Bind("Parcelas") %>' FormaPagtoPadrao='<%# Bind("TipoPagto") %>' IdCliente='<%# Eval("IdCli") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label85" runat="server" Text="Conta Bancária"></asp:Label></td>
                                                    <td>
                                                        <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                                                            DataTextField="Descricao" DataValueField="IdContaBanco" AppendDataBoundItems="True"
                                                            SelectedValue='<%# Bind("IdContaBanco") %>' OnDataBinding="drpContaBanco_DataBinding">
                                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                                        </asp:DropDownList><asp:Image ID="imgConta" runat="server" ImageUrl="~/Images/Help.gif" ToolTip='<%# BancosDisponiveis() %>'></asp:Image></td>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label89" runat="server" Text="Plano conta contábil"></asp:Label></td>
                                                    <td>
                                                        <asp:DropDownList ID="drpPlanoContaContabil" runat="server" AppendDataBoundItems="True" DataSourceID="odsPlanoContaContabil"
                                                            DataTextField="Descricao" DataValueField="IdContaContabil" SelectedValue='<%# Bind("IdContaContabil") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader"  style="width: 170px">
                                                        <asp:Label ID="lblEmailCobranca" runat="server" Text="Email Cobrança"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtEmailCobranca" runat="server" Text='<%# Bind("EmailCobranca") %>' Width="300px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="lblDescontoEcommerce" Text="Desconto em pedidos abertos no E-commerce" runat="server" />
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtDescontoEcommerce" runat="server" Text='<%# Bind("DescontoEcommerce")%>' Width="70px" />
                                                        <asp:Label ID="Label91" runat="server" Text="%"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 1000px">
                                            <table id="tblDadosCompl" style="width: 100%">
                                                <tr>
                                                    <td colspan="4" align="center" bgcolor="#D2D2D2">
                                                        <asp:Label ID="Label57" runat="server" Text="Dados Complementares" Font-Bold="True"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label19" runat="server" Text="Situação"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'
                                                            OnLoad="drpSituacao_Load" DataSourceID="odsSituacaoCliente" DataTextField="Translation"
                                                            DataValueField="Key">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label45" runat="server" Text="Tipo"></asp:Label>
                                                        <asp:Label ID="Label109" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpTipoCliente" runat="server" SelectedValue='<%# Bind("IdTipoCliente") %>'
                                                            DataSourceID="odsTipoCliente" DataTextField="Name" DataValueField="Id"
                                                            AppendDataBoundItems="True">
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label24" runat="server" Text="Login"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:TextBox ID="txtLogin" runat="server" MaxLength="20" Text='<%# Bind("Login") %>'></asp:TextBox>
                                                        <asp:HiddenField ID="hdfSenha" runat="server" Value='<%# Bind("Senha") %>' />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label40" runat="server" Text="Loja"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True" DataSourceID="odsLoja"
                                                            DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdLoja") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <uc1:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdCli"
                                                            Text='<%# Bind("IdCli") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label39" runat="server" Text="Contato"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:TextBox ID="txtContato" runat="server" MaxLength="50" Text='<%# Bind("Contato") %>'
                                                            Width="140px"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label69" runat="server" Text="E-mail Contato"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:TextBox ID="txtEmailContato" runat="server" MaxLength="50" Text='<%# Bind("EmailContato") %>'
                                                            Width="140px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label42" runat="server" Text="Vendedor"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:DropDownList ID="drpFuncionario" runat="server" AppendDataBoundItems="True"
                                                            DataSourceID="odsFuncionario" DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdFunc") %>'
                                                            OnDataBound="drpFuncionario_DataBound">
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label82" runat="server" Text="Setor Contato"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:TextBox ID="txtSetorContato" runat="server" MaxLength="50" Text='<%# Bind("SetorContato") %>'
                                                            Width="140px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label44" runat="server" OnLoad="chkIgnorarBloqueio_Load" Text="Pedidos Prontos"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:CheckBox ID="chkIgnorarBloqueio" runat="server" Checked='<%# Bind("IgnorarBloqueioPedPronto") %>'
                                                            OnLoad="chkIgnorarBloqueio_Load" Text="Ignorar bloqueio de emissão de pedidos caso haja pedidos não entregues" />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label58" runat="server" Text="Comissionado"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtIdComissionado" runat="server" onblur="getComissionado()" onkeypress="return soNumeros(event, true, true)"
                                                            Text='<%# Bind("IdComissionado") %>' Width="50px"></asp:TextBox>
                                                        <asp:ImageButton ID="imgComissionado" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="openComissionado(); return false" />
                                                        &nbsp;
                                                        <asp:Label ID="lblComissionado" runat="server" Text='<%# Eval("Comissionado.Nome") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label50" runat="server" OnLoad="chkIgnorarBloqueio_Load" Text="Bloquear Pedido"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:CheckBox ID="chkBloquearPedidoContaVenc" runat="server" Checked='<%# Bind("BloquearPedidoContaVencida") %>'
                                                            Text="Bloquear emissão de pedido se houver conta vencida" />
                                                    </td>
                                                    <td class="dtvHeader" style='<%= Glass.Configuracoes.ComissaoConfig.UsarPercComissaoCliente ? "": "display: none" %>;'
                                                        align="left">Perc. Comissão Vendedor
                                                    </td>
                                                    <td style='<%= Glass.Configuracoes.ComissaoConfig.UsarPercComissaoCliente ? "": "display: none" %>'
                                                        align="left">
                                                        <asp:TextBox ID="txtPercComissaoFunc" runat="server" Text='<%# Bind("PercComissaoFunc") %>'
                                                            Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                                        %
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class='dtvHeader' style="width: 170px; display: '<%= Glass.Configuracoes.FinanceiroConfig.UsarControleCobrancaEmail ? "" : "none" %>'">
                                                        <asp:Label ID="Label16" runat="server" Text="E-mail Cobrança" Visible='<%# Glass.Configuracoes.FinanceiroConfig.UsarControleCobrancaEmail %>'></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px;">
                                                        <asp:CheckBox ID="chkNaoReceberEmailCobrancaVencidas" runat="server" Checked='<%# Bind("NaoReceberEmailCobrancaVencida") %>'
                                                            Text='<%# "Não receber e-mail de cobrança (contas vencidas)" %>' Visible="<%# Glass.Configuracoes.FinanceiroConfig.UsarControleCobrancaEmail %>" />
                                                        <br />
                                                        <asp:CheckBox ID="chkNaoReceberEmailCobrancaVencer" runat="server" Checked='<%# Bind("NaoReceberEmailCobrancaVencer") %>'
                                                            Text='<%# "Não receber e-mail de cobrança (contas a vencer)" %>' Visible="<%# Glass.Configuracoes.FinanceiroConfig.UsarControleCobrancaEmail %>" />
                                                    </td>
                                                    <td align="left" class='dtvHeader' style='<%= Glass.Configuracoes.PCPConfig.GerarOrcamentoFerragensAluminiosPCP ? "": "none" %>'>
                                                        <asp:Label ID="Label68" runat="server" OnLoad="chkGerarOrcamento_Load" Text="Gerar orçamento"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:CheckBox ID="chkGerarOrcamento" runat="server" Checked='<%# Bind("GerarOrcamentoPcp") %>'
                                                            OnLoad="chkGerarOrcamento_Load" Text="Gerar orçamento de alumínios e ferragens ao finalizar conferência de pedido." />
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px; <%# !Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoPronto ? "display: none": "" %>">
                                                        <asp:Label ID="Label130" runat="server" Text="E-mail Ped. Pronto"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px; <%# !Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoPronto ? "display: none": "" %>">
                                                        <asp:CheckBox ID="chkNaoRecebeEmailPedPronto" runat="server" Checked='<%# Bind("NaoReceberEmailPedPronto") %>'
                                                            Text='<%# "Não receber e-mail pedido " + (Glass.Configuracoes.PedidoConfig.LiberarPedido ? "conf./" : "") + "pronto" %>'
                                                            Visible="<%# Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoPronto %>" />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="lblRota" runat="server" OnLoad="lblRota_Load" Text="Rota"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpRota" runat="server" AppendDataBoundItems="True" DataSourceID="odsRota"
                                                            DataTextField="CodInterno" DataValueField="IdRota" OnLoad="drpRota_Load" SelectedValue='<%# Bind("IdRota") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px; <%# !Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoConfirmado ? "display: none": "" %>">
                                                        <asp:Label ID="Label150" runat="server" Text="E-mail Ped. Finalizado PCP"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px; <%# !Glass.Configuracoes.PCPConfig.EmailSMS.EnviarEmailPedidoConfirmado ? "display: none": "" %>">
                                                        <asp:CheckBox ID="chkNaoRecebeEmailPedPcp" runat="server" Checked='<%# Bind("NaoReceberEmailPedPcp") %>'
                                                            Text="Não enviar e-mail quando o pedido for finalizado no PCP." />
                                                        <br />
                                                    </td>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label71" runat="server" Text="Transportador"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpTransportador" runat="server" AppendDataBoundItems="True"
                                                            DataSourceID="odsTransportador" DataTextField="Name" DataValueField="Id"
                                                            SelectedValue='<%# Bind("IdTransportador") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label29" runat="server" Text="Observação"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <asp:TextBox ID="txtObservacao" runat="server" MaxLength="200" Text='<%# Bind("Obs") %>'
                                                                        TextMode="MultiLine" Width="300px"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <uc4:ctrlLimiteTexto ID="lmtObsDataEntr" runat="server" IdControlToValidate="txtObservacao" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label81" runat="server" Text="Observação da Liberação"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="TextBox3" runat="server" MaxLength="1000" Text='<%# Bind("ObsLiberacao") %>'
                                                            TextMode="MultiLine" Width="300px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="<%= ExibirEstoqueClientes() %>">
                                                        <asp:Label ID="Label74" runat="server" Text="Estoque do Cliente"></asp:Label>
                                                    </td>
                                                    <td align="left" style="<%= ExibirEstoqueClientes() %>">
                                                        <asp:CheckBox ID="chkEstoqueCliente" runat="server" Text="Controlar Estoque de Vidros do Cliente"
                                                            Checked='<%# Bind("ControlarEstoqueVidros") %>' />
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="<%= ExibirNaoEnviarEmailLiberacao() %>">
                                                        <asp:Label ID="Label78" runat="server" Text="Não enviar e-mail ao liberar pedido"></asp:Label>
                                                    </td>
                                                    <td align="left" style="<%= ExibirNaoEnviarEmailLiberacao() %>">
                                                        <asp:CheckBox ID="chkNaoEnviarEmailLiberacao" runat="server" Checked='<%# Bind("NaoEnviarEmailLiberacao") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="<%= UsarControleOrdemCarga() %>">
                                                        <asp:Label ID="Label79" runat="server" Text="Ordem de Carga"></asp:Label>
                                                    </td>
                                                    <td align="left" style="<%= UsarControleOrdemCarga() %>">
                                                        <asp:CheckBox ID="CheckBox1" runat="server" Text="Pode gerar somente OC de transferência?"
                                                            Checked='<%# Bind("SomenteOcTransferencia") %>' />
                                                    </td>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label80" runat="server" OnLoad="UrlSistema_Load" Text="URL do Sistema WebGlass"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtUrlSistema" runat="server" Text='<%# Bind("UrlSistema") %>' Width="200px"
                                                            OnLoad="UrlSistema_Load"></asp:TextBox>
                                                        <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtUrlSistema"
                                                            ErrorMessage="Informe uma URL válida." SetFocusOnError="True" OnLoad="UrlSistema_Load"
                                                            ValidationExpression="((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)"
                                                            ValidationGroup="c" ToolTip="Informe uma URL válida.">*</asp:RegularExpressionValidator>--%>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="<%= ControlarPedidosImportados() %>">
                                                        <asp:Label ID="Label87" runat="server" Text="Importação"></asp:Label>
                                                    </td>
                                                    <td align="left" style="<%= ControlarPedidosImportados() %>">
                                                        <asp:CheckBox ID="CheckBox4" runat="server" Text="Cliente de importação?"
                                                            Checked='<%# Bind("Importacao") %>' /></td>
                                                    <td align="left" class="dtvHeader" style='<%= Glass.Configuracoes.ProjetoConfig.UtilizarEditorCADImagensProjeto ? "" : "display: none;"  %>'>
                                                        <asp:Label ID="Label43" runat="server" Text="Editor CAD"></asp:Label></td>
                                                    <td align="left" style='<%= Glass.Configuracoes.ProjetoConfig.UtilizarEditorCADImagensProjeto ? "" : "display: none;"  %>'>
                                                        <asp:CheckBox ID="CheckBox5" runat="server" Text="Habilitar editor CAD no E-Commerce?"
                                                            Checked='<%# Bind("HabilitarEditorCad") %>' /></td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label90" runat="server" Text="Subgrupo Prod."></asp:Label>
                                                    </td>
                                                    <td align="left" colspan="3">
                                                        <sync:CheckBoxListDropDown ID="drpSubgrupo" runat="server" DataSourceID="odsSubgrupo"
                                                            DataTextField="DescrGrupoSubGrupo" DataValueField="IdSubgrupoProd" Width="300px"
                                                            SelectedValue='<%# Bind("IdsSubgrupoProd") %>'>
                                                        </sync:CheckBoxListDropDown>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="lblAtendente" runat="server" Text="Atendente"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 381px">
                                                        <asp:DropDownList ID="drpAtendente" runat="server" AppendDataBoundItems="True"
                                                            DataSourceID="odsFuncionario" DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdFuncAtendente") %>'
                                                            OnDataBound="drpFuncionario_DataBound">
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" style="display: none"></td>
                                                    <td align="left" style="display: none"></td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label17" runat="server" Text="Histórico" />
                                                    </td>
                                                    <td align="left" colspan="3">
                                                        <asp:TextBox ID="txtHistorico" runat="server" Text='<%# Bind("Historico") %>' TextMode="MultiLine"
                                                            Width="600px" Height="70px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="Label76" runat="server" Text="Observação para NF-e" />
                                                    </td>
                                                    <td align="left" colspan="3">
                                                        <asp:TextBox ID="txtObsNfe" runat="server" Text='<%# Bind("ObsNfe") %>' TextMode="MultiLine"
                                                            Width="600px" Height="70px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader">
                                                        <asp:Label ID="lblLogoClienteIns" runat="server" Text="Logo do Cliente" />
                                                    </td>
                                                    <td align="left" colspan="3">
                                                        <asp:FileUpload ID="filLogoCliente" runat="server" />
                                                        <uc6:ctrlImagemPopup ID="ctrlImagemPopup" runat="server" ImageUrl='<%# Glass.Global.UI.Web.Process.Cliente.ClienteRepositorioImagens.Instance.ObterUrl((int)Eval("IdCli")) %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 1000px">
                                            <table id="tblContatos" style="width: 100%">
                                                <tr>
                                                    <td colspan="4" align="center" bgcolor="#D2D2D2">
                                                        <asp:Label ID="Label59" runat="server" Text="Dados dos Contatos" Font-Bold="True"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label60" runat="server" Text="Contato 1"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtContato1" runat="server" Text='<%# Bind("Contato1") %>' MaxLength="50"></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label61" runat="server" Text="Contato 2"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtContato2" runat="server" Text='<%# Bind("Contato2") %>' MaxLength="50"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label62" runat="server" Text="Cel. Contato 1"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtCelContato1" runat="server" MaxLength="14" onkeydown="return maskTelefone(event, this);"
                                                            onkeypress="return soTelefone(event)" Text='<%# Bind("CelContato1") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label63" runat="server" Text="Cel. Contato 2"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtCelContato2" runat="server" MaxLength="14" onkeydown="return maskTelefone(event, this);"
                                                            onkeypress="return soTelefone(event)" Text='<%# Bind("CelContato2") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label64" runat="server" Text="Ramal Contato 1"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtRamalContato1" runat="server" Text='<%# Bind("RamalContato1") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label65" runat="server" Text="Ramal Contato 2"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtRamalContato2" runat="server" Text='<%# Bind("RamalContato2") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr class="alt">
                                                    <td align="left" class="dtvHeader" style="width: 170px">
                                                        <asp:Label ID="Label66" runat="server" Text="Email Contato 1"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 380px">
                                                        <asp:TextBox ID="txtEmailContato1" runat="server" Text='<%# Bind("EmailContato1") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" style="width: 160px">
                                                        <asp:Label ID="Label67" runat="server" Text="Email Contato 2"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtEmailContato2" runat="server" Text='<%# Bind("EmailContato2") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="return onUpdate();" ValidationGroup="c" />
                                <asp:Button ID="btnAlterarSenha" runat="server" Text="Alterar Senha" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                    CausesValidation="false" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="return onInsert();"
                                    ValidationGroup="c" />
                                <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" OnClick="btnCancelar_Click"
                                    Text="Cancelar" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfTipoUsuario" runat="server" />
    <asp:HiddenField ID="hdfCNPJ" runat="server" />
    <asp:HiddenField ID="hdfCampoCidade" runat="server" />
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPais" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.PaisDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTabelaDescontoAcrescimo" runat="server"
        SelectMethod="ObtemDescritoresTabelaDescontoAcrescimo" TypeName="Glass.Global.Negocios.IClienteFluxo">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
        SelectMethod="ObterFuncionariosAtivosAssociadosAClientes" TypeName="Glass.Global.Negocios.IFuncionarioFluxo">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCliente" runat="server"
        DataObjectTypeName="Glass.Global.Negocios.Entidades.Cliente"
        InsertMethod="SalvarClienteRetornando"
        SelectMethod="ObtemCliente"
        CreateDataObjectMethod="CriarCliente"
        SelectByKeysMethod="ObtemCliente"
        TypeName="Glass.Global.Negocios.IClienteFluxo"
        UpdateMethod="SalvarCliente"
        UpdateStrategy="GetAndUpdate"
        OnInserting="odsCli_Inserting"
        OnInserted="odsCli_Inserted"
        OnUpdating="odsCliente_Updating"
        OnUpdated="odsCliente_Updated">
        <SelectParameters>
            <asp:QueryStringParameter Name="IdCli" QueryStringField="idCli" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacaoCliente" runat="server"
        SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.SituacaoCliente, Glass.Data" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsIndicadorIEDestinatario" runat="server"
        SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Glass.IndicadorIEDestinatario, Glass.Comum" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="ObtemLojas"
        TypeName="Glass.Global.Negocios.ILojaFluxo">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
        TypeName="Glass.Data.DAL.RotaDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoCliente" runat="server"
        SelectMethod="ObtemDescritoresTipoCliente" TypeName="Glass.Global.Negocios.IClienteFluxo">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTransportador" runat="server"
        SelectMethod="ObtemDescritoresTransportadores" TypeName="Glass.Global.Negocios.ITransportadorFluxo">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="ObterBancoAgrupado"
        TypeName="Glass.Data.DAL.ContaBancoDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCrt" runat="server"
        SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.CrtCliente, Glass.Data" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoContaContabil" runat="server" SelectMethod="GetSorted"
        TypeName="Glass.Data.DAL.PlanoContaContabilDAO">
        <SelectParameters>
            <asp:Parameter DefaultValue="0" Name="natureza" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForCadCliente"
        TypeName="Glass.Data.DAL.SubgrupoProdDAO">
    </colo:VirtualObjectDataSource>
  
    <script type="text/javascript">

        drpTipoPessoaChanged(false);

        maskCurrency(FindControl("txtLimite", "input"), 2, ".", ",");
        maskCurrency(FindControl("txtLimiteCheques", "input"), 2, ".", ",");
        maskCurrency(FindControl("txtValorMediaIni", "input"), 2, ".", ",");
        maskCurrency(FindControl("txtValorMediaFim", "input"), 2, ".", ",");

        cidadeUf();

    </script>

    <asp:ValidationSummary ID="validationSummary" runat="server" DisplayMode="List" ShowMessageBox="true"
        ShowSummary="false" ValidationGroup="c" />
</asp:Content>
