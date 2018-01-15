function arredondaAltura(controle, tipoCalculo)
{
    if (tipoCalculo != 4 && tipoCalculo != 6 && tipoCalculo != 7 && tipoCalculo != 8 && tipoCalculo != 9)
        return;
    
    var altura = new Number(controle.value.replace(',', '.'));
    var alturaTemp = altura - parseInt(altura, 10);
    
    var arredondar = tipoCalculo == 4 ? 0.5 : tipoCalculo == 6 ? 1 : 0;
    
    if (tipoCalculo < 7)
    {
        if (alturaTemp > 0 && alturaTemp < arredondar)
            altura = parseInt(altura, 10) + arredondar;
        else if (alturaTemp > arredondar)
            altura = parseInt(altura, 10) + (arredondar * 2);
    }
    else if (tipoCalculo == 7 && altura < 6)
        altura = 6;
    
    if (controle !== undefined && controle != null)
        controle.value = altura.toString().replace('.', ',');
}

function valorAluminio(controleAltura, valor, qtde,  arredondar, tipoCalculo)
{
    if (tipoCalculo != 4 && tipoCalculo != 6 && tipoCalculo != 7 && tipoCalculo != 8 && tipoCalculo != 9)
        return;
    
    // Coloca um valor padrão para a variável, se ela não tiver valor
    arredondar = arredondar == false ? false : true;

    // Arredonda altura
    if (arredondar)
        arredondaAltura(controleAltura, tipoCalculo);
    
    var total = new Number(valor.replace(',', '.'));
    var altura = new Number(controleAltura.value.replace(',', '.'));
    
    if (altura == 6) // Está sendo considerado venda por barra de 6m
        total = total * qtde;
    else if (altura < 6) // Está sendo considerado venda por barra de 6m
        total = (total * ((altura % 6) / 6)) * qtde;
    else // Está sendo considerado venda por metro linear ML
        total = (total / 6.0) * qtde * altura;
        
    return total;
}

function valorUnitAluminio(controleAltura, valor, qtde, arredondar, tipoCalculo)
{
    // Coloca um valor padrão para a variável, se ela não tiver valor
    arredondar = arredondar == false ? false : true;

    // Arredonda altura
    if (arredondar)
        arredondaAltura(controleAltura, tipoCalculo);

    var total = new Number(valor.replace(',', '.'));
    var altura = new Number(controleAltura.value.replace(',', '.'));

    if (altura == 6) // Está sendo considerado venda por barra de 6m
        total = total / qtde;
    else if (altura < 6) // Está sendo considerado venda por barra de 6m
        total = ((total / qtde) / (altura % 6)) * 6;
    else // Está sendo considerado venda por metro linear ML
        total = total / (qtde * altura);

    return total;
}