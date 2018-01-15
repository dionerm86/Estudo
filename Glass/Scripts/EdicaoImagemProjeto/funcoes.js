function arredonda(valor, casasDecimais) {
    return (Math.round(valor * Math.pow(10, casasDecimais))) / Math.pow(10, casasDecimais);
}

function valorOut(valor) {
    valor = Math.round(valor * 100) / 100;
    var arrayValor = valor.toString().split(".");
    if (arrayValor[1] == null)
    { arrayValor[1] = "00"; };
    if (arrayValor[1].length == 1)
    { arrayValor[1] = arrayValor[1] + "0"; };
    if (Math.abs(arrayValor[0]) > 999) {
        var resto = arrayValor[0].substring(arrayValor[0].length - 3, arrayValor[0].length);
        var milhar = arrayValor[0].substring(0, arrayValor[0].length - 3);
        arrayValor[0] = milhar + "." + resto;
    };
    valor = arrayValor[0] + "," + arrayValor[1];
    return valor;
}

function fnc_timeUnix(data, horario) {
    var dia = data.toString().substr(0, 2);
    var mes = data.toString().substr(2, 2);
    var ano = '20' + data.toString().substr(4, 2);

    var hora = horario.toString().substr(0, 2);
    var minutos = horario.toString().substr(2, 2);
    var segundos = horario.toString().substr(4, 2);

    var data = new Date(ano, mes - 1, dia, hora, minutos, segundos);
    var time = data.getTime() / 1000;
    return time;
}

function getDataAtual() {
    var data = new Date();
    var ano = String(data.getFullYear()).substring(2);
    var mes = data.getMonth() + 1;
    if (mes < 10) {
        mes = '0' + String(mes);
    }
    else {
        mes = String(mes);
    }
    var dia = data.getDate();
    if (dia < 10) {
        dia = '0' + String(dia);
    }
    else {
        dia = String(dia);
    }
    return ano + mes + dia;
}

function getData(data) {
    while (data.length < 6) {
        data = '0' + data;
    }
    var ano = String(data.substring(4));
    var mes = String(data.substr(2, 2));
    var dia = String(data.substr(0, 2));
    return ano + mes + dia;
}

// 0 - menor  1 - igual  2 - maior
function verificaData(data1, data2) {
    if (data1 < data2) {
        return 0;
    }
    else if (data1 == data2) {
        return 1;
    }
    else {
        return 2;
    }
}


function avancaDias(lnDias, ldDia, ldMes, ldAno) {
    var ndiasmes = "";
    var ltDia = ldDia;
    var ltMes = ldMes;
    var ltAno = ldAno;
    // Pega o numero de dias do mes
    //31 dias
    if ((ldMes == 01) || (ldMes == 03) || (ldMes == 05) || (ldMes == 07) || (ldMes == 08) || (ldMes == 10) || (ldMes == 12))
    { ndiasmes = 31; }
    else if ((ldMes == 04) || (ldMes == 06) || (ldMes == 09) || (ldMes == 11))	//30 dias
    { ndiasmes = 30; }
    else   //fevereiro
    {
        //Calcula ano bissexto
        if (((ldAno % 4) == 0) && ((ldAno % 100) == 0))
            ndiasmes = 29
        else if ((ldAno % 400) == 0)
            ndiasmes = 29
        else
            ndiasmes = 28
    }

    // Incrementa dias
    if ((ldDia + lnDias) <= ndiasmes)
    { ltDia = ldDia + lnDias; }
    else {
        ltDia = parseInt((ldDia + lnDias) % ndiasmes)

        if (parseInt(ldMes + ((ldDia + lnDias) / ndiasmes)) <= 12)
        { ltMes = parseInt(ldMes + ((ldDia + lnDias) / ndiasmes)); }
        else {
            ltMes = parseInt((ldMes + ((ldDia + lnDias) / ndiasmes)) % 12);
            ltAno = parseInt(ldAno + ((ldMes + ((ldDia + lnDias) / ndiasmes)) / 12));
        }
    }
    if (ltDia < 10)
    { ltDia = "0" + ltDia; }
    if (ltMes < 10)
    { ltMes = "0" + ltMes; }
    return ltDia.toString() + ltMes.toString() + ltAno.toString().substr(2, 2);
}



