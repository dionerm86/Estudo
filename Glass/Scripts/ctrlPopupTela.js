/**
 * Tipo para o controle de popup. Mantém as funções para o uso do controle agrupadas no objeto.
 * @param nomeControle O nome do controle que está sendo representado.
 * @param exibirFechar O botão fechar deve ser exibido?
 * @param tipoExibicao O tipo de exibição do controle.
 * @param baseUrl O caminho relativo até a pasta raiz do site.
 */
PopupTelaType = function(nomeControle, exibirFechar, tipoExibicao, baseUrl)
{
    var itemAberto = [];
    var controle = nomeControle;
    var fechar = !exibirFechar ? false : true;
    var tipoExibicaoControle = !!tipoExibicao ? tipoExibicao : 0;
    var fechando = false;
    var rootSite = baseUrl;

    // Tipo para a variável urlAberta
    ItemAbertoType = function(url, posicao)
    {
        this.Posicao = posicao;
        this.URL = url;
        this.Carregou = false;

        return this;
    };

    /**
     * Altera o popup pai, se houver.
     * @param exibir O popup será exibido? (caso contrário será escondido)
     */
    var alterarPopupPai = function(exibir)
    {
        // Altera o popup pai, se necessário
        if (window != window.opener)
        {
            // Altera o botão "Fechar"
            var fechar = FindControl(controle + "_fechar", "div", window.opener.document);
            if (!!fechar) fechar.style.display = exibir ? "table-row" : "none";

            // Altera os iframes do controle
            var i = 0, item = null;
            while (!!(item = FindControl(controle + "_iframe_item" + i, "iframe", window.opener.document)))
            {
                item.style.overflow = exibir ? "" : "hidden";
                item.scrolling = exibir ? "" : "no";
                item.contentDocument.getElementById("tabelaPrincipal").style.display = exibir ? "" : "none";
                i++;
            }

            // Altera as bordas dos DIVs principais
            var i = 0, item = null;
            while (!!(item = FindControl(controle + "_item" + i, "div", window.opener.document)))
            {
                item.style.borderStyle = exibir ? "solid" : "none";
                i++;
            }
        }
    };

    /**
     * Exibe o popup com animações.
     */
    var exibirPopup = function()
    {
        var p = document.getElementById(controle + "_blanket");
        $(p).fadeIn();
    };

    /**
     * Exibe o popup com animações.
     */
    var esconderPopup = function()
    {
        var p = document.getElementById(controle + "_blanket");
        $(p).fadeOut();
    };

    // Funções para controles de popup de janela
    if (tipoExibicaoControle == 1)
    {
        /**
         * Verifica se uma URL foi carregada.
         * @param posicao A posição da janela que será verificada (pode ser vazia para verificar a última).
         */
        this.CarregouUrl = function(posicao)
        {
            if (itemAberto.length == 0)
                return false;

            // Normaliza a variável com a posição
            posicao = !!posicao ? posicao : itemAberto[itemAberto.length - 1].Posicao;

            // Verifica se a URL foi carregada
            for (var u in itemAberto)
            {
                if (itemAberto[u].Posicao == posicao)
                    return itemAberto[u].Carregou;
            }

            // Não encontrou a URL indicada
            return false;
        }

        /**
         * Retorna o documento aberto.
         * @param posicao A posição da página que foi aberta (pode ser vazia para retornar a última).
         * @param retornarSemCarregar A página pode ser retornada sem que esteja carregada?
         */
        this.Pagina = function(posicao, retornarSemCarregar)
        {
            // Normaliza a variável com a posição
            posicao = !!posicao ? posicao : itemAberto[itemAberto.length - 1].Posicao;

            if (!retornarSemCarregar && !this.CarregouUrl(posicao))
                return null;

            // Retorna a window do item aberto
            return document.getElementById(controle + "_iframe_item" + posicao).contentWindow;
        }

        /**
         * Fecha o popup se a tela for fechada.
         * @param event O evento ocorrido (jQuery.event).
         */
        var fechaPopupBeforeUnload = function(event)
        {
            event.data.Popup.Fechar();
        };

        /**
         * Remove o evento que fecha o popup ao fechar a janela
         * se ele for ocorrer por conta de um submit de formulário.
         * @param event O evento ocorrido (jQuery.event).
         */
        var naoFechaPopupSubmit = function(event)
        {
            $(event.data.Janela).off("beforeunload", fechaPopupBeforeUnload);
        };

        /**
         * Indica o carregamento de uma página para o controle.
         * @param posicao A posição do item no controle interno.
         */
        this.IndicaCarregamento = function(posicao)
        {
            // Verifica se a URL foi carregada
            for (var u in itemAberto)
            {
                if (itemAberto[u].Posicao == posicao)
                {
                    itemAberto[u].Carregou = true;
                    break;
                }
            }

            // Exibe a página e esconde o ícone
            document.getElementById(controle + "_iframe_container_item" + posicao).style.display = "table-row";
            document.getElementById(controle + "_load_item" + posicao).style.display = "none";

            var iframe = document.getElementById(controle + "_iframe_item" + posicao);

            if (!iframe.contentWindow.onload)
            {
                // Faz com que a função window.close() feche o popup
                $(iframe.contentWindow).on("beforeunload", $(iframe.contentWindow).prop("onbeforeunload"));
                $(iframe.contentWindow).on("beforeunload", { Popup: this }, fechaPopupBeforeUnload);

                // Desabilita o evento onbeforeunload para os submit dos formulários
                for (var f = 0; f < iframe.contentDocument.forms.length; f++)
                {
                    $(iframe.contentDocument.forms[f]).on("submit", $(iframe.contentDocument.forms[f]).prop("onsubmit"));
                    $(iframe.contentDocument.forms[f]).on("submit", { Janela: iframe.contentWindow }, naoFechaPopupSubmit);
                    
                    if (iframe.contentWindow["__doPostBack"])
                        iframe.contentDocument.forms[f].onsubmit = null;
                }
            }
        }
    }

    /**
     * Fecha um popup aberto.
     * @param posicao O número do item que será fechado (pode ser null para fechar o último).
     */
    this.Fechar = function(posicao)
    {
        if (fechando || itemAberto.length == 0) return;
        fechando = true;

        // Exibe novamente o popup pai, se necessário
        alterarPopupPai(true);

        try
        {
            // Normaliza a variável com a posição
            posicao = !!posicao ? posicao : itemAberto[itemAberto.length - 1].Posicao;

            // Esconde o popup
            esconderPopup();

            // Fecha a janela interna
            if (tipoExibicaoControle == 1)
            {
                var pagina = this.Pagina(posicao);

                try
                {
                    $(pagina).off("beforeunload", fechaPopupBeforeUnload);
                    pagina.close();
                }
                catch (err)
                {
                    try
                    {
                        // Tratamento de erro: Firefox não permite o fechamento da tela
                        // do IFrame a partir da página principal
                        netscape.security.PrivilegeManager.enablePrivilege("UniversalBrowserWrite");
                        pagina.close();
                    }
                    catch (err1) { }
                }

                // Limpa o texto interno, fechando a tela
                document.getElementById(controle + "_texto").innerHTML = "";
            }

            // Remove a referência do popup na lista de itens abertos
            itemAberto.pop();
        }
        finally
        {
            fechando = false;
        }
    };

    /**
     * Formata uma URL para uso no controle, adicionando o parâmetro "popup".
     * @param url A URL que será formatada.
     */
    var preparaUrl = function(url)
    {
        var adicionar = "popup=true&exibirCabecalhoPopup=true&controlePopup=true";

        var partes = url.split('?');
        if (partes.length == 1)
            return url + "?" + adicionar;

        if (partes[1].indexOf("postData=getPostData()") > -1)
            return url;

        var parametros = partes[1].split('&');
        var novos = [];

        for (var i = 0; i < parametros.length; i++)
        {
            if (parametros[i].split('=')[0].toLowerCase() != "popup")
                novos.push(parametros[i]);
        }

        novos.push(adicionar);
        url = partes[0] + "?" + novos.join('&');

        // Substitui os caracteres ' e " por &quot;
        return url.split("'").join("&quot;").split('"').join("&quot;");
    };

    /**
     * Abre um popup.
     * @param url A URL que será exibida. (Pode ser vazia para utilizar o caminho padrão)
     * @param altura O tamanho vertical da janela.
     * @param largura O tamanho horizontal da janela.
     * @param exibirBotaoFechar O botão "Fechar" será exibido?
     */
    var abrirUrl = function(url, altura, largura, exibirBotaoFechar)
    {
        // Fecha todos os popups abertos
        while (itemAberto.length > 0)
            this.Fechar(itemAberto.pop().Posicao);

        // Exibe o popup
        exibirPopup();

        // Carrega a tela
        var url = !!url ? url : document.getElementById(controle + "_hdfInnerHTML").value;
        url = preparaUrl(url);

        var posicao = itemAberto.length;
        exibirBotaoFechar = exibirBotaoFechar !== undefined && exibirBotaoFechar != null ? exibirBotaoFechar : fechar;

        document.getElementById(controle + "_texto").innerHTML +=
                "<div id='" + controle + "_item" + posicao + "' style='display: inline-block; border: 1px solid #ddd; -webkit-border-radius: 5px; " +
                    "-mz-border-radius: 5px; border-radius: 5px; width: " + largura + "px; height: " + altura + "px; background-color: white; overflow: auto'>" +
                    "<div style='display: table; width: 100%; height: 100%'>" +

                        (exibirBotaoFechar ? "<div id='" + controle + "_fechar' style='display: table-row; text-align: right'>" +
                            "<div style='margin: 8px'>" +
                                "<a href='#' onclick='" + controle + ".Fechar(" + posicao + "); return false' style='text-decoration: none'>" +
                                    "<img src='../images/excluirGrid.gif' border='0' style='position: relative; top: 2px' /> " +
                                    "Fechar" +
                                "</a>" +
                            "</div>" +
                        "</div>" : "") +

                        "<div id='" + controle + "_load_item" + posicao + "' style='display: table-cell; height: 100%; vertical-align: middle'>" +
                            "<img src='" + rootSite + "images/load.gif' style='height: 32px' />" +
                        "</div>" +
                        "<div id='" + controle + "_iframe_container_item" + posicao + "' style='display: none; height: 100%'>" +
                            "<iframe id='" + controle + "_iframe_item" + posicao + "' frameborder='0' style='width: 100%; height: 100%' " +
                                "src='" + url + "' onload='" + controle + ".IndicaCarregamento(" + posicao + ")'></iframe>" +
                        "</div>" +
                    "</div>" +
                "</div>";

        // Salva o popup na lista de itens abertos
        itemAberto.push(new ItemAbertoType(url, posicao));

        // Esconde o popup pai, se necessário
        alterarPopupPai(false);
    };

    /**
     * Abre um popup.
     */
    var abrirHtml = function()
    {
        // Fecha todos os popups abertos
        while (itemAberto.length > 0)
            this.Fechar(itemAberto.pop().Posicao);

        // Exibe o popup
        exibirPopup();

        // Salva o popup na lista de itens abertos
        itemAberto.push(controle);
    };

    /**
     * Abre um popup.
     */
    this.Abrir = tipoExibicaoControle == 0 ? abrirHtml : abrirUrl;
};