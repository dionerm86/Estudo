<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AtribuirRetalhos.aspx.cs"
    Inherits="Glass.UI.Web.Utils.AtribuirRetalhos" MasterPageFile="~/Painel.master" Title="Atribuir Retalhos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <link rel="stylesheet" type="text/css" href="../Style/StyleProd.css" />
    <link rel="stylesheet" type="text/css" href="../Style/GridView.css" />
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery/jquery-1.9.0.min.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">
        var retalhosProdutos = [];
        var linhasOpener = [];

        function load()
        {
            bloquearPagina();
            var linhas = window.opener.document.getElementById("lstProd").rows;
            
            for (var i = 1; i < linhas.length; i++)
            {
                if (linhas[i].style.display == "none")
                    continue;

                // Verifica se o controle de retalhos está sendo exibido
                var c = "ctrlRetalhos_" + linhas[i].getAttribute("objId");

                if (window.opener[c] == null || window.opener[c] == undefined)
                    continue;
                
                if (window.opener.document.getElementById(c + "_imgSelecionar").style.display == "none")
                    continue;

                var qtdImprimir = parseInt(FindControl("txtQtdImp", "input", linhas[i]).value, 10);
                
                // Peças repostas
                if (qtdImprimir == 0) qtdImprimir = 1;
                
                var idsSelecionados = window.opener.eval(c).RetalhosAssociados();
                idsSelecionados = idsSelecionados ? idsSelecionados.split(",") : null;

                for (var j = 1; j <= qtdImprimir; j++)
                {
                    var id = idsSelecionados != null && idsSelecionados.length > 0 ? idsSelecionados[0] : "";
                    
                    if (id != "")
                    {
                        var temp = []
                        for (var k = 1; k < idsSelecionados.length; k++)
                            temp.push(idsSelecionados[k]);

                        idsSelecionados = temp;
                    }
                    
                    var numLinha = countItem["lstRetalhos"] ? countItem["lstRetalhos"] : 1;
                    var retalhos = AtribuirRetalhos.GetRetalhos(linhas[i].getAttribute("objId"), numLinha, j, id).value.split("|");

                    addItem([linhas[i].cells[1].innerHTML, linhas[i].cells[2].innerHTML, linhas[i].cells[3].innerHTML,
                        linhas[i].cells[4].innerHTML, linhas[i].cells[5].innerHTML, linhas[i].cells[6].innerHTML,
                        linhas[i].cells[7].innerHTML, j, retalhos[0]],
                        ["Pedido", "Produto", "Largura x Altura", "Tot. M²", "Proc.", "Apl.", "Qtd.",
                        "Número peça", "Retalho(s)"], "lstRetalhos");

                    retalhosProdutos.push({
                        IdProdPed: linhas[i].getAttribute("objId"),
                        NumeroPeca: j,
                        IdRetalhoProducao: retalhos[2] != "" ? retalhos[2] : null,
                        NumeroLinha: numLinha,
                        IdsRetalhosPossiveis: retalhos[1]
                    });
                }
                
                linhasOpener.push({
                    IdProdPed: linhas[i].getAttribute("objId"),
                    Opener: i
                });
            }

            $.each(document.getElementById("lstRetalhos").rows, function(index, value)
            {
                value.cells[0].innerHTML = "";
                value.cells[0].style.display = "none";
            });

            desbloquearPagina(true);
        }

        function otimizar()
        {
            bloquearPagina();
            
            var mensagem, numRetalhos = 0;

            try
            {
                var resultado = JSON.stringify(retalhosProdutos);
                resultado = AtribuirRetalhos.Otimizar(resultado).value;
                retalhosProdutos = eval(resultado);

                for (var i in retalhosProdutos)
                {
                    var retalhos = FindControl("retalhos_" + retalhosProdutos[i].NumeroLinha + "_" + retalhosProdutos[i].NumeroPeca, "table");
                    var inputs = retalhos.getElementsByTagName("input");

                    for (var j = 0; j < inputs.length; j++)
                    {
                        var idRetalhoProducaoInput = inputs[j].parentNode.getAttribute("idRetalhoProducao");
                        inputs[j].checked = idRetalhoProducaoInput == retalhosProdutos[i].IdRetalhoProducao;
                        
                        if (inputs[j].checked)
                            numRetalhos++;
                    }
                }
                
                mensagem = "Retalhos otimizados com sucesso!\nNúmero de retalhos utilizados: " + numRetalhos;
            }
            catch (err)
            {
                mensagem = err.message;
            }
            finally
            {
                desbloquearPagina(true);
                alert(mensagem);
            }
        }

        function atribuir(item)
        {
            var idRetalhoProducao = !item.checked ? null :
                item.parentNode.getAttribute("idRetalhoProducao");

            var tabela = item;
            while (tabela.nodeName.toLowerCase() != "table")
                tabela = tabela.parentNode;

            var inputs = tabela.getElementsByTagName("input");
            for (var i = 0; i < inputs.length; i++)
                inputs[i].checked = inputs[i].id != item.id ? false : inputs[i].checked;

            var dadosTabela = tabela.id.split("_");
            for (var i in retalhosProdutos)
            {
                if (retalhosProdutos[i].NumeroLinha == dadosTabela[1] &&
                    retalhosProdutos[i].NumeroPeca == dadosTabela[2])
                {
                    retalhosProdutos[i].IdRetalhoProducao = idRetalhoProducao;
                    break;
                }
            }
        }

        function validar()
        {
            var usados = [];
            var linhas = document.getElementById("lstRetalhos").rows;
            var retalhos = new Object();

            for (var i in retalhosProdutos)
            {
                if (retalhosProdutos[i].IdRetalhoProducao != null)
                {
                    //for (var j = 0; j < usados.length; j++)
                    //{
                    //    if (usados[j].IdRetalhoProducao == retalhosProdutos[i].IdRetalhoProducao)
                    //    {
                    //        var linhaUsada = linhas[usados[j].NumeroLinha];
                    //        var linhaNova = linhas[retalhosProdutos[i].NumeroLinha];

                    //        alert("O mesmo retalho não pode ser usado para 2 produtos diferentes. " +
                    //            "Pedidos " + linhaUsada.cells[1].innerHTML + " e " + linhaNova.cells[1].innerHTML +
                    //            ", Produtos " + linhaUsada.cells[2].innerHTML + " (" + linhaUsada.cells[8].innerHTML +
                    //            ") e " + linhaNova.cells[2].innerHTML + " (" + linhaNova.cells[8].innerHTML + ").");

                    //        return null;
                    //    }
                    //}

                    //usados.push(retalhosProdutos[i]);

                    if (!retalhos[retalhosProdutos[i].IdProdPed])
                        retalhos[retalhosProdutos[i].IdProdPed] = [];

                    retalhos[retalhosProdutos[i].IdProdPed].push(retalhosProdutos[i].IdRetalhoProducao);
                }
            }

            return retalhos;
        }

        function confirmar()
        {
            bloquearPagina();

            try
            {
                var retalhos = validar();
                if (retalhos == null)
                    return;
                
                var linhas = window.opener.document.getElementById("lstProd").rows;

                for (var i = 1; i < linhas.length; i++)
                {
                    if (linhas[i].style.display == "none")
                        continue;
                        
                    var c = "ctrlRetalhos_" + linhas[i].getAttribute("objId");

                    if (window.opener[c] == null || window.opener[c] == undefined)
                        return false;
                    
                    var idsRetalhos = window.opener.eval(c);

                    if (!idsRetalhos)
                        continue;

                    idsRetalhos.RetalhosAssociados("");

                    for (var j = 0; j < linhasOpener.length; j++)
                        if (i == linhasOpener[j].Opener)
                        {
                            if (retalhos[linhasOpener[j].IdProdPed])
                                idsRetalhos.RetalhosAssociados(retalhos[linhasOpener[j].IdProdPed].join(","));
                            
                            break;
                        }
                }

                desbloquearPagina(false);
                closeWindow();
            }
            finally
            {
                desbloquearPagina(true);
            }
        }

        function imprimir()
        {
            if (validar() == null)
                return;
                
            openWindow(600, 800, "../Relatorios/RelRetalhosAssociados.aspx");
        }

        window.onunload = function()
        {
            window.opener.alterarAjaxRetalhos(false);
        };

        $(document).ready(load);
    </script>

    <table>
        <tr>
            <td align="center">
                <table id="lstRetalhos">
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnOtimizar" runat="server" 
                    onclientclick="otimizar(); return false" Text="Otimizar retalhos" />
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar"
                    OnClientClick="confirmar(); return false" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar"
                    OnClientClick="closeWindow(); return false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="imprimir(); return false">
                    <img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