function avancaDiasDiasUteis(lnDias, ldDia, ldMes, ldAno) {
    var ndiasmes = '';
    var textDia = '';
    var textMes = '';

    var dia = ldDia;
    var mes = ldMes;
    var ano = ldAno;
    var data = new Date();

    // Incrementa dias
    var incremento = lnDias;
    while (incremento > 0) {
        // Pega o numero de dias do mes
        if (mes == 01 || mes == 03 || mes == 05 || mes == 07 || mes == 08 || mes == 10 || mes == 12) //31 dias
        {
            ndiasmes = 31;
        }
        else if (mes == 04 || mes == 06 || mes == 09 || mes == 11) //30 dias
        {
            ndiasmes = 30;
        }
        else   //fevereiro
        {
            //Calcula ano bissexto
            if (((ldAno % 4) == 0) && ((ldAno % 100) == 0)) {
                ndiasmes = 29;
            }
            else if ((ldAno % 400) == 0) {
                ndiasmes = 29;
            }
            else {
                ndiasmes = 28;
            }
        }

        // Se não estourar o ultimo dia do mes
        if ((dia + 1) <= ndiasmes) {
            dia++;
        }
        else // Estorou o ultimo dia do mes
        {
            // Mes vai estourar
            if (mes == 12) {
                dia = 01;
                mes = 01;
                ano++;
            }
            else {
                dia = 01;
                mes++;
            }
        }

        // Verifica se o dia selecionado é um dia util se não for não decrementa
        textDia = dia;
        if (dia < 10) {
            textDia = "0" + textDia;
        }
        textMes = mes - 1;
        if (textMes < 10) {
            textMes = "0" + textMes;
        }

        data.setFullYear(ano, textMes, textDia);
        if (data.getDay() != 0 && data.getDay() != 6) {
            incremento--;
        }
    }

    // Ajusta o dia, mes e ano para o resultado, pois foi alterado para funcionar na forma da data do javascript
    textDia = dia;
    if (dia < 10) {
        textDia = "0" + textDia;
    }
    textMes = mes;
    if (mes < 10) {
        textMes = "0" + (mes - 1);
    }
    if (mes < 10) {
        textMes = "0" + mes;
    }
    return textDia.toString() + textMes.toString() + ano.toString().substr(2, 2);
}

function validarCPF(cpf) {
    var constante = 0;
    var digitoVerificadorCalculado = 0;
    var digitoVerificadorDigitado = eval(cpf.charAt(9) + cpf.charAt(10)); // Retorna o digito verificador do cpf digitado
    var resto = 0;
    var soma1 = 0;
    var soma2 = 0;
    var valido = false;

    if (cpf == "11111111111" || cpf == "22222222222" || cpf == "33333333333" || cpf == "44444444444" || cpf == "55555555555" ||
		  cpf == "66666666666" || cpf == "77777777777" || cpf == "88888888888" || cpf == "99999999999" || cpf == "00000000000" || cpf.length < 11) {
        valido = false;
    }
    else {
        // Cálculo do primeiro digito verificador
        var primeiroDigitoVerificador = 0;
        constante = 10;
        for (var i = 0; i <= 8; i++) {
            soma1 += eval(cpf.charAt(i)) * constante;
            constante--;
        }
        resto = soma1 % 11;
        if (resto == 0 || resto == 1) {
            primeiroDigitoVerificador = 0;
        }
        else {
            primeiroDigitoVerificador = 11 - resto;
        }
        // Cálculo do segundo digito verificador
        var segundoDigitoVerificador = 0;
        constante = 11;
        for (var i = 0; i <= 9; i++) {
            soma2 += eval(cpf.charAt(i)) * constante;
            constante--;
        }
        resto = soma2 % 11;
        if (resto == 0 || resto == 1) {
            segundoDigitoVerificador = 0;
        }
        else {
            segundoDigitoVerificador = 11 - resto;
        }
        // Junta os dois digitos verificadores calculados
        digitoVerificadorCalculado = eval((primeiroDigitoVerificador * 10) + segundoDigitoVerificador);
    }
    // Verifica se o digito verificador calculado é igual ao digitado	
    if (digitoVerificadorCalculado == digitoVerificadorDigitado) {
        valido = true;
    }
    else {
        valido = false;
    }
    return valido;
}

