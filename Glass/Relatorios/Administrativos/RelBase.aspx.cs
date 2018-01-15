using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.UI.Web.Relatorios.Administrativos
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
            Ajax.Utility.RegisterTypeForAjax(typeof(RelBase));
            ProcessaReport(pchTabela);
        }

        protected override Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam,
            HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login)
        {
            // Verifica qual relatório será chamado
            switch (Request["rel"])
            {
                case "LucroAproximado":
                    report.ReportPath = "Relatorios/Administrativos/rptLucroAproximado.rdlc";
                    var lucro = LucroAproximadoDAO.Instance.GetLucroAproximado(Request["dataIni"], Request["dataFim"]);
                    report.DataSources.Add(new ReportDataSource("LucroAproximado", lucro));
                    break;
                case "RecebimentosTipo":
                    {
                        report.ReportPath = "Relatorios/Administrativos/rptRecebimentosTipo.rdlc";
                        var lstReceb = RecebimentoDAO.Instance.GetRecebimentosTipo(Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaUint(Request["idLoja"]), 0);
                        report.DataSources.Add(new ReportDataSource("Recebimento", lstReceb));
                        break;
                    }
                case "RecebimentosTipoGrafico":
                    report.ReportPath = "Relatorios/Administrativos/rptRecebimentosTipoGrafico.rdlc";
                    var rt = RecebimentoDAO.Instance.GetRecebimentosTipo(Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaUint(Request["idLoja"]), Glass.Conversoes.StrParaUint(Request["idFunc"]));
                    report.DataSources.Add(new ReportDataSource("Recebimento", rt));

                    var recebimentoImagem = new RecebimentoImagem();
                    recebimentoImagem.Buffer = Glass.Conversoes.DecodificaPara64(Request["tempFile"]);
                    report.DataSources.Add(new ReportDataSource("RecebimentoImagem", new RecebimentoImagem[1] { recebimentoImagem }));
                    break;
                case "GraficoVendas":
                    report.ReportPath = "Relatorios/Administrativos/rptGraficoVendas.rdlc";
                    var chartVendasDict = ChartVendasDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idLoja"]), 
                        Conversoes.StrParaInt(Request["tipoFunc"]), Glass.Conversoes.StrParaUint(Request["idVendedor"]),
                        Conversoes.StrParaUint(Request["idCliente"]), Request["nomeCliente"], Request["dataIni"], Request["dataFim"], Request["tipoPedido"],
                        Request["agrupar"].StrParaInt(), login);

                    var chartVendasLista = new List<ChartVendas>();
                    foreach (KeyValuePair<uint, List<ChartVendas>> entry in chartVendasDict)
                        foreach (ChartVendas c in entry.Value)
                            chartVendasLista.Add(c);

                    var chartVendas = chartVendasLista.ToArray();
                    report.DataSources.Add(new ReportDataSource("ChartVendas", chartVendas));

                    var chartVendasImagem = new ChartVendasImagem();
                    chartVendasImagem.Buffer = Glass.Conversoes.DecodificaPara64(Request["tempFile"]);
                    report.DataSources.Add(new ReportDataSource("ChartVendasImagem", new ChartVendasImagem[1] { chartVendasImagem }));

                    break;
                case "GraficoOrcaVendas":
                    {
                        report.ReportPath = "Relatorios/Administrativos/rptGraficoOrcaVendas.rdlc";
                        var situacao = !string.IsNullOrEmpty(Request["situacao"]) ? Request["situacao"].Split(',').Select(f => f.StrParaInt()) : null;
                        var chartOrcaVendas = ChartOrcaVendasDAO.Instance.GetOrcaVendas(Glass.Conversoes.StrParaUint(Request["idLoja"]),
                            Glass.Conversoes.StrParaUint(Request["idVendedor"]), situacao, Glass.Conversoes.StrParaInt(Request["tipoFunc"]), Request["dataIni"], Request["dataFim"]);
                        report.DataSources.Add(new ReportDataSource("ChartOrcaVendas", chartOrcaVendas));
                        var chartOrcaVendasImagem = new ChartOrcaVendasImagem();
                        chartOrcaVendasImagem.Buffer = Glass.Conversoes.DecodificaPara64(Request["tempFile"]);
                        report.DataSources.Add(new ReportDataSource("ChartOrcaVendasImagem", new ChartOrcaVendasImagem[1] { chartOrcaVendasImagem }));
                        break;
                    }
                case "GraficoOrcamentos":
                {
                    var idLoja = Request["idLoja"].StrParaUint();
                    var idVendedor = string.IsNullOrEmpty(Request["idVendedor"])
                        ? 0
                        : Request["idVendedor"].StrParaUint();
                    var agrupar = string.IsNullOrEmpty(Request["agrupar"])
                        ? 0
                        : Request["agrupar"].StrParaInt();
                    var situacao = string.IsNullOrEmpty(Request["situacao"])
                        ? 0
                        : Request["situacao"].StrParaInt();

                    report.ReportPath = "Relatorios/Administrativos/rptGraficoOrcamento.rdlc";
                    var v = GraficoOrcamentosDAO.Instance.GetOrcamentos(idLoja, idVendedor, new int[] { situacao }, Request["dtIni"],
                        Request["dtFim"], agrupar, false);
                    report.DataSources.Add(new ReportDataSource("GraficoOrcamentos", v));

                    var graficoOrcamentosImagem = new GraficoOrcamentosImagem
                    {
                        Buffer = Glass.Conversoes.DecodificaPara64(Request["tempFile"])
                    };
                    report.DataSources.Add(new ReportDataSource("GraficoOrcamentosImagem",
                        new GraficoOrcamentosImagem[1] {graficoOrcamentosImagem}));
                    break;
                }
                case "GraficoProdutos":
                {
                    var idLoja = Request["idLoja"].StrParaUint();
                    var idVendedor = string.IsNullOrEmpty(Request["idVend"]) ? 0 : Request["idVend"].StrParaUint();
                    var idCliente = string.IsNullOrEmpty(Request["idCliente"]) ? 0 : Request["idCliente"].StrParaInt();
                    var nomeCliente = Request["nomeCliente"];
                    var grupo = !string.IsNullOrEmpty(Request["grupo"]) ? Request["grupo"].StrParaUint() : 0;
                    var subGrupo = !string.IsNullOrEmpty(Request["subGrupo"])
                        ? Request["subGrupo"].StrParaUint()
                        : 0;
                    var quantidade = !string.IsNullOrEmpty(Request["quantidade"])
                        ? Request["quantidade"].StrParaInt()
                        : 0;
                    var tipo = !string.IsNullOrEmpty(Request["tipo"]) ? Request["tipo"].StrParaInt() : 0;
                    var dataIni = Request["dtIni"];
                    var dataFim = Request["dtFim"];
                    var codProduto = Request["codProduto"];
                    var descricaoProduto = Request["descProduto"];
                    var apenasMS = Convert.ToBoolean(Request["apenasMS"]);

                    report.ReportPath = "Relatorios/Administrativos/rptGraficoProduto.rdlc";

                    //Busca os dados que servirão para preencher as séries do gráfico
                    var p = GraficoProdutosDAO.Instance.GetMaisVendidos(idLoja, idVendedor, idCliente, nomeCliente,
                        grupo, subGrupo, quantidade,
                        tipo, dataIni, dataFim, codProduto, descricaoProduto, apenasMS);
                    report.DataSources.Add(new ReportDataSource("GraficoProdutos", p));

                    var graficoProdutosImagem = new GraficoProdutosImagem();
                    graficoProdutosImagem.Buffer = Glass.Conversoes.DecodificaPara64(Request["tempFile"]);
                    report.DataSources.Add(new ReportDataSource("GraficoProdutosImagem",
                        new GraficoProdutosImagem[1] {graficoProdutosImagem}));


                    break;
                }
                case "PedidoConferido":
                    report.ReportPath = "Relatorios/Administrativos/rptPedidosConferidos.rdlc";
                    var lstPedConf = PedidoConferidoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idPedido"]), Glass.Conversoes.StrParaUint(Request["idLoja"]),
                        Glass.Conversoes.StrParaUint(Request["idConferente"]), Glass.Conversoes.StrParaUint(Request["idFunc"]), Glass.Conversoes.StrParaInt(Request["situacao"]),
                        Request["dataIni"], Request["dataFim"]);
                    report.DataSources.Add(new ReportDataSource("PedidoConferido", lstPedConf));
                    break;
                case "ListaBenef":
                    report.ReportPath = "Relatorios/Administrativos/rptBeneficiamentos.rdlc";
                    uint Benef_IdFunc = Glass.Conversoes.StrParaUint(Request["idFunc"]);
                    var lstBenef = BenefConfigDAO.Instance.GetForRpt(Request["dataIni"], Request["dataFim"],
                        Glass.Conversoes.StrParaUint(Request["idLoja"]), Glass.Conversoes.StrParaUint(Request["idFunc"]));
                    if (lstBenef.Count > 0)
                    {
                        lstBenef[0].Criterio = "Loja: " + LojaDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(Request["idLoja"])) + "    ";
                        if (Benef_IdFunc > 0) lstBenef[0].Criterio += "Vendedor: " + FuncionarioDAO.Instance.GetNome(Benef_IdFunc) + "    ";
                        lstBenef[0].Criterio += "Data início: " + Request["dataIni"] + "    ";
                        lstBenef[0].Criterio += "Data fim: " + Request["dataFim"];
                    }
                    report.DataSources.Add(new ReportDataSource("BenefConfig", lstBenef));
                    break;
                case "FluxoCaixa":
                    report.ReportPath = "Relatorios/Administrativos/rptFluxoCaixa.rdlc";
                    var lstFluxoCaixa = FluxoCaixaDAO.Instance.GetForRpt(Request["dataIni"], Request["dataFim"], Request["prevCustoFixo"] == "1", Request["tipoConta"]);
                    report.DataSources.Add(new ReportDataSource("FluxoCaixa", lstFluxoCaixa));
                    break;
                case "FluxoCaixaSintetico":
                    report.ReportPath = "Relatorios/Administrativos/rptFluxoCaixaSintetico.rdlc";
                    var lstFluxoCaixaSintetico = FluxoCaixaSinteticoDAO.Instance.GetForRpt(Request["dataIni"], Request["dataFim"], Request["prevCustoFixo"] == "1", Request["tipoConta"]);
                    report.DataSources.Add(new ReportDataSource("FluxoCaixaSintetico", lstFluxoCaixaSintetico));
                    break;
                case "PrevisaoFinanceira":
                    report.ReportPath = "Relatorios/Administrativos/rptPrevisaoFinanceira.rdlc";
                    report.DataSources.Add(new ReportDataSource("PrevisaoFinanceira_Receber", new Data.RelModel.PrevisaoFinanceira[] { 
                        PrevisaoFinanceiraDAO.Instance.GetReceber(Glass.Conversoes.StrParaUint(Request["idLoja"]), Request["data"], Request["prevPedidos"] == "true") }));
                    report.DataSources.Add(new ReportDataSource("PrevisaoFinanceira_Pagar", new Data.RelModel.PrevisaoFinanceira[] { 
                        PrevisaoFinanceiraDAO.Instance.GetPagar(Glass.Conversoes.StrParaUint(Request["idLoja"]), Request["data"], Request["previsaoCustoFixo"] == "true") }));
                    lstParam.Add(new ReportParameter("Tipo", Request["tipo"]));
                    lstParam.Add(new ReportParameter("ExibirPrevisaoCustoFixo", Request["previsaoCustoFixo"]));
                    lstParam.Add(new ReportParameter("prevPedidos", (Request["prevPedidos"] == "true").ToString()));
                    break;
                case "DescParc":
                    report.ReportPath = "Relatorios/Administrativos/rptDescParc.rdlc";

                    var contasRec = Glass.Data.DAL.ContasReceberDAO.Instance.GetListContaComDescontoRpt(Glass.Conversoes.StrParaUint(Request["idPedido"]),
                        Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]), Glass.Conversoes.StrParaUint(Request["idLoja"]), Glass.Conversoes.StrParaUint(Request["idCliente"]), Request["nomeCli"], Glass.Conversoes.StrParaUint(Request["idFunc"]),
                        Glass.Conversoes.StrParaDecimal(Request["valorIniAcres"]), Glass.Conversoes.StrParaDecimal(Request["valorFimAcres"]), Glass.Conversoes.StrParaDecimal(Request["valorIniDesc"]),
                        Glass.Conversoes.StrParaDecimal(Request["valorFimDesc"]), Request["dataIni"], Request["dataFim"], Request["dataDescIni"], Request["dataDescFim"], Glass.Conversoes.StrParaUint(Request["idOrigemDesconto"]));

                    report.DataSources.Add(new ReportDataSource("ContasReceber", contasRec));
                    lstParam.Add(new ReportParameter("LiberarPedido", PedidoConfig.LiberarPedido.ToString()));
                    break;
                case "DescParcPag":
                    report.ReportPath = "Relatorios/Administrativos/rptDescParcPag.rdlc";
                    report.DataSources.Add(new ReportDataSource("ContasPagar", Glass.Data.DAL.ContasPagarDAO.Instance.GetListContaComDescontoRpt(Glass.Conversoes.StrParaUint(Request["idCompra"]),
                        Glass.Conversoes.StrParaUint(Request["numeroNf"]), Request["dataIni"], Request["dataFim"], Request["dataDescIni"], Request["dataDescFim"])));
                    break;
                case "RelacaoVendas":
                    report.ReportPath = "Relatorios/Administrativos/rptRelacaoVendas.rdlc";
                    report.DataSources.Add(new ReportDataSource("RelacaoVendas", RelacaoVendasDAO.Instance.GetForRpt(Request["dataIni"], Request["dataFim"],
                        Glass.Conversoes.StrParaInt(Request["situacao"]), Glass.Conversoes.StrParaInt(Request["agrupar"]), Glass.Conversoes.StrParaInt(Request["ordenar"]), login)));
                    break;
                case "MovimentacoesFinanceiras":
                    bool MovimentacoesFinanceiras_detalhado = bool.Parse(Request["detalhado"]);
                    report.ReportPath = "Relatorios/Administrativos/rptMovimentacoesFinanceiras" + (MovimentacoesFinanceiras_detalhado ? "Det" : "") + ".rdlc";
                    report.DataSources.Add(new ReportDataSource("MovimentacoesFinanceiras", MovimentacoesFinanceirasDAO.Instance.GetForRpt(Request["dataIni"], Request["dataFim"], MovimentacoesFinanceiras_detalhado)));
                    break;
                case "IFD":
                    report.ReportPath = "Relatorios/Administrativos/rptIFD.rdlc";
                    lstParam.Add(new ReportParameter("Data", Request["data"]));
                    report.DataSources.Add(new ReportDataSource("MovimentacoesFinanceiras", MovimentacoesFinanceirasDAO.Instance.GetForRpt(Request["data"], Request["data"], false)));
                    report.DataSources.Add(new ReportDataSource("PrevisaoFinanceira_Receber", new Data.RelModel.PrevisaoFinanceira[] { PrevisaoFinanceiraDAO.Instance.GetReceber(0, Request["data"], false) }));
                    report.DataSources.Add(new ReportDataSource("PrevisaoFinanceira_Pagar", new Data.RelModel.PrevisaoFinanceira[] { PrevisaoFinanceiraDAO.Instance.GetPagar(0, Request["data"], false) }));
                    report.DataSources.Add(new ReportDataSource("FluxoCaixaIFD", FluxoCaixaIFDDAO.Instance.GetForRpt(Request["data"])));
                    report.DataSources.Add(new ReportDataSource("EstoqueIFD", EstoqueIFDDAO.Instance.GetForRpt(Request["data"])));
                    report.DataSources.Add(new ReportDataSource("MovimentacaoDiaIFD", MovimentacaoDiaIFDDAO.Instance.GetForRpt(Request["data"])));
                    report.DataSources.Add(new ReportDataSource("TitulosIFD", TitulosIFDDAO.Instance.GetForRpt(Request["data"])));
                    break;
                case "AlteracoesSistema":
                    report.ReportPath = "Relatorios/Administrativos/rptAlteracoesSistema.rdlc";
                    report.DataSources.Add(new ReportDataSource("AlteracoesSistema", AlteracoesSistemaDAO.Instance.GetForRpt(Request["tipo"], Request["dataIni"],
                        Request["dataFim"], Request["tabela"].StrParaInt(), Request["campo"], Request["idFunc"].StrParaUint())));
                    break;
                case "GraficoDRE":
                {
                    report.ReportPath = "Relatorios/Administrativos/rptGraficoDRE.rdlc";

                    var idCategoriaConta = Request["idCategoriaConta"].StrParaUint();
                    var idGrupoConta = Request["idGrupoConta"].StrParaUint();
                    var idPlanoConta = Request["idPlanoConta"].StrParaUint();
                    var idLoja = Request["idLoja"].StrParaInt();
                    var dataIni = Request["dataIni"];
                    var dataFim = Request["dataFim"];
                    var grupos = Request["grupos"];
                    var tipoMov = Request["tipoMov"].StrParaInt();
                    var tipoConta = Request["tipoConta"].StrParaInt();
                    var ajustado = !string.IsNullOrEmpty(Request["ajustado"]) && Request["ajustado"].ToLower() == "true";

                    var ids = new List<uint>();

                    var lojas = idLoja == 0
                        ? LojaDAO.Instance.GetAll()
                        : new [] {LojaDAO.Instance.GetElementByPrimaryKey(idLoja)};

                    foreach (var l in lojas)
                        ids.Add((uint) l.IdLoja);

                    var dados = ChartDREDAO.Instance.ObterDados(idCategoriaConta, idGrupoConta, idPlanoConta, dataIni,
                        dataFim, tipoMov, tipoConta, ajustado, ids);

                    var data = new List<ChartDRE>();
                    foreach (var entry in dados)
                    {
                        foreach (var c in entry.Value)
                        {
                            data.Add(c);
                        }
                    }

                    report.DataSources.Add(new ReportDataSource("ChartDRE", data.ToArray()));

                    var chartDREImagem = new ChartDREImagem
                    {
                        Buffer = Glass.Conversoes.DecodificaPara64(Request["tempFile"])
                    };
                    report.DataSources.Add(new ReportDataSource("ChartDREImagem", new ChartDREImagem[1] {chartDREImagem}));

                    break;
                }
                case "PrevFinanRecebPedido":
                    report.ReportPath = "Relatorios/Administrativos/rptPrevisaoFinanRecebPedidos.rdlc";
                    var pedidos = PrevFinanPedidosDAO.Instance.GetPrevFinanPedidosRpt(Glass.Conversoes.StrParaUint(Request["idLoja"]), Request["dataIni"], Request["dataFim"]);
                    report.DataSources.Add(new ReportDataSource("PrevFinanPedidos", pedidos));
                    break;
                case "ContasPagarReceber":
                    report.ReportPath = "Relatorios/Administrativos/rptContasPagarReceber.rdlc";
                    var contasPagRec = ContasPagarReceberDAO.Instance.GetContasPagarReceberRpt(Glass.Conversoes.StrParaUint(Request["idCli"]),
                        Request["nomeCli"], Glass.Conversoes.StrParaUint(Request["idFornec"]), Request["nomeFornec"], Request["dtIni"], Request["dtFim"],
                        Glass.Conversoes.StrParaFloat(Request["valorIni"]), Glass.Conversoes.StrParaFloat(Request["valorFim"]));
                    report.DataSources.Add(new ReportDataSource("ContasPagarReceber", contasPagRec));
                    break;

                case "RecebimentosDetalhados":
                    {
                        report.ReportPath = "Relatorios/Administrativos/rptRecebimentosDetalhados.rdlc";
                        var lstReceb = RecebimentoDAO.Instance.GetRecebimentosDetalhados(Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaUint(Request["idLoja"]), 0);
                        report.DataSources.Add(new ReportDataSource("Recebimento", lstReceb));
                        break;
                    }
                case "CentroCustosMes":
                    {
                        report.ReportPath = "Relatorios/Administrativos/rptCentroCustosMes.rdlc";

                        var idLojaCentroCustoMes = Glass.Conversoes.StrParaInt(Request["idLoja"]);
                        var valorEstoque = MovEstoqueCentroCustoDAO.Instance.ObtemSaldoEstoque(idLojaCentroCustoMes);

                        lstParam.Add(new ReportParameter("Ano", Request["ano"]));
                        lstParam.Add(new ReportParameter("ValorEstoque", valorEstoque.ToString()));

                        var lstCentroCustos = CentroCustoDAO.Instance.GetForRelCentroCustoMes(idLojaCentroCustoMes, Glass.Conversoes.StrParaInt(Request["ano"]));
                        report.DataSources.Add(new ReportDataSource("CentroCustos", lstCentroCustos));
                        break;
                    }
                case "CentroCustosPlanoContas":
                    {
                        report.ReportPath = "Relatorios/Administrativos/rptCentroCustosPlanoConta.rdlc";

                        var idLojaCentroCustoPlanoConta = Glass.Conversoes.StrParaInt(Request["idLoja"]);
                        var valorEstoque = MovEstoqueCentroCustoDAO.Instance.ObtemSaldoEstoque(idLojaCentroCustoPlanoConta);

                        lstParam.Add(new ReportParameter("Periodo", Request["dataIni"] + " à " + Request["dataFim"]));
                        lstParam.Add(new ReportParameter("ValorEstoque", valorEstoque.ToString()));

                        var lstCentroCustos = CentroCustoDAO.Instance.GetForRelCentroCustoPlanoConta(idLojaCentroCustoPlanoConta, Request["dataIni"], Request["dataFim"]);
                        report.DataSources.Add(new ReportDataSource("CentroCustos", lstCentroCustos));
                        break;
                    }
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
