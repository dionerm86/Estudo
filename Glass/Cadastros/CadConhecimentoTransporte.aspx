<%@ Page Title="Cadastro CTe" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadConhecimentoTransporte.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadConhecimentoTransporte" %>

<%@ Register Src="~/Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/CTe/CobrancaCte.ascx" TagName="ctrlDupl" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CTe/VeiculoCte.ascx" TagName="ctrlVeiculoCte" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CTe/SeguroCte.ascx" TagName="ctrlSeguroCte" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CTe/EntregaCte.ascx" TagName="ctrlEntregaCte" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CTe/ComponenteCte.ascx" TagName="ctrlComponenteValorCte"
    TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CTe/InfoCte.ascx" TagName="ctrlInfoCte" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CTe/ImpostosCte.ascx" TagName="ctrlImpostosCte" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CTe/ConhecimentoTransRod.ascx" TagName="ctrlConhecimentoTransRod"
    TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CTe/ComplCte.ascx" TagName="ctrlComplCte" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CTe/ParticipanteCte.ascx" TagName="ctrlPartCte" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlNaturezaOperacao.ascx" TagName="ctrlNaturezaOperacao"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/CTe/EfdCte.ascx" TagName="EfdCte" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <link type="text/css" rel="stylesheet" href="<%= ResolveUrl("~/Style/CTe/CadCTe.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery/jquery.maskMoney.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function ajustaValidador(val, nomeOriginalValidador, nomeOriginalControle, complementoID) {
            var prop = ['controltovalidate', 'errormessage', 'display', 'validationGroup', 'evaluationfunction',
                'clientvalidationfunction', 'validateemptytext', 'valuetocompare', 'operator', 'initialvalue',
                'minimumvalue', 'maximumvalue', 'validationexpression'];

            eval('var ' + nomeOriginalValidador + complementoID + ' = document.all ? ' +
                'document.all[\'' + nomeOriginalValidador + complementoID + '\'] : ' +
                'document.getElementById(\'' + nomeOriginalValidador + complementoID + '\')');

            for (var p in prop) {
                eval(nomeOriginalValidador + complementoID + '.' + prop[p] + ' = ' + nomeOriginalValidador + '.' + prop[p]);

                if (typeof eval(nomeOriginalValidador + complementoID + '.' + prop[p]) == 'string')
                    eval(nomeOriginalValidador + complementoID + '.' + prop[p] + ' = ' + nomeOriginalValidador + complementoID + '.' + prop[p] +
                        '.replace(\'' + nomeOriginalControle + '\', \'' + nomeOriginalControle + complementoID + '\');');
            }
        }

        function abrirBuscaNf() {
            openWindow(600, 800, '../Utils/SelNotaFiscalAutorizada.aspx');
        }

        function buscarNf(numNFe){

            if(numNFe == null || numNFe == "")
                return false;

            var retorno = CadConhecimentoTransporte.BuscarNF(numNFe, <%= IsEntradaTerceiros().ToString().ToLower() %>);

            if(retorno.error != null){
                alert(retorno.error.description);
                setNfReferenciada("", "");
                return false;
            }

            var dadosRetorno = retorno.value.split(';');
            setNfReferenciada(dadosRetorno[0], dadosRetorno[1]);
        }

        //Máscara para campos com valor decimal
        function mascaraValor(controle, precisao)
        {
            $("#"+controle.id).unmaskMoney();
            $("#"+controle.id).maskMoney({showSymbol:false,symbol:"R$", decimal:",", precision:precisao ,thousands:".", allowZero:true});
        }

        // Seta informações da nota fiscal selecionada no popup.
        function setNfReferenciada(idNf, numNf)
        {
            FindControl('txtNumNfIns', 'input').value = numNf;
            FindControl('hdfIdNf', 'input').value = idNf;
            FindControl('lnkInsProd', 'input').click();
        }

        // Seta cidade selecionada no popup
        function setCidade(idCidade, nomeCidade, controleTxt, controleHdf)
        {
            FindControl(controleHdf, 'input').value = idCidade;
            FindControl(controleTxt, 'input').value = nomeCidade;
        }

        // Função chamada antes de inserir dados
        function onInsert()
        {
            return validar();
        }

        // Função chamada antes atualizar dados
        function onUpdate()
        {
            return validar();
        }

        // Validação de dados de inserção ou atualização
        function validar()
        {
            if (!validate('c'))
                return false;

            var tipoCTe = FindControl('dtvConhecimentoTransporte_drpTipoCte', 'select');
            var lotacao = FindControl('ctrlConhecimentoTransRod_chkLotacao', 'input');
            var veiculo = FindControl('CtrlVeiculoCte1_drpPlaca', 'select');
            var drpResponsavelSeguro = FindControl("CtrlSeguroCte_drpRespSeguro", 'select');
            var emitente = FindControl('ctrlParticipanteEmitente_lblDescrPart', 'span');

            if(emitente.innerText == "")
            {
                alert('Informe o emitente');
                return false;
            }

            if ((tipoCTe.value == '0' || tipoCTe.value == '3') && lotacao != null && lotacao.checked == true)
            {
                if(veiculo.value == 'selecione')
                {
                    alert('Para cte de lotação deve(m) ser selecionada(s) a(s) placa(s) de Veículo(s)');
                    veiculo.focus();
                    return false;
                }
            }

            if (tipoCTe.value == '2' && FindControl("dtvConhecimentoTransporte_ctrlDataAnulacao_txtData","input") != null && FindControl("dtvConhecimentoTransporte_ctrlDataAnulacao_txtData","input").value == "") {

                alert('Campo Data Anulação deve ser preenchido.');
                return false;
            }

            var isEntradaTerceiros = <%= IsEntradaTerceiros().ToString().ToLower() %>;

            if (!isEntradaTerceiros && (tipoCTe.value == '0' || tipoCTe.value == '3'))
            {
                if(drpResponsavelSeguro.value == '6')
                {
                    alert('Para cte Normal ou de Substituição, deve ser informado o responsável pelo seguro da carga.');
                    drpResponsavelSeguro.focus();
                    return false;
                }
            }

            /* Ordem Coleta
            Verifica dados de Ordem Coleta*/
            var idControle = 'ctl00_ContentPlaceHolder1_dtvConhecimentoTransporte_ctrlConhecimentoTransRod_ctrlOrdem';
            var tabelaOrdemColeta = document.getElementById(idControle + '_tabelaOrdemColeta');

            if (tabelaOrdemColeta != null)
            {
                var ultimaLinhaTabela = tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1];
                var dropTransportador = FindControl(idControle + '_drpTransportador', 'select', ultimaLinhaTabela);
                var txtNumeroOrdemColeta = FindControl(idControle + '_txtNumeroOrdColeta', 'input', ultimaLinhaTabela);
                var txtData = FindControl(idControle + '_txtData' + (tabelaOrdemColeta.rows.length - 1), 'input', ultimaLinhaTabela);

                if(txtNumeroOrdemColeta.value == '' && txtData.value == '')
                {}
                else if(txtNumeroOrdemColeta.value == '' || txtData.value == '')
                {
                    txtNumeroOrdemColeta.style.border = 'solid 1px red';
                    txtData.style.border = 'solid 1px red';

                    alert('Número Ordem Coleta e Data Emissão devem ser ambos preenchidos ou vazios.');
                    return false;
                }
            }



            /*Fim Ordem Coleta*/

            /* Participante Cte.
            Verifica dados Participante Cte*/
            var lblDecrPartExpedidor = FindControl('ctrlPartCte_ctrlParticipanteExpedidor_lblDescrPart', 'span');
            var lblDecrPartRecebedor = FindControl('ctrlPartCte_ctrlParticipanteRecebedor_lblDescrPart', 'span');

            if(drpResponsavelSeguro.value != "6")
            {
                if(drpResponsavelSeguro.value == "1" && lblDecrPartExpedidor.innerText == '')
                {
                    alert('Se o responsável pelo seguro é o expedidor, o mesmo deve ser informado como participante');
                    return false;
                }
                else if(drpResponsavelSeguro.value == "2" && lblDecrPartRecebedor.innerText == '')
                {
                    alert('Se o responsável pelo seguro é o recebedor, o mesmo deve ser informado como participante');
                    return false;
                }
            }
            /*Fim Participante Cte*/

            /* Cobrança Cte.
            Verifica dados Participante Cte*/
            var chkContaPagar = FindControl("CtrlDupl1_chkGerarContasPagar", "input");
            if (chkContaPagar != null && chkContaPagar.checked && FindControl("ctrlParticipanteEmitente_drpPart", "select").value != "1"){
                alert('Para gerar contas a pagar é necessário que o emitente seja um fornecedor.');
                return false;
            }
            /*Fim cobrança Cte*/

            return true;
        }

        /**
         * Exibe o controle da data de anulação se o tipo do CT-e for de Anulação de valores, e esconde o mesmo controle, caso não seja.
         * @param {?Object} controle O dropDownList referente ao tipo do CT-e.
         */
        function exibirEsconderDataAnulacao(controle){
            var tipoCTeAnulacao = 2;

            if(controle.value == tipoCTeAnulacao){
                FindControl("divLabelDataAnulacao","div").style.display = "";
                FindControl("divDataAnulacao","div").style.display = "";
            }else{
                FindControl("divLabelDataAnulacao","div").style.display = "none";
                FindControl("divDataAnulacao","div").style.display = "none";
            }
        }         
        

    </script>

    <div class="pagina">
        <div class="campos-obrigatorios">
            <asp:DetailsView ID="dtvConhecimentoTransporte" runat="server" AutoGenerateRows="False"
                DataKeyNames="IdCte" DefaultMode="Insert" GridLines="None" DataSourceID="odsConhecimentoTransporte"
                Width="100%" OnItemInserting="dtvConhecimentoTransporte_ItemInserting" OnItemUpdating="dtvConhecimentoTransporte_ItemUpdating"
                OnItemCommand="dtvConhecimentoTransporte_ItemCommand">
                <HeaderTemplate>
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
                        ShowSummary="False" ValidationGroup="c" />
                </HeaderTemplate>
                <Fields>
                    <asp:TemplateField>
                        <InsertItemTemplate>
                            <div class="dtvTitulo">
                                Dados CT-e
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label21" runat="server" Text="Natureza de Operação *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <uc2:ctrlNaturezaOperacao ID="ctrlNaturezaOperacao" runat="server" CodigoNaturezaOperacao='<%# Bind("IdNaturezaOperacao") %>'
                                        PermitirVazio="False" ValidationGroup="c" ErrorMessage="Preencha a natureza de operação" />
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label6" runat="server" Text="Tipo Documento CT-e *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpTipoDocumentoCte" runat="server" Enabled="false" SelectedValue='<%# Bind("TipoDocumentoCte") %>'>
                                        <asp:ListItem Value="2">Saída</asp:ListItem>
                                        <asp:ListItem Value="3">Entrada (terceiros)</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="dtvRow" runat="server" id="terceiros1" onload="ExibirDadosTerceiros">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label2" runat="server" Text="Número CT-e *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtNumCte" runat="server" MaxLength="50" Width="200px" Text='<%# Bind("NumeroCte") %>'
                                        onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvNumCte" runat="server" ErrorMessage="Número do CT-e deve ser preenchido"
                                        ControlToValidate="txtNumCte" Display="Dynamic" ValidationGroup="c">*</asp:RequiredFieldValidator>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label36" runat="server" Text="Data Emissão *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <uc2:ctrlData ID="ctrlDataEmissao" runat="server" ReadOnly="ReadWrite" DataNullable='<%# Bind("DataEmissao") %>'
                                        ValidateEmptyText="true" ValidationGroup="c" ErrorMessage="Campo Data Emissão deve ser preenchido."
                                        ExibirHoras="True" />
                                </div>
                            </div>
                            <div class="dtvRow" runat="server" id="terceiros2" onload="ExibirDadosTerceiros">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label9" runat="server" Text="Chave Acesso"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtChaveAcesso" runat="server" MaxLength="50" Width="200px" Text='<%# Bind("ChaveAcesso") %>'></asp:TextBox>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label126" runat="server" Text="Data Entrada/Saída"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <uc2:ctrlData ID="ctrlDataEntradaSaida" runat="server" ReadOnly="ReadWrite" DataNullable='<%# Bind("DataEntradaSaida") %>'
                                        ValidateEmptyText="false" ValidationGroup="c" ErrorMessage="Campo Data Entrada Saída deve ser preenchido."
                                        ExibirHoras="True" />
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label54" runat="server" Text="Cidade de Envio do CT-e*"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCidadeCte" runat="server" MaxLength="50" Enabled="False" Width="200px"
                                        ReadOnly="True"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvTxtCidadeCte" runat="server" ErrorMessage="campo cidade não pode ser vazio."
                                        ControlToValidate="txtCidadeCte" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
                                    <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx?controleTxt=txtCidadeCte&controleHdf=hdfCidadeCte'); return false;" />
                                    <asp:HiddenField ID="hdfCidadeCte" runat="server" Value='<%# Bind("IdCidadeCte") %>' />
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label17" runat="server" Text="Cidade Início *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCidadeInicio" runat="server" Enabled="False" MaxLength="50" Width="200px"
                                        ReadOnly="True"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvTxtCidadeInicio" runat="server" ErrorMessage="campo cidade início não pode ser vazio."
                                        ControlToValidate="txtCidadeInicio" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
                                    <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx?controleTxt=txtCidadeInicio&controleHdf=hdfCidadeInicio'); return false;" />
                                    <asp:HiddenField ID="hdfCidadeInicio" runat="server" Value='<%# Bind("IdCidadeInicio") %>' />
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label18" runat="server" Text="Cidade Fim *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCidadeFim" runat="server" Enabled="False" MaxLength="50" Width="200px"
                                        ReadOnly="True"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvTxtCidadeFim" runat="server" ErrorMessage="campo cidade fim não pode ser vazio."
                                        ControlToValidate="txtCidadeFim" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
                                    <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx?controleTxt=txtCidadeFim&controleHdf=hdfCidadeFim'); return false;" />
                                    <asp:HiddenField ID="hdfCidadeFim" runat="server" Value='<%# Bind("IdCidadeFim") %>' />
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label1" runat="server" Text="CT-e Anterior"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCteAnterior" runat="server" MaxLength="50" Width="200px" Text='<%# Bind("IdCteAnterior") %>'
                                        ReadOnly="true"></asp:TextBox>
                                </div>
                                <%--<div class="dtvHeader">
                                    <asp:Label ID="Label36" runat="server" Text="Cidade Orig. Frete"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCidadeOrigFrete" runat="server" Enabled="False" MaxLength="50"
                                        Width="200px" ReadOnly="True"></asp:TextBox>
                                    <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                        OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx?controleTxt=txtCidadeOrigFrete&controleHdf=hdfCidadeOrigFrete'); return false;" />
                                    <asp:HiddenField ID="hdfCidadeOrigFrete" runat="server" Value='<%# Bind("IdCidadeOrigFrete") %>' />
                                </div>--%>
                            </div>
                            <%--<div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label69" runat="server" Text="Cidade Dest. Frete"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCidadeDestFrete" runat="server" Enabled="False" MaxLength="50"
                                        Width="200px" ReadOnly="True"></asp:TextBox>
                                    <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                        OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx?controleTxt=txtCidadeDestFrete&controleHdf=hdfCidadeDestFrete'); return false;" />
                                    <asp:HiddenField ID="hdfCidadeDestFrete" runat="server" Value='<%# Bind("IdCidadeDestFrete") %>' />
                                </div>

                            </div>--%>
                            <%--<div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label2" runat="server" Text="Número Cte"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtNumCte" runat="server" MaxLength="50" Width="200px" Text='<%# Bind("NumeroCte") %>'
                                        ReadOnly="True"></asp:TextBox>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label3" runat="server" Text="Cod. Aleatório"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCodAleatorio" runat="server" Enabled="False"  OnLoad="txtCodAleatorio_Load" MaxLength="50"
                                        Width="80px" Text='<%# Bind("CodAleatorio") %>' ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>--%>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:CheckBox ID="chkGerarContasReceber" runat="server" Checked='<%# Bind("GerarContasReceber") %>' Text="Gerar contas à receber" />
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label5" runat="server" Text="Modelo"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtModelo" runat="server" Enabled="False" OnLoad="txtModelo_Load"
                                        MaxLength="2" Width="30px" Text='<%# Bind("Modelo") %>' ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label4" runat="server" Text="Série *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtSerie" runat="server" MaxLength="3" Width="25px" Text='<%# Bind("Serie") %>'
                                        OnLoad="txtSerie_Load">
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvTxtSerie" runat="server" ErrorMessage="campo série não pode ser vazio."
                                        ControlToValidate="txtSerie" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label7" runat="server" Text="Tipo Emissão *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtTipoEmissao" runat="server" MaxLength="160" Width="200px" Text="Normal"
                                        Enabled="false" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label8" runat="server" Text="Tipo CT-e *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpTipoCte" runat="server" Height="20px" Width="220px" SelectedValue='<%# Bind("TipoCte") %>' OnLoad="drpTipoCte_Load">
                                        <asp:ListItem Value="selecione" Text="Selecione um Tipo"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="CT-e Normal"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="CT-e de Complemento de Valores"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="CT-e de Anulação de Valores"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="CT-e Substituto"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="cvdrpTipoCte" ControlToValidate="drpTipoCte" runat="server"
                                        ErrorMessage="Selecione um tipo de CT-e" ValueToCompare="selecione" Operator="NotEqual"
                                        ValidationGroup="c">*</asp:CompareValidator>
                                </div>
                                <div class="dtvHeader" id="divLabelDataAnulacao" style="display:none">
                                    <asp:Label ID="Label37" runat="server" Text="Data Anulação"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow" id="divDataAnulacao" style="display:none">
                                    <uc2:ctrlData ID="ctrlDataAnulacao" runat="server"  DataString='<%# Bind("DataAnulacao") %>'/>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label10" runat="server" Text="Tipo Serviço *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpTipoServico" runat="server" Height="20px" Width="180px"
                                        SelectedValue='<%# Bind("TipoServico") %>'>
                                        <asp:ListItem Value="selecione" Text="Selecione um Tipo"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="Normal"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Subcontratação"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Redespacho"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="Redespacho Intermediário"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="cvdrpTipoServico" ControlToValidate="drpTipoServico" runat="server"
                                        ErrorMessage="Selecione um tipo de serviço" ValueToCompare="selecione" Operator="NotEqual"
                                        ValidationGroup="c">*</asp:CompareValidator>
                                </div>                                
                            </div>                            
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label11" runat="server" Text="Retirada"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:CheckBox ID="chkRetirada" runat="server" Checked='<%# Bind("Retirada") %>' Text="Retirada" />
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label12" runat="server" Text="Detalhes Retirada"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtDetRetirada" runat="server" MaxLength="160" Width="350px" Text='<%# Bind("DetalhesRetirada") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label13" runat="server" Text="Valor Total *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtVlrTot" runat="server" MaxLength="20" onclick="mascaraValor(this, 2); return false;"
                                        Width="140px" Text='<%# Bind("ValorTotal") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="campo valor total não pode ser vazio."
                                        ControlToValidate="txtVlrTot" ValidationGroup="c">*</asp:RequiredFieldValidator>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label14" runat="server" Text="Valor Receber *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtValorReceber" runat="server" MaxLength="20" onclick="mascaraValor(this, 2); return false;"
                                        Width="140px" Text='<%# Bind("ValorReceber") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvtxtValorReceber" runat="server" ErrorMessage="campo valor receber não pode ser vazio."
                                        ControlToValidate="txtValorReceber" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label15" runat="server" Text="Inform. Adicionais"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtInformAdicionais" runat="server" MaxLength="50" Width="650px" Rows="2" TextMode="MultiLine"
                                        Text='<%# Bind("InformAdicionais") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvTitulo">
                                Cobrança
                            </div>
                            <uc1:ctrlDupl ID="CtrlDupl1" runat="server" ObjCobrancaCte='<%# Bind("ObjCobrancaCte") %>'
                                TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <div class="dtvTitulo">
                                Veículo
                            </div>
                            <uc1:ctrlVeiculoCte ID="CtrlVeiculoCte1" runat="server" ObjVeiculoCte='<%# Bind("ObjVeiculoCte") %>'
                                TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <div runat="server" id="div3" onload="NaoExibirDadosTerceiros" class="dtvTitulo">
                                Seguro
                            </div>
                            <div runat="server" id="divCtrlSeguroCte" onload="NaoExibirDadosTerceiros">
                                <uc1:ctrlSeguroCte ID="CtrlSeguroCte" runat="server" ObjSeguroCte='<%# Bind("ObjSeguroCte") %>'
                                    TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' OnLoad="NaoExibirDadosTerceiros" />
                            </div>
                            <div class="dtvTitulo">
                                Entrega
                            </div>
                            <uc1:ctrlEntregaCte ID="ctrlEntregaCte" runat="server" ObjEntregaCte='<%# Bind("ObjEntregaCte") %>'
                                TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <div class="dtvTitulo">
                                Componente Valor
                            </div>
                            <uc1:ctrlComponenteValorCte ID="ctrlComponenteValorCte" runat="server" ObjComponenteValorCte='<%# Bind("ObjComponenteValorCte") %>'
                                TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <div class="dtvTitulo">
                                Info CT-e
                            </div>
                            <uc1:ctrlInfoCte ID="ctrlInfoCte" runat="server" ObjInfoCte='<%# Bind("ObjInfoCte")%>'
                                TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <div class="dtvTitulo">
                                Impostos CT-e
                            </div>
                            <uc1:ctrlImpostosCte ID="ctrlImpostosCte" runat="server" ObjImpostoCte='<%# Bind("ObjImpostoCte") %>'
                                TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <div class="dtvTitulo" runat="server" id="lblConhecimento" onload="NaoExibirDadosTerceiros">
                                Conhecimento Transporte Rodoviário
                            </div>
                            <div runat="server" id="divCtrlConhecimentoTransRod" onload="NaoExibirDadosTerceiros">
                                <uc1:ctrlConhecimentoTransRod ID="ctrlConhecimentoTransRod" runat="server" ObjCteRod='<%# Bind("ObjConhecimentoTransporteRodoviario") %>'
                                    TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            </div>
                            <div class="dtvTitulo" runat="server" id="lblCtrlComplCte" onload="divctrlComplCte_Load">
                                Complemento CT-e
                            </div>
                            <div runat="server" id="divCtrlComplCte" onload="divctrlComplCte_Load">
                                <uc1:ctrlComplCte ID="ctrlComplCte" runat="server" ObjComplCte='<%# Bind("ObjComplCte") %>'
                                    TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            </div>
                            <div class="dtvTitulo">
                                Participante CT-e
                            </div>
                            <uc1:ctrlPartCte ID="ctrlPartCte" runat="server" ObjParticipanteCte='<%# Bind("ObjParticipanteCte") %>'
                                TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <div class="dtvTitulo">
                                Dados para EFD do CT-e
                            </div>
                            <uc1:EfdCte ID="ctrlEfdCte" runat="server" ObjEfdCte='<%# Bind("ObjEfdCte") %>' TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <div class="dtvTitulo">
                                Dados CT-e
                            </div>
                            <div class="item">
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label21" runat="server" Text="Natureza de Operação:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowROMaior">
                                        <asp:Label runat="server" ID="lblNaturezaOperacaoCod" Visible="true" Text='<%# Eval("CodigoNaturezaOperacao") %>'></asp:Label>
                                        -
                                        <asp:Label runat="server" ID="lblCfopDescricao" Text='<%# Eval("DescricaoCfop") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label29" runat="server" Text="Tipo Documento CT-e:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="Label30" runat="server" Text='<%# Eval("DescricaoTipoDocumentoCte") %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label9" runat="server" Text="Chave Acesso:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowROMaior" style="width: 600px">
                                        <asp:Label ID="lblChaveAcesso" runat="server" Text='<%# Eval("ChaveAcessoExibir") %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label228" runat="server" Text="Data Emissão"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="Label129" runat="server" Text='<%# Eval("DataEmissao") %>'></asp:Label>
                                    </div>
                                    <div id="Div1" class="dtvHeaderRO" runat="server" onload="ExibirDadosTerceiros">
                                        <asp:Label ID="Label126" runat="server" Text="Data Entrada/Saída"></asp:Label>
                                    </div>
                                    <div id="Div2" class="dtvAlternatingRowRO" runat="server" onload="ExibirDadosTerceiros">
                                        <asp:Label ID="Label227" runat="server" Text='<%# Eval("DataEntradaSaida") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label1271" runat="server" Text="CT-e Anterior:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label runat="server" ID="Label128" Text='<%# Eval("NumeroCteAnterior") %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label54" runat="server" Text="Cidade:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label runat="server" ID="lblCidadeCte" Visible="true" Text='<%# Eval("NomeCidadeCte") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label17" runat="server" Text="Cidade Início:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label runat="server" ID="lblCidadeInicio" Visible="true" Text='<%# Eval("NomeCidadeInicio") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label18" runat="server" Text="Cidade Fim:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label runat="server" ID="lblCidadeFim" Visible="true" Text='<%# Eval("NomeCidadeFim") %>'></asp:Label>
                                    </div>
                                </div>
                                <%--<div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label36" runat="server" Text="Cidade Orig. Frete"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label runat="server" ID="lblCidOrigFrete" Visible="true" Text='<%# Glass.Data.DAL.CidadeDAO.Instance.GetNome(((WebGlass.Business.ConhecimentoTransporte.Entidade.Cte)Container.DataItem).IdCidadeOrigFrete) %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label69" runat="server" Text="Cidade Dest. Frete"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label runat="server" ID="lblCidadeDest" Visible="true" Text='<%# Glass.Data.DAL.CidadeDAO.Instance.GetNome(((WebGlass.Business.ConhecimentoTransporte.Entidade.Cte)Container.DataItem).IdCidadeDestFrete) %>'></asp:Label>
                                    </div>
                                </div>--%>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label2" runat="server" Text="Número CT-e:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label runat="server" ID="lblNumCte" Text='<%# Eval("NumeroCte") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label3" runat="server" Text="Cod. Aleatório:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label runat="server" ID="lblCodAleatorio" Text='<%# Eval("CodAleatorio") %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label5" runat="server" Text="Modelo:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblModelo" runat="server" Text='<%# Eval("Modelo") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label4" runat="server" Text="Série:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblSerie" runat="server" Text='<%# Eval("Serie") %>'></asp:Label>
                                    </div>
                                    <%--<div class="dtvHeaderRO">
                                        <asp:Label ID="Label6" runat="server" Text="Data Emissão:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label runat="server" ID="lblDataEmissao" Text='<%# Convert.ToDateTime(Eval("DataEmissao").ToString()).ToShortDateString() %>'></asp:Label>
                                    </div>--%>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label7" runat="server" Text="Tipo Emissão:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label runat="server" ID="lblTipoEmissao" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.Cte)Container.DataItem).TipoEmissaoString %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label8" runat="server" Text="Tipo CT-e:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label runat="server" ID="lblTipoCte" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.Cte)Container.DataItem).TipoCteString %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label10" runat="server" Text="Tipo Serviço:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label runat="server" ID="Label16" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.Cte)Container.DataItem).TipoServicoString %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label11" runat="server" Text="Retirada:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label runat="server" ID="lblRetirada" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.Cte)Container.DataItem).Retirada ? "Sim" : "Não"  %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label13" runat="server" Text="Valor Total:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblValorTotal" runat="server" Text='<%# Eval("ValorTotal", "{0:C}") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label14" runat="server" Text="Valor Receber:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblValorReceber" runat="server" Text='<%# Eval("ValorReceber", "{0:C}") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="lblGerarContasReceber1" runat="server" Text="Gerar conta à receber:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblGerarContasReceber" runat="server" Text='<%# (bool)Eval("GerarContasReceber") ? "Sim" : "Não" %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label15" runat="server" Text="Inform. Adicionais:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowROMaior">
                                        <asp:Label ID="lblInformAdicionais" runat="server" Text='<%# Eval("InformAdicionais") %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label12" runat="server" Text="Detalhes Retirada:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblDetRetirada" runat="server" Text='<%# Eval("DetalhesRetirada") %>'></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <asp:Repeater runat="server" ID="rptCobranca" DataSource='<%# Eval("ObjCobrancaCte.ObjCobrancaDuplCte")%>'>
                                <HeaderTemplate>
                                    <div class="dtvTitulo">
                                        Duplicata
                                    </div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="dtvRowRO">
                                        <div class="dtvHeaderRO">
                                            <asp:Label ID="Label19" runat="server" Text="Número Duplicata:"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRowRO">
                                            <asp:Label ID="lblNumDupl" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.CobrancaDuplCte)Container.DataItem).NumeroDupl %>'></asp:Label>
                                        </div>
                                        <div class="dtvHeaderRO">
                                            <asp:Label ID="Label20" runat="server" Text="Data Vencimento:"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRowRO">
                                            <asp:Label ID="lblDataNascimento" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.CobrancaDuplCte)Container.DataItem).DataVenc %>'></asp:Label>
                                        </div>
                                        <div class="dtvHeaderRO">
                                            <asp:Label ID="Label22" runat="server" Text="Valor Duplicata:"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRowRO">
                                            <asp:Label ID="lblValorDuplicata" runat="server" Text='<%# Eval("ValorDupl").ToString() != "0" ? String.Format("{0:C}",Eval("ValorDupl")) : "" %>'></asp:Label>
                                        </div>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                </FooterTemplate>
                            </asp:Repeater>
                            <div class="dtvTitulo">
                                Fatura
                            </div>
                            <div class="item">
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label23" runat="server" Text="Número Fatura:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblNumFatura" runat="server" Text='<%# Eval("ObjCobrancaCte.NumeroFatura") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label24" runat="server" Text="Valor Fatura:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblValorOrigFatura" runat="server" Text='<%# Eval("ObjCobrancaCte.ValorOrigFatura").ToString() != "0,00" ? Eval("ObjCobrancaCte.ValorOrigFatura", "{0:C}") : "" %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label25" runat="server" Text="Desconto Fatura:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblDescFatura" runat="server" Text='<%# Eval("ObjCobrancaCte.DescontoFatura").ToString() != "0,00" ? Eval("ObjCobrancaCte.DescontoFatura", "{0:C}") : "" %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label26" runat="server" Text="Valor Líquido Fatura:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblVlrLiquidoFatura" runat="server" Text='<%# Eval("ObjCobrancaCte.ValorLiquidoFatura").ToString() != "0,00" ? Eval("ObjCobrancaCte.ValorLiquidoFatura", "{0:C}") : "" %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO" runat="server" visible='<%# IsEntradaTerceiros() %>'>
                                        <asp:Label ID="Label116" runat="server" Text="Contas a Pagar:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO" runat="server" visible='<%# IsEntradaTerceiros() %>'>
                                        <asp:Label ID="Label119" runat="server" Text='<%# ((bool)Eval("ObjCobrancaCte.GerarContasPagar") ? "G" : "Não g") + "erar contas a pagar" %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO" runat="server" visible='<%# IsEntradaTerceiros() %>'>
                                        <asp:Label ID="Label120" runat="server" Text="Plano de Contas:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO" runat="server" visible='<%# IsEntradaTerceiros() %>'>
                                        <asp:Label ID="Label125" runat="server" Text='<%# Eval("ObjCobrancaCte.DescricaoPlanoContas") %>'></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <asp:Repeater runat="server" ID="rptVeiculo" Visible='<%# ((System.Collections.Generic.List<WebGlass.Business.ConhecimentoTransporte.Entidade.VeiculoCte>)Eval("ObjVeiculoCte")).Count > 0 %>'
                                DataSource='<%# Eval("ObjVeiculoCte")%>'>
                                <HeaderTemplate>
                                    <div class="dtvTitulo">
                                        Veículo
                                    </div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="item">
                                        <div class="dtvRowRO">
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label27" runat="server" Text="Placa:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="lblPlaca" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.VeiculoCte)Container.DataItem).Placa %>'></asp:Label>
                                            </div>
                                            <%--<div class="dtvHeaderRO">
                                        <asp:Label ID="Label29" runat="server" Text="Valor Frete"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="Label30" runat="server" Text='<%# String.Format("{0:c}", ((WebGlass.Business.ConhecimentoTransporte.Entidade.VeiculoCte)Container.DataItem).ValorFrete) %>'></asp:Label>
                                    </div>--%>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <div class="dtvTitulo" runat="server" id="lblSeguro" onload="NaoExibirDadosTerceiros">
                                Seguro
                            </div>
                            <div class="item" runat="server" id="divSeguro" onload="NaoExibirDadosTerceiros">
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label28" runat="server" Text="Nome Seguradora:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblNomeSeguradora" runat="server" Text='<%# Eval("ObjSeguroCte.NomeSeguradora") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label32" runat="server" Text="Responsável Seguro:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblRespSeguro" runat="server" Text='<%# Eval("ObjSeguroCte.DescricaoResponsavelSeguro") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label31" runat="server" Text="Número Apólice:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblNumApolice" runat="server" Text='<%# Eval("ObjSeguroCte.NumeroApolice") %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label34" runat="server" Text="Número Averbação:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblNumAverbacao" runat="server" Text='<%# Eval("ObjSeguroCte.NumeroAverbacao") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label38" runat="server" Text="Valor Carga Averbação:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblVlrCargaAverb" runat="server" Text='<%# Eval("ObjSeguroCte.ValorCargaAverbacao").ToString() != "0,00" ? String.Format("{0:C}", Eval("ObjSeguroCte.ValorCargaAverbacao")) : "" %>'></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="dtvTitulo">
                                Entrega
                            </div>
                            <div class="item">
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label33" runat="server" Text="Tipo Período Data:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblTpPerData" runat="server" Text='<%# Eval("ObjEntregaCte.DescricaoTipoPeriodoData") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label39" runat="server" Text="Tipo Período Hora:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="lblTpPeriodoHora" runat="server" Text='<%# Eval("ObjEntregaCte.DescricaoTipoPeriodoHora") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label35" runat="server" Text="Data/Hora Programada:" Visible='<%# Eval("ObjEntregaCte.TipoPeriodoData").ToString() == "1" || Eval("ObjEntregaCte.TipoPeriodoHora").ToString() == "1"%>'></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="Label40" runat="server" Visible='<%# Eval("ObjEntregaCte.TipoPeriodoData").ToString() == "1" || Eval("ObjEntregaCte.TipoPeriodoHora").ToString() == "1"%>'
                                            Text='<%# Convert.ToDateTime(Eval("ObjEntregaCte.DataHoraProg")).ToShortDateString() + " " + Convert.ToDateTime(Eval("ObjEntregaCte.DataHoraProg")).ToShortTimeString()%>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <%--<div class="dtvHeaderRO">
                                        <asp:Label ID="Label41" runat="server" Text="Previsão inicial:" Visible='<%# Eval("ObjEntregaCte.TipoPeriodoData").ToString() == "3" ||  Eval("ObjEntregaCte.TipoPeriodoHora").ToString() == "3" %>'></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="Label42" runat="server" Visible='<%# Eval("ObjEntregaCte.TipoPeriodoData").ToString() == "3" || Eval("ObjEntregaCte.TipoPeriodoHora").ToString() == "4"%>'
                                            Text='<%# Eval("ObjEntregaCte.DataHoraIni") == "3"
                                        ? Convert.ToDateTime(Eval("ObjEntregaCte.DataHoraIni")).ToShortTimeString() + " " + Convert.ToDateTime(Eval("ObjEntregaCte.DataHoraIni")).ToShortDateString()
                                        : Convert.ToDateTime(Eval("ObjEntregaCte.DataHoraIni")).ToShortTimeString() + " " + Convert.ToDateTime(Eval("ObjEntregaCte.DataHoraIni")).ToShortDateString()
                                        + " a " + Convert.ToDateTime(Eval("ObjEntregaCte.DataHoraFim").ToString()).ToShortTimeString() + " " + Convert.ToDateTime(Eval("ObjEntregaCte.DataHoraFim").ToString()).ToShortDateString()
                                           %>'></asp:Label>
                                    </div>--%>
                                    <%--<div class="dtvHeaderRO">
                                        <asp:Label ID="Label45" runat="server" Visible='<%# Eval("ObjEntregaCte.TipoPeriodoData").ToString() == "3" || Eval("ObjEntregaCte.TipoPeriodoData").ToString() == "4"
                                        || Eval("ObjEntregaCte.TipoPeriodoHora").ToString() == "3" || Eval("ObjEntregaCte.TipoPeriodoHora").ToString() == "4" %>'
                                            Text="Data/Hora Final:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="Label46" runat="server" Visible='<%# Eval("ObjEntregaCte.TipoPeriodoData").ToString() == "3" || Eval("ObjEntregaCte.TipoPeriodoData").ToString() == "4"
                                        || Eval("ObjEntregaCte.TipoPeriodoHora").ToString() == "3" || Eval("ObjEntregaCte.TipoPeriodoHora").ToString() == "4" %>'
                                            Text='<%# Eval("ObjEntregaCte.DataHoraFim") == "3"
                                        ? Convert.ToDateTime(Eval("ObjEntregaCte.DataHoraFim")).ToShortTimeString() + " " + Convert.ToDateTime(Eval("ObjEntregaCte.DataHoraFim")).ToShortDateString()
                                        : Convert.ToDateTime(Eval("ObjEntregaCte.DataHoraIni")).ToShortTimeString() + " " + Convert.ToDateTime(Eval("ObjEntregaCte.DataHoraIni")).ToShortDateString()
                                        + " a " + Convert.ToDateTime(Eval("ObjEntregaCte.DataHoraFim")).ToShortTimeString() + " " + Convert.ToDateTime(Eval("ObjEntregaCte.DataHoraFim")).ToShortDateString()
                                           %>'></asp:Label>
                                    </div>--%>
                                </div>
                            </div>
                            <asp:Repeater runat="server" ID="rptComponente" Visible='<%# ((System.Collections.Generic.List<WebGlass.Business.ConhecimentoTransporte.Entidade.ComponenteValorCte>)Eval("ObjComponenteValorCte")).Count > 0 %>'
                                DataSource='<%# Eval("ObjComponenteValorCte")%>'>
                                <HeaderTemplate>
                                    <div class="dtvTitulo">
                                        Componente Valor
                                    </div>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="item">
                                        <div class="dtvRowRO">
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label43" runat="server" Text="Nome Componente:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label44" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.ComponenteValorCte)Container.DataItem).NomeComponente %>'></asp:Label>
                                            </div>
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label47" runat="server" Text="Valor Componente:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label48" runat="server" Text='<%# String.Format("{0:C}",((WebGlass.Business.ConhecimentoTransporte.Entidade.ComponenteValorCte)Container.DataItem).ValorComponente) %>'></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <div class="dtvTitulo">
                                Info CT-e
                            </div>
                            <div class="item">
                                <asp:Repeater runat="server" ID="rptInfo" DataSource='<%# Eval("ObjInfoCte.ObjInfoCargaCte")%>'>
                                    <ItemTemplate>
                                        <div class="dtvRowRO">
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label49" runat="server" Text="Tipo Unidade:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label50" runat="server" Text='<%# Eval("DescricaoTipoUnidade") %>'></asp:Label>
                                            </div>
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label51" runat="server" Text="Tipo Medida:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label52" runat="server" Text='<%# Eval("TipoMedida") %>'></asp:Label>
                                            </div>
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label53" runat="server" Text="Quantidade:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label55" runat="server" Text='<%# Eval("Quantidade")%>'></asp:Label>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label56" runat="server" Text="Produto:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowROMaior">
                                        <asp:Label ID="Label57" runat="server" Text='<%# Eval("ObjInfoCte.ProdutoPredominante") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label58" runat="server" Text="Valor Carga:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="Label59" runat="server" Text='<%# Eval("ObjInfoCte.ValorCarga").ToString() != "0,00" ? Eval("ObjInfoCte.ValorCarga") : "" %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label60" runat="server" Text="Outras Características:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowROMaior">
                                        <asp:Label ID="Label61" runat="server" Text='<%# Eval("ObjInfoCte.OutrasCaract") %>'></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="dtvTitulo">
                                Impostos CT-e
                            </div>
                            <asp:Repeater runat="server" ID="rptImpostos" DataSource='<%# Eval("ObjImpostoCte") %>'>
                                <ItemTemplate>
                                    <div class="dtvSubTitulo">
                                        <asp:Label ID="Label41" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.ImpostoCte)Container.DataItem).DescricaoTipoImposto %>'></asp:Label>
                                    </div>
                                    <div class="item">
                                        <div class="dtvRowRO">
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label62" runat="server" Text="CST:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label63" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.ImpostoCte)Container.DataItem).Cst %>'></asp:Label>
                                            </div>
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label64" runat="server" Text="Valor BC:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label65" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.ImpostoCte)Container.DataItem).BaseCalc.ToString("C") %>'></asp:Label>
                                            </div>
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label66" runat="server" Text="Percentual Red. BC:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label67" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.ImpostoCte)Container.DataItem).PercRedBaseCalc %>'></asp:Label>
                                            </div>
                                        </div>
                                        <div class="dtvRowRO">
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label68" runat="server" Text="Alíquota:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label70" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.ImpostoCte)Container.DataItem).Aliquota %>'></asp:Label>
                                            </div>
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label71" runat="server" Text="Valor:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label72" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.ImpostoCte)Container.DataItem).Valor.ToString("C") %>'></asp:Label>
                                            </div>
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label73" runat="server" Text="BC ST Retido:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label74" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.ImpostoCte)Container.DataItem).BaseCalcStRetido.ToString("C") %>'></asp:Label>
                                            </div>
                                        </div>
                                        <div class="dtvRowRO">
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label75" runat="server" Text="Alíquota ST Retido:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label76" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.ImpostoCte)Container.DataItem).AliquotaStRetido %>'></asp:Label>
                                            </div>
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label77" runat="server" Text="Valor ST Retido:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label78" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.ImpostoCte)Container.DataItem).ValorStRetido.ToString("C") %>'></asp:Label>
                                            </div>
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label79" runat="server" Text="Valor Crédito:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label80" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.ImpostoCte)Container.DataItem).ValorCred.ToString("C") %>'></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <div class="dtvTitulo" runat="server" id="Div6" onload="NaoExibirDadosTerceiros">
                                Conhecimento Transporte Rodoviário
                            </div>
                            <div runat="server" id="div12" onload="NaoExibirDadosTerceiros">
                                <div class="item">
                                    <div class="dtvRowRO">
                                        <div class="dtvHeaderRO">
                                            <asp:Label ID="Label83" runat="server" Text="Lotação:"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRowRO">
                                            <asp:Label ID="Label84" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.Cte)Container.DataItem).ObjConhecimentoTransporteRodoviario.Lotacao ? "Sim" : "Não"  %>'></asp:Label>
                                        </div>
                                        <asp:Repeater runat="server" DataSource='<%# Eval("ObjConhecimentoTransporteRodoviario.ObjLacreCteRod") %>'
                                            ID="rptLacre">
                                            <ItemTemplate>
                                                <div class="dtvHeaderRO">
                                                    <asp:Label ID="Label85" runat="server" Text="Número Lacre:"></asp:Label>
                                                </div>
                                                <div class="dtvAlternatingRowRO">
                                                    <asp:Label ID="Label86" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.LacreCteRod)Container.DataItem).NumeroLacre%>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                                <div id="div15" runat="server" onload="div15_Load">
                                    <div class="item">
                                        <asp:Repeater runat="server" DataSource='<%# Eval("ObjConhecimentoTransporteRodoviario.ObjOrdemColetaCteRod") %>'
                                            ID="rptOrdemColeta">
                                            <ItemTemplate>
                                                <div class="dtvRowRO">
                                                    <div class="dtvHeaderRO">
                                                        <asp:Label ID="Label89" runat="server" Text="Transportador:"></asp:Label>
                                                    </div>
                                                    <div class="dtvAlternatingRowROMaior">
                                                        <asp:Label ID="Label90" runat="server" Text='<%# Glass.Data.DAL.TransportadorDAO.Instance.GetElement(((WebGlass.Business.ConhecimentoTransporte.Entidade.OrdemColetaCteRod)Container.DataItem).IdTransportador).Nome %>'></asp:Label>
                                                    </div>
                                                    <div class="dtvHeaderRO">
                                                        <asp:Label ID="Label91" runat="server" Text="Número Ordem Coleta:"></asp:Label>
                                                    </div>
                                                    <div class="dtvAlternatingRowRO">
                                                        <asp:Label ID="Label92" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.OrdemColetaCteRod)Container.DataItem).Numero %>'></asp:Label>
                                                    </div>
                                                </div>
                                                <div class="dtvRowRO">
                                                    <div class="dtvHeaderRO">
                                                        <asp:Label ID="Label93" runat="server" Text="Série:"></asp:Label>
                                                    </div>
                                                    <div class="dtvAlternatingRowRO">
                                                        <asp:Label ID="Label94" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.OrdemColetaCteRod)Container.DataItem).Serie %>'></asp:Label>
                                                    </div>
                                                    <div class="dtvHeaderRO">
                                                        <asp:Label ID="Label95" runat="server" Text="Data Emissão:"></asp:Label>
                                                    </div>
                                                    <div class="dtvAlternatingRowRO">
                                                        <asp:Label ID="Label96" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.OrdemColetaCteRod)Container.DataItem).DataEmissao %>'></asp:Label>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                    <div class="item">
                                        <asp:Repeater runat="server" DataSource='<%# Eval("ObjConhecimentoTransporteRodoviario.ObjValePedagioCteRod") %>'
                                            ID="rptVale">
                                            <ItemTemplate>
                                                <div class="dtvRowRO">
                                                    <div class="dtvHeaderRO">
                                                        <asp:Label ID="Label97" runat="server" Text="Fornecedor:"></asp:Label>
                                                    </div>
                                                    <div class="dtvAlternatingRowRO">
                                                        <asp:Label ID="Label98" runat="server" Text='<%# Glass.Data.DAL.FornecedorDAO.Instance.GetElement(((WebGlass.Business.ConhecimentoTransporte.Entidade.ValePedagioCteRod)Container.DataItem).IdFornec).Nomefantasia %>'></asp:Label>
                                                    </div>
                                                    <div class="dtvHeaderRO">
                                                        <asp:Label ID="Label99" runat="server" Text="Número Compra:"></asp:Label>
                                                    </div>
                                                    <div class="dtvAlternatingRowRO">
                                                        <asp:Label ID="Label100" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.ValePedagioCteRod)Container.DataItem).NumeroCompra %>'></asp:Label>
                                                    </div>
                                                    <div class="dtvHeaderRO">
                                                        <asp:Label ID="Label101" runat="server" Text="Cnpj Comprador:"></asp:Label>
                                                    </div>
                                                    <div class="dtvAlternatingRowRO">
                                                        <asp:Label ID="Label102" runat="server" Text='<%# ((WebGlass.Business.ConhecimentoTransporte.Entidade.ValePedagioCteRod)Container.DataItem).CnpjComprador%>'></asp:Label>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </div>
                            <div class="dtvTitulo" runat="server" id="Div8" onload="divctrlComplCte_Load">
                                Complemento CT-e
                            </div>
                            <div class="item" runat="server" id="div9" onload="divctrlComplCte_Load">
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label105" runat="server" Text="Sigla Passagem:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="Label106" runat="server" Text='<%# Eval("ObjComplCte.ObjComplPassagemCte.SiglaPassagem") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label107" runat="server" Text="Rota:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="Label108" runat="server" Text='<%# Eval("ObjComplCte.IdRota").ToString() != "0" ? Glass.Data.DAL.RotaDAO.Instance.GetElement(Convert.ToUInt32(Eval("ObjComplCte.IdRota"))).Descricao : "" %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label109" runat="server" Text="Caract. Transporte:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="Label110" runat="server" Text='<%# Eval("ObjComplCte.CaractTransporte") %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label111" runat="server" Text="Caract. Serviço:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="Label112" runat="server" Text='<%# Eval("ObjComplCte.CaractServico") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label113" runat="server" Text="Sigla Origem:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="Label114" runat="server" Text='<%# Eval("ObjComplCte.SiglaOrigem") %>'></asp:Label>
                                    </div>
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label117" runat="server" Text="Sigla Destino:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="Label118" runat="server" Text='<%# Eval("ObjComplCte.SiglaDestino") %>'></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="dtvTitulo">
                                Participante CT-e
                            </div>
                            <div class="item">
                                <%--<div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label115" runat="server" Text="Número Sequência"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowRO">
                                        <asp:Label ID="Label116" runat="server" Text='<%# Eval("ObjParticipanteCte[0].NumSeq") %>'></asp:Label>
                                    </div>
                                </div>--%>
                                <asp:Repeater runat="server" DataSource='<%# Eval("ObjParticipanteCte") %>' ID="rptParticipante">
                                    <ItemTemplate>
                                        <div class="dtvRowRO">
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label121" runat="server" Text="Tipo Participante:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowRO">
                                                <asp:Label ID="Label122" runat="server" Text='<%# Eval("DescricaoTipoParticipante") %>'></asp:Label>
                                            </div>
                                            <div class="dtvHeaderRO">
                                                <asp:Label ID="Label123" runat="server" Text="Participante:"></asp:Label>
                                            </div>
                                            <div class="dtvAlternatingRowROMaior">
                                                <asp:Label ID="Label124" runat="server" Text='<%# Eval("NomeParticipante") %>'></asp:Label>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                            <div class="dtvTitulo">
                                Dados para EFD do CT-e
                            </div>
                            <div class="item">
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label121" runat="server" Text="Natureza BC do Crédito:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowROMaior">
                                        <asp:Label ID="Label122" runat="server" Text='<%# Eval("ObjEfdCte.DescrNaturezaBcCred") %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO" style="height: 30px">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label123" runat="server" Text="Indicador Natureza Frete:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowROMaior">
                                        <asp:Label ID="Label42" runat="server" Text='<%# Eval("ObjEfdCte.DescrIndNaturezaFrete") %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO" style="height: 30px">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label45" runat="server" Text="Tipo de Contribuição Social:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowROMaior">
                                        <asp:Label ID="Label46" runat="server" Text='<%# Eval("ObjEfdCte.DescrCodCont") %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label69" runat="server" Text="Tipo de Crédito:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowROMaior">
                                        <asp:Label ID="Label103" runat="server" Text='<%# Eval("ObjEfdCte.DescrCodCred") %>'></asp:Label>
                                    </div>
                                </div>
                                <div class="dtvRowRO">
                                    <div class="dtvHeaderRO">
                                        <asp:Label ID="Label104" runat="server" Text="Plano de Conta Contábil:"></asp:Label>
                                    </div>
                                    <div class="dtvAlternatingRowROMaior">
                                        <asp:Label ID="Label115" runat="server" Text='<%# Eval("ObjEfdCte.DescrCodCred") %>'></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <div class="dtvTitulo">
                                Dados CT-e
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label21" runat="server" Text="Natureza de Operação *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <uc2:ctrlNaturezaOperacao ID="ctrlNaturezaOperacao" runat="server" CodigoNaturezaOperacao='<%# Bind("IdNaturezaOperacao") %>'
                                        PermitirVazio="False" ValidationGroup="c" ErrorMessage="Preencha a natureza de operação" />
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label6" runat="server" Text="Tipo Documento CT-e *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpTipoDocumentoCte" runat="server" Enabled="false" SelectedValue='<%# Bind("TipoDocumentoCte") %>'>
                                        <asp:ListItem Value="2">Saída</asp:ListItem>
                                        <asp:ListItem Value="3">Entrada (terceiros)</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="dtvRow" runat="server" id="terceiros1" onload="ExibirDadosTerceiros">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label2" runat="server" Text="Número CT-e *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtNumCte" runat="server" MaxLength="50" Width="200px" Text='<%# Bind("NumeroCte") %>'
                                        onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvNumCte" runat="server" ErrorMessage="Número do CT-e deve ser preenchido"
                                        ControlToValidate="txtNumCte" Display="Dynamic" ValidationGroup="c">*</asp:RequiredFieldValidator>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label36" runat="server" Text="Data Emissão *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <uc2:ctrlData ID="ctrlDataEmissao" runat="server" ReadOnly="ReadWrite" DataNullable='<%# Bind("DataEmissao") %>'
                                        ValidateEmptyText="true" ValidationGroup="c" ErrorMessage="Campo Data Emissão deve ser preenchido."
                                        ExibirHoras="True" />
                                </div>
                            </div>
                            <div class="dtvRow" runat="server" id="terceiros2" onload="ExibirDadosTerceiros">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label9" runat="server" Text="Chave Acesso"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtChaveAcesso" runat="server" MaxLength="50" Width="200px" Text='<%# Bind("ChaveAcesso") %>'></asp:TextBox>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label126" runat="server" Text="Data Entrada/Saída"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <uc2:ctrlData ID="ctrlDataEntradaSaida" runat="server" ReadOnly="ReadWrite" DataNullable='<%# Bind("DataEntradaSaida") %>'
                                        ValidateEmptyText="false" ValidationGroup="c" ErrorMessage="Campo Data Entrada Saída deve ser preenchido."
                                        ExibirHoras="True" />
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label54" runat="server" Text="Cidade de Envio do CT-e*"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCidadeCte" Text='<%# Glass.Data.DAL.CidadeDAO.Instance.GetNome(((WebGlass.Business.ConhecimentoTransporte.Entidade.Cte)Container.DataItem).IdCidadeCte) %>'
                                        runat="server" MaxLength="50" Enabled="False" Width="200px" ReadOnly="True"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvTxtCidadeCte" runat="server" ErrorMessage="campo cidade não pode ser vazio."
                                        ControlToValidate="txtCidadeCte" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
                                    <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx?controleTxt=txtCidadeCte&controleHdf=hdfCidadeCte'); return false;" />
                                    <asp:HiddenField ID="hdfCidadeCte" runat="server" Value='<%# Bind("IdCidadeCte") %>' />
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label17" runat="server" Text="Cidade Início *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCidadeInicio" Text='<%# Glass.Data.DAL.CidadeDAO.Instance.GetNome(((WebGlass.Business.ConhecimentoTransporte.Entidade.Cte)Container.DataItem).IdCidadeInicio) %>'
                                        runat="server" Enabled="False" MaxLength="50" Width="200px" ReadOnly="True"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvTxtCidadeInicio" runat="server" ErrorMessage="campo cidade início não pode ser vazio."
                                        ControlToValidate="txtCidadeInicio" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
                                    <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx?controleTxt=txtCidadeInicio&controleHdf=hdfCidadeInicio'); return false;" />
                                    <asp:HiddenField ID="hdfCidadeInicio" runat="server" Value='<%# Bind("IdCidadeInicio") %>' />
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label18" runat="server" Text="Cidade Fim *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCidadeFim" runat="server" Text='<%# Glass.Data.DAL.CidadeDAO.Instance.GetNome(((WebGlass.Business.ConhecimentoTransporte.Entidade.Cte)Container.DataItem).IdCidadeFim) %>'
                                        Enabled="False" MaxLength="50" Width="200px" ReadOnly="True"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvTxtCidadeFim" runat="server" ErrorMessage="campo cidade fim não pode ser vazio."
                                        ControlToValidate="txtCidadeFim" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
                                    <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx?controleTxt=txtCidadeFim&controleHdf=hdfCidadeFim'); return false;" />
                                    <asp:HiddenField ID="hdfCidadeFim" runat="server" Value='<%# Bind("IdCidadeFim") %>' />
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label1" runat="server" Text="CT-e Anterior"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCteAnterior" runat="server" MaxLength="50" Width="200px" Text='<%# Bind("IdCteAnterior") %>'
                                        ReadOnly="true"></asp:TextBox>
                                </div>
                                <%--<div class="dtvHeader">
                                    <asp:Label ID="Label36" runat="server" Text="Cidade Orig. Frete"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCidadeOrigFrete" runat="server" Text='<%# Glass.Data.DAL.CidadeDAO.Instance.GetNome(((WebGlass.Business.ConhecimentoTransporte.Entidade.Cte)Container.DataItem).IdCidadeOrigFrete) %>' Enabled="False" MaxLength="50"
                                        Width="200px" ReadOnly="True"></asp:TextBox>
                                    <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                        OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx?controleTxt=txtCidadeOrigFrete&controleHdf=hdfCidadeOrigFrete'); return false;" />
                                    <asp:HiddenField ID="hdfCidadeOrigFrete" runat="server" Value='<%# Bind("IdCidadeOrigFrete") %>' />
                                </div>--%>
                            </div>
                            <%--<div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label69" runat="server" Text="Cidade Dest. Frete"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCidadeDestFrete" runat="server" Text='<%# Glass.Data.DAL.CidadeDAO.Instance.GetNome(((WebGlass.Business.ConhecimentoTransporte.Entidade.Cte)Container.DataItem).IdCidadeDestFrete) %>' Enabled="False" MaxLength="50"
                                        Width="200px" ReadOnly="True"></asp:TextBox>
                                    <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                        OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx?controleTxt=txtCidadeDestFrete&controleHdf=hdfCidadeDestFrete'); return false;" />
                                    <asp:HiddenField ID="hdfCidadeDestFrete" runat="server" Value='<%# Bind("IdCidadeDestFrete") %>' />
                                </div>

                            </div>--%>
                            <%--<div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label2" runat="server" Text="Número Cte"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtNumCte" runat="server" MaxLength="50" Width="200px" Text='<%# Bind("NumeroCte") %>'
                                        ReadOnly="True"></asp:TextBox>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label3" runat="server" Text="Cod. Aleatório"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtCodAleatorio" runat="server" Enabled="False"  OnLoad="txtCodAleatorio_Load" MaxLength="50"
                                        Width="80px" Text='<%# Bind("CodAleatorio") %>' ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>--%>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:CheckBox ID="chkGerarContasReceber" runat="server" Checked='<%# Bind("GerarContasReceber") %>' Text="Gerar contas à receber" />
                                </div>
                                <div class="dtvAlternatingRow">
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label5" runat="server" Text="Modelo"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtModelo" runat="server" Enabled="False" OnLoad="txtModelo_Load"
                                        MaxLength="2" Width="30px" Text='<%# Bind("Modelo") %>' ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label4" runat="server" Text="Série *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtSerie" runat="server" MaxLength="3" Width="25px" Text='<%# Bind("Serie") %>'
                                        OnLoad="txtSerie_Load">
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvTxtSerie" runat="server" ErrorMessage="campo série não pode ser vazio."
                                        ControlToValidate="txtSerie" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label7" runat="server" Text="Tipo Emissão *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtTipoEmissao" runat="server" MaxLength="160" Width="200px" Text='<%# Eval("TipoEmissaoString") %>'
                                        Enabled="false" ReadOnly="true" OnLoad="txtTipoEmissao_Load"></asp:TextBox>
                                    <asp:HiddenField runat="server" ID="hdfTipoEmissao" Value='<%# Bind("TipoEmissao") %>' />
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label8" runat="server" Text="Tipo CT-e *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpTipoCte" runat="server" Height="20px" Width="220px" SelectedValue='<%# Bind("TipoCte") %>' OnLoad="drpTipoCte_Load">
                                        <asp:ListItem Value="selecione" Text="Selecione um Tipo"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="CT-e Normal"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="CT-e de Complemento de Valores"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="CT-e de Anulação de Valores"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="CT-e Substituto"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="cvdrpTipoCte" ControlToValidate="drpTipoCte" runat="server"
                                        ErrorMessage="Selecione um tipo de CT-e" ValueToCompare="selecione" Operator="NotEqual"
                                        ValidationGroup="c">*</asp:CompareValidator>
                                </div>                                
                                <div class="dtvHeader">
                                    <asp:Label ID="Label10" runat="server" Text="Tipo Serviço *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpTipoServico" runat="server" Height="20px" Width="180px"
                                        SelectedValue='<%# Bind("TipoServico") %>'>
                                        <asp:ListItem Value="selecione" Text="Selecione um Tipo"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="Normal"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Subcontratação"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Redespacho"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="Redespacho Intermediário"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="cvdrpTipoServico" ControlToValidate="drpTipoServico" runat="server"
                                        ErrorMessage="Selecione um tipo de serviço" ValueToCompare="selecione" Operator="NotEqual"
                                        ValidationGroup="c">*</asp:CompareValidator>
                                </div>
                            </div>
                            <div class="dtvHeader" id="divLabelDataAnulacao" style="display:none">
                                    <asp:Label ID="Label37" runat="server" Text="Data Anulação"></asp:Label>
                            </div>
                            <div class="dtvAlternatingRow" id="divDataAnulacao" style="display:none">
                                <uc2:ctrlData ID="ctrlDataAnulacao" runat="server"  DataString='<%# Bind("DataAnulacao") %>'/>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label11" runat="server" Text="Retirada"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:CheckBox ID="chkRetirada" runat="server" Checked='<%# Bind("Retirada") %>' Text="Retirada" />
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label12" runat="server" Text="Detalhes Retirada"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtDetRetirada" runat="server" MaxLength="160" Width="350px" Text='<%# Bind("DetalhesRetirada") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label13" runat="server" Text="Valor Total *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtVlrTot" runat="server" MaxLength="20" onclick="mascaraValor(this, 2); return false;"
                                        Width="140px" Text='<%# Bind("ValorTotal", "{0:C}") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvtxtValorTotal" runat="server" ErrorMessage="campo valor total não pode ser vazio."
                                        ControlToValidate="txtVlrTot" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label14" runat="server" Text="Valor Receber *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtValorReceber" runat="server" MaxLength="20" onclick="mascaraValor(this, 2); return false;"
                                        Width="140px" Text='<%# Bind("ValorReceber", "{0:C}") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvtxtValorReceber" runat="server" ErrorMessage="campo valor receber não pode ser vazio."
                                        ControlToValidate="txtValorReceber" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label15" runat="server" Text="Inform. Adicionais"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtInformAdicionais" runat="server" MaxLength="50" Width="650px" Rows="2" TextMode="MultiLine"
                                        Text='<%# Bind("InformAdicionais") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvTitulo">
                                Cobrança
                            </div>
                            <uc1:ctrlDupl ID="CtrlDupl1" runat="server" ObjCobrancaCte='<%# Bind("ObjCobrancaCte") %>'
                                TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <div class="dtvTitulo">
                                Veículo
                            </div>
                            <uc1:ctrlVeiculoCte ID="CtrlVeiculoCte1" runat="server" ObjVeiculoCte='<%# Bind("ObjVeiculoCte") %>'
                                TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <div runat="server" id="divSeguro" class="dtvTitulo" onload="NaoExibirDadosTerceiros">
                                Seguro
                            </div>
                            <div runat="server" id="divCtrlSeguroCte" onload="NaoExibirDadosTerceiros">
                                <uc1:ctrlSeguroCte ID="CtrlSeguroCte" runat="server" ObjSeguroCte='<%# Bind("ObjSeguroCte") %>'
                                    TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            </div>
                            <div class="dtvTitulo">
                                Entrega
                            </div>
                            <uc1:ctrlEntregaCte ID="ctrlEntregaCte" runat="server" ObjEntregaCte='<%# Bind("ObjEntregaCte") %>'
                                TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <div class="dtvTitulo">
                                Componente Valor
                            </div>
                            <uc1:ctrlComponenteValorCte ID="ctrlComponenteValorCte" runat="server" ObjComponenteValorCte='<%# Bind("ObjComponenteValorCte") %>'
                                TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <div class="dtvTitulo">
                                Info CT-e
                            </div>
                            <uc1:ctrlInfoCte ID="ctrlInfoCte" runat="server" ObjInfoCte='<%# Bind("ObjInfoCte")%>'
                                TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <div class="dtvTitulo">
                                Impostos CT-e
                            </div>
                            <uc1:ctrlImpostosCte ID="ctrlImpostosCte" runat="server" ObjImpostoCte='<%# Bind("ObjImpostoCte") %>'
                                TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <div runat="server" id="divConhecimento" class="dtvTitulo" onload="NaoExibirDadosTerceiros">
                                Conhecimento Transporte Rodoviário
                            </div>
                            <div runat="server" id="divctrlConhecimentoTransRod" class="dtvTitulo" onload="NaoExibirDadosTerceiros">
                                <uc1:ctrlConhecimentoTransRod ID="ctrlConhecimentoTransRod" runat="server" ObjCteRod='<%# Bind("ObjConhecimentoTransporteRodoviario") %>'
                                    TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            </div>
                            <div runat="server" id="div10" class="dtvTitulo" onload="divctrlComplCte_Load">
                                Complemento CT-e
                            </div>
                            <div runat="server" id="div11" class="dtvTitulo" onload="divctrlComplCte_Load">
                                <uc1:ctrlComplCte ID="ctrlComplCte" runat="server" ObjComplCte='<%# Bind("ObjComplCte") %>'
                                    TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            </div>
                            <div class="dtvTitulo">
                                Participante CT-e
                            </div>
                            <uc1:ctrlPartCte ID="ctrlPartCte" runat="server" ObjParticipanteCte='<%# Bind("ObjParticipanteCte") %>'
                                TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <div class="dtvTitulo">
                                Dados para EFD do CT-e
                            </div>
                            <uc1:EfdCte ID="ctrlEfdCte" runat="server" ObjEfdCte='<%# Bind("ObjEfdCte") %>' TipoDocumentoCte='<%# GetTipoDocumentoCte() %>' />
                            <asp:Label runat="server" Style="display: none;" ID="lblCodAleatorio" Text='<%# Bind("CodAleatorio") %>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <EditItemTemplate>
                            <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                OnClientClick="return onUpdate();" ValidationGroup="c" />
                            <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                CausesValidation="false" />
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" ValidationGroup="c"
                                OnClientClick="return onInsert();" />
                            <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" Text="Cancelar" />
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" />
                            <asp:Button ID="btnEmitir" runat="server" ForeColor="Red" OnClientClick="return confirm(&quot;Tem certeza que deseja emitir este CTe?&quot;);"
                                Text="Emitir CTe" Width="125px" OnClick="btnEmitir_Click" OnLoad="btnEmitirFinalizar_Load"
                                CausesValidation="false" />
                            <asp:Button ID="btnFinalizar" runat="server" ForeColor="Red" OnClick="btnFinalizar_Click"
                                OnClientClick="return confirm(&quot;Tem certeza que deseja finalizar esse CTe?&quot;)"
                                OnLoad="btnEmitirFinalizar_Load" Text="Finalizar" />
                            <asp:Button ID="btnPreVisualizar" runat="server" OnClick="btnPreVisualizar_Click"
                                OnLoad="btnPreVisualizar_Load" Text="Pré-visualizar" ToolTip="Exibe uma prévia de como ficará o DACTE após emissão do cte." />
                            <asp:Button ID="btnVoltar" runat="server" CausesValidation="false" OnClick="btnCancelar_Click"
                                Text="Voltar" />
                            <asp:ImageButton ID="imgObsLancFiscal" runat="server" ImageUrl="~/Images/Nota.gif"
                                OnClientClick='<%# "openWindow(600, 800, \"../Utils/SetObsLancFiscal.aspx?idCte=" + Eval("IdCte") + "\"); return false" %>'
                                ToolTip="Observações do Lançamento Fiscal" />
                            <asp:ImageButton ID="imgAjustes" runat="server" ImageUrl="~/Images/dinheiro.gif"
                                OnClientClick='<%# Eval("IdCte", "openWindow(600, 950, \"../Listas/LstAjusteDocumentoFiscal.aspx?idCte={0}\"); return false;") %>'
                                ToolTip="Ajustes do Documento Fiscal" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            * campos obrigatórios
        </div>
        <div class="dtv">
            <div id="lnkProduto" runat="server">
                <asp:GridView GridLines="None" ID="grdNfCte" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsNfCte" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdNf,IdCte"
                    OnRowDeleted="grdNf_RowDeleted" ShowFooter="True" OnRowCommand="grdNf_RowCommand"
                    OnRowDataBound="grdNf_RowDataBound" EmptyDataText="Nenhuma Nota fiscal associada">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="if (!confirm('Deseja excluir esse produto?')) return false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif"
                                    ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="NF" SortExpression="NumeroNFe">
                            <ItemTemplate>
                                <asp:Label ID="Label320" runat="server" Text='<%# Eval("NumeroNFe") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblNumeroNFe" runat="server" Text='<%# Eval("NumeroNFe") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumNfIns" runat="server" Columns="6" onblur="buscarNf(this.value);"
                                    onkeypress="return soNumeros(event, true, true);" Enabled="false"></asp:TextBox>
                                <asp:HiddenField runat="server" ID="hdfIdNf" />
                                <asp:Label ID="lblDescrProd0" runat="server"></asp:Label>
                                <a href="#" onclick="abrirBuscaNf(); return false;">
                                    <img src="../Images/Pesquisar.gif" border="0" /></a>
                            </FooterTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Modelo" SortExpression="Modelo">
                            <ItemTemplate>
                                <asp:Label ID="lblModelo" runat="server" Text='<%# Eval("Modelo") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblModeloF" runat="server" Text='<%# Eval("Modelo") %>'></asp:Label>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CFOP" SortExpression="IdCfop">
                            <ItemTemplate>
                                <asp:Label ID="lblIdCfop" runat="server" Text='<%# Eval("CodCfop") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblIdCfopF" runat="server" Text='<%# Eval("CodCfop") %>'></asp:Label>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Doc." SortExpression="TipoDocumento">
                            <ItemTemplate>
                                <asp:Label ID="lblTipoDocumento" runat="server" Text='<%# Eval("TipoDocumentoString") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTipoDocumentoF" runat="server" Text='<%# Eval("TipoDocumentoString") %>'></asp:Label>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Emitente" SortExpression="NomeEmitente">
                            <ItemTemplate>
                                <asp:Label ID="lblEmitente" runat="server" Text='<%# Eval("NomeEmitente") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblEmitenteF" runat="server" Text='<%# Eval("NomeEmitente") %>'></asp:Label>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Emissão" SortExpression="DataEmissao">
                            <ItemTemplate>
                                <asp:Label ID="lblDataEmissao" runat="server" Text='<%# Eval("DataEmissao") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblDataEmissaoF" runat="server" Text='<%# Eval("DataEmissao") %>'></asp:Label>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="lnkInsProd" CausesValidation="false" runat="server" CommandName="Insert"
                                    OnClick="lnkInsCte_Click" Style='display: none' ImageUrl="~/Images/ok.gif"></asp:ImageButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <asp:GridView runat="server" ID="grdChavesAcesso" GridLines="None" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" ShowFooter="True"
                    EmptyDataText="Nenhuma Chave de Acesso associada" DataSourceID="odsChaveAcessoCte"
                    OnRowDeleted="grdChavesAcesso_RowDeleted" OnRowCommand="grdChavesAcesso_RowCommand"
                    DataKeyNames="IdChaveAcessoCte">
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="if (!confirm('Deseja realmente excluir essa chave de acesso?')) return false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif"
                                    ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                            <FooterTemplate>
                                <asp:ImageButton ID="lnkAddChaveAcesso" CausesValidation="false" runat="server" CommandName="Insert"
                                    ImageUrl="~/Images/ok.gif" OnClick="lnkAddChaveAcesso_Click"></asp:ImageButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Chave de Acesso">
                            <ItemTemplate>
                                <asp:Label ID="Label331" runat="server" Text='<%# Eval("ChaveAcesso") %>' onkeypress="return soNumeros(event, true, true);"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumChaveAcesso" runat="server" Text='<%# Bind("ChaveAcesso") %>' Width="300px" MaxLength="44"
                                    onkeypress="return soNumeros(event, true, true)" ></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumChaveAcesso" runat="server" Width="300px" MaxLength="44" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            </FooterTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="PIN">
                            <ItemTemplate>
                                <asp:Label ID="Label330" runat="server" Text='<%# Eval("Pin") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumPin" runat="server" Text='<%# Bind("Pin") %>' MaxLength="9"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNumPin" runat="server" MaxLength="9"></asp:TextBox>
                            </FooterTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Finalidade da Chave de Acesso">
                            <ItemTemplate>
                                <asp:Label ID="lblFinalidadeChaveAcesso" runat="server" Text='<%# Eval("FinalidadeChaveAcesso") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpFinalidadeChaveAcesso" runat="server" SelectedValue='<%# Bind("FinalidadeChaveAcesso") %>'
                                    DataSourceID="odsFinalidadeChaveAcesso" DataTextField="Translation" DataValueField="Key">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpFinalidadeChaveAcesso" runat="server" DataSourceID="odsFinalidadeChaveAcesso"
                                    DataTextField="Translation" DataValueField="Key">
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>

                    <EditRowStyle CssClass="edit"></EditRowStyle>

                    <PagerStyle CssClass="pgr"></PagerStyle>
                </asp:GridView>
            </div>
        </div>
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsConhecimentoTransporte" runat="server"
            SelectMethod="GetCte" InsertMethod="Insert" DataObjectTypeName="WebGlass.Business.ConhecimentoTransporte.Entidade.Cte"
            TypeName="WebGlass.Business.ConhecimentoTransporte.CteOds" UpdateMethod="Update"
            OnInserted="odsConhecimentoTransporte_Inserted" OnUpdated="odsConhecimentoTransporte_Updated">
            <SelectParameters>
                <asp:QueryStringParameter Name="idCte" QueryStringField="idCte" Type="UInt32" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsNFCte" runat="server" SelectMethod="GetList"
            InsertMethod="Insert" DataObjectTypeName="WebGlass.Business.ConhecimentoTransporte.Entidade.NfeCte"
            TypeName="WebGlass.Business.ConhecimentoTransporte.NotaFiscalCteOds" EnablePaging="True"
            SortParameterName="sortExpression" MaximumRowsParameterName="pageSize" StartRowIndexParameterName="startRow"
            UpdateMethod="Update" DeleteMethod="Delete">
            <SelectParameters>
                <asp:QueryStringParameter Name="idCte" QueryStringField="idCte" Type="UInt32" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsChaveAcessoCte" runat="server"
            DataObjectTypeName="Glass.Fiscal.Negocios.Entidades.Cte.ChaveAcessoCte"
            TypeName="Glass.Fiscal.Negocios.ICTeFluxo" EnablePaging="True"
            SelectMethod="PesquisarChavesAcesso"
            SelectByKeysMethod="ObtemChaveAcesso"
            InsertMethod="SalvarChaveAcesso"
            UpdateMethod="SalvarChaveAcesso" UpdateStrategy="GetAndUpdate"
            DeleteMethod="ApagarChaveAcesso" DeleteStrategy="GetAndDelete">
            <SelectParameters>
                <asp:QueryStringParameter Name="idCte" QueryStringField="idCte" Type="Int32" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFinalidadeChaveAcesso" runat="server"
            SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
            <SelectParameters>
                <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.FinalidadeChaveAcesso, Glass.Data" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
        <asp:HiddenField ID="hdfNumProdutos" runat="server" />
        <asp:HiddenField ID="hdfCFOP" runat="server" />
        <asp:HiddenField ID="hdfChaveAcesso" runat="server" />
    </div>

    <script type="text/javascript">
        var numNf = FindControl("txtNumNfIns", "input");
        if (numNf != null) numNf.focus();

        if (typeof pegarValorInfo !== 'undefined' && typeof pegarValorInfo === 'function')
            // Chamado 19338
            setTimeout(function(){ pegarValorInfo("ctl00_ctl00_Pagina_Conteudo_dtvConhecimentoTransporte_ctrlInfoCte_ctrlCargaCte"); }, 2000);

        var controle = FindControl('dtvConhecimentoTransporte_drpTipoCte', 'select');
        exibirEsconderDataAnulacao(controle);

    </script>

</asp:Content>
