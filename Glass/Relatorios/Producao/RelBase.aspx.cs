using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class RelBase : Glass.Relatorios.UI.Web.ReportPage
    {
        protected override object[] Parametros
        {
            get { return new object[] { }; }
        }

        protected override Glass.Relatorios.UI.Web.ReportPage.JavaScriptData DadosJavaScript
        {
            get
            {
                return new JavaScriptData(false, "false");
            }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            ProcessaReport(pchTabela);
        }
    
        protected override Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam,
            HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login, string diretorioLogotipos)
        {
            // Verifica qual relatório será chamado
            switch (Request["rel"])
            {
                case "GraficoProducaoPerdaDiaria":
    
                    report.ReportPath = (Request["tipoGrafico"] == "1") ?
                        "Relatorios/Producao/rptGraficoProdPerdaDiaria.rdlc" : "Relatorios/Producao/rptGraficoProdPerdaProduto.rdlc";
    
                    RecebimentoImagem imgGrafico;
    
                    if (Request["tipoGrafico"] == "1")
                    {
                        imgGrafico = new RecebimentoImagem();
                        imgGrafico.Buffer = Glass.Conversoes.DecodificaPara64(Request["grfProdAcumulada"]);
                        report.DataSources.Add(new ReportDataSource("grafProdAcumulada", new RecebimentoImagem[1] { imgGrafico }));
    
                        imgGrafico = new RecebimentoImagem();
                        imgGrafico.Buffer = Glass.Conversoes.DecodificaPara64(Request["grfIndicePerda"]);
                        report.DataSources.Add(new ReportDataSource("grafIndicePerda", new RecebimentoImagem[1] { imgGrafico }));
    
                        imgGrafico = new RecebimentoImagem();
                        imgGrafico.Buffer = Glass.Conversoes.DecodificaPara64(Request["grfProdDiaria"]);
                        report.DataSources.Add(new ReportDataSource("grafProdDiaria", new RecebimentoImagem[1] { imgGrafico }));
    
                        imgGrafico = new RecebimentoImagem();
                        imgGrafico.Buffer = Glass.Conversoes.DecodificaPara64(Request["grfPerdaMensal"]);
                        report.DataSources.Add(new ReportDataSource("grafPerdaMensal", new RecebimentoImagem[1] { imgGrafico }));
    
                        imgGrafico = new RecebimentoImagem();
                        imgGrafico.Buffer = Glass.Conversoes.DecodificaPara64(Request["grfPerdaSetores"]);
                        report.DataSources.Add(new ReportDataSource("grafPerdaSetores", new RecebimentoImagem[1] { imgGrafico }));
                    }
                    else if (Request["tipoGrafico"] == "2")
                    {
                        imgGrafico = new RecebimentoImagem();
                        imgGrafico.Buffer = Glass.Conversoes.DecodificaPara64(Request["grf10mm"]);
                        report.DataSources.Add(new ReportDataSource("graf10mm", new RecebimentoImagem[1] { imgGrafico }));
    
                        imgGrafico = new RecebimentoImagem();
                        imgGrafico.Buffer = Glass.Conversoes.DecodificaPara64(Request["grf8mm"]);
                        report.DataSources.Add(new ReportDataSource("graf8mm", new RecebimentoImagem[1] { imgGrafico }));
    
                        imgGrafico = new RecebimentoImagem();
                        imgGrafico.Buffer = Glass.Conversoes.DecodificaPara64(Request["grf6mm"]);
                        report.DataSources.Add(new ReportDataSource("graf6mm", new RecebimentoImagem[1] { imgGrafico }));
    
                        report.DataSources.Add(new ReportDataSource("GraficoProdPerdaDiaria",
                            GraficoProdPerdaDiariaDAO.Instance.GetPerdaProduto(Convert.ToInt32(Request["setor"]),
                                Request["incluirTrocaDevolucao"] != null && Request["incluirTrocaDevolucao"].ToLower() == "true", Request["mes"], Request["ano"])));
                    }
    
                    break;
                case "PlanilhaMetragem":
                    var agruparPecas = Request["agruparPecas"] == "true";
                    report.ReportPath = agruparPecas ? "Relatorios/Producao/rptPlanilhaMetragemAgrupada.rdlc" : "Relatorios/Producao/rptPlanilhaMetragem.rdlc";
    
                    var PlanilhaMetragem_idPedido = !String.IsNullOrEmpty(Request["idPedido"]) ? Glass.Conversoes.StrParaUint(Request["idPedido"]) : 0;
                    var PlanilhaMetragem_idFunc = !String.IsNullOrEmpty(Request["idFunc"]) ? Glass.Conversoes.StrParaUint(Request["idFunc"]) : 0;
                    var PlanilhaMetragem_idCliente = !String.IsNullOrEmpty(Request["idCliente"]) ? Glass.Conversoes.StrParaUint(Request["idCliente"]) : 0;
                    var PlanilhaMetragem_idImpressao = !String.IsNullOrEmpty(Request["idImpressao"]) ? Glass.Conversoes.StrParaUint(Request["idImpressao"]) : 0;
    
                    report.DataSources.Add(new ReportDataSource("Metragem", MetragemDAO.Instance.GetForRpt(PlanilhaMetragem_idPedido,
                        PlanilhaMetragem_idImpressao, PlanilhaMetragem_idFunc, Request["codCliente"], PlanilhaMetragem_idCliente, Request["nomeCliente"],
                        Request["dataIni"], Request["dataFim"], Request["dataIniEnt"], Request["dataFimEnt"],
                        Glass.Conversoes.StrParaInt(Request["situacao"]), Glass.Conversoes.StrParaUint(Request["idSetor"]), Request["setoresPosteriores"] == "true",
                        Request["idsRotas"], Glass.Conversoes.StrParaUint(Request["idTurno"]))));
                    break;
                case "ProducaoForno":
                    report.ReportPath = "Relatorios/Producao/rptProducaoForno.rdlc";
                    report.DataSources.Add(new ReportDataSource("ProducaoForno", ProducaoFornoDAO.Instance.GetForRpt(Request["dataIni"], Request["dataFim"])));
                    break;
                case "ProducaoSituacao":
                    report.ReportPath = "Relatorios/Producao/rptProducaoSituacao.rdlc";
                    uint ProducaoSituacao_idFunc = Glass.Conversoes.StrParaUint(Request["idFunc"]);
                    uint ProducaoSituacao_idPedido = !String.IsNullOrEmpty(Request["idPedido"]) ? Glass.Conversoes.StrParaUint(Request["idPedido"]) : 0;
                    report.DataSources.Add(new ReportDataSource("ProducaoSituacao", ProducaoSituacaoDAO.Instance.GetForRpt(ProducaoSituacao_idFunc, ProducaoSituacao_idPedido, Request["dataIni"], Request["dataFim"])));
                    lstParam.Add(new ReportParameter("LiberarPedido", PedidoConfig.LiberarPedido.ToString()));
                    lstParam.Add(new ReportParameter("Agrupar", !String.IsNullOrEmpty(Request["agrupar"]) ? (Request["agrupar"].ToLower() == "true").ToString() : "false"));
                    break;
                case "ProducaoSituacaoData":
                    report.ReportPath = "Relatorios/Producao/rptProducaoSituacaoData.rdlc";
                    uint ProducaoSituacaoData_idPedido = !String.IsNullOrEmpty(Request["idPedido"]) ? Glass.Conversoes.StrParaUint(Request["idPedido"]) : 0;
                    uint ProducaoSituacaoData_idCliente = !String.IsNullOrEmpty(Request["idCliente"]) ? Glass.Conversoes.StrParaUint(Request["idCliente"]) : 0;
                    report.DataSources.Add(new ReportDataSource("ProducaoSituacaoData", ProducaoSituacaoDataDAO.Instance.GetForRpt(Request["dataIni"], Request["dataFim"], ProducaoSituacaoData_idPedido, ProducaoSituacaoData_idCliente, Request["nomeCliente"])));
                    lstParam.Add(new ReportParameter("LiberarPedido", PedidoConfig.LiberarPedido.ToString()));
                    lstParam.Add(new ReportParameter("ExportarExcel", (Request["exportarExcel"] == "true").ToString().ToLower()));
                    break;
                case "ProducaoFornoResumo":
                    report.ReportPath = "Relatorios/Producao/rptProducaoFornoResumo.rdlc";
                    report.DataSources.Add(new ReportDataSource("ProducaoFornoResumo", ProducaoFornoResumoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaInt(Request["setor"]),
                        DateTime.Parse(Request["dtIni"]), DateTime.Parse(Request["dtFim"]), Request["idTurno"].StrParaInt())));
                    break;
                case "RelacaoBoxProducao":
                    report.ReportPath = "Relatorios/Producao/rptRelacaoBoxProducao.rdlc";
                    report.DataSources.Add(new ReportDataSource("RelacaoBoxProducao", RelacaoBoxProducaoDAO.Instance.GetForRpt(Request["data"])));
                    break;
                case "Perdas":
                    {
                        report.ReportPath = "Relatorios/Producao/rptPerdas.rdlc";

                        var idFuncPerda = !string.IsNullOrEmpty(Request["idFuncPerda"]) ? Request["idFuncPerda"].StrParaUint() : 0;
                        var idPedido = !string.IsNullOrEmpty(Request["idPedido"]) ? Request["idPedido"].StrParaUint() : 0;
                        var idCliente = !string.IsNullOrEmpty(Request["idCliente"]) ? Request["idCliente"].StrParaUint() : 0;

                        lstParam.Add(new ReportParameter("Agrupar", Request["agrupar"].StrParaInt().ToString()));

                        report.DataSources.Add(new ReportDataSource("ProdutoPedidoProducao", ProdutoPedidoProducaoDAO.Instance.GetForRptPerda(idFuncPerda, idPedido, idCliente, Request["nomeCliente"],
                            Request["dataIni"], Request["dataFim"], Request["idsSetor"])));
                        break;
                    }
                case "PerdasRepos":
                    {
                        report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/Producao/rptPerdasRepos{0}.rdlc");

                        var idFuncPerda = !string.IsNullOrEmpty(Request["idFuncPerda"]) ? Request["idFuncPerda"].StrParaUint() : 0;
                        var idLoja = !string.IsNullOrEmpty(Request["idLoja"]) ? Request["idLoja"].StrParaUint() : 0;
                        var idPedido = !string.IsNullOrEmpty(Request["idPedido"]) ? Request["idPedido"].StrParaUint() : 0;
                        var idCliente = !string.IsNullOrEmpty(Request["idCliente"]) ? Request["idCliente"].StrParaUint() : 0;

                        lstParam.Add(new ReportParameter("ExibirValorCustoVenda", PCPConfig.ExibirCustoVendaRelatoriosProducao.ToString()));

                        report.DataSources.Add(new ReportDataSource("ProdutoPedidoProducao",
                            ProdutoPedidoProducaoDAO.Instance.GetForRptPerdaReposPeca(idFuncPerda, idPedido, idLoja, idCliente,
                            Request["nomeCliente"], Request["codInterno"], Request["descrProd"], Request["dataIni"], Request["dataFim"],
                            Request["idSetor"], Glass.Conversoes.StrParaUint(Request["turno"]), Request["idTipoPerda"],
                            Glass.Conversoes.StrParaInt(Request["idCorVidro"]), Glass.Conversoes.StrParaFloat(Request["espessura"]), Glass.Conversoes.StrParaUint(Request["numeroNFe"]))));
                        break;
                    }
                case "ProducaoContagem":
                    report.ReportPath = "Relatorios/Producao/rptProducaoContagem.rdlc";
                    ProducaoContagem[] lstProdCont = ProducaoContagemDAO.Instance.GetForRpt(Request["idCarregamento"].StrParaInt(), Request["idPedido"].StrParaUint(), Request["idImpressao"].StrParaUint(),
                        Request["codCliente"], Request["codRota"], Glass.Conversoes.StrParaUint(Request["idCliente"]), Request["nomeCliente"], Request["numEtiqueta"],
                        Request["dataIni"], Request["dataFim"], Request["dataIniEnt"], Request["dataFimEnt"], Request["dataIniFabr"], Request["dataFimFabr"], Request["dataIniConfPed"], Request["dataFimConfPed"], Glass.Conversoes.StrParaInt(Request["idSetor"]), Request["situacao"],
                        Glass.Conversoes.StrParaInt(Request["situacaoPedido"]), Glass.Conversoes.StrParaInt(Request["tiposSituacoes"]), Request["idsSubgrupos"], Glass.Conversoes.StrParaUint(Request["tipoEntrega"]),
                        Request["pecasProdCanc"], Glass.Conversoes.StrParaUint(Request["idFunc"]), Request["tipoPedido"], 0, Glass.Conversoes.StrParaInt(Request["altura"]),
                        Glass.Conversoes.StrParaInt(Request["largura"]), Glass.Conversoes.StrParaInt(Request["espessura"]), Request["idsProc"], Request["idsApl"],
                        Request["aguardExpedicao"] == "true", Request["aguardEntrEstoque"] == "true", Glass.Conversoes.StrParaUint(Request["fastDelivery"]));
                    report.DataSources.Add(new ReportDataSource("ProducaoContagem", lstProdCont));
                    break;
                case "ProducaoData":
                    report.ReportPath = "Relatorios/Producao/rptProducaoData.rdlc";
                    report.DataSources.Add(new ReportDataSource("ProducaoData", ProducaoDataDAO.Instance.GetForRpt(Glass.Conversoes.StrParaInt(Request["tipoData"]), Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaUint(Request["idProcesso"]),
                        Glass.Conversoes.StrParaUint(Request["idAplicacao"]), Request["tipo"], Request["situacao"], Request["naoCortados"] == "true", Request["codInternoMP"], Request["descrMP"])));
                    break;
                case "PecasPendentes":
                    report.ReportPath = "Relatorios/Producao/rptPecasPendentes.rdlc";
    
                    var dtIni = Request["dataIni"].ConverteData().GetValueOrDefault(DateTime.Now);
                    var dtFim = Request["dataFim"].ConverteData().GetValueOrDefault(DateTime.Now);
                    bool usarProximoSetor = false;
                    bool.TryParse(Request["usarProximoSetor"], out usarProximoSetor);
                    
                    // Recupera a relação da peças pendentes
                    var lstPcaPend = PecasPendentesDAO.Instance.GetListForRpt
                        (Request["tipoPeriodo"], usarProximoSetor, dtIni, dtFim);
    
                    string descricaoPeriodo = null;
    
                    switch (Request["tipoPeriodo"])
                    {
                        case "PeriodoEntrega":
                            descricaoPeriodo = "Período (Entrega)";
                            break;
                        case "PeriodoFabrica":
                            descricaoPeriodo = "Período (Fábrica)";
                            break;
                    }
                    lstParam.Add(new ReportParameter("UsarProximoSetor", usarProximoSetor ? "Próximo Setor a ser efetuado a peça" : ""));
                    lstParam.Add(new ReportParameter("Periodo", descricaoPeriodo + " de " + Request["dataIni"] + " à " + Request["dataFim"]));
                    report.DataSources.Add(new ReportDataSource("PecasPendentes", lstPcaPend));
                    break;
                case "ProducaoDiaria":
                    report.ReportPath = "Relatorios/Producao/rptProducaoDiaria.rdlc";
                    lstParam.Add(new ReportParameter("Data", Request["data"]));
                    var dados = WebGlass.Business.ProducaoDiariaRealizada.Fluxo.BuscarEValidar.Instance.ObtemDadosProducao(DateTime.Parse(Request["data"]));
                    if (dados.Count > 0)
                        dados[0].AlteraImagemGrafico(Request["imagem"]);
                    report.DataSources.Add(new ReportDataSource("ProducaoDiariaRealizada", dados));
                    break;
                case "CapacidadeProducaoPedido":
                    report.ReportPath = "Relatorios/Producao/rptPedidosCapacidadeProducao.rdlc";
                    report.DataSources.Add(new ReportDataSource("CapacidadeProducaoPedido", CapacidadeProducaoPedidoDAO.Instance.ObtemRelatorioPedidosCapacidadeProducao(
                        Request["data"].StrParaDate().GetValueOrDefault(), Request["horaInicial"], Request["horaFinal"], Request["idSetor"].StrParaUint())));
                    break;
                case "RoteiroProducao":
                    report.ReportPath = "Relatorios/Producao/rptRoteiroProducao.rdlc";
                    report.DataSources.Add(new ReportDataSource("RoteiroProducao", RoteiroProducaoDAO.Instance.ObtemParaRelatorio(0,
                        Glass.Conversoes.StrParaUint(Request["grupoProd"]), Glass.Conversoes.StrParaUint(Request["subgrupoProd"]), Glass.Conversoes.StrParaUint(Request["processo"]))));
                    break;
                default:
                    Response.Write("Nenhum relatório especificado.");
                    return null;
            }
    
            // Atribui parâmetros ao relatório
            lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogo(PageRequest)));
            lstParam.Add(new ReportParameter("TextoRodape", Geral.TextoRodapeRelatorio(login.Nome)));
            lstParam.Add(new ReportParameter("CorRodape", "DimGray"));

            return null;
        }
    }
}
