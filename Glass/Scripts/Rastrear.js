var mapa = null;
var pontos = null;
var titulos = null;
var textos = null;
var marcadores = null; // Vetor que armazenas todas as informações dos marcadores
var dtUltPonto = null;
var selectedPoint = null;
var secToRefresh = 90; // Intervalo de tempo em segundos que ocorrerá atualização da posição das equipes

google.load("maps", "2");

// Pega o tipo do navegador
var ie5 = (document.getElementById && document.all);

// Associa um evento
function _addHandler(comp, descr, func) {
    if (comp.addEventListener)
        comp.addEventListener(descr, func, false);
    else if (comp.attachEvent)
        comp.attachEvent('on' + descr, func);
}

function iniciar() {
    if (GBrowserIsCompatible()) {
        mapa = new GMap2(document.getElementById("mapa"));

        mapa.setCenter(new GLatLng(0, 0), 5);
        mapa.enableScrollWheelZoom();
        mapa.addControl(new GLargeMapControl());
        mapa.addControl(new GMapTypeControl());
        mapa.addControl(new GOverviewMapControl());

        // Atualiza posição das equipes em intervalos de 'secToRefresh' segundos
        setInterval("mudarMarcadores();", secToRefresh * 1000);

        mudarMarcadores();
    }
}

function mudarMarcadores() {
    var refresh = (new Date()).getMilliseconds().toString();

    limparMarcadores();

    GDownloadUrl("../Handlers/XmlRastreamento.ashx?refresh=" + refresh, function(data) {
        var xml = GXml.parse(data);
        var markers = xml.getElementsByTagName("marker");

        if (markers.length == 0)
            return;

        // Inicializa vetor de objetos de marcadores
        marcadores = new Array(markers.length);

        for (var i = 0; i < markers.length; i++) {
            // Cria um novo marcador, verificando se o marcador é uma equipe ou uma instalação
            mark = criaMarcador(markers[i], i, markers[i].getAttribute("tipo"));

            // Verifica se a latitude deste marcador é válida antes de adcioná-lo no mapa
            lat = markers[i].getAttribute("lat");
            if (lat != "" && lat != null && lat != "0")
                mapa.addOverlay(mark);
        }

        // Adiciona instalações de todas as equipes na lista
        listaInstalacao(0);

        posicionar();
    });
}

// Cria um marcador no mapa
// dadosXml: Xml contendo informações do ponto a partir do qual será criado um marcador
// posicao: posição do vetor de equipes para guardar esta equipe
// tipo: E-Equipe I-Instalação
function criaMarcador(dadosXml, posicao, tipo) {
    var lat = parseFloat(dadosXml.getAttribute("lat"));
    var lng = parseFloat(dadosXml.getAttribute("lng"));

    marcadores[posicao] = new Object();
    marcadores[posicao].ponto = new GLatLng(lat, lng);
    marcadores[posicao].titulo = dadosXml.getAttribute("titulo");
    marcadores[posicao].texto = dadosXml.getAttribute("texto");
    marcadores[posicao].status = dadosXml.getAttribute("status");
    marcadores[posicao].dtUltPonto = dadosXml.getAttribute("dtUltPonto");
    marcadores[posicao].idEquipe = dadosXml.getAttribute("idEquipe");
    
    var marker = null;

    // Se for ponto de Instalação
    if (tipo == "I") {
        marker = new GMarker(marcadores[posicao].ponto, { "icon": iconeInstalacao() });

        marcadores[posicao].idPedido = dadosXml.getAttribute("idPedido");
        marcadores[posicao].tempo = dadosXml.getAttribute("tempo");
        marcadores[posicao].situacao = dadosXml.getAttribute("situacao");

        // Se a instalação estiver concluída, cria evento para este ponto (referente à alguma instalação) que irá abrir informações sobre a mesma
        if (dadosXml.getAttribute("situacao") == "C") {            
            GEvent.addListener(marker, "click", function() {
                var conteudo = "<span style=\"font-weight: bold\">" + marcadores[posicao].titulo + "</span><br />" + marcadores[posicao].texto;
                marker.openInfoWindowHtml(conteudo);
            });
        }

        marcadores[posicao].marker = marker;

        return marker;
    }
    // Se for ponto de Equipe
    else if (tipo == "E") {
        marker = new GMarker(marcadores[posicao].ponto, { "icon": iconeEquipe(posicao) });

        marcadores[posicao].marker = marker;

        // Adiciona Equipe à listagem
        addEquipe(lat, lng, marcadores[posicao], posicao);
        
        // Mostra rota percorrida
        var pontosEquipe = dadosXml.getAttribute("pontosEquipe");
        if (pontosEquipe != null) {
            pontosEquipe = pontosEquipe.split('|');
            var ptsEquipe = new Array(pontosEquipe.length);
            
            // Preenche vetor com coordendas do google GLatLng
            for (var i = 0; i < pontosEquipe.length; i++) {
                var lat = parseFloat(pontosEquipe[i].split(';')[0]);
                var lng = parseFloat(pontosEquipe[i].split(';')[1]);
                ptsEquipe[i] = new GLatLng(lat, lng);
            }

            mapa.addOverlay(new GPolyline(ptsEquipe, "#FF0000"));
        }
        
        // Cria evento para este ponto (referente à alguma equipe) que irá abrir informações sobre a mesma
        GEvent.addListener(marker, "click", function() {
            var conteudo = "<span style=\"font-weight: bold\">" + marcadores[posicao].titulo + "</span><br />" + marcadores[posicao].texto;
            marker.openInfoWindowHtml(conteudo);
        });
    }
    
    return marker;
}

