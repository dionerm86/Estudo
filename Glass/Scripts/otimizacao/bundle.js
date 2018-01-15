//#region Adiciona as referências de js e css
$("<link/>", {
    rel: "stylesheet",
    type: "text/css",
    href: "../../Style/Otimizacao/telerik.windows7.min.css"
}).appendTo("head");

$("<link/>", {
    rel: "stylesheet",
    type: "text/css",
    href: "../../Style/jquery/cupertino/jquery-ui-1.9.2.custom.min.css"
}).appendTo("head");

$("<link/>", {
    rel: "stylesheet",
    type: "text/css",
    href: "../../Style/Otimizacao/otimizacao.css"
}).appendTo("head");

$("<link/>", {
    rel: "stylesheet",
    type: "text/css",
    href: "../../Style/jquery/jquery.utils.css"
}).appendTo("head");

$("<link/>", {
    rel: "stylesheet",
    type: "text/css",
    href: "../../Scripts/jquery/jqwidgets/styles/jqx.base.css"
}).appendTo("head");

$("<link/>", {
    rel: "stylesheet",
    type: "text/css",
    href: "../../Scripts/jquery/jqwidgets/styles/jqx.darkblue.css"
}).appendTo("head");

$("<script/>", {
    type: "text/javascript",
    src: "../../Scripts/jquery/jquery-ui-1.10.2.js"
}).appendTo("head");

$("<script/>", {
    type: "text/javascript",
    src: "../../Scripts/jquery/jquery.contextmenu.r2.js"
}).appendTo("head");

$("<script/>", {
    type: "text/javascript",
    src: "../../Scripts/jquery/jquery-collision.js"
}).appendTo("head");

$("<script/>", {
    type: "text/javascript",
    src: "../../Scripts/jquery/jQueryRotate.2.2.js"
}).appendTo("head");

$("<script/>", {
    type: "text/javascript",
    src: "../../Scripts/jquery/jcanvas.js"
}).appendTo("head");

$("<script/>", {
    type: "text/javascript",
    src: "../../Scripts/jquery/jlinq/jlinq.js"
}).appendTo("head");

$("<script/>", {
    type: "text/javascript",
    src: "../../Scripts/jquery/jqwidgets/jqxcore.js"
}).appendTo("head");

$("<script/>", {
    type: "text/javascript",
    src: "../../Scripts/jquery/jqwidgets/jqxbuttons.js"
}).appendTo("head");

$("<script/>", {
    type: "text/javascript",
    src: "../../Scripts/jquery/jqwidgets/jqxscrollbar.js"
}).appendTo("head");

$("<script/>", {
    type: "text/javascript",
    src: "../../Scripts/jquery/jqwidgets/jqxlistbox.js"
}).appendTo("head");

$("<script/>", {
    type: "text/javascript",
    src: "../../Scripts/jquery/jqwidgets/jqxdragdrop.js"
}).appendTo("head");

$("<script/>", {
    type: "text/javascript",
    src: "../../Scripts/Grid.js"
}).appendTo("head");

$("<script/>", {
type: "text/javascript",
src: "../../Scripts/jquery/jquery.pajinate.js"
}).appendTo("head");

$("<script/>", {
    type: "text/javascript",
    src: "../../Scripts/otimizacao/html2canvas.js"
}).appendTo("head");
//#endregion

//#region Chama quando o documento estiver pronto
$(document).ready(function() {
    //#region Atribui funções
    $(function() {
        //Cria o accordion
        $("#accordion").accordion();
        //Altera a altura do accordion
        $(".ui-accordion-content").css("height", "100%");

        //Evento chamado quando se clica na listagem de informações do projeto
        $(".info-li").click(function() {
            //Recupera o id da li clicada
            var id = $(this).attr("id");

            //Recupera a div filha da li clicada
            var $div = $("#" + id + "_div");

            //Se a div estiver escondida, exibe e altera a cor da fonte da li
            if ($div.css("display") == "none") {
                $div.show();
                $(this).css("color", "red");
            }
            //Se não, esconde e altera a cor da fonte
            else {
                $div.hide();
                $(this).css("color", "");
            }
        });
    });

    //#region Cria o tooltip
    $(function() {
        $(document).tooltip({
            position: {
                my: "center top-40",
                at: "center top",
                using: function(position, feedback) {
                    $(this).css(position);
                    $("<div>")
                .addClass("arrow")
                .addClass(feedback.vertical)
                .addClass(feedback.horizontal)
                .appendTo(this);
                }
            }
        });
    });
    //#endregion

    //#region Evento click botão Otimizar
    $("#otimizarButton").click(function() {
        $(this).removeClass("t-button");
        bloquearPagina();
        iniciaOtimizacao();
        $("#hdfPecas").val("");
        criarLista("100%", "400px");
        $(this).addClass("t-button");
    });
    //#endregion

    //#region Evento click botão Limpar
    $("#limparButton").click(function() {
        $(this).removeClass("t-button");

        $('#tabs').empty();
        $('#error p').empty();
        $("#dadosOtimizacao").empty();
        
        $('#selecionaTodos').attr('checked', false);
        $(".chapaVidroCheckBox").attr('checked', false);

        var $info = $("#info");

        for (i = 0; i < $info.children().length; i++) {

            if ($info.children()[i].getAttribute("class") == "area")
                $("#" + $info.children()[i].id).remove()

            if ($info.children()[i].id == "areaChapas")
                $("#areaChapas").children().text("0,00 m");
        }
        $(this).addClass("t-button");
    });
    //#endregion

    //#region Evento que seleciona todos os check box da grid de chapas
    $("#selecionaTodos").click(function() {
        $(this).closest('table[id$=grdChapaVidro]').find(':checkbox').prop('checked', this.checked);
    });
    //#endregion
});

