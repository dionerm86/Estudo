var CapacidadeProducaoDiariaType = (function($)
{
    function salvar(controle)
    {
        var data = $("span[id^=" + controle + "][id*=lblData]");
        var maximoVendas = $("input[id^=" + controle + "][id*=txtMaximoVendas]");
        var setores = $("input[id^=" + controle + "][id*=txtSetor]");
        var classificacoes = $("input[id^=" + controle + "][id*=txtClassificacao]");

        var objeto = {
            "Data": data.text(),
            "MaximoVendasM2": maximoVendas.val(),
            "CapacidadeSetores": [],
            "CapacidadeClassificacoes": []
        };

        for (var i = 0; i < setores.length; i++)
        {
            var codigoSetor = $(setores[i].parentNode).attr("codigoSetor");
            objeto["CapacidadeSetores"].push({
                "Setor": codigoSetor,
                "Capacidade": $(setores[i]).val()
            });
        }

        for (var i = 0; i < classificacoes.length; i++) {
            var codigoClassificacao = $(classificacoes[i].parentNode).attr("codigoClassificacao");
            objeto["CapacidadeClassificacoes"].push({
                "Classificacao": codigoClassificacao,
                "Capacidade": $(classificacoes[i]).val()
            });
        }

        var resposta = ctrlCapacidadeProducaoDiaria.Salvar(JSON.stringify(objeto));

        if (resposta.error != null)
            alert(resposta.error.description);
        else
            atualizarPagina();
    };

    function exibirEdicao(controle, exibirItensEditar)
    {
        var botaoEditar = $("input[id^=" + controle + "][id*=imgEditar]");
        var botaoAtualizar = $("input[id^=" + controle + "][id*=imgAtualizar]");
        var botaoCancelar = $("input[id^=" + controle + "][id*=imgCancelar]");

        var maximoVendasLabel = $("span[id^=" + controle + "][id*=lblMaximoVendas]");
        var maximoVendasText = $("input[id^=" + controle + "][id*=txtMaximoVendas]");

        var setoresLabel = $("span[id^=" + controle + "][id*=lblSetor]");
        var setoresText = $("input[id^=" + controle + "][id*=txtSetor]");

        var classificacoesLabel = $("span[id^=" + controle + "][id*=lblClassificacao]");
        var classificacoesText = $("input[id^=" + controle + "][id*=txtClassificacao]");

        var exibir = [botaoEditar, maximoVendasLabel, setoresLabel, classificacoesLabel];
        var editar = [botaoAtualizar, botaoCancelar, maximoVendasText, setoresText, classificacoesText];

        for (var i in exibir)
        {
            exibir[i].each(function()
            {
                if (exibirItensEditar)
                    $(this).hide();
                else
                    $(this).show();
            });
        }

        for (var i in editar)
        {
            editar[i].each(function()
            {
                if (exibirItensEditar)
                    $(this).show();
                else
                    $(this).hide();
            });
        }
    };

    return function(nomeControle)
    {
        var editando = false;

        this.Atualizar = function()
        {
            if (editando)
                salvar(nomeControle);
        };

        this.Editar = function()
        {
            editando = true;
            exibirEdicao(nomeControle, true);
        };

        this.Cancelar = function()
        {
            editando = false;
            exibirEdicao(nomeControle, false);
        };
    };
})(jQuery);