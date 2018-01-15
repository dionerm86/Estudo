using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.Model.Cte;
using Glass.Data.DAL.CTe;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;
using Glass.Data.Helper;
using Colosoft;
using Glass.Configuracoes;

namespace Glass.Data.Handlers
{
    public class Dacte : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            bool isCodigoBarras = context.Request["isCodigoBarras"] == "true";
            bool dadosAdicionais = context.Request["dadosAdicionais"] == "true";

            context.Response.ContentType = isCodigoBarras ? "image/jpeg" : "application/pdf";

            uint idCte = Glass.Conversoes.StrParaUint(context.Request["idCte"]);
            byte[] bytes = isCodigoBarras ? GetBytesCodigoBarra(idCte, dadosAdicionais) :
                GetBytesRelatorio(context, idCte, context.Request["previsualizar"] == "true");

            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
        }

        public static byte[] GetBytesRelatorio(HttpContext context, uint idCte, bool preVisualizar)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            return GetBytesRelatorio(context, idCte, preVisualizar, out warnings, out streamids, out mimeType, out encoding, out extension);
        }

        public static byte[] GetBytesRelatorio(HttpContext context, uint idCte, bool preVisualizar, out Warning[] warnings, out string[] streamids,
            out string mimeType, out string encoding, out string extension)
        {
            var report = new LocalReport();
            var lstParam = new List<ReportParameter>();

            var cte = ConhecimentoTransporteDAO.Instance.GetElement(idCte);
            var dacte = CTeDAO.GetForDacte(context, cte.ChaveAcesso);

            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCte(cte.IdCte);
            var idLoja = participante.Where(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Emitente).First().IdLoja;
            report.DataSources.Add(new ReportDataSource("CTe", new CTe[] { dacte }));
                        
            var cteRod = DAL.CTe.ConhecimentoTransporteRodoviarioDAO.Instance.GetElement(idCte);

            if (cteRod != null && cteRod.Lotacao)
                report.ReportPath = "Relatorios/CTe/rptDacteRetrato.rdlc";
            else
                report.ReportPath = "Relatorios/CTe/rptDacteRetratoFracionado.rdlc";

            if (cte.TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.ContingenciaFsda)
            {
                report.ReportPath = report.ReportPath.Replace(".", "FS.");

                lstParam.Add(new ReportParameter("Cabecalho_DadosAdicionaisCte", dacte.DadosAdicionaisFs));
                lstParam.Add(new ReportParameter("Cabecalho_CodigoBarrasAdicionais", Utils.GetUrlSite(context) +
                    "/Handlers/Dacte.ashx?idCte=" + idCte + "&isCodigoBarras=true&dadosAdicionais=true"));
            }

            report.ReportPath = context.Server.MapPath("~/" + report.ReportPath);

            lstParam.Add(new ReportParameter("Producao", (dacte.TipoAmbiente == (int)Glass.Data.CTeUtils.ConfigCTe.TipoAmbienteCte.Producao && cte.Situacao != (int)ConhecimentoTransporte.SituacaoEnum.Cancelado && !preVisualizar).ToString().ToLower()));

            // Parâmetros para o cabeçalho/rodapé do relatório
            lstParam.Add(new ReportParameter("Cabecalho_EnderecoEmit", dacte.EnderecoEmit != null ? dacte.EnderecoEmit : ""));
            lstParam.Add(new ReportParameter("Cabecalho_RazaoSocialEmit", dacte.RazaoSocialEmit != null ? dacte.RazaoSocialEmit : ""));
            lstParam.Add(new ReportParameter("Cabecalho_Modelo", dacte.Modelo != null ? dacte.Modelo : ""));
            lstParam.Add(new ReportParameter("Cabecalho_SerieCte", dacte.SerieCte != null ? dacte.SerieCte : ""));
            lstParam.Add(new ReportParameter("Cabecalho_NumeroCte", dacte.NumeroCte != null ? dacte.NumeroCte : ""));
            lstParam.Add(new ReportParameter("Cabecalho_DHEmissao", dacte.DHEmissao != null ? dacte.DHEmissao : ""));
            lstParam.Add(new ReportParameter("Cabecalho_SuframaDest", dacte.InscSuframa != null ? dacte.InscSuframa : ""));
            lstParam.Add(new ReportParameter("Cabecalho_TipoCTe", dacte.TipoCte != null ? dacte.TipoCte : ""));
            lstParam.Add(new ReportParameter("Cabecalho_TipoServico", dacte.TipoServico != null ? dacte.TipoServico : ""));
            lstParam.Add(new ReportParameter("Cabecalho_TomadorServico", dacte.TipoTomador == "0" ? "Remetente" : (dacte.TipoTomador == "1" ? "Expedidor"
                : (dacte.TipoTomador == "2" ? "Recebedor" : "Destinatário"))));
            lstParam.Add(new ReportParameter("Cabecalho_FormaPagt", dacte.FormaPagamento.ToString()));
            lstParam.Add(new ReportParameter("Cabecalho_NatOperacao", dacte.NatOperacao != null ? dacte.NatOperacao : ""));
            lstParam.Add(new ReportParameter("Cabecalho_ChaveAcesso", dacte.ChaveAcesso != null ? dacte.ChaveAcesso : ""));
            lstParam.Add(new ReportParameter("Cabecalho_CodigoBarras", Utils.GetUrlSite(context) +
                "/Handlers/Dacte.ashx?idCte=" + idCte + "&isCodigoBarras=true"));
            lstParam.Add(new ReportParameter("Cabecalho_ProtAutorizacao", String.IsNullOrEmpty(dacte.ProtocoloAutorizacao) ? "." : dacte.ProtocoloAutorizacao));
            lstParam.Add(new ReportParameter("Cabecalho_OrigemPrestacao", String.IsNullOrEmpty(dacte.OrigemPrestacao) ? "." : dacte.OrigemPrestacao));
            lstParam.Add(new ReportParameter("Cabecalho_DestinoPrestacao", String.IsNullOrEmpty(dacte.DestinoPrestacao) ? "." : dacte.DestinoPrestacao));
            lstParam.Add(new ReportParameter("Cabecalho_CnpjIEEmit", dacte.CnpjCpfEmitente + " - " + dacte.InscEstEmitente));

            //Trecho responsável pelo preenchimento das propriedades da classe que popula
            //a tabela de componentes do valor da prestação do Cte replicando os dados entre as colunas
            //e posteriormente entre as linhas
            #region Tabela Componentes

            var listaComponentes = new List<LinhaComponenteValor>();

            if (dacte.ListaComponentes != null && dacte.ListaComponentes.Count > 0)
            {
                int count = 0;

                foreach (var tcv in dacte.ListaComponentes)
                {
                    if (count == 0)
                    {
                        listaComponentes.Add(new LinhaComponenteValor());
                        listaComponentes[listaComponentes.Count() - 1].Nome1 = tcv.NomeComponente.ToString().ToUpper();
                        listaComponentes[listaComponentes.Count() - 1].Valor1 = tcv.ValorComponente.ToString().ToUpper();
                        count++;
                        continue;
                    }
                    else if (count == 1)
                    {
                        listaComponentes[listaComponentes.Count() - 1].Nome2 = tcv.NomeComponente.ToString().ToUpper();
                        listaComponentes[listaComponentes.Count() - 1].Valor2 = tcv.ValorComponente.ToString().ToUpper();
                        count++;
                        continue;
                    }
                    else if (count == 2)
                    {
                        listaComponentes[listaComponentes.Count() - 1].Nome3 = tcv.NomeComponente.ToString().ToUpper();
                        listaComponentes[listaComponentes.Count() - 1].Valor3 = tcv.ValorComponente.ToString().ToUpper();
                        count++;
                        continue;
                    }
                    else if (count == 3)
                    {
                        listaComponentes[listaComponentes.Count() - 1].Nome4 = tcv.NomeComponente.ToString().ToUpper();
                        listaComponentes[listaComponentes.Count() - 1].Valor4 = tcv.ValorComponente.ToString().ToUpper();
                        count = 0;
                        continue;
                    }
                }
            }

            #endregion

            //Trecho responsável pelo preenchimento das propriedades da classe que popula
            //a tabela de informações da carga do cte replicando os dados entre as colunas
            //e posteriormente entre as linhas
            #region Tabela InfoCarga

            var listaInfoCarga = new List<LinhaInfCarga>();

            if (dacte.ListaInfoCargaCte.Count > 0)
            {
                int count = 0;

                foreach (var infCarga in dacte.ListaInfoCargaCte)
                {
                    if (count == 0)
                    {
                        listaInfoCarga.Add(new LinhaInfCarga());
                        listaInfoCarga[listaInfoCarga.Count() - 1].Quantidade1 = infCarga.Quantidade.ToString().ToUpper();
                        listaInfoCarga[listaInfoCarga.Count() - 1].TipoMedida1 = infCarga.TipoMedida.ToString().ToUpper();
                        listaInfoCarga[listaInfoCarga.Count() - 1].UnidadeMedida1 = ((InfoCargaCte.TipoUnidadeEnum) infCarga.TipoUnidade).Translate().Format().ToUpper();
                        count++;
                        continue;
                    }
                    else if (count == 1)
                    {
                        listaInfoCarga[listaInfoCarga.Count() - 1].Quantidade2 = infCarga.Quantidade.ToString().ToUpper();
                        listaInfoCarga[listaInfoCarga.Count() - 1].TipoMedida2 = infCarga.TipoMedida.ToString().ToUpper();
                        listaInfoCarga[listaInfoCarga.Count() - 1].UnidadeMedida2 = ((InfoCargaCte.TipoUnidadeEnum)infCarga.TipoUnidade).Translate().Format().ToUpper();
                        count++;
                        continue;
                    }
                    else if (count == 2)
                    {
                        listaInfoCarga[listaInfoCarga.Count() - 1].Quantidade3 = infCarga.Quantidade.ToString().ToUpper();
                        listaInfoCarga[listaInfoCarga.Count() - 1].TipoMedida3 = infCarga.TipoMedida.ToString().ToUpper();
                        listaInfoCarga[listaInfoCarga.Count() - 1].UnidadeMedida3 = ((InfoCargaCte.TipoUnidadeEnum)infCarga.TipoUnidade).Translate().Format().ToUpper();
                        count = 0;
                        continue;
                    }
                }
            }

            #endregion

            //Trecho responsável pelo preenchimento das propriedades da classe que popula
            //a tabela de documentos originários do cte replicando os dados entre as colunas
            //e posteriormente entre as linhas
            #region Tabela Documentos Originários

            var listaDocsOriginarios = new List<LinhaDocumentosOriginarios>();

            if (dacte.ListaDocumentosOriginarios != null && dacte.ListaDocumentosOriginarios.Count > 0)
            {
                int count = 0;

                foreach (var docsOriginarios in dacte.ListaDocumentosOriginarios)
                {
                    if (count == 0)
                    {
                        listaDocsOriginarios.Add(new LinhaDocumentosOriginarios());
                        listaDocsOriginarios[listaDocsOriginarios.Count() - 1].DocEmitenteNf1 = docsOriginarios.DocEmitenteNf != null ? docsOriginarios.DocEmitenteNf.ToString().ToUpper() : "";
                        listaDocsOriginarios[listaDocsOriginarios.Count() - 1].TipoDoc1 = docsOriginarios.TipoDoc != null ? docsOriginarios.TipoDoc.ToString().ToUpper() : "";
                        listaDocsOriginarios[listaDocsOriginarios.Count() - 1].Serie1 = docsOriginarios.Serie != null ? docsOriginarios.Serie.ToString().ToUpper() : "";
                        listaDocsOriginarios[listaDocsOriginarios.Count() - 1].NumeroDoc1 = docsOriginarios.NumeroDoc != null ? docsOriginarios.NumeroDoc.ToString().ToUpper() : "";
                        count++;
                        continue;
                    }
                    else if (count == 1)
                    {
                        listaDocsOriginarios[listaDocsOriginarios.Count() - 1].DocEmitenteNf2 = docsOriginarios.DocEmitenteNf != null ? docsOriginarios.DocEmitenteNf.ToString().ToUpper() : "";
                        listaDocsOriginarios[listaDocsOriginarios.Count() - 1].TipoDoc2 = docsOriginarios.TipoDoc != null ? docsOriginarios.TipoDoc.ToString().ToUpper() : "";
                        listaDocsOriginarios[listaDocsOriginarios.Count() - 1].Serie2 = docsOriginarios.Serie != null ? docsOriginarios.Serie.ToString().ToUpper() : "";
                        listaDocsOriginarios[listaDocsOriginarios.Count() - 1].NumeroDoc2 = docsOriginarios.NumeroDoc != null ? docsOriginarios.NumeroDoc.ToString().ToUpper() : "";
                        count = 0;
                        continue;
                    }
                }
            }

            #endregion

            //Trecho responsável pelo preenchimento das propriedades da classe que popula
            //a tabela de identificação dos lacres em trânsito replicando os dados entre as colunas
            //e posteriormente entre as linhas
            #region Tabela Lacres
            var listaLacres = new List<LinhaLacreCte>();

            if (dacte.ListaNumeroLacre != null && dacte.ListaNumeroLacre.Count > 0)
            {
                int count = 0;

                foreach (var lacres in dacte.ListaNumeroLacre)
                {
                    if (count == 0)
                    {
                        listaLacres.Add(new LinhaLacreCte());
                        listaLacres[listaLacres.Count() - 1].NumLacre1 = lacres.NumeroLacre.ToString().ToUpper();
                        count++;
                        continue;
                    }
                    else if (count == 1)
                    {
                        listaLacres[listaLacres.Count() - 1].NumLacre2 = lacres.NumeroLacre.ToString().ToUpper();
                        count++;
                        continue;
                    }
                    else if (count == 2)
                    {
                        listaLacres[listaLacres.Count() - 1].NumLacre3 = lacres.NumeroLacre.ToString().ToUpper();
                        count = 0;
                        continue;
                    }
                }
            }

            #endregion

            report.DataSources.Add(new ReportDataSource("LinhaComponenteValor", listaComponentes));
            report.DataSources.Add(new ReportDataSource("LinhaInfoCarga", listaInfoCarga));
            report.DataSources.Add(new ReportDataSource("LinhaDocumentosOriginarios", listaDocsOriginarios));
            report.DataSources.Add(new ReportDataSource("LinhaLacreCte", listaLacres));

            #region tomador


            #endregion

            //lstParam.Add(new ReportParameter("ProdutoPredominante", dacte.ProdutoPredominante));
            //lstParam.Add(new ReportParameter("OutrasCaractCarga", dacte.OutCarctCarga));
            //lstParam.Add(new ReportParameter("ValorTotalMercadoria", dacte.Valortotalme));


            
            //lstParam.Add(new ReportParameter("Logotipo", empresa != ControleSistema.ClienteSistema.CenterBox &&
            //    empresa != ControleSistema.ClienteSistema.Vipal ? Utils.GetReportLogoColor(context.Request, idLoja) :
            //    Utils.GetReportLogoColor(context.Request, idLoja).Replace("Color", "Nf")));

            #region LogoTipo

            var logotipo = Logotipo.GetReportLogoColor(context.Request, idLoja.GetValueOrDefault(0));

            report.EnableExternalImages = true;
            lstParam.Add(new ReportParameter("Logotipo", logotipo));

            #endregion

            // Atribui parâmetros ao relatório
            report.SetParameters(lstParam);

            return report.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
        }

        private static byte[] GetBytesCodigoBarra(uint idCte, bool dadosAdicionais)
        {
            string chaveAcesso = ConhecimentoTransporteDAO.Instance.ObtemChaveAcesso(idCte);
            CTe cte = CTeDAO.GetForDacte(chaveAcesso);
            return !dadosAdicionais ? cte.BarCodeImage : cte.BarCodeImageDadosAdicionais;
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