String.prototype.format = String.prototype.format = function() {
    var s = this,
        i = arguments.length;

    while (i--) {
        s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
    }
    return s;
};

//#region Método de criação da ListBox
function criarLista(w, h) {
    $('#pecasRemovidasListBox').jqxListBox('clear');
    $("#pecasRemovidasListBox").jqxListBox({ width: w, height: h, allowDrag: true });

    $('#pecasRemovidasListBox').on('change', function(event) {
        if (event.args) {
            var item = event.args.item;
            if (item) {
                //alert(item.label + " " + item.value);
            }
        }
    });
    $("#pecasRemovidasListBox").on('dragStart', function(event) {
        var args = event.args;
        var label = args.label;
        var value = args.value;
        var originalEvent = args.originalEvent;

        var $chapa = $("#tabs").find("li[style='display: list-item;']").find(".chapa");
    });
    $("#pecasRemovidasListBox").on('dragEnd', function(event) {
        if (event.args.label) {
            var args = event.args;
            var label = args.label;
            var value = args.value;
            var originalEvent = args.originalEvent;

            var ev = event.args.originalEvent;
            var x = ev.pageX;
            var y = ev.pageY;
            if (event.args.originalEvent && event.args.originalEvent.originalEvent && event.args.originalEvent.originalEvent.touches) {
                var touch = event.args.originalEvent.originalEvent.changedTouches[0];
                x = touch.pageX;
                y = touch.pageY;
            }

            var $chapa = $("#tabs").find("li[style='display: list-item;']").find(".chapa");

            var offset = $chapa.offset();
            var width = $chapa.width();
            var height = $chapa.height();
            var right = parseInt(offset.left) + width;
            var bottom = parseInt(offset.top) + height;
            if (x >= parseInt(offset.left) && x <= right) {
                if (y >= parseInt(offset.top) && y <= bottom) {

                    var $viewState = $("#ViewState");
                    var $lista = JSON.parse($viewState.val());

                    $.each($lista, function(index, peca) {
                        if ($chapa.attr("idProd") == peca.idProd && peca.idPeca == value) {

                            var $peca = $(atob(peca.html));

                            //Recupera altura e largura
                            var h = $peca.height();
                            var w = $peca.width();

                            //Recupera os atributos altura e largura reais da peça
                            var largura = $peca.attr("largura");
                            var altura = $peca.attr("altura");

                            var descricao = $peca.attr("descricao");

                            $peca.clearCanvas();

                            $peca.drawText({
                                fillStyle: "#000",
                                x: 55, y: 70,
                                font: "10pt Arial",
                                text: descricao,
                                fromCenter: false,
                                rotate: 0
                            });
                            $peca.saveCanvas();
                            $peca.restoreCanvas();

                            $peca.drawText({
                                fillStyle: "#000",
                                x: -5, y: 71,
                                font: "10pt Arial",
                                text: $peca.height() > $peca.width() ? largura : altura,
                                fromCenter: false,
                                rotate: -90
                            });
                            $peca.saveCanvas();
                            $peca.restoreCanvas();

                            $peca.drawText({
                                fillStyle: "#000",
                                x: 130, y: 7,
                                font: "10pt Arial",
                                text: $peca.height() > $peca.width() ? altura : largura,
                                fromCenter: false,
                                rotate: 0
                            });
                            $peca.saveCanvas();
                            $peca.restoreCanvas();

                            var relX = ev.pageX - $chapa.offset().left;
                            var relY = ev.pageY - $chapa.offset().top;

                            $peca.css("left", relX);
                            $peca.css("top", relY);

                            $chapa.append($peca);
                            removePecaLista($peca);
                            criaMenuContexto();
                            dragDrop($chapa);

                            verificaPosicionamentoPeca($peca);

                            $(".peca").removeClass("colisao");

                            var $colisao = $chapa.collision(".peca");

                            if ($colisao.length > 1) {
                                $colisao.each(function() {
                                    $peca.addClass("colisao");
                                });
                            }
                            else {
                                $(".peca").removeClass("colisao");
                            }
                        }
                    });
                }
                else {
                    alert("Verifique se a peça é compatível com a chapa.");
                    return false;
                }
            }
        }
    });
}
//#endregion

