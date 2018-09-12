var isChrome = navigator.appVersion.indexOf(" Chrome/") > -1;

//------------------------------------------------------------------------------------
// Encontra um controle do tipo indicado na página que contenha o Id desejado.
//
// Parâmetros:
//   id - a parte do nome do Id do controle que será usada como filtro de busca.
//   tipo - o tipo da tag do controle (ex: input, select...)
//
// Retorno:
//   Um objeto representando o controle, se este for encontrado; caso contrário, nulo.
//------------------------------------------------------------------------------------
function FindControl(id, tipo, parent)
{
    parent = typeof parent == "object" ? parent : document;
    var listaControles = parent.getElementsByTagName(tipo);

    for (var i = 0; i < listaControles.length; i++)
        if (listaControles[i].id.indexOf(id) != -1)
            return listaControles[i];

    return null;
}

//------------------------------------------------------------------------
// Função de restrição de teclas.
// Só aceita digitação de teclas do sistema.
//
// Parâmetros:
//   e: window.event.
//
// Retorno:
//   Verdadeiro, se o caracter digitado for aceito; caso contrário, falso.
//------------------------------------------------------------------------
function soSistema(e, apagar)
{
    var key = (e.which) ? e.which : e.keyCode;
    apagar = apagar == false ? false : true;

    if ((e.ctrlKey && key != "v".charCodeAt(0) && key != "V".charCodeAt(0)) || e.altKey)
        return true;

    var backSpace = "\b".charCodeAt(0);
    var arrowLeft = 37;
    var arrowRight = 39;
    var arrowTop = 38;
    var arrowDown = 40;
    var esc = 27;
    var tab = 9; //"\t".charCodeAt(0);
    var shift = 16;
    var alt = 18;
    var ctrl = 17;
    var home = 36;
    var end = 35;
    var pageDown = 34;
    var pageUp = 33;
    var del = 46

    var ignore = [arrowLeft, arrowRight, arrowTop, arrowDown, esc, tab, shift, alt, ctrl, home, end, pageDown, pageUp];
    if (apagar)
    {
        ignore.push(backSpace);
        ignore.push(del);
    }

    var targ = null;

    if (e.target)
        targ = e.target;
    else if (e.srcElement)
        targ = e.srcElement;

    for (var i = 0; i < ignore.length; i++)
        if (key == ignore[i])
            return true;

    return false;
}

//------------------------------------------------------------------------
// Função de restrição de números.
// Só aceita digitação de números.
//
// Parâmetros:
//   e: window.event.
//   inteiro: O número deve ser inteiro?
//   naoNegativo: O número deve ser não-negativo?
//
// Retorno:
//   Verdadeiro, se o caracter digitado for aceito; caso contrário, falso.
//------------------------------------------------------------------------
function soNumeros(e, inteiro, naoNegativo)
{
    var key = (e.which) ? e.which : e.keyCode;

    var zero = "0".charCodeAt(0);
    var nove = "9".charCodeAt(0);

    var targ = null;

    if (e.target)
        targ = e.target;
    else if (e.srcElement)
        targ = e.srcElement;

    if (targ != null && targ.value.toString().indexOf(',') > -1 && key == ",".charCodeAt(0))
        return false;

    var retorno = (key >= zero && key <= nove) || soSistema(e);

    if (!inteiro)
        retorno = retorno || (key == ",".charCodeAt(0));

    if (!naoNegativo)
        retorno = retorno || (key == "-".charCodeAt(0));

    return retorno;
}

//------------------------------------------------------------------------
// Função de restrição de teclas.
// Só aceita digitação de teclas para o código interno do produto.
//
// Parâmetros:
//   e: window.event.
//
// Retorno:
//   Verdadeiro, se o caracter digitado for aceito; caso contrário, falso.
//------------------------------------------------------------------------
function soCodigoInterno(e)
{
    var key = (e.which) ? e.which : e.keyCode;

    var a = "a".charCodeAt(0);
    var A = "A".charCodeAt(0);
    var z = "z".charCodeAt(0);
    var Z = "Z".charCodeAt(0);
    var traco = "-".charCodeAt(0);
    var barra = "/".charCodeAt(0);

    return (key >= a && key <= z) || (key >= A && key <= Z) ||
        key == traco || key == barra || soNumeros(e, true, true);
}

//------------------------------------------------------------------------
// Função de restrição de data.
// Só aceita digitação de data.
//
// Parâmetros:
//   e: window.event.
//
// Retorno:
//   Verdadeiro, se o caracter digitado for aceito; caso contrário, falso.
//------------------------------------------------------------------------
function soData(e) {
    var key = (e.which) ? e.which : e.keyCode;

    var barra = "/".charCodeAt(0);

    var retorno = key == barra || soNumeros(e, true, true);

    return retorno;
}

//------------------------------------------------------------------------
// Função de restrição de hora.
// Só aceita digitação de hora.
//
// Parâmetros:
//   e: window.event.
//
// Retorno:
//   Verdadeiro, se o caracter digitado for aceito; caso contrário, falso.
//------------------------------------------------------------------------
function soHora(e) {
    var key = (e.which) ? e.which : e.keyCode;

    var doisPontos = ":".charCodeAt(0);

    var retorno = key == doisPontos || soNumeros(e, true, true);

    return retorno;
}

//------------------------------------------------------------------------
// Função de restrição de telefone.
// Só aceita digitação de caracteres que podem ser aceitos em um telefone.
//
// Parâmetros:
//   e: window.event.
//
// Retorno:
//   Verdadeiro, se o caracter digitado for aceito; caso contrário, falso.
//------------------------------------------------------------------------
function soTelefone(e)
{
    var key = (e.which) ? e.which : e.keyCode;

    var espaco = " ".charCodeAt(0);
    var abreParenteses = "(".charCodeAt(0);
    var fechaParenteses = ")".charCodeAt(0);
    var traco = "-".charCodeAt(0);

    var retorno = ((key == espaco) || (key == abreParenteses) || (key == fechaParenteses) || (key == traco));
    retorno = retorno || (soNumeros(e, true, true));

    return retorno;
}

