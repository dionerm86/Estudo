var mapa = null;
var pontos = null;
var titiulos = null;
var textos = null;
var drawPolyline;
var countP = 0; // Conta quantos pontos já foram inseridos no mapa
var pontosInst; // Pontos de instalacao
var distTotal = 0;
var distCorrente = 0;
var minutosDist = 20; // Minutos de distância máximo para que um ponto seja ligado ao próximo

google.load("maps", "2");

// Mostra rota no mapa
function iniciar() {
    if (GBrowserIsCompatible()) {
        mapa = new GMap2(document.getElementById("mapa"));

        mapa.setCenter(new GLatLng(0, 0), 5);
        mapa.enableScrollWheelZoom();
        mapa.addControl(new GLargeMapControl());
        mapa.addControl(new GMapTypeControl());
        mapa.addControl(new GOverviewMapControl());
        
        mudarMarcadoresRota();
    }
}

function mudarMarcadoresRota() {
    var equipe = document.getElementById("IdEquipe").value;
    var dtInicio = document.getElementById("DtInicio").value;
    var dtFim = document.getElementById("DtFim").value;
    var refresh = (new Date()).getMilliseconds().toString();

    limparMarcadores();

    document.getElementById("lblDistancia").innerHTML = "0km / 0km";

    GDownloadUrl("../Handlers/XmlRota.ashx?IdEquipe=" + equipe + "&dtInicio=" + dtInicio +
        "&dtFim=" + dtFim + "&refresh=" + refresh, function(data) {
            var xml = GXml.parse(data);

            var markers = xml.getElementsByTagName("marker");
            pontosInst = markers;

            if (markers.length == 0)
                return;

            pontos = new Array(markers.length);
            titulos = new Array(markers.length);
            textos = new Array(markers.length);
            
            // Cria os pontos de instalação e de referência
            setMarcadores();
            
            if (pontos.length > 0) {
                mapa.setCenter(pontos[0], 14);
                distCorrente = 0;
                drawPolyline = setInterval("drawRoute()", 1000);
            }
        });
}

// Cria os pontos de instalação e de referência
function setMarcadores() {
    var lat;
    var lng;
    var pontoCorrente;
    var ultimoPontoNormal = null;
    
    distTotal = 0;

    for (var i = 0; i < pontosInst.length; i++) {
        if (pontosInst[i] == undefined)
            continue;

        if (pontosInst[i].getAttribute("tipo") == "I") // Ponto de Instalação
            mapa.addOverlay(criaMarcador(pontosInst[i], i, "I"));
        else {
            if (pontosInst[i].getAttribute("tipo") == "R") // Ponto de referência da rota (exibe velocidade entre outros)
                mapa.addOverlay(criaMarcador(pontosInst[i], i, "R"));
            else 
                criaMarcador(pontosInst[i], i, "N"); // Ponto normal

            lat = parseFloat(pontosInst[i].getAttribute("lat"));
            lng = parseFloat(pontosInst[i].getAttribute("lng"));
            pontoCorrente = new GLatLng(lat, lng);

            // Se o último pontos for válido e se o ponto corrente estiver a menos de 
            // minutosDist minutos de distância do último ponto, e menos de 200 de distância, calcula a distância entre eles
            if (ultimoPontoNormal != null && pontosInst[i].getAttribute("minDiff") < minutosDist &&
                pontoCorrente.distanceFrom(ultimoPontoNormal) < 200)
                distTotal += pontoCorrente.distanceFrom(ultimoPontoNormal);
        
            ultimoPontoNormal = pontoCorrente;
        }
    }

    distTotal = distTotal / 1000; // Converte distância para km
    distTotal = distTotal.toString().substring(0, distTotal.toString().indexOf('.') + 3);

    document.getElementById("lblDistancia").innerHTML = "0km / " + distTotal + "km";
}

