<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelEtiquetas.aspx.cs" Inherits="Glass.UI.Web.Relatorios.RelEtiquetas" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Etiquetas</title>
    
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/Geral.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery/jquery-1.8.2.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>

    <script type="text/javascript">
        function load()
        {
            try
            {
                var loaded = document.getElementById("<%= hdfLoad.ClientID %>").value == "true";
                if (loaded)
                    return;

                FindControl("hdfOpener", "input").value = window.opener.location.href;
                var hdf = document.getElementById("hdfIdsProdPedNf");

                //Impressao da peça principal de um produto de composição. Impressão na leitura da produção
                if (GetQueryString("producao") == "true") {

                    //verifica se pode imprimir
                    var returnValue = RelEtiquetas.PodeImprimir(GetQueryString("etq"));

                    if (returnValue.error != null) {
                        alert(returnValue.error.description);
                        return;
                    }

                    // Adiciona no hiddenfield desta página o idProdPed junto com a qtd a ser impressa
                    hdf.value = returnValue.value;
                    FindControl("hdfTipoEtiqueta", "input").value = "1";
                }
                else if (hdf.value == "" && window.opener.FindControl("drpTipoEtiqueta", "select") != null)
                {
                    FindControl("hdfTipoEtiqueta", "input").value = window.opener.FindControl("drpTipoEtiqueta", "select").value;
                    var idsProdPedNf = FindControl("hdfIdProdPedNf", "input", window.opener.document).value;
                   
                    // Se este hiddenfield for nulo, é impressão individual
                    if (hdf == null) return;

                    // Remove a última ','
                    idsProdPedNf = idsProdPedNf.substring(0, idsProdPedNf.length - 1);

                    // Gera um vetor dos ids
                    idsProdPedNf = idsProdPedNf.split(',');
                    
                    // Busca as qtde a serem impressas dos produtos
                    for (var i = 0; i < idsProdPedNf.length; i++) 
                    {
                        var isReposicao = idsProdPedNf[i].indexOf("R") == 0;
                    
                        // Qtd já impressa
                        var qtdImpressa = window.opener.document.getElementById("hdfQtdImpresso_" + idsProdPedNf[i]);
                        qtdImpressa = qtdImpressa != null ? qtdImpressa.value : 0;

                        // Qtd informada para ser impressa
                        var qtdImpUsuario = window.opener.document.getElementById("txtQtdImp_" + idsProdPedNf[i]);
                        qtdImpUsuario = qtdImpUsuario != null && !isReposicao ? qtdImpUsuario.value : 0;

                        // Qtd máxima que pode ser impressa
                        var qtdImpMaxima = window.opener.document.getElementById("hdfQtdImp_" + idsProdPedNf[i]);
                        qtdImpMaxima = qtdImpMaxima != null ? qtdImpMaxima.value : 0;

                        // Etiquetas
                        var etiquetas = window.opener.document.getElementById("hdfEtiquetas_" + idsProdPedNf[i]).value;

                        // Obs da etiqueta
                        var obs = window.opener.document.getElementById("txtObs_" + idsProdPedNf[i]).value;

                        var retornoIsProdutoLaminadoComposicao = RelEtiquetas.IsProdutoLaminadoComposicao(idsProdPedNf[i]);

                        if (retornoIsProdutoLaminadoComposicao.error != null) {
                            alert(retornoIsProdutoLaminadoComposicao.error.description);
                            return;
                        }

                        var isProdutoLaminadoComposicao = retornoIsProdutoLaminadoComposicao.value

                        // Verifica se a quantidade a ser impressa foi informada
                        if (!isReposicao && (qtdImpUsuario == "" || qtdImpUsuario == 0) && !isProdutoLaminadoComposicao)
                        {
                            alert("Informe a quantidade a ser impressa de todos os itens.");
                            habilitaImpressao();
                            closeWindow();
                            return false;
                        }

                        // Verifica se a qtde especificada para ser impressa é maior que a qtde máxima permitida
                        if (parseInt(qtdImpUsuario) > parseInt(qtdImpMaxima))
                        {
                            alert("A quantidade especificada de um dos itens para ser impressa está maior que a quantidade máxima ('Qtd.' - 'Qtd já impresso').");
                            habilitaImpressao();
                            closeWindow();
                            return false;
                        }

                        // Adiciona no hiddenfield desta página o idProdPed junto com a qtd a ser impressa
                        hdf.value += idsProdPedNf[i].toString() + "\t" + qtdImpressa + "\t" + qtdImpUsuario + "\t" + obs + "\t" + etiquetas + "|";
                    }
                }

                // alert("teste");

                //***RETALHO DE PRODUÇÃO***//
                var $lstProd = $("table[id*=tblRetalhos]", window.opener.document);
                
                var $rows = $lstProd.children("tbody").children("tr");

                var $retalhos = [];

                $.each($rows, function(index, value)
                {
                    if ($(this).css("display") != "none")
                    {
                        var altura = $(this).find("input[id*=txtAltura]");
                        var largura = $(this).find("input[id*=txtLargura]");
                        var quantidade = $(this).find("input[id*=txtQuantidade]");
                        var observacao = $(this).find("input[id*=txtObservacao]");

                        if (altura.val() != "" && largura.val() != "" && quantidade.val() != "")
                        {
                            var $retalho = new Object();
                            $retalho.Altura = altura.val();
                            $retalho.Largura = largura.val();
                            $retalho.Quantidade = quantidade.val();
                            $retalho.Observacao = observacao.val();
                            $retalhos.push($retalho);
                        }
                    }
                });

                $("#hdfRetalhos").val(JSON.stringify($retalhos));
                $("input[id*=hdfSomenteRetalhos]").val($("input[id*=hdfSomenteRetalhos]", window.opener.document).val());
                $("input[id*=hdfIdSolucaoOtimizacao]").val($("input[id*=hdfIdSolucaoOtimizacao]", window.opener.document).val());

                if (window.opener.document.location.href.toLowerCase().indexOf("lstetiquetaimprimir") > -1)
                {
                    $lstProd = $("table[id*=lstProd]", window.opener.document);
                    $rows = $lstProd.children("tbody").children("tr");

                    var dadosRetalhos = [];
                    $.each($rows, function(index, value)
                    {
                        if (index > 0 && $(this).css("display") != "none")
                        {
                            var idProdPed = value.getAttribute("objId");
                            var idsRetalhos = $("input[id*=hdfIdRetalhosProducao]", value).val();

                            if (idsRetalhos != "")
                                dadosRetalhos.push(idProdPed + "|" + idsRetalhos);
                        }
                    });

                    document.getElementById("hdfRetalhosProdutos").value = dadosRetalhos.join("+");
                }

                document.getElementById("<%= hdfLoad.ClientID %>").value = "true";
                preparaPostBack();
                document.getElementById("form1").submit();
            }
            catch (err)
            {
                habilitaImpressao();
                alert(err);
                //closeWindow();
            }
        }

        function habilitaImpressao()
        {
            window.opener.document.getElementById("lnkImprimir").style.visibility = "visible";
        }
    </script>
</head>
<body onload="load()">
    <form id="form1" runat="server">
    <asp:HiddenField ID="hdfIdsProdPedNf" runat="server" />
    <asp:HiddenField ID="hdfTipoEtiqueta" runat="server" />
    <asp:HiddenField ID="hdfLoad" runat="server" Value="false" />
    <asp:HiddenField ID="hdfOpener" runat="server" />
    <asp:HiddenField ID="hdfRetalhos" runat="server" />
    <asp:HiddenField ID="hdfSomenteRetalhos" runat="server" />
    <asp:HiddenField ID="hdfRetalhosProdutos" runat="server" />
    <asp:HiddenField ID="hdfIdSolucaoOtimizacao" runat="server" />
    <asp:PlaceHolder ID="pchTabela" runat="server"></asp:PlaceHolder>
    </form>
</body>
</html>
