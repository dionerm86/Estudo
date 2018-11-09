using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using System.Text;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.IO;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstEtiquetaImprimir : System.Web.UI.Page
    {
        #region Propriedades

        /// <summary>
        /// Itens da otimização.
        /// </summary>
        public IEnumerable<EtiquetaProducao> ItensOtimizacao { get; set; }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstEtiquetaImprimir));
    
            txtNumero.Focus();
    
            if (!IsPostBack)
            {
                lnkImportarArquivo.Visible = EtiquetaConfig.TipoExportacaoEtiqueta != DataSources.TipoExportacaoEtiquetaEnum.Nenhum;
    
                if (!String.IsNullOrEmpty(Request["idPedido"]))
                    txtNumero.Text = Request["idPedido"];

                lnkArqOtimizacaoSemSag.Visible = PCPConfig.PermitirGerarArqOtimizacaoSemSag;
                lnkArqOtimizacaoSemExportadas.Visible = PCPConfig.ExibirOpcaoExportarApenasNaoExportadasOptyway;

                int id = 0;
                if (int.TryParse(Request["idSolucaoOtimizacao"], out id))
                {
                    hdfIdSolucaoOtimizacao.Value = id.ToString();
                    var otimizacaoFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                        .GetInstance<Glass.Otimizacao.Negocios.IOtimizacaoFluxo>();

                    ItensOtimizacao = otimizacaoFluxo.ObterItensPelaSolucao(id).Select(f => new EtiquetaProducao(f));
                }
                
            }
    
            if (!String.IsNullOrEmpty(hdfIdsPedidoNFe.Value) && !String.IsNullOrEmpty(hdfIdProdPedNf.Value))
            {
                StringBuilder script = new StringBuilder();
    
                foreach (string id in hdfIdsPedidoNFe.Value.Split(','))
                    if (!String.IsNullOrEmpty(id))
                    {
                        script.AppendFormat("document.getElementById('{0}').value = '{1}'; getProduto('{2}');\n",
                            txtNumero.ClientID, id, hdfIdProdPedNf.Value.TrimEnd(','));
                    }
    
                Page.ClientScript.RegisterStartupScript(GetType(), "init", script.ToString(), true);
    
                // Limpa os campos para não haver erros ao buscar os produtos
                hdfIdsPedidoNFe.Value = "";
                hdfIdProdPedNf.Value = "";
            }

            if (EtiquetaConfig.RelatorioEtiqueta.ModeloEtiquetaPorLoja && !Data.Helper.UserInfo.GetUserInfo.IsAdministrador)
            {
                drpLoja.Enabled = false;
                drpLoja.SelectedValue = UserInfo.GetUserInfo.IdLoja.ToString();
            }
        }

        /// <summary>
        /// Obtém os produtos do grupo vidro ou de pedido mão de obra do pedido passado para serem impressos.
        /// </summary>
        /// <param name="idPedidoStr">idPedidoStr.</param>
        /// <param name="idLojaStr">idLojaStr.</param>
        /// <param name="idProcessoStr">idProcessoStr.</param>
        /// <param name="idAplicacaoStr">idAplicacaoStr.</param>
        /// <param name="idsProdPedAmbiente">idsProdPedAmbiente.</param>
        /// <param name="idCorVidroStr">idCorVidroStr.</param>
        /// <param name="espessuraStr">espessuraStr.</param>
        /// <param name="idSubgrupoProdStr">idSubgrupoProdStr.</param>
        /// <param name="alturaMinStr">alturaMinStr.</param>
        /// <param name="alturaMaxStr">alturaMaxStr.</param>
        /// <param name="larguraMinStr">larguraMinStr.</param>
        /// <param name="larguraMaxStr">larguraMaxStr.</param>
        /// <param name="noCache">noCache.</param>
        /// <returns>Retorna os produtos do grupo vidro ou de pedido mão de obra do pedido passado para serem impressos.</returns>
        [Ajax.AjaxMethod()]
        public string GetProdByPedido(
            string idPedidoStr,
            string idLojaStr,
            string idProcessoStr,
            string idAplicacaoStr,
            string idsProdPedAmbiente,
            string idCorVidroStr,
            string espessuraStr,
            string idSubgrupoProdStr,
            string alturaMinStr,
            string alturaMaxStr,
            string larguraMinStr,
            string larguraMaxStr,
            string noCache)
        {
            try
            {
                var idPedido = idPedidoStr.StrParaUint();

                // Verifica se já foi gerado um espelho para este pedido.
                if (!PedidoEspelhoDAO.Instance.ExisteEspelho(null, idPedido))
                {
                    return "Erro\tAinda não foi gerada uma conferência para este pedido.";
                }

                var situacaoPedidoEspelho = PedidoEspelhoDAO.Instance.ObtemSituacao(null, idPedido);

                // Verifica se o espelho não está em aberto.
                if (situacaoPedidoEspelho == PedidoEspelho.SituacaoPedido.Processando ||
                    situacaoPedidoEspelho == PedidoEspelho.SituacaoPedido.Aberto)
                {
                    return "Erro\tEsta conferência do pedido " + idPedido + " ainda não foi finalizada.";
                }

                var idLoja = idLojaStr.StrParaIntNullable();
                var idProcesso = idProcessoStr.StrParaUint();
                var idAplicacao = idAplicacaoStr.StrParaUint();
                var idCorVidro = idCorVidroStr.StrParaUint();
                var espessura = espessuraStr.StrParaFloat();
                var idSubgrupoProd = idSubgrupoProdStr.StrParaUint();
                var alturaMin = alturaMinStr.StrParaFloat();
                var alturaMax = alturaMaxStr.StrParaFloat();
                var larguraMin = larguraMinStr.StrParaInt();
                var larguraMax = larguraMaxStr.StrParaInt();

                var permissaoImpEtiq = Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetas);
                var permissaoImpMaoDeObra = Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra);
                var pedidoMaoDeObra = PedidoDAO.Instance.IsMaoDeObra(null, idPedido);

                if (!permissaoImpEtiq && !permissaoImpMaoDeObra)
                {
                    return "Erro\tVocê não tem permissão para imprimir etiquetas.";
                }

                var produtosImprimir = new List<string>();

                if (!pedidoMaoDeObra)
                {
                    // Verifica se o funcionário pode imprimir qualquer tipo de etiquetas.
                    if (!permissaoImpEtiq)
                    {
                        if (permissaoImpMaoDeObra)
                        {
                            return "Erro\tVocê pode imprimir etiquetas apenas de pedidos de mão de obra.";
                        }
                        else
                        {
                            return "Erro\tVocê não tem permissão para imprimir etiquetas.";
                        }
                    }

                    var produtosPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetProdToEtiq(
                        idPedido,
                        idProcesso,
                        idAplicacao,
                        idCorVidro,
                        espessura,
                        idSubgrupoProd,
                        alturaMin,
                        alturaMax,
                        larguraMin,
                        larguraMax,
                        idLoja);

                    // Filtra pelos produtos desejados.
                    if (!string.IsNullOrWhiteSpace(idsProdPedAmbiente))
                    {
                        var ids = idsProdPedAmbiente.Split(',').Select(x => x.StrParaUint()).ToList();
                        produtosPedidoEspelho = produtosPedidoEspelho.Where(x => ids.Contains(x.IdProdPed)).ToList();
                    }

                    // Verifica se há produtos a serem adicionados.
                    if (string.IsNullOrWhiteSpace(idsProdPedAmbiente) && produtosPedidoEspelho.Count == 0)
                    {
                        var idsImpressoes = ImpressaoEtiquetaDAO.Instance.GetIdsByPedido(null, idPedido);
                        var producao = PedidoDAO.Instance.IsProducao(null, idPedido) ? ", que não seja para estoque," : string.Empty;
                        var mensagem = $"Erro\tNão há nenhum produto{producao} que atende aos filtros selecionados neste Pedido que não tenha sido impresso.";

                        if (!string.IsNullOrWhiteSpace(idsImpressoes))
                        {
                            mensagem += $"\nImpressões: {idsImpressoes}.";
                        }

                        return mensagem;
                    }

                    var idsProdPedQuantidadeEtiquetasExportadas = ProdutosPedidoEspelhoDAO.Instance.ObterQuantidadeEtiquetasExportadas(
                        null,
                        produtosPedidoEspelho.Select(f => (int)f.IdProdPed).ToList());

                    foreach (var produtoPedidoEspelho in produtosPedidoEspelho)
                    {
                        if (produtoPedidoEspelho.PecaReposta && produtoPedidoEspelho.NumEtiqueta == null)
                        {
                            throw new Exception($"A etiqueta da peça repostas está nula. IdProdPed: {produtoPedidoEspelho.IdProdPed}.");
                        }

                        var qtde = produtoPedidoEspelho.Qtde;

                        if (produtoPedidoEspelho.PecaReposta)
                        {
                            qtde = 1;
                        }
                        else if (produtoPedidoEspelho.IsProdutoLaminadoComposicao)
                        {
                            qtde = (int)produtoPedidoEspelho.QtdeImpressaoProdLamComposicao;
                        }
                        else if (produtoPedidoEspelho.IsProdFilhoLamComposicao)
                        {
                            qtde = ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(null, produtoPedidoEspelho.IdProdPedParent.Value) * produtoPedidoEspelho.Qtde;

                            var idProdPedParentPai = ProdutosPedidoEspelhoDAO.Instance.ObterIdProdPedParent(null, produtoPedidoEspelho.IdProdPedParent.Value);

                            if (idProdPedParentPai > 0)
                            {
                                qtde *= ProdutosPedidoEspelhoDAO.Instance.ObtemQtde(null, idProdPedParentPai.Value);
                            }
                        }

                        var descricaoProduto = produtoPedidoEspelho.DescricaoProdutoComBenef.Replace("|", string.Empty).Replace(";", string.Empty) +
                            (produtoPedidoEspelho.PecaReposta ? " (Reposta)" : string.Empty);
                        var codigoProcesso = produtoPedidoEspelho?.CodProcesso?.Replace("|", string.Empty).Replace(";", string.Empty) ?? string.Empty;
                        var codigoAplicacao = produtoPedidoEspelho?.CodAplicacao?.Replace("|", string.Empty).Replace(";", string.Empty) ?? string.Empty;
                        var observacaoGrid = !string.IsNullOrWhiteSpace(produtoPedidoEspelho.Obs) && !PCPConfig.Etiqueta.NaoExibirObsPecaAoImprimirEtiqueta
                            ? produtoPedidoEspelho.Obs.Replace("|", string.Empty).Replace(";", string.Empty)
                            : string.Empty;
                        var numEtiquetaPecaReposta = produtoPedidoEspelho.PecaReposta && !string.IsNullOrWhiteSpace(produtoPedidoEspelho.NumEtiqueta)
                            ? produtoPedidoEspelho.NumEtiqueta.ToLower()
                            : string.Empty;
                        var larguraProducao = produtoPedidoEspelho.Redondo ?
                            0 :
                            produtoPedidoEspelho.LarguraProducao;
                        var quantidadeImpressa = produtoPedidoEspelho.PecaReposta
                            ? 1
                            : produtoPedidoEspelho.QtdImpresso;
                        var quantidadeEtiquetasExportadas =
                            idsProdPedQuantidadeEtiquetasExportadas?.Where(f => f.IdProdPed == produtoPedidoEspelho.IdProdPed)?.Sum(f => f.QuantidadeExportada) ?? 0;
                        var qtdeCalcular = qtde > 0
                            ? qtde
                            : produtoPedidoEspelho.Qtde;
                        var totM2 = produtoPedidoEspelho.PecaReposta
                            ? produtoPedidoEspelho.TotM / produtoPedidoEspelho.Qtde
                            : produtoPedidoEspelho.TotM / produtoPedidoEspelho.Qtde * (qtdeCalcular - produtoPedidoEspelho.QtdImpresso);
                        var totM2Calc = produtoPedidoEspelho.PecaReposta
                            ? produtoPedidoEspelho.TotM2Calc / produtoPedidoEspelho.Qtde
                            : produtoPedidoEspelho.TotM2Calc / produtoPedidoEspelho.Qtde * (qtdeCalcular - produtoPedidoEspelho.QtdImpresso);

                        produtosImprimir.Add(string.Format(
                            "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14}",
                            produtoPedidoEspelho.IdProdPed,
                            string.Empty,
                            produtoPedidoEspelho.IdPedido,
                            descricaoProduto,
                            codigoProcesso,
                            codigoAplicacao,
                            qtde,
                            quantidadeImpressa,
                            produtoPedidoEspelho.AlturaProducao,
                            larguraProducao,
                            observacaoGrid,
                            totM2.ToString("0.##"),
                            numEtiquetaPecaReposta,
                            totM2Calc.ToString("0.##"),
                            quantidadeEtiquetasExportadas));
                    }
                }
                else
                {
                    var ambientes = AmbientePedidoEspelhoDAO.Instance.GetForEtiquetas(
                        idPedido,
                        idProcesso,
                        idAplicacao,
                        idCorVidro,
                        espessura,
                        idSubgrupoProd,
                        alturaMin,
                        alturaMax,
                        larguraMin,
                        larguraMax);

                    // Filtra pelos produtos desejados.
                    if (!string.IsNullOrWhiteSpace(idsProdPedAmbiente))
                    {
                        var ids = idsProdPedAmbiente.Split(',').Select(x => x.Replace("A", string.Empty).StrParaUint()).ToList();
                        ambientes = ambientes.Where(x => ids.Contains(x.IdAmbientePedido)).ToList();
                    }

                    // Verifica se há produtos a serem adicionados.
                    if (string.IsNullOrEmpty(idsProdPedAmbiente) && ambientes.Count == 0)
                    {
                        var msg = "Erro\tNão há nenhum produto que atende aos filtros selecionados neste Pedido que não tenha sido impresso.";
                        var idsImpressoes = ImpressaoEtiquetaDAO.Instance.GetIdsByPedido(null, idPedido);

                        if (!string.IsNullOrWhiteSpace(idsImpressoes))
                        {
                            msg += $"\nImpressões: {idsImpressoes}";
                        }

                        return msg;
                    }

                    foreach (var ambiente in ambientes)
                    {
                        var totM2 = ambiente.TotM / ambiente.Qtde.Value * (ambiente.Qtde.Value - ambiente.QtdeImpresso.Value);
                        var descricaoAmbiente = ambiente.Ambiente.Replace("|", string.Empty).Replace(";", string.Empty) +
                            (ambiente.Redondo
                                ? " REDONDO"
                                : string.Empty);
                        var codigoProcesso = ambiente?.CodProcesso?.Replace("|", string.Empty).Replace(";", string.Empty) ?? string.Empty;
                        var codigoAplicacao = ambiente?.CodAplicacao?.Replace("|", string.Empty).Replace(";", string.Empty);
                        var larguraAmbiente = ambiente.Redondo ? 0 : ambiente.Largura.Value;

                        produtosImprimir.Add(string.Format(
                            "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14}",
                            string.Empty,
                            ambiente.IdAmbientePedido,
                            ambiente.IdPedido,
                            descricaoAmbiente,
                            codigoProcesso,
                            codigoAplicacao,
                            ambiente.Qtde,
                            ambiente.QtdeImpresso,
                            ambiente.Altura,
                            larguraAmbiente,
                            string.Empty,
                            totM2.ToString("0.##"),
                            string.Empty,
                            string.Empty,
                            string.Empty));
                    }
                }

                return $"ok\t{string.Join("|", produtosImprimir.Select(f => f.Replace('\t', ' ')))}";
            }
            catch (Exception ex)
            {
                return MensagemAlerta.FormatErrorMsg("Erro\t", ex);
            }
        }

        [Ajax.AjaxMethod]
        public string GetProdByNFe(string numeroNotaStr, string idFornecStr, string idsProdNf, string idCorVidroStr, 
            string espessuraStr, string alturaMinStr, string alturaMaxStr, string larguraMinStr, string larguraMaxStr, string noCache)
        {
            try
            {
                if (!Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasNFe) &&
                    !Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetas))
                    return "Erro\tVocê não tem permissão de imprimir de etiquetas de NF-e.";

                uint numeroNota = Glass.Conversoes.StrParaUint(numeroNotaStr);
                uint? idFornec = Glass.Conversoes.StrParaUintNullable(idFornecStr);
    
                uint idNf = NotaFiscalDAO.Instance.GetIdByNumeroEntradaTerc(numeroNota, idFornec);
    
                // Verifica se já foi gerado um espelho para este pedido
                if (idNf == 0)
                    return "Erro\t" + (idFornec > 0 ? "Essa nota fiscal não existe para esse fornecedor." : 
                        "Essa nota fiscal de importação não existe");
    
                // Verifica se o espelho não está em aberto
                if (idFornec > 0 && !NotaFiscalDAO.Instance.IsFinalizada(null, idNf))
                    return "Erro\tEssa nota fiscal ainda não foi finalizada.";
                else if (idFornec.GetValueOrDefault() == 0 && !NotaFiscalDAO.Instance.IsAutorizada(idNf))
                    return "Erro\tEssa nota fiscal ainda não foi autorizada.";

                if(!NotaFiscalDAO.Instance.GerarEtiqueta(idNf))
                    return "Erro\tEssa nota fiscal não deve gerar etiqueta.";
                
                uint idCorVidro = Glass.Conversoes.StrParaUint(idCorVidroStr);
                float espessura = Glass.Conversoes.StrParaFloat(espessuraStr);
                float alturaMin = Glass.Conversoes.StrParaFloat(alturaMinStr);
                float alturaMax = Glass.Conversoes.StrParaFloat(alturaMaxStr);
                int larguraMin = Glass.Conversoes.StrParaInt(larguraMinStr);
                int larguraMax = Glass.Conversoes.StrParaInt(larguraMaxStr);
    
                StringBuilder str = new StringBuilder();
    
                var lstProd = ProdutosNfDAO.Instance.GetForImpressaoEtiqueta(idNf, idCorVidro, espessura, alturaMin,
                    alturaMax, larguraMin, larguraMax);
    
                // Filtra pelos produtos desejados
                if (!String.IsNullOrEmpty(idsProdNf))
                {
                    List<uint> ids = idsProdNf.Split(',').Select(x => Glass.Conversoes.StrParaUint(x.Replace("N", ""))).ToList();  //new List<uint>(Array.ConvertAll(idsProdNf.Split(','), x => Glass.Conversoes.StrParaUint(x.Replace("N", ""))));
                    lstProd = lstProd.Where(x => ids.Contains(x.IdProdNf)).ToList(); //Array.FindAll(lstProd, x => ids.Contains(x.IdProdNf));
                }
                
                // Verifica se há produtos a serem adicionados
                if (String.IsNullOrEmpty(idsProdNf) && lstProd.Count == 0)
                {
                    string idsImpressoes = ImpressaoEtiquetaDAO.Instance.GetIdsByNFe(idNf);
    
                    return "Erro\tNão há nenhum produto do grupo Vidro (que seja matéria-prima e que " +
                        "tenha informações de altura, largura e quantidade) ou PVB nesta NF-e que não tenha sido impresso." + 
                        (!String.IsNullOrEmpty(idsImpressoes) ? "\nImpressões: " + idsImpressoes : String.Empty);
                }
    
                foreach (ProdutosNf p in lstProd)
                {
                    float totM2 = p.TotM / p.Qtde * (p.Qtde - p.QtdImpresso);
    
                    str.Append(p.IdProdNf + ";");
                    str.Append(numeroNota + (idFornec > 0 ? " (Forn.: " + FornecedorDAO.Instance.GetNome(idFornec.Value) + ")" : " (Importação)") + ";");
                    str.Append(p.DescrProduto.Replace("|", "").Replace(";", "") + ";");
                    str.Append(p.Qtde.ToString() + ";");
                    str.Append(p.QtdImpresso.ToString() + ";");
                    str.Append(p.Altura + ";");
                    str.Append(p.Largura.ToString() + ";");
                    str.Append(totM2.ToString("0.##") + ";");
                    str.Append(p.IdProd + ";");
                    str.Append(p.Lote + "|");
                }
    
                return "Ok\t" + str.ToString().Replace('\t', ' ').TrimEnd('|');
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\t", ex);
            }
        }
    
        protected bool IsExportacaoOptyWay()
        {
            return EtiquetaConfig.TipoExportacaoEtiqueta == DataSources.TipoExportacaoEtiquetaEnum.OptyWay;
        }
    
        protected int GetTipoImpressaoEtiquetaPedido()
        {
            return (int)ProdutoImpressaoDAO.TipoEtiqueta.Pedido;
        }
    
        [Ajax.AjaxMethod()]
        public string ValidarPedidosJaExportados(string idsProdPed)
        {
            try
            {
                string idsPedido = ProdutosPedidoEspelhoDAO.Instance.ObtemIdsPedido(idsProdPed.Replace("R", "").Replace("A", ""));
    
                if (!String.IsNullOrEmpty(idsPedido))
                    idsPedido = Glass.Data.DAL.ArquivoOtimizacaoDAO.Instance.PedidosJaExportados(idsPedido);
    
                if (!String.IsNullOrEmpty(idsPedido))
                    return "true\t" + idsPedido;
    
                return "false\t";
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao validar pedidos já exportados.", ex);
            }
        }
    
        [Ajax.AjaxMethod()]
        public string AlteraDataFabricacao(string valor, string data, string alterarReposicao)
        {
            try
            {
                string ids = valor.Remove(valor.LastIndexOf(','));
                PedidoEspelhoDAO.Instance.AlterarDataFabrica(ids, Convert.ToDateTime(data), bool.Parse(alterarReposicao));
                return "Alteração feita com sucesso. Atenção: foram alterados somentes os pedidos nos quais esta nova data de fábrica é menor que a data de entrega.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    
        [Ajax.AjaxMethod()]
        public string ImpressoesProcessando()
        {
            return ImpressaoEtiquetaDAO.Instance.ObtemImpressoesProcessando();
        }
    
        [Ajax.AjaxMethod]
        public string GetLinkRetalhos(string nomeControle, string idProdPed, string campoQtdeImprimir)
        {
            if (!PCPConfig.Etiqueta.UsarControleRetalhos)
                return "";
    
            Glass.UI.Web.Controls.ctrlRetalhoAssociado controle = new Glass.UI.Web.Controls.ctrlRetalhoAssociado();
            controle = controle.LoadControl("~/Controls/ctrlRetalhoAssociado.ascx") as Glass.UI.Web.Controls.ctrlRetalhoAssociado;
    
            controle.ID = "ctrlRetalhos_" + nomeControle;
            controle.IdProdPed = Glass.Conversoes.StrParaUint(idProdPed.TrimStart('R'));
            controle.CampoQtdeImprimir = campoQtdeImprimir;
            controle.Callback = "callbackRetalhos";
            controle.CallbackSelecao = "callbackSelecaoRetalhos";
    
            StringBuilder sb = new StringBuilder();
    
            using (StringWriter sw = new StringWriter(sb))
                using (HtmlTextWriter writer = new HtmlTextWriter(sw))
                {
                    controle.RenderControl(writer);
                }
    
            string retorno = sb.ToString(), script = controle.ObtemVariavelJavaScript();
            var dadosScript = script.Substring(4).Split('=');
            script = "window['" + dadosScript[0].Trim() + "'] = " + dadosScript[1].Trim() + "; ";
    
            int indexScript = retorno.IndexOf("<script");
    
            if (indexScript > -1)
            {
                script += retorno.Substring(indexScript).Replace("</script>", "").
                    Replace("<script type=\"text/javascript\">", "");
    
                retorno = retorno.Remove(indexScript);
            }
    
            return retorno + "|" + script + "|" + dadosScript[0].Trim();
        }

        protected void btnNova_Click(object sender, EventArgs e)
        {
            Response.Redirect("LstEtiquetaImprimir.aspx");
        }     

        [Ajax.AjaxMethod()]
        public string PodeImprimir()
        {
            return ImpressaoEtiquetaDAO.Instance.ObtemImpressoesProcessando() + "|" +
                ImpressaoEtiquetaDAO.Instance.ExistemProdutosImpressaoSemImpressao().ToString().ToLower();
        }

        [Ajax.AjaxMethod()]
        public string PodeImprimirPedidoImportado(string idPedido)
        {
            if (PCPConfig.PermitirImpressaoDePedidosImportadosApenasConferidos && PedidoDAO.Instance.IsPedidoImportado(null, idPedido.StrParaUint()))
                return PedidoEspelhoDAO.Instance.IsPedidoConferido(idPedido.StrParaUint()).ToString();

            return "true";
        }

        [Ajax.AjaxMethod()]
        public string PodeImprimirPedidosImportados(string idsPedido)
        {
            if (PCPConfig.PermitirImpressaoDePedidosImportadosApenasConferidos)
                return PedidoEspelhoDAO.Instance.VerificarPedidoConferidos(idsPedido).ToString();

            return "";
        }

        public bool PermissaoParaImprimir()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetas);
        }

        public bool PermissaoParaImprimirMaoDeObra()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra);
        }

        public bool PermissaoParaImprimirNFe()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasNFe);
        }

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

        /// <summary>
        /// Valida os retalhos selecionados.
        /// </summary>
        [Ajax.AjaxMethod()]
        public void ValidaRetalhos(string[] dados)
        {
            var dicRetalhos = new Dictionary<uint, double>();

            foreach (var d in dados)
            {
                var idProdPed = d.Split('|')[0].StrParaUint();
                var idsRetalhos = d.Split('|')[1].Split(',').Select(f => f.StrParaUint()).ToArray();
                var qtdImpUsuario = d.Split('|')[2].StrParaUint();

                for (int j = 0; j < qtdImpUsuario; j++)
                {
                    string validacaoRetalho = "";

                    for (int i = 0; i < idsRetalhos.Length; i++)
                    {
                        var ret = RetalhoProducaoDAO.Instance.Obter(null, idsRetalhos[i]);

                        if (!dicRetalhos.ContainsKey(idsRetalhos[i]))
                            dicRetalhos.Add(idsRetalhos[i], ret.TotMUsando);
                        else
                            ret.TotMUsando = dicRetalhos[idsRetalhos[i]];

                        validacaoRetalho = Data.RelDAL.EtiquetaDAO.Instance.ValidaRetalho(null, idProdPed, ret);

                        if (string.IsNullOrEmpty(validacaoRetalho))
                        {
                            var idPedido = ProdutosPedidoEspelhoDAO.Instance.ObtemIdPedido(null, idProdPed);
                            var tipoPedido = PedidoDAO.Instance.ObterTipoPedido(null, idPedido);

                            float altura;
                            float largura;

                            if (tipoPedido == Data.Model.Pedido.TipoPedidoEnum.MaoDeObra)
                            {
                                var idAmbientePedido = ProdutosPedidoEspelhoDAO.Instance.ObtemIdAmbientePedido(null, idProdPed);
                                altura = AmbientePedidoDAO.Instance.ObtemAltura(idAmbientePedido.GetValueOrDefault(0));
                                largura = AmbientePedidoDAO.Instance.ObtemLargura(idAmbientePedido.GetValueOrDefault(0));
                            }
                            else
                            {
                                altura = ProdutosPedidoEspelhoDAO.Instance.ObtemAltura(null, idProdPed);
                                largura = ProdutosPedidoEspelhoDAO.Instance.ObtemLargura(null, idProdPed);
                            }

                            dicRetalhos[idsRetalhos[i]] += (altura * largura) / 1000000;
                            break;
                        }
                        else if (i + 1 < idsRetalhos.Length)
                            continue;
                        else
                            throw new Exception(validacaoRetalho);
                    }
                }
            }
        }

        #region Tipos Aninhados

        /// <summary>
        /// Reprsenta o wrapper do item de otimização para a etiqueta de producao.
        /// </summary>
        public class EtiquetaProducao
        {
            #region Variáveis Locais

            private readonly Glass.Otimizacao.Negocios.ItemOtimizacao _item;

            #endregion

            #region Propriedades

            public string Tipo => _item.Tipo.ToString();

            public int? IdProdPed => _item.IdProdPed;

            public int? IdAmbiente => null;

            public int? IdPedido => _item.IdPedido;

            public int? IdProdNf => null;

            public int? IdNf => null;

            public string DescricaoProduto => _item.DescricaoProduto;

            public string CodProcesso => _item.CodProcesso;

            public string CodAplicacao => _item.CodAplicacao;

            public int Qtd => _item.Qtde;

            public int QtdImpresso => _item.QtdImpresso;

            public int QtdImprimir => _item.QtdAImprimir;

            public float Altura => _item.AlturaProducao;

            public float Largura => _item.LarguraProducao;

            public string Obs => _item.Obs;

            public float TotM2 => _item.TotM2;

            public string PlanoCorte => _item.PlanoCorteEtiqueta;

            public bool ArquivoOtimizado => true;

            public string Etiquetas => string.Join("_", _item.Etiquetas ?? new string[0]);

            public bool? AtualizarTotais => null;

            public float TotMCalc => _item.TotM2Calc;

            public string Lote => null;

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="item"></param>
            public EtiquetaProducao(Glass.Otimizacao.Negocios.ItemOtimizacao item)
            {
                _item = item;
            }

            #endregion
        }

        #endregion
    }
}