// Desenha rota dinamicamente
function drawRoute() {
    if (countP < pontos.length) {
        // Se for ponto de instalação, retorna
        if (!pontosInst[countP + 1] || pontosInst[countP + 1].getAttribute("tipo") == "I")
            return;

        var pontosTemp = new Array(2);
        pontosTemp[0] = pontos[countP];
        pontosTemp[1] = pontos[countP + 1];

        // Se os pontos que serão ligados estiverem a menos de minutosDist minutos de
        // distância temporal e menos de 200 de distância, serão ligados, caso contrário não serão ligados
        if (pontosInst[countP + 1].getAttribute("minDiff") < minutosDist && pontosTemp[0].distanceFrom(pontosTemp[1]) < 200) {
            distCorrente += pontosTemp[0].distanceFrom(pontosTemp[1]) / 1000;
            var distCurrPrint = distCorrente.toString().substring(0, distCorrente.toString().indexOf('.') + 3);
            document.getElementById("lblDistancia").innerHTML = distCurrPrint + "km / " + distTotal + "km";

            mapa.addOverlay(new GPolyline(pontosTemp, "#FF0000"));
        }

        mapa.panTo(pontos[countP]);
        countP++;
    }
    else
        clearInterval(drawPolyline);
}

// Desenha toda a rota de uma só vez
function drawAllRoute() {
    var ptsFisc = new Array(pontosInst.length);
    var lat, lng, countPontos = 0;

    for (var i = 0; i < pontosInst.length; i++) {
        // Se não for ponto de instalação
        if (pontosInst[i].getAttribute("tipo") != "I") {
            lat = parseFloat(pontosInst[i].getAttribute("lat"));
            lng = parseFloat(pontosInst[i].getAttribute("lng"));

            if (pontosInst[i].getAttribute("minDiff") < minutosDist) {
                if (pontosInst[i + 1]) {
                    if (new GLatLng(lat, lng).distanceFrom(new GLatLng(parseFloat(pontosInst[i+1].getAttribute("lat")), parseFloat(pontosInst[i+1].getAttribute("lng")))) < 200) {
                        ptsFisc[countPontos] = new GLatLng(lat, lng);
                        countPontos++;
                    }
                }
                else {
                    ptsFisc[countPontos] = new GLatLng(lat, lng);
                    countPontos++;
                }
            }
            else {
                mapa.addOverlay(new GPolyline(ptsFisc, "#FF0000"));
                countPontos = 0;
                ptsFisc = new Array(pontosInst.length);
            }
            
            if (pontosInst[i].getAttribute("tipo") == "R") // Ponto de referência da rota (exibe velocidade entre outros) 
                mapa.addOverlay(criaMarcador(pontosInst[i], i, "R"));
        }
    }

    if (countPontos > 0)
        mapa.addOverlay(new GPolyline(ptsFisc, "#FF0000"));

    document.getElementById("lblDistancia").innerHTML = distTotal + "km / " + distTotal + "km";
}

// Cria um ícone no mapa, podendo ser de referência ou de instalação
function criaMarcador(dadosXml, numero, tipo) {
    var lat = parseFloat(dadosXml.getAttribute("lat"));
    var lng = parseFloat(dadosXml.getAttribute("lng"));

    var ponto = new GLatLng(lat, lng);

    pontos[numero] = ponto;
    titulos[numero] = dadosXml.getAttribute("titulo");
    textos[numero] = dadosXml.getAttribute("texto");

    // Cria um ícone para este marcador
    var marker = null;

    if (tipo == "I") // Ponto de Instalação
        marker = new GMarker(ponto, { "icon": iconeInstalacao() });
    else if (tipo == "R") // Ponto de Referência
        marker = new GMarker(ponto, { "icon": iconeReferencia() });

    // Atribui evento click ao ícone, que irá abrir um balão com a descrição dada ao mesmo no XML
    if (marker != null) {
        GEvent.addListener(marker, "click", function() {
            var conteudo = "<span style=\"font-weight: bold\">" + titulos[numero] + "</span><br />" + textos[numero];
            marker.openInfoWindowHtml(conteudo);
        });
    }

    return marker;
}

// Limpa todos os marcadores no mapa
function limparMarcadores() {
    mapa.clearOverlays();
}