//#region Método de início da otimização
function iniciaOtimizacao() {
    var $areaTotalPecas = $(FindControl("areaTotalPecas", "input"));

    var $checkedRecords = $('input:checkbox[class=chapaVidroCheckBox ]:checked');

    var output = new Array();

    for (i = 0; i < $checkedRecords.toArray().length; i++) {

        var check = $checkedRecords[i];

        var chapa = check.value.split(";");
        var areaChapa = (parseFloat(chapa[1]) / 1000) * (parseFloat(chapa[2]) / 1000);
        var qtd = parseInt((parseFloat($areaTotalPecas.val()) / areaChapa) + 2);

        output.push(check.value + ";" + qtd + "|");
    }

    if (output.length < 1) {
        alert('Selecione uma chapa.'); // e informe a quantidade.');
        $('#tabs').empty();
        $('#error p').empty();
        $("#dadosOtimizacao").empty();
        desbloquearPagina(true);
        $("#otimizarButton").attr("disabled", false);

        return;
    }

    otimizar(output.join(), request()["pedidos"], request()["orcamentos"]);
}
//#endregion

//#region Exclui uma peça da lista
//Chamado a partir do menu de contexto. OBS: A peça é destruída, para recuperá-la só otimizando e novo
function removePeca(id) {
    addPecaLista(id);
    var $div = $("#" + id);
    $div.remove();
}
//#endregion

//#region Adiciona uma peça na ListBox
function addPecaLista(id) {
    var $div = $("#" + id);
    
    var $hdfPecas = $("#hdfPecas");
    var $lista = [];

    if ($hdfPecas.val() == "" || $hdfPecas.val() == "[]") {
        var $peca = new Object();
        $peca.id = $div.attr("idPeca");
        $peca.value = $div.attr("descricao") + " (" + $div.attr("caption") + ")";

        $lista = [$peca];

        $hdfPecas.val(JSON.stringify($lista));
    }
    else {
        $lista = JSON.parse($hdfPecas.val());

        $.each($lista, function(index, value) {

            if ($div.attr("idPeca") == value.id) {
                alert("Essa peça já foi adicionada.");
                return false;
            }
        });

        var $peca = new Object();
        $peca.id = $div.attr("idPeca");
        $peca.value = $div.attr("descricao") + " (" + $div.attr("caption") + ")";

        $lista.push($peca);

        $hdfPecas.val(JSON.stringify($lista));
    }

    $("#pecasRemovidasListBox").jqxListBox({ source: $lista, displayMember: "value", valueMember: "id", width: '100%', height: '400px', allowDrag: true });

    viewState(id, false);
}
//#endregion

//#region Remove uma peça da ListBox
function removePecaLista($peca) {
    var $hdfPecas = $("#hdfPecas");
    var $lista = JSON.parse($hdfPecas.val());

    $lista = jQuery.grep($lista, function(value) {
        return value.id != $peca.attr("idPeca");
    });

    $hdfPecas.val(JSON.stringify($lista));

    $("#pecasRemovidasListBox").jqxListBox({ source: $lista, displayMember: "value", valueMember: "id", width: '100%', height: '400px', allowDrag: true });

    viewState($peca.attr("id"), true);
}
//#endregion

//#region ViewState da ListBox e da Chapa
function viewState(id, remover) {
    var $div = $("#" + id);

    var $viewState = $("#ViewState");
    var $lista = [];

    if (!remover) {
        if ($viewState.val() == "" || $viewState.val() == "[]") {
            var $peca = new Object();
            $peca.idPeca = $div.attr("idPeca");
            $peca.idProd = $div.attr("idProd");
            $peca.html = btoa(document.getElementById(id).outerHTML);

            $lista = [$peca];
        }
        else {
            $lista = JSON.parse($viewState.val());

            $.each($lista, function(index, value) {
                if ($div.attr("idPeca") == value.id) {
                    alert("Essa peça já foi adicionada.");
                    return false;
                }
            });

            var $peca = new Object();
            $peca.idPeca = $div.attr("idPeca");
            $peca.idProd = $div.attr("idProd");
            $peca.html = $peca.html = btoa(document.getElementById(id).outerHTML);

            $lista.push($peca);
        }
    }
    else {
        $lista = JSON.parse($viewState.val());
        $lista = jQuery.grep($lista, function(value) {
            return value.idPeca != $div.attr("idPeca");
        });
    }

    $viewState.val(JSON.stringify($lista));
}

