using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using WebGlass.Business.ProducaoDiariaRealizada.Entidade;
using WebGlass.Business.ProducaoDiariaRealizada.Fluxo;
using System.Web.UI.HtmlControls;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class PainelComercialSetores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((PainelGraficos)this.Master).TempoSegundosAtualizar = 1200;
            MontaGraficoPainelComercial();

            /* Chamado 18480. */
            (this.Master as PainelGraficos).ConteudoPainel =
                "<b>M2:</b> Pedidos que não estão nas situações Ativo e Cancelado; " +
                    "Pedidos dos tipos Produção e Venda; Caso haja conferência gerada, " +
                    "considera pedidos em conferência nas situações Finalizado, Impresso Comum ou Impresso.<br />" +
                "<b>Capaciade:</b> É a capacidade de produção definida para o setor na tela PCP > Capacidade de Produção Diária.<br />" +
                "<b>Data:</b> Data de fábrica do pedido, se houver conferência gerada, senão, data de entrega.";
        }
    
        private void MontaGraficoPainelComercial()
        {
            var lstCharts = new List<Chart>();
            var idsSetores = SetorDAO.Instance.ObtemIdsSetoresPainelComercial();
    
            for (int i = 0; i < idsSetores.Count; i++)
            {
                var dataFabrica = DateTime.Today;
                int countData = 0;
                var lstResultado = new List<ProducaoDiariaRealizada>();
    
                while (countData < 10)
                {
                    if (!dataFabrica.DiaUtil())
                        dataFabrica = dataFabrica.AddDays(1);
                    else
                    {
                        var dataEntrega = dataFabrica;
    
                        for (int j = 0; j < PCPConfig.Etiqueta.DiasDataFabrica; j++)
                        {
                            dataEntrega = dataEntrega.AddDays(1);
    
                            while (!dataEntrega.DiaUtil())
                                dataEntrega = dataEntrega.AddDays(1);
                        }
    
                        lstResultado.AddRange(BuscarEValidar.Instance.ObtemDadosProducaoForPainelComercialSetores(idsSetores[i], dataFabrica, dataEntrega));
    
                        dataFabrica = dataFabrica.AddDays(1);
                        countData++;
                    }
                }
    
    
                #region Declarações
    
                //Grafico
                var chart = new Chart();
                chart.Width = 450;
    
                // Series do Gráfico.
                var serieCapacidade = new Series("Capacidade");
                var seriePrevisto = new Series("Previsto");
    
                #endregion
    
                #region Áreas e Eixos
    
                // Configurações do ChartArea.
                chart.ChartAreas.Add("Area");
    
                // Configurações do Eixo Y.
                chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                chart.ChartAreas[0].AxisX.IsMarginVisible = true;
    
                // Configurações do Eixo X.
                chart.ChartAreas[0].AxisY.IsMarginVisible = false;
                chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
    
                #endregion
    
                #region Título
    
                // Configurações do título do gráfico.
                chart.Titles.Add("ProducaoDiaria");
                chart.Titles[0].Font = new Font("Arial", 12, FontStyle.Bold);
                chart.Titles[0].Text = Data.Helper.Utils.GetSetores.Where(f => f.IdSetor == idsSetores[i]).FirstOrDefault().Descricao;
                chart.Titles[0].Docking = Docking.Top;
                chart.Titles[0].Alignment = ContentAlignment.MiddleCenter;
    
                #endregion
    
                #region Legenda
    
                // Legenda do gráfico.
                //chart.Legends.Add("ProducaoDiaria");
    
                #endregion
    
                #region Séries
    
                // Define o tipo de gráfico.
                serieCapacidade.ChartType = SeriesChartType.Bar;
                seriePrevisto.ChartType = SeriesChartType.Bar;
    
                // Define o tipo de valor do eixo X.
                serieCapacidade.XValueType = ChartValueType.String;
                seriePrevisto.XValueType = ChartValueType.String;
    
                // Define se o valor será mostrado em uma label.
                serieCapacidade.IsValueShownAsLabel = false;
                seriePrevisto.IsValueShownAsLabel = true;
    
                // Define a cor das séries.
                ColorConverter cc = new ColorConverter();
                serieCapacidade.Color = (Color)cc.ConvertFromString("#F08C41");
                seriePrevisto.Color = (Color)cc.ConvertFromString("#418CF0");
    
                // Insere as séries no gráfico.
                chart.Series.Add(serieCapacidade);
                chart.Series.Add(seriePrevisto);
    
                #endregion
    
                #region Dados
    
                lstResultado = lstResultado.OrderBy(f => f.DataProducao).ToList();
    
                // Desenha os dados no gráfico.
                chart.Series[0].Points.DataBindXY(lstResultado, "DataProducaoStr", lstResultado, "Capacidade");
                chart.Series[1].Points.DataBindXY(lstResultado, "DataProducaoStr", lstResultado, "M2Previsto");
    
                #endregion
    
                lstCharts.Add(chart);
            }
    
            var lstDivs = new List<HtmlGenericControl>() { new HtmlGenericControl("DIV") };
            var countDivAtual = 0;
    
            for (int i = 0; i < lstCharts.Count; i++)
            {
                if (i % 3 == 0)
                {
                    lstDivs.Add(new HtmlGenericControl("DIV"));
                    countDivAtual++;
                }
    
                lstDivs[countDivAtual].Controls.Add(lstCharts[i]);
            }
    
            foreach (var div in lstDivs)
                divGraficos.Controls.Add(div);
        }
    }
}
