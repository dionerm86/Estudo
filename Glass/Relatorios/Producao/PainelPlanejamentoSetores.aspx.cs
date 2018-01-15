using System;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class PainelPlanejamentoSetores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ((TextBox)ctrlData.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");

            // Monta os gráficos
            this.MontarGraficoProducaoRealizada();

            /* Chamado 18480. */
            (this.Master as PainelGraficos).ConteudoPainel =
                "<b>Previto:</b> Total de M2 calculado dos produtos que estão em produção e possuem roteiro associado.<br />" +
                "<b>Realizado:</b> Total de M2 calculado dos produtos que estão em produção, possuem roteiro associado " +
                    "e foram marcados como pronto até a data.<br />" +
                "<b>Pendente:</b> É a diferença entre o M2 Previsto e o M2 Realizado.<br />" +
                "<b>Data:</b> Data de fábrica do pedido.";
        }
    
        private void MontarGraficoProducaoRealizada()
        {
            chtPrevisaoProducao.Visible = Glass.Configuracoes.ProducaoConfig.CapacidadeProducaoPorSetor;
            var dataConsulta = DateTime.Parse(((TextBox)ctrlData.FindControl("txtData")).Text);

            if (!chtPrevisaoProducao.Visible)
                return;
    
            // Obtém os dados da produção pendente.
            var lstResultado = WebGlass.Business.ProducaoDiariaRealizada.Fluxo.BuscarEValidar.Instance.ObtemDadosProducaoForPainelPlanejamentoSetores(dataConsulta);
    
            // Se nenhum dado for buscado o gráfico não é gerado.
            if (lstResultado != null && lstResultado.Count > 0 && chtPrevisaoProducao != null)
            {
                #region Declarações
    
                // Series do Gráfico.
                var serieCapacidade = new Series("Capacidade");
                var seriePrevisto = new Series("Previsto");
                var serieProducao = new Series("Realizado");
    
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
                serieCapacidade.ChartType = SeriesChartType.Column;
                seriePrevisto.ChartType = SeriesChartType.Column;
                serieProducao.ChartType = SeriesChartType.Column;
    
                // Define o tipo de valor do eixo X.
                serieCapacidade.XValueType = ChartValueType.String;
                seriePrevisto.XValueType = ChartValueType.String;
                serieProducao.XValueType = ChartValueType.String;
    
                // Define se o valor será mostrado em uma label.
                serieCapacidade.IsValueShownAsLabel = true;
                seriePrevisto.IsValueShownAsLabel = true;
                serieProducao.IsValueShownAsLabel = true;
    
                // Define a cor das séries.
                ColorConverter cc = new ColorConverter();
                serieCapacidade.Color = (Color)cc.ConvertFromString("#F08C41");
                seriePrevisto.Color = (Color)cc.ConvertFromString("#418CF0");
                serieProducao.Color = (Color)cc.ConvertFromString("#01F08C");
    
                // Insere as séries no gráfico.
                this.chtPrevisaoProducao.Series.Add(serieCapacidade);
                this.chtPrevisaoProducao.Series.Add(seriePrevisto);
                this.chtPrevisaoProducao.Series.Add(serieProducao);

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
                this.chtPrevisaoProducao.Series[0].Points.DataBindXY(lstResultado, "NomeSetor", lstResultado, "Capacidade");
                this.chtPrevisaoProducao.Series[1].Points.DataBindXY(lstResultado, "NomeSetor", lstResultado, "M2Previsto");
                this.chtPrevisaoProducao.Series[2].Points.DataBindXY(lstResultado, "NomeSetor", lstResultado, "M2Realizado");
    
                #endregion
            }
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e) { }
    }
}