function dragDrop($chapa) {
    $("#" + $chapa.attr("id") + " img").draggable(
    {
        containment: $chapa,
        scroll: false,  //Define que não terá barra de rolagens
        cursor: "move", //Define o formato do cursor
        collide: "flag",
        obstacle: ".peca",
        preventCollision: true,
        opacity: 0.40,
        snap: true,
        snapMode: "outer",
        snapTolerance: 5,
        stack: ".peca",
        zIndex: 1,
        drag: function() {

            $(".peca").removeClass("colisao");

            var $colisao = $(this).collision(".peca");

            if ($colisao.length > 1) {
                $colisao.each(function() {
                    $(this).addClass("colisao");
                });
            }
            else
                $(".peca").removeClass("colisao");
        },
        stop: function() {
            $(".peca").removeClass("colisao");

            verificaPosicionamentoPeca($(this));

            var $colisao = $(this).collision(".peca");

            if ($colisao.length > 1) {
                $colisao.each(function() {
                    $(this).addClass("colisao");
                });
            }
            else
                $(".peca").removeClass("colisao");
        }
    });
}

//Verifica se a paça saiu da chapa
function verificaPosicionamentoPeca($peca) {

    //Recupera altura e largura da peça
    var pHeight = $peca.height();
    var pWidth = $peca.width();

    //Recupera posicionamento da peça
    var pTop = parseInt($peca.css("top").substring(0, $peca.css("top").length - 2));
    var pLeft = parseInt($peca.css("left").substring(0, $peca.css("left").length - 2));

    //Recupera altura e largura da chapa
    var cWidth = parseInt($peca.parent().css("width").substring(0, $peca.parent().css("width").length - 2));
    var cHeight = parseInt($peca.parent().css("height").substring(0, $peca.parent().css("height").length - 2));

    //Verifica se a peça esta rotacionada
    var rotacionada = $peca.css("transform") != "none";

    //Verifica se a paça saiu da chapa
    if (pTop + (rotacionada ? pWidth : pHeight) > cHeight)
        $peca.css("top", (cHeight - (rotacionada ? pWidth : pHeight) - 3) + "px");
    if (pLeft + (rotacionada ? pHeight : pWidth) > cWidth)
        $peca.css("left", (cWidth - (rotacionada ? pHeight : pWidth) - 3) + "px");
}

function criaMenuContexto() {
    $('.peca').contextMenu('myMenu1', {
        bindings: {
            'open': function(t)
            {
                alert('Trigger was ' + t.id + '\nAction was Open');
            },
            'rotate': function(t)
            {
                //Recupera a peça, ou seja, o canvas selecionado
                var $peca = $("#" + t.id);
                rotacionar($peca, true);
            },
            'remove': function(t)
            {
                removePeca(t.id);
            },
            'delete': function(t)
            {
                var $div = $("#" + t.id);
                if (confirm("Tem certeza que deseja excluir essa peça?"))
                {
                    $div.remove();
                }
            }
        }
    });
}

function rotacionar($peca, tela) {

    //Recupera altura e largura da peça
    var pHeight = $peca.height();
    var pWidth = $peca.width();
    
    //Recupera altura e largura da chapa
    var cWidth = parseInt($peca.parent().css("width").substring(0, $peca.parent().css("width").length - 2));
    var cHeight = parseInt($peca.parent().css("height").substring(0, $peca.parent().css("height").length - 2));

    //Verifica se pode retacionar
    if (tela && (pHeight > cWidth || pWidth > cHeight)) {
        alert("Essa peça não pode ser rotacionada.");
        return;
    }

    var rotate = $peca.css("transform") != "none" ? "none" : "rotate(-90deg) translate(-{0}px, -{0}px)".format((pWidth - pHeight) / 2);
    var margin = rotate != "none" ? "0 -{0}px {0}px 0".format(pWidth - pHeight) : "0px";
    
    $peca.css("transform", rotate);
    $peca.css("-ms-transform", rotate);
    $peca.css("-moz-transform", rotate);
    $peca.css("-webkit-transform", rotate);
    $peca.css("margin", margin);

    if (tela)
        verificaPosicionamentoPeca($peca);
}

function request() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