// Adiciona item à lista de equipes
function addEquipe(lat, lng, mark, posicao) {
    // Busca a tabela que receberá a lista de equipes que estiverem sendo monitoradas
    var tbSpd = document.getElementById("MapMarks");

    // Cria a linha referente à equipe que será inserida na tabela acima
    var tr = document.createElement("tr");
    //tr.style.backgroundColor = "#CCFFCC";	
    tr.setAttribute("lat", lat);
    tr.setAttribute("lng", lng);
    tr.setAttribute("idEquipe", mark.idEquipe);
    tr.setAttribute("pos", posicao);
    tr.style.cursor = "pointer";

    // Adiciona uma célula à linha para dar margem
    td = document.createElement("td");
    td.width = "5px";
    tr.appendChild(td);

    // Adiciona uma célula à linha para identificar se a equipe está ativa ou não
    td = document.createElement("td");
    td.innerHTML = mark.status;
    tr.appendChild(td);

    // Adiciona evento ao item da lista para mudar cor de fundo quando o mouse estiver sobre o mesmo
    _addHandler(tr, "mouseover", function(e) {
        var obj = (ie5 ? e.srcElement.parentElement : this);

        if (obj == selectedPoint) return;
        obj.style.backgroundColor = "#FFFFFF";
    });

    // Adiciona evento ao item da lista para mudar cor de fundo quando o mouse sair de foco do mesmo		
    _addHandler(tr, "mouseout", function(e) {
        var obj = (ie5 ? e.srcElement.parentElement : this);

        if (obj == selectedPoint) return;
        obj.style.backgroundColor = "";
    });

    // Adiciona evento click ao item da lista que irá mudar a posição do mapa para a posição da equipe clicada
    _addHandler(tr, "click", function(e) {
        var obj = (ie5 ? e.srcElement.parentElement : this);

        if (obj == selectedPoint)
            return;
        else if (selectedPoint != null)
            selectedPoint.style.backgroundColor = "";

        if (obj.getAttribute("lat") != null && obj.getAttribute("lat") != "" && obj.getAttribute("lat") != "0") {

            // Centraliza o mapa nesta equipe clicada
            mapa.panTo(new GLatLng(obj.getAttribute("lat"), obj.getAttribute("lng")));

            // limpa lista de instalações e adiciona apenas instalações desta equipe clicada
            limparListaInst();
            listaInstalacao(obj.getAttribute("idEquipe"));

            obj.style.backgroundColor = "#CCCCCC";
            selectedPoint = obj;

            var pos = parseInt(obj.getAttribute("pos"));
            var conteudo = "<span style=\"font-weight: bold\">" + marcadores[pos].titulo + "</span><br />" + marcadores[pos].texto;
            marcadores[pos].marker.openInfoWindowHtml(conteudo);
        }
    });

    // Pega os dois primeiros membros da equipe
    var posVazio = mark.titulo.indexOf(' '); // Posição do primeiro espaço
    var posVazio2 = posVazio > 0 ? mark.titulo.indexOf(' ', posVazio + 1) : -1; // Posição do segundo espaço
    var nome = posVazio == -1 || posVazio2 == -1 ? mark.titulo : mark.titulo.substring(0, posVazio2);

    // Adiciona uma célula à linha com o nome da equipe
    td = document.createElement("td");
    td.innerHTML = nome + " - " + mark.dtUltPonto;
    tr.appendChild(td);

    // Adiciona linha com nome da equipe à tabela
    tbSpd.appendChild(tr);
}