//------------------------------------------------------------------------
// Função de restrição de CEP.
// Só aceita digitação de caracteres que podem ser aceitos em um CEP.
//
// Parâmetros:
//   e: window.event.
//
// Retorno:
//   Verdadeiro, se o caracter digitado for aceito; caso contrário, falso.
//------------------------------------------------------------------------
function soCep(e)
{
    var key = (e.which) ? e.which : e.keyCode;

    var traco = "-".charCodeAt(0);
    var ponto = ".".charCodeAt(0);

    var retorno = ((key == traco) || (key == ponto));
    retorno = retorno || (soNumeros(e, true, true));

    return retorno;
}

//------------------------------------------------------------------------
// Função de restrição de CPF/CNPJ.
// Só aceita digitação de caracteres que podem ser aceitos em um CPF/CNPJ.
//
// Parâmetros:
//   e: window.event.
//   cpf: O número será um CPF?
//
// Retorno:
//   Verdadeiro, se o caracter digitado for aceito; caso contrário, falso.
//------------------------------------------------------------------------
function soCpfCnpj(e, cpf)
{
    var key = (e.which) ? e.which : e.keyCode;

    var ponto = ".".charCodeAt(0);

    var retorno = (key == ponto);
    if (!cpf)
    {
        var traco = "-".charCodeAt(0);
        retorno = retorno || (key == traco);
    }

    retorno = retorno || (soNumeros(e, true, true));

    return retorno;
}

//------------------------------------------------------------------------
// Função de validação de Chave de Acesso.
// Verifica se a Chave de Acesso da NFe digitada é válida.
//
// Parâmetros:
//   val: (Passado pelo CustomValidator).
//   args:  (Passado pelo CustomValidator).
//
// Retorno:
//   (nenhum - O retorno é feito em args.IsValid)
//------------------------------------------------------------------------
function validarChaveAcesso(val, args) {
    var chave = args.Value;

    if (chave == "00000000000000000000000000000000000000000000") {
        args.IsValid = false;
        return;
    }

    if (chave.length != 44) {
        args.IsValid = false;
        return;
    }

    var multiplicadores = new Array(4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2);
    var numeros = chave.split("");

    var total = 0;
    for (var i = 0; i < 43; i++)
        total += parseInt(numeros[i]) * multiplicadores[i];

    total %= 11;
    var digitoVerificador = 0;
    if (total == 0 || total == 1) {
        digitoVerificador = 0;
    }
    else{
        digitoVerificador = 11 - total;
    }

    if (digitoVerificador == chave.substring(43)) {
        args.IsValid = true;
    }
    else {
        args.IsValid = false;
    }
}

//------------------------------------------------------------------------
// Função de validação de CPF.
// Verifica se um CPF digitado é válido.
//
// Parâmetros:
//   val: (Passado pelo CustomValidator).
//   args:  (Passado pelo CustomValidator).
//
// Retorno:
//   (nenhum - O retorno é feito em args.IsValid)
//------------------------------------------------------------------------
function validarCpf(val, args) {
    var cpf = args.Value.replace('.', '').replace('.', '').replace('-', '');

    if (cpf.length != 11) {
        args.IsValid = false;
        return;
    }

    var multiplicadores = new Array(11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1);
    var numeros = cpf.split("");

    if (numeros[0] == numeros[1] && numeros[1] == numeros[2] && numeros[2] == numeros[3] && numeros[3] == numeros[4] &&
        numeros[4] == numeros[5] && numeros[5] == numeros[6] && numeros[6] == numeros[7] && numeros[7] == numeros[8] &&
        numeros[8] == numeros[9]) {
        args.IsValid = false;
        return;
    }

    var total = 0;
    for (var i = 0; i < 9; i++)
        total += parseInt(numeros[i]) * multiplicadores[i + 1];

    total %= 11;

    if (total < 2) {
        if (parseInt(numeros[9]) != 0) {
            args.IsValid = false;
            return;
        }
    }
    else if (parseInt(numeros[9]) != (11 - total)) {
        args.IsValid = false;
        return;
    }

    total = 0;
    for (var i = 0; i < 10; i++)
        total += parseInt(numeros[i]) * multiplicadores[i];

    total %= 11;

    if (total < 2) {
        if (parseInt(numeros[10]) != 0) {
            args.IsValid = false;
            return;
        }
    }
    else if (parseInt(numeros[10]) != (11 - total)) {
        args.IsValid = false;
        return;
    }
}

//------------------------------------------------------------------------
// Função de validação de CNPJ.
// Verifica se um CNPJ digitado é válido.
//
// Parâmetros:
//   val: (Passado pelo CustomValidator).
//   args:  (Passado pelo CustomValidator).
//
// Retorno:
//   (nenhum - O retorno é feito em args.IsValid)
//------------------------------------------------------------------------
function validarCnpj(val, args) {
    var cnpj = args.Value.replace('.', '').replace('.', '').replace('.', '').replace('-', '').replace('/', '');

    if (cnpj.length != 14) {
        args.IsValid = false;
        return;
    }

    var multiplicadores = new Array(6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2);
    var numeros = cnpj.split("");

    var total = 0;
    for (var i = 0; i < 12; i++)
        total += parseInt(numeros[i]) * multiplicadores[i + 1];
    total %= 11;

    if (total < 2) {
        if (parseInt(numeros[12]) != 0) {
            args.IsValid = false;
            return;
        }
    }
    else if (parseInt(numeros[12]) != (11 - total)) {
        args.IsValid = false;
        return;
    }

    var total = 0;
    for (var i = 0; i < 13; i++)
        total += parseInt(numeros[i]) * multiplicadores[i];
    total %= 11;

    if (total < 2) {
        if (parseInt(numeros[13]) != 0) {
            args.IsValid = false;
            return;
        }
    }
    else if (parseInt(numeros[13]) != (11 - total)) {
        args.IsValid = false;
        return;
    }
}