function validaCpfCnpj(valor, idSpan, idBtCadastro) {
    sohNumero(valor);
    var valor = valor.value;
    var tam = valor.length;
    var resultadoValid = false;
    var numeros, digitos, soma, i, resultado, digitos_iguais;
    digitos_iguais = 1;
    if (tam == 11 || tam == 14) {
        for (i = 0; i < valor.length - 1; i++)
            if (valor.charAt(i) != valor.charAt(i + 1)) {
            digitos_iguais = 0;
            break;
        }
        if (!digitos_iguais) {
            if (tam == 11) // CPF
            {
                resultadoValid = validarCPF(valor);
            }
            else if (tam == 14) // CNPJ
            {
                tamanho = valor.length - 2
                numeros = valor.substring(0, tamanho);
                digitos = valor.substring(tamanho);
                soma = 0;
                pos = tamanho - 7;
                for (i = tamanho; i >= 1; i--) {
                    soma += numeros.charAt(tamanho - i) * pos--;
                    if (pos < 2) {
                        pos = 9;
                    }
                }
                resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != digitos.charAt(0)) {
                    resultadoValid = false;
                }
                tamanho = tamanho + 1;
                numeros = valor.substring(0, tamanho);
                soma = 0;
                pos = tamanho - 7;
                for (i = tamanho; i >= 1; i--) {
                    soma += numeros.charAt(tamanho - i) * pos--;
                    if (pos < 2) {
                        pos = 9;
                    }
                }
                resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != digitos.charAt(1)) {
                    resultadoValid = false;
                }
                else {
                    resultadoValid = true;
                }
            }
        }
        else {
            resultadoValid = false;
        }
    }

    if (resultadoValid == true && valor != '') {
        document.getElementById(idSpan).innerHTML = "V&aacute;lido";
        if (idBtCadastro != '') {
            document.getElementById(idBtCadastro).disabled = 0;
        }
    }
    else if (resultadoValid == false && valor != '') {
        document.getElementById(idSpan).innerHTML = "Inv&aacute;lido";
        if (idBtCadastro != '') {
            document.getElementById(idBtCadastro).disabled = 1;
        }
    }
    if (valor == '') {
        document.getElementById(idSpan).innerHTML = '';
        if (idBtCadastro != '') {
            document.getElementById(idBtCadastro).disabled = 0;
        }
    }
    return resultadoValid;
}

function validaCodigo(editCodigo) {
    var codigo = editCodigo.value;
    var caracteres_n_permitidos = "/,;\?=§š(){}[]@#$%!&*´'^~<>¨¬°` ";
    var retorno = '';
    for (var i = 0; i <= (codigo.length - 1); i++) {
        if (caracteres_n_permitidos.indexOf(codigo.charAt(i)) == -1)
        { retorno = retorno + (codigo.charAt(i)); }
    }
    editCodigo.value = retorno;
}

function validaCodigoMaterial(editCodigo) {
    var codigo = editCodigo.value;
    var caracteres_n_permitidos = "/,;\?=§š(){}[]@#$%!&*´'^~<>¨¬°` ";
    var retorno = '';
    for (var i = 0; i <= (codigo.length - 1); i++) {
        if (caracteres_n_permitidos.indexOf(codigo.charAt(i)) == -1)
        { retorno = retorno + (codigo.charAt(i)); }
    }
    editCodigo.value = retorno;
}