function otimizar(chapas, pedidos, orcamentos) {

    var inicio = new Date();
    
    $('#tabs').empty();
    $('#error p').empty();
    $("#dadosOtimizacao").empty();

    var existeVirgula = true;

    while (existeVirgula) {
        chapas = chapas.replace(",", "");
        existeVirgula = chapas.indexOf(",") > -1;
    }

    chapas = chapas.substring(0, chapas.lastIndexOf("|"));

    var params = "{'idchapas':'" + chapas + "', 'pedidos':'" + pedidos + "', 'orcamentos':'" + orcamentos + "'}";

    $.ajax({
        type: "POST",
        url: "../Service/OtimizacaoService.asmx/Otimizar",
        data: params,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(response) {
        
            var totalArea = 0;
            var totalUtilizado = 0;
            var totalDesperdicado = 0;
            var totalPecas = 0;
            var tempo = 0;
        
            if (response.d.Status == 1) {
                desbloquearPagina(true);
                $("#error").css("display", "block");
                $("#error p").append(response.d.Message);
                return false;
            }
            else if (response.d.Status == 0) {

                var $container = $("#tabs");
                $container.css("display", "block");

                var $altPageNavigation = $("<div class='alt_page_navigation'></div>");
                $container.append($altPageNavigation);

                //Cria a lista ul
                var $ul = $("<ul class='alt_content' style='list-style-type: none;'></ul>");

                //Adiciona a lista no tabStrip
                $container.append($ul);

                // List<Otimizacao>
                var Model = response.d.Object;

                //Percorre a lista dos objetos otimizados
                for (i = 0; i < Model.length; i++) {
                    var m = Model[i];
                    //Cria item da lista ul li
                    var $li = $("<li></li>");
                    $li.attr("id", "li-" + m.NumeroChapaStr);
                    $ul.append($li);

                    //********************** Cria a div que representará a chapa ****************************//
                    var $chapa = $("<div></div>");
                    //Atribui um id
                    $chapa.attr("id", "Chapa" + m.NumeroChapaStr);
                    $chapa.attr("idProd", m.IdProd);
                    $chapa.attr("caption", m.DescricaoChapa + " (" + parseFloat(m.LarguraChapaRealToString.replace(',', '.')).toFixed(2) + " x " + parseFloat(m.AlturaChapaRealToString.replace(',', '.')).toFixed(2) + ")");
                    //Atribui a classe chapa
                    $chapa.addClass("chapa");
                    //Atribui os itens de estilo. Se Altura for maior que largura, inverte os valores para renderizar
                    //a chapa sempre na horizontal
                    var wChapa = m.AlturaChapa > m.LarguraChapa ? m.AlturaChapa.toString().replace(',', '.') + "px" : m.LarguraChapa.toString().replace(',', '.') + "px";
                    var hChapa = m.AlturaChapa < m.LarguraChapa ? m.AlturaChapa.toString().replace(',', '.') + "px" : m.LarguraChapa.toString().replace(',', '.') + "px";

                    var styleChapa = "position:relative; background-color: #cccccc; border:solid 1px #cccccc; margin: 0px auto; width: {0}; height: {1}; z-index:0".format(wChapa, hChapa);
                    $chapa.attr("style", styleChapa);
                    //**************************************************************************************//

                    //********************** Cria a div que representará a peça ****************************//                        

                    //Recupera uma lista dos itens que têm o mesmo número da chapa do item corrente, usando linq para jquery
                    var lista = jLinq.from(m.PecasMapeadas)
                                .ignoreCase()
                                .equals("Chapa", m.NumeroChapa)
                                .select();

                    for (j = 0; j < lista.length; j++) {
                        var $peca = $("<canvas></canvas>");
                        $peca.attr("id", "peca" + (j + 1) + "_Chapa" + m.NumeroChapa);
                        $peca.addClass("peca");

                        var t = lista[j].PosicaoYPecaHTML.toString().replace(',', '.') + "px";
                        var l = lista[j].PosicaoXPecaHTML.toString().replace(',', '.') + "px";
                        var w = (lista[j].LarguraPecaHTML).toString().replace(',', '.') + "px";
                        var h = (lista[j].AlturaPecaHTML).toString().replace(',', '.') + "px";
                        var cor = lista[j].Cor;
                        var cliente = lista[j].NomeCliente;
                        var indice = lista[j].Indice;

                        var margin_top = "0";

                        var margin_left = "0";
                        
                        var vertical = lista[j].LarguraPecaHTML < lista[j].AlturaPecaHTML;
                        var maior = vertical ? h : w;
                        var menor = vertical ? w : h;

                        var stylePeca = 'text:align:center; z-index: 1; position:absolute; top:{0}; left: {1}; width: {2}; height: {3};border: dashed {4}px gray; margin-top:{5}; margin-left:{6};'.format(t, l, maior, menor, 2, margin_top, margin_left);
                        $peca.attr("style", stylePeca);
                        $peca.attr("caption", parseFloat(lista[j].LarguraPecaReal.toString().replace(',', '.')).toFixed(2) + " x " + parseFloat(lista[j].AlturaPecaReal.toString().replace(',', '.')).toFixed(2));
                        $peca.attr("altura", parseFloat(lista[j].AlturaPecaReal.toString().replace(',', '.')).toFixed(2));
                        $peca.attr("largura", parseFloat((lista[j].LarguraPecaReal).toString().replace(',', '.')).toFixed(2));
                        $peca.attr("descricao", (indice) + " / " + cliente);
                        $peca.attr("idProd", lista[j].IdProd);
                        $peca.attr("idPeca", "{0}{1}".format(lista[j].IdProd, indice));
                        $peca.attr("title", (indice) + " / " + cliente + " - " + $peca.attr("caption"));

                        var largura = $peca.attr("largura");
                        var altura = $peca.attr("altura");
                        var descricao = $peca.attr("descricao");

                        $peca.clearCanvas();
                        
                        if (altura < 300 || largura < 300) {
                            $peca.drawText({
                                fillStyle: "#000",
                                x: 150, y: 70,
                                font: "35pt Arial",
                                text: indice,
                                fromCenter: false
                            });

                        }
                        else {
                            $peca.drawText({
                                fillStyle: "#000",
                                x: 75, y: 70,
                                font: "25pt Arial",
                                text: descricao.toString().substring(0, 10),
                                fromCenter: false
                            });
                        
                        }
                        $peca.saveCanvas();
                        $peca.restoreCanvas();

                        if (altura < 300) {
                            $peca.drawText({
                                fillStyle: "#000",
                                x: -30, y: 71,
                                font: "35pt Arial",
                                text: vertical ? largura : altura,
                                fromCenter: false,
                                rotate: -90
                            });
                        }
                        else {
                            $peca.drawText({
                                fillStyle: "#000",
                                x: -30, y: 71,
                                font: "25pt Arial",
                                text: vertical ? largura : altura,
                                fromCenter: false,
                                rotate: -90
                            });
                        }
                        $peca.saveCanvas();
                        $peca.restoreCanvas();

                        if (largura < 300) {
                            $peca.drawText({
                                fillStyle: "#000",
                                x: 130, y: 7,
                                font: "30pt Arial",
                                text: vertical ? altura : largura,
                                fromCenter: false,
                                rotate: 0
                            });
                        }
                        else {
                            $peca.drawText({
                                fillStyle: "#000",
                                x: 130, y: 7,
                                font: "25pt Arial",
                                text: vertical ? altura : largura,
                                fromCenter: false,
                                rotate: 0
                            });
                        }
                        $peca.saveCanvas();
                        $peca.restoreCanvas();

                        var imagem = obterImagem($peca[0]);

                        $chapa.append(imagem);

                        rotacionar($(imagem), false);
                        if (vertical)
                            rotacionar($(imagem), false);
                    }
                    //**************************************************************************************//

                    //********************** Cria a tabela ****************************//
                    var $table = $('<table>');
                    $table.css("width", "100%");
                    $table.css("height", "100%");
                    $table.css("border-collapse", "collapse");

                    // thead
                    $table.append("<thead class='ui-widget-header'>").children('thead')
                        .append('<tr />').children('tr').append("<th colspan='2' style='border:solid 1px #cccccc;padding:7px'>" + m.DescricaoChapa + " (" + parseFloat(m.LarguraChapaRealToString.replace(',', '.')).toFixed(2) + " x " + parseFloat(m.AlturaChapaRealToString.replace(',', '.')).toFixed(2) + ")</th>");

                    var $tbody = $table.append('<tbody />').children('tbody');

                    var $tdChapa = $("<td style='vertical-align:top;border:solid 1px #cccccc;padding:5px; width:80%'></td>");
                    $tdChapa.append($chapa);

                    var $tdInfo = $("<td style='vertical-align:top;border:solid 1px #cccccc;padding:5px; width:20%'></td>");

                    var $tableInfo = $('<table>');
                    $tableInfo.css("width", "100%");
                    $tableInfo.css("border-collapse", "collapse");

                    var areaChapa = (parseFloat(m.LarguraChapaRealToString.replace(',', '.')) * parseFloat(m.AlturaChapaRealToString.replace(',', '.'))) / 1000000;

                    totalArea += areaChapa;
                    totalUtilizado += parseFloat(m.AreaUtilizada.toString().replace(',', '.'));
                    totalDesperdicado += parseFloat(m.AreaDesperdicada.toString().replace(',', '.'))
                    totalPecas += m.PecasMapeadas.length;
                    tempo += m.Tempo;

                    var percentualUtilizado = ((parseFloat(m.AreaUtilizada.toString().replace(',', '.')) * 100) / areaChapa).toFixed(2);
                    var percentualDesperdicado = ((parseFloat(m.AreaDesperdicada.toString().replace(',', '.')) * 100) / areaChapa).toFixed(2);

                    $tableInfo.append('<tbody />').children('tbody').append('<tr/>').children('tr:last').append("<td style='font-weight:bold;'>Área</td>");
                    $tableInfo.append('<tbody />').children('tbody').append('<tr/>').children('tr:last').append("<td>" + parseFloat(areaChapa.toString().replace(',', '.')).toFixed(2) + "m²</td>");

                    $tableInfo.children('tbody').append('<tr/>').children('tr:last').append("<td style='font-weight:bold;'>Utilizado</td>");
                    $tableInfo.children('tbody').append('<tr/>').children('tr:last').append("<td colspan='2'>" + parseFloat(m.AreaUtilizada.toString().replace(',', '.')).toFixed(2) + "m² (" + percentualUtilizado + "%)</td>");

                    $tableInfo.children('tbody').append('<tr/>').children('tr:last').append("<td style='font-weight:bold;'>Desperdiçado</td>");
                    $tableInfo.children('tbody').append('<tr/>').children('tr:last').append("<td colspan='2'>" + parseFloat(m.AreaDesperdicada.toString().replace(',', '.')).toFixed(2) + "m² (" + percentualDesperdicado + "%)</td>");

                    $tableInfo.children('tbody').append('<tr/>').children('tr:last').append("<td style='font-weight:bold;'>Qtde Peças</td>");
                    $tableInfo.children('tbody').append('<tr/>').children('tr:last').append("<td colspan='2'>" + m.PecasMapeadas.length + "</td>");

                    $chapa.attr("utilizado", ((parseFloat(m.AreaUtilizada.toString().replace(',', '.')) * 100) / areaChapa).toFixed(2));
                    $chapa.attr("perda", ((parseFloat(m.AreaDesperdicada.toString().replace(',', '.')) * 100) / areaChapa).toFixed(2));

                    $tdInfo.append($tableInfo);

                    $tbody.append('<tr/>').children('tr:last')
                        .append($tdInfo)
                        .append($tdChapa);

                    $li.append($table);

                    dragDrop($chapa);
                }

                $('#tabs').pajinate({
                    items_per_page: 1,
                    item_container_id: '.alt_content',
                    nav_panel_id: '.alt_page_navigation',
                    nav_label_first: 'Primeiro',
                    nav_label_last: 'Último',
                    nav_label_prev: 'Anterior',
                    nav_label_next: 'Próximo'
                });

                criaMenuContexto();
            }

            $("#accordion").accordion("option", "active", 2);

            $("#dadosOtimizacao").css("display", "");
            $("#tituloDadosOtimizacao").css("display", "");

            $tableDados = $("<table class='gridStyle' style='text-align:center; width:100%; margin: 5px 0 10px 0; border:solid 1px #ccc;padding:0'><thead><tr style='background-color:#ccc;'><th>Área</th><th>Qtde</th><th>Util.</th><th>Desp.</th></tr></thead>");

            var percentualTotalUtilizado = ((parseFloat(totalUtilizado.toString().replace(',', '.')) * 100) / totalArea).toFixed(2);
            var percentualTotalDesperdicado = ((parseFloat(totalDesperdicado.toString().replace(',', '.')) * 100) / totalArea).toFixed(2);

            $tableDados.append("<tbody />")
            .children('tbody')
            .append('<tr/>')
            .children('tr:last')
            .append("<td>" + totalArea.toFixed(2) + "m²</td><td>" + totalPecas + "</td><td>" + totalUtilizado.toFixed(2) + "m²<br />(" + percentualTotalUtilizado + "%)</td><td>" + totalDesperdicado.toFixed(2) + "m²<br />(" + percentualTotalDesperdicado + "%)</td>");

            $("#dadosOtimizacao").append($tableDados);

            var fim = new Date();

            var sp = getTimeSpan(fim - inicio);

            $("#dadosOtimizacao").append("<span style='font-weight:bold'>Tempo:</span> " + (sp.hour < 10 ? "0" + sp.hour : sp.hour) + ":" + (sp.minute < 10 ? "0" + sp.minute : sp.minute) + ":" + (sp.second < 10 ? "0" + sp.second : sp.second) + ":" + (sp.milisecond < 10 ? "0" + sp.milisecond : sp.milisecond));

            var $print = $('<a class="link-imprimir" onclick="return printPartOfPage(\'tabs\');" href="#" id="printButton" title="Imprimir" alt="Imprimir"></a>');
            $print.append('<img style="vertical-align:middle" src="../../Images/printer.png" border="0">');
            $print.append('<span style="margin-left:5px;vertical-align:middle">Imprimir</span>');

            $("#dadosOtimizacao").append($print);

            desbloquearPagina(true);
        },
        error: function(response) {
            desbloquearPagina(true);
            $("#error").css("display", "block");

            var erro = "";

            erro = response.responseText;

            $("#error p").append("Ocorreu um erro: <br />" + erro);
        }
    });
}