// Adiciona instalações na lista, se idEquipe for = 0, mostra na lista instalações de todos as equipes,
// se for > 0, mostra instalações apenas da equipe passada
function listaInstalacao(idEquipe) {

    for (var i = 0; i < marcadores.length; i++) {
        if (marcadores[i].idPedido == null || (idEquipe != 0 && marcadores[i].idEquipe != idEquipe))
            continue;

        // Busca a tabela que receberá a lista de instalacoes
        var tbSpd = document.getElementById("Instalacoes");

        // Cria a linha referente à instalação que será inserida na tabela acima
        var tr = document.createElement("tr");
        //tr.style.backgroundColor = "#CCFFCC";	
        tr.setAttribute("lat", marcadores[i].ponto.lat());
        tr.setAttribute("lng", marcadores[i].ponto.lng());
        tr.setAttribute("pos", i); // salva a posição do marcador a qual esta tr está relacionado
        tr.style.cursor = "pointer";

        // Adiciona uma célula à linha para dar margem
        td = document.createElement("td");
        td.width = "5px";
        tr.appendChild(td);

        // Adiciona uma célula à linha com o ícone de um prédio, identificando a instalação
        td = document.createElement("td");
        td.innerHTML = "<img src=\"../Images/building.gif\" height=\"18px\" width=\"18px\" border=\"0px\"/>";
        tr.appendChild(td);

        // Adiciona evento ao item da lista para mudar cor de fundo quando o mouse estiver sobre o mesmo
        _addHandler(tr, "mouseover", function(e) {
            var obj = (ie5 ? e.srcElement.parentElement : this);

            if (obj == selectedPoint) return;
            obj.style.backgroundColor = "#FFFFFF";
        });

        // Adiciona evento ao item da lista para mudar cor de fundo quando o mouse sair de foco do mesmo		
        _addHandler(tr, "mouseout", function(e) {
            var obj = (ie5 ? e.srcElement.parentElement : this);

            if (obj == selectedPoint) return;
            obj.style.backgroundColor = "";
        });

        // Adiciona evento click ao item da lista que irá mudar a posição do mapa para a posição da instalação clicada
        _addHandler(tr, "click", function(e) {
            var obj = (ie5 ? e.srcElement.parentElement : this);

            if (obj == selectedPoint)
                return;
            else if (selectedPoint != null)
                selectedPoint.style.backgroundColor = "";

            if (obj.getAttribute("lat") != null && obj.getAttribute("lat") != "" && obj.getAttribute("lat") != "0") {
                mapa.panTo(new GLatLng(obj.getAttribute("lat"), obj.getAttribute("lng")));

                obj.style.backgroundColor = "#CCCCCC";
                selectedPoint = obj;

                var pos = parseInt(obj.getAttribute("pos"));
                var conteudo = "<span style=\"font-weight: bold\">" + marcadores[pos].titulo + "</span><br />" + marcadores[pos].texto;
                marcadores[pos].marker.openInfoWindowHtml(conteudo);
            }
        });
        
        // Adiciona uma célula à linha com dados da instalacao
        td = document.createElement("td");
        td.innerHTML = "Ped. " + marcadores[i].idPedido + (marcadores[i].situacao == "C" ? " - Tempo: " + marcadores[i].tempo : " - Pendente");
        
        // Se a instalação estiver pendente, muda a cor do texto para vermelho
        if (marcadores[i].situacao == "P")
            td.style.color = "red";
        
        tr.appendChild(td);

        // Adiciona linha com dados da instalação à tabela
        tbSpd.appendChild(tr);
    }
}

// Limpa todos os marcadores do mapa e das listas
function limparMarcadores() {
    var tbSpd = document.getElementById("MapMarks");
    var count = tbSpd.childNodes.length;

    for (i = 0; i < count; i++)
        tbSpd.removeChild(tbSpd.childNodes[0]);

    mapa.clearOverlays();

    limparListaInst();
}

