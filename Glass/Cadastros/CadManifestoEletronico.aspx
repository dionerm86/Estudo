<%@ Page Title="Cadastro MDF-e" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadManifestoEletronico.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadManifestoEletronico" %>

<%@ Register Src="~/Controls/MDFe/ctrlRodoviario.ascx" TagName="ctrlRodoviario" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/MDFe/ctrlParticipanteMDFe.ascx" TagName="ctrlParticipantes" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/MDFe/ctrlCidadeCarga.ascx" TagName="ctrlCidadesCarga" TagPrefix="uc3" %>
<%@ Register Src="~/Controls/MDFe/ctrlUFPercursoMDFe.ascx" TagName="ctrlUFsPercurso" TagPrefix="uc4" %>
<%@ Register Src="~/Controls/MDFe/ctrlAverbacaoSeguroMDFe.ascx" TagName="ctrlAverbacaoSeguro" TagPrefix="uc6" %>
<%@ Register Src="~/Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <link type="text/css" rel="stylesheet" href="<%= ResolveUrl("~/Style/CTe/CadCTe.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery/jquery.maskMoney.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        //Máscara para campos com valor decimal
        function mascaraValor(controle, precisao) {
            $("#" + controle.id).unmaskMoney();
            $("#" + controle.id).maskMoney({ showSymbol: false, symbol: "R$", decimal: ",", precision: precisao, thousands: "", allowZero: true });
        }

        function onInsert() {

            return validar();
        }

        // Validação de dados de inserção ou atualização
        function validar() {
            var tipoEmitente = FindControl("dtvManifestoEletronico_drpTipoEmitente", "select").value;
            var modelo = FindControl("dtvManifestoEletronico_txtModelo", "input").value;
            var serie = FindControl("dtvManifestoEletronico_txtSerie", "input").value;

            var emitente = FindControl("dtvManifestoEletronico_ctrlParticipantes_ctrlParticipanteEmitente_hdfIdPart", "input").value;

            var valoresPedagio = FindControl("dtvManifestoEletronico_ctrlRodoviario_dtvRodoviario_ctrlPedagio_hdfValorPedagio", "input").value.split(';');

            var veiculoTracao = FindControl("dtvManifestoEletronico_ctrlRodoviario_dtvRodoviario_drpVeiculoTracao", "select").value;
            var condutores = FindControl("dtvManifestoEletronico_ctrlRodoviario_dtvRodoviario_ctrlCondutorVeiculo_hdfCondutores", "input").value.split(';');

            var lacres = FindControl("dtvManifestoEletronico_ctrlRodoviario_dtvRodoviario_ctrlLacreRodoviario_hdfLacres", "input").value.split(';');

            var responsavelSeguro = FindControl("dtvManifestoEletronico_drpResponsavelSeguro", "select").value;
            var seguradora = FindControl("dtvManifestoEletronico_drpSeguradora", "select").value;

            var valorCarga = FindControl("dtvManifestoEletronico_txtValorCarga", "input").value;
            var codigoUnidade = FindControl("dtvManifestoEletronico_drpCodigoUnidade", "select").value;
            var quantidadeCarga = FindControl("dtvManifestoEletronico_txtQuantidadeCarga", "input").value;

            // IDENTIFICAÇÃO DO MDFe
            if (modelo == null || modelo == '') {
                alert('É necessário informar o modelo do MDF-e');
                return false;
            }
            if (serie == null || serie == '') {
                alert('É necessário informar a série do MDF-e');
                return false;
            }

            //IDENTIFICAÇÃO DO EMITENTE
            if (emitente == null || emitente == '') {
                alert('É necessário informar o Emitente do MDF-e');
                return false;
            }

            //INFORMAÇÕES DO MODAL
            // valoresPedagio
            for (var i = 0; i < valoresPedagio.length; i++) {
                if (valoresPedagio[i] != null && valoresPedagio[i] != '') {
                    if (valoresPedagio[i].split(',')[0].length > 13 || valoresPedagio[i].split(',')[1].length > 2) {
                        alert('O valor do vale pedágio é maior que o permitido');
                        return false;
                    }
                }
            }
            if (veiculoTracao == null || veiculoTracao == '') {
                alert('É necessário informar o Veículo Tração do MDF-e');
                return false;
            }
            if (condutores == null || condutores.length == 1) {
                alert('É necessário informar o Condutor do MDF-e');
                return false;
            }

            // Verifica se existe lacres repetidos
            if (lacres != null && lacres.length > 1) {
                for (var i = 0; i < lacres.length; i++) {
                    for (var j = 0; j < lacres.length; j++) {
                        if (i != j) {
                            if (lacres[i] == lacres[j]) {
                                alert('Não é possível inserir lacres repetidos.');
                                return false;
                            }
                        }
                    }
                }
            }

            //SEGURO DA CARGA
            if (responsavelSeguro == "Contratante") {
                if (seguradora == null || seguradora == '' || seguradora == 0) {
                    alert('Quando o responsável pelo seguro é o Contratante, é obrigatório informar a Seguradora!');
                    return false;
                }
            }

            //TOTALIZADORES
            if (valorCarga == null || valorCarga == '') {
                alert('É necessário informar o valor da carga do MDF-e');
                return false;
            }
            if (valorCarga.split(',')[0].length > 13 || valorCarga.split(',')[1].length > 2) {
                alert('O valor da carga é maior que o permitido');
                return false;
            }
            if (codigoUnidade == null || codigoUnidade == '') {
                alert('É necessário informar o código da unidade do MDF-e');
                return false;
            }
            if (quantidadeCarga == null || quantidadeCarga == '') {
                alert('É necessário informar a quantidade da carga do MDF-e');
                return false;
            }
            if (quantidadeCarga.split(',')[0].length > 11 || quantidadeCarga.split(',')[1].length > 4) {
                alert('O peso da carga é maior que o permitido');
                return false;
            }
        }

        // Exibe a tabela de associar NFe ou CTe de acordo com o tipo Emitente
        function exibirDocumentos(botao, idCidadeDescarga) {
            var linha = null;
            var tipoEmitente = FindControl("hdfTipoEmitente", "input").value;
            if (tipoEmitente == "TransportadorCargaPropria") {
                linha = document.getElementById("NFesViculadas_" + idCidadeDescarga);
            }
            else {
                linha = document.getElementById("CTesViculados_" + idCidadeDescarga);
            }

            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " Documentos";
        }

        var popUp = "";

        // Abre a tela de associar NFe
        function abrirBuscaNf(botao) {
            popUp = openWindow(600, 800, '../Utils/SelNotaFiscalAutorizada.aspx?IdControle=' + botao.id.substring(0, botao.id.lastIndexOf("_")));
        }

        //Bloqueia a seleção de notas caso sejam informações externas.
        function bloquearSelecaoNota(idControle) {
            FindControl('hdfIdNf', 'input').value = "";
            var chaveAcesso = FindControl('txtChaveAcesso', 'input').value;
            var fsda = FindControl('txtFsda', 'input').value;

            if (chaveAcesso != "" || fsda != "") {
                FindControl('imbSelNotaFiscal', 'input').style.display = "none";
            }
            else {
                FindControl('imbSelNotaFiscal', 'input').style.display = "";
            }
        }

        // Seta informações da NFe selecionada no popup.
        function setNfReferenciada(idControle, idNf, numNf) {
            FindControl(idControle + '_txtNumNfIns', 'input').value = numNf;
            FindControl(idControle + '_hdfIdNf', 'input').value = idNf;

            var retorno = CadManifestoEletronico.BuscarInfoNfe(idNf);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return;
            }
            var resultado = retorno.value.split('|');

            FindControl(idControle + '_txtChaveAcesso', 'input').value = resultado[0];
            FindControl(idControle + '_txtChaveAcesso', 'input').disabled = true;

            FindControl(idControle + '_txtFsda', 'input').value = resultado[1];
            FindControl(idControle + '_txtFsda', 'input').disabled = true;

            return false;
        }

        //Salva as informações da nota referênciada
        function SalvarNfReferenciada() {
            var idCidadeDescarga = FindControl('hdfIdCidadeDescargaNFe', 'input').value;
            var idNf = FindControl('hdfIdNf', 'input').value;
            var chaveAcesso = FindControl('txtChaveAcesso', 'input').value;
            var fsda = FindControl('txtFsda', 'input').value;

            var retorno = CadManifestoEletronico.InserirNfeCidadeDescarga(idCidadeDescarga, idNf, chaveAcesso, fsda);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return;
            }

            var resultado = retorno.value.split('|');

            if (resultado[0] == "Erro") {
                alert(resultado[1]);
                return false;
            }

            alert(resultado[1])

            return true;
        }

        // Abre a tela de associar CTe
        function abrirBuscaCTe(botao) {
            popUp = openWindow(600, 800, '../Utils/SelConhecimentoTransporteAutorizado.aspx?IdControle=' + botao.id.substring(0, botao.id.lastIndexOf("_")));
        }

        //Bloqueia a seleção de notas caso sejam informações externas.
        function bloquearSelecaoCte(idControle) {
            FindControl('hdfIdCTe', 'input').value = "";
            var chaveAcesso = FindControl('txtChaveAcessoCte', 'input').value;
            var fsda = FindControl('txtFsdaCTe', 'input').value;

            if (chaveAcesso != "" || fsda != "") {
                FindControl('imbSelCTe', 'input').style.display = "none";
            }
            else {
                FindControl('imbSelCTe', 'input').style.display = "";
            }
        }

        // Seta informações da CTe selecionada no popup.
        function setCTeReferenciado(idControle, idCTe, numCTe) {
            FindControl(idControle + '_txtNumCTeIns', 'input').value = numCTe;
            FindControl(idControle + '_hdfIdCTe', 'input').value = idCTe;

            var retorno = CadManifestoEletronico.BuscarInfoCte(idCTe);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return;
            }

            var resultado = retorno.value;

            FindControl(idControle + '_txtChaveAcessoCte', 'input').value = resultado;
            FindControl(idControle + '_txtChaveAcessoCte', 'input').disabled = true;

            //Utilizar quando implementar FDSA no CTe
            //FindControl(idControle + '_txtFsdaCTe', 'input').value = retorno;
            FindControl(idControle + '_txtFsdaCTe', 'input').disabled = true;

            return false;
        }

        //Salva as informações de CTe associadas
        function SalvarCTeReferenciada(idControle) {
            var idCidadeDescarga = FindControl('hdfIdCidadeDescargaCTe', 'input').value;
            var idCTe = FindControl('hdfIdCTe', 'input').value;
            var chaveAcesso = FindControl('txtChaveAcessoCte', 'input').value;
            var fsda = FindControl('txtFsdaCTe', 'input').value;

            var retorno = CadManifestoEletronico.InserirCteCidadeDescarga(idCidadeDescarga, idCTe, chaveAcesso, fsda);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return;
            }

            var resultado = retorno.value.split('|');

            if (resultado[0] == "Erro") {
                alert(resultado[1]);
                return false;
            }

            alert(resultado[1])

            return true;
        }

        function setCidade(idCidade, nomeCidade) {
            var retorno = CadManifestoEletronico.InserirCidadeDescargaMdfe(idCidade).value;

            var resultado = retorno.split('|');

            if (resultado[0] == "Erro") {
                alert(resultado[1])
                return false;
            }

            alert(resultado[0])

            //Realiza o postBack da página.
            __doPostBack(null, null);

            return true;
        }

    </script>

    <div class="pagina">
        <div class="campos-obrigatorios">
            <asp:DetailsView ID="dtvManifestoEletronico" runat="server" AutoGenerateRows="false" GridLines="None" Width="100%" DefaultMode="Insert"
                DataKeyNames="IdManifestoEletronico" DataSourceID="odsManifestoEletronico">
                <Fields>
                    <asp:TemplateField ShowHeader="false">
                        <ItemTemplate>
                            <div class="dtvTitulo">
                                Identificação do MDF-e
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label7" runat="server" Text="Número MDF-e"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label53" runat="server" Text='<%# Eval("NumeroManifestoEletronico") %>'></asp:Label>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label8" runat="server" Text="Chave de Acesso"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label54" runat="server" Text='<%# Eval("ChaveAcesso") %>'></asp:Label>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label1" runat="server" Text="Tipo do Emitente"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:HiddenField ID="hdfTipoEmitente" runat="server" Value='<%# Eval("TipoEmitente") %>' />
                                    <asp:Label ID="Label3" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("TipoEmitente")) %>'></asp:Label>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label2" runat="server" Text="Tipo do Trasportador"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label4" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("TipoTransportador")) %>'></asp:Label>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label5" runat="server" Text="Modelo"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label11" runat="server" Text='<%# Eval("Modelo") %>'></asp:Label>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label6" runat="server" Text="Série"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label12" runat="server" Text='<%# Eval("Serie") %>'></asp:Label>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label9" runat="server" Text="Tipo Emissão"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label27" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("TipoEmissao")) %>'></asp:Label>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label10" runat="server" Text="Modalidade de transporte"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label28" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Modal")) %>'></asp:Label>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label29" runat="server" Text="UF Início"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label31" runat="server" Text='<%# Eval("UFInicio") %>'></asp:Label>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label30" runat="server" Text="UF Fim"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label32" runat="server" Text='<%# Eval("UFFim") %>'></asp:Label>
                                </div>
                            </div>
                            <asp:Repeater runat="server" ID="rptCidadesCarga" DataSource='<%# Eval("CidadesCarga")%>'>
                                <ItemTemplate>
                                    <div class="dtvRow">
                                        <div class="dtvHeader">
                                            <asp:Label ID="Label29" runat="server" Text="Cidade Carga"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRow">
                                            <asp:Label ID="Label31" runat="server" Text='<%# Eval("NomeCidade") %>'></asp:Label>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Repeater runat="server" ID="rptUFsPercurso" DataSource='<%# Eval("UFsPercurso")%>'>
                                <ItemTemplate>
                                    <div class="dtvRow">
                                        <div class="dtvHeader">
                                            <asp:Label ID="Label29" runat="server" Text="UF Percurso"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRow">
                                            <asp:Label ID="Label31" runat="server" Text='<%# Eval("UFPercurso") %>'></asp:Label>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label55" runat="server" Text="Data Início Viagem"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label129" runat="server" Text='<%# Eval("DataInicioViagem") %>'></asp:Label>
                                </div>
                            </div>

                            <!-- IDENTIFICAÇÃO DO EMITENTE -->
                            <div class="dtvTitulo">
                                Identificação do Emitente
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label49" runat="server" Text="Emitente"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label51" runat="server" Text='<%# Eval("Emitente") %>'></asp:Label>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label50" runat="server" Text="Contratante"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label52" runat="server" Text='<%# Eval("Contratante") %>'></asp:Label>
                                </div>
                            </div>

                            <!-- INFORMAÇÕES DO MODAL -->
                            <div class="dtvTitulo">
                                Informações do modal
                            </div>
                            <div class="dtvSubTitulo">
                                CIOT
                            </div>
                            <asp:Repeater runat="server" ID="rptCIOT" DataSource='<%# Eval("Rodoviario.CiotRodoviario")%>'>
                                <ItemTemplate>
                                    <div class="dtvRow">
                                        <div class="dtvHeader">
                                            <asp:Label ID="Label29" runat="server" Text="CIOT"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRow">
                                            <asp:Label ID="Label31" runat="server" Text='<%# Eval("CIOT") %>'></asp:Label>
                                        </div>
                                        <div class="dtvHeader">
                                            <asp:Label ID="Label43" runat="server" Text="CPF CNPJ Responsável"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRow">
                                            <asp:Label ID="Label44" runat="server" Text='<%# Eval("CPFCNPJCIOT") %>'></asp:Label>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <div class="dtvSubTitulo">
                                Vale Pedágio
                            </div>
                            <asp:Repeater runat="server" ID="rptPedagioRodoviario" DataSource='<%# Eval("Rodoviario.PedagioRodoviario")%>'>
                                <ItemTemplate>
                                    <div class="dtvRow">
                                        <div class="dtvHeader">
                                            <asp:Label ID="Label29" runat="server" Text="Fornecedor Pedágio"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRow">
                                            <asp:Label ID="Label31" runat="server" Text='<%# Eval("NomeFornecedor") %>'></asp:Label>
                                        </div>
                                        <div class="dtvHeader">
                                            <asp:Label ID="Label45" runat="server" Text="Responsável Pagamento"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRow">
                                            <asp:Label ID="Label28" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("ResponsavelPedagio")) %>'></asp:Label>
                                        </div>
                                    </div>
                                    <div class="dtvRow">
                                        <div class="dtvHeader">
                                            <asp:Label ID="Label43" runat="server" Text="Número Compra"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRow">
                                            <asp:Label ID="Label44" runat="server" Text='<%# Eval("NumeroCompra") %>'></asp:Label>
                                        </div>
                                        <div class="dtvHeader">
                                            <asp:Label ID="Label47" runat="server" Text="Valor Vale Pedágio"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRow">
                                            <asp:Label ID="Label48" runat="server" Text='<%# Eval("ValorValePedagio") %>'></asp:Label>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <div class="dtvSubTitulo">
                                Veículo
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label42" runat="server" Text="Veiculo Tração"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label46" runat="server" Text='<%# Eval("Rodoviario.PlacaTracao") %>'></asp:Label>
                                </div>
                            </div>
                            <asp:Repeater runat="server" ID="rptCondutorVeiculo" DataSource='<%# Eval("Rodoviario.CondutorVeiculo")%>'>
                                <ItemTemplate>
                                    <div class="dtvRow">
                                        <div class="dtvHeader">
                                            <asp:Label ID="Label29" runat="server" Text="Condutor"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRow">
                                            <asp:Label ID="Label31" runat="server" Text='<%# Eval("NomeCondutor") %>'></asp:Label>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Repeater runat="server" ID="rptVeiculoRodoviario" DataSource='<%# Eval("Rodoviario.VeiculoRodoviario")%>'>
                                <ItemTemplate>
                                    <div class="dtvRow">
                                        <div class="dtvHeader">
                                            <asp:Label ID="Label29" runat="server" Text="Veiculo Reboque"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRow">
                                            <asp:Label ID="Label31" runat="server" Text='<%# Eval("Placa") %>'></asp:Label>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <div class="dtvSubTitulo">
                                Lacres
                            </div>
                            <asp:Repeater runat="server" ID="rptLacreRodoviario" DataSource='<%# Eval("Rodoviario.LacreRodoviario")%>'>
                                <ItemTemplate>
                                    <div class="dtvRow">
                                        <div class="dtvHeader">
                                            <asp:Label ID="Label29" runat="server" Text="Lacre"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRow">
                                            <asp:Label ID="Label31" runat="server" Text='<%# Eval("Lacre") %>'></asp:Label>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                            <!-- SEGURO DA CARGA -->
                            <div class="dtvTitulo">
                                Seguro da Carga
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label15" runat="server" Text="Responsável Seguro"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label33" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("ResponsavelSeguro")) %>'></asp:Label>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label16" runat="server" Text="Seguradora"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label34" runat="server" Text='<%# Eval("NomeSeguradora") %>'></asp:Label>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label25" runat="server" Text="Número da Apólice"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label35" runat="server" Text='<%# Eval("NumeroApolice") %>'></asp:Label>
                                </div>
                            </div>
                            <asp:Repeater runat="server" ID="rptAverbacaoSeguro" DataSource='<%# Eval("AverbacaoSeguro")%>'>
                                <ItemTemplate>
                                    <div class="dtvRow">
                                        <div class="dtvHeader">
                                            <asp:Label ID="Label29" runat="server" Text="Número da Averbação"></asp:Label>
                                        </div>
                                        <div class="dtvAlternatingRow">
                                            <asp:Label ID="Label31" runat="server" Text='<%# Eval("NumeroAverbacao") %>'></asp:Label>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                            <!-- TOTALIZADORES -->
                            <div class="dtvTitulo">
                                Totalizadores
                            </div>
                            <div class="dtvRow" style="display: none">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label17" runat="server" Text="Quantidade de CT-e"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtTotalCTe" runat="server" Enabled="false" Width="30px" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label18" runat="server" Text="Quantidade de NF-e"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtTotalNFe" runat="server" Enabled="false" Width="30px" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader" style="display: none">
                                    <asp:Label ID="Label21" runat="server" Text="Quantidade de MDF-e"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow" style="display: none">
                                    <asp:TextBox ID="txtTotalMDFe" runat="server" Enabled="false" Width="30px" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label22" runat="server" Text="Valor Total da Carga"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label37" runat="server" Text='<%# Eval("ValorCarga") %>'></asp:Label>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label23" runat="server" Text="Unidade de Médida da Carga"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label39" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("CodigoUnidade")) %>'></asp:Label>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label24" runat="server" Text="Peso Bruto Total da Carga"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:Label ID="Label38" runat="server" Text='<%# Eval("QuantidadeCarga") %>'></asp:Label>
                                </div>
                            </div>

                            <!-- INFORMAÇÕES ADICIONAIS -->
                            <div class="dtvTitulo">
                                Informações Adicionais
                            </div>
                            <div class="dtvRowInfo">
                                <div class="dtvHeaderInfo">
                                    <asp:Label ID="Label19" runat="server" Text="Info. Adic. do Fisco"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRowInfo">
                                    <asp:Label ID="Label40" runat="server" Text='<%# Eval("InformacoesAdicionaisFisco") %>'></asp:Label>
                                </div>
                            </div>
                            <div class="dtvRowInfo">
                                <div class="dtvHeaderInfo">
                                    <asp:Label ID="Label20" runat="server" Text="Info. Compl. Contribuinte"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRowInfo">
                                    <asp:Label ID="Label41" runat="server" Text='<%# Eval("InformacoesComplementares") %>'></asp:Label>
                                </div>
                            </div>
                        </ItemTemplate>
                        <InsertItemTemplate>
                            <!-- IDENTIFICAÇÃO DO MDFe -->
                            <div class="dtvTitulo">
                                Identificação do MDF-e
                            </div>
                            <div class="dtvRow" style="display: none">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label7" runat="server" Text="Número MDF-e *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">

                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label8" runat="server" Text="Chave de Acesso *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">

                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label1" runat="server" Text="Tipo do Emitente *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpTipoEmitente" runat="server" SelectedValue='<%# Bind("TipoEmitente") %>' Width="250px"
                                        DataSourceID="odsTipoEmitente" DataTextField="Translation" DataValueField="Key">
                                    </asp:DropDownList>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label2" runat="server" Text="Tipo do Trasportador"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpTipoTransportador" runat="server" SelectedValue='<%# Bind("TipoTransportador") %>'  Width="250px"
                                        DataSourceID="odsTipoTransportador" DataTextField="Translation" DataValueField="Key">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label5" runat="server" Text="Modelo *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtModelo" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                        OnLoad="txtModelo_Load" MaxLength="2" Width="30px" Text='<%# Bind("Modelo") %>' ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label6" runat="server" Text="Série *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtSerie" runat="server" MaxLength="3" Width="25px" Text='<%# Bind("Serie") %>'
                                        onkeypress="return soNumeros(event, false, true);" OnLoad="txtSerie_Load">
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvTxtSerie" runat="server" ErrorMessage="campo série não pode ser vazio."
                                        ControlToValidate="txtSerie" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label9" runat="server" Text="Tipo Emissão *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpTipoEmissao" runat="server" SelectedValue='<%# Bind("TipoEmissao") %>' Width="250px"
                                        DataSourceID="odsTipoEmissao" DataTextField="Translation" DataValueField="Key" Enabled="false">
                                    </asp:DropDownList>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label10" runat="server" Text="Modalidade de transporte *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpModal" runat="server" SelectedValue='<%# Bind("Modal") %>' Width="250px"
                                        DataSourceID="odsModal" DataTextField="Translation" DataValueField="Key" Enabled="false">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label29" runat="server" Text="UF Início *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpUfInicio" runat="server" SelectedValue='<%# Bind("UFInicio") %>' Width="250px"
                                        DataSourceID="odsUFsPercurso" DataTextField="NomeUf" DataValueField="NomeUf">
                                    </asp:DropDownList>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label30" runat="server" Text="UF Fim *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpUfFim" runat="server" SelectedValue='<%# Bind("UFFim") %>' Width="250px"
                                        DataSourceID="odsUFsPercurso" DataTextField="NomeUf" DataValueField="NomeUf">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <uc3:ctrlCidadesCarga ID="ctrlCidadesCarga" runat="server" IdManifestoEletronico='<%# Bind("IdManifestoEletronico") %>' CidadesCarga='<%# Bind("CidadesCarga") %>' />
                            <uc4:ctrlUFsPercurso ID="ctrlUFsPercurso" runat="server" IdManifestoEletronico='<%# Bind("IdManifestoEletronico") %>' UFsPercurso='<%# Bind("UFsPercurso") %>' />
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label55" runat="server" Text="Data Início Viagem"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <uc5:ctrlData ID="ctrlDataInicioViagem" runat="server" ReadOnly="ReadWrite" DataNullable='<%# Bind("DataInicioViagem") %>'
                                        ValidateEmptyText="true" ValidationGroup="c" ErrorMessage="Campo Data Início Viagem deve ser preenchido."
                                        ExibirHoras="True" />
                                </div>
                            </div>

                            <!-- IDENTIFICAÇÃO DO EMITENTE -->
                            <div class="dtvTitulo">
                                Identificação do Emitente
                            </div>
                            <uc2:ctrlParticipantes ID="ctrlParticipantes" runat="server" IdManifestoEletronico='<%# Bind("IdManifestoEletronico") %>' Participantes='<%# Bind("Participantes") %>' />

                            <!-- INFORMAÇÕES DO MODAL -->
                            <div class="dtvTitulo">
                                Informações do modal
                            </div>
                            <uc1:ctrlRodoviario ID="ctrlRodoviario" runat="server" IdManifestoEletronico='<%# Bind("IdManifestoEletronico") %>' Rodoviario='<%# Bind("Rodoviario") %>' />

                            <!-- SEGURO DA CARGA -->
                            <div class="dtvTitulo">
                                Seguro da Carga
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label15" runat="server" Text="Responsável Seguro *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpResponsavelSeguro" runat="server" SelectedValue='<%# Bind("ResponsavelSeguro") %>' Width="250px"
                                        DataSourceID="odsResponsavelSeguro" DataTextField="Translation" DataValueField="Key">
                                    </asp:DropDownList>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label16" runat="server" Text="Seguradora"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpSeguradora" runat="server" SelectedValue='<%# Bind("IdSeguradora") %>' Width="250px"
                                        DataSourceID="odsSeguradora" DataTextField="NomeSeguradora" DataValueField="IdSeguradora" AppendDataBoundItems="true">
                                        <asp:ListItem Value="0" Text="Selecione"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label25" runat="server" Text="Número da Apólice"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox id="txtNumeroApolice" runat="server" MaxLength="20" Width="150px"
                                        Text='<%# Bind("NumeroApolice") %>'></asp:TextBox>
                                </div>
                            </div>
                            <uc6:ctrlAverbacaoSeguro ID="ctrlAverbacaoSeguro" runat="server" IdManifestoEletronico='<%# Bind("IdManifestoEletronico") %>' AverbacaoSeguro='<%# Bind("AverbacaoSeguro") %>' />

                            <!-- TOTALIZADORES -->
                            <div class="dtvTitulo">
                                Totalizadores
                            </div>
                            <div class="dtvRow" style="display: none">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label17" runat="server" Text="Quantidade de CT-e"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtTotalCTe" runat="server" Enabled="false" Width="30px" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label18" runat="server" Text="Quantidade de NF-e"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtTotalNFe" runat="server" Enabled="false" Width="30px" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader" style="display: none">
                                    <asp:Label ID="Label21" runat="server" Text="Quantidade de MDF-e"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow" style="display: none">
                                    <asp:TextBox ID="txtTotalMDFe" runat="server" Enabled="false" Width="30px" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label22" runat="server" Text="Valor Total da Carga *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtValorCarga" runat="server" MaxLength="15" onclick="mascaraValor(this, 2); return false;"
                                        Width="150px" Text='<%# Bind("ValorCarga") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label23" runat="server" Text="Unid. de Médida da Carga *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpCodigoUnidade" runat="server" SelectedValue='<%# Bind("CodigoUnidade") %>'  Width="250px"
                                        DataSourceID="odsCodigoUnidade" DataTextField="Translation" DataValueField="Key">
                                    </asp:DropDownList>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label24" runat="server" Text="Peso Bruto Total da Carga *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtQuantidadeCarga" runat="server" MaxLength="15" onclick="mascaraValor(this, 4); return false;"
                                        Width="150px" Text='<%# Bind("QuantidadeCarga") %>'></asp:TextBox>
                                </div>
                            </div>

                            <!-- INFORMAÇÕES ADICIONAIS -->
                            <div class="dtvTitulo">
                                Informações Adicionais
                            </div>
                            <div class="dtvRowInfo">
                                <div class="dtvHeaderInfo">
                                    <asp:Label ID="Label19" runat="server" Text="Info. Adic. do Fisco"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRowInfo">
                                    <asp:TextBox ID="txtInformacoesAdicionaisFisco" runat="server" TextMode="MultiLine" Rows="3" MaxLength="50" Width="853px"
                                        Text='<%# Bind("InformacoesAdicionaisFisco") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRowInfo">
                                <div class="dtvHeaderInfo">
                                    <asp:Label ID="Label20" runat="server" Text="Info. Compl. Contribuinte"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRowInfo">
                                    <asp:TextBox ID="txtInformacoesComplementares" runat="server" TextMode="MultiLine" Rows="3" MaxLength="50" Width="853px"
                                        Text='<%# Bind("InformacoesComplementares") %>'></asp:TextBox>
                                </div>
                            </div>
                        </InsertItemTemplate>
                        <EditItemTemplate>
                            <!-- IDENTIFICAÇÃO DO MDFe -->
                            <div class="dtvTitulo">
                                Identificação do MDF-e
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label7" runat="server" Text="Número MDF-e *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                                    <asp:HiddenField ID="hdfDataCad" runat="server" Value='<%# Bind("DataCad") %>' />
                                    <asp:HiddenField ID="hdfUsucad" runat="server" Value='<%# Bind("Usucad") %>' />
                                    <asp:HiddenField ID="hdfCodigoAleatorio" runat="server" Value='<%# Bind("CodigoAleatorio") %>' />
                                    <asp:HiddenField ID="hdfDataEmissao" runat="server" Value='<%# Bind("DataEmissao") %>' />
                                    <asp:TextBox ID="txtNumeroManifestoEletronico" runat="server" MaxLength="9" Width="150px" Enabled="false"
                                        Text='<%# Bind("NumeroManifestoEletronico") %>' onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label8" runat="server" Text="Chave de Acesso *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtChaveAcesso" runat="server" MaxLength="44" Width="300px" Enabled="false"
                                        Text='<%# Eval("ChaveAcesso") %>' onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label1" runat="server" Text="Tipo do Emitente *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:HiddenField ID="hdfTipoEmitente" runat="server" Value='<%# Eval("TipoEmitente") %>' />
                                    <asp:DropDownList ID="drpTipoEmitente" runat="server" SelectedValue='<%# Bind("TipoEmitente") %>' Width="250px"
                                        DataSourceID="odsTipoEmitente" DataTextField="Translation" DataValueField="Key">
                                    </asp:DropDownList>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label2" runat="server" Text="Tipo do Trasportador"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpTipoTransportador" runat="server" SelectedValue='<%# Bind("TipoTransportador") %>'  Width="250px"
                                        DataSourceID="odsTipoTransportador" DataTextField="Translation" DataValueField="Key">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label5" runat="server" Text="Modelo *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtModelo" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                        OnLoad="txtModelo_Load" MaxLength="2" Width="30px" Text='<%# Bind("Modelo") %>' ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label6" runat="server" Text="Série *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtSerie" runat="server" MaxLength="3" Width="25px" Text='<%# Bind("Serie") %>'
                                        onkeypress="return soNumeros(event, false, true);" OnLoad="txtSerie_Load">
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvTxtSerie" runat="server" ErrorMessage="campo série não pode ser vazio."
                                        ControlToValidate="txtSerie" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label9" runat="server" Text="Tipo Emissão *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpTipoEmissao" runat="server" SelectedValue='<%# Bind("TipoEmissao") %>' Width="250px"
                                        DataSourceID="odsTipoEmissao" DataTextField="Translation" DataValueField="Key" Enabled="false">
                                    </asp:DropDownList>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label10" runat="server" Text="Modalidade de transporte *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpModal" runat="server" SelectedValue='<%# Bind("Modal") %>' Width="250px"
                                        DataSourceID="odsModal" DataTextField="Translation" DataValueField="Key" Enabled="false">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label29" runat="server" Text="UF Início *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpUfInicio" runat="server" SelectedValue='<%# Bind("UFInicio") %>' Width="250px"
                                        DataSourceID="odsUFsPercurso" DataTextField="NomeUf" DataValueField="NomeUf">
                                    </asp:DropDownList>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label30" runat="server" Text="UF Fim *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpUfFim" runat="server" SelectedValue='<%# Bind("UFFim") %>' Width="250px"
                                        DataSourceID="odsUFsPercurso" DataTextField="NomeUf" DataValueField="NomeUf">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <uc3:ctrlCidadesCarga ID="ctrlCidadesCarga" runat="server" IdManifestoEletronico='<%# Eval("IdManifestoEletronico") %>' CidadesCarga='<%# Bind("CidadesCarga") %>' />
                            <uc4:ctrlUFsPercurso ID="ctrlUFsPercurso" runat="server" IdManifestoEletronico='<%# Eval("IdManifestoEletronico") %>' UFsPercurso='<%# Bind("UFsPercurso") %>' />
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label55" runat="server" Text="Data Início Viagem"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <uc5:ctrlData ID="ctrlDataInicioViagem" runat="server" ReadOnly="ReadWrite" DataNullable='<%# Bind("DataInicioViagem") %>'
                                        ValidateEmptyText="true" ValidationGroup="c" ErrorMessage="Campo Data Início Viagem deve ser preenchido."
                                        ExibirHoras="True" />
                                </div>
                            </div>

                            <!-- IDENTIFICAÇÃO DO EMITENTE -->
                            <div class="dtvTitulo">
                                Identificação do Emitente
                            </div>
                            <uc2:ctrlParticipantes ID="ctrlParticipantes" runat="server" IdManifestoEletronico='<%# Eval("IdManifestoEletronico") %>' Participantes='<%# Bind("Participantes") %>' />

                            <!-- INFORMAÇÕES DO MODAL -->
                            <div class="dtvTitulo">
                                Informações do modal
                            </div>
                            <uc1:ctrlRodoviario ID="ctrlRodoviario" runat="server" IdManifestoEletronico='<%# Eval("IdManifestoEletronico") %>' Rodoviario='<%# Bind("Rodoviario") %>' />

                            <!-- SEGURO DA CARGA -->
                            <div class="dtvTitulo">
                                Seguro da Carga
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label15" runat="server" Text="Responsável Seguro *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpResponsavelSeguro" runat="server" SelectedValue='<%# Bind("ResponsavelSeguro") %>' Width="250px"
                                        DataSourceID="odsResponsavelSeguro" DataTextField="Translation" DataValueField="Key">
                                    </asp:DropDownList>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label16" runat="server" Text="Seguradora"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpSeguradora" runat="server" SelectedValue='<%# Bind("IdSeguradora") %>' Width="250px"
                                        DataSourceID="odsSeguradora" DataTextField="NomeSeguradora" DataValueField="IdSeguradora" AppendDataBoundItems="true">
                                        <asp:ListItem Value="0" Text="Selecione"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label25" runat="server" Text="Número da Apólice"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox id="txtNumeroApolice" runat="server" MaxLength="20" Width="150px"
                                        Text='<%# Bind("NumeroApolice") %>'></asp:TextBox>
                                </div>
                            </div>
                            <uc6:ctrlAverbacaoSeguro ID="ctrlAverbacaoSeguro" runat="server" IdManifestoEletronico='<%# Eval("IdManifestoEletronico") %>' AverbacaoSeguro='<%# Bind("AverbacaoSeguro") %>' />

                            <!-- TOTALIZADORES -->
                            <div class="dtvTitulo">
                                Totalizadores
                            </div>
                            <div class="dtvRow" style="display: none">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label17" runat="server" Text="Quantidade de CT-e"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtTotalCTe" runat="server" Enabled="false" Width="30px" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label18" runat="server" Text="Quantidade de NF-e"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtTotalNFe" runat="server" Enabled="false" Width="30px" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader" style="display: none">
                                    <asp:Label ID="Label21" runat="server" Text="Quantidade de MDF-e"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow" style="display: none">
                                    <asp:TextBox ID="txtTotalMDFe" runat="server" Enabled="false" Width="30px" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label22" runat="server" Text="Valor Total da Carga *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtValorCarga" runat="server" MaxLength="15" onclick="mascaraValor(this, 2); return false;"
                                        Width="150px" Text='<%# Bind("ValorCarga") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRow">
                                <div class="dtvHeader">
                                    <asp:Label ID="Label23" runat="server" Text="Unid. de Médida da Carga *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:DropDownList ID="drpCodigoUnidade" runat="server" SelectedValue='<%# Bind("CodigoUnidade") %>'  Width="250px"
                                        DataSourceID="odsCodigoUnidade" DataTextField="Translation" DataValueField="Key">
                                    </asp:DropDownList>
                                </div>
                                <div class="dtvHeader">
                                    <asp:Label ID="Label24" runat="server" Text="Peso Bruto Total da Carga *"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRow">
                                    <asp:TextBox ID="txtQuantidadeCarga" runat="server" MaxLength="15" onclick="mascaraValor(this, 4); return false;"
                                        Width="150px" Text='<%# Bind("QuantidadeCarga") %>'></asp:TextBox>
                                </div>
                            </div>

                            <!-- INFORMAÇÕES ADICIONAIS -->
                            <div class="dtvTitulo">
                                Informações Adicionais
                            </div>
                            <div class="dtvRowInfo">
                                <div class="dtvHeaderInfo">
                                    <asp:Label ID="Label19" runat="server" Text="Info. Adic. do Fisco"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRowInfo">
                                    <asp:TextBox ID="txtInformacoesAdicionaisFisco" runat="server" TextMode="MultiLine" Rows="3" MaxLength="50" Width="853px"
                                        Text='<%# Bind("InformacoesAdicionaisFisco") %>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="dtvRowInfo">
                                <div class="dtvHeaderInfo">
                                    <asp:Label ID="Label20" runat="server" Text="Info. Compl. Contribuinte"></asp:Label>
                                </div>
                                <div class="dtvAlternatingRowInfo">
                                    <asp:TextBox ID="txtInformacoesComplementares" runat="server" TextMode="MultiLine" Rows="3" MaxLength="50" Width="853px"
                                        Text='<%# Bind("InformacoesComplementares") %>'></asp:TextBox>
                                </div>
                            </div>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="false">
                        <ItemTemplate>
                            <asp:Button ID="btnEditar" CausesValidation="false" runat="server" Text="Editar" OnClick="btnEditar_Click" />
                            <asp:Button ID="btnEmitir" CausesValidation="false" runat="server" Text="Emitir" Visible="false"
                                OnClick="btnEmitir_Click" OnLoad="btnEmitir_Load" />
                            <asp:Button ID="btnImprimirContingencia" CausesValidation="false" runat="server" Text="Imprimir Contingência" Visible="false"
                                OnClientClick="if (!confirm('Após imprimir não será possível realizar alteração. Deseja imprimir esse MDF-e?')) return false"
                                OnClick="btnImprimirContingencia_Click" OnLoad="btnImprimirContingencia_Load" />
                            <asp:Button ID="btnPreVisualizar" CausesValidation="false" runat="server" Text="Pré-Visualizar" Visible="false"
                                OnClick="btnPreVisualizar_Click" OnLoad="btnPreVisualizar_Load" />
                            <asp:Button ID="btnVoltar" CausesValidation="false" runat="server" Text="Voltar" OnClick="btnVoltar_Click" />
                        </ItemTemplate>
                        <InsertItemTemplate>
                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="return onInsert();" />
                            <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" Text="Cancelar" OnClick="btnVoltar_Click" />
                        </InsertItemTemplate>
                        <EditItemTemplate>
                            <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar" OnClientClick="return onInsert();" />
                            <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
                        </EditItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
        </div>
        <div class="dtv">
            <div id="cidadeDescarga" runat="server">
                <asp:GridView ID="grdCidadeDescarga" runat="server" ShowFooter="true" EnableViewState="false"
                    GridLines="None" AllowPaging="true" AllowSorting="true" AutoGenerateColumns="false" CssClass="gridStyle"
                    DataSourceID="odsCidadeDescarga" DataKeyNames="IdCidadeDescarga" EmptyDataText="Nenhuma cidade descarga adicionada"
                    OnDataBound="grdCidadeDescarga_DataBound" OnRowCommand="grdCidadeDescarga_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir"
                                    OnClientClick="if (!confirm('Deseja excluir essa cidade?')) return false"
                                    CommandName="ExcluirCidadeDescarga" CommandArgument='<%# Eval("IdCidadeDescarga") %>' />
                                <asp:ImageButton ID="imbExibirDocumentos" runat="server" ImageUrl="~/Images/mais.gif" ToolTip="Exibir Documentos"
                                    OnClientClick='<%# "exibirDocumentos(this, " + Eval("IdCidadeDescarga") + "); return false" %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="false" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cidade Descarga *">
                            <ItemTemplate>
                                <asp:Label ID="Label30" runat="server" Text='<%# Eval("NomeCidade") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label Text="Inserir Cidade de Descarga" Font-Bold="true" Font-Size="Small" runat="server" />
                                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx');window.close(); return false;" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <!-- NFes Vinculadas -->
                                </td> </tr>
                                <tr id="NFesViculadas_<%# Eval("IdCidadeDescarga") %>" style="display: none;" class="<%= GetAlternateClass() %>">
                                    <asp:HiddenField ID="hdfIdCidadeDescargaNFe" runat="server" Value='<%# Eval("IdCidadeDescarga") %>' />
                                    <td colspan="13">
                                        <asp:GridView ID="grdNFeCidadeDescarga" runat="server" AutoGenerateColumns="False" GridLines="None" EnableViewState="false"
                                            Width="100%" class="pos" ShowFooter="True" CellPadding="0" OnDataBound="grdNFeCidadeDescarga_DataBound"
                                            DataSourceID="odsNFeCidadeDescargaMDFe" DataKeyNames="IdCidadeDescargaMdfe" EmptyDataText="Nenhuma NFe referênciada.">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                                            ToolTip="Excluir" OnClientClick="if (!confirm('Deseja excluir essa NFe?')) return false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="NFe">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label31" runat="server" Text='<%# Eval("NumeroNFe") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtNumNfIns" runat="server" Columns="6" onblur="buscarNf(this.value);"
                                                            onkeypress="return soNumeros(event, true, true);" Enabled="false"></asp:TextBox>
                                                        <asp:HiddenField runat="server" ID="hdfIdNf" />
                                                        <asp:ImageButton ID="imbSelNotaFiscal" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick='<%# "abrirBuscaNf(this); return false;" %>' />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Chave de Acesso">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblChaveAcesso" runat="server" Text='<%# Eval("ChaveAcesso") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtChaveAcesso" runat="server" Columns="45" MaxLength="44" onkeypress="return soNumeros(event, true, true);" onblur="bloquearSelecaoNota();" ></asp:TextBox>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Numero Documento Fsda">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFsda" runat="server" Text='<%# Eval("NumeroDocumentoFsda") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtFsda" runat="server" Columns="10" onkeypress="return soNumeros(event, true, true);" onblur="bloquearSelecaoNota();" ></asp:TextBox>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Modelo" SortExpression="Modelo">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblModelo" runat="server" Text='<%# Eval("Modelo") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tipo Doc." SortExpression="TipoDocumento">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTipoDocumento" runat="server" Text='<%# Eval("TipoDocumentoString") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Emitente" SortExpression="NomeEmitente">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmitente" runat="server" Text='<%# Eval("NomeEmitente") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Data Emissão" SortExpression="DataEmissao">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDataEmissao" runat="server" Text='<%# Eval("DataEmissao") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <FooterTemplate>
                                                        <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="SalvarNfReferenciada();" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <colo:VirtualObjectDataSource ID="odsNFeCidadeDescargaMDFe" runat="server" Culture="pt-BR" EnablePaging="true"
                                            SortParameterName="sortExpression" MaximumRowsParameterName="pageSize" StartRowIndexParameterName="startRow"
                                            TypeName="Glass.Data.DAL.NFeCidadeDescargaMDFeDAO" DataObjectTypeName="Glass.Data.Model.NFeCidadeDescargaMDFe"
                                            SelectMethod="GetList" SelectCountMethod="GetCount" DeleteMethod="Delete">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="hdfIdCidadeDescargaNFe" Name="idCidadeDescarga" PropertyName="Value" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>
                                <!-- CTes Vinculados -->
                                <tr id="CTesViculados_<%# Eval("IdCidadeDescarga") %>" style="display: none;" class="<%= GetAlternateClass() %>">
                                    <asp:HiddenField ID="hdfIdCidadeDescargaCTe" runat="server" Value='<%# Eval("IdCidadeDescarga") %>' />
                                    <td colspan="13">
                                        <asp:GridView ID="grdCTeCidadeDescarga" runat="server" AutoGenerateColumns="False" GridLines="None" EnableViewState="false"
                                            Width="100%" class="pos" ShowFooter="True" CellPadding="0" OnDataBound="grdCTeCidadeDescarga_DataBound"
                                            DataSourceID="odsCTeCidadeDescargaMDFe" DataKeyNames="IdCidadeDescargaMdfe" EmptyDataText="Nenhum CTe referênciado.">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                                            ToolTip="Excluir" OnClientClick="if (!confirm('Deseja excluir esse CTe?')) return false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CTe">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label31" runat="server" Text='<%# Eval("NumeroCTe") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtNumCTeIns" runat="server" Columns="6" onblur="buscarCTe(this.value);"
                                                            onkeypress="return soNumeros(event, true, true);" Enabled="false"></asp:TextBox>
                                                        <asp:HiddenField runat="server" ID="hdfIdCTe" />
                                                        <asp:ImageButton ID="imbSelCTe" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick='<%# "abrirBuscaCTe(this); return false;" %>' />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Chave de Acesso">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblChaveAcessoCte" runat="server" Text='<%# Eval("ChaveAcesso") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtChaveAcessoCte" runat="server" Columns="45" MaxLength="44" onkeypress="return soNumeros(event, true, true);" onblur="bloquearSelecaoCte();" ></asp:TextBox>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Numero Documento Fsda">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFsdaCTe" runat="server" Text='<%# Eval("NumeroDocumentoFsda") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtFsdaCTe" runat="server" Columns="10" onkeypress="return soNumeros(event, true, true);" onblur="bloquearSelecaoCte();" ></asp:TextBox>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Modelo" SortExpression="Modelo">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblModelo" runat="server" Text='<%# Eval("Modelo") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tipo Doc." SortExpression="TipoDocumentoCTe">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTipoDocumento" runat="server" Text='<%# Eval("TipoDocumentoString") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Emitente" SortExpression="NomeEmitente">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmitente" runat="server" Text='<%# Eval("NomeEmitente") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Data Emissão" SortExpression="DataEmissao">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDataEmissao" runat="server" Text='<%# Eval("DataEmissao") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <FooterTemplate>
                                                        <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="SalvarCTeReferenciada(this);" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <colo:VirtualObjectDataSource ID="odsCTeCidadeDescargaMDFe" runat="server" Culture="pt-BR" EnablePaging="true"
                                            SortParameterName="sortExpression" MaximumRowsParameterName="pageSize" StartRowIndexParameterName="startRow"
                                            TypeName="Glass.Data.DAL.CTeCidadeDescargaMDFeDAO" DataObjectTypeName="Glass.Data.Model.CTeCidadeDescargaMDFe"
                                            SelectMethod="GetList" SelectCountMethod="GetCount" DeleteMethod="Delete">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="hdfIdCidadeDescargaCTe" Name="idCidadeDescarga" PropertyName="Value" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource ID="odsCidade" runat="server" Culture="pt-BR"
                    TypeName="Glass.Data.DAL.CidadeDAO" DataObjectTypeName="Glass.Data.Model.Cidade"
                    SelectMethod="GetList">
                </colo:VirtualObjectDataSource>
            </div>
        </div>
        <div>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsManifestoEletronico" runat="server"
                TypeName="Glass.Data.DAL.ManifestoEletronicoDAO" DataObjectTypeName="Glass.Data.Model.ManifestoEletronico"
                SelectMethod="ObterManifestoEletronicoPeloId"
                InsertMethod="InsertComTransacao"
                UpdateMethod="UpdateComTransacao"
                OnInserted="odsManifestoEletronico_Inserted"
                OnUpdated="odsManifestoEletronico_Updated">
                <SelectParameters>
                    <asp:QueryStringParameter Name="idManifestoEletronico" QueryStringField="IdMDFe" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCidadeDescarga" runat="server" EnablePaging="true"
                SortParameterName="sortExpression" MaximumRowsParameterName="pageSize" StartRowIndexParameterName="startRow"
                TypeName="Glass.Data.DAL.CidadeDescargaMDFeDAO" DataObjectTypeName="Glass.Data.Model.CidadeDescargaMDFe"
                SelectMethod="GetList" SelectCountMethod="GetCount">
                <SelectParameters>
                    <asp:QueryStringParameter Name="idManifestoEletronico" QueryStringField="IdMDFe" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoEmitente" runat="server"
                TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
                <SelectParameters>
                    <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoEmitenteEnum, Glass.Data" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoTransportador" runat="server"
                TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
                <SelectParameters>
                    <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoTransportadorEnum, Glass.Data" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoEmissao" runat="server"
                TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
                <SelectParameters>
                    <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoEmissao, Glass.Data" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsModal" runat="server"
                TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
                <SelectParameters>
                    <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.ModalEnum, Glass.Data" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <asp:ObjectDataSource ID="odsUFsPercurso" runat="server"
                TypeName="Glass.Data.DAL.CidadeDAO" DataObjectTypeName="Glass.Data.Model.Cidade"
                SelectMethod="ObterUF">
            </asp:ObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsResponsavelSeguro" runat="server"
                TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
                <SelectParameters>
                    <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.ResponsavelEnum, Glass.Data" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
            <asp:ObjectDataSource ID="odsSeguradora" runat="server"
                TypeName="Glass.Data.DAL.CTe.SeguradoraDAO" DataObjectTypeName="Glass.Data.Model.Cte.Seguradora"
                SelectMethod="ObterSeguradoras">
            </asp:ObjectDataSource>
            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCodigoUnidade" runat="server"
                TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
                <SelectParameters>
                    <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.CodigoUnidadeEnum, Glass.Data" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>
        </div>
    </div>

</asp:Content>
