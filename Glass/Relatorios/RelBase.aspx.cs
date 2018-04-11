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
using Colosoft;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Relatorios
{
    public partial class RelBase : Glass.Relatorios.UI.Web.ReportPage
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
            HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login, string diretorioLogotipos)
        {
            var idLojaLogotipo = new uint?();
            var incluirDataTextoRodape = true;

            // Nome do relat�rio
            string reportName = null;

            // Verifica qual relat�rio ser� chamado
            switch (Request["rel"])
            {
                case "Fiscal":
                case "FiscalAutFin":
                case "FiscalAutFinProd":
                    {
                        var arquivoCaminhoRpt = Request["rel"] == "Fiscal" ? "Relatorios/NFe/rptFiscal{0}.rdlc" :
                            "Relatorios/NFe/rptFiscalAutFin" + (Request["rel"] == "FiscalAutFinProd" ? "Prod" : string.Empty) + "{0}.rdlc";

                        report.ReportPath = Data.Helper.Utils.CaminhoRelatorio(arquivoCaminhoRpt);                       

                        var numeroNfe = Glass.Conversoes.StrParaUint(Request["numeroNfe"]);
                        var idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                        var idLoja = Glass.Conversoes.StrParaUint(Request["idLoja"]);
                        var idCliente = Glass.Conversoes.StrParaUint(Request["idCliente"]);
                        var idFornec = Glass.Conversoes.StrParaUint(Request["idFornec"]);
                        var tipoDoc = Glass.Conversoes.StrParaInt(Request["tipoDocumento"]);
                        var situacao = Request["situacao"];
                        var formaEmissao = Glass.Conversoes.StrParaInt(Request["formaEmissao"]);
                        var loja = LojaDAO.Instance.GetElement(login.IdLoja);
                        var dados = NotaFiscalDAO.Instance.GetListPorSituacao(numeroNfe, idPedido, Request["modelo"], idLoja, idCliente,
                            Request["nomeCliente"], Glass.Conversoes.StrParaInt(Request["tipoFiscal"]), idFornec, Request["nomeFornec"],
                            Request["codRota"], tipoDoc, situacao, Request["dataIni"], Request["dataFim"],
                            Request["idsCfop"], Request["tiposCfop"], Request["dataEntSaiIni"], Request["dataEntSaiFim"],
                            Glass.Conversoes.StrParaUint(Request["formaPagto"]), Request["idsFormaPagtoNotaFiscal"],
                            Glass.Conversoes.StrParaInt(Request["tipoNf"]), Glass.Conversoes.StrParaInt(Request["finalidade"]), formaEmissao, Request["infCompl"],
                            Request["codInternoProd"], Request["descrProd"], Request["valorInicial"],
                            Request["valorFinal"], null, Request["lote"], Glass.Conversoes.StrParaInt(Request["ordenar"]), null, 0, int.MaxValue);

                        if (FiscalConfig.Relatorio.RecuperarTotalCst60)
                            foreach (var nf in dados)
                                nf.TotalCST60 = ProdutosNfDAO.Instance.GetTotalByNfCST(nf.IdNf, "60");

                        lstParam.Add(new ReportParameter("Cabecalho_NomeLoja", String.IsNullOrEmpty(loja.RazaoSocial) ? "." : loja.RazaoSocial));
                        lstParam.Add(new ReportParameter("Cabecalho_EmailLoja", String.IsNullOrEmpty(loja.EmailContato) ? "." : loja.EmailContato));
                        lstParam.Add(new ReportParameter("Cabecalho_Criterio", dados.Count > 0 ? dados[0].Criterio : ""));
                        lstParam.Add(new ReportParameter("Agrupar", Request["agrupar"]));

                        if (Request["rel"] == "FiscalAutFin")
                            lstParam.Add(new ReportParameter("ExibeTotalImposto", (situacao.Contains("2") || situacao.Contains("13")).ToString()));

                        if (Request["rel"] != "FiscalAutFinProd")
                            report.DataSources.Add(new ReportDataSource("NotaFiscal", dados));
                        else
                            report.DataSources.Add(new ReportDataSource("ProdutosNf", ProdutosNfDAO.Instance.GetForRptFiscal(dados.ToArray(),
                                Glass.Conversoes.StrParaInt(Request["ordenar"]))));

                        break;
                    }
                case "EstoqueProdutos":
                    {
                        var tipoColunas = !String.IsNullOrEmpty(Request["tipoColunas"]) ? Glass.Conversoes.StrParaInt(Request["tipoColunas"]) : 0;
                        report.ReportPath = "Relatorios/rptEstoqueProdutos" + (PedidoConfig.LiberarPedido && tipoColunas == 0 ? "Lib" : "") + ".rdlc";
                        var idLoja = Glass.Conversoes.StrParaUint(Request["idLoja"]);
                        var idProd = !String.IsNullOrEmpty(Request["idProd"]) ? Glass.Conversoes.StrParaUint(Request["idProd"]) : 0;
                        var idGrupo = !String.IsNullOrEmpty(Request["idGrupo"]) ? Glass.Conversoes.StrParaInt(Request["idGrupo"]) : 0;
                        var idSubgrupo = !String.IsNullOrEmpty(Request["idSubgrupo"]) ? Glass.Conversoes.StrParaInt(Request["idSubgrupo"]) : 0;
                        var ordenar = !String.IsNullOrEmpty(Request["orderBy"]) ? Glass.Conversoes.StrParaInt(Request["orderBy"]) : 0;

                        lstParam.Add(new ReportParameter("ExibirGrupoProduto", (idProd == 0).ToString()));

                        if (!report.ReportPath.Contains("Lib"))
                            lstParam.Add(new ReportParameter("Reserva", (!PedidoConfig.LiberarPedido || tipoColunas == 1).ToString()));

                        report.DataSources.Add(new ReportDataSource("EstoqueProdutos", Glass.Data.RelDAL.EstoqueProdutosDAO.Instance.GetForRpt(
                            idLoja, idProd, Request["codInterno"], Request["descr"],
                            idGrupo, idSubgrupo, ordenar, Request["dataIni"],
                            Request["dataFim"], Request["dataIniLib"], Request["dataFimLib"], tipoColunas)));

                        break;
                    }
                case "ExtratoEstoque":
                    {
                        report.ReportPath = "Relatorios/rptExtratoEstoque.rdlc";

                        report.DataSources.Add(new ReportDataSource("MovEstoque", MovEstoqueDAO.Instance.GetForRpt(
                            Glass.Conversoes.StrParaUint(Request["idLoja"]), Request["codInterno"], Request["descricao"], Request["codOtimizacao"],
                            Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaInt(Request["tipoMov"]),
                            Glass.Conversoes.StrParaInt(Request["situacaoProd"]), Request["idsGrupoProd"],
                            Request["idSubgrupoProd"], Glass.Conversoes.StrParaUint(Request["idCorVidro"]),
                            Glass.Conversoes.StrParaUint(Request["idCorFerragem"]), Glass.Conversoes.StrParaUint(Request["idCorAluminio"]),
                            Convert.ToBoolean(Request["naoBuscarEstoqueZero"]), Request["lancManual"] == "true")));

                        break;
                    }
                case "ExtratoEstoqueFiscal":
                    {
                        report.ReportPath = "Relatorios/rptExtratoEstoqueFiscal.rdlc";

                        report.DataSources.Add(new ReportDataSource("MovEstoqueFiscal", MovEstoqueFiscalDAO.Instance.GetForRpt(
                            Glass.Conversoes.StrParaUint(Request["idLoja"]), Request["codInterno"], Request["descricao"], Request["ncm"], Request["numeroNfe"].StrParaInt(),
                            Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaInt(Request["tipoMov"]), Glass.Conversoes.StrParaInt(Request["situacaoProd"]),
                            Glass.Conversoes.StrParaUint(Request["idCfop"]), Glass.Conversoes.StrParaUint(Request["idGrupoProd"]),
                            Glass.Conversoes.StrParaUint(Request["idSubgrupoProd"]), Glass.Conversoes.StrParaUint(Request["idCorVidro"]),
                            Glass.Conversoes.StrParaUint(Request["idCorFerragem"]), Glass.Conversoes.StrParaUint(Request["idCorAluminio"]),
                            Convert.ToBoolean(Request["naoBuscarEstoqueZero"]), Request["lancManual"] == "true")));

                        break;
                    }
                case "ExtratoEstoqueCliente":
                    {
                        report.ReportPath = "Relatorios/rptExtratoEstoqueCliente.rdlc";

                        report.DataSources.Add(new ReportDataSource("MovEstoqueCliente", MovEstoqueClienteDAO.Instance.GetForRpt(
                            Glass.Conversoes.StrParaUint(Request["idLoja"]), Glass.Conversoes.StrParaUint(Request["idCliente"]), Request["codInterno"],
                            Request["descricao"], Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaInt(Request["tipoMov"]),
                            Glass.Conversoes.StrParaInt(Request["situacaoProd"]), Glass.Conversoes.StrParaUint(Request["idCfop"]),
                            Glass.Conversoes.StrParaUint(Request["idGrupoProd"]), Glass.Conversoes.StrParaUint(Request["idSubgrupoProd"]),
                            Glass.Conversoes.StrParaUint(Request["idCorVidro"]), Glass.Conversoes.StrParaUint(Request["idCorFerragem"]),
                            Glass.Conversoes.StrParaUint(Request["idCorAluminio"]))));

                        break;
                    }
                case "InfoPedidos":
                    {
                        report.ReportPath = "Relatorios/rptInfoPedidos.rdlc";
                        var fastDelivery = Glass.Conversoes.StrParaFloat(Request["fastDelivery"]);
                        var m2 = Glass.Conversoes.StrParaFloat(Request["m2"]);
                        var m2Interno = Glass.Conversoes.StrParaFloat(Request["m2Interno"]);
                        var idPedido = !String.IsNullOrEmpty(Request["idPedido"]) ? Glass.Conversoes.StrParaUint(Request["idPedido"]) : 0;
                        var idCliente = !String.IsNullOrEmpty(Request["idCliente"]) ? Glass.Conversoes.StrParaUint(Request["idCliente"]) : 0;
                        var tipo = !String.IsNullOrEmpty(Request["tipo"]) ? Glass.Conversoes.StrParaInt(Request["tipo"]) : 0;
                        var tipoFastDelivery = !String.IsNullOrEmpty(Request["tipoFastDelivery"]) ? Glass.Conversoes.StrParaInt(Request["tipoFastDelivery"]) : 0;

                        lstParam.Add(new ReportParameter("Data", Request["data"]));
                        lstParam.Add(new ReportParameter("FastDelivery", fastDelivery.ToString()));
                        lstParam.Add(new ReportParameter("M2", m2.ToString()));
                        lstParam.Add(new ReportParameter("M2Interno", m2Interno.ToString()));
                        lstParam.Add(new ReportParameter("TextoAdicionalProducao", ""));

                        report.DataSources.Add(new ReportDataSource("PedidoRpt", Glass.Data.RelDAL.PedidoRptDAL.Instance.CopiaLista(PedidoDAO.Instance.GetForInfoPedidos(Request["data"],
                            Request["data"], idPedido, idCliente, Request["nomeCliente"], tipo, tipoFastDelivery), PedidoRpt.TipoConstrutor.ListaPedidos, false, login)));

                        break;
                    }
                case "InfoPedidosPeriodo":
                    {
                        report.ReportPath = "Relatorios/rptInfoPedidosPeriodo.rdlc";

                        report.DataSources.Add(new ReportDataSource("InfoPedidos", Glass.Data.RelDAL.InfoPedidosDAO.Instance.GetInfoPedidos(Request["dataIni"], Request["dataFim"])));

                        lstParam.Add(new ReportParameter("DataInicio", Request["dataIni"]));
                        lstParam.Add(new ReportParameter("DataFim", Request["dataFim"]));

                        break;
                    }
                case "LiberarPedidoMov":
                case "LiberarPedidoMovSemValor":
                    {
                        report.ReportPath = "Relatorios/rptLiberacaoPedidoMov" + (Request["rel"] == "LiberarPedidoMovSemValor" ? "SemValor" : "") + ".rdlc";
                        var idClienteLPM = !String.IsNullOrEmpty(Request["idCliente"]) ? Glass.Conversoes.StrParaUint(Request["idCliente"]) : 0;
                        var situacaoLPM = !String.IsNullOrEmpty(Request["situacao"]) ? Glass.Conversoes.StrParaInt(Request["situacao"]) : 0;
                        var itens = Glass.Data.RelDAL.LiberarPedidoMovDAO.Instance.GetForRpt(idClienteLPM, Request["nomeCliente"],
                            Glass.Conversoes.StrParaUint(Request["idFunc"]), Request["dataIni"], Request["dataFim"], situacaoLPM);
                        var itensCredito = Glass.Data.RelDAL.LiberarPedidoMovDAO.Instance.GetCreditoGeradoRpt(idClienteLPM, Request["nomeCliente"],
                            Glass.Conversoes.StrParaUint(Request["idFunc"]), Request["dataIni"], Request["dataFim"], situacaoLPM);
                        var itensPagtoAntecip = Glass.Data.RelDAL.LiberarPedidoMovDAO.Instance.GetPagtoAntecipadoRpt(idClienteLPM, Request["nomeCliente"],
                            Glass.Conversoes.StrParaUint(Request["idFunc"]), Request["dataIni"], Request["dataFim"], situacaoLPM);
                        var itensSinal = Glass.Data.RelDAL.LiberarPedidoMovDAO.Instance.GetSinalRpt(idClienteLPM, Request["nomeCliente"],
                            Glass.Conversoes.StrParaUint(Request["idFunc"]), Request["dataIni"], Request["dataFim"], situacaoLPM);

                        lstParam.Add(new ReportParameter("Criterio", itens.Length > 0 ? itens[0].Criterio : ""));

                        report.DataSources.Add(new ReportDataSource("LiberarPedidoMov", itens));
                        report.DataSources.Add(new ReportDataSource("LiberarPedidoMovCredito", itensCredito));
                        report.DataSources.Add(new ReportDataSource("LiberarPedidoMovPagtoAntecip", itensPagtoAntecip));
                        report.DataSources.Add(new ReportDataSource("LiberarPedidoMovSinal", itensSinal));

                        break;
                    }
                case "MovCredito":
                    {
                        report.ReportPath = "Relatorios/rptMovCredito.rdlc";
                        var idCliente = Glass.Conversoes.StrParaUint(Request["idCliente"]);
                        var inicio = DateTime.Parse(Request["inicio"]);
                        var fim = DateTime.Parse(Request["fim"]);

                        lstParam.Add(new ReportParameter("NomeCliente", idCliente + " - " + ClienteDAO.Instance.GetNome(idCliente)));
                        lstParam.Add(new ReportParameter("DataInicio", inicio.ToString()));
                        lstParam.Add(new ReportParameter("DataFim", fim.ToString()));
                        lstParam.Add(new ReportParameter("LiberacaoPedido", PedidoConfig.LiberarPedido.ToString()));
                        lstParam.Add(new ReportParameter("TotalGerado", Request["gerado"]));
                        lstParam.Add(new ReportParameter("TotalUtilizado", Request["utilizado"]));
                        lstParam.Add(new ReportParameter("CreditoAtual", ClienteDAO.Instance.GetCredito(idCliente).ToString("C")));

                        report.DataSources.Add(new ReportDataSource("Credito", Glass.Data.RelDAL.CreditoDAO.Instance.GetCredito(idCliente, inicio, fim, Request["movimentacao"], Request["sort"], 0, 0)));

                        break;
                    }
                case "MovCreditoFornec":
                    {
                        report.ReportPath = "Relatorios/rptMovCreditoFornec.rdlc";
                        var idFornecCred = Glass.Conversoes.StrParaUint(Request["idFornec"]);
                        var inicioFornec = DateTime.Parse(Request["inicio"]);
                        var fimFornec = DateTime.Parse(Request["fim"]);

                        lstParam.Add(new ReportParameter("NomeFornecedor", idFornecCred + " - " + FornecedorDAO.Instance.GetNome(idFornecCred)));
                        lstParam.Add(new ReportParameter("DataInicio", inicioFornec.ToString()));
                        lstParam.Add(new ReportParameter("DataFim", fimFornec.ToString()));
                        lstParam.Add(new ReportParameter("LiberacaoPedido", PedidoConfig.LiberarPedido.ToString()));
                        lstParam.Add(new ReportParameter("TotalGerado", Request["gerado"]));
                        lstParam.Add(new ReportParameter("TotalUtilizado", Request["utilizado"]));
                        lstParam.Add(new ReportParameter("CreditoAtual", FornecedorDAO.Instance.GetCredito(idFornecCred).ToString("C")));

                        report.DataSources.Add(new ReportDataSource("Credito", Glass.Data.RelDAL.CreditoDAO.Instance.GetCreditoFornecedor(idFornecCred, inicioFornec,
                            fimFornec, Request["movimentacao"], Request["sort"], 0, 0)));

                        break;
                    }
                case "PrecoBeneficiamentos":
                    {
                        reportName = "PrecoBeneficiamentos";
                        break;
                    }
                case "MemoriaCalculoOrcamento":
                    {
                        report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptMemoriaCalculo{0}.rdlc");
                        var orca = OrcamentoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idOrca"]));
                        var memoriaCalculoOrca = Glass.Data.RelDAL.MemoriaCalculoDAO.Instance.GetMemoriaCalculo(orca);
                        var dadosMemoriaCalculo = MemoriaCalculoDAO.Instance.GetDadosMemoriaCalculo(orca);

                        lstParam.Add(new ReportParameter("Cabecalho_Titulo", "Or�amento"));
                        lstParam.Add(new ReportParameter("Cabecalho_Id", memoriaCalculoOrca[0].Id));
                        lstParam.Add(new ReportParameter("Cabecalho_IdProjeto", memoriaCalculoOrca[0].IdProjeto));
                        lstParam.Add(new ReportParameter("Cabecalho_NomeCliente", memoriaCalculoOrca[0].NomeCliente));
                        lstParam.Add(new ReportParameter("Cabecalho_NomeFuncionario", memoriaCalculoOrca[0].NomeFuncionario));
                        lstParam.Add(new ReportParameter("Cabecalho_Data", memoriaCalculoOrca[0].Data.ToString()));
                        lstParam.Add(new ReportParameter("Cabecalho_NomeLoja", memoriaCalculoOrca[0].NomeLoja));
                        lstParam.Add(new ReportParameter("FormatTotM", Glass.Configuracoes.Geral.GetFormatTotM()));
                        lstParam.Add(new ReportParameter("TemProdutoLamComposicao", (dadosMemoriaCalculo != null && dadosMemoriaCalculo.Any(f => f.IsProdLamComposicao)).ToString()));
                        lstParam.Add(new ReportParameter("ExibirMargemLucro", true.ToString()));

                        report.DataSources.Add(new ReportDataSource("MemoriaCalculo", memoriaCalculoOrca));
                        report.DataSources.Add(new ReportDataSource("DadosMemoriaCalculo", dadosMemoriaCalculo));

                        break;
                    }
                case "MemoriaCalculoPedido":
                    {
                        report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptMemoriaCalculo{0}.rdlc");
                        var ped = PedidoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idPed"]));
                        var memoriaCalculoPed = Glass.Data.RelDAL.MemoriaCalculoDAO.Instance.GetMemoriaCalculo(ped);

                        lstParam.Add(new ReportParameter("Cabecalho_Titulo", "Pedido"));
                        lstParam.Add(new ReportParameter("Cabecalho_Id", memoriaCalculoPed[0].Id));
                        lstParam.Add(new ReportParameter("Cabecalho_IdProjeto", memoriaCalculoPed[0].IdProjeto));
                        lstParam.Add(new ReportParameter("Cabecalho_NomeCliente", memoriaCalculoPed[0].NomeCliente));
                        lstParam.Add(new ReportParameter("Cabecalho_NomeFuncionario", memoriaCalculoPed[0].NomeFuncionario));
                        lstParam.Add(new ReportParameter("Cabecalho_Data", memoriaCalculoPed[0].Data.ToString()));
                        lstParam.Add(new ReportParameter("Cabecalho_NomeLoja", memoriaCalculoPed[0].NomeLoja));
                        lstParam.Add(new ReportParameter("FormatTotM", Glass.Configuracoes.Geral.GetFormatTotM()));
                        lstParam.Add(new ReportParameter("TemProdutoLamComposicao", ped.TemProdutoLamComposicao.ToString()));
                        lstParam.Add(new ReportParameter("ExibirMargemLucro", "true"));

                        report.DataSources.Add(new ReportDataSource("MemoriaCalculo", memoriaCalculoPed));
                        report.DataSources.Add(new ReportDataSource("DadosMemoriaCalculo", Glass.Data.RelDAL.MemoriaCalculoDAO.Instance.GetDadosMemoriaCalculo(ped)));

                        break;
                    }
                case "MemoriaCalculoPedidoEspelho":
                    {
                        report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptMemoriaCalculo{0}.rdlc");
                        var pedEsp = PedidoEspelhoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idPed"]));
                        var memoriaCalculoPedEsp = Glass.Data.RelDAL.MemoriaCalculoDAO.Instance.GetMemoriaCalculo(pedEsp);
                        var dadosMemoriaCalculoPesEsp = Glass.Data.RelDAL.MemoriaCalculoDAO.Instance.GetDadosMemoriaCalculo(pedEsp);

                        if (pedEsp.TemProdutoLamComposicao)
                            dadosMemoriaCalculoPesEsp = dadosMemoriaCalculoPesEsp.Where(f => f.IsProdLamComposicao).ToArray();

                        lstParam.Add(new ReportParameter("Cabecalho_Titulo", "Pedido PCP"));
                        lstParam.Add(new ReportParameter("Cabecalho_Id", memoriaCalculoPedEsp[0].Id));
                        lstParam.Add(new ReportParameter("Cabecalho_IdProjeto", memoriaCalculoPedEsp[0].IdProjeto));
                        lstParam.Add(new ReportParameter("Cabecalho_NomeCliente", memoriaCalculoPedEsp[0].NomeCliente));
                        lstParam.Add(new ReportParameter("Cabecalho_NomeFuncionario", memoriaCalculoPedEsp[0].NomeFuncionario));
                        lstParam.Add(new ReportParameter("Cabecalho_Data", memoriaCalculoPedEsp[0].Data.ToString()));
                        lstParam.Add(new ReportParameter("Cabecalho_NomeLoja", memoriaCalculoPedEsp[0].NomeLoja));
                        lstParam.Add(new ReportParameter("FormatTotM", Glass.Configuracoes.Geral.GetFormatTotM()));
                        lstParam.Add(new ReportParameter("TemProdutoLamComposicao", pedEsp.TemProdutoLamComposicao.ToString()));
                        lstParam.Add(new ReportParameter("ExibirMargemLucro", "true"));

                        report.DataSources.Add(new ReportDataSource("MemoriaCalculo", memoriaCalculoPedEsp));
                        report.DataSources.Add(new ReportDataSource("DadosMemoriaCalculo", dadosMemoriaCalculoPesEsp));

                        break;
                    }
                case "comLucr":
                    {
                        report.ReportPath = "Relatorios/rptVendasComLucr.rdlc";
                        var situacaoComLucr = !String.IsNullOrEmpty(Request["situacao"]) ? Glass.Conversoes.StrParaInt(Request["situacao"]) : (int)Glass.Data.Model.Pedido.SituacaoPedido.Confirmado;
                        var lstComLucr = PedidoDAO.Instance.GetForRptLucr(Request["idLoja"], Request["idVend"], situacaoComLucr, Request["DtIni"], Request["DtFim"],
                            Glass.Conversoes.StrParaInt(Request["TipoVenda"]), Glass.Conversoes.StrParaInt(Request["agruparFunc"]), Request["orderBy"]);

                        if (Request["idVend"] != null)
                        {
                            var idPedido = new List<uint>();
                            foreach (var p in lstComLucr)
                                idPedido.Add(p.IdPedido);

                            lstParam.Add(new ReportParameter("ValorComissao", ComissaoConfigDAO.Instance.GetComissaoValor(0, Glass.Conversoes.StrParaUint(Request["idVend"]), null, null, idPedido.ToArray()).ToString()));
                        }
                        else
                            lstParam.Add(new ReportParameter("ValorComissao", "0"));

                        lstParam.Add(new ReportParameter("Agrupar", (Request["agrupar"] == "1").ToString()));

                        report.DataSources.Add(new ReportDataSource("PedidoRpt", Glass.Data.RelDAL.PedidoRptDAL.Instance.CopiaLista(lstComLucr, PedidoRpt.TipoConstrutor.ListaPedidos, false, login)));

                        break;
                    }
                case "semLucr":
                    {
                        report.ReportPath = "Relatorios/rptVendasSemLucr.rdlc";
                        var situacaoSemLucr = !String.IsNullOrEmpty(Request["situacao"]) ? Glass.Conversoes.StrParaInt(Request["situacao"]) : (int)Glass.Data.Model.Pedido.SituacaoPedido.Confirmado;
                        var lstSemLucr = PedidoDAO.Instance.GetForRptLucr(Request["idLoja"], Request["idVend"], Request["idPedido"].StrParaInt(), Request["idCliente"].StrParaInt(),
                            Request["nomeCliente"], situacaoSemLucr, Request["DtIni"], Request["DtFim"], Glass.Conversoes.StrParaInt(Request["TipoVenda"]),
                            Glass.Conversoes.StrParaInt(Request["agruparFunc"]), Request["orderBy"]);

                        if (Request["idVend"] != null)
                        {
                            var idPedido = new List<uint>();
                            foreach (var p in lstSemLucr)
                                idPedido.Add(p.IdPedido);

                            lstParam.Add(new ReportParameter("ValorComissao", ComissaoConfigDAO.Instance.GetComissaoValor(0, Glass.Conversoes.StrParaUint(Request["idVend"]), null, null, idPedido.ToArray()).ToString()));
                        }
                        else
                            lstParam.Add(new ReportParameter("ValorComissao", "0"));

                        lstParam.Add(new ReportParameter("Agrupar", (Request["agrupar"] == "1").ToString()));

                        report.DataSources.Add(new ReportDataSource("PedidoRpt", Glass.Data.RelDAL.PedidoRptDAL.Instance.CopiaLista(lstSemLucr, PedidoRpt.TipoConstrutor.ListaPedidos, false, login)));

                        break;
                    }
                case "SemImposto":
                    {
                        report.ReportPath = "Relatorios/rptVendasSemImposto.rdlc";
                        var lstSemImposto = PedidoDAO.Instance.GetForRptSemImposto(Request["idLoja"], Request["idVend"], Request["DtIni"], Request["DtFim"],
                            Glass.Conversoes.StrParaInt(Request["TipoVenda"]), Request["orderBy"]);

                        if (Request["idVend"] != null)
                        {
                            List<uint> idPedido = new List<uint>();
                            foreach (Glass.Data.Model.Pedido p in lstSemImposto)
                                idPedido.Add(p.IdPedido);

                            lstParam.Add(new ReportParameter("ValorComissao", ComissaoConfigDAO.Instance.GetComissaoValor(0, Glass.Conversoes.StrParaUint(Request["idVend"]),
                                null, null, idPedido.ToArray()).ToString()));
                        }
                        else
                            lstParam.Add(new ReportParameter("ValorComissao", "0"));

                        lstParam.Add(new ReportParameter("UsarLiberacao", PedidoConfig.LiberarPedido.ToString()));

                        report.DataSources.Add(new ReportDataSource("PedidoRpt", Glass.Data.RelDAL.PedidoRptDAL.Instance.CopiaLista(lstSemImposto, PedidoRpt.TipoConstrutor.ListaPedidos, false, login)));

                        break;
                    }
                case "vendasProd":
                    {
                        report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptVendasProd{0}.rdlc");
                        var lstProd = ProdutoDAO.Instance.GetRptVendasProd(Glass.Conversoes.StrParaUint(Request["idCliente"]), Request["nomeCliente"],
                            Request["codRota"], Request["idLoja"], Request["idsGrupos"], Request["idsSubgrupo"], Request["codInterno"], Request["descrProd"],
                            Request["incluirMateriaPrima"] == "1", Request["dtIni"], Request["dtFim"], Request["dtIniPed"], Request["dtFimPed"],
                            Request["dtIniEnt"], Request["dtFimEnt"], Request["situacao"], Request["situacaoProd"], Request["tipoVenda"],
                            Glass.Conversoes.StrParaUint(Request["idFunc"]), Glass.Conversoes.StrParaUint(Request["idFuncCliente"]),
                            Glass.Conversoes.StrParaInt(Request["tipoFastDelivery"]), Request["idPedido"],
                            Glass.Conversoes.StrParaInt(Request["tipoDesconto"]), Request["agruparCli"] == "1" && Request["agrupar"] == "false",
                            Request["agruparPedido"] == "1" && Request["agrupar"] == "false", Request["agruparLiberacao"] == "1", Request["agruparAmbiente"] == "1",
                            Glass.Conversoes.StrParaInt(Request["buscarNotaFiscal"]), Request["idLiberacao"].StrParaInt(), Glass.Conversoes.StrParaInt(Request["ordenacao"]));

                        var lstProdCorEsp = lstProd
                            .Where(f => f.IdGrupoProd == (int)NomeGrupoProd.Vidro)
                            .GroupBy(f => new { f.IdCorVidro, f.Espessura })
                            .Select(f => new
                            {
                                Cor = f.FirstOrDefault().DescrCor,
                                Espessura = f.Key.Espessura,
                                TotM2ML = f.Sum(x => x.TotalM2 > 0 ? x.TotalM2 : x.TotalML),
                                ValorTotalVendido = f.Sum(t => t.TotalVend),
                                QtdVendido = f.Sum(q => q.TotalQtde)

                            })
                            .OrderBy(f => f.Cor).ThenBy(f => f.Espessura)
                            .ToList();                       

                        lstParam.Add(new ReportParameter("AgruparGrupo", (Request["agruparGrupo"] == "1" || Request["agrupar"] == "true").ToString()));
                        lstParam.Add(new ReportParameter("AgruparCorEspessura", (Request["agruparCorEsp"] == "1" && Request["agrupar"] == "false").ToString()));
                        lstParam.Add(new ReportParameter("AgruparCli", (Request["agruparCli"] == "1" && Request["agrupar"] == "false").ToString()));
                        lstParam.Add(new ReportParameter("AgruparPedido", (Request["agruparPedido"] == "1" && Request["agrupar"] == "false").ToString()));
                        lstParam.Add(new ReportParameter("AgruparNcm", (Request["agruparNcm"] == "1").ToString()));
                        lstParam.Add(new ReportParameter("AgruparLiberacao", (Request["agruparLiberacao"] == "1" && Request["agrupar"] == "false").ToString()));
                        lstParam.Add(new ReportParameter("ApenasGrupo", (Request["agrupar"] == "true").ToString()));
                        lstParam.Add(new ReportParameter("SemValores", (Request["semValores"] == "true").ToString()));
                        lstParam.Add(new ReportParameter("AgruparAmbiente", (Request["agruparAmbiente"] == "1" && Request["agrupar"] == "false").ToString()));

                        report.DataSources.Add(new ReportDataSource("Produto", lstProd.ToArray()));
                        report.DataSources.Add(new ReportDataSource("ProdCorEspessura", lstProdCorEsp.ToArray()));

                        break;
                    }
                case "producaoProd":
                    {
                        report.ReportPath = "Relatorios/rptProducaoProd.rdlc";
                        var lstProducaoProd = ProdutoDAO.Instance.GetRptProducaoProd(Request["idLoja"], Request["idsGrupos"],
                            Glass.Conversoes.StrParaUint(Request["idSubgrupo"]), Request["codInterno"], Request["descrProd"],
                            Request["incluirMateriaPrima"] == "1", Request["dtIni"], Request["dtFim"],
                            Request["dtIniPed"], Request["dtFimPed"], Request["dtIniEnt"],
                            Request["dtFimEnt"], Request["situacao"], Request["situacaoProd"],
                            Request["tipoPedido"], Glass.Conversoes.StrParaUint(Request["idFuncCliente"]), Glass.Conversoes.StrParaInt(Request["tipoFastDelivery"]),
                            Request["idPedido"], Request["agruparPedido"] == "1" && Request["agrupar"] == "false", login);

                        lstParam.Add(new ReportParameter("AgruparGrupo", (Request["agruparGrupo"] == "1" || Request["agrupar"] == "true").ToString()));
                        lstParam.Add(new ReportParameter("AgruparCorEspessura", (Request["agruparCorEsp"] == "1" && Request["agrupar"] == "false").ToString()));
                        lstParam.Add(new ReportParameter("AgruparPedido", (Request["agruparPedido"] == "1" && Request["agrupar"] == "false").ToString()));
                        lstParam.Add(new ReportParameter("ApenasGrupo", (Request["agrupar"] == "true").ToString()));

                        report.DataSources.Add(new ReportDataSource("Produto", lstProducaoProd.ToArray()));

                        break;
                    }
                case "comprasProd":
                    {
                        report.ReportPath = "Relatorios/rptComprasProd.rdlc";
                        _loadSubreport = Request["exibirDetalhes"] == "1";
                        var lstProdComp = ProdutoDAO.Instance.GetRptComprasProd(Glass.Conversoes.StrParaUint(Request["idFornec"]),
                            Request["nomeFornec"], Request["idLoja"], Glass.Conversoes.StrParaUint(Request["idGrupo"]),
                            Glass.Conversoes.StrParaUint(Request["idSubgrupo"]), Request["codInterno"], Request["descrProd"],
                            Request["dtIni"], Request["dtFim"], Glass.Conversoes.StrParaUint(Request["idFunc"]),
                            Request["agruparFornec"] == "1", Request["tipoCfop"], _loadSubreport, Request["comSemNF"]);

                        lstParam.Add(new ReportParameter("AgruparGrupo", (Request["agruparGrupo"] == "1").ToString()));
                        lstParam.Add(new ReportParameter("AgruparFornec", (Request["agruparFornec"] == "1").ToString()));
                        lstParam.Add(new ReportParameter("ExibirDetalhes", _loadSubreport.ToString()));

                        report.DataSources.Add(new ReportDataSource("Produto", lstProdComp.ToArray()));

                        break;
                    }
                case "VendaAVista":
                    {
                        report.ReportPath = "Relatorios/rptVendasAVista.rdlc";
                        var lstAVista = PedidoDAO.Instance.GetForRptAVista(Request["idLoja"], Request["idVend"], Glass.Conversoes.StrParaUint(Request["IdFormaPagto"]),
                            Glass.Conversoes.StrParaUint(Request["tipoCartao"]), Request["DtIni"], Request["DtFim"]);
                        var lstTotais = PedidoDAO.Instance.TotaisAVista(Request["idLoja"], Request["idVend"], Glass.Conversoes.StrParaUint(Request["IdFormaPagto"]),
                            Glass.Conversoes.StrParaUint(Request["tipoCartao"]), Request["DtIni"], Request["DtFim"]);

                        lstParam.Add(new ReportParameter("TotalDinheiro", lstTotais[0]));
                        lstParam.Add(new ReportParameter("TotalCheque", lstTotais[1]));
                        lstParam.Add(new ReportParameter("TotalCartao", lstTotais[2]));
                        lstParam.Add(new ReportParameter("TotalConstrucard", lstTotais[3]));
                        lstParam.Add(new ReportParameter("TotalPermuta", lstTotais[4]));
                        lstParam.Add(new ReportParameter("TotalDeposito", lstTotais[5]));

                        report.DataSources.Add(new ReportDataSource("PedidoRpt", Glass.Data.RelDAL.PedidoRptDAL.Instance.CopiaLista(lstAVista, PedidoRpt.TipoConstrutor.ListaPedidos, false, login)));

                        break;
                    }
                case "ContasRecebidas":
                    {
                        report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptContasRecebidasRetrato{0}.rdlc");
                        bool? recebida = Request["ExibirAReceber"] == "true" ? (bool?)null : true;
                        var contasRecebidas = ContasReceberDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idPedido"]),
                            Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]), Glass.Conversoes.StrParaUint(Request["idAcerto"]), Glass.Conversoes.StrParaUint(Request["idAcertoParcial"]),
                            Glass.Conversoes.StrParaUint(Request["idTrocaDev"]), Glass.Conversoes.StrParaUint(Request["numeroNFe"]), Glass.Conversoes.StrParaUint(Request["idLoja"]),
                            Glass.Conversoes.StrParaUint(Request["idCli"]), Glass.Conversoes.StrParaUint(Request["idFunc"]), Glass.Conversoes.StrParaUint(Request["idFuncRecebido"]),
                            Glass.Conversoes.StrParaUint(Request["tipoEntrega"]), Request["nomeCli"], Request["dtIniVenc"], Request["dtFimVenc"], Request["dtIniRec"],
                            Request["dtFimRec"], Request["dataIniCad"], Request["dataFimCad"], null, null, Request["idsFormaPagto"], Glass.Conversoes.StrParaUint(Request["tipoBoleto"]),
                            Glass.Conversoes.StrParaFloat(Request["valorInicial"]), Glass.Conversoes.StrParaFloat(Request["valorFinal"]), Glass.Conversoes.StrParaInt(Request["ordenar"]),
                            Request["renegociadas"] == "true", recebida, Glass.Conversoes.StrParaUint(Request["idComissionado"]), Glass.Conversoes.StrParaUint(Request["idRota"]),
                            Request["obs"], Request["tipoConta"], Glass.Conversoes.StrParaUint(Request["numArqRemessa"]), bool.Parse(Request["refObra"]), Glass.Conversoes.StrParaInt(Request["contasCnab"]),
                            Glass.Conversoes.StrParaInt(Request["idVendedorAssociado"]), Glass.Conversoes.StrParaInt(Request["idVendedorObra"]), Request["idComissao"].StrParaInt(),
                            Request["idSinal"].StrParaInt(), Request["numCte"].StrParaInt(), bool.Parse(Request["protestadas"]), bool.Parse(Request["contasVinculadas"]), Request["tipoContasBuscar"], Request["numAutCartao"]);

                        var idsContas = contasRecebidas.Select(f => f.IdContaR).ToList();

                        var pagtos = PagtoContasReceberDAO.Instance.ObtemPagtos(string.Join(",", idsContas.Select(f => f.ToString()).ToArray()));

                        if (report.ReportPath.Contains("rptContasRecebidasLocal"))
                            ContasReceberDAO.Instance.PreencheLocalizacao(ref contasRecebidas);
                        else
                        {
                            lstParam.Add(new ReportParameter("ExibirComissao", (FinanceiroConfig.RelatorioContasRecebidas.ExibirComissao).ToString()));
                            lstParam.Add(new ReportParameter("ExibirPedidos", (FinanceiroConfig.RelatorioContasRecebidas.ExibirPedidos).ToString()));
                        }

                        lstParam.Add(new ReportParameter("LiberarPedido", PedidoConfig.LiberarPedido.ToString()));
                        lstParam.Add(new ReportParameter("ExibirAReceber", (Request["ExibirAReceber"] == "true").ToString()));

                        report.DataSources.Add(new ReportDataSource("ContasReceber", contasRecebidas));
                        report.DataSources.Add(new ReportDataSource("PagtoContasReceber", pagtos));

                        break;
                    }
                case "ContasReceber":
                case "ContasReceberTotal":
                    {
                        // rpContasReceberSemObs.rdlc
                        // rpContasReceberRetrato.rdlc
                        // rpContasReceber.rdlc
                        // rpContasReceberResumido.rdlc
                        report.ReportPath = Request["rel"] == "ContasReceber" ? Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptContasReceberRetrato{0}.rdlc") :
                            "Relatorios/rptContasReceberTotal.rdlc";

                        var contasReceber = ContasReceberDAO.Instance.GetNaoRecebidasRpt(Glass.Conversoes.StrParaUint(Request["idContaR"]), Glass.Conversoes.StrParaUint(Request["idPedido"]),
                            Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]), Glass.Conversoes.StrParaUint(Request["idAcerto"]), Glass.Conversoes.StrParaUint(Request["idTrocaDev"]),
                            Glass.Conversoes.StrParaUint(Request["numeroNFe"]), Glass.Conversoes.StrParaUint(Request["idLoja"]), Request["lojaCliente"] == "true",
                            Glass.Conversoes.StrParaUint(Request["idCli"]), Glass.Conversoes.StrParaUint(Request["idFunc"]), Glass.Conversoes.StrParaUint(Request["tipoEntrega"]),
                            Request["nomeCli"], Request["dtIni"], Request["dtFim"], Request["dtIniLib"], Request["dtFimLib"], Request["dataCadIni"],
                            Request["dataCadFim"], Glass.Conversoes.StrParaFloat(Request["precoInicial"]), Glass.Conversoes.StrParaFloat(Request["precoFinal"]),
                            Glass.Conversoes.StrParaUint(Request["formaPagto"]), Glass.Conversoes.StrParaInt(Request["situacaoPedido"]), Request["incluirParcCartao"] == "true",
                            Request["contasRenegociadas"].StrParaIntNullable().GetValueOrDefault(), Request["apenasNfe"] == "true", Glass.Conversoes.StrParaUint(Request["apenasContasAntecipadas"]), Request["agrupar"],
                            Glass.Conversoes.StrParaInt(Request["sort"]), Glass.Conversoes.StrParaInt(Request["contasCnab"]), Request["idsRotas"],
                            Request["obs"], Request["tipoContasBuscar"], Request["tipoConta"], Glass.Conversoes.StrParaUint(Request["numArqRemessa"]),
                            (Request["refObra"] != null ? Request["refObra"].ToLower() == "true" : false),
                            (Request["exibirContasVinculadas"] != null ? Request["exibirContasVinculadas"].ToLower() == "true" : false),
                            Request["protestadas"].StrParaInt(), Glass.Conversoes.StrParaUint(Request["idContaBanco"]), Request["numCte"].StrParaInt());

                        if (Request["rel"] == "ContasReceber")
                        {
                            lstParam.Add(new ReportParameter("ExibirColunaDataLib", PedidoConfig.LiberarPedido.ToString()));
                            lstParam.Add(new ReportParameter("Agrupar", Request["agrupar"].ToString())); // 1-Cliente, 2-Data Venc, 3-Data Cad, 4-Comissionado

                            if (report.ReportPath == "Relatorios/rptContasReceber.rdlc" ||
                                report.ReportPath == "Relatorios/rptContasReceberRetrato.rdlc")
                            {
                                decimal creditoTotal = 0;
                                decimal chequeDevolvidoTotal = 0;
                                var idCliContasRec = new List<uint>();

                                foreach (var contaRec in contasReceber)
                                    if (!idCliContasRec.Contains(contaRec.IdCliente))
                                    {
                                        idCliContasRec.Add(contaRec.IdCliente);
                                        creditoTotal += contaRec.CreditoCliente;

                                        var chequesDev = ChequesDAO.Instance.GetDevolvidosPorCliente(contaRec.IdCliente);
                                        if (chequesDev != null)
                                            contaRec.TotalChequeDevolvido = chequesDev.Sum(f => f.Valor - f.ValorReceb);

                                        chequeDevolvidoTotal += contaRec.TotalChequeDevolvido;
                                    }

                                lstParam.Add(new ReportParameter("CreditoTotal", creditoTotal.ToString()));
                                lstParam.Add(new ReportParameter("ChequeDevolvidoTotal", chequeDevolvidoTotal.ToString()));
                                lstParam.Add(new ReportParameter("ExibirPedidos", (FinanceiroConfig.RelatorioContasRecebidas.ExibirPedidos).ToString()));

                                var idCli = Request["idCli"].StrParaUintNullable();
                                var cheques = new List<Cheques>();

                                if (idCli.GetValueOrDefault(0) > 0)
                                    cheques = ChequesDAO.Instance.GetDevolvidosPorCliente(idCli.Value).ToList();

                                report.DataSources.Add(new ReportDataSource("Cheques", cheques));
                            }
                        }
                        else
                        {
                            var listaClientes = new List<uint>();

                            foreach (var c in contasReceber)
                                if (listaClientes.Contains(c.IdCliente))
                                {
                                    c.CreditoCliente = 0;
                                    c.LimiteCliente = 0;
                                }
                                else
                                    listaClientes.Add(c.IdCliente);
                        }

                        report.DataSources.Add(new ReportDataSource("ContasReceber", contasReceber.ToArray()));

                        break;
                    }
                case "DebitosCliente":
                    {
                        report.ReportPath = "Relatorios/rptDebitosCliente.rdlc";
                        var debitos = ContasReceberDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idPedido"]),
                            Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]), 0, 0, 0, 0, 0, Glass.Conversoes.StrParaUint(Request["idCli"]),
                            Glass.Conversoes.StrParaUint(Request["idFunc"]), 0, 0, Request["nomeCli"], null, null, null, null, null, null, null, null,
                            "", 0, 0, 0, 1, null, false, 0, 0, null, null, 0, true, 0, 0, 0, 0, 0, 0, false, false, null, "");

                        lstParam.Add(new ReportParameter("LiberarPedido", PedidoConfig.LiberarPedido.ToString()));

                        report.DataSources.Add(new ReportDataSource("ContasReceber", debitos));

                        break;
                    }
                case "ContasPagar":
                    {
                        report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptContasPagarRetrato{0}.rdlc");
                        var lstPrevisaoPg = new decimal[4];
                        var contasPagar = ContasPagarDAO.Instance.GetPagtosForRpt(Request["idContaPg"].StrParaIntNullable(), Request["idCompra"].StrParaUint(), Request["nf"],
                            Glass.Conversoes.StrParaUint(Request["idLoja"]), Glass.Conversoes.StrParaUint(Request["idCustoFixo"]), Glass.Conversoes.StrParaUint(Request["idImpostoServ"]),
                            Glass.Conversoes.StrParaUint(Request["idFornec"]), Request["nomeFornec"], Request["dtIni"], Request["dtFim"], Request["dataCadIni"],
                            Request["dataCadFim"], Request["idsFormaPagto"], Glass.Conversoes.StrParaFloat(Request["valorInicial"]),
                            Glass.Conversoes.StrParaFloat(Request["valorFinal"]), Glass.Conversoes.StrParaInt(Request["tipo"]), Request["incluirCheques"] == "true",
                            Request["incluirCheques"] == "true", Request["previsaoCustoFixo"] == "true", Request["comissao"] == "true",
                            Request["planoConta"], Glass.Conversoes.StrParaUint(Request["idPagtoRestante"]), Request["custoFixo"] == "true", Request["ordenar"],
                            ref lstPrevisaoPg, Request["contasSemValor"] == "true", Request["dtBaixadoIni"], Request["dtBaixadoFim"],
                            Request["dtNfCompraIni"], Request["dtNfCompraFim"], Glass.Conversoes.StrParaUint(Request["numCte"]), Glass.Conversoes.StrParaUint(Request["idTransportadora"]),
                            Request["nomeTransportadora"], Request["idFuncComissao"].StrParaInt(), Request["idComissao"].StrParaInt());

                        lstParam.Add(new ReportParameter("Salarios", lstPrevisaoPg[0].ToString()));
                        lstParam.Add(new ReportParameter("Ferias", lstPrevisaoPg[1].ToString()));
                        lstParam.Add(new ReportParameter("DecTerc", lstPrevisaoPg[2].ToString()));
                        lstParam.Add(new ReportParameter("Ipva", lstPrevisaoPg[3].ToString()));
                        lstParam.Add(new ReportParameter("Agrupar", !String.IsNullOrEmpty(Request["agrupar"]) ? Request["agrupar"] : "0"));
                        lstParam.Add(new ReportParameter("ExibirSoPrevisaoCustoFixo", (Request["ExibirSoPrevisaoCustoFixo"] == "true").ToString()));
                        lstParam.Add(new ReportParameter("ExibirCreditoFornec", FinanceiroConfig.FormaPagamento.CreditoFornecedor.ToString()));

                        report.DataSources.Add(new ReportDataSource("ContasPagar", contasPagar));

                        break;
                    }
                case "ContasPagas":
                    {
                        report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptContasPagasRetrato{0}.rdlc");
                        var contasPagas = ContasPagarDAO.Instance.GetPagasForRpt(0, Request["idCompra"].StrParaUint(), Request["nf"], Request["idLoja"].StrParaUint(), Request["idCustoFixo"].StrParaUint(),
                            Request["IdImpostoServ"].StrParaUint(), Request["idFornec"].StrParaUint(), Request["nomeFornec"], Request["formaPagto"].StrParaUint(), Request["dataIniCad"],
                            Request["dataFimCad"], Request["dtIniPago"], Request["dtFimPago"], Request["dtIniVenc"], Request["dtFimVenc"], Request["valorInicial"].StrParaFloat(),
                            Request["valorFinal"].StrParaFloat(), Request["tipo"].StrParaInt(), Request["comissao"] == "true", Request["renegociadas"] == "true", Request["jurosMulta"] == "true",
                            Request["planoConta"], Request["custoFixo"] == "true", Request["exibirAPagar"] == "true", Request["idComissao"].StrParaInt(), Request["numCte"].StrParaInt(), Request["observacao"],
                            Request["ordenar"]);

                        lstParam.Add(new ReportParameter("Agrupar", !String.IsNullOrEmpty(Request["agrupar"]) ? Request["agrupar"] : "0"));
                        lstParam.Add(new ReportParameter("ExibirAPagar", (Request["exibirAPagar"] == "true").ToString()));

                        var lancamentosAvulsosCG = CaixaGeralDAO.Instance.GetSaldoLancAvulsos(Glass.Conversoes.StrParaDate(Request["dtIniPago"].ToString()),
                            Glass.Conversoes.StrParaDate(Request["dtFimPago"].ToString()), Request["idFornec"].StrParaUint());
                        var lancamentosAvulsosCD = Glass.Data.DAL.CaixaDiarioDAO.Instance.GetSaldoLancAvulsos(Glass.Conversoes.StrParaDate(Request["dtIniPago"].ToString()),
                            Glass.Conversoes.StrParaDate(Request["dtFimPago"].ToString()));

                        var valorPagtoPermuta = ContasPagarDAO.Instance.ObtemValorPagtoPermuta(string.Join(",", contasPagas.Select(f => f.IdContaPg.ToString()).ToArray()));
                        var valorPagtoEncontroContas = ContasPagarDAO.Instance.ObtemValorPagtoEncontroContas(string.Join(",", contasPagas.Select(f => f.IdContaPg.ToString()).ToArray()));

                        lstParam.Add(new ReportParameter("LancManual", (lancamentosAvulsosCG + lancamentosAvulsosCD).ToString()));
                        lstParam.Add(new ReportParameter("ValorPagtoPermuta", valorPagtoPermuta.ToString()));
                        lstParam.Add(new ReportParameter("valorPagtoEncontroContas", valorPagtoEncontroContas.ToString()));

                        report.DataSources.Add(new ReportDataSource("ContasPagar", contasPagas));

                        break;
                    }
                case "CaixaDiario":
                    {
                        report.ReportPath = "Relatorios/rptCaixaDiario.rdlc";

                        var idFunc = Request["idFunc"].StrParaUint();
                        var caixaDiario = Glass.Data.DAL.CaixaDiarioDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idLoja"]),
                            idFunc, DateTime.Parse(Request["Data"]));

                        var idLoja = string.IsNullOrEmpty(Request["idLoja"]) ? 0 : Glass.Conversoes.StrParaUint(Request["idLoja"]);

                        if (caixaDiario[0].NomeLoja == " " && idLoja > 0)
                            caixaDiario[0].NomeLoja = LojaDAO.Instance.GetNome(idLoja);                        

                        lstParam.Add(new ReportParameter("SomenteTotais", (Request["somenteTotais"] == "true").ToString()));
                        lstParam.Add(new ReportParameter("NomeFuncionarioFiltro", idFunc > 0 ? FuncionarioDAO.Instance.GetNome(null, idFunc) : string.Empty));
                        lstParam.Add(new ReportParameter("DataFechamentoFiltro", Request["Data"]));

                        report.DataSources.Add(new ReportDataSource("CaixaDiario", caixaDiario));
                        report.DataSources.Add(new ReportDataSource("Cartoes", caixaDiario[0].Cartoes));

                        break;
                    }
                case "CaixaGeral":
                    {
                        report.ReportPath = "Relatorios/rptCaixaGeral.rdlc";
                        var caixaGeral = CaixaGeralDAO.Instance.GetMovimentacoes(Glass.Conversoes.StrParaUint(Request["id"]), Glass.Conversoes.StrParaUint(Request["idFunc"]),
                            Request["dtIni"], Request["dtFim"], Request["valorIni"], Request["valorFim"], Request["apenasDinheiro"] == "true", Request["apenasCheque"] == "true",
                            Glass.Conversoes.StrParaInt(Request["tipoMov"]), Glass.Conversoes.StrParaInt(Request["tipoConta"]), Request["semEstorno"] == "true", Glass.Conversoes.StrParaUint(Request["idLoja"]), login);

                        lstParam.Add(new ReportParameter("LiberarPedido", PedidoConfig.LiberarPedido.ToString()));
                        lstParam.Add(new ReportParameter("MostrarResumo", Glass.Conversoes.StrParaInt(Request["tipoConta"]) != 0 ? "false" : "true"));
                        lstParam.Add(new ReportParameter("ExibirDebitoCredito", FinanceiroConfig.CaixaGeral.ExibirDebitoCredito.ToString()));
                        lstParam.Add(new ReportParameter("ExibirColunasContaRecebida", FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber.ToString()));
                        lstParam.Add(new ReportParameter("DescricaoContaReceberContabil", FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil));
                        lstParam.Add(new ReportParameter("DescricaoContaReceberNaoContabil", FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil));
                        lstParam.Add(new ReportParameter("SomenteTotais", (Request["somenteTotais"] == "true").ToString()));

                        report.DataSources.Add(new ReportDataSource("CaixaGeral", caixaGeral));

                        break;
                    }
                case "Produtos":
                    {
                        reportName = Request["rel"];
                        break;
                    }
                case "ProdutosPreco":
                    {
                        report.ReportPath = "Relatorios/rptProdutosPreco.rdlc";

                        var produtosPreco = ProdutoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idFornec"]), Request["nomeFornec"],
                            Glass.Conversoes.StrParaUint(Request["idGrupo"]), Glass.Conversoes.StrParaUint(Request["idSubgrupo"]), Request["codInterno"],
                            Request["descr"], 0, Glass.Conversoes.StrParaInt(Request["situacao"]), false,
                            0, 0, 0, 0, Glass.Conversoes.StrParaInt(Request["orderBy"]));

                        lstParam.Add(new ReportParameter("Agrupar", (Request["agrupar"] == "1").ToString()));
                        lstParam.Add(new ReportParameter("Criterio", produtosPreco.Count > 0 ? produtosPreco[0].Criterio : ""));

                        report.DataSources.Add(new ReportDataSource("Produto", produtosPreco.ToArray()));

                        break;
                    }
                case "ProdutosTrib":
                    {
                        report.ReportPath = "Relatorios/rptProdutosTrib.rdlc";
                        var produtosTrib = ProdutoDAO.Instance.GetForRpt(0, null, Glass.Conversoes.StrParaUint(Request["idGrupo"]), Glass.Conversoes.StrParaUint(Request["idSubgrupo"]),
                            Request["codInterno"], Request["descr"], !String.IsNullOrEmpty(Request["tipoProd"]) ? Glass.Conversoes.StrParaInt(Request["tipoProd"]) : 1,
                            Glass.Conversoes.StrParaInt(Request["situacao"]), Request["apenasProdutosEstoqueBaixa"] == "true",
                            0, 0, 0, 0, Glass.Conversoes.StrParaInt(Request["orderBy"]));

                        report.DataSources.Add(new ReportDataSource("Produto", produtosTrib.ToArray()));

                        break;
                    }
                case "ListaImpostoServ":
                    {
                        report.ReportPath = "Relatorios/rptListaImpostoServ.rdlc";
                        bool? ImpostoServ_Contabil = Request["contabil"] == String.Empty ? null : Request["contabil"] == "True" ? (bool?)true : (bool?)false;
                        var lstImpostoServ = ImpostoServDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idImpostoServ"]), Request["dataIni"],
                            Request["dataFim"], Glass.Conversoes.StrParaFloat(Request["valorIni"]), Glass.Conversoes.StrParaFloat(Request["valorFim"]),
                            Glass.Conversoes.StrParaUint(Request["idFornec"]), Request["nomeFornec"], ImpostoServ_Contabil, Glass.Conversoes.StrParaInt(Request["tipoPagto"]),
                            bool.Parse(Request["centroCustoDivergente"]));

                        report.DataSources.Add(new ReportDataSource("ImpostoServ", lstImpostoServ.ToArray()));

                        break;
                    }
                case "ListaCompras":
                    {
                        report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptListaCompras{0}.rdlc");

                        var idCompra = string.IsNullOrEmpty(Request["numCompra"]) ? 0 : Glass.Conversoes.StrParaUint(Request["numCompra"]);
                        var idPedidoCompra = string.IsNullOrEmpty(Request["numPedido"]) ? 0 : Glass.Conversoes.StrParaUint(Request["numPedido"]);
                        var idFornecCompra = string.IsNullOrEmpty(Request["idFornec"]) ? 0 : Glass.Conversoes.StrParaUint(Request["idFornec"]);
                        var situacao = string.IsNullOrEmpty(Request["situacao"]) ? 0 : Glass.Conversoes.StrParaInt(Request["situacao"]);
                        var compras = CompraDAO.Instance.GetListForRpt(idCompra, idPedidoCompra, Request["nfPedido"], idFornecCompra, Request["nomeFornec"],
                            Request["obs"], situacao, Request["emAtraso"] == "true", Request["dataIni"], Request["dataFim"], Request["dataFabIni"],
                            Request["dataFabFim"], Request["dataSaidaIni"], Request["dataSaidaFim"], Request["dataFinIni"], Request["dataFinFim"],
                            Request["dataEntIni"], Request["dataEntFim"], Request["idsGrupoProd"], Glass.Conversoes.StrParaUint(Request["idSubgrupoProd"]),
                            Request["codProd"], Request["descrProd"], Request["centroCustoDivergente"].ToLower() == "true", Request["idLoja"].StrParaInt());

                        lstParam.Add(new ReportParameter("AgruparPorFornecedor", (Request["agruparPorFornecedor"].ToLower() == "true").ToString()));

                        report.DataSources.Add(new ReportDataSource("Compra", compras.ToArray()));

                        break;
                    }
                case "ListaPedidos":
                case "ListaPedidosRota":
                case "ListaPedidosProd":
                case "ListaPedidosSimples":
                    {
                        report.ReportPath = Request["rel"] == "ListaPedidosProd" ? "Relatorios/rptListaPedidosProd.rdlc" :
                            Request["rel"] == "ListaPedidosSimples" ? "Relatorios/rptListaPedidosSimples.rdlc" :
                            Request["rel"] == "ListaPedidosRota" ? "Relatorios/rptListaPedidosRota.rdlc" :
                            PedidoConfig.RelatorioListaPedidos.ExibirRelatorioListaPedidosPaisagem;

                        if ((report.ReportPath.Contains("ListaPedidos.rdlc") || report.ReportPath.Contains("ListaPedidosPaisagem.rdlc") ||
                            report.ReportPath.Contains("ListaPedidosRota")) && Request["exibirPronto"] == "true")
                        {
                            report.ReportPath = report.ReportPath.Replace(".", "Pronto.");
                            lstParam.Add(new ReportParameter("LiberarPedido", PedidoConfig.LiberarPedido.ToString()));
                        }

                        var agrupar = Request["agrupar"];

                        Glass.Data.Model.Pedido[] pedidos;

                        if (Request["simples"] == "true")
                            pedidos = PedidoDAO.Instance.PesquisarRelatorioVendasPedidosSimples(Request["dtFimEnt"], Request["dtIniEnt"], Request["idsRota"]);
                        else
                            pedidos = PedidoDAO.Instance.PesquisarRelatorioVendasPedidos(Request["altura"].StrParaFloat(), Request["cidade"].StrParaInt(), Request["codCliente"], Request["codProd"],
                                Request["comSemNf"], Request["dtFimEnt"], Request["dataFimInst"], Request["dtFim"], Request["dataFimPronto"], Request["dtFimSit"], Request["dtIniEnt"],
                                Request["dataIniInst"], Request["dtIni"], Request["dataIniPronto"], Request["dtIniSit"], Request["desconto"].StrParaInt(), Request["descrProd"],
                                Request["exibirProdutos"] == "true", Request["rel"] == "ListaPedidosRota" ? true : false, Request["fastDelivery"].StrParaInt(), Request["idCarregamento"].StrParaInt(), Request["idCli"],
                                Request["idFunc"].StrParaInt(), Request["idMedidor"].StrParaInt(), Request["idOC"].StrParaInt(), Request["IdOrcamento"].StrParaInt(), Request["IdPedido"].StrParaInt(),
                                Request["idsBenef"], Request["idsGrupos"], Request["idsPedidos"], Request["idsRota"], Request["idsSubgrupoProd"], Request["idVendAssoc"].StrParaInt(),
                                Request["largura"].StrParaInt(), login, Request["loja"], Request["nomeCli"], Request["diasDifProntoLib"].StrParaInt(), Request["observacao"],
                                Request["ordenacao"].StrParaInt(), Request["origemPedido"].StrParaInt(), Request["pedidosSemAnexos"] == "true", Request["situacao"], Request["situacaoProd"],
                                Request["tipo"], Request["tipoCliente"], Request["tipoEntrega"].StrParaInt(), Request["tipoFiscal"].StrParaInt(), Request["tipoVenda"],
                                Request["trazerPedCliVinculado"] == "true", Request["usuCad"].StrParaInt());

                        if (Request["rel"] == "ListaPedidosProd")
                            lstParam.Add(new ReportParameter("FastDelivery", PedidoConfig.Pedido_FastDelivery.FastDelivery.ToString()));

                        if (Request["rel"] != "ListaPedidosSimples")
                        {
                            lstParam.Add(new ReportParameter("Producao", PCPConfig.ControlarProducao.ToString()));
                            lstParam.Add(new ReportParameter("EsconderTotal", (Request["esconderTotal"] == "true").ToString()));

                            if (report.ReportPath == "Relatorios/rptListaPedidosProd.rdlc")
                                lstParam.Add(new ReportParameter("FastDelivery", PedidoConfig.Pedido_FastDelivery.FastDelivery.ToString()));
                        }
                        else
                            agrupar = "1";

                        if (report.ReportPath.Contains("rptListaPedidosPaisagem.rdlc"))
                            lstParam.Add(new ReportParameter("ExibirValorIpi", PedidoConfig.TelaListagemRelatorio.ExibirValorIPI.ToString()));

                        lstParam.Add(new ReportParameter("Agrupar", !string.IsNullOrEmpty(agrupar) ? agrupar : "0"));

                        if (Request["rel"] == "ListaPedidosProd")
                            lstParam.Add(new ReportParameter("ExibirValorCustoVenda", PCPConfig.ExibirCustoVendaRelatoriosProducao.ToString()));

                        /* Chamado 41537. */
                        if (report.ReportPath.Contains("ListaPedidos.rdlc"))
                            lstParam.Add(new ReportParameter("ExibirTotaisVendedorCliente", PedidoConfig.RelatorioListaPedidos.ExibirTotaisVendedorCliente.ToString().ToLower()));

                        report.DataSources.Add(new ReportDataSource("PedidoRpt",
                            PedidoRptDAL.Instance.CopiaLista(pedidos, PedidoRpt.TipoConstrutor.ListaPedidos, Request["mostrarDescontoTotal"] == "true", login)));

                        break;
                    }
                case "Pedidos":
                    {
                        report.ReportPath = "Relatorios/rptListaPedidos.rdlc";
                        var lstPedidosDefault = PedidoDAO.Instance.GetListForRpt(Glass.Conversoes.StrParaUint(Request["idPedido"]), Glass.Conversoes.StrParaUint(Request["idLoja"]),
                            Glass.Conversoes.StrParaUint(Request["idCliente"]), Request["nomeCliente"], Request["codPedCliente"], Glass.Conversoes.StrParaUint(Request["idCidade"]),
                            Request["endereco"], Request["bairro"], Request["complemento"], Request["byVend"], Request["byConf"], Request["maoObra"], Request["maoObraEspecial"], Request["producao"],
                            Glass.Conversoes.StrParaFloat(Request["altura"]), Glass.Conversoes.StrParaInt(Request["largura"]), Glass.Conversoes.StrParaInt(Request["diasProntoLib"]),
                            Glass.Conversoes.StrParaFloat(Request["valorDe"]), Glass.Conversoes.StrParaFloat(Request["valorAte"]), Request["dataCadIni"], Request["dataCadFim"],
                            Glass.Conversoes.StrParaInt(Request["fastDelivery"]), Glass.Conversoes.StrParaInt(Request["tipoVenda"]), Glass.Conversoes.StrParaInt(Request["origemPedido"]), Request["obs"]);

                        lstParam.Add(new ReportParameter("Agrupar", !String.IsNullOrEmpty(Request["agrupar"]) ? Request["agrupar"] : "0"));
                        lstParam.Add(new ReportParameter("Producao", PCPConfig.ControlarProducao.ToString()));
                        lstParam.Add(new ReportParameter("ExibirTotaisVendedorCliente", Configuracoes.PedidoConfig.RelatorioListaPedidos.ExibirTotaisVendedorCliente.ToString().ToLower()));
                        lstParam.Add(new ReportParameter("EsconderTotal", (Request["esconderTotal"] == "true").ToString()));

                        report.DataSources.Add(new ReportDataSource("PedidoRpt", Glass.Data.RelDAL.PedidoRptDAL.Instance.CopiaLista(lstPedidosDefault,
                            PedidoRpt.TipoConstrutor.ListaPedidos, false, login)));

                        break;
                    }
                case "ListaOrcamento":
                    {
                        var trocador = 0;
                        report.ReportPath = "Relatorios/rptListaOrcamentos.rdlc";
                        var orcamentos = OrcamentoDAO.Instance.GetForRptLista(Glass.Conversoes.StrParaUint(Request["IdLoja"]), Glass.Conversoes.StrParaUint(Request["IdVend"]),
                            (Request["Situacao"].Split(',').Select(f => int.TryParse(f, out trocador)).Select(f => trocador)), Request["dtIniSit"], Request["dtFimSit"], Request["DtIni"], Request["DtFim"]);

                        report.DataSources.Add(new ReportDataSource("Orcamento", orcamentos.ToArray()));

                        break;
                    }
                case "ListaMedicao":
                     {
                        report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptListaMedicao{0}.rdlc");
                        var medicoes = MedicaoDAO.Instance.GetListForRpt(Request["IdMedicao"].StrParaUint(), Request["idOrcamento"].StrParaUint(), Request["idPedido"].StrParaUint(),
                            Glass.Conversoes.StrParaUint(Request["IdMedidor"]), Request["NomeMedidor"], Glass.Conversoes.StrParaUint(Request["IdVendedor"]),
                            Glass.Conversoes.StrParaUint(Request["situacao"]), Request["dataIni"], Request["dataFim"], Request["dataEfetuar"], Glass.Conversoes.StrParaInt(Request["idCliente"]), Request["NomeCliente"], Request["bairro"],
                            Request["endereco"], Request["telefone"], Glass.Conversoes.StrParaUint(Request["idLoja"]), Request["obs"], Glass.Conversoes.StrParaInt(Request["ordenarPor"]));

                        lstParam.Add(new ReportParameter("Agrupar", !String.IsNullOrEmpty(Request["agrupar"]) ? Request["agrupar"] : "0"));
                        report.DataSources.Add(new ReportDataSource("Medicao", medicoes.ToArray()));

                        break;
                    }
                case "ListaClientes":
                    {
                        reportName = "ListaClientes";
                        break;
                    }
                case "ListaSugestaoCliente":
                    {
                        reportName = "ListaSugestaoCliente";
                        break;
                    }
                case "ListaFuncionarios":
                    {
                        reportName = "ListaFuncionarios";
                        break;
                    }
                case "ListaFornecedores":
                    {
                        reportName = "ListaFornecedores";
                        break;
                    }
                case "ConsultaRapidaCliente":
                    {
                        report.ReportPath = "Relatorios/rptConsultaRapidaCliente.rdlc";
                        var idCliConsRapida = Glass.Conversoes.StrParaUint(Request["idCli"]);
                        var situacaoCliente = Request["situacao"];
                        //dados cadastrais
                        var clCons = ClienteDAO.Instance.GetElement(idCliConsRapida);
                        //>>nome do vendedor n�o est� chegando correto, ver o que �
                        var clVendedor = clCons.IdFunc == null ? "" : FuncionarioDAO.Instance.GetNome((uint)clCons.IdFunc);
                        //dados financeiros
                        var financLimDisp = clCons.Limite > 0 ? (clCons.Limite - ContasReceberDAO.Instance.GetDebitos((uint)clCons.IdCli, null)).ToString("C") : "";
                        var financPagtoPadrao = FormaPagtoDAO.Instance.GetDescricao((uint)clCons.IdFormaPagto.GetValueOrDefault(0)) ?? "";
                        var parc = ParcelasDAO.Instance.GetPadraoCliente((uint)clCons.IdCli);
                        var financParcPadrao = parc != null ? parc.DescrCompleta : String.Empty;
                        var formasPagtoDisp = FormaPagtoDAO.Instance.GetByCliente((uint)clCons.IdCli);
                        var parcelasDisp = new List<Parcelas>(ParcelasDAO.Instance.GetByClienteFornecedor((uint)clCons.IdCli, 0, false, ParcelasDAO.TipoConsulta.Todos));
                        //pedidos prontos e n�o liberados a x dias
                        var paramPedidosProntos = "Pedidos prontos por�m n�o liberados a " + PedidoConfig.NumeroDiasPedidoProntoAtrasado + " dias";
                        var pedsBloqueioEmissao = PedidoDAO.Instance.GetPedidosBloqueioEmissaoByCliente(idCliConsRapida);
                        //descontos / acr�scimos
                        var descAcresCliente = DescontoAcrescimoClienteDAO.Instance.GetOcorrenciasByCliente(idCliConsRapida);
                        var lstDacProdutos = new List<DescontoAcrescimoCliente>();
                        //d�bitos
                        var debitosCliente = ContasReceberDAO.Instance.GetDebitosList(idCliConsRapida, 0, 0, null, 0, 0, null, null, null, 0, 0);
                        var sugestoes = SugestaoClienteDAO.Instance.GetList(0, idCliConsRapida, 0, null, null, null, null, 0, null, situacaoCliente, null, 0, 0);

                        foreach (var dac in descAcresCliente)
                            lstDacProdutos.AddRange(DescontoAcrescimoClienteDAO.Instance.GetOcorrenciasByClienteGrupoSubgrupo(idCliConsRapida, (uint)dac.IdGrupoProd, (uint)(dac.IdSubgrupoProd ?? 0)));

                        parcelasDisp.RemoveAll(delegate (Parcelas p) { return p.NaoUsar; });

                        lstParam.Add(new ReportParameter("clVendedor", clVendedor));
                        lstParam.Add(new ReportParameter("financLimDisp", financLimDisp));
                        lstParam.Add(new ReportParameter("financPagtoPadrao", financPagtoPadrao));
                        lstParam.Add(new ReportParameter("financParcPadrao", financParcPadrao));
                        lstParam.Add(new ReportParameter("paramPedidosProntos", paramPedidosProntos));

                        report.DataSources.Add(new ReportDataSource("Cliente", new Cliente[] { clCons }));
                        report.DataSources.Add(new ReportDataSource("FormaPagto", formasPagtoDisp.ToArray()));
                        report.DataSources.Add(new ReportDataSource("Parcelas", parcelasDisp.ToArray()));
                        report.DataSources.Add(new ReportDataSource("PedidoRpt", Glass.Data.RelDAL.PedidoRptDAL.Instance.CopiaLista(pedsBloqueioEmissao,
                            PedidoRpt.TipoConstrutor.ListaPedidos, false, login)));
                        report.DataSources.Add(new ReportDataSource("DescontoAcrescimoCliente", descAcresCliente.ToArray()));
                        report.DataSources.Add(new ReportDataSource("DacProdutos", lstDacProdutos.ToArray()));
                        report.DataSources.Add(new ReportDataSource("ContasReceber", debitosCliente.ToArray()));
                        report.DataSources.Add(new ReportDataSource("SugestaoCliente", sugestoes.ToArray()));

                        break;
                    }
                case "SinaisReceber":
                    {
                        report.ReportPath = "Relatorios/rptSinaisReceber.rdlc";
                        var lstSinais = PedidoDAO.Instance.GetSinaisNaoRecebidosRpt(Glass.Conversoes.StrParaUint(Request["idCli"]),
                            Glass.Conversoes.StrParaUint(Request["idPedido"]), Request["pagtoAntecipado"] == "true");

                        report.DataSources.Add(new ReportDataSource("PedidoRpt", Glass.Data.RelDAL.PedidoRptDAL.Instance.CopiaLista(lstSinais, PedidoRpt.TipoConstrutor.ListaPedidos, false, login)));

                        break;
                    }
                case "SinaisRecebidos":
                    {
                        report.ReportPath = "Relatorios/rptSinaisRecebidos.rdlc";
                        var lstSinaisReceb = PedidoDAO.Instance.GetSinaisRecebidosRpt(Glass.Conversoes.StrParaUint(Request["idCli"]), Glass.Conversoes.StrParaUint(Request["idPedido"]),
                            0, Request["dataIni"], Request["dataFim"], Request["pagtoAntecipado"] == "true", Glass.Conversoes.StrParaInt(Request["ordenacao"]));

                        report.DataSources.Add(new ReportDataSource("PedidoRpt", Glass.Data.RelDAL.PedidoRptDAL.Instance.CopiaLista(lstSinaisReceb, PedidoRpt.TipoConstrutor.ListaPedidos, false, login)));

                        break;
                    }
                case "SinaisPagar":
                    {
                        report.ReportPath = "Relatorios/rptSinaisPagar.rdlc";
                        var lstSinaisCompra = CompraDAO.Instance.GetSinaisNaoPagosRpt(Glass.Conversoes.StrParaUint(Request["idFornec"]), Glass.Conversoes.StrParaUint(Request["idCompra"]));

                        report.DataSources.Add(new ReportDataSource("Compra", lstSinaisCompra.ToArray()));

                        break;
                    }
                case "SinaisPagos":
                    {
                        report.ReportPath = "Relatorios/rptSinaisPagos.rdlc";
                        var lstSinaisCompraReceb = CompraDAO.Instance.GetSinaisPagosRpt(Glass.Conversoes.StrParaUint(Request["idFornec"]),
                            Glass.Conversoes.StrParaUint(Request["idCompra"]), 0, Request["dataIni"], Request["dataFim"]);

                        report.DataSources.Add(new ReportDataSource("Compra", lstSinaisCompraReceb.ToArray()));

                        break;
                    }
                case "CustoFixo":
                    {
                        report.ReportPath = "Relatorios/rptCustoFixo.rdlc";
                        var lstPrevisao = new decimal[4];
                        var CustoFixo_idCustoFixo = !String.IsNullOrEmpty(Request["idCustoFixo"]) ? Glass.Conversoes.StrParaUint(Request["idCustoFixo"]) : 0;
                        var lstCustoFixo = CustoFixoDAO.Instance.GetForRpt(CustoFixo_idCustoFixo, Glass.Conversoes.StrParaUint(Request["idLoja"]),
                            Glass.Conversoes.StrParaUint(Request["idFornec"]), Request["nomeFornec"], Glass.Conversoes.StrParaInt(Request["diaVencIni"]),
                            Glass.Conversoes.StrParaInt(Request["diaVencFim"]), Request["descricao"], Glass.Conversoes.StrParaInt(Request["situacao"]), ref lstPrevisao);

                        lstParam.Add(new ReportParameter("Salarios", lstPrevisao[0].ToString()));
                        lstParam.Add(new ReportParameter("Ferias", lstPrevisao[1].ToString()));
                        lstParam.Add(new ReportParameter("DecTerc", lstPrevisao[2].ToString()));
                        lstParam.Add(new ReportParameter("Ipva", lstPrevisao[3].ToString()));

                        report.DataSources.Add(new ReportDataSource("CustoFixo", lstCustoFixo.ToArray()));

                        break;
                    }
                case "Acerto":
                    {
                        report.ReportPath = "Relatorios/rptAcerto.rdlc";
                        var acerto = AcertoDAO.Instance.GetAcertoDetails(Glass.Conversoes.StrParaUint(Request["idAcerto"]));
                        var lstContasReceber = ContasReceberDAO.Instance.GetByAcerto(null, acerto.IdAcerto, false);
                        var lstContasReceberReneg = ContasReceberDAO.Instance.GetRenegByAcerto(acerto.IdAcerto, false);
                        var lstCheque = ChequesDAO.Instance.GetByAcerto(acerto.IdAcerto);

                        lstParam.Add(new ReportParameter("MostrarJurosReneg", true.ToString()));
                        lstParam.Add(new ReportParameter("Liberacao", PedidoConfig.LiberarPedido.ToString()));

                        report.DataSources.Add(new ReportDataSource("Acerto", new Acerto[] { acerto }));
                        report.DataSources.Add(new ReportDataSource("ContasReceber", lstContasReceber.ToArray()));
                        report.DataSources.Add(new ReportDataSource("ContasReceberReneg", lstContasReceberReneg.ToArray()));
                        report.DataSources.Add(new ReportDataSource("Cheques", lstCheque.ToArray()));

                        break;
                    }
                case "ListaAcerto":
                    {
                        report.ReportPath = "Relatorios/rptListaAcerto.rdlc";
                        var lstAcerto = AcertoDAO.Instance.GetListRpt(Glass.Conversoes.StrParaUint(Request["idAcerto"]),
                            Glass.Conversoes.StrParaUint(Request["idPedido"]), Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]),
                            Glass.Conversoes.StrParaUint(Request["idCliente"]), Request["dataIni"], Request["dataFim"],
                            Glass.Conversoes.StrParaUint(Request["idFormaPagto"]), Request["numNotaFiscal"].StrParaInt());

                        report.DataSources.Add(new ReportDataSource("Acerto", lstAcerto.ToArray()));

                        break;
                    }
                case "Pagto":
                    {
                        report.ReportPath = "Relatorios/rptPagto.rdlc";
                        var pagto = PagtoDAO.Instance.GetPagto(Glass.Conversoes.StrParaUint(Request["idPagto"]));
                        var lstChequePagto = ChequesDAO.Instance.GetByPagto(pagto.IdPagto);
                        var lstContaPagto = ContasPagarDAO.Instance.GetByPagto(pagto.IdPagto);
                        var lstContaPagtoReneg = ContasPagarDAO.Instance.GetRenegociadasPagto(pagto.IdPagto);

                        report.DataSources.Add(new ReportDataSource("Pagto", new Pagto[] { pagto }));
                        report.DataSources.Add(new ReportDataSource("Cheques", lstChequePagto.ToArray()));
                        report.DataSources.Add(new ReportDataSource("ContasPagar", lstContaPagto));
                        report.DataSources.Add(new ReportDataSource("ContasPagarReneg", lstContaPagtoReneg));

                        break;
                    }
                case "Compra":
                    {
                        report.ReportPath = FinanceiroConfig.FinanceiroRec.ImprimirCompraComBenef ? "Relatorios/rptCompraComBenef.rdlc" :
                            Glass.Configuracoes.Geral.NaoVendeVidro() ? "Relatorios/rptCompraNaoVendeVidro.rdlc" : string.Empty; ;

                        if (string.IsNullOrEmpty(report.ReportPath))
                            report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptCompra{0}.rdlc");

                        var compra = CompraDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idCompra"]));
                        var lstProdCompra = ProdutosCompraDAO.Instance.GetForRpt(compra.IdCompra);

                        lstParam.Add(new ReportParameter("NaoExibirValor", compra.IsCompraSemValores.ToString()));

                        if (FinanceiroConfig.FinanceiroRec.ImprimirCompraComBenef)
                            lstParam.Add(new ReportParameter("AgruparBeneficiamentos", PedidoConfig.RelatorioPedido.AgruparBenefRelatorio.ToString()));

                        report.DataSources.Add(new ReportDataSource("Compra", new Compra[] { compra }));
                        report.DataSources.Add(new ReportDataSource("ParcelasCompra", ParcelasCompraDAO.Instance.GetByCompra(compra.IdCompra)));
                        report.DataSources.Add(new ReportDataSource("ProdutosCompra", lstProdCompra));

                        break;
                    }
                case "ListaProducao":
                    {
                        report.ReportPath = "Relatorios/rptListaProducao.rdlc";
                        var lstProducao = PedidoDAO.Instance.GetForCorteRpt(Glass.Conversoes.StrParaUint(Request["idPedido"]), Request["dtIni"],
                            Request["dtFim"], Glass.Conversoes.StrParaInt(Request["situacao"]));

                        report.DataSources.Add(new ReportDataSource("Pedido", lstProducao));

                        break;
                    }
                case "ListaCheque":
                    {
                        report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptCheques{0}.rdlc");
                        var idCli = !String.IsNullOrEmpty(Request["idCli"]) ? Glass.Conversoes.StrParaUint(Request["idCli"]) : 0;
                        var idFornec = !String.IsNullOrEmpty(Request["idFornec"]) ? Glass.Conversoes.StrParaUint(Request["idFornec"]) : 0;
                        var valorInicial = !String.IsNullOrEmpty(Request["valorInicial"]) ?
                            Glass.Conversoes.StrParaFloat(Request["valorInicial"]) : 0;
                        var valorFinal = !String.IsNullOrEmpty(Request["valorFinal"]) ?
                            Glass.Conversoes.StrParaFloat(Request["valorFinal"]) : 0;
                        var lstCheques = ChequesDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idLoja"]),
                            Glass.Conversoes.StrParaUint(Request["idPedido"]),
                            Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]), Glass.Conversoes.StrParaUint(Request["idAcerto"]),
                            Glass.Conversoes.StrParaUint(Request["numeroNfe"]),
                            Glass.Conversoes.StrParaInt(Request["tipo"]), Glass.Conversoes.StrParaInt(Request["NumCheque"]),
                            Request["Situacao"], Request["reapresentado"] == "true", Glass.Conversoes.StrParaInt(Request["advogado"]),
                            Request["titular"], Request["agencia"], Request["conta"], Request["dataIni"], Request["dataFim"],
                            Request["dataCadIni"], Request["dataCadFim"], Request["cpfCnpj"], idCli, Request["nomeCli"], idFornec,
                            Request["nomeFornec"], valorInicial, valorFinal, Request["nomeUsuCad"], Request["idsRotas"],
                            Request["chequesCaixaDiario"] == "true", Request["ordenacao"], Request["obs"]);

                        lstParam.Add(new ReportParameter("LiberarPedido", PedidoConfig.LiberarPedido.ToString()));
                        lstParam.Add(new ReportParameter("Agrupar", (Request["agrupar"] == "true").ToString()));
                        lstParam.Add(new ReportParameter("ExibirTotalDevolvido", (Request["situacao"] == ((int)Cheques.SituacaoCheque.Devolvido).ToString()).ToString()));

                        report.DataSources.Add(new ReportDataSource("Cheques", lstCheques.ToArray()));

                        break;
                    }
                case "ListaConferencia":
                    {
                        report.ReportPath = "Relatorios/rptListaConferencia.rdlc";
                        var lstConferencia = PedidoConferenciaDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idPedido"]),
                            Glass.Conversoes.StrParaUint(Request["idConferente"]), Glass.Conversoes.StrParaUint(Request["IdLoja"]),
                            Request["NomeCliente"], Glass.Conversoes.StrParaInt(Request["Situacao"]), Glass.Conversoes.StrParaInt(Request["SitPedido"]),
                            Request["dataConferencia"], Request["dataFinalIni"], Request["dataFinalFim"]);

                        report.DataSources.Add(new ReportDataSource("PedidoConferencia", lstConferencia.ToArray()));

                        break;
                    }
                case "ListaOrcamentoSite":
                    {
                        report.ReportPath = "Relatorios/rptListaOrcamentoSite.rdlc";
                        var lstOrcaSite = OrcamentoSiteDAO.Instance.GetForRpt(0, Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaInt(Request["Emitido"]));

                        report.DataSources.Add(new ReportDataSource("OrcamentoSite", lstOrcaSite.ToArray()));

                        break;
                    }
                case "ListaOrdemInst":
                    {
                        report.ReportPath = "Relatorios/rptListaOrdemInst.rdlc";
                        var lstOrdemInst = InstalacaoDAO.Instance.GetForRpt(0, Glass.Conversoes.StrParaUint(Request["IdOrdemInst"]), 0, Glass.Conversoes.StrParaUint(Request["idEquipe"]),
                            null, "2", null, null, null, null, null, null, 0, null, null, null);

                        report.DataSources.Add(new ReportDataSource("Instalacao", lstOrdemInst.ToArray()));

                        break;
                    }
                case "ListaInstalacao":
                    {
                        report.ReportPath = "Relatorios/rptListaInstalacao.rdlc";
                        var lstInstalacao = InstalacaoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["IdPedido"]), Glass.Conversoes.StrParaUint(Request["IdOrdemInst"]),
                            Glass.Conversoes.StrParaUint(Request["idOrcamento"]), Glass.Conversoes.StrParaUint(Request["idEquipe"]), Request["tiposInstalacao"], Request["situacoes"],
                            Request["dataIni"], Request["dataFim"], Request["dataIniEnt"], Request["dataFimEnt"], Request["dataIniOrdemInst"], Request["dataFimOrdemInst"],
                            Glass.Conversoes.StrParaUint(Request["idLoja"]), Request["nomeCliente"], Request["telefone"], Request["observacao"]);

                        lstParam.Add(new ReportParameter("Agrupar", Request["agrupar"]));
                        lstParam.Add(new ReportParameter("ExibirValorProdutos", PedidoConfig.Instalacao.ExibirValorProdutosInstalacao.ToString()));

                        report.DataSources.Add(new ReportDataSource("Instalacao", lstInstalacao.ToArray()));

                        break;
                    }
                case "Deposito":
                    {
                        report.ReportPath = "Relatorios/rptDeposito.rdlc";

                        var deposito = DepositoChequeDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["IdDeposito"]));
                        var lstChequesDeposito = ChequesDAO.Instance.GetByDeposito(deposito.IdDeposito);
                        var lstChequesDepositoDev = ChequesDAO.Instance.GetByDepositoDev(deposito.IdDeposito);
                        var ordemCheque = Request["ordemCheque"];

                        lstParam.Add(new ReportParameter("OrdemCheque", !String.IsNullOrEmpty(ordemCheque) ? ordemCheque : "1"));

                        report.DataSources.Add(new ReportDataSource("DepositoCheque", new DepositoCheque[] { deposito }));
                        report.DataSources.Add(new ReportDataSource("Cheques", lstChequesDeposito.ToArray()));
                        report.DataSources.Add(new ReportDataSource("ChequesDev", lstChequesDepositoDev.ToArray()));

                        break;
                    }
                case "ListaDeposito":
                    {
                        report.ReportPath = "Relatorios/rptListaDeposito.rdlc";
                        var lstDeposito = DepositoChequeDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["IdDeposito"]),
                            Glass.Conversoes.StrParaUint(Request["IdContaBanco"]), Request["dataIni"], Request["dataFim"]);

                        report.DataSources.Add(new ReportDataSource("DepositoCheque", lstDeposito.ToArray()));

                        break;
                    }
                case "ListaPlanoContas":
                    {
                        report.ReportPath = "Relatorios/rptListaPlanoContas{0}.rdlc";

                        if (bool.Parse(Request["mes"]))
                            report.ReportPath = String.Format(report.ReportPath, "Mes");
                        else if (bool.Parse(Request["ajustado"]))
                            report.ReportPath = String.Format(report.ReportPath, "Ajustado");
                        else
                            report.ReportPath = String.Format(report.ReportPath, "");

                        uint trocador = 0;
                        var idsPlanoConta = (Request["idsPlanoConta"].Split(',')).Select(f => uint.TryParse(f, out trocador)).Select(f => trocador).Where(f => f > 0).ToArray();

                        var lstPlanoContas = Glass.Data.RelDAL.PlanoContasDAO.Instance.GetForRpt(
                            Glass.Conversoes.StrParaUint(Request["IdCategoriaConta"]), Glass.Conversoes.StrParaUint(Request["IdGrupoConta"]), idsPlanoConta,
                            Glass.Conversoes.StrParaUint(Request["idLoja"]), Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaInt(Request["tipoMov"]),
                            Glass.Conversoes.StrParaInt(Request["tipoConta"]), bool.Parse(Request["ajustado"]), bool.Parse(Request["exibirChequeDevolvido"]),
                            bool.Parse(Request["mes"]), Request["ordenar"].StrParaInt());

                        decimal receita = 0;
                        decimal despesaVariavel = 0;
                        decimal despesaFixa = 0;
                        decimal lucroLiquido = 0;
                        decimal imc = 0;
                        decimal pontoEquilibrio = 0;

                        foreach (var p in lstPlanoContas)
                        {
                            if (p.TipoCategoria == (int)TipoCategoriaConta.Receita)
                                receita += p.Valor;
                            else if (p.TipoCategoria == (int)TipoCategoriaConta.DespesaVariavel)
                                despesaVariavel += p.Valor;
                            else if (p.TipoCategoria == (int)TipoCategoriaConta.DespesaFixa)
                                despesaFixa += p.Valor;
                        }

                        lstParam.Add(new ReportParameter("Ajustado", Request["ajustado"]));

                        if (!bool.Parse(Request["ajustado"]))
                        {
                            imc = receita - despesaVariavel;
                            var imcPercent = decimal.Round(((imc * 100) / (receita > 0 ? receita : 1)) / 100, 2);
                            lucroLiquido = imc - despesaFixa;

                            //PE (R$) = Custo fixo / IMC (�ndice Margem Contribui��o)
                            //Ex: PE (R$) = R$ 7.500 / 0,35 = R$ 21.428,57
                            pontoEquilibrio = decimal.Round((despesaFixa / (imcPercent == 0 ? 1 : imcPercent)), 2);

                            if (!bool.Parse(Request["mes"]))
                            {
                                lstParam.Add(new ReportParameter("ValorReceita", receita.ToString()));
                                lstParam.Add(new ReportParameter("PontoEquilibrio", String.Format("{0:N2}", pontoEquilibrio)));
                                lstParam.Add(new ReportParameter("IMC", String.Format("{0:N2}", imc)));
                            }
                        }

                        report.DataSources.Add(new ReportDataSource("PlanoContas", lstPlanoContas));

                        break;
                    }
                case "ListaPlanoContasDet":
                    {
                        report.ReportPath = "Relatorios/rptListaPlanoContasDet" + (Request["agruparDetalhes"].ToLower() == "true" ? "Agrupar" : "") + ".rdlc";
                        var lstPlanoContasDet = Glass.Data.RelDAL.PlanoContasDAO.Instance.GetForRptDetalhes(
                            Glass.Conversoes.StrParaUint(Request["IdCategoriaConta"]), Glass.Conversoes.StrParaUint(Request["IdGrupoConta"]), Glass.Conversoes.StrParaUint(Request["IdPlanoConta"]),
                            Glass.Conversoes.StrParaUint(Request["idLoja"]), Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaInt(Request["tipoMov"]),
                            Glass.Conversoes.StrParaInt(Request["tipoConta"]), Request["ajustado"].ToLower() == "true",
                            Request["exibirChequeDevolvido"].ToLower() == "true", Request["ordenar"].StrParaInt());

                        lstParam.Add(new ReportParameter("Ajustado", Request["ajustado"]));
                        lstParam.Add(new ReportParameter("Criterio", lstPlanoContasDet.Length > 0 ? lstPlanoContasDet[0].Criterio : "."));

                        report.DataSources.Add(new ReportDataSource("PlanoContas", lstPlanoContasDet));

                        break;
                    }
                case "ListaPlanoContasDesc":
                    {
                        Situacao auxiliar;

                        report.ReportPath = "Relatorios/rptListaPlanoContasDesc.rdlc";
                        var lstPlanoContasDesc = Glass.Data.DAL.PlanoContasDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idGrupo"]),
                            Enum.TryParse<Situacao>(Request["situacao"], out auxiliar) ? (Situacao?)auxiliar : null);

                        report.DataSources.Add(new ReportDataSource("PlanoContas", lstPlanoContasDesc.ToArray()));

                        break;
                    }
                case "ProducaoInst":
                    {
                        report.ReportPath = "Relatorios/rptProducaoInst" + (Request["exibirProdutos"] == "true" ? "Detalhada.rdlc" : ".rdlc");

                        if (Request["exibirProdutos"] == "true")
                        {
                            var prodInst = ProducaoInstDAO.Instance.GetProdInst(Glass.Conversoes.StrParaUint(Request["idEquipe"]),
                                Glass.Conversoes.StrParaInt(Request["tipoInstalacao"]), Request["dataIni"], Request["dataFim"]);

                            report.DataSources.Add(new ReportDataSource("ProducaoInst", prodInst));
                        }
                        else
                        {
                            var lstProducaoInst = Glass.Data.RelDAL.ProducaoInstDAO.Instance.GetProducaoInst(Glass.Conversoes.StrParaUint(Request["idEquipe"]),
                                Glass.Conversoes.StrParaInt(Request["tipoInstalacao"]), Request["dataIni"], Request["dataFim"]);

                            report.DataSources.Add(new ReportDataSource("ProducaoInst", lstProducaoInst));
                        }

                        break;
                    }
                case "ExtratoBancario":
                    {
                        report.ReportPath = "Relatorios/rptExtratoBancario.rdlc";
                        var lstMovBanco = MovBancoDAO.Instance.GetMovimentacoes(Glass.Conversoes.StrParaUint(Request["idContaBanco"]),
                            Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaFloat(Request["valorInicial"]),
                            Glass.Conversoes.StrParaFloat(Request["valorFinal"]), Glass.Conversoes.StrParaInt(Request["tipoMov"]),
                            Request["lancManual"] == "true");

                        lstParam.Add(new ReportParameter("ExibirLinhaVermelhaSaida", "true"));
                        lstParam.Add(new ReportParameter("EsconderSaldo", (Glass.Conversoes.StrParaUint(Request["idContaBanco"]) == 0).ToString())); // Esconde coluna saldo se n�o houver filtro por conta banc�ria

                        report.DataSources.Add(new ReportDataSource("MovBanco", lstMovBanco));

                        break;
                    }
                case "EquipesInst":
                    {
                        report.ReportPath = "Relatorios/rptEquipesInst.rdlc";
                        var lstEquipesInst = EquipeDAO.Instance.GetRpt(Glass.Conversoes.StrParaInt(Request["tipo"]));

                        report.DataSources.Add(new ReportDataSource("Equipe", lstEquipesInst.ToArray()));

                        break;
                    }
                case "DeficitEstoque":
                    {
                        report.ReportPath = "Relatorios/rptDeficitEstoque.rdlc";
                        var lstDeficit = ProdutoDAO.Instance.GetRptDeficit(Request["codInterno"], Request["Descricao"], Glass.Conversoes.StrParaUint(Request["idGrupo"]),
                            Glass.Conversoes.StrParaUint(Request["idSubGrupo"]));

                        report.DataSources.Add(new ReportDataSource("Produto", lstDeficit.ToArray()));

                        break;
                    }
                case "EstoqueMinimo":
                    {
                        report.ReportPath = "Relatorios/rptEstoqueMinimo.rdlc";
                        var lstMinimo = ProdutoLojaDAO.Instance.GetEstoqueMinRpt(Glass.Conversoes.StrParaUint(Request["idLoja"]), Request["codInterno"], Request["Descricao"],
                            Glass.Conversoes.StrParaUint(Request["idGrupo"]), Glass.Conversoes.StrParaUint(Request["idSubGrupo"]), Request["abaixoEstMin"] == "true",
                            Glass.Conversoes.StrParaUintNullable(Request["idCorVidro"]), Glass.Conversoes.StrParaUintNullable(Request["idCorFerragem"]),
                            Glass.Conversoes.StrParaUintNullable(Request["idCorAluminio"]), Request["tipoBox"]);

                        report.DataSources.Add(new ReportDataSource("ProdutoLoja", lstMinimo));

                        break;
                    }
                case "ProdutoProjeto":
                    {
                        report.ReportPath = "Relatorios/rptProdutoProjeto.rdlc";
                        var lstProdProj = ProdutoProjetoDAO.Instance.GetList(Request["codInterno"], Request["Descricao"], Request["codInternoAssoc"], Request["descricaoAssoc"],
                            Glass.Conversoes.StrParaInt(Request["tipo"]), null, 0, 300);

                        report.DataSources.Add(new ReportDataSource("ProdutoProjeto", lstProdProj.ToArray()));

                        break;
                    }
                case "Comissao":
                    {
                        report.ReportPath = "Relatorios/rptComissao.rdlc";

                        var comissaoContasReceber = !string.IsNullOrEmpty(Request["contasRecebidas"]) ? bool.Parse(Request["contasRecebidas"]) : false;

                        var lstComissao = !comissaoContasReceber ? ComissaoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["tipoFunc"]), Glass.Conversoes.StrParaUint(Request["idFunc"]),
                            Glass.Conversoes.StrParaUint(Request["idPedido"]), Request["dataIni"], Request["dataFim"]) :
                            ComissaoDAO.Instance.GetForRptContasRecebidas(Request["idComissao"].StrParaUint(), Request["idFunc"].StrParaUint(), Request["idLoja"].StrParaUint(), Request["dataIni"], Request["dataFim"]);

                        report.DataSources.Add(new ReportDataSource("Comissao", lstComissao.ToArray()));

                        break;
                    }
                case "ComissaoDetalhada":
                case "RecibosComissao":
                    {
                        report.ReportPath = "Relatorios/rptComissaoDetalhada.rdlc";
                        var idsComissao = new List<uint>();
                        var exibirComissao = false;

                        if (Request["rel"] == "ComissaoDetalhada")
                            idsComissao.Add(Glass.Conversoes.StrParaUint(Request["idComissao"]));
                        else
                            idsComissao.AddRange(ComissaoDAO.Instance.GetForRptRecibos(Glass.Conversoes.StrParaUint(Request["tipoFunc"]), Glass.Conversoes.StrParaUint(Request["idFunc"]),
                                Glass.Conversoes.StrParaUint(Request["idPedido"]), Request["dataIni"], Request["dataFim"]));

                        var lstComissaoDetalhada = ComissaoPedidoDAO.Instance.GetComissaoDetalhada(idsComissao.ToArray());

                        if (lstComissaoDetalhada[0].TipoFunc == 0)
                            exibirComissao = ComissaoConfigDAO.Instance.IsFaixaUnica(lstComissaoDetalhada[0].IdFunc);
                        else
                            exibirComissao = true;

                        // Insere na vari�vel idLojaLogotipo o id da loja do pedido da primeira comiss�o da lista, para fazer a compara��o de lojas
                        idLojaLogotipo = PedidoDAO.Instance.ObtemIdLoja(lstComissaoDetalhada[0].IdPedido);

                        // Verifica se a loja de todos os pedidos � a mesma, caso seja, ent�o a logomarca a ser buscada ser� a logomarca desta loja
                        // caso contr�rio ser� buscada a logomarca da loja do funcion�rio que gerou a comiss�o.
                        foreach (var comissao in lstComissaoDetalhada)
                            if (idLojaLogotipo != PedidoDAO.Instance.ObtemIdLoja(comissao.IdPedido))
                            {
                                idLojaLogotipo = FuncionarioDAO.Instance.ObtemIdLoja(comissao.IdFunc);
                                break;
                            }

                        lstParam.Add(new ReportParameter("NomeLoja", login.NomeLoja));
                        lstParam.Add(new ReportParameter("LiberarPedido", PedidoConfig.LiberarPedido.ToString()));
                        lstParam.Add(new ReportParameter("exibirComissao", exibirComissao.ToString()));
                        lstParam.Add(new ReportParameter("ExibirObs", Configuracoes.ComissaoConfig.ExibirObsComissionadoOuFuncionarioRelatorioComissao.ToString()));

                        report.DataSources.Add(new ReportDataSource("ComissaoDetalhada", Glass.Data.RelDAL.ComissaoDetalhadaDAO.Instance.GetByTipo(
                            Request["rel"] == "ComissaoDetalhada" ? lstComissaoDetalhada[0].IdFunc : 0, lstComissaoDetalhada[0].TipoFunc)));
                        report.DataSources.Add(new ReportDataSource("ComissaoPedido", lstComissaoDetalhada));
                        report.DataSources.Add(new ReportDataSource("DebitoComissao", DebitoComissaoDAO.Instance.GetForRpt(idsComissao.ToArray())));

                        break;
                    }
                case "ComissaoContasRecebidas":
                    {
                        report.ReportPath = "Relatorios/rptComissaoContasRecebidas.rdlc";

                        var lstContasRecebidas = ContasReceberDAO.Instance.GetContasRecebidasByComissao(Request["idComissao"].StrParaUint(), Request["idFunc"].StrParaUint(), true);
                        var lstComissaoDetalhada = Glass.Data.RelDAL.ComissaoDetalhadaDAO.Instance.GetByFuncionario(Request["idFunc"].StrParaUint());

                        var periodo = ComissaoDAO.Instance.ObtemPeriodo(Request["idComissao"].StrParaUint());
                        var periodoRec = ComissaoDAO.Instance.ObtemPeriodoRec(Request["idComissao"].StrParaUint());

                        idLojaLogotipo = ComissaoDAO.Instance.ObtemIdLoja(Request["idComissao"].StrParaUint());

                        lstParam.Add(new ReportParameter("Periodo", periodo));
                        lstParam.Add(new ReportParameter("PeriodoRec", periodoRec));

                        lstParam.Add(new ReportParameter("IdComissao", Request["idComissao"]));

                        report.DataSources.Add(new ReportDataSource("ContasRecebidas", lstContasRecebidas));
                        report.DataSources.Add(new ReportDataSource("ComissaoDetalhada", lstComissaoDetalhada));

                        break;
                    }
                case "PedidoPcp":
                    {
                        report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/ModeloPedidoPcp/rptPedidoPcp{0}.rdlc");

                        var pedido = PedidoDAO.Instance.ObterPedidosPorIdsPedidoParaImpressaoPcp(null, Request["idPedido"]);
                        var pedidoEspelho = PedidoEspelhoDAO.Instance.GetByIds(Request["idPedido"]);

                        #region Vari�veis criadas para salvar dados dos pedidos

                        var pedidoAcrescimos = new string[pedido.Length];
                        var pedidoDatasEntrega = new string[pedido.Length];
                        var pedidoDatasFabrica = new string[pedido.Length];
                        var pedidoDatasPedido = new string[pedido.Length];
                        var pedidoDescontos = new string[pedido.Length];
                        var pedidoObservacoes = new string[pedido.Length];
                        var pedidoRotasCliente = new string[pedido.Length];
                        var pedidoTiposEntrega = new string[pedido.Length];
                        var pedidoCidadesUf = new string[pedido.Length];
                        var pedidoContatosCliente = new string[pedido.Length];
                        var pedidoNomesCliente = new string[pedido.Length];
                        var pedidoCodsCliente = new string[pedido.Length];
                        var pedidoNomesFunc = new string[pedido.Length];
                        var pedidoLocalizacoesObra = new string[pedido.Length];
                        var pedidoBairros = new string[pedido.Length];
                        var pedidoCeps = new string[pedido.Length];
                        var pedidoTelefonesContatoCliente = new string[pedido.Length];
                        var pedidoEnderecos = new string[pedido.Length];
                        var pedidoCpfsCnpjs = new string[pedido.Length];
                        var pedidoRgsInscEsts = new string[pedido.Length];

                        #endregion

                        var dicProdutos = new Dictionary<uint, uint>();
                        var strProds = Request["produtos"] != null ? Request["produtos"].Split(',') : null;
                        var prods = string.Empty;
                        var agruparProdutos = false;

                        /* Chamado 55724. */
                        if (pedido.Count() != pedidoEspelho.Count())
                            throw new Exception("Um ou mais pedidos impressos n�o possuem confer�ncia gerada.");

                        #region Preenche propriedades dos pedidos em vari�veis

                        for (var i = 0; i < pedido.Length; i++)
                        {
                            pedidoAcrescimos[i] = pedidoEspelho[i].AcrescimoTotal.ToString();
                            pedidoDatasEntrega[i] = string.IsNullOrWhiteSpace(pedido[i].DataEntregaExibicao) ? "." : pedido[i].DataEntregaExibicao;
                            pedidoDatasFabrica[i] = pedidoEspelho[i].DataFabrica == null ? "." : pedidoEspelho[i].DataFabrica.Value.ToShortDateString();
                            pedidoDatasPedido[i] = string.IsNullOrWhiteSpace(pedido[i].DataPedidoString) ? "." : pedido[i].DataPedidoString;
                            pedidoDescontos[i] = pedidoEspelho[i].DescontoTotal.ToString();
                            pedidoObservacoes[i] = string.IsNullOrWhiteSpace(pedidoEspelho[i].Obs) ? (string.IsNullOrWhiteSpace(pedido[i].Obs) ? "." : pedido[i].Obs) : pedidoEspelho[i].Obs;
                            pedidoRotasCliente[i] = string.IsNullOrWhiteSpace(pedido[i].RptRotaCliente) ? "." : pedido[i].RptRotaCliente;
                            pedidoTiposEntrega[i] = string.IsNullOrWhiteSpace(pedido[i].DescrTipoEntrega) ? "." : pedido[i].DescrTipoEntrega;
                            pedidoCidadesUf[i] = string.IsNullOrWhiteSpace(pedido[i].RptCidade) || string.IsNullOrWhiteSpace(pedido[i].RptUf) ? "." : string.Format("{0}/{1}", pedido[i].RptCidade, pedido[i].RptUf);
                            pedidoContatosCliente[i] = string.IsNullOrWhiteSpace(pedido[i].ContatoCliente) ? "." : pedido[i].ContatoCliente;
                            pedidoNomesCliente[i] = string.IsNullOrWhiteSpace(pedido[i].NomeCliente) ? "." : pedido[i].NomeCliente;
                            pedidoCodsCliente[i] = string.IsNullOrWhiteSpace(pedido[i].CodCliente) ? "." : pedido[i].CodCliente;
                            pedidoNomesFunc[i] = string.IsNullOrWhiteSpace(pedido[i].NomeFunc) ? "." : pedido[i].NomeFunc;
                            pedidoLocalizacoesObra[i] = string.IsNullOrWhiteSpace(pedido[i].LocalizacaoObra) ? "." : pedido[i].LocalizacaoObra;
                            pedidoBairros[i] = string.IsNullOrWhiteSpace(pedido[i].RptBairro) ? "." : pedido[i].RptBairro;
                            pedidoCeps[i] = string.IsNullOrWhiteSpace(pedido[i].RptCep) ? "." : pedido[i].RptCep;
                            pedidoTelefonesContatoCliente[i] = string.IsNullOrWhiteSpace(pedido[i].RptTelContCli) ? "." : pedido[i].RptTelContCli;

                            pedidoEnderecos[i] = string.Format("{0}{1}{2}", string.IsNullOrWhiteSpace(pedido[i].RptEndereco) ? "." : pedido[i].RptEndereco,
                                string.IsNullOrWhiteSpace(pedido[i].RptNumero) ? "." : pedido[i].RptNumero,
                                string.IsNullOrWhiteSpace(pedido[i].RptCompl) ? string.Empty : pedido[i].RptCompl);

                            pedidoCpfsCnpjs[i] = string.IsNullOrWhiteSpace(pedido[i].RptCpfCnpj) ? "." : pedido[i].RptCpfCnpj;
                            pedidoRgsInscEsts[i] = string.IsNullOrWhiteSpace(pedido[i].RptRgEscinst) ? "." : pedido[i].RptRgEscinst;
                        }

                        #endregion

                        if (strProds != null && strProds.Length > 0 && !string.IsNullOrEmpty(strProds[0]))
                        {
                            foreach (var prod in strProds)
                            {
                                if (string.IsNullOrEmpty(prod))
                                    continue;

                                if (prod.Split('_').Length > 1)
                                    dicProdutos.Add(Glass.Conversoes.StrParaUint(prod.Split('_')[0]), Glass.Conversoes.StrParaUint(prod.Split('_')[1]));
                                else
                                    dicProdutos.Add(Glass.Conversoes.StrParaUint(prod.Split('_')[0]), 0);

                                prods += prod.Split('_')[0] + ",";
                            }

                            if (!string.IsNullOrEmpty(prods))
                                prods = prods.Remove(prods.Length - 1);
                        }


                        if (!string.IsNullOrEmpty(Request["agruparProdutos"]))
                            agruparProdutos = bool.Parse(Request["agruparProdutos"]);

                        var lstProdPed = ProdutosPedidoEspelhoDAO.Instance.GetForRptPcp(Request["idPedido"], Request["grupos"], prods, agruparProdutos);

                        if (dicProdutos.Count > 0)
                        {
                            for (var i = 0; i < lstProdPed.Length; i++)
                            {
                                if (lstProdPed[i].IdProdPed == 0)
                                    continue;

                                if (lstProdPed[i].IdProdPed > 0 && dicProdutos[lstProdPed[i].IdProdPed] > 0)
                                {
                                    lstProdPed[i].Qtde = dicProdutos[lstProdPed[i].IdProdPed];
                                    lstProdPed[i].QtdeSomada = dicProdutos[lstProdPed[i].IdProdPed];

                                    var totM2 = Glass.Global.CalculosFluxo.ArredondaM2(lstProdPed[i].Largura, Convert.ToInt32(lstProdPed[i].Altura),
                                        Convert.ToSingle(lstProdPed[i].QtdeSomada), (int)lstProdPed[i].IdProd, lstProdPed[i].Redondo);

                                    lstProdPed[i].TotMSomada = totM2;
                                    lstProdPed[i].TotM = totM2;

                                    lstProdPed[i].Peso = Data.Helper.Utils.CalcPeso((int)lstProdPed[i].IdProd, lstProdPed[i].Espessura, Convert.ToSingle(lstProdPed[i].TotMSomada),
                                    Convert.ToSingle(lstProdPed[i].QtdeSomada), lstProdPed[i].Altura, false);


                                    if (!string.IsNullOrEmpty(lstProdPed[i].Etiquetas))
                                    {
                                        var qtde = Convert.ToInt32(lstProdPed[i].QtdeSomada);
                                        var etiquetas = new List<string>(lstProdPed[i].Etiquetas.Split(','));
                                        etiquetas.RemoveRange(qtde, etiquetas.Count - qtde);
                                        lstProdPed[i].Etiquetas = string.Join(",", etiquetas.ToArray());
                                    }
                                }
                            }
                        }

                        if (!report.ReportPath.Contains("rptPedidoPcpOP.rdlc") && !report.ReportPath.Contains("rptPedidoPcpCmv.rdlc") && !report.ReportPath.Contains("rptPedidoPcpVintage.rdlc") &&
                            !report.ReportPath.Contains("rptPedidoPcpRV.rdlc") && !report.ReportPath.Contains("rptPedidoPcpVitralVarejo.rdlc"))
                        {
                            var ambientesMaoDeObraQuantidade = new Dictionary<int, int>();

                            foreach (var produtoPedido in lstProdPed)
                            {
                                if (produtoPedido == null || produtoPedido.IdAmbientePedido == null ||
                                    ambientesMaoDeObraQuantidade.ContainsKey((int)produtoPedido.IdAmbientePedido))
                                    continue;

                                ambientesMaoDeObraQuantidade.Add((int)produtoPedido.IdAmbientePedido, produtoPedido.QtdeAmbiente);
                            }

                            lstParam.Add(new ReportParameter("QuantidadeAmbienteMaoDeObra", ambientesMaoDeObraQuantidade.Sum(f => f.Value).ToString()));
                        }

                        var lstResumoCorte = ResumoCorteDAO.Instance.GetProdutosByPedidoEspelho(lstProdPed);
 
                        if (report.ReportPath.Contains("rptPedidoPcpPerfectGlass.rdlc"))
                        {
                            var imagens = new List<Imagem>();
                            var idsItemProjeto = new List<int>();
                            uint idProjetoModelo = 0;
                            uint idGrupoModelo;
                            string descricaoGrupoModelo;

                            foreach (var prodPed in lstProdPed)
                            {
                                if (prodPed.IdItemProjeto.GetValueOrDefault() == 0)
                                    continue;

                                idProjetoModelo = ItemProjetoDAO.Instance.ObtemIdProjetoModelo(null, (uint)prodPed.IdItemProjeto);
                                idGrupoModelo = ProjetoModeloDAO.Instance.ObtemGrupoModelo(null, idProjetoModelo);
                                descricaoGrupoModelo = GrupoModeloDAO.Instance.ObtemDescricao(idGrupoModelo);

                                if (descricaoGrupoModelo.ToUpper() == "CORTINA DE VIDRO" && !idsItemProjeto.Contains((int)prodPed.IdItemProjeto))
                                {
                                    var imagemEngenharia = ImagemDAO.Instance.ObterImagemEngenharia((int)prodPed.IdItemProjeto, 1);

                                    if (imagemEngenharia != null)
                                    {
                                        imagens.AddRange(imagemEngenharia);
                                        idsItemProjeto.Add((int)prodPed.IdItemProjeto);
                                    }
                                }
                            }
                            
                            report.DataSources.Add(new ReportDataSource("Imagem", imagens.ToArray()));
                            report.DataSources.Add(new ReportDataSource("ItemProjeto", ItemProjetoDAO.Instance.GetByString(string.Join(",", idsItemProjeto))));
                        }
                        
                        #region Par�metros pedido

                        lstParam.Add(new ReportParameter("Pedido_Acrescimos", pedidoAcrescimos));
                        lstParam.Add(new ReportParameter("Pedido_DatasEntrega", pedidoDatasEntrega));
                        lstParam.Add(new ReportParameter("Pedido_DatasFabrica", pedidoDatasFabrica));
                        lstParam.Add(new ReportParameter("Pedido_DatasPedido", pedidoDatasPedido));
                        lstParam.Add(new ReportParameter("Pedido_Descontos", pedidoDescontos));
                        lstParam.Add(new ReportParameter("Pedido_Observacoes", pedidoObservacoes));
                        lstParam.Add(new ReportParameter("Pedido_RotasCliente", pedidoRotasCliente));
                        lstParam.Add(new ReportParameter("Pedido_TiposEntrega", pedidoTiposEntrega));
                        lstParam.Add(new ReportParameter("Pedido_CidadesUf", pedidoCidadesUf));
                        lstParam.Add(new ReportParameter("Pedido_ContatosCliente", pedidoContatosCliente));
                        lstParam.Add(new ReportParameter("Pedido_NomesCliente", pedidoNomesCliente));
                        lstParam.Add(new ReportParameter("Pedido_CodsCliente", pedidoCodsCliente));
                        lstParam.Add(new ReportParameter("Pedido_NomesFunc", pedidoNomesFunc));
                        lstParam.Add(new ReportParameter("Pedido_LocalizacoesObra", pedidoLocalizacoesObra));
                        lstParam.Add(new ReportParameter("Pedido_Bairros", pedidoBairros));
                        lstParam.Add(new ReportParameter("Pedido_Ceps", pedidoCeps));
                        lstParam.Add(new ReportParameter("Pedido_TelefonesContatoCliente", pedidoTelefonesContatoCliente));
                        lstParam.Add(new ReportParameter("Pedido_Enderecos", pedidoEnderecos));
                        lstParam.Add(new ReportParameter("Pedido_CpfsCnpjs", pedidoCpfsCnpjs));
                        lstParam.Add(new ReportParameter("Pedido_RgsInscEsts", pedidoRgsInscEsts));

                        #endregion

                        lstParam.Add(new ReportParameter("AgruparBeneficiamentos", PedidoConfig.RelatorioPedido.AgruparBenefRelatorio.ToString()));
                        lstParam.Add(new ReportParameter("FormatTotM", Glass.Configuracoes.Geral.GetFormatTotM()));
                        lstParam.Add(new ReportParameter("AgruparProdutos", agruparProdutos.ToString()));
                        lstParam.Add(new ReportParameter("EsconderResumo", "false"));
                        lstParam.Add(new ReportParameter("ExibirAmbiente", PedidoConfig.DadosPedido.AmbientePedido.ToString()));
                        lstParam.Add(new ReportParameter("ExibirDescrAmbiente", PedidoConfig.DadosPedido.AmbientePedido.ToString()));
                        lstParam.Add(new ReportParameter("Observacoes", string.IsNullOrEmpty(outrosParametros[0].ToString()) ? "." : outrosParametros[0].ToString()));
                        lstParam.Add(new ReportParameter("ExibirTotaisPecasComposicaoECompostas", lstProdPed.Any(f => f.IsProdFilhoLamComposicao || f.IsProdutoLaminadoComposicao).ToString()));

                        report.DataSources.Add(new ReportDataSource("Pedido", pedido));
                        report.DataSources.Add(new ReportDataSource("PedidoEspelho", pedidoEspelho.ToArray()));
                        report.DataSources.Add(new ReportDataSource("ProdutosPedidoEspelho", lstProdPed));
                        report.DataSources.Add(new ReportDataSource("ResumoCorte", lstResumoCorte));

                        break;
                    }
                case "PedidoPcpAgrupado":
                    {
                        report.ReportPath = "Relatorios/rptPedidoPcpAgrupado.rdlc";
                        var idImpressao = !String.IsNullOrEmpty(Request["idImpressao"]) ? Glass.Conversoes.StrParaUint(Request["idImpressao"]) : 0;
                        var idPedido = !String.IsNullOrEmpty(Request["idPedido"]) ? Glass.Conversoes.StrParaUint(Request["idPedido"]) : 0;
                        float? altura = !String.IsNullOrEmpty(Request["altura"]) ? (float?)Glass.Conversoes.StrParaFloat(Request["altura"]) : null;
                        int? largura = !String.IsNullOrEmpty(Request["largura"]) ? (int?)Glass.Conversoes.StrParaInt(Request["largura"]) : null;

                        lstParam.Add(new ReportParameter("NomeLoja", login.NomeLoja));
                        lstParam.Add(new ReportParameter("NumImpressao", idImpressao.ToString()));

                        report.DataSources.Add(new ReportDataSource("ProdutoImpressao", ProdutoImpressaoDAO.Instance.GetListImpressao(
                            idImpressao, null, idPedido, 0, Request["descrProduto"],
                            Request["etiqueta"], altura, largura, null, 0, int.MaxValue).ToArray()));

                        break;
                    }
                case "HistoricoCliente":
                    {
                        report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptHistoricoCliente{0}.rdlc");
                        var lstHistorico = ContasReceberDAO.Instance.GetForRptHist(Glass.Conversoes.StrParaUint(Request["idCliente"]),
                            Glass.Conversoes.StrParaUint(Request["idPedido"]), Request["dtIniVenc"], Request["dtFimVenc"], Request["dtIniRec"], Request["dtFimRec"],
                            Request["dtIniCad"], Request["dtFimCad"], Glass.Conversoes.StrParaFloat(Request["vIniVenc"]), Glass.Conversoes.StrParaFloat(Request["vFimVenc"]),
                            Glass.Conversoes.StrParaFloat(Request["vIniRec"]), Single.Parse(Request["vFimRec"], System.Globalization.NumberStyles.Any),
                            Request["emAberto"] == "true", Request["recEmDia"] == "true", Request["recComAtraso"] == "true", Request["buscarParcCartao"] == "true",
                            Request["contasRenegociadas"].StrParaInt(), Request["buscaPedRepoGarantia"] == "true", Request["buscarChequeDevolvido"] == "true", Request["sort"]);

                        report.DataSources.Add(new ReportDataSource("ContasReceber", lstHistorico.ToArray()));
                    }
                    break;
                case "HistoricoFornec":
                    {
                        report.ReportPath = "Relatorios/rptHistoricoFornec.rdlc";
                        var lstHistoricoFornec = ContasPagarDAO.Instance.GetForRptHist(Glass.Conversoes.StrParaUint(Request["idFornec"]), Request["dtIniVenc"],
                            Request["dtFimVenc"], Request["dtIniPag"], Request["dtFimPag"], Single.Parse(Request["vIniVenc"], System.Globalization.NumberStyles.Any),
                            Single.Parse(Request["vFimVenc"], System.Globalization.NumberStyles.Any), Single.Parse(Request["vIniPag"], System.Globalization.NumberStyles.Any),
                            Single.Parse(Request["vFimPag"], System.Globalization.NumberStyles.Any), Request["emAberto"] == "true", Request["pagEmDia"] == "true",
                            Request["pagComAtraso"] == "true", Request["sort"]);

                        report.DataSources.Add(new ReportDataSource("ContasPagar", lstHistoricoFornec));

                        break;
                    }
                case "Obra":
                    {
                        var caminhoRelatorio = Request["obraDetalhada"] == "true" ? string.Format("Relatorios/ModeloObra/rptObraDetalhada{0}.rdlc", ControleSistema.GetSite().ToString()) :
                            string.Format("Relatorios/ModeloObra/rptObra{0}.rdlc", ControleSistema.GetSite().ToString());

                        if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorio))))
                            report.ReportPath = caminhoRelatorio;
                        else
                            report.ReportPath = Request["obraDetalhada"] == "true" ? "Relatorios/rptObraDetalhada.rdlc" :
                                Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptObra{0}.rdlc");

                        var obra = ObraDAO.Instance.GetElement(Request["idObra"].StrParaUint());
                        var lstPedido = PedidoDAO.Instance.GetForRptObra(obra.IdObra);
                        var lstProdsPed = new List<ProdutosPedido>();

                        for (var i = 0; i < lstPedido.Length; i++)
                            lstProdsPed.AddRange(ProdutosPedidoDAO.Instance.GetByPedido(lstPedido[i].IdPedido, PedidoConfig.LiberarPedido));

                        if (report.ReportPath.Contains("rptObraGuaporeVidros.rdlc") && Request["obraDetalhada"] == "false")
                        {
                            var idLoja = (int)ClienteDAO.Instance.ObtemIdLoja(obra.IdCliente);

                            if (idLoja > 0)
                            {
                                var loja = LojaDAO.Instance.GetElement((uint)idLoja);

                                lstParam.Add(new ReportParameter("NomeLoja", loja.RazaoSocial));
                                lstParam.Add(new ReportParameter("CNPJLoja",
                                    string.Format("CNPJ: {0}", loja.Cnpj)));
                                lstParam.Add(new ReportParameter("EnderecoLoja",
                                    string.Format("{0}, {1} - {2}", loja.Endereco, loja.Numero, loja.Bairro)));
                                lstParam.Add(new ReportParameter("CEPCidadeUFLoja",
                                    string.Format("CEP: {0} - {1} - {2}", loja.Cep, loja.Cidade, loja.Uf)));
                                lstParam.Add(new ReportParameter("EmailContatoLoja",
                                    string.Format("E-mail: {0}", loja.EmailContato)));
                                lstParam.Add(new ReportParameter("TelefoneLoja",
                                    string.Format("TEL: {0}", loja.Telefone)));
                                lstParam.Add(new ReportParameter("CidadeLoja", loja.Cidade));
                            }
                        }

                        if (report.ReportPath != "Relatorios/rptObraDetalhada.rdlc")
                        {
                            lstParam.Add(new ReportParameter("ExibirSaldoDevedor", FinanceiroConfig.FinanceiroRec.ExibirSaldoDevedorRelsRecebimento.ToString()));

                            report.DataSources.Add(new ReportDataSource("Pedido", lstPedido));
                            report.DataSources.Add(new ReportDataSource("Cheques", ChequesDAO.Instance.GetByObra(obra.IdObra)));
                        }

                        report.DataSources.Add(new ReportDataSource("Obra", new Obra[] { obra }));
                        report.DataSources.Add(new ReportDataSource("ProdutoObra", ProdutoObraDAO.Instance.GetForRpt(obra.IdObra).ToArray()));
                        report.DataSources.Add(new ReportDataSource("ProdutosPedido", lstProdsPed));

                        break;
                    }
                case "ObraGerarCredito":
                    {
                        report.ReportPath = "Relatorios/rptObraGerarCredito.rdlc";

                        var agrupar = Request["agrupar"];

                        /* Chamado 45420. */
                        if (!string.IsNullOrEmpty(agrupar))
                        {
                            report.ReportPath = "Relatorios/rptObraGerarCreditoAgrupado.rdlc";
                            // 1 - Cliente.
                            lstParam.Add(new ReportParameter("Agrupar", agrupar));
                        }

                        var idCliente = string.IsNullOrEmpty(Request["idCliente"]) ? 0 : Glass.Conversoes.StrParaUint(Request["idCliente"]);
                        var nomeCliente = Request["nomeCliente"];
                        var situacao = Request["situacao"];
                        var dataIni = Request["dataIni"];
                        var dataFim = Request["dataFim"];
                        var dataFinIni = Request["dataFinIni"];
                        var dataFinFim = Request["dataFinFim"];
                        var idObra = string.IsNullOrEmpty(Request["idObra"]) ? 0 : Glass.Conversoes.StrParaUint(Request["idObra"]);
                        var idPedido = string.IsNullOrEmpty(Request["idPedido"]) ? 0 : Glass.Conversoes.StrParaUint(Request["idPedido"]);
                        var descricao = Request["descricao"];

                        var obras = ObraDAO.Instance.GetListRpt(idCliente, nomeCliente, Glass.Conversoes.StrParaUint(Request["idFunc"]),
                            Glass.Conversoes.StrParaUint(Request["idFuncCad"]), Glass.Conversoes.StrParaUint(Request["idFormaPagto"]),
                            Glass.Conversoes.StrParaInt(situacao), dataIni, dataFim, dataFinIni, dataFinFim, true, null, idObra, idPedido, descricao);

                        report.DataSources.Add(new ReportDataSource("Obra", obras));
                        report.DataSources.Add(new ReportDataSource("ProdutoObra", ProdutoObraDAO.Instance.GetForRpt(obras)));

                        break;
                    }
                case "ObraPagAntecipado":
                    {
                        report.ReportPath = "Relatorios/rptObraPagtoAntecipado.rdlc";
                        var idCliente = string.IsNullOrEmpty(Request["idCliente"]) ? 0 : Glass.Conversoes.StrParaUint(Request["idCliente"]);
                        var nomeCliente = Request["nomeCliente"];
                        var situacao = Request["situacao"];
                        var dataIni = Request["dataIni"];
                        var dataFim = Request["dataFim"];
                        var dataFinIni = Request["dataFinIni"];
                        var dataFinFim = Request["dataFinFim"];
                        var idObra = string.IsNullOrEmpty(Request["idObra"]) ? 0 : Glass.Conversoes.StrParaUint(Request["idObra"]);
                        var idPedido = string.IsNullOrEmpty(Request["idPedido"]) ? 0 : Glass.Conversoes.StrParaUint(Request["idPedido"]);
                        var descricao = Request["descricao"];
                        var obras = ObraDAO.Instance.GetListRpt(idCliente, nomeCliente, Glass.Conversoes.StrParaUint(Request["idFunc"]),
                            Glass.Conversoes.StrParaUint(Request["idFuncCad"]), Glass.Conversoes.StrParaUint(Request["idFormaPagto"]),
                            Glass.Conversoes.StrParaInt(situacao), dataIni, dataFim, dataFinIni, dataFinFim, false, null, idObra, idPedido, descricao);

                        report.DataSources.Add(new ReportDataSource("Obra", obras));
                        report.DataSources.Add(new ReportDataSource("ProdutoObra", ProdutoObraDAO.Instance.GetForRpt(obras)));

                        break;
                    }
                case "AntecipFornec":
                    {
                        report.ReportPath = "Relatorios/rptAntecipFornec.rdlc";
                        var AntecipFornec = AntecipacaoFornecedorDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idAntecipFornec"]));
                        var lstNotas = NotaFiscalDAO.Instance.GetForRptAntecipFornec(AntecipFornec.IdAntecipFornec);
                        var lstCompras = CompraDAO.Instance.GetForRptAntecipFornec(AntecipFornec.IdAntecipFornec);

                        report.DataSources.Add(new ReportDataSource("AntecipFornec", new AntecipacaoFornecedor[] { AntecipFornec }));
                        report.DataSources.Add(new ReportDataSource("NotaFiscal", lstNotas));
                        report.DataSources.Add(new ReportDataSource("Compra", lstCompras.ToArray()));
                        report.DataSources.Add(new ReportDataSource("Cheques", ChequesDAO.Instance.GetByAntecipFornec(AntecipFornec.IdAntecipFornec)));

                        break;
                    }
                case "ListaAntecipFornec":
                    {
                        report.ReportPath = "Relatorios/rptListaAntecipFornec.rdlc";
                        var Antecips = AntecipacaoFornecedorDAO.Instance.GetListRpt(Glass.Conversoes.StrParaUint(Request["idFornec"]),
                            Request["nomeCliente"], Glass.Conversoes.StrParaUint(Request["idFunc"]), Glass.Conversoes.StrParaUint(Request["idFormaPagto"]), Glass.Conversoes.StrParaInt(Request["situacao"]),
                            Request["dataIni"], Request["dataFim"], null);

                        report.DataSources.Add(new ReportDataSource("AntecipFornec", Antecips));

                        break;
                    }
                case "PedidoEspelho":
                    {
                        // Ao recuperar o valor da query string "pedidos", deve ser buscado sem Request, o motivo disso � para
                        // que a tela de impress�o seletiva no menu PCP funcione corretamente
                        report.ReportPath = "Relatorios/rptPedidoEspelho.rdlc";
                        var lstPedEsp = PedidoEspelhoDAO.Instance.GetForRpt(Conversoes.StrParaUint(Request["idPedido"]),
                            Conversoes.StrParaUint(Request["idCliente"]), Request["NomeCliente"], Conversoes.StrParaUint(Request["idLoja"]),
                            Conversoes.StrParaUint(Request["idFunc"]), Conversoes.StrParaUint(Request["idFuncionarioConferente"]),
                            Conversoes.StrParaInt(Request["situacao"]), Request["situacaoPedOri"], Request["idsProcesso"], Request["dataIniEnt"],
                            Request["dataFimEnt"], Request["dataIniFab"], Request["dataFimFab"], Request["dataIniFin"], Request["dataFimFin"],
                            Request["dataIniConf"], Request["dataFimConf"], Request["dataIniEmis"], Request["dataFimEmis"], false, Request["pedidos"],
                            Request["pedidosSemAnexos"] == "true", Request["pedidosAComprar"] == "true", Request["situacaoCnc"], Request["dataIniSituacaoCnc"],
                            Request["dataFimSituacaoCnc"], Request["tipoPedido"], Request["idsRotas"], Conversoes.StrParaInt(Request["origemPedido"]), Conversoes.StrParaInt(Request["pedidosConferidos"]), login);
                        var lstProdPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetForRpt(Conversoes.StrParaUint(Request["idPedido"]),
                            Conversoes.StrParaUint(Request["idCliente"]), Request["NomeCliente"], Conversoes.StrParaUint(Request["idLoja"]),
                            Conversoes.StrParaUint(Request["idFunc"]), Conversoes.StrParaUint(Request["idFuncionarioConferente"]),
                            Conversoes.StrParaInt(Request["situacao"]), Request["situacaoPedOri"], Request["idsProcesso"], Request["dataIniEnt"],
                            Request["dataFimEnt"], Request["dataIniFab"], Request["dataFimFab"], Request["dataIniFin"], Request["dataFimFin"],
                            Request["dataIniConf"], Request["dataFimConf"], false, Request["pedidosSemAnexos"] == "true", Request["pedidosAComprar"] == "true",
                            Request["pedidos"], Request["situacaoCnc"], Request["dataIniSituacaoCnc"], Request["dataFimSituacaoCnc"], Request["tipoPedido"],
                            Request["idsRotas"], Conversoes.StrParaInt(Request["origemPedido"]), Conversoes.StrParaInt(Request["pedidosConferidos"]));

                        lstParam.Add(new ReportParameter("ExibirSituacaoCnc", PCPConfig.UsarControleGerenciamentoProjCnc.ToString()));
                        report.DataSources.Add(new ReportDataSource("PedidoEspelho", lstPedEsp.ToArray()));
                        report.DataSources.Add(new ReportDataSource("ProdutosPedidoEspelhoRpt", ProdutosPedidoEspelhoRptDAO.Instance.CopiaLista(lstProdPedEsp.ToArray(),
                            ProdutosPedidoEspelhoRpt.TipoConstrutor.ListaPedidoEspelho)));

                        break;
                    }
                case "Producao":
                case "ProducaoAgrupada":
                case "ProducaoPassou":
                case "ProducaoPedidos":
                    {
                        switch (Request["rel"])
                        {
                            case "Producao":
                                report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/Producao/rptProducao{0}.rdlc"); break;

                            case "ProducaoAgrupada":
                                report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/Producao/rptProducaoAgrupado{0}.rdlc"); break;

                            case "ProducaoPassou":
                                report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/Producao/rptProducaoRoteiro{0}.rdlc"); break;

                            case "ProducaoPedidos":
                                report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptLstPedidos.rdlc"); break;

                            default: throw new Exception("Relat�rio n�o mapeado.");
                        }

                        var idCarregamento = Request["idCarregamento"].StrParaInt();
                        var idLiberarPedido = Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]);
                        var idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                        var idPedidoImportado = Request["idPedidoImportado"];
                        var idLoja = Glass.Conversoes.StrParaUint(Request["idLoja"]);
                        var idCliente = Glass.Conversoes.StrParaUint(Request["idCliente"]);
                        var idImpressao = Glass.Conversoes.StrParaUint(Request["idImpressao"]);
                        var idSetor = Glass.Conversoes.StrParaInt(Request["idSetor"]);
                        var tipoEntrega = Glass.Conversoes.StrParaUint(Request["tipoEntrega"]);
                        var idFunc = Glass.Conversoes.StrParaUint(Request["idFunc"]);
                        var situacao = Glass.Conversoes.StrParaInt(Request["situacaoPedido"]);
                        var idCorVidro = Glass.Conversoes.StrParaUint(Request["idCorVidro"]);
                        var altura = Glass.Conversoes.StrParaInt(Request["altura"]);
                        var largura = Glass.Conversoes.StrParaInt(Request["largura"]);
                        var espessura = Glass.Conversoes.StrParaInt(Request["espessura"]);
                        var dataIniFabr = Request["dataIniFabr"];
                        var dataFimFabr = Request["dataFimFabr"];
                        var dataIniConfPed = Request["dataIniConfPed"];
                        var dataFimConfPed = Request["dataFimConfPed"];
                        var relatorioSetor = (Request["setorFiltrado"] == "true") ? true : false;
                        var pedFastDelivery = Glass.Conversoes.StrParaUint(Request["fastDelivery"]);
                        var produtoComposicao = Request["produtoComposicao"].StrParaIntNullable();
                        var relatorioPerda = situacao == (int)ProdutoPedidoProducao.SituacaoEnum.Perda && Request["rel"] == "Producao";

                        if (relatorioPerda)
                        {
                            report.ReportPath = report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/Producao/rptProducaoAltLarg{0}.rdlc");
                            var lstPerdaProducao = Glass.Data.RelDAL.ProducaoDAO.Instance.GetForRpt(idCarregamento, idLiberarPedido, idPedido,
                                idPedidoImportado, idImpressao, Request["codRota"], Request["codCliente"], idCliente, Request["nomeCliente"],
                                Request["numEtiqueta"], Request["dataIni"], Request["dataFim"], Request["dataIniEnt"], Request["dataFimEnt"], idSetor,
                                situacao, Glass.Conversoes.StrParaInt(Request["tiposSituacoes"]), Glass.Conversoes.StrParaUint(Request["idSubgrupo"]), tipoEntrega,
                                Request["pecasCanc"] == "1", idFunc, Request["tipoPedido"], idCorVidro, altura, largura,
                                Request["aguardExpedicao"] == "true", Request["aguardEntrEstoque"] == "true");

                            report.DataSources.Add(new ReportDataSource("Producao", lstPerdaProducao.ToArray()));
                        }

                        if (Request["rel"] == "Producao" || Request["rel"] == "ProducaoPassou")
                        {
                            var lstProdPedProd = ProdutoPedidoProducaoDAO.Instance.GetForRpt(idCarregamento, idLiberarPedido, idPedido,
                                idPedidoImportado, idImpressao, Request["codCliente"], Request["codRota"], idCliente,
                                Request["nomeCliente"], Request["numEtiqueta"], Request["dataIni"], Request["dataFim"],
                                Request["dataIniEnt"], Request["dataFimEnt"], dataIniFabr, dataFimFabr, dataIniConfPed, dataFimConfPed,
                                idSetor, Request["situacao"], situacao, Glass.Conversoes.StrParaInt(Request["tiposSituacoes"]),
                                Request["idsSubgrupos"], tipoEntrega, Request["pecasProdCanc"], idFunc, Request["tipoPedido"], idCorVidro,
                                altura, largura, espessura, Request["idsProc"], Request["idsApl"], Request["aguardExpedicao"] == "true",
                                Request["aguardEntrEstoque"] == "true", Request["idsBenef"], Request["planoCorte"],
                                Request["numEtiquetaChapa"], pedFastDelivery, Request["pecaParadaProducao"] == "true",
                                Request["pecasRepostas"] == "true", Glass.Conversoes.StrParaUint(Request["idLoja"]), produtoComposicao);

                            var setores = SetorProducaoDAO.Instance.GetLeiturasSetores(lstProdPedProd);

                            if (!relatorioPerda)
                            {
                                var numeroSetores = 0;
                                var numSetor = 0;
                                var titulosSetores = new string[30];

                                for (var i = 0; i < Data.Helper.Utils.GetSetores.Length; i++)
                                {
                                    if (Data.Helper.Utils.GetSetores[i].ExibirRelatorio)
                                    {
                                        titulosSetores[numeroSetores++] = Data.Helper.Utils.GetSetores[i].Sigla;

                                        if (relatorioSetor && Data.Helper.Utils.GetSetores[i].IdSetor == idSetor)
                                            numSetor = i;
                                    }
                                }

                                lstParam.Add(new ReportParameter("NumeroSetores", numeroSetores.ToString()));
                                lstParam.Add(new ReportParameter("TitulosSetores", titulosSetores));

                                if (relatorioSetor)
                                {
                                    report.ReportPath = report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/Producao/rptProducaoSetor{0}.rdlc");
                                    lstParam.Add(new ReportParameter("NumeroSetor", numSetor.ToString()));
                                    setores = SetorProducaoDAO.Instance.GetUltLeiturasSetores(lstProdPedProd, idSetor > 0 ?
                                            idSetor : Data.Helper.Utils.GetSetores.Where(x => x.NumeroSequencia == 1).First().IdSetor);
                                }
                            }

                            lstParam.Add(new ReportParameter("Agrupar", string.IsNullOrEmpty(Request["agrupar"]) ? "0" : Request["agrupar"]));

                            /* Chamado 51919. */
                            if (!report.ReportPath.Contains("rptProducaoAgrupado") && !report.ReportPath.Contains("rptProducaoRoteiro") && !report.ReportPath.Contains("rptProducaoAltLarg") &&
                                !report.ReportPath.Contains("rptProducaoSetor"))
                                report.DataSources.Add(new ReportDataSource("ProdutoPedidoProducaoRpt", ProdutoPedidoProducaoRptDAL.Instance.CopiaLista(lstProdPedProd)));
                            else
                                report.DataSources.Add(new ReportDataSource("ProdutoPedidoProducao", lstProdPedProd));

                            report.DataSources.Add(new ReportDataSource("SetorProducao", setores.ToArray()));
                        }
                        else if (Request["rel"] == "ProducaoPedidos")
                        {
                            var lstIdsPedidos = ProdutoPedidoProducaoDAO.Instance.GetListIdsPedidos(idCarregamento, idLiberarPedido.ToString(), idPedido,
                                idPedidoImportado, idImpressao, Request["codCliente"], Request["codRota"], idCliente,
                                Request["nomeCliente"], Request["numEtiqueta"], Request["dataIni"], Request["dataFim"],
                                Request["dataIniEnt"], Request["dataFimEnt"], dataIniFabr, dataFimFabr, dataIniConfPed, dataFimConfPed,
                                idSetor, Request["situacao"], situacao, Glass.Conversoes.StrParaInt(Request["tiposSituacoes"]),
                                Request["idsSubgrupos"], tipoEntrega, Request["pecasProdCanc"], idFunc, Request["tipoPedido"], idCorVidro,
                                altura, largura, espessura, Request["idsProc"], Request["idsApl"], Request["aguardExpedicao"] == "true",
                                Request["aguardEntrEstoque"] == "true", Request["idsBenef"], Request["planoCorte"],
                                Request["numEtiquetaChapa"], pedFastDelivery, Request["pecaParadaProducao"] == "true",
                                Request["pecasRepostas"] == "true", Glass.Conversoes.StrParaUint(Request["idLoja"]), produtoComposicao);

                            var pedidos = PedidoDAO.Instance.ObterPedidosProducao(null, lstIdsPedidos);
                            lstParam.Add(new ReportParameter("Titulo", "Pedidos produ��o"));
                            report.ReportPath = "Relatorios/rptLstPedidos.rdlc";
                            report.DataSources.Add(new ReportDataSource("Pedidos", pedidos));

                            break;
                        }
                        else
                        {
                            var lstProdPedProd = ProdutoPedidoProducaoDAO.Instance.GetForRptConsultaAgrupada(idPedido,
                                idLoja, Glass.Conversoes.StrParaUint(Request["idFunc"]), Glass.Conversoes.StrParaUint(Request["idRota"]), idCliente,
                                Request["nomeCliente"], Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaInt(Request["situacao"]), (uint)idSetor,
                                Request["idsSubgrupo"], true, Request["agruparDia"] == "true", false, Request["tipoCliente"]);
                            var lstProdPedProdAgrupProcApl = ProdutoPedidoProducaoDAO.Instance.GetForRptConsultaAgrupada(
                                idPedido, idLoja, Glass.Conversoes.StrParaUint(Request["idFunc"]),
                                Glass.Conversoes.StrParaUint(Request["idRota"]), idCliente, Request["nomeCliente"], Request["dataIni"], Request["dataFim"],
                                Glass.Conversoes.StrParaInt(Request["situacao"]), (uint)idSetor, Request["idsSubgrupo"], true,
                                Request["agruparDia"] == "true", true, Request["tipoCliente"]);
                            lstParam.Add(new ReportParameter("AgruparDia", Request["agruparDia"]));

                            report.DataSources.Add(new ReportDataSource("ProdutoPedidoProducao", lstProdPedProd.ToArray()));
                            report.DataSources.Add(new ReportDataSource("ProdutoPedidoProducaoAgrupProcApl", lstProdPedProdAgrupProcApl.ToArray()));
                        }

                        var cont = ProdutoPedidoProducaoDAO.Instance.GetCountBySituacao(idCarregamento, idLiberarPedido, idPedido,
                            idPedidoImportado, idImpressao, Request["codRota"], Request["codCliente"], idCliente, Request["nomeCliente"],
                            Request["numEtiqueta"], Request["dataIni"], Request["dataFim"], Request["dataIniEnt"], Request["dataFimEnt"],
                            dataIniFabr, dataFimFabr, dataIniConfPed, dataFimConfPed, idSetor, Request["situacao"], situacao,
                            Glass.Conversoes.StrParaInt(Request["tiposSituacoes"]), Request["idsSubgrupos"], tipoEntrega,
                            Request["pecasProdCanc"], idFunc, Request["tipoPedido"], idCorVidro, altura, largura, espessura,
                            Request["idsProc"], Request["idsApl"], Request["aguardExpedicao"] == "true",
                            Request["aguardEntrEstoque"] == "true", Request["idsBenef"], Request["planoCorte"],
                            Request["numEtiquetaChapa"], pedFastDelivery, Request["pecaParadaProducao"] == "true",
                            Request["pecasRepostas"] == "true", Glass.Conversoes.StrParaUint(Request["idLoja"]), produtoComposicao);

                        lstParam.Add(new ReportParameter("PecasPendentes", string.Format("{0} ({1} / {2} m�)", cont.Pendentes, cont.TotMPendentes, cont.TotMPendentesCalc)));
                        lstParam.Add(new ReportParameter("PecasProntas", string.Format("{0} ({1} / {2} m�)", cont.Prontas, cont.TotMProntas, cont.TotMProntasCalc)));
                        lstParam.Add(new ReportParameter("PecasEntregues", string.Format("{0} ({1} / {2} m�)", cont.Entregues, cont.TotMEntregues, cont.TotMEntreguesCalc)));
                        lstParam.Add(new ReportParameter("PecasPerdidas", string.Format("{0} ({1} / {2} m�)", cont.Perdidas, cont.TotMPerdidas, cont.TotMPerdidasCalc)));
                        lstParam.Add(new ReportParameter("PecasCanceladas", string.Format("{0} ({1} / {2} m�)", cont.Canceladas, cont.TotMCanceladas, cont.TotMCanceladasCalc)));

                        if (Request["rel"] == "ProducaoAgrupada")
                            lstParam.Add(new ReportParameter("ExibirValorCustoVenda", PCPConfig.ExibirCustoVendaRelatoriosProducao.ToString()));

                        break;
                    }
                case "ListaLiberacao":
                    {
                        var idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                        var idLiberarPedido = Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]);
                        var idLoja = 0;

                        if (idPedido > 0)
                            idLoja = (int)PedidoDAO.Instance.ObtemIdLoja(idPedido);
                        if (idLiberarPedido > 0)
                            idLoja = (int)LiberarPedidoDAO.Instance.ObtemIdLoja(idLiberarPedido);

                        var calcularIcmsPedido = idLoja > 0 ? LojaDAO.Instance.ObtemCalculaIcmsPedido((uint)idLoja) :
                            (Glass.Conversoes.StrParaUint(Request["idLoja"]) > 0 ? LojaDAO.Instance.ObtemCalculaIcmsPedido(Glass.Conversoes.StrParaUint(Request["idLoja"])) :
                            PedidoConfig.Impostos.CalcularIcmsPedido);

                        report.ReportPath = "Relatorios/rptListaLiberacao" + (calcularIcmsPedido ? "Icms" : "") + ".rdlc";
                        var lstLiberacao = LiberarPedidoDAO.Instance.GetForRpt(idLiberarPedido, idPedido,
                            Glass.Conversoes.StrParaIntNullable(Request["numeroNfe"]), Glass.Conversoes.StrParaUint(Request["idFunc"]), Glass.Conversoes.StrParaUint(Request["idCliente"]), Request["nomeCliente"],
                            Glass.Conversoes.StrParaInt(Request["liberacaoNf"]), Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaInt(Request["situacao"]), Glass.Conversoes.StrParaUint(Request["idLoja"]),
                            Request["dataIniCanc"], Request["dataFimCanc"]);

                        // Recupera os pedidos de cada libera��o
                        foreach (var lp in lstLiberacao)
                            lp.IdsPedidos = LiberarPedidoDAO.Instance.IdsPedidos(null, lp.IdLiberarPedido.ToString());

                        report.DataSources.Add(new ReportDataSource("LiberarPedido", lstLiberacao));

                        break;
                    }
                case "VendasCliente":
                    {
                        report.ReportPath = "Relatorios/rptVendasClienteComissionado.rdlc";
                        var idCliente = Glass.Conversoes.StrParaUint(Request["idCliente"]);
                        var idComissionado = Glass.Conversoes.StrParaUint(Request["idComissionado"]);
                        var idsFunc = Request["idsFunc"];
                        var idsFuncAssociaCliente = Request["idsFuncAssociaCliente"];
                        var mesInicio = Glass.Conversoes.StrParaInt(Request["mesInicio"]);
                        var anoInicio = Glass.Conversoes.StrParaInt(Request["anoInicio"]);
                        var mesFim = Glass.Conversoes.StrParaInt(Request["mesFim"]);
                        var anoFim = Glass.Conversoes.StrParaInt(Request["anoFim"]);
                        var tipoMedia = Request["tipoMedia"];
                        var ordenar = Glass.Conversoes.StrParaInt(Request["ordenar"]);
                        var tipoVendas = Glass.Conversoes.StrParaInt(Request["tipoVendas"]);
                        var valorMinimo = Glass.Conversoes.StrParaDecimal(Request["valorMinimo"]);
                        var valorMaximo = Glass.Conversoes.StrParaDecimal(Request["valorMaximo"]);
                        var idLoja = Glass.Conversoes.StrParaUint(Request["idLoja"]);
                        var situacao = Request["situacaoCliente"].StrParaInt();
                        var lojaCliente = Request["lojaCliente"] == "true";
                        var idTabelaDescontoAcrescimo = Glass.Conversoes.StrParaInt(Request["idTabelaDescontoAcrescimo"]);
                        var tipoPedido = Request["tipoPedido"];

                        report.DataSources.Add(new ReportDataSource("VendasMeses", Glass.Data.RelDAL.VendasMesesDAO.Instance.GetVendasMeses(
                            idCliente, Request["nomeCliente"], Request["idsRota"], Request["revenda"].ToLower() == "true",
                            idComissionado, Request["nomeComissionado"], mesInicio, anoInicio, mesFim, anoFim, tipoMedia, ordenar, tipoVendas, idsFunc, Request["nomeFunc"],
                            idsFuncAssociaCliente, valorMinimo, valorMaximo, idLoja, lojaCliente, Request["tipoCliente"], idTabelaDescontoAcrescimo, situacao, true, tipoPedido)));

                        break;
                    }
                case "VendasComissionado":
                    {
                        report.ReportPath = "Relatorios/rptVendasClienteComissionado.rdlc";
                        var idCliente = !String.IsNullOrEmpty(Request["idCliente"]) ? Glass.Conversoes.StrParaUint(Request["idCliente"]) : 0;
                        var idComissionado = !String.IsNullOrEmpty(Request["idComissionado"]) ? Glass.Conversoes.StrParaUint(Request["idComissionado"]) : 0;
                        var mesInicio = Glass.Conversoes.StrParaInt(Request["mesInicio"]);
                        var anoInicio = Glass.Conversoes.StrParaInt(Request["anoInicio"]);
                        var mesFim = Glass.Conversoes.StrParaInt(Request["mesFim"]);
                        var anoFim = Glass.Conversoes.StrParaInt(Request["anoFim"]);
                        var ordenar = Glass.Conversoes.StrParaInt(Request["ordenar"]);
                        var tipoVendas = Glass.Conversoes.StrParaInt(Request["tipoVendas"]);

                        report.DataSources.Add(new ReportDataSource("VendasMeses", Glass.Data.RelDAL.VendasMesesDAO.Instance.GetVendasMeses(
                            idCliente, Request["nomeCliente"], Request["idRota"], false,
                            idComissionado, Request["nomeComissionado"], mesInicio, anoInicio, mesFim,
                            anoFim, null, ordenar, tipoVendas, null, null, null, 0, 0, 0, false, Request["tipoCliente"], 0, 0, false, "")));

                        break;
                    }
                case "LimiteCliente":
                    {
                        report.ReportPath = "Relatorios/rpt" + (Request["limite"] == "true" ? "" : "Debito") + "LimiteCliente.rdlc";
                        var limite = Request["limite"];
                        var Debitos_IdCliente = Glass.Conversoes.StrParaUint(Request["idCliente"]);

                        lstParam.Add(new ReportParameter("ExibirLiberacao", PedidoConfig.LiberarPedido.ToString()));
                        lstParam.Add(new ReportParameter("Agrupar", Request["agrupar"]));
                        //lstParam.Add(new ReportParameter("AgruparPorRota", "0"));
                        lstParam.Add(new ReportParameter("Ordenar", Request["ordenar"]));
                        lstParam.Add(new ReportParameter("TotalCheques", ContasReceberDAO.Instance.GetDebitosByTipo(Debitos_IdCliente, ContasReceberDAO.TipoDebito.ChequesTotal).ToString("C")));

                        if (report.ReportPath == "Relatorios/rptDebitoLimiteCliente.rdlc")
                        {
                            lstParam.Add(new ReportParameter("TotalChequesEmAberto", ContasReceberDAO.Instance.GetDebitosByTipo(Debitos_IdCliente, ContasReceberDAO.TipoDebito.ChequesEmAberto).ToString("C")));
                            lstParam.Add(new ReportParameter("TotalChequesDevolvidos", ContasReceberDAO.Instance.GetDebitosByTipo(Debitos_IdCliente, ContasReceberDAO.TipoDebito.ChequesDevolvidos).ToString("C")));
                            lstParam.Add(new ReportParameter("TotalChequesProtestados", ContasReceberDAO.Instance.GetDebitosByTipo(Debitos_IdCliente, ContasReceberDAO.TipoDebito.ChequesProtestados).ToString("C")));
                        }

                        lstParam.Add(new ReportParameter("TotalContasRec", ContasReceberDAO.Instance.GetDebitosByTipo(Debitos_IdCliente, ContasReceberDAO.TipoDebito.ContasAReceberTotal).ToString("C")));
                        lstParam.Add(new ReportParameter("TotalContasRecAntecip", ContasReceberDAO.Instance.GetDebitosByTipo(Debitos_IdCliente, ContasReceberDAO.TipoDebito.ContasAReceberAntecipadas).ToString("C")));
                        lstParam.Add(new ReportParameter("TotalPedidos", ContasReceberDAO.Instance.GetDebitosByTipo(Debitos_IdCliente, ContasReceberDAO.TipoDebito.PedidosEmAberto).ToString("C")));
                        lstParam.Add(new ReportParameter("ItensBuscados", Request["buscarItens"].Split(',')));

                        report.DataSources.Add(new ReportDataSource("ContasReceber", ContasReceberDAO.Instance.GetDebitosRpt(Debitos_IdCliente, Glass.Conversoes.StrParaUint(Request["idPedido"]),
                            Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]), Request["buscarItens"], Glass.Conversoes.StrParaInt(Request["tipoBuscaData"]), Request["dataIni"], Request["dataFim"],
                            Glass.Conversoes.StrParaInt(Request["ordenar"])).ToArray()));

                        break;
                    }
                case "AcertoCheque":
                    {
                        report.ReportPath = "Relatorios/rptAcertoCheque.rdlc";

                        report.DataSources.Add(new ReportDataSource("AcertoCheque", new AcertoCheque[] { AcertoChequeDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idAcertoCheque"])) }));
                        report.DataSources.Add(new ReportDataSource("Cheques", ChequesDAO.Instance.GetByAcertoCheque(Glass.Conversoes.StrParaUint(Request["idAcertoCheque"]), false).ToArray()));
                        report.DataSources.Add(new ReportDataSource("ChequesAcertados", ChequesDAO.Instance.GetByAcertoCheque(Glass.Conversoes.StrParaUint(Request["idAcertoCheque"]), true).ToArray()));

                        break;
                    }
                case "ComissaoCalcular":
                    {
                        report.ReportPath = "Relatorios/rptComissaoCalcular.rdlc";
                        var ComissaoCalcular_tipo = (Glass.Data.Model.Pedido.TipoComissao)Glass.Conversoes.StrParaInt(Request["tipo"]);
                        var pedidos = PedidoDAO.Instance.GetPedidosForComissao(ComissaoCalcular_tipo, Glass.Conversoes.StrParaUint(Request["idFunc"]),
                            Request["dataIni"], Request["dataFim"], Request["isRelatorio"] == "true", Request["tipoPedido"],"");
                        var comissao = new Dictionary<uint?, decimal>();
                        var pedidosFunc = new Dictionary<uint?, List<uint>>();
                        var desconto = new Dictionary<uint?, decimal>();
                        var total = new Dictionary<uint?, decimal>();
                        uint ComissaoCalcular_idFunc = 0;

                        for (var i = 0; i < pedidos.Length; i++)
                        {
                            uint? id = ComissaoCalcular_tipo == Glass.Data.Model.Pedido.TipoComissao.Funcionario ? pedidos[i].IdFunc :
                                ComissaoCalcular_tipo == Glass.Data.Model.Pedido.TipoComissao.Comissionado ? pedidos[i].IdComissionado :
                                ComissaoCalcular_tipo == Glass.Data.Model.Pedido.TipoComissao.Instalador ? pedidos[i].IdInstalador : null;

                            if (!comissao.ContainsKey(id))
                            {
                                comissao.Add(id, 0);
                                desconto.Add(id, 0);
                                total.Add(id, 0);
                                pedidosFunc.Add(id, new List<uint>());
                            }

                            comissao[id] += !pedidos[i].ComissaoAPagar ? pedidos[i].ValorComissaoRecebida :
                                Glass.Configuracoes.ComissaoConfig.DescontarComissaoPerc ? pedidos[i].ValorComissaoPagar : pedidos[i].ValorBaseCalcComissao;

                            desconto[id] += pedidos[i].DescontoReal;
                            total[id] += pedidos[i].TotalSemDesconto;
                            pedidosFunc[id].Add(pedidos[i].IdPedido);
                        }

                        for (var i = 0; i < pedidos.Length; i++)
                        {
                            var id = ComissaoCalcular_tipo == Glass.Data.Model.Pedido.TipoComissao.Funcionario ? pedidos[i].IdFunc :
                                ComissaoCalcular_tipo == Glass.Data.Model.Pedido.TipoComissao.Comissionado && pedidos[i].IdComissionado != null ? pedidos[i].IdComissionado.Value :
                                ComissaoCalcular_tipo == Glass.Data.Model.Pedido.TipoComissao.Instalador && pedidos[i].IdInstalador != null ? pedidos[i].IdInstalador.Value : 0;

                            pedidos[i].ValorComissaoRpt = ComissaoCalcular_tipo == Glass.Data.Model.Pedido.TipoComissao.Funcionario ||
                                ComissaoCalcular_tipo == Glass.Data.Model.Pedido.TipoComissao.Instalador ? (Configuracoes.ComissaoConfig.DescontarComissaoPerc ? comissao[id] :
                                ComissaoConfigDAO.Instance.GetComissaoValor(pedidos[i].Total, id, 0, ComissaoCalcular_tipo)) : pedidos[i].ValorComissao;

                            pedidos[i].Comissao_DescontoTotal = desconto[id];
                            pedidos[i].Comissao_TotalPedidos = total[id];                           
                        }

                        report.DataSources.Add(new ReportDataSource("Pedido", pedidos));
                        lstParam.Add(new ReportParameter("Tipo", Request["tipo"]));
                        lstParam.Add(new ReportParameter("ExibirComissao", (Request["tipo"] == "1" ||
                            (uint.TryParse(Request["idFunc"], out ComissaoCalcular_idFunc) && ComissaoConfigDAO.Instance.IsFaixaUnica(ComissaoCalcular_idFunc))).ToString()));
                        lstParam.Add(new ReportParameter("Ordenar", Request["ordenar"]));

                        break;
                    }
                case "ResumoDiario":
                    {
                        report.ReportPath = "Relatorios/rptResumoDiario.rdlc";

                        lstParam.Add(new ReportParameter("Funcionario", login.Nome));

                        report.DataSources.Add(new ReportDataSource("ResumoDiario", new Data.RelModel.ResumoDiario[] { Glass.Data.RelDAL.ResumoDiarioDAO.Instance.GetResumoDiario(Request["data"],
                        Glass.Conversoes.StrParaUint(Request["idLoja"])) }));

                        break;
                    }
                case "ResumoDiarioCreditoGerado":
                    {
                        report.ReportPath = "Relatorios/rptResumoDiarioCreditoGerado.rdlc";

                        lstParam.Add(new ReportParameter("Funcionario", login.Nome));

                        report.DataSources.Add(new ReportDataSource("ResumoDiario", ResumoDiarioDAO.Instance.ObterResumoDiarioCreditoGerado(Request["data"],
                            Request["idLoja"].StrParaInt())));

                        break;
                    }
                case "EstoqueFiscal":
                    {
                        report.ReportPath = "Relatorios/rptEstoqueFiscal.rdlc";
                        var idLoja = Glass.Conversoes.StrParaUint(Request["idLoja"]);
                        var idGrupo = Glass.Conversoes.StrParaUint(Request["idGrupo"]);
                        var idSubgrupo = Glass.Conversoes.StrParaUint(Request["idSubgrupo"]);

                        report.DataSources.Add(new ReportDataSource("ProdutoLoja", ProdutoLojaDAO.Instance.GetForRptEstoque(idLoja,
                            Request["codProd"], Request["descr"], idGrupo, idSubgrupo,
                            Request["apenasEstoqueFiscal"] == "true", Request["apenasPosseTerceiros"] == "true", Request["apenasProdutosProjeto"] == "true",
                            Glass.Conversoes.StrParaUintNullable(Request["idCorVidro"]), Glass.Conversoes.StrParaUintNullable(Request["idCorFerragem"]),
                            Glass.Conversoes.StrParaUintNullable(Request["idCorAluminio"]), Glass.Conversoes.StrParaInt(Request["situacao"]), 1,
                            Request["aguardSaidaEstoque"] == "true", Glass.Conversoes.StrParaInt(Request["ordenacao"]))));

                        break;
                    }
                case "EstoqueReal":
                    {
                        report.ReportPath = "Relatorios/rptEstoqueReal.rdlc";
                        var idLoja = Glass.Conversoes.StrParaUint(Request["idLoja"]);
                        var idGrupo = Glass.Conversoes.StrParaUint(Request["idGrupo"]);
                        var idSubgrupo = Glass.Conversoes.StrParaUint(Request["idSubgrupo"]);

                        var estoqueReal = ProdutoLojaDAO.Instance.GetForRptEstoque(idLoja,
                            Request["codProd"], Request["descr"], idGrupo, idSubgrupo,
                            Request["apenasEstoqueFiscal"] == "true", Request["apenasPosseTerceiros"] == "true", Request["apenasProdutosProjeto"] == "true",
                            Glass.Conversoes.StrParaUintNullable(Request["idCorVidro"]), Glass.Conversoes.StrParaUintNullable(Request["idCorFerragem"]),
                            Glass.Conversoes.StrParaUintNullable(Request["idCorAluminio"]), Glass.Conversoes.StrParaInt(Request["situacao"]), 0, Request["aguardSaidaEstoque"] == "true",
                            Glass.Conversoes.StrParaInt(Request["ordenacao"]));

                        report.DataSources.Add(new ReportDataSource("ProdutoLoja", estoqueReal));

                        break;
                    }
                case "Molde":
                    {
                        report.ReportPath = "Relatorios/rptMolde.rdlc";
                        var idMolde = Glass.Conversoes.StrParaUint(Request["idMolde"]);

                        report.DataSources.Add(new ReportDataSource("Molde", new Molde[] { MoldeDAO.Instance.GetElement(idMolde) }));

                        break;
                    }
                case "ListaAntecipContaRec":
                    {
                        report.ReportPath = "Relatorios/rptListaAntecipContaRec.rdlc";
                        var idAntecipContaRec = !String.IsNullOrEmpty(Request["idAntecipContaRec"]) ? Glass.Conversoes.StrParaUint(Request["idAntecipContaRec"]) : 0;
                        var idContaBanco = !String.IsNullOrEmpty(Request["idContaBanco"]) ? Glass.Conversoes.StrParaUint(Request["idContaBanco"]) : 0;
                        var numeroNFe = !String.IsNullOrEmpty(Request["numeroNFe"]) ? Glass.Conversoes.StrParaUint(Request["numeroNFe"]) : 0;
                        var idCliente = !String.IsNullOrEmpty(Request["idCliente"]) ? Glass.Conversoes.StrParaUint(Request["idCliente"]) : 0;

                        report.DataSources.Add(new ReportDataSource("AntecipContaRec", AntecipContaRecDAO.Instance.GetForRpt(idAntecipContaRec, idContaBanco,
                            numeroNFe, idCliente, Request["nomeCliente"], Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaInt(Request["situacao"])).ToArray()));

                        break;
                    }
                case "AntecipContaRec":
                    {
                        report.ReportPath = "Relatorios/rptAntecipContaRec.rdlc";
                        var idAntecipContaRec = Glass.Conversoes.StrParaUint(Request["IdAntecipContaRec"]);

                        report.DataSources.Add(new ReportDataSource("AntecipContaRec", new AntecipContaRec[] { AntecipContaRecDAO.Instance.GetElement(idAntecipContaRec) }));
                        report.DataSources.Add(new ReportDataSource("ContasReceber", ContasReceberDAO.Instance.GetByAntecipacao(null, idAntecipContaRec).ToArray()));

                        break;
                    }
                case "ContaRecAntecip":
                    {
                        report.ReportPath = "Relatorios/rptContaRecAntecip.rdlc";

                        lstParam.Add(new ReportParameter("Agrupar", Request["agrupar"]));

                        report.DataSources.Add(new ReportDataSource("ContasReceber", ContasReceberDAO.Instance.GetAntecipForRpt(Glass.Conversoes.StrParaUint(Request["idCli"]), Request["nomeCli"],
                            Request["dtIniAntecip"], Request["dtFimAntecip"], Glass.Conversoes.StrParaInt(Request["sitAntecip"]), Glass.Conversoes.StrParaUint(Request["idContaBanco"]), float.Parse(Request["valorInicial"]),
                            Glass.Conversoes.StrParaFloat(Request["valorFinal"])).ToArray()));

                        break;
                    }
                case "MovEstoque":
                case "MovEstoqueTotal":
                case "MovEstoqueSemValor":
                case "MovEstoqueTotalComparativo":
                    {
                        switch (Request["rel"])
                        {
                            case "MovEstoque":
                                report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptMovEstoque{0}.rdlc"); break;

                            case "MovEstoqueTotal":
                                report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptMovEstoqueTotal{0}.rdlc"); break;

                            case "MovEstoqueSemValor":
                                report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptMovEstoqueSemValor{0}.rdlc"); break;

                            case "MovEstoqueTotalComparativo":
                                report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptMovEstoqueTotalComparativo{0}.rdlc"); break;

                            default: throw new Exception("Relat�rio n�o mapeado.");
                        }
                                                
                        var movEstoqueFiscal = Request["fiscal"] == "1";
                        var movEstoqueCliente = Request["cliente"] == "1";
                        MovimentacaoEstoque[] mov = null;
                        var idsGrupoProd = Request["idsGrupoProd"];
                        var idsSubgrupoProd = Request["idsSubgrupoProd"];
                        var numeroNfe = Request["numeroNfe"].StrParaInt();

                        if (String.IsNullOrEmpty(idsGrupoProd))
                            idsGrupoProd = Request["idGrupoProd"];
                        if (string.IsNullOrEmpty(idsSubgrupoProd))
                            idsSubgrupoProd = Request["idSubgrupoProd"];

                        if (Request["rel"] == "MovEstoque" || Request["rel"] == "MovEstoqueSemValor")
                            mov = MovimentacaoEstoqueDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idCliente"]),
                                Glass.Conversoes.StrParaUint(Request["idLoja"]), Request["codInterno"], Request["descricao"], Request["ncm"], numeroNfe,
                                Request["codOtimizacao"], Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaInt(Request["tipoMov"]),
                                Glass.Conversoes.StrParaInt(Request["situacaoProd"]), Glass.Conversoes.StrParaUint(Request["idCfop"]),
                                idsGrupoProd, idsSubgrupoProd,
                                Glass.Conversoes.StrParaUint(Request["idCorVidro"]), Glass.Conversoes.StrParaUint(Request["idCorFerragem"]),
                                Glass.Conversoes.StrParaUint(Request["idCorAluminio"]), movEstoqueFiscal, movEstoqueCliente,
                                Convert.ToBoolean(Request["naoBuscarEstoqueZero"]), false, null).ToArray();
                        else
                            mov = MovimentacaoEstoqueDAO.Instance.GetForRptTotal(Glass.Conversoes.StrParaUint(Request["idCliente"]),
                                Glass.Conversoes.StrParaUint(Request["idLoja"]), Request["codInterno"], Request["descricao"], Request["ncm"], numeroNfe,
                                Request["codOtimizacao"], "01/01/2000", Request["dataFim"], Glass.Conversoes.StrParaInt(Request["tipoMov"]),
                                Glass.Conversoes.StrParaInt(Request["situacaoProd"]), Glass.Conversoes.StrParaUint(Request["idCfop"]),
                                idsGrupoProd, idsSubgrupoProd,
                                Glass.Conversoes.StrParaUint(Request["idCorVidro"]), Glass.Conversoes.StrParaUint(Request["idCorFerragem"]),
                                Glass.Conversoes.StrParaUint(Request["idCorAluminio"]), movEstoqueFiscal, movEstoqueCliente,
                                Convert.ToBoolean(Request["naoBuscarEstoqueZero"]), Convert.ToBoolean(Request["usarValorFiscal"]), false, null);

                        if (Request["rel"] == "MovEstoqueTotal")
                            lstParam.Add(new ReportParameter("ExibirProdutosQtdZerada", Convert.ToBoolean(Request["naoBuscarEstoqueZero"]).ToString()));

                        if (Request["rel"] == "MovEstoqueTotalComparativo")
                        {
                            lstParam.Add(new ReportParameter("ExibirProdutosQtdZerada", Convert.ToBoolean(Request["naoBuscarEstoqueZero"]).ToString()));

                            // Recupera a quantidade de saldo do estoque fiscal ou real.
                            var movComparativo = new List<MovimentacaoEstoque>(MovimentacaoEstoqueDAO.Instance.GetForRptTotalComparativo(
                                Glass.Conversoes.StrParaUint(Request["idCliente"]), Glass.Conversoes.StrParaUint(Request["idLoja"]),
                                Request["codInterno"], Request["descricao"], numeroNfe, "01/01/2000",
                                Request["dataFim"], Glass.Conversoes.StrParaInt(Request["tipoMov"]), Glass.Conversoes.StrParaInt(Request["situacaoProd"]),
                                Glass.Conversoes.StrParaUint(Request["idCfop"]), Request["idsGrupoProd"],
                                idsSubgrupoProd, Glass.Conversoes.StrParaUint(Request["idCorVidro"]),
                                Glass.Conversoes.StrParaUint(Request["idCorFerragem"]), Glass.Conversoes.StrParaUint(Request["idCorAluminio"]), !movEstoqueFiscal,
                                Convert.ToBoolean(Request["naoBuscarEstoqueZero"]), Convert.ToBoolean(Request["usarValorFiscal"])));

                            // Salva a quantidade de estoque fiscal ou real junto com o complemento da quantidade.
                            for (var i = 0; i < mov.Length; i++)
                                for (var j = 0; j < movComparativo.Count; j++)
                                    if (mov[i].IdProd == movComparativo[j].IdProd)
                                    {
                                        // Salva a quantidade em estoque fiscal ou real do produto.
                                        mov[i].QtdeSaldoRealFiscal = movComparativo[j].QtdeSaldo;
                                        // Recupera o id do subgrupo do produto.
                                        var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)mov[i].IdProd);
                                        int tipoCalculo = GrupoProdDAO.Instance.TipoCalculo((int)mov[i].IdProd);

                                        // Caso o tipo de c�lculo do produto for por barra de alum�nio o campo complemento qtd deve ser preenchido.
                                        if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                                            tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6)
                                            mov[i].ComplementoQtdRealFiscal = " (" + Math.Round(movComparativo[j].QtdeSaldo / 6, 2) + " barras)";
                                        // Remove da lista o item que j� foi preenchido.
                                        movComparativo.RemoveAt(j);
                                        break;
                                    }
                        }

                        lstParam.Add(new ReportParameter("Fiscal", movEstoqueFiscal.ToString()));

                        if (Request["rel"] == "MovEstoqueSemValor" || Request["rel"] == "MovEstoque")
                            lstParam.Add(new ReportParameter("NumeroCasasDecimais", Glass.Configuracoes.Geral.NumeroCasasDecimaisTotM.ToString()));

                        if (Request["rel"] != "MovEstoqueTotalComparativo")
                            lstParam.Add(new ReportParameter("Cliente", movEstoqueCliente.ToString()));

                        // Caso seja requisitado o invent�rio comparativo s�o feitas altera��es na vari�vel mov, sendo assim ela deve ser adicionada ao report ap�s todas as atribui��es.
                        report.DataSources.Add(new ReportDataSource("MovimentacaoEstoque", mov));

                        break;
                    }
                case "PedidoInterno":
                    {
                        report.ReportPath = "Relatorios/rptPedidoInterno.rdlc";
                        var idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);

                        report.DataSources.Add(new ReportDataSource("PedidoInterno", new Glass.Data.Model.PedidoInterno[] { PedidoInternoDAO.Instance.GetElement(idPedido) }));
                        report.DataSources.Add(new ReportDataSource("ProdutoPedidoInterno", ProdutoPedidoInternoDAO.Instance.GetByPedidoInterno(idPedido).ToArray()));

                        break;
                    }
                case "ListaPedidoInterno":
                    {
                        if (Request["agrupar"] == "true")
                            report.ReportPath = "Relatorios/rptListaPedidoInternoAnalitico.rdlc";
                        else
                            report.ReportPath = "Relatorios/rptListaPedidoInterno.rdlc";

                        var lstPedidos = PedidoInternoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idPedido"]), Glass.Conversoes.StrParaUint(Request["idFunc"]),
                            Glass.Conversoes.StrParaUint(Request["idFuncAut"]), Request["dataIni"], Request["dataFim"]);

                        report.DataSources.Add(new ReportDataSource("PedidoInterno", lstPedidos.ToArray()));

                        report.DataSources.Add(new ReportDataSource("ProdutoPedidoInterno",
                            ProdutoPedidoInternoDAO.Instance.GetByPedidoInternoForRpt(string.Join(",", lstPedidos.Select(f => f.IdPedidoInterno))).ToArray()));

                        break;
                    }
                case "ListaProdutoPedidoInterno":
                    {
                        report.ReportPath = "Relatorios/rptListaProdutosInterno.rdlc";

                        report.DataSources.Add(new ReportDataSource("ProdutoPedidoInterno", ProdutoPedidoInternoDAO.Instance.GetForRptProdutoInterno(Request["dataIni"], Request["dataFim"],
                            Convert.ToInt32(Request["idSubGrupo"]), Convert.ToInt32(Request["idGrupo"]), Convert.ToInt32(Request["idPedido"]), Convert.ToInt32(Request["idFunc"]),
                            Convert.ToInt32(Request["idFuncReceb"])).ToArray()));

                        break;
                    }
                case "ImpressoesPendentes":
                    {
                        report.ReportPath = "Relatorios/rptImpressoesPendentes.rdlc";

                        lstParam.Add(new ReportParameter("AgruparCliente", (Request["agruparCliente"] == "true").ToString()));

                        report.DataSources.Add(new ReportDataSource("ProdutosPedidoEspelho", ProdutosPedidoEspelhoDAO.Instance.GetImprPendRpt(
                            Glass.Conversoes.StrParaUint(Request["IdPedido"]), (Glass.Conversoes.StrParaUint(Request["idCorVidro"])), Glass.Conversoes.StrParaInt(Request["espessura"]),
                            Request["dataIni"], Request["dataFim"], Request["buscarPecasDeBox"] == "true",
                            Request["buscaPecaReposta"] == "true", Glass.Conversoes.StrParaUint(Request["rota"]), Request["codProcesso"], Request["codAplicacao"], 
                            Request["idLoja"].StrParaIntNullable(), Request["sortExpression"]).ToArray()));

                        break;
                    }
                case "PedidoInstalacao":
                    {
                        report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptPedidoInstalacao{0}.rdlc");
                        var idInstalacao = Glass.Conversoes.StrParaUint(Request["idInstalacao"]);
                        var inst = InstalacaoDAO.Instance.GetElement(idInstalacao);
                        var ped = PedidoDAO.Instance.GetElement(inst.IdPedido);

                        lstParam.Add(new ReportParameter("ExibirDescrAmbiente", "true"));
                        lstParam.Add(new ReportParameter("ExibirValorProdutos", PedidoConfig.Instalacao.ExibirValorProdutosInstalacao.ToString()));

                        report.DataSources.Add(new ReportDataSource("Instalacao", new Data.Model.Instalacao[] { inst }));
                        report.DataSources.Add(new ReportDataSource("Pedido", new Glass.Data.Model.Pedido[] { ped }));
                        report.DataSources.Add(new ReportDataSource("ProdutosInstalacao", ProdutosInstalacaoDAO.Instance.GetByInstalacao(idInstalacao).ToArray()));

                        break;
                    }
                case "ProdutosComprar":
                    {
                        report.ReportPath = "Relatorios/rptProdutosComprar.rdlc";
                        var idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                        var idsPedidosEspelho = Request["buscarPedidos"] != "true" ? null :
                            PedidoEspelhoDAO.Instance.GetIdsForRpt(idPedido, Glass.Conversoes.StrParaUint(Request["idCliente"]), Request["nomeCliente"],
                            Glass.Conversoes.StrParaUint(Request["idLoja"]), Glass.Conversoes.StrParaUint(Request["idFunc"]),
                            Glass.Conversoes.StrParaUint(Request["idFuncionarioConferente"]), Glass.Conversoes.StrParaInt(Request["situacao"]),
                            Request["situacaoPedOri"], Request["idsProcesso"], Request["dataIniEnt"], Request["dataFimEnt"], Request["dataIniFab"],
                            Request["dataFimFab"], Request["dataIniFin"], Request["dataFimFin"], Request["dataIniConf"], Request["dataFimConf"], false,
                            Request["pedidosSemAnexos"].ToLower() == "true", true, null, Request["situacaoCnc"], Request["dataIniSituacaoCnc"],
                            Request["dataFimSituacaoCnc"], null, Request["idsRotas"], null, null, 0, Glass.Conversoes.StrParaInt(Request["origemPedido"]), Conversoes.StrParaInt(Request["pedidosConferidos"])).ToArray();
                        var idsPedidosEspelhoStr = idsPedidosEspelho == null || idsPedidosEspelho.Length == 0 ? null :
                            String.Join(",", Array.ConvertAll<uint, string>(idsPedidosEspelho, new Converter<uint, string>(
                                delegate (uint x) { return x.ToString(); }
                            )));
                        var lstProdPedCompra = ProdutosPedidoEspelhoDAO.Instance.GetForCompra(idPedido,
                            idsPedidosEspelhoStr, 0, Request["codInterno"], Request["descrProd"], Glass.Conversoes.StrParaUint(Request["idPedido"]), true);

                        report.DataSources.Add(new ReportDataSource("ProdutosPedidoEspelho", lstProdPedCompra));

                        break;
                    }
                case "TrocaDevolucao":
                    {
                        report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/ModeloTrocaDevolucao/rptTrocaDevolucao{0}.rdlc");
                        var idTrocaDevolucao = Glass.Conversoes.StrParaUint(Request["idTrocaDevolucao"]);
                        var trocaDev = TrocaDevolucaoDAO.Instance.GetElement(idTrocaDevolucao);

                        if (report.ReportPath.Contains("rptTrocaDevolucaoNRC"))
                        {
                            lstParam.Add(new ReportParameter("IdTrocaDev", idTrocaDevolucao.ToString()));
                            lstParam.Add(new ReportParameter("IdPedido", trocaDev.IdPedido.ToString()));
                        }

                        report.DataSources.Add(new ReportDataSource("TrocaDevolucao", new TrocaDevolucao[] { trocaDev }));
                        report.DataSources.Add(new ReportDataSource("ProdutoTrocado", ProdutoTrocadoDAO.Instance.GetByTrocaDevolucao(idTrocaDevolucao).ToArray()));
                        report.DataSources.Add(new ReportDataSource("ProdutoTrocaDevolucao", ProdutoTrocaDevolucaoDAO.Instance.GetByTrocaDevolucao(idTrocaDevolucao).ToArray()));

                        break;
                    }
                case "ListaTrocaDevolucao":
                    {
                        report.ReportPath = "Relatorios/rptListaTrocaDev.rdlc";
                        var td = TrocaDevolucaoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idTrocaDevolucao"]), Glass.Conversoes.StrParaUint(Request["idPedido"]),
                            Glass.Conversoes.StrParaInt(Request["tipo"]), Glass.Conversoes.StrParaInt(Request["situacao"]), Glass.Conversoes.StrParaUint(Request["idCli"]), Request["nomeCli"],
                            Request["idsFunc"], Request["idsFuncionarioAssociadoCliente"], Request["dataIni"], Request["dataFim"], Glass.Conversoes.StrParaUint(Request["idProduto"]),
                            Glass.Conversoes.StrParaInt(Request["alturaMin"]), Glass.Conversoes.StrParaInt(Request["alturaMax"]), Glass.Conversoes.StrParaInt(Request["larguraMin"]),
                            Glass.Conversoes.StrParaInt(Request["larguraMax"]), Glass.Conversoes.StrParaUint(Request["idOrigemTrocaDevolucao"]), Request["idTipoPerda"].StrParaUint(),
                            Request["idSetor"].StrParaInt(), Request["tipoPedido"], Request["idGrupo"].StrParaInt(), Request["idSubgrupo"].StrParaInt());

                        lstParam.Add(new ReportParameter("AgruparFunc", (Request["agrupar"].ToLower() == "true").ToString()));
                        lstParam.Add(new ReportParameter("AgruparFuncionarioAssociadoCliente",
                            (!String.IsNullOrEmpty(Request["agruparFuncionarioAssociadoCliente"]) ?
                            Request["agruparFuncionarioAssociadoCliente"].ToLower() == "true" : false).ToString()));
                        report.DataSources.Add(new ReportDataSource("TrocaDevolucao", td.ToArray()));

                        break;
                    }
                case "ControlePerdasExternas":
                    {
                        report.ReportPath = "Relatorios/rptControlePerdasExt.rdlc";
                        var pt = ProdutoTrocadoDAO.Instance.GetForRptPerdasExternas(Glass.Conversoes.StrParaUint(Request["idTrocaDevolucao"]),
                            Glass.Conversoes.StrParaUint(Request["idPedido"]), Glass.Conversoes.StrParaInt(Request["tipo"]), Glass.Conversoes.StrParaInt(Request["situacao"]),
                            Glass.Conversoes.StrParaUint(Request["idCli"]), Request["nomeCli"], Request["idsFunc"],
                            Request["idsFuncionarioAssociadoCliente"], Request["dataIni"], Request["dataFim"],
                            Glass.Conversoes.StrParaUint(Request["idProduto"]), Glass.Conversoes.StrParaInt(Request["idOrigemTrocaDevolucao"]), Request["idTipoPerda"].StrParaUint(),
                            Request["idSetor"].StrParaInt());

                        lstParam.Add(new ReportParameter("AgruparFunc", (Request["agruparFunc"].ToLower() == "true").ToString()));
                        lstParam.Add(new ReportParameter("AgruparFuncionarioAssociadoCliente",
                            (!String.IsNullOrEmpty(Request["agruparFuncionarioAssociadoCliente"]) ?
                            Request["agruparFuncionarioAssociadoCliente"].ToLower() == "true" : false).ToString()));
                        lstParam.Add(new ReportParameter("AgruparDepto", (Request["agruparDepto"].ToLower() == "true").ToString()));

                        report.DataSources.Add(new ReportDataSource("ProdutoTrocado", pt.ToArray()));

                        break;
                    }
                case "MovFunc":
                    {
                        report.ReportPath = "Relatorios/rptMovFunc.rdlc";
                        var idFunc = Glass.Conversoes.StrParaUint(Request["idFunc"]);
                        var idPedido = !String.IsNullOrEmpty(Request["idPedido"]) ? Glass.Conversoes.StrParaUint(Request["idPedido"]) : 0;
                        var idLiberarPedido = !String.IsNullOrEmpty(Request["idLiberarPedido"]) ? Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]) : 0;

                        lstParam.Add(new ReportParameter("LiberacaoPedido", PedidoConfig.LiberarPedido.ToString()));
                        lstParam.Add(new ReportParameter("SomaSaldos", MovFuncDAO.Instance.GetSomaSaldos(idFunc, idPedido, idLiberarPedido, Request["dataIni"], Request["dataFim"], Request["tipo"].StrParaInt()).ToString()));

                        report.DataSources.Add(new ReportDataSource("MovFunc", MovFuncDAO.Instance.GetForRpt(idFunc, idPedido, idLiberarPedido, Request["dataIni"], Request["dataFim"], Request["tipo"].StrParaInt()).ToArray()));

                        break;
                    }
                case "ListaDevolucaoPagto":
                    {
                        report.ReportPath = "Relatorios/rptListaDevolucaoPagto.rdlc";
                        var idCliente = !String.IsNullOrEmpty(Request["idCliente"]) ? Glass.Conversoes.StrParaUint(Request["idCliente"]) : 0;

                        report.DataSources.Add(new ReportDataSource("DevolucaoPagto", DevolucaoPagtoDAO.Instance.GetForRpt(idCliente,
                            Request["nomeCliente"], Request["dataIni"], Request["dataFim"]).ToArray()));

                        break;
                    }
                case "DevolucaoPagto":
                    {
                        report.ReportPath = "Relatorios/rptDevolucaoPagto.rdlc";
                        var idDevolucaoPagto = Glass.Conversoes.StrParaUint(Request["idDevolucaoPagto"]);

                        report.DataSources.Add(new ReportDataSource("DevolucaoPagto", new DevolucaoPagto[] { DevolucaoPagtoDAO.Instance.GetElement(idDevolucaoPagto) }));
                        report.DataSources.Add(new ReportDataSource("Cheques", ChequesDAO.Instance.GetByDevolucaoPagto(idDevolucaoPagto).ToArray()));

                        break;
                    }
                case "ClienteDescAcresc":
                    {
                        report.ReportPath = "Relatorios/rptClienteDescAcresc.rdlc";
                        var idCliente = !String.IsNullOrEmpty(Request["idCliente"]) ? Glass.Conversoes.StrParaUint(Request["idCliente"]) : 0;
                        var idVendedor = !String.IsNullOrEmpty(Request["idVendedor"]) ? Glass.Conversoes.StrParaUint(Request["idVendedor"]) : 0;
                        var idGrupoProd = !String.IsNullOrEmpty(Request["idGrupoProd"]) ? Glass.Conversoes.StrParaUint(Request["idGrupoProd"]) : 0;
                        var idSubgrupoProd = !String.IsNullOrEmpty(Request["idSubgrupoProd"]) ? Glass.Conversoes.StrParaUint(Request["idSubgrupoProd"]) : 0;
                        var idRota = Glass.Conversoes.StrParaUint(Request["idRota"]);

                        var lista = DescontoAcrescimoClienteDAO.Instance.GetForRpt(idCliente, Request["nomeCliente"],
                            idGrupoProd, idSubgrupoProd, Request["codInternoProd"], Request["descrProd"], idRota, idVendedor, Request["idLoja"].StrParaUint(), (SituacaoCliente)Enum.Parse(typeof(SituacaoCliente), Request["situacao"]));

                        report.DataSources.Add(new ReportDataSource("DescontoAcrescimoCliente", lista.ToArray()));

                        break;
                    }
                case "QuitarParcCartao":
                    {
                        report.ReportPath = "Relatorios/rptQuitarParcCartao.rdlc";

                        lstParam.Add(new ReportParameter("Agrupar", Request["agrupar"]));

                        report.DataSources.Add(new ReportDataSource("ContasReceber", ContasReceberDAO.Instance.GetParcCartaoRpt(Conversoes.StrParaUint(Request["idPedido"]),
                            Conversoes.StrParaUint(Request["idLiberarPedido"]), Conversoes.StrParaUint(Request["idAcerto"]), Conversoes.StrParaUint(Request["idLoja"]),
                            Conversoes.StrParaUint(Request["idCliente"]), Conversoes.StrParaUint(Request["tipoEntrega"]), Request["nome"], Request["dataIni"], Request["dataFim"],
                            Conversoes.StrParaUint(Request["tipoCartao"]), Conversoes.StrParaUint(Request["idAcertoCheque"]), Request["agrupar"] == "true", false,
                            Request["dataCadIni"], Request["dataCadFim"], Request["nCNI"], Request["valorIni"].StrParaDecimal(), Request["valorFim"].StrParaDecimal(),
                            Request["TipoRecebCartao"].StrParaEnum<TipoCartaoEnum>(), Request["numAutCartao"], Request["numEstabCartao"], Request["ultDigCartao"]).ToArray()));

                        break;
                    }
                case "ParcCartaoQuitadas":
                    {
                        report.ReportPath = "Relatorios/rptParcCartaoQuitadas.rdlc";

                        var parcCartao = ContasReceberDAO.Instance.GetParcCartaoRpt(Conversoes.StrParaUint(Request["idPedido"]),
                            Conversoes.StrParaUint(Request["idLiberarPedido"]), Conversoes.StrParaUint(Request["idAcerto"]), 0,
                            Conversoes.StrParaUint(Request["idCliente"]), Conversoes.StrParaUint(Request["tipoEntrega"]), Request["nome"], Request["dataIni"], Request["dataFim"],
                            0, Conversoes.StrParaUint(Request["idAcertoCheque"]), false, true,
                            "", "", Request["nCNI"], 0, 0, 0, null, null, null).ToArray();

                        report.DataSources.Add(new ReportDataSource("ContasReceber", parcCartao));

                        break;
                    }
                case "ChapaVidro":
                    {
                        report.ReportPath = "Relatorios/rptChapaVidro.rdlc";

                        var lista = ChapaVidroDAO.Instance.GetForRpt(Request["codInterno"], Request["produto"],
                            Glass.Conversoes.StrParaUint(Request["idSubgrupo"])).ToArray();

                        report.DataSources.Add(new ReportDataSource("ChapaVidro", lista));

                        break;
                    }
                case "SaidaEstoque":
                    {
                        report.ReportPath = "Relatorios/rptSaidaEstoque.rdlc";

                        var id = !String.IsNullOrEmpty(Request["idSaidaEstoque"]) ? Glass.Conversoes.StrParaUint(Request["idSaidaEstoque"]) : 0;
                        report.DataSources.Add(new ReportDataSource("SaidaEstoque", new SaidaEstoque[] { SaidaEstoqueDAO.Instance.GetElement(id) }));
                        report.DataSources.Add(new ReportDataSource("ProdutoSaidaEstoque", ProdutoSaidaEstoqueDAO.Instance.GetForRpt(id)));

                        break;
                    }
                case "ListaSaidaEstoque":
                    {
                        report.ReportPath = "Relatorios/rptListaSaidaEstoque.rdlc";
                        var idPedido = !String.IsNullOrEmpty(Request["idPedido"]) ? Glass.Conversoes.StrParaUint(Request["idPedido"]) : 0;
                        var idLiberarPedido = !String.IsNullOrEmpty(Request["idLiberarPedido"]) ? Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]) : 0;
                        var idVolume = Glass.Conversoes.StrParaUint(Request["idVolume"]);

                        report.DataSources.Add(new ReportDataSource("SaidaEstoque", SaidaEstoqueDAO.Instance.GetForRpt(idPedido, idLiberarPedido, idVolume,
                            Glass.Conversoes.StrParaUint(Request["idFunc"]), Request["dataIni"], Request["dataFim"]).ToArray()));

                        break;
                    }
                case "PecasSemSaida":
                    {
                        report.ReportPath = "Relatorios/rptPecasSemSaida" + (Glass.Configuracoes.Geral.NaoVendeVidro() ? "SemVidro" : "") + ".rdlc";

                        lstParam.Add(new ReportParameter("Liberacao", PedidoConfig.LiberarPedido.ToString()));

                        report.DataSources.Add(new ReportDataSource("ProdutosPedido", ProdutosPedidoDAO.Instance.GetForRptPecasSemSaida(Glass.Conversoes.StrParaUint(Request["idCliente"]),
                            Request["nomeCliente"], Glass.Conversoes.StrParaUint(Request["idPedido"]), Request["dataIni"], Request["dataFim"],
                            Glass.Conversoes.StrParaUint(Request["idLoja"])).ToArray()));

                        break;
                    }
                case "EntradaEstoque":
                    {
                        report.ReportPath = "Relatorios/rptEntradaEstoque.rdlc";
                        var id = !String.IsNullOrEmpty(Request["idEntradaEstoque"]) ? Glass.Conversoes.StrParaUint(Request["idEntradaEstoque"]) : 0;

                        report.DataSources.Add(new ReportDataSource("EntradaEstoque", new EntradaEstoque[] { EntradaEstoqueDAO.Instance.GetElement(id) }));
                        report.DataSources.Add(new ReportDataSource("ProdutoEntradaEstoque", Glass.Data.DAL.ProdutoEntradaEstoqueDAO.Instance.GetForRpt(id).ToArray()));

                        break;
                    }
                case "ListaEntradaEstoque":
                    {
                        report.ReportPath = "Relatorios/rptListaEntradaEstoque.rdlc";
                        var idCompra = !String.IsNullOrEmpty(Request["idCompra"]) ? Glass.Conversoes.StrParaUint(Request["idCompra"]) : 0;
                        var numeroNFe = !String.IsNullOrEmpty(Request["numNFe"]) ? Glass.Conversoes.StrParaUint(Request["numNFe"]) : 0;

                        report.DataSources.Add(new ReportDataSource("EntradaEstoque", EntradaEstoqueDAO.Instance.GetForRpt(idCompra, numeroNFe,
                            Glass.Conversoes.StrParaUint(Request["idFunc"]), Request["dataIni"], Request["dataFim"]).ToArray()));

                        break;
                    }
                case "PecasSemEntrada":
                    {
                        report.ReportPath = "Relatorios/rptPecasSemEntrada" + (Glass.Configuracoes.Geral.NaoVendeVidro() ? "SemVidro" : "") + ".rdlc";

                        report.DataSources.Add(new ReportDataSource("ProdutoEntradaEstoque", Glass.Data.RelDAL.ProdutoEntradaEstoqueDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idFornec"]),
                            Request["nomeFornec"], Glass.Conversoes.StrParaUint(Request["idCompra"]), Glass.Conversoes.StrParaUint(Request["numNFe"]), Request["dataIni"], Request["dataFim"]).ToArray()));

                        break;
                    }
                case "DescAcrescCliente":
                    {
                        report.ReportPath = "Relatorios/rptDescontoAcrescimoCliente.rdlc";

                        report.DataSources.Add(new ReportDataSource("DescontoAcrescimoCliente", DescontoAcrescimoClienteDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idCli"]),
                            Request["nomeCli"], 0, 0, null, null, 0, 0, 0, 0).ToArray()));

                        break;
                    }
                case "PrecoTabCliente":
                    {
                        report.ReportPath = ProdutoConfig.TelaPrecoTabelaClienteRelatorio.UsarRelatorioPrecoTabelClienteRetrato ?
                            Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptPrecoTabClienteRetrato{0}.rdlc") : Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptPrecoTabCliente{0}.rdlc");

                        lstParam.Add(new ReportParameter("ExibirPerc", (Request["ExibirPerc"] == "true").ToString()));

                        var precosBeneficiamento =
                            Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IBeneficiamentoFluxo>()
                                .PesquisarPrecosPadraoBeneficiamentos(string.Empty);

                        lstParam.Add(new ReportParameter("IncluirBeneficiamento", (Request["incluirBeneficiamento"] == "true").ToString()));

                        lstParam.Add(new ReportParameter("ExibirValorOriginal", (Request["exibirValorOriginal"] == "true").ToString()));

                        lstParam.Add(new ReportParameter("ExibirColunaCustoEmPrecoBeneficiamento", "true"));

                        report.DataSources.Add(new ReportDataSource("BenefConfigPreco", precosBeneficiamento.ToArray()));

                        report.DataSources.Add(new ReportDataSource("Produto", ProdutoDAO.Instance.GetForRptPrecoTab(Glass.Conversoes.StrParaUint(Request["idCli"]),
                            Request["nomeCli"], Glass.Conversoes.StrParaUint(Request["idGrupo"]), Request["idsSubgrupo"], Request["codInterno"], Request["descrProd"],
                            Glass.Conversoes.StrParaInt(Request["tipoValor"]), Glass.Conversoes.StrParaDecimal(Request["alturaInicio"]),
                            Glass.Conversoes.StrParaDecimal(Request["alturaFim"]), Glass.Conversoes.StrParaDecimal(Request["larguraInicio"]),
                            Glass.Conversoes.StrParaDecimal(Request["larguraFim"]), Glass.Conversoes.StrParaInt(Request["ordenacao"]), bool.Parse(Request["produtoDesconto"])).ToArray()));

                        break;
                    }
                case "ClientesCredito":
                    {
                        report.ReportPath = "Relatorios/rptListaClientesCredito.rdlc";

                        report.DataSources.Add(new ReportDataSource("Cliente", ClienteDAO.Instance.GetListCreditoRpt(Request["idCliente"],
                            Request["nomeCli"], null, Request["telefone"], Request["cpfCnpj"])));

                        break;
                    }
                case "LimiteChequeCpfCnpj":
                    {
                        report.ReportPath = "Relatorios/rptLimiteChequeCpfCnpj.rdlc";
                        var limites = WebGlass.Business.Cheque.Fluxo.LimiteCheque.Instance.ObtemItens(Request["cpfCnpj"], null, 0, 0);
                        report.DataSources.Add(new ReportDataSource("Limite", limites));

                        break;
                    }
                case "FornecedoresCredito":
                    {
                        report.ReportPath = "Relatorios/rptListaFornecedoresCredito.rdlc";

                        report.DataSources.Add(new ReportDataSource("Fornecedor", FornecedorDAO.Instance.GetListCreditoRpt(Glass.Conversoes.StrParaUint(Request["idFornec"]),
                            Request["nomeFornec"], Request["cpfCnpj"])));

                        break;
                    }
                case "NotaPromissoria":
                    {
                        report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/ModeloNotaPromissoria/rptNotaProm{0}.rdlc");
                        var idContaR = Glass.Conversoes.StrParaUint(Request["idContaR"]);
                        var idLiberarPedido = Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]);
                        var idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                        var idAcerto = Glass.Conversoes.StrParaUint(Request["idAcerto"]);
                        var n =
                            idAcerto > 0 ? Glass.Data.RelDAL.NotaPromissoriaDAO.Instance.GetByAcertoRenegociado(idAcerto) :
                            PedidoConfig.LiberarPedido && idLiberarPedido > 0 ? Glass.Data.RelDAL.NotaPromissoriaDAO.Instance.GetByLiberacao(idLiberarPedido) :
                            idPedido > 0 ? Glass.Data.RelDAL.NotaPromissoriaDAO.Instance.GetByPedido(idPedido) :
                            Glass.Data.RelDAL.NotaPromissoriaDAO.Instance.GetByContaReceber(idContaR);

                        var notas = new List<NotaPromissoria>();
                        for (var i = 0; i < FinanceiroConfig.DadosLiberacao.NumeroViasNotaPromissoria; i++)
                            notas.AddRange(n);

                        report.DataSources.Add(new ReportDataSource("NotaPromissoria", notas));

                        break;
                    }
                case "ContaRecebida":
                    {
                        report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptContaRecebida{0}.rdlc");

                        var lstContasReceber = new ContasReceber[] { ContasReceberDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idContaR"])) };
                        var lstCheque = ChequesDAO.Instance.GetByContaR(lstContasReceber[0].IdContaR);

                        report.DataSources.Add(new ReportDataSource("ContasReceber", lstContasReceber));
                        report.DataSources.Add(new ReportDataSource("Cheques", lstCheque.ToArray()));

                        break;
                    }
                case "ListaSubgrupo":
                    {
                        report.ReportPath = "Relatorios/rptListaSubgrupo.rdlc";
                        var lstSubGrupo = SubgrupoProdDAO.Instance.GetRptSubGrupo();

                        report.DataSources.Add(new ReportDataSource("SubgrupoProd", lstSubGrupo));

                        break;
                    }
                case "ListaRota":
                    {
                        reportName = "ListaRota";
                        break;
                    }
                case "DadosRota":
                    {
                        reportName = "DadosRota";
                        break;
                    }
                case "SugestaoCompra":
                    {
                        report.ReportPath = "Relatorios/rptSugestaoCompra.rdlc";

                        lstParam.Add(new ReportParameter("IsProducao", (Request["isProducao"] == "1").ToString()));
                        lstParam.Add(new ReportParameter("IsSugestaoPorVenda", (Request["vendasMeses"].StrParaInt() > 0).ToString()));

                        report.DataSources.Add(new ReportDataSource("Produto", ProdutoDAO.Instance.GetForSugestaoCompra(Glass.Conversoes.StrParaUint(Request["idLoja"]),
                            Request["idsProd"], Request["isProducao"] == "1", Request["vendasMeses"].StrParaInt(), Request["diasEstoque"].StrParaInt()).ToArray()));

                        break;
                    }
                case "Sinal":
                    {
                        var caminhoRelatorio = string.Format("Relatorios/ModeloSinal/rptSinal{0}.rdlc", ControleSistema.GetSite().ToString());

                        if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorio))))
                            report.ReportPath = caminhoRelatorio;
                        else
                            report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptSinal{0}.rdlc");                            

                        var idSinal = Glass.Conversoes.StrParaUint(Request["idSinal"]);

                        lstParam.Add(new ReportParameter("ExibirSaldoDevedor", FinanceiroConfig.FinanceiroRec.ExibirSaldoDevedorRelsRecebimento.ToString()));

                        report.DataSources.Add(new ReportDataSource("Sinal", new Sinal[] { SinalDAO.Instance.GetSinalDetails(idSinal) }));
                        report.DataSources.Add(new ReportDataSource("Pedido", PedidoDAO.Instance.GetBySinal(idSinal, SinalDAO.Instance.IsPagtoAntecipado(idSinal))));
                        report.DataSources.Add(new ReportDataSource("Cheques", ChequesDAO.Instance.GetBySinal(idSinal)));

                        break;
                    }
                case "SinalCompra":
                    {
                        report.ReportPath = "Relatorios/rptSinalCompra.rdlc";

                        report.DataSources.Add(new ReportDataSource("SinalCompra", new SinalCompra[] { SinalCompraDAO.Instance.GetSinalCompraDetails(null, Glass.Conversoes.StrParaUint(Request["idSinalCompra"])) }));
                        report.DataSources.Add(new ReportDataSource("Compra", CompraDAO.Instance.GetBySinalCompra(Glass.Conversoes.StrParaUint(Request["idSinalCompra"]))));
                        report.DataSources.Add(new ReportDataSource("Cheques", ChequesDAO.Instance.GetBySinalCompra(null, Glass.Conversoes.StrParaUint(Request["idSinalCompra"]))));

                        break;
                    }
                case "ImpostoServ":
                    {
                        report.ReportPath = "Relatorios/rptImpostoServ.rdlc";

                        report.DataSources.Add(new ReportDataSource("ImpostoServ", new ImpostoServ[] { ImpostoServDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idImpostoServ"])) }));

                        break;
                    }
                case "ProdutosLiberar":
                    {
                        report.ReportPath = "Relatorios/rptProdutosLiberar.rdlc";
                        var ProdutosLiberar_prodPed = ProdutosPedidoDAO.Instance.GetForLiberacao(Glass.Conversoes.StrParaUint(Request["idPedido"]));

                        report.DataSources.Add(new ReportDataSource("ProdutosPedidoRpt", Glass.Data.RelDAL.ProdutosPedidoRptDAL.Instance.CopiaLista(ProdutosLiberar_prodPed)));

                        break;
                    }
                case "ListaPassivos":
                    {
                        report.ReportPath = "Relatorios/rptListaPassivos.rdlc";

                        lstParam.Add(new ReportParameter("Detalhes", Request["detalhes"]));

                        report.DataSources.Add(new ReportDataSource("ListaPassivos", Glass.Data.RelDAL.ListaPassivosDAO.Instance.GetForRpt(
                            Glass.Conversoes.StrParaUint(Request["idLoja"]), Request["dtIni"], Request["dtFim"], Glass.Conversoes.StrParaUint(Request["idGrupoConta"]),
                            Request["planoConta"]).ToArray()));

                        break;
                    }
                case "AgendamentoInstalacao":
                    {
                        report.ReportPath = "Relatorios/rptAgendamentoInstalacoes.rdlc";

                        if (Request["tipo"] == "equipe")
                        {
                            lstParam.Add(new ReportParameter("Tipo", Request["tipo"]));

                            report.DataSources.Add(new ReportDataSource("AgendamentoInstalacao", AgendamentoInstalacaoDAO.Instance.ObterListaPorEquipe(Glass.Conversoes.StrParaUint(Request["id"]),
                                Request["dataIni"], Request["dataFim"]).ToArray()));
                        }
                        else if (Request["tipo"] == "cliente")
                        {
                            lstParam.Add(new ReportParameter("Tipo", Request["tipo"]));

                            report.DataSources.Add(new ReportDataSource("AgendamentoInstalacao", AgendamentoInstalacaoDAO.Instance.ObterListaPorCliente(Glass.Conversoes.StrParaUint(Request["id"]),
                                Request["dataIni"], Request["dataFim"]).ToArray()));
                        }

                        break;
                    }
                case "FichaClientes":
                    {
                        reportName = Request["rel"];
                        break;
                    }
                case "FichaFornecedor":
                    {
                        reportName = Request["rel"];
                        break;
                    }
                case "FichaProdutos":
                    {
                        reportName = Request["rel"];
                        break;
                    }
                case "ListaCfop":
                    {
                        report.ReportPath = "Relatorios/rptListaCfop.rdlc";

                        report.DataSources.Add(new ReportDataSource("Cfop", Glass.Data.DAL.CfopDAO.Instance.GetListForRpt()));
                        report.DataSources.Add(new ReportDataSource("NaturezaOperacao", WebGlass.Business.NaturezaOperacao.Fluxo.BuscarEValidar.Instance.ObtemParaRelatorio()));

                        break;
                    }
                case "CredFornec":
                    {
                        report.ReportPath = "Relatorios/rptCreditoFornecedor.rdlc";
                        var cred = CreditoFornecedorDAO.Instance.ObterCreditoFornecedor(Glass.Conversoes.StrParaUint(Request["idCreditoFornecedor"]));
                        var lstCheque = ChequesDAO.Instance.GetByCreditoFornecedor(Request["idCreditoFornecedor"].StrParaUint());

                        report.DataSources.Add(new ReportDataSource("CreditoFornecedor", new CreditoFornecedor[] { cred }));
                        report.DataSources.Add(new ReportDataSource("PagtoCreditoFornecedor", cred.Pagamentos));
                        report.DataSources.Add(new ReportDataSource("Cheques", lstCheque));

                        break;
                    }
                case "ListaCredFornec":
                    {
                        report.ReportPath = "Relatorios/rptListaCreditoFornecedor.rdlc";
                        var creds = CreditoFornecedorDAO.Instance.ObterListaCreditoFornecedor(Glass.Conversoes.StrParaUintNullable(Request["idCreditoFornec"]), 
                            Glass.Conversoes.StrParaUintNullable(Request["idFornecedor"]),
                            Glass.Conversoes.StrParaDate(Request["dataIni"]), Glass.Conversoes.StrParaDate(Request["dataFim"]));
                        var pg = PagtoCreditoFornecedorDAO.Instance.GetPagamentos(0);

                        report.DataSources.Add(new ReportDataSource("CreditoFornecedor", creds.ToArray()));
                        report.DataSources.Add(new ReportDataSource("PagtoCreditoFornecedor", pg));

                        break;
                    }
                case "ListaComissionado":
                    {
                        reportName = "ListaComissionado";
                        break;
                    }
                case "EntregaPorRota":
                    {
                        report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptListaEntregaPorRota{0}.rdlc");
                        var lista = Glass.Data.RelDAL.EntregaPorRotaDAO.Instance.ObterListaRpt(Glass.Conversoes.StrParaUint(Request["rotaId"]), Request["dataIni"], Request["dataFim"], Request["ids"]);

                        lstParam.Add(new Microsoft.Reporting.WebForms.ReportParameter("Motorista", Request["motorista"]));
                        lstParam.Add(new Microsoft.Reporting.WebForms.ReportParameter("Veiculo",
                            !string.IsNullOrEmpty(Request["idVeiculo"]) ? VeiculoDAO.Instance.GetElement(Request["idVeiculo"]).DescricaoCompleta : string.Empty));
                        lstParam.Add(new Microsoft.Reporting.WebForms.ReportParameter("Rota", Request["rota"]));
                        lstParam.Add(new Microsoft.Reporting.WebForms.ReportParameter("AgruparPorCidade", (Request["agruparPorCidade"] == "true").ToString()));

                        report.DataSources.Add(new ReportDataSource("EntregaPorRota", lista.ToArray()));

                        break;
                    }
                case "ListaTabelaCliente":
                    {
                        report.ReportPath = "Relatorios/rptListaClienteTabelaDesconto.rdlc";
                        var listaClienteTabela = Glass.Data.DAL.ClienteDAO.Instance.GetListForRptTabelaDesconto();

                        report.DataSources.Add(new ReportDataSource("Cliente", listaClienteTabela));

                        break;
                    }
                case "RetCartaCorrecao":
                    {
                        report.ReportPath = "Relatorios/rptRetCartaCorrecao.rdlc";
                        var dadosRetCarta = CartaCorrecaoDAO.Instance.ObterRetorno(Convert.ToUInt32(Request["idCarta"]));
                        idLojaLogotipo =
                            NotaFiscalDAO.Instance.ObtemIdLoja(
                                (uint)CartaCorrecaoDAO.Instance.ObterIdNotaFiscal(Request["idCarta"].StrParaInt()));

                        foreach (KeyValuePair<string, string> item in dadosRetCarta)
                            lstParam.Add(new ReportParameter(item.Key, item.Value));

                        break;
                    }
                case "PontoEquilibrio":
                    {
                        report.ReportPath = "Relatorios/rptPontoEquilibrio.rdlc";
                        var listPE = Glass.Data.RelDAL.PontoEquilibrioDAO.Instance.GetPontoEquilibrio(Request["dataIni"], Request["dataFim"], login);
                        var listaItens = new List<Data.RelModel.PontoEquilibrio>();

                        foreach (var p in listPE)
                            if (p.subItens.Count > 0)
                                foreach (var s in p.subItens)
                                    listaItens.Add(s);

                        lstParam.Add(new ReportParameter("DataIni", Request["dataIni"]));
                        lstParam.Add(new ReportParameter("DataFim", Request["dataFim"]));

                        report.DataSources.Add(new ReportDataSource("PontoEquilibrio", listPE));
                        report.DataSources.Add(new ReportDataSource("ItensPontoEquilibrio", listaItens));

                        break;
                    }
                case "DeclaracaoCheque":
                    {
                        report.ReportPath = "Relatorios/rptDeclaracaoCheque.rdlc";
                        var clienteDeclaracao = ClienteDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["cliente"]));
                        var nomeCnpjCli = clienteDeclaracao.Nome.ToUpper() + (clienteDeclaracao.TipoPessoa == "J" ? ", CNPJ: " : ", CPF: ") + clienteDeclaracao.CpfCnpj;
                        var textoDec = Request["texto"];
                        var lojaDec = LojaDAO.Instance.GetNome(login.IdLoja);
                        var cidadeDec = LojaDAO.Instance.GetCidade(login.IdLoja, true);
                        var idsCheque = Request["cheques"].Remove(Request["cheques"].LastIndexOf(',')).Split(',');
                        var listaChequesDec = new List<Cheques>();

                        foreach (var id in idsCheque)
                        {
                            var cheque = ChequesDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(id));
                            listaChequesDec.Add(cheque);
                        }

                        listaChequesDec.Sort(delegate (Cheques c1, Cheques c2) { return c1.DataVenc.Value.CompareTo(c2.DataVenc); });

                        lstParam.Add(new ReportParameter("Texto", textoDec));
                        lstParam.Add(new ReportParameter("Cliente", nomeCnpjCli));
                        lstParam.Add(new ReportParameter("Loja", lojaDec));
                        lstParam.Add(new ReportParameter("Cidade", cidadeDec));

                        report.DataSources.Add(new ReportDataSource("Cheques", listaChequesDec));

                        break;
                    }
                case "Exportacao":
                    {
                        report.ReportPath = "Relatorios/rptExportacao.rdlc";
                        var idExportacao = Conversoes.StrParaUint(Request["idExportacao"]);
                        Exportacao exp = ExportacaoDAO.Instance.GetElement(idExportacao);
                        report.DataSources.Add(new ReportDataSource("Exportacao", new Exportacao[] { exp }));
                        report.DataSources.Add(new ReportDataSource("PedidoExportacao", PedidoExportacaoDAO.Instance.GetForRpt(exp.IdExportacao, 0)));
                        var idsPedido = ProdutoPedidoExportacaoDAO.Instance.ObtemIdsPedidoPeloIdExportacao(idExportacao);
                        report.DataSources.Add(new ReportDataSource("ProdutosPedido", ProdutosPedidoDAO.Instance.ObterProdutosComExportados(idsPedido)));

                        break;
                    }
                case "DefinicaoCargaRota":
                    {
                        report.ReportPath = "Relatorios/rptDefinicaoCargaRota.rdlc";
                        var rota_definicaoCargaRota = RotaDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["rotaId"]));
                        var rotaDesc = Request["rota"];
                        var dataIni = Request["dataIni"];
                        var dataFim = Request["dataFim"];
                        var indices = Request["indices"];
                        var indice = new List<string>(indices.Split(','));
                        var input = DefinicaoCargaRotaDAO.Instance.ObterDados((int)rota_definicaoCargaRota.IdRota, dataIni, dataFim);
                        var output = new DefinicaoCargaRota[input.Count];

                        for (var i = 0; i < indice.Count; i++)
                        {
                            var item = input.Find(new Predicate<DefinicaoCargaRota>(
                            delegate (DefinicaoCargaRota x)
                            {
                                return x.IdCliente == Glass.Conversoes.StrParaInt(indice[i]);
                            }));

                            output[i] = item;
                        }

                        lstParam.Add(new ReportParameter("Rota", "Rota: " + rota_definicaoCargaRota.CodInterno));

                        report.DataSources.Add(new ReportDataSource("DefinicaoCargaRota", output));

                        break;
                    }
                case "RetalhosProducao":
                    {
                        report.ReportPath = "Relatorios/rptRetalhosProducao.rdlc";

                        var retalhoProducao = RetalhoProducaoDAO.Instance.GetForRpt(Request["codInterno"], Request["descrProduto"], Request["dataIni"],
                            Request["dataFim"], Request["dataUsoIni"], Request["dataUsoFim"], Glass.Conversoes.StrParaEnum<SituacaoRetalhoProducao>(Request["situacao"]),
                            Request["idsCores"], Conversoes.StrParaDouble(Request["espessura"]), Conversoes.StrParaDouble(Request["alturaInicio"]),
                            Conversoes.StrParaDouble(Request["alturaFim"]), Conversoes.StrParaDouble(Request["larguraInicio"]),
                            Conversoes.StrParaDouble(Request["larguraFim"]), Request["numEtiqueta"], Request["observacao"]).ToArray();

                        if (PCPConfig.ExibirTotalM2RetalhoCorEspessura)
                        {
                            report.ReportPath = "Relatorios/rptRetalhosProducaoCorEspessura.rdlc";
                            var retalhoPorCorEspessura = retalhoProducao
                                .Where(f => f.Situacao == SituacaoRetalhoProducao.EmUso)
                                .GroupBy(f => new { f.CorVidro, f.Espessura })
                                .Select(f => new
                                {
                                    CorVidro = f.FirstOrDefault().CorVidro,
                                    Espessura = f.Key.Espessura,
                                    TotalM2 = f.Sum(x => x.TotMUsando)
                                })
                                .OrderBy(f => f.CorVidro).ThenBy(f => f.Espessura)
                                .ToList();

                            report.DataSources.Add(new ReportDataSource("M2RetalhoCorEspessura", retalhoPorCorEspessura));
                        }

                        report.DataSources.Add(new ReportDataSource("RetalhoProducao", retalhoProducao));

                        break;
                    }
                case "EncontroContas":
                    {
                        report.ReportPath = "Relatorios/rptEncontroContas.rdlc";
                        var idEncontroContas = Glass.Conversoes.StrParaUint(Request["idEncontroContas"]);

                        report.DataSources.Add(new ReportDataSource("EncontroContas", EncontroContasDAO.Instance.GetForRpt(idEncontroContas)));
                        report.DataSources.Add(new ReportDataSource("ContasPagarEncontroContas", ContasPagarEncontroContasDAO.Instance.GetListForRpt(idEncontroContas)));
                        report.DataSources.Add(new ReportDataSource("ContasReceberEncontroContas", ContasReceberEncontroContasDAO.Instance.GetListForRpt(idEncontroContas)));

                        break;
                    }
                case "ListaEncontroContas":
                    {
                        report.ReportPath = "Relatorios/rptListaEncontroContas.rdlc";

                        report.DataSources.Add(new ReportDataSource("EncontroContas", EncontroContasDAO.Instance.GetForListRpt(Glass.Conversoes.StrParaUint(Request["idEncontroContas"]),
                            Glass.Conversoes.StrParaUint(Request["idCliente"]), Request["nomeCliente"], Glass.Conversoes.StrParaUint(Request["idFornecedor"]), Request["nomeFornecedor"],
                            Request["obs"], Glass.Conversoes.StrParaInt(Request["situacao"]), Request["dataCadIni"], Request["dataCadFim"], Glass.Conversoes.StrParaUint(Request["usuCad"]),
                            Request["dataFinIni"], Request["dataFinFim"], Glass.Conversoes.StrParaUint(Request["usuFin"]))));

                        break;
                    }
                case "ListaDepositoNaoIdentificado":
                    {
                        report.ReportPath = "Relatorios/rptListaDepositoNaoIdentificado.rdlc";

                        report.DataSources.Add(new ReportDataSource("DepositoNaoIdentificado",
                            DepositoNaoIdentificadoDAO.Instance.GetListRpt(Glass.Conversoes.StrParaUint(Request["idDepositoNaoIdentificado"]),
                            Glass.Conversoes.StrParaUint(Request["idContaBanco"]), Glass.Conversoes.StrParaDecimal(Request["valorIni"]),
                            Glass.Conversoes.StrParaDecimal(Request["ValorFim"]), Request["dataCadIni"], Request["dataCadFim"], Request["dataMovIni"],
                            Request["dataMovFim"], Glass.Conversoes.StrParaInt(Request["situacao"]))));

                        break;
                    }
                case "ListaCartaoNaoIdentificado":
                    {
                        report.ReportPath = "Relatorios/rptListaCartaoNaoIdentificado.rdlc";

                        var registros = Microsoft.Practices.ServiceLocation.ServiceLocator
                            .Current.GetInstance<Financeiro.Negocios.ICartaoNaoIdentificadoFluxo>()
                            .PesquisarCartoesNaoIdentificados(
                            Conversoes.StrParaInt(Request["idDepositoNaoIdentificado"]),
                            Conversoes.StrParaInt(Request["idContaBanco"]), 
                            Conversoes.StrParaDecimal(Request["valorIni"]),
                            Conversoes.StrParaDecimal(Request["ValorFim"]),
                            (SituacaoCartaoNaoIdentificado)Conversoes.StrParaInt(Request["situacao"]),
                            Conversoes.StrParaInt(Request["tipoCartao"]),
                            Conversoes.StrParaDate(Request["dataCadIni"]),
                            Conversoes.StrParaDate(Request["dataCadFim"]),
                            Conversoes.StrParaDate(Request["dataVendaIni"]),
                            Conversoes.StrParaDate(Request["dataVendaFim"]),
                            Request["nAutorizacao"],
                            Request["numEstabelecimento"],
                            Request["ultimosDigitosCartao"],
                            Request["codArquivo"].StrParaInt(),
                            Request["dataImportacao"].StrParaDate());

                        lstParam.Add(new ReportParameter("Criterio", registros.GetSearchParameterDescriptions().Join(" ").Format() ?? ""));
                        report.DataSources.Add(new ReportDataSource("CartaoNaoIdentificado", registros));
                        break;
                    }
                case "ListaArquivoCartaoNaoIdentificado":
                    {
                        report.ReportPath = "Relatorios/rptListaArquivoCartaoNaoIdentificado.rdlc";

                        var registros = Microsoft.Practices.ServiceLocation.ServiceLocator
                            .Current.GetInstance<Financeiro.Negocios.IArquivoCartaoNaoIdentificadoFluxo>()
                            .PesquisarArquivosCartaoNaoIdentificado(
                            (SituacaoArquivoCartaoNaoIdentificado)Conversoes.StrParaInt(Request["situacao"]),                            
                            Conversoes.StrParaDate(Request["dataCadIni"]),
                            Conversoes.StrParaDate(Request["dataCadFim"]),                            
                            Request["nomeFunc"]);

                        lstParam.Add(new ReportParameter("Criterio", registros.GetSearchParameterDescriptions().Join(" ").Format() ?? ""));
                        report.DataSources.Add(new ReportDataSource("ArquivoCartaoNaoIdentificado", registros));
                        break;
                    }
                case "CorEspessura":
                    {
                        report.ReportPath = "Relatorios/rptCorEspessura.rdlc";
                        var lstPed = PedidoDAO.Instance.GetForRpt(Request["idsPedido"], false);
                        var lstProdPedRpt = new List<ProdutosPedidoRpt>();
                        var lstPedidoUsar = new List<Glass.Data.Model.Pedido>();

                        foreach (var p in lstPed)
                        {
                            var lstProdutosPedidosRpt = ProdutosPedidoRptDAL.Instance.CopiaLista(ProdutosPedidoDAO.Instance.GetForRpt(p.IdPedido, false));

                            foreach (var ppr in lstProdutosPedidosRpt)
                                lstProdPedRpt.Add(ppr);

                            lstPedidoUsar.Add(p);
                        }

                        report.DataSources.Add(new ReportDataSource("Pedido", lstPedidoUsar.ToArray()));
                        report.DataSources.Add(new ReportDataSource("ProdutosPedidoRpt", lstProdPedRpt.ToArray()));

                        break;
                    }
                case "CotacaoCompraCalculada":
                    {
                        report.ReportPath =
                            Request["exibirValor"] == "true" ?
                                "Relatorios/rptCotacaoCompraCalculada.rdlc" : "Relatorios/rptCotacaoCompraCalculadaFornec.rdlc";

                        var prioridadesCalculo = WebGlass.Business.CotacaoCompra.Fluxo.CalcularCotacao.Instance.ObtemTiposCalculoCotacao();
                        var cnpj = string.Empty;
                        var nomeFantasia = string.Empty;
                        var endereco = string.Empty;
                        var telefone = String.Empty;
                        var cotacaoCompra = WebGlass.Business.CotacaoCompra.Fluxo.CalcularCotacao.Instance.Calcular(null,
                            Glass.Conversoes.StrParaUint(Request["id"]), (Glass.Data.Model.CotacaoCompra.TipoCalculoCotacao)Glass.Conversoes.StrParaInt(Request["tipo"]), true);
                        var idLoja = cotacaoCompra != null && cotacaoCompra[0].CodigoCotacaoCompra > 0 ?
                            FuncionarioDAO.Instance.ObtemIdLoja(
                                (uint)CotacaoCompraDAO.Instance.ObtemIdFuncFin(
                                    cotacaoCompra[0].CodigoCotacaoCompra).GetValueOrDefault()) : 0;

                        if (Request["exibirValor"] != null && Request["exibirValor"].ToLower() != "true")
                        {
                            if (idLoja > 0)
                            {
                                cnpj = LojaDAO.Instance.ObtemCnpj(idLoja);
                                endereco = LojaDAO.Instance.ObtemEnderecoCompleto(idLoja);
                                nomeFantasia = LojaDAO.Instance.GetNome(idLoja);
                                telefone = LojaDAO.Instance.ObtemTelefone(null, idLoja);
                            }

                            lstParam.Add(new ReportParameter("LojaCnpj", cnpj));
                            lstParam.Add(new ReportParameter("LojaNome", nomeFantasia));
                            lstParam.Add(new ReportParameter("LojaEndereco", endereco));
                            lstParam.Add(new ReportParameter("LojaTelefone", telefone));
                        }

                        lstParam.Add(new ReportParameter("TipoCalculo", Array.Find(prioridadesCalculo, x => x.Id == Glass.Conversoes.StrParaInt(Request["tipo"])).Descr));
                        report.DataSources.Add(new ReportDataSource("CotacaoCompraCalculada", cotacaoCompra));

                        break;
                    }
                case "CotacaoCompraFornecedor":
                    {
                        report.ReportPath = "Relatorios/rptCotacaoCompraCalculada.rdlc";

                        report.DataSources.Add(new ReportDataSource("CotacaoCompraCalculada", WebGlass.Business.CotacaoCompra.Fluxo.CotacaoPorFornecedor.Instance.ObtemDados(
                            Glass.Conversoes.StrParaUint(Request["id"]), true)));

                        break;
                    }
                case "Volume":
                    {
                        report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/OrdemCarga/rptVolume{0}.rdlc");                        
                        var pedidosVolume = PedidoDAO.Instance.GetForGeracaoVolumeRpt(Request["idPedido"].StrParaUint(), 0, null, 0, null, null, null, null, null, null, 0, 0, null, null);
                        var volumes = VolumeDAO.Instance.GetListForRpt(0, null, pedidosVolume[0].IdPedido, 0, null, null, null, null, 0, null);
                        var volumeProdutos = VolumeProdutosPedidoDAO.Instance.GetListForRpt(string.Join(",", volumes.Select(p => p.IdVolume.ToString()).ToArray()));

                        report.DataSources.Add(new ReportDataSource("Pedido", pedidosVolume));
                        report.DataSources.Add(new ReportDataSource("Volume", volumes));
                        report.DataSources.Add(new ReportDataSource("VolumeProdutosPedido", volumeProdutos));

                        break;
                    }
                case "OrdemCarga":
                    {
                        report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/OrdemCarga/rptOrdemCarga{0}.rdlc");                        
                        var oc = WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo.Instance.GetForRptInd(Glass.Conversoes.StrParaUint(Request["idOrdemCarga"]));
                        var pedidosOC = oc[0].Pedidos.ToArray();
                        var produtosOC = ProdutosPedidoDAO.Instance.GetByPedidosForOcRpt(oc[0].IdOrdemCarga, oc[0].IdsPedidos);

                        lstParam.Add(new ReportParameter("ExibirEnderecoCliente", OrdemCargaConfig.ExibirEnderecoClienteRptOC.ToString()));

                        report.DataSources.Add(new ReportDataSource("OrdemCarga", oc));
                        report.DataSources.Add(new ReportDataSource("Pedido", pedidosOC));
                        report.DataSources.Add(new ReportDataSource("ProdutosPedido", produtosOC));

                        break;
                    }
                case "ListaCarregamento":
                    {
                        report.ReportPath = "Relatorios/OrdemCarga/rptListaCarregamentos.rdlc";
                        var carregamentos = WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Instance.GetListForRpt(Glass.Conversoes.StrParaUint(Request["carregamento"]),
                            Request["idOC"].StrParaUint(), Request["idPedido"].StrParaUint(), Request["idRota"].StrParaInt(), Request["motorista"].StrParaUint(), Request["placa"],
                            Request["situacao"], Request["dtPrevSaidaIni"], Request["dtPrevSaidaFim"], Glass.Conversoes.StrParaUint(Request["idLoja"]));

                        report.DataSources.Add(new ReportDataSource("Carregamento", carregamentos));

                        break;
                    }
                case "ListaOrdemCarga":
                    {
                        report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/OrdemCarga/rptListaOrdemCarga{0}.rdlc");                        
                        var ocs = WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo.Instance.GetListForRpt(Glass.Conversoes.StrParaUint(Request["idCarregamento"]),
                            Glass.Conversoes.StrParaUint(Request["idOC"]), Glass.Conversoes.StrParaUint(Request["idPedido"]), Glass.Conversoes.StrParaUint(Request["idCli"]),
                            Request["nomeCli"], Glass.Conversoes.StrParaUint(Request["idLoja"]), Glass.Conversoes.StrParaUint(Request["idRota"]), Request["dtEntPedIni"],
                            Request["dtEntPedFin"], Request["situacao"], Request["tipo"], Request["idClienteExterno"].StrParaUint(), Request["nomeClienteExterno"], Request["codRotasExternas"]);

                        report.DataSources.Add(new ReportDataSource("OrdemCarga", ocs));

                        break;
                    }
                case "PendenciaCarregamento":
                    {
                        report.ReportPath = "Relatorios/OrdemCarga/rptPendenciaCarregamento.rdlc";
                        var idCarregamento = Glass.Conversoes.StrParaUint(Request["idCarregamento"]);
                        var idCliCarregamento = Glass.Conversoes.StrParaUint(Request["idCliente"]);
                        var idLojaCarregamento = Glass.Conversoes.StrParaUint(Request["idLoja"]);
                        var dataSaidaIni = Request["dataSaidaIni"];
                        var dataSaidaFim = Request["dataSaidaFim"];
                        var rotas = Request["rotas"];
                        var cliCarregamento = new List<Cliente>() { ClienteDAO.Instance.GetElement(idCliCarregamento) };
                        var itensCarregamentoPendencia = ItemCarregamentoDAO.Instance.GetItensPendentes(idCarregamento, 0, idCliCarregamento, null, idLojaCarregamento,
                            null, null, false, dataSaidaIni, dataSaidaFim, rotas, false, Request["idClienteExterno"].StrParaUint(), Request["nomeClienteExterno"], Request["codRotasExternas"]);
                        var itensVolumes = itensCarregamentoPendencia.Where(x => x.TipoItem == (long)ItemCarregamento.TipoItemCarregamento.Volume);
                        var itensVenda = itensCarregamentoPendencia.Where(x => x.TipoItem == (long)ItemCarregamento.TipoItemCarregamento.Venda);
                        var itensRevenda = itensCarregamentoPendencia.Where(x => x.TipoItem == (long)ItemCarregamento.TipoItemCarregamento.Revenda);

                        lstParam.Add(new ReportParameter("IdCarregamento", idCarregamento.ToString()));

                        report.DataSources.Add(new ReportDataSource("Cliente", cliCarregamento));
                        report.DataSources.Add(new ReportDataSource("ItensVolumes", itensVolumes));
                        report.DataSources.Add(new ReportDataSource("ItensVenda", itensVenda));
                        report.DataSources.Add(new ReportDataSource("ItensRevenda", itensRevenda));

                        break;
                    }
                case "Carregamento":
                    {
                        report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/OrdemCarga/rptCarregamento{0}.rdlc");

                        var idCarregamentoRpt = Glass.Conversoes.StrParaUint(Request["idCarregamento"]);
                        var carregamento = CarregamentoDAO.Instance.GetListForRpt(idCarregamentoRpt, 0, 0, 0, 0, null, null, null, null, 0);
                        var ocsCarregamento = OrdemCargaDAO.Instance.GetOCsForCarregamento(idCarregamentoRpt);
                        var pedidosCarregamento = new List<Glass.Data.Model.Pedido>();
                        var cidadesCarregamento = CarregamentoDAO.Instance.ObtemCidadesCarregamento(ocsCarregamento);

                        foreach (var oc in ocsCarregamento)
                        {
                            var p = oc.Pedidos.Where(f => f.Importado).ToList();
                            p.ForEach(f => f.IdOrdemCarga = oc.IdOrdemCarga);
                            pedidosCarregamento.AddRange(p);
                        }

                        lstParam.Add(new ReportParameter("ExibirNomeFantasiaCliente", OrdemCargaConfig.ExibirNomeFantasiaClienteRptCarregamento.ToString()));
                        lstParam.Add(new ReportParameter("Ordenar", Request["Ordenar"]));

                        report.DataSources.Add(new ReportDataSource("Carregamento", carregamento));
                        report.DataSources.Add(new ReportDataSource("OrdemCarga", ocsCarregamento));
                        report.DataSources.Add(new ReportDataSource("Pedido", pedidosCarregamento));
                        report.DataSources.Add(new ReportDataSource("CidadesCarregamento", cidadesCarregamento));

                        break;
                    }
                case "InventarioEstoque":
                    {
                        report.ReportPath = "Relatorios/rptInventarioEstoque.rdlc";
                        var inventario = WebGlass.Business.InventarioEstoque.Fluxo.CRUD.Instance.ObtemItem(Glass.Conversoes.StrParaUint(Request["id"]));

                        report.DataSources.Add(new ReportDataSource("InventarioEstoque", new[] { inventario }));
                        report.DataSources.Add(new ReportDataSource("ProdutoInventarioEstoque", inventario.Produtos));

                        break;
                    }
                case "PrecoFornecedor":
                    {
                        reportName = "PrecoFornecedor";
                        break;
                    }
                case "EtiquetaBox":
                    {
                        report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/ModeloEtiquetaBox/rptEtiquetaBox{0}.rdlc");

                        var prodPed = new List<ProdutosPedido>();
                        var lstPecasBox = new List<Etiqueta>();
                        var idsProdPedQtd = "";

                        var reimpressao = !string.IsNullOrEmpty(Request["idImpressao"]);

                        if (reimpressao)
                            idsProdPedQtd = ProdutoImpressaoDAO.Instance.ObtemQtdeBoxImpresso(Glass.Conversoes.StrParaInt(Request["idImpressao"]));
                        else
                            idsProdPedQtd = Request["idsProdPedQtd"].ToString().TrimEnd('|');


                        foreach (var prod in idsProdPedQtd.Split('|'))
                        {
                            prodPed.Add(ProdutosPedidoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(prod.Split(';')[0])));
                            var qtdImprimir = Glass.Conversoes.StrParaInt(prod.Split(';')[1]);

                            /* Chamado 15816.
                             * Haviam v�rias impress�es de box sem produtos, criamos esta verifica��o para impedir que isto ocorra novamente. */
                            if (qtdImprimir == 0)
                                continue;

                            var idPedido = Glass.Conversoes.StrParaUint(prodPed[prodPed.Count - 1].IdPedido.ToString());
                            var codPedCli = PedidoDAO.Instance.ObtemPedCli(idPedido);
                            var dataEntrega = PedidoDAO.Instance.ObtemDataEntrega(idPedido);
                            var idCliente = PedidoDAO.Instance.ObtemIdCliente(idPedido);
                            var nomeCliente = ClienteDAO.Instance.GetNome(idCliente);
                            var codRota = RotaDAO.Instance.ObtemCodRota(idCliente);
                            var descrRota = RotaDAO.Instance.ObtemDescrRota(idCliente);
                            var nomeCidade = CidadeDAO.Instance.GetNome(ClienteDAO.Instance.ObtemIdCidade(null, idCliente));
                            var codInternoProd = ProdutoDAO.Instance.GetCodInterno((int)prodPed[prodPed.Count - 1].IdProd);
                            var descrProd = ProdutoDAO.Instance.ObtemDescricao((int)prodPed[prodPed.Count - 1].IdProd);
                            var altura = Glass.Conversoes.StrParaInt(prodPed[prodPed.Count - 1].Altura.ToString());
                            var largura = Glass.Conversoes.StrParaInt(prodPed[prodPed.Count - 1].Largura.ToString());
                            var alturaLargura = altura + " X " + largura;
                            var descrAlturaLargura = "Altura X Largura";
                            var codAplicacao = prodPed[prodPed.Count - 1].CodAplicacao;
                            var codProcesso = prodPed[prodPed.Count - 1].CodProcesso;
                            var tipoEntrega = Glass.Data.Helper.Utils.GetDescrTipoEntrega(PedidoDAO.Instance.ObtemTipoEntrega(idPedido));

                            for (var i = 0; i < qtdImprimir; i++)
                            {
                                var pecaBox = new Etiqueta();
                                pecaBox.BarCodeData = idPedido.ToString();
                                pecaBox.IdPedido = idPedido.ToString();
                                pecaBox.IdProdPedBox = (int)prodPed[prodPed.Count - 1].IdProdPed;
                                pecaBox.CodCliente = codPedCli;
                                pecaBox.DataEntrega = dataEntrega;
                                pecaBox.IdCliente = idCliente;
                                pecaBox.NomeCliente = nomeCliente;
                                pecaBox.CodRota = codRota;
                                pecaBox.DescrRota = descrRota;
                                pecaBox.NomeCidade = nomeCidade;
                                pecaBox.CodInterno = codInternoProd;
                                pecaBox.DescrProd = descrProd;
                                pecaBox.AlturaLargura = alturaLargura;
                                pecaBox.DescrAlturaLargura = descrAlturaLargura;
                                pecaBox.CodApl = codAplicacao;
                                pecaBox.CodProc = codProcesso;
                                pecaBox.DescrTipoEntrega = tipoEntrega;
                                pecaBox.Altura = altura.ToString();
                                pecaBox.Largura = largura.ToString();

                                lstPecasBox.Add(pecaBox);
                            }
                        }

                        if (lstPecasBox == null || lstPecasBox.Count == 0)
                            throw new Exception("Informe a quantidade a ser impressa.");

                        if (!reimpressao)
                        {
                            var idImpressao = ImpressaoEtiquetaDAO.Instance.NovaImpressaoBox(UserInfo.GetUserInfo.CodUser, lstPecasBox);
                            ImpressaoEtiquetaDAO.Instance.AtualizaSituacao(null, idImpressao, ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa);
                        }

                        report.DataSources.Add(new ReportDataSource("Etiqueta", lstPecasBox));

                        break;
                    }
                case "ConciliacaoBancaria":
                    {
                        lstParam.Add(new ReportParameter("ExibirLinhaVermelhaSaida", "true"));

                        report.ReportPath = "Relatorios/rptConciliacaoBancaria.rdlc";
                        report.DataSources.Add(new ReportDataSource("ConciliacaoBancaria", new[] { ConciliacaoBancariaDAO.Instance.ObtemElemento(Glass.Conversoes.StrParaUint(Request["id"])) }));
                        report.DataSources.Add(new ReportDataSource("MovBanco", Glass.Data.DAL.MovBancoDAO.Instance.ObtemMovimentacoesDaConciliacao(Glass.Conversoes.StrParaUint(Request["id"]))));

                        break;
                    }
                case "MotivoFinalizacaoFinanceiro":
                    {
                        report.ReportPath = "Relatorios/rptMotivoFinalizacaoFinanceiro.rdlc";

                        report.DataSources.Add(new ReportDataSource("ObservacaoFinalizacaoFinanceiro", Glass.Data.DAL.ObservacaoFinalizacaoFinanceiroDAO.
                            Instance.ObtemParaRelatorio(Request["idPedido"].StrParaUint(), Request["numCli"].StrParaUint(), Request["nomeCli"],
                            Request["idFunc"].StrParaUint(), Request["dataIni"], Request["dataFim"], Request["motivo"])));

                        break;
                    }
                case "ProdutosCaixa":
                    {
                        report.ReportPath = "Relatorios/rptProdutosCaixa.rdlc";
                        var idLoja = Glass.Conversoes.StrParaUint(Request["idLoja"]);
                        var idsPedEsp = PedidoEspelhoDAO.Instance.GetIdsForRpt(Glass.Conversoes.StrParaUint(Request["idPedido"]),
                            Glass.Conversoes.StrParaUint(Request["idCliente"]), Request["nomeCliente"], idLoja, 0, 0,
                            Glass.Conversoes.StrParaInt(Request["situacao"]), Request["situacaoPedOri"], null, Request["dtEntIni"], Request["dtEntFim"], null, null, Request["dataIniFin"],
                            Request["dataFimFin"], null, null, false, false, false, null, null, null, null, null, Request["idsRota"], Request["dtCompraIni"], Request["dtCompraFim"],
                            Glass.Conversoes.StrParaInt(Request["idCompra"])).ToArray();
                        var idsPedEspStr = "";
                        var compraGerada = Request["compraGerada"];
                        var ordenarPor = Glass.Conversoes.StrParaInt(Request["ordenarPor"]);

                        // Chamado 9519, feito para que, caso o usu�rio filtre os pedidos pela loja, exiba no relat�rio a logomarca da loja filtrada.
                        idLojaLogotipo = idLoja > 0 ? (uint?)idLoja : null;

                        if (String.IsNullOrEmpty(compraGerada) || compraGerada == "0")
                        {
                            idsPedEspStr = idsPedEsp == null || idsPedEsp.Length == 0 ? null :
                            String.Join(",", Array.ConvertAll<uint, string>(idsPedEsp, new Converter<uint, string>(
                                delegate (uint x) { return x.ToString(); }
                            )));
                        }
                        else
                        {
                            foreach (var id in idsPedEsp)
                            {
                                if (compraGerada == "1" && PedidosCompraDAO.Instance.PossuiCompraProdBenefGerada(id))
                                    idsPedEspStr += id.ToString() + ",";
                                else if (compraGerada == "2" && PedidosCompraDAO.Instance.PossuiCompraProdBenefGerada(id))
                                    idsPedEspStr += id.ToString() + ",";
                            }
                        }

                        idsPedEspStr = idsPedEspStr.TrimEnd(',');
                        
                        var lstProdPedCaixa = new List<ProdutosPedidoEspelho>();
                        var contador = 1;

                        foreach (var prodCaixa in ProdutosPedidoEspelhoDAO.Instance.GetListCompraProdBenef(idsPedEspStr, 0, null, 0, 0, null, null, null, null, ordenarPor == 1 ? "DataEntrega, IdPedido" : null, 0, 0).ToList())
                        {
                            if ((prodCaixa.Beneficiamentos == null || prodCaixa.Beneficiamentos.Count == 0) || prodCaixa.IdProd == 0)
                                continue;
                            
                            foreach (var benef in prodCaixa.Beneficiamentos)
                            {
                                var idProdCx = BenefConfigDAO.Instance.ObtemIdProd(benef.IdBenefConfig).GetValueOrDefault();

                                if (idProdCx > 0)
                                {
                                    var benefConfig = BenefConfigDAO.Instance.GetElementByPrimaryKey(benef.IdBenefConfig);
                                    var prodPedCaixa = new ProdutosPedidoEspelho();

                                    prodPedCaixa.DescrProduto = ProdutoDAO.Instance.ObtemDescricao((int)prodCaixa.IdProd) + "|" +
                                        (benefConfig.Descricao.IndexOf("MM") > 0 ?
                                        benefConfig.Descricao.Remove(benefConfig.Descricao.IndexOf("MM") - 3) +
                                        "|" + benefConfig.Descricao.Substring(benefConfig.Descricao.IndexOf("MM") - 2) : "|");

                                    prodPedCaixa.IdProd = (uint)benefConfig.IdProd.GetValueOrDefault();
                                    prodPedCaixa.Qtde = prodCaixa.Qtde;
                                    prodPedCaixa.Altura = prodCaixa.Altura;
                                    prodPedCaixa.Largura = prodCaixa.Largura;
                                    prodPedCaixa.Espessura = prodCaixa.Espessura;
                                    prodPedCaixa.CompraGerada = PedidosCompraDAO.Instance.GetValoresCampo(@"Select pc.* From pedidos_compra pc where
                                        coalesce(pc.produtoBenef, false) And pc.idPedido=" + prodCaixa.IdPedido, "idCompra", ",", null);
                                    prodPedCaixa.IdPedido = prodCaixa.IdPedido;

                                    var idClienteCx = PedidoDAO.Instance.ObtemIdCliente(prodPedCaixa.IdPedido);
                                    var nomeClienteCx = ClienteDAO.Instance.GetNomeByPedido(prodPedCaixa.IdPedido);

                                    prodPedCaixa.IdCliente = idClienteCx;
                                    prodPedCaixa.NomeCliente = nomeClienteCx;
                                    prodPedCaixa.RotaCliente = RotaDAO.Instance.ObtemCodRota(idClienteCx);

                                    prodPedCaixa.Obs = contador++.ToString();

                                    lstProdPedCaixa.Add(prodPedCaixa);
                                }
                            }
                        }

                        report.DataSources.Add(new ReportDataSource("ProdutosPedidoEspelho", lstProdPedCaixa));

                        break;
                    }
                case "ListaFinalizarPedidoFinanceiro":
                    {
                        report.ReportPath = "Relatorios/rptListaFinalizarPedidoFinanceiro.rdlc";
                        var pedidos_ListaFinalizarPedidoFinanceiro = PedidoDAO.Instance.ObtemItensFinalizarFinanceiroRpt(Glass.Conversoes.StrParaUint(Request["numPedido"]),
                            Request["numPedidoCli"], Glass.Conversoes.StrParaUint(Request["numCli"]), Request["nomeCli"], Glass.Conversoes.StrParaUint(Request["numOrca"]),
                            Request["endereco"], Request["bairro"], Request["dataPedidoIni"], Request["dataPedidoFim"], Glass.Conversoes.StrParaUint(Request["loja"]),
                            Glass.Conversoes.StrParaInt(Request["situacao"]), Glass.Conversoes.StrParaFloat(Request["altura"]), Glass.Conversoes.StrParaInt(Request["largura"]));

                        report.DataSources.Add(new ReportDataSource("Pedido", pedidos_ListaFinalizarPedidoFinanceiro));

                        break;
                    }
                case "ListaCTe":
                    {
                        report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/rptListaCTe{0}.rdlc");
                        var ctes = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.GetListForRpt(Glass.Conversoes.StrParaInt(Request["numCte"]), 0, Request["situacao"],
                            Glass.Conversoes.StrParaUint(Request["cfop"]), Glass.Conversoes.StrParaIntNullable(Request["formaPagto"]), Glass.Conversoes.StrParaInt(Request["tipoEmissao"]),
                            Glass.Conversoes.StrParaIntNullable(Request["tipoCte"]), Glass.Conversoes.StrParaIntNullable(Request["tipoServico"]), Request["dataIni"],
                            Request["dataFim"], Glass.Conversoes.StrParaUint(Request["idTransportador"]), Glass.Conversoes.StrParaInt(Request["ordenar"]),
                            Glass.Conversoes.StrParaUint(Request["tipoRemetente"]), Glass.Conversoes.StrParaUint(Request["idRemetente"]),
                            Glass.Conversoes.StrParaUint(Request["tipoDestinatario"]), Glass.Conversoes.StrParaUint(Request["idDestinatario"]),
                            Glass.Conversoes.StrParaUint(Request["tipoRecebedor"]), Glass.Conversoes.StrParaUint(Request["idRecebedor"]));

                        report.DataSources.Add(new ReportDataSource("ConhecimentoTransporte", ctes));

                        break;
                    }
                case "ListaPendenciaCarregamento":
                    {
                        report.ReportPath = "Relatorios/OrdemCarga/rptListaPendenciaCarregamento.rdlc";
                        var itensPendentesCarregamento = ItemCarregamentoDAO.Instance.GetItensPendentes(Glass.Conversoes.StrParaUint(Request["idCarregamento"]),
                            0, Glass.Conversoes.StrParaUint(Request["idCliente"]), Request["nomeCliente"], Glass.Conversoes.StrParaUint(Request["idLoja"]),
                            null, null, true, Request["dataSaidaIni"], Request["dataSaidaFim"], Request["rotas"], Request["ignorarPedidoVendaTransferencia"].ToLower() == "true",
                            Request["idClienteExterno"].StrParaUint(), Request["nomeClienteExterno"], Request["codRotasExternas"]);

                        report.DataSources.Add(new ReportDataSource("ItemCarregamento", itensPendentesCarregamento));

                        break;
                    }
                case "DIPJEntradaFornecedores":
                case "DIPJEntradaProdutos":
                case "DIPJSaidaClientes":
                case "DIPJSaidaProdutos":
                    {
                        report.ReportPath = "Relatorios/DIPJ/rptDIPJ.rdlc";
                        var dataEmissaoInicial = Request["dataEmissaoInicial"].StrParaDate().GetValueOrDefault();
                        var dataEmissaoFinal = Request["dataEmissaoFinal"].StrParaDate().GetValueOrDefault();


                        string titulo = null;
                        string tituloColunaId = null;
                        string tituloColunaNome = "Nome";
                        IEnumerable<Dipj> dados = null;

                        switch (Request["rel"])
                        {
                            case "DIPJEntradaFornecedores":
                                titulo = "DIPJ (Entrada de Fornecedores)";
                                tituloColunaId = "Identifica��o";
                                dados = Glass.Data.RelDAL.DipjDAO.Instance
                                         .ObtemEntradaFornecedores(dataEmissaoInicial, dataEmissaoFinal, 0);
                                break;
                            case "DIPJEntradaProdutos":
                                titulo = "DIPJ (Entrada de Produtos)";
                                tituloColunaId = "Classifica��o Fiscal";
                                tituloColunaNome = "Mercadoria";
                                dados =
                                    Glass.Data.RelDAL.DipjDAO.Instance
                                         .ObtemEntradaProdutos(dataEmissaoInicial, dataEmissaoFinal, 0);
                                break;
                            case "DIPJSaidaProdutos":
                                titulo = "DIPJ (Sa�da de Produtos)";
                                tituloColunaId = "Classifica��o Fiscal";
                                tituloColunaNome = "Mercadoria";
                                dados = Glass.Data.RelDAL.DipjDAO.Instance
                                         .ObtemSaidaProdutos(dataEmissaoInicial, dataEmissaoFinal, 0);
                                break;
                            case "DIPJSaidaClientes":
                                titulo = "DIPJ (Sa�da de Clientes)";
                                tituloColunaId = "Identifica��o";
                                tituloColunaNome = "Nome";
                                dados = Glass.Data.RelDAL.DipjDAO.Instance
                                         .ObtemSaidaClientes(dataEmissaoInicial, dataEmissaoFinal, 0);
                                break;
                        }

                        lstParam.Add(new ReportParameter("Titulo", titulo));
                        lstParam.Add(new ReportParameter("TituloColunaId", tituloColunaId));
                        lstParam.Add(new ReportParameter("TituloColunaNome", tituloColunaNome));
                        lstParam.Add(new ReportParameter("DataEmissaoInicial", dataEmissaoInicial.ToString("dd/MM/yyyy")));
                        lstParam.Add(new ReportParameter("DataEmissaoFinal", dataEmissaoFinal.ToString("dd/MM/yyyy")));

                        report.DataSources.Add(new ReportDataSource("Dipj", dados));

                        break;
                    }
                case "AcertoChequesDevolvidos":
                    {
                        report.ReportPath = "Relatorios/rptAcertoChequesDevolvidosAbertos.rdlc";
                        var dados = Glass.Data.DAL.AcertoChequeDAO.Instance.GetListForRpt(Glass.Conversoes.StrParaUint(Request["idAcertoCheque"]),
                            Glass.Conversoes.StrParaUint(Request["idFunc"]), Glass.Conversoes.StrParaUint(Request["idCliente"]),
                            Request["nomeCliente"], Request["dataIni"], Request["dataFim"], Request["chequesProprios"], Request["chequesCaixaDiario"] == "true");
                        var lstChequesAcertados = new List<Cheques>();

                        foreach (var acerto in dados)
                            foreach (var chequeAcertado in ChequesDAO.Instance.GetByAcertoCheque(acerto.IdAcertoCheque, true))
                                lstChequesAcertados.Add(chequeAcertado);

                        report.DataSources.Add(new ReportDataSource("AcertoCheque", dados));
                        report.DataSources.Add(new ReportDataSource("ChequesAcertados", lstChequesAcertados));

                        break;
                    }
                case "TempoGastoParaLiberacao":
                    {
                        report.ReportPath = "Relatorios/rptTempoGastoParaLiberacao.rdlc";
                        uint ProducaoSituacaoData_idPedido = !String.IsNullOrEmpty(Request["idPedido"]) ? Glass.Conversoes.StrParaUint(Request["idPedido"]) : 0;
                        uint ProducaoSituacaoData_idCliente = !String.IsNullOrEmpty(Request["idCliente"]) ? Glass.Conversoes.StrParaUint(Request["idCliente"]) : 0;
                        report.DataSources.Add(new ReportDataSource("ProducaoSituacaoData", ProducaoSituacaoDataDAO.Instance.GetForRptTempoLiberacao(Request["dataIni"],
                            Request["dataFim"], ProducaoSituacaoData_idPedido, ProducaoSituacaoData_idCliente, Request["nomeCliente"], Request["idLoja"].StrParaUint(), Request["idRota"].StrParaUint())));
                        lstParam.Add(new ReportParameter("LiberarPedido", PedidoConfig.LiberarPedido.ToString()));
                        break;
                    }
                case "PosicaoMateriaPrima":
                    {
                        report.ReportPath = "Relatorios/rptPosicaoMateriaPrima.rdlc";
                        var posicaoMateriaPrima = PosicaoMateriaPrimaDAO.Instance.GetPosMateriaPrima(null, null, null, null, null, null, null, false);
                        var chapas = new List<PosicaoMateriaPrimaChapa>();

                        foreach (var p in posicaoMateriaPrima)
                            chapas.AddRange(p.Chapas);

                        report.DataSources.Add(new ReportDataSource("PosicaoMateriaPrima", posicaoMateriaPrima));
                        report.DataSources.Add(new ReportDataSource("Chapas", chapas));

                        break;
                    }
                case "ChapasDisponiveis":
                    {
                        report.ReportPath = "Relatorios/rptListaChapasDisponiveis.rdlc";

                        var chapas = Glass.Data.RelDAL.ChapasDisponiveisDAO.Instance.ObtemChapasDisponiveisRpt(Request["idFornec"].StrParaUint(), Request["nomeFornec"],
                            Request["codInternoProd"], Request["descrProd"], Request["numeroNfe"].StrParaInt(),
                            Request["lote"], Request["altura"].StrParaInt(), Request["largura"].StrParaInt(),
                            Request["idCor"], Request["espessura"].StrParaInt(), Request["numEtiqueta"], Request["idLoja"].StrParaInt());

                        report.DataSources.Add(new ReportDataSource("ChapasDisponiveis", chapas));

                        break;
                    }
                case "RegistroImpArqRemessa":
                    {
                        report.ReportPath = "Relatorios/rptRegImpArqRemessa.rdlc";

                        var contas = WebGlass.Business.ArquivoRemessa.Fluxo.RegistroArquivoRemessaFluxo.Instance.GetListForRpt(Request["idContaR"].StrParaUint(), Request["idPedido"].StrParaUint(),
                            Request["idLiberarPedido"].StrParaUint(), Request["idAcerto"].StrParaUint(), Request["idAcertoParcial"].StrParaUint(), Request["idTrocaDev"].StrParaUint(),
                            Request["numNfe"].StrParaUint(), Request["idCli"].StrParaUint(), Request["nomeCli"], Request["idLoja"].StrParaUint(), bool.Parse(Request["lojaCliente"]),
                            Request["numArquivoRemessa"].StrParaUint(), Request["idFormaPagto"].StrParaUint(), Request["obs"], Request["dataIniVenc"], Request["dataFimVenc"],
                            Request["dataIniRec"], Request["dataFimRec"], Request["valorVecIni"].StrParaDecimal(), Request["valorVecFim"].StrParaDecimal(), Request["valorRecIni"].StrParaDecimal(),
                            Request["valorRecFim"].StrParaDecimal(), Request["recebida"].StrParaInt(), Request["codOcorrencia"].StrParaInt(), Request["nossoNumero"], Request["numDoc"],
                            Request["usoEmpresa"], Request["idContaBanco"].StrParaInt());

                        var idsContasR = string.Join(",", contas.GroupBy(f => f.IdContaR).Select(f => f.Key.ToString()).ToArray());

                        var detalhes = WebGlass.Business.ArquivoRemessa.Fluxo.RegistroArquivoRemessaFluxo.Instance.GetListRegistros(idsContasR);

                        report.DataSources.Add(new ReportDataSource("ContasReceber", contas));
                        report.DataSources.Add(new ReportDataSource("RegistroArquivoRemessa", detalhes));

                        break;
                    }
                case "MovChapa":
                    {
                        report.ReportPath = "Relatorios/rptMovChapa.rdlc";

                        var movChapaFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Estoque.Negocios.IMovChapaFluxo>();

                        var movsChapa = movChapaFluxo.ObtemMovChapa(Request["idsCorVidro"], Request["espessura"].StrParaFloat(), Request["altura"].StrParaInt(), Request["largura"].StrParaInt(),
                            Request["dataIni"].StrParaDate().GetValueOrDefault(), Request["dataFim"].StrParaDate().GetValueOrDefault());

                        var movsChapaDetalhes = new List<Estoque.Negocios.Entidades.MovChapaDetalhe>();

                        foreach (var mc in movsChapa)
                            movsChapaDetalhes.AddRange(mc.Chapas);

                        lstParam.Add(new ReportParameter("ExibirDetalhes",
                            (Request["exibirDetalhes"] != null && Request["exibirDetalhes"].ToString().ToLower() == "true").ToString().ToLower()));

                        report.DataSources.Add(new ReportDataSource("MovChapa", movsChapa));
                        report.DataSources.Add(new ReportDataSource("MovChapaDetalhe", movsChapaDetalhes));

                        break;
                    }
                case "Parcelas":
                    {
                        report.ReportPath = "Relatorios/rptParcelas.rdlc";

                        var parcelas =
                            Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                                .GetInstance<Glass.Financeiro.Negocios.IParcelasFluxo>().PesquisarParcelas();

                        lstParam.Add(new ReportParameter("ExibirColunaDesconto", FinanceiroConfig.UsarDescontoEmParcela.ToString().ToLower()));
                        report.DataSources.Add(new ReportDataSource("Parcelas", parcelas));

                        break;
                    }
                case "EtqCavalete":
                    {
                        report.ReportPath = "Relatorios/ModeloEtiquetaCavalete/rptEtiquetaCavalete.rdlc";

                        var idCavalete = Request["idCavalete"].StrParaInt();

                        var cavalete = ServiceLocator.Current.GetInstance<PCP.Negocios.ICavaleteFluxo>()
                            .ObterCavalete(idCavalete);

                        if (cavalete == null)
                            throw new Exception("O cavalete n�o foi encontrado.");

                        report.DataSources.Add(new ReportDataSource("Cavalete", new List<PCP.Negocios.Entidades.Cavalete>() { cavalete }));

                        break;
                    }

                case "ResultadoOtimizacao":
                    {
                        report.ReportPath = "Relatorios/rptResultadoOtimizacao.rdlc";

                        var idOtimizacao = Request["idOtimizacao"].StrParaInt();

                        var otimizacao = ServiceLocator.Current.GetInstance<PCP.Negocios.IOtimizacaoFluxo>()
                            .ObterOtimizacao(idOtimizacao);

                        if (otimizacao == null)
                            throw new Exception("A otimiza��o n�o foi encontrada.");

                        var layouts = new List<Glass.PCP.Negocios.Entidades.LayoutPecaOtimizada>();
                        var pecas = new List<Glass.PCP.Negocios.Entidades.PecaOtimizada>();

                        layouts.AddRange(otimizacao.LayoutsOtimizacao);
                        foreach (var l in layouts)
                            pecas.AddRange(l.PecasOtimizadas);

                        report.DataSources.Add(new ReportDataSource("Otimizacao", new List<Glass.PCP.Negocios.Entidades.Otimizacao>() { otimizacao }));
                        report.DataSources.Add(new ReportDataSource("Layouts", layouts));
                        report.DataSources.Add(new ReportDataSource("Pecas", pecas));

                        break;
                    }

                case "PecasOtimizacao":
                    {
                        report.ReportPath = "Relatorios/rptPecasOtimizacao.rdlc";

                        var idOtimizacao = Request["idOtimizacao"].StrParaInt();

                        var otimizacao = ServiceLocator.Current.GetInstance<PCP.Negocios.IOtimizacaoFluxo>()
                            .ObterOtimizacao(idOtimizacao);

                        if (otimizacao == null)
                            throw new Exception("A otimiza��o n�o foi encontrada.");

                        report.DataSources.Add(new ReportDataSource("Otimizacao", new List<Glass.PCP.Negocios.Entidades.Otimizacao>() { otimizacao }));
                        report.DataSources.Add(new ReportDataSource("Layouts", otimizacao.LayoutsOtimizacao));

                        break;
                    }

                case "AberturaFornada":
                    {
                        report.ReportPath = "Relatorios/rptAberturaFornada.rdlc";

                        var lst = new[] { new { BarCodeImage = Data.Helper.Utils.GetBarCode("AF") } };

                        report.DataSources.Add(new ReportDataSource("Fornada", lst));

                        break;
                    }

                case "Fornada":
                    {
                        var analitico = Request["analitico"] == "true";

                        var dados = Glass.Data.DAL.FornadaDAO.Instance.PesquisarFornadasRpt(Request["idFornada"].StrParaInt(), Request["idPedido"].StrParaInt(),
                            Request["dataIni"], Request["dataFim"], Request["numEtiqueta"], Request["espessura"].StrParaInt(), Request["idsCorVidro"], false);

                        var lstProdCorEsp = Glass.Data.DAL.FornadaDAO.Instance.PesquisarFornadasRpt(Request["idFornada"].StrParaInt(), Request["idPedido"].StrParaInt(),
                            Request["dataIni"], Request["dataFim"], Request["numEtiqueta"], Request["espessura"].StrParaInt(), Request["idsCorVidro"], true);
                        
                        report.ReportPath = analitico ? "Relatorios/rptFornadaAnalitico.rdlc" : "Relatorios/rptFornadaSintetico.rdlc";

                        //Se for o relat�rio analitico exibe tamb�m as pe�as passadas nessa fornada
                        if (analitico)
                        {
                            var produtoPedidoProducao = ProdutoPedidoProducaoDAO.Instance.ObterPecasFornadaRpt(string.Join(",", dados.Select(f => f.IdFornada).ToArray()));
                            report.DataSources.Add(new ReportDataSource("ProdutoPedidoProducao", produtoPedidoProducao));
                        }
                        report.DataSources.Add(new ReportDataSource("Fornada", dados));
                        report.DataSources.Add(new ReportDataSource("ProdCorEspessura", lstProdCorEsp));

                        break;
                    }

                case "PedidoProntoSemCarregamento":
                    {
                        var idCarregamento = Request["idCarregamento"].StrParaUint();
                        var pedidos = PedidoDAO.Instance.ObterPedidosProntosSemCarregamento(null, idCarregamento);
                        lstParam.Add(new ReportParameter("Titulo", "Pedidos prontos sem carregamento"));
                        report.ReportPath = "Relatorios/rptLstPedidos.rdlc";
                        report.DataSources.Add(new ReportDataSource("Pedidos", pedidos));

                        break;
                    }

                case "PedidosPendentesLeitura":
                    {
                        var idSetor = Request["idSetor"].StrParaUint();
                        var pedidos = PedidoDAO.Instance.ObterPedidosPendentesLeitura(null, idSetor);
                        var nomeSetor = SetorDAO.Instance.ObtemDescricaoSetor(null, (int)idSetor);
                        lstParam.Add(new ReportParameter("Titulo", "Pedidos com pe�as dispon�veis para leitura no setor " + nomeSetor));
                        report.ReportPath = "Relatorios/rptLstPedidos.rdlc";
                        report.DataSources.Add(new ReportDataSource("Pedidos", pedidos));
                        break;
                    }

                case "ComprovanteTef":
                    {
                        var transacoes = TransacaoCapptaTefDAO.Instance.GetListTransacoes(Request["codControle"]);
                        lstParam.Add(new ReportParameter("Reimpressao", (Request["reimpressao"] != null && Request["reimpressao"].ToLower() == "true").ToString().ToLower()));
                        report.ReportPath = "Relatorios/rptComprovanteCapptaTef.rdlc";
                        report.DataSources.Add(new ReportDataSource("Transacao", transacoes));
                        break;
                    }

                default:
                    throw new Exception("Nenhum relat�rio especificado.");
            }

            if (reportName != null)
            {
                var reportDocument = (Colosoft.Reports.ReportDocument)Colosoft.Reports.ReportDocumentRepository.Current.CreateReportDocument(reportName);
                reportDocument.DataSourcesParameters.Fill(Request);
                reportDocument.RefreshDocumentFromParameters(System.Globalization.CultureInfo.CurrentCulture);
                reportDocument.RefreshDataSources();

                return reportDocument;
            }

            var textoRodape = Geral.TextoRodapeRelatorio(login.Nome, incluirDataTextoRodape);

            if (Request["EnvioEmail"] != null && Request["EnvioEmail"].ToLower() == "true")
                textoRodape = String.Empty;

            lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogo(diretorioLogotipos, idLojaLogotipo)));
            lstParam.Add(new ReportParameter("TextoRodape", textoRodape));
            lstParam.Add(new ReportParameter("CorRodape", "DimGray"));

            return null;
        }

        protected override void report_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            // S� carrega o subreport do relat�rio de Produtos Comprados se o checkbox de exibir detalhes estiver marcado
            if (!_loadSubreport)
                e.DataSources.Add(new ReportDataSource("Produto", new Produto[0]));
            else
                base.report_SubreportProcessing(sender, e);
        }
    }
}