//------------------------------------------------------------------------
// Função de validação de CPF/CNPJ.
// Verifica se um CPF/CNPJ digitado é válido.
//
// Parâmetros:
//   val: (Passado pelo CustomValidator).
//   args:  (Passado pelo CustomValidator).
//
// Retorno:
//   (nenhum - O retorno é feito em args.IsValid)
//------------------------------------------------------------------------
function validarCpfCnpj(val, args) {
    var cpfCnpj = args.Value.replace('.', '').replace('.', '').replace('.', '').replace('-', '').replace('/', '');

    var cpf = (cpfCnpj.length == 11);

    if (cpf)
    {
        val.errormessage = "CPF inválido.";
        val.innerHTML = val.errormessage;
        return validarCpf(val, args);
    }
    else
    {
        val.errormessage = "CNPJ inválido.";
        val.innerHTML = val.errormessage;
        return validarCnpj(val, args);
    }
}

//------------------------------------------------------------------------
// Função de validação de e-mail.
// Verifica se um e-mail digitado é válido.
//
// Parâmetros:
//   val: (Passado pelo CustomValidator).
//   args:  (Passado pelo CustomValidator).
//
// Retorno:
//   (nenhum - O retorno é feito em args.IsValid)
//------------------------------------------------------------------------
function validaEmailControl(val, args)
{
    args.IsValid = validaEmail(args.Value);
}

function validaEmail(email) {
    var filtro = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
    return filtro.test(email)
}

//------------------------------------------------------------------------
// Função de validação de números.
// Verifica se um número digitado é válido.
//
// Parâmetros:
//   val: (Passado pelo CustomValidator).
//   args:  (Passado pelo CustomValidator).
//
// Retorno:
//   (nenhum - O retorno é feito em args.IsValid)
//------------------------------------------------------------------------
function validarNumero(val, args)
{
    args.IsValid = !isNaN(args.Value.replace(/,/g, "."));
}

// Verifica se o valor do control está no formato MM/yyyy
function validaMesAno(control) {
    if (control == null || control.value == "" || control.value.length != 7 || control.value.indexOf('/') == -1) {
        alert("Mês/Ano Inválidos.");
        control.focus();
        return false;
    }

    var mes = control.value.substring(0, 2);
    var ano = parseInt(control.value.substring(3, 7));

    if (mes == 0 || mes > 12) {
        alert("Mês/Ano Inválidos.");
        control.focus();
        return false;
    }

    if (ano < 1900 || ano > 2300) {
        alert("Mês/Ano Inválidos.");
        control.focus();
        return false;
    }

    return true;
}

//Máscara para Telefone
function maskTelefone(e, txt) {
    // Pega a tecla que foi pressionada
    var key = (e.which) ? e.which : e.keyCode;

    // Se o backspace tiver sido pressionado, não faz nada
    if (key == "\b".charCodeAt(0))
        return;

    /*
    if (!soTelefone(e))
        return false;
    */

    var strTelefone = '';
    var prefixo = '';
    var incluirNonoDigito = false;

    strTelefone = strTelefone + txt.value;

    // Máscara para 0800
    if (strTelefone.length >= 1 && strTelefone[0] == "0") {
        if (strTelefone.length == 4 || strTelefone.length == 8) {
            strTelefone = strTelefone + ' ';
            txt.value = strTelefone;
        }

        txt.maxLength = 13;
    }
    // Máscara para telefone com ddd, com o 9º dígito ou não
    else {
        if (strTelefone.length == 1) {
            strTelefone = '(' + strTelefone;
            txt.value = strTelefone;
        }

        if (strTelefone.length == 3) {
            strTelefone = strTelefone + ') ';
            txt.value = strTelefone;
        }

        if (strTelefone.length >= 3) {
            prefixo = strTelefone.substring(1, 3);

            incluirNonoDigito =
                prefixo == "11" ||
                prefixo == "12" ||
                prefixo == "13" ||
                prefixo == "14" ||
                prefixo == "15" ||
                prefixo == "16" ||
                prefixo == "17" ||
                prefixo == "18" ||
                prefixo == "19" ||
                prefixo == "21" ||
                prefixo == "22" ||
                prefixo == "24" ||
                prefixo == "27" ||
                prefixo == "28" ||
                prefixo == "31" ||
                prefixo == "32" ||
                prefixo == "33" ||
                prefixo == "34" ||
                prefixo == "35" ||
                prefixo == "37" ||
                prefixo == "38" ||
                prefixo == "41" ||
                prefixo == "42" ||
                prefixo == "43" ||
                prefixo == "44" ||
                prefixo == "45" ||
                prefixo == "46" ||
                prefixo == "47" ||
                prefixo == "48" ||
                prefixo == "49" ||
                prefixo == "51" ||
                prefixo == "52" ||
                prefixo == "53" ||
                prefixo == "54" ||
                prefixo == "55" ||
                prefixo == "61" ||
                prefixo == "62" ||
                prefixo == "63" ||
                prefixo == "64" ||
                prefixo == "65" ||
                prefixo == "66" ||
                prefixo == "67" ||
                prefixo == "68" ||
                prefixo == "69" ||
                prefixo == "71" ||
                prefixo == "72" ||
                prefixo == "73" ||
                prefixo == "74" ||
                prefixo == "75" ||
                prefixo == "77" ||
                prefixo == "79" ||
                prefixo == "81" ||
                prefixo == "82" ||
                prefixo == "83" ||
                prefixo == "84" ||
                prefixo == "85" ||
                prefixo == "87" ||
                prefixo == "88" ||
                prefixo == "91" ||
                prefixo == "92" ||
                prefixo == "93" ||
                prefixo == "94" ||
                prefixo == "95" ||
                prefixo == "96" ||
                prefixo == "97" ||
                prefixo == "98" ||
                prefixo == "99";
    }

    if (strTelefone.length == 9) {
            if (!incluirNonoDigito || strTelefone.substring(5, 6) != "9") {
                strTelefone = strTelefone + '-';
                txt.value = strTelefone;
            }
        }
        else if (strTelefone.length == 10) {
            if (incluirNonoDigito && strTelefone.substring(5, 6) == "9") {
                strTelefone = strTelefone + '-';
                txt.value = strTelefone;
            }
        }

        txt.maxLength = incluirNonoDigito && strTelefone.substring(5, 6) == "9" ? 15 : 14;

        if (strTelefone.length == 14 && txt.maxLength == 14) {
            strTelefone = strTelefone.replace('-', '');
            strTelefone = strTelefone.substring(0, 09) + '-' + strTelefone.substring(09);
            txt.value = strTelefone;
        }
        else if (strTelefone.length == 15 && txt.maxLength == 15) {
            strTelefone = strTelefone.replace('-', '');
            strTelefone = strTelefone.substring(0, 10) + '-' + strTelefone.substring(10);
            txt.value = strTelefone;
        }
    }
}