function sohNumero(oCampo) {
    campo = oCampo.value;
    var numeros = "0123456789";
    var retorno = '';
    for (var i = 0; i <= (campo.length - 1); i++) {
        if (numeros.indexOf(campo.charAt(i)) !== -1)
        { retorno = retorno + (campo.charAt(i)); }
    }
    oCampo.value = retorno;
}

function sohNumeroComMais(oCampo) {
    campo = oCampo.value;
    var numeros = "0123456789+";
    var retorno = '';
    for (var i = 0; i <= (campo.length - 1); i++) {
        if (numeros.indexOf(campo.charAt(i)) !== -1)
        { retorno = retorno + (campo.charAt(i)); }
    }
    oCampo.value = retorno;
}

function sohNumeroComMaisVirgula(oCampo) {
    campo = oCampo.value;
    var numeros = "0123456789+,";
    var retorno = '';
    for (var i = 0; i <= (campo.length - 1); i++) {
        if (numeros.indexOf(campo.charAt(i)) !== -1)
        { retorno = retorno + (campo.charAt(i)); }
    }
    oCampo.value = retorno;
}

function sohNumeroComVirgula(oCampo) {
    campo = oCampo.value;
    var numeros = "0123456789,";
    var retorno = '';
    for (var i = 0; i <= (campo.length - 1); i++) {
        if (numeros.indexOf(campo.charAt(i)) !== -1)
        { retorno = retorno + (campo.charAt(i)); }
    }
    oCampo.value = retorno;
}

function sohNumeroComVirgulaComTraco(oCampo) {
    campo = oCampo.value;
    var numeros = "0123456789,-";
    var retorno = '';
    for (var i = 0; i <= (campo.length - 1); i++) {
        if (numeros.indexOf(campo.charAt(i)) !== -1)
        { retorno = retorno + (campo.charAt(i)); }
    }
    oCampo.value = retorno;
}

function sohNumeroTracoX(oCampo) {
    campo = oCampo.value;
    var numeros = "0123456789-Xx";
    var retorno = '';
    for (var i = 0; i <= (campo.length - 1); i++) {
        if (numeros.indexOf(campo.charAt(i)) !== -1)
        { retorno = retorno + (campo.charAt(i)); }
    }
    oCampo.value = retorno;
}

function sohNumeroComTraco(oCampo) {
    campo = oCampo.value;
    var numeros = "0123456789-";
    var retorno = '';
    for (var i = 0; i <= (campo.length - 1); i++) {
        if (numeros.indexOf(campo.charAt(i)) !== -1)
        { retorno = retorno + (campo.charAt(i)); }
    }
    oCampo.value = retorno;
}

function sohLetraNumeroTraco(oCampo) {
    campo = oCampo.value;
    var numeros = "0123456789-_abcdefghijklmnopqrstuvwxyzABCDEFGHIOJKLMNOPQRSTUXWXYZV";
    var retorno = '';
    for (var i = 0; i <= (campo.length - 1); i++) {
        if (numeros.indexOf(campo.charAt(i)) !== -1)
        { retorno = retorno + (campo.charAt(i)); }
    }
    oCampo.value = retorno;
}

function sohLetraNumero(oCampo) {
    campo = oCampo.value;
    var numeros = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIOJKLMNOPQRSTUXWXYZV";
    var retorno = '';
    for (var i = 0; i <= (campo.length - 1); i++) {
        if (numeros.indexOf(campo.charAt(i)) !== -1)
        { retorno = retorno + (campo.charAt(i)); }
    }
    oCampo.value = retorno;
}

function semEspacos(oCampo) {
    campo = oCampo.value;
    var numeros = " ";
    var retorno = '';
    for (var i = 0; i <= (campo.length - 1); i++) {
        if (numeros.indexOf(campo.charAt(i)) == -1)
        { retorno = retorno + (campo.charAt(i)); }
    }
    oCampo.value = retorno;
}

function semPorcentagem(oCampo) {
    campo = oCampo.value;
    var numeros = "%";
    var retorno = '';
    for (var i = 0; i <= (campo.length - 1); i++) {
        if (numeros.indexOf(campo.charAt(i)) == -1)
        { retorno = retorno + (campo.charAt(i)); }
    }
    oCampo.value = retorno;
}

