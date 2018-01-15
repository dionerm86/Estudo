using System;
using System.Collections.Generic;
using Glass.Data.Helper;
using System.IO;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Xml;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class SelArquivoOtimizado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnImportarArquivo_Click(object sender, EventArgs e)
        {
            FilaOperacoes.ArquivoOtimizacao.AguardarVez();

            try
            {
                if (!fluArquivoOtimizado.HasFile)
                {
                    Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                    return;
                }

                List<ProdutosPedidoEspelho> lstProdPed = new List<ProdutosPedidoEspelho>();

                string planoCorte = String.Empty;
                string etiquetasJaImpressas = String.Empty;
                int qtdPecasImpressas = 0;
                string idsPedido = "";

                List<string> lstEtiquetas = new List<string>();
                var pedidosAlteradosAposExportacao = new List<int>();

                if (EtiquetaConfig.TipoExportacaoEtiqueta == DataSources.TipoExportacaoEtiquetaEnum.OptyWay)
                {
                    #region Retorno Opty Way

                    // L� o arquivo de otimiza��o enviado
                    XmlDocument xmlDoc = new XmlDocument();
                    /* Chamado 50941. */
                    xmlDoc.LoadXml(new StreamReader(fluArquivoOtimizado.FileContent).ReadToEnd());

                    foreach (XmlNode node in xmlDoc["DATAPACKET"]["ROWDATA"])
                    {
                        // A tag que possui o atributo "ELABORATO", cont�m o plano de corte, as tags subsequentes � ela cont�m as pe�as
                        if (node.Attributes["ELABORATO"] != null)
                        {
                            if (node.Attributes["CODCOM"] == null)
                                throw new Exception("Tag ELABORATO sem informa��o do plano de corte (CODCOM). Provavelmente foi inserida uma pe�a ap�s a otimiza��o das etiquetas, fazendo com que esta ficasse sem plano de corte.");

                            planoCorte =
                                string.Format("{0}Z{1}-{2}{3}",
                                    node.Attributes["CODCOM"].Value,
                                    DateTime.Now.ToString("MMyy"),
                                    node.Attributes["ELABORATO"].Value.Substring(0, node.Attributes["ELABORATO"].Value.IndexOf('/')).PadLeft(2, '0'),
                                    node.Attributes["ELABORATO"].Value.Substring(
                                    node.Attributes["ELABORATO"].Value.IndexOf('/'),
                                    node.Attributes["ELABORATO"].Value.IndexOf(' ') - node.Attributes["ELABORATO"].Value.IndexOf('/')));

                            continue;
                        }

                        if (node.Attributes["NOTES"] == null)
                            throw new Exception("Uma ou mais pe�as n�o possuem o c�digo da etiqueta exportado pelo WebGlass (Campo NOTES).");

                        int largura = 0;
                        if (node.Attributes["DIMXPZR"] != null)
                            largura = Glass.Conversoes.StrParaInt(node.Attributes["DIMXPZR"].Value.Split('.')[0]);
                        else if (node.Attributes["DIMXPZ"] != null)
                            largura = Glass.Conversoes.StrParaInt(node.Attributes["DIMXPZ"].Value.Split('.')[0]);

                        int altura = 0;
                        if (node.Attributes["DIMYPZR"] != null)
                            altura = Glass.Conversoes.StrParaInt(node.Attributes["DIMYPZR"].Value.Split('.')[0]);
                        else if (node.Attributes["DIMYPZ"] != null)
                            altura = Glass.Conversoes.StrParaInt(node.Attributes["DIMYPZ"].Value.Split('.')[0]);

                        // Pega a etiqueta da pe�a
                        string etiqueta = node.Attributes["NOTES"].Value;

                        // Verifica se a etiqueta � aceita no webglass
                        if (!etiqueta.Contains(".") || !etiqueta.Contains("/") || !etiqueta.Contains("-"))
                            throw new Exception("Uma das etiquetas do arquivo � inv�lida. Etiqueta: Notes=" + etiqueta);

                        // Busca o produtoPedidoEspelho pela etiqueta
                        bool isPecaReposta = ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(etiqueta, true);

                        // Verifica se a etiqueta j� foi impressa
                        if (!isPecaReposta && ProdutoImpressaoDAO.Instance.EstaImpressa(etiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Pedido))
                        {
                            etiquetasJaImpressas += etiqueta + ", ";
                            continue;
                        }

                        // Salva o pedido da etiqueta
                        idsPedido += etiqueta.Split('-')[0] + ",";

                        if (lstEtiquetas.Contains(etiqueta.Trim()))
                        {
                            // Caso a empresa tenha colocado arquivos de t�bua ou algo do tipo no arquivo de retorno, deve ser sugerido � eles que 
                            // preencham o campo NOTES com "ignorar", para n�o dar erro no arquivo e permitir importar as outras pe�as
                            if (etiqueta.Trim().ToLower() != "ignorar")
                                throw new Exception("Este arquivo possui etiquetas duplicadas. Etiqueta: " + etiqueta);

                            continue;
                        }

                        lstEtiquetas.Add(etiqueta.Trim());

                        // Pega a posi��o da pe�a no arquivo de otimiza��o
                        int posicaoArqOtimiz = Glass.Conversoes.StrParaInt(node.Attributes["POSPZ"].Value.Split('/')[0]);

                        // Pega a posi��o de ordena��o da pe�a no arquivo de otimiza��o
                        int numSeq = node.Attributes["POSTOT"] != null ? Glass.Conversoes.StrParaInt(node.Attributes["POSTOT"].Value) : 0;

                        // Pega o campo forma, se houver
                        string forma = node.Attributes["SAGOMA"] != null ? node.Attributes["SAGOMA"].Value : String.Empty;

                        ProdutosPedidoEspelho prodPed;
                        var msgErro = "A etiqueta '" + etiqueta + "' n�o possui uma pe�a associada. " +
                            "O pedido pode ter sido alterado ap�s a gera��o do arquivo para o Opty Way ou a impress�o pode ter sido cancelada. " +
                            "Refa�a a otimiza��o das etiquetas com um novo arquivo de otimiza��o gerado pelo sistema.";

                        try
                        {
                            uint? idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(etiqueta);
                            uint? idProdPed = null;

                            if (idProdPedProducao == null)
                            {
                                // Se j� houver uma etiqueta cancelada com este c�digo, apenas re-insere a mesma ao inv�s de dar mensagem de erro
                                if (ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducaoCanc(etiqueta) > 0)
                                {
                                    ProdutoPedidoProducaoDAO.Instance.InserePeca(null, etiqueta, String.Empty, UserInfo.GetUserInfo.CodUser, true);
                                    idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(etiqueta);
                                }
                                else
                                {
                                    Glass.MensagemAlerta.ShowMsg(msgErro, Page);
                                    return;
                                }
                            }

                            idProdPed = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(idProdPedProducao.Value);

                            prodPed = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(etiqueta, idProdPed, true);

                            if (prodPed == null)
                            {
                                Glass.MensagemAlerta.ShowMsg(msgErro, Page);
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            Glass.MensagemAlerta.ErrorMsg(msgErro, ex, Page);
                            return;
                        }

                        // Verifica se a altura e largura retornadas no optyway batem com a altura e largura do produto,
                        // para evitar que caso ocorra algum erro a pe�a seja produzida, no caso da vidr�lia esta valida��o n�o pode ser feita
                        // pois � acrescentado +2 na largura ao exportar para o optyway, como a pe�a pode ser girada no optyway o campo altura e largura
                        // podem ser invertidos, impossibilitando que a verifica��o seja feita
                        // Verifica se a altura e largura batem, considerando 10cm como taxa de erro para vidros fora do esquadro e ou redondos
                        //if (!prodPed.Redondo && (prodPed.AlturaProducao != altura && Math.Abs(prodPed.AlturaProducao - altura) > 100) ||
                        //    ((prodPed.LarguraProducao != largura && Math.Abs(prodPed.LarguraProducao - largura) > 100)))
                        //{
                        //    // Caso a largura e altura n�o tenham sido validadas, inverte as mesmas e compara novamente, isso deve ser feito
                        //    // pois pode ser que a pe�a tenha sido girada no optyway
                        //    if ((prodPed.AlturaProducao != largura && Math.Abs(prodPed.AlturaProducao - largura) > 100) ||
                        //        ((prodPed.LarguraProducao != altura && Math.Abs(prodPed.LarguraProducao - altura) > 100)))
                        //    {
                        //        Glass.MensagemAlerta.ShowMsg("A pe�a " + etiqueta + " importada n�o condiz com a pe�a cadastrada no sistema, gere novamente o arquivo de otimiza��o no WebGlass e exporte-o para o optyway.", Page);
                        //        return;
                        //    }
                        //}

                        float qtde = prodPed.Qtde;

                        /* Chamado 44607. */
                        if (prodPed.IsProdutoLaminadoComposicao)
                            qtde = (int)prodPed.QtdeImpressaoProdLamComposicao;
                        else if (prodPed.IsProdFilhoLamComposicao)
                        {
                            qtde = ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(prodPed.IdProdPedParent.Value) * prodPed.Qtde;

                            var idProdPedParentPai = ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParent(null, prodPed.IdProdPedParent.Value);

                            if (idProdPedParentPai.GetValueOrDefault(0) > 0)
                                qtde *= ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(idProdPedParentPai.Value);
                        }

                        if (Glass.Conversoes.StrParaInt(etiqueta.Split('/')[1]) > qtde)
                        {
                            Glass.MensagemAlerta.ShowMsg("Etiqueta " + etiqueta + " � inv�lida. O produto referido tem quantidade " + prodPed.Qtde +
                                " e a etiqueta indica " + etiqueta.Split('/')[1] + ".", Page);

                            return;
                        }

                        if (prodPed.IdPedido > 0)
                        {
                            var dataFinalizacaoPCP = PedidoEspelhoDAO.Instance.ObtemDataConf(prodPed.IdPedido);

                            if (dataFinalizacaoPCP != null && dataFinalizacaoPCP >
                                ArquivoOtimizacaoDAO.Instance.ObtemDataUltimaExportacaoEtiqueta(etiqueta))
                            {
                                pedidosAlteradosAposExportacao.Add((int)prodPed.IdPedido);
                                continue;
                            }
                        }

                        prodPed.PlanoCorte = planoCorte;
                        prodPed.Etiquetas += etiqueta + "_";
                        prodPed.PecaReposta = isPecaReposta;

                        try
                        {
                            // Insere/Atualiza etiqueta na tabela de produto_impressao
                            ProdutoImpressaoDAO.Instance.InsertOrUpdatePecaComTransacao(etiqueta, planoCorte, posicaoArqOtimiz, numSeq,
                                ProdutoImpressaoDAO.TipoEtiqueta.Pedido, forma);

                            //// Atualiza campo forma
                            //if (!String.IsNullOrEmpty(forma))
                            //    ProdutosPedidoEspelhoDAO.Instance.AtualizaForma(prodPed.IdProdPed, forma);
                        }
                        catch (Exception ex)
                        {
                            Glass.MensagemAlerta.ErrorMsg("Falha ao importar etiquetas", ex, Page);
                            return;
                        }

                        // Marca a qtd como 1, pois cada produto no arquivo � qtd 1, caso o produto se repita, soma a qtd
                        prodPed.QtdAImprimir = !isPecaReposta ? 1 : 0;

                        bool jaInserido = false;
                        for (int i = 0; i < lstProdPed.Count; i++)
                            if (prodPed.IdProdPed == lstProdPed[i].IdProdPed && prodPed.PlanoCorte == lstProdPed[i].PlanoCorte && prodPed.PecaReposta == lstProdPed[i].PecaReposta)
                            {
                                if (!isPecaReposta)
                                    lstProdPed[i].QtdAImprimir += 1;

                                lstProdPed[i].Etiquetas += etiqueta + "_";
                                jaInserido = true;
                                break;
                            }

                        if (!jaInserido)
                            lstProdPed.Add(prodPed);

                        qtdPecasImpressas++;
                    }

                    #endregion
                }
                else
                {
                    #region Retorno Corte Certo

                    XmlDocument xmlDoc = new XmlDocument();

                    using (var conteudoArquivo = new StreamReader(fluArquivoOtimizado.FileContent))
                    {
                        xmlDoc.LoadXml(conteudoArquivo.ReadToEnd());
                    }

                    foreach (XmlNode node in xmlDoc["PF_EXPORT"]["CUT_PLAN"])
                    {
                        // A tag <PLAN> cont�m o plano de corte, as tags subsequentes � ela cont�m as pe�as
                        if (node.Name == "PLAN")
                        {
                            planoCorte = node.InnerXml.PadLeft(2, '0') + "-" + ContadorPlanoCorteDAO.Instance.GetNext().ToString().PadLeft(8, '0');
                            continue;
                        }

                    string etiqueta = node["desc"].InnerXml;
    
                    string[] vetEtiq = etiqueta.Split('_');

                    if (vetEtiq == null || vetEtiq.Length < 4 || vetEtiq[0] == "")
                    {
                        MensagemAlerta.ShowMsg("Algumas pe�as foram Alteradas ou Inclu�das no arquivo. Por favor Otimize novamente!", Page);
                        return;
                    }

                    etiqueta = vetEtiq[0] + "-" + vetEtiq[1] + "." + vetEtiq[2] + "/" + vetEtiq[3];
    
                    // Busca o produtoPedidoEspelho pela etiqueta
                        bool isPecaReposta = ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(etiqueta, true);

                        // Verifica se a etiqueta j� foi impressa
                        if (!isPecaReposta && ProdutoImpressaoDAO.Instance.EstaImpressa(etiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Pedido))
                        {
                            etiquetasJaImpressas += etiqueta + ", ";
                            continue;
                        }

                        if (lstEtiquetas.Contains(etiqueta.Trim()))
                            continue;

                        lstEtiquetas.Add(etiqueta);

                        ProdutosPedidoEspelho prodPed;

                        try
                        {
                            uint? idProdPedProducao = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(etiqueta);
                            uint? idProdPed = null;

                            if (idProdPedProducao != null)
                                idProdPed = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(idProdPedProducao.Value);

                            prodPed = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(etiqueta, idProdPed, true);
                        }
                        catch (Exception ex)
                        {
                            Glass.MensagemAlerta.ErrorMsg("A etiqueta '" + etiqueta + "' n�o possui uma pe�a associada. " +
                                "O pedido pode ter sido alterado ap�s a gera��o do arquivo para o Corte Certo. " +
                                "Refa�a a otimiza��o das etiquetas com um novo arquivo de otimiza��o gerado pelo sistema.", ex, Page);

                            return;
                        }

                        float qtde = prodPed.Qtde;

                        /* Chamado 52296. */
                        if (prodPed.IsProdutoLaminadoComposicao)
                            qtde = (int)prodPed.QtdeImpressaoProdLamComposicao;
                        else if (prodPed.IsProdFilhoLamComposicao)
                        {
                            qtde = ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(prodPed.IdProdPedParent.Value) * prodPed.Qtde;

                            var idProdPedParentPai = ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParent(null, prodPed.IdProdPedParent.Value);

                            if (idProdPedParentPai.GetValueOrDefault(0) > 0)
                                qtde *= ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(idProdPedParentPai.Value);
                        }

                        if (Glass.Conversoes.StrParaInt(vetEtiq[3]) > qtde)
                        {
                            Glass.MensagemAlerta.ShowMsg("Etiqueta " + etiqueta + " � inv�lida. O produto referido tem quantidade " + prodPed.Qtde +
                                " e a etiqueta indica " + vetEtiq[3] + ".", Page);
                            return;
                        }

                        if (prodPed.IdPedido > 0)
                        {
                            var dataFinalizacaoPCP = PedidoEspelhoDAO.Instance.ObtemDataConf(prodPed.IdPedido);

                            if (dataFinalizacaoPCP != null && dataFinalizacaoPCP >
                                ArquivoOtimizacaoDAO.Instance.ObtemDataUltimaExportacaoEtiqueta(etiqueta))
                            {
                                pedidosAlteradosAposExportacao.Add((int)prodPed.IdPedido);
                                continue;
                            }
                        }

                        prodPed.PlanoCorte = planoCorte;
                        prodPed.Etiquetas += etiqueta + "_";
                        prodPed.PecaReposta = isPecaReposta;

                        try
                        {
                            // Insere/Atualiza etiqueta na tabela de produto_impressao
                            ProdutoImpressaoDAO.Instance.InsertOrUpdatePecaComTransacao(etiqueta, planoCorte, 0, 0,
                                ProdutoImpressaoDAO.TipoEtiqueta.Pedido, null);
                        }
                        catch (Exception ex)
                        {
                            Glass.MensagemAlerta.ErrorMsg("Falha ao importar etiquetas", ex, Page);
                            return;
                        }

                        // Marca a qtd como 1, pois cada produto no arquivo � qtd 1, caso o produto se repita, soma a qtd
                        prodPed.QtdAImprimir = !isPecaReposta ? 1 : 0;
                        bool jaInserido = false;
                        for (int i = 0; i < lstProdPed.Count; i++)
                            if (prodPed.IdProdPed == lstProdPed[i].IdProdPed && prodPed.PlanoCorte == lstProdPed[i].PlanoCorte && prodPed.PecaReposta == lstProdPed[i].PecaReposta)
                            {
                                if (!isPecaReposta)
                                    lstProdPed[i].QtdAImprimir += 1;

                                lstProdPed[i].Etiquetas += etiqueta + "_";
                                jaInserido = true;
                                break;
                            }

                        if (!jaInserido)
                            lstProdPed.Add(prodPed);

                        qtdPecasImpressas++;
                    }

                    #endregion
                }

                if (lstEtiquetas == null || lstEtiquetas.Count == 0)
                    throw new Exception("N�o h� etiquetas para importar.");

                string extensao = fluArquivoOtimizado.FileName.Substring(fluArquivoOtimizado.FileName.LastIndexOf("."));

                ArquivoOtimizacao a = ArquivoOtimizacaoDAO.Instance.InserirArquivoOtimizacao(ArquivoOtimizacao.DirecaoEnum.Importar,
                    extensao, lstEtiquetas, null, null);

                // Salva arquivo otimizado
                fluArquivoOtimizado.SaveAs(Data.Helper.Utils.GetArquivoOtimizacaoPath + a.NomeArquivo);

                #region Ordena as pe�as por cor/espessura

                // Ordena as pe�as por cor/espessura
                lstProdPed.Sort(new Comparison<ProdutosPedidoEspelho>(delegate (ProdutosPedidoEspelho x, ProdutosPedidoEspelho y)
                {
                    int cor = Comparer<string>.Default.Compare(x.Cor, y.Cor);
                    if (cor == 0)
                        return Comparer<float>.Default.Compare(x.Espessura, y.Espessura);
                    else
                        return cor;
                }));

                #endregion

                #region Gera um script para inserir estas pe�as na tela

                string script = String.Empty;

                if (!String.IsNullOrEmpty(etiquetasJaImpressas))
                    script += "alert('As etiquetas " + etiquetasJaImpressas + "j� foram impressas');";

                foreach (ProdutosPedidoEspelho ppe in lstProdPed)
                {
                    // Verificar se o produto j� foi totalmente impresso
                    if (ppe.Qtde == ppe.QtdImpresso && !ppe.PecaReposta)
                    {
                        qtdPecasImpressas -= ProdutoImpressaoDAO.Instance.QuantidadeImpressa(null, (int)ppe.IdProdPed);
                        continue;
                    }

                    float totM2 = ppe.PecaReposta ? (ppe.TotM / ppe.Qtde) : (ppe.TotM / ppe.Qtde) * ppe.QtdAImprimir;
                    float totM2Calc = ppe.PecaReposta ? (ppe.TotM2Calc / ppe.Qtde) : (ppe.TotM2Calc / ppe.Qtde) * ppe.QtdAImprimir;

                    string descrProduto = (ppe.DescrProduto != null ? ppe.DescrProduto : "").Replace("\t", "").Replace("'", "");
                    string obs = (ppe.ObsGrid != null ? ppe.ObsGrid : "").Replace("\t", "").Replace("'", "");
                    string codProcesso = (ppe.CodProcesso != null ? ppe.CodProcesso : "").Replace("\t", "").Replace("'", "");
                    string codAplicacao = (ppe.CodAplicacao != null ? ppe.CodAplicacao : "").Replace("\t", "").Replace("'", "");
                    string planoCorteEtiq = (ppe.PlanoCorte != null ? ppe.PlanoCorte : "").Replace("\t", "").Replace("'", "");
                    string etiquetas = (ppe.Etiquetas != null ? ppe.Etiquetas : "").Replace("\t", "").Replace("'", "");

                    // Foi incluido uma chamada de fun��o para remover o bot�o de Excluir das pe�as otimizadas
                    script += "window.opener.setProdEtiqueta(" + ppe.IdProdPed + ", null, " + ppe.IdPedido + ", null, null, '" + descrProduto.Replace("'", "") + (ppe.PecaReposta ? " (Reposta)" : "") +
                        "', '" + codProcesso + "', '" + codAplicacao + "', " + (!ppe.PecaReposta ? ppe.Qtde : 1) + ", " + (!ppe.PecaReposta ? ppe.QtdImpresso : 1) +
                        ", " + ppe.QtdAImprimir + ", " + ppe.AlturaProducao + ", " + ppe.LarguraProducao + ", '" + obs + "', '" + totM2 + "', '" + planoCorteEtiq.Replace("'", "") +
                        "', null, true, '" + etiquetas.TrimEnd('_') + "', null, '" + totM2Calc + "', null); ";
                }

                script += "window.opener.escondeRemoverTodasAsLinhas();";

                #endregion

                var mensagemPedidoAlteradoAposExportacao =
                    pedidosAlteradosAposExportacao.Count > 0 ?
                        string.Format("O(s) pedido(s) {0} foi(ram) reaberto(s) ap�s a exporta��o das pe�as. A otimiza��o deve ser feita novamente com um novo arquivo gerado pelo sistema.",
                            string.Join(", ", pedidosAlteradosAposExportacao)) :
                        string.Empty;

                if (qtdPecasImpressas == 0)
                    MensagemAlerta.ShowMsg(string.Format("Todas as pe�as deste arquivo j� foram impressas. {0}",
                        mensagemPedidoAlteradoAposExportacao), Page);
                else
                    Page.ClientScript.RegisterClientScriptBlock(typeof(string), "setPecas",
                        string.Format("window.opener.limpar();{0}alert('{1} pe�as importadas. {2}');closeWindow();",
                            script, qtdPecasImpressas, mensagemPedidoAlteradoAposExportacao), true);
            }
            finally
            {
                FilaOperacoes.ArquivoOtimizacao.ProximoFila();
            }
        }
    }
}