//Máscara para CEP
function maskCep(e, objTextBox) {
    // Pega a tecla que foi pressionada
    var key = (e.which) ? e.which : e.keyCode;

    // Se o backspace tiver sido pressionado, não faz nada
    if (key == "\b".charCodeAt(0))
        return;

    var v = objTextBox.value;

    v = v.replace(/D/g, "");
    v = v.replace(/^(\d{5})(\d)/, "$1-$2");

    objTextBox.value = v;
}

//Máscara para CPF
function maskCPF(e, txt) {
    // Pega a tecla que foi pressionada
    var key = (e.which) ? e.which : e.keyCode;

    // Se o backspace tiver sido pressionado, não faz nada
    if (soSistema(e, true))
        return;

    var mycpf = '';
    mycpf = mycpf + txt.value;
    if (mycpf.length == 3) {
        mycpf = mycpf + '.';
        txt.value = mycpf;
    }
    if (mycpf.length == 7) {
        mycpf = mycpf + '.';
        txt.value = mycpf;
    }
    if (mycpf.length == 11) {
        mycpf = mycpf + '-';
        txt.value = mycpf;
    }
    if (mycpf.length == 14)
    {
        return false;
    }
}

//Máscara para CNPJ
function maskCNPJ(e, txt) {
    // Pega a tecla que foi pressionada
    var key = (e.which) ? e.which : e.keyCode;

    // Se o backspace tiver sido pressionado, não faz nada
    if (soSistema(e, true))
        return;

    var mycnpj = '';
    mycnpj = mycnpj + txt.value;
    if (mycnpj.length == 2) {
        mycnpj = mycnpj + '.';
        txt.value = mycnpj;
    }
    if (mycnpj.length == 6) {
        mycnpj = mycnpj + '.';
        txt.value = mycnpj;
    }
    if (mycnpj.length == 10) {
        mycnpj = mycnpj + '/';
        txt.value = mycnpj;
    }
    if (mycnpj.length == 15) {
        mycnpj = mycnpj + '-';
        txt.value = mycnpj;
    }
    if (mycnpj.length == 18)
    {
        return false;
    }
}

// Utilizado para selecionar a data do componente de data
function SelecionaData(nomeCampo, botao)
{
    try
    {
        var campo = FindControl(nomeCampo, "input");
        if (campo != null)
            displayCalendar(campo, 'dd/mm/yyyy', botao);
    }
    catch(err) {
        alert(err);
        return false;
    }

    return false;
}

// Utilizado para selecionar a data/hora do componente de data
function SelecionaDataHora(nomeCampo, botao, horaPadrao, minutoPadrao) {
    try {
        var campo = FindControl(nomeCampo, "input");
        if (campo != null)
            displayCalendar(campo, 'dd/mm/yyyy hh:ii', botao, true, horaPadrao, minutoPadrao);
    }
    catch (err) {
        return false;
    }

    return false;
}

//-----------------------------------------------------
// Máscara de data
//-----------------------------------------------------
function mascara_mesAno(e, data)
{
    var key = (e.which) ? e.which : e.keyCode;
    if (key == 6)
        return true;

    var mydata = '';
    mydata = mydata + data.value;
    if (mydata.length == 2) {
        mydata = mydata + '/';
        data.value = mydata;
    }
}

function mascara_data(e, data)
{
    var key = (e.which) ? e.which : e.keyCode;
    if (key == 8)
        return true;

    var mydata = '';
    mydata = mydata + data.value;
    if (mydata.length == 2) {
        mydata = mydata + '/';
        data.value = mydata;
    }

    if (mydata.length == 5) {
        mydata = mydata + '/';
        data.value = mydata;
    }
}

function mascara_hora(e, hora)
{
    var key = (e.which) ? e.which : e.keyCode;
    if (key == 8)
        return true;

    var myhora = '';
    myhora = myhora + hora.value;

    if (myhora.length == 2) {
        myhora = myhora + ':';
        hora.value = myhora;
    }
}

