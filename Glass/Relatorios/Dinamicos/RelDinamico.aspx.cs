using Glass.Configuracoes;
using Glass.Data.Helper;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Colosoft;

namespace Glass.UI.Web.Relatorios.Dinamicos
{
    public partial class RelDinamico : Glass.Relatorios.UI.Web.ReportPage
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (DadosJavaScript.BackgroundLoading)
                return;

            ProcessaReport(pchTabela);
        }

        protected override Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam,
            HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login, string diretorioLogotipos)
        {
            var relatorioDinamicoFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IRelatorioDinamicoFluxo>();
            
            // Recupera dados do relatório dinâmico
            var relatorioDinamico = relatorioDinamicoFluxo.ObterRelatorioDinamico(int.Parse(PageRequest["id"]));
            
            // Define o caminho do relatório
            report.ReportPath = string.Format("Upload/RelatorioDinamico/{0}.rdlc", relatorioDinamico.IdRelatorioDinamico);

            // Monta os filtros da consulta
            var filtros = new List<Tuple<Glass.Global.Negocios.Entidades.RelatorioDinamicoFiltro, string>>();
            foreach (var key in PageRequest.QueryString.AllKeys)
            {
                if (key == null || key.IndexOf('_') <= 0)
                    continue;

                // Pega o id do RelatorioDinamicoFiltro e o recupera em seguida
                var idRelatorioDinamicoFiltro = key.Split('_')[1];
                var filtro = relatorioDinamico.Filtros.Where(f => f.IdRelatorioDinamicoFiltro == idRelatorioDinamicoFiltro.StrParaInt()).FirstOrDefault();

                filtros.Add(new Tuple<Glass.Global.Negocios.Entidades.RelatorioDinamicoFiltro, string>(filtro, PageRequest[key]));
            }

            int count;

            // Recupera os dados a serem preenchidos no relatório
            var lista = relatorioDinamicoFluxo.PesquisarListaDinamica(relatorioDinamico.IdRelatorioDinamico, filtros, 0, int.MaxValue, out count);

            System.Data.DataTable dt = new System.Data.DataTable();

            // Cria as colunas da grid
            foreach (var campo in lista.First().Keys)
                dt.Columns.Add(campo.RemoverAcentosEspacos(), typeof(string));

            foreach (var item in lista)
            {
                var dr = dt.NewRow();

                foreach (var campo in item)
                    dr[campo.Key] = campo.Value;

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();

            report.DataSources.Add(new ReportDataSource("ListaRegistros", dt));

            // Recupera parâmetro de agrupamento
            var agrupamento = filtros.Where(f => f.Item1.TipoControle == Data.Model.TipoControle.Agrupamento).FirstOrDefault();

            var criterio = string.Join("    ", filtros.OrderBy(f => f.Item1.NumSeq)
                .Where(f => f.Item2 != "" && f.Item2 != "|")
                .Select(f => f.Item1.NomeFiltro + ": " + f.Item2.Replace("|", " "))
            );

            // Define parâmetro padrões
            lstParam.Add(new ReportParameter("AgruparPor", agrupamento != null ? agrupamento.Item2 : "."));
            lstParam.Add(new ReportParameter("Criterio", criterio));
            lstParam.Add(new ReportParameter("NomeRelatorio", relatorioDinamico.NomeRelatorio));
            lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogo(PageRequest)));
            lstParam.Add(new ReportParameter("TextoRodape", Geral.TextoRodapeRelatorio(login.Nome, true)));
            lstParam.Add(new ReportParameter("CorRodape", "DimGray"));

            return null;
        }
    }
}