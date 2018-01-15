function exibirParcelas(botao, nomeTabela)
{
    for (iTip = 0; iTip < 2; iTip++)
    {
        TagToTip(nomeTabela, FADEIN, 300, COPYCONTENT, false, TITLE, 'Parcelas', CLOSEBTN, true,
            CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true /*, 
            FIX, [botao, 9-getTableWidth(nomeTabela), -41-getTableHeight(nomeTabela)]*/);
    }
}

function habilitar(checkbox, valor, pagto)
{
    for (i = 1; i < pagto.options.length; i++)
        if (pagto.options[i].value == valor)
        {
            pagto.options[i].style.display = checkbox.checked ? "" : "none";
            break;
        }
    
    if (pagto.value == valor)
    {
        var selecionou = false;
        for (i = 1; i < pagto.options.length; i++)
            if (pagto.options[i].style.display == "")
            {
                pagto.selectedIndex = i;
                selecionou = true;
                break;
            }
        
        if (!selecionou)
            pagto.selectedIndex = 0;
    }
}

function validarParcelasUsar(val, args)
{
    var nomeControle = val.id.substr(0, val.id.indexOf("ctvParcelasUsar"));
    var parcelas = nomeControle + "cblParcelas";
    
    args.IsValid = false;
    eval(val.id).errormessage = "Selecione uma parcela para continuar.";
    
    var inputs = document.getElementById(parcelas).getElementsByTagName("input");
    for (i = 0; i < inputs.length; i++)
        if (inputs[i].checked)
        {
            args.IsValid = true;
            break;
        }
    
    /*
    if (args.IsValid)
    {
        eval(val.id).errormessage = "Selecione a forma de pagamento padrão para continuar.";
        args.IsValid = document.getElementById(nomeControle + "drpTipoPagto").selectedIndex > 0;
    }
    */
}