function verifica_data(data) {
    situacao = "";

    if (data.length != 10)
        situacao = "falsa";

    dia = (data.substring(0, 2));
    mes = (data.substring(3, 5));
    ano = (data.substring(6, 10));

    // verifica o dia valido para cada mes
    if ((dia < 01) || (dia < 01 || dia > 30) && (mes == 04 || mes == 06 || mes == 09 || mes == 11) || dia > 31)
        situacao = "falsa";

    // verifica se o mes e valido
    if (mes < 01 || mes > 12)
        situacao = "falsa";

    // verifica se e ano bissexto
    if (mes == 2 && (dia < 01 || dia > 29 || (dia > 28 && (parseInt(ano / 4) != ano / 4))))
        situacao = "falsa";

    if (data.value == "")
        situacao = "falsa";

    if (situacao == "falsa") {
        alert("Data inválida!");
        //data.focus();
        return false;
    }

    return true;
}

function verifica_hora(hora) {
    situacao = "";

    if (hora.length != 5)
        situacao = "falsa";

    hrs = (hora.substring(0, 2));
    min = (hora.substring(3, 5));

    // verifica data e hora
    if ((hrs < 00) || (hrs > 23) || (min < 00) || (min > 59))
        situacao = "falsa";

    if (hora.value == "")
        situacao = "falsa";

    if (situacao == "falsa") {
        alert("Hora inválida!");
        //hora.focus();
        return false;
    }
    else
        return true;
}

// Utilizado para preencher dados do cliente selecionado após
// escolher o mesmo no popup
function setCliente(idCliente, nomeCliente) {
    var numCli = FindControl("txtNumCli", "input");
    var nomeCli = FindControl("txtNomeCliente", "input");
    var hdfCli = FindControl("hdfCliente", "input");

    if (numCli)
        numCli.value = idCliente;

    if (nomeCli)
        nomeCli.value = nomeCliente;

    if (hdfCli)
        hdfCli.value = idCliente;
}

// Utilizado para preencher dados do fornecedor selecionado após
// escolher o mesmo no popup
function setFornec(idFornec, nomeFantasia, idConta)
{
    FindControl("txtNomeFornec", "input").value = nomeFantasia;

    var hdfFornec = FindControl("hdfFornec", "input");
    if (hdfFornec != null)
        hdfFornec.value = idFornec;

    var txtNumFornec = FindControl("txtNumFornec", "input")
    if (txtNumFornec != null)
        txtNumFornec.value = idFornec;

    var ddlPlanoConta = FindControl("ddlPlanoConta", "select");
    if (ddlPlanoConta != null && idConta != null)
        ddlPlanoConta.value = idConta;
}

// Subtrai os dois valores passados por parâmetro, substituindo caracteres
// "R$", "." e convertendo para Number
function subtract(value1, value2)
{
    var num1 = value1 == '' ? 0 : new Number(value1.toString().replace("R$", "").replace(" ", "").replace(".", "").replace(",", ".")).toFixed(2);
    var num2 = value2 == '' ? 0 : new Number(value2.toString().replace("R$", "").replace(" ", "").replace(".", "").replace(",", ".")).toFixed(2);
    var result = num1 - num2;

    return result;
}

function find(controlName)
{
    return document.getElementById(controlName);
}

var controlePopup = [];

// Função para abrir popup
function openWindow(altura, largura, url, opener, exibirBotaoFechar, abrirNovaJanela)
{
    while (controlePopup.length > 0)
        closeWindow();

    abrirNovaJanela = abrirNovaJanela == false ? false : true;
    opener = !!opener ? opener : window;

    // Ajuste no tamanho por conta do layout
    var prop = largura / altura;
    altura += 75;
    largura += (75 * prop);

    // Verifica se é um popup dentro de popup
    var popupSec = !abrirNovaJanela && opener != window.top;

    if (!abrirNovaJanela && opener.document.body.style.display == "none")
    {
        setTimeout(function() { openWindow(altura, largura, url, opener, abrirNovaJanela); }, 100);
        return;
    }

    var alturaMax = !abrirNovaJanela ? opener.innerHeight - 60 : screen.availHeight;
    var larguraMax = !abrirNovaJanela ? opener.innerWidth - 60 : screen.availWidth;

    // Corrige o tamanho se for um popup dentro de outro
    if (popupSec)
    {
        alturaMax = opener.innerHeight + 7;
        larguraMax = opener.innerWidth - 3;
    }

    var maximizar = altura >= alturaMax || largura >= larguraMax;

    if (altura > alturaMax) altura = alturaMax;
    if (largura > larguraMax) largura = larguraMax;

    if (abrirNovaJanela && !maximizar)
        altura += 30;

    var scrY = (alturaMax - altura) / 2;
    var scrX = (larguraMax - largura) / 2;

    if (abrirNovaJanela && maximizar && isChrome)
    {
        altura -= 68;
        largura -= 20;
    }

    if (abrirNovaJanela)
    {
        return opener.open(url, "popup_" + new Date().getMilliseconds(), 'scrollbars=1, width=' + largura + ', ' +
            'height=' + altura + ', left=' + scrX + ', top=' + scrY);
    }
    else
    {
        // Recupera o controle de popup
        var p = FindControl("popupAbrir", "div", opener.document).id.replace("_blanket", "");
        p = eval("opener." + p);

        // Soma um valor na altura por conta do botão "Fechar",
        // "escondendo" o popup pai se necessário
        exibirBotaoFechar = exibirBotaoFechar == false ? false : true;
        var somar = exibirBotaoFechar ? 20 : 0;
        p.Abrir(url, altura + somar, largura, exibirBotaoFechar);

        // Inclui na pilha
        controlePopup.push(p);
    }

    return false;
}

// Função para abrir popup, retornando o popup aberto
function openWindowRet(altura, largura, url) {
    var alturaMax = window.innerHeight - 60;
    var larguraMax = window.innerWidth - 60;
    var scrY = (alturaMax - altura) / 2;
    var scrX = (larguraMax - largura) / 2;

    return window.open(url, "popup_" + new Date().getMilliseconds(), 'scrollbars=1, width=' + largura + ', ' +
        'height=' + altura + ', left=' + scrX + ', top=' + scrY);

    return false;
}

