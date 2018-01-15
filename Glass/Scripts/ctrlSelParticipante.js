function ctrlSelParticipante_buscarPart(nomeControle, inicioUrl)
{
    var tipoPart = document.getElementById(nomeControle + "_drpPart").value;

    openWindow(600, 800, inicioUrl + (tipoPart == 0 ? "SelCliente.aspx" :
        tipoPart == 1 ? "SelFornec.aspx" : tipoPart == 2 ? "SelTransp.aspx" :
        tipoPart == 3 ? "SelLoja.aspx" : "SelAdminCartao.aspx") +
        "?callback=setForPart&controle=" + nomeControle);
}

function ctrlSelParticipante_setFornec(idFornec, nomeControle)
{
    ctrlSelParticipante_setPart(idFornec, nomeControle);
}

function ctrlSelParticipante_setCliente(idCliente, nomeControle)
{
    ctrlSelParticipante_setPart(idCliente, nomeControle);
}

function ctrlSelParticipante_setLoja(idLoja, nomeControle)
{
    ctrlSelParticipante_setPart(idLoja, nomeControle);
}

function ctrlSelParticipante_setTransp(idTransp, nomeControle)
{
    ctrlSelParticipante_setPart(idTransp, nomeControle);
}

function ctrlSelParticipante_setAdminCartao(idAdminCartao, nomeControle)
{
    ctrlSelParticipante_setPart(idAdminCartao, nomeControle);
}

function ctrlSelParticipante_setPart(idPart, nomeControle)
{
    var tipoPart = document.getElementById(nomeControle + "_drpPart").value;
    var resposta = ctrlSelParticipante.GetNomePart(tipoPart, idPart).value.split(';');

    if (resposta[0] == "Erro")
    {
        alert(resposta[1]);
        ctrlSelParticipante_limparPart(nomeControle);
        return;
    }

    document.getElementById(nomeControle + "_hdfIdPart").value = idPart;
    document.getElementById(nomeControle + "_lblDescrPart").innerHTML = resposta[1];
}

function ctrlSelParticipante_limparPart(nomeControle)
{
    document.getElementById(nomeControle + "_hdfIdPart").value = "";
    document.getElementById(nomeControle + "_lblDescrPart").innerHTML = "";
}

function ctrlSelParticipante_validaPart(val, args)
{
    var nomeControle = val.id.substr(0, val.id.lastIndexOf("_"));
    var idPart = document.getElementById(nomeControle + "_hdfIdPart").value;
    args.IsValid = idPart != "";
}