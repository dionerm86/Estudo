using System;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.Model;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class RelLiberacao : Glass.Relatorios.UI.Web.ReportPage
    {
        protected override object[] Parametros
        {
            get { return new object[] { txtObs.Text }; }
        }

        protected override Glass.Relatorios.UI.Web.ReportPage.JavaScriptData DadosJavaScript
        {
            get
            {
                return new JavaScriptData(
                    Request.QueryString["init"] == "1" && !bool.Parse(hdfInit.Value),
                    "document.getElementById('" + hdfLoad.ClientID + "').value == 'false'"
                );
            }
        }

        private bool _loadSubreport = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (DadosJavaScript.BackgroundLoading)
                return;

            ProcessaReport(pchTabela);
        }

        protected override Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam,
            HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login, string diretorioLogotipos = "")
        {
            var idLojaLogotipo = new uint?();
            var incluirDataTextoRodape = true;

            report.ReportPath = Liberacao.RelatorioLiberacaoPedido.NomeRelatorio;

            var otimizado =
                Liberacao.DadosLiberacao.UsarRelatorioLiberacao4Vias &&
                !report.ReportPath.Contains("rptLiberacaoPedidoMSVidros") &&
                !Liberacao.RelatorioLiberacaoPedido.DuplicarViasDaLiberacaoSeClienteRota;

            #region Padrão (Otimizado)

            if (otimizado)
            {
                // Recupera a liberação
                var liberacao = LiberarPedidoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]));

                // Recupera os pedidos liberados, já ordenando pelo tipo venda e idPedido
                var lstPedidosLib = PedidoDAO.Instance.GetByLiberacao(liberacao.IdLiberarPedido)
                    // Ordena os pedidos da liberação de forma de os pedidos de gerantia e reposição sejam os últimos da lista.
                    .OrderBy(f => f.TipoVenda == (int)Data.Model.Pedido.TipoVendaPedido.Garantia ? 10 :
                        f.TipoVenda == (int)Data.Model.Pedido.TipoVendaPedido.Reposição ? 11 :
                        f.TipoVenda)
                    .ThenBy(f => f.IdPedido)
                    .ToList();

                var caminhoRelatorio = string.Empty;

                /* Chamado 57066. */
                if (Liberacao.RelatorioLiberacaoPedido.UsarImpressaoLiberacaoPorTipoEntrega && !lstPedidosLib.Any(f => f.TipoEntrega != lstPedidosLib[0].TipoEntrega))
                {
                    var nomePastaTipoEntrega = lstPedidosLib[0].TipoEntrega == (int)Data.Model.Pedido.TipoEntregaPedido.Balcao ? "Balcao" : "Entrega";

                    caminhoRelatorio = string.Format("Relatorios/ModeloLiberacao/{0}/{1}/rptLiberacao.rdlc", ControleSistema.GetSite().ToString(), nomePastaTipoEntrega);
                }
                else
                    caminhoRelatorio = string.Format("Relatorios/ModeloLiberacao/{0}/rptLiberacao.rdlc", ControleSistema.GetSite().ToString());

                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorio))))
                    report.ReportPath = caminhoRelatorio;
                else
                    report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/ModeloLiberacao/Padrao/rptLiberacao.rdlc");

                // Customiza texto a ser exibido junto com o nome fantasia do cliente
                liberacao.NomeClienteFantasia += MontarInfoAdicionalRelLiberacao(liberacao.IdCliente);
                
                // Texto que irá aparecer na via de expedição/almoxarife
                var textoResumo = Liberacao.RelatorioLiberacaoPedido.TextoResumosCorteRelatorio4Vias(liberacao.IdLiberarPedido);
                if (Liberacao.RelatorioLiberacaoPedido.ExibirObsLiberacaoClienteViaExpedicao)
                    textoResumo += string.Format("\n\n{0}", ClienteDAO.Instance.ObtemObsLiberacao(liberacao.IdCliente));

                // Texto que irá aparecer na via do cliente
                var textoResumoCliente = Liberacao.RelatorioLiberacaoPedido.TextoResumoClienteRelatorio4Vias;
                if (Liberacao.RelatorioLiberacaoPedido.ExibirObsLiberacaoClienteViaCliente)
                    textoResumoCliente += string.Format("\n\n{0}", ClienteDAO.Instance.ObtemObsLiberacao(liberacao.IdCliente));

                // Texto que irá aparecer na via da empresa
                var textoResumoEmpresa = Liberacao.RelatorioLiberacaoPedido.TextoResumoEmpresaRelatorio4Vias;

                var textoParcelas = "";
                if (Liberacao.RelatorioLiberacaoPedido.TextoParcelasInvertido)
                {
                    foreach (var p in ParcelaLiberacaoDAO.Instance.ObtemParcelasLiberacao(liberacao.IdLiberarPedido))
                    {
                        var texto = Glass.Formatacoes.InverteString(p.DataParcela.ToShortDateString().Replace("/", "")) +
                            Glass.Formatacoes.InverteString(p.ValorParcela.ToString().Replace(",", "")).Replace(".", "");
                        textoParcelas += string.Format("{0}-{1}/{2}", texto.Substring(0, 8), texto.Substring(8, 2), texto.Substring(10)) + Environment.NewLine;
                    }
                }

                /* Chamado 48385. */
                var exibirValorPedidoMaoDeObraViaAlmoxarife = !Liberacao.RelatorioLiberacaoPedido.ExibirValorPedidoMaoDeObraViaAlmoxarife ?
                    !lstPedidosLib.Any(f => f.TipoPedido == (int)Data.Model.Pedido.TipoPedidoEnum.MaoDeObra) : true;

                // Mostra a observação do cliente
                var obsCliente = Liberacao.RelatorioLiberacaoPedido.ExibirObservacaoCliente ?
                    ClienteDAO.Instance.ObtemObs(liberacao.IdCliente) : String.Empty;

                // Mostra as observações dos pedidos liberados
                var obsPedidos = String.Empty;
                var obsLiberacao = String.Empty;
                var obsLiberacaoCliente = string.Empty;

                // Recupera o nome da loja para exibir no relatório
                var nomeLoja = string.Empty;

                foreach (var p in lstPedidosLib)
                {
                    if (!String.IsNullOrEmpty(p.Obs) && Liberacao.RelatorioLiberacaoPedido.ExibirObservacaoPedidos)
                        obsPedidos += "Ped. " + p.IdPedido + " - " + p.Obs.TrimEnd('\r').TrimEnd('\n') + ";";

                    if (!String.IsNullOrEmpty(p.ObsLiberacao))
                        obsLiberacao += "Ped. " + p.IdPedido + " (Obs. Lib.) - " + p.ObsLiberacao.TrimEnd('\r').TrimEnd('\n') + ";\n";

                    /* Chamado 54552. */
                    if (Geral.ConsiderarLojaClientePedidoFluxoSistema && string.IsNullOrEmpty(nomeLoja))
                        nomeLoja = LojaDAO.Instance.GetNome(p.IdLoja);
                }

                if (string.IsNullOrEmpty(nomeLoja))
                    nomeLoja = LojaDAO.Instance.GetNome(FuncionarioDAO.Instance.ObtemIdLoja(liberacao.IdFunc));

                if (!Liberacao.RelatorioLiberacaoPedido.NaoMostrarObsLiberacaoClienteNaLiberacao)
                {
                    var obsLibCli = ClienteDAO.Instance.ObtemObsLiberacao(liberacao.IdCliente);

                    obsLiberacaoCliente +=
                        string.IsNullOrEmpty(obsLibCli) ?
                            string.Empty :
                            string.Format("Obs. Lib. Cliente - {0}", obsLibCli);
                }

                //Define se a via do almoxerifado vai ser igual a do cliente caso o tipo do pedido seja mão de obra
                var viaAlmoxarifadoIgualCliente = "false";
                if (Liberacao.UsarViaAlmoxarifadoIgualClienteSeMaoDeObra && lstPedidosLib.Any(f => f.DescricaoTipoPedido.Contains("Mão de obra")))
                    viaAlmoxarifadoIgualCliente = "true";

                var produtosLib = ProdutosLiberarPedidoDAO.Instance.GetForRpt(liberacao.IdLiberarPedido);
                var produtosCortados = ProdutosPedidoDAO.Instance.ObterProdutosCortados(liberacao.IdLiberarPedido);

                // Carrega Datasets para o relatório                
                var parcelasLiberacao = ParcelaLiberacaoDAO.Instance.ObtemParcelasLiberacao(liberacao.IdLiberarPedido);
                var produtosCortadosRpt = ProdutosCortadosRptDAO.Instance.CopiaLista(produtosCortados.ToArray());
                var lstPedidoRpt = PedidoRptDAL.Instance.CopiaLista(lstPedidosLib.ToArray(), PedidoRpt.TipoConstrutor.RelatorioLiberacao, false, login);
                var lstProdLib = ProdutosLiberarPedidoRptDAL.Instance.CopiaLista(produtosLib);
                var pecasCanceladas = ProdutoPedidoProducaoRptDAL.Instance.CopiaLista(ProdutoPedidoProducaoDAO.Instance.PesquisarProdutosProducaoRelatorioLiberacao((int)liberacao.IdLiberarPedido, true).ToArray());
                var cheques = ChequesDAO.Instance.GetByLiberacaoPedido(null, liberacao.IdLiberarPedido);
                var resumoCorte = ResumoCorteDAO.Instance.ObterResumoCorte(lstProdLib, false);
                var resumoCorteComRevenda = ResumoCorteDAO.Instance.ObterResumoCorte(lstProdLib, true);

                // Se for mão de obra, apaga a quantidade de ambientes de todos os produtos contidos no mesmo, exceto de um deles,
                // para que ao somar a quantidade de ambientes (peças) no relatório a soma fique correta
                var lstIdAmbModificado = new List<uint>();
                foreach (var plp in lstProdLib)
                {
                    if (!plp.PedidoMaoDeObra || plp.IdAmbientePedido == null)
                        continue;

                    if (!lstIdAmbModificado.Contains(plp.IdAmbientePedido.Value))
                    {
                        lstIdAmbModificado.Add(plp.IdAmbientePedido.Value);
                        continue;
                    }

                    plp.QtdeAmbiente = 0;
                }

                // Calcula totais do pedido a serem exibidos na liberação
                CalcularTotaisPedidos(lstProdLib, ref lstPedidoRpt);

                var envioEmail = Request["EnvioEmail"] != null && Request["EnvioEmail"].ToLower() == "true";
                var exibirApenasViaCliente = Request["ApenasViaCliente"] != null && Request["ApenasViaCliente"].ToLower() == "true";

                // Verifica quando as vias devem ser exibidas
                bool exibirViaEmpresa, exibirViaCliente, exibirViaExpedicao, exibirViaAlmoxarife;
                VerificarVisibilidadeVias(Request, lstPedidosLib, produtosLib, out exibirViaEmpresa, out exibirViaCliente, out exibirViaExpedicao, out exibirViaAlmoxarife, envioEmail, exibirApenasViaCliente);

                #region Recupera a quantidade de vidros e materiais dos pedidos

                // Recupera a quantidade de peças de vidro de cada pedido da liberação.
                var numeroVidros =
                    lstPedidosLib.Count > 0 ?
                        lstPedidosLib.Select(f => produtosLib.Count(g => g.IdPedido == f.IdPedido && g.IsVidro).ToString()).ToArray() :
                        new string[] { "0" };

                // Recupera a quantidade de materiais (produtos que não são vidro) de cada pedido da liberação.
                var numeroMateriais =
                    lstPedidosLib.Count > 0 ?
                        lstPedidosLib.Select(f => produtosLib.Count(g => g.IdPedido == f.IdPedido && !g.IsVidro).ToString()).ToArray() :
                        new string[] { "0" };

                #endregion

                lstParam.Add(new ReportParameter("ObsCliente", !String.IsNullOrEmpty(obsCliente) ? "Obs.: " + obsCliente : "."));
                lstParam.Add(new ReportParameter("ObsPedidos", obsPedidos));
                lstParam.Add(new ReportParameter("ObsLiberacaoRpt", obsLiberacao.TrimEnd('\n')));
                lstParam.Add(new ReportParameter("ObsLiberacaoCliente", obsLiberacaoCliente));
                lstParam.Add(new ReportParameter("IdLiberarPedido", Request["idLiberarPedido"]));
                lstParam.Add(new ReportParameter("ExibirProdutos", Liberacao.DadosLiberacao.LiberarPedidoProdutos.ToString()));
                lstParam.Add(new ReportParameter("ExibirLogoTel", "false"));
                lstParam.Add(new ReportParameter("TelefoneLoja", LojaDAO.Instance.GetElement(login.IdLoja).Telefone));
                lstParam.Add(new ReportParameter("NomeLojaLib", nomeLoja));
                lstParam.Add(new ReportParameter("AgruparResumoProdutos", Liberacao.DadosLiberacao.AgruparResumoLiberacaoProduto.ToString()));
                lstParam.Add(new ReportParameter("DataGrande", "false"));
                lstParam.Add(new ReportParameter("ExibirProdutosViaEmpresa", Liberacao.ExibirProdutosViaEmpresa.ToString()));
                lstParam.Add(new ReportParameter("ExibirTabelaParcelas", (FinanceiroConfig.DadosLiberacao.ExibirDescricaoParcelaLiberacao && liberacao.TipoPagto == (int)LiberarPedido.TipoPagtoEnum.APrazo).ToString()));
                lstParam.Add(new ReportParameter("ExibirValoresResumo", Liberacao.RelatorioLiberacaoPedido.ExibirValoresResumosCorte.ToString()));
                lstParam.Add(new ReportParameter("ExibirResumoViaEmpresa", Liberacao.RelatorioLiberacaoPedido.ExibirResumoLiberacaoViaEmpresa.ToString()));
                lstParam.Add(new ReportParameter("ExibirResumoViaCliente", Liberacao.RelatorioLiberacaoPedido.ExibirResumoCorteNaViaCliente.ToString()));
                lstParam.Add(new ReportParameter("ExibirObsLibApenasViaEmpresa", Liberacao.RelatorioLiberacaoPedido.ExibirObsLibApenasViaEmpresa.ToString()));
                lstParam.Add(new ReportParameter("ExibirObsLiberacaoClienteApenasViaEmpresa", Liberacao.RelatorioLiberacaoPedido.ExibirObsLiberacaoClienteApenasViaEmpresa.ToString()));
                lstParam.Add(new ReportParameter("ExibirObsLiberacaoResumo", Liberacao.RelatorioLiberacaoPedido.ExibirObsLiberacaoResumo.ToString()));
                lstParam.Add(new ReportParameter("NaoMostrarObsLiberacaoNaLiberacao", Liberacao.RelatorioLiberacaoPedido.NaoMostrarObsLiberacaoNaLiberacao.ToString()));
                lstParam.Add(new ReportParameter("NaoMostrarObsLiberacaoClienteNaLiberacao", Liberacao.RelatorioLiberacaoPedido.NaoMostrarObsLiberacaoClienteNaLiberacao.ToString()));
                lstParam.Add(new ReportParameter("TextoResumo", textoResumo));
                lstParam.Add(new ReportParameter("TextoResumoCliente", textoResumoCliente));
                lstParam.Add(new ReportParameter("TextoResumoEmpresa", textoResumoEmpresa));
                lstParam.Add(new ReportParameter("TextoParcelas", textoParcelas));
                lstParam.Add(new ReportParameter("NumeroCasasDecimaisTotM", Geral.NumeroCasasDecimaisTotM.ToString()));
                lstParam.Add(new ReportParameter("RatearDescontoProdutos", PedidoConfig.RatearDescontoProdutos.ToString()));
                lstParam.Add(new ReportParameter("ColunaExtraParcela", "false"));
                lstParam.Add(new ReportParameter("ViaAlmoxerifadoIgualCliente", viaAlmoxarifadoIgualCliente));
                lstParam.Add(new ReportParameter("ExibirValorPedidoMaoDeObraViaAlmoxarife", exibirValorPedidoMaoDeObraViaAlmoxarife.ToString()));
                lstParam.Add(new ReportParameter("NumeroViasEmpresa", exibirViaEmpresa ? "1" : "0"));
                lstParam.Add(new ReportParameter("NumeroViasCliente", exibirViaCliente ? "1" : "0"));
                lstParam.Add(new ReportParameter("NumeroViasAlmoxarife", exibirViaAlmoxarife ? "1" : "0"));
                lstParam.Add(new ReportParameter("NumeroViasExpedicao", exibirViaExpedicao ? "1" : "0"));
                lstParam.Add(new ReportParameter("NumeroVidros", numeroVidros));
                lstParam.Add(new ReportParameter("NumeroMateriais", numeroMateriais));
                lstParam.Add(new ReportParameter("ExibirSaldoDevedor", FinanceiroConfig.FinanceiroRec.ExibirSaldoDevedorRelsRecebimento.ToString()));
                lstParam.Add(new ReportParameter("ExibirProdutosCortados", (!produtosCortadosRpt.Any()).ToString()));
                lstParam.Add(new ReportParameter("EnderecoLoja", LojaDAO.Instance.ObtemEnderecoCompleto(FuncionarioDAO.Instance.ObtemIdLoja(liberacao.IdFunc))));

                report.DataSources.Add(new ReportDataSource("LiberarPedido", new LiberarPedido[] { liberacao }));
                report.DataSources.Add(new ReportDataSource("ParcelaLiberacao", parcelasLiberacao));
                report.DataSources.Add(new ReportDataSource("PedidoRpt", lstPedidoRpt));
                report.DataSources.Add(new ReportDataSource("Cheques", cheques));
                report.DataSources.Add(new ReportDataSource("ProdutosLiberarPedidoRpt", lstProdLib));
                report.DataSources.Add(new ReportDataSource("ResumoCorte", resumoCorte));
                report.DataSources.Add(new ReportDataSource("ResumoCorteComRevenda", resumoCorteComRevenda));
                report.DataSources.Add(new ReportDataSource("ProdutoPedidoProducaoRpt", pecasCanceladas));
                report.DataSources.Add(new ReportDataSource("ProdutosCortados", produtosCortadosRpt));
            }

            #endregion

            #region Padrão

            else
            {
                // Busca os pedidos liberados
                var lstPedidosLib = PedidoDAO.Instance.GetByLiberacao(Request["idLiberarPedido"].StrParaUint());

                if (!report.ReportPath.Contains("rptLiberacaoPedidoMSVidros"))
                {
                    var caminhoRelatorio = string.Empty;

                    /* Chamado 57066. */
                    if (Liberacao.RelatorioLiberacaoPedido.UsarImpressaoLiberacaoPorTipoEntrega && !lstPedidosLib.Any(f => f.TipoEntrega != lstPedidosLib[0].TipoEntrega))
                    {
                        var nomePastaTipoEntrega = lstPedidosLib[0].TipoEntrega == (int)Data.Model.Pedido.TipoEntregaPedido.Balcao ? "Balcao" : "Entrega";

                        caminhoRelatorio = string.Format("Relatorios/ModeloLiberacao/{0}/{1}/rptLiberacao.rdlc", ControleSistema.GetSite().ToString(), nomePastaTipoEntrega);
                    }
                    else
                        caminhoRelatorio = string.Format("Relatorios/ModeloLiberacao/{0}/rptLiberacaoPedido.rdlc", ControleSistema.GetSite().ToString());

                    if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorio))))
                        report.ReportPath = caminhoRelatorio;
                    else
                        report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/ModeloLiberacao/rptLiberacaoPedido.rdlc");
                }

                var LiberacaoPedido_idCliente = LiberarPedidoDAO.Instance.GetIdCliente(Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]));
                var multNumeroViasResumoLiberacao = Liberacao.RelatorioLiberacaoPedido.DuplicarViasDaLiberacaoSeClienteRota && RotaClienteDAO.Instance.IsClienteAssociado(LiberacaoPedido_idCliente) ? 2 : 1;
                var numeroViasResumoLiberacao = multNumeroViasResumoLiberacao * (!Liberacao.DadosLiberacao.UsarRelatorioLiberacao4Vias ? 1 :
                    Liberacao.RelatorioLiberacaoPedido.NumeroViasAlmoxarifeLiberacao + Liberacao.RelatorioLiberacaoPedido.NumeroViasExpedicaoLiberacao);
                var numeroItens = Math.Max(2, numeroViasResumoLiberacao);
                var liberarPedido = new List<LiberarPedido>();
                var lstParcLib = Glass.Data.RelDAL.ParcelaLiberacaoDAO.Instance.ObtemParcelasLiberacao(Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]));
                var telefoneLoja = LojaDAO.Instance.GetElement(login.IdLoja).Telefone;

                for (var i = 0; i < numeroItens; i++)
                {
                    var item = LiberarPedidoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]));
                    item.NumeroVia = i < 2 ? i + 1 : 2;
                    item.NumeroResumo = i < numeroViasResumoLiberacao ? i + 1 : numeroViasResumoLiberacao;
                    item.ResumoAlmoxarife = i >= (Liberacao.RelatorioLiberacaoPedido.NumeroViasExpedicaoLiberacao * multNumeroViasResumoLiberacao);

                    liberarPedido.Add(item);

                    // Adiciona informações ao nome do cliente
                    item.NomeClienteFantasia += MontarInfoAdicionalRelLiberacao(item.IdCliente);
                }

                var nomeLoja = LojaDAO.Instance.GetNome(FuncionarioDAO.Instance.ObtemIdLoja(liberarPedido[0].IdFunc));

                if (Geral.ConsiderarLojaClientePedidoFluxoSistema)
                {
                    var idCliente = liberarPedido[0].IdCliente;

                    if (idCliente > 0)
                    {
                        var idLojaCliente = ClienteDAO.Instance.ObtemIdLoja(idCliente);

                        if (idLojaCliente > 0)
                            nomeLoja = LojaDAO.Instance.GetNome(idLojaCliente);
                    }
                }

                lstParam.Add(new ReportParameter("ExibirProdutos", Liberacao.DadosLiberacao.LiberarPedidoProdutos.ToString()));
                lstParam.Add(new ReportParameter("ExibirAssinar", report.ReportPath.Contains("4Vias").ToString()));
                lstParam.Add(new ReportParameter("TelefoneLoja", !String.IsNullOrEmpty(telefoneLoja) ? telefoneLoja : "."));
                lstParam.Add(new ReportParameter("NomeLoja", string.IsNullOrEmpty(nomeLoja) ? "." : nomeLoja));
                lstParam.Add(new ReportParameter("NumeroVias", "2"));
                lstParam.Add(new ReportParameter("AgruparResumoProdutos", Liberacao.DadosLiberacao.AgruparResumoLiberacaoProduto.ToString()));

                if (!Liberacao.DadosLiberacao.UsarRelatorioLiberacao4Vias)
                {
                    var descrVias = new string[] { "(Via da Empresa)", "(Via do Cliente)", "\n(Via de Expedição)", "\n(Via do Almoxarife)" };

                    lstParam.Add(new ReportParameter("ExibirLogoTel", (!Geral.NaoVendeVidro()).ToString()));
                    lstParam.Add(new ReportParameter("ExibirCaixaObs", (!Geral.NaoVendeVidro()).ToString()));
                    lstParam.Add(new ReportParameter("NumeroResumos", (Liberacao.RelatorioLiberacaoPedido.NumeroViasAlmoxarifeLiberacao * numeroViasResumoLiberacao).ToString()));
                    lstParam.Add(new ReportParameter("NaoVendeVidro", Geral.NaoVendeVidro().ToString()));
                    lstParam.Add(new ReportParameter("TituloVia", descrVias));
                }
                else
                {
                    var cliente = ClienteDAO.Instance.GetElement(liberarPedido[0].IdCliente);
                    // Para esta configuração funcionar, o Liberacao.DadosLiberacao.NumeroViasLiberacao deve ser 1
                    var exibirViaEmpresa = Liberacao.RelatorioLiberacaoPedido.ExibirViaEmpresaRelatorio4Vias;

                    // Texto que irá aparecer na via de expedição/almoxarife
                    var textoResumo = Liberacao.RelatorioLiberacaoPedido.TextoResumosCorteRelatorio4Vias(liberarPedido[0].IdLiberarPedido);

                    if (Liberacao.RelatorioLiberacaoPedido.ExibirObsLiberacaoClienteViaExpedicao)
                        textoResumo += string.Format("\n\n{0}", cliente.ObsLiberacao);

                    // Texto que irá aparecer na via do cliente
                    var textoResumoCliente = Liberacao.RelatorioLiberacaoPedido.TextoResumoClienteRelatorio4Vias;

                    if (Liberacao.RelatorioLiberacaoPedido.ExibirObsLiberacaoClienteViaCliente)
                        textoResumoCliente += string.Format("\n\n{0}", cliente.ObsLiberacao);

                    //texto que irá aparecer na via da empresa
                    var textoResumoEmpresa = Liberacao.RelatorioLiberacaoPedido.TextoResumoEmpresaRelatorio4Vias;

                    if (!Liberacao.DadosLiberacao.ExibirViaEmpresaPedidoReposicao)
                        /* Chamado 16868. */
                        exibirViaEmpresa = !lstPedidosLib.Any(f => f.TipoVenda.GetValueOrDefault() == (int)Data.Model.Pedido.TipoVendaPedido.Reposição);

                    var descrVias = new string[] { "(Via da Empresa)", "(Via do Cliente)", "\n(Via de Expedição)", "\n(Via do Almoxarife)" };

                    var textoParcelas = "";
                    if (Liberacao.RelatorioLiberacaoPedido.TextoParcelasInvertido)
                    {
                        foreach (var p in lstParcLib)
                        {
                            var texto = Glass.Formatacoes.InverteString(p.DataParcela.ToShortDateString().Replace("/", "")) +
                                Glass.Formatacoes.InverteString(p.ValorParcela.ToString().Replace(",", "")).Replace(".", "");
                            textoParcelas += string.Format("{0}-{1}/{2}", texto.Substring(0, 8), texto.Substring(8, 2), texto.Substring(10)) + Environment.NewLine;
                        }
                    }

                    lstParam.Add(new ReportParameter("TituloVia", descrVias));
                    lstParam.Add(new ReportParameter("IdLiberarPedido", Request["idLiberarPedido"]));
                    lstParam.Add(new ReportParameter("DataGrande", "false"));
                    lstParam.Add(new ReportParameter("ExibirTabelaParcelas", (FinanceiroConfig.DadosLiberacao.ExibirDescricaoParcelaLiberacao && liberarPedido[0].TipoPagto == (int)LiberarPedido.TipoPagtoEnum.APrazo).ToString()));
                    lstParam.Add(new ReportParameter("ExibirResumoViaEmpresa", Liberacao.RelatorioLiberacaoPedido.ExibirResumoLiberacaoViaEmpresa.ToString()));
                    lstParam.Add(new ReportParameter("ExibirObsLibApenasViaEmpresa", Liberacao.RelatorioLiberacaoPedido.ExibirObsLibApenasViaEmpresa.ToString()));
                    lstParam.Add(new ReportParameter("ExibirResumoViaCliente", Liberacao.RelatorioLiberacaoPedido.ExibirResumoCorteNaViaCliente.ToString()));
                    lstParam.Add(new ReportParameter("ExibirViaEmpresa", exibirViaEmpresa.ToString()));
                    lstParam.Add(new ReportParameter("ExibirValoresResumo", Liberacao.RelatorioLiberacaoPedido.ExibirValoresResumosCorte.ToString()));
                    lstParam.Add(new ReportParameter("NumeroResumosAlmoxarife", (Liberacao.RelatorioLiberacaoPedido.NumeroViasAlmoxarifeLiberacao * numeroViasResumoLiberacao).ToString()));
                    lstParam.Add(new ReportParameter("NumeroResumosExpedicao", (Liberacao.RelatorioLiberacaoPedido.NumeroViasExpedicaoLiberacao * numeroViasResumoLiberacao).ToString()));
                    lstParam.Add(new ReportParameter("TextoResumo", textoResumo));
                    lstParam.Add(new ReportParameter("TextoResumoCliente", textoResumoCliente));
                    lstParam.Add(new ReportParameter("TextoResumoEmpresa", textoResumoEmpresa));
                    lstParam.Add(new ReportParameter("NumeroViasResumoLiberacao", numeroViasResumoLiberacao.ToString()));
                    lstParam.Add(new ReportParameter("ExibirProdutosViaEmpresa", Liberacao.ExibirProdutosViaEmpresa.ToString()));
                    lstParam.Add(new ReportParameter("NaoMostrarObsLiberacaoNaLiberacao", Liberacao.RelatorioLiberacaoPedido.NaoMostrarObsLiberacaoNaLiberacao.ToString()));
                    if (!report.ReportPath.Contains("rptLiberacaoPedido4ViasTemperForte"))
                        lstParam.Add(new ReportParameter("NaoMostrarObsLiberacaoClienteNaLiberacao", Liberacao.RelatorioLiberacaoPedido.NaoMostrarObsLiberacaoClienteNaLiberacao.ToString()));
                    lstParam.Add(new ReportParameter("TextoParcelas", textoParcelas));
                    if (!report.ReportPath.Contains("rptLiberacaoPedido4ViasTemperForte"))
                        lstParam.Add(new ReportParameter("ExibirLogoTel", "false"));

                    /* Chamado 48385. */
                    if (report.ReportPath.Contains("rptLiberacaoPedido4Vias.rdlc"))
                        lstParam.Add(new ReportParameter("ExibirValorPedidoMaoDeObraViaAlmoxarife",
                            (!Liberacao.RelatorioLiberacaoPedido.ExibirValorPedidoMaoDeObraViaAlmoxarife ?
                                !lstPedidosLib.Any(f => f.TipoPedido == (int)Data.Model.Pedido.TipoPedidoEnum.MaoDeObra) : true).ToString()));

                    /* Chamado 36277. */
                    if (report.ReportPath.Contains("4Vias"))
                        lstParam.Add(new ReportParameter("NumeroCasasDecimaisTotM", Geral.NumeroCasasDecimaisTotM.ToString()));

                    if (report.ReportPath.Contains("rptLiberacaoPedidoMSVidros"))
                    {
                        var dadosEmpresa = new List<string>();
                        var loja = LojaDAO.Instance.GetElement(ClienteDAO.Instance.ObtemIdLoja(liberarPedido[0].IdCliente));

                        dadosEmpresa.Add(loja.RazaoSocial);
                        dadosEmpresa.Add(loja.Cidade + " - " + loja.Uf + " - " + loja.Cep);
                        dadosEmpresa.Add("Fone: " + loja.Telefone + " Fax: " + loja.Fax + " credito@msvidros.com.br");

                        lstParam.Add(new ReportParameter("DadosEmpresa", dadosEmpresa.ToArray()));

                        var dadosCliente = new List<string>();

                        dadosCliente.Add(cliente.IdCli + " - " + cliente.Nome + " (" + cliente.NomeFantasia + ")");
                        dadosCliente.Add(cliente.EnderecoEntrega + " N " + cliente.NumeroEntrega);
                        dadosCliente.Add(cliente.CidadeEntrega);
                        dadosCliente.Add(cliente.BairroEntrega);
                        dadosCliente.Add(cliente.UfEntrega);
                        dadosCliente.Add(cliente.CepEntrega);
                        dadosCliente.Add(cliente.TelCont + "  " + cliente.TelCel);

                        lstParam.Add(new ReportParameter("DadosCliente", dadosCliente.ToArray()));
                    }
                    /* Chamado 47680. */ /* Chamado 47815 */
                    else if (!report.ReportPath.Contains("rptLiberacaoPedido4ViasTemperForte"))
                    {
                        lstParam.Add(new ReportParameter("ExibirObsLiberacaoClienteApenasViaEmpresa", Liberacao.RelatorioLiberacaoPedido.ExibirObsLiberacaoClienteApenasViaEmpresa.ToString()));
                        lstParam.Add(new ReportParameter("NaoMostrarObsLiberacaoClienteNaLiberacao", Liberacao.RelatorioLiberacaoPedido.NaoMostrarObsLiberacaoClienteNaLiberacao.ToString()));
                        lstParam.Add(new ReportParameter("ExibirLogoTel", "false"));
                    }

                    // Mostra a observação do cliente
                    var obsCliente = Liberacao.RelatorioLiberacaoPedido.ExibirObservacaoCliente ?
                        ClienteDAO.Instance.ObtemObs(LiberacaoPedido_idCliente) : String.Empty;

                    lstParam.Add(new ReportParameter("ObsCliente", !String.IsNullOrEmpty(obsCliente) ? "Obs.: " + obsCliente : "."));

                    // Mostra as observações dos pedidos liberados
                    var obsPedidos = String.Empty;
                    var obsLiberacao = String.Empty;
                    var obsLiberacaoCliente = string.Empty;

                    //Verifica se todos os pedidos são do tipo entrega para mostrar
                    //somente a via da exp. ou almox.
                    var pedidosEntrega = true;
                    var pedidosBalcao = true;

                    foreach (var p in lstPedidosLib)
                    {
                        if (!String.IsNullOrEmpty(p.Obs) && Liberacao.RelatorioLiberacaoPedido.ExibirObservacaoPedidos)
                            obsPedidos += "Ped. " + p.IdPedido + " - " + p.Obs.TrimEnd('\r').TrimEnd('\n') + ";";

                        if (!String.IsNullOrEmpty(p.ObsLiberacao))
                            obsLiberacao += "Ped. " + p.IdPedido + " (Obs. Lib.) - " + p.ObsLiberacao.TrimEnd('\r').TrimEnd('\n') + ";\n";

                        if (!p.TipoEntrega.HasValue ||
                            (p.TipoEntrega.Value != (int)Data.Model.Pedido.TipoEntregaPedido.Entrega &&
                            p.TipoEntrega.Value != (int)Data.Model.Pedido.TipoEntregaPedido.Temperado))
                            pedidosEntrega = false;

                        if (!p.TipoEntrega.HasValue || p.TipoEntrega.Value != (int)Data.Model.Pedido.TipoEntregaPedido.Balcao)
                            pedidosBalcao = false;
                    }

                    if (!Liberacao.RelatorioLiberacaoPedido.NaoMostrarObsLiberacaoClienteNaLiberacao)
                        obsLiberacaoCliente +=
                            string.IsNullOrEmpty(cliente.ObsLiberacao) ?
                                string.Empty :
                                string.Format("Obs. Lib. Cliente - {0}", cliente.ObsLiberacao);

                    //Verifica se o relatorio é para o anexo do e-mail e se vai mostrar apenas a via do cliente.
                    var apenasViaCliente = (Request["EnvioEmail"] != null ? Request["EnvioEmail"].ToLower() == "true" : false) &&
                        Glass.Configuracoes.Liberacao.RelatorioLiberacaoPedido.ExibirApenasViaClienteNoEnvioEmail;
                    lstParam.Add(new ReportParameter("ExibirApenasViaClienteNoEnvioEmail", apenasViaCliente.ToString()));

                    //Se não for envio de e-mail verifica se é pra mostrar apenas a via de expedição e ou almoxarife
                    //caso todos os pedidos sejam do tipo entrega e não for relatório completo.
                    var apenasViaExpAlm = !apenasViaCliente && Liberacao.RelatorioLiberacaoPedido.ExibirApenasViaExpAlmPedidosEntrega &&
                        pedidosEntrega && !(Request["RelatorioCompleto"] != null ? Request["RelatorioCompleto"].ToLower() == "true" : false);
                    lstParam.Add(new ReportParameter("ExibirSomenteViaExpAlmPedidosEntrega", apenasViaExpAlm.ToString()));

                    var apenasViaExpAlmPedidosBalcao = !apenasViaCliente && Liberacao.RelatorioLiberacaoPedido.ExibirApenasViaExpAlmPedidosBalcao &&
                        pedidosBalcao && !(Request["RelatorioCompleto"] != null ? Request["RelatorioCompleto"].ToLower() == "true" : false);
                    lstParam.Add(new ReportParameter("ExibirSomenteViaExpAlmPedidosBalcao", apenasViaExpAlmPedidosBalcao.ToString()));

                    lstParam.Add(new ReportParameter("ObsPedidos", !String.IsNullOrEmpty(obsPedidos) ? obsPedidos : "."));
                    lstParam.Add(new ReportParameter("ObsLiberacao", obsLiberacao.TrimEnd('\n')));

                    /* Chamado 47680. */
                    if (!report.ReportPath.Contains("rptLiberacaoPedidoMSVidros"))
                    {
                        /* Chamado 47815 */
                        if (!report.ReportPath.Contains("rptLiberacaoPedido4ViasTemperForte"))
                            lstParam.Add(new ReportParameter("ObsLiberacaoCliente", obsLiberacaoCliente));

                        lstParam.Add(new ReportParameter("ExibirObsLiberacaoResumo", Liberacao.RelatorioLiberacaoPedido.ExibirObsLiberacaoResumo.ToString()));
                    }

                    // Define se será usada duas colunas extras na tabela de parcela
                    lstParam.Add(new ReportParameter("ColunaExtraParcela", "false"));

                    // Define se a via do almoxerifado vai ser igual a do cliente caso o tipo do pedido seja mão de obra.
                    var viaAlmoxarifadoIgualCliente = "false";
                    if (Liberacao.UsarViaAlmoxarifadoIgualClienteSeMaoDeObra && lstPedidosLib.Any(f => f.DescricaoTipoPedido.Contains("Mão de obra")))
                        viaAlmoxarifadoIgualCliente = "true";

                    lstParam.Add(new ReportParameter("ViaAlmoxerifadoIgualCliente", viaAlmoxarifadoIgualCliente));
                }

                var pedidosLib = new List<Data.Model.Pedido>(lstPedidosLib);

                var tiposVendaUltimo = new List<int?> {
                    (int)Data.Model.Pedido.TipoVendaPedido.Garantia,
                    (int)Data.Model.Pedido.TipoVendaPedido.Reposição
                };

                pedidosLib.Sort(new Comparison<Data.Model.Pedido>(
                    delegate (Data.Model.Pedido x, Data.Model.Pedido y)
                    {
                        if (tiposVendaUltimo.Contains(x.TipoVenda) != tiposVendaUltimo.Contains(y.TipoVenda))
                            return tiposVendaUltimo.Contains(x.TipoVenda) ? 1 : -1;
                        else
                            return Comparer<uint>.Default.Compare(x.IdPedido, y.IdPedido);
                    }));

                var produtosLib = ProdutosLiberarPedidoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]));
                var pecasCanc = ProdutoPedidoProducaoDAO.Instance.PesquisarProdutosProducaoRelatorioLiberacao(Request["idLiberarPedido"].StrParaInt(), true);
                var numeroVidros = new string[pedidosLib.Count];
                var numeroMateriais = new string[pedidosLib.Count];
                var posicaoProdutos = 0;

                if (pedidosLib == null || pedidosLib.Count == 0)
                {
                    numeroVidros = new string[] { "0" };
                    numeroMateriais = new string[] { "0" };
                }

                foreach (var p in pedidosLib)
                {
                    var tempVidros = new List<ProdutosLiberarPedido>(produtosLib).FindAll(
                        new Predicate<ProdutosLiberarPedido>(delegate (ProdutosLiberarPedido prod)
                        {
                            return prod.IdPedido == p.IdPedido && prod.IsVidro;
                        })
                    );

                    var tempMateriais = new List<ProdutosLiberarPedido>(produtosLib).FindAll(
                        new Predicate<ProdutosLiberarPedido>(delegate (ProdutosLiberarPedido prod)
                        {
                            return prod.IdPedido == p.IdPedido && !prod.IsVidro;
                        })
                    );

                    numeroVidros[posicaoProdutos] = tempVidros.Count.ToString();
                    numeroMateriais[posicaoProdutos] = tempMateriais.Count.ToString();
                    posicaoProdutos++;
                }

                var lstProdLib = Glass.Data.RelDAL.ProdutosLiberarPedidoRptDAL.Instance.CopiaLista(produtosLib);

                // Se for mão de obra, apaga a quantidade de ambientes de todos os produtos contidos no mesmo, exceto de um deles,
                // para que ao somar a quantidade de ambientes (peças) no relatório a soma fique correta
                var lstIdAmbModificado = new List<uint>();
                foreach (var plp in lstProdLib)
                {
                    if (!plp.PedidoMaoDeObra || plp.IdAmbientePedido == null)
                        continue;

                    if (!lstIdAmbModificado.Contains(plp.IdAmbientePedido.Value))
                    {
                        lstIdAmbModificado.Add(plp.IdAmbientePedido.Value);
                        continue;
                    }

                    plp.QtdeAmbiente = 0;
                }

                lstParam.Add(new ReportParameter("RatearDescontoProdutos", PedidoConfig.RatearDescontoProdutos.ToString()));
                lstParam.Add(new ReportParameter("NumeroVidros", numeroVidros));
                lstParam.Add(new ReportParameter("NumeroMateriais", numeroMateriais));
                report.DataSources.Add(new ReportDataSource("LiberarPedido", liberarPedido));
                report.DataSources.Add(new ReportDataSource("ParcelaLiberacao", lstParcLib));
                report.DataSources.Add(new ReportDataSource("PedidoRpt", PedidoRptDAL.Instance.CopiaLista(pedidosLib.ToArray(), PedidoRpt.TipoConstrutor.RelatorioLiberacao, false, login)));
                report.DataSources.Add(new ReportDataSource("Cheques", ChequesDAO.Instance.GetByLiberacaoPedido(null, Conversoes.StrParaUint(Request["idLiberarPedido"]))));
                report.DataSources.Add(new ReportDataSource("ProdutosLiberarPedidoRpt", lstProdLib));
                report.DataSources.Add(new ReportDataSource("ResumoCorte", ResumoCorteDAO.Instance.GetProdutosByLiberacaoPedido(lstProdLib, numeroViasResumoLiberacao)));
                report.DataSources.Add(new ReportDataSource("ProdutoPedidoProducaoRpt", ProdutoPedidoProducaoRptDAL.Instance.CopiaLista(pecasCanc.ToArray())));
            }

            #endregion

            var textoRodape = Geral.TextoRodapeRelatorio(login.Nome, incluirDataTextoRodape);

            if (Request["EnvioEmail"] != null && Request["EnvioEmail"].ToLower() == "true")
                textoRodape = String.Empty;

            lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogo(PageRequest, idLojaLogotipo)));
            lstParam.Add(new ReportParameter("TextoRodape", textoRodape));
            lstParam.Add(new ReportParameter("CorRodape", "DimGray"));

            return null;
        }

        /// <summary>
        /// Calcula entrada, pagto. antecipado, total e fast delivery de cada pedido, para valores ficarem corretos se cliente usar liberação parcial
        /// </summary>
        /// <param name="lstProdLib"></param>
        /// <param name="lstPedidoRpt"></param>
        private static void CalcularTotaisPedidos(ProdutosLiberarPedidoRpt[] lstProdLib, ref PedidoRpt[] lstPedidoRpt)
        {
            foreach (var p in lstPedidoRpt)
            {
                // Calcula o total bruto do pedido
                var totalBrutoPedido = (p.Total / (1 + ((decimal)p.TaxaFastDelivery / 100))) + p.DescontoExibirLib;

                // Calcula o total liberado dos produtos
                var totalProdLiberado = lstProdLib.Where(f => f.IdPedido == p.IdPedido).Sum(f => f.TotalProdLiberado);

                // Chamado 49734
                if (totalProdLiberado == 0 || totalBrutoPedido == 0)
                    continue;

                // Calcula o desconto rateado com base no total liberado
                var descontoRateado = (totalProdLiberado / totalBrutoPedido) * p.DescontoExibirLib;

                // Calcula a entrada rateado com base no total liberado
                var entradaRateado = (totalProdLiberado / totalBrutoPedido) * p.ValorEntrada;

                // Calcula a entrada rateado com base no total liberado
                var pagtoAntecipRateado = ((totalProdLiberado - descontoRateado) / (p.Total > 0 ? p.Total : 1)) * p.ValorPagamentoAntecipado;

                // Calcula o total liberado do pedido
                var totalLiberado =
                    // Se a entrada for igual ao total do pedido, o total deverá ser exibido zerado
                    entradaRateado == p.Total ? 0 :

                    // Se a empresa rateia desconto nos produtos, o total do pedido deverá ser o total liberado
                    PedidoConfig.RatearDescontoProdutos ? totalProdLiberado :

                    // Cálculo padrão para atender tanto empresas que liberam parcialmente quanto as que não liberam
                    (totalProdLiberado - descontoRateado) * (1 + ((decimal)p.TaxaFastDelivery / 100)) - entradaRateado - pagtoAntecipRateado;

                // Salva os valores obtidos na model
                p.DescontoRateadoLib = descontoRateado;
                p.EntradaRateadaLib = entradaRateado;
                p.PagtoAntecipRateadoLib = pagtoAntecipRateado;
                p.TotalPedidoLib = totalLiberado;
            }
        }

        /// <summary>
        /// Verifica quando as vias devem aparecer ou não
        /// </summary>
        private static void VerificarVisibilidadeVias(System.Collections.Specialized.NameValueCollection Request, List<Data.Model.Pedido> lstPedidosLib, ProdutosLiberarPedido[] produtosLib,
            out bool exibirViaEmpresa, out bool exibirViaCliente, out bool exibirViaExpedicao, out bool exibirViaAlmoxarife, bool envioEmail, bool exibirApenasViaCliente)
        {
            // Verifica se todos os pedidos são do tipo entrega para mostrar somente a via da exp. ou almox.
            var pedidosEntrega = !lstPedidosLib.Any(f =>
                f.TipoEntrega != (int)Data.Model.Pedido.TipoEntregaPedido.Entrega &&
                f.TipoEntrega != (int)Data.Model.Pedido.TipoEntregaPedido.Temperado);

            // Verifica se todos os pedidos são do tipo balcão para mostrar somente a via da exp. ou almox.
            var pedidosBalcao = !lstPedidosLib.Any(f =>
                f.TipoEntrega != (int)Data.Model.Pedido.TipoEntregaPedido.Balcao);

            // Verifica se o relatorio é para o anexo do e-mail e se vai mostrar apenas a via do cliente.
            var apenasViaCliente = (Request["EnvioEmail"] == "true" && (Liberacao.RelatorioLiberacaoPedido.ExibirApenasViaClienteNoEnvioEmail || Liberacao.TelaLiberacao.ExibirRelatorioCliente)) || exibirApenasViaCliente;

            // Verifica se é pra mostrar apenas a via de expedição e/ou almoxarife
            // caso todos os pedidos sejam do tipo entrega/balcão e não for relatório completo.
            var relatorioCompleto = Request["RelatorioCompleto"] == "true";
            var apenasViaExpAlm = Liberacao.RelatorioLiberacaoPedido.ExibirApenasViaExpAlmPedidosEntrega && pedidosEntrega && !relatorioCompleto;
            var apenasViaExpAlmPedidosBalcao = Liberacao.RelatorioLiberacaoPedido.ExibirApenasViaExpAlmPedidosBalcao && pedidosBalcao && !relatorioCompleto;

            // Define se a via da empresa será exibida
            exibirViaEmpresa = Liberacao.RelatorioLiberacaoPedido.ExibirViaEmpresaRelatorio4Vias &&
                !apenasViaCliente &&
                !apenasViaExpAlm &&
                !apenasViaExpAlmPedidosBalcao &&
                (Liberacao.DadosLiberacao.ExibirViaEmpresaPedidoReposicao || !lstPedidosLib.Any(f => f.TipoVenda.GetValueOrDefault() == (int)Data.Model.Pedido.TipoVendaPedido.Reposição));

            // Define se a via do cliente será exibida
            exibirViaCliente = apenasViaCliente ||
                (!apenasViaExpAlm && !apenasViaExpAlmPedidosBalcao);

            // Define se a via da expedição será exibida 
            // (se não exibir apenas a via do cliente, se a config AgruparResumoLiberacaoProduto não estiver marcada e se houver ao menos um produto que seja vidro)
            exibirViaExpedicao = !apenasViaCliente &&
                !Liberacao.DadosLiberacao.AgruparResumoLiberacaoProduto &&
                new List<ProdutosLiberarPedido>(produtosLib).Any(f => f.IsVidro);

            // Define se a via do almoxarife será exibida
            // (se não exibir apenas a via do cliente, se a config AgruparResumoLiberacaoProduto não estiver marcada e se houver ao menos um produto que não seja vidro)
            exibirViaAlmoxarife = !apenasViaCliente &&
                !Liberacao.DadosLiberacao.AgruparResumoLiberacaoProduto &&
                new List<ProdutosLiberarPedido>(produtosLib).Any(f => !f.IsVidro);

            //caso todos os pedidos da liberação sejam do tipo entrega e a configuraçãoo esteja ativa, não exibe a via do cliente
            if (pedidosEntrega && Liberacao.RelatorioLiberacaoPedido.NaoExibirViaClienteSeTodosPedidosForemTipoEntrega && !envioEmail)
                exibirViaCliente = false;
        }

        /// <summary>
        /// Monta o nome do cliente com informações adicionais na impressão da liberação
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        protected string MontarInfoAdicionalRelLiberacao(uint idCliente)
        {
            // Pega o texto base que será usado como informação adicional
            var info = Liberacao.RelatorioLiberacaoPedido.InformacaoAdicionalLiberacao;

            if (string.IsNullOrEmpty(info))
                return string.Empty;

            // Recupera dados do cliente
            var cliente = ClienteDAO.Instance.GetElement(idCliente);

            // Recupera campos que poderão ser usados na montagem das informações adicionais
            var codRota = cliente.CodigoRota;
            var telefoneGeral = cliente.Telefone;
            var telCont = cliente.TelCont;
            var telCel = cliente.TelCel;
            var cidade = CidadeDAO.Instance.GetNome((uint?)cliente.IdCidade);
            var endereco =
                !string.IsNullOrEmpty(cliente.EnderecoEntrega) &&
                !string.IsNullOrEmpty(cliente.NumeroEntrega) &&
                !string.IsNullOrEmpty(cliente.BairroEntrega) &&
                !string.IsNullOrEmpty(cliente.CidadeEntrega) ? cliente.EnderecoCompletoEntrega : cliente.EnderecoCompleto;

            // Preenche as informações necessárias com dados do banco
            info = info
                .Replace("[codrota]", codRota)
                .Replace("[telgeral]", telefoneGeral)
                .Replace("[telcont]", telCont)
                .Replace("[telcel]", telCel)
                .Replace("[cidade]", cidade)
                .Replace("[endereco]", endereco);

            // Limpa caracteres desnecessários
            info = info
                .Replace(" (Rota: )", "")
                .Replace(") (", " ")
                .Replace(" / )", ")");

            return info;
        }

        protected override void report_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            base.report_SubreportProcessing(sender, e);
        }
    }
}