// Função para fechar o popup
function closeWindow()
{
    if (controlePopup.length > 0)
    {
        var p = controlePopup.pop();
        p.Fechar(0);
        p = null;
    }
    else if (window == window.top)
        window.close();
    else if (window.opener != null)
        window.opener.closeWindow();
}

function triggerEvent(element, eventName)
{
    var evt = document.createEvent("Event");
    evt.initEvent(eventName, false, false);
    element.dispatchEvent(evt);
}

function redirectUrl(url)
{
    // Desabilita o evento onbeforeunload do controle de
    // popup, se a tela for um popup
    if (window != window.top)
    {
        for (var i = 0; i < window.opener.controlePopup.length; i++)
            triggerEvent(window.opener.controlePopup[i].Pagina(0, true).document.forms[0], "submit");
    }

    window.location.href = url;
}

// Função que verifica se a data passada é maior que a data atual
// Params:
//     date -> Data (dd/mm/yyyy)
// Return:
//     True -> date > today
//     False -> date < today
function dateGreaterThenNow(date) {
    date = date.split("/");

    var today = new Date();
    var day = today.getDate().toString();
    var month = (today.getMonth() + 1).toString();
    var year = today.getFullYear().toString();

    if (day.length == 1)
        day = "0" + day;

    if (month.length == 1)
        month = "0" + month;

    return parseInt(date[2].toString() + date[1].toString() + date[0].toString()) >=
        parseInt(year + month + day);
}

// True: A primeira é maior que a segunda
// False: A segunda é maior que a primeira
function firstGreaterThenSec(date1, date2) {
    var data1 = date1.split("/");
    var data2 = date2.split("/");

    if (data1[0].length == 1)
        data1[0] = "0" + data1[0];

    if (data2[0].length == 1)
        data2[0] = "0" + data2[0];

    if (data1[1].length == 1)
        data1[1] = "0" + data1[1];

    if (data2[1].length == 1)
        data2[1] = "0" + data2[1];

    return parseInt(data1[2].toString() + data1[1].toString() + data1[0].toString()) >
        parseInt(data2[2].toString() + data2[1].toString() + data2[0].toString());
}

// True: A primeira é maior que a segunda
// False: A segunda é maior que a primeira
function firstEqualOrGreaterThenSec(date1, date2) {
    var data1 = date1.split("/");
    var data2 = date2.split("/");

    if (data1[0].length == 1)
        data1[0] = "0" + data1[0];

    if (data2[0].length == 1)
        data2[0] = "0" + data2[0];

    if (data1[1].length == 1)
        data1[1] = "0" + data1[1];

    if (data2[1].length == 1)
        data2[1] = "0" + data2[1];

    return parseInt(data1[2].toString() + data1[1].toString() + data1[0].toString()) >=
        parseInt(data2[2].toString() + data2[1].toString() + data2[0].toString());
}

// Verificar se o enter foi pressionado
function isEnter(e) {
    return e.keyCode == 13;
}

function atualizarPagina()
{
    if (!!window["atualizarPaginaBotao"])
        atualizarPaginaBotao();
    else if (!!window["__doPostBack"])
        __doPostBack(null, null);
    else
        redirectUrl(window.location.href);
}

// Função utilizada para dar reload na pagina quando o <Enter> for pressionado
function onEnter(evento) {
    if (isEnter)
    {
        atualizarPagina();
        return false;
    }
}

// Retorna o formulário
function getForm() {
    var formObj = find("aspnetForm");

    if (formObj == null)
        formObj = find("form1");

    return formObj;
}

// Realiza submit se o enter tiver sido pressionado
function submitOnEnter(e) {
    if (isEnter(e)) {
        var formObj = getForm();

        if (formObj != null)
            formObj.submit();
    }
}

// Dispara o evento onClick do controle passado
function cOnClick(controlName, type) {
    try {
        FindControl(controlName, type == null ? "input" : type).click();
    }
    catch (err) {
        alert(err);
        return false;
    }
}

//-----------------------------------------------------------------
// Faz a pesquisa de endereço pelo CEP.
//
// Parâmetros:
//   cep - o número do CEP que será pesquisado.
//   cTipoLogradouro - o controle que receberá o tipo do logradouro
//   cLogradouro - o controle que receberá o logradouro
//   cBairro - o controle que receberá o bairro
//   cCidade - o controle que receberá a cidade
//   cUf - o controle que receberá a UF
//
// Retorno:
//   (nenhum).
//-----------------------------------------------------------------
function pesquisarCep(cep, cTipoLogradouro, cLogradouro, cBairro, cCidade, cUf, cIdCidade)
{
    var resposta = MetodosAjax.PesquisarCep(cep).value;
    var dadosResposta = resposta.split('|');

    if (dadosResposta[0] == "Erro")
    {
        alert(dadosResposta[1]);
        return;
    }

    if (cTipoLogradouro != null) cTipoLogradouro.value = dadosResposta[2];
    if (cLogradouro != null) cLogradouro.value = (cTipoLogradouro == null ? dadosResposta[2] + " " : "") + dadosResposta[3];
    if (cBairro != null) cBairro.value = dadosResposta[4];
    if (cCidade != null) cCidade.value = dadosResposta[5];
    if (cUf != null) cUf.value = dadosResposta[6];
    if (cIdCidade != null) cIdCidade.value = dadosResposta[7] != "" && dadosResposta[7] != "0" ? dadosResposta[7] : "";
}

// ---------------------------------------
// Função que retorna a largura da tabela.
// ---------------------------------------
function getTableWidth(id)
{
    var tabela = document.getElementById(id);
    var display = tabela.style.display;
    tabela.style.display = "";
    var retorno = tabela.clientWidth;
    tabela.style.display = display;
    return retorno;
}

