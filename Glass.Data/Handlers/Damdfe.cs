using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.RelDAL;
using Glass.Data.RelModel;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Glass.Data.Handlers
{
    public class Damdfe : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            bool isCodigoBarras = context.Request["isCodigoBarras"] == "true";

            context.Response.ContentType = isCodigoBarras ? "image/jpeg" : "application/pdf";

            var idMDFe = Glass.Conversoes.StrParaInt(context.Request["IdMDFe"]);
            byte[] bytes = isCodigoBarras ? GetBytesCodigoBarra(idMDFe) :
                GetBytesRelatorio(context, idMDFe, context.Request["previsualizar"] == "true");

            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
        }

        public static byte[] GetBytesRelatorio(HttpContext context, int idMDFe, bool preVisualizar)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            return GetBytesRelatorio(context, idMDFe, preVisualizar, out warnings, out streamids, out mimeType, out encoding, out extension);
        }

        public static byte[] GetBytesRelatorio(HttpContext context, int idMDFe, bool preVisualizar, out Warning[] warnings, out string[] streamids,
            out string mimeType, out string encoding, out string extension)
        {
            var report = new LocalReport();
            var lstParam = new List<ReportParameter>();

            var mdfe = ManifestoEletronicoDAO.Instance.ObterManifestoEletronicoPeloId(idMDFe);
            var damdfe = MDFeDAO.ObterParaDAMDFE(context, mdfe.ChaveAcesso);

            var participanteEmitente = mdfe.Participantes.Where(f => f.TipoParticipante == Model.TipoParticipanteEnum.Emitente).FirstOrDefault();
            var idLojaEmitente = participanteEmitente.IdLoja.GetValueOrDefault(0);

            report.ReportPath = "Relatorios/MDFe/rptDamdfeRetrato.rdlc";
            report.ReportPath = context.Server.MapPath("~/" + report.ReportPath);

            report.DataSources.Add(new ReportDataSource("MDFe", new MDFe[] { damdfe }));

            // Parâmetros para o cabeçalho/rodapé do relatório
            lstParam.Add(new ReportParameter("CodigoBarras", Utils.GetUrlSite(context) +
                "/Handlers/Damdfe.ashx?IdMDFe=" + idMDFe + "&isCodigoBarras=true"));
            lstParam.Add(new ReportParameter("Producao", (damdfe.TipoAmbiente == (int)Glass.Data.MDFeUtils.ConfigMDFe.TipoAmbienteMDFe.Producao && 
                mdfe.Situacao != Model.SituacaoEnum.Cancelado && !preVisualizar).ToString().ToLower()));
            // Define se o MDF-e foi emitida em contingência
            lstParam.Add(new ReportParameter("Contingencia", (mdfe.TipoEmissao == Model.TipoEmissao.Contingencia).ToString().ToLower()));

            #region LogoTipo

            var logotipo = Logotipo.GetReportLogoColor(context.Request, (uint)idLojaEmitente);

            report.EnableExternalImages = true;
            lstParam.Add(new ReportParameter("Logotipo", logotipo));

            #endregion

            // Atribui parâmetros ao relatório
            report.SetParameters(lstParam);

            return report.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
        }

        private static byte[] GetBytesCodigoBarra(int idMDFe)
        {
            string chaveAcesso = ManifestoEletronicoDAO.Instance.ObterChaveAcesso(null, idMDFe);
            MDFe mdfe = MDFeDAO.ObterParaDAMDFE(chaveAcesso);
            return mdfe.BarCodeImage;
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
