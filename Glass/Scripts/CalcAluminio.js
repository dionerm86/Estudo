function arredondaAltura(controle)
{
    if (dadosProduto.IsAluminio === false) {
        return;
    }

    var altura = new Number(controle.value.replace(',', '.'));
    var alturaTemp = altura - parseInt(altura, 10);    
    var arredondar = 0;

    if (dadosProduto.TipoCalculo == 4) {
        arredondar = 0.5;
    }
    else if (dadosProduto.TipoCalculo == 6) {
        arredondar = 1;
    }

    if (dadosProduto.TipoCalculo < 7) {
        if (alturaTemp > 0 && alturaTemp < arredondar) {
            altura = parseInt(altura, 10) + arredondar;
        }
        else if (alturaTemp > arredondar) {
            altura = parseInt(altura, 10) + (arredondar * 2);
        }
    }
    else if (dadosProduto.TipoCalculo == 7 && altura < 6) {
        altura = 6;
    }
    
    if (controle !== undefined && controle != null) {
        controle.value = altura.toString().replace('.', ',');
    }
}

function valorAluminio(controleAltura, valor, qtde,  arredondar)
{
    // Coloca um valor padrão para a variável, se ela não tiver valor
    arredondar = arredondar == false ? false : true;

    // Arredonda altura
    if (arredondar)
        arredondaAltura(controleAltura);
    
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

function valorUnitAluminio(controleAltura, valor, qtde, arredondar)
{
    // Coloca um valor padrão para a variável, se ela não tiver valor
    arredondar = arredondar == false ? false : true;

    // Arredonda altura
    if (arredondar)
        arredondaAltura(controleAltura);

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