// --------------------------------------
// Função que retorna a altura da tabela.
// --------------------------------------
function getTableHeight(id)
{
    var tabela = document.getElementById(id);
    var display = tabela.style.display;
    tabela.style.display = "";
    var retorno = tabela.clientHeight;
    tabela.style.display = display;
    return retorno;
}

// -------------------------------------------------------
// Função que remove os espaços do início e fim do string.
// -------------------------------------------------------
function Trim(str) {
    if (!str) return str;
    return str.toString().replace(/^\s+|\s+$/g, "");
}

// -------------------------------------------------------
// Simula a tecla tab
// -------------------------------------------------------
function nextField(current)
{
    for (i = 0; i < current.form.elements.length; i++)
    {
        if (current.form.elements[i].tabIndex == current.tabIndex + 1)
        {
            current.form.elements[i].focus();
            if (current.form.elements[i].type == "text")
                current.form.elements[i].select();
        }
    }
}

// --------------------------------------------------------------------------
// Função que faz a validação dos controles do ASP, se existir algum na tela.
// --------------------------------------------------------------------------
function validate(validationGroup)
{
    if (typeof Page_ClientValidate == 'function')
    {
        if (typeof Page_ValidationSummaries == 'object')
        {
            var tamanho = Page_ValidationSummaries.length;
            if (tamanho > 0)
            {
                var temp = new Array();
                for (i = 0; i < tamanho; i++)
                    if (Page_ValidationSummaries[i] != null && typeof Page_ValidationSummaries[i] != 'undefined')
                        temp.push(Page_ValidationSummaries[i]);

                Page_ValidationSummaries = temp;
            }
        }

        if (!Page_ClientValidate(validationGroup))
            return false;
    }

    return true;
}

// ----------------------------------------
// Função que converte de string para data.
// ----------------------------------------
function toDate(string)
{
    var data = string.split(' ');
    var partesData = data[0].split('/');
    var partesHora = data.length > 1 ? data[1].split(':') : null;

    var dia = partesData[0];
    var mes = parseInt(partesData[1], 10) - 1;
    var ano = partesData[2];

    var horas = partesHora != null ? partesHora[0] : 0;
    var minutos = partesHora != null ? partesHora[1] : 0;
    var seguntos = partesHora != null && partesHora.length > 2 ? partesHora[2] : 0;

    var retorno = new Date(ano, mes, dia, horas, minutos, seguntos);

    // Corrige um erro misterioso ao converter a string para datetime, no qual caso fosse dia
    // 20/10, ao converter a data ficava 19/10
    if (dia == 20 && retorno.getDate() == 19)
        retorno.setDate(20);

    return retorno;
}

// ---------------------------------------
// Função que verifica se a data é válida.
// ---------------------------------------
function isDataValida(string)
{
    try
    {
        var dadosData = string.split(" ");
        var temHoras = dadosData.length > 1;

        var temp = dadosData[0].split("/");
        if (temHoras)
        {
            var horasTemp = dadosData[1].split(":");
            for (i = 0; i < horasTemp.length; i++)
                temp.push(horasTemp[i]);
        }

        for (i = 0; i < temp.length; i++)
            if (!isNaN(temp[i]) && parseInt(temp[i], 10) < 10)
                temp[i] = "0" + parseInt(temp[i], 10);

        string = temp[0] + "/" + temp[1] + "/" + temp[2];
        if (temHoras)
        {
            string += " " + temp[3] + ":" + temp[4];
            if (temp.length > 5)
                string += ":" + temp[5];
        }

        var teste = toDate(string);

        if (Object.prototype.toString.call(teste) !== "[object Date]" )
            return false;

        var resultado = (teste.getDate() < 10 ? "0" : "") + teste.getDate() + "/" +
            ((teste.getMonth() + 1) < 10 ? "0" : "") + (teste.getMonth() + 1) + "/" +
            teste.getFullYear();

        if (temHoras)
            resultado += " " + (teste.getHours() < 10 ? "0" : "") + teste.getHours() + ":" +
                (teste.getMinutes() < 10 ? "0" : "") + teste.getMinutes() + (temp.length > 5 ?
                ":" + (teste.getSeconds() < 10 ? "0" : "") + teste.getSeconds() : "");

        // Código necessário para corrigir erro misterioso no javascript, no qual caso o dia da variável "string" seja 21,
        // ao efetuar a conversão acima (var teste = toDate(string);) o dia é convertido para o dia 20
        if (string.toString().indexOf('21') == 0 && resultado.toString().indexOf('20') == 0)
            resultado = resultado.toString().replace('20', '21');

        if (string.toString().indexOf('19') == 0 && resultado.toString().indexOf('18') == 0)
            resultado = resultado.toString().replace('18', '19');

        return !isNaN(teste.getTime()) && string == resultado;
    }
    catch (err)
    {
        return false;
    }
}

// ---------------------------------------
// Função que verifica se a hora é válida.
// ---------------------------------------
function isHoraValida(string)
{
    var data = new Date();
    data = (data.getDate() < 10 ? "0" : "") + data.getDate() + "/" +
        ((data.getMonth() + 1) < 10 ? "0" : "") + (data.getMonth() + 1) + "/" +
        data.getFullYear() + " " + string;

    return isDataValida(data);
}

// ---------------------------------------------
// Função que calcula a diferença entre 2 datas.
// ---------------------------------------------
function diferencaDatas(data1, data2)
{
    var dia = 1000 * 60 * 60 * 24;
    return Math.ceil((data1.getTime() - data2.getTime()) / dia);
}

