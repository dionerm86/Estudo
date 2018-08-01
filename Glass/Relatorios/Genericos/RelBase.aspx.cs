using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.RelModel;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.RelDAL;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.UI.Web.Relatorios.Genericos
{
    public partial class RelBase : Glass.Relatorios.UI.Web.ReportPage
    {
        protected override object[] Parametros
        {
            get { return new object[] { hdfItensRecebidos.Value, hdfParcelas.Value }; }
        }

        protected override Glass.Relatorios.UI.Web.ReportPage.JavaScriptData DadosJavaScript
        {
            get
            {
                return new JavaScriptData(
                    hdfLoad.Value != "true",
                    "document.getElementById('" + hdfLoad.ClientID + "') == 'true'"
                );
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (DadosJavaScript.BackgroundLoading)
                return;

            ProcessaReport(pchTabela);
        }

        protected override Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam,
            HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login, string diretorioLogotipos)
        {
            Glass.Data.RelModel.Recibo recibo;

            // Verifica qual relatório será chamado
            switch (Request["rel"])
            {
                case "recibo":
                    if (string.IsNullOrEmpty(outrosParametros[0].ToString()) || string.IsNullOrEmpty(outrosParametros[1].ToString()))
                        return null;

                    var idOrcamento = Request["idOrcamento"] != "0" && Request["idOrcamento"] != "" ? Request["idOrcamento"].StrParaUint() : 0;
                    var idPedido = Request["idPedido"] != "0" && Request["idPedido"] != "" ? Request["idPedido"].StrParaUint() : 0;
                    var idLiberacao = Request["idLiberacao"] != "0" && Request["idLiberacao"] != "" ? Request["idLiberacao"].StrParaUint() : 0;
                    var idsContaR = outrosParametros[1].ToString();

                    if (idsContaR == null || idsContaR == "0")
                    {
                        if (PedidoConfig.LiberarPedido)
                        {
                            var contasRec = ContasReceberDAO.Instance.GetByLiberacaoPedido(idLiberacao, true);
                            if (contasRec != null && contasRec.Count > 0)
                                idsContaR = string.Join(",", contasRec.Select(f => f.IdContaR.ToString()));
                        }
                        else
                        {
                            var contasRec = ContasReceberDAO.Instance.GetByPedido(null, idPedido, false, true);
                            if (contasRec != null && contasRec.Count > 0)
                                idsContaR = string.Join(",", contasRec.Select(f => f.IdContaR.ToString()));
                        }
                    }

                    var orcamento = new Data.Model.Orcamento();
                    var pedido = new Data.Model.Pedido();
                    var liberacao = new LiberarPedido();

                    var nomeCliente = string.Empty;
                    var nomeVendedor = string.Empty;
                    var idLoja = new uint();
                    var total = new decimal();

                    #region Orçamento

                    // Recupera os dados do orçamento.
                    if (idOrcamento > 0)
                    {
                        orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(idOrcamento);

                        if (orcamento.IdCliente > 0)
                            nomeCliente = ClienteDAO.Instance.GetNome(orcamento.IdCliente.Value);
                        else
                            nomeCliente = orcamento.NomeCliente;

                        nomeVendedor = orcamento.IdFuncionario > 0 ? FuncionarioDAO.Instance.GetNome(orcamento.IdFuncionario.Value) : login.Nome;
                        idLoja = orcamento.IdLoja > 0 ? orcamento.IdLoja.Value : login.IdLoja;
                        total = orcamento.Total;

                        idPedido = 0;
                        idLiberacao = 0;
                    }

                    #endregion

                    #region Pedido

                    // Recupera os dados do pedido.
                    else if (idPedido > 0)
                    {
                        pedido = PedidoDAO.Instance.GetElementByPrimaryKey(idPedido);
                        nomeCliente = ClienteDAO.Instance.GetNome(pedido.IdCli);
                        nomeVendedor = pedido.IdFunc > 0 ? FuncionarioDAO.Instance.GetNome(pedido.IdFunc) : login.Nome;
                        idLoja = pedido.IdPedido > 0 ? pedido.IdLoja : login.IdLoja;
                        total = pedido.Total;

                        // Se houver pcp, usa o total do mesmo
                        var totalEspelho = PedidoEspelhoDAO.Instance.ObtemTotal(idPedido);
                        if (totalEspelho > 0)
                            total = totalEspelho;

                        idLiberacao = 0;
                    }

                    #endregion
                    
                    #region Liberação

                    // Recupera os dados da liberação.
                    else if (idLiberacao > 0)
                    {
                        liberacao = LiberarPedidoDAO.Instance.GetElement(idLiberacao);
                        nomeCliente = ClienteDAO.Instance.GetNome(liberacao.IdCliente);
                        nomeVendedor = !string.IsNullOrEmpty(liberacao.NomeFunc) ? liberacao.NomeFunc : login.Nome;
                        idLoja = (uint)FuncionarioDAO.Instance.GetElementByPrimaryKey(liberacao.IdFunc).IdLoja;
                        total = liberacao.Total;
                    }

                    #endregion

                    recibo = new Data.RelModel.Recibo();
                    recibo.Tipo = Conversoes.StrParaInt(Request["referente"]);
                    recibo.IdOrcamento = orcamento.IdOrcamento;
                    recibo.IdPedido = pedido.IdPedido;
                    recibo.IdLiberarPedido = liberacao.IdLiberarPedido;
                    recibo.Cliente = nomeCliente;
                    recibo.IdLoja = idLoja;
                    recibo.SinalPedido = pedido.IdPedido > 0 ? pedido.ValorEntrada + pedido.ValorPagamentoAntecipado : 0;
                    recibo.Total = total;
                    recibo.Vendedor = nomeVendedor;
                    recibo.Items = outrosParametros[0].ToString();
                    recibo.NumParcelas = idsContaR;
                    recibo.ValorReferente = Request["valorRef"];
                    recibo.MotivoReferente = Request["motivoRef"];
                    report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/Genericos/ModeloRecibo/rptRecibo{0}.rdlc");
                    
                    if (report.ReportPath == "Relatorios/Genericos/ModeloRecibo/rptReciboVidrosEVidros.rdlc")
                        lstParam.Add(new ReportParameter("ImagemCabecalho",
                            "file:///" + PageRequest.PhysicalApplicationPath.Replace('\\', '/') + "Images/cabecalhoOrcamentoVivrosEVidros.jpg"));

                    if (ReciboConfig.Relatorio.UsarParcelasPedido && pedido.IdPedido > 0)
                        report.DataSources.Add(new ReportDataSource("ParcelasPedido", pedido.NumParc > 0 ? ParcelasPedidoDAO.Instance.GetByPedido(pedido.IdPedido) :
                        new ParcelasPedido[0]));

                    report.DataSources.Add(new ReportDataSource("Recibo", new Data.RelModel.Recibo[] { recibo }));
                    break;
                case "reciboPgAntec":
                    Sinal sinal = SinalDAO.Instance.GetSinalDetails(Glass.Conversoes.StrParaUint(Request["idPgAntecipado"]));
                    Cliente clientePgto = ClienteDAO.Instance.GetElementByPrimaryKey(sinal.IdCliente);

                    recibo = new Data.RelModel.Recibo();
                    recibo.Tipo = Glass.Conversoes.StrParaInt(Request["referente"]);
                    recibo.IdSinal = sinal.IdSinal;
                    recibo.Cliente = clientePgto.Nome;
                    recibo.IdLoja = (uint)FuncionarioDAO.Instance.GetElementByPrimaryKey(sinal.UsuCad).IdLoja;
                    recibo.Total = sinal.TotalSinal;
                    recibo.Vendedor = FuncionarioDAO.Instance.GetNome(sinal.UsuCad);
                    recibo.Items = outrosParametros[0].ToString();
                    recibo.NumParcelas = outrosParametros[1].ToString();
                    recibo.ValorReferente = Request["valorRef"];
                    recibo.MotivoReferente = Request["motivoRef"];
                    report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/Genericos/ModeloRecibo/rptRecibo{0}.rdlc");

                    report.DataSources.Add(new ReportDataSource("Recibo", new Data.RelModel.Recibo[] { recibo }));
                    break;
                case "reciboAcerto":

                    uint idAcerto = Request["idAcerto"] != "0" && Request["idAcerto"] != "" ? Glass.Conversoes.StrParaUint(Request["idAcerto"]) : 0;

                    if (idAcerto > 0 && !AcertoDAO.Instance.Exists(idAcerto))
                    {
                        Response.Write("O acerto informado não existe.");
                        return null;
                    }

                    Acerto acerto = AcertoDAO.Instance.GetByCliList(Convert.ToInt32(idAcerto), 0, 0, 0, null, null, 0, 0, 0, 0, null, 0, 10)[0];

                    recibo = new Data.RelModel.Recibo();
                    recibo.Tipo = Glass.Conversoes.StrParaInt(Request["referente"]);
                    recibo.IdAcerto = acerto.IdAcerto;
                    recibo.Cliente = ClienteDAO.Instance.GetNome(acerto.IdCli);
                    recibo.IdLoja = (uint)FuncionarioDAO.Instance.GetElementByPrimaryKey(acerto.UsuCad).IdLoja;
                    recibo.Total = acerto.TotalAcerto;
                    recibo.Vendedor = FuncionarioDAO.Instance.GetNome(acerto.UsuCad);
                    recibo.Items = outrosParametros[0].ToString();
                    recibo.NumParcelas = outrosParametros[1].ToString();
                    recibo.ValorReferente = Request["valorRef"];
                    recibo.MotivoReferente = Request["motivoRef"];
                    report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/Genericos/ModeloRecibo/rptRecibo{0}.rdlc");

                    report.DataSources.Add(new ReportDataSource("Recibo", new Data.RelModel.Recibo[] { recibo }));

                    break;
                case "termoaceitacao":
                    if (!PedidoDAO.Instance.PedidoExists(Glass.Conversoes.StrParaUint(Request["ped"])))
                    {
                        Response.Write("O pedido informado não existe.");
                        return null;
                    }

                    Glass.Data.Model.Pedido pedTermo = PedidoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["ped"]));

                    if (pedTermo.IdOrcamento == null)
                        pedTermo.IdOrcamento = 0;

                    pedTermo.InfoAdicional = Request["infAdic"]?.Replace("\\n", "\n") ?? string.Empty;

                    report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/Genericos/rptAceitacao{0}.rdlc");

                    report.DataSources.Add(new ReportDataSource("PedidoRpt", PedidoRptDAL.Instance.CopiaLista(new Glass.Data.Model.Pedido[] { pedTermo }, PedidoRpt.TipoConstrutor.TermoAceitacao, false, login)));
                    break;
                case "riscoquebra":
                    // Verifica se pedido passado existe
                    if (!PedidoDAO.Instance.PedidoExists(Glass.Conversoes.StrParaUint(Request["idPedido"])))
                    {
                        Response.Write("O pedido informado não existe.");
                        return null;
                    }

                    var risco = new Data.RelModel.RiscoQuebra();
                    Glass.Data.Model.Pedido ped = PedidoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idPedido"]));
                    Cliente cli = ClienteDAO.Instance.GetElementByPrimaryKey(ped.IdCli);
                    cli.Cidade = CidadeDAO.Instance.GetNome((uint?)cli.IdCidade);
                    risco.IdPedido = ped.IdPedido;
                    risco.NomeLoja = LojaDAO.Instance.GetElementByPrimaryKey(ped.IdLoja).NomeFantasia;
                    risco.CidadeData = LojaDAO.Instance.GetElement(ped.IdLoja).Cidade + " " + Formatacoes.DataExtenso(DateTime.Now);
                    risco.Cliente = cli.Nome;
                    risco.Endereco = !string.IsNullOrEmpty(ped.LocalizacaoObra) ? ped.LocalizacaoObra : cli.EnderecoCompleto;
                    risco.Telefone = cli.Telefone;
                    risco.Texto = Request["texto"];

                    report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/Genericos/rptRiscoQuebra{0}.rdlc");

                    report.DataSources.Add(new ReportDataSource("RiscoQuebra", new Data.RelModel.RiscoQuebra[] { risco }));
                    break;

                case "reciboContaPagar":
                    {
                        var idContaPagar = Request["idContaPagar"] != "0" && Request["idContaPagar"] != "" ? Glass.Conversoes.StrParaInt(Request["idContaPagar"]) : 0;
                        var contaPg = ContasPagarDAO.Instance.GetPagasForRpt(idContaPagar, 0, null, 0, 0, 0, 0, null, null, null, null, null, null, null, null, 0, 0, 0, false, true, false, null, false,
                            false, 0, 0, null, null);

                        if (contaPg.Length == 0)
                            throw new Exception("A conta a pagar informada não existe ou não está paga.");

                        recibo = new Data.RelModel.Recibo();
                        recibo.IdContaPagar = idContaPagar;
                        recibo.IdLoja = contaPg[0].IdLoja.GetValueOrDefault(0);
                        recibo.Total = contaPg[0].ValorPago;
                        recibo.Cliente = FornecedorDAO.Instance.GetElementByPrimaryKey(contaPg[0].IdFornec.GetValueOrDefault(0)).Nome;
                        recibo.MotivoReferente = contaPg[0].DescrPlanoConta;

                        report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/Genericos/ModeloRecibo/rptRecibo{0}.rdlc");
                        recibo.Tipo = Glass.Conversoes.StrParaInt(Request["referente"]);

                        report.DataSources.Add(new ReportDataSource("Recibo", new Data.RelModel.Recibo[] { recibo }));

                        break;
                    }
            }

            // Atribui parâmetros ao relatório
            lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogoColor(PageRequest)));
            lstParam.Add(new ReportParameter("TextoRodape", Geral.TextoRodapeRelatorio(login.Nome)));
            lstParam.Add(new ReportParameter("CorRodape", "DimGray"));

            return null;
        }
    }
}