// Limpa lista de instalações
function limparListaInst() {
    var tbInst = document.getElementById("Instalacoes");
    var count = tbInst.childNodes.length;

    for (i = 0; i < count; i++)
        tbInst.removeChild(tbInst.childNodes[0]);
}

function posicionar() {
    if (marcadores.length == 0) return;

    var pontoNE = null;
    var pontoSO = null;

    for (var i = 0; i < marcadores.length; i++) {
        if (marcadores[i].ponto != null && marcadores[i].ponto != "" && marcadores[i].ponto != "(NaN, NaN)") {
            pontoNE = marcadores[i].ponto;
            pontoSO = marcadores[i].ponto;
            break;
        }
    }

    if (pontoNE != null) {
        for (var j = i; j < marcadores.length; j++) {
            if (marcadores[j].ponto.lat() > pontoNE.lat())
                pontoNE = new GLatLng(marcadores[j].ponto.lat(), pontoNE.lng());
            else if (marcadores[j].ponto.lat() < pontoSO.lat())
                pontoSO = new GLatLng(marcadores[j].ponto.lat(), pontoSO.lng());

            if (marcadores[j].ponto.lng() > pontoNE.lng())
                pontoNE = new GLatLng(pontoNE.lat(), marcadores[j].ponto.lng());
            else if (marcadores[j].ponto.lng() < pontoSO.lng())
                pontoSO = new GLatLng(pontoSO.lat(), marcadores[j].ponto.lng());
        }

        var pontoCentral = new GLatLngBounds(pontoSO, pontoNE).getCenter();
        var zoom = mapa.getBoundsZoomLevel(new GLatLngBounds(pontoSO, pontoNE));

        mapa.setCenter(pontoCentral, zoom);
    }
}

function iconeEquipe() {
    var icon = new GIcon();

    icon.image = "../Images/equipe.gif";
    icon.shadow = G_DEFAULT_ICON.shadow;
    icon.iconSize = new GSize(21, 21);
    icon.shadowSize = G_DEFAULT_ICON.shadowSize;
    icon.iconAnchor = new GPoint(0, 0);
    icon.infoWindowAnchor = G_DEFAULT_ICON.infoWindowAnchor;
    icon.infoShadowAnchor = G_DEFAULT_ICON.infoShadowAnchor;

    return icon;
}

function iconeInstalacao() {
    var icon = new GIcon();

    icon.image = "../Images/building.gif";
    icon.shadow = G_DEFAULT_ICON.shadow;
    icon.iconSize = new GSize(22, 22);
    icon.shadowSize = G_DEFAULT_ICON.shadowSize;
    icon.iconAnchor = new GPoint(0, 0);
    icon.infoWindowAnchor = G_DEFAULT_ICON.infoWindowAnchor;
    icon.infoShadowAnchor = G_DEFAULT_ICON.infoShadowAnchor;

    return icon;
}

function tratarErros() {
    if (gdir.getStatus().code == G_GEO_UNKNOWN_ADDRESS)
        alert("Não foi possível encontrar uma posição geográfica para este endereço. Pode ser que ele seja relativamente novo ou pode estar incorreto.\nCódigo de erro: " + gdir.getStatus().code);
    else if (gdir.getStatus().code == G_GEO_SERVER_ERROR)
        alert("Não foi possível fazer uma requisição geográfica ou de direções, embora o motivo do erro não seja conhecido.\nCódigo de erro: " + gdir.getStatus().code);
    else if (gdir.getStatus().code == G_GEO_MISSING_QUERY)
        alert("O parâmetro q na URI não foi encontrado ou não tem valor. Para requisições geográficas, isso significa que um endereço vazio foi digitado. Para requisições de endereço, significa que não foi especificada uma consulta.\nCódigo de erro: " + gdir.getStatus().code);
    else if (gdir.getStatus().code == G_GEO_BAD_KEY)
        alert("A chave usada é inválida ou não corresponde ao domínio para a qual foi dada.\nCódigo de erro: " + gdir.getStatus().code);
    else if (gdir.getStatus().code == G_GEO_BAD_REQUEST)
        alert("Uma requisição de direções não pôde ser feita com sucesso.\nCódigo de erro: " + gdir.getStatus().code);
    else
        alert("Erro desconhecido.");
}

google.setOnLoadCallback(iniciar);