using System;
using System.Collections.Generic;
using Glass.Data.Helper;
using System.IO;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Xml;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.UI.Web.Utils
{
    public partial class SelArquivoOtimizado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnImportarArquivo_Click(object sender, EventArgs e)
        {
            if (!fluArquivoOtimizado.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }

            var lstProdPed = new List<ProdutosPedidoEspelho>();

            var planoCorte = string.Empty;
            var etiquetasJaImpressas = string.Empty;
            var qtdPecasImpressas = 0;

            List<string> lstEtiquetas = new List<string>();
            var pedidosAlteradosAposExportacao = new List<int>();

            if (EtiquetaConfig.TipoExportacaoEtiqueta == DataSources.TipoExportacaoEtiquetaEnum.OptyWay)
            {
                try
                {
                    // Lê o arquivo de otimização enviado
                    XmlDocument xmlDoc = new XmlDocument();
                    /* Chamado 50941. */
                    xmlDoc.LoadXml(new StreamReader(fluArquivoOtimizado.FileContent).ReadToEnd());

                    ImpressaoEtiquetaDAO.Instance.ImportarArquivoOtimizacaoOptyWay(xmlDoc, ref lstEtiquetas, ref etiquetasJaImpressas, ref pedidosAlteradosAposExportacao, ref lstProdPed, ref qtdPecasImpressas);
                }
                catch (Exception ex)
                {
                    MensagemAlerta.ShowMsg(ex.Message, Page);
                    return;
                }
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
                    // A tag <PLAN> contém o plano de corte, as tags subsequentes à ela contém as peças
                    if (node.Name == "PLAN")
                    {
                        planoCorte = node.InnerXml.PadLeft(2, '0') + "-" + ContadorPlanoCorteDAO.Instance.GetNext().ToString().PadLeft(8, '0');
                        continue;
                    }

                string etiqueta = node["desc"].InnerXml;
    
                string[] vetEtiq = etiqueta.Split('_');

                if (vetEtiq == null || vetEtiq.Length < 4 || vetEtiq[0] == "")
                {
                    MensagemAlerta.ShowMsg("Algumas peças foram Alteradas ou Incluídas no arquivo. Por favor Otimize novamente!", Page);
                    return;
                }

                etiqueta = vetEtiq[0] + "-" + vetEtiq[1] + "." + vetEtiq[2] + "/" + vetEtiq[3];
    
                // Busca o produtoPedidoEspelho pela etiqueta
                    bool isPecaReposta = ProdutoPedidoProducaoDAO.Instance.IsPecaReposta(etiqueta, true);

                    // Verifica se a etiqueta já foi impressa
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
                        Glass.MensagemAlerta.ErrorMsg("A etiqueta '" + etiqueta + "' não possui uma peça associada. " +
                            "O pedido pode ter sido alterado após a geração do arquivo para o Corte Certo. " +
                            "Refaça a otimização das etiquetas com um novo arquivo de otimização gerado pelo sistema.", ex, Page);

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
                        MensagemAlerta.ShowMsg(string.Format("Etiqueta {0} é inválida. O produto referido tem quantidade {1} e a etiqueta indica {2}.", etiqueta, qtde, vetEtiq[3]), Page);
                        return;
                    }

                    if (prodPed.IdPedido > 0)
                    {
                        var dataFinalizacaoPCP = PedidoEspelhoDAO.Instance.ObtemDataConf(null, prodPed.IdPedido);

                        if (dataFinalizacaoPCP != null && dataFinalizacaoPCP >
                            ArquivoOtimizacaoDAO.Instance.ObtemDataUltimaExportacaoEtiqueta(null, etiqueta))
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

                    // Marca a qtd como 1, pois cada produto no arquivo é qtd 1, caso o produto se repita, soma a qtd
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
                throw new Exception("Não há etiquetas para importar.");

            string extensao = fluArquivoOtimizado.FileName.Substring(fluArquivoOtimizado.FileName.LastIndexOf("."));

            ArquivoOtimizacao a = ArquivoOtimizacaoDAO.Instance.InserirArquivoOtimizacao(ArquivoOtimizacao.DirecaoEnum.Importar,
                extensao, lstEtiquetas, null, null);

            // Salva arquivo otimizado
            fluArquivoOtimizado.SaveAs(Data.Helper.Utils.GetArquivoOtimizacaoPath + a.NomeArquivo);

            #region Ordena as peças por cor/espessura

            // Ordena as peças por cor/espessura
            lstProdPed.Sort(new Comparison<ProdutosPedidoEspelho>(delegate (ProdutosPedidoEspelho x, ProdutosPedidoEspelho y)
            {
                int cor = Comparer<string>.Default.Compare(x.Cor, y.Cor);
                if (cor == 0)
                    return Comparer<float>.Default.Compare(x.Espessura, y.Espessura);
                else
                    return cor;
            }));

            #endregion

            #region Gera um script para inserir estas peças na tela

            string script = String.Empty;

            if (!string.IsNullOrEmpty(etiquetasJaImpressas))
                script += "alert('As etiquetas " + etiquetasJaImpressas + "já foram impressas');";

            foreach (ProdutosPedidoEspelho ppe in lstProdPed)
            {
                // Verificar se o produto já foi totalmente impresso
                if (ppe.Qtde == ppe.QtdImpresso && !ppe.PecaReposta)
                {
                    qtdPecasImpressas -= ProdutoImpressaoDAO.Instance.QuantidadeImpressa(null, (int)ppe.IdProdPed);
                    continue;
                }

                    float totM2 = ppe.PecaReposta ? (ppe.TotM / ppe.Qtde) : (ppe.TotM / ppe.Qtde) * ppe.QtdAImprimir;
                    float totM2Calc = ppe.PecaReposta ? (ppe.TotM2Calc / ppe.Qtde) : (ppe.TotM2Calc / ppe.Qtde) * ppe.QtdAImprimir;

                    string descrProduto = TratarStringScript(ppe.DescrProduto);
                    string obs = TratarStringScript(ppe.Obs);
                    string codProcesso = TratarStringScript(ppe.CodProcesso);
                    string codAplicacao = TratarStringScript(ppe.CodAplicacao);
                    string planoCorteEtiq = TratarStringScript(ppe.PlanoCorte);
                    string etiquetas = TratarStringScript(ppe.Etiquetas);

                    // Foi incluido uma chamada de função para remover o botão de Excluir das peças otimizadas
                    script += "window.opener.setProdEtiqueta(" + ppe.IdProdPed + ", null, " + ppe.IdPedido + ", null, null, '" + descrProduto.Replace("'", "") + (ppe.PecaReposta ? " (Reposta)" : "") +
                    "', '" + codProcesso + "', '" + codAplicacao + "', " + (!ppe.PecaReposta ? ppe.Qtde : 1) + ", " + (!ppe.PecaReposta ? ppe.QtdImpresso : 1) +
                    ", " + ppe.QtdAImprimir + ", " + ppe.AlturaProducao + ", " + ppe.LarguraProducao + ", '" + obs + "', '" + totM2 + "', '" + planoCorteEtiq.Replace("'", "") +
                    "', null, true, '" + etiquetas.TrimEnd('_') + "', null, '" + totM2Calc + "', null); ";
            }

            script += "window.opener.escondeRemoverTodasAsLinhas();";

            #endregion

            var mensagemPedidoAlteradoAposExportacao =
                pedidosAlteradosAposExportacao.Count > 0 ?
                    string.Format("O(s) pedido(s) a seguir foi(ram) reaberto(s) após a exportação das peças, a otimização deve ser feita novamente com um novo arquivo gerado pelo sistema: {0}",
                        string.Join(", ", pedidosAlteradosAposExportacao.Distinct())) :
                    string.Empty;

            if (qtdPecasImpressas == 0 && pedidosAlteradosAposExportacao.Count() == 0)
                MensagemAlerta.ShowMsg(string.Format("{0} ou todas as peças deste arquivo já foram impressas.",
                    mensagemPedidoAlteradoAposExportacao), Page);

            else if (pedidosAlteradosAposExportacao.Count() > 0)
                MensagemAlerta.ShowMsg(mensagemPedidoAlteradoAposExportacao, Page);

            else
                Page.ClientScript.RegisterClientScriptBlock(typeof(string), "setPecas",
                    string.Format("window.opener.limpar();{0}alert('{1} peças importadas. {2}');closeWindow();",
                        script, qtdPecasImpressas, mensagemPedidoAlteradoAposExportacao), true);
        }

        /// <summary>
        /// Trata os dados que serão executados via javacscript na tela
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        private string TratarStringScript(string texto)
        {
            if (texto == null)
                return string.Empty;

            return texto.Replace("\t", "").Replace("'", "").Replace("\r", "").Replace("\n", "");
        }
    }
}
