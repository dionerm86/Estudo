using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class RelEtiquetas : Glass.Relatorios.UI.Web.ReportPage
    {
        protected override object[] Parametros
        {
            get
            {
                return new object[] {
                    hdfIdsProdPedNf.Value,
                    hdfTipoEtiqueta.Value,
                    hdfOpener.Value,
                    hdfRetalhos.Value,
                    hdfSomenteRetalhos.Value,
                    hdfRetalhosProdutos.Value
                };
            }
        }

        protected override bool UsarThread
        {
            get
            {
                return EtiquetaConfig.RelatorioEtiqueta.UsarThreadRelatorioEtiqueta;
            }
        }

        protected override Glass.Relatorios.UI.Web.ReportPage.JavaScriptData DadosJavaScript
        {
            get
            {
                return new JavaScriptData(
                    hdfLoad.Value != "true",
                    "document.getElementById('" + hdfLoad.ClientID + "').value != 'true'",
                    delegate(Exception ex) { return "try { window.opener.habilitarImpressao(); } catch (err) { }"; }
                );
            }
        }

        string idImpressao = string.Empty;
        string numEtiqueta = string.Empty;
        uint idProdPed = 0;
        uint idProdNf = 0;
        uint idAmbientePedido = 0;
        uint idRetalhoProducao = 0;
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Relatorios.RelEtiquetas));

            if (DadosJavaScript.BackgroundLoading)
                return;

            idImpressao = Glass.Conversoes.StrParaUint(Request["idImpressao"]).ToString();
            numEtiqueta = Request["numEtiqueta"];
            idProdPed = Glass.Conversoes.StrParaUint(Request["idProdPed"]);
            idProdNf = Glass.Conversoes.StrParaUint(Request["idProdNf"]);
            idAmbientePedido = Glass.Conversoes.StrParaUint(Request["idAmbientePedido"]);
            idRetalhoProducao = Glass.Conversoes.StrParaUint(Request["idRetalhoProducao"]);

            // Se for impressão individual, salva o idProdPed com qtd 1 no hiddenfield
            if (Request["ind"] == "1")
            {
                if (idProdPed > 0)
                    idImpressao = ProdutoImpressaoDAO.Instance.GetIdImpressaoByProdPed(idProdPed, true);

                if (idProdNf > 0)
                    idImpressao = ProdutoImpressaoDAO.Instance.GetIdImpressaoByProdNf(idProdNf, true);

                if (idAmbientePedido > 0)
                    idImpressao = ProdutoImpressaoDAO.Instance.GetIdImpressaoByAmbientePedido(idAmbientePedido, true);

                if (idRetalhoProducao > 0)
                    idImpressao = ProdutoImpressaoDAO.Instance.GetIdImpressaoByRetalhoProducao(idRetalhoProducao, true);

                if (idImpressao == "0" && !String.IsNullOrEmpty(idImpressao))
                    throw new Exception("Esta etiqueta ainda não foi impressa pelo controle de impressão.");

                ProdutoImpressaoDAO.TipoEtiqueta? tipoEtiqueta = null;
                
                if (idImpressao != "0" && !String.IsNullOrEmpty(idImpressao))
                {
                    idImpressao = ImpressaoEtiquetaDAO.Instance.GetAtivas(idImpressao);
                    tipoEtiqueta = ImpressaoEtiquetaDAO.Instance.GetTipoImpressao(Glass.Conversoes.StrParaUint(idImpressao.Split(',')[0]));
                }

                if (!String.IsNullOrEmpty(numEtiqueta) &&
                    /* Chamado 15725.
                     * Deve-se verificar se a etiqueta está impressa de acordo com o tipo correto da mesma. */
                    //!ProdutoImpressaoDAO.Instance.EstaImpressa(numEtiqueta.Replace("%2f", "/"), ProdutoImpressaoDAO.TipoEtiqueta.Pedido))
                    !ProdutoImpressaoDAO.Instance.EstaImpressa(numEtiqueta.Replace("%2f", "/"),
                    tipoEtiqueta.GetValueOrDefault(ProdutoImpressaoDAO.TipoEtiqueta.Pedido)))
                {
                    ClientScript.RegisterStartupScript(typeof(string), "RelEtiquetas", "alert('Etiqueta não existe ou foi cancelada.');closeWindow();", true);

                    return;
                    //throw new Exception("Etiqueta não existe ou foi cancelada");
                }

                if (String.IsNullOrEmpty(idImpressao))
                {
                    ClientScript.RegisterStartupScript(typeof(string), "RelEtiquetas", "alert('Esta etiqueta ainda não foi impressa ou sua impressão foi cancelada.');closeWindow();", true);
                    
                    return;
                    //throw new Exception("Esta etiqueta ainda não foi impressa ou sua impressão foi cancelada.");
                }
            }
    
            ProcessaReport(pchTabela);
        }
    
        protected override Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam, HttpRequest PageRequest,
            System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login)
        {
            // Pega idImpressão, se houver
            idImpressao = Glass.Conversoes.StrParaUint(Request["idImpressao"]).ToString();
            numEtiqueta = Request["numEtiqueta"];
            idProdPed = Glass.Conversoes.StrParaUint(Request["idProdPed"]);
            idProdNf = Glass.Conversoes.StrParaUint(Request["idProdNf"]);
            idAmbientePedido = Glass.Conversoes.StrParaUint(Request["idAmbientePedido"]);
            idRetalhoProducao = Glass.Conversoes.StrParaUint(Request["idRetalhoProducao"]);
            ProdutoImpressaoDAO.TipoEtiqueta? tipoEtiqueta = null;

            // Se for impressão individual, salva o idProdPed com qtd 1 no hiddenfield
            if (Request["ind"] == "1")
            {
                if (idProdPed > 0)
                    idImpressao = ProdutoImpressaoDAO.Instance.GetIdImpressaoByProdPed(idProdPed, true);
    
                if (idProdNf > 0)
                    idImpressao = ProdutoImpressaoDAO.Instance.GetIdImpressaoByProdNf(idProdNf, true);
    
                if (idAmbientePedido > 0)
                    idImpressao = ProdutoImpressaoDAO.Instance.GetIdImpressaoByAmbientePedido(idAmbientePedido, true);
    
                if (idRetalhoProducao > 0)
                    idImpressao = ProdutoImpressaoDAO.Instance.GetIdImpressaoByRetalhoProducao(idRetalhoProducao, true);
    
                if (idImpressao == "0" && !String.IsNullOrEmpty(idImpressao))
                    throw new Exception("Esta etiqueta ainda não foi impressa pelo controle de impressão.");

                if (idImpressao != "0" && !String.IsNullOrEmpty(idImpressao))
                {
                    idImpressao = ImpressaoEtiquetaDAO.Instance.GetAtivas(idImpressao);
                    tipoEtiqueta = ImpressaoEtiquetaDAO.Instance.GetTipoImpressao(Glass.Conversoes.StrParaUint(idImpressao.Split(',')[0]));
                }

                if (!String.IsNullOrEmpty(numEtiqueta) &&
                    /* Chamado 15725.
                     * Deve-se verificar se a etiqueta está impressa de acordo com o tipo correto da mesma. */
                    //!ProdutoImpressaoDAO.Instance.EstaImpressa(numEtiqueta.Replace("%2f", "/"), ProdutoImpressaoDAO.TipoEtiqueta.Pedido))
                    !ProdutoImpressaoDAO.Instance.EstaImpressa(numEtiqueta.Replace("%2f", "/"),
                    tipoEtiqueta.GetValueOrDefault(ProdutoImpressaoDAO.TipoEtiqueta.Pedido)))
                    throw new Exception("Etiqueta não existe ou foi cancelada");

                if (String.IsNullOrEmpty(idImpressao))
                    throw new Exception("Esta etiqueta ainda não foi impressa ou sua impressão foi cancelada.");                
            }
            else if (idImpressao != "0" && !String.IsNullOrEmpty(idImpressao))
            {
                idImpressao = ImpressaoEtiquetaDAO.Instance.GetAtivas(idImpressao);
                tipoEtiqueta = ImpressaoEtiquetaDAO.Instance.GetTipoImpressao(Glass.Conversoes.StrParaUint(idImpressao.Split(',')[0]));
            }
            else
            {
                if (outrosParametros[1] != null && outrosParametros[1].ToString() != String.Empty)
                    tipoEtiqueta = (ProdutoImpressaoDAO.TipoEtiqueta)Glass.Conversoes.StrParaInt(outrosParametros[1].ToString());
    
                if ((outrosParametros[2] != null && outrosParametros[2].ToString() != string.Empty) &&
                    (outrosParametros[2].ToString().ToLower().Contains("cadreporpeca") ||
                    outrosParametros[2].ToString().ToLower().Contains("cadmarcarpecaproducao") ||
                    outrosParametros[2].ToString().ToLower().Contains("cadretalhoproducao") ||
                    outrosParametros[2].ToString().ToLower().Contains("cadperdachapavidro")))
                    tipoEtiqueta = ProdutoImpressaoDAO.TipoEtiqueta.Retalho;
            }

            if (tipoEtiqueta == null)
            {
                var excessao = new Exception("Não foi possível identificar o tipo de impressão de etiqueta.");

                ErroDAO.Instance.InserirFromException(string.Format("ImprimirEtiqueta - OutrosParametros: {0}, {1}, {2}, {3}",
                    outrosParametros != null && outrosParametros.Length > 0 ? outrosParametros[0].ToString() : "ND",
                    outrosParametros != null && outrosParametros.Length > 1 ? outrosParametros[1].ToString() : "ND",
                    outrosParametros != null && outrosParametros.Length > 2 ? outrosParametros[2].ToString() : "ND",
                    outrosParametros != null && outrosParametros.Length > 3 ? outrosParametros[3].ToString() : "ND"), excessao);

                throw excessao;
            }
    
            if ((!String.IsNullOrEmpty(outrosParametros[0].ToString()) || (idImpressao != "0" && !String.IsNullOrEmpty(idImpressao)) || idProdPed > 0) ||
                (outrosParametros[2].ToString().ToLower().Contains("cadreporpeca") ||
                outrosParametros[2].ToString().ToLower().Contains("cadmarcarpecaproducao") || 
                outrosParametros[2].ToString().ToLower().Contains("cadretalhoproducao") ||
                outrosParametros[2].ToString().ToLower().Contains("cadperdachapavidro")) && (outrosParametros[3] != null && outrosParametros[3].ToString() != String.Empty))
            {
                Etiqueta[] lstEtiq = null;
    
                bool reImprimir = Request["ind"] == "1" || Request["idImpressao"] != null;
    
                // Busca etiquetas, salvando impressão realizada
                switch (tipoEtiqueta.Value)
                {
                    #region ProdutoImpressaoDAO.TipoEtiqueta.Pedido
    
                    case ProdutoImpressaoDAO.TipoEtiqueta.Pedido:
                        string[] dadosRetalhos = outrosParametros[5] != null ? outrosParametros[5].ToString().Split('+') : new string[0];
    
                        if (hdfSomenteRetalhos.Value == "1")
                        {
                            List<string> usar = new List<string>();
                            int index;
    
                            foreach (string dados in outrosParametros[0].ToString().Split('|'))
                            {
                                if (String.IsNullOrEmpty(dados))
                                    continue;
    
                                string[] dadosEtiqueta = dados.Split('\t');
    
                                if ((index = Array.FindIndex(dadosRetalhos, x => !String.IsNullOrEmpty(x) ? 
                                    x.Split('|')[0] == dadosEtiqueta[0] : false)) > -1)
                                {
                                    if (!dadosEtiqueta[0].Contains("R"))
                                        dadosEtiqueta[2] = dadosRetalhos[index].Split('|')[1].Split(',').Length.ToString();
    
                                    usar.Add(String.Join("\t", dadosEtiqueta));
                                }
                            }
    
                            outrosParametros[0] = String.Join("|", usar.ToArray());
                        }
    
                        lstEtiq = EtiquetaDAO.Instance.GetListPedidoComTransacao(login.CodUser, idImpressao, idProdPed, idAmbientePedido, outrosParametros[0].ToString(), false,
                            reImprimir, Request["numEtiqueta"], Request["apenasPlano"] == "true", dadosRetalhos);
    
                        break;
    
                    #endregion
    
                    #region ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal
    
                    case ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal:
                        lstEtiq = EtiquetaDAO.Instance.GetListNFe(login.CodUser, idImpressao, idProdNf, outrosParametros[0].ToString(),
                            reImprimir, Request["numEtiqueta"]);
    
                        break;
    
                    #endregion
    
                    #region ProdutoImpressaoDAO.TipoEtiqueta.Retalho
    
                    case ProdutoImpressaoDAO.TipoEtiqueta.Retalho :
                        var listaRetalhoProducao = new List<RetalhoProducao>();
    
                        if (idRetalhoProducao > 0)
                        {
                            listaRetalhoProducao.Add(RetalhoProducaoDAO.Instance.Obter(idRetalhoProducao));
                        }
                        else
                        {
                            var retalhos = Newtonsoft.Json.JsonConvert.DeserializeObject<RetalhoProducaoAuxiliarCollection>(outrosParametros[3].ToString());
                            var ids = new List<uint>();
    
                            if (Request["numEtiqueta"] != null)
                            {
                                string[] numeroEtiqueta = Request["numEtiqueta"].ToString().Split(';');
    
                                foreach (var r in retalhos)
                                {
                                    foreach (string n in numeroEtiqueta)
                                    {
                                        ids = RetalhoProducaoDAO.Instance.CriarRetalho(r.Altura.ToString(), r.Largura.ToString(),
                                            r.Quantidade.ToString(), r.Observacao.ToString(), n, login);
    
                                        foreach (uint id in ids)
                                        {
                                            RetalhoProducao ret = RetalhoProducaoDAO.Instance.Obter(id);
                                            listaRetalhoProducao.Add(ret);
                                        }
                                    }
                                }
                            }
                            else if (Request["idProd"] != null)
                            {
                                var dados = Request["idProd"].ToString().Split(';');
    
                                uint idProd = dados.Length == 1 ? Conversoes.ConverteValor<uint>(dados[0]) : Conversoes.ConverteValor<uint>(dados[1]);
                                uint idProdNF = dados.Length == 1 ? 0 : Conversoes.ConverteValor<uint>(dados[0]);
    
                                ids = RetalhoProducaoDAO.Instance.CriarRetalho(retalhos.GetList(), idProd, idProdNF, login);
    
                                foreach (uint id in ids)
                                {
                                    RetalhoProducao ret = RetalhoProducaoDAO.Instance.Obter(id);
                                    listaRetalhoProducao.Add(ret);
                                }
                            }
                        }
    
                        lstEtiq = EtiquetaDAO.Instance.GetListRetalho(login.CodUser, idImpressao, listaRetalhoProducao, idRetalhoProducao, reImprimir);

                        foreach (var etiqueta in lstEtiq)
                            if (string.IsNullOrEmpty(etiqueta.NomeCliente))
                                etiqueta.NomeCliente = " ";
    
                    break;
    
                    #endregion
                }

                /* Chamado 43775. */
                var idLojaPedido =
                    EtiquetaConfig.RelatorioEtiqueta.ModeloEtiquetaPorLoja && lstEtiq != null && lstEtiq.Length > 0 && lstEtiq[0].IdPedido.StrParaInt() > 0 ?
                        PedidoDAO.Instance.ObtemIdLoja(lstEtiq[0].IdPedido.StrParaUint()) : 0;

                if (Request["reposicao"] == "true")
                    report.ReportPath = NomeArquivoRelatorioReposicao((int)idLojaPedido);
                else
                    report.ReportPath = EtiquetaConfig.RelatorioEtiqueta.NomeArquivoRelatorio((int)idLojaPedido,
                        tipoEtiqueta.Value == ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal,
                        tipoEtiqueta.Value == ProdutoImpressaoDAO.TipoEtiqueta.Retalho);
    
                report.DataSources.Add(new ReportDataSource("Etiqueta", lstEtiq));
                lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogo()));
                lstParam.Add(new ReportParameter("IsRetalho", (tipoEtiqueta.Value == ProdutoImpressaoDAO.TipoEtiqueta.Retalho).ToString().ToLower()));
            }

            return null;
        }

        /// <summary>
        /// Nome do arquivo do relatório.
        /// </summary>
        public string NomeArquivoRelatorioReposicao(int idLoja)
        {
            if (EtiquetaConfig.RelatorioEtiqueta.ModeloEtiquetaPorLoja && idLoja > 0)
            {
                var caminhoRelatorioLoja = string.Format("Relatorios/ModeloEtiqueta/rptEtiqueta{0}Rep{1}.rdlc", ControleSistema.GetSite().ToString(), idLoja);

                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorioLoja))))
                    return caminhoRelatorioLoja;
            }

            var caminhoRelatorio = string.Format("Relatorios/ModeloEtiqueta/rptEtiqueta{0}Rep.rdlc", ControleSistema.GetSite().ToString());

            if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorio))))
                return caminhoRelatorio;

            return PedidoConfig.LiberarPedido ? "Relatorios/ModeloEtiqueta/rptEtiquetaLib.rdlc" :
                "Relatorios/ModeloEtiqueta/rptEtiqueta.rdlc";
        }

        #region Métodos AJAX

        [Ajax.AjaxMethod()]
        public string IsProdutoLaminadoComposicao(string idProdPed)
        {
            /* Etiqueta de nota ou plano de corte, não devem ser verificadas. */
            if (string.IsNullOrEmpty(idProdPed) || idProdPed.Contains("A") || idProdPed.Contains("N") || idProdPed.Contains("_"))
                return "false";

            idProdPed = idProdPed.Replace("R", "");

            if (idProdPed.StrParaUintNullable().GetValueOrDefault() == 0)
                return "Erro|Não foi possível recuperar o produto da peça. ID: " + idProdPed;

            var idProd = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(null, idProdPed.StrParaUint());

            if (idProd == 0)
                return "Erro|Não foi possível recuperar o produto da peça. ID: " + idProdPed;

            var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)idProd);

            return (tipoSubgrupo == TipoSubgrupoProd.VidroDuplo || tipoSubgrupo == TipoSubgrupoProd.VidroLaminado).ToString();
        }

        [Ajax.AjaxMethod()]
        public string PodeImprimir(string etiqueta)
        {
            var idProdPed = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(null, etiqueta);
            var qtdImpressa = ProdutosPedidoEspelhoDAO.Instance.ObterQtdeImpresso(idProdPed);

            return idProdPed + "\t" + qtdImpressa + "\t" + 1 + "\t" + "" + "\t" + etiqueta + "|";
        }

        #endregion
    }
}
