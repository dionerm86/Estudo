using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class ProducaoDiaria : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Glass.Configuracoes.ProducaoConfig.CapacidadeProducaoPorSetor)
                Response.Redirect("~/WebGlass/Main.aspx", true);
    
            if (!IsPostBack)
                ctrlData.Data = DateTime.Today;
    
            // Obt�m os dados da produ��o pendente.
            var lstResultado = WebGlass.Business.ProducaoDiariaRealizada.Fluxo.BuscarEValidar.Instance.ObtemDadosProducaoForPainelPlanejamentoSetores(ctrlData.Data);
    
            MontarTabelaProducaoRealizada(lstResultado);
            MontarGraficoProducaoRealizada(lstResultado);
        }
    
        private void MontarTabelaProducaoRealizada(IEnumerable<WebGlass.Business.ProducaoDiariaRealizada.Entidade.ProducaoDiariaRealizada> dados)
        {
            grdProducaoDiaria.DataSource = dados;
            
            if (grdProducaoDiaria.Columns.Count == 0)
            {
                grdProducaoDiaria.Columns.Add(new BoundField()
                {
                    HeaderText = "Setor",
                    DataField = "NomeSetor"
                });
    
                grdProducaoDiaria.Columns.Add(new BoundField()
                {
                    HeaderText = "Previsto",
                    DataField = "M2Previsto",
                    DataFormatString = "{0} m�"
                });
    
                grdProducaoDiaria.Columns.Add(new BoundField()
                {
                    HeaderText = "Realizado",
                    DataField = "M2Realizado",
                    DataFormatString = "{0} m�"
                });
            }
    
            grdProducaoDiaria.DataBind();
        }
    
        private void MontarGraficoProducaoRealizada(IEnumerable<WebGlass.Business.ProducaoDiariaRealizada.Entidade.ProducaoDiariaRealizada> dados)
        {
            // Se nenhum dado for buscado o gr�fico n�o � gerado.
            if (dados != null && dados.Count() > 0 && chtPrevisaoProducao != null)
            {
                #region Declara��es
    
                // Series do Gr�fico.
                var seriePrevisto = new Series("Previsto");
                var serieProducao = new Series("Realizado");
    
                #endregion
    
                #region �reas e Eixos
    
                // Configura��es do ChartArea.
                this.chtPrevisaoProducao.ChartAreas.Add("Area");
    
                // Configura��es do Eixo Y.
                this.chtPrevisaoProducao.ChartAreas[0].AxisX.IsMarginVisible = false;
                this.chtPrevisaoProducao.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
    
                // Configura��es do Eixo X.
                this.chtPrevisaoProducao.ChartAreas[0].AxisY.IsMarginVisible = false;
                this.chtPrevisaoProducao.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;
                this.chtPrevisaoProducao.ChartAreas[0].AxisY.IntervalOffset = 99999;
                this.chtPrevisaoProducao.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
    
                #endregion
    
                #region Legenda
    
                // Legenda do gr�fico.
                this.chtPrevisaoProducao.Legends.Add("ProducaoDiaria");
    
                #endregion
    
                #region S�ries
    
                // Define o tipo de gr�fico.
                seriePrevisto.ChartType = SeriesChartType.Column;
                serieProducao.ChartType = SeriesChartType.Column;
    
                // Define o tipo de valor do eixo X.
                seriePrevisto.XValueType = ChartValueType.String;
                serieProducao.XValueType = ChartValueType.String;
    
                // Define se o valor ser� mostrado em uma label.
                seriePrevisto.IsValueShownAsLabel = true;
                serieProducao.IsValueShownAsLabel = true;
    
                // Define se a label poder� ser exibida fora da �rea da s�rie.
                seriePrevisto.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Yes;
                serieProducao.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Yes;
    
                // Define a cor das s�ries.
                ColorConverter cc = new ColorConverter();
                seriePrevisto.Color = (Color)cc.ConvertFromString("#418CF0");
                serieProducao.Color = (Color)cc.ConvertFromString("#01F08C");
    
                // Insere as s�ries no gr�fico.
                this.chtPrevisaoProducao.Series.Add(seriePrevisto);
                this.chtPrevisaoProducao.Series.Add(serieProducao);
    
                #endregion
    
                #region Dados
    
                // Desenha os dados no gr�fico.
                this.chtPrevisaoProducao.Series[0].Points.DataBindXY(dados, "NomeSetor", dados, "M2Previsto");
                this.chtPrevisaoProducao.Series[1].Points.DataBindXY(dados, "NomeSetor", dados, "M2Realizado");
    
                #endregion
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    }
}
