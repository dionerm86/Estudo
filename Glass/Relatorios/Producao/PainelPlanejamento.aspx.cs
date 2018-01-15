using System;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;
using WebGlass.Business.ProducaoDiariaRealizada.Fluxo;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class PainelPlanejamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Monta os gráficos
            MontaGraficoPainelComercial();

            /* Chamado 18480. */
            (this.Master as PainelGraficos).ConteudoPainel =
                "<b>Previsto:</b> Total de M2 calculado dos produtos que estão em produção.<br />" +
                "<b>Realizado:</b> Total de M2 calculado dos produtos que estão em produção e foram marcados como pronto até a data.<br />" +
                "<b>Pendente:</b> É a diferença entre o M2 Previsto e o M2 Realizado.<br />" +
                "<b>Data:</b> Data de fábrica do pedido.";
        }

        private void MontaGraficoPainelComercial()
        {
            var dados = BuscarEValidar.Instance.ObtemDadosProducaoForPainelPlanejamento();

            #region Declarações

            // Series do Gráfico.
            var seriePrevisto = new Series("Previsto");
            var serieRealizado = new Series("Realizado");
            var seriePendente = new Series("Pendente");

            #endregion

            #region Áreas e Eixos

            // Configurações do ChartArea.
            this.chtPrevisaoProducao.ChartAreas.Add("Area");

            // Configurações do Eixo Y.
            this.chtPrevisaoProducao.ChartAreas[0].AxisY.IsMarginVisible = false;
            this.chtPrevisaoProducao.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
            this.chtPrevisaoProducao.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
            this.chtPrevisaoProducao.ChartAreas[0].AxisY.IntervalOffset = 99999;

            // Configurações do Eixo X.
            this.chtPrevisaoProducao.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
            this.chtPrevisaoProducao.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
            this.chtPrevisaoProducao.ChartAreas[0].AxisX.IsMarginVisible = false;
            

            #endregion

            #region Séries

            // Define o tipo de gráfico.
            seriePrevisto.ChartType = SeriesChartType.Column;
            serieRealizado.ChartType = SeriesChartType.Column;
            seriePendente.ChartType = SeriesChartType.Column;

            // Define o tipo de valor do eixo X.
            seriePrevisto.XValueType = ChartValueType.String;
            serieRealizado.XValueType = ChartValueType.String;
            seriePendente.XValueType = ChartValueType.String;

            // Define se o valor será mostrado em uma label.
            seriePrevisto.IsValueShownAsLabel = true;
            serieRealizado.IsValueShownAsLabel = true;
            seriePendente.IsValueShownAsLabel = true;

            // Define a cor das séries.
            ColorConverter cc = new ColorConverter();
            seriePrevisto.Color = (Color)cc.ConvertFromString("#418CF0");
            serieRealizado.Color = (Color)cc.ConvertFromString("#01F08C");
            seriePendente.Color = (Color)cc.ConvertFromString("#FF3030");

            // Insere as séries no gráfico.
            this.chtPrevisaoProducao.Series.Add(seriePrevisto);
            this.chtPrevisaoProducao.Series.Add(serieRealizado);
            this.chtPrevisaoProducao.Series.Add(seriePendente);

            this.chtPrevisaoProducao.Series[0].BorderWidth = 3;
            this.chtPrevisaoProducao.Series[0].BorderDashStyle = ChartDashStyle.Solid;
            this.chtPrevisaoProducao.Series[0].BorderColor = Color.White;

            this.chtPrevisaoProducao.Series[1].BorderWidth = 3;
            this.chtPrevisaoProducao.Series[1].BorderDashStyle = ChartDashStyle.Solid;
            this.chtPrevisaoProducao.Series[1].BorderColor = Color.White;

            this.chtPrevisaoProducao.Series[2].BorderWidth = 3;
            this.chtPrevisaoProducao.Series[2].BorderDashStyle = ChartDashStyle.Solid;
            this.chtPrevisaoProducao.Series[2].BorderColor = Color.White;

            #endregion

            #region Dados

            // Desenha os dados no gráfico.
            this.chtPrevisaoProducao.Series[0].Points.DataBindXY(dados, "DataFabricaStr", dados, "TotMPrevisto");
            this.chtPrevisaoProducao.Series[1].Points.DataBindXY(dados, "DataFabricaStr", dados, "TotMRealizado");
            this.chtPrevisaoProducao.Series[2].Points.DataBindXY(dados, "DataFabricaStr", dados, "TotMPendente");

            #endregion
        }
    }
}