using System;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.RelModel;
using System.Collections.Generic;
using Glass.Data.RelDAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class RelPedido : Glass.Relatorios.UI.Web.ReportPage
    {
        public enum TipoRelatorioPedido
        {
            Normal,
            MemoriaCalculo,
            Pcp,
            MemoriaCalculoPcp
        }
    
        protected override object[] Parametros
        {
            get { return new object[] { }; }
        }

        protected override Glass.Relatorios.UI.Web.ReportPage.JavaScriptData DadosJavaScript
        {
            get
            {
                TipoRelatorioPedido tipo = !String.IsNullOrEmpty(Request["tipo"]) ? (TipoRelatorioPedido)Glass.Conversoes.StrParaInt(Request["tipo"]) : TipoRelatorioPedido.Normal;
    
                return new JavaScriptData(
                    UserInfo.GetUserInfo.IdCliente > 0 && 
                        PedidoDAO.Instance.ObtemIdCliente(Glass.Conversoes.StrParaUint(Request["idPedido"])) != UserInfo.GetUserInfo.IdCliente ||
                        (tipo != TipoRelatorioPedido.Normal && tipo != TipoRelatorioPedido.Pcp),
                    "false"
                );
            }
        }
    
        private bool IsRelOtimizado(int idLoja)
        {
            var nomeRelatorio = PedidoConfig.RelatorioPedido.NomeArquivoRelatorio((uint)idLoja).Item1;

            if (nomeRelatorio.ToLower().Contains("rptpedidoa4orca") ||
                nomeRelatorio.ToLower().Contains("rptpedidoannis.rdlc") || 
                nomeRelatorio.ToLower().Contains("rptpedidocmv") ||
                nomeRelatorio.ToLower().Contains("rptpedidodekor") || 
                nomeRelatorio.ToLower().Contains("rptpedidoespacovidros") ||
                nomeRelatorio.ToLower().Contains("rptpedidofuncional") ||
                nomeRelatorio.ToLower().Contains("rptpedidombtemper") ||
                nomeRelatorio.ToLower().Contains("rptpedidotempera") ||
                nomeRelatorio.ToLower().Contains("rptpedidomercosul") ||
                nomeRelatorio.ToLower().Contains("rptpedidonrc") ||
                nomeRelatorio.ToLower().Contains("rptpedidoouropreto") ||
                nomeRelatorio.ToLower().Contains("rptpedidoterra") ||
                nomeRelatorio.ToLower().Contains("rptpedidovidrovalle") || 
                nomeRelatorio.ToLower().Contains("rptpedidovintage") ||
                nomeRelatorio.ToLower().Contains("rptpedidovidrorapido") ||
                nomeRelatorio.ToLower().Contains("rptpedidovidrosevidros"))
                return false;

            return true;
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            TipoRelatorioPedido tipo = !String.IsNullOrEmpty(Request["tipo"]) ? (TipoRelatorioPedido)Glass.Conversoes.StrParaInt(Request["tipo"]) : TipoRelatorioPedido.Normal;
    
            // (ACESSO EXTERNO) Verifica se o usuário tem acesso à este relatório
            if (UserInfo.GetUserInfo.IdCliente > 0 && PedidoDAO.Instance.ObtemIdCliente(Glass.Conversoes.StrParaUint(Request["idPedido"])) != UserInfo.GetUserInfo.IdCliente)
                return;
    
            if (!PCPConfig.CriarClone && tipo == TipoRelatorioPedido.Pcp)
            {
                Response.Redirect("~/Relatorios/RelBase.aspx?rel=PedidoPcp&idPedido=" + Request["idPedido"]);
                return;
            }
    
            if (tipo == TipoRelatorioPedido.Normal || tipo == TipoRelatorioPedido.Pcp)
            {
                ProcessaReport(pchTabela);
            }
            else
            {
                bool isPcp = tipo == TipoRelatorioPedido.MemoriaCalculoPcp;
    
                Response.Redirect("~/Relatorios/RelBase.aspx?rel=MemoriaCalculoPedido" + 
                    (!isPcp ? "" : "Espelho") + "&idPed=" + Request["idPedido"]);
    
                return;
            }
        }

        protected override Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam,
            HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login)
        {
            TipoRelatorioPedido tipo = !String.IsNullOrEmpty(Request["tipo"]) ? (TipoRelatorioPedido)Glass.Conversoes.StrParaInt(Request["tipo"]) : TipoRelatorioPedido.Normal;
            Glass.Data.Model.Pedido[] pedido = PedidoDAO.Instance.GetForRpt(Request["idPedido"], tipo == TipoRelatorioPedido.Pcp, login);
            uint idLoja = pedido[0].IdLoja;
            
            List<ProdutosPedido> prodPedido = new List<ProdutosPedido>();
            List<ParcelasPedido> parcPedido = new List<ParcelasPedido>();
            List<AmbientePedido> ambPedido = new List<AmbientePedido>();
            List<TextoPedido> textoPedido = new List<TextoPedido>();        
            
            report.ReportPath = PedidoConfig.RelatorioPedido.NomeArquivoRelatorio(idLoja).Item1;

            Dictionary<int, List<string>> parametros = new Dictionary<int, List<string>>();
            for (int i = 0; i < 20; i++)
                parametros.Add(i, new List<string>());

            for (int i = 0; i < pedido.Length; i++)
            {
                parcPedido.AddRange(ParcelasPedidoDAO.Instance.GetForRpt(pedido[i].IdPedido));
                ambPedido.AddRange(AmbientePedidoDAO.Instance.GetByPedido(pedido[i].IdPedido, tipo == TipoRelatorioPedido.Pcp));
                prodPedido.AddRange(PedidoConfig.RelatorioPedido.ExibirItensProdutosPedido ?
                    ProdutosPedidoDAO.Instance.GetForRpt(pedido[i].IdPedido, tipo == TipoRelatorioPedido.Pcp) :
                    ProdutosPedidoDAO.Instance.GetForRptAmbiente(pedido[i].IdPedido, PedidoConfig.RelatorioPedido.AgruparAmbientesRelatorio,
                    tipo == TipoRelatorioPedido.Pcp));

                textoPedido.AddRange(TextoPedidoDAO.Instance.GetByPedido(pedido[i].IdPedido));

                float taxaEntrega = 0f;
                if (PedidoConfig.Pedido_FastDelivery.FastDelivery && pedido[i].FastDelivery)
                    taxaEntrega = PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery;

                string separador = " - ";
                string enderecoLoja = pedido[i].RptEnderecoLoja + pedido[i].RptComplLoja + separador + pedido[i].RptBairroLoja + separador +
                    pedido[i].RptTelefoneLoja + separador + "CEP: " + pedido[i].RptCepLoja;

                parametros[0].Add(taxaEntrega.ToString() + "%");
                parametros[1].Add(pedido[i].IdPedido.ToString());
                parametros[2].Add(String.IsNullOrEmpty(pedido[i].NomeFunc) ? "." : pedido[i].NomeFunc);
                parametros[3].Add(pedido[i].DataPedido.ToString("dd/MM/yyyy"));
                parametros[4].Add(String.IsNullOrEmpty(pedido[i].FoneFaxLoja) ? "." : pedido[i].FoneFaxLoja);
                parametros[5].Add(String.IsNullOrEmpty(pedido[i].RptNomeLoja) ? "." : pedido[i].RptNomeLoja);
                parametros[6].Add(String.IsNullOrEmpty(pedido[i].DadosLoja) ? "." : pedido[i].DadosLoja);
                parametros[7].Add(pedido[i].DataCad.ToString("dd/MM/yyyy"));
                parametros[8].Add(pedido[i].DescrSituacaoPedido);
                parametros[9].Add(String.IsNullOrEmpty(pedido[i].EmailLoja) ? "." : pedido[i].EmailLoja);
                parametros[10].Add(String.IsNullOrEmpty(pedido[i].RptTelefoneLoja) ? "." : pedido[i].RptTelefoneLoja);
                parametros[11].Add(String.IsNullOrEmpty(enderecoLoja) ? "." : enderecoLoja);
                parametros[12].Add(String.IsNullOrEmpty(pedido[i].DataEntregaString) ? "." : pedido[i].DataEntregaString);
                parametros[13].Add((pedido[i].TipoPedido == (int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObra).ToString());
                parametros[14].Add(String.IsNullOrEmpty(pedido[i].CodCliente) ? "." : pedido[i].CodCliente);
                parametros[15].Add(!String.IsNullOrEmpty(pedido[i].CodCliente) ? pedido[i].CodCliente.ToString() : ".");
                parametros[16].Add(!PedidoConfig.LiberarPedido ? ContasReceberDAO.Instance.GetTotalDescontoParcelas(pedido[i].IdPedido).ToString() : ".");
                parametros[17].Add(!String.IsNullOrEmpty(pedido[i].Obs) ? "Obs: " + pedido[i].Obs : ".");
                
                string codRota = string.Empty;
                string descrRota = string.Empty;

                // Só exibe o código da rota se não for tipo de entrega balcão
                if (PedidoDAO.Instance.ObtemTipoEntrega(pedido[i].IdPedido) != (uint)Glass.Data.Model.Pedido.TipoEntregaPedido.Balcao)
                {
                    codRota = RotaDAO.Instance.ObtemCodRota(pedido[i].IdCli);
                    descrRota = RotaDAO.Instance.ObtemDescrRota(pedido[i].IdCli);
                }

                parametros[18].Add(!String.IsNullOrEmpty(codRota) ? codRota : ".");
                parametros[19].Add(!String.IsNullOrEmpty(descrRota) ? descrRota : ".");

                // Indica se o pedido é de cliente
                pedido[i].RptIsCliente = login.IdCliente > 0;
            }

            if (!IsRelOtimizado((int)idLoja))
            {
                report.DataSources.Add(new ReportDataSource("Pedido", pedido));
                report.DataSources.Add(new ReportDataSource("ProdutosPedido", prodPedido));
            }
            else
            {
                report.DataSources.Add(new ReportDataSource("PedidoRpt", PedidoRptDAL.Instance.CopiaLista(pedido, PedidoRpt.TipoConstrutor.RelatorioPedido, false, login)));
                report.DataSources.Add(new ReportDataSource("ProdutosPedidoRpt", ProdutosPedidoRptDAL.Instance.CopiaLista(prodPedido.ToArray())));
            }

            report.DataSources.Add(new ReportDataSource("ParcelasPedido", parcPedido));
            report.DataSources.Add(new ReportDataSource("AmbientePedido", ambPedido));
            report.DataSources.Add(new ReportDataSource("TextoPedido", textoPedido));

            lstParam.Add(new ReportParameter("TaxaEntrega", parametros[0].ToArray()));
            lstParam.Add(new ReportParameter("AgruparBeneficiamentos", PedidoConfig.RelatorioPedido.AgruparBenefRelatorio.ToString()));
            lstParam.Add(new ReportParameter("ExibirM2Calc", PedidoConfig.RelatorioPedido.ExibirM2CalcRelatorio.ToString()));
            lstParam.Add(new ReportParameter("Cabecalho_DataLocal", LojaDAO.Instance.GetCidade(pedido[0].IdLoja, false) + ", " + Formatacoes.DataExtenso(pedido[0].DataPedido)));
            lstParam.Add(new ReportParameter("Cabecalho_IdPedido", parametros[1].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_NomeFunc", parametros[2].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_DataPedido", parametros[3].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_FoneFaxLoja", parametros[4].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_NomeLoja", parametros[5].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_DadosLoja", parametros[6].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_DataCad", parametros[7].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_Situacao", parametros[8].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_EmailLoja", parametros[9].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_TelefoneLoja", parametros[10].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_EnderecoLoja", parametros[11].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_DataEntrega", parametros[12].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_MaoDeObra", parametros[13].ToArray()));
            lstParam.Add(new ReportParameter("ExibirComissao", (!PedidoConfig.Comissao.UsarComissionadoCliente && false).ToString())); // Não exibe a comissão em nenhum relatório
            lstParam.Add(new ReportParameter("ImpressoPor", login.Nome));
            lstParam.Add(new ReportParameter("TipoRpt", ((int)tipo).ToString()));
            lstParam.Add(new ReportParameter("FormatTotM", Geral.GetFormatTotM()));
            lstParam.Add(new ReportParameter("Cabecalho_CodCliente", parametros[14].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_NumPedCli", parametros[15].ToArray()));
            lstParam.Add(new ReportParameter("DescontoParcelas", parametros[16].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_Obs", parametros[17].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_CpfCnpjLoja", LojaDAO.Instance.ObtemCnpj(idLoja)));
            lstParam.Add(new ReportParameter("Cabecalho_InscEstLoja", LojaDAO.Instance.ObtemInscEst(idLoja)));
            lstParam.Add(new ReportParameter("Cabecalho_Rota", parametros[18].ToArray()));
            lstParam.Add(new ReportParameter("Cabecalho_DescrRota", parametros[19].ToArray()));
            lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogo(PageRequest, idLoja)));

            if (report.ReportPath.Contains("rptPedidoA4ProcApl") || report.ReportPath.Contains("rptPedidoModeloVidros"))
                lstParam.Add(new ReportParameter("TemProdutoLamComposicao", pedido[0].TemProdutoLamComposicao.ToString()));

            return null;
        }
    }
}
