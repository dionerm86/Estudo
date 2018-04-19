<%@ Page Title="Notas Fiscais" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstNotaFiscal.aspx.cs" Inherits="Glass.UI.Web.Listas.LstNotaFiscal" %>

<%@ Register Src="../Controls/ctrlTextoTooltip.ascx" TagName="ctrlTextoTooltip" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc4" %>
<%@ Register src="../Controls/ctrlBoleto.ascx" tagname="ctrlBoleto" tagprefix="uc5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        function openMotivoCanc(idNf) {
            openWindow(150, 400, "../Utils/SetMotivoCancNFe.aspx?idNf=" + idNf);
            return false;
        }

        function openRptDanfe(idNf) {
            openWindow(600, 800, "../Relatorios/NFe/RelBase.aspx?rel=Danfe&idNf=" + idNf);
            return false;
        }
    
        function openRptDanfes() {        
            var numeroNfe = FindControl("txtNumNf", "input").value == "" ? "0" : FindControl("txtNumNf", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value == "" ? "0" : FindControl("txtNumPedido", "input").value;
            var modelo = FindControl("txtModelo", "input").value;
            var idLoja = FindControl("drpLoja", "select").value == "" ? "0" : FindControl("drpLoja", "select").value;
            var idCliente = FindControl("txtIdCliente", "input").value == "" ? "0" : FindControl("txtIdCliente", "input").value;
            var nomeCliente = FindControl("txtNomeCliente", "input").value;
            var tipoFiscal = FindControl("drpTipoFiscal", "select").value == "" ? "0" : FindControl("drpTipoFiscal", "select").value;
            var idFornec = FindControl("txtIdFornec", "input").value == "" ? "0" : FindControl("txtIdFornec", "input").value;
            var nomeFornec = FindControl("txtNomeFornecedor", "input").value;
            var codRota = FindControl("txtRota", "input").value;
            var situacao = FindControl("cboSituacao", "select").itens();
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idsCfop = FindControl("drpCfop", "select").itens();
            var dataEntSaiIni = FindControl("ctrlDataEntSaiIni_txtData", "input").value;
            var dataEntSaiFim = FindControl("ctrlDataEntSaiFim_txtData", "input").value;
            var formaPagto = FindControl("drpFormaPagto", "select").value == "" ? "0" : FindControl("drpFormaPagto", "select").value;
            var idsFormaPagtoNotaFiscal = FindControl("cbdFormaPagtoNotaFiscal", "select").itens() == "" ? "0" : FindControl("cbdFormaPagtoNotaFiscal", "select").itens();
            var tipoNf = FindControl("drpTipoNota", "select").value == "" ? "0" : FindControl("drpTipoNota", "select").value;
            var finalidade = FindControl("drpFinalidade", "select").value == "" ? "0" : FindControl("drpFinalidade", "select").value;
            var formaEmissao = FindControl("drpFormaEmissao", "select").value == "" ? "0" : FindControl("drpFormaEmissao", "select").value;
            var infCompl = FindControl("txtInfCompl", "input").value;
            var ordenar = FindControl("drpOrdenar", "select").value;
            var agrupar = FindControl("drpAgrupar", "select").value;
            var tiposCfop = FindControl("cbxdrpTipoCFOP", "select").itens();
            var codInternoProd = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescr", "input").value;
            var valorInicial = FindControl("txtValorInicial", "input").value;
            var valorFinal = FindControl("txtValorFinal", "input").value;
         
            var retorno = MetodosAjax.ObterIdNf(numeroNfe, idPedido, modelo, idLoja, idCliente, nomeCliente, tipoFiscal, idFornec,
            nomeFornec, codRota, "0", situacao, dataIni, dataFim, idsCfop, tiposCfop, dataEntSaiIni, dataEntSaiFim,
            formaPagto, idsFormaPagtoNotaFiscal, tipoNf, finalidade, formaEmissao, infCompl, codInternoProd, descrProd,
            valorInicial, valorFinal).value;
        
            //var campoIdNf = document.getElementById("campoIdNf");
            //
            //if (campoIdNf == null) {
            //    campoIdNf = document.createElement("input");
            //    campoIdNf.id = "campoIdNf";
            //    campoIdNf.name = "idNf";
            //    document.formPost.appendChild(campoIdNf);
            //}
            //
            //campoIdNf.value = retorno;
            //
            //document.formPost.action = "../Relatorios/NFe/RelBase.aspx?rel=Danfe";
            //document.formPost.submit();
        
            openWindow(600, 800, "../Relatorios/NFe/RelBase.aspx?rel=Danfes&idNf=" + retorno);
            return false;
        }
    
        function openRptTerc(idNf) {
            openWindow(600, 800, "../Relatorios/NFe/RelBase.aspx?rel=NfTerceiros&idNf=" + idNf);
            return false;
        }
    
        function salvarNota(idNf)
        {
            redirectUrl('<%= this.ResolveClientUrl("../Handlers/NotaXml.ashx") %>?idNf=' + idNf);
        }
    
        function openReport(produtos, exportarExcel)
        {
            var numeroNfe = FindControl("txtNumNf", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var modelo = FindControl("txtModelo", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idCliente = FindControl("txtIdCliente", "input").value;
            var nomeCliente = FindControl("txtNomeCliente", "input").value;
            var tipoFiscal = FindControl("drpTipoFiscal", "select").value;
            var idFornec = FindControl("txtIdFornec", "input").value;
            var nomeFornec = FindControl("txtNomeFornecedor", "input").value;
            var codRota = FindControl("txtRota", "input").value;
            var situacao = FindControl("cboSituacao", "select").itens();
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idsCfop = FindControl("drpCfop", "select").itens();
            var dataEntSaiIni = FindControl("ctrlDataEntSaiIni_txtData", "input").value;
            var dataEntSaiFim = FindControl("ctrlDataEntSaiFim_txtData", "input").value;
            var formaPagto = FindControl("drpFormaPagto", "select").value;
            var idsFormaPagtoNotaFiscal = FindControl("cbdFormaPagtoNotaFiscal", "select").itens();
            var tipoNf = FindControl("drpTipoNota", "select").value;
            var finalidade = FindControl("drpFinalidade", "select").value;
            var formaEmissao = FindControl("drpFormaEmissao", "select").value;
            var infCompl = FindControl("txtInfCompl", "input").value;
            var ordenar = FindControl("drpOrdenar", "select").value;
            var agrupar = FindControl("drpAgrupar", "select").value;
            var tiposCfop = FindControl("cbxdrpTipoCFOP", "select").itens();
            var codInternoProd = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescr", "input").value;
            var valorInicial = FindControl("txtValorInicial", "input").value;
            var valorFinal = FindControl("txtValorFinal", "input").value;
            var lote = FindControl("txtLote", "input").value;
        
            numeroNfe = numeroNfe == "" ? "0" : numeroNfe;
            idPedido = idPedido == "" ? "0" : idPedido;
            idCliente = idCliente == "" ? "0" : idCliente;
            idFornec = idFornec == "" ? "0" : idFornec;
        
            var fiscal = <%= (!IsAutorizadaFinalizada()).ToString().ToLower() %> ? "Fiscal" : 
                "FiscalAutFin" + (produtos ? "Prod" : "");
        
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=" + fiscal + "&numeroNfe=" + numeroNfe + "&idPedido=" + idPedido + "&modelo=" + modelo +
                "&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente + "&tipoFiscal=" + tipoFiscal + "&idFornec=" + idFornec +
                "&nomeFornec=" + nomeFornec + "&idLoja=" + idLoja + "&tipoDocumento=0&situacao=" + situacao + "&dataIni=" + dataIni +
                "&dataFim=" + dataFim + "&idsCfop=" + idsCfop + "&dataEntSaiIni=" + dataEntSaiIni + "&dataEntSaiFim=" + dataEntSaiFim +
                "&tipoNf=" + tipoNf + "&finalidade=" + finalidade + "&formaEmissao=" + formaEmissao + "&infCompl=" + infCompl +
                "&codRota=" + codRota + "&formaPagto=" + formaPagto + "&idsFormaPagtoNotaFiscal=" + idsFormaPagtoNotaFiscal + "&ordenar=" + ordenar +
                "&agrupar=" + agrupar + "&tiposCfop=" + tiposCfop + "&exportarExcel=" + exportarExcel +
                "&codInternoProd=" + codInternoProd + "&descrProd=" + descrProd + "&valorInicial=" + valorInicial +
                "&valorFinal=" + valorFinal + "&lote=" + lote);
        }
    
        function openLoteNotas()
        {
            var numeroNfe = FindControl("txtNumNf", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var modelo = FindControl("txtModelo", "input").value;
            var idCliente = FindControl("txtIdCliente", "input").value;
            var nomeCliente = FindControl("txtNomeCliente", "input").value;
            var tipoFiscal = FindControl("drpTipoFiscal", "select").value;
            var idFornec = FindControl("txtIdFornec", "input").value;
            var nomeFornec = FindControl("txtNomeFornecedor", "input").value;
            var codRota = FindControl("txtRota", "input").value;
            var situacao = FindControl("cboSituacao", "select").itens();
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idsCfop = FindControl("drpCfop", "select").itens();
            var dataEntSaiIni = FindControl("ctrlDataEntSaiIni_txtData", "input").value;
            var dataEntSaiFim = FindControl("ctrlDataEntSaiFim_txtData", "input").value;
            var formaPagto = FindControl("drpFormaPagto", "select").value;
            var idsFormaPagtoNotaFiscal = FindControl("cbdFormaPagtoNotaFiscal", "select").itens();
            var tipoNf = FindControl("drpTipoNota", "select").value;
            var finalidade = FindControl("drpFinalidade", "select").value;
            var formaEmissao = FindControl("drpFormaEmissao", "select").value;
            var infCompl = FindControl("txtInfCompl", "input").value;
            var ordenar = FindControl("drpOrdenar", "select").value;
            var agrupar = FindControl("drpAgrupar", "select").value;
            var tiposCfop = FindControl("cbxdrpTipoCFOP", "select").itens();
            var codInternoProd = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescr", "input").value;
            var valorInicial = FindControl("txtValorInicial", "input").value;
            var valorFinal = FindControl("txtValorFinal", "input").value;
        
            numeroNfe = numeroNfe == "" ? "0" : numeroNfe;
            idPedido = idPedido == "" ? "0" : idPedido;
            idCliente = idCliente == "" ? "0" : idCliente;
            idFornec = idFornec == "" ? "0" : idFornec;
         
            redirectUrl('<%= this.ResolveClientUrl("../Handlers/NotaXmlLote.ashx") %>?numeroNfe=' + numeroNfe 
                + '&idPedido=' + idPedido 
                + '&modelo=' + modelo 
                + "&idCliente=" + idCliente 
                + "&nomeCliente=" + nomeCliente 
                + "&tipoFiscal=" + tipoFiscal
                + "&idFornec=" + idFornec 
                + "&nomeFornec=" + nomeFornec 
                + "&idLoja=" + idLoja 
                + "&tipoDocumento=0&situacao=" + situacao 
                + "&dataIni=" + dataIni 
                + "&dataFim=" + dataFim 
                + "&idsCfop=" + idsCfop 
                + "&dataEntSaiIni=" + dataEntSaiIni 
                + "&dataEntSaiFim=" + dataEntSaiFim 
                + "&tipoNf=" + tipoNf 
                + "&finalidade=" + finalidade 
                + "&formaEmissao=" + formaEmissao 
                + "&infCompl=" + infCompl 
                + "&codRota=" + codRota 
                + "&formaPagto=" + formaPagto 
                + "&idsFormaPagtoNotaFiscal=" + idsFormaPagtoNotaFiscal 
                + "&ordenar=" + ordenar 
                + "&agrupar=" + agrupar 
                + "&tiposCfop=" + tiposCfop
                + "&codInternoProd=" + codInternoProd
                + "&descrProd=" + descrProd,
                + "&valorInicial=" + valorInicial,
                + "&valorFinal=" + valorFinal);
        }
    
        function openRota() {
            if (FindControl("txtRota", "input").value != "")
                return true;

            openWindow(500, 700, "../Utils/SelRota.aspx");
            return false;
        }

        function setRota(codInterno) {
            FindControl("txtRota", "input").value = codInterno;
        }
    
        function loadProduto(codInterno)
        {
            if (codInterno.value == "")
                return false;

            try
            {
                var retorno = MetodosAjax.GetProd(codInterno.value).value.split(';');
            
                if (retorno[0] == "Erro")
                {
                    alert(retorno[1]);
                    codInterno.value = "";
                    return false;
                }
            
                FindControl("txtDescr", "input").value = retorno[2];
            }
            catch(err)
            {
                alert(err.value);
            }
        }
    
        function abrirCartaCorrecao(idNf) {
            openWindow(600, 800, "../Cadastros/CadCartaCorrecao.aspx?popup=true&idNf=" + idNf);
            return false;
        }
    
        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCliente", "input").value = "";
                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
        }
    
        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNomeFornecedor", "input").value = "";
                return false;
            }

            FindControl("txtNomeFornecedor", "input").value = retorno[1];
        }

        function openFornec() {
            if (FindControl("txtIdFornecedor", "input").value != "")
                return true;

            openWindow(500, 700, "../Utils/SelFornec.aspx");

            return false;
        }

        function ajuste(idNf) {
            redirectUrl("LstAjusteApuracaoIdentificacaoDocFiscal.aspx?idNf=" + idNf);
        }

        function exibirCentroCusto(idNf) {

            openWindow(365, 700, '../Utils/SelCentroCusto.aspx?idNf=' + idNf);
            return false;
        }

        function anexarXMLTer(idNfTer){
            openWindow(600, 800, "../Utils/AnexarXMLNFeEntradaTerceiros.aspx?idNfTer=" + idNfTer);
        }

        function baixarXMLTer(idNfTer){
            redirectUrl('<%= this.ResolveClientUrl("../Handlers/NFeEntradaTerceirosXML.ashx") %>?idNfTer=' + idNfTer);
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Num. NF" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumNf" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label21" runat="server" Text="Modelo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtModelo" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="25px" MaxLength="3" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton13" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc4:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true"
                            OnSelectedIndexChanged="drpLoja_SelectedIndexChanged"/>
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cboSituacao" runat="server" CheckAll="False" Title="Selecione a situação"
                                Width="188px">
                                <asp:ListItem Value="1">Aberta</asp:ListItem>
                                <asp:ListItem Value="2">Autorizada</asp:ListItem>
                                <asp:ListItem Value="3">Não emitida</asp:ListItem>
                                <asp:ListItem Value="4">Cancelada</asp:ListItem>
                                <asp:ListItem Value="5">Inutilizada</asp:ListItem>
                                <asp:ListItem Value="6">Denegada</asp:ListItem>
                                <asp:ListItem Value="7">Processo de emissão</asp:ListItem>
                                <asp:ListItem Value="8">Processo de cancelamento</asp:ListItem>
                                <asp:ListItem Value="9">Processo de inutilização</asp:ListItem>
                                <asp:ListItem Value="10">Falha ao emitir</asp:ListItem>
                                <asp:ListItem Value="11">Falha ao cancelar</asp:ListItem>
                                <asp:ListItem Value="12">Falha ao inutilizar</asp:ListItem>
                                <asp:ListItem Value="13">Finalizada</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="CFOP" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpCfop" runat="server" DataSourceID="odsCfop" DataTextField="CodInterno"
                                DataValueField="IdCfop">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:Label ID="Label15" runat="server" Text="Tipo CFOP" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbxdrpTipoCFOP" runat="server" DataSourceID="odsTipoCfop"
                                DataTextField="Descricao" DataValueField="IdTipoCfop" Title="Selecione os tipos de CFOP" Width="200px">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Tipo NF" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoNota" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Entrada</asp:ListItem>
                                <asp:ListItem Value="2">Saída</asp:ListItem>
                                <asp:ListItem Value="3">Entrada (terceiros)</asp:ListItem>
                                <asp:ListItem Value="5">Transporte</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="Finalidade" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFinalidade" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Normal</asp:ListItem>
                                <asp:ListItem Value="2">Complementar</asp:ListItem>
                                <asp:ListItem Value="3">Ajuste</asp:ListItem>
                                <asp:ListItem Value="4">Devolução/Retorno</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label25" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtRota" runat="server" MaxLength="20" Width="80px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" OnClientClick="return openRota();" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label14" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtIdCliente" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label19" runat="server" Text="Tipo Fiscal" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoFiscal" runat="server">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Consumidor Final</asp:ListItem>
                                <asp:ListItem Value="2">Revenda</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtIdFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeFornecedor" runat="server" Width="170px" onkeypress="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label17" runat="server" Text="Forma de emissão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFormaEmissao" runat="server">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Normal</asp:ListItem>
                                <asp:ListItem Value="3">Contingência com SCAN</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label8" runat="server" Text="Período Emissão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Período Entrada/Saída" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataEntSaiIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataEntSaiFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="loadProduto(this);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label20" runat="server" Text="Lote" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLote" runat="server" Width="60px"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton12" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label26" runat="server" Text="Forma Pagto." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFormaPagto" runat="server">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">À Vista</asp:ListItem>
                                <asp:ListItem Value="2">À Prazo</asp:ListItem>
                                <asp:ListItem Value="3">Outros</asp:ListItem>
                            </asp:DropDownList>
                            <sync:CheckBoxListDropDown ID="cbdFormaPagtoNotaFiscal" runat="server" DataSourceID="odsFormaPagtoNotaFiscal"
                                DataTextField="Translation" DataValueField="Value" Width="150px">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label27" runat="server" Text="Total Nota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorInicial" runat="server" onkeypress="return soNumeros(event, false, true);"
                                Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label28" runat="server" Text="a" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorFinal" runat="server" onkeypress="return soNumeros(event, false, true);"
                                Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Inf. Compl./Obs." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtInfCompl" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Agrupar relatório por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAgrupar" runat="server">
                                <asp:ListItem Value="0">Não agrupar</asp:ListItem>
                                <asp:ListItem Value="1">Emitente</asp:ListItem>
                                <asp:ListItem Value="2">Destinatário</asp:ListItem>
                                <asp:ListItem Value="3">CFOP</asp:ListItem>
                                <asp:ListItem Value="4">Data Emissão</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="1">Número NF (decresc.)</asp:ListItem>
                                <asp:ListItem Value="2">Número NF (cresc.)</asp:ListItem>
                                <asp:ListItem Value="3" Selected="True">Data de emissão (descresc.)</asp:ListItem>
                                <asp:ListItem Value="4">Data de emissão (cresc.)</asp:ListItem>
                                <asp:ListItem Value="5">Data de entrada/saída (descresc.)</asp:ListItem>
                                <asp:ListItem Value="6">Data de entrada/saída (cresc.)</asp:ListItem>
                                <asp:ListItem Value="7">Valor Total(cresc.)</asp:ListItem>
                                <asp:ListItem Value="8">Valor Total (descresc.)</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblDataVencimentoCertificado" runat="server" Font-Bold="True" Font-Size="Small"
                    ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr style="<%= String.IsNullOrEmpty(GetTipoContingenciaNfe()) ? "display: none": "" %>">
            <td align="center">
                <asp:Label ID="lblContingenciaNFe" runat="server" Text="Nota Fiscal em Contingência: "
                    Font-Bold="true" Font-Size="Medium" ForeColor="Blue"></asp:Label>
            </td>
        </tr>
        <tr style="<%= String.IsNullOrEmpty(GetTipoContingenciaNfe()) ? "display: none": "" %>">
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdNf" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    DataSourceID="odsNf" DataKeyNames="IdNf" EmptyDataText="Nenhum Nota Fiscal encontrada."
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" OnRowCommand="grdNf_RowCommand" 
                    OnRowDataBound="grdNf_RowDataBound">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfIdNf" runat="server" Value='<%# Eval("IdNf") %>' />
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# LinkEditarNf((uint)Eval("IdNf"), Eval("TipoDocumento").ToString(), (int)Eval("Situacao")) %>'
                                    Visible='<%# Eval("EditVisible") %>' OnLoad="lnkEditar_Load">
                                      <img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" Visible='<%# Eval("TerceirosEmAbertoVisible") %>'
                                    ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick="return confirm(&quot;Tem certeza que deseja excluir esta Nota Fiscal?&quot;);"
                                    ToolTip="Excluir" />
                                <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%# (int)Eval("Situacao") != (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.FinalizadaTerceiros %>'>
                                    <a href="#" onclick="openWindow(450, 700, '../Utils/ShowLogNfe.aspx?IdNf=<%# Eval("IdNf") %>'); return false;">
                                        <img src="../Images/blocodenotas.png" title="Log de eventos" border="0" /></a>
                                </asp:PlaceHolder>
                                <uc2:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="NotaFiscal" IdRegistro='<%# Eval("IdNf") %>' />
                                <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("PrintDanfeVisible") %>'>
                                    <a href="#" onclick="openRptDanfe('<%# Eval("IdNf") %>');">
                                        <img border="0" src="../Images/Relatorio.gif" border="0" /></a></asp:PlaceHolder>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# Eval("PrintNfTercVisible") %>'>
                                    <a href="#" onclick="openRptTerc('<%# Eval("IdNf") %>');">
                                        <img border="0" src="../Images/Relatorio.gif" border="0" /></a></asp:PlaceHolder>
                                <asp:LinkButton ID="lnkConsultaSitLote" runat="server" CommandName="ConsultaSitLote"
                                    Visible='<%# Eval("ConsSitLoteVisible") %>' CommandArgument='<%# Eval("IdNf") %>'>
                                    <img border="0" src="../Images/ConsSitLoteNFe.gif" title="Consulta Situação do Lote" border="0" /></asp:LinkButton>
                                <asp:LinkButton ID="lnkConsultaSitNFe" runat="server" CommandName="ConsultaSitNFe"
                                    Visible='<%# Eval("ConsSitVisible") %>' CommandArgument='<%# Eval("IdNf") %>'>
                                    <img border="0" src="../Images/ConsSitNFe.gif" title="Consulta Situação da NFe" border="0" /></asp:LinkButton>
                                <asp:LinkButton ID="lnkSalvarXmlNota" runat="server" Visible='<%# Eval("BaixarXmlVisible") %>'
                                    OnClientClick='<%# "salvarNota(\"" + Eval("IdNf") + "\"); return false;" %>'><img border="0" 
                                    src="../Images/disk.gif" title="Salvar arquivo da nota fiscal" /></asp:LinkButton>
                                <asp:LinkButton ID="lnkAnexarXMLTer" runat="server" Visible='<%# Eval("AnexarXMLTercVisible") %>'
                                    OnClientClick='<%# "anexarXMLTer(\"" + Eval("IdNf") + "\"); return false;" %>'><img border="0" 
                                    src="../Images/page_attach.gif" title="Anexar XML Entrada Terceiros" /></asp:LinkButton>
                                <asp:LinkButton ID="lnkBaixarXMLTer" runat="server" Visible='<%# Eval("BaixarXMLTercVisible") %>'
                                    OnClientClick='<%# "baixarXMLTer(\"" + Eval("IdNf") + "\"); return false;" %>'><img border="0" 
                                    src="../Images/page_save.gif" title="Baixar XML Entrada Terceiros" /></asp:LinkButton>
                                <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/clipe.gif" Visible='<%# Eval("ExibirDocRef") %>'
                                    OnClientClick='<%# "openWindow(600, 800, \"../Utils/DocRefNotaFiscal.aspx?idNf=" + Eval("IdNf") + "\"); return false" %>'
                                    ToolTip="Processos/Documentos Referenciados" />
                                <asp:ImageButton ID="imgAguaGasEnergia" runat="server" ImageUrl="~/Images/page_gear.png"
                                    OnClientClick='<%# "openWindow(600, 800, \"../Utils/InfoAdicNotaFiscal.aspx?idNf=" + Eval("IdNf") + "&tipo=" + Eval("TipoDocumento") +"\"); return false" %>'
                                    ToolTip="Informações adicionais" Visible='<%# Eval("ExibirLinkInfoAdic") %>' />
                                <asp:ImageButton ID="imgEmitirFs" runat="server" ImageUrl="~/Images/arrow_right.gif"
                                    ToolTip="Emitir NF-e" Visible='<%# Eval("EmitirNfFsVisible") %>' CommandArgument='<%# Eval("IdNf") %>'
                                    CommandName="Emitir" />
                                <uc5:ctrlBoleto ID="ctrlBoleto1" runat="server" Visible='<%# Eval("ExibirBoleto") %>'
                                    CodigoNotaFiscal='<%# Eval("IdNf") != null ? Glass.Conversoes.StrParaInt(Eval("IdNf").ToString()) : (int?)null %>' />
                                <asp:ImageButton ID="imgObsLancFiscal" runat="server" ImageUrl="~/Images/Nota.gif"
                                    OnClientClick='<%# "openWindow(600, 800, \"../Utils/SetObsLancFiscal.aspx?idNf=" + Eval("IdNf") + "\"); return false" %>'
                                    ToolTip="Observações do Lançamento Fiscal" />
                                <asp:ImageButton ID="imgAjustes" runat="server" ImageUrl="~/Images/dinheiro.gif" 
                                    onclientclick='<%# Eval("IdNf", "openWindow(600, 950, \"../Listas/LstAjusteDocumentoFiscal.aspx?idNf={0}\"); return false;") %>' 
                                    ToolTip="Ajustes do Documento Fiscal" />
                                <asp:ImageButton ID="imgReenviarXml" runat="server" ImageUrl="~/Images/reenvio_email.png"
                                    ToolTip='<%# (bool)Eval("ReenviarEmailXmlVisible") ? "Reenviar e-mail XML / DANFE" : (bool)Eval("ReenviarEmailXmlCancelamentoVisible") ? "Reenviar e-mail XML / DANFE (Cancelamento)" : "" %>' 
                                    CommandName='<%# (bool)Eval("ReenviarEmailXmlVisible") ? "ReenviarEmailXml" : (bool)Eval("ReenviarEmailXmlCancelamentoVisible") ? "ReenviarEmailXmlCancelamento" : "" %>'
                                    CommandArgument='<%# Eval("IdNf") %>' Visible='<%# (bool)Eval("ReenviarEmailXmlVisible") || (bool)Eval("ReenviarEmailXmlCancelamentoVisible") %>'
                                    OnClientClick="if (!confirm('Deseja reenviar o e-mail do XML / DANFE?')) return false"/>                                
                                <asp:HiddenField ID="hdfCorLinha" runat="server" Value='<%# Eval("CorLinhaGrid") %>' />
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumeroNFe" HeaderText="Num. NF" SortExpression="NumeroNFe" />
                        <asp:BoundField DataField="Serie" HeaderText="Série" SortExpression="Serie" />
                        <asp:BoundField DataField="Modelo" HeaderText="Modelo" SortExpression="Modelo" />
                        <asp:BoundField DataField="CodCfop" HeaderText="CFOP" SortExpression="CodCfop" />
                        <asp:BoundField DataField="TipoDocumentoString" HeaderText="Tipo" SortExpression="TipoDocumentoString" />
                        <asp:BoundField DataField="DescrUsuCad" HeaderText="Funcionário" SortExpression="DescrUsuCad" />
                        <asp:BoundField DataField="NomeEmitente" HeaderText="Emitente" SortExpression="NomeEmitente" />
                        <asp:BoundField DataField="NomeDestRem" HeaderText="Destinatário/Remetente" SortExpression="NomeDestRem" />
                        <asp:BoundField DataField="TotalNota" HeaderText="Total" SortExpression="TotalNota"
                            DataFormatString="{0:C}" />
                        <asp:BoundField DataField="DataEmissao" DataFormatString="{0:d}" HeaderText="Data Emissão"
                            SortExpression="DataEmissao" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="SituacaoString">
                            <ItemTemplate>
                                <asp:Label ID="lblSituacao" runat="server" ForeColor='<%# Eval("CorSituacao") %>'
                                    Text='<%# ((bool)Eval("EmitirNfFsVisible") ? "FS-DA: " : "") + Eval("SituacaoString") %>'
                                    Style='<%# (bool)Eval("ExibirReabrir") ? "position: relative; bottom: 3px": "" %>'></asp:Label>
                                <asp:ImageButton ID="imbReabrir" runat="server" CommandArgument='<%# Eval("IdNf") %>'
                                    CommandName="Reabrir" ImageUrl="~/Images/cadeado.gif" OnClientClick="return confirm(&quot;Deseja reabrir esta nota fiscal?&quot;)"
                                    ToolTip="Reabrir pedido" Visible='<%# Eval("ExibirReabrir") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("SituacaoString") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkCompl" runat="server" ToolTip="Gerar NFe Complementar" CommandName="Complementar"
                                    Visible='<%# Eval("GerarNFComplVisible") %>' CommandArgument='<%# Eval("IdNf") %>'
                                    OnClientClick="return confirm('Tem certeza que deseja gerar uma NF-e Complementar desta Nota?')">
                                    <img src="../Images/document_add.gif" border="0" /></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgCCe" runat="server" ImageUrl="~/Images/email.png" ToolTip="Criar Carta de Correção"
                                    OnClientClick='<%# "abrirCartaCorrecao(" + Eval("IdNf") + "); return false" %>'
                                    Visible='<%# Eval("CartaCorrecaoVisible") %>' />
                                <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%# ((bool)Eval("ExibirPedidosVisible") || (bool)Eval("ExibirComprasVisible")) %>'>
                                    <a href="#" onmouseover='TagToTip("nfe_<%# Eval("IdNf") %>", COPYCONTENT, false); return false;'
                                        onmouseout='UnTip();'>
                                        <img src="../Images/cart.png" border="0" /></a>
                                    <div id="nfe_<%# Eval("IdNf") %>" style="display: none">
                                        <asp:Label ID="Label1" runat="server" Text='<%# (bool)Eval("ExibirPedidosVisible") ? "Pedido(s): " + Eval("IdsPedido") : "Compra(s): " + Eval("IdCompras") %>'></asp:Label><br />
                                    </div>
                                </asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="ajuste(<%# Eval("IdNf") %>)">
                                    <img src="../Images/Nota.gif" border="0" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbCentroCusto" runat="server" ImageUrl='<%# "~/Images/" + ((bool)Eval("CentroCustoCompleto") ? "cash_blue.png" : "cash_red.png") %>' Visible='<%# (bool)Eval("ExibirCentroCusto") %>' 
                                    ToolTip="Exibir Centro de Custos" OnClientClick='<%# "exibirCentroCusto(" + Eval("IdNf") + "); return false" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkSepararValores" runat="server" CommandName="SepararValores"
                                    Visible='<%# Eval("SepararValoresVisible") %>' CommandArgument='<%# Eval("IdNf") %>' ToolTip="Vincular valores">
                                    <img border="0" src="../Images/separar_valores.png" title="Vincular valores" border="0" /></asp:LinkButton>
                                <asp:LinkButton ID="lnkCancelarSepararValores" runat="server" CommandName="CancelarSepararValores"
                                    Visible='<%# Eval("CancelarSeparacaoValoresVisible") %>' CommandArgument='<%# Eval("IdNf") %>' ToolTip="Desvincular valores">
                                    <img border="0" src="../Images/separar_valores_cancelar.png" title="Desvincular valores" border="0" /></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEmitirNFCe" runat="server" ImageUrl="~/Images/arrow_right.gif"
                                    ToolTip="Emitir NFC-e" Visible='<%# Eval("EmitirNFCeVisible") %>' CommandArgument='<%# Eval("IdNf") %>'
                                    CommandName="EmitirNFCe"></asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgInfoNota" runat="server" ImageUrl="~/Images/info.gif"
                                    OnClientClick='<%# "openWindow(600, 800, \"../Utils/LogMovimentacaoNotaFiscal.aspx?idNf=" + Eval("IdNf") + "\"); return false" %>'
                                    ToolTip="Log Movimentações de Estoque da NFe " 
                                    Visible='<%# (int)Eval("Situacao") == (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.Autorizada || (int)Eval("Situacao") == (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.FinalizadaTerceiros  %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <asp:GridView GridLines="None" ID="grdNfAutFin" runat="server" AllowPaging="True"
                    AutoGenerateColumns="False" DataSourceID="odsNf" DataKeyNames="IdNf" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Nenhum Nota Fiscal encontrada." OnRowCommand="grdNf_RowCommand"
                    Visible="False" OnRowDataBound="grdNf_RowDataBound">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar0" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadNotaFiscal.aspx?idNf=" + Eval("IdNf") + "&tipo=" + Eval("TipoDocumento") + ((int)Eval("Situacao") == (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.FinalizadaTerceiros ? "&manual=1" : "") %>'
                                    Visible='<%# Eval("EditVisible") %>' OnLoad="lnkEditar_Load">
                                     <img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                                <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%# (int)Eval("Situacao") != (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.FinalizadaTerceiros %>'>
                                    <a href="#" onclick="openWindow(450, 700, '../Utils/ShowLogNfe.aspx?IdNf=<%# Eval("IdNf") %>'); return false;">
                                        <img src="../Images/blocodenotas.png" title="Log de eventos" border="0" /></a>
                                </asp:PlaceHolder>
                                <uc2:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="NotaFiscal" Visible='<%# (int)Eval("Situacao") == (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.FinalizadaTerceiros %>'
                                    IdRegistro='<%# Eval("IdNf") %>' />
                                <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%# Eval("PrintDanfeVisible") %>'>
                                    <a href="#" onclick="openRptDanfe('<%# Eval("IdNf") %>');">
                                        <img border="0" src="../Images/Relatorio.gif" border="0" /></a></asp:PlaceHolder>
                                <asp:LinkButton ID="lnkConsultaSitLote0" runat="server" CommandName="ConsultaSitLote"
                                    Visible='<%# Eval("ConsSitVisible") %>' CommandArgument='<%# Eval("IdNf") %>'>
                                    <img border="0" src="../Images/ConsSitLoteNFe.gif" title="Consulta Situação do Lote" border="0" /></asp:LinkButton>
                                <asp:LinkButton ID="lnkConsultaSitNFe0" runat="server" CommandName="ConsultaSitNFe"
                                    Visible='<%# Eval("ConsSitVisible") %>' CommandArgument='<%# Eval("IdNf") %>'>
                                    <img border="0" src="../Images/ConsSitNFe.gif" title="Consulta Situação da NFe" border="0" /></asp:LinkButton>
                                <asp:LinkButton ID="lnkSalvarXmlNota0" runat="server" Visible='<%# Eval("PrintDanfeVisible") %>'
                                    OnClientClick='<%# "salvarNota(\"" + Eval("IdNf") + "\"); return false;" %>'><img border="0" 
                                    src="../Images/disk.gif" title="Salvar arquivo da nota fiscal" /></asp:LinkButton>
                                <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/clipe.gif" Visible='<%# Eval("ExibirDocRef") %>'
                                    OnClientClick='<%# "openWindow(600, 800, \"../Utils/DocRefNotaFiscal.aspx?idNf=" + Eval("IdNf") + "\"); return false" %>'
                                    ToolTip="Processos/Documentos Referenciados" />
                                <asp:ImageButton ID="imgAguaGasEnergia" runat="server" ImageUrl="~/Images/page_gear.png"
                                    OnClientClick='<%# "openWindow(600, 800, \"../Utils/InfoAdicNotaFiscal.aspx?idNf=" + Eval("IdNf") + "&tipo=" + Eval("TipoDocumento") +"\"); return false" %>'
                                    ToolTip="Informações adicionais" Visible='<%# Eval("ExibirLinkInfoAdic") %>' />
                                <asp:ImageButton ID="imgEmitirFs" runat="server" CommandArgument='<%# Eval("IdNf") %>'
                                    CommandName="Emitir" ImageUrl="~/Images/arrow_right.gif" ToolTip="Emitir NF-e"
                                    Visible='<%# Eval("EmitirNfFsVisible") %>' />
                                <uc5:ctrlBoleto ID="ctrlBoleto1" runat="server" Visible='<%# Eval("ExibirBoleto") %>'
                                    CodigoNotaFiscal='<%# Eval("IdNf") != null ? Glass.Conversoes.StrParaInt(Eval("IdNf").ToString()) : (int?)null %>' />
                                <asp:HiddenField ID="hdfCorLinha" runat="server" Value='<%# Eval("CorLinhaGrid") %>' />
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumeroNFe" HeaderText="Num. NF" SortExpression="NumeroNFe" />
                        <asp:BoundField DataField="Serie" HeaderText="Série" SortExpression="Serie" />
                        <asp:BoundField DataField="CodCfop" HeaderText="CFOP" SortExpression="CodCfop" />
                        <asp:BoundField DataField="TipoDocumentoString" HeaderText="Tipo" SortExpression="TipoDocumentoString" />
                        <asp:BoundField DataField="DataEmissao" HeaderText="Data Emissão" SortExpression="DataEmissao" />
                        <asp:BoundField DataField="DataSaidaEnt" HeaderText="Data Entrada/Saída" SortExpression="DataSaidaEnt" />
                        <asp:BoundField DataField="NomeEmitente" HeaderText="Emitente" SortExpression="NomeEmitente" />
                        <asp:BoundField DataField="NomeDestRem" HeaderText="Destinatário/Remetente" SortExpression="NomeDestRem" />
                        <asp:BoundField DataField="BcIcms" HeaderText="Base Calc. ICMS" SortExpression="BcIcms"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="BcIcmsSt" DataFormatString="{0:c}" HeaderText="Base Calc. ICMS ST"
                            SortExpression="BcIcmsSt" />
                        <asp:BoundField DataField="Valoricms" DataFormatString="{0:c}" HeaderText="Valor ICMS"
                            SortExpression="Valoricms" />
                        <asp:BoundField DataField="ValorIcmsSt" DataFormatString="{0:c}" HeaderText="Valor ICMS ST"
                            SortExpression="ValorIcmsSt" />
                        <asp:BoundField DataField="ValorIpi" DataFormatString="{0:c}" HeaderText="Valor IPI"
                            SortExpression="ValorIpi" />
                        <asp:BoundField DataField="TotalNota" HeaderText="Total" SortExpression="TotalNota"
                            DataFormatString="{0:C}" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="SituacaoString">
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" ForeColor='<%# Eval("CorSituacao") %>' Style='<%# (bool)Eval("ExibirReabrir") ? "position: relative; bottom: 3px": "" %>'
                                    Text='<%# ((bool)Eval("EmitirNfFsVisible") ? "FS-DA: " : "") + Eval("SituacaoString") %>'></asp:Label>
                                <asp:ImageButton ID="imbReabrir0" runat="server" CommandArgument='<%# Eval("IdNf") %>'
                                    CommandName="Reabrir" ImageUrl="~/Images/cadeado.gif" OnClientClick="return confirm(&quot;Deseja reabrir esta nota fiscal?&quot;)"
                                    ToolTip="Reabrir pedido" Visible='<%# Eval("ExibirReabrir") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("SituacaoString") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkCompl" runat="server" ToolTip="Gerar NFe Complementar" CommandName="Complementar"
                                    Visible='<%# Eval("GerarNFComplVisible") %>' CommandArgument='<%# Eval("IdNf") %>'
                                    OnClientClick="return confirm('Tem certeza que deseja gerar uma NF-e Complementar desta Nota?')">
                                    <img src="../Images/document_add.gif" border="0" /></asp:LinkButton>
                                <asp:ImageButton ID="imgCCe" runat="server" ImageUrl="~/Images/email.png" ToolTip="Criar Carta de Correção"
                                    OnClientClick='<%# "abrirCartaCorrecao(" + Eval("IdNf") + "); return false" %>'
                                    Visible='<%# Eval("SituacaoString")=="Autorizada" %>' />
                                <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%# Eval("ExibirPedidosVisible") %>'>
                                    <a href="#" onmouseover='TagToTip("nfe_<%# Eval("IdNf") %>", COPYCONTENT, false); return false;'
                                        onmouseout='UnTip();'>
                                        <img src="../Images/cart.png" border="0" /></a>
                                    <div id="nfe_<%# Eval("IdNf") %>" style="display: none">
                                        <asp:Label ID="Label1" runat="server" Text='<%# "Pedidos: " + Eval("IdsPedido") %>'></asp:Label><br />
                                    </div>
                                </asp:PlaceHolder>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="ajuste(<%# Eval("IdNf") %>)">
                                    <img src="../Images/Nota.gif" border="0" /></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <div runat="server" id="divNumNotasFs" style="padding: 8px 0; font-size: small">
                    Número de notas fiscais a emitir (Formulário de Segurança):
                    <asp:Label ID="lblNumNotasFs" runat="server" Text="0"></asp:Label>
                </div>
                <div runat="server" id="divContingencia" style="padding: 8px 0">
                    <asp:LinkButton ID="lnkAlterarContingenciaNFe" runat="server" OnClick="lnkAlterarContingenciaNFe_Click"
                        OnClientClick="if (!confirm('Deseja alterar o modo de Contingência da NFe?')) return false">Habilitar Contingência da NFe</asp:LinkButton>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:LinkButton ID="lnkAlterarContingenciaFsDa" runat="server" OnClick="lnkAlterarContingenciaFsDa_Click"
                        OnClientClick="if (!confirm('Deseja alterar o modo de Contingência da NFe para FS-DA?')) return false"> <img border="0" src="../Images/fsda.gif" /> Habilitar Contingência FS-DA da NFe</asp:LinkButton>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:LinkButton ID="lnkDesabilitarContingenciaNFe" runat="server" OnClick="lnkDesabilitarContingenciaNFe_Click"
                        OnClientClick="if (!confirm('Deseja desabilitar o modo de Contingência da NFe?')) return false"> Desabilitar Contingência da NFe</asp:LinkButton>
                </div>
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center" colspan="3">
                            <asp:LinkButton ID="lbkDanfe" runat="server" OnClientClick="openRptDanfes(); return false;"
                                Visible="False"> <img border="0" src="../Images/Printer.png" />Imprimir Danfes</asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:LinkButton ID="lkbRelatorio" runat="server" OnClientClick="openReport(false, false); return false;"> <img border="0" src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td align="left">
                            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openReport(false, true); return false;"><img border="0" 
                                src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                        </td>
                        <td>
                            <asp:LinkButton OnClientClick="window.open('http://www.nfe.fazenda.gov.br/portal/disponibilidade.aspx?versao=0.00&tipoConteudo=Skeuqr8PQBY=');"
                                 target="_blank" runat="server"><img border="0" 
                                src="../Images/nfe.png" height="24px" width="24px" /> Consultar Disponibilidade</asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span id="relatorioProd" runat="server">
                                <asp:LinkButton ID="lnkRelatorioProd" runat="server" OnClientClick="openReport(true, false); return false;"> <img border="0" src="../Images/Printer.png" /> Imprimir (Produtos)</asp:LinkButton>
                            </span>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td align="left">
                            <span id="relatorioProdExcel" runat="server">
                                <asp:LinkButton ID="lnkExportarExcel0" runat="server" OnClientClick="openReport(true, true); return false;"><img border="0" 
                src="../Images/Excel.gif" /> Exportar para o Excel (Produtos)</asp:LinkButton>
                            </span>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lkbLoteXml" runat="server" OnClientClick="openLoteNotas(); return false;"> <img border="0" src="../Images/disk.gif" /> Baixar XMLs em Lote</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNf" runat="server" DataObjectTypeName="Glass.Data.Model.NotaFiscal"
                    DeleteMethod="Delete" SelectMethod="GetListaPadrao" TypeName="Glass.Data.DAL.NotaFiscalDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCountListaPadrao"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumNf" Name="numeroNFe" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtModelo" Name="modelo" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtIdCliente" Name="idCliente" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeCliente" Name="nomeCliente" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpTipoFiscal" Name="tipoFiscal" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="txtIdFornec" Name="idFornec" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeFornecedor" Name="nomeFornec" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtRota" Name="codRota" PropertyName="Text" Type="String" />
                        <asp:Parameter Name="tipoDoc" Type="Int32" />
                        <asp:ControlParameter ControlID="cboSituacao" Name="situacao" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpCfop" Name="idsCfop" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="cbxdrpTipoCFOP" Name="idsTiposCfop" PropertyName="SelectedValue" />  
                        <asp:ControlParameter ControlID="ctrlDataEntSaiIni" Name="dataEntSaiIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataEntSaiFim" Name="dataEntSaiFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpFormaPagto" Name="formaPagto" PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="cbdFormaPagtoNotaFiscal" Name="idsFormaPagtoNotaFiscal" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="drpTipoNota" Name="tipoNf" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpFinalidade" Name="finalidade" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="drpFormaEmissao" Name="formaEmissao" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="txtInfCompl" Name="infCompl" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInternoProd" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descrProd" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtValorInicial" Name="valorInicial" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtValorFinal" Name="valorFinal" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpOrdenar" Name="ordenar" PropertyName="SelectedValue" Type="Int32" />
                        <asp:Parameter Name="cnpjFornecedor" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCfop" runat="server" SelectMethod="GetSortedByCodInterno"
                    TypeName="Glass.Data.DAL.CfopDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCfop" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.TipoCfopDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormasPagto" runat="server" SelectMethod="GetFormasPagtoNf"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagtoNotaFiscal" runat="server"
                    TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.FormaPagtoNotaFiscalEnum, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">

        $( document ).ready(function() {
            var falhaEmitirNFce = GetQueryString('falhaEmitirNFce');
            var idNf = GetQueryString('idNf');

            var txtConfirmacao = "Houve uma falha de conexão ao tentar emitir a NFC-e.";
            txtConfirmacao+="\n\nNesse caso é possível realizar a emissão em Contingência Offline, porém a mesma deverá ser posteriormente autorizada.";
            txtConfirmacao+="\nA não autorização em 24 hrs, seja por inconsistência ou persistência do problema, poderá resultar em custos e riscos adicionais.";
            txtConfirmacao+="\n\nDeseja prosseguir?";

            if(falhaEmitirNFce == "true" && confirm(txtConfirmacao)){
                var ret = LstNotaFiscal.EmitirNfcOffline(idNf);

                if(ref.error!=null){
                    alert(ref.error.description);
                    return false;
                }

                window.location.href = window.location.origin + "/Listas/LstNotaFiscal.aspx";
            }

        });

    </script>

</asp:Content>