// Posiciona o mapa na localização do ponto atual
function posicionar() {
    if (pontos.length == 0) return;

    var pontoNE = pontos[0];
    var pontoSO = pontos[0];

    for (var i = 1; i < pontos.length; i++) {
        if (pontos[i].lat() > pontoNE.lat())
            pontoNE = new GLatLng(pontos[i].lat(), pontoNE.lng());
        else if (pontos[i].lat() < pontoSO.lat())
            pontoSO = new GLatLng(pontos[i].lat(), pontoSO.lng());

        if (pontos[i].lng() > pontoNE.lng())
            pontoNE = new GLatLng(pontoNE.lat(), pontos[i].lng());
        else if (pontos[i].lng() < pontoSO.lng())
            pontoSO = new GLatLng(pontoSO.lat(), pontos[i].lng());
    }

    var pontoCentral = new GLatLngBounds(pontoSO, pontoNE).getCenter();
    var zoom = mapa.getBoundsZoomLevel(new GLatLngBounds(pontoSO, pontoNE));

    mapa.setCenter(pontoCentral, zoom - 2);
}

// Redimensiona o mapa quanda a janela for redimensionada
function resizeMap() {
    var container = document.getElementById("mapa");

    if (container == null)
        return;

    container.style.width = document.documentElement.clientWidth - 30 + 'px';
    container.style.height = document.documentElement.clientHeight - 80 + 'px';
    
    if (mapa)
        mapa.checkResize();
}

function iconeReferencia() {
    var icon = new GIcon();

    icon.image = "http://maps.google.com/mapfiles/kml/pal4/icon57.png";
    icon.shadow = "http://maps.google.com/mapfiles/kml/pal4/icon57s.png";
    icon.iconSize = new GSize(21, 21);
    icon.shadowSize = new GSize(30, 16);
    icon.iconAnchor = new GPoint(0, 0);
    icon.infoWindowAnchor = new GPoint(16, 0);

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

function iniciarPausar() {
    var linkIniciarPausar = document.getElementById("linkIniciarPausar");

    if (document.getElementById("linkIniciarPausar").innerHTML == "Iniciar") {
        document.getElementById("linkIniciarPausar").innerHTML = "Pausar";

        drawPolyline = setInterval("drawRoute()", document.getElementById("valor_velocidade").value);
    }
    else {
        clearInterval(drawPolyline);
        document.getElementById("linkIniciarPausar").innerHTML = "Iniciar";
    }

    document.getElementById("linkParar").disabled = false;
}

function parar() {
    document.getElementById("linkIniciarPausar").innerHTML = "Iniciar";

    // Para o loop de desenho de rota
    clearInterval(drawPolyline);
    // Zera o contador de pontos desenhados
    countP = 0;
    // Apaga todos os itens que foram desenhados no mapa
    limparMarcadores();
    // Centraliza o mapa na posição inicial
    mapa.panTo(pontos[0]);
    // Desenha os pontos de instalação e de referência
    setMarcadores();
    // Zera a distância corrente
    distCorrente = 0;

    document.getElementById("linkParar").disabled = true;
}

function alterarVelocidade(controle) {
    document.getElementById("valor_velocidade").value = controle.value;
}

function desenhaRotaCompleta() {
    parar();

    drawAllRoute();
}

function mudaPeriodo() {
    var equipe = document.getElementById("IdEquipe").value;
    var dtInicio = document.getElementById("DtInicio").value;
    var dtFim = document.getElementById("DtFim").value;

    if (Mapa.ExistePontos(equipe, dtInicio, dtFim).value == "False") {
        alert("Não há pontos cadastros pela equipe no período informado.");
        return false;
    }

    parar();

    // Para o loop de desenho de rota
    clearInterval(drawPolyline);
    drawPolyline = null;

    document.getElementById("linkIniciarPausar").innerHTML = "Pausar";

    mudarMarcadoresRota();
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
        alert("Erro desconhecido.\nCódigo de erro: " + gdir.getStatus().code);
}

// Chama a função iniciar() quando o google terminar de carregar o mapa
google.setOnLoadCallback(iniciar);