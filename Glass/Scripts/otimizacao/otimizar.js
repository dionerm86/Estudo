$(document).ready(function() {
    $("#otimizarButton").click(function() {
        $("#otimizarButton").attr("disabled", true);
        iniciaOtimizacao();
        //otimizarTeste();
    });
});

function iniciaOtimizacao() {
    var $areaTotalChapa = $(FindControl("areaTotalChapa", "input"));
    var $areaTotalPecas = $(FindControl("areaTotalPecas", "input"));

    var $checkedRecords = $('input:checkbox[class=chapaVidroCheckBox ]:checked');

    var $qtdeTextBox = $('.qtdeTextBox');

    var output = new Array();

    for (i = 0; i < $checkedRecords.toArray().length; i++) {

        var check = $checkedRecords[i];
        var text = "";

        $("input[type=text]").each(function() {
            if ($(this).attr("identidade") == check.value.split(";")[0] + "_TextBox") {
                text = $(this);
            }
        });

        if (text.val() == "") {
            desbloquearPagina(true);
            $("#otimizarButton").attr("disabled", false);
            alert("Preencha a quantidade de chapas que deverá ser usada");
            text.css("background-color", "#CDCDCD");
            //text.css("border", "solid 1px #FF0000");
            text.focus();
            return;
        }
        else {
            output.push(check.value + ";" + text.val() + "|");
        }
    }

    if (output.length < 1) {
        alert('Selecione uma chapa e informe a quantidade.');
        $('#result').empty();
        desbloquearPagina(true);
        $("#otimizarButton").attr("disabled", false);

        return;
    }

    if (parseFloat($areaTotalChapa.val()) < parseFloat($areaTotalPecas.val())) {
        alert("A área total das chapas não é o bastante para otimizar");
        $('#result').empty();
        desbloquearPagina(true);
        $("#otimizarButton").attr("disabled", false);
        return;
    }

    var data = { chapas: output, pedidos: '<%= Request["pedidos"] %>' };

    //$('#result').load('PecasOtimizadas.ascx', $.param(data, true));

    $('#result').load('PecasOtimizadas.ascx', $.param(data, true),
                function(response, status, xhr) {
                    /*$('#result').empty();
                    if (xhr.responseText.indexOf('{') == 0) {
                    var obj = $.parseJSON(response);
                        
                    if (obj.error == true) {
                    alert(obj.message);
                    }
                        
                    $('#result').html("<p style='color:#FF0000; font-size:16px; padding-left:20px; background-color:#FFFF00'>" + obj.message + "</p>");
                    }
                    else {
                    $('#result').html(response);
                    }*/

                    desbloquearPagina(true);

                    $("#otimizarButton").attr("disabled", false);
                });

}

function atualizarAreaTotalChapa() {

    var $areaTotalChapa = $(FindControl("areaTotalChapa", "input"));
    var $areaTotalPecas = $(FindControl("areaTotalPecas", "input"));

    var $checkedRecords = $('input:checkbox[class=chapaVidroCheckBox ]:checked');

    var $qtdeTextBox = $('.qtdeTextBox');

    var area = 0;

    for (i = 0; i < $checkedRecords.toArray().length; i++) {

        var check = $checkedRecords[i];

        var chapa = check.value.split(";");

        var $text = "";
        var input = "";

        $("input[type=text]").each(function() {
            if ($(this).attr("identidade") == check.value.split(";")[0] + "_TextBox") {
                $text = $(this);
                input = $(this);
            }
        });

        if (input.val() != "") {

            area += (chapa[1] * chapa[2]) * input.val();
        }
    }

    $areaTotalChapa.val(area);

    if (parseFloat($areaTotalChapa.val()) < parseFloat($areaTotalPecas.val())) {
        $('#result').html("<p style='color:#FF0000; font-size:16px; padding-left:20px; background-color:#FFFF00'>A área total das chapas não é o bastante para otimizar</p>");
    }
    else {
        $("#result").empty();
    }
}

function atualizarAreaChapa(text, dadosChapa) {
    var $areaTotalChapa = $(FindControl("areaTotalChapa", "input"));
    var $areaTotalPecas = $(FindControl("areaTotalPecas", "input"));

    var chapa = dadosChapa.split(";");

    var area = (chapa[1] * chapa[2]) * text.value;

    var liIds = $('#info li').map(function(i, n) {
        return $(n).attr('id');
    }).get().join(',');

    $("#areaChapas").remove();

    if (liIds.indexOf("chapa" + chapa[0]) >= 0) {
        $("#" + "chapa" + chapa[0]).remove();

        if (!isNaN(area)) {
            $("#info").append("<li id=chapa" + chapa[0] + " class='area'>Área total de " + text.value + " chapa(s) " + chapa[4] + " selecionada(s): <span style='font-weight:bold;'>" + formatDecimal((area / 1000).toFixed(2).toString()) + " m</span></li>");
        }
    }
    else {
        $("#info").append("<li id=chapa" + chapa[0] + " class='area'>Área total de " + text.value + " chapa(s) " + chapa[4] + " selecionada(s): <span style='font-weight:bold;'>" + formatDecimal((area / 1000).toFixed(2).toString()) + " m</span></li>");
    }

    atualizarAreaTotalChapa();

    $("#info").append("<li class='area' id='areaChapas'>Área total das chapas selecionada: <span style='font-weight:bold;'>" + formatDecimal((parseFloat($areaTotalChapa.val()) / 1000).toFixed(2).toString()) + " m</span></li>");

}

function qtdeTextBox_Blur(text, dadosChapa) {
    if (text.value != "") {

        $(text).css("background-color", "#ffffff");
        //$("#" + text.id).css("border", "");

        atualizarAreaChapa(text, dadosChapa);
    }
    else {
        $(text).css("background-color", "#CDCDCD");
        //$("#" + text.id).css("border", "solid 1px #FF0000");
    }
}

function habilitaTexto(check) {

    var id = check.value.split(";")[0];
    var $text = "";
    var input = "";

    $("input[type=text]").each(function() {
        if ($(this).attr("identidade") == id + "_TextBox") {
            $text = $(this);
            input = $(this);
        }
    });

    if (check.checked == true) {
        $text.attr("disabled", false);
    }
    else {
        $text.attr("value", "");
        $text.css("background-color", "");
        $text.attr("disabled", true);

        atualizarAreaChapa(input, check.value);
        $("#result").empty();
    }
}

function formatDecimal(nStr) {
    nStr += '';
    var x = nStr.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? ',' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + '.' + '$2');
    }
    return x1 + x2;
}

function limpar() {
    $('#result').empty();
    $(".chapaVidroCheckBox").attr('checked', false);
    $(".qtdeTextBox").attr("value", " ");
    $(".qtdeTextBox").attr("disabled", true);
    $("#areaTotalChapa").val("0");

    var $info = $("#info");

    for (i = 0; i < $info.children().length; i++) {

        if ($info.children()[i].getAttribute("class") == "area")
            $("#" + $info.children()[i].id).remove()

        if ($info.children()[i].id == "areaChapas")
            $("#areaChapas").children().text("0,00 m");
    }
}