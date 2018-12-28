// Calcula em tempo real o valor total do produto
function CalcProd_CalcTotalProd(valorIns, totM2, totM2Calc, m2Minimo, total, qtde, altura, campoAltura, largura, arredondarAluminio, tipoCalc, alturaBenef, larguraBenef, percDescontoQtdeAtual, percDescontoQtde) {
    try {
        if (valorIns == "")
            return;

        percDescontoQtdeAtual = typeof percDescontoQtdeAtual == "number" ? percDescontoQtdeAtual : 0;
        percDescontoQtde = typeof percDescontoQtde == "number" ? percDescontoQtde : 0;

        if (percDescontoQtdeAtual != percDescontoQtde)
        {
            total = CalcProd_CalcValorUnitProd(valorIns, totM2, totM2Calc, m2Minimo, total, qtde, altura, campoAltura, largura, arredondarAluminio,
                tipoCalc, alturaBenef, larguraBenef, percDescontoQtdeAtual, percDescontoQtde);

            total = total.replace("R$", "").replace(" ", "").replace(",", ".");
        }

        // Se o M2 tiver sido informado, calcula o total do item baseado no M2
        if ((tipoCalc == 2 || tipoCalc == 10) && totM2 != "" && totM2 != "0") {
            // Verifica se há um m² mínimo para a venda deste produto
            //if (m2Minimo != "" && totM2Calc < m2Minimo)
            //    totM2Calc = m2Minimo;
            
            total = total * totM2Calc;
        }
        // Se for cálculo de alumínio
        else if (tipoCalc == 4 || tipoCalc == 6 || tipoCalc == 7 || tipoCalc == 9)
            total = valorAluminio(campoAltura, total, qtde, arredondarAluminio);
        // Se for cálculo por perímetro
        else if (tipoCalc == 3)
        {
            alturaBenef = !isNaN(alturaBenef) && alturaBenef >= 0 && alturaBenef <= 2 ? alturaBenef : 2;
            larguraBenef = !isNaN(larguraBenef) && larguraBenef >= 0 && larguraBenef <= 2 ? larguraBenef : 2;
            total = total * qtde * (((altura * alturaBenef) + (largura * larguraBenef)) / 1000);
        }
        // Se for cálculo por ML
        else if (tipoCalc == 8)
            total = total * qtde * altura;
        // Se o M2 não tiver sido informado, calcula o total do item baseado na qtd
        else
            total = total * qtde;

        total = total * (1 - (percDescontoQtde / 100));
        return "R$ " + total.toFixed(2).replace('.', ',');
    }
    catch (err) {
        return "";
    }
}

function CalcProd_CalcValorUnitProd(valorIns, totM2, totM2Calc, m2Minimo, total, qtde, altura, campoAltura, largura, arredondarAluminio, tipoCalc, alturaBenef, larguraBenef, percDescontoQtdeAtual, percDescontoQtde)
{
    try
    {
        percDescontoQtdeAtual = typeof percDescontoQtdeAtual == "number" ? percDescontoQtdeAtual : 0;
        percDescontoQtde = typeof percDescontoQtde == "number" ? percDescontoQtde : 0;

        if (percDescontoQtdeAtual != percDescontoQtde)
        {
            total = CalcProd_CalcTotalProd(valorIns, totM2, totM2Calc, m2Minimo, total, qtde, altura, campoAltura, largura, arredondarAluminio,
                tipoCalc, alturaBenef, larguraBenef, 0, 0);

            total = parseFloat(total.replace("R$", "").replace(" ", "").replace(",", "."));
            var perc = Math.min(percDescontoQtde, percDescontoQtdeAtual);
            total = total * (1 / (1 - ((perc != 100 ? perc : 0) / 100)));
            total = parseFloat(total.toFixed(6));
        }

        // Se o M2 tiver sido informado, calcula o total do item baseado no M2
        if ((tipoCalc == 2 || tipoCalc == 10) && totM2 != "" && totM2 != "0")
        {
            // Verifica se há um m² mínimo para a venda deste produto
            //if (m2Minimo != "" && totM2Calc < m2Minimo)
            //    totM2Calc = m2Minimo;

            valorIns = total / totM2Calc;
        }
        // Se for cálculo de alumínio
        else if (tipoCalc == 4 || tipoCalc == 6 || tipoCalc == 7 || tipoCalc == 9)
            valorIns = valorUnitAluminio(campoAltura, total, qtde, arredondarAluminio);
        // Se for cálculo por perímetro
        else if (tipoCalc == 3)
        {
            alturaBenef = !isNaN(alturaBenef) && alturaBenef >= 0 && alturaBenef <= 2 ? alturaBenef : 2;
            larguraBenef = !isNaN(larguraBenef) && larguraBenef >= 0 && larguraBenef <= 2 ? larguraBenef : 2;
            valorIns = total / ((((altura * alturaBenef) + (largura * larguraBenef)) / 1000) * qtde);
        }
        // Se for cálculo por ML
        else if (tipoCalc == 8)
            valorIns = total / (qtde * altura);
        // Se o M2 não tiver sido informado, calcula o total do item baseado na qtd
        else
            valorIns = total / qtde;

        return "R$ " + valorIns.toFixed(2).replace(".", ",");
    }
    catch (err)
    {
        return "";
    }
}

function CalcProd_IsAlturaInteira(tipoCalc)
{
    return tipoCalc != 4 && tipoCalc != 6 && tipoCalc != 7 && tipoCalc != 8 && tipoCalc != 9;
}

function CalcProd_IsQtdeInteira(tipoCalc)
{
    return tipoCalc != 5;
}

function CalcProd_DesabilitarAltura(tipoCalc)
{
    return tipoCalc == 1 || tipoCalc == 5 || tipoCalc == 11;
}

function CalcProd_DesabilitarLargura(tipoCalc)
{
    return tipoCalc == 1 || tipoCalc == 4 || tipoCalc == 5 || tipoCalc == 6 || tipoCalc == 7 || tipoCalc == 8 || tipoCalc == 9 || tipoCalc == 11;
}