function getTimeSpan(ticks) {
    var d = new Date(ticks);
    return {
        hour: d.getUTCHours(),
        minute: d.getMinutes(),
        second: d.getSeconds(),
        milisecond: d.getMilliseconds()
    }
}

function printPartOfPage(elementId) {

    var $printContent = $("<div></div");
    var windowUrl = '';  //'about:blank';
    var uniqueName = new Date();

    var printWindow = openWindowRet(screen.height, screen.width, windowUrl);

    var $tabs = $("#" + elementId).clone();

    var $chapas = $tabs.find("div[class=chapa]")

    var count = 0;

    $chapas.each(function(index, item) {
        var $chapa = $(item);

        $chapa.children("img").css("background-color", "#f9f9f9");

        var $pagina = $("<div></div>");

        var $tabela = $("<table style='text-align:center; width:100%; height:auto; border:solid 1px #fff; padding:10px;'></table>");

        $tabela.append("<tbody />")
                    .children('tbody')
                    .append('<tr/>')
                    .children('tr:last')
                    .append(
                        "<td style='vertical-align:top;border-bottom:solid 1px #ccc;text-align:center;'>" + 
                        "<table style='text-align:center; font-family:Arial;font-size:12px; width:100%'><tr><td>Resumo de Otimização</td><td>" + obterFooterImpressao() + "</td></tr></table>" +
                        "</td>"
                    );

        $tabela.children('tbody')
                    .append('<tr/>')
                    .children('tr:last')
                    .append("<td style='padding-top:10px;vertical-align:top;font-family:Arial; font-size:12px'>" + $chapa.attr("caption") + " - Perda: " + $chapa.attr("perda") + "%</td>")

        $tabela.children('tbody')
                    .append('<tr/>')
                    .children('tr:last')
                    .append("<td style='vertical-align:top; padding-top:10px'></td>")
                    .children('td:last')
                    .append($chapa);

        var $tabelaPecas = $("<table style='margin-top:20px; text-align:left; width:100%; height:auto; font-family:Arial; font-size:10px; padding:1px; border:solid 1px #fff'><thead><tr style='background-color:#ccc;'><th>Índice</th><th>Cliente</th><th>Altura</th><th>Largura</th></tr></thead></table>");

        var $bodyPecas = $tabelaPecas.append('<tbody />').children('tbody');

        $chapa.children("img").each(function(index, item) {
            
            var $indice = $("<td style='padding:2px;'>" + $(item).attr("descricao").substring(0, $(item).attr("descricao").indexOf("/")) + "</td>");
            var $cliente = $("<td style='padding:2px;'>" + $(item).attr("descricao").substring($(item).attr("descricao").indexOf("/") + 1) + "</td>");
            var $altura = $("<td style='padding:2px;'>" + $(item).attr("altura") + "</td>");
            var $largura = $("<td style='padding:2px;'>" + $(item).attr("largura") + "</td>");

            $bodyPecas.append('<tr/>').children('tr:last')
                            .append($indice)
                            .append($cliente)
                            .append($altura)
                            .append($largura);
        });

        $tabela.children('tbody')
                    .append('<tr/>')
                    .children('tr:last')
                    .append("<td style='vertical-align:top;'></td>")
                    .children('td:last')
                    .append($tabelaPecas);

        $pagina.append($tabela);

        if (count < $chapas.length - 1)
            $pagina.css("page-break-after", "always");

        count++;
        $printContent.append($pagina);
    });

    printWindow.document.write($printContent[0].outerHTML);
    printWindow.document.close();
    printWindow.focus();
    printWindow.print();
    printcloseWindow();
}

function obterImagem(canvas) {
    var image = new Image();
    image.src = canvas.toDataURL("image/png");

    var $img = $(image);
    $img.attr("id", $(canvas).attr("id"));
    $img.attr("class", $(canvas).attr("class"));
    $img.attr("style", $(canvas).attr("style"));
    $img.attr("caption", $(canvas).attr("caption"));
    $img.attr("altura", $(canvas).attr("altura"));
    $img.attr("largura", $(canvas).attr("largura"));
    $img.attr("descricao", $(canvas).attr("descricao"));
    $img.attr("idprod", $(canvas).attr("idprod"));
    $img.attr("idpeca", $(canvas).attr("idpeca"));
    $img.attr("title", $(canvas).attr("title"));
    return image;
}

function obterFooterImpressao() {
    var footer = "";
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "../Service/OtimizacaoService.asmx/ObterFooterImpressao",
        dataType: "json",
        async: false,
        success: function(data) {
           footer = data.d;
        }
    });

    return footer;
}