function pula_campo(maxlength, id, proximo) {
    if (document.getElementById(id).value.length >= maxlength) {
        document.getElementById(proximo).focus();
    }
}

function popup(caminho, nome, largura, altura, rolagem) {
    var esquerda = (screen.width - largura) / 2;
    var cima = (screen.height - altura) / 2 - 50;
    window.open(caminho, null, 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=' + rolagem + ',resizable=no,copyhistory=no,top=' + cima + ',left=' + esquerda + ',width=' + largura + ',height=' + altura);
}

function validaEmail(idEmail) {
    email = idEmail.value;
    parteInicial = '';
    parteMeio = '';
    parteFinal = '';
    if (email != '') {
        tamanho = email.length;
        posArroba = email.indexOf('@');
        posPonto = email.indexOf('.');
        if (posArroba >= 0) {
            parteInicial = email.substring(0, posArroba);
        }
        if (posArroba >= 0 && posPonto >= 0) {
            parteMeio = email.substring(posArroba + 1, posPonto);
        }
        if (posPonto >= 0) {
            parteFinal = email.substring(posPonto + 1, tamanho);
        }
        if (parteInicial == '' || parteMeio == '' || parteFinal == '') {
            alert("E-mail incorreto...");
        }
    }
}

function validaExtImagem(componente, manipulado) {
    var arrayExtensao = componente.value.split('.');
    var extensao = arrayExtensao[arrayExtensao.length - 1];
    if (manipulado == 0) // IMAGENS NÃO MANIPULADAS PELO PHP
    {
        if (extensao.toUpperCase() != 'GIF' && extensao.toUpperCase() != 'JPG' && extensao.toUpperCase() != 'JPEG' && extensao.toUpperCase() != 'PNG' && extensao.toUpperCase() != 'PHP') {
            componente.value = '';
            alert('É permitido apenas extensões GIF, JPG, JPEG e PNG.');
        }
    }
    else  // IMAGENS MANIPULADAS PELO PHP
    {
        if (extensao.toUpperCase() != 'JPG' && extensao.toUpperCase() != 'PNG' && extensao.toUpperCase() != 'PHP') {
            componente.value = '';
            alert('É permitido apenas extensões JPG e PNG.');
        }
    }
}

function LimitarCaracter(numMaxCaracter) {
    Campo = document.getElementById("mensagem");
    Display = document.getElementById("contador");
    Caracteres = numMaxCaracter - Campo.value.length;
    if (Caracteres >= 0) {
        Display.innerHTML = Caracteres;
    }
    if (Campo.value.length >= numMaxCaracter) {
        Campo.value = Campo.value.substring(0, numMaxCaracter);
    }
}

function criaObjetoXML() {
    var requerimento;
    try {
        if (window.XMLHttpRequest) {
            requerimento = new XMLHttpRequest();
            if (requerimento.readyState == null) {
                requerimento.readyState = 1; //Carregando
                requerimento.addEventListener(
														"load",
														function() {
														    requerimento.readyState = 4; //Completo
														    if (typeof requerimento.onReadyStateChange == "function")
														    { requerimento.onReadyStateChange(); }
														},
														false
														);
            }
            return requerimento;
        }
        if (window.ActiveXObject) {
            var prefixos = ["MSXML2", "Microsoft", "MSXML", "MSXML3"];
            for (var i = 0; i < prefixos.length; i++) {
                try {
                    requerimento = new ActiveXObject(prefixos[i] + ".XmlHttp");
                    return requerimento;
                }
                catch (ex) { };
            }
        }
    }
    catch (ex) { }
    alert("Esta função XmlHttp Object não é suportada pelo seu browser");
}

function fnc_coloca_foco_primeiro_campo() {
    // Coloca o focus no primeiro campo da página
    if (document.forms[0].length > 0) {
        for (var cont = 0; cont < document.forms[0].length; cont++) {
            campo = document.forms[0].elements[cont];
            if (campo.type == 'text') {
                campo.focus();
                break;
            }
        }
    }
}

// ====== Funções de criptografia ===============================================================================================
function fnc_base64_decode(data) {
    var b64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    var o1, o2, o3, h1, h2, h3, h4, bits, i = 0,
		ac = 0,
		dec = "",
		tmp_arr = [];

    if (!data) {
        return data;
    }

    data += '';

    do { // unpack four hexets into three octets using index points in b64
        h1 = b64.indexOf(data.charAt(i++));
        h2 = b64.indexOf(data.charAt(i++));
        h3 = b64.indexOf(data.charAt(i++));
        h4 = b64.indexOf(data.charAt(i++));

        bits = h1 << 18 | h2 << 12 | h3 << 6 | h4;

        o1 = bits >> 16 & 0xff;
        o2 = bits >> 8 & 0xff;
        o3 = bits & 0xff;

        if (h3 == 64) {
            tmp_arr[ac++] = String.fromCharCode(o1);
        } else if (h4 == 64) {
            tmp_arr[ac++] = String.fromCharCode(o1, o2);
        } else {
            tmp_arr[ac++] = String.fromCharCode(o1, o2, o3);
        }
    } while (i < data.length);

    dec = tmp_arr.join('');

    return dec;
}

function fnc_base64_encode(data) {
    var b64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    var o1, o2, o3, h1, h2, h3, h4, bits, i = 0,
		ac = 0,
		enc = "",
		tmp_arr = [];

    if (!data) {
        return data;
    }

    do { // pack three octets into four hexets
        o1 = data.charCodeAt(i++);
        o2 = data.charCodeAt(i++);
        o3 = data.charCodeAt(i++);

        bits = o1 << 16 | o2 << 8 | o3;

        h1 = bits >> 18 & 0x3f;
        h2 = bits >> 12 & 0x3f;
        h3 = bits >> 6 & 0x3f;
        h4 = bits & 0x3f;

        // use hexets to index into b64, and append result to encoded string
        tmp_arr[ac++] = b64.charAt(h1) + b64.charAt(h2) + b64.charAt(h3) + b64.charAt(h4);
    } while (i < data.length);

    enc = tmp_arr.join('');

    var r = data.length % 3;

    return (r ? enc.slice(0, r - 3) : enc) + '==='.slice(r || 3);
}

function fnc_avanca_retrodece_dias_uteis_feriado(data_inicial, dias_uteis, lista_time_UNIX_feriados, tipo_operacao) {
    // Milisegundos em um dia
    var milisegundos_dia = 86400000;

    // Array com os feriados
    var array_time_UNIX_feriados = lista_time_UNIX_feriados.split(',');

    // Time referente a data inicial fornecida pelo usuário (DDMMAA)
    var time_data = new Date(parseInt(Number(20 + data_inicial.substr(4, 2))), parseInt(Number(data_inicial.substr(2, 2))) - 1, parseInt(Number(data_inicial.substr(0, 2))), 0, 0, 0, 0);

    // Laço para avançar/retroceder
    while (dias_uteis > 0) {
        if (tipo_operacao == '+') // Avança um dia
        {
            time_data.setTime(time_data.getTime() + milisegundos_dia);
        }
        else // Retroceder um dia
        {
            time_data.setTime(time_data.getTime() - milisegundos_dia);
        }

        // Se não for nem SÁBADO, DOMINGO e feriado	
        if (time_data.getDay() != 0 && time_data.getDay() != 6 && fnc_in_array(time_data.getTime(), array_time_UNIX_feriados) == false) {
            dias_uteis--;
        }
    }

    var dia = Number(time_data.getDate());
    if (dia < 10) {
        dia = '0' + dia;
    }

    var mes = Number(time_data.getMonth()) + 1; // Meses em javascripr começam a partir do 0
    if (mes < 10) {
        mes = '0' + mes;
    }

    var ano = time_data.getFullYear().toString().substr(2, 2);

    return dia.toString() + mes.toString() + ano.toString();
}