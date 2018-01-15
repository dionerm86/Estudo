function exibirCorItemProjeto(botao, nomeTabela)
{
    for (iTip = 0; iTip < 2; iTip++)
    {
        TagToTip(nomeTabela, FADEIN, 300, COPYCONTENT, false, TITLE, 'Alterar cor dos itens de projeto', CLOSEBTN, true,
            CLOSEBTNTEXT, 'Fechar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, false,
            FIX, [botao, -11 - getTableWidth(nomeTabela), -41 - getTableHeight(nomeTabela)]);
    }
}

function alterar(nomeControle)
{
    document.getElementById(nomeControle + "_hdfCorVidro").value = document.getElementById(nomeControle + "_drpCorVidro").value;
    document.getElementById(nomeControle + "_hdfCorAluminio").value = document.getElementById(nomeControle + "_drpCorAluminio").value;
    document.getElementById(nomeControle + "_hdfCorFerragem").value = document.getElementById(nomeControle + "_drpCorFerragem").value;
    
    UnTip();
    document.getElementById(nomeControle + "_btnAplicar").click();
}