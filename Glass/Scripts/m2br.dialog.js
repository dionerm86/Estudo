/*
* M2brDialog 
* - Substitui alertas e e janelas do sistema operacional
* plugin jQuery desenvolvido por Davi Ferreira (contato@daviferreira.com)
* collab Rômulo Alves (romulo@logmania.net)
* @version 2.1 2008-12-08
*
* Copyright (c) M2BRNET (http://www.m2brnet.com)
* Dual licensed under the MIT (MIT-LICENSE.txt)
* and GPL (GPL-LICENSE.txt) licenses.
*/

// variável para armazenar setTimeOut
var m2brTimer = '';

jQuery.fn.m2brDialog = function(opcoes) {

    // opções padrão
    var defaults = {
        largura: 300,
        altura: 120,
        tipo: 'alerta',
        titulo: '',
        texto: 'Alerta!',
        draggable: false,
        botoes: {
            1: {
                label: 'OK',
                tipo: 'fechar'
            }
        },
        tempoExibicao: 0,
        condicao: {
            origem: function() { return false; },
            retorno: function() { return false; }
        },
        loadCallback: null,
        unloadCallback: null
    };
    var opcoes = jQuery.extend(defaults, opcoes);

    // seta onclick do elemento para exibir janela
    jQuery(this).click(function() {

        // elimina outra instância de alerta
        fechaJanela(false);

        // verifica se tem condicao
        if (opcoes.condicao.origem() == true) {
            opcoes.condicao.retorno();
            return true;
        }

        // limpa qualquer timeout para evitar conflitos
        clearTimeout(m2brTimer);

        // oculta elementos select para evitar bug de sobreposição
        jQuery('select').each(function() {
            jQuery(this).attr("displayAtual", jQuery(this).css("display"));
            jQuery(this).hide();
        });

        // cria overlay
        jQuery(document.body).prepend('<div id="m2brOverlayFixed"><div id="m2brOverlay"></div></div>');

        // cria divs para janela e configura classe CSS
        jQuery(document.body).prepend('<div id="m2brDialogJanelaFixed"><div id="m2brDialogJanela"></div></div>');
        jQuery('#m2brDialogJanela')
			.append('<h2><a href="#" id="m2brDialogFechar"></a>' + opcoes.titulo + '</h2>')
			.append('<div id="m2brDialogJanela-texto">' + opcoes.texto + '</div>')
			.addClass('m2brDialog-' + opcoes.tipo);

        // configura clique do botão fechar
        jQuery('#m2brDialogFechar').click(fechaJanela);

        // drag and drop da janela, caso configurada
        if (opcoes.draggable == true && jQuery.isFunction(jQuery.fn.draggable)) {
            jQuery('#m2brDialogJanela').draggable({ handle: 'h2' });
            jQuery('#m2brDialogJanela h2').css('cursor', 'move');
        }

        // animação para exibir overlay e janela
        jQuery('#m2brOverlay').fadeIn(150, function() {
            // hack de opacity para IE
            if (jQuery.browser.msie == true && jQuery.browser.version < 8) {
                jQuery('#m2brOverlay').css('opacity', 0.65);
            }
            // exibe janela
            jQuery('#m2brDialogJanela').fadeIn(200, function() {
                // configura onclick do overlay para fechar a janela
                jQuery('#m2brOverlay').click(fechaJanela);
            });
        });

        // configura tamanho e margem do dialog
        jQuery('#m2brDialogJanela').css({
            height: opcoes.altura + 'px',
            width: opcoes.largura + 'px',
            marginLeft: '-' + (opcoes.largura / 2) + 'px',
            marginTop: '-' + (opcoes.altura / 2) + 'px'
        });

        // hack para IE6 já que não aceita fixed
        if (jQuery.browser.msie == true && jQuery.browser.version < 7) {
            jQuery('#m2brOverlay').css('top', jQuery(window).scrollTop());
            jQuery('#m2brDialogJanela').css('top', jQuery(window).scrollTop() + (jQuery(window).height() / 2) - (opcoes.altura / 2));
        }

        // cria botões
        jQuery('#m2brDialogJanela').append('<div id="m2brDialogJanela-botoes"></div>');
        for (x in opcoes.botoes) {
            // cria HTML do botão
            jQuery('#m2brDialogJanela-botoes').append('<a href="#" id="m2brDialog-botao-' + x + '" class="m2brDialogBotao">' + opcoes.botoes[x].label + '</a>');
            // ação - fechar janela
            if (opcoes.botoes[x].tipo == 'fechar') {
                jQuery('#m2brDialog-botao-' + x).click(fechaJanela);
                // ação - redirecionamento para link
            } else if (opcoes.botoes[x].tipo == 'link') {
                jQuery('#m2brDialog-botao-' + x).attr('href', opcoes.botoes[x].endereco);
                // ação - função javascript
            } else if (opcoes.botoes[x].tipo == 'script') {
                jQuery('#m2brDialog-botao-' + x).click(opcoes.botoes[x].funcao);
                jQuery('#m2brDialog-botao-' + x).click(fechaJanela);
            }
        } // fim for

        // impede múltiplos cliques no mesmo botão
        jQuery('#m2brDialogJanela-botoes a').click(function() {
            jQuery('#m2brDialogFechar').hide();
            jQuery('#m2brOverlay').unbind('click');
            jQuery('#m2brDialogJanela-botoes').html('<div class="carregando"></a>');
        });

        // define timeOut
        if (!isNaN(opcoes.tempoExibicao) && opcoes.tempoExibicao > 0) { m2brTimer = setTimeout(fechaJanela, opcoes.tempoExibicao * 1000); }

        if (opcoes.loadCallback) {
            if (typeof opcoes.loadCallback == "string")
                window[opcoes.loadCallback].call(this, opcoes);
                
            else if (typeof opcoes.loadCallback == "function")
                opcoes.loadCallback.call(this, opcoes);
        }

        return false;

    }); // fim this.click

    // fecha janela
    var fechaJanela = function(chamarCallback) {
        // remove binds
        jQuery('#m2brOverlay').unbind('click');
        // limpa timeout
        clearTimeout(m2brTimer);
        // remove overlay + janela
        jQuery('#m2brOverlayFixed').remove();
        jQuery('#m2brDialogJanelaFixed').remove();

        // exibe selects
        jQuery('select').each(function() {
            var display = jQuery(this).attr("displayAtual");
            jQuery(this).css("display", display);
        });

        chamarCallback = chamarCallback == false ? false : true;

        if (chamarCallback && typeof opcoes.unloadCallback == 'function')
            opcoes.unloadCallback.call(this, opcoes);
        
        return false;
    }; // fim fechaJanela

};                   // fim fn