// --------------------------------------------------------
// Função que trata leitura de código de barras da produção
// --------------------------------------------------------
function corrigeLeituraEtiqueta(codBarras) {

    if (codBarras.toString().indexOf(".cni") >= 0)
        codBarras = codBarras.toString().substring(0, codBarras.toString().indexOf(".cni"));

    if (codBarras.toString().indexOf("-") <= 0)
        codBarras = codBarras.replace('F', '_').replace('f', '_');

    if (codBarras.toString().indexOf("c") > 2)
        codBarras = codBarras.toString().replace('c', '_');

    /* Chamado 31382. Samuel.
       Dessa forma somente a etiqueta concatenada com espessura, altura e largura irá substituir o 'C' para '_',
       não interferindo no plano de corte onde o 'C' está sempre na posição 0. */
    if (codBarras.toString().indexOf("C") > 2)
        codBarras = codBarras.toString().replace('C', '_');

    if (codBarras.toString().indexOf("s") > 0)
        codBarras = codBarras.toString().replace('s', '_');

    if (codBarras.toString().indexOf("S") > 0)
        codBarras = codBarras.toString().replace('S', '_');

    // Caso a empresa concatene a espessura, altura e largura na etiqueta, remove estas informações
    if (codBarras.toString().indexOf("_") > 0) {
        codBarras = codBarras.split("_");
        codBarras = codBarras[codBarras.length-1];
    }

    codBarras = codBarras.replace('A', '.').replace('B', '/').replace('D', '-')

    return codBarras.toString().replace('a', '.').replace('b', '/').replace('d', '-').replace('ç', '/').replace('Ç', '/').replace(';', '/').replace("ò", '/').replace("'", '-');
}

// -----------------------------------------------------------------------
// Atualiza os validadores da tela, mudando a função (evaluationfunction).
// -----------------------------------------------------------------------
function atualizaValidadores()
{
    if (typeof(Page_Validators) != "undefined")
        for (var i = 0; i < Page_Validators.length; i++)
            if (typeof Page_Validators[i].evaluationfunction == "string")
                Page_Validators[i].evaluationfunction = this[Page_Validators[i].evaluationfunction];
}


function mascara_periodo(e, text) {
    var key = (e.which) ? e.which : e.keyCode;
    if (key == 6)
        return true;

    if (key >= 48 && key <= 57 || key >= 96 && key <= 105) {
        var mydata = '';
        mydata = mydata + text.value;
        if (mydata.length == 2) {
            if (mydata >= 1 && mydata <= 12) {
                mydata = mydata + '/';
                text.value = mydata;
            }
            else {
                alert("Informe um valor válido para mês");
                text.value = "";
                return false;
            }

        }
    }
    else if (!(key >= 48 && key <= 57 || key >= 96 && key <= 105) && key != 8 && key != 46 && key != 37 && key != 39) {
        return false;
    }

    if (mydata.length >= 7) {
        text.value = mydata.substring(0, 7);
        return false;
    }
}

function GetQueryString(name, url) {
  if (!url) {
    url = window.location.href
  }

  name = name.replace(/[\[\]]/g, "\\$&")

  var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)", "gi")
  var results = regex.exec(url)

  if (!results || !results[2]) {
    return ''
  }

  return decodeURIComponent(results[2].replace(/\+/gi, " "))
}

// -----------------------------------------------------------------------
// Verifica se um Guid é valido.
// -----------------------------------------------------------------------
function isGUID(objGuid) {

    var str = (objGuid);
    var reEmail = /^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$/

    if (objGuid == "")
        return false;
    else if (reEmail.test(str))
        return true;
    else
        return false;
}

// -----------------------------------------------------------------------
// Lê o código do cheque e preenche os campos
// -----------------------------------------------------------------------
function verificaLeituraCheque(controle, e) {
  var retLeitura = controle.value.toString().trim();

  if (retLeitura.indexOf("<") == -1)
    return false;

  retLeitura = retLeitura.substring(retLeitura.indexOf("<"), retLeitura.length);

  var leuCheque =
    retLeitura.substring(retLeitura.length - 1) == ":" ||
    retLeitura.substring(retLeitura.length - 1) == "Ç" ||
    retLeitura.substring(retLeitura.length - 1) == "ç" ||
    retLeitura.substring(retLeitura.length - 1) == "?";

  if (leuCheque) {
    controle.value = controle.value.toString().substring(0, controle.value.length - retLeitura.length - 1).trim();

    var codBanco = retLeitura.substring(1, 4);
    var numeroCheque = retLeitura.substring(13, 19);
    var digitoVerificadorCheque = 0;

    FindControl("txtBanco", "input").value = codBanco;
    FindControl("txtAgencia", "input").value = retLeitura.substring(4, 8);
    FindControl("txtNumero", "input").value = numeroCheque;

    for (var i = 0; i < numeroCheque.length; i++) {
      digitoVerificadorCheque += (7 - i) * numeroCheque.charAt(i);
    }

    var resto = digitoVerificadorCheque % 11;

    if (resto == 0 || resto == 1)
      digitoVerificadorCheque = 0;
    else
      digitoVerificadorCheque = 11 - resto;

    FindControl("txtDigitoNum", "input").value = digitoVerificadorCheque;

    var conta = "";

    switch (codBanco) {
      case "001":
      case "033":
      case "237":
      case "341":
      case "399":
      case "748": conta = retLeitura.substring(26, 31) + "-" + retLeitura.substring(31, 32); break;
      case "409": conta = retLeitura.substring(25, 31) + "-" + retLeitura.substring(31, 32); break;
      case "356": conta = retLeitura.substring(24, 31) + "-" + retLeitura.substring(31, 32); break;
      case "104":
      case "320":
      case "389": conta = retLeitura.substring(23, 31) + "-" + retLeitura.substring(31, 32); break;
      default:
        conta = retLeitura.substring(23, 31) + "-" + retLeitura.substring(31, 32); break;
      }

      FindControl("txtConta", "input").value = conta;
      FindControl("txtCMC7", "input").value = retLeitura.substring(1, retLeitura.length - 1);
    }

    return false;
}
