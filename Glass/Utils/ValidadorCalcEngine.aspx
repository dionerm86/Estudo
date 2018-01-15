<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ValidadorCalcEngine.aspx.cs" Inherits="Glass.UI.Web.Utils.ValidadorCalcEngine"
    Title="" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu"></asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        var nomesArquivoCalc = '<%= Request["nome"] %>';

        if (nomesArquivoCalc == "0")
            nomesArquivoCalc = ValidadorCalcEngine.GetListCalcEngine().value;

        nomesArquivoCalc = nomesArquivoCalc.split("|")

        var processar = null;

        processar = function (arquivos, falhas) {
            if (arquivos.length > 0) {
                var arquivo = arquivos.pop();
                $.ajax({
                    url: "../Handlers/ValidadorCalcEngine.ashx?Nome=" + arquivo,
                    type: 'POST',
                    success: function (dados) {

                        if (!dados.Sucesso) {
                            falhas.push(dados);
                        }

                        montaRetorno(dados);
                        processar(arquivos, falhas);
                    },
                    error: function (errorText) {
                        alert("Erro na validação");
                        processar(arquivos, falhas);
                    }
                });
            } else if (falhas.length > 0) {
                alert(falhas.length + ' falhas');
            }
        };

        processar(nomesArquivoCalc, []);

        function montaRetorno(retornoValidacao) {           

            var tabela = document.getElementById("tabela");
            var row = tabela.insertRow(0);
            var cell1 = row.insertCell(0);

            var erros = "";

            if (retornoValidacao.Sucesso)
                erros = "<h1><font color=\"green\">" + retornoValidacao.Arquivo + "</font></h1>";
            else
                erros = "<h1><font color=\"red\">" + retornoValidacao.Arquivo + "</font></h1>";

            if (!retornoValidacao.Sucesso)
            {
                for (i = 0; i < retornoValidacao.ErroProfile.length; i++)
                {
                    erros += "<ul>";

                    if (retornoValidacao.ErroProfile[i].includes("success"))
                        erros += "<li><font color=\"green\">" + retornoValidacao.ErroProfile[i] + "</font>";
                    else
                        erros += "<li><font color=\"red\">" + retornoValidacao.ErroProfile[i] + "</font>";

                    if (i < retornoValidacao.LinhasErro.length)
                    {
                        erros += "<ul>";
                        erros += "<li><a href=\"#nowhere\">" + retornoValidacao.Mensagem[i] + "</a>";
                        erros += "<ul class=\"list\" style=\"list-style-type:none\">";
                    
                        var linhasDivergentes = retornoValidacao.LinhasErro[i].Erros.split('|');

                        for (var j = 0; j < linhasDivergentes.length; j++) {

                            if (linhasDivergentes[j].includes(") -"))
                                erros += "<li><pre href=\"#\"><font color=\"red\">" + linhasDivergentes[j] + "</font></pre></li>";
                            else
                                erros += "<li><pre href=\"#\"><font color=\"green\">" + linhasDivergentes[j] + "</font></pre></li>"
                        }
                    
                        erros += "</ul>";
                        erros +="</li>";
                        erros += "</ul>";
                    }

                    erros += "</li>";
                    erros += "</ul>";
                }
                
            }

            cell1.innerHTML = erros;
        }

    </script>
    
    <style>

        .list {
          max-height:0;
          overflow:hidden;
          transition:0.5s linear;
          }
        a:focus + ul.list {
          max-height:1000em;
          }
        a:focus {
          pointer-events:none;
          }

    </style>

    <table id="tabela"></table>
    <label id="status"></label>
</asp